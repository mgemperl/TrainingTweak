using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.FluentBuilder.Implementation;
using MCM.Abstractions.Ref;
using MCM.Abstractions.Settings.Base.Global;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml.Serialization;

namespace TrainingTweak
{
    public class Settings : IDisposable
    {
        public const string InstanceID = "TrainingTweak_v3";

        private FluentGlobalSettings _settings;
        private static Settings _instance;

        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Settings();
                }
                return _instance;
            }
        }

        public void Dispose()
        {
            try
            {
                _settings.Unregister();
            }
            catch (Exception exc)
            {

            }
        }
        // I used the fluent builder in order to support localization at runtime.
        // This is almost certainly not how settings should be implemented for localization.
        // I just couldn't figure out how to do it through Bannerlord's own localization system, myself.
        // I do not recommend imitating this.

        float _testFloat = 0;
        public void BuildSettings()
        {
            /*
            if (_settings != null)
            {
                _settings.Unregister();
                _settings = null;
            }
            */

            var builder = new DefaultSettingsBuilder(InstanceID, SubModule.ModName)
                .SetFormat("json")
                .SetFolderName(SubModule.ModuleFolderName)
                .SetSubFolder("")
                .CreateGroup(Strings.XpMultipliersSettingGroup, groupBuilder =>
                    groupBuilder.AddFloatingInteger("Test Name", 0f, 20f,
                        new ProxyRef<float>(() => _testFloat, (value) => _testFloat = value),
                        floatBuilder => floatBuilder
                            .SetRequireRestart(false)
                            .SetHintText("Test Hint")));


            /*
            var builder = new DefaultSettingsBuilder(InstanceID, SubModule.ModName)
                .SetFormat("json")
                .SetFolderName(SubModule.ModuleFolderName)
                .SetSubFolder("")
                .CreateGroup(Strings.XpMultipliersSettingGroup, groupBuilder =>
                    groupBuilder.AddFloatingInteger(Strings.PlayerPartyTrainingMultDisplay, 0f, 20f,
                        new PropertyRef(typeof(Settings).GetProperty(
                            nameof(PlayerPartyTrainingXpMultiplier)), this),
                        floatBuilder => floatBuilder
                            .SetRequireRestart(false)
                            .SetHintText(Strings.PlayerPartyTrainingMultHint))
                    .AddFloatingInteger(Strings.PlayerClanPartyTrainingMultDisplay, 0f, 20f,
                        new PropertyRef(typeof(Settings).GetProperty(
                            nameof(PlayerClanPartyTrainingXpMultiplier)), this),
                        floatBuilder => floatBuilder
                            .SetRequireRestart(false)
                            .SetHintText(Strings.PlayerClanPartyTrainingMultHint))
                    .AddFloatingInteger(Strings.PlayerClanGarrisonTrainingMultDisplay, 0f, 20f,
                        new PropertyRef(typeof(Settings).GetProperty(
                            nameof(PlayerClanGarrisonTrainingXpMultiplier)), this),
                        floatBuilder => floatBuilder
                            .SetRequireRestart(false)
                            .SetHintText(Strings.PlayerClanGarrisonTrainingMultHint))
                    .AddFloatingInteger(Strings.NonPlayerClanPartyTrainingMultDisplay, 0f, 20f,
                        new PropertyRef(typeof(Settings).GetProperty(
                            nameof(NonPlayerClanPartyTrainingXpMultiplier)), this),
                        floatBuilder => floatBuilder
                            .SetRequireRestart(false)
                            .SetHintText(Strings.NonPlayerClanPartyTrainingMultHint))
                    .AddFloatingInteger(Strings.NonPlayerClanGarrisonTrainingMultDisplay, 0f, 20f,
                        new PropertyRef(typeof(Settings).GetProperty(
                            nameof(NonPlayerClanGarrisonTrainingXpMultiplier)), this),
                        floatBuilder => floatBuilder
                            .SetRequireRestart(false)
                            .SetHintText(Strings.NonPlayerClanGarrisonTrainingMultHint)))
                .CreateGroup(Strings.TierLimitsSettingGroup, groupBuilder =>
                    groupBuilder.AddInteger(Strings.RaiseTheMeekTierDisplay, 0, 20,
                        new PropertyRef(typeof(Settings).GetProperty(
                            nameof(RaiseTheMeekMaxTierTrained)), this),
                        intBuilder => intBuilder
                            .SetRequireRestart(false)
                            .SetHintText(Strings.RaiseTheMeekTierHint)));
                            */

            _settings = builder.BuildAsGlobal();
            _settings.Register();
        }

        public float PlayerPartyTrainingXpMultiplier { get; set; } = 0.75f;

        public float PlayerClanPartyTrainingXpMultiplier { get; set; } = 0.75f;

        public float PlayerClanGarrisonTrainingXpMultiplier { get; set; } = 4.0f;

        public float NonPlayerClanPartyTrainingXpMultiplier { get; set; } = 0.75f;

        public float NonPlayerClanGarrisonTrainingXpMultiplier { get; set; } = 4.0f;

        /*
        [SettingProperty(displayName: "Raise The Meek Max Tier Trained", 
            minValue: 0, maxValue: 20, requireRestart: false,
            hintText: "Max tier of troops in parties that will be trained by the " +
                      "'Raise The Meek' perk.")]
        [SettingPropertyGroup(TierLimits, order: 1)]
        */
        public int RaiseTheMeekMaxTierTrained { get; set; } = 3;

        /*
        [SettingProperty(displayName: "Garrison Max Tier Trained", 
            minValue: 0, maxValue: 20, requireRestart: false,
            hintText: "Max tier of troops in garrisons that will be trained by this mod.")]
        [SettingPropertyGroup(TierLimits)]
        */
        public int GarrisonTrainingMaxTierTrained { get; set; } = 20;

        /*
        [SettingProperty(displayName: "All Training Max Tier Trained", 
            minValue: 0, maxValue: 20, requireRestart: false,
            hintText: "Max tier of troops that will be trained by this mod. Overrides " +
                      "other tier limit settings if lower than them.")]
        [SettingPropertyGroup(TierLimits)]
        */
        public int AllTrainingMaxTierTrained { get; set; } = 20;

        /*
        [SettingProperty(displayName: "Level Difference Factor", 
            minValue: 0, maxValue: 25, requireRestart: false,
            hintText: "For every level the trainer is above the troop, training xp is increased by X percent.")]
        [SettingPropertyGroup(GeneralSettings, order: 2)]
        */
        public float LevelDifferenceFactor { get; set; } = 6;

        /*
        [SettingProperty(displayName: "Leadership Skill Factor", 
            minValue: 0, maxValue: 5, requireRestart: false,
            hintText:"For each skill level in leadership, training xp is increased by X percent.")]
        [SettingPropertyGroup(GeneralSettings)]
        */
        public float LeadershipSkillFactor { get; set; } = 0.4f;

        /*
        [SettingProperty(displayName: "Training Xp Per Leadership Xp", 
            minValue: 0, maxValue: 100, requireRestart: false,
            hintText: "How much xp a trainer has to train troops to get 1 leadership xp.")]
        [SettingPropertyGroup(GeneralSettings)]
        */
        public float TrainingXpPerLeadershipXp { get; set; } = 10.0f;

        /*
        [SettingProperty(displayName: "Wounded Receive Training",
            requireRestart: false,
            hintText: "Whether wounded troops count toward group size during training.")]
        [SettingPropertyGroup(GeneralSettings)]
        */
        public bool WoundedReceiveTraining { get; set; } = false;

        /*
        [SettingProperty(displayName: "Upgradeable Receive Training",
            requireRestart: false,
            hintText: "Whether upgradeable troops count toward group size during training.")]
        [SettingPropertyGroup(GeneralSettings)]
        */
        public bool UpgradeableReceiveTraining { get; set; } = true;

        /*
        [SettingProperty(displayName: "Debug Mode", 
            requireRestart: false,
            hintText: "Whether this mod displays potential errors it finds while running.")]
        [SettingPropertyGroup(GeneralSettings)]
        */
        public bool DebugMode { get; set; } = false;

        /*
        [SettingProperty(displayName: FinancialSolutions,
            requireRestart: false,
            hintText: "Enable patches to help prevent lords from bankrupting themselves on high-tier troops.")]
        [SettingPropertyGroup(FinancialSolutions, isMainToggle: true, order: 4)]
        */
        public bool EnableFinancialSolutions { get; set; } = true;

        /*
        [SettingProperty(displayName: "Player Town Tax Income Multiplier", 
            minValue: 0, maxValue: 20, requireRestart: false,
            hintText: "Multiplier for all tax income from player-owned towns and castles.")]
        [SettingPropertyGroup(FinancialSolutions)]
        */
        public float PlayerTownTaxIncomeMultiplier { get; set; } = 1.0f;

        /*
        [SettingProperty(displayName: "Player Village Tax Income Multiplier", 
            minValue: 0, maxValue: 20, requireRestart: false,
            hintText: "Multiplier for all tax income from player-owned villages.")]
        [SettingPropertyGroup(FinancialSolutions)]
        */
        public float PlayerVillageTaxIncomeMultiplier { get; set; } = 1.0f;

        /*
        [SettingProperty(displayName: "Non-Player Town Tax Income Multiplier", 
            minValue: 0, maxValue: 20, requireRestart: false,
            hintText: "Multiplier for all tax income for AI-owned towns and castles.")]
        [SettingPropertyGroup(FinancialSolutions)]
        */
        public float NonPlayerTownTaxIncomeMultiplier { get; set; } = 3.0f;

        /*
        [SettingProperty(displayName: "Non-Player Village Tax Income Multiplier", 
            minValue: 0, maxValue: 20, requireRestart: false,
            hintText: "Multiplier for all tax income for AI-owned villages.")]
        [SettingPropertyGroup(FinancialSolutions)]
        */
        public float NonPlayerVillageTaxIncomeMultiplier { get; set; } = 3.0f;

        /*
        [SettingProperty(displayName: "Player Party Wage Multiplier", 
            minValue: 0, maxValue: 20, requireRestart: false,
            hintText: "Multiplier for all party wages for parties in the player's clan.")]
        [SettingPropertyGroup(FinancialSolutions)]
        */
        public float PlayerClanPartyWageMultiplier { get; set; } = 1.0f;

        /*
        [SettingProperty(displayName: "Non-Player Party Wage Multiplier", 
            minValue: 0, maxValue: 20, requireRestart: false,
            hintText: "Multiplier for all party wages for parties not in the player's clan.")]
        [SettingPropertyGroup(FinancialSolutions)]
        */
        public float NonPlayerClanPartyWageMultiplier { get; set; } = 1.0f;

        /*
        [SettingProperty(displayName: "Troop Upgrade Cost Multiplier", 
            minValue: 0, maxValue: 50, requireRestart: false,
            hintText: "Multiplier for the upgrade cost of all troops.")]
        [SettingPropertyGroup(FinancialSolutions)]
        */
        public float TroopUpgradeCostMultiplier { get; set; } = 1.0f;

        /*
        [SettingProperty(displayName: BaseTraining,
            requireRestart: false,
            hintText: "Allow heroes with neither training perk to train troops.")]
        [SettingPropertyGroup(BaseTraining, isMainToggle: true, order: 3)]
        */
        public bool EnableBaseTraining { get; set; } = true;

        /*
        [SettingProperty(displayName: "Basic Training Xp Gain",
            minValue: 0, maxValue: 10, requireRestart: false,
            hintText: "Xp per troop when trained by a hero without either training perk.")]
        [SettingPropertyGroup(BaseTraining)]
        */
        public int BaseTrainingXpGain { get; set; } = 5;

        /*
        [SettingProperty(displayName: "Basic Training Max Tier Trained",
            minValue: 0, maxValue: 20, requireRestart: false,
            hintText: "Max tier trained by a hero without either training perk.")]
        [SettingPropertyGroup(BaseTraining)]
        */
        public int BaseTrainingMaxTierTrained { get; set; } = 1;

    }
}
