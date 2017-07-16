using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Phoenix.Plugins.Runes
{
    public partial class Runy : Form
    {
        public Runy()
        {
            InitializeComponent();
            RuneTree.FillTreeView(this.treeView1);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            RuneTree.GetRunes();
           
        }

        private void button4_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            RuneTree.FillTreeView(this.treeView1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (Rune r in RuneTree.Runes.Where(run => run.Id.ToString() == treeView1.SelectedNode.Tag.ToString()))
            {
                RuneTree.findRune(r);
                r.RecallSvitek();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (Rune r in RuneTree.Runes.Where(run => run.Id.ToString() == treeView1.SelectedNode.Tag.ToString()))
            {
                RuneTree.findRune(r);
                r.Gate();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (Rune r in RuneTree.Runes.Where(run => run.Id.ToString() == treeView1.SelectedNode.Tag.ToString()))
            {
                RuneTree.findRune(r);
                r.Recall();
            }
        }
    }
}
