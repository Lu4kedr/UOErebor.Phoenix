using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phoenix.Plugins.DrinkManager
{
    public class PotionLoseArgs : EventArgs
    {
        public Potion Potion { get; set; }

    }
}
