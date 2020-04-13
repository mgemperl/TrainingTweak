using System;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace TrainingTweak
{
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
