using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phoenix.Plugins
{
    public class Poisoning
    {
        public static ushort PoisonColor { get; set; }


        [Command,BlockMultipleExecutions]
        public void poisset()
        {
            UOItem tmp = new UOItem(UIManager.TargetObject());
            tmp.Click();
            UO.Wait(200);
            PoisonColor = tmp.Color;
        }

        [Command,BlockMultipleExecutions]
        public void pois()
        {
            if (PoisonColor == default(ushort))
            {
                UO.PrintError("Nemas nastaveny poison");
                return;
            }

            UOItem zbran = World.Player.Layers[Layer.RightHand];

            UOItem pois = World.Player.Backpack.AllItems.FindType(0x0F0E, PoisonColor);
            if (pois == null)
            {
                UO.PrintError("Nemas nastaveny typ poisonu");
                return;
            }
            UO.Warmode(false);
            zbran.WaitTarget();
            pois.Use();
        }


    }
}
