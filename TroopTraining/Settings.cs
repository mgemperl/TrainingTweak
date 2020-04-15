using System.Xml.Serialization;

namespace TrainingTweak
{
    public class Settings
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

        /// <summary>
        /// A multiplier for all xp awarded to player troops by this mod.
        /// </summary>
        [XmlElement]
        public double PlayerPartyTrainingXpMultiplier { get; set; } = 1.0;

        /// <summary>
        /// A multiplier for all training xp this mod gives to troops in parties not owned
        /// by the player.
        /// (This includes player-owned caravans)
        /// </summary>
        [XmlElement]
        public double PlayerClanPartyTrainingXpMultiplier { get; set; } = 1.0;

        /// <summary>
        /// A multiplier for all training xp this mod gives to troops in parties not owned
        /// by the player.
        /// (This includes player-owned caravans)
        /// </summary>
        [XmlElement]
        public double NonPlayerClanPartyTrainingXpMultiplier { get; set; } = 1.0;

        /// <summary>
        /// XP per troop for a hero without either training perk.
        /// </summary>
        [XmlElement]
        public int BaseTrainingXpGain { get; set; } = 5;

        /// <summary>
        /// Max tier trained by hero without either training perk.
        /// </summary>
        [XmlElement]
        public int BaseTrainingMaxTierTrained { get; set; } = 1;

        /// <summary>
        /// Max tier trained by Raise The Meek perk. 
        /// </summary>
        [XmlElement]
        public int RaiseTheMeekMaxTierTrained { get; set; } = 3;

        /// <summary>
        /// For every X levels the trainer is above the troop, the xp gain is increased by 100%.
        /// For example, if set to 5, a level 20 trainer training a level 10 troops will
        /// result in x3 experience gained.
        /// </summary>
        [XmlElement]
        public int LevelDifferenceMultiple { get; set; } = 4;

        /// <summary>
        /// How much xp a trainer has to train troops to get 1 leadership xp.
        /// </summary>
        [XmlElement]
        public double TrainingXpPerLeadershipXp { get; set; } = 20.0;
	
        /// <summary>
        /// Whether wounded troops receive training xp.
        /// </summary>
        [XmlElement]
        public bool WoundedReceiveTraining { get; set; } = false;

        /// <summary>
        /// Whether upgradeable troops receive training xp.
        /// </summary>
        [XmlElement]
        public bool UpgradeableReceiveTraining { get; set; } = true;

        /// <summary>
        /// A multiplier for all xp given to player-owned garrisons by this mod.
        /// </summary>
        [XmlElement]
        public double PlayerClanGarrisonTrainingXpMultiplier { get; set; } = 1.0;

        /// <summary>
        /// A multiplier for all xp given to non-player-owned garrisons by this mod.
        /// </summary>
        [XmlElement]
        public double NonPlayerClanGarrisonTrainingXpMultiplier { get; set; } = 1.0;
    }
}
