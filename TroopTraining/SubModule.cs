using HarmonyLib;
using ModLib;
using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TrainingTweak.CampaignBehaviors;
using System.IO;
using ModLib.GUI.ViewModels;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using psai.Editor;

namespace TrainingTweak
{
    // TODO: Add optional leadership skill training xp
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
            PatchModlibLocalization();

            // Try loading localization
            try
            {
                string path = Path.Combine(BasePath.Name, "Modules",
                    "TrainingTweak", "ModuleData", "Localization");
                Strings.LoadStrings(path, TaleWorlds.MountAndBlade.Module
                    .CurrentModule.GlobalTextManager);

            }
            catch (Exception exc)
            {
                Util.Warning("Training Tweak mod failed to load language file. " +
                    $"Using en-US.\n\n{exc.Message}");
            }

            bool settingsInitialized = false;

            // Try initializing and loading settings file
            try
            {
                FileDatabase.Initialise(ModuleFolderName);
                Settings loaded = FileDatabase.Get<Settings>(Settings.Instance.ID);
                settingsInitialized = true;

                if (loaded != null)
                {
                    Settings.Instance = loaded;
                }
            }
            catch (Exception exc)
            {
                Util.Warning("Training Tweak mod failed to initialize config file. " +
                    $"Using default values. Not integrating with mod configuration menu." +
                    $"\n\n{exc.Message}");
            }

            if (settingsInitialized)
            {
                // Hook into ModLib mod configuration menu
                SettingsDatabase.RegisterSettings(Settings.Instance);
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

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);

            // If configured to apply financial solutions harmony patches
            if (_harmony != null
                && Settings.Instance.EnableFinancialSolutions
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
                    if (Settings.Instance.PlayerTownTaxIncomeMultiplier != 1.0f
                        || Settings.Instance.NonPlayerTownTaxIncomeMultiplier != 1.0f)
                    {
                        originalMethod = taxModel.GetType().GetMethod("CalculateTownTax")
                            ?.DeclaringType?.GetMethod("CalculateTownTax");
                        postfix = typeof(Patches).GetMethod("CalculateTownTaxPostfix");
                        PatchMethod(originalMethod, postfix);
                    }

                    // Patch village taxes
                    if (Settings.Instance.PlayerVillageTaxIncomeMultiplier != 1.0f
                        || Settings.Instance.NonPlayerVillageTaxIncomeMultiplier != 1.0f)
                    {
                        originalMethod = taxModel.GetType().GetMethod("CalculateVillageTaxFromIncome")
                            ?.DeclaringType?.GetMethod("CalculateVillageTaxFromIncome");
                        postfix = typeof(Patches).GetMethod("CalculateVillageTaxPostfix");
                        PatchMethod(originalMethod, postfix);
                    }

                    // Patch party wages
                    if (Settings.Instance.PlayerClanPartyWageMultiplier != 1.0f
                        || Settings.Instance.NonPlayerClanPartyWageMultiplier != 1.0f)
                    {
                        originalMethod = wageModel.GetType().GetMethod("GetTotalWage")
                            ?.DeclaringType?.GetMethod("GetTotalWage");
                        postfix = typeof(Patches).GetMethod("PartyWagePostfix");
                        PatchMethod(originalMethod, postfix);
                    }

                    // Patch troop upgrade costs
                    if (Settings.Instance.TroopUpgradeCostMultiplier != 1.0f)
                    {
                        originalMethod = wageModel.GetType().GetMethod("GetGoldCostForUpgrade")
                            ?.DeclaringType?.GetMethod("GetGoldCostForUpgrade");
                        postfix = typeof(Patches).GetMethod("UpgradeCostPostfix");
                        PatchMethod(originalMethod, postfix);
                    }
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
        
        private void PatchModlibLocalization()
        {
            var originalMethod = typeof(SettingsBase).GetMethod(
                "GetSettingPropertyGroups", BindingFlags.NonPublic | BindingFlags.Instance);
            var postfix = this.GetType().GetMethod("ModLibPatch");
            PatchMethod(originalMethod, postfix);
        }

        public static void ModLibPatch(SettingsBase __instance, 
            ref List<SettingPropertyGroup> __result)
        {
            // If this is my mod
            if (__instance.ModName == SubModule.ModName)
            {
                foreach (var group in __result)
                {
                    var attr = group.GroupToggleSettingProperty?.SettingAttribute;
                    group.GroupToggleSettingProperty?.SettingAttribute.GetType().GetProperty("HintText")
                        .SetValue(attr, "TEST_GROUP_HINT");
                    group.GroupToggleSettingProperty?.SettingAttribute.GetType().GetProperty("DisplayName")
                        .SetValue(attr, "TEST__GROUP_DISPLAY_NAME");
                    group.Attribute.GetType().GetProperty("GroupName")
                        ?.SetValue(group.Attribute, "TEST_GROUP_NAME");

                    foreach (var prop in group.SettingProperties)
                    {
                        prop.SettingAttribute.GetType().GetProperty("HintText")
                            .SetValue(prop.SettingAttribute, "TEST_HINT");
                        prop.SettingAttribute.GetType().GetProperty("DisplayName")
                            .SetValue(prop.SettingAttribute, "TEST_DISPLAY_NAME");
                    }
                }
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
