using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phoenix.Plugins.Autoheal
{
    public class AutoHeal
    {
        public static bool HealRun = false;
        private System.Timers.Timer Automat;
        public static HealedPlayers Healedplayers;
        public static int MinimalPlayerHP;
        public static int MinimalPatientHP;
        public static bool CastPause;
        private int ParaX, ParaY;
        public static string CrystalCmd;
        public static bool CleanBandage;
        private bool CrystalState;
        private bool Paralyze;


        public AutoHeal()
        {
            CastPause = false;
            Healedplayers = null;
            MinimalPlayerHP = 80;
            MinimalPatientHP = 87;
            CrystalState = false;
            CleanBandage = true;
            CrystalCmd = ".enlightment";
            HealRun = false;
            Automat = new System.Timers.Timer(200);
            Automat.Start();
        }


        [Command]
        public void healadd()
        {
            UO.PrintWarning("Zamer koho lecit");
            Healedplayers.Add(new UOCharacter(UIManager.TargetObject()));
        }
        [Command]
        public void healremove(int index)
        {
            Healedplayers.Remove(index);
        }

        [Command]
        public void healclear()
        {
            Healedplayers = new HealedPlayers();
        }

        [Command]
        public void healinfo()
        {
            for(var i=0;i< Healedplayers.Count();i++)
            {
                UO.PrintWarning("Pacient: {0}, Equip: {1} ID: {2}", Healedplayers[i].Player.Name?? Healedplayers[i].Serial.ToString(), Healedplayers[i].Equip,i);
            }
        }


        [Command]
        public void heal()
        {
            if(HealRun)
            {
                HealRun = false; 
                Automat.Elapsed -= Automat_Elapsed;
                Other.OnParalyze -= Other_OnParalyze;
                UO.PrintError("Healing Off");
            }
            else
            {
                HealRun = true;
                if (Healedplayers == null) Healedplayers = new HealedPlayers();
                Automat.Elapsed += Automat_Elapsed;
                Other.OnParalyze += Other_OnParalyze;
                UO.PrintInformation("Healing On");

            }
        }

        private void Other_OnParalyze(object sender, EventArgs e)
        {
            ParaX = World.Player.X;
            ParaY = World.Player.Y;
            Paralyze = true;
        }

        private void Automat_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Patient temp = null;
            try
            {
                Automat.Elapsed -= Automat_Elapsed;
                if (ParaX != World.Player.X || ParaY != World.Player.Y) Paralyze = false;
                if (Paralyze)
                {
                    UO.RunCmd("harmself");
                    UO.Wait(1600);
                    Paralyze = false;
                }


                if (World.Player.Hidden || !Bandage.ResDone || CastPause) return;
                if (World.Player.Hits < MinimalPlayerHP)
                {
                    UO.RunCmd("bandage");
                    return;
                }
                temp = Healedplayers.GetPatient(MinimalPatientHP);
                if (temp == null)
                {
                    if (CrystalState)
                    {
                        CrystalState = false;
                        UO.Say(CrystalCmd);
                    }
                }
                else
                {
                    if(!CrystalState)
                    {
                        CrystalState = true;
                        UO.Say(CrystalCmd);
                    }
                    if (!Music.MusicDone && temp.Player.Hits > 65)
                    {
                        temp = null;
                    }
                    else
                    {
                        UO.RunCmd("bandage", temp.Equip, CleanBandage);
                    }

                }
            }
            finally
            {
                Automat.Elapsed += Automat_Elapsed;
            }
        }
    }
}
