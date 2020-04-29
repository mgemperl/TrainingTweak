using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace TrainingTweak
{
    public static class Strings
    {
        public const string XpPlaceholder = "[xp]";
        public static GameTextManager TextManager { get; set; }

        public static void LoadStrings(string localizationFolderPath)
        {
            List<string> fileNames = new List<string>();

            if (Directory.Exists(localizationFolderPath))
            {
                fileNames = Directory.EnumerateFiles(localizationFolderPath, "*.xml")
                    .Select(path => Path.GetFileName(path))
                    .ToList();
            }

            // If language files exist
            if (fileNames.Count > 0)
            {
                string filePath;
                // If localization for user language and country is present
                if (fileNames.Contains(CultureInfo.CurrentCulture.Name + ".xml"))
                {
                    filePath = Path.Combine(localizationFolderPath, fileNames
                        .Find(fileName => fileName == CultureInfo.CurrentCulture.Name + ".xml"));
                }
                // If localization for user language is present
                else if (!fileNames.Where(fileName => MatchesUserLanguage(fileName))
                    .IsEmpty())
                {
                    filePath = Path.Combine(localizationFolderPath, fileNames
                        .Find(fileName => MatchesUserLanguage(fileName)));
                }
                // If only one language is available
                else if (fileNames.Count == 1)
                {
                    filePath = Path.Combine(localizationFolderPath, fileNames.First());
                }
                else
                {
                    // Use en-US
                    filePath = Path.Combine(localizationFolderPath, "en-US.xml");
                }

                //strings = ReadLanguageFile(filePath);
                TextManager.LoadGameTexts(filePath);
            }
        }

        private static bool MatchesUserLanguage(string fileName)
        {
            string[] splitCulture = CultureInfo.CurrentCulture.Name.Split('-');
            string userLanguage = string.Join("", splitCulture.Take(splitCulture.Length - 1));

            splitCulture = fileName.Split('-');
            string fileLanguage = string.Join("", splitCulture.Take(
                Math.Min(1, splitCulture.Length - 1)));

            return userLanguage == fileLanguage;
        }

        private static string SafeFetchString(string id)
        {
            string str;
            try
            {
                str = TextManager.FindText(id).ToString();
            }
            catch (Exception exc)
            {
                str = $"MISSING_STRING[{id}]";
            }

            return str;
        }

        public static string DailyTrainingMessage
        {
            get
            {
                return SafeFetchString("str_tt_daily_training_message");
            }
        }
        public static string UpgradesAvailableMessage 
        {
            get
            {
                return TextManager.FindText("str_tt_upgrades_available_message").ToString();
            }
        }

        public static string FatalErrorMessage
        {
            get
            {
                return TextManager.FindText("TrainingTweak_FatalErrorMessage").ToString();
            }
        }
        public static string FatalErrorDisclaimer
        {
            get
            {
                return TextManager.FindText("TrainingTweak_FatalErrorDisclaimer").ToString();
            }
        }
        public static string WarningMessageHeader 
        {
            get
            {
                return TextManager.FindText("TrainingTweak_WarningMessageHeader").ToString();
            }
        }
        public static string WarningDisclaimer 
        {
            get
            {
                return TextManager.FindText("TrainingTweak_WarningDisclaimer").ToString();
            }
        }
        public static string DebugModeNote 
        {
            get
            {
                return TextManager.FindText("TrainingTweak_DebugModeNote").ToString();
            }
        }
        public static string NullPartyDetected
        {
            get
            {
                return TextManager.FindText("TrainingTweak_NullPartyDetected").ToString();
            }
        }
        public static string NullMemberRosterDetected
        {
            get
            {
                return TextManager.FindText("TrainingTweak_NullMemberRosterDetected").ToString();
            }
        }
        public static string NullCharacterDetected 
        {
            get
            {
                return TextManager.FindText("TrainingTweak_NullCharacterDetected").ToString();
            }
        }
        public static string NullTownDetected 
        {
            get
            {
                return TextManager.FindText("TrainingTweak_NullTownDetected").ToString();
            }
        }
        public static string NullHeroDetected 
        {
            get
            {
                return TextManager.FindText("TrainingTweak_NullHeroDetected").ToString();
            }
        }
        public static string NullCharacter 
        {
            get
            {
                return TextManager.FindText("TrainingTweak_NullCharacter").ToString();
            }
        }
        public static string HeroNotInPartyDetected
        {
            get
            {
                return TextManager.FindText("TrainingTweak_HeroNotInPartyDetected").ToString();
            }
        }
        public static string PartyLeaderHeader
        {
            get
            {
                return TextManager.FindText("TrainingTweak_PartyLeaderHeader").ToString();
            }
        }
        public static string SettlementHeader
        {
            get
            {
                return TextManager.FindText("TrainingTweak_SettlementHeader").ToString();
            }
        }
        public static string HeroHeader 
        {
            get
            {
                return TextManager.FindText("TrainingTweak_HeroHeader").ToString();
            }
        }
    }
}
