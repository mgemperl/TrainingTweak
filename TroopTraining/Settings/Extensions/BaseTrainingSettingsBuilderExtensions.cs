using MCM.Abstractions.FluentBuilder;
using MCM.Common;

namespace TrainingTweak.Settings.Extensions;

public static class BaseTrainingSettingsBuilderExtensions
{
   public static ISettingsBuilder AddBaseTrainingSettingsGroupToggle(
      this ISettingsBuilder settingsBuilder, BaseTrainingSettings instance, int order) =>
      settingsBuilder.CreateGroup(
         $"{Strings.BaseTrainingSettingsGroup}",
         groupBuilder => groupBuilder
            .SetGroupOrder(order)
            .AddToggle(nameof(BaseTrainingSettings.EnableBaseTraining),
               Strings.BaseTrainingSettingsGroup,
               new PropertyRef(typeof(BaseTrainingSettings).GetProperty(
                  nameof(BaseTrainingSettings.EnableBaseTraining))!, instance),
               builder => builder
                  .SetRequireRestart(false)
                  .SetHintText(Strings.EnableBaseTrainingHint))
      );
   
   public static ISettingsBuilder AddBaseTrainingSettingsSubGroup(
      this ISettingsBuilder settingsBuilder, BaseTrainingSettings instance, int order) =>
      settingsBuilder.CreateGroup(
         $"{Strings.BaseTrainingSettingsGroup}/{Strings.GeneralSettingsGroup}",
         groupBuilder => groupBuilder
            .SetGroupOrder(order)
            .AddInteger(nameof(BaseTrainingSettings.BaseTrainingXpAmount),
               Strings.BaseTrainingXpAmountDisplay, 0, 100,
               new PropertyRef(typeof(ModSettings).GetProperty(
                  nameof(BaseTrainingSettings.BaseTrainingXpAmount))!, instance),
               builder => builder
                  .SetRequireRestart(false)
                  .SetHintText(Strings.BaseTrainingXpAmountHint))
            .AddInteger(nameof(BaseTrainingSettings.BaseTrainingMaxTierTrained),
               Strings.BaseTrainingTierDisplay, 0, 20,
               new PropertyRef(typeof(ModSettings).GetProperty(
                  nameof(BaseTrainingSettings.BaseTrainingMaxTierTrained))!, instance),
               builder => builder
                  .SetRequireRestart(false)
                  .SetHintText(Strings.BaseTrainingTierHint))
      );
}