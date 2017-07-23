using Phoenix.Communication.Packets;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phoenix.Plugins
{
    public class Bandage
    {
        private DateTime StartBandage;
        private DateTime StartRes;
        private System.Timers.Timer RFS;
        public static bool BandageDone { get; private set; }
        public static bool ResDone { get; private set; }
        readonly string[] bandageDoneCalls = { " byl uspesne osetren", "leceni se ti nepovedlo.", "prestal krvacet", " neni zranen.", "nevidis cil.", "cil je moc daleko." };
        readonly string[] ressurectionCalls = { "duch neni ve ", "ozivil jsi", "ozivila jsi" };


        public Bandage()
        {
            StartBandage = DateTime.Now;
            StartRes = DateTime.Now;
            ResDone = true;
            BandageDone = true;
        }


        [Command]
        public void bandage()
        {
            Heal(15);
            UOItem tmp = new UOItem(Aliases.GetObject("ActualWeapon"));
            new UOItem(Aliases.GetObject("ActualShield")).Use();
            if(tmp.Layer!=Layer.LeftHand)
            {
                if (World.Player.Mana == World.Player.MaxMana)
                    tmp.Equip();
                else
                {
                    World.Player.WaitTarget();
                    tmp.Use();
                }

            }
        }
        [Command]
        public void bandage(bool Shaman)
        {
            Heal(15, false);
        }

        [Command]
        public void bandage(int Equip, bool CleanBandage)
        {
            Heal(Equip, CleanBandage);
        }
        [Command, BlockMultipleExecutions]
        public void res()
        {
            Res();
        }
        [Command, BlockMultipleExecutions]
        public void shamanres()
        {
            Res(false);
        }

        private void Res(bool CleanBandage = true)
        {
            StartRes = DateTime.Now;
            bool first = true;
            UOItem bandage;
            if (!CleanBandage)
                bandage = new UOItem(World.Player.Backpack.AllItems.FindType(0x0E20));
            else
                bandage = new UOItem(World.Player.Backpack.AllItems.FindType(0x0E21));
            World.FindDistance = 3;
            Journal.Clear();
            foreach (UOItem corps in World.Ground.Where(x => x.Graphic == 0x2006).ToList())
            {
                if (first)
                {
                    ResDone = false;
                    UO.Wait(200);
                    first = false;
                }
                corps.WaitTarget();
                bandage.Use();
                UO.Wait(200);
                if (Journal.Contains("Jako Priest nemuzes ozivovat")) continue;
                if (Journal.Contains("Jako Shaman nemuzes takhle ozivovat.")) continue;

                if (Journal.Contains("Duch neni ve "))
                {
                    UO.Say(" dej WAR ");
                    return;
                }
                if (Journal.Contains("Necht se navrati "))
                {
                    ResFailSafe();
                    UO.Say("Resuju ");
                    return;
                }

            }
        }

        private void ResFailSafe()
        {
            Core.RegisterServerMessageCallback(0x1C, OnCalls);
            RFS = new System.Timers.Timer(200);
            RFS.Elapsed += RFS_Elapsed;
            RFS.Start();
        }

        private void RFS_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if(DateTime.Now-StartRes>TimeSpan.FromSeconds(7))
            {
                ResDone = true;
                Core.UnregisterServerMessageCallback(0x1C, OnCalls);
                RFS.Elapsed -= RFS_Elapsed;
                RFS.Stop();
                RFS.Dispose();
                RFS = null;
            }
        }

        private void Heal(int Equip, bool CleanBandage = true)
        {
            if (( DateTime.Now - StartBandage < TimeSpan.FromSeconds(6)) && !BandageDone) return;
            else
                Journal.EntryAdded -= Journal_EntryAdded;
            if (Equip == 15 && World.Player.Hits == World.Player.MaxHits ) return;
            StartBandage = DateTime.Now;
            BandageDone = false;
            if (CleanBandage) UO.Say(".heal" + Equip.ToString());
            else UO.Say(".samheal" + Equip.ToString());
            Journal.EntryAdded += Journal_EntryAdded;
        }


        private void Journal_EntryAdded(object sender, JournalEntryAddedEventArgs e)
        {
            if (bandageDoneCalls.Any(x => e.Entry.Text.ToLowerInvariant().Contains(x)))
            {
                Journal.EntryAdded -= Journal_EntryAdded;
                BandageDone = true;
            }
        }



        CallbackResult OnCalls(byte[] data, CallbackResult prev)
        {
            AsciiSpeech speech = new AsciiSpeech(data);
            foreach (string s in ressurectionCalls)
            {
                if (speech.Text.ToLowerInvariant().Contains(s))
                {
                    ResDone = true;
                    return CallbackResult.Normal;
                }
            }
            return CallbackResult.Normal;
        }
    }
}
