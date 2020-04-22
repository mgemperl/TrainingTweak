using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TrainingTweak;

public class Patches
{
	[HarmonyPatch(typeof(SettlementTaxModel), "CalculateTownTax")]
	public static int CalculateTownTaxPostfix(ref int __result)
	{
		int multiplied = (int)Math.Ceiling(__result * 
			Math.Max(0, Settings.Instance.TownTaxIncomeMultiplier));
		return multiplied;
	}

	[HarmonyPatch(typeof(SettlementTaxModel), "CalculateVillageTaxFromIncome")]
	public static int CalculateVillageTaxPostfix(ref int __result)
	{
		int multiplied = (int)Math.Ceiling(__result * 
			Math.Max(0, Settings.Instance.VillageTaxIncomeMultiplier));
		return multiplied;
	}
}
