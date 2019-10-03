using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace ConvexHull
{
    public partial class Form1 : Form
    {
        private List<Point> points = new List<Point>();
        private SolidBrush brush = new SolidBrush(Color.Red);
        private Pen pen = new Pen(Color.Black, 1);
        private Graphics g;


        public Form1()
        {
            InitializeComponent();
            pictureBox1.Image = new Bitmap(pictureBox1.Width,pictureBox1.Height);
            g = Graphics.FromImage(pictureBox1.Image);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.Clear(Color.White);
        }

        private void buildBtn_Click(object sender, EventArgs e)
        {

        }

        //очищаем экран и обнуляем список
        private void clearBtn_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            points.Clear();
            pictureBox1.Invalidate();
        }

        //рисуем заданные точки и добавляем их в массив точек
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            points.Add(e.Location);
            g.FillEllipse(brush, e.X - 3, e.Y - 3, 7, 7);
            pictureBox1.Invalidate();
        }
    }
}
