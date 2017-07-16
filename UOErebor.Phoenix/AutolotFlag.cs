using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Phoenix.Plugins
{
    [Flags]
    public enum AutolotFlag
    {
        [XmlEnum(Name ="Gold_Items")]
        Gold_Items=0,
        [XmlEnum(Name = "Food")]
        Food = 1,
        [XmlEnum(Name = "Gems")]
        Gems = 2,
        [XmlEnum(Name = "Regs")]
        Regeants = 4,
        [XmlEnum(Name = "Feathers")]
        Feathers =8,
        [XmlEnum(Name = "Bolts")]
        Bolts = 16,
        [XmlEnum(Name = "Leathers")]
        Leathers = 32,
        [XmlEnum(Name = "Ex1")]
        Extend1 =64,
        [XmlEnum(Name = "Ex2")]
        Extend2 = 128

    }
}
