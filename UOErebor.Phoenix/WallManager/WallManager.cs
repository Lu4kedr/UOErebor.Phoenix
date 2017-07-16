using Phoenix.Communication;
using Phoenix.Communication.Packets;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phoenix.Plugins.WallManager
{
    [RuntimeObject]
    public class WallManager
    {
        private WallCollection Collection;
        private DateTime WallCall;
        private int TmpCounter = 0;
        System.Timers.Timer bw;


        public WallManager()
        {
            Core.RegisterServerMessageCallback(0x1C, onWallSpeech);
            Core.RegisterServerMessageCallback(0x1A, OnBuildWall, CallbackPriority.Lowest);
            Collection = new WallCollection();
            bw = new System.Timers.Timer(10000);
            bw.Elapsed += Bw_Elapsed;
            bw.Start();

        }

        private void Bw_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            List<Wall> tmp = Collection.GetList();
            DateTime delayedPrint = DateTime.Now;

            foreach (Wall w in tmp)
            {
                if (DateTime.Now - w.CreateTime < TimeSpan.FromSeconds((int)w.Type))
                {
                    if (!new UOItem(w.Serial).Exist)
                    {
                        Collection.Remove(w);
                    }
                    if (w.Distance < 17)
                    {
                        UO.PrintObject(w.Serial, ((int)w.Type - (int)(DateTime.Now - w.CreateTime).TotalSeconds).ToString());
                        delayedPrint = DateTime.Now;
                    }
                }
                else Collection.Remove(w);
            }


        }


        CallbackResult onWallSpeech(byte[] data, CallbackResult prevResult) // 0x1C
        {
            AsciiSpeech packet = new AsciiSpeech(data);
            if (packet.Text.Contains("Anna Tir Kemen"))
            {
                WallCall = DateTime.Now;
                new WallTimer((int)WallTime.Stone, packet.Name);

            }
            if (packet.Text.Contains("Anna Tir Esgal"))
            {
                WallCall = DateTime.Now;
                new WallTimer((int)WallTime.Energy, packet.Name);
            }

            return CallbackResult.Normal;
        }

        CallbackResult OnBuildWall(byte[] data, CallbackResult prev) // 0x1A
        {
            PacketReader pr = new PacketReader(data);
            pr.Skip(3);
            uint serial = pr.ReadUInt32();
            ushort graphic = pr.ReadUInt16();
            ushort X = pr.ReadUInt16();
            ushort Y = pr.ReadUInt16();

            if (graphic == 0x3947 || graphic == 0x3956)  // Energy
            {
                TmpCounter++;
                if (DateTime.Now - WallCall < TimeSpan.FromSeconds(10) && TmpCounter % 7 == 0)
                    Collection.Add(new Wall() { CreateTime = DateTime.Now, Serial = serial, Type = WallTime.EnergyLast, X = X, Y = Y });
                return CallbackResult.Normal;

            }
            else

            if (graphic == 0x0080)  // Stone
            {
                TmpCounter++;
                if (DateTime.Now - WallCall < TimeSpan.FromSeconds(10) && TmpCounter % 5 == 0)
                    Collection.Add(new Wall() { CreateTime = DateTime.Now, Serial = serial, Type = WallTime.StoneLast, X = X, Y = Y });

                return CallbackResult.Normal;

            }

            return CallbackResult.Normal;
        }
    }
}
