using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phoenix.Plugins
{
    public class Abilites
    {
        private DateTime LastLeap = DateTime.Now;



        [Command]
        public void Leap()
        {
            if (DateTime.Now - LastLeap > TimeSpan.FromMilliseconds(4400))
            {
                UOCharacter ch = new UOCharacter(Aliases.LastAttack);
                if (ch.Distance < 10 && World.Player.Warmode)
                {
                    if (!World.Player.Warmode)
                    {
                        UO.Warmode(true);
                    }
                    UO.Attack(ch);
                    UO.Say(".leap");
                    LastLeap = DateTime.Now;
                    while (DateTime.Now - LastLeap < TimeSpan.FromMilliseconds(4400))
                    {
                        UO.Wait(100);
                    }
                    World.Player.Print("===== LEAP =====");
                }
                else
                    UO.PrintError("Moc Daleko");

            }
            else
                UO.PrintError("Jeste nemuzes pouzit Leap");
        }


        [Command,BlockMultipleExecutions]
        public void probo()
        {
            UOCharacter target = new UOCharacter(Aliases.GetObject("laststatus"));
            bool first = true;
            while (World.Player.Hidden)
            {

                UO.Wait(100);
                if (!target.Serial.Equals(Aliases.GetObject("laststatus")))
                    target = new UOCharacter(Aliases.GetObject("laststatus"));
                if (DateTime.Now - Hiding.HiddenTime < TimeSpan.FromSeconds(3)) continue;
                if (first)
                {
                    UO.PrintError("Muzes Bodat!");
                    first = false;
                }
                if (target.Distance < 2)
                {
                    Journal.Clear();
                    target.WaitTarget();
                    UO.Say(".usehand");
                    Journal.WaitForText(true, 500, "Utok se nepovedl.", "Cil prilis daleko.", "Nevidis na cil.", "Nedosahnes na cil.");
                }

            }
            UO.RunCmd("hidoff");
            UO.RunCmd("hid");

        }

        [Command,BlockMultipleExecutions]
        public void bomba()
        {
            UOCharacter cil = new UOCharacter(Aliases.GetObject("laststatus"));
            if (cil.Distance > 5)
            {
                UO.PrintError("Moc daleko");
                return;
            }
            cil.WaitTarget();
            UO.Say(".throwexplosion");
            UO.Wait(100);
            new UOItem(Aliases.GetObject("ActualWeapon")).Equip();
            new UOItem(Aliases.GetObject("ActualShield")).Use();
        }

        [Command,BlockMultipleExecutions]
        public void kudla()
        {
            UOCharacter cil = new UOCharacter(Aliases.GetObject("laststatus"));
            if (cil.Distance > 6)
            {
                UO.PrintError("Moc daleko");
                return;
            }
            UO.Say(".throw");
            //new UOItem(Aliases.GetObject("ActualWeapon")).Equip();
            if (Journal.WaitForText(true, 1000, "Nemas zadny cil.", "Nevidis na cil"))
            {
                UO.PrintInformation("HAZ!!");
                return;
            }
            UO.Wait(1000);
            UO.PrintInformation("3");
            UO.Wait(1000);
            UO.PrintInformation("2");
            UO.Wait(1000);
            UO.PrintInformation("1");
            UO.Wait(1000);
            UO.PrintInformation("HAZ!!");
            World.Player.Print("Hazej!!!");

        }
    }
}
