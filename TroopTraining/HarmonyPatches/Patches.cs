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
        private static bool _financialSolutionsDisabled = false;
        private static bool _disableNativeTrainingDisabled = false;

        /// <summary>
        /// Harmony postfix for town tax income.
        /// </summary>
        public static void CalculateTownTaxPostfix(Town town, ref int __result)
        {
            if (!_financialSolutionsDisabled)
            {
                try
                {
                    if (Settings.Instance.EnableFinancialSolutions)
                    {
                        float multiplier = (town?.Settlement?.OwnerClan == Clan.PlayerClan)
                            ? Settings.Instance.PlayerTownTaxIncomeMultiplier
                            : Settings.Instance.NonPlayerTownTaxIncomeMultiplier;

                        __result = (int)Math.Ceiling(__result * Math.Max(0, multiplier));
                    }
                }
                catch (Exception exc)
                {
                    _financialSolutionsDisabled = true;
                    Util.Warning(
                        $"{Strings.FinancialSolutionsPatchFailed}\n\n" +
                        $"{Strings.FatalErrorDisclaimer}",
                        exc: exc);
                }
            }
        }

        /// <summary>
        /// Harmony postfix for village tax income
        /// </summary>
        public static void CalculateVillageTaxPostfix(Village village, ref int __result)
        {
            if (!_financialSolutionsDisabled)
            {
                try
                {
                        if (Settings.Instance.EnableFinancialSolutions)
                        {
                            float multiplier = (village?.Settlement?.OwnerClan == Clan.PlayerClan)
                                ? Settings.Instance.PlayerVillageTaxIncomeMultiplier
                                : Settings.Instance.NonPlayerVillageTaxIncomeMultiplier;

                            __result = (int)Math.Ceiling(__result * Math.Max(0, multiplier));
                        }
                }
                catch (Exception exc)
                {
                    _financialSolutionsDisabled = true;
                    Util.Warning(
                        $"{Strings.FinancialSolutionsPatchFailed}\n\n" +
                        $"{Strings.FatalErrorDisclaimer}",
                        exc: exc);
                }
            }
        }

        /// <summary>
        /// Harmony postfix for total party wages.
        /// </summary>
        public static void PartyWagePostfix(MobileParty mobileParty, ref int __result)
        {
            if (!_financialSolutionsDisabled)
            {
                try
                {
                    if (Settings.Instance.EnableFinancialSolutions)
                    {
                        float multiplier = (mobileParty.Party.Owner.Clan == Clan.PlayerClan)
                            ? Settings.Instance.PlayerClanPartyWageMultiplier
                            : Settings.Instance.NonPlayerClanPartyWageMultiplier;

                        __result = (int)Math.Round(__result * Math.Max(0, multiplier));
                    }
                }
                catch (Exception exc)
                {
                    _financialSolutionsDisabled = true;
                    Util.Warning(
                        $"{Strings.FinancialSolutionsPatchFailed}\n\n" +
                        $"{Strings.FatalErrorDisclaimer}",
                        exc: exc);
                }
            }
        }

        /// <summary>
        /// Postfix for troop upgrade costs.
        /// </summary>
        public static void UpgradeCostPostfix(ref int __result)
        {
            if (!_financialSolutionsDisabled)
            {
                try
                {
                        if (Settings.Instance.EnableFinancialSolutions)
                        {
                            __result = (int)Math.Round(__result
                                * Math.Max(0, Settings.Instance.TroopUpgradeCostMultiplier));
                        }
                }
                catch (Exception exc)
                {
                    _financialSolutionsDisabled = true;
                    Util.Warning(
                        $"{Strings.FinancialSolutionsPatchFailed}\n\n" +
                        $"{Strings.FatalErrorDisclaimer}",
                        exc: exc);
                }
            }
        }

        /// <summary>
        /// Postfix for disabling native party training
        /// </summary>
        public static void GetPerkExperiencesForTroopsPostfix(
            PerkObject perk, ref int __result)
        {
            if (!_disableNativeTrainingDisabled)
            {
                try
                {
                    if (Settings.Instance.EnableTrainingPerkOverrides
                        && (perk == DefaultPerks.Leadership.RaiseTheMeek
                            || perk == DefaultPerks.Leadership.CombatTips))
                    {
                        __result = 0;
                    }
                }
                catch (Exception exc)
                {
                    _disableNativeTrainingDisabled = true;
                    Util.Warning(
                        $"{Strings.DisableNativeTrainingPatchFailed}\n\n" +
                        $"{Strings.FatalErrorDisclaimer}",
                        exc: exc);

                }
            }
        }

        /// <summary>
        /// Postfix for disabling native garrison training
        /// </summary>
        public static void CalculateDailyTroopXpBonusPostfix(ref int __result)
        {
            if (!_disableNativeTrainingDisabled)
            {
                try
                {
                    if (Settings.Instance.EnableGarrisonTraining)
                    {
                        __result = 0;
                    }
                }
                catch (Exception exc)
                {
                    _disableNativeTrainingDisabled = true;
                    Util.Warning(
                        $"{Strings.DisableNativeTrainingPatchFailed}\n\n" +
                        $"{Strings.FatalErrorDisclaimer}",
                        exc: exc);

                }
            }
        }
    } 
}
