namespace TrainingTweak.Settings;

public class TrainingPerkSettings
{
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
}