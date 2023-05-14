using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;

namespace TrainingTweak.HarmonyPatches;

public class TroopUpgradeCostPatch : HarmonyPatch
{
   private static bool Faulted { get; set; }
   protected override MethodInfo OriginalMethod => UpgradeModel
      .GetType().GetMethod(nameof(UpgradeModel.GetGoldCostForUpgrade))
      ?.DeclaringType?.GetMethod(nameof(UpgradeModel.GetGoldCostForUpgrade));
   protected override MethodInfo Postfix =>
      GetType().GetMethod(nameof(UpgradeCostPostfix));
   
   PartyTroopUpgradeModel UpgradeModel => Campaign.Current.Models.PartyTroopUpgradeModel;

   public TroopUpgradeCostPatch(Harmony harmony) : base(harmony) { }
   
   public static void UpgradeCostPostfix(ref int __result)
   {
      if (Faulted) return;
      
      try
      {
         if (!Settings.Instance.EnableFinancialSolutions) return;
         
         var multiplier = Settings.Instance.TroopUpgradeCostMultiplier;
         __result = (int)Math.Round(__result * Math.Max(0, multiplier));
      }
      catch (Exception exc)
      {
         Faulted = true;
         Settings.Instance.EnableFinancialSolutions = false;
         Util.Warning(
            $"{Strings.FinancialSolutionsPatchFailed}\n\n" +
            $"{Strings.FatalErrorDisclaimer}",
            exc: exc);
      }
   }
}