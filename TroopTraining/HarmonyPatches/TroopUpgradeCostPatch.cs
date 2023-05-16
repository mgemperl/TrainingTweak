using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Localization;
using TrainingTweak.Settings;
using HarmonyPostfix = TrainingTweak.HarmonyPatches.Base.HarmonyPostfix;

namespace TrainingTweak.HarmonyPatches;

public class TroopUpgradeCostPatch : HarmonyPostfix
{
   private static bool Faulted { get; set; }
   private static FinancialSettings Settings => ModSettings.Instance.Financial;
   protected override MethodInfo OriginalMethod => UpgradeModel
      .GetType().GetMethod(nameof(UpgradeModel.GetGoldCostForUpgrade))
      ?.DeclaringType?.GetMethod(nameof(UpgradeModel.GetGoldCostForUpgrade));
   protected override MethodInfo Patch =>
      GetType().GetMethod(nameof(UpgradeCostPostfix));
   
   PartyTroopUpgradeModel UpgradeModel => Campaign.Current.Models.PartyTroopUpgradeModel;

   public TroopUpgradeCostPatch(Harmony harmony) : base(harmony) { }
   
   public static void UpgradeCostPostfix(ref ExplainedNumber __result)
   {
      if (Faulted) return;
      
      try
      {
         if (!Settings.EnableFinancialSolutions) return;
         
         var multiplier = Settings.TroopUpgradeCostMultiplier;
         var native = __result.ResultNumber;
         var target = (int)Math.Round(native * Math.Max(0, multiplier));
         var diff = target - native;
         __result.Add(diff, new TextObject("Applying TrainingTweak multiplier"));
      }
      catch (Exception exc)
      {
         Faulted = true;
         Settings.EnableFinancialSolutions = false;
         Util.Warning(
            $"{Strings.FinancialSolutionsPatchFailed}\n\n" +
            $"{Strings.FatalErrorDisclaimer}",
            exc: exc);
      }
   }
}