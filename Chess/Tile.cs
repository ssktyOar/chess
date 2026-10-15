using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal class Tile : Button
    {
        // Number represents what type of tile it is
        // 0 - empty, 1 - pawn, 2 - rook, 3 - knight, 4 - bishop, 5 - queen, 6 - king
        // If number is positive, then figure is white
        // Otherwise, it's black
        internal int chessType = 0;
        internal int positionX;
        internal int positionY;

        private readonly Board board;

        public Tile(Board b, int type, string name, int _x, int _y, Color c)
        {
            chessType = type;
            positionX = _x;
            positionY = _y;
            Location = new Point(positionX * 125, positionY * 125);
            Size = new Size(125, 125);
            Text = name;
            BackColor = c;
            ForeColor = Color.FromArgb(150, 150, 150);
            board = b;
            // Click += new EventHandler(Choose);
            MouseClick += new MouseEventHandler(Choose);
        }

        internal void AcceptTile(int tp, string s, Image? img)
        {
            chessType = tp;
            Text = s;
            Image = img;
        }

        internal void NullTile() 
        {
            chessType = 0;
            Text = "";
            Image = null;
        }


        private void Choose(object? sender, EventArgs e)
        {
            if (board.MovingTile != null)
            {
                board.DestinationTile = this;
                board.MovePiece();
            }
            else if (chessType != 0)
            {
                board.MovingTile = this;
            }
        }
    }
}
