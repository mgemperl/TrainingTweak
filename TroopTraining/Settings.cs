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
        public const string InstanceID = "TrainingTweak_v3_dev";

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
                //_settings.Unregister();
            }
            catch (Exception exc)
            {

            }
        }

float _testFloat = 0;
public void BuildSettings()
{
            /*
    var builder = new DefaultSettingsBuilder("Test_v1", "Test Mod")
        .SetFormat("json")
        .SetFolderName("Test Mod")
        .CreateGroup("Test Group", groupBuilder =>
            groupBuilder.AddFloatingInteger("tf_00", "Test Name", 0f, 20f,
                new ProxyRef<float>(() => _testFloat, (value) => _testFloat = value),
                builder => builder  
                    .SetHintText("Test Hint")));

    _settings = builder.BuildAsGlobal();
    _settings.Register();
}
            */


            int order = 0;
            var builder = new DefaultSettingsBuilder(InstanceID, SubModule.ModName)
                .SetFormat("xml")
                .SetFolderName(SubModule.ModuleFolderName);

            // Training Perk Overrides Group
            builder.CreateGroup($"{Strings.TrainingPerkGroup}",
                groupBuilder => groupBuilder
                .SetGroupOrder(order++)
                .SetIsMainToggle(true)
                .AddBool(nameof(EnableTrainingPerkOverrides),
                    Strings.TrainingPerkGroup,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(EnableTrainingPerkOverrides)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.EnableTrainingPerkOverridesHint))
                );

            // Training Perk xp amounts
            builder.CreateGroup($"{Strings.TrainingPerkGroup}/{Strings.TrainingPerkXpGain}",
                groupBuilder => groupBuilder
                .SetGroupOrder(order++)
                .AddInteger(nameof(RaiseTheMeekXpAmount),
                    Strings.RaiseTheMeekXpAmountDisplay, 0, 100,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(RaiseTheMeekXpAmount)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.RaiseTheMeekXpAmountHint))
                .AddInteger(nameof(CombatTipsXpAmount),
                    Strings.CombatTipsXpAmountDisplay, 0, 100,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(CombatTipsXpAmount)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.CombatTipsXpAmountHint))
                );

            // Training Perk tier limits
            builder.CreateGroup($"{Strings.TrainingPerkGroup}/{Strings.TierLimitsSettingsGroup}",
                groupBuilder => groupBuilder
                .SetGroupOrder(order++)
                .AddInteger(nameof(RaiseTheMeekMaxTierTrained),
                    Strings.RaiseTheMeekMaxTierTrainedDisplay, 0, 20,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(RaiseTheMeekMaxTierTrained)), this),
                    intBuilder => intBuilder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.RaiseTheMeekMaxTierTrainedHint))
                .AddInteger(nameof(ComatTipsMaxTierTrained),
                    Strings.CombatTipsMaxTierTrainedDisplay, 0, 20,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(ComatTipsMaxTierTrained)), this),
                    intBuilder => intBuilder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.CombatTipsMaxTierTrainedHint))
                );

            // Training Perk xp multipliers 
            builder.CreateGroup($"{Strings.TrainingPerkGroup}" +
                    $"/{Strings.XpMultipliersSettingsGroup}",
                groupBuilder => groupBuilder
                .SetGroupOrder(order++)
                .AddFloatingInteger(nameof(PlayerPartyTrainingXpMultiplier),
                    Strings.PlayerPartyTrainingMultDisplay, 0f, 20f,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(PlayerPartyTrainingXpMultiplier)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.PlayerPartyTrainingMultHint)
                        .AddValueFormat("#0%"))
                .AddFloatingInteger(nameof(PlayerClanPartyTrainingXpMultiplier),
                    Strings.PlayerClanPartyTrainingMultDisplay, 0f, 20f,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(PlayerClanPartyTrainingXpMultiplier)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.PlayerClanPartyTrainingMultHint)
                        .AddValueFormat("#0%"))
                .AddFloatingInteger(nameof(NonPlayerClanPartyTrainingXpMultiplier),
                    Strings.NonPlayerClanPartyTrainingMultDisplay, 0f, 20f,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(NonPlayerClanPartyTrainingXpMultiplier)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.NonPlayerClanPartyTrainingMultHint)
                        .AddValueFormat("#0%"))
                );

            // Training Perk General
            builder.CreateGroup($"{Strings.TrainingPerkGroup}" +
                    $"/{Strings.GeneralSettingsGroup}",
                groupBuilder => groupBuilder
                .SetGroupOrder(order++)
                .AddFloatingInteger(nameof(LevelDifferenceFactor),
                    Strings.LevelDifferenceFactorDisplay, 0f, 20f,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(LevelDifferenceFactor)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.LevelDifferenceFactorHint))
                .AddFloatingInteger(nameof(LeadershipSkillFactor),
                    Strings.LeadershipSkillFactorDisplay, 0f, 20f,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(LeadershipSkillFactor)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.LeadershipSkillFactorHint))
                .AddFloatingInteger(nameof(TrainingXpPerLeadershipXp),
                    Strings.TrainingXpPerLeadershipXpDisplay, 0f, 20f,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(TrainingXpPerLeadershipXp)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.TrainingXpPerLeadershipXpHint))
                .AddBool(nameof(WoundedReceiveTraining),
                    Strings.WoundedReceiveTrainingDisplay,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(WoundedReceiveTraining)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.WoundedReceiveTrainingHint))
                .AddBool(nameof(UpgradeableReceiveTraining),
                    Strings.UpgradeableReceiveTrainingDisplay,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(UpgradeableReceiveTraining)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.UpgradeableReceiveTrainingHint))
                );

            // Base Training Settings
            builder.CreateGroup($"{Strings.BaseTrainingSettingsGroup}",
                groupBuilder => groupBuilder
                .SetGroupOrder(order++)
                .SetIsMainToggle(true)
                .AddBool(nameof(EnableBaseTraining),
                    Strings.BaseTrainingSettingsGroup,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(EnableBaseTraining)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.EnableBaseTrainingHint))
                );

            builder.CreateGroup($"{Strings.BaseTrainingSettingsGroup}/" +
                $"{Strings.GeneralSettingsGroup}",
                groupBuilder => groupBuilder
                .SetGroupOrder(order++)
                .AddInteger(nameof(BaseTrainingXpAmount),
                    Strings.BaseTrainingXpAmountDisplay, 0, 100,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(BaseTrainingXpAmount)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.BaseTrainingXpAmountHint))
                .AddInteger(nameof(BaseTrainingMaxTierTrained),
                    Strings.BaseTrainingTierDisplay, 0, 20,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(BaseTrainingMaxTierTrained)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.BaseTrainingTierHint))
                );


            // Garrison Training Overrides
            builder.CreateGroup($"{Strings.GarrisonGroup}",
                groupBuilder => groupBuilder
                .SetGroupOrder(order++)
                .SetIsMainToggle(true)
                .AddBool(nameof(EnableGarrisonTraining),
                    Strings.GarrisonGroup,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(EnableGarrisonTraining)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.EnableGarrisonTrainingOverrideHint))
                );

            builder.CreateGroup($"{Strings.GarrisonGroup}/" +
                $"{Strings.GeneralSettingsGroup}",
                groupBuilder => groupBuilder
                .SetGroupOrder(order++)
                .AddInteger(nameof(LevelOneTrainingFieldXpAmount),
                    Strings.LevelOneTrainingFieldXpAmountDisplay, 0, 100,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(LevelOneTrainingFieldXpAmount)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.LevelOneTrainingFieldXpAmountHint))
                .AddInteger(nameof(GarrisonTrainingMaxTierTrained),
                    Strings.GarrisonMaxTierTrainedDisplay, 0, 20,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(GarrisonTrainingMaxTierTrained)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.GarrisionMaxTierTrainedHint))
                .AddFloatingInteger(nameof(PlayerClanGarrisonTrainingXpMultiplier),
                    Strings.PlayerClanGarrisonTrainingMultDisplay, 0f, 20f,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(PlayerClanGarrisonTrainingXpMultiplier)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.PlayerClanGarrisonTrainingMultHint)
                        .AddValueFormat("#0%"))
                .AddFloatingInteger(nameof(NonPlayerClanGarrisonTrainingXpMultiplier),
                    Strings.NonPlayerClanGarrisonTrainingMultDisplay, 0f, 20f,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(NonPlayerClanGarrisonTrainingXpMultiplier)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.NonPlayerClanGarrisonTrainingMultHint)
                        .AddValueFormat("#0%"))
                );

            // Financial Solutions Settings
            builder.CreateGroup($"{Strings.FinancialSolutionsSettingGroup}",
                groupBuilder => groupBuilder
                .SetGroupOrder(order++)
                .SetIsMainToggle(true)
                .AddBool(nameof(EnableFinancialSolutions),
                    Strings.FinancialSolutionsSettingGroup,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(EnableFinancialSolutions)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.EnableFinancialSolutionsHint))
                );

            builder.CreateGroup($"{Strings.FinancialSolutionsSettingGroup}" +
                    $"/{Strings.GeneralSettingsGroup}",
                groupBuilder => groupBuilder
                .SetGroupOrder(order++)
                .AddFloatingInteger(nameof(TroopUpgradeCostMultiplier),
                    Strings.TroopUpgradeCostMultiplierDisplay, 0f, 50f,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(TroopUpgradeCostMultiplier)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.TroopUpgradeCostMultiplierHint)
                        .AddValueFormat("#0%"))
                );

            builder.CreateGroup($"{Strings.FinancialSolutionsSettingGroup}" +
                    $"/{Strings.PlayerSubgroup}",
                groupBuilder => groupBuilder
                .SetGroupOrder(order++)
                .AddFloatingInteger(nameof(PlayerTownTaxIncomeMultiplier),
                    Strings.PlayerTownTaxIncomeMultiplierDisplay, 0f, 20f,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(PlayerTownTaxIncomeMultiplier)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.PlayerTownTaxIncomeMultiplierHint)
                        .AddValueFormat("#0%"))
                .AddFloatingInteger(nameof(PlayerVillageTaxIncomeMultiplier),
                    Strings.PlayerVillageTaxIncomeMultiplierDisplay, 0f, 20f,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(PlayerVillageTaxIncomeMultiplier)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.PlayerVillageTaxIncomeMultiplierHint)
                        .AddValueFormat("#0%"))
                .AddFloatingInteger(nameof(PlayerClanPartyWageMultiplier),
                    Strings.PlayerClanPartyWageMultiplierDisplay, 0f, 20f,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(PlayerClanPartyWageMultiplier)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.PlayerClanPartyWageMultiplierHint)
                        .AddValueFormat("#0%"))
                );

            builder.CreateGroup($"{Strings.FinancialSolutionsSettingGroup}" +
                    $"/{Strings.AISubgroup}",
                groupBuilder => groupBuilder
                .SetGroupOrder(order++)
                .AddFloatingInteger(nameof(NonPlayerTownTaxIncomeMultiplier),
                    Strings.NonPlayerTownTaxIncomeMultiplierDisplay, 0f, 20f,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(NonPlayerTownTaxIncomeMultiplier)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.NonPlayerTownTaxIncomeMultiplierHint)
                        .AddValueFormat("#0%"))
                .AddFloatingInteger(nameof(NonPlayerVillageTaxIncomeMultiplier),
                    Strings.NonPlayerVillageTaxIncomeMultiplierDisplay, 0f, 20f,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(NonPlayerVillageTaxIncomeMultiplier)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.NonPlayerVillageTaxIncomeMultiplierHint)
                        .AddValueFormat("#0%"))
                .AddFloatingInteger(nameof(NonPlayerClanPartyWageMultiplier),
                    Strings.NonPlayerClanPartyWageMultiplierDisplay, 0f, 20f,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(NonPlayerClanPartyWageMultiplier)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.NonPlayerClanPartyWageMultiplierHint)
                        .AddValueFormat("#0%"))
                );

            // Debug stuff
            builder.CreateGroup($"{Strings.DebugSettingGroup}",
                groupBuilder => groupBuilder
                .SetGroupOrder(order++)
                .AddBool(nameof(DebugMode),
                    Strings.EnableDebugModeDisplay,
                    new PropertyRef(typeof(Settings).GetProperty(
                        nameof(DebugMode)), this),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetOrder(order++)
                        .SetHintText(Strings.EnableDebugModeHint))
                );

            // Build and register the settings
            _settings = builder.BuildAsGlobal();
            _settings.Register();
        }

        // Training Perk Settings
        public bool EnableTrainingPerkOverrides { get; set; } = true;
        public int RaiseTheMeekXpAmount { get; set; } = 24;
        public int CombatTipsXpAmount { get; set; } = 12;
        public int RaiseTheMeekMaxTierTrained { get; set; } = 3;
        public int ComatTipsMaxTierTrained { get; set; } = 20;
        public float PlayerPartyTrainingXpMultiplier { get; set; } = 1f;
        public float PlayerClanPartyTrainingXpMultiplier { get; set; } = 1f;
        public float NonPlayerClanPartyTrainingXpMultiplier { get; set; } = 1f;
        public float LevelDifferenceFactor { get; set; } = 6;
        public float LeadershipSkillFactor { get; set; } = 0.4f;
        public float TrainingXpPerLeadershipXp { get; set; } = 10.0f;
        public bool WoundedReceiveTraining { get; set; } = false;
        public bool UpgradeableReceiveTraining { get; set; } = true;

        // Base Training Settings
        public bool EnableBaseTraining { get; set; } = true;
        public int BaseTrainingXpAmount { get; set; } = 5;
        public int BaseTrainingMaxTierTrained { get; set; } = 1;

        // Garrison Settings 
        public bool EnableGarrisonTraining { get; set; } = true;
        public int LevelOneTrainingFieldXpAmount { get; set; } = 4;
        public int GarrisonTrainingMaxTierTrained { get; set; } = 20;
        public float PlayerClanGarrisonTrainingXpMultiplier { get; set; } = 1.0f;
        public float NonPlayerClanGarrisonTrainingXpMultiplier { get; set; } = 1.0f;

        // Financial Solutions Settings
        public bool EnableFinancialSolutions { get; set; } = true;
        public float PlayerTownTaxIncomeMultiplier { get; set; } = 1.0f;
        public float PlayerVillageTaxIncomeMultiplier { get; set; } = 1.0f;
        public float NonPlayerTownTaxIncomeMultiplier { get; set; } = 3.0f;
        public float NonPlayerVillageTaxIncomeMultiplier { get; set; } = 3.0f;
        public float PlayerClanPartyWageMultiplier { get; set; } = 1.0f;
        public float NonPlayerClanPartyWageMultiplier { get; set; } = 1.0f;
        public float TroopUpgradeCostMultiplier { get; set; } = 1.0f;

        // Debug
        public bool DebugMode { get; set; } = false;

    }
}
