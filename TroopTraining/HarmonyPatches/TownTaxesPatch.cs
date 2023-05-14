using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;

namespace TrainingTweak.HarmonyPatches;

public class TownTaxesPatch : HarmonyPatch
{
   private static bool Faulted { get; set; }
   
   protected override MethodInfo OriginalMethod => TaxModel
      .GetType().GetMethod(nameof(TaxModel.CalculateTownTax))
      ?.DeclaringType?.GetMethod(nameof(TaxModel.CalculateTownTax));
   protected override MethodInfo Postfix =>
      GetType().GetMethod(nameof(CalculateTownTaxPostfix));
   
   private SettlementTaxModel TaxModel => Campaign.Current.Models.SettlementTaxModel;
   
   public TownTaxesPatch(Harmony harmony) : base(harmony) { }

   private static void CalculateTownTaxPostfix(Town town, ref int __result)
   {
      if (Faulted) return;
      
      try
      {
         if (!Settings.Instance.EnableFinancialSolutions) return;
         
         float multiplier = town?.Settlement?.OwnerClan == Clan.PlayerClan
            ? Settings.Instance.PlayerTownTaxIncomeMultiplier
            : Settings.Instance.NonPlayerTownTaxIncomeMultiplier;

         __result = (int)Math.Ceiling(__result * Math.Max(0, multiplier));
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