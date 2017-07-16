using Phoenix;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lumber
{
    public class Class1
    {
        public Class1()
        {



            

                
        }
        [Command]
        public void test(int x, int y)
        {
            Ultima.Tile l = Ultima.Map.Felucca.Tiles.GetLandTile(x, y);
            var i=DataFiles.Tiledata.GetLand(l.ID);
            UO.Print(i.Flags.ToString());
        }


        [Command]
        public void test3()
        {
            var i = Field.GetField(World.Player.X, World.Player.Y);
        }

        [Command]
        public void test2(int x, int y)
        {
            var l = Ultima.Map.Felucca.Tiles.GetStaticTiles(x, y); // z==1

            foreach(var v in l)
            {
                var i = DataFiles.Tiledata.GetArt(v.ID);
            }

        }
    }
}
