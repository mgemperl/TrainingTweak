using HarmonyLib;
using ModLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TrainingTweak.CampaignBehaviors;

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

                if (Settings.Instance.EnableFinancialSolutions)
                {
                    _harmony = new Harmony("mod.bannerlord.mareus.trainingtweak");
                }
                
            }
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);

            if (Settings.Instance.EnableFinancialSolutions
                && Campaign.Current?.Models?.SettlementTaxModel != null)
            {
                try
                {
                    SettlementTaxModel taxModel = Campaign.Current.Models.SettlementTaxModel;

                    // Patch town taxes
                    var originalMethod = taxModel.GetType().GetMethod("CalculateTownTax");
                    var postfix = typeof(Patches).GetMethod("CalculateTownTaxPostfix");
                    if (originalMethod != null)
                    {
                        var info = _harmony.Patch(
                            original: originalMethod,
                            postfix: new HarmonyMethod(postfix));
                    }

                    // Patch village taxes
                    originalMethod = taxModel.GetType()
                        .GetMethod("CalculateVillageTaxFromIncome");
                    postfix = typeof(Patches).GetMethod("CalculateVillageTaxPostfix");
                    if (originalMethod != null)
                    {
                        var info = _harmony.Patch(
                            original: originalMethod,
                            postfix: new HarmonyMethod(postfix));
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

        public override void OnGameEnd(Game game)
        {
            base.OnGameEnd(game);

            if (_harmony != null
                && Settings.Instance.EnableFinancialSolutions)
            {
                _harmony.UnpatchAll(HarmonyId);
            }
        }
    }
}
