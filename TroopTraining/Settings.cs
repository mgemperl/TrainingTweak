using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }

        public int RaiseTheMeekMaxTierTrained { get; set; } = 3;

        /// <summary>
        /// For every X levels the hero is over the troop, the training xp gain for the troop is doubled.
        /// For example, if set to 5, a level 25 hero training a level 10 troop will result in x3 
        /// experience during training.
        /// </summary>
        public int LevelDifferenceMultiplierMultiple { get; set; } = 5;

        /// <summary>
        /// How many troop xp points gained through training to result in one
        /// leadership skill xp.
        /// </summary>
        public int TrainingXPToLeadershipXP { get; set; } = 0;
    }
}
