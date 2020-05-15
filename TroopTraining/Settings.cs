using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml.Serialization;

namespace TrainingTweak
{
    public class Settings
    {
        public const string InstanceID = "TrainingTweak_v1";
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

            set
            {
                _instance = value;
            }
        }

        [XmlElement]
        public float PlayerPartyTrainingXpMultiplier { get; set; } = 0.75f;

        [XmlElement]
        public float PlayerClanPartyTrainingXpMultiplier { get; set; } = 0.75f;

        [XmlElement]
        public float PlayerClanGarrisonTrainingXpMultiplier { get; set; } = 4.0f;

        [XmlElement]
        public float NonPlayerClanPartyTrainingXpMultiplier { get; set; } = 0.75f;

        [XmlElement]
        public float NonPlayerClanGarrisonTrainingXpMultiplier { get; set; } = 4.0f;

        [XmlElement]
        public int RaiseTheMeekMaxTierTrained { get; set; } = 3;

        [XmlElement]
        public int GarrisonTrainingMaxTierTrained { get; set; } = 20;

        [XmlElement]
        public int AllTrainingMaxTierTrained { get; set; } = 20;

        [XmlElement]
        public float LevelDifferenceFactor { get; set; } = 6;

        [XmlElement]
        public float LeadershipSkillFactor { get; set; } = 0.4f;

        [XmlElement]
        public float TrainingXpPerLeadershipXp { get; set; } = 10.0f;

        [XmlElement]
        public bool WoundedReceiveTraining { get; set; } = false;

        [XmlElement]
        public bool UpgradeableReceiveTraining { get; set; } = true;
        [XmlElement]
        public bool DebugMode { get; set; } = false;

        [XmlElement]
        public bool EnableFinancialSolutions { get; set; } = true;

        [XmlElement]
        public float PlayerTownTaxIncomeMultiplier { get; set; } = 1.0f;

        [XmlElement]
        public float PlayerVillageTaxIncomeMultiplier { get; set; } = 1.0f;

        [XmlElement]
        public float NonPlayerTownTaxIncomeMultiplier { get; set; } = 3.0f;


        [XmlElement]
        public float NonPlayerVillageTaxIncomeMultiplier { get; set; } = 3.0f;

        [XmlElement]
        public float PlayerClanPartyWageMultiplier { get; set; } = 1.0f;

        [XmlElement]
        public float NonPlayerClanPartyWageMultiplier { get; set; } = 1.0f;

        [XmlElement]
        public float TroopUpgradeCostMultiplier { get; set; } = 1.0f;

        [XmlElement]
        public bool EnableBaseTraining { get; set; } = true;

        [XmlElement]
        public int BaseTrainingXpGain { get; set; } = 5;

        [XmlElement]
        public int BaseTrainingMaxTierTrained { get; set; } = 1;
    }
}
