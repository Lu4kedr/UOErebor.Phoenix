using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phoenix.Plugins.Weapons
{
    public class Weapons
    {

        public static List<WeaponSet> weapons = new List<WeaponSet>();
        private WeaponSet ActualWeapon = null;


        public Weapons()
        {
            
            
        }
        [Command]
        public void weaponsclear()
        {
            weapons = new List<WeaponSet>();
        }

        [Command("weaponadd"),BlockMultipleExecutions]
        public void Add()
        {
            UO.PrintInformation("Zamer zbran");
            UOItem weap = new UOItem(UIManager.TargetObject());
            UO.PrintInformation("Zamer stit");
            UOItem shiel = new UOItem(UIManager.TargetObject());
            if (weap.Serial == 0xFFFFFFFF && shiel.Serial == 0xFFFFFFFF) return;
            weapons.Add(new WeaponSet() { Weapon = weap, Shield = shiel });
            if (weapons.Count > 0 && ActualWeapon == null)
            {
                ActualWeapon = weapons[0];
                Aliases.SetObject("ActualWeapon", ActualWeapon.Weapon);
                Aliases.SetObject("ActualShield", ActualWeapon.Shield);
            }

        }

        public static void Remove(int index)
        {
            if (index >= 0 && index >= weapons.Count) return;
            weapons.RemoveAt(index);

        }

        [Command("switch"),BlockMultipleExecutions]
        public void SwitchWeapons()
        {


            if (weapons.Count < 1)
            {
                UO.PrintError("Neams nastaveny zbrane");
                return;
            }
            int indxActualW = weapons.IndexOf(ActualWeapon == null ? weapons[0] : ActualWeapon);
            if (indxActualW < weapons.Count)
            {
                if (indxActualW + 1 == weapons.Count)
                {
                    ActualWeapon = weapons[0];
                }
                else
                {
                    ActualWeapon = weapons[indxActualW + 1];
                }
                Aliases.SetObject("ActualWeapon", ActualWeapon.Weapon);
                Aliases.SetObject("ActualShield", ActualWeapon.Shield);
                if ((new UOItem(ActualWeapon.Weapon)).Exist)
                    ActualWeapon.Equip();


                UO.ClickObject(World.Player);
            }
        }
    }
}
