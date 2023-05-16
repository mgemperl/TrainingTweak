using System;
using MCM.Abstractions.Base.Global;
using MCM.Abstractions.FluentBuilder;
using TrainingTweak.Settings.Extensions;

namespace TrainingTweak.Settings;

public class ModSettings : IDisposable
{
    public const string InstanceID = "TrainingTweak_v4";

    private FluentGlobalSettings _settings;
    private static ModSettings _instance;

    public static ModSettings Instance => _instance ??= _instance = new ModSettings(); 
    
    public TrainingPerkSettings TrainingPerk { get; }
    public BaseTrainingSettings BaseTraining { get; }
    public GarrisonTrainingSettings GarrisonTraining { get; }
    public FinancialSettings Financial { get; }

    private ModSettings()
    {
        TrainingPerk = new();
        BaseTraining = new();
        GarrisonTraining = new();
        Financial = new();
    }
    
    public void RegisterSettings()
    {
        int order = 0;
        var settingsBuilder = BaseSettingsBuilder.Create(InstanceID, SubModule.ModName)!
            .SetFormat("xml")
            .SetFolderName(SubModule.ModuleFolderName);
            
        // Add training perk settings
        settingsBuilder
            .AddTrainingPerkSettingsGroupToggle(TrainingPerk, order++)
            .AddTrainingPerkXpAmountSettingsSubGroup(TrainingPerk, order++)
            .AddTrainingPerkTierLimitsSettingsSubGroup(TrainingPerk, order++)
            .AddTrainingPerkXpMultiplierSettingsSubGroup(TrainingPerk, order++)
            .AddTrainingPerkGeneralSettingsSubGroup(TrainingPerk, order++);
        
        // Add base training settings
        settingsBuilder
            .AddBaseTrainingSettingsGroupToggle(BaseTraining, order++)
            .AddBaseTrainingSettingsSubGroup(BaseTraining, order++);
        
        // Add garrison training settings
        settingsBuilder
            .AddGarrisonTrainingSettingsGroupToggle(GarrisonTraining, order++)
            .AddGarrisonTrainingSettingsSubGroup(GarrisonTraining, order++);
      
        // Add Financial Solutions
        settingsBuilder
            .AddFinancialSolutionsSettingsGroupToggle(Financial, order++)
            .AddFinancialSolutionsPlayerSettingsSubGroup(Financial, order++)
            .AddFinancialSolutionsAISettingsSubGroup(Financial, order++)
            .AddFinancialSolutionsGeneralSettingsSubGroup(Financial, order++);

        _settings = settingsBuilder.BuildAsGlobal();
        _settings.Register();
    }

    public void Dispose()
    {
        try
        {
            //_settings.Unregister();
        }
        catch (Exception) { }
    }
}