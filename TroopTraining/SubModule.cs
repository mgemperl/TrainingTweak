using System;
using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TrainingTweak.HarmonyPatches;
using TrainingTweak.Settings;
using HarmonyPatch = TrainingTweak.HarmonyPatches.Base.HarmonyPatch;

namespace TrainingTweak
{
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
                /*
                var textMan = TaleWorlds.MountAndBlade.Module.CurrentModule.GlobalTextManager;
                textMan.loa(Path.Combine(BasePath.Name, "Modules", "TrainingTweak",
                    "ModuleData", "module_strings.xml"));
                    */
            }
            catch (Exception exc)
            {
                Util.Warning("Training Tweak mod failed to load " +
                    "module_strings.xml language file. Using English." +
                    $"\n\n{exc.Message}");
            }

            InformationManager.DisplayMessage(new InformationMessage(
                "Training Tweak loaded", Color.White));
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            // Register settings with MCM
            try
            {
                ModSettings.Instance.RegisterSettings();
            }
            catch (Exception exc)
            {
                Util.Warning(Strings.SettingsRegistrationFailed, exc);
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            // If playing in the campaign game mode
            if (gameStarterObject is not CampaignGameStarter gameStarter) return;
            
            base.OnGameStart(game, gameStarter);
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            if (game.GameType is not Campaign) 
                return;

            var patches = new List<HarmonyPatch>
            {
                new TroopTrainingPatch(_harmony),
                new TroopUpgradeCostPatch(_harmony),
                new PartyWagePatch(_harmony),
                new TownTaxesPatch(_harmony),
                new VillageTaxesPatch(_harmony)
            };

            foreach (var patch in patches)
                patch.Apply();
        }

        
        public override void OnGameEnd(Game game)
        {
            base.OnGameEnd(game);

            try
            {
                _harmony.UnpatchAll(HarmonyId);
            }
            catch (Exception exc) {}
        }

        protected override void OnSubModuleUnloaded()
        {
            ModSettings.Instance.Dispose();
        }
    }
}
