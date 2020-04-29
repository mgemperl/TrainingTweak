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
    // TODO: Add tier limit for all training types (incl. garrisons)
    public class SubModule : MBSubModuleBase
    {
        public static readonly string ModuleFolderName = "TrainingTweak";
        public static readonly string ModName = "Training Tweak";
        public const string HarmonyId = "mod.bannerlord.mareus.trainingtweak";

        private Harmony _harmony;

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
                Util.Warning("Training Tweak mod failed to load string file." +
                    "\n\n{exc.Message}");
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            // If playing in the campaign game mode
            if (gameStarterObject is CampaignGameStarter)
            {
                base.OnGameStart(game, gameStarterObject);

                // Try loading config file
                try
                {
                    Settings.Instance = SettingsLoader.LoadSettings(
                        Path.Combine(BasePath.Name, "Modules", "TrainingTweak",
                        "ModuleData", "config.xml"));
                }
                catch (Exception exc)
                {
                    Util.Warning("Training Tweak mod failed to load config file. " +
                        $"Using default values.\n\n{exc.Message}");
                }

                _harmony = new Harmony(HarmonyId);
                var gameStarter = (CampaignGameStarter)gameStarterObject;
                gameStarter.AddBehavior(new PartyTrainingBehavior());
            }
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
                    originalMethod = taxModel.GetType().GetMethod("CalculateTownTax")
                        ?.DeclaringType?.GetMethod("CalculateTownTax");
                    postfix = typeof(HarmonyPatches.Patches)
                        .GetMethod("CalculateTownTaxPostfix");
                    PatchMethod(originalMethod, postfix);

                    // Patch village taxes
                    originalMethod = taxModel.GetType().GetMethod("CalculateVillageTaxFromIncome")
                        ?.DeclaringType?.GetMethod("CalculateVillageTaxFromIncome");
                    postfix = typeof(HarmonyPatches.Patches)
                        .GetMethod("CalculateVillageTaxPostfix");
                    PatchMethod(originalMethod, postfix);

                    // Patch party wages
                    originalMethod = wageModel.GetType().GetMethod("GetTotalWage")
                        ?.DeclaringType?.GetMethod("GetTotalWage");
                    postfix = typeof(HarmonyPatches.Patches)
                        .GetMethod("PartyWagePostfix");
                    PatchMethod(originalMethod, postfix);

                    // Patch troop upgrade costs
                    originalMethod = wageModel.GetType().GetMethod("GetGoldCostForUpgrade")
                        ?.DeclaringType?.GetMethod("GetGoldCostForUpgrade");
                    postfix = typeof(HarmonyPatches.Patches)
                        .GetMethod("UpgradeCostPostfix");
                    PatchMethod(originalMethod, postfix);
                }
                catch (Exception exc)
                {
                    Util.Warning("Training Tweak mod failed to apply " +
                        "Financial Solutions patches. Continuing without them.", exc);
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
    }
}
