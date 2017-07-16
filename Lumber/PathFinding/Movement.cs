using Phoenix;
using Phoenix.WorldData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Lumber.PathFinding
{
    public class Movement
    {

        private Keys NORTH = System.Windows.Forms.Keys.PageUp;
        private Keys SOUTH = System.Windows.Forms.Keys.End;
        private Keys WEST = System.Windows.Forms.Keys.Home;
        private Keys EAST = System.Windows.Forms.Keys.PageDown;

        private Keys UP = System.Windows.Forms.Keys.Up;
        private Keys DOWN = System.Windows.Forms.Keys.Down;
        private Keys LEFT = System.Windows.Forms.Keys.Left;
        private Keys RIGHT = System.Windows.Forms.Keys.Right;
        private DateTime start;


        public void moveToPosition(Point p)
        {
            start = DateTime.Now;
            moveToPosition(p.X, p.Y);
        }
        private void moveToPosition(int x, int y)
        {
            int px = World.Player.X;
            int py = World.Player.Y;

            //UO.Print("Moving to {0},{1}", x, y);

            while (!inPosition(x, y))
            {
                if (py == y)
                {
                    if (px > x)
                    {
                        while (!inPosition(x, y) && px > x)
                        {
                            moveWest();
                            px = World.Player.X;
                            py = World.Player.Y;
                        }
                    }
                    else
                    {
                        while (!inPosition(x, y) && px < x)
                        {
                            moveEast();
                            px = World.Player.X;
                            py = World.Player.Y;
                        }
                    }
                }
                else if (px == x)
                {
                    if (py > y)
                    {
                        while (!inPosition(x, y) && py > y)
                        {
                            moveNorth();
                            px = World.Player.X;
                            py = World.Player.Y;
                        }
                    }
                    else
                    {
                        while (!inPosition(x, y) && py < y)
                        {
                            moveSouth();
                            px = World.Player.X;
                            py = World.Player.Y;
                        }
                    }
                }
                else if (py > y)
                {
                    if (px > x)
                    {
                        while (!inPosition(x, y) && px > x && py > y)
                        {
                            moveUp();
                            px = World.Player.X;
                            py = World.Player.Y;
                        }
                    }
                    else
                    {
                        while (!inPosition(x, y) && px < x && py > y)
                        {
                            moveRight();
                            px = World.Player.X;
                            py = World.Player.Y;
                        }
                    }
                }
                else if (py < y)
                {
                    if (px > x)
                    {
                        while (!inPosition(x, y) && px > x && py < y)
                        {
                            moveLeft();
                            px = World.Player.X;
                            py = World.Player.Y;
                        }
                    }
                    else
                    {
                        while (!inPosition(x, y) && px < x && py < y)
                        {
                            moveDown();
                            px = World.Player.X;
                            py = World.Player.Y;
                        }
                    }
                }

                px = World.Player.X;
                py = World.Player.Y;
            }
            //UO.Print("In position!");
        }

        private bool inPosition(int x, int y)
        {
            int px = World.Player.X;
            int py = World.Player.Y;
            //if (px > x + 1 || py > y + 1 || px < x - 1 || py < y - 1)
            if (px != x || py != y)
            {
                if (DateTime.Now - start > TimeSpan.FromSeconds(4)) return true;
                return false;
            }
            return true;
        }

        private void move(Keys direction)
        {
            int px = World.Player.X;
            int py = World.Player.Y;

            Keys dir = intToDirection(World.Player.Direction);
            if (dir == direction)
            {
                UO.Press(direction);
                UO.Wait(100);
            }
            else
            {
                UO.Press(direction);
                UO.Press(direction);
                UO.Wait(100);
            }
            if (px == World.Player.X && py == World.Player.Y)
            {
                move(changeDirection(direction));
            }
        }

        private Keys changeDirection(Keys direction)
        {

            if (direction == NORTH)
            {
                return EAST;
            }

            if (direction == SOUTH)
            {
                return WEST;
            }

            if (direction == WEST)
            {
                return NORTH;
            }

            if (direction == EAST)
            {
                return SOUTH;
            }

            if (direction == RIGHT)
            {
                return DOWN;
            }

            if (direction == DOWN)
            {
                return LEFT;
            }

            if (direction == LEFT)
            {
                return UP;
            }

            return RIGHT;
        }

        private Keys intToDirection(int dir)
        {
            if (dir == 0)
            {
                return NORTH;
            }
            if (dir == 1)
            {
                return RIGHT;
            }
            if (dir == 2)
            {
                return EAST;
            }
            if (dir == 3)
            {
                return DOWN;
            }
            if (dir == 4)
            {
                return SOUTH;
            }
            if (dir == 5)
            {
                return LEFT;
            }
            if (dir == 6)
            {
                return WEST;
            }

            return UP;
        }

        /*
            West(Home)   North(PageUp)
                       x
            South(End)   East(PageDown)
        */
        private void moveNorth()
        {
            move(NORTH);
        }

        private void moveSouth()
        {
            move(SOUTH);
        }

        private void moveWest()
        {
            move(WEST);
        }

        private void moveEast()
        {
            move(EAST);
        }

        private void moveRight()
        {
            move(RIGHT);
        }

        private void moveLeft()
        {
            move(LEFT);
        }

        private void moveDown()
        {
            move(DOWN);
        }

        private void moveUp()
        {
            move(UP);
        }

        

    }
}
