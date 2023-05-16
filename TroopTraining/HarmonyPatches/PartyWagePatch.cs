﻿using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Engine;
using TaleWorlds.Localization;
using TrainingTweak.Settings;
using HarmonyPostfix = TrainingTweak.HarmonyPatches.Base.HarmonyPostfix;

namespace TrainingTweak.HarmonyPatches;

public class PartyWagePatch : HarmonyPostfix
{
   private static bool Faulted { get; set; }
   private static FinancialSettings Settings => ModSettings.Instance.Financial;
   
   protected override MethodInfo? OriginalMethod => WageModel
      ?.GetType().GetMethod(nameof(WageModel.GetTotalWage))
      ?.DeclaringType?.GetMethod(nameof(WageModel.GetTotalWage));

   protected override MethodInfo? Patch => 
      GetType().GetMethod(nameof(PartyWagePostfix));
   
   private PartyWageModel? WageModel => Campaign.Current.Models.PartyWageModel;
   
   public PartyWagePatch(Harmony harmony) : base(harmony) { }
   
   public static void PartyWagePostfix(MobileParty mobileParty, 
      ref ExplainedNumber __result)
   {
      if (Faulted) return;
      
      try
      {
         if (!Settings.EnableFinancialSolutions) return;
         
         float multiplier = (mobileParty.Party.Owner.Clan == Clan.PlayerClan)
            ? Settings.PlayerClanPartyWageMultiplier
            : Settings.NonPlayerClanPartyWageMultiplier;

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