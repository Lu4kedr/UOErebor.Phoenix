using Phoenix.Communication;
using Phoenix.Plugins.Autoheal;
using Phoenix.Plugins.Equips;
using Phoenix.Plugins.Weapons;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phoenix.Plugins
{
    [Serializable]
    public class SaveClass
    {
        private event EventHandler<StoodUpEventHandlerArgs> StoodUp;
        private bool autoMorf,printStoodUps;
        private DateTime LastHitTime;
        private string texttemp;


        public bool Klerik
        {
            get
            {
                return AutoHeal.CleanBandage;
            }
            set
            {
                AutoHeal.CleanBandage = value;
            }
        }
        public string CrystalCmd
        {
            get
            {
                return AutoHeal.CrystalCmd;
            }
            set
            {
                AutoHeal.CrystalCmd = value;
            }
        }
        public HealedPlayers Healedplayers
        {
            get
            {
                return AutoHeal.Healedplayers;
            }
            set
            {
                AutoHeal.Healedplayers = value;
            }
        }

        public int MinToBandage
        {
            get
            {
                return AutoHeal.MinimalPlayerHP;
            }
            set
            {
                AutoHeal.MinimalPlayerHP = value;
            }
        }
        public List<EqSet> Equipy
        {
            get
            {
                return Equips.Equips.equipy;
            }
            set
            {
                Equips.Equips.equipy = value;  
            }
        }
        public List<WeaponSet> Weapons
        {
            get
            {
                return Phoenix.Plugins.Weapons.Weapons.weapons;
            }
            set
            {
                Phoenix.Plugins.Weapons.Weapons.weapons = value;
            }
        }
        public uint ActualWeapon
        {
            get
            {
                return Aliases.GetObject("ActualWeapon");
            }
            set
            {
                Aliases.SetObject("ActualWeapon", value);
            }
        }
        public uint ActualShield
        {
            get
            {
                return Aliases.GetObject("ActualShield");
            }
            set
            {
                Aliases.SetObject("ActualShield", value);
            }
        }

        public string AutolotState
        {
            get
            {
                return Autolot.AutolotState.ToString();
            }
            set
            {
                texttemp = value;
                Autolot.AutolotState = (AutolotFlag)Enum.Parse(typeof(AutolotFlag), value);
            }
        }

        public int GoldLimit
        {
            get
            {
                return Phoenix.Plugins.Autolot.GoldLimit;
            }
            set
            {
                Phoenix.Plugins.Autolot.GoldLimit = value;
            }
        }
        public bool AutoHarm
        {
            get
            {
                return Other.AutoUnParalyze;
            }
            set
            {
                Other.AutoUnParalyze = value;
            }
        }

        public bool PrintHp
        {
            get
            {
                return Other.HPPrint;
            }
            set
            {
                Other.HPPrint = value;
            }
        }

        public bool OnHitBandage
        {
            get
            {
                return Other.OnHitBandage;
            }
            set
            {
                Other.OnHitBandage = value;
            }
        }
        public bool HarmArrow { get; set; }
        public ushort PoisColor
        {
            get
            {
                return Poisoning.PoisonColor;
            }
            set
            {
                Poisoning.PoisonColor = value;
            }
        }
        public List<string> TrackIgnored
        {
            get
            {
                return Tracking.Ignored;
            }
            set
            {
                Tracking.Ignored = value;
            }
        }
        public uint LotBackpack
        {
            get
            {
                return Aliases.GetObject("LotBackpack");
            }
            set
            {
                Aliases.SetObject("LotBackpack", value);
            }
        }
        public uint CarvTool
        {
            get
            {
                return Aliases.GetObject("CarvTool");
            }
            set
            {
                Aliases.SetObject("CarvTool", value);
            }
        }

        public bool AutoMorf
        {
            get
            {
                return autoMorf;
            }
            set
            {
                if(value)
                {
                    Core.UnregisterServerMessageCallback(0x77, automorf);
                    Core.RegisterServerMessageCallback(0x77, automorf);
                    UO.Print("Morf ON");
                }
                else
                {
                    Core.UnregisterServerMessageCallback(0x77, automorf);
                    UO.Print("Morf Off");
                }
                autoMorf = value;
            }
        }
        public bool PrintStoodUp {
        get
            {
                return printStoodUps;
            }
            set
            {
                if(value)
                {
                    Core.UnregisterServerMessageCallback(0x6E, onStoodUp);
                    Core.RegisterServerMessageCallback(0x6E, onStoodUp);
                    StoodUp += Main_StoodUp;
                    UO.Print("Naprahy ON");
                }
                else
                {
                    Core.UnregisterServerMessageCallback(0x6E, onStoodUp);
                    StoodUp -= Main_StoodUp;
                    UO.Print("Naprahy Off");
                }
                printStoodUps = value;
            }
        }



        public List<Runes.Rune> Runes
        {
            get
            {
                return Phoenix.Plugins.Runes.RuneTree.Runes;
            }
            set
            {
                Plugins.Runes.RuneTree.Runes = value;
            }
        }

        public bool AutoDrink { get; set; }
        public int CriticalHP { get; set; }

        public SaveClass()
        {

            Klerik = true;
            CrystalCmd = ".enlightment";
            Healedplayers = new HealedPlayers();
            Equipy = new List<EqSet>();
            Weapons = new List<WeaponSet>();

            AutoDrink = true;
            GoldLimit = 6000;
            AutoHarm = true;
            PoisColor = 0;
            TrackIgnored = new List<string>();
            LotBackpack = 0;
            CarvTool = 0;
            LastHitTime = DateTime.Now;
            HarmArrow = true;
            AutoMorf = true;

        }



        CallbackResult automorf(byte[] data, CallbackResult prev)
        {

            PacketReader reader = new PacketReader(data);
            reader.Skip(5);
            ushort model = reader.ReadUInt16();
            if ((model == 0x000A) || (model == 0x0009)) model = 39;//demon-summ
            if ((model == 0x00AD)) model = 11;//elder spider
            if ((model == 0x002F)) model = 990;//repear 
            if ((model == 0x003A)) model = 990;//spirit 
            ByteConverter.BigEndian.ToBytes((ushort)model, data, 5);
            return CallbackResult.Normal;

        }

        CallbackResult onStoodUp(byte[] data, CallbackResult prev)
        {

            PacketReader p = new PacketReader(data);
            p.Skip(1);
            uint serial = p.ReadUInt32();
            ushort action = p.ReadUInt16();
            if (StoodUp != null && (action == 26 || action == 11 || action == 29))
            {
                foreach (EventHandler<StoodUpEventHandlerArgs> ev in StoodUp.GetInvocationList())
                {
                    ev.BeginInvoke(this, new StoodUpEventHandlerArgs() { action = action, serial = serial }, null, null);
                }
            }

            return CallbackResult.Normal;
        }
        private void Main_StoodUp(object sender, StoodUpEventHandlerArgs e)
        {

            if ((e.action == 26 || e.action == 11 || e.action == 29) && e.serial == World.Player.Serial && new UOCharacter(Aliases.LastAttack).Distance < 2)
            {
                if (e.action == 29)
                {
                    UO.Wait(200);
                    if (DateTime.Now - LastHitTime < TimeSpan.FromMilliseconds(200)) return;
                }
                World.Player.Print(0x0077, "Naprah na " + new UOCharacter(Aliases.LastAttack).Name);
            }
        }

        private class StoodUpEventHandlerArgs : EventArgs
        {
            public ushort action { get; set; }
            public uint serial { get; set; }
        }
    }
}
