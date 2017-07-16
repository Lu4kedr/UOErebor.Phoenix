using Phoenix.Gui;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Phoenix.Plugins
{
    public class GameWindowSize
    {
        private int Width, Height;
        public GameWindowSize(GameWIndoSizeDATA Data)
        {
            if (Data.Height == default(int) || Data.Width == default(int))
            {
                Width = 800;
                Height = 600;
            }
            else
            {
                Width = Data.Width;
                Height = Data.Height;
            }
            try
            {
                int offset;
                if (FindPattern(sizePatternOld, out offset))
                {
                    int sizeOffset = Patch((IntPtr)offset, true);

                    Marshal.WriteInt32((IntPtr)sizeOffset, 0, Width);
                    Marshal.WriteInt32((IntPtr)sizeOffset, 4, Height);
                    Debug.WriteLine("Actual GameWindowSize patched.", "GameWindowSize");

                    if (!FindPattern(thingiePattern, out offset))
                        throw new Exception("Old thingie pattern not found!");
                    Marshal.WriteInt32(Marshal.ReadIntPtr((IntPtr)offset, 25), Width);
                    Marshal.WriteInt32(Marshal.ReadIntPtr((IntPtr)offset, 35), Height);
                    Debug.WriteLine("Old thingie patched.", "GameWindowSize");
                }
                else if (FindPattern(sizePatternNew, out offset))
                    Patch((IntPtr)offset, false);
            }
            catch (Exception ex) { ExceptionDialog.Show(ex, "GameWindowSize exception."); }
        }

        private int Patch(IntPtr offset, bool old)
        {
            Debug.WriteLine("SetGameWindowSize " + (old ? "old" : "new") + " pattern found.", "GameWindowSize");

            uint oldProtect;
            if (!VirtualProtect(offset, 22, 0x40, out oldProtect))
                throw new Win32Exception();

            int sizeOffset = Marshal.ReadInt32(offset, old ? 0x16 : 0x18);

            Marshal.WriteByte(offset, 0, 0xC7);
            Marshal.WriteByte(offset, 1, 0x05);
            Marshal.WriteInt32(offset, 2, sizeOffset);
            Marshal.WriteInt32(offset, 6, Width);

            Marshal.WriteByte(offset, 10, 0xC7);
            Marshal.WriteByte(offset, 11, 0x05);
            Marshal.WriteInt32(offset, 12, sizeOffset + 4);
            Marshal.WriteInt32(offset, 16, Height);

            Marshal.WriteByte(offset, 20, (byte)(old ? 0xC3 : 0xEB));//old ? retn : jmp short -27
            Marshal.WriteByte(offset, 21, unchecked((byte)-27));

            Debug.WriteLine("SetGameWindowSize patched.", "GameWindowSize");
            return sizeOffset;
        }

        [DllImport("kernel32.dll")]
        private static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);

        private bool FindPattern(byte[] pattern, out int offset)
        {
            ProcessModule module = Process.GetCurrentProcess().MainModule;
            for (offset = module.BaseAddress.ToInt32(); offset < module.BaseAddress.ToInt32() + module.ModuleMemorySize - pattern.Length; offset++)
                for (int x = 0; x < pattern.Length; x++)
                {
                    if (pattern[x] != 0xFF && Marshal.ReadByte((IntPtr)offset, x) != pattern[x])
                        break;
                    if (x == pattern.Length - 1)
                        return true;
                }
            return false;
        }

        private readonly byte[] sizePatternOld = new byte[]
                                        {
                                            0x3D, 0x20, 0x03, 0x00, 0x00,   //cmp   eax, 320h
                                            0x75, 0x1D,                     //jnz   0x1D
                                            0x8B, 0x4C, 0x24, 0x08,         //mov   ecx, [esp+arg_4]
                                            0xB8, 0x58, 0x02, 0x00, 0x00,   //mov   eax, 258h
                                            0x3B, 0xC8,                     //cmp   ecx, eax
                                            0x75, 0x10                      //jnz   0x10
                                        };

        private readonly byte[] sizePatternNew = new byte[]
                                        {
                                            0x3D, 0x20, 0x03, 0x00, 0x00,   //cmp   eax, 320h
                                            0x75, 0xE8,                     //jnz   0xE8
                                            0x56,                           //push  esi
                                            0x8B, 0x74, 0x24, 0x0C,         //mov   esi, [esp+4+arg_4]
                                            0xB8, 0x58, 0x02, 0x00, 0x00,   //mov   eax, 258h
                                            0x3B, 0xF0,                     //cmp   esi, eax
                                            0x5E,                           //pop   esi
                                            0x75, 0xD9                      //jnz   0xD9
                                        };

        private readonly byte[] thingiePattern = new byte[]
                                                 {
                                                     0xA0, 0xFF, 0xFF, 0xFF, 0x00,
                                                     0x83, 0xEC, 0x08,
                                                     0xA8, 0x01,
                                                     0x75, 0x1B,
                                                     0x0C, 0x01,
                                                     0xA2, 0xFF, 0xFF, 0xFF, 0x00,
                                                     0xE8, 0xFF, 0xFF, 0xFF, 0x00
                                                 };
    }
}