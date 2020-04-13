using Messages.FromBattleServer.ToBattleServerManager;
using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace TrainingTweak
{
    // TODO: Change this to a campaign behavior. It's better design-wise.
    public static class TrainingEventHandlers
    {
        /// <summary>
        /// Apply hero's training onto their party.
        /// </summary>
        private static int ExecuteDailyTraining(Hero hero, double xpMultiplier)
        {
            // If hero isn't in a party or party has no members, do nothing
            if (hero?.PartyBelongedTo?.MemberRoster == null)
            {
                return 0;
            }    

            var partyMembers = hero.PartyBelongedTo.MemberRoster;
            int totalXp = 0;
            double baseXpGain = 0;
            int maxTierTrained = 0;

            // If hero has Raise The Meek perk
            if (hero.GetPerkValue(DefaultPerks.Leadership.RaiseTheMeek))
            {
                baseXpGain = Campaign.Current.Models.PartyTrainingModel
                    .GetTroopPerksXp(DefaultPerks.Leadership.RaiseTheMeek);
                maxTierTrained = Math.Max(0, Settings.Instance.RaiseTheMeekMaxTierTrained);
            }
            // If hero has Combat Tips perk
            else if (hero.GetPerkValue(DefaultPerks.Leadership.CombatTips))
            {
                baseXpGain = Campaign.Current.Models.PartyTrainingModel
                    .GetTroopPerksXp(DefaultPerks.Leadership.CombatTips);
                maxTierTrained = int.MaxValue;
            }
            // If player has neither perk, do nothing
            else
            {
                return 0;
            }

            // Apply xp multiplier
            baseXpGain *= xpMultiplier;

            // For each group in the hero's party
            for (int idx = 0; idx < partyMembers.Count; idx++)
            {
                var curMember = partyMembers.GetCharacterAtIndex(idx);
                // If member is a troop, and low enough tier to be trained
                if (curMember.IsSoldier && curMember.Tier <= maxTierTrained)
                {
                    int numInGroup = partyMembers.GetElementNumber(idx);
                    int numUpgradeable = partyMembers.GetElementCopyAtIndex(idx)
                        .NumberReadyToUpgrade;

                    // If there are troops in group to train
                    if (numUpgradeable < numInGroup)
                    {
                        int numNotTrained = 0;
                        // Remove wounded from training if configured to
                        if (!Settings.Instance.WoundedReceiveTraining)
                        {
                            numNotTrained = partyMembers.GetElementWoundedNumber(idx);
                        }
                        // Remove upgradeable from training if configured to
                        if (!Settings.Instance.UpgradeableReceiveTraining)
                        {
                            // Allow wounded troops to be considered the upgradeable ones
                            numNotTrained = Math.Max(numNotTrained, numUpgradeable);
                        }

                        // Apply level difference multiplier
                        double levelDiffMult = 1.0;
                        if (Settings.Instance.LevelDifferenceMultiple >= 1.0)
                        {
                            levelDiffMult = 1.0 + Math.Max(0, (hero.Level - curMember.Level))
                                / (double)Settings.Instance.LevelDifferenceMultiple;
                        }
                        double xpPerTroop = levelDiffMult * baseXpGain;
                        int xpForCurGroup = (int)Math.Round(
                            (numInGroup - numNotTrained) * xpPerTroop);

                        // Add xp to current troop group
                        if (xpForCurGroup > 0)
                        {
                            partyMembers.AddXpToTroopAtIndex(xpForCurGroup, idx);
                            totalXp += xpForCurGroup;
                        }
                    }
                }
            }

            // If configured to give hero leadership xp
            if (Settings.Instance.TrainingXpPerLeadershipXp > 0
                && totalXp > 0)
            {
                // Give hero leadership xp
                hero.AddSkillXp(DefaultSkills.Leadership,
                    (int)Math.Ceiling((double) totalXp 
                        / Settings.Instance.TrainingXpPerLeadershipXp));
            }

            return totalXp;
        }

        /// <summary>
        /// Handler to be invoked each day on the daily tick event.
        /// </summary>
        public static void DailyTrainingTickHandler(MBCampaignEvent campaignEvent, 
            params object[] delegateParams)
        {
            int totalXp = 0;

            // If player character is in a party
            if (Hero.MainHero.PartyBelongedTo != null)
            {
                // Apply training for player character
                totalXp += ExecuteDailyTraining(Hero.MainHero,
                    Math.Max(0, Settings.Instance.PlayerPartyTrainingXpMultiplier));

                // Apply training for companions in player's party if configured to
                if (Settings.Instance.CompanionsInPartyGiveTraining
                    && Hero.MainHero.CompanionsInParty != null)
                {
                    // For each companion in player's party
                    foreach (Hero companion in Hero.MainHero.CompanionsInParty)
                    {
                        totalXp += ExecuteDailyTraining(companion,
                            Math.Max(0, Settings.Instance.PlayerPartyTrainingXpMultiplier));
                    }
                }
            }

            // If troops were trained
            if (totalXp > 0)
            {
                // Display training results to player
                InformationManager.DisplayMessage(new InformationMessage(
                    $"Total xp gained from training: {totalXp}"));

                // If there are troops ready to upgrade
                if (Hero.MainHero?.PartyBelongedTo?.MemberRoster != null
                    && !Hero.MainHero.PartyBelongedTo.MemberRoster
                        .Where(elem => elem.NumberReadyToUpgrade > 0)
                        .IsEmpty())
                {
                    // Inform player
                    InformationManager.DisplayMessage(new InformationMessage(
                        "Some troops are ready to upgrade."));
                }
            }

            // Apply to AI
            if (Settings.Instance.NonPlayerPartiesReceiveTraining)
            {
                // For all active parties that aren't the player party, 
                //     that exist, and that have members
                foreach (MobileParty party in Campaign.Current.MobileParties
                    .Where(party => party != Hero.MainHero.PartyBelongedTo 
                        && party?.MemberRoster != null
                        && party.IsActive))
                {
                    // For each hero in the party
                    foreach (var member in party.MemberRoster
                        .Where(member => member.Character.IsHero))
                    {
                        // Apply training for that hero's party
                        ExecuteDailyTraining(member.Character.HeroObject,
                            Math.Max(0, Settings.Instance.AIPartyTrainingXpMultiplier));
                    }
                }
            }
        }
    }
}
