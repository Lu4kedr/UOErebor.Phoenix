using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Phoenix.WorldData;

namespace Phoenix.Plugins.Summons
{
    public partial class OneTargetDetails : UserControl
    {
        public UOCharacter Target { get; set; }
        private System.Timers.Timer t = null;
        private string Name_
        {
            get
            {
                return this.lbl_Name.Text;
            }
            set
            {
                this.lbl_Name.Text = value;
            }
        }
        private int ActualHP
        {
            set
            {
                lbl_ActualHP.Text = value.ToString();
                if (value >= 0 && value <= 100)
                {
                    progressBar1.Value = value;
                }
                lbl_ActualHP.Refresh();
            }
        }
        private int Distance
        {
            set
            {
                lbl_Dist.Text = value.ToString();
                if (value >= 0 && value <= 18)
                {
                    progressBar2.Value = value;
                }
                lbl_Dist.Refresh();
            }
        }

        public OneTargetDetails()
        {
            InitializeComponent();
            t = new System.Timers.Timer(200);
            t.Elapsed += T_Elapsed;
            
            Target = new UOCharacter(0xFFF);
            t.Start();

        }

        public void Dispel()
        {
            UO.Cast(StandardSpell.Dispel, Target);
        }
        public void Rename()
        {
            UO.Say(",rename {0} {1}",Target.Serial,"\"S u m m o n\"");
        }

        public void SetCharacter(Serial Target)
        {
            this.Target = new UOCharacter(Target);

        }

        private void T_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!Target.Exist) return;
            this.BeginInvoke(new MethodInvoker(delegate
            {
                lbl_Name.Text = Target.Name;
                ActualHP = Target.Hits;
                Distance = Target.Distance;
            }));

            this.Refresh();
        }

        private void btn_Rename_Click(object sender, EventArgs e)
        {
            Rename();
        }

        private void btn_DIsp_Click(object sender, EventArgs e)
        {
            Dispel();
        }
    }
}
