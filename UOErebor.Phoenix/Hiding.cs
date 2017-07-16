using Phoenix.Communication;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phoenix.Plugins
{
    public class Hiding
    {
        public static DateTime HiddenTime;
        private int Hits = 0;
        readonly private int[] HiddenDelay = {3200,2600,2150,1600 }; // TODO delays 25, 50, 75, 100 skill 
        System.Timers.Timer HiddenCheck;

        [Command]
        public void hid()
        {
            hiding(true);
        }

        private void hiding(bool First)
        {
            UO.RunCmd("bandage");
            Core.UnregisterClientMessageCallback(0x02, ForceWalk);
            Hits = WorldData.World.Player.Hits;
            int Delay = 0;
            if (WorldData.World.Player.Skills[21].Value > 99) Delay = HiddenDelay[3];
            else if (WorldData.World.Player.Skills[21].Value > 74) Delay = HiddenDelay[2];
            else if (WorldData.World.Player.Skills[21].Value > 49) Delay = HiddenDelay[1];
            else if (WorldData.World.Player.Skills[21].Value > 24) Delay = HiddenDelay[0];

            int step = 2;
            UO.Warmode(false);
            UO.UseSkill(StandardSkill.Hiding);
            World.Player.Print("3");
            DateTime startHid = DateTime.Now;
            while (DateTime.Now - startHid < TimeSpan.FromMilliseconds(Delay))
            {
                if (Hits>World.Player.Hits)
                {

                    Core.UnregisterClientMessageCallback(0x02, ForceWalk);
                    if (First) hiding(false);
                    return;
                }
                if (DateTime.Now - startHid > TimeSpan.FromMilliseconds(Delay / 3) && step == 2)
                {
                    World.Player.Print("2");
                    step--;
                }
                if (DateTime.Now - startHid > TimeSpan.FromMilliseconds((Delay / 3) * 2) && step == 1)
                {
                    World.Player.Print("1");
                    step--;

                    Core.RegisterClientMessageCallback(0x02, ForceWalk, CallbackPriority.Highest);
                }
                UO.Wait(10);
            }
            if (Journal.WaitForText(true, 300, "nepovedlo se ti schovat.", "skryti se povedlo."))
            {
                UO.Wait(100);
                if (!World.Player.Hidden)
                    Core.UnregisterClientMessageCallback(0x02, ForceWalk);
                else
                    HiddenTime = DateTime.Now;
                    UO.RunCmd("bandage");
            }
            else
            {
                Core.UnregisterClientMessageCallback(0x02, ForceWalk);

            }
            RunHidCheck();

        }

        private void RunHidCheck()
        {
            HiddenCheck = new System.Timers.Timer(100);
            HiddenCheck.Elapsed += HiddenCheck_Elapsed;
            HiddenCheck.Start();
        }

        private void HiddenCheck_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
           if(!World.Player.Hidden)
            {
                HiddenCheck.Stop();
                hidoff();
                UO.Say(".resync");
                UO.PrintInformation("Resynced");
                HiddenCheck.Elapsed += HiddenCheck_Elapsed;
                HiddenCheck.Dispose();
            }
        }

        [Command]
        public void hidoff()
        {
            Core.UnregisterClientMessageCallback(0x02, ForceWalk);
        }

        CallbackResult ForceWalk(byte[] data, CallbackResult prev)//0x02 clientReq
        {
            PacketReader pr = new PacketReader(data);
            PacketWriter pw = new PacketWriter();
            byte cmd = pr.ReadByte();
            byte dir = pr.ReadByte();
            byte seq = pr.ReadByte();
            uint fwalkPrev = pr.ReadUInt32();
            if (Convert.ToUInt16(dir) > 7)
            {
                dir = Convert.ToByte(Convert.ToUInt16(dir) - 128);
                pw.Write(cmd);
                pw.Write(dir);
                pw.Write(seq);
                pw.Write(fwalkPrev);
                Core.SendToServer(pw.GetBytes());
                return CallbackResult.Eat;
            }

            return CallbackResult.Normal;
        }
    }
}
