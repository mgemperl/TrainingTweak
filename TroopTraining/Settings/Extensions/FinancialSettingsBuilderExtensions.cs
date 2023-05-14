using MCM.Abstractions.FluentBuilder;
using MCM.Common;

namespace TrainingTweak.Settings.Extensions;

public static class FinancialSettingsBuilderExtensions
{
   
   public static ISettingsBuilder AddFinancialSolutionsSettingsGroupToggle(
      this ISettingsBuilder settingsBuilder, FinancialSettings instance, int order) =>
      settingsBuilder.CreateGroup($"{Strings.FinancialSolutionsSettingGroup}",
         groupBuilder => groupBuilder
            .SetGroupOrder(order)
            .AddToggle(nameof(FinancialSettings.EnableFinancialSolutions),
               Strings.FinancialSolutionsSettingGroup,
               new PropertyRef(typeof(ModSettings).GetProperty(
                  nameof(FinancialSettings.EnableFinancialSolutions))!, instance),
               builder => builder
                  .SetRequireRestart(false)
                  .SetHintText(Strings.EnableFinancialSolutionsHint))
      );
   
   public static ISettingsBuilder AddFinancialSolutionsPlayerSettingsSubGroup(
      this ISettingsBuilder settingsBuilder, FinancialSettings instance, int order) =>
      settingsBuilder.CreateGroup(
         $"{Strings.FinancialSolutionsSettingGroup}/{Strings.PlayerSubgroup}",
         groupBuilder => groupBuilder
            .SetGroupOrder(order)
            .AddFloatingInteger(nameof(FinancialSettings.PlayerTownTaxIncomeMultiplier),
               Strings.PlayerTownTaxIncomeMultiplierDisplay, 0f, 20f,
               new PropertyRef(typeof(ModSettings).GetProperty(
                  nameof(FinancialSettings.PlayerTownTaxIncomeMultiplier))!, instance),
               builder => builder
                  .SetRequireRestart(false)
                  .SetHintText(Strings.PlayerTownTaxIncomeMultiplierHint)
                  .AddValueFormat("#0%"))
            .AddFloatingInteger(nameof(FinancialSettings.PlayerVillageTaxIncomeMultiplier),
               Strings.PlayerVillageTaxIncomeMultiplierDisplay, 0f, 20f,
               new PropertyRef(typeof(ModSettings).GetProperty(
                  nameof(FinancialSettings.PlayerVillageTaxIncomeMultiplier))!, instance),
               builder => builder
                  .SetRequireRestart(false)
                  .SetHintText(Strings.PlayerVillageTaxIncomeMultiplierHint)
                  .AddValueFormat("#0%"))
            .AddFloatingInteger(nameof(FinancialSettings.PlayerClanPartyWageMultiplier),
               Strings.PlayerClanPartyWageMultiplierDisplay, 0f, 20f,
               new PropertyRef(typeof(ModSettings).GetProperty(
                  nameof(FinancialSettings.PlayerClanPartyWageMultiplier))!, instance),
               builder => builder
                  .SetRequireRestart(false)
                  .SetHintText(Strings.PlayerClanPartyWageMultiplierHint)
                  .AddValueFormat("#0%"))
      );
   
   public static ISettingsBuilder AddFinancialSolutionsAISettingsSubGroup(
      this ISettingsBuilder settingsBuilder, FinancialSettings instance, int order) =>
      settingsBuilder.CreateGroup(
         $"{Strings.FinancialSolutionsSettingGroup}/{Strings.AISubgroup}",
         groupBuilder => groupBuilder
            .SetGroupOrder(order)
            .AddFloatingInteger(nameof(FinancialSettings.NonPlayerTownTaxIncomeMultiplier),
               Strings.NonPlayerTownTaxIncomeMultiplierDisplay, 0f, 20f,
               new PropertyRef(typeof(ModSettings).GetProperty(
                  nameof(FinancialSettings.NonPlayerTownTaxIncomeMultiplier))!, instance),
               builder => builder
                  .SetRequireRestart(false)
                  .SetHintText(Strings.NonPlayerTownTaxIncomeMultiplierHint)
                  .AddValueFormat("#0%"))
            .AddFloatingInteger(nameof(FinancialSettings.NonPlayerVillageTaxIncomeMultiplier),
               Strings.NonPlayerVillageTaxIncomeMultiplierDisplay, 0f, 20f,
               new PropertyRef(typeof(ModSettings).GetProperty(
                  nameof(FinancialSettings.NonPlayerVillageTaxIncomeMultiplier))!, instance),
               builder => builder
                  .SetRequireRestart(false)
                  .SetHintText(Strings.NonPlayerVillageTaxIncomeMultiplierHint)
                  .AddValueFormat("#0%"))
            .AddFloatingInteger(nameof(FinancialSettings.NonPlayerClanPartyWageMultiplier),
               Strings.NonPlayerClanPartyWageMultiplierDisplay, 0f, 20f,
               new PropertyRef(typeof(ModSettings).GetProperty(
                  nameof(FinancialSettings.NonPlayerClanPartyWageMultiplier))!, instance),
               builder => builder
                  .SetRequireRestart(false)
                  .SetHintText(Strings.NonPlayerClanPartyWageMultiplierHint)
                  .AddValueFormat("#0%"))
      );
   
   public static ISettingsBuilder AddFinancialSolutionsGeneralSettingsSubGroup(
      this ISettingsBuilder settingsBuilder, FinancialSettings instance, int order) =>
      settingsBuilder.CreateGroup(
         $"{Strings.FinancialSolutionsSettingGroup}/{Strings.GeneralSettingsGroup}",
         groupBuilder => groupBuilder
            .SetGroupOrder(order)
            .AddFloatingInteger(nameof(FinancialSettings.TroopUpgradeCostMultiplier),
               Strings.TroopUpgradeCostMultiplierDisplay, 0f, 50f,
               new PropertyRef(typeof(ModSettings).GetProperty(
                  nameof(FinancialSettings.TroopUpgradeCostMultiplier))!, instance),
               builder => builder
                  .SetRequireRestart(false)
                  .SetHintText(Strings.TroopUpgradeCostMultiplierHint)
                  .AddValueFormat("#0%"))
      );

}