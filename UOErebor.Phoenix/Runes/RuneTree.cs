using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Phoenix.Plugins.Runes
{

    public class RuneTree
    {

        public static List<Rune> Runes = new List<Rune>();



        public static void GetRunes()
        {
            UOItem a, aa;
            List<uint> runeTypes = new List<uint>() { 0x1F14, 0x1F15, 0x1F16, 0x1F17 };
            UO.Print("Zamer bednu s runama");
            UOItem runeBox = new UOItem(UIManager.TargetObject());
            runeBox.Click();
            UO.Wait(200);
            runeBox.Use();
            Runes.Clear();
            openBoxes(runeBox, 200);
            foreach (UOItem it in runeBox.AllItems.Where(item => runeTypes.Any(typ => item.Graphic == typ)))
            {
                string tmps = "";
                uint[] tmp = { 0x0, 0x0, 0x0 };
                string[] tmp2 = { "null", "null" };

                it.Click();
                UO.Wait(100);
                if (it.Container != runeBox)
                {
                    tmp[0] = it.Container;
                    a = new UOItem(it.Container);
                    a.Click();
                    UO.Wait(100);
                    tmp2[0] = a.Name;
                    if (a.Container != runeBox)
                    {
                        tmp[1] = a.Container;
                        aa = new UOItem(a.Container);
                        aa.Click();
                        UO.Wait(100);

                        tmp2[1] = aa.Name;
                    }

                    if (tmp2[1] != "null" && tmp2[0] != "null")
                    {
                        tmps = tmp2[0];
                        tmp2[0] = tmp2[1];
                        tmp2[1] = tmps;
                    }

                }
                tmp[2] = runeBox;
                Runes.Add(new Rune() { Name = it.Name, Id = it.Serial, Containers = tmp, ContainersName = tmp2 });
            }
            UO.PrintInformation("Nacteno");
        }

        private static void openBoxes(UOItem mainBox, int delay)
        {
            List<Graphic> containerType = new List<Graphic>() { 0x09AA, 0x0E7D, 0x0E75, 0x0E79, 0x09B0, 0x0E76 };
            foreach (UOItem it in mainBox.AllItems.Where(x => containerType.Any(xx => xx == x.Graphic)))
            {

                it.Use();
                UO.Wait(delay);
                foreach (UOItem it2 in it.AllItems.Where(x => containerType.Any(xx => xx == x.Graphic)))
                {
                    it2.Use();
                    UO.Wait(delay);
                    foreach (UOItem it3 in it2.AllItems.Where(x => containerType.Any(xx => xx == x.Graphic)))
                    {
                        it3.Use();
                        UO.Wait(delay);

                    }
                }

            }
        }

        public static void findRune(Rune r)
        {
            if (new UOItem(r.Containers[2]).Distance < 5)
            {
                if (r.Containers[2] > 0x0) new UOItem(r.Containers[2]).Use();
                UO.Wait(100);
                if (r.Containers[1] > 0x0) new UOItem(r.Containers[1]).Use();
                UO.Wait(100);
                if (r.Containers[0] > 0x0) new UOItem(r.Containers[0]).Use();
                UO.Wait(100);
            }
            else UO.PrintError("Nedosahnes na bednu");
        }

        public static void FillTreeView(TreeView t)
        {
            try
            {
                t.Nodes.Clear();
                Runes.Sort((x, y) => x.Name.CompareTo(y.Name));
                foreach (Rune r in Runes)
                {
                    if (r.ContainersName[0] != "null")
                    {
                        if (t.Nodes[r.ContainersName[0]] == null)
                            t.Nodes.Add(new TreeNode(r.ContainersName[0]) { Name = r.ContainersName[0] });
                        if (r.ContainersName[1] != "null")
                        {
                            if (t.Nodes[r.ContainersName[0]].Nodes[r.ContainersName[1]] == null)
                                t.Nodes[r.ContainersName[0]].Nodes.Add(new TreeNode(r.ContainersName[1]) { Name = r.ContainersName[1] });
                            t.Nodes[r.ContainersName[0]].Nodes[r.ContainersName[1]].Nodes.Add(new TreeNode(r.Name) { Name = r.Name, Tag = r.Id });
                        }
                        else
                        {
                            t.Nodes[r.ContainersName[0]].Nodes.Add(new TreeNode(r.Name) { Name = r.Name, Tag = r.Id });
                        }
                    }
                    else
                    {
                        t.Nodes.Add(new TreeNode(r.Name) { Name = r.Name, Tag = r.Id });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


    }
}
