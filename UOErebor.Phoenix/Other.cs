﻿using Phoenix.Communication;
using Phoenix.Communication.Packets;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Phoenix.Plugins
{
    public class Other
    {
        private int x;
        private int Exp;
        readonly static string[] onParaCalls = { "nohama ti projela silna bolest", "citis, ze se nemuzes hybat.", " crying awfully." };
        readonly static string[] onHitCalls =  { "Tvuj cil krvaci.", "Skvely zasah!", "Kriticky zasah!", "Vysavas staminu", "Vysavas zivoty!" };
        readonly static string[] TextFilterarr = { "you put ", " cancelled", "unexpected", " way to use that "};
        private static bool autoUnParalyze;
        private static bool onHitBandage;
        private static bool hpPrint;
        private static bool onHitVet;

        public static event EventHandler OnParalyze;

        public static bool OnHitBandage
        {
            get
            {
                return onHitBandage;
            }
            set
            {
                if(value)
                {
                    Core.UnregisterServerMessageCallback(0x1C, OnHitBand);

                    Core.RegisterServerMessageCallback(0x1C, OnHitBand);
                }
                else
                {
                    Core.UnregisterServerMessageCallback(0x1C, OnHitBand);
                }
                onHitBandage = value;
            }
        }
        public static bool OnHitVet
        {
            get
            {
                return onHitVet;
            }
            set
            {
                if (value)
                {
                    Core.UnregisterServerMessageCallback(0x1C, OnHitVete);

                    Core.RegisterServerMessageCallback(0x1C, OnHitVete);
                }
                else
                {
                    Core.UnregisterServerMessageCallback(0x1C, OnHitVete);
                }
                onHitVet = value;
            }
        }
        
        public static bool AutoUnParalyze
        {
            get
            {
                return autoUnParalyze;
            }
            set
            {
                if (value)
                {
                    Core.UnregisterServerMessageCallback(0x1C, OnParaCalls);
                    Core.RegisterServerMessageCallback(0x1C, OnParaCalls);
                }
                else
                {
                    Core.UnregisterServerMessageCallback(0x1C, OnParaCalls);

                }
                autoUnParalyze = value;
            }
        }

        public static bool HPPrint
        {
            get
            {
                return hpPrint;
            }
            set
            {
                if(value)
                {

                    Core.RegisterServerMessageCallback(0xA1, onHpChanged);
                }
                else
                    Core.UnregisterServerMessageCallback(0xA1, onHpChanged);
                hpPrint = value;
            }

        }

        [Command]
        public void hpprint()
        {
            if (HPPrint)
            {
                HPPrint = false;
                UO.PrintError("Vypis HP vypnut");
            }
            else
            {
                HPPrint = true;
                UO.PrintInformation("Vypis HP zapnut");
            }
        }
        public Other()
        {
            Exp = 0;

        }



        [Command, BlockMultipleExecutions]
        public void friend()
        {
            foreach (UOCharacter sum in World.Characters)
            {
                if (sum.Renamable || sum.Notoriety == Notoriety.Innocent)
                {
                    UO.Say("all friend");
                    UO.WaitTargetObject(sum.Serial);
                    UO.Wait(100);
                }
                UO.Wait(100);
            }
        }

        [Command, BlockMultipleExecutions]
        public void kill()
        {



            int wait = 100;
            foreach (UOCharacter enemy in World.Characters.Where(c => c.Notoriety > Notoriety.Criminal && c.Distance < 12).ToList())
            {
                if (enemy.Renamable) continue;
                if (enemy.Distance > 11) continue;
                if (enemy.Notoriety == Notoriety.Enemy || enemy.Notoriety == Notoriety.Murderer)
                {
                    UO.Say("all kill");
                    UO.WaitTargetObject(enemy.Serial);
                    UO.Wait(wait = wait + 25);
                }
                UO.Wait(100);
            }
        }

        [Command("vyber"), BlockMultipleExecutions]
        public void TakeAllFrom()
        {
            TakeAllFrom(10);
        }

        [Command("vyberpvp"), BlockMultipleExecutions]
        public void TakeAllFromPVP()
        {
            TakeAllFrom(1250);
        }
        [Command("vyber"), BlockMultipleExecutions]
        public void TakeAllFrom(int delay)
        {
            UO.PrintInformation("Zamer kontainer ktery chces vybrat");
            Serial target = UIManager.TargetObject();
            if (target == Aliases.Self)
            {
                UO.PrintError("Nezameruj sebe");
                return;
            }
            new UOItem(target).Use();
            UO.Wait(100);
            foreach (var it in new UOItem(target).AllItems)
            {
                it.Move(ushort.MaxValue, Aliases.GetObject("LotBackpack")); // TODO lotbackpack
                UO.Wait(delay);
            }
        }





        [Command("presun"), BlockMultipleExecutions]
        public void MoveX(int amount)
        {
            var i = 0;
            UO.PrintInformation("Odkud");
            UOItem from = new UOItem(UIManager.TargetObject());
            UO.PrintInformation("Kam");
            UOItem to = new UOItem(UIManager.TargetObject());
            UO.Wait(300);
            foreach (var it in from.AllItems)
            {
                if (i >= amount) break;
                it.Move(1, to);
                UO.Wait(100);
                i++;
            }
        }

        [Command("id"), BlockMultipleExecutions]
        public void Identificate()
        {
            UO.PrintInformation("Bagl s NeID itemy");
            UOItem from = new UOItem(UIManager.TargetObject());
            UO.Wait(300);
            World.Player.WaitTarget();
            UO.Say("identifikace");
            UO.Wait(100);
            foreach (var it in from.AllItems)
            {
                it.Click();
                UO.Wait(200);
                if (it.Name == "Magicky predmet")
                {
                    it.WaitTarget();
                    UO.Say("identifikace");
                }
                UO.Wait(100);
            }
        }



        [Command]
        public void war()
        {
            if (World.Player.Warmode)
            {
                UO.Warmode(false);
                UO.PrintInformation("Warmode off");
            }

            else
            {
                UO.Warmode(true);
                UO.PrintInformation("Warmode on");
            }
        }


        [Command]
        public void clearexp()
        {
            Exp = 0;
        }

        [Command]
        public void exp()
        {
            UO.PrintInformation("ziskano zkusenosti: {0}", Exp);
        }


        [Command]
        public void rename(string newName)
        {
            UOCharacter ch = new UOCharacter(UIManager.TargetObject());
            PacketWriter pw = new PacketWriter(0x75);
            pw.Write((UInt32)ch.Serial);
            var tmp = newName;
            while (tmp.Length != 30)
            {
                tmp += " ";
            }
            pw.WriteAsciiString(tmp, tmp.Length);
            Core.SendToServer(pw.GetBytes());

        }

        [Command]
        public void rename(Serial Target,string newName)
        {

            PacketWriter pw = new PacketWriter(0x75);
            pw.Write((UInt32)Target);
            var tmp = newName;
            while (tmp.Length != 30)
            {
                tmp += " ";
            }
            pw.WriteAsciiString(tmp, tmp.Length);
            Core.SendToServer(pw.GetBytes());

        }

        [ServerMessageHandler(0x1C, Priority = CallbackPriority.Lowest)]
        public CallbackResult onExp(byte[] data, CallbackResult prevResult)
        {
            AsciiSpeech packet = new AsciiSpeech(data);
            if (packet.Text.Contains(" zkusenosti."))
            {

                //string[] numbers = Regex.Split(packet.Text, @"\D+");
                string number = Regex.Match(packet.Text, @"-?\d+").Value;
                if (!string.IsNullOrEmpty(number))
                {
                    Exp += int.Parse(number);

                }
            }
            return CallbackResult.Normal;
        }

        [ServerMessageHandler(0x1C)]
        public CallbackResult textFilter(byte[] data, CallbackResult prevResult)
        {
            AsciiSpeech speech = new AsciiSpeech(data);
            foreach (string s in TextFilterarr)
            {
                if (speech.Text.ToLowerInvariant().Contains(s))
                {
                    return CallbackResult.Eat;
                }
            }
            return CallbackResult.Normal;
        }

        [ServerMessageHandler(0x22, Priority = CallbackPriority.Lowest)]
        public CallbackResult OnWalkRequestSucceeded(byte[] data, CallbackResult prevResult)
        {
            if (World.Player.Hidden)
            {
                if (prevResult < CallbackResult.Sent)
                {
                    if (x % 5 == 0) UO.Print(0x011C, "Stealth : {0}", x);
                    x++;
                }
            }
            else
            {
                x = 1;
            }
            return CallbackResult.Normal;
        }

        [ServerMessageHandler(0x65)]//weather
        public CallbackResult Filter(byte[] data, CallbackResult prevResult) { return CallbackResult.Eat; }


        [ServerMessageHandler(0x4F)]
        public CallbackResult OnSunLight(byte[] data, CallbackResult prevResult)
        {
            if (prevResult < CallbackResult.Sent)
            {
                if (data[1] > 16)//max 31-tma
                {
                    byte[] newData = new byte[2];
                    newData[0] = 0x4F;
                    newData[1] = (byte)17;
                    Core.SendToClient(newData);

                    // UO.Print(0x015C, "Light level fixed.");
                    return CallbackResult.Sent;
                }
            }
            return CallbackResult.Normal;
        }

        [ServerMessageHandler(0x11)]
        public CallbackResult OnNextTarget(byte[] data, CallbackResult prevResult)
        {
            PacketReader reader = new PacketReader(data);
            if (reader.ReadByte() != 0x11) throw new Exception("Invalid packet passed to OnNextTarget method.");
            ushort blockSize = reader.ReadUInt16();
            uint serial = reader.ReadUInt32();
            if (serial == Aliases.Self)
            {
                return CallbackResult.Normal;
            }
            Aliases.SetObject("laststatus", serial);
            UOCharacter cil = World.GetCharacter(serial);
            if (cil.Hits < 0 || cil.Hits > 10000)
            {
                cil.RequestStatus(20);
                return CallbackResult.Normal;
            }
            else
            {
                ushort color = 0;
                string not = cil.Notoriety.ToString();
                switch (not)
                {

                    case "Criminal":
                        color = 0x0026;
                        break;

                    case "Enemy":
                        color = 0x0031;
                        break;

                    case "Guild":
                        color = 0x0B50;
                        break;

                    case "Innocent":
                        color = 0x0058;
                        break;

                    case "Murderer":
                        color = 0x0026;
                        break;

                    case "Neutral":
                        color = 0x03BC;
                        break;
                    case "Unknown":
                        color = 0x03BC;
                        break;
                    default:
                        color = Phoenix.Env.DefaultInfoColor;
                        break;
                }
                UO.Print(color, "{0} : {1}/{2} ({3})", cil.Name, cil.Hits, cil.MaxHits, cil.Distance);
                return CallbackResult.Normal;
            }
        }


        public static CallbackResult onHpChanged(byte[] data, CallbackResult prevResult)//0xa1
        {
            UOCharacter character = new UOCharacter(Phoenix.ByteConverter.BigEndian.ToUInt32(data, 1));
            //if (character.Serial == World.Player.Serial) return CallbackResult.Normal;
            ushort maxHits = 100; // Nejvyssi HITS bez nakouzleni
            ushort hits = Phoenix.ByteConverter.BigEndian.ToUInt16(data, 7);
            ushort[] color = new ushort[4];
            color[0] = 0x0026;//red
            color[2] = 0x0175;//green
            color[1] = 0x099;//yellow
            color[3] = 0x0FAB;//fialova - enemy;
            int col = 0;



            if (Math.Abs(character.Hits - hits) > 2) 
            {
                if (character.Renamable)
                {
                    character.Print(0x005d, "[{0}% HP]   {1}", ((maxHits / 100) * hits), (hits - character.Hits));
                    return CallbackResult.Normal;

                }
                if (character.Hits > hits)
                {
                    if (character.Poisoned) col = 2;
                    else col = 0;
                }
                else
                {
                    if (character.Poisoned) col = 2;
                    else col = 1;
                }

                if ((character.Model == 0x0190 || character.Model == 0x0191))
                {
                    if (character.Serial == World.Player.Serial)
                    {
                        character.Print(color[col], "[{0}% HP]   {1}", ((maxHits / 100) * hits), (hits - character.Hits));


                    }
                    else
                        character.Print(color[col], "{2} [{0}% HP]   {1}", ((maxHits / 100) * hits), (hits - character.Hits), character.Name);
                }



                if (character.Serial == Aliases.LastAttack)
                    character.Print(color[3], "[{0}% HP]   {1}", ((maxHits / 100) * hits), (hits - character.Hits));

            }
            return CallbackResult.Normal;
        }

        static CallbackResult OnParaCalls(byte[] data, CallbackResult prev)
        {
            AsciiSpeech speech = new AsciiSpeech(data);

            foreach (string s in onParaCalls)
            {

                if (speech.Text.ToLowerInvariant().Contains(s))
                {
                    if(OnParalyze!=null)
                    {
                        OnParalyze.BeginInvoke(null, new EventArgs(), null, null);
                    }
                    return CallbackResult.Normal;

                }
            }
            return CallbackResult.Normal;

        }

        static CallbackResult OnHitBand(byte[] data, CallbackResult prevResult)
        {
            AsciiSpeech packet = new AsciiSpeech(data);

            foreach (string s in onHitCalls)
            {
                if (packet.Text.Contains(s) && World.Player.Hits < World.Player.MaxHits - 10)
                {
                    Core.UnregisterServerMessageCallback(0x1C, OnHitBand);
                    UO.Say(",bandage");
                    Core.RegisterServerMessageCallback(0x1C, OnHitBand);
                    return CallbackResult.Normal;
                }
            }
            return CallbackResult.Normal;
        }

        static CallbackResult OnHitVete(byte[] data, CallbackResult prevResult)
        {
            AsciiSpeech packet = new AsciiSpeech(data);

            foreach (string s in onHitCalls)
            {
                if (packet.Text.Contains(s) && Veterinary.healed)
                {
                    Core.UnregisterServerMessageCallback(0x1C, OnHitVete);
                    UO.Say(",healpet");
                    Core.RegisterServerMessageCallback(0x1C, OnHitVete);
                    return CallbackResult.Normal;
                }
            }
            return CallbackResult.Normal;
        }

        [Command]
        public void dmg()
        {
            while(!UO.Dead)
            {
                UO.Warmode(false);
                UO.Press(System.Windows.Forms.Keys.PageUp);
                UO.Press(System.Windows.Forms.Keys.PageUp);
                while (World.Player.Hits > 120)
                    UO.Wait(100);
                UO.Press(System.Windows.Forms.Keys.End);
                UO.Press(System.Windows.Forms.Keys.End);
                while (World.Player.Hits < 130)
                    UO.Wait(100);
                UO.Wait(200);
            }
        }

        [Command]
        public void dropdispel()
        {
            try
            {
                var dBomb = World.Player.Backpack.AllItems.FindType(0x0F0E, 0x0993);
                UO.DropHere(1, dBomb);
            }
            catch
            {
                UO.PrintError("Nemas Dispel Bombu");
            }
        }
    }
}
