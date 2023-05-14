namespace TrainingTweak.Settings;

public class FinancialSettings
{
   public bool EnableFinancialSolutions { get; set; } = true;
   public float PlayerTownTaxIncomeMultiplier { get; set; } = 2.0f;
   public float PlayerVillageTaxIncomeMultiplier { get; set; } = 2.0f;
   public float NonPlayerTownTaxIncomeMultiplier { get; set; } = 2.5f;
   public float NonPlayerVillageTaxIncomeMultiplier { get; set; } = 2.5f;
   public float PlayerClanPartyWageMultiplier { get; set; } = 1.0f;
   public float NonPlayerClanPartyWageMultiplier { get; set; } = 1.0f;
   public float TroopUpgradeCostMultiplier { get; set; } = 1.0f; 
}