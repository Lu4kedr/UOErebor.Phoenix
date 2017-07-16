using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phoenix.Plugins.WallManager
{
    public class Wall
    {

        public DateTime CreateTime { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public uint Serial { get; set; }
        public WallTime Type { get; set; }
        public int Distance
        {
            get
            {
                return (int)Math.Abs(Math.Sqrt(Math.Pow(World.Player.X - X, 2) + Math.Pow(World.Player.Y - Y, 2)));

            }
        }
    }
}
