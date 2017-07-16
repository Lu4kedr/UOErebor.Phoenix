using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phoenix.Plugins.Weapons
{
    [Serializable]
    public class WeaponSet
    {
        private Dictionary<Graphic, string> typ;
        private Dictionary<UOColor, string> materials;
        private UOItem weapon;
        private UOItem shield;
        public uint Weapon
        {
            get
            {
                if (weapon == null) return 0;
                else return weapon.Serial;
            }
            set
            {
                weapon = new UOItem(value);

            }
        }
        public uint Shield
        {
            get { return shield.Serial; }
            set
            {
                shield = new UOItem(value);

                // Try get Name of set
                if (typ.ContainsKey(weapon.Graphic))
                {
                    if (materials.ContainsKey(weapon.Color))
                    {
                        Name = materials[weapon.Color] + " " + typ[weapon.Graphic];
                    }
                    else Name = typ[weapon.Graphic];
                }
                else
                {
                    if (materials.ContainsKey(weapon.Color))
                    {
                        Name = materials[weapon.Color];
                    }
                    else
                    {
                        if (typ.ContainsKey(shield.Graphic))
                        {
                            if (materials.ContainsKey(shield.Color))
                            {
                                Name = materials[shield.Color] + " " + typ[shield.Graphic];
                            }
                            else Name = "WeaponSet";
                        }
                    }
                }
            }
        }
        private string setName = "-";

        public string Name
        {
            get
            {
                return setName;

            }
            set { setName = value; }
        }
        public WeaponSet()
        {
            typ = new Dictionary<Graphic, string>();
            typ.Add(0x13F9, "staff,  zbran");
            typ.Add(0x13F8, "staff,  zbran");
            typ.Add(0x13FF, " Katana,  zbran");
            typ.Add(0x13FE, " Katana,  zbran");
            typ.Add(0x143F, " Hala,  zbran");
            typ.Add(0x143E, " Hala,  zbran");
            typ.Add(0x13FB, " LBA,  zbran");
            typ.Add(0x13FA, " LBA,  zbran");
            typ.Add(0x1438, " Kladivo,  zbran");
            typ.Add(0x1439, " Kladivo,  zbran");
            typ.Add(0x13BA, " Vik,  zbran");
            typ.Add(0x13B9, " Vik,  zbran");
            typ.Add(0x13B1, " Luk,  zbran");
            typ.Add(0x13B2, " Luk,  zbran");
            typ.Add(0x13B6, " Scimitar,  zbran");
            typ.Add(0x13B5, " Scimitar,  zbran");
            typ.Add(0x1401, " Kryss,  zbran");
            typ.Add(0x1400, " Kryss,  zbran");


            materials = new Dictionary<UOColor, string>();
            materials.Add(0x099A, "Copper");
            materials.Add(0x0763, "Iron");
            materials.Add(0x097F, "Verite");
            materials.Add(0x0985, "Valorite");
            materials.Add(0x0989, "Obsidian");
            materials.Add(0x0999, "Adamantium");
        }


        public void Equip()
        {
            if (shield.Serial != 0x00 && !(shield.Layer == Layer.LeftHand)) shield.Equip();
            if (weapon.Serial != 0x00) weapon.Equip();
        }
    }
}
