using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TrainingTweak.Settings;
using HarmonyPrefix = TrainingTweak.HarmonyPatches.Base.HarmonyPrefix;

namespace TrainingTweak.HarmonyPatches;

public class TroopTrainingPatch : HarmonyPrefix
{
   private static bool Faulted { get; set; }
   private static TrainingPerkSettings PerkSettings => ModSettings.Instance.TrainingPerk;
   private static BaseTrainingSettings BaseSettings => ModSettings.Instance.BaseTraining;
   private static GarrisonTrainingSettings GarrisonSettings => 
       ModSettings.Instance.GarrisonTraining;

   protected override MethodInfo? OriginalMethod =>
       typeof(MobilePartyTrainingBehavior).GetMethod("OnDailyTickParty", 
           BindingFlags.NonPublic | BindingFlags.Instance);

   protected override MethodInfo? Patch =>
       GetType().GetMethod(nameof(DailyExperiencePrefix));

   public TroopTrainingPatch(Harmony harmony) : base(harmony) { }
   
   
   public static bool DailyExperiencePrefix(MobileParty party)
   {
      if (Faulted) 
          return true;
    
      try
      {
         if (!PerkSettings.EnableTrainingPerkOverrides
             && !BaseSettings.EnableBaseTraining) 
             return true;
         
         HandleDailyTraining(party);
         ApplyBowTrainerPerk(party);
      }
      catch (Exception exc)
      {
         Faulted = true;
         Util.Warning(
            $"{Strings.TrainingPatchFailed}\n\n" +
            $"{Strings.FatalErrorDisclaimer}",
            exc: exc);
      }
      
      return false;
   }

   private static void ApplyBowTrainerPerk(MobileParty party)
   {
       if (!party.HasPerk(DefaultPerks.Bow.Trainer) || party.IsDisbanding)
           return;
       
       Hero? heroWithLowestBowSkill = null;
       int lowestSkillLevel = int.MaxValue;
       foreach (var troop in party.MemberRoster.GetTroopRoster())
       {
           if (!troop.Character.IsHero) 
               continue;
           
           int skillValue = troop.Character.HeroObject.GetSkillValue(DefaultSkills.Bow);
           
           if (skillValue >= lowestSkillLevel) 
               continue;
           
           lowestSkillLevel = skillValue;
           heroWithLowestBowSkill = troop.Character.HeroObject;
       }
       
       heroWithLowestBowSkill?.AddSkillXp(
           DefaultSkills.Bow, DefaultPerks.Bow.Trainer.PrimaryBonus);
   }
   
   private static void HandleDailyTraining(MobileParty party)
   {
       if (party is not { IsActive: true } || party.MemberRoster is null)
           return;

        // If it is the player's party, and configured to train parties
       if (party.IsMainParty)
       {
           TrainPlayerParty(party);

       }
        // Otherwise, if it is an AI lord party or player-owned caravan
       else if (party.IsLordParty
                || (party.IsCaravan && party.Party?.Owner == Hero.MainHero))
       {
           TrainAiParty(party);
       }
        // Otherwise, if it is a garrison
        else if (party.IsGarrison)
       {
           TrainGarrison(party);
       }
   }

   private static void TrainPlayerParty(MobileParty party)
   {
       // If not configured to train player party, or multiplier is 0
       if (PerkSettings.PlayerPartyTrainingXpMultiplier <= 0) return;
       
       // Train player party
       var totalXp = ComputeDailyTrainingXp(party, PerkSettings.PlayerPartyTrainingXpMultiplier);

       // If troops received no training xp, do nothing
       if (totalXp <= 0) return;
       
       // Display training results to player
       string xpGainedNotice = Strings.DailyTrainingMessage.Replace(
           Strings.XpPlaceholder, totalXp.ToString());
       InformationManager.DisplayMessage(
           new InformationMessage(xpGainedNotice));

       // If there are troops ready to upgrade
       if (party.MemberRoster.GetTroopRoster()
           .Any(elem => elem.Character.UpgradeTargets.Length > 0))
       {
           // Inform player
           InformationManager.DisplayMessage(new InformationMessage(
               $"{Strings.UpgradesAvailableMessage}"));
       }

   }

   private static void TrainAiParty(MobileParty party)
   {
       // Get multiplier for this party
       float multiplier = party.Party?.Owner == Hero.MainHero
           ? PerkSettings.PlayerClanPartyTrainingXpMultiplier
           : PerkSettings.NonPlayerClanPartyTrainingXpMultiplier;

       // If configured to train this party
       if(multiplier > 0)
       {
           // Train AI party
           ComputeDailyTrainingXp(party, multiplier);
       }
   }

