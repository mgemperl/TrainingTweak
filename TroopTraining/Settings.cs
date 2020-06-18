namespace TrainingTweak
{
    public class Settings
    {
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

            set => _instance = value ?? new Settings();
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
        public int BaseTrainingXpAmount { get; set; } = 4;
        public int BaseTrainingMaxTierTrained { get; set; } = 1;

        // Garrison Settings 
        public bool EnableGarrisonTraining { get; set; } = true;
        public int LevelOneTrainingFieldXpAmount { get; set; } = 4;
        public int GarrisonTrainingMaxTierTrained { get; set; } = 20;
        public float PlayerClanGarrisonTrainingXpMultiplier { get; set; } = 1.0f;
        public float NonPlayerClanGarrisonTrainingXpMultiplier { get; set; } = 1.0f;

        // Financial Solutions Settings
        public bool EnableFinancialSolutions { get; set; } = true;
        public float PlayerTownTaxIncomeMultiplier { get; set; } = 2.0f;
        public float PlayerVillageTaxIncomeMultiplier { get; set; } = 2.0f;
        public float NonPlayerTownTaxIncomeMultiplier { get; set; } = 2.5f;
        public float NonPlayerVillageTaxIncomeMultiplier { get; set; } = 2.5f;
        public float PlayerClanPartyWageMultiplier { get; set; } = 1.0f;
        public float NonPlayerClanPartyWageMultiplier { get; set; } = 1.0f;
        public float TroopUpgradeCostMultiplier { get; set; } = 1.0f;

        // Debug
        public bool DebugMode { get; set; } = false;
    }
}
