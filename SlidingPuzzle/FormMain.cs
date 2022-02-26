using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SlidingPuzzle
{
    public partial class FormMain : Form
    {
        OpenFileDialog openFileDialog;
        Image image;
        PictureBox pictureBox;
        PictureBox[] pictureBoxes;
        Image[] images;
        PictureTile blankTile;
        Controller controller;
        bool isRuning;
        string imageFile;

        public FormMain()
        {
            InitializeComponent();
            Resize += FormMain_Resize;
            buttonStart.Click += ButtonStart_Click;
            buttonOpenImage.Click += ButtonOpenImage_Click;
            openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image File|*jpg;*.jpeg;*.png;*.bmp|All File|*";
            numericUpDownRows.MouseWheel += NumericUpDownRows_MouseWheel;
            numericUpDownColumns.MouseWheel += NumericUpDownColumns_MouseWheel;
        }

        private void NumericUpDownColumns_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
            if (e.Delta > 0 && numericUpDownColumns.Value < numericUpDownColumns.Maximum)
            {
                numericUpDownColumns.Value++;
            }
            else if (e.Delta < 0 && numericUpDownColumns.Value > numericUpDownColumns.Minimum)
            {
                numericUpDownColumns.Value--;
            }
        }

        private void NumericUpDownRows_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
            if (e.Delta > 0 && numericUpDownRows.Value < numericUpDownRows.Maximum)
            {
                numericUpDownRows.Value++;
            }
            if (e.Delta < 0 && numericUpDownRows.Value > numericUpDownRows.Minimum)
            {
                numericUpDownRows.Value--;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (isRuning)
            {
                switch (keyData)
                {
                    case Keys.Down:
                        MoveTile(blankTile.X - 1, blankTile.Y);
                        CheckStatus();
                        break;
                    case Keys.Up:
                        MoveTile(blankTile.X + 1, blankTile.Y);
                        CheckStatus();
                        break;
                    case Keys.Right:
                        MoveTile(blankTile.X, blankTile.Y - 1);
                        CheckStatus();
                        break;
                    case Keys.Left:
                        MoveTile(blankTile.X, blankTile.Y + 1);
                        CheckStatus();
                        break;
                }
            }
            return false;
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (isRuning || image == null || Width < 50 || Height < 100)
            {
                return;
            }
            LoadImage(imageFile);
        }

        private void ButtonOpenImage_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadImage(openFileDialog.FileName);
            }
        }

        private void LoadImage(string fileName)
        {
            try
            {
                image = CreateBitmapImage(Image.FromFile(fileName));
                imageFile = fileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            if (image != null)
            {
                if (pictureBoxes != null)
                {
                    RemoveTiles();
                }
                else if (pictureBox != null)
                {
                    groupBoxPuzzle.Controls.Remove(pictureBox);
                    pictureBox.Dispose();
                    pictureBox = null;
                }
                numericUpDownRows.Enabled = true;
                numericUpDownColumns.Enabled = true;
                buttonOpenImage.Enabled = true;
                buttonStart.Enabled = true;
                isRuning = false;
                buttonStart.Text = "Start";
                pictureBox = new PictureBox();
                pictureBox.Size = image.Size;
                groupBoxPuzzle.Controls.Add(pictureBox);
                pictureBox.Image = image;
            }
        }

        private Bitmap CreateBitmapImage(Image image)
        {
            int h, w;
            if ((float)image.Height / image.Width > (float)groupBoxPuzzle.Height / groupBoxPuzzle.Width)
            {
                h = groupBoxPuzzle.Height;
                w = h * image.Width / image.Height;
            }
            else
            {
                w = groupBoxPuzzle.Width;
                h = w * image.Height / image.Width;
            }
            Bitmap bitmap = new Bitmap(w, h);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);
            graphics.DrawImage(image, new Rectangle(0, 0, w, h));
            graphics.Flush();
            return bitmap;
        }

        private void RemoveTiles()
        {
            for (int i = 0; i < controller.SquaresCount; i++)
            {
                pictureBoxes[i].Image = null;
                groupBoxPuzzle.Controls.Remove(pictureBoxes[i]);
            }
            pictureBoxes = null;
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            if (isRuning)
            {
                LoadImage(imageFile);
            }
            else
            {
                controller = new Controller();
                controller.X = (int)numericUpDownRows.Value;// int.Parse(comboBoxRows.Text);
                controller.Y = (int)numericUpDownColumns.Value;// int.Parse(comboBoxColumns.Text);
                controller.SquaresCount = controller.X * controller.Y;
                GenerateLevel();
                buttonOpenImage.Enabled = false;
                numericUpDownRows.Enabled = false;
                numericUpDownColumns.Enabled = false;
                buttonStart.Text = "New Game";
                isRuning = true;
            }
        }

        private void GenerateLevel()
        {
            if (pictureBox != null)
            {
                groupBoxPuzzle.Controls.Remove(pictureBox);
                pictureBox.Dispose();
                pictureBox = null;
            }
            if (pictureBoxes == null)
            {
                images = new Image[controller.SquaresCount];
                pictureBoxes = new PictureBox[controller.SquaresCount];
            }
            int countRow = controller.X;
            int countCol = controller.Y;
            int unitWidth = image.Width / countCol;
            int unitHeight = image.Height / countRow;
            for (int i = 0; i < controller.SquaresCount; i++)
            {
                controller.Indices[i] = i;
                if (pictureBoxes[i] == null)
                {
                    pictureBoxes[i] = new PictureTile();
                }
                pictureBoxes[i].Width = unitWidth;
                pictureBoxes[i].Height = unitHeight;
                ((PictureTile)pictureBoxes[i]).Index = i;
                CreateBitmapImages(i, countCol, unitWidth, unitHeight);
                pictureBoxes[i].Location = new Point(unitWidth * (i % countCol), unitHeight * (i / countCol));
                if (!groupBoxPuzzle.Controls.Contains(pictureBoxes[i]))
                    groupBoxPuzzle.Controls.Add(pictureBoxes[i]);
            }
            for (int i = 0; i < controller.SquaresCount; i++)
            {
                pictureBoxes[i].Image = images[controller.Indices[i]];
                ((PictureTile)pictureBoxes[i]).ImageIndex = controller.Indices[i];
            }
            int k = 0;
            for (int i = 0; i < controller.X; i++)
                for (int j = 0; j < controller.Y; j++)
                {
                    ((PictureTile)pictureBoxes[k]).X = i;
                    ((PictureTile)pictureBoxes[k]).Y = j;
                    k++;
                }
            using (Graphics grp = Graphics.FromImage(images[controller.Indices[controller.SquaresCount - 1]]))
            {
                grp.FillRectangle(Brushes.White, 0, 0, unitWidth, unitHeight);
            }
            blankTile = controller.SearchTile(controller.X - 1, controller.Y - 1, pictureBoxes);
            Shuffle();
            for (int i = 0; i < controller.SquaresCount; i++)
            {
                pictureBoxes[i].Click += Tile_Click;
            }
        }

        private void CreateBitmapImages(int index, int constCol, int unitWidth, int unitHeight)
        {
            images[index] = new Bitmap(unitWidth, unitHeight);
            Graphics graphics = Graphics.FromImage(images[index]);
            graphics.Clear(Color.White);
            graphics.DrawImage(image, new Rectangle(0, 0, unitWidth, unitHeight), new Rectangle(unitWidth * (index % constCol), unitHeight * (index / constCol), unitWidth, unitHeight), GraphicsUnit.Pixel);
            graphics.Flush();
        }

        public void Shuffle()
        {
            Random rand = new Random();
            do
            {
                int n = 0;
                while (n++ < 4000)
                {
                    switch (rand.Next(4))
                    {
                        case 0:
                            MoveTile(blankTile.X, blankTile.Y - 1);
                            break;
                        case 1:
                            MoveTile(blankTile.X, blankTile.Y + 1);
                            break;
                        case 2:
                            MoveTile(blankTile.X - 1, blankTile.Y);
                            break;
                        case 3:
                            MoveTile(blankTile.X + 1, blankTile.Y);
                            break;
                    }
                }
            } while (IsWin());
        }

        private void MoveTile(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < controller.X && y < controller.Y)
            {
                PictureTile secondTile = controller.SearchTile(x, y, pictureBoxes);
                if (secondTile != null)
                {
                    blankTile = controller.SwapTiles(blankTile, secondTile, images);
                }
            }
        }

        private void Tile_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < controller.SquaresCount; i++)
            {
                if (sender == pictureBoxes[i])
                {
                    int move;
                    if (((PictureTile)pictureBoxes[i]).X == blankTile.X)
                    {
                        if (blankTile.Y > ((PictureTile)pictureBoxes[i]).Y)
                        {
                            move = blankTile.Y - ((PictureTile)pictureBoxes[i]).Y;
                            for (int j = 0; j < move; j++)
                            {
                                MoveTile(blankTile.X, blankTile.Y - 1);
                            }
                        }
                        else
                        {
                            move = ((PictureTile)pictureBoxes[i]).Y - blankTile.Y;
                            for (int j = 0; j < move; j++)
                            {
                                MoveTile(blankTile.X, blankTile.Y + 1);
                            }
                        }
                    }
                    else if (((PictureTile)pictureBoxes[i]).Y == blankTile.Y)
                    {
                        if (blankTile.X > ((PictureTile)pictureBoxes[i]).X)
                        {
                            move = blankTile.X - ((PictureTile)pictureBoxes[i]).X;
                            for (int j = 0; j < move; j++)
                            {
                                MoveTile(blankTile.X - 1, blankTile.Y);
                            }
                        }
                        else
                        {
                            move = ((PictureTile)pictureBoxes[i]).X - blankTile.X;
                            for (int j = 0; j < move; j++)
                            {
                                MoveTile(blankTile.X + 1, blankTile.Y);
                            }
                        }
                    }
                    CheckStatus();
                }
            }
        }

        private void CheckStatus()
        {
            if (IsWin())
            {
                RemoveTiles();
                LoadImage(imageFile);
                Task.Run(() =>
                {
                    System.Threading.Thread.Sleep(500);
                    Invoke(new Action(() =>
                    {
                        pictureBox.Visible = false;
                    }));
                    System.Threading.Thread.Sleep(300);
                    Invoke(new Action(() =>
                    {
                        pictureBox.Visible = true;
                    }));
                });
            }
        }

        private bool IsWin()
        {
            foreach (PictureTile item in pictureBoxes)
            {
                if (item.ImageIndex != item.Index)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
