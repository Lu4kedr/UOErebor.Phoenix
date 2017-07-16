using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phoenix.Plugins.DrinkManager
{
    public struct Potion
    {
        public string Name { get; set; }
        public string Command { get; set; }
        public int Amount { get; set; }
        public ushort Color { get; set; }
    }
}
