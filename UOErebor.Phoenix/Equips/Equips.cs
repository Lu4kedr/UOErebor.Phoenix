using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Phoenix.Plugins.Equips
{
    public class Equips
    {
        public static List<EqSet> equipy = new List<EqSet>();


        [Command("equipdel")]
        public static void Remove(int index)
        {
            if (index >= 0 && index >= equipy.Count) return;
            equipy.RemoveAt(index);

        }

        [Command]
        public void equipsclear()
        {
            equipy = new List<EqSet>();
        }

        [Command]
        public void eq()
        {
            for (int i = 0; i < equipy.Count; i++)
            {
                UO.PrintWarning("Equip: {0}, ID: {1}", equipy[i].SetName,i);
            }
        }

        [Command("equipadd")]
        public void Add()
        {
            UO.PrintInformation("Zamer Bagl se setem");
            UOItem bag = new UOItem(UIManager.TargetObject());
            if (bag.Items.Count() > 0)
            {
                bag.Click();
                UO.Wait(200);
                equipy.Add(new EqSet(bag));
            }
        }

        [Command, BlockMultipleExecutions]
        public void dress(int index)
        {
            equipy[index].DressOnly();
        }

        [Command, BlockMultipleExecutions]
        public void dresssave(int index)
        {
            UO.PrintWarning("Zamer baglik do ktereho se schova aktualni equip");
            equipy[index].Dress(new UOItem(UIManager.TargetObject()));
        }

    }
}
