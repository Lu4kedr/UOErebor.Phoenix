using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Phoenix.Plugins
{
    public class Autolot
    {
        private static AutolotFlag autolotState;
        public static AutolotFlag AutolotState {
        get
            {

                return autolotState;

            }
            set
            {
                autolotState = value;
                Notepad.WriteLine(autolotState.ToString());
                Reload();
            }
        }
        public static int GoldLimit;
        public static bool LotRunning;

        

        private List<Graphic> toCarv = new List<Graphic> { 0x2006, 0x0EE3, 0x0EE4, 0x0EE5, 0x0EE6, 0x2006 };//pvuciny+mrtvola
        private List<Serial> IgnoreList = new List<Serial>();
        private readonly List<string> jezdidla = new List<string> { "body of mustang", "body of zostrich", "body of oclock", "body of orn", "oody of ledni medved", "body of ridgeback", "body of ridgeback savage" };
        private static ushort[] FOOD = { 0x0978, 0x097A, 0x097B, 0x097E, 0x098C, 0x0994, 0x09B7, 0x09B9, 0x09C0, 0x09C9, 0x09D0, 0x09D1, 0x09D2, 0x09E9, 0x09EA, 0x09EC, 0x09F2, 0x0C5C, 0x0C64, 0x0C66, 0x0C6A, 0x0C6D, 0x0C70, 0x0C72, 0x0C74, 0x0C77, 0x0C79, 0x0C7B, 0x0C7F, 0x0D39, 0x103B, 0x1040, 0x1041, 0x1608, 0x1609, 0x160A, 0x171F, 0x1726, 0x1727, 0x1728, 0x172A };
        public static ushort Extend1 = default(ushort);
        public static ushort Extend2 = default(ushort);

        private static Dictionary<Graphic, int> LotItems = new Dictionary<Graphic, int>() { { 0x0eed, 0 }, { 0x14EB, 0 }, { 0x0E76, 0 } };
        private static bool food, gems, regeants, feathers, bolts, leather, extend1, extend2;
        private System.Timers.Timer LotRun;
        private bool CarvProgress = false;




        private static bool _Food
        {
            get { return food; }
            set
            {
                try
                {
                    food = value;
                    if (value)
                    {
                        foreach (ushort r in FOOD)
                        {
                            LotItems.Add(r, 0);
                        }
                        AutolotState |= AutolotFlag.Food;
                    }
                    else
                    {
                        foreach (ushort r in FOOD)
                        {
                            LotItems.Remove(r);
                        }
                        AutolotState &= ~AutolotFlag.Food;
                    }
                }
                catch (Exception ex) { UO.PrintError(ex.Message); }
            }
        }
        private static bool _Gems
        {
            get { return gems; }
            set
            {
                try
                {
                    gems = value;
                    if (value)
                    {
                        for (ushort i = 0x0F0F; i < 0x0F31; i++)
                        {
                            LotItems.Add(i, 0);
                        }
                        AutolotState |= AutolotFlag.Gems;
                    }

                    else
                    {
                        for (ushort i = 0x0F0F; i < 0x0F31; i++)
                        {
                            LotItems.Remove(i);
                        }
                        AutolotState &= ~AutolotFlag.Gems;
                    }
                }
                catch (Exception ex) { UO.PrintError(ex.Message); }
            }
        }
        private static bool _Regeants
        {
            get { return regeants; }
            set
            {
                try
                {
                    regeants = value;
                    if (value)
                    {
                        for (ushort i = 0x0F78; i < 0x0F92; i++)
                        {
                            LotItems.Add(i, 0);
                        }
                        AutolotState |= AutolotFlag.Regeants;
                    }
                    else
                    {
                        for (ushort i = 0x0F78; i < 0x0F92; i++)
                        {
                            LotItems.Remove(i);
                        }
                        AutolotState &= ~AutolotFlag.Regeants;
                    }
                }
                catch (Exception ex) { UO.PrintError(ex.Message); }
            }
        }
        private static bool _Feathers
        {
            get { return feathers; }
            set
            {
                feathers = value;
                try
                {
                    if (value)
                    {
                        LotItems.Add(0x1BD1, 0);
                        AutolotState |= AutolotFlag.Feathers;
                    }
                    else
                    {
                        LotItems.Remove(0x1BD1);
                        AutolotState &= ~AutolotFlag.Feathers;
                    }
                }
                catch (Exception ex) { UO.PrintError(ex.Message); }
            }
        }
        private static bool _Bolts
        {
            get { return bolts; }
            set
            {
                try
                {
                    bolts = value;
                    if (value)
                    {
                        LotItems.Add(0x1BFB, 0);
                        LotItems.Add(0x0F3F, 0);
                        AutolotState |= AutolotFlag.Bolts;
                    }
                    else
                    {
                        LotItems.Remove(0x1BFB);
                        LotItems.Remove(0x0F3F);
                        AutolotState &= ~AutolotFlag.Bolts;
                    }
                }
                catch (Exception ex) { UO.PrintError(ex.Message); }
            }
        }
        private static bool _Leather
        {
            get { return leather; }
            set
            {
                leather = value;
                try
                {
                    if (value)
                    {
                        LotItems.Add(0x1078, 0);
                        AutolotState |= AutolotFlag.Leathers;
                    }
                    else
                    {
                        LotItems.Remove(0x1078);
                        AutolotState &= ~AutolotFlag.Leathers;

                    }
                }
                catch (Exception ex) { UO.PrintError(ex.Message); }
            }
        }
        private static bool _Extend1
        {
            get { return extend1; }
            set
            {
                try
                {
                    extend1 = value;
                    if (value)
                    {
                        LotItems.Add(Extend1, 0);
                        AutolotState |= AutolotFlag.Extend1;
                    }
                    else
                    {
                        LotItems.Remove(Extend1);
                        AutolotState &= ~AutolotFlag.Extend1;

                    }
                }
                catch (Exception ex) { UO.PrintError(ex.Message); }
            }
        }
        private static bool _Extend2
        {
            get { return extend2; }
            set
            {
                try
                {
                    extend2 = value;
                    if (value)
                    {
                        LotItems.Add(Extend2, 0);
                        AutolotState |= AutolotFlag.Extend2;
                    }
                    else
                    {
                        LotItems.Remove(Extend2);
                        AutolotState &= ~AutolotFlag.Extend2;

                    }
                }
                catch (Exception ex) { UO.PrintError(ex.Message); }
            }
        }

        [Command]
        public void lotfood()
        {
            if (_Food)
            {
                _Food = false;
                
                UO.PrintError("Lot Jidla Off");
            }
            else
            {
                _Food = true;
                UO.PrintInformation("Lot Jidla On");
            }
            Reload();
        }
        [Command]
        public void lotgems()
        {
            if (_Gems)
            {
                _Gems = false;
                UO.PrintError("Lot Drahokamu Off");
            }
            else
            {
                _Gems = true;
                UO.PrintInformation("Lot Drahokamu On");
            }
            Reload();
        }

        [Command]
        public void lotregs()
        {
            if (_Regeants)
            {
                _Regeants = false;
                UO.PrintError("Lot Regu Off");
            }
            else
            {
                _Regeants = true;
                UO.PrintInformation("Lot Regu On");
            }
            Reload();
        }

        [Command]
        public void lotfeathers()
        {
            if (_Feathers)
            {
                _Feathers = false;
                UO.PrintError("Lot Peri Off");
            }
            else
            {
                _Feathers = true;
                UO.PrintInformation("Lot Peri On");
            }
            Reload();
        }

        [Command]
        public void lotbolts()
        {
            if (_Bolts)
            {
                _Bolts = false;
                UO.PrintError("Lot Sipek Off");
            }
            else
            {
                _Bolts = true;
                UO.PrintInformation("Lot Sipek On");
            }
            Reload();
        }

        [Command]
        public void lotleather()
        {
            if (_Leather)
            {
                _Leather = false;
                UO.PrintError("Lot Kuzi Off");
            }
            else
            {
                _Leather = true;
                UO.PrintInformation("Lot Kuzi On");
            }
            Reload();
        }

        [Command]
        public void lotex1()
        {
            if (_Extend1)
            {
                _Extend1 = false;
                UO.PrintError("Lot Ex1 Off");
            }
            else
            {
                _Extend1 = true;
                UO.PrintInformation("Lot Ex1 On");
            }
            Reload();
        }

        [Command]
        public void lotex2()
        {
            if (_Extend2)
            {
                _Extend2 = false;
                UO.PrintError("Lot Ex2 Off");
            }
            else
            {
                _Extend2 = true;
                UO.PrintInformation("Lot Ex2 On");
            }
            Reload();
        }

        [Command]
        public void lotsetex1()
        {
            UO.PrintWarning("Zamer item");
            Extend1 = new UOItem(UIManager.TargetObject()).Color;

        }
        [Command]
        public void lotsetex2()
        {
            UO.PrintWarning("Zamer item");
            Extend2 = new UOItem(UIManager.TargetObject()).Color;
        }


        public Autolot()
        {
            GoldLimit = 5000;

            LotRunning = false;

            Aliases.SetObject("LotBackpack", 0xFFFFFFF);
            Aliases.SetObject("CarvTool", 0xFFFFFFF);
        }


        public static void Reload()
        {
            if ((AutolotState & AutolotFlag.Bolts) != 0)
            {
                if (!_Bolts) _Bolts = true;
            }
            else
            {
                if (_Bolts) _Bolts = false;
            }

            if ((AutolotState & AutolotFlag.Extend1) != 0)
            {
                if (!_Extend1) _Extend1 = true;
            }
            else
            {
                if (_Extend1) _Extend1 = false;
            }

            if ((AutolotState & AutolotFlag.Extend2) != 0)
            {
                if (!_Extend2) _Extend2 = true;
            }
            else
            {
                if (_Extend2) _Extend2 = false;
            }

            if ((AutolotState & AutolotFlag.Feathers) != 0)
            {
                if (!_Feathers) _Feathers = true;
            }
            else
            {
                if (_Feathers) _Feathers = false;
            }

            if ((AutolotState & AutolotFlag.Food) != 0)
            {
                if (!_Food) _Food = true;
            }
            else
            {
                if (_Food) _Food = false;
            }

            if ((AutolotState & AutolotFlag.Gems) != 0)
            {
                if (!_Gems) _Gems = true;
            }
            else
            {
                if (_Gems) _Gems = false;
            }

            if ((AutolotState & AutolotFlag.Leathers) != 0)
            {
                if (!_Leather) _Leather = true;
            }
            else
            {
                if (_Leather) _Leather = false;
            }

            if ((AutolotState & AutolotFlag.Regeants) != 0)
            {
                if (!_Regeants) _Regeants = true;
            }
            else
            {
                if (_Regeants) _Regeants = false;
            }
        }

        [Command,BlockMultipleExecutions]
        public void lotset()
        {
            UO.Print("Zamer lotovaci batoh");
            Aliases.SetObject("LotBackpack", UIManager.TargetObject());
            UO.Wait(200);
            UO.Print("Zamer nuz");
            Aliases.SetObject("CarvTool", UIManager.TargetObject());
        }


        [Command]
        public void lot()
        {
            if (LotRunning)
            {
                LotRun.Elapsed -= LotRun_Elapsed;
                LotRunning = false;
                LotRun.Stop();
                LotRun.Dispose();
                LotRun = null;
                UO.PrintError("Lot off");
            }
            else
            {
                LotRunning = true;
                LotRun = new System.Timers.Timer(300);
                LotRun.Elapsed += LotRun_Elapsed;
                LotRun.Start();
                UO.PrintInformation("Lot On");
            }
        }


        [Command("lotzem"),BlockMultipleExecutions]
        public void lotground()
        {
            World.FindDistance = 5;
            foreach (UOItem i in World.Ground.Where(item => item.Distance<3 && LotItems.Any(li => item.Graphic == li.Key)).ToList())
            {
                UO.MoveItem(i, ushort.MaxValue, Aliases.GetObject("LotBackpack") == 0xFFFFFFF ? World.Player.Backpack : Aliases.GetObject("LotBackpack"));
                UO.Wait(300);
            }
        }

        private void LotRun_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            LotRun.Elapsed -= LotRun_Elapsed;
            if (LotRunning) Checker();
            UO.Wait(200);
            LotRun.Elapsed += LotRun_Elapsed;
        }


        private void Checker()
        {
            if (CarvProgress) return;
            // Check Gold & mesec
            if (World.Player.Backpack.AllItems.FindType(0x0EED).Amount >= GoldLimit)
            {
                UO.Say(".mesec");
            }
            // scissors and leather
            if (leather)
            {
                UOItem scissors = null;
                foreach (var it in World.Player.Backpack.AllItems.Where(x => x.Graphic == 0x0F9E || x.Graphic == 0x0F9F))
                {
                    scissors = it;
                }
                if (scissors != null)
                {
                    foreach (var le in World.Player.Backpack.AllItems.Where(x => x.Graphic == 0x1078))
                    {
                        le.WaitTarget();
                        scissors.Use();
                    }

                }
            }

            if(bolts)
            {
                foreach (var ar in World.Player.Backpack.AllItems.Where(x => x.Graphic == 0x0F3F && x.Color == 0x0000))
                {
                    ar.Click();
                    UO.Wait(100);
                    if (ar.Name == "arrow" || ar.Name == "arrows")
                        ar.DropHere(ushort.MaxValue);
                }
            }

            World.FindDistance = 4;
            if (!World.Player.Hidden)
            {
                foreach (UOItem it in World.Ground.Where(x => x.Graphic == 0x2006 & x.Distance < 4 & x.Items.CountItems()>0 & x.Items.CountItems() < 9).ToList()) 
                {
                    if (CarvProgress) return;
                    Lot(it);
                    UO.Wait(250);
                }
            }
        }

        private void Lot(UOItem it)
        {
            if (it.Items.CountItems() < 1 || it.Items.CountItems() > 9) return;
            foreach (var i in it.Items.Where(item => item.Container==it & LotItems.Any(li => item.Graphic == li.Key)).ToList())
            {
                UO.MoveItem(i, ushort.MaxValue, Aliases.GetObject("LotBackpack") == 0xFFFFFFF ? World.Player.Backpack : Aliases.GetObject("LotBackpack"));
                UO.Wait(250);
            }
        }


        [Command("kuch")]
        public void Carving()
        {
            try
            {
                if (Aliases.GetObject("CarvTool") == 0xFFFFFFF)
                {
                    UO.PrintError("Neni nastaven nuz");
                    return;
                }
                World.FindDistance = 4;
                CarvProgress = true;
                foreach (UOItem it in World.Ground.Where(x => x.Distance < 4 && toCarv.Any(p => x.Graphic == p)))
                {
                    if (IgnoreList.Contains(it)) continue;
                    it.Click();
                    UO.Wait(200);
                    if (jezdidla.Contains(it.Name.ToLower())) continue;

                    it.WaitTarget();
                    new UOItem(Aliases.GetObject("CarvTool")).Use();
                    UO.Wait(300);
                    IgnoreList.Add(it);
                }
            }
            finally
            {

                new UOItem(Aliases.GetObject("ActualWeapon")).Equip();
                new UOItem(Aliases.GetObject("ActualShield")).Use();
                CarvProgress = false;
            }

        }

    }
}
