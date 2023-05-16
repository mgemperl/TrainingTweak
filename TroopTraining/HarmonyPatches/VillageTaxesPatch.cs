using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TrainingTweak.Settings;
using HarmonyPostfix = TrainingTweak.HarmonyPatches.Base.HarmonyPostfix;

namespace TrainingTweak.HarmonyPatches;

public class VillageTaxesPatch : HarmonyPostfix
{
   private static bool Faulted { get; set; }
   private static FinancialSettings Settings => ModSettings.Instance.Financial;
   
   protected override MethodInfo OriginalMethod => TaxModel.GetType()
      .GetMethod(nameof(TaxModel.CalculateVillageTaxFromIncome))
      ?.DeclaringType?.GetMethod(nameof(TaxModel.CalculateVillageTaxFromIncome));

   protected override MethodInfo Patch =>
      GetType().GetMethod(nameof(CalculateVillageTaxPostfix));
   
   private SettlementTaxModel TaxModel => Campaign.Current.Models.SettlementTaxModel;
   
   public VillageTaxesPatch(Harmony harmony) : base(harmony) { }
   
   private static void CalculateVillageTaxPostfix(Village village, ref int __result)
   {
      if (Faulted) return;
      
      try
      {
         if (!Settings.EnableFinancialSolutions) return;
         
         float multiplier = (village?.Settlement?.OwnerClan == Clan.PlayerClan)
            ? Settings.PlayerVillageTaxIncomeMultiplier
            : Settings.NonPlayerVillageTaxIncomeMultiplier;

         __result = (int)Math.Ceiling(__result * Math.Max(0, multiplier));
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