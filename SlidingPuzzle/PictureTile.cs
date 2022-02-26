using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlidingPuzzle
{
    public class PictureTile : System.Windows.Forms.PictureBox
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Index { get; set; }
        public int ImageIndex { get; set; }
    }
}
