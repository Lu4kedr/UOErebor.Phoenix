using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Phoenix.Plugins.Runes
{
    [Serializable]
    public class Rune
    {
        public string Name { get; set; }
        public uint Id { get; set; }
        [XmlArray]
        public uint[] Containers { get; set; }
        [XmlArray]
        public string[] ContainersName { get; set; }

        public void Recall()
        {
            new UOItem(Id).WaitTarget();
            UO.Cast(StandardSpell.Recall);
        }
        public void RecallSvitek()
        {
            DateTime n = DateTime.Now;
            UOItem svitek = new UOItem(0x00);
            DateTime start = DateTime.Now;
            while (svitek.Name != "Recall")
            {
                svitek = new UOItem(World.Player.Backpack.AllItems.FindType(0x1F4C));
                svitek.Click();
                if (DateTime.Now - n > TimeSpan.FromSeconds(2)) return;
            }
            new UOItem(Id).WaitTarget();
            svitek.Use();

        }
        public void Gate()
        {
            new UOItem(Id).WaitTarget();
            UO.Cast(StandardSpell.GateTravel);
        }


    }
}
