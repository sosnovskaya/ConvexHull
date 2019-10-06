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

        //рисуем заданные точки и добавляем их в массив точек
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            points.Add(e.Location);
            g.FillEllipse(brush, e.Location.X - 3, e.Location.Y - 3, 7, 7);
            pictureBox1.Invalidate();
        }

        private void buildBtn_Click(object sender, EventArgs e)
        {
            int n = points.Count;
            if (n <= 5)
                bruteHull(points);
            else
            {
                List<Point> leftHull, rightHull;
                leftHull = points.GetRange(0, n / 2);
                rightHull = points.GetRange(n /2 , n/2);
                bruteHull(leftHull);
                bruteHull(rightHull);
            }
        }

        //очищаем экран и обнуляем список
        private void clearBtn_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            points.Clear();
            pictureBox1.Invalidate();
        }



        //алгоритм построения оболочки для малого количества точек
        private void bruteHull(List<Point> points)
        {
            bool allPointsOnTheRight;
            Point pti, ptj;
            for (int i = 0;i < points.Count; i++)
            {
                for(int j = 0; j < points.Count; j++)
                {
                    if (i == j)
                        continue;

                    pti = points[i];
                    ptj = points[j];

                    allPointsOnTheRight = true;

                    for (int k = 0; k < points.Count; k++)
                    {
                        if (k == i || k == j)
                            continue;
                        int side = findPointPos(points[k],pti,ptj);
                        if(side > 0)
                        {
                            allPointsOnTheRight = false;
                            break;
                        }
                    }

                    if(allPointsOnTheRight)
                    {
                        g.DrawLine(pen,pti,ptj);
                        pictureBox1.Invalidate();
                    }
                }
            }
        }

        //0 - принадлежит линии, > 0 - левее, < 0 - правее
        int findPointPos(PointF p, Point A, Point B)
        {
            float result = (B.X - A.X) * (p.Y - B.Y) - (B.Y - A.Y) * (p.X - B.X);
            return (int)result;
        }
    }
}
