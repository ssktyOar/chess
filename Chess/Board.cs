using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal class Board : Panel
    {
        private readonly int[] order = [2, 3, 4, 5, 6, 4, 3, 2];
        private readonly int[] unitsWhite = [8, 2, 2, 2, 1, 1];
        private readonly int[] unitsBlack = [8, 2, 2, 2, 1, 1];
        private readonly string[] names = ["Rook", "Knight", "Bishop", "Queen", "King", "Bishop", "Knight", "Rook"];
        internal readonly Tile[,] tiles;
        private readonly Image White;
        private readonly Image Black;
        private int moveOrder = 1;
        //private bool swapWhite = true;
        //private bool swapBlack = true;

        internal Tile? MovingTile;
        internal Tile? DestinationTile;

        public Board()
        {
            White = Image.FromFile("White.bmp");
            Black = Image.FromFile("Black.bmp");
            Image img;
            Tile t; int side; Color c;
            Location = new Point(435, 12);
            Name = "Board";
            Size = new Size(1000, 1000);
            TabIndex = 4;
            tiles = new Tile[8, 8];
            for (int y = 0; y < 8; y++)
            {
                if (y < 4) // For easier spawning black and white team
                {
                    side = -1;
                    img = Black;
                }
                else
                {
                    side = 1;
                    img = White;
                }
                for (int x = 0; x < 8; x++)
                {
                    if ((x+y)%2 > 0) // Is tile dark or light colored?
                    {
                        c = Color.FromArgb(100, 100, 100);
                    }
                    else
                    {
                        c = Color.White;
                    }
                    // Spawning line of main black or white figures
                    if (y == 0 || y == 7)
                    {
                        t = new Tile(this, order[x] * side, names[x], x, y, c) { Image = img };
                        Controls.Add(t);
                        tiles[x, y] = t;
                    }
                    // Spawning line of pawns
                    else if (y == 1 || y == 6)
                    {
                        t = new Tile(this, side, "Pawn", x, y, c) { Image = img };
                        Controls.Add(t);
                        tiles[x, y] = t;
                    }
                    else
                    { // Empty tiles
                        t = new Tile(this, 0, "", x, y, c);
                        Controls.Add(t);
                        tiles[x, y] = t;
                    }
                }
            }
        }

        internal void SaveGame(object? sender, EventArgs e)
        {
            try
            {
                // Saving the current state of the desk
                File.OpenWrite("savegame.txt");
                List<string> content = [];
                for (int x = 0; x < 8; x++) 
                {
                    for (int y = 0; y < 8; y++)
                    {
#pragma warning disable CS1690 // Accessing a member on a field of a marshal-by-reference class may cause a runtime exception
                        content.Add(tiles[x, y].chessType.ToString() + " " + tiles[x, y].Text + x.ToString() + " " + y.ToString());
#pragma warning restore CS1690 // Accessing a member on a field of a marshal-by-reference class may cause a runtime exception
                    }
                }
                File.WriteAllLines("savegame.txt", content);
            }
            catch
            {
                Console.WriteLine("The game haven't been saved");
            }
        }

        internal void LoadGame(object? sender, EventArgs e)
        {
            try
            {
                File.OpenRead("savegame.txt");
                string[] sss = File.ReadAllLines("savegame.txt");
                string[] _s;
                Image img; int _type;

                for (int i = 0; i < sss.Length; i++) 
                {
                    _s = sss[i].Split(" ");
                    _type = Convert.ToInt32(_s[0]);
                    if (_type > 0)
                    {
                        img = White;
                    }
                    else
                    {
                        img = Black;
                    }
                    tiles[Convert.ToInt32(_s[2]), Convert.ToInt32(_s[3])].AcceptTile(_type, _s[1], img);
                }
            }
            catch
            {
                Console.WriteLine("The save haven't been loaded");
            }
        }

        internal void MovePiece()
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            lock (MovingTile)
            {

                lock (DestinationTile)
                {
                    if (moveOrder < 0 && MovingTile.chessType > 0 || moveOrder < 0 && MovingTile.chessType > 0)
                    {
                        // MovingTile = null; DestinationTile = null; return;
                    }
                    // In case if it's some bug or misclick, operation will be interrupted
                    if (MovingTile == null || MovingTile.chessType == 0 || DestinationTile == null)
                    { MovingTile = null; DestinationTile = null; return; }

                    if (DestinationTile == MovingTile) { MovingTile = null; DestinationTile = null; return; }

                    // Preventing friendly fire, unless it's rook targeting it's own king - than rook will do swapping move and will protect king

                    // && !(Math.Abs(MovingTile.chessType) == 2 && Math.Abs(DestinationTile.chessType) == 6) && (swapBlack || swapWhite)
                    if ((MovingTile.chessType > 0 && DestinationTile.chessType > 0 || MovingTile.chessType < 0 && DestinationTile.chessType < 0) )
                    { MovingTile = null; DestinationTile = null; return; }

                    // If this is a pawn, checking it's movement assording to rules
                    if (MovingTile.chessType == 1 || MovingTile.chessType == -1)
                    {
                        if (Math.Abs(MovingTile.positionX - DestinationTile.positionX) > 1)
                        { MovingTile = null; DestinationTile = null; return; }

                        // Checking if these are white pawns
                        if (MovingTile.chessType == 1)
                        {
                            if (MovingTile.positionY - DestinationTile.positionY <= 2)
                            {
                                if (MovingTile.positionX == DestinationTile.positionX)
                                {
                                    // Preventing figure from going throught others
                                    if (tiles[MovingTile.positionX, MovingTile.positionY - 1].chessType != 0 && MovingTile.chessType > 0 || tiles[MovingTile.positionX, MovingTile.positionY + 1].chessType != 0 && MovingTile.chessType < 0)
                                    {
                                        DestinationTile = null;
                                        MovingTile = null;
                                        return;
                                    }
                                    else
                                    {
                                        if (MovingTile.positionY - DestinationTile.positionY == 2 && MovingTile.positionY != 6)
                                        {
                                            DestinationTile = null;
                                            MovingTile = null;
                                            return;
                                        }
                                        moveOrder *= (-1);
                                        Thread.MemoryBarrier();
                                        DestinationTile.AcceptTile(MovingTile.chessType, MovingTile.Text, MovingTile.Image);
                                        Thread.MemoryBarrier();
                                        MovingTile.NullTile();
                                        DestinationTile = null;
                                        MovingTile = null;
                                    }

                                }
                                else if ((Math.Abs(MovingTile.positionX - DestinationTile.positionX) == 1) 
                                    && (MovingTile.positionY - DestinationTile.positionY == 1)
                                    && DestinationTile.chessType != 0)
                                {
                                    moveOrder *= (-1);
                                    DestinationTile.AcceptTile(MovingTile.chessType, MovingTile.Text, MovingTile.Image);
                                    MovingTile.NullTile();
                                    DestinationTile = null;
                                    MovingTile = null;
                                }
                            } else { DestinationTile = null; MovingTile = null; return; }
                        }
                        // Flipping the numbers if those are black pawns
                        else
                        {
                            if (DestinationTile.positionY - MovingTile.positionY <= 2)
                            {
                                if (MovingTile.positionX == DestinationTile.positionX)
                                {
                                    // Preventing figure from going throught others
                                    if (tiles[MovingTile.positionX, MovingTile.positionY + 1].chessType != 0 && MovingTile.chessType < 0 || DestinationTile.positionY != tiles[MovingTile.positionX, MovingTile.positionY + 1].positionY)
                                    {
                                        DestinationTile = null;
                                        MovingTile = null;
                                        return;
                                    }
                                    else
                                    {
                                        if (DestinationTile.positionY - MovingTile.positionY == 2 && MovingTile.positionY != 1)
                                        {
                                            DestinationTile = null;
                                            MovingTile = null;
                                            return;
                                        }
                                        moveOrder *= (-1);
                                        Thread.MemoryBarrier();
                                        DestinationTile.AcceptTile(MovingTile.chessType, MovingTile.Text, MovingTile.Image);
                                        Thread.MemoryBarrier();
                                        MovingTile.NullTile();
                                        DestinationTile = null;
                                        MovingTile = null;
                                    }

                                }
                                else if ((Math.Abs(MovingTile.positionX - DestinationTile.positionX) == 1)
                                    && (DestinationTile.positionY - MovingTile.positionY == 1)
                                    && DestinationTile.chessType != 0)
                                {
                                    moveOrder *= (-1);
                                    DestinationTile.AcceptTile(MovingTile.chessType, MovingTile.Text, MovingTile.Image);
                                    MovingTile.NullTile();
                                    DestinationTile = null;
                                    MovingTile = null;
                                }
                            } else { DestinationTile = null; MovingTile = null; return; }
                        }

                    }

                    // Code for rooks
                    else if (MovingTile.chessType == 2 || MovingTile.chessType == -2)
                    {
                        // If target not on the X or Y axis, then no movement
                        if (DestinationTile.positionY != MovingTile.positionY && DestinationTile.positionX != MovingTile.positionX)
                        { MovingTile = null; DestinationTile = null; return; }
                        else
                        {
                            int max, min;
                            if (DestinationTile.positionY == MovingTile.positionY)
                            {
                                if (MovingTile.positionX > DestinationTile.positionX)
                                {
                                    max = MovingTile.positionX;
                                    min = DestinationTile.positionX + 1;
                                }
                                else
                                {
                                    min = MovingTile.positionX + 1;
                                    max = DestinationTile.positionX;
                                }
                                for (int i = min; min < max; min++)
                                {
                                    if (tiles[i, DestinationTile.positionY].chessType != 0) 
                                    { MovingTile = null; DestinationTile = null; return; }
                                }
                                moveOrder *= (-1);
                                Thread.MemoryBarrier();
                                DestinationTile.AcceptTile(MovingTile.chessType, MovingTile.Text, MovingTile.Image);
                                Thread.MemoryBarrier();
                                MovingTile.NullTile();
                                DestinationTile = null;
                                MovingTile = null;
                                return;
                            }
                            else
                            {
                                if (MovingTile.positionY > DestinationTile.positionY)
                                {
                                    max = MovingTile.positionY;
                                    min = DestinationTile.positionY + 1;
                                }
                                else
                                {
                                    min = MovingTile.positionY + 1;
                                    max = DestinationTile.positionY;
                                }
                                for (int i = min; min < max; min++)
                                {
                                    if (tiles[DestinationTile.positionX, i].chessType != 0)
                                    { MovingTile = null; DestinationTile = null; return; }
                                }
                                moveOrder *= (-1);
                                Thread.MemoryBarrier();
                                DestinationTile.AcceptTile(MovingTile.chessType, MovingTile.Text, MovingTile.Image);
                                Thread.MemoryBarrier();
                                MovingTile.NullTile();
                                DestinationTile = null;
                                MovingTile = null;
                                return;
                            }
                        }
                    }

                    // Code for knights
                    else if (MovingTile.chessType == 3 || MovingTile.chessType == -3)
                    {
                        if (Math.Abs(MovingTile.positionX - DestinationTile.positionX) == 2 && Math.Abs(MovingTile.positionY - DestinationTile.positionY) == 1
                            || Math.Abs(MovingTile.positionX - DestinationTile.positionX) == 1 && Math.Abs(MovingTile.positionY - DestinationTile.positionY) == 2)
                        {
                            moveOrder *= (-1);
                            Thread.MemoryBarrier();
                            DestinationTile.AcceptTile(MovingTile.chessType, MovingTile.Text, MovingTile.Image);
                            Thread.MemoryBarrier();
                            MovingTile.NullTile();
                            DestinationTile = null;
                            MovingTile = null;
                            return;
                        }
                        else { MovingTile = null; DestinationTile = null; return; }
                    }

                    // Code for Bishops
                    else if (MovingTile.chessType == 4 || MovingTile.chessType == -4)
                    {
                        if (Math.Abs(MovingTile.positionX - DestinationTile.positionX) == Math.Abs(MovingTile.positionY - DestinationTile.positionY))
                        {
                            int xS, yS, _x, _y;

                            if (MovingTile.positionX > DestinationTile.positionX)
                            {
                                xS = 1;
                            } else { xS = -1; }

                            if (MovingTile.positionY > DestinationTile.positionY)
                            {
                                yS = 1;
                            }
                            else { yS = -1; }

                            _x = MovingTile.positionX;
                            _y = MovingTile.positionY;

                            while (_x < 6 && _x > 1 && _y < 6 && _y > 1 && _x != DestinationTile.positionX && _y != DestinationTile.positionY)
                            {
                                _x += xS;
                                _y += yS;
                                if (tiles[_x, _y].chessType != 0)
                                {
                                    MovingTile = null; DestinationTile = null; return;
                                }
                            }
                            moveOrder *= (-1);
                            Thread.MemoryBarrier();
                            DestinationTile.AcceptTile(MovingTile.chessType, MovingTile.Text, MovingTile.Image);
                            Thread.MemoryBarrier();
                            MovingTile.NullTile();
                            DestinationTile = null;
                            MovingTile = null;
                            return;
                        }
                        else
                        {
                            MovingTile = null; DestinationTile = null; return;
                        }
                    }

                    // Code for Queen
                    else if (MovingTile.chessType == 5 || MovingTile.chessType == -5)
                    {
                        if (Math.Abs(MovingTile.positionX - DestinationTile.positionX) == Math.Abs(MovingTile.positionY - DestinationTile.positionY))
                        {
                            int xS, yS, _x, _y;

                            if (MovingTile.positionX > DestinationTile.positionX)
                            {
                                xS = 1;
                            }
                            else { xS = -1; }

                            if (MovingTile.positionY > DestinationTile.positionY)
                            {
                                yS = 1;
                            }
                            else { yS = -1; }

                            _x = MovingTile.positionX;
                            _y = MovingTile.positionY;

                            while (_x < 6 && _x > 1 && _y < 6 && _y > 1 && _x != DestinationTile.positionX && _y != DestinationTile.positionY)
                            {
                                _x += xS;
                                _y += yS;
                                if (tiles[_x, _y].chessType != 0)
                                {
                                    MovingTile = null; DestinationTile = null; return;
                                }
                            }

                            Thread.MemoryBarrier();
                            DestinationTile.AcceptTile(MovingTile.chessType, MovingTile.Text, MovingTile.Image);
                            Thread.MemoryBarrier();
                            MovingTile.NullTile();
                            DestinationTile = null;
                            MovingTile = null;
                            return;
                        }
                        if (DestinationTile.positionY != MovingTile.positionY && DestinationTile.positionX != MovingTile.positionX)
                        { MovingTile = null; DestinationTile = null; return; }
                        else
                        {
                            int max, min;
                            if (DestinationTile.positionY == MovingTile.positionY)
                            {
                                if (MovingTile.positionX > DestinationTile.positionX)
                                {
                                    max = MovingTile.positionX;
                                    min = DestinationTile.positionX + 1;
                                }
                                else
                                {
                                    min = MovingTile.positionX + 1;
                                    max = DestinationTile.positionX;
                                }
                                for (int i = min; min < max; min++)
                                {
                                    if (tiles[i, DestinationTile.positionY].chessType != 0)
                                    { MovingTile = null; DestinationTile = null; return; }
                                }
                                moveOrder *= (-1);
                                Thread.MemoryBarrier();
                                DestinationTile.AcceptTile(MovingTile.chessType, MovingTile.Text, MovingTile.Image);
                                Thread.MemoryBarrier();
                                MovingTile.NullTile();
                                DestinationTile = null;
                                MovingTile = null;
                                return;
                            }
                            else
                            {
                                if (MovingTile.positionY > DestinationTile.positionY)
                                {
                                    max = MovingTile.positionY;
                                    min = DestinationTile.positionY + 1;
                                }
                                else
                                {
                                    min = MovingTile.positionY + 1;
                                    max = DestinationTile.positionY;
                                }
                                for (int i = min; min < max; min++)
                                {
                                    if (tiles[DestinationTile.positionX, i].chessType != 0)
                                    { MovingTile = null; DestinationTile = null; return; }
                                }
                                moveOrder *= (-1);
                                Thread.MemoryBarrier();
                                DestinationTile.AcceptTile(MovingTile.chessType, MovingTile.Text, MovingTile.Image);
                                Thread.MemoryBarrier();
                                MovingTile.NullTile();
                                DestinationTile = null;
                                MovingTile = null;
                                return;
                            }
                        }

                            // MovingTile = null; DestinationTile = null; return;
                    }

                    // Code for the King
                    else if (MovingTile.chessType == 6 || MovingTile.chessType == -6)
                    {
                        if (Math.Abs(MovingTile.positionX - DestinationTile.positionX) <= 1 && Math.Abs(MovingTile.positionY - DestinationTile.positionY) <= 1)
                        {
                            moveOrder *= (-1);
                            Thread.MemoryBarrier();
                            DestinationTile.AcceptTile(MovingTile.chessType, MovingTile.Text, MovingTile.Image);
                            Thread.MemoryBarrier();
                            MovingTile.NullTile();
                            DestinationTile = null;
                            MovingTile = null;
                            return;
                        }
                        else
                        {
                            MovingTile = null; DestinationTile = null; return;
                        }
                    }

                    MovingTile = null; DestinationTile = null;
                }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }

        }
    }
}
