using Phoenix.Communication;
using Phoenix.Plugins.Autoheal;
using Phoenix.Plugins.Equips;
using Phoenix.Plugins.Runes;
using Phoenix.Plugins.Weapons;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Phoenix.Plugins
{
    [RuntimeObject]
    public class Initialize
    {
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);


        GameWindowSize GWS;
        System.Timers.Timer Wait2CharLoad;
        private SaveClass SaveClass = null;

        private GameWIndoSizeDATA GWSDATA;
        private short LastHits;

        public Initialize()
        {
            XmlSerializeHelper<GameWIndoSizeDATA>.Load("WindowSize", out GWSDATA, false);
            GWS = new GameWindowSize(GWSDATA);
            Wait2CharLoad = new System.Timers.Timer(100);
            Wait2CharLoad.Elapsed += CharLoad;
            Wait2CharLoad.Start();
        }

        private void CharLoad(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (World.Player.Name == null) return;
            Wait2CharLoad.Elapsed -= CharLoad;
            Wait2CharLoad.Stop();
            Wait2CharLoad.Dispose();
            setEQ();
            try
            {
                XmlSerializeHelper<SaveClass>.Load(World.Player.Name, out SaveClass, true);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.InnerException.Message);
            }
            if (SaveClass == null)
                SaveClass = new SaveClass();

            Wait2CharLoad = null;

            Core.Window.FormClosing += Window_FormClosing;
            Other.OnParalyze += Other_OnParalyze;
            World.Player.Changed += Player_Changed;
            Autolot.Reload();
            World.Player.Print("=== Loaded === ");
            UO.PrintInformation("=== Loaded === ");

        }

        private void Player_Changed(object sender, ObjectChangedEventArgs e)
        {
            if (SaveClass.AutoDrink)
            {
                if (Math.Abs(LastHits - World.Player.Hits) > World.Player.Hits & DrinkManager.DrinkManager.Annouced)
                {
                    UO.Say(".potionheal");
                }
                if (World.Player.Hits < SaveClass.CriticalHP & DrinkManager.DrinkManager.Annouced)
                {
                    UO.Say(".potionheal");
                }
                LastHits = World.Player.Hits;
            }
            string tmp = PrintState();
            if (tmp != Client.Text)
                Client.Text = tmp;
        }

        private string PrintState()
        {
            UOPlayer p = World.Player;
            string temp = "";
            temp = "HP: " + p.Hits + "/" + p.MaxHits //11
                + "  ||  Mana: " + p.Mana + "/" + p.MaxMana//19
                + "  ||  Stamina: " + p.Stamina + "/" + p.MaxStamina//22
                + "  ||  Sila: " + p.Strenght //15
                + "  ||  Stealth: " + p.Skills["Stealth"].RealValue / 10 //18
                + "  ||  Weight: " + p.Weight + "/" + p.MaxWeight//21
                + "  ||  Armor: " + p.Armor //16
                + "  ||  Gold: " + p.Gold;//20  =109-->110+110=220 124-->125+125=290

            if (World.GetCharacter(Aliases.GetObject("laststatus")).Distance < 20 && World.GetCharacter(Aliases.GetObject("laststatus")).Hits > -1)
            {
                if (temp.Length < 145)
                {
                    for (int i = 0; i < 145 - temp.Length; i++)
                    {
                        temp += " ";
                    }
                    temp += "                              ";//40 znaku
                    temp += "                              ";//40 znaku
                }
                temp += World.GetCharacter(Aliases.GetObject("laststatus")).Name
                    + ": "
                    + World.GetCharacter(Aliases.GetObject("laststatus")).Hits
                    + "/"
                    + World.GetCharacter(Aliases.GetObject("laststatus")).MaxHits
                    + "   Distance: "
                    + World.GetCharacter(Aliases.GetObject("laststatus")).Distance;

            }
            return temp;
        }
        private void Other_OnParalyze(object sender, EventArgs e)
        {
            if(SaveClass.AutoHarm & !AutoHeal.HealRun)
            {
                if (SaveClass.HarmArrow)
                {
                    UO.RunCmd("harmself");
                    UO.Wait(1600);
                }
                else
                {
                    UO.RunCmd("arrowself");
                    UO.Wait(2200);
                }

            }
        }

        private void Window_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            XmlSerializeHelper<SaveClass>.Save(World.Player.Name, SaveClass, true);

            XmlSerializeHelper<GameWIndoSizeDATA>.Save("WindowSize", GWSDATA, false);
        }



        private void setEQ()
        {
            UO.Wait(800);
            World.Player.Click();
            UO.Wait(800);
            World.Player.WaitTarget();
            UO.Say(".setequip15");

        }

        [Command]
        public void setresolution(int Width, int Height)
        {
            GWSDATA.Height = Height;
            GWSDATA.Width = Width;
        }

        [Command]
        public void naprahy()
        {
            if (SaveClass.PrintStoodUp)
                SaveClass.PrintStoodUp = false;
            else
                SaveClass.PrintStoodUp = true;
        }

        [Command]
        public void automorf()
        {
            if (SaveClass.AutoMorf)
                SaveClass.AutoMorf = false;
            else
                SaveClass.AutoMorf = true;
        }

        [Command]
        public void setbandage()
        {
            if (SaveClass.Klerik)
            {
                SaveClass.Klerik = false;
                SaveClass.CrystalCmd = ".improvement";
            }

            else
            {
                SaveClass.Klerik = true;
                SaveClass.CrystalCmd = ".enlightment";
            }
        }

        [Command]
        public void goldlimit()
        {
            UO.PrintInformation("Aktualni Limit goldu je: {0}",SaveClass.GoldLimit);
            UO.PrintWarning("pro nastaveni hodnoty: ,goldlimit CASTKA");
            UO.PrintInformation("Funkce pracuje pouze pri zaplem autolotu - ,lot");
        }
        [Command]
        public void goldlimit(int Castka)
        {
            SaveClass.GoldLimit = Castka;
            UO.PrintInformation("Aktualni Limit goldu je: {0}", SaveClass.GoldLimit);

        }
        [Command]
        public void autospell()
        {
            if(SaveClass.HarmArrow)
            {
                SaveClass.HarmArrow = false;
                UO.PrintWarning("Na paru automaticky castim Sipku");
            }
            else
            {
                SaveClass.HarmArrow = true;
                UO.PrintWarning("Na paru automaticky castim Harm");
            }
        }


        [Command]
        public void autosipka()
        {
            if (SaveClass.AutoHarm)
            {
                SaveClass.AutoHarm = false;
                UO.PrintError("Auto odparovani Off");
            }
            else
            {
                SaveClass.AutoHarm = true;
                UO.PrintInformation("Auto odparovani ON");
            }
        }

        [Command]
        public void runy()
        {
            Application.Run(new Runy());
        }

        [Command]
        public void autodrink(bool Enable)
        {
            SaveClass.AutoDrink = Enable;
            if(Enable)
            {
                UO.PrintInformation("Automaticke piti On");
            }
            else UO.PrintError("Automaticke piti Off");
        }

        [Command]
        public void criticalhits(int MinimalHits2Drink)
        {
            SaveClass.CriticalHP = MinimalHits2Drink;
        }



        [Command]
        public void help()
        {
            Notepad.WriteLine(" Prehled funkci:");
            Notepad.WriteLine("         Veskere nastaveni se uklada zavrenim herniho okna Krizkem vpravo nahore ;-)");
            Notepad.WriteLine("Autolot:");
            Notepad.WriteLine("     ,lot - zapnuti/vypnuti");
            Notepad.WriteLine("     ,lotzem - jendorazove vylotovani nastavenych veci pod nohama");
            Notepad.WriteLine("     ,kuch - rozrezani okolnich mrtvol");
            Notepad.WriteLine("     ,lotset - nastaveni lotovaciho batohu a rezaciho nastroje");
            Notepad.WriteLine("     ,lotsetex1 - nastaveni dodatecneho typu k lotu");
            Notepad.WriteLine("     ,lotsetex2 - nastaveni dodatecneho typu k lotu");
            Notepad.WriteLine("  Nasledujici prikazy slouzi k zapinani/vypinani lotu ruznych typu predmetu");
            Notepad.WriteLine("     ,lotfood");
            Notepad.WriteLine("     ,lotgems");
            Notepad.WriteLine("     ,lotregs");
            Notepad.WriteLine("     ,lotfeathers");
            Notepad.WriteLine("     ,lotbolts");
            Notepad.WriteLine("     ,lotleather - jsou-li v batohu nuzky tak automaticky kuze nastriha");
            Notepad.WriteLine("     ,lotex1");
            Notepad.WriteLine("     ,lotex2");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("Ability:");
            Notepad.WriteLine("     ,leap");
            Notepad.WriteLine("     ,probo");
            Notepad.WriteLine("     ,bomba");
            Notepad.WriteLine("     ,kudla");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("Poisoning:");
            Notepad.WriteLine("     ,poisset - nastaveni lahve, ktera se bude pouzivat");
            Notepad.WriteLine("     ,pois - aplikvoani nastavene lahve na zbran v prave ruce");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("Leceni:");
            Notepad.WriteLine("     ,bandage - po nalognuti se postava sama nastavi na eq15 na ktery tento prikaz hazi bandu");
            Notepad.WriteLine("     ,bandage true . k leceni pouzije krvave bandaze - Shaman");
            Notepad.WriteLine("     ,bandage Equip CleanBandage - Equip: 0-15 CleanBandage: true/false");
            Notepad.WriteLine("     ,res - oziveni pomoci cistych bandazi");
            Notepad.WriteLine("     ,shamanres - oziveni pomoci krvavych bandazi");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("Herni okno:");
            Notepad.WriteLine("     ,setresolution X Y - X - sirka Y - delka okna maximum klienta je 800x600 timto lze rozmery upravit libovolne");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("Dalsi Nastaveni/Funkce:");
            Notepad.WriteLine("     ,autodrink true/false - zapnuti/vypnuti automatickeho piti Healu pri nastavenych HP, nebo pri dmg jehoz opakovani = smrt");
            Notepad.WriteLine("     ,criticalhits X - nastaveni minimalni urovne pro piti X= ciselna hodnota");
            Notepad.WriteLine("     ,naprahy - zaponani/vypinani vypisu naprahu");
            Notepad.WriteLine("     ,automorf - automaticka zmena velkych monster a spiritu na humanoidy");
            Notepad.WriteLine("     ,setbandage - prepinani mezi Klerikem a Shamanem u Autohealu");
            Notepad.WriteLine("     ,goldlimit - vypis aktualniho nastaveni limitu goldu v lotbaglu pred zavolanim .mesec");
            Notepad.WriteLine("     ,goldlimit Castka");
            Notepad.WriteLine("     ,autospell - prepinani mezi automatickou Sipkou nebo Harmem");
            Notepad.WriteLine("     ,autosipka - zapnuti/vypnuti automaticke sipky");
            Notepad.WriteLine("     ,runy - nove okno s fcemi pro nacteni run a jejich pouziti");
            Notepad.WriteLine("     ,hid - odpocita hiding a blokuje beh tesne pred hidnutim");
            Notepad.WriteLine("     ,hidoff -vypne blokovani behu, pro odhidnuti, pripadne kdyz se blokovani automaticky nevypne ( vypada jako lagy pri behu, pri chuzi normalni hra)");
            Notepad.WriteLine("     ,friend - da 'all friend' na vsechny modre a vsechny ovladane summony v okoli");
            Notepad.WriteLine("         summony je treba mit otargetovane, nebo mit postahovane zalozky");
            Notepad.WriteLine("     ,kill - misto 'all friend pouzije 'all kill' na vse s cervenou karmou v okoli");
            Notepad.WriteLine("     ,presun X - presune X nahodnych veci z 1 kontejneru do loticiho backpacku");
            Notepad.WriteLine("     ,id pouzeje 'identifikace' u vetesnika na vsechny predmety v danem batohu");
            Notepad.WriteLine("     ,war - prepina warmod DURAZNE doporucuji nastavit jako hotkej na klavesu Tab");
            Notepad.WriteLine("     ,exp - vypise ziskane zkusenosti od pusteni Phoenixe");
            Notepad.WriteLine("     ,clearexp vymaze exp pocitadlo");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("Magery:");
            Notepad.WriteLine("     ,ccast frostbolt/necrobolt Target - zvoleny spell zacasti na target");
            Notepad.WriteLine("     ,reactivearmor Target - vycasti rectiv na zadany target a odpocita jeho konec pro casteni na sebe ,reactivearmor self, jsou-li svitky pouzije prvni svitky");
            Notepad.WriteLine("     ,teleport target - zakouzli na dany target Teleport, jsou li svitky v batohu casti ze svitku");
            Notepad.WriteLine("     ,invis - zacasti na sebe Invis spell a odpocita ho");
            Notepad.WriteLine("     ,arrowself - zacasti na sebe sipku");
            Notepad.WriteLine("     ,harmself - zacasti na sebe harm");
            Notepad.WriteLine("     ,arrowself true - zacasti na sebe sipku a zautoci na predesly target");
            Notepad.WriteLine("     ,harmself true - zacasti na sebe harm a zautoci na predesly target");
            Notepad.WriteLine("     ,autocast Spell Charged Target - automaticky casti zadany Spell, pri zadani Charged(true/false) jako true casti nabijeci spelly");
            Notepad.WriteLine("         Spells - Harm, Fireball, Flame, Meteor, Bolt, Mind");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("Music:");
            Notepad.WriteLine("     ,peace - zacne uspavat laststatus");
            Notepad.WriteLine("     ,entic - zacne oslabovat laststatus");
            Notepad.WriteLine("     ,setprovo - vyhodi targety pro nastaveni 2 cilu");
            Notepad.WriteLine("     ,provo vyprovokuje nastavene cile");
           // Notepad.WriteLine("     ,musicreset - pri erroru ..Callback already registred pouzit toto");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("Targeting:");
            Notepad.WriteLine("     ,targetnext - targetuje cervenou a sedou karmu, v pripade ze nikdo takovy neni targetuje i modrou");
            Notepad.WriteLine("     ,targetnext true - targetuje nejblizsi cervenou karmu");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("Tracking");
            Notepad.WriteLine("     ,track - vypise potrebne informace o prikazu ,track");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("Veterinary:");
            Notepad.WriteLine("     ,petset - nastaveni zvirete ktere se ma lecit");
            Notepad.WriteLine("     ,petheal - pokusi se vylecit peta");
            Notepad.WriteLine("     ,autopetheal - leci peta do full hp");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("Voodoo:");
            Notepad.WriteLine("     ,obet - provede prikaz .sacrafire, ma-li hrac alespon 80 hp");
            Notepad.WriteLine("     ,boost type - vyhazuje targety pro oznaceni hlav, ktere maji dostat voodoo daneho typu, do zruseni targetu");
            Notepad.WriteLine("         v batohu musi byt pro dany typ potion, hlavy mohou byt vy pytliku mimo postavu, script si je bere a vraci");
            Notepad.WriteLine("         typy- str, dex, int, def");
            Notepad.WriteLine("     ,selfboost type - hodi na sebe voodoo, vlastni hlava a potiony musi byt v otevrenem baglu u postavy");
            Notepad.WriteLine("         v pripade ze nenajde hlavu zacasti spell Cunning a Protection");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("Autoheal:    ");
            Notepad.WriteLine("     ,healinfo - vypise info o pridanych hracich do sc");
            Notepad.WriteLine("     ,healclear - vymaze seznam hracu");
            Notepad.WriteLine("     ,healadd - prida zamereneho hrace do seznau lecenych");
            Notepad.WriteLine("     ,healremove X - vymaze leceneho s X=ID, ziskane z ,healinfo");
            Notepad.WriteLine("     ,heal - zapina/vypina automaticke leceni");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("Weapons: ");
            Notepad.WriteLine("     ,weaponadd - prida zbran do seznamu");
            Notepad.WriteLine("     ,weaponsclear - vymaze zapamatovane zbrane");
            Notepad.WriteLine("     ,switch - prehazuje mezi nastavenymi zbranemi");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("     ");
            Notepad.WriteLine("Equipy:     ");
            Notepad.WriteLine("     ,equipadd - zamerenim na baglik s equipem ho nacte a ulozi, pred pridanim musi byt baglik otevreny");
            Notepad.WriteLine("     ,eq - vypis ulozenych equipu");
            Notepad.WriteLine("     ,equipsclear - vymaze zapamatovane equipy");
            Notepad.WriteLine("     ,equipdel X - vymaze equip s danym id=X");
            Notepad.WriteLine("     ,dress X - oblece equip s id=X");
            Notepad.WriteLine("     ,dresssave X - oblece equip s id=X a aktualni  schova do zamereneho pytliku");
            Notepad.WriteLine("     ");
        }


    }
}
