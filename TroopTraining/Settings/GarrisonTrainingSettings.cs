namespace TrainingTweak.Settings;

public class GarrisonTrainingSettings
{
   public bool EnableGarrisonTraining { get; set; } = true;
   public int LevelOneTrainingFieldXpAmount { get; set; } = 4;
   public int GarrisonTrainingMaxTierTrained { get; set; } = 20;
   public float PlayerClanGarrisonTrainingXpMultiplier { get; set; } = 1.0f;
   public float NonPlayerClanGarrisonTrainingXpMultiplier { get; set; } = 1.0f;
}