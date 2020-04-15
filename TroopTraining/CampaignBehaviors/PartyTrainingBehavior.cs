using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace TrainingTweak.CampaignBehaviors
{
    public class PartyTrainingBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(
                this, TrainParty);
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        private void TrainParty(MobileParty party)
        {
            // If party or member list is null, do nothing
            if (party?.MemberRoster == null)
            {
                return;
            }

            // If it is player party
            if (party == Hero.MainHero.PartyBelongedTo)
            {
                // If configured to train player party
                if (Settings.Instance.PlayerPartyTrainingXpMultiplier > 0)
                {
                    // Train player party
                    int totalXp = TrainParty(party,
                        Settings.Instance.PlayerPartyTrainingXpMultiplier);

                    // If troops were trained
                    if (totalXp > 0)
                    {
                        // Display training results to player
                        InformationManager.DisplayMessage(new InformationMessage(
                            $"Total xp gained from training: {totalXp}"));

                        // If there are troops ready to upgrade
                        if (!party.MemberRoster.Where(elem => elem.NumberReadyToUpgrade > 0)
                            .IsEmpty())
                        {
                            // Inform player
                            InformationManager.DisplayMessage(new InformationMessage(
                                "Some troops are ready to upgrade."));
                        }
                    }
                }
            }
            // If it is a party owned by player
            else if(party.Party?.Owner == Hero.MainHero)
            {
                // If configured to train player clan parties
                if (Settings.Instance.PlayerClanPartyTrainingXpMultiplier > 0)
                {
                    // Train player clan party
                    int totalXp = TrainParty(party,
                        Settings.Instance.PlayerClanPartyTrainingXpMultiplier);
                }
            }
            // Is a non-player AI party
            else
            {
                // If configured to train AI parties
                if (Settings.Instance.NonPlayerClanPartyTrainingXpMultiplier > 0)
                {
                    // Train AI party
                    int totalXp = TrainParty(party,
                        Settings.Instance.NonPlayerClanPartyTrainingXpMultiplier);
                }
            }
        }

        private int TrainParty(MobileParty party, double multiplier)
        {
            int totalXp = 0;
            // For each hero in the party
            foreach (var member in party.MemberRoster
                .Where(member => member.Character.IsHero))
            {
                Hero hero = member.Character.HeroObject;
                int baseXpGain;

                // If hero has raise the meek perk
                if (hero.GetPerkValue(DefaultPerks.Leadership.RaiseTheMeek))
                {
                    baseXpGain = Campaign.Current.Models.PartyTrainingModel
                        .GetTroopPerksXp(DefaultPerks.Leadership.RaiseTheMeek);
                    totalXp += ExecuteDailyTraining(hero, baseXpGain * multiplier,
                        Settings.Instance.RaiseTheMeekMaxTierTrained);
                }
                // Otherwise, if hero has combat tips perk
                else if (hero.GetPerkValue(DefaultPerks.Leadership.CombatTips))
                {
                    baseXpGain = Campaign.Current.Models.PartyTrainingModel
                        .GetTroopPerksXp(DefaultPerks.Leadership.CombatTips);
                    totalXp += ExecuteDailyTraining(hero, baseXpGain * multiplier,
                        int.MaxValue);
                    
                }
                // Hero has neither perk
                else
                {
                    baseXpGain = Settings.Instance.BaseTrainingXpGain;
                    totalXp += ExecuteDailyTraining(hero, baseXpGain * multiplier,
                        Settings.Instance.BaseTrainingMaxTierTrained);
                }
            }

            return totalXp;
        }

        /// <summary>
        /// Apply hero's training onto their party.
        /// </summary>
        private static int ExecuteDailyTraining(Hero hero, double baseXpGain,
            int maxTierTrained)
        {
            // If configured not to do this training
            if (baseXpGain <= 0 || maxTierTrained <= 0)
            {
                return 0;
            }

            var partyMembers = hero.PartyBelongedTo.MemberRoster;
            int totalXp = 0;

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
                    (int)Math.Ceiling((double)totalXp
                        / Settings.Instance.TrainingXpPerLeadershipXp));
            }

            return totalXp;
        }
    }
}