   private static void TrainGarrison (MobileParty party)
   {
       // Check if garrison's town doesn't exist, or if a null is lurking amongst us
       if (GarrisonSettings.EnableGarrisonTraining 
           && party.CurrentSettlement?.Town == null)
           return;
        
       // Get multiplier for this garrison
       float multiplier = party.Party?.Owner == Hero.MainHero
           ? GarrisonSettings.PlayerClanGarrisonTrainingXpMultiplier
           : GarrisonSettings.NonPlayerClanGarrisonTrainingXpMultiplier;

       if (multiplier <= 0)
           return;

       // Get base xp gain for this garrison
       int trainingFieldLevel = party.CurrentSettlement.Town.Buildings
           .Find(buil => buil.BuildingType == DefaultBuildingTypes.CastleTrainingFields
                         || buil.BuildingType == DefaultBuildingTypes.SettlementTrainingFields)
           ?.CurrentLevel ?? 0;
       float xpPerTroop = trainingFieldLevel
                          * GarrisonSettings.LevelOneTrainingFieldXpAmount
                          * multiplier;

       // If garrison gaining no xp, return
       if (!(xpPerTroop > 0)) return;
        
       // For each group in the garrison
       var members = party.MemberRoster;
       for (int idx = 0; idx < members.Count; idx++)
       {
           // If not of a tier configured to be trained, skip
           if (members.GetCharacterAtIndex(idx).Tier
               > GarrisonSettings.GarrisonTrainingMaxTierTrained) 
               continue;
            
           int numInGroup = members.GetElementNumber(idx);
           int numUpgradeable = members.GetElementCopyAtIndex(idx)
               .Character.UpgradeTargets.Length;

           // If there are no trainable troops, skip
           if (numUpgradeable >= numInGroup) 
               continue;
            
           int numNotTrained = 0;
           // Remove wounded from training if configured to
           if (!PerkSettings.WoundedReceiveTraining)
           {
               numNotTrained = members.GetElementWoundedNumber(idx);
           }
           // Remove upgradeable from training if configured to
           if (!PerkSettings.UpgradeableReceiveTraining)
           {
               // Allow wounded troops to be considered the upgradeable ones
               numNotTrained = Math.Max(numNotTrained, numUpgradeable);
           }

           int xpForCurGroup = (int)Math.Ceiling(
               (numInGroup - numNotTrained) * xpPerTroop);
           // Add xp to current troop group
           if (xpForCurGroup > 0)
               members.AddXpToTroopAtIndex(xpForCurGroup, idx);
       }
   }

   private static int ComputeDailyTrainingXp(MobileParty party, float multiplier)
   {
       int totalXp = 0;
       
       // For each hero in the party
       foreach (var member in party.MemberRoster.GetTroopRoster()
                    .Where(member => member.Character.IsHero))
       {
           if (member.Character.HeroObject is not {} hero)
               continue;

           // If hero has raise the meek perk, and perks are overridden
           if (PerkSettings.EnableTrainingPerkOverrides
               && hero.GetPerkValue(DefaultPerks.Leadership.RaiseTheMeek))
           {
               totalXp += ExecuteHeroDailyTraining(
                   hero: hero,
                   party: party,
                   baseXpGain: PerkSettings.RaiseTheMeekXpAmount * multiplier,
                   maxTierTrained: PerkSettings.RaiseTheMeekMaxTierTrained);
           }

           // If hero has combat tips perk, and perks are overridden
           if (PerkSettings.EnableTrainingPerkOverrides
               && hero.GetPerkValue(DefaultPerks.Leadership.CombatTips))
           {
               totalXp += ExecuteHeroDailyTraining(
                   hero: hero, 
                   party: party, 
                   baseXpGain: PerkSettings.CombatTipsXpAmount * multiplier, 
                   maxTierTrained: PerkSettings.ComatTipsMaxTierTrained);
           }

           // If base training enabled, and Hero has neither perk
           if (BaseSettings.EnableBaseTraining
               && !hero.GetPerkValue(DefaultPerks.Leadership.RaiseTheMeek)
               && !hero.GetPerkValue(DefaultPerks.Leadership.CombatTips))
           {
               totalXp += ExecuteHeroDailyTraining(
                   hero: hero, 
                   party: party, 
                   baseXpGain: BaseSettings.BaseTrainingXpAmount * multiplier, 
                   maxTierTrained: BaseSettings.BaseTrainingMaxTierTrained);
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
                   .Character.UpgradeTargets.Length;

               // If there are troops in group to train
               if (numUpgradeable < numInGroup)
               {
                   int numNotTrained = 0;
                   // Remove wounded from training if configured to
                   if (!PerkSettings.WoundedReceiveTraining)
                   {
                       numNotTrained = partyMembers.GetElementWoundedNumber(idx);
                   }
                   // Remove upgradeable from training if configured to
                   if (!PerkSettings.UpgradeableReceiveTraining)
                   {
                       // Allow wounded troops to be considered the upgradeable ones
                       numNotTrained = Math.Max(numNotTrained, numUpgradeable);
                   }

                   // Compute level difference and leadership skill multiplier
                   float levelDiffAndLeadershipMult = 
                       1.0f + Math.Max(0, hero.Level - curMember.Level)
                          * (PerkSettings.LevelDifferenceFactor / 100.0f)
                          + Math.Max(0, hero.GetSkillValue(DefaultSkills.Leadership))
                          * (PerkSettings.LeadershipSkillFactor / 100.0f);

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
       if (PerkSettings.TrainingXpPerLeadershipXp <= 0 || totalXp <= 0) 
           return totalXp;
       
       // Give hero leadership xp
       int leadershipXp = (int)Math.Ceiling(
           totalXp / PerkSettings.TrainingXpPerLeadershipXp);
       hero.AddSkillXp(DefaultSkills.Leadership, leadershipXp);

       return totalXp;
   }
   
}