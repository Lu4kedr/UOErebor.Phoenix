using Lumber;
using Lumber.PathFinding;
using Phoenix;
using Phoenix.Communication;
using Phoenix.Plugins.Runes;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Lumber
{
    [RuntimeObject]
    public class Lumb
    {
        List<Field> Fields = new List<Field>();
        DateTime StartMine;
        List<string> TopMonster = new List<string>() { "golem", "spirit" };
        Graphic[] Humanoid = { 0x0191, 0x0190 };
        public string AlarmPath = Core.Directory + @"\afk.wav";

        private Serial DoorLeft;
        private Serial DoorRight;
        private Graphic DoorLeftClosedGraphic;
        private Graphic DoorRightClosedGraphic;
        private Settings Settings;
        private bool UseCrystal = false;
        private Check Check = new Check();
        private const int LumbDelay = 3300;
        private Point HomeLocation;
        private uint LumbRune=0x0;
        private Movement Mov;
        private uint LogsBox;
        private uint ResourceBox;
        private Graphic Log = 0x1BDD;
        private bool Stoodup=false;

        private int HomeDistance
        {
            get
            {
                return (int)(Math.Sqrt(Math.Pow(HomeLocation.X - World.Player.X, 2) + Math.Pow(HomeLocation.Y - World.Player.Y, 2)));
            }
        }

        public Lumb()
        {
            StartMine = DateTime.Now;
            HomeLocation = new Point(2730, 3273);
            Mov = new Movement();
            XmlSerializeHelper<Settings>.Load("Lumber", out Settings, false);
            if (Settings == null)
                Settings = new Settings();
            SettingsLoad();
            Core.Window.FormClosing += Window_FormClosing;
        }


        private void SettingsLoad()
        {
            DoorLeft = Settings.DoorLeft;
            DoorRight = Settings.DoorRight;
            DoorLeftClosedGraphic = Settings.DoorLeftClosedGraphic;
            DoorRightClosedGraphic = Settings.DoorRightClosedGraphic;
            UseCrystal = Settings.UseCrystal;
            HomeLocation = new Point(Settings.HomeLocationX, Settings.HomeLocationY);
            LumbRune = Settings.LumbRune;
            LogsBox = Settings.LogsBox;
            ResourceBox = Settings.ResourceBox;
            UO.PrintWarning("Lumber - Nastaveni Nacteno");

        }

        private void SetingsSave()
        {
            Settings.DoorLeft = DoorLeft;
            Settings.DoorRight = DoorRight;
            Settings.DoorLeftClosedGraphic = DoorLeftClosedGraphic;
            Settings.DoorRightClosedGraphic = DoorRightClosedGraphic;
            Settings.UseCrystal = UseCrystal;
            Settings.HomeLocationX = HomeLocation.X;
            Settings.HomeLocationY = HomeLocation.Y;
            Settings.LumbRune = LumbRune;
            Settings.LogsBox = LogsBox;
            Settings.ResourceBox = ResourceBox;
        }
        private void Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            SetingsSave();
            XmlSerializeHelper<Settings>.Save("Lumber", Settings, false);
        }
        #region Commands
        [Command,BlockMultipleExecutions]
        public void ldoorset()
        {
            UO.PrintWarning("Zamer leve zavrene dvere");
            DoorLeft = UIManager.TargetObject();
            UO.PrintWarning("Zamer prave zavrene dvere");
            DoorRight = UIManager.TargetObject();
            DoorLeftClosedGraphic = new UOItem(DoorLeft).Graphic;
            DoorRightClosedGraphic = new UOItem(DoorRight).Graphic;
            
        }

        [Command, BlockMultipleExecutions]
        public void lruneset()
        {
            UO.PrintWarning("Zamer runu do lesa a nasledne nahrej runy u postavy");
            LumbRune = UIManager.TargetObject();

        }

        [Command, BlockMultipleExecutions]
        public void lhomeloc()
        {
            UO.PrintWarning("Aktualni poloha ulozena vychozi, mela by mit nadosah runy a jidlo");
            HomeLocation = new Point(World.Player.X, World.Player.Y);

        }

        [Command, BlockMultipleExecutions]
        public void lboxset()
        {
            UO.PrintWarning("Zamer bednu v bance kam davat drevo");
            LogsBox = UIManager.TargetObject();

            UO.PrintWarning("Zamer bednu v bance ze ktere se budou brat battle axe, GH, invisky, warmace a recally");
            ResourceBox = UIManager.TargetObject();

        }

        [Command, BlockMultipleExecutions]
        public void crystal(bool Use)
        {
            UseCrystal = Use;
            if (UseCrystal)
                UO.PrintInformation("Crystal On");
            else UO.PrintError("Crystal Off");

        }
        #endregion
        [Command,BlockMultipleExecutions]
        public void Lumber()
        {
            try
            {
                int tmpx=0, tmpy=0;
                Check.Start();
                for (var x = HomeLocation.X - 50; x <= HomeLocation.X + 50; x++)
                {
                    for (var y = HomeLocation.Y - 50; y <= HomeLocation.Y + 50; y++)
                    {
                        var tmp = Field.GetField((ushort)x, (ushort)y);
                        if (tmp != null) Fields.Add(tmp);
                    }
                }
                // Pripad kdy zaciname v domovske pozici - GH
                if (HomeDistance < 18)
                {
                    MoveTo(HomeLocation);
                    tmpx = World.Player.X;
                    tmpy = World.Player.Y;
                    Recall(1);
                }

                // Find trees and walkable fields
                for (var x = World.Player.X - 200; x <= World.Player.X + 200; x++)
                {
                    for (var y = World.Player.Y - 200; y <= World.Player.Y + 200; y++)
                    {
                        var tmp = Field.GetField((ushort)x, (ushort)y);
                        if (tmp != null) Fields.Add(tmp);
                    }
                }



                UO.Print("Nacteno {0} poli", Fields.Count);
                while (World.Player.X == tmpx || World.Player.Y == tmpy) UO.Wait(200);

                var tmpCn = World.Player.Layers.Count(x => x.Graphic == 0x0F47 || x.Graphic == 0x0F48);
                if (tmpCn > 0)
                {
                    new UOItem(World.Player.Layers.First(x => x.Graphic == 0x0F47 || x.Graphic == 0x0F48)).Equip();
                }

                var tmptrees = Fields.Where(x => x.IsTree & x.Distance<20 & (Math.Abs(x.Tree.Z - World.Player.Z) < 3) & (x.Tree.Harvested == DateTime.MinValue || (DateTime.Now - x.Tree.Harvested) > TimeSpan.FromMinutes(30))).ToList();

                while (true)
                {
                    // znovu najde v okoli 20 poli stromy pokud predesly list je temer vytezen
                    UO.PrintInformation("Debug: hledani stromu");
                    if (tmptrees.Count(x => x.Tree.Harvested == DateTime.MinValue || (DateTime.Now - x.Tree.Harvested) > TimeSpan.FromMinutes(30)) < 5)
                        tmptrees = Fields.Where(x => x.IsTree & x.Distance < 20 & (Math.Abs(x.Tree.Z - World.Player.Z) < 3) & (x.Tree.Harvested == DateTime.MinValue || (DateTime.Now - x.Tree.Harvested) > TimeSpan.FromMinutes(30))).ToList();
                    UO.PrintWarning("Debug: nalezeno celkem {0} stromu, zbyva vytezit {1}", tmptrees.Count(x=>x.IsTree), tmptrees.Count(x => x.Tree.Harvested == DateTime.MinValue || (DateTime.Now - x.Tree.Harvested) > TimeSpan.FromMinutes(30)));

                    UO.PrintInformation("Debug: hledani nejblizsiho stromu");
                    var trees = tmptrees.Where(x => x.IsTree & Math.Abs(x.Tree.Z - World.Player.Z) < 3 & (x.Tree.Harvested == DateTime.MinValue || (DateTime.Now - x.Tree.Harvested) > TimeSpan.FromMinutes(30))).ToList();

                    UO.PrintInformation("Debug: trizeni stromu");
                    trees.Sort((x, y) => x.Distance.CompareTo(y.Distance));
                    int tmpPi = 0;
                    var tmpP = trees[tmpPi].ClosestWalkable;
                    UO.PrintInformation("Debug: hledani pole pro tezeni nalezeneho stromu");
                    while (tmpP.X == -1 || tmpP.Y == -1)
                    {
                        tmpP = trees[++tmpPi].ClosestWalkable;
                        if (tmpPi > 50)
                        {
                            UO.PrintError("Nenalezene stromy !!");
                            UO.TerminateAll();
                        }
                    }
                    UO.PrintInformation("Debug: presun ke stromu");
                    while(!MoveTo(tmpP))
                    {
                        UO.PrintError("Neuspesny presun na pozici");
                        UO.PrintInformation("Debug: hledani pole pro tezeni nasledujciho stromu");
                        tmpP = trees[++tmpPi].ClosestWalkable;
                        while (tmpP.X == -1 || tmpP.Y == -1)
                        {
                            tmpP = trees[++tmpPi].ClosestWalkable;
                            if (tmpPi > 50)
                            {
                                UO.PrintError("Nenalezene stromy !!");
                                UO.TerminateAll();
                            }
                        }
                    }

                    UO.PrintInformation("Debug: hledani stromu ve vzdalenosti 2 pole");
                    var tr = Fields.Where(x => x.IsTree & x.Distance < 3 & (x.Tree.Harvested == DateTime.MinValue || (DateTime.Now - x.Tree.Harvested) > TimeSpan.FromMinutes(30)));
                    UO.PrintInformation("Debug: tezeni okolnich stromu");
                    foreach (var tree in tr)
                    {
                        Mine(tree);
                        CheckCK();
                    }
                    UO.PrintInformation("Debug: Vytezeno");

                }
            }
            finally
            {
                Check.Stop();
            }
        }




        private void Mine(Field T,int Try=0)
        {
            if (StartMine == null) StartMine = DateTime.Now;
            int tmp = LumbDelay - (int)(DateTime.Now - StartMine).TotalMilliseconds;
            if (tmp > 0)
            {
                for (double i = 0; i < tmp; i += tmp / 10)
                {
                    if (i > 100) CheckCK();

                    UO.Wait(tmp / 10);
                }
            }
            if (Try == 0 & UseCrystal) UO.Say(".vigour");
            if (T.Mine())
            {
                StartMine = DateTime.Now;

                if (Try == 0 & UseCrystal)
                {
                    UO.Wait(1500);
                    UO.Say(".vigour");
                }
            }
            else
            {
                if (Try == 0 & UseCrystal)
                {
                    UO.Wait(1500);
                    UO.Say(".vigour");
                    return;

                }
            }
            CheckCK();
            UO.Wait(300);
            if (Check.NoLog)
            {
                Check.NoLog = false;
                T.Harvested();
                    
                if (Check.MaxedWeight)
                {
                    var sp = new Point(World.Player.X, World.Player.Y);
                    Unload();
                    MoveTo(sp);
                    Check.MaxedWeight = false;
                }
                return;
            }



            if(Check.SkillDelay)
            {
                Check.SkillDelay = false;
                UO.Wait(5500);
            }

            Mine(T,(Try+1));
        }



        private void Unload()
        {
            Recall(0);

            UOItem dltmp = new UOItem(DoorLeft);
            UOItem drtmp = new UOItem(DoorRight);
            if (dltmp.Graphic == DoorLeftClosedGraphic) dltmp.Use();
            if (drtmp.Graphic == DoorRightClosedGraphic) drtmp.Use();


            UO.Wait(200);
            MoveTo(HomeLocation);

            openBank(14);
            UO.Wait(500);
            UOItem box = new UOItem(LogsBox);
            box.Use();
            UO.Wait(500);
            UOItem resourceBox = new UOItem(ResourceBox);
            resourceBox.Use();
            UO.Wait(500);

            new UOItem(World.Player.Layers.First(x => x.Graphic == Log)).WaitTarget();
            UO.Say(".movetype");
            box.WaitTarget();
            UO.Wait(500);

            foreach (var it in World.Player.Backpack.Items.Where(x => x.Graphic == Log))
            {
                it.Move(ushort.MaxValue, box);
                UO.Wait(200);
            }

            UO.Wait(100);

            int tmpCnt = World.Player.Layers.Count(x => x.Graphic == 0x0F47 || x.Graphic == 0x0F48);
            if (tmpCnt == 0)
            {
                tmpCnt = resourceBox.AllItems.Count(x => x.Graphic == 0x0F47 || x.Graphic == 0x0F48);
                if (tmpCnt == 0)
                {
                    UO.PrintError("Nemas battle axe");
                    UO.TerminateAll();
                }
                else
                {
                    new UOItem(resourceBox.AllItems.First(x => x.Graphic == 0x0F47 || x.Graphic == 0x0F48)).Move(1, World.Player.Backpack);
                }
            }


            tmpCnt = World.Player.Layers.Count(x => x.Graphic == 0x1407 || x.Graphic == 0x1406);
            if (tmpCnt == 0)
            {
                tmpCnt = resourceBox.AllItems.Count(x => x.Graphic == 0x1407 || x.Graphic == 0x1406);
                if (tmpCnt == 0)
                {
                    UO.PrintError("Nemas war mace");
                    UO.TerminateAll();
                }
                else
                {
                    new UOItem(resourceBox.AllItems.First(x => x.Graphic == 0x1407 || x.Graphic == 0x1406)).Move(1, World.Player.Backpack);
                }
            }


            SelfFeed();
            resourceBox.Use();
            if (World.Player.Backpack.AllItems.FindType(0x1F4C).Amount < 8)
                resourceBox.AllItems.FindType(0x1F4C).Move(9, World.Player.Backpack);

            // GH
            if (World.Player.Backpack.AllItems.FindType(0x0F0E, 0x0160).Amount < 1)
                resourceBox.AllItems.FindType(0x0F0E, 0x0160).Move(2, World.Player.Backpack);

            // Invisky
            if (World.Player.Backpack.AllItems.FindType(0x0F0E, 0x0447).Amount < 1)
                resourceBox.AllItems.FindType(0x0F0E, 0x0447).Move(2, World.Player.Backpack);

            UO.Wait(200);
            Recall(1);

        }


        private void SelfFeed()
        {
            World.FindDistance = 4;
            World.Ground.FindType(0x097B).Use();
            UO.Wait(100);
            World.Ground.FindType(0x097B).Use();
            UO.Wait(100);
            World.Ground.FindType(0x097B).Use();
            UO.Wait(100);
            World.Ground.FindType(0x097B).Use();
            UO.Wait(100);
        }




        private void openBank(int equip)
        {
            Core.UnregisterServerMessageCallback(0xB0, onGumpBank);
            Core.RegisterServerMessageCallback(0xB0, onGumpBank);
            UO.Say(".equip{0}", equip);
            UO.Wait(600);
            Core.UnregisterServerMessageCallback(0xB0, onGumpBank);
        }


        private CallbackResult onGumpBank(byte[] data, CallbackResult prevResult)
        {
            byte cmd = 0xB1; //1 byte
            uint ID, gumpID;
            uint buttonID = 9; //4 byte
            uint switchCount = 0;
            uint textCount = 0;

            PacketReader pr = new PacketReader(data);
            if (pr.ReadByte() != 0xB0) return CallbackResult.Normal;
            pr.ReadInt16();
            ID = pr.ReadUInt32();
            gumpID = pr.ReadUInt32();


            PacketWriter reply = new PacketWriter();
            reply.Write(cmd);
            reply.WriteBlockSize();
            reply.Write(ID);
            reply.Write(gumpID);
            reply.Write(buttonID);
            reply.Write(switchCount);
            reply.Write(textCount);

            Core.SendToServer(reply.GetBytes());
            return CallbackResult.Sent;
        }


        private void Recall(int v)
        {

            int x = World.Player.X;
            int y = World.Player.Y;
            UO.Warmode(false);
            switch (v)
            {
                case 0:
                    while (World.Player.X == x | World.Player.Y == y)
                    {
                        while (World.Player.Mana < 20)
                        {
                            UO.UseSkill(StandardSkill.Meditation);
                            UO.Wait(2500);
                        }
                        UO.WaitTargetSelf();
                        UO.Say(".recallhome");
                        Journal.WaitForText(true, 10000, "Kouzlo se nezdarilo.");
                        Journal.ClearAll();
                        UO.Wait(200);
                    }
                    break;
                case 1:
                    MoveTo(HomeLocation);
                    while (World.Player.X == x || World.Player.Y == y)
                    {
                        while (World.Player.Mana < 20)
                        {
                            UO.UseSkill(StandardSkill.Meditation);
                            UO.Wait(2500);
                        }


                        foreach (Rune r in RuneTree.Runes.Where(run => run.Id.ToString() == LumbRune.ToString()))
                        {
                            RuneTree.findRune(r);
                            r.RecallSvitek();
                        }


                        Journal.ClearAll();
                        UO.Wait(500);
                        Journal.WaitForText(true, 10000, "Kouzlo se nezdarilo.");
                        UO.Wait(200);
                    }
                    break;
                case 2:
                    while (World.Player.X == x | World.Player.Y == y)
                    {
                        while (World.Player.Mana < 20)
                        {
                            UO.Wait(500);
                        }
                        UO.WaitTargetSelf();
                        UO.Say(".recallhome");
                        Journal.WaitForText(true, 10000, "Kouzlo se nezdarilo.");
                        Journal.ClearAll();
                        UO.Wait(200);
                    }
                    break;
            }


        }


        private void CheckCK()
        {
            World.FindDistance = 19;
            foreach (var ch in World.Characters)
            {
                if (ch.Notoriety > Notoriety.Criminal && ch.Notoriety < Notoriety.Invulnerable)
                {
                    int x = World.Player.X;
                    int y = World.Player.Y;
                    if (Humanoid.Any(c => c == ch.Model))
                    {
                        // CK

                        UO.Say(".potioninvis");
                        UO.Wait(100);
                        Recall(2);
                        System.Media.SoundPlayer my_wave_file = new System.Media.SoundPlayer(AlarmPath);
                        my_wave_file.Play();

                        while (!World.Player.Dead || World.Player.X == x || World.Player.Y == y) UO.Wait(200);

                        UOItem dltmp = new UOItem(DoorLeft);
                        UOItem drtmp = new UOItem(DoorRight);
                        if (dltmp.Graphic == DoorLeftClosedGraphic) dltmp.Use();
                        if (drtmp.Graphic == DoorRightClosedGraphic) drtmp.Use();


                        UO.Wait(200);
                        MoveTo(HomeLocation);
                    }
                    else
                    {
                        ch.Click();
                        UO.Wait(200);
                        if (TopMonster.Any(c => c == ch.Name.ToLowerInvariant()))
                        {
                            if (ch.Distance > 0)
                            {
                                UO.Say(".potioninvis");
                                UO.Say(".recallhome");
                                System.Media.SoundPlayer my_wave_file = new System.Media.SoundPlayer(AlarmPath);
                                my_wave_file.Play();
                                while (!World.Player.Dead || World.Player.X == x || World.Player.Y == y) UO.Wait(200);
                            }
                        }
                        else
                        {
                            System.Media.SoundPlayer my_wave_file = new System.Media.SoundPlayer(AlarmPath);
                            my_wave_file.Play();

                            try
                            {
                                var myPos = new Point(World.Player.X, World.Player.Y);
                                Battle(ch.Serial);
                                UO.Wait(100);
                                MoveTo(myPos);

                            }
                            catch (Exception ex) { UO.PrintError(ex.Message); }

                        }

                    }
                    return;
                }
            }
        }

        private bool MoveTo(int X, int Y)
        {
            if (World.Player.X == X && World.Player.Y == Y) return true;
            int x=0, y = 0;
            List<Point> tmp;
            tmp = GetWay(new Point(World.Player.X, World.Player.Y), new Point(X, Y));
            for (int i = 0; i < tmp.Count; i++)
            {
                if (i % 10 == 0)
                {
                    UO.Wait(100);
                    tmp = GetWay(new Point(World.Player.X, World.Player.Y), new Point(X, Y));
                    i = 0;
                }
                x = World.Player.X;
                y = World.Player.Y;
                Mov.moveToPosition(tmp[i]);
                if (World.Player.X == x && World.Player.Y == y) return false;

            }
            return true;

        }

        private bool MoveTo(Point location)
        {
           return MoveTo(location.X, location.Y);
        }

        private List<Point> GetWay(Point StartPosition, Point EndPosition)
        {
            var searchParams = new SearchParameters(StartPosition, EndPosition, Fields);
            var pathfinder = new PathFinder(searchParams);
            return pathfinder.FindPath();
        }


        public void moveXField(int distance)
        {
            
            Fields.Sort((a, b) => a.Distance.CompareTo(b.Distance));
            Field tmp;
            try
            {
                tmp = Fields.First(x => x.Distance >= distance & x.IsWalkable);
                MoveTo(tmp.X, tmp.Y);
            }
            catch
            {
                tmp = null;
                UO.PrintError("Nenalezeno pole");
            }
        }



        public void Battle(Serial target)
        {
            Journal.Clear();
            Stoodup = false;
            UOCharacter enemy = new UOCharacter(target);
            UOItem weapon;
            var tmpCnt = World.Player.Layers.Count(x => x.Graphic == 0x1407 || x.Graphic == 0x1406);
            if (tmpCnt > 0)
            {
                weapon = new UOItem(World.Player.Layers.First(x => x.Graphic == 0x1407 || x.Graphic == 0x1406));
            }
            else
            {
                MessageBox.Show("Nemas zbran ");
                return;
            }
            try
            {
                weapon.Equip();
                try
                {
                    UO.Attack(enemy);
                }
                catch { }
                Core.RegisterServerMessageCallback(0x6E, onStoodUp);
                while (!Journal.Contains(true, "Ziskala jsi ", "Ziskal jsi ", "gogo"))
                {
                    if (enemy.Hits < 1 || enemy.Hits > 1000 || enemy.Dead) break;
                    if(enemy.Distance>1) MoveTo(enemy.X, enemy.Y);
                    if (Journal.Contains("Vysavas zivoty!"))
                    {
                        Journal.SetLineText(Journal.Find("Vysavas zivoty!"), " ");
                        UO.RunCmd("bandage");
                        weapon.Equip();
                        try
                        {
                            UO.Attack(enemy);
                        }
                        catch { }
                    }
                    while (!Stoodup)
                    {
                        if (enemy.Hits < 1 || enemy.Hits > 1000 || enemy.Dead) break;
                            UO.Wait(300);
                    }
                    Stoodup = false;
                    moveXField(6);
                    UO.Wait(300);

                }
            }
            finally
            {
                var tmpCn = World.Player.Layers.Count(x => x.Graphic == 0x0F47 || x.Graphic == 0x0F48);
                if (tmpCn > 0)
                {
                    new UOItem(World.Player.Layers.First(x => x.Graphic == 0x0F47 || x.Graphic == 0x0F48)).Equip();
                }
                    Core.UnregisterServerMessageCallback(0x6E, onStoodUp);
            }
        }


        CallbackResult onStoodUp(byte[] data, CallbackResult prev)
        {

            PacketReader p = new PacketReader(data);
            p.Skip(1);
            uint serial = p.ReadUInt32();
            ushort action = p.ReadUInt16();
            if ((action == 26 || action == 11) && serial == World.Player.Serial && new UOCharacter(Aliases.LastAttack).Distance < 3)
            {
                UO.Print(SpeechFont.Normal, 0x0076, "Naprah na " + new UOCharacter(Aliases.LastAttack).Name);
                Stoodup = true;
 
            }

            return CallbackResult.Normal;
        }
    }
}
