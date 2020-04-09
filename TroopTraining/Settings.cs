using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;

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
        /// A multiplier for all xp awarded by this mod.
        /// </summary>
        [XmlElement]
        public double TrainingXpMultiplier { get; set; } = 1.0;

        /// <summary>
        /// Max tier trained by Raise The Meek perk. 
        /// </summary>
        [XmlElement]
        public int RaiseTheMeekMaxTierTrained { get; set; } = 3;

        /// <summary>
        /// For every X levels the trainer is above the troop, the xp gain is increased by 100%.
        /// For example, if set to 5, a level 20 trainer training a level 10 troops will
       ///  result in x3 experience gained.
        /// </summary>
        [XmlElement]
        public int LevelDifferenceMultiple { get; set; } = 5;

        /// <summary>
        /// How much xp a trainer has to train troops to get 1 leadership xp.
        /// </summary>
        [XmlElement]
        public double TrainingXpPerLeadershipXp { get; set; } = 10.0;

        /// <summary>
        /// Whether wounded troops receive training xp.
        /// </summary>
        public bool WoundedReceiveTraining { get; set; } = false;

        /// <summary>
        /// Whether upgradeable troops receive training xp.
        /// </summary>
        public bool UpgradeableReceiveTraining { get; set; } = true;
    }
}
