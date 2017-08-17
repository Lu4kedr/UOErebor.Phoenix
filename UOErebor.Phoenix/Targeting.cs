using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phoenix.Plugins
{
    public class Targeting
    {
        private readonly Notoriety[] Filter = new Notoriety[] { Notoriety.Murderer, Notoriety.Enemy };
        private readonly List<uint> used = new List<uint>();

        [Command]
        public void targetnext()
        {
            Targetnext(false);
        }
        [Command]
        public void targetnext(bool Closest)
        {
            Targetnext(Closest);
        }

        private void Targetnext(bool closest)
        {
            if (closest)
            {
                List<UOCharacter> redList = new List<UOCharacter>();
                List<UOCharacter> sortedlist;
                redList.Clear();
                var tlist = World.Characters.Where(x => x.Notoriety > Notoriety.Criminal && x.Distance < 15 && x.Serial != World.Player.Serial && !x.Renamable).ToList();
                if(World.Player.Notoriety>Notoriety.Guild)
                    tlist= World.Characters.Where(x => x.Distance < 15 && x.Serial != World.Player.Serial && !x.Renamable).ToList();
                foreach (UOCharacter ch in tlist)
                {
                    redList.Add(ch);
                }
                sortedlist = redList.OrderBy(o => o.Distance).ToList();

                if (sortedlist.Count < 1) return;
                Aliases.LastAttack = sortedlist[0].Serial;
                Aliases.SetObject("laststatus", sortedlist[0].Serial);
                byte[] data = new byte[5];
                data[0] = 0xAA;
                ByteConverter.BigEndian.ToBytes((uint)sortedlist[0].Serial, data, 1);
                Core.SendToClient(data, false);
                return;
            }

            bool first = true;
            tryagain:
            var list = World.Characters.Where(x => x.Notoriety > Notoriety.Criminal && x.Distance < 19 && x.Serial != World.Player.Serial && !x.Renamable).ToList();
            if (list.Count < 1)
            {
                list = World.Characters.Where(x => x.Distance < 19 && x.Serial != World.Player.Serial && !x.Renamable).ToList();


            }
            foreach (UOCharacter mob in list)
            {
                if (used.Contains(mob.Serial)) continue;
                used.Add(mob.Serial);
                Aliases.LastAttack = mob.Serial;
                Aliases.SetObject("laststatus", mob.Serial);
                byte[] data = new byte[5];
                data[0] = 0xAA;
                ByteConverter.BigEndian.ToBytes((uint)mob.Serial, data, 1);
                Core.SendToClient(data, false);
                return;
            }
            used.Clear();
            if (first)
            {
                first = false;
                goto tryagain;
            }
        }
    }
}
