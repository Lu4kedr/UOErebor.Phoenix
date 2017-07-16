using Phoenix.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phoenix.Plugins
{
    [RuntimeObject]
    public class TreasureMap
    {
        private const float MapSizeMultiplier = 3f;
        private const ushort MapZoom = 720;


        [ServerMessageHandler(0x90)]
        public CallbackResult OnMapInfo(byte[] data, CallbackResult prevResult)
        {
            PacketReader reader = new PacketReader(data);
            reader.Skip(1); // opcode
            uint serial = reader.ReadUInt32();
            ushort gump = reader.ReadUInt16();
            ushort ux = reader.ReadUInt16();
            ushort uy = reader.ReadUInt16();
            ushort lx = reader.ReadUInt16();
            ushort ly = reader.ReadUInt16();
            ushort width = reader.ReadUInt16();
            ushort height = reader.ReadUInt16();

            ushort x = (ushort)(ux + 180 * (lx - ux > 361 ? -1 : 1));
            ushort y = (ushort)(uy + 180 * (ly - uy > 361 ? -1 : 1));

            UO.PrintInformation("Map is located at {0},{1}", x, y);
            //Track(x, y);

            PacketWriter writer = new PacketWriter(0x90);
            writer.Write(serial);
            writer.Write(gump);
            writer.Write((ushort)(x - MapZoom));
            writer.Write((ushort)(y - MapZoom));
            writer.Write((ushort)(x + MapZoom));
            writer.Write((ushort)(y + MapZoom));
            writer.Write((ushort)(width * MapSizeMultiplier));
            writer.Write((ushort)(height * MapSizeMultiplier));
            Core.SendToClient(writer.GetBytes());

            return CallbackResult.Sent;
        }

        [ServerMessageHandler(0x56)]
        public CallbackResult OnPinInfo(byte[] data, CallbackResult prevResult)
        {
            if (data[5] > 4)
                return prevResult;

            ushort x = ByteConverter.BigEndian.ToUInt16(data, 7);
            ushort y = ByteConverter.BigEndian.ToUInt16(data, 9);

            Array.Copy(ByteConverter.BigEndian.GetBytes((ushort)(x * MapSizeMultiplier)), 0, data, 7, 2);
            Array.Copy(ByteConverter.BigEndian.GetBytes((ushort)(y * MapSizeMultiplier)), 0, data, 9, 2);

            return prevResult;
        }

        [ClientMessageHandler(0x56)]
        public CallbackResult OnPinAction(byte[] data, CallbackResult prevResult)
        {
            if (data[5] > 4)
                return prevResult;

            ushort x = ByteConverter.BigEndian.ToUInt16(data, 7);
            ushort y = ByteConverter.BigEndian.ToUInt16(data, 9);

            Array.Copy(ByteConverter.BigEndian.GetBytes((ushort)(x / MapSizeMultiplier)), 0, data, 7, 2);
            Array.Copy(ByteConverter.BigEndian.GetBytes((ushort)(y / MapSizeMultiplier)), 0, data, 9, 2);

            return prevResult;
        }
    }
}
