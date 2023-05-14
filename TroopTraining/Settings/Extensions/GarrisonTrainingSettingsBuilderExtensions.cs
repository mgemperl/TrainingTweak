using MCM.Abstractions.FluentBuilder;
using MCM.Common;

namespace TrainingTweak.Settings.Extensions;

public static class GarrisonTrainingSettingsBuilderExtensions
{
   public static ISettingsBuilder AddGarrisonTrainingSettingsGroupToggle(
      this ISettingsBuilder settingsBuilder, GarrisonTrainingSettings instance, int order) =>
      settingsBuilder.CreateGroup($"{Strings.GarrisonGroup}",
         groupBuilder => groupBuilder
            .SetGroupOrder(order)
            .AddToggle(nameof(GarrisonTrainingSettings.EnableGarrisonTraining),
               Strings.GarrisonGroup,
               new PropertyRef(typeof(ModSettings).GetProperty(
                  nameof(GarrisonTrainingSettings.EnableGarrisonTraining))!, instance),
               builder => builder
                  .SetRequireRestart(false)
                  .SetHintText(Strings.EnableGarrisonTrainingOverrideHint))
      );
   
   public static ISettingsBuilder AddGarrisonTrainingSettingsSubGroup(
      this ISettingsBuilder settingsBuilder, GarrisonTrainingSettings instance, int order) =>
   settingsBuilder.CreateGroup(
       $"{Strings.GarrisonGroup}/{Strings.GeneralSettingsGroup}",
            groupBuilder => groupBuilder
                .SetGroupOrder(order)
                .AddInteger(nameof(GarrisonTrainingSettings.LevelOneTrainingFieldXpAmount),
                    Strings.LevelOneTrainingFieldXpAmountDisplay, 0, 100,
                    new PropertyRef(typeof(ModSettings).GetProperty(
                        nameof(GarrisonTrainingSettings.LevelOneTrainingFieldXpAmount))!, 
                        instance),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetHintText(Strings.LevelOneTrainingFieldXpAmountHint))
                .AddInteger(nameof(GarrisonTrainingSettings.GarrisonTrainingMaxTierTrained),
                    Strings.GarrisonMaxTierTrainedDisplay, 0, 20,
                    new PropertyRef(typeof(ModSettings).GetProperty(
                        nameof(GarrisonTrainingSettings.GarrisonTrainingMaxTierTrained))!, 
                        instance),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetHintText(Strings.GarrisionMaxTierTrainedHint))
                .AddFloatingInteger(
                    nameof(GarrisonTrainingSettings.PlayerClanGarrisonTrainingXpMultiplier),
                    Strings.PlayerClanGarrisonTrainingMultDisplay, 0f, 20f,
                    new PropertyRef(typeof(ModSettings).GetProperty(
                        nameof(GarrisonTrainingSettings.PlayerClanGarrisonTrainingXpMultiplier))!, 
                        instance),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetHintText(Strings.PlayerClanGarrisonTrainingMultHint)
                        .AddValueFormat("#0%"))
                .AddFloatingInteger(
                    nameof(GarrisonTrainingSettings.NonPlayerClanGarrisonTrainingXpMultiplier),
                    Strings.NonPlayerClanGarrisonTrainingMultDisplay, 0f, 20f,
                    new PropertyRef(typeof(ModSettings).GetProperty(
                        nameof(GarrisonTrainingSettings.NonPlayerClanGarrisonTrainingXpMultiplier))!, 
                        instance),
                    builder => builder
                        .SetRequireRestart(false)
                        .SetHintText(Strings.NonPlayerClanGarrisonTrainingMultHint)
                        .AddValueFormat("#0%"))
        );
}