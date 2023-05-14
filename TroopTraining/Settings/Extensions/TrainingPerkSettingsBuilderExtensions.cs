using MCM.Abstractions.FluentBuilder;
using MCM.Common;

namespace TrainingTweak.Settings.Extensions;

public static class TrainingPerkSettingsBuilderExtensions
{
   public static ISettingsBuilder AddTrainingPerkSettingsGroupToggle(
      this ISettingsBuilder settingsBuilder, TrainingPerkSettings instance, int order) =>
      settingsBuilder.CreateGroup(
         $"{Strings.TrainingPerkGroup}",
         groupBuilder => groupBuilder
            .SetGroupOrder(order)
            .AddToggle(
               id: nameof(TrainingPerkSettings.EnableTrainingPerkOverrides),
               name: Strings.TrainingPerkGroup,
               @ref: new PropertyRef(typeof(TrainingPerkSettings).GetProperty(
                  nameof(TrainingPerkSettings.EnableTrainingPerkOverrides))!, instance),
               builder: builder => builder
                  .SetRequireRestart(false)
                  .SetHintText(Strings.EnableTrainingPerkOverridesHint))
         
      );
   
   public static ISettingsBuilder AddTrainingPerkXpAmountSettingsSubGroup(
      this ISettingsBuilder settingsBuilder, TrainingPerkSettings instance, int order) =>
      settingsBuilder.CreateGroup(
         $"{Strings.TrainingPerkGroup}/{Strings.TrainingPerkXpGain}",
         groupBuilder => groupBuilder
            .SetGroupOrder(order)
            .AddInteger(nameof(TrainingPerkSettings.RaiseTheMeekXpAmount),
               Strings.RaiseTheMeekXpAmountDisplay, 0, 100,
               new PropertyRef(typeof(TrainingPerkSettings).GetProperty(
                  nameof(TrainingPerkSettings.RaiseTheMeekXpAmount))!, instance),
               builder => builder
                  .SetRequireRestart(false)
                  .SetHintText(Strings.RaiseTheMeekXpAmountHint))
            .AddInteger(nameof(TrainingPerkSettings.CombatTipsXpAmount),
               Strings.CombatTipsXpAmountDisplay, 0, 100,
               new PropertyRef(typeof(TrainingPerkSettings).GetProperty(
                  nameof(TrainingPerkSettings.CombatTipsXpAmount))!, instance),
               builder => builder
                  .SetRequireRestart(false)
                  .SetHintText(Strings.CombatTipsXpAmountHint))
      );
   
   public static ISettingsBuilder AddTrainingPerkTierLimitsSettingsSubGroup(
      this ISettingsBuilder settingsBuilder, TrainingPerkSettings instance, int order) =>
      settingsBuilder.CreateGroup(
         $"{Strings.TrainingPerkGroup}/{Strings.TierLimitsSettingsGroup}",
         groupBuilder => groupBuilder
            .SetGroupOrder(order)
            .AddInteger(nameof(TrainingPerkSettings.RaiseTheMeekMaxTierTrained),
               Strings.RaiseTheMeekMaxTierTrainedDisplay, 0, 20,
               new PropertyRef(typeof(TrainingPerkSettings).GetProperty(
                  nameof(TrainingPerkSettings.RaiseTheMeekMaxTierTrained))!, instance),
               intBuilder => intBuilder
                  .SetRequireRestart(false)
                  .SetHintText(Strings.RaiseTheMeekMaxTierTrainedHint))
            .AddInteger(nameof(TrainingPerkSettings.ComatTipsMaxTierTrained),
               Strings.CombatTipsMaxTierTrainedDisplay, 0, 20,
               new PropertyRef(typeof(TrainingPerkSettings).GetProperty(
                  nameof(TrainingPerkSettings.ComatTipsMaxTierTrained))!, instance),
               intBuilder => intBuilder
                  .SetRequireRestart(false)
                  .SetHintText(Strings.CombatTipsMaxTierTrainedHint))
      );
   
