using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phoenix.Plugins
{
    public class BoltLot
    {
        bool sipky = false;
        [Command]
        public void SipkyLot()
        {
            if (!sipky)
            {
                sipky = true;
                World.Player.Backpack.Changed += World_ItemAdded;
                t = new System.Timers.Timer(500);
                t.Elapsed += T_Elapsed;
                t.Start();
                UO.PrintInformation("Lot On");
            }
            else
            {
                sipky = false;
                World.Player.Backpack.Changed -= World_ItemAdded;
                t.Stop();
                t = null;
                UO.PrintInformation("Lot OFF");
            }
        }

        private void T_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            t.Elapsed -= T_Elapsed;
            List<Serial> tmp;
            UOItem temp;
            lock (lockBolt)
            {
                tmp = bolts.ToList();
            }
            foreach (var it in tmp)
            {
                temp = new UOItem(it);
                if (Math.Sqrt(Math.Pow((temp.X - World.Player.X), 2) + Math.Pow((temp.Y - World.Player.Y), 2)) < 4)
                {
                    temp.Move(ushort.MaxValue, UO.Backpack);
                    UO.Wait(200);
                }
                UO.Wait(100);
            }
            World.FindDistance = 5;
            foreach (var git in World.Ground.Where(x => x.Graphic == 0x1BFB && x.Distance < 4).ToList())
            {
                if (git.X > 200 && git.Y > 200)
                    git.Move(ushort.MaxValue, UO.Backpack);

            }
            t.Elapsed += T_Elapsed;
        }


        System.Timers.Timer t;
        List<Serial> bolts_ = new List<Serial>();
        List<Serial> bolts = new List<Serial>();
        object lockBolt = new object();
        private void World_ItemAdded(object sender, ObjectChangedEventArgs e)
        {
            UOItem tmp = new UOItem(e.ItemSerial);

            if (tmp.Graphic == 0x1BFB)
            {
                if (tmp.X > 200 && tmp.Y > 200)
                {
                    if (bolts_.Count > 1000)
                    {
                        bolts_.Clear();
                    }
                    bolts_.Add(tmp);
                    lock (lockBolt)
                    {
                        bolts = bolts_;
                    }

                }
            }

        }

    }
}
