using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phoenix.Plugins
{
    public class Magery
    {
        private bool acast;
        


        Dictionary<string, int> SpellsDelays = new Dictionary<string, int>
        {
            { "Fireball", 2060 },
            { "Flame", 4100 },
            { "Meteor", 6750 },
            { "Lightning", 3550 },
            { "Bolt", 3100 },
            { "frostbolt", 3000 },
            { "Harm", 1500 },
            { "Mind", 3450 },
            { "Invis", 3300 }
        };

        public Magery()
        {
            acast = false;
        }


        [Command]
        public void ReactiveArmor(Serial target)
        {
            DateTime start;
            UO.Warmode(false);
            if (World.Player.Backpack.AllItems.FindType(0x1F2D).Amount > 0)
            {
                new UOObject(target).WaitTarget();
                World.Player.Backpack.AllItems.FindType(0x1F2D).Use();
            }
            else
                UO.Cast(StandardSpell.ReactiveArmor, target);
            if (!Journal.WaitForText(true, 2000, "Kouzlo se nezdarilo."))
            {
                start = DateTime.Now;
            }
            else return;
            while (DateTime.Now - start < TimeSpan.FromSeconds(9)) UO.Wait(100);
            UO.PrintError("Reactiv vyprsel");
        }

        [Command]
        public void Teleport(Serial target)
        {
            if (new UOCharacter(target).Distance > 12)
            {
                UO.PrintError("Moc daleko");
                return;
            }
            DateTime start;
            UO.Warmode(false);
            if (World.Player.Backpack.AllItems.FindType(0x1F42).Amount > 0)
            {
                new UOObject(target).WaitTarget();
                World.Player.Backpack.AllItems.FindType(0x1F42).Use();
            }
            else
                UO.Cast(StandardSpell.Teleport, target);
            if (!Journal.WaitForText(true, 3300, "Kouzlo se nezdarilo."))
            {
                start = DateTime.Now;
            }
            else return;
            UO.Attack(target);
        }

        [Command]
        public void Invis()
        {

            UO.Warmode(false);
            UO.Cast("Invis", Aliases.Self);

            if (Journal.WaitForText(true, SpellsDelays["Invis"] / 3, "Kouzlo se nezdarilo.")) return;
            World.Player.Print(3);
            if (Journal.WaitForText(true, SpellsDelays["Invis"] / 3, "Kouzlo se nezdarilo.")) return;
            World.Player.Print(2);
            if (!Journal.WaitForText(true, SpellsDelays["Invis"] / 3, "Kouzlo se nezdarilo."))
            {
                World.Player.Print(1);
            }
        }

        [Command]
        public void harmself()
        {
            HarmSelf(false);
        }
        [Command]
        public void arrowself()
        {
            HarmSelf(false,true);
        }

        [Command]
        public void harmself(bool AttackLast)
        {
            HarmSelf(AttackLast);
        }
        [Command]
        public void arrowself(bool AttackLast)
        {
            HarmSelf(AttackLast, true);
        }


        private void HarmSelf(bool attacklast, bool war = false)
        {
            UO.Warmode(false);
            if (war)
            {
                UO.Cast(StandardSpell.MagicArrow, World.Player.Serial);
                UO.Wait(2200);
            }
            else
            {
                UO.Cast(StandardSpell.Harm, World.Player.Serial);
                UO.Wait(1600);
            }
            if (attacklast)
            {
                UO.Warmode(true);
                UO.Attack(Aliases.GetObject("laststatus").IsValid ? Aliases.GetObject("laststatus") : 0x00);
            }
        }


        [Command]
        public void ccast(string Spell, Serial Target)
        {
            if (Spell == "frostbolt" || Spell == "necrobolt")
            {
                new UOCharacter(Target).WaitTarget();
                UO.Say(".{0}", Spell);
            }
        }

        [Command]
        public void nhcast(string Spell, Serial Target)
        {
            try
            {
                var mana = World.Player.Mana;
                int tmp = 0;
                Autoheal.AutoHeal.CastPause = true;
                if (Spell == "frostbolt" || Spell == "necrobolt")
                {
                    new UOCharacter(Target).WaitTarget();
                    UO.Say(".{0}", Spell);
                }
                else
                {
                    UO.Cast(Spell, Target);
                }
                tmp = Math.Abs(mana - World.Player.Mana);
                while (tmp < 5)
                {
                    UO.Wait(200);
                    tmp = Math.Abs(mana - World.Player.Mana);
                }
            }
            finally
            {
                Autoheal.AutoHeal.CastPause = false;
            }
        }

        [Command]
        public void Autocast(string Spell, bool charged, Serial target)
        {
            int spellsNum = 5;
            int chargeNum = 3;
            bool firstCharge = true;
            if (acast)
            {
                acast = false;
                UO.PrintError("Autocast OFF");
            }
            else
            {
                acast = true;
                UO.PrintError("Autocast ON");
                UOCharacter targ = new UOCharacter(target);
                DateTime start = DateTime.Now;
                while (acast)
                {
                    if (targ.Hits < 1 || targ.Hits > 200 || targ.Dead || !targ.Exist)
                    {
                        acast = false;
                        UO.PrintError("Autocast OFF");
                        return;
                    }

                    switch (Spell)
                    {
                        case "Harm":
                            UO.Cast(StandardSpell.Harm, target);
                            UO.Wait(SpellsDelays[Spell]);
                            break;
                        case "Fireball":
                            UO.Cast(StandardSpell.Fireball, target);
                            UO.Wait(SpellsDelays[Spell]);
                            break;
                        case "Flame":
                            if (charged)
                            {
                                for (int i = 0; i < chargeNum; i++)
                                {
                                    if (!acast) return;
                                    if (targ.Hits < 1 || targ.Hits > 200 || targ.Dead || !targ.Exist)
                                    {
                                        acast = false;
                                        UO.PrintError("Autocast OFF");
                                        return;
                                    }
                                    UO.Cast(StandardSpell.Fireball, target);
                                    UO.Wait(SpellsDelays["Fireball"]);
                                    if (!firstCharge) break;
                                }

                                if (!acast) return;
                                UO.Cast(StandardSpell.FlameStrike, target);
                                UO.Wait(SpellsDelays[Spell]);

                            }
                            else
                            {
                                UO.Cast(StandardSpell.FlameStrike, target);
                                UO.Wait(SpellsDelays[Spell]);
                            }
                            break;
                        case "Meteor":
                            UO.Cast(StandardSpell.MeteorShower, target);
                            UO.Wait(SpellsDelays[Spell]);
                            break;
                        case "Bolt":
                            if (charged)
                            {
                                for (int i = 0; i < chargeNum; i++)
                                {
                                    if (!acast) return;
                                    if (targ.Hits < 1 || targ.Hits > 200 || targ.Dead || !targ.Exist)
                                    {
                                        acast = false;
                                        UO.PrintError("Autocast OFF");
                                        return;
                                    }
                                    UO.Cast(StandardSpell.Lightning, target);
                                    UO.Wait(SpellsDelays["Lightning"]);
                                    if (!firstCharge) break;
                                }
                                if (firstCharge) firstCharge = false;
                                for (int i = 0; i < spellsNum; i++)
                                {
                                    if (!acast) return;
                                    if (targ.Hits < 1 || targ.Hits > 200 || targ.Dead || !targ.Exist)
                                    {
                                        acast = false;
                                        UO.PrintError("Autocast OFF");
                                        return;
                                    }
                                    UO.Cast(StandardSpell.EnergyBolt, target);
                                    UO.Wait(SpellsDelays[Spell]);
                                }
                            }
                            else
                            {
                                UO.Cast(StandardSpell.EnergyBolt, target);
                                UO.Wait(SpellsDelays[Spell]);
                            }
                            break;

                        case "Mind":
                            if (charged)
                            {
                                for (int i = 0; i < chargeNum; i++)
                                {
                                    if (!acast) return;
                                    if (targ.Hits < 1 || targ.Hits > 200 || targ.Dead || !targ.Exist)
                                    {
                                        acast = false;
                                        UO.PrintError("Autocast OFF");
                                        return;
                                    }
                                    UO.Cast(StandardSpell.Harm, target);
                                    UO.Wait(SpellsDelays["Harm"]);
                                    if (!firstCharge) break;
                                }
                                if (firstCharge) firstCharge = false;
                                for (int i = 0; i < spellsNum; i++)
                                {
                                    if (!acast) return;
                                    if (targ.Hits < 1 || targ.Hits > 200 || targ.Dead || !targ.Exist)
                                    {
                                        acast = false;
                                        UO.PrintError("Autocast OFF");
                                        return;
                                    }
                                    UO.Cast(StandardSpell.MindBlast, target);
                                    UO.Wait(SpellsDelays[Spell]);
                                }
                            }
                            else
                            {
                                UO.Cast(StandardSpell.MindBlast, target);
                                UO.Wait(SpellsDelays[Spell]);
                            }
                            break;

                    }
                }
            }
        }

    }
}
