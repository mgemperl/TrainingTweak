using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TrainingTweak;

public class Patches
{
	/// <summary>
	/// Harmony postfix for town tax income.
	/// </summary>
	public static void CalculateTownTaxPostfix(Town town, ref int __result)
	{
		float multiplier = (town?.Settlement?.OwnerClan == Clan.PlayerClan)
			? Settings.Instance.PlayerTownTaxIncomeMultiplier
			: Settings.Instance.NonPlayerTownTaxIncomeMultiplier;

		__result = (int)Math.Ceiling(__result * Math.Max(0, multiplier));
	}

	/// <summary>
	/// Harmony postfix for village tax income
	/// </summary>
	public static void CalculateVillageTaxPostfix(Village village, ref int __result)
	{
		float multiplier = (village?.Settlement?.OwnerClan == Clan.PlayerClan)
			? Settings.Instance.PlayerVillageTaxIncomeMultiplier
			: Settings.Instance.NonPlayerVillageTaxIncomeMultiplier;

		__result = (int)Math.Ceiling(__result * Math.Max(0, multiplier));
	}
}
