using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace TrainingTweak
{
    public static class SettingsLoader
    {
        public static Settings LoadSettings(string filepath)
        {
            Settings settings;

            using (XmlReader reader = XmlReader.Create(filepath))
            {
                reader.MoveToContent();
                XmlRootAttribute root = new XmlRootAttribute()
                {
                    ElementName = reader.Name
                };

                XmlSerializer serializer = new XmlSerializer(typeof(Settings), root);
                settings = (Settings) serializer.Deserialize(reader);
            }

            return settings;
        }
    }
}