   public static ISettingsBuilder AddTrainingPerkXpMultiplierSettingsSubGroup(
      this ISettingsBuilder settingsBuilder, TrainingPerkSettings instance, int order) =>
      settingsBuilder.CreateGroup(
         $"{Strings.TrainingPerkGroup}/{Strings.XpMultipliersSettingsGroup}",
         groupBuilder => groupBuilder
            .SetGroupOrder(order)
            .AddFloatingInteger(nameof(TrainingPerkSettings.PlayerPartyTrainingXpMultiplier),
               Strings.PlayerPartyTrainingMultDisplay, 0f, 20f,
               new PropertyRef(typeof(ModSettings).GetProperty(
                  nameof(TrainingPerkSettings.PlayerPartyTrainingXpMultiplier))!, instance),
               builder => builder
                  .SetRequireRestart(false)
                  .SetHintText(Strings.PlayerPartyTrainingMultHint)
                  .AddValueFormat("#0%"))
            .AddFloatingInteger(nameof(TrainingPerkSettings.PlayerClanPartyTrainingXpMultiplier),
               Strings.PlayerClanPartyTrainingMultDisplay, 0f, 20f,
               new PropertyRef(typeof(ModSettings).GetProperty(
                  nameof(TrainingPerkSettings.PlayerClanPartyTrainingXpMultiplier))!, instance),
               builder => builder
                  .SetRequireRestart(false)
                  .SetHintText(Strings.PlayerClanPartyTrainingMultHint)
                  .AddValueFormat("#0%"))
            .AddFloatingInteger(nameof(TrainingPerkSettings.NonPlayerClanPartyTrainingXpMultiplier),
               Strings.NonPlayerClanPartyTrainingMultDisplay, 0f, 20f,
               new PropertyRef(typeof(ModSettings).GetProperty(
                  nameof(TrainingPerkSettings.NonPlayerClanPartyTrainingXpMultiplier))!, instance),
               builder => builder
                  .SetRequireRestart(false)
                  .SetHintText(Strings.NonPlayerClanPartyTrainingMultHint)
                  .AddValueFormat("#0%"))
      );
   
   public static ISettingsBuilder AddTrainingPerkGeneralSettingsSubGroup(
      this ISettingsBuilder settingsBuilder, TrainingPerkSettings instance, int order) =>
      settingsBuilder.CreateGroup(
         $"{Strings.TrainingPerkGroup}/{Strings.GeneralSettingsGroup}",
         groupBuilder => groupBuilder
             .SetGroupOrder(order)
             .AddFloatingInteger(nameof(TrainingPerkSettings.LevelDifferenceFactor),
                 Strings.LevelDifferenceFactorDisplay, 0f, 20f,
                 new PropertyRef(typeof(ModSettings).GetProperty(
                     nameof(TrainingPerkSettings.LevelDifferenceFactor))!, instance),
                 builder => builder
                     .SetRequireRestart(false)
                     .SetHintText(Strings.LevelDifferenceFactorHint))
             .AddFloatingInteger(nameof(TrainingPerkSettings.LeadershipSkillFactor),
                 Strings.LeadershipSkillFactorDisplay, 0f, 20f,
                 new PropertyRef(typeof(ModSettings).GetProperty(
                     nameof(TrainingPerkSettings.LeadershipSkillFactor))!, instance),
                 builder => builder
                     .SetRequireRestart(false)
                     .SetHintText(Strings.LeadershipSkillFactorHint))
             .AddFloatingInteger(nameof(TrainingPerkSettings.TrainingXpPerLeadershipXp),
                 Strings.TrainingXpPerLeadershipXpDisplay, 0f, 20f,
                 new PropertyRef(typeof(ModSettings).GetProperty(
                     nameof(TrainingPerkSettings.TrainingXpPerLeadershipXp))!, instance),
                 builder => builder
                     .SetRequireRestart(false)
                     .SetHintText(Strings.TrainingXpPerLeadershipXpHint))
             .AddBool(nameof(TrainingPerkSettings.WoundedReceiveTraining),
                 Strings.WoundedReceiveTrainingDisplay,
                 new PropertyRef(typeof(ModSettings).GetProperty(
                     nameof(TrainingPerkSettings.WoundedReceiveTraining))!, instance),
                 builder => builder
                     .SetRequireRestart(false)
                     .SetHintText(Strings.WoundedReceiveTrainingHint))
             .AddBool(nameof(TrainingPerkSettings.UpgradeableReceiveTraining),
                 Strings.UpgradeableReceiveTrainingDisplay,
                 new PropertyRef(typeof(ModSettings).GetProperty(
                     nameof(TrainingPerkSettings.UpgradeableReceiveTraining))!, instance),
                 builder => builder
                     .SetRequireRestart(false)
                     .SetHintText(Strings.UpgradeableReceiveTrainingHint))
        );
   
 
}