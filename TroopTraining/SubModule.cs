using System;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace TrainingTweak
{
    // TODO: Support when hero has both training perks.
    // TODO: Add optional leadership skill training xp
    // TODO: Add optional base-line xp gain (perhaps 5xp up to tier one if hero has neither perk)
    // TODO: Add separate multipliers for player clan parties, and non-player-owned AI parties
    // TODO: Change to use campaign behaviors. It's better design-wise.
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

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
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);

            // If this is campaign mode
            if (Campaign.Current?.DailyTickEvent != null)
            {
                // Add daily tick handler for training
                Campaign.Current.DailyTickEvent.AddHandler(
                    TrainingEventHandlers.DailyTrainingTickHandler);
            }
        }
    }
}
