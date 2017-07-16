using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lumber
{
    [Serializable]
    public class Settings
    {
        public uint DoorLeft { get; set; }
        public uint DoorRight { get; set; }
        public ushort DoorLeftClosedGraphic { get; set; }
        public ushort DoorRightClosedGraphic { get; set; }
        public bool UseCrystal { get; set; }
        public int HomeLocationX { get; set; }
        public int HomeLocationY { get; set; }
        public uint LumbRune { get; set; }
        public uint LogsBox { get; set; }
        public uint ResourceBox { get; set; }


        public Settings()
        {
            DoorLeft = 0x40000963;
            DoorRight = 0x40000962;
            DoorLeftClosedGraphic = 0x0841;
            DoorRightClosedGraphic = 0x0843;
            UseCrystal = true;
            HomeLocationX = 2731;
            HomeLocationY = 3271;
            LumbRune = 0x40078E02;
            LogsBox = 0xFFFFFFF;
            ResourceBox = 0xFFFFFFF;


        }
    }
}
