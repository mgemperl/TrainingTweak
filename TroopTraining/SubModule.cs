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
    // TODO: Support when hero has both training perks.
    // TODO: Add optional leadership skill training xp
    // TODO: Add optional base-line xp gain (perhaps 5xp up to tier one if hero has neither perk)
    // TODO: Add separate multipliers for player clan parties, and non-player-owned AI parties
    // TODO: Change to use campaign behaviors. It's better design-wise.
    public class SubModule : MBSubModuleBase
    {
        public static readonly string ModuleFolderName = "TrainingTweak";
        public static readonly string ModName = "Training Tweak";

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            // Attempt to load settings xml file
            try
            {
                Settings.Instance = SettingsLoader.LoadSettings(
                    $"{BasePath.Name}/Modules/TrainingTweak/config.xml");
            }
            catch (Exception exc)
            {
                MessageBox.Show("Training Tweak mod failed to load config file. " +
                    $"Using default values.\n\n{exc.Message}");
            }

            // Attempt to hook into ModLib mod configuration menu
            try
            {
                SettingsDatabase.RegisterSettings(Settings.Instance);
            }
            catch (Exception exc)
            {
                MessageBox.Show($"{exc.Message}\n{exc.StackTrace}");
            }
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);

            MessageBox.Show("Finished initialization");
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
    }
}
