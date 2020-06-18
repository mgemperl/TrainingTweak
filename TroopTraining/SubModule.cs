using System;
﻿using HarmonyLib;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TrainingTweak.CampaignBehaviors;
using System.IO;

namespace TrainingTweak
{
    public class SubModule : MBSubModuleBase
    {
        public static readonly string ModuleFolderName = "TrainingTweak";
        public static readonly string ModName = "Training Tweak";
        public const string HarmonyId = "mod.bannerlord.mareus.trainingtweak";

        private Harmony _harmony;

        // TODO: Update module_strings.xml 
        // TODO: Update training behavior to use mod-internal perk and training field xp amounts
        // TODO: Implement harmony patch to set perk and training field xp amounts to 0

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            _harmony = new Harmony(HarmonyId);

            // Try loading strings
            try
            {
                var textMan = TaleWorlds.MountAndBlade.Module.CurrentModule.GlobalTextManager;
                textMan.LoadGameTexts(Path.Combine(BasePath.Name, "Modules", "TrainingTweak",
                    "ModuleData", "module_strings.xml"));
            }
            catch (Exception exc)
            {
                Util.Warning("Training Tweak mod failed to load " +
                    "module_strings.xml language file. Using English." +
                    $"\n\n{exc.Message}");
            }
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            // Register settings with MCM
            try
            {
                Settings.Instance.RegisterSettings();
            }
            catch (Exception exc)
            {
                Util.Warning(Strings.SettingsRegistrationFailed, exc);
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            // If playing in the campaign game mode
            if (gameStarterObject is CampaignGameStarter)
            {
                base.OnGameStart(game, gameStarterObject);

                var gameStarter = (CampaignGameStarter)gameStarterObject;
                gameStarter.AddBehavior(new PartyTrainingBehavior());
            }
        }

        public override void OnGameLoaded(Game game, object initializerObject)
        {
            base.OnGameLoaded(game, initializerObject);
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);

            if (_harmony != null
                && Campaign.Current?.Models?.SettlementTaxModel != null
                && Campaign.Current?.Models?.PartyWageModel != null
                && Campaign.Current?.Models?.PartyTrainingModel != null
                && Campaign.Current?.Models?.DailyTroopXpBonusModel != null)
            {
                MethodInfo originalMethod;
                MethodInfo postfix;

                try
                {
                    // Patch town taxes
                    SettlementTaxModel taxModel = Campaign.Current.Models.SettlementTaxModel;
                    originalMethod = taxModel.GetType().GetMethod(nameof(taxModel.CalculateTownTax))
                        ?.DeclaringType?.GetMethod(nameof(taxModel.CalculateTownTax));
                    postfix = typeof(HarmonyPatches.Patches)
                        .GetMethod(nameof(HarmonyPatches.Patches.CalculateTownTaxPostfix));
                    PatchMethod(originalMethod, postfix);

                    // Patch village taxes
                    originalMethod = taxModel.GetType()
                        .GetMethod(nameof(taxModel.CalculateVillageTaxFromIncome))
                        ?.DeclaringType?.GetMethod(nameof(taxModel.CalculateVillageTaxFromIncome));
                    postfix = typeof(HarmonyPatches.Patches)
                        .GetMethod(nameof(HarmonyPatches.Patches.CalculateVillageTaxPostfix));
                    PatchMethod(originalMethod, postfix);


                    // Patch party wages
                    PartyWageModel wageModel = Campaign.Current.Models.PartyWageModel;
                    originalMethod = wageModel.GetType().GetMethod(nameof(wageModel.GetTotalWage))
                        ?.DeclaringType?.GetMethod(nameof(wageModel.GetTotalWage));
                    postfix = typeof(HarmonyPatches.Patches)
                        .GetMethod(nameof(HarmonyPatches.Patches.PartyWagePostfix));
                    PatchMethod(originalMethod, postfix);

                    // Patch troop upgrade costs
                    originalMethod = wageModel.GetType().GetMethod(nameof(wageModel.GetGoldCostForUpgrade))
                        ?.DeclaringType?.GetMethod(nameof(wageModel.GetGoldCostForUpgrade));
                    postfix = typeof(HarmonyPatches.Patches)
                        .GetMethod(nameof(HarmonyPatches.Patches.UpgradeCostPostfix));
                    PatchMethod(originalMethod, postfix);

                }
                catch (Exception exc)
                {
                    Util.Warning(Strings.FinancialSolutionsPatchFailed, exc);
                    _harmony.UnpatchAll();
                }

                try
                {
                    // Patch perk xp gain
                    PartyTrainingModel partyTrainingModel = Campaign.Current.Models.PartyTrainingModel;
                    originalMethod = partyTrainingModel.GetType()
                        .GetMethod(nameof(partyTrainingModel.GetPerkExperiencesForTroops))
                            ?.DeclaringType?.GetMethod(
                                nameof(partyTrainingModel.GetPerkExperiencesForTroops));
                    postfix = typeof(HarmonyPatches.Patches)
                        .GetMethod(nameof(HarmonyPatches.Patches.GetPerkExperiencesForTroopsPostfix));
                    PatchMethod(originalMethod, postfix);

                    // Patch garrison xp gain
                    DailyTroopXpBonusModel garrisonTrainingModel =
                        Campaign.Current.Models.DailyTroopXpBonusModel;
                    originalMethod = garrisonTrainingModel.GetType()
                        .GetMethod(nameof(garrisonTrainingModel.CalculateDailyTroopXpBonus))
                            ?.DeclaringType?.GetMethod(
                                nameof(garrisonTrainingModel.CalculateDailyTroopXpBonus));
                    postfix = typeof(HarmonyPatches.Patches)
                        .GetMethod(nameof(HarmonyPatches.Patches.CalculateDailyTroopXpBonusPostfix));
                    PatchMethod(originalMethod, postfix);
                }
                catch (Exception exc)
                {
                    Util.Warning(Strings.DisableNativeTrainingPatchFailed, exc);
                }
                
            }
        }

        private void PatchMethod(MethodInfo original, MethodInfo postfix)
        {
            if (original != null)
            {
                _harmony.Patch(
                    original: original,
                    postfix: new HarmonyMethod(postfix));
            }
            else
            {
                throw new InvalidOperationException(
                    $"Original method not found for patch: {postfix.Name}");
            }
        }
        
        public override void OnGameEnd(Game game)
        {
            base.OnGameEnd(game);

            if (_harmony != null)
            {
                //_harmony.UnpatchAll(HarmonyId);
            }
        }

        protected override void OnSubModuleUnloaded()
        {
            Settings.Instance.Dispose();
        }
    }
}
