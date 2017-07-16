using System;

namespace Lumber
{
    public class StaticTree
    {
        public ushort ID { get; set; }
        public ushort X { get; set; }
        public ushort Y { get; set; }
        public sbyte Z { get; set; }
        public DateTime Harvested { get; set; }
    }
}