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
