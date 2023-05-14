using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TrainingTweak.HarmonyPatches;

public class TroopTrainingPatch : HarmonyPatch
{
   private static bool Faulted { get; set; }
   
   protected override MethodInfo OriginalMethod => TrainingModel.GetType()
      .GetMethod(nameof(TrainingModel.GetEffectiveDailyExperience))
      ?.DeclaringType?.GetMethod(nameof(TrainingModel.GetEffectiveDailyExperience));

   protected override MethodInfo Postfix =>
       GetType().GetMethod(nameof(DailyExperiencePostfix));
   
   PartyTrainingModel TrainingModel => Campaign.Current.Models.PartyTrainingModel;

   public TroopTrainingPatch(Harmony harmony) : base(harmony) { }
   
   public static void DailyExperiencePostfix(MobileParty party, 
       TroopRosterElement troop, ref ExplainedNumber __result)
   {
      if (Faulted) return;
      
      try
      {
         if (!Settings.Instance.EnableTrainingPerkOverrides) return;

         var native = __result.ResultNumber;
         try
         {
             var target = HandleDailyTraining(party, troop);
             var diff = target - native;
             __result.Add(diff,
                 new TextObject("Disabling native training for TrainingTweak"));
         }
         catch (Exception) {}
      }
      catch (Exception exc)
      {
         Faulted = true;
         Util.Warning(
            $"{Strings.DisableNativeTrainingPatchFailed}\n\n" +
            $"{Strings.FatalErrorDisclaimer}",
            exc: exc);

      }
   }
   
   private static float HandleDailyTraining(MobileParty party,
       TroopRosterElement troop)
   {
       if (party is not { IsActive: true } || party.MemberRoster is null)
           throw new ArgumentException("Invalid party");

        // If it is the player's party, and configured to train parties
        if ((Settings.Instance.EnableTrainingPerkOverrides 
                || Settings.Instance.EnableBaseTraining)
            && party.IsMainParty)
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
        // Otherwise, if it is a lord party or a player-owned caravan, 
        //            and configured to train parties
        else if ((Settings.Instance.EnableTrainingPerkOverrides 
                || Settings.Instance.EnableBaseTraining)
            && party.IsLordParty || (party.IsCaravan && party.Party?.Owner == Hero.MainHero))
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
        else if (Settings.Instance.EnableGarrisonTraining && party.IsGarrison)
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
                    $"{Strings.NullTownDetected}: {party.Name} " +
                    $"\n{Strings.SettlementHeader}: {party.CurrentSettlement?.Name}" +
                    $"\n\n{Strings.WarningDisclaimer}" +
                    $"\n\n{Strings.DebugModeNote}");
            }

            return;
        }

        // Get base xp gain for this garrison
        int trainingFieldLevel = party.CurrentSettlement.Town.Buildings
            .Find(buil => buil.BuildingType == DefaultBuildingTypes.CastleTrainingFields
                || buil.BuildingType == DefaultBuildingTypes.SettlementTrainingFields)
            ?.CurrentLevel ?? 0;

        float xpPerTroop = trainingFieldLevel
            * Settings.Instance.LevelOneTrainingFieldXpAmount
            * multiplier;

        // If training this garrison
        if (xpPerTroop > 0)
        {
            var members = party.MemberRoster;

            // For each group in the garrison
            for (int idx = 0; idx < members.Count; idx++)
            {
                // If of a tier configured to be trained
                if (members.GetCharacterAtIndex(idx).Tier 
                    <= Settings.Instance.GarrisonTrainingMaxTierTrained)
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
    }

    private static int TrainParty(MobileParty party, float multiplier)
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
                        $"{Strings.NullHeroDetected}: {party.Name} " +
                        $"\n{Strings.PartyLeaderHeader}: {party.Leader?.Name}" +
                        $"\n{Strings.HeroHeader}: {member.Character.Name}" +
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
                        $"{Strings.HeroNotInPartyDetected}: {party.Name}" +
                        $"\n{Strings.PartyLeaderHeader}: {party.Leader?.Name}" +
                        $"\n{Strings.HeroHeader}: {member.Character.Name}" +
                        $"\n\n{Strings.WarningDisclaimer}" +
                        $"\n\n{Strings.DebugModeNote}");
                }

                // We can just ignore this issue ourselves, so no need to skip
            }

            Hero hero = member.Character.HeroObject;

            // If hero has raise the meek perk, and perks are overridden
            if (Settings.Instance.EnableTrainingPerkOverrides
                && hero.GetPerkValue(DefaultPerks.Leadership.RaiseTheMeek))
            {
                totalXp += ExecuteHeroDailyTraining(
                    hero: hero,
                    party: party,
                    baseXpGain: Settings.Instance.RaiseTheMeekXpAmount * multiplier,
                    maxTierTrained: Settings.Instance.RaiseTheMeekMaxTierTrained);
            }

            // If hero has combat tips perk, and perks are overridden
            if (Settings.Instance.EnableTrainingPerkOverrides
                && hero.GetPerkValue(DefaultPerks.Leadership.CombatTips))
            {
                totalXp += ExecuteHeroDailyTraining(
                    hero: hero, 
                    party: party, 
                    baseXpGain: Settings.Instance.CombatTipsXpAmount * multiplier, 
                    maxTierTrained: Settings.Instance.ComatTipsMaxTierTrained);
            }

            // If base training enabled, and Hero has neither perk
            if (Settings.Instance.EnableBaseTraining
                && !hero.GetPerkValue(DefaultPerks.Leadership.RaiseTheMeek)
                && !hero.GetPerkValue(DefaultPerks.Leadership.CombatTips))
            {
                totalXp += ExecuteHeroDailyTraining(
                    hero: hero, 
                    party: party, 
                    baseXpGain: Settings.Instance.BaseTrainingXpAmount * multiplier, 
                    maxTierTrained: Settings.Instance.BaseTrainingMaxTierTrained);
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
            int leadershipXp = (int)Math.Ceiling(
                totalXp / Settings.Instance.TrainingXpPerLeadershipXp);
            hero.AddSkillXp(DefaultSkills.Leadership, leadershipXp);
        }

        return totalXp;
    }
   
}