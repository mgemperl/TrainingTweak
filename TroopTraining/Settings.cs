using ModLib;
using ModLib.Attributes;
using System.Xml.Serialization;

namespace TrainingTweak
{
    public class Settings : SettingsBase
    {
        public const string InstanceID = "TrainingTweakSettings";

        private static Settings _instance = null;

        private const string XpMultipliers = "Training Xp Multipliers";
        private const string TierLimits = "Tier Limits";
        private const string Debugging = "Debugging";

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
        public override string ID { get; set; } = InstanceID;

        public override string ModuleFolderName => SubModule.ModuleFolderName;
        public override string ModName => SubModule.ModName;

        [XmlElement]
        [SettingProperty("Player Party Training Xp Multiplier", 0, 20, 
            "Multiplier for all xp this mod gives to the player's party.")]
        [SettingPropertyGroup(XpMultipliers)]
        public float PlayerPartyTrainingXpMultiplier { get; set; } = 1.0f;

        [XmlElement]
        [SettingProperty("Player Clan Party Training Xp Multiplier", 0, 20, 
            "Multiplier for all xp this mod gives to AI parties in the player's clan.")]
        [SettingPropertyGroup(XpMultipliers)]
        public float PlayerClanPartyTrainingXpMultiplier { get; set; } = 1.0f;

        [XmlElement]
        [SettingProperty("Player Clan Garrison Training Xp Multiplier", 0, 20, 
            "Multiplier for all xp this mod gives to garrisons of player-owned settlements.")]
        [SettingPropertyGroup(XpMultipliers)]
        public float PlayerClanGarrisonTrainingXpMultiplier { get; set; } = 1.0f;

        [XmlElement]
        [SettingProperty("Non-Player Clan Party Training Xp Multiplier", 0, 20, 
            "Multiplier for all xp this mod gives to AI parties not in the player's clan.")]
        [SettingPropertyGroup(XpMultipliers)]
        public float NonPlayerClanPartyTrainingXpMultiplier { get; set; } = 1.0f;

        [XmlElement]
        [SettingProperty("Non-Player Clan Garrison Training Xp Multiplier", 0, 20, 
            "Multiplier for all xp this mod gives to garrisons of settlements not owned by the player.")]
        [SettingPropertyGroup(XpMultipliers)]
        public float NonPlayerClanGarrisonTrainingXpMultiplier { get; set; } = 1.0f;

        [XmlElement]
        [SettingProperty("Base Training Xp Gain", 0, 10, 
            "Xp per troop when trained by a hero without either training perk.")]
        public int BaseTrainingXpGain { get; set; } = 5;

        [XmlElement]
        [SettingProperty("Base Training Max Tier Trained", 0, 10, 
            "Max tier trained by a hero without either training perk.")]
        [SettingPropertyGroup(TierLimits)]
        public int BaseTrainingMaxTierTrained { get; set; } = 1;

        [XmlElement]
        [SettingProperty("Raise The Meek Max Tier Trained", 0, 10, 
            "Max tier trained by the 'Raise The Meek' perk.")]
        [SettingPropertyGroup(TierLimits)]
        public int RaiseTheMeekMaxTierTrained { get; set; } = 3;

        [XmlElement]
        [SettingProperty("Level Difference Multiple", 0, 20, 
            "For every X levels the trainer is above the troop, training xp is increased by 100%.")]
        public int LevelDifferenceMultiple { get; set; } = 4;

        [XmlElement]
        [SettingProperty("Training Xp Per Leadership Xp", 0, 999, 
            "How much xp a trainer has to train troops to get 1 leadership xp.")]
        public float TrainingXpPerLeadershipXp { get; set; } = 20.0f;
	
        [XmlElement]
        [SettingProperty("Wounded Receive Training", 
            "Whether wounded troops count toward group size during training.")]
        public bool WoundedReceiveTraining { get; set; } = false;

        [XmlElement]
        [SettingProperty("Upgradeable Receive Training", 
            "Whether upgradeable troops count toward group size during training.")]
        public bool UpgradeableReceiveTraining { get; set; } = true;

        [XmlElement]
        [SettingProperty("Debug Mode", 
            "Whether this mod displays potential errors it finds while running.")]
        public bool DebugMode { get; set; } = false;
        
    }
}
