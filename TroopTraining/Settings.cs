using MBOptionScreen.Attributes;
using MBOptionScreen.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace TrainingTweak
{
    public class Settings : SettingsBase
    {
        public const string InstanceID = "TrainingTweakSettings";

        private static Settings _instance = null;

        private const string XpMultipliers = "Training Xp Multipliers";
        private const string TierLimits = "Tier Limits";
        private const string FinancialSolutions = "Financial Solutions";
        private const string GeneralSettings = "General";

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

            set
            {
                _instance = value;
            }
        }

        [XmlElement]
        public override string Id { get; set; } = InstanceID;

        public override List<SettingPropertyGroupDefinition> GetSettingPropertyGroups()
        {
            return new SettingPropertyGroupDefinition[]
            {
                new SettingPropertyGroupDefinition(XpMultipliers),
                new SettingPropertyGroupDefinition(TierLimits),
                new SettingPropertyGroupDefinition(GeneralSettings),
                new SettingPropertyGroupDefinition(FinancialSolutions)
            }.ToList();
        }

        public override string ModuleFolderName => SubModule.ModuleFolderName;
        public override string ModName => SubModule.ModName;

        [XmlElement]
        [SettingProperty("Player Party Training Xp Multiplier", 0, 20,
            hintText: "Multiplier for all xp this mod gives to the player's party.")]
        [SettingPropertyGroup(XpMultipliers)]
        public float PlayerPartyTrainingXpMultiplier { get; set; } = 0.75f;

        [XmlElement]
        [SettingProperty("Player Clan Party Training Xp Multiplier", 0, 20,
            hintText: "Multiplier for all xp this mod gives to AI parties in the player's clan.")]
        [SettingPropertyGroup(XpMultipliers)]
        public float PlayerClanPartyTrainingXpMultiplier { get; set; } = 0.75f;

        [XmlElement]
        [SettingProperty("Player Clan Garrison Training Xp Multiplier", 0, 20,
            hintText: "Multiplier for all xp this mod gives to garrisons of player-owned settlements.")]
        [SettingPropertyGroup(XpMultipliers)]
        public float PlayerClanGarrisonTrainingXpMultiplier { get; set; } = 1.0f;

        [XmlElement]
        [SettingProperty("Non-Player Clan Party Training Xp Multiplier", 0, 20,
            hintText: "Multiplier for all xp this mod gives to AI parties not in the player's clan.")]
        [SettingPropertyGroup(XpMultipliers)]
        public float NonPlayerClanPartyTrainingXpMultiplier { get; set; } = 0.75f;

        [XmlElement]
        [SettingProperty("Non-Player Clan Garrison Training Xp Multiplier", 0, 20,
            hintText: "Multiplier for all xp this mod gives to garrisons of settlements not owned by the player.")]
        [SettingPropertyGroup(XpMultipliers)]
        public float NonPlayerClanGarrisonTrainingXpMultiplier { get; set; } = 1.0f;

        [XmlElement]
        [SettingProperty("Base Training Xp Gain", 0, 10,
            hintText: "Xp per troop when trained by a hero without either training perk.")]
        public int BaseTrainingXpGain { get; set; } = 5;

        [XmlElement]
        [SettingProperty("Base Training Max Tier Trained", 0, 20,
            hintText: "Max tier trained by a hero without either training perk.")]
        [SettingPropertyGroup(TierLimits)]
        public int BaseTrainingMaxTierTrained { get; set; } = 1;

        [XmlElement]
        [SettingProperty("Raise The Meek Max Tier Trained", 0, 20,
            hintText: "Max tier trained by the 'Raise The Meek' perk.")]
        [SettingPropertyGroup(TierLimits)]
        public int RaiseTheMeekMaxTierTrained { get; set; } = 3;

        [XmlElement]
        [SettingProperty("Level Difference Factor", 0, 25,
            hintText: "For every level the trainer is above the troop, training xp is increased by X percent.")]
        public float LevelDifferenceFactor { get; set; } = 6;

        [XmlElement]
        [SettingProperty("Leadership Skill Factor", 0, 5,
            hintText:"For each skill level in leadership, training xp is increased by X percent.")]
        public float LeadershipSkillFactor { get; set; } = 0.4f;

        [XmlElement]
        [SettingProperty("Training Xp Per Leadership Xp", 0, 999,
            hintText: "How much xp a trainer has to train troops to get 1 leadership xp.")]
        public float TrainingXpPerLeadershipXp { get; set; } = 20.0f;


        [XmlElement]
        [SettingProperty("Wounded Receive Training",
            hintText: "Whether wounded troops count toward group size during training.")]
        public bool WoundedReceiveTraining { get; set; } = false;

        [XmlElement]
        [SettingProperty("Upgradeable Receive Training",
            hintText: "Whether upgradeable troops count toward group size during training.")]
        public bool UpgradeableReceiveTraining { get; set; } = true;

        [XmlElement]
        [SettingProperty("Debug Mode",
            hintText: "Whether this mod displays potential errors it finds while running.")]
        public bool DebugMode { get; set; } = true;

        [XmlElement]
        [SettingProperty("Financial Solutions",
            hintText: "Enable patches to help prevent lords from bankrupting themselves on high-tier troops.")]
        [SettingPropertyGroup(FinancialSolutions, true)]
        public bool EnableFinancialSolutions { get; set; } = true;

        [XmlElement]
        [SettingProperty("Player Town Tax Income Multiplier", 0, 20,
            hintText: "Multiplier for all tax income from player-owned towns and castles.")]
        [SettingPropertyGroup(FinancialSolutions)]
        public float PlayerTownTaxIncomeMultiplier { get; set; } = 1.0f;

        [XmlElement]
        [SettingProperty("Player Village Tax Income Multiplier", 0, 20,
            hintText: "Multiplier for all tax income from player-owned villages.")]
        [SettingPropertyGroup(FinancialSolutions)]
        public float PlayerVillageTaxIncomeMultiplier { get; set; } = 1.0f;

        [XmlElement]
        [SettingProperty("Non-Player Town Tax Income Multiplier", 0, 20,
            hintText: "Multiplier for all tax income for AI-owned towns and castles.")]
        [SettingPropertyGroup(FinancialSolutions)]
        public float NonPlayerTownTaxIncomeMultiplier { get; set; } = 4.0f;

        [XmlElement]
        [SettingProperty("Non-Player Village Tax Income Multiplier", 0, 20,
            hintText: "Multiplier for all tax income for AI-owned villages.")]
        [SettingPropertyGroup(FinancialSolutions)]
        public float NonPlayerVillageTaxIncomeMultiplier { get; set; } = 4.0f;
        
    }
}
