using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TrainingTweak;

namespace TrainingTweak.HarmonyPatches
{
    public class Patches
    {
        /// <summary>
        /// Harmony postfix for town tax income.
        /// </summary>
        public static void CalculateTownTaxPostfix(Town town, ref int __result)
        {
            if (Settings.Instance.EnableFinancialSolutions)
            {
                float multiplier = (town?.Settlement?.OwnerClan == Clan.PlayerClan)
                    ? Settings.Instance.PlayerTownTaxIncomeMultiplier
                    : Settings.Instance.NonPlayerTownTaxIncomeMultiplier;

                __result = (int)Math.Ceiling(__result * Math.Max(0, multiplier));
            }
        }

        /// <summary>
        /// Harmony postfix for village tax income
        /// </summary>
        public static void CalculateVillageTaxPostfix(Village village, ref int __result)
        {
            if (Settings.Instance.EnableFinancialSolutions)
            {
                float multiplier = (village?.Settlement?.OwnerClan == Clan.PlayerClan)
                    ? Settings.Instance.PlayerVillageTaxIncomeMultiplier
                    : Settings.Instance.NonPlayerVillageTaxIncomeMultiplier;

                __result = (int)Math.Ceiling(__result * Math.Max(0, multiplier));
            }
        }

        /// <summary>
        /// Harmony postfix for total party wages.
        /// </summary>
        public static void PartyWagePostfix(MobileParty mobileParty, ref int __result)
        {
            if (Settings.Instance.EnableFinancialSolutions)
            {
                float multiplier = (mobileParty.Party.Owner.Clan == Clan.PlayerClan)
                    ? Settings.Instance.PlayerClanPartyWageMultiplier
                    : Settings.Instance.NonPlayerClanPartyWageMultiplier;

                __result = (int)Math.Round(__result * Math.Max(0, multiplier));
            }
        }

        /// <summary>
        /// Postfix for troop upgrade costs.
        /// </summary>
        public static void UpgradeCostPostfix(ref int __result)
        {
            if (Settings.Instance.EnableFinancialSolutions)
            {
                __result = (int)Math.Round(__result
                    * Math.Max(0, Settings.Instance.TroopUpgradeCostMultiplier));
            }
        }
    } 
}
