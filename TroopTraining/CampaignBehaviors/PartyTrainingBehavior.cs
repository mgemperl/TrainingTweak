using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace TrainingTweak.CampaignBehaviors
{
    public class PartyTrainingBehavior : CampaignBehaviorBase
    {
        private static bool _disabled = false;
        private static HashSet<string> _reported = new HashSet<string>();

        private const int NativeMaxRaiseTheMeekTier = 3;
        private const int NativeMaxCompatTipsTier = 5;

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
                Util.Warning(
                    $"{Strings.FatalErrorMessage}\n\n" +
                    $"{Strings.FatalErrorDisclaimer}",
                    exc: exc);
            }
        }

        private void HandleDailyTraining(MobileParty party)
        {
            if (party == null)
            {
                if (Settings.Instance.DebugMode 
                    && !_reported.Contains("null-party"))
                {
                    _reported.Add("null-party");
                    Util.DebugMessage($"{Strings.WarningMessageHeader}\n\n" +
                        $"{Strings.NullPartyDetected}" +
                        $"\n\n{Strings.WarningDisclaimer}" +
                        $"\n\n{Strings.DebugModeNote}");
                }
                return;
            }
            else if (!party.IsActive)
            {
                // Ignore inactive parties
                return;
            }
            else if (party.MemberRoster == null)
            {
                if (Settings.Instance.DebugMode
                    && !_reported.Contains($"{party.Name}-roster"))
                {
                    _reported.Add($"{party.Name}-roster");
                    Util.DebugMessage($"{Strings.WarningMessageHeader}\n\n" +
                        $"{Strings.NullMemberRosterDetected} {party.Name} " +
                        $"{Strings.PartyLeaderHeader} {party.Leader?.Name}" +
                        $"\n\n{Strings.WarningDisclaimer}" +
                        $"\n\n{Strings.DebugModeNote}");
                }
                return;
            }
            // If a character in the party is null
            else if (!party.MemberRoster
                .Where(elem => elem.Character == null).IsEmpty())
            {
                if (Settings.Instance.DebugMode
                    && !_reported.Contains($"{party.Name}-character"))
                {
                    _reported.Add($"{party.Name}-character");
                    Util.DebugMessage($"{Strings.WarningMessageHeader}\n\n" +
                        $"Detected null member character for party:\n{party.Name} " +
                        $"led by {party.Leader?.Name}" +
                        $"\n\n{Strings.WarningDisclaimer}" +
                        $"\n\n{Strings.DebugModeNote}");
                }
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
                        string xpGainedNotice = Strings.DailyTrainingMessage.Replace(
                            Strings.XpPlaceholder, totalXp.ToString());
                        InformationManager.DisplayMessage(
                            new InformationMessage(xpGainedNotice));

                        // If there are troops ready to upgrade
                        if (!party.MemberRoster.Where(elem => elem.NumberReadyToUpgrade > 0)
                            .IsEmpty())
                        {
                            // Inform player
                            InformationManager.DisplayMessage(new InformationMessage(
                                $"{Strings.UpgradesAvailableMessage}"));
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
                if (multiplier > 0)
                {
                    // Train AI party
                    TrainGarrison(party, multiplier);
                }
            }
        }

        private void TrainGarrison (MobileParty party, float multiplier)
        {
            // Check if garrison's town doesn't exist, or if a null is lurking amongst us
            if (party.CurrentSettlement?.Town == null)
            {
                if (Settings.Instance.DebugMode
                    && !_reported.Contains($"{party.Name}-town"))
                {
                    _reported.Add($"{party.Name}-town");
                    Util.DebugMessage($"{Strings.WarningMessageHeader}\n\n" +
                        $"Detected null town for garrison:\n{party.Name} " +
                        $"in settlement {party.CurrentSettlement?.Name}" +
                        $"\n\n{Strings.WarningDisclaimer}" +
                        $"\n\n{Strings.DebugModeNote}");
                }

                return;
            }

            // Get base xp gain for this garrison
            Town town = party.CurrentSettlement.Town;
            var trainingModel = Campaign.Current.Models.DailyTroopXpBonusModel;
            float xpPerTroop = trainingModel.CalculateDailyTroopXpBonus(town)
                * trainingModel.CalculateGarrisonXpBonusMultiplier(town)
                * multiplier;

            // If training this garrison, and garrison has member list
            if (xpPerTroop > 0)
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
                if (member.Character.HeroObject == null)
                {
                    if (Settings.Instance.DebugMode
                        && !_reported.Contains($"{party.Name}-heroObj"))
                    {
                        _reported.Add($"{party.Name}-heroObj");
                        Util.DebugMessage($"{Strings.WarningMessageHeader}\n\n" +
                            $"Detected hero with null hero object in party:\n" +
                            $"{party.Name} led by {party.Leader?.Name}\n" +
                            $"Character with null hero object: " +
                            $"{member.Character.Name}" +
                            $"\n\n{Strings.WarningDisclaimer}" +
                            $"\n\n{Strings.DebugModeNote}");
                    }

                    continue;
                }
                else if (member.Character.HeroObject.PartyBelongedTo != party)
                {
                    if (Settings.Instance.DebugMode
                        && !_reported.Contains($"{party.Name}-heroBelong"))
                    {
                        _reported.Add($"{party.Name}-heroBelong");
                        Util.DebugMessage($"{Strings.WarningMessageHeader}\n\n" +
                            $"Hero doesn't consider itself a member of its party.\n" +
                            $"Party: {party.Name} led by {party.Leader?.Name}\n" +
                            $"Hero: {member.Character.Name}" +
                            $"\n\n{Strings.WarningDisclaimer}" +
                            $"\n\n{Strings.DebugModeNote}");
                    }

                    // We can just ignore this issue ourselves, so no need to skip
                }

                Hero hero = member.Character.HeroObject;
                int baseXpGain;

                // If hero has raise the meek perk
                if (hero.GetPerkValue(DefaultPerks.Leadership.RaiseTheMeek))
                {
                    baseXpGain = Campaign.Current.Models.PartyTrainingModel
                        .GetTroopPerksXp(DefaultPerks.Leadership.RaiseTheMeek);
                    totalXp += ExecuteHeroDailyTraining(
                        hero: hero, 
                        party: party, 
                        baseXpGain: baseXpGain * multiplier, 
                        nativeXpGain: baseXpGain,
                        maxTierTrained: Settings.Instance.RaiseTheMeekMaxTierTrained,
                        nativeMaxTierTrained: NativeMaxRaiseTheMeekTier);
                }

                // If hero has combat tips perk
                if (hero.GetPerkValue(DefaultPerks.Leadership.CombatTips))
                {
                    baseXpGain = Campaign.Current.Models.PartyTrainingModel
                        .GetTroopPerksXp(DefaultPerks.Leadership.CombatTips);
                    totalXp += ExecuteHeroDailyTraining(
                        hero: hero, 
                        party: party, 
                        baseXpGain: baseXpGain * multiplier, 
                        nativeXpGain: baseXpGain,
                        maxTierTrained: int.MaxValue, 
                        nativeMaxTierTrained: NativeMaxCompatTipsTier);
                    
                }

                // If Hero has neither perk
                if (!hero.GetPerkValue(DefaultPerks.Leadership.RaiseTheMeek)
                    && !hero.GetPerkValue(DefaultPerks.Leadership.CombatTips))
                {
                    baseXpGain = Settings.Instance.BaseTrainingXpGain;
                    totalXp += ExecuteHeroDailyTraining(
                        hero: hero, 
                        party: party, 
                        baseXpGain: baseXpGain * multiplier, 
                        nativeXpGain: 0,
                        maxTierTrained: Settings.Instance.BaseTrainingMaxTierTrained, 
                        nativeMaxTierTrained: 0);
                }
            }

            return totalXp;
        }

        /// <summary>
        /// Apply hero's training onto their party.
        /// </summary>
        private static int ExecuteHeroDailyTraining(Hero hero, MobileParty party,
            float baseXpGain, int nativeXpGain, int maxTierTrained, int nativeMaxTierTrained)
        {
            // If configured not to do this training
            if (baseXpGain <= 0 || maxTierTrained <= 0)
            {
                return 0;
            }

            var partyMembers = party.MemberRoster;
            int totalXp = 0;

            // For each group in the hero's party
            for (int idx = 0; idx < partyMembers.Count; idx++)
            {
                var curMember = partyMembers.GetCharacterAtIndex(idx);
                // If member isn't a hero and is low enough tier to be trained
                if (!curMember.IsHero && curMember.Tier <= maxTierTrained)
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

                        // Compute level difference and leadership skill multiplier
                        float levelDiffAndLeadershipMult = 1.0f 
                            + Math.Max(0, hero.Level - curMember.Level)
                                * (Settings.Instance.LevelDifferenceFactor / 100.0f)
                            + Math.Max(0, hero.GetSkillValue(DefaultSkills.Leadership))
                                * (Settings.Instance.LeadershipSkillFactor / 100.0f);

                        // Compute xp to give to current group
                        float xpPerTroop = levelDiffAndLeadershipMult * baseXpGain;
                        int xpForCurGroup = (int)Math.Round(
                            (numInGroup - numNotTrained) * xpPerTroop);

                        // If group was given xp by Native
                        if (curMember.Tier <= nativeMaxTierTrained)
                        {
                            // Subtract native perk xp gain
                            xpForCurGroup -= nativeXpGain;
                        }

                        // If positive xp gain
                        if (xpForCurGroup > 0)
                        {
                            // Add xp to current troop group
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
