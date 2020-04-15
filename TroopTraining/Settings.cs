using ModLib;
using ModLib.Attributes;
using System.Xml.Serialization;

namespace TrainingTweak
{
    public class Settings : SettingsBase
    {
        private static Settings _instance = null;

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
        public override string ID { get; set; } = "trainingtweaksettings.mareus";

        public override string ModuleFolderName => SubModule.ModuleFolderName;
        public override string ModName => SubModule.ModName;

        /// <summary>
        /// A multiplier for all xp awarded to player troops by this mod.
        /// </summary>
        [XmlElement]
        [SettingProperty("Player Party Xp Multiplier", 0, 9999, 
            "Multiplier for all xp awarded to the player's party by this mod.")]
        public float PlayerPartyTrainingXpMultiplier { get; set; } = 1.0f;

        /// <summary>
        /// A multiplier for all training xp this mod gives to troops in parties not owned
        /// by the player.
        /// (This includes player-owned caravans)
        /// </summary>
        [XmlElement]
        [SettingProperty("TEST", 0, 9999, "test")]
        public float PlayerClanPartyTrainingXpMultiplier { get; set; } = 1.0f;

        /// <summary>
        /// A multiplier for all training xp this mod gives to troops in parties not owned
        /// by the player.
        /// (This includes player-owned caravans)
        /// </summary>
        [XmlElement]
        [SettingProperty("TEST", 0, 9999, "test")]
        public float NonPlayerClanPartyTrainingXpMultiplier { get; set; } = 1.0f;

        /// <summary>
        /// XP per troop for a hero without either training perk.
        /// </summary>
        [XmlElement]
        [SettingProperty("TEST", 0, 9999, "test")]
        public int BaseTrainingXpGain { get; set; } = 5;

        /// <summary>
        /// Max tier trained by hero without either training perk.
        /// </summary>
        [XmlElement]
        [SettingProperty("TEST", 0, 9999, "test")]
        public int BaseTrainingMaxTierTrained { get; set; } = 1;

        /// <summary>
        /// Max tier trained by Raise The Meek perk. 
        /// </summary>
        [XmlElement]
        [SettingProperty("TEST", 0, 9999, "test")]
        public int RaiseTheMeekMaxTierTrained { get; set; } = 3;

        /// <summary>
        /// For every X levels the trainer is above the troop, the xp gain is increased by 100%.
        /// For example, if set to 5, a level 20 trainer training a level 10 troops will
        /// result in x3 experience gained.
        /// </summary>
        [XmlElement]
        [SettingProperty("TEST", 0, 9999, "test")]
        public int LevelDifferenceMultiple { get; set; } = 4;

        /// <summary>
        /// How much xp a trainer has to train troops to get 1 leadership xp.
        /// </summary>
        [XmlElement]
        [SettingProperty("TEST", 0, 9999, "test")]
        public float TrainingXpPerLeadershipXp { get; set; } = 20.0f;
	
        /// <summary>
        /// Whether wounded troops receive training xp.
        /// </summary>
        [XmlElement]
        [SettingProperty("TEST", "test")]
        public bool WoundedReceiveTraining { get; set; } = false;

        /// <summary>
        /// Whether upgradeable troops receive training xp.
        /// </summary>
        [XmlElement]
        [SettingProperty("TEST", "test")]
        public bool UpgradeableReceiveTraining { get; set; } = true;

        /// <summary>
        /// A multiplier for all xp given to player-owned garrisons by this mod.
        /// </summary>
        [XmlElement]
        [SettingProperty("TEST", 0, 9999, "test")]
        public float PlayerClanGarrisonTrainingXpMultiplier { get; set; } = 1.0f;

        /// <summary>
        /// A multiplier for all xp given to non-player-owned garrisons by this mod.
        /// </summary>
        [XmlElement]
        [SettingProperty("TEST", 0, 9999, "test")]
        public float NonPlayerClanGarrisonTrainingXpMultiplier { get; set; } = 1.0f;
        
    }
}
