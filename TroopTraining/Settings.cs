using ModLib;
using ModLib.Attributes;
using System.Xml.Serialization;

namespace TrainingTweak
{
    public class Settings : SettingsBase
    {
        public const string InstanceID = "TrainingTweakSettings";

        private static Settings _instance = null;

        private const string XpMultipliers = "Xp Multipliers";
        private const string GeneralSettings = "General Settings";
        private const string TierLimits = "Tier Limits";
        private const string TrainingModifiers = "Training Modifiers";

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

        /// <summary>
        /// A multiplier for all xp awarded to player troops by this mod.
        /// </summary>
        [XmlElement]
        [SettingProperty("Player Party Xp Multiplier", 0, 20, 
            "Multiplier for all xp this mod gives to the player's party.")]
        [SettingPropertyGroup(XpMultipliers)]
        public float PlayerPartyTrainingXpMultiplier { get; set; } = 1.0f;

        /// <summary>
        /// A multiplier for all training xp this mod gives to troops in parties not owned
        /// by the player.
        /// (This includes player-owned caravans)
        /// </summary>
        [XmlElement]
        [SettingProperty("Player Clan Party Xp Multiplier", 0, 20, 
            "Multiplier for all xp this mod gives to AI parties in the player's clan.")]
        [SettingPropertyGroup(XpMultipliers)]
        public float PlayerClanPartyTrainingXpMultiplier { get; set; } = 1.0f;

        /// <summary>
        /// A multiplier for all xp given to player-owned garrisons by this mod.
        /// </summary>
        [XmlElement]
        [SettingProperty("Player Clan Garrison Xp Multiplier", 0, 20, 
            "Multiplier for all xp this mod gives to garrisons of player-owned settlements.")]
        [SettingPropertyGroup(XpMultipliers)]
        public float PlayerClanGarrisonTrainingXpMultiplier { get; set; } = 1.0f;

        /// <summary>
        /// A multiplier for all training xp this mod gives to troops in parties not owned
        /// by the player.
        /// </summary>
        [XmlElement]
        [SettingProperty("Non-Player Clan Party Xp Multiplier", 0, 20, 
            "Multiplier for all xp this mod gives to AI parties not in the player's clan.")]
        [SettingPropertyGroup(XpMultipliers)]
        public float NonPlayerClanPartyTrainingXpMultiplier { get; set; } = 1.0f;

        /// <summary>
        /// A multiplier for all xp given to non-player-owned garrisons by this mod.
        /// </summary>
        [XmlElement]
        [SettingProperty("Non-Player Clan Garrison Xp Multiplier", 0, 20, 
            "Multiplier for all xp this mod gives to garrisons of settlements not owned by the player.")]
        [SettingPropertyGroup(XpMultipliers)]
        public float NonPlayerClanGarrisonTrainingXpMultiplier { get; set; } = 1.0f;

        /// <summary>
        /// XP per troop for a hero without either training perk.
        /// </summary>
        [XmlElement]
        [SettingProperty("Base Training Xp Gain", 0, 10, 
            "Xp per troop when trained by a hero without either training perk.")]
        public int BaseTrainingXpGain { get; set; } = 5;

        /// <summary>
        /// Max tier trained by hero without either training perk.
        /// </summary>
        [XmlElement]
        [SettingProperty("Base Training Max Tier Trained", 0, 10, 
            "Max tier trained by a hero without either training perk.")]
        [SettingPropertyGroup(TierLimits)]
        public int BaseTrainingMaxTierTrained { get; set; } = 1;

        /// <summary>
        /// Max tier trained by Raise The Meek perk. 
        /// </summary>
        [XmlElement]
        [SettingProperty("Raise The Meek Max Tier Trained", 0, 10, 
            "Max tier trained by the 'Raise The Meek' perk.")]
        [SettingPropertyGroup(TierLimits)]
        public int RaiseTheMeekMaxTierTrained { get; set; } = 3;

        /// <summary>
        /// For every X levels the trainer is above the troop, the xp gain is increased by 100%.
        /// For example, if set to 5, a level 20 trainer training a level 10 troops will
        /// result in x3 experience gained.
        /// </summary>
        [XmlElement]
        [SettingProperty("Level Difference Multiple", 0, 20, 
            "For every X levels the trainer is above the troop, training xp is increased by 100%.")]
        public int LevelDifferenceMultiple { get; set; } = 4;

        /// <summary>
        /// How much xp a trainer has to train troops to get 1 leadership xp.
        /// </summary>
        [XmlElement]
        [SettingProperty("Training Xp Per Leadership Xp", 0, 999, 
            "How much xp a trainer has to train troops to get 1 leadership xp.")]
        public float TrainingXpPerLeadershipXp { get; set; } = 20.0f;
	
        /// <summary>
        /// Whether wounded troops receive training xp.
        /// </summary>
        [XmlElement]
        [SettingProperty("Wounded Receive Training", 
            "Whether wounded troops count toward group size during training.")]
        public bool WoundedReceiveTraining { get; set; } = false;

        /// <summary>
        /// Whether upgradeable troops receive training xp.
        /// </summary>
        [XmlElement]
        [SettingProperty("Upgradeable Receive Training", 
            "Whether upgradeable troops count toward group size during training.")]
        public bool UpgradeableReceiveTraining { get; set; } = true;

        
        
    }
}
