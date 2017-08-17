using Phoenix.Communication.Packets;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phoenix.Plugins.DrinkManager
{
    [RuntimeObject]
    public class DrinkManager
    {
        private event EventHandler<PotionLoseArgs> OnPotionLose;
        Dictionary<UOColor, Potion> PotionCounter;
        Dictionary<string, int> PotionDelays;
        DateTime DrinkTime = DateTime.Now;
        int LastDrinkDelay = 0;
        System.Timers.Timer bw;
        System.Timers.Timer cureTimer;
        public static bool Annouced = false;
        private bool CureDrinked;
        private int CureDelay;
        private DateTime CureDrinkTime;
        private bool CureLastAnnouced;

        public DrinkManager()
        {
            PotionCounter = new Dictionary<UOColor, Potion>()
            {
                {0, new Potion(){Name="Greater Heal", Amount=0, Command=".potionheal",Color=0x0160}},
                {1, new Potion(){Name="Spell Shield", Amount=0, Command="_",Color=0x0059}},
                {2, new Potion(){Name="Greater Energy", Amount=0, Command="_",Color=0x00C5}},
                {3, new Potion(){Name="Invisibility", Amount=0, Command=".potioninvis",Color=0x0447}},
                {4, new Potion(){Name="Cure", Amount=0, Command=".potioncure",Color=0x002B}},
                {5, new Potion(){Name="Greather Cure", Amount=0, Command=".potioncure",Color=0x008E}},
                {6, new Potion(){Name="Great Cleverness", Amount=0, Command=".potionclever",Color=0x047D}},
                {7, new Potion(){Name="Cleverness", Amount=0, Command=".potionclever",Color=0x073E}},
                {8, new Potion(){Name="Greater Strength", Amount=0, Command=".potionstrength",Color=0x076B}},
                {9, new Potion(){Name="Strength", Amount=0, Command=".potionstrength",Color=0x0388}},
                {10, new Potion(){Name="Shrink", Amount=0, Command=".potionshrink",Color=0x0995}},
                {11, new Potion(){Name="Reflection", Amount=0, Command=".potionreflex",Color=0x0985}},
                {12, new Potion(){Name="Mobility", Amount=0, Command=".potionmobility",Color=0x000F}},
                {13, new Potion(){Name="Dispell Explosion", Amount=0, Command="_",Color=0x0993}},
                {14, new Potion(){Name="Greater Refresh", Amount=0, Command=".potionrefresh",Color=0x00ED}},
                {15, new Potion(){Name="Refresh", Amount=0, Command=".potionrefresh",Color=0x0027}},
                {16, new Potion(){Name="Greater Poison", Amount=0, Command="_",Color=0x0179} },
                {17, new Potion(){Name="Nightsight", Amount=0, Command=".potionnightsight",Color=0x0980} },
                {18, new Potion(){Name="Agility", Amount=0, Command=".potionagility",Color=0x0005 } }


            };

            PotionDelays = new Dictionary<string, int>()
            {
                {".potioncure", 20 },
                {".potionheal", 20 },
                {".potioninvis", 20 },
                {".potionclever", 5 },
                {".potionrefresh", 20 },
                {".potionstrength", 5 },
                {".potionagility", 5 },
                {".potionnightsight", 5 },
                {".potionstoneskin", 5 },
                {".potionmobility", 5 },
                {".potionreflex", 15 },

            };



            OnPotionLose += DrinkManager_OnPotionLose;
            cureTimer = new System.Timers.Timer(550);
            cureTimer.Elapsed += CureTimer_Elapsed;
            cureTimer.Start();
            bw = new System.Timers.Timer(1000);
            bw.Elapsed += Bw_Elapsed;
            bw.Start();
            Core.RegisterClientMessageCallback(0xAD, OnPotionRequest);
        }

        private void CureTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!CureLastAnnouced && DateTime.Now > (CureDrinkTime + TimeSpan.FromSeconds(CureDelay)) )
            {
                CureLastAnnouced = true;
                UO.PrintWarning("<<<<< CURE VYPRSELO! >>>>>");
                World.Player.Print("< CURE VYPRSELO >");
            }
        }

        private void Bw_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (DateTime.Now - DrinkTime > TimeSpan.FromSeconds(LastDrinkDelay + 1))
            {
                if (!Annouced)
                {
                    UO.PrintInformation("Muzes Pit!");
                    World.Player.Print("Muzes Pit!");
                    Annouced = true;
                }
            }
            else
            {
                Annouced = false;
                if (TimeSpan.FromSeconds(LastDrinkDelay + 1) - (DateTime.Now - DrinkTime) < TimeSpan.FromSeconds(6)) 
                {
                    UO.PrintWarning("Piti za {0} s",(int)((TimeSpan.FromSeconds(LastDrinkDelay + 1) - (DateTime.Now - DrinkTime)).TotalSeconds));
                }
            }
            CountPotion();
        }

        private CallbackResult OnPotionRequest(byte[] data, CallbackResult prevState)
        {
            UnicodeSpeechRequest a = new UnicodeSpeechRequest(data);
            if (a.Text[0] != '.') return CallbackResult.Normal;
            bool check=false;
            if (PotionDelays.ContainsKey(a.Text))
            {
                foreach (var it in PotionCounter.Values)
                {
                    if (it.Command.Equals(a.Text) && it.Amount == 0) check = false;
                    else check = true;
                }
                if (!check) return CallbackResult.Eat;
                if (DateTime.Now - DrinkTime > TimeSpan.FromSeconds(LastDrinkDelay+1))
                {
                    DrinkTime = DateTime.Now;
                    LastDrinkDelay = PotionDelays[a.Text];
                    if (a.Text == ".potioncure")
                    {
                        CureDrinked = true;
                        CureDrinkTime = DateTime.Now;
                    }
                    return CallbackResult.Normal;
                }
                else
                {
                    UO.PrintError("Jeste nemuzes pit!!");
                    return CallbackResult.Eat;
                }
            }
            return CallbackResult.Normal;
        }

        private void DrinkManager_OnPotionLose(object sender, PotionLoseArgs e)
        {
            if(CureDrinked && e.Potion.Command==".potioncure")
            {
                CureDrinked = false;
                if (e.Potion.Name == "Greather Cure")
                    CureDelay = 380;
                else CureDelay = 5;
                CureLastAnnouced = false;

            }
            if (e.Potion.Amount > 0)
                UO.PrintError("Zbyva  {0}  {1}", e.Potion.Amount, e.Potion.Name);
            else
                UO.PrintError("Dosel potion: {0}", e.Potion.Name);

        }



        private void CountPotion()
        {
            Potion tmp;
            for(var i=0;i<PotionCounter.Count;i++)
            {
                tmp = new Potion() { Color = PotionCounter[i].Color, Command = PotionCounter[i].Command, Name = PotionCounter[i].Name, Amount=0 };
                foreach (UOItem it in World.Player.Backpack.AllItems.Where(x => x.Graphic == 0x0F0E && x.Color == PotionCounter[i].Color).ToList())
                {
                    tmp.Amount += it.Amount;
                }

                if (PotionCounter[i].Amount != tmp.Amount)
                {

                    PotionCounter[i] = tmp;
                    EventHandler<PotionLoseArgs> temp = OnPotionLose;
                    if (temp != null)
                    {
                        foreach (EventHandler<PotionLoseArgs> ev in OnPotionLose.GetInvocationList())
                        {
                            ev.BeginInvoke(this, new PotionLoseArgs() { Potion = tmp }, null, null);
                        }
                    }

                }
                UO.Wait(50);
            }
        }


    }
}
