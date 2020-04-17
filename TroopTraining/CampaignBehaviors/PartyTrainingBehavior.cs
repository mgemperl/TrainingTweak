using System;
using System.Linq;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace TrainingTweak.CampaignBehaviors
{
    public class PartyTrainingBehavior : CampaignBehaviorBase
    {
        private static bool _disabled = false;

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(
                this, SafeHandleDailyTraining);
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        private void SafeHandleDailyTraining(MobileParty party)
        {
            try
            {
                if (!_disabled)
                {
                    HandleDailyTraining(party);
                }
            }
            catch (Exception exc)
            {
                _disabled = true;
                Util.Warning("Training Tweak has encountered an error and is " +
                    "stopping.\n\nYou may continue playing without Training Tweak, " +
                    "but your game state may already be corrupted.",
                    exc: exc, showDisclaimer: true);
            }
        }

        private void HandleDailyTraining(MobileParty party)
        {
            // If party or member list is invalid, do nothing
            if (party?.MemberRoster == null || !party.IsActive)
            {
                return;
            }

            // If it is the player's party
            if (party.IsMainParty)
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
            // Otherwise, if it is a lord party or a player-owned caravan
            else if (party.IsLordParty || (party.IsCaravan && party.Party?.Owner == Hero.MainHero))
            {
                // Get multiplier for this party
                float multiplier = (party.Party?.Owner == Hero.MainHero)
                    ? Settings.Instance.PlayerClanPartyTrainingXpMultiplier
                    : Settings.Instance.NonPlayerClanPartyTrainingXpMultiplier;

                // If configured to train this party
                if(multiplier > 0)
                {
                    // Train AI party
                    TrainParty(party, multiplier);
                }
            }
            // Otherwise, if it is a garrison
            else if (party.IsGarrison)
            {
                // Get multiplier for this garrison
                float multiplier = (party.Party?.Owner == Hero.MainHero)
                    ? Settings.Instance.PlayerClanGarrisonTrainingXpMultiplier
                    : Settings.Instance.NonPlayerClanGarrisonTrainingXpMultiplier;

                // If configured to train this garrison
                if(multiplier > 0)
                {
                    // Train AI party
                    TrainGarrison(party, multiplier);
                }
            }
        }

        private void TrainGarrison (MobileParty party, float multiplier)
        {
            // Check if garrison's town doesn't exist, or if a null is lurking amongst us
            if (party.CurrentSettlement?.Town == null
                || party.MemberRoster.Contains(null))
            {
                return;
            }

            // Get base xp gain for this garrison
            Town town = party.CurrentSettlement.Town;
            var trainingModel = Campaign.Current.Models.DailyTroopXpBonusModel;
            float xpPerTroop = trainingModel.CalculateDailyTroopXpBonus(town)
                * trainingModel.CalculateGarrisonXpBonusMultiplier(town)
                * multiplier;

            // If training this garrison, and garrison has member list
            if (xpPerTroop > 0 && party.MemberRoster != null)
            {
                var members = party.MemberRoster;

                // For each group in the garrison
                for (int idx = 0; idx < members.Count; idx++)
                {
                    int numInGroup = members.GetElementNumber(idx);
                    int numUpgradeable = members.GetElementCopyAtIndex(idx)
                        .NumberReadyToUpgrade;

                    // If there are troops in group to train
                    if (numUpgradeable < numInGroup)
                    {
                        int numNotTrained = 0;
                        // Remove wounded from training if configured to
                        if (!Settings.Instance.WoundedReceiveTraining)
                        {
                            numNotTrained = members.GetElementWoundedNumber(idx);
                        }
                        // Remove upgradeable from training if configured to
                        if (!Settings.Instance.UpgradeableReceiveTraining)
                        {
                            // Allow wounded troops to be considered the upgradeable ones
                            numNotTrained = Math.Max(numNotTrained, numUpgradeable);
                        }

                        int xpForCurGroup = (int)Math.Ceiling(
                            (numInGroup - numNotTrained) * xpPerTroop);
                        // Add xp to current troop group
                        if (xpForCurGroup > 0)
                        {
                            members.AddXpToTroopAtIndex(xpForCurGroup, idx);
                        }
                    }
                }
            }
        }

        private int TrainParty(MobileParty party, float multiplier)
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
                    totalXp += ExecuteHeroDailyTraining(hero, party, 
                        baseXpGain * multiplier,
                        Settings.Instance.RaiseTheMeekMaxTierTrained);
                }

                // If hero has combat tips perk
                if (hero.GetPerkValue(DefaultPerks.Leadership.CombatTips))
                {
                    baseXpGain = Campaign.Current.Models.PartyTrainingModel
                        .GetTroopPerksXp(DefaultPerks.Leadership.CombatTips);
                    totalXp += ExecuteHeroDailyTraining(hero, party, 
                        baseXpGain * multiplier,
                        int.MaxValue);
                    
                }

                // If Hero has neither perk
                if (!hero.GetPerkValue(DefaultPerks.Leadership.RaiseTheMeek)
                    && !hero.GetPerkValue(DefaultPerks.Leadership.CombatTips))
                {
                    baseXpGain = Settings.Instance.BaseTrainingXpGain;
                    totalXp += ExecuteHeroDailyTraining(hero, party, 
                        baseXpGain * multiplier,
                        Settings.Instance.BaseTrainingMaxTierTrained);
                }
            }

            return totalXp;
        }

        /// <summary>
        /// Apply hero's training onto their party.
        /// </summary>
        private static int ExecuteHeroDailyTraining(Hero hero, MobileParty party,
            float baseXpGain, int maxTierTrained)
        {
            // If configured not to do this training, or there's a null lurking
            if (baseXpGain <= 0 || maxTierTrained <= 0
                || party.MemberRoster.Contains(null))
            {
                return 0;
            }

            var partyMembers = party.MemberRoster;
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
                        float levelDiffMult = 1.0f;
                        if (Settings.Instance.LevelDifferenceMultiple >= 1.0)
                        {
                            levelDiffMult = 1.0f + Math.Max(0, (hero.Level - curMember.Level))
                                / (float)Settings.Instance.LevelDifferenceMultiple;
                        }
                        float xpPerTroop = levelDiffMult * baseXpGain;
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
                    (int)Math.Ceiling((float)totalXp
                        / Settings.Instance.TrainingXpPerLeadershipXp));
            }

            return totalXp;
        }
    }
}
