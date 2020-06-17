using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace TrainingTweak
{
    public static class Strings
    {
        public const string XpPlaceholder = "[xp]";

        private static string FetchString(string id, 
            string backup = "")
        {
            string str;
            TextObject text;

            if (Module.CurrentModule.GlobalTextManager.TryGetText(
                    id, null, out text))
            {
                str = text.ToString();
            }
            else
            {
                str = (backup.Length > 0)
                    ? backup
                    : $"MISSING_STRING[{id}]";
            }

            return str;
        }

        // General settings group names
        public static string TrainingPerkXpGain => FetchString(
            "str_tt_training_perks_xp_gain_group", "Perk Xp Gain");
        public static string XpMultipliersSettingsGroup => FetchString(
            "str_tt_xp_multipliers_group", "Xp Multipliers");
        public static string TierLimitsSettingsGroup => FetchString(
            "str_tt_tier_limits_group", "Tier Limits");
        public static string GeneralSettingsGroup => FetchString(
            "str_tt_general_group", "General");

        public static string AISubgroup => FetchString(
            "str_tt_ai_subgroup", "AI");
        public static string PlayerSubgroup => FetchString(
            "str_tt_player_subgroup", "Player");


        // Training Perk Settings
        public static string TrainingPerkGroup => FetchString(
            "str_tt_training_perks_group", "Training Perk Overrides");
        public static string EnableTrainingPerkOverridesHint => FetchString(
            "str_tt_enable_training_perk_overrides_hint",
            "Enable tweaks to the native training perks.");
        public static string RaiseTheMeekXpAmountDisplay => FetchString(
            "str_tt_raise_the_meek_xp_amount_display", "Raise The Meek Base Xp");
        public static string RaiseTheMeekXpAmountHint => FetchString(
            "str_tt_raise_the_meek_xp_amount_hint",
            "Daily Xp gain from Raise The Meek before multipliers are applied. " +
            "Native value is 30.");
        public static string CombatTipsXpAmountDisplay => FetchString(
            "str_tt_combat_tips_xp_amount_display", "Combat Tips Base Xp");
        public static string CombatTipsXpAmountHint => FetchString(
            "str_tt_combat_tips_xp_amount_hint",
            "Daily Xp gain from Combat Tips before multipliers are applied. " +
            "Native value is 15.");
        public static string RaiseTheMeekMaxTierTrainedDisplay => FetchString(
            "str_tt_raise_the_meek_tier_display",
            "Raise The Meek Max Tier Trained");
        public static string RaiseTheMeekMaxTierTrainedHint => FetchString(
            "str_tt_raise_the_meek_tier_hint",
            "Max tier of troops in parties that will be trained by the 'Raise The Meek' perk. Native value is 3.");
        public static string CombatTipsMaxTierTrainedDisplay => FetchString(
            "str_tt_combat_tips_tier_display",
            "Combat Tips Max Tier Trained");
        public static string CombatTipsMaxTierTrainedHint => FetchString(
            "str_tt_combat_tips_tier_hint",
            "Max tier of troops in parties that will be trained by the 'Combat Tips' perk. Native value is the max possible troop tier.");
        public static string PlayerPartyTrainingMultDisplay => FetchString(
            "str_tt_player_party_training_mult_display",
            "Player Party Training Xp Multiplier");
        public static string PlayerPartyTrainingMultHint => FetchString(
            "str_tt_player_party_training_mult_hint",
            "Multiplier for all xp this mod gives to the player's party.");
        public static string PlayerClanPartyTrainingMultDisplay => FetchString(
            "str_tt_player_clan_party_training_mult_display",
            "Player Clan Party Training Xp Multiplier");
        public static string PlayerClanPartyTrainingMultHint => FetchString(
            "str_tt_player_clan_party_training_mult_hint",
            "Multiplier for all xp this mod gives to AI parties in the player's clan.");
        public static string NonPlayerClanPartyTrainingMultDisplay => FetchString(
            "str_tt_non_player_clan_party_training_mult_display",
            "Non-Player Clan Party Training Xp Multiplier");
        public static string NonPlayerClanPartyTrainingMultHint => FetchString(
            "str_tt_non_player_clan_party_training_mult_hint",
            "Multiplier for all xp this mod gives to AI parties not in the player's clan.");
        public static string LevelDifferenceFactorDisplay => FetchString(
            "str_tt_level_difference_factor_display",
            "Level Difference Factor");
        public static string LevelDifferenceFactorHint => FetchString(
            "str_tt_level_difference_factor_hint",
            "For every level the trainer is above the troop, training xp is increased by X percent.");
        public static string LeadershipSkillFactorDisplay => FetchString(
            "str_tt_leadership_skill_factor_display",
            "Leadership Skill Factor");
        public static string LeadershipSkillFactorHint => FetchString(
            "str_tt_leadership_skill_factor_hint",
            "For each skill level in leadership, training xp is increased by X percent.");
        public static string TrainingXpPerLeadershipXpDisplay => FetchString(
            "str_tt_training_xp_per_leadership_xp_display",
            "Training Xp Per Leadership Xp");
        public static string TrainingXpPerLeadershipXpHint => FetchString(
            "str_tt_training_xp_per_leadership_xp_hint",
            "How much xp a trainer has to train troops to get 1 leadership xp.");
        public static string WoundedReceiveTrainingDisplay => FetchString(
            "str_tt_wounded_receive_training_display",
            "Wounded Receive Training");
        public static string WoundedReceiveTrainingHint => FetchString(
            "str_tt_wounded_receive_training_hint",
            "Whether wounded troops count toward group size during training.");
        public static string UpgradeableReceiveTrainingDisplay => FetchString(
            "str_tt_upgradeable_receive_training_display",
            "Upgradeable Receive Training");
        public static string UpgradeableReceiveTrainingHint => FetchString(
            "str_tt_upgradeable_receive_training_hint",
            "Whether upgradeable troops count toward group size during training.");

        // Basic Training Settings
        public static string BaseTrainingSettingsGroup => FetchString(
            "str_tt_base_training_group", "Basic Training");
        public static string EnableBaseTrainingHint => FetchString(
            "str_tt_enable_base_training_hint", 
            "Allow heroes with neither training perk to perform basic training.");
        public static string BaseTrainingXpAmountDisplay => FetchString(
            "str_tt_base_training_xp_amount_display", "Basic Training Xp Gain");
        public static string BaseTrainingXpAmountHint => FetchString(
            "str_tt_base_training_xp_amount_hint", 
            "Daily Xp gain from training by heroes with neither training perk.");
        public static string BaseTrainingTierDisplay => FetchString(
            "str_tt_base_training_tier_display",
            "Basic Training Max Tier Trained");
        public static string BaseTrainingTierHint => FetchString(
            "str_tt_base_training_tier_hint",
            "Max tier of troops in parties that will be trained by heroes with " +
            "neither training perk.");


        // Garrison Training Settings
        public static string GarrisonGroup => FetchString(
            "str_tt_garrison_group", "Garrison Training Override");
        public static string EnableGarrisonTrainingOverrideHint => FetchString(
            "str_tt_enable_garrison_training_override_hint", 
            "Enable tweaks to native garrison training.");
        public static string LevelOneTrainingFieldXpAmountDisplay => FetchString(
            "str_tt_level_one_training_field_xp_amount_display",
            "Level One Training Field Xp Gain");
        public static string LevelOneTrainingFieldXpAmountHint => FetchString(
            "str_tt_level_one_training_field_xp_amount_hint",
            "Garrison Xp gain from a level 1 training field. " +
            "Level 2 will be double this amount, level 3 will be triple, and so on. " +
            "Native value is 1 (which is ridiculous).");
        public static string GarrisonMaxTierTrainedDisplay => FetchString(
            "str_tt_garrison_max_tier_trained_display",
            "Garrison Max Tier Trained");
        public static string GarrisionMaxTierTrainedHint => FetchString(
            "str_tt_garrison_max_tier_trained_hint",
            "Max tier of troops in garrisons that will be trained by this mod.");
        public static string PlayerClanGarrisonTrainingMultDisplay => FetchString(
            "str_tt_player_clan_garrison_training_mult_display",
            "Player Clan Garrison Training Xp Multiplier");
        public static string PlayerClanGarrisonTrainingMultHint => FetchString(
            "str_tt_player_clan_garrison_training_mult_hint",
            "Multiplier for all xp this mod gives to garrisons of player-owned settlements.");
        public static string NonPlayerClanGarrisonTrainingMultDisplay => FetchString(
            "str_tt_non_player_clan_garrison_training_mult_display",
            "Non-Player Clan Garrison Training Xp Multiplier");
        public static string NonPlayerClanGarrisonTrainingMultHint => FetchString(
            "str_tt_non_player_clan_garrison_training_mult_hint",
            "Multiplier for all xp this mod gives to garrisons of settlements not owned by the player.");


        // Financial Solutions Settings
        public static string FinancialSolutionsSettingGroup => FetchString(
            "str_tt_financial_solutions_group", "Financial Solutions");
        public static string EnableFinancialSolutionsHint => FetchString(
            "str_tt_enable_financial_solutions_hint",
            "Enable patches to help prevent lords from bankrupting themselves on high-tier troops.");
        public static string TroopUpgradeCostMultiplierDisplay => FetchString(
            "str_tt_troop_upgrade_cost_mult_display",
            "Troop Upgrade Cost Multiplier");
        public static string TroopUpgradeCostMultiplierHint => FetchString(
            "str_tt_troop_upgrade_cost_mult_hint",
            "Multiplier for the upgrade cost of all troops.");
        public static string PlayerTownTaxIncomeMultiplierDisplay => FetchString(
            "str_tt_player_town_income_mult_display",
            "Player Town Tax Income Multiplier");
        public static string PlayerTownTaxIncomeMultiplierHint => FetchString(
            "str_tt_player_town_income_mult_hint",
            "Multiplier for all tax income from player-owned towns and castles.");
        public static string PlayerVillageTaxIncomeMultiplierDisplay => FetchString(
            "str_tt_player_village_income_mult_display",
            "Player Village Tax Income Multiplier");
        public static string PlayerVillageTaxIncomeMultiplierHint => FetchString(
            "str_tt_player_village_income_mult_hint",
            "Multiplier for all tax income from player-owned villages. " +
            "Note: This takes a few days to take effect.");
        public static string NonPlayerTownTaxIncomeMultiplierDisplay => FetchString(
            "str_tt_ai_town_income_mult_display",
            "Non-Player Town Tax Income Multiplier");
        public static string NonPlayerTownTaxIncomeMultiplierHint => FetchString(
            "str_tt_ai_town_income_mult_hint",
            "Multiplier for all tax income from AI-owned towns and castles.");
        public static string NonPlayerVillageTaxIncomeMultiplierDisplay => FetchString(
            "str_tt_ai_village_income_mult_display",
            "Non-Player Village Tax Income Multiplier");
        public static string NonPlayerVillageTaxIncomeMultiplierHint => FetchString(
            "str_tt_ai_village_income_mult_hint",
            "Multiplier for all tax income from AI-owned villages. " +
            "Note: This takes a few days to take effect.");
        public static string PlayerClanPartyWageMultiplierDisplay => FetchString(
            "str_tt_player_clan_party_wage_mult_display",
            "Player Clan Party Wage Multiplier");
        public static string PlayerClanPartyWageMultiplierHint => FetchString(
            "str_tt_player_clan_party_wage_mult_hint",
            "Multiplier for all party wages for parties in the player's clan.");
        public static string NonPlayerClanPartyWageMultiplierDisplay => FetchString(
            "str_tt_ai_clan_party_wage_mult_display",
            "Non-Player Clan Party Wage Multiplier");
        public static string NonPlayerClanPartyWageMultiplierHint => FetchString(
            "str_tt_ai_clan_party_wage_mult_hint",
            "Multiplier for all party wages for parties not in the player's clan.");
        
        // In-Game strings
        public static string DailyTrainingMessage => FetchString(
            "str_tt_daily_training_message", 
            "Total xp gained from training: [xp]");
        public static string UpgradesAvailableMessage => FetchString(
            "str_tt_upgrades_available_message", 
            "Some troops are ready to upgrade.");

        // Debug and error messages
        public static string DebugSettingGroup => FetchString(
            "str_tt_debug_group", "Debug");
        public static string EnableDebugModeDisplay => FetchString(
            "str_tt_enable_debug_mode_display",
            "Enable Debug Mode");
        public static string EnableDebugModeHint => FetchString(
            "str_tt_enable_debug_mode_hint",
            "Whether this mod displays potential errors it finds while running.");
        public static string TrainingOverrideFatalErrorMessage => FetchString(
            "str_tt_training_fatal_error_message",
            "Training Tweak's training features have encountered an error and are stopping, " +
            "but you may continue playing without them. Re-enabling native training.");
        public static string FatalErrorDisclaimer => FetchString(
            "str_tt_fatal_error_disclaimer",
            "Note: This was not necessarily caused by Training Tweak, it just " +
            "encountered an issue that prevented it from functioning properly.");
        public static string WarningMessageHeader  => FetchString(
            "str_tt_warning_message_header",
            "Training Tweak may have detected a corrupted game state.");
        public static string WarningDisclaimer  => FetchString(
            "str_tt_warning_disclaimer",
            "Note: This was likely not caused by Training Tweak, it just " +
            "detected a potential source of errors in the game state.");
        public static string DebugModeNote  => FetchString(
            "str_tt_debug_mode_note",
            "You may disable these debugging notices in the mod options menu " +
            "by disabling Debug Mode.");
        public static string NullPartyDetected => FetchString(
            "str_tt_null_party_detected",
            "Detected null party in the game state.");
        public static string NullMemberRosterDetected => FetchString(
            "str_tt_null_member_roster_detected",
            "Detected null member roster for party");
        public static string NullCharacterDetected  => FetchString(
            "str_tt_null_character_detected",
            "Detected null member character for party");
        public static string NullTownDetected  => FetchString(
            "str_tt_null_town_detected",
            "Detected null town for garrison");
        public static string NullHeroDetected  => FetchString(
            "str_tt_null_hero_detected",
            "Detected hero with null hero object in party");
        public static string NullCharacter  => FetchString(
            "str_tt_null_character",
            "Null Character");
        public static string HeroNotInPartyDetected => FetchString(
            "str_tt_hero_not_in_party_detected",
            "Hero doesn't consider itself a member of its party");
        public static string PartyLeaderHeader => FetchString(
            "str_tt_party_leader_header",
            "Party Leader");
        public static string SettlementHeader => FetchString(
            "str_tt_settlement_header",
            "Settlement");
        public static string HeroHeader  => FetchString(
            "str_tt_hero_header", 
            "Hero");
        public static string FinancialSolutionsPatchFailed => FetchString(
            "str_tt_financial_solutions_patches_failed",
            "Training Tweak mod's Financial Solutions patches have failed. " +
            "Continuing with Financial Solutions disabled.");
        public static string DisableNativeTrainingPatchFailed => FetchString(
            "str_tt_disable_native_training_failed",
            "Training Tweak mod failed to disable native training. Native training " +
            "and Training Tweak's training will both run in parallel, likely resulting in " +
            "excessive xp gain.");
        public static string ConfigFileFailed => FetchString(
            "str_tt_config_file_failed",
            "Training Tweak failed to load config file. Using default settings.");
        public static string SettingsRegistrationFailed => FetchString(
            "str_tt_settings_registration_failed",
            "Training Tweak failed to register settings with MCM, so its " +
            "mod options page will not appear. Using default settings.");
    }
}
