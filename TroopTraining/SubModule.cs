using HarmonyLib;
using ModLib;
using System;
using System.Windows.Forms;
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
                MessageBox.Show("Training Tweak mod failed to initialize config file. " +
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

                _harmony = new Harmony("mod.bannerlord.mareus.trainingtweak");
                _harmony.PatchAll();
            }
        }

        public override void OnGameEnd(Game game)
        {
            base.OnGameEnd(game);
            _harmony.UnpatchAll(HarmonyId);
        }
    }
}
