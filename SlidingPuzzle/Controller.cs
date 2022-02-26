using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlidingPuzzle
{
    public class Controller
    {
        private int[] indices;
        private int squaresCount;
        public int SquaresCount
        {
            get
            {
                return squaresCount;
            }
            set
            {
                squaresCount = value;
                indices = new int[squaresCount];
            }
        }

        public int X { get; set; }
        public int Y { get; set; }

        public int[] Indices { get { return indices; } set { indices = value; } }

        public PictureTile SearchTile(int x, int y, System.Windows.Forms.PictureBox[] pictureBoxes)
        {
            for (int i = 0; i < squaresCount; i++)
            {
                if (((PictureTile)pictureBoxes[i]).X == x && ((PictureTile)pictureBoxes[i]).Y == y)
                    return ((PictureTile)pictureBoxes[i]);
            }
            return null;
        }

        public PictureTile SwapTiles(PictureTile Tile1, PictureTile Tile2, System.Drawing.Image[] images)
        {
            int tmp = Tile2.ImageIndex;
            Tile2.Image = images[Tile1.ImageIndex];
            Tile2.ImageIndex = Tile1.ImageIndex;
            Tile1.Image = images[tmp];
            Tile1.ImageIndex = tmp;
            return Tile2;
        }
    }
}
