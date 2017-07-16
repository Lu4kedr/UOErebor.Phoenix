using Phoenix.WorldData;
using System;
using System.Xml.Serialization;

namespace Phoenix.Plugins.Autoheal
{
    [Serializable]
    public class Patient
    {
        public uint Serial { get; set; }
        public int Equip { get; set; }

        [XmlIgnore]
        public UOCharacter Player
        {
            get
            {
                return new UOCharacter(Serial);
            }
        }
    }
}