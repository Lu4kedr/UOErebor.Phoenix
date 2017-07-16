using Phoenix;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Lumber
{
    public class Field
    {
        public bool IsWalkable { get; set; }
        public bool IsTree { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public int Distance
        {
            get
            {
                return (int)(Math.Sqrt(Math.Pow(X-World.Player.X, 2)+ Math.Pow(Y - World.Player.Y, 2)));
            }
        }

        public StaticTree Tree { get; set; }

        public Point ClosestWalkable
        {
            get
            {
                if (!IsWalkable)
                {
                    for (var x = X - 1; x <= X + 1; x++)
                    {
                        for (var y = Y - 1; y <= Y + 1; y++)
                        {
                            var tmp = GetField((ushort)x, (ushort)y);
                            if (tmp != null & tmp.IsWalkable)
                                return new Point(tmp.X, tmp.Y);
                        }
                    }
                }
                else return new Point(X, Y);

                return new Point(-1, -1);
            }
        }

        public bool Mine()
        {
            if (IsTree)
            {
                UO.WaitTargetTile(Tree.X, Tree.Y, Tree.Z, Tree.ID);
                UO.Say(".usehand");
                return true;
            }
            else UO.PrintError("Neni Strom");
            return false;
        }

        public void Harvested()
        {
            Tree.Harvested = DateTime.Now;
        }

        public static Field GetField(ushort x, ushort y)
        {
            StaticTree t = new StaticTree();
            Field f = new Field();
            f.X = x;
            f.Y = y;
            var l = Ultima.Map.Felucca.Tiles.GetStaticTiles(x, y);
            if (l.Length == 0)
            {
                Ultima.Tile tl = Ultima.Map.Felucca.Tiles.GetLandTile(x, y);
                var i = DataFiles.Tiledata.GetLand(tl.ID);
                if (i.Name.Contains("forest") || i.Name.Contains("grass"))
                {
                    f.IsWalkable = true;
                    f.IsTree = false;
                    f.Tree = new StaticTree();
                }
                else
                {
                    if(((DataFiles.Tiledata.GetArt(tl.ID).Flags & MulLib.TileData.Flags.Impassible) != 0))
                    {
                        f.IsWalkable = true;
                        f.IsTree = false;
                        f.Tree = new StaticTree();
                    }
                    else
                    {
                        f.IsWalkable = false;
                        f.IsTree = false;
                        f.Tree = new StaticTree();

                    }

                }
                return f;

            }
            else
            {
                foreach(var i in l)
                {
                    if(DataFiles.Tiledata.GetArt(i.ID).Name.Contains("tree"))
                    {
                        t.ID = i.ID;
                        t.X = x;
                        t.Y = y;
                        t.Z = (sbyte)i.Z;
                        t.Harvested = DateTime.MinValue;

                        f.IsWalkable = false;
                        f.IsTree = true;
                        f.Tree = t;
                        return f;
                    }
                    if (((DataFiles.Tiledata.GetArt(i.ID).Flags & MulLib.TileData.Flags.Impassible) == 0))
                    {
                        f.IsWalkable = true;
                        f.IsTree = false;
                        f.Tree = t;
                        return f;

                    }
                    else
                    {
                        f.IsWalkable = false;
                        f.IsTree = false;
                        f.Tree = t;
                    }
                }
                return f;
                //if (l.Length == 1 & !DataFiles.Tiledata.GetArt(l[0].ID).Name.Contains("tree"))
                //{
                //    if (((DataFiles.Tiledata.GetArt(l[0].ID).Flags & MulLib.TileData.Flags.Impassible) != 0))
                //    {
                //        f.IsWalkable = false;
                //        f.IsTree = false;
                //        f.Tree = new StaticTree();
                //    }
                //    else
                //    {
                //        f.IsWalkable = true;
                //        f.IsTree = false;
                //        f.Tree = new StaticTree();
                //    }
                //    return f;
                //}
                //else
                //{
                //    for (var i = 0; i < l.Length; i++)
                //    {
                //        var data = DataFiles.Tiledata.GetArt(l[i].ID);
                //        if (data.Name.Contains("tree"))
                //        {
                //            t.ID = l[i].ID;
                //            t.X = x;
                //            t.Y = y;
                //            t.Z = (sbyte)l[i].Z;
                //            t.Harvested = DateTime.MinValue;

                //            f.IsWalkable = false;
                //            f.IsTree = true;
                //            f.Tree = t;
                //            return f;
                //        }

                //    }
                //}
            }

        }
    }
}
