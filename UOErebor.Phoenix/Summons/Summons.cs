using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Phoenix.Plugins.Summons
{
    public partial class Summons : Form
    {
        private bool mouseDown;
        private Point lastLocation;
        private bool border = true;
        public IntPtr hwnd;
        public static bool _visible { get; set; }
        public static string _text { get; set; }
        private System.Timers.Timer t;
        private List<UOCharacter> List;
        private OneTargetDetails[] Slots;
        public Summons()
        {
            List = new List<UOCharacter>();
            InitializeComponent();
            hwnd = this.Handle;
            //WindowsServices.SetWindowExTransparent(hwnd);
            t = new System.Timers.Timer(1000);
            t.Elapsed += T_Elapsed;

            Slots = new OneTargetDetails[] {oneTargetDetails1, oneTargetDetails2,oneTargetDetails3,oneTargetDetails4,oneTargetDetails5 };
            foreach(var s in Slots)
            {
                //s.Hide();
            }
            this.FormClosing += Summons_FormClosing;
            t.Start();
        }

        private void Summons_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.FormClosing -= Summons_FormClosing;
            t.Elapsed -= T_Elapsed;
            t.Stop();
            t.Dispose();
            t = null;
        }

        private void T_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if(List.Count>0)
            {
                for(var i=0;i<List.Count;i++)
                {
                    if(List[i].Hits<1 || List[i].Hits>1000 || List[i].Distance>18)
                    {
                        List.RemoveAt(i);
                    }
                }
            }
            for(var k=List.Count;k<Slots.Length;k++)
            {

                Slots[k].Invoke(new MethodInvoker(delegate
                {
                    Slots[k].Hide();
                }
               ));
            }

            var tmp = World.Characters.Where(x => x.Renamable).ToList();
            tmp.Sort((x, y) => x.Distance.CompareTo(y.Distance));
            foreach (var ch in tmp) 
            {
                if(List.Count<5 )
                {
                    if(!List.Contains(ch))
                    List.Add(ch);

                }
            }
            if (List.Count > 0)
            {
                for (var i = 0; i < List.Count; i++)
                {
                    Slots[i].Invoke(new MethodInvoker(delegate
                    {
                        if (!Slots[i].Visible) Slots[i].Show();
                        if (Slots[i].Target.Serial != List[i].Serial)
                            Slots[i].SetCharacter(List[i].Serial);
                    }
                    ));

                }
            }

            UOCharacter lastatt = new UOCharacter(Aliases.LastAttack);
            oneTargetDetails0.Invoke(new MethodInvoker(delegate
            {
                if (oneTargetDetails0.Target.Serial != lastatt.Serial)
                    oneTargetDetails0.SetCharacter(lastatt.Serial);
            }
            ));
        }



        /// <summary>
        /// udalosti mysi
        /// </summary>

        private void afkatt_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }
        private void afkatt_MouseDoubleClick(object sender, EventArgs e)
        {
            if (border)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                border = !border;
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.Sizable;
                border = !border;
            }
        }

        private void afkatt_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void afkatt_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button.Equals(MouseButtons.Right)) this.Close();
        }

        private void afkatt_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }


        /// <summary>
        /// Prusvitnost a neregistrovani mysi
        /// </summary>
        public static class WindowsServices
        {
            const int WS_EX_TRANSPARENT = 0x00000020;
            const int GWL_EXSTYLE = (-20);

            [DllImport("user32.dll")]
            static extern int GetWindowLong(IntPtr hwnd, int index);

            [DllImport("user32.dll")]
            static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

            public static void SetWindowExTransparent(IntPtr hwnd)
            {
                var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
                SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
            }
            public static void makeNormal(IntPtr hwnd)
            {
                int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
                SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle & ~WS_EX_TRANSPARENT);
            }
        }

        public void clickeable()
        {
            WindowsServices.makeNormal(hwnd);
        }

    }
}

