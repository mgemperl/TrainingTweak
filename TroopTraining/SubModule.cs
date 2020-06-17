using System;
﻿using HarmonyLib;
using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TrainingTweak.CampaignBehaviors;
using System.IO;
using TaleWorlds.Localization;
using MCM.Abstractions.FluentBuilder;

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
                Util.Warning("Training Tweak mod failed to load module_string file." +
                    $"\n\n{exc.Message}");
            }

        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            try
            {
                Settings.Instance.BuildSettings();
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
            Settings.Instance.BuildSettings();
        }


            

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);

           

            if (_harmony != null
                && Campaign.Current?.Models?.SettlementTaxModel != null
                && Campaign.Current?.Models?.PartyWageModel != null)
            {
                try
                {
                    SettlementTaxModel taxModel = Campaign.Current.Models.SettlementTaxModel;
                    PartyWageModel wageModel = Campaign.Current.Models.PartyWageModel;
                    MethodInfo originalMethod;
                    MethodInfo postfix;

                    // Patch town taxes
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
                    Util.Warning(Strings.FinancialSolutionsFailed, exc);
                    _harmony.UnpatchAll(HarmonyId);
                }
            }
        }

        private void PatchMethod(MethodInfo original, MethodInfo postfix)
        {
            if (original != null && _harmony != null)
            {
                _harmony.Patch(
                    original: original,
                    postfix: new HarmonyMethod(postfix));
            }
        }
        
        public override void OnGameEnd(Game game)
        {
            base.OnGameEnd(game);

            if (_harmony != null)
            {
                _harmony.UnpatchAll(HarmonyId);
            }
        }

        protected override void OnSubModuleUnloaded()
        {
            Settings.Instance.Dispose();
        }
    }
}
