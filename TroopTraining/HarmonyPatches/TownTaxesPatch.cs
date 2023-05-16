using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;
using TrainingTweak.Settings;
using HarmonyPostfix = TrainingTweak.HarmonyPatches.Base.HarmonyPostfix;

namespace TrainingTweak.HarmonyPatches;

public class TownTaxesPatch : HarmonyPostfix
{
   private static bool Faulted { get; set; }
   private static FinancialSettings Settings => ModSettings.Instance.Financial;
   
   protected override MethodInfo OriginalMethod => TaxModel
      .GetType().GetMethod(nameof(TaxModel.CalculateTownTax))
      ?.DeclaringType?.GetMethod(nameof(TaxModel.CalculateTownTax));
   protected override MethodInfo Patch =>
      GetType().GetMethod(nameof(CalculateTownTaxPostfix));
   
   private SettlementTaxModel TaxModel => Campaign.Current.Models.SettlementTaxModel;
   
   public TownTaxesPatch(Harmony harmony) : base(harmony) { }

   private static void CalculateTownTaxPostfix(Town town, ref ExplainedNumber __result)
   {
      if (Faulted) return;
      
      try
      {
         if (!Settings.EnableFinancialSolutions) return;
         
         float multiplier = town?.Settlement?.OwnerClan == Clan.PlayerClan
            ? Settings.PlayerTownTaxIncomeMultiplier
            : Settings.NonPlayerTownTaxIncomeMultiplier;

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