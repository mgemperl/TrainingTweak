using System;
using System.Text;
using TaleWorlds.Library;

namespace TrainingTweak
{
    public static class Util
    {
        public static void Warning(string message, Exception? exc = null)
        {
            string excString = exc == null 
                ? string.Empty 
                : $"\n\n{FlattenException(exc)}";
            InformationManager.DisplayMessage(new InformationMessage(
                $"{message}{excString}", Color.FromVector3(new Vec3(255, 0, 0))));
        }

        public static string FlattenException(Exception exc)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(exc.Message);
            builder.AppendLine(exc.StackTrace);
            while (exc.InnerException != null)
            {
                exc = exc.InnerException;
                builder.AppendLine(exc.Message);
                builder.AppendLine(exc.StackTrace);
            }
            return builder.ToString();
        }
    }
}
