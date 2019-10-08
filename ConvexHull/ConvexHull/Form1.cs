using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace ConvexHull
{
    public partial class Form1 : Form
    {
        private List<Point> points = new List<Point>();
        private SolidBrush brushRed = new SolidBrush(Color.Red);
        private SolidBrush brushRight = new SolidBrush(Color.Green);//вершины правой кучи
        private SolidBrush brushLeft = new SolidBrush(Color.Yellow);//вершины левой кучи
        private Pen penResult = new Pen(Color.Black, 1);
        private Pen penProcess = new Pen(Color.Blue, 1);
        Point mid = new Point(10000, 10000);
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
            g.FillEllipse(brushRed, e.Location.X - 3, e.Location.Y - 3, 7, 7);
            pictureBox1.Invalidate();
        }

        private void buildBtn_Click(object sender, EventArgs e)
        {
            Point[] hull;
            hull = DivivdeAndConguer(points).ToArray();
            for (int i = 0; i < hull.Length - 1; i++)
            {
                g.DrawLine(penResult, hull[i], hull[i + 1]);
                pictureBox1.Invalidate();
                pictureBox1.Refresh();
                System.Threading.Thread.Sleep(1000);
            }
            g.DrawLine(penResult,hull[hull.Length-1], hull[0]);
            pictureBox1.Invalidate();
        }

        List<Point>  DivivdeAndConguer(List<Point> points)
        {
            points = points.OrderBy(p => p.X).ToList();
            int n = points.Count;
            if (n <= 5)
                return BruteHull(points);
            else
            {
                List<Point> leftHull, rightHull;
                leftHull = points.GetRange(0, n / 2 );
                rightHull = points.GetRange(n / 2, n / 2 + n % 2);
                leftHull = DivivdeAndConguer(leftHull);
                rightHull = DivivdeAndConguer(rightHull);

                
                Point[] hull = rightHull.ToArray();
                Point[] hull2 = leftHull.ToArray();
                
                foreach (Point prop in hull)
                    g.FillEllipse(brushRight, prop.X - 3, prop.Y - 3, 7, 7);
                foreach (Point prop in hull2)
                    g.FillEllipse(brushLeft, prop.X - 3, prop.Y - 3, 7, 7);

                for (int i = 0; i < hull.Length - 1; i++)
                {
                    g.DrawLine(penProcess, hull[i], hull[i + 1]);
                    pictureBox1.Invalidate();
                    pictureBox1.Refresh();
                    System.Threading.Thread.Sleep(1000);
                }
                g.DrawLine(penProcess, hull[hull.Length -1], hull[0]);
                pictureBox1.Invalidate();

                for (int i = 0; i < hull2.Length - 1; i++)
                {
                    g.DrawLine(penProcess, hull2[i], hull2[i + 1]);
                    pictureBox1.Invalidate();
                    pictureBox1.Refresh();
                    System.Threading.Thread.Sleep(1000);
                }
                g.DrawLine(penProcess, hull2[hull2.Length - 1], hull2[0]);
                pictureBox1.Invalidate();

                return merge(leftHull,rightHull);
            }
        }

        List<Point> merge(List<Point> lefthull, List<Point> righthull)
        {
            List<Point> ConvexHull = new List<Point>();
            int n1 = lefthull.Count;
            int n2 = righthull.Count;
            int rihtmostLeft = 0, leftmostRight = 0;

            //крайняя права точка левой оболочки
            for (int i = 1; i < n1; i++)
                if (lefthull[i].X > lefthull[rihtmostLeft].X)
                    rihtmostLeft = i;
            g.FillEllipse(brushRed, lefthull[rihtmostLeft].X, lefthull[rihtmostLeft].Y - 3, 7, 7);
            //крайняя левая точка правой оболочки
            for (int i = 1; i < n2; i++)
                if (righthull[i].X < righthull[leftmostRight].X)
                    leftmostRight = i;
            g.FillEllipse(brushRed, righthull[leftmostRight].X, righthull[leftmostRight].Y - 3, 7, 7);

            // 
            int lowerLeft = rihtmostLeft, lowerRight = leftmostRight;
            bool tangentIsFinded = false;
            while (!tangentIsFinded)
            {
                tangentIsFinded = true;
                while (findPointPos(lefthull[(lowerLeft + 1) % n1], righthull[lowerRight], lefthull[lowerLeft]) <= 0)
                    lowerLeft = (lowerLeft + 1) % n1;

                while (findPointPos(righthull[(n2 + lowerRight - 1) % n2], lefthull[lowerLeft],righthull[lowerRight]) >= 0)
                {
                    lowerRight = (n2 + lowerRight - 1) % n2;
                    tangentIsFinded = false;
                }
            }
            g.DrawLine(penProcess, lefthull[lowerLeft], righthull[lowerRight]);
            pictureBox1.Invalidate();
            pictureBox1.Refresh();
            System.Threading.Thread.Sleep(1000);

            tangentIsFinded = false;
            int upperLeft = rihtmostLeft, upperRight = leftmostRight;
            while (!tangentIsFinded)
            {
                tangentIsFinded = true;
                while (findPointPos(lefthull[(n1 + upperLeft - 1) % n1],righthull[upperRight],lefthull[upperLeft]) >= 0)
                    upperLeft = (n1 + upperLeft - 1) % n1;

                while(findPointPos(righthull[(upperRight + 1) % n2],lefthull[upperLeft], righthull[upperRight]) <= 0)
                {
                    upperRight = (upperRight + 1) % n2;
                    tangentIsFinded = false;
                }
            }
            g.DrawLine(penProcess, lefthull[upperLeft], righthull[upperRight]);
            pictureBox1.Invalidate();
            pictureBox1.Refresh();
            System.Threading.Thread.Sleep(1000);

            /*
            
            // finding the upper tangent 
            int upperLeft = rihtmostLeft, upperRight = leftmostRight;
            bool done = false;
            while (!done)
            {
                done = true;
                while (orientation(righthull[upperRight], lefthull[upperLeft],lefthull[(upperLeft + 1) % n1]) >= 0)
                    upperLeft = (upperLeft + 1) % n1;

                while (orientation(lefthull[upperLeft], righthull[upperRight], righthull[(n2 + upperRight - 1) % n2]) <= 0)
                {
                    upperRight = (n2 + upperRight - 1) % n2;
                    done = false;
                }
            }
            int uppera = upperLeft, upperb = upperRight;
            g.DrawLine(penProcess, lefthull[upperLeft], righthull[upperRight]);
            pictureBox1.Invalidate();
            pictureBox1.Refresh();
            System.Threading.Thread.Sleep(1000);

            upperLeft = rihtmostLeft; upperRight = leftmostRight;
            done = false;
            while (!done)//finding the lower tangent 
            {
                done = true;
                while (orientation(lefthull[upperLeft], righthull[upperRight], righthull[(upperRight + 1) % n2]) >= 0)
                    upperRight = (upperRight + 1) % n2;

                while (orientation(righthull[upperRight], lefthull[upperLeft], lefthull[(n1 + upperLeft - 1) % n1]) <= 0)
                {
                    upperLeft = (n1 + upperLeft - 1) % n1;
                    done = false;
                }
            }
            g.DrawLine(penProcess, lefthull[upperLeft], righthull[upperRight]);
            pictureBox1.Invalidate();
            pictureBox1.Refresh();
            System.Threading.Thread.Sleep(1000);

            int lowera = upperLeft, lowerb = upperRight;
            //ret contains the convex hull after merging the two convex hulls 
            //with the points sorted in anti-clockwise order 
            int ind = uppera;
            ConvexHull.Add(lefthull[uppera]);
            while (ind != lowera)
            {
                ind = (ind + 1) % n1;
                ConvexHull.Add(lefthull[ind]);
            }

            ind = lowerb;
            ConvexHull.Add(righthull[lowerb]);
            while (ind != upperb)
            {
                ind = (ind + 1) % n2;
                ConvexHull.Add(righthull[ind]);
            }
            */

            return lefthull;
        }

        //алгоритм построения оболочки для малого количества точек
        //Попарно проверяет все возможные ребра(пары вершин),если все остальные точки находятся по одну сторону(в даннов случае слева)
        //мы добавляем эту пару точек в список,это будет список вершин.Удаляем повторяющиеся и сортируем против часовой стрелки
        List<Point> BruteHull(List<Point> points)
        {
            bool allPointsOnTheLeft;
            Point pti, ptj;
            List<Point> vertices = new List<Point>();
            for (int i = 0; i < points.Count; i++)
            {
                for (int j = 0; j < points.Count; j++)
                {
                    if (i == j)
                        continue;
                    pti = points[i];
                    ptj = points[j];
                    allPointsOnTheLeft = true;
                    for (int k = 0; k < points.Count; k++)
                    {
                        if (k == i || k == j)
                            continue;
                        int side = findPointPos(points[k], pti, ptj);
                        if (side < 0)
                        {
                            allPointsOnTheLeft = false;
                            break;
                        }
                    }
                    if (allPointsOnTheLeft)
                    {
                        vertices.Add(pti);
                        vertices.Add(ptj);
                    }
                }
            }
            vertices = vertices.Distinct().ToList();//убираем повторяющие точки в списке
            vertices = vertices.OrderBy(p => Math.Atan2(p.X, p.Y)).ToList();//упорядочиваем против часовой
            return vertices;
        }


        // Checks whether the line is crossing the polygon 
        int orientation(Point a, Point b, Point c)
        {
            int res = (b.Y - a.Y) * (c.X - b.X) - (c.Y - b.Y) * (b.X - a.X);
            if (res == 0)
                return 0;
            if (res > 0)
                return 1;
            return -1;
        }

        //Положение точки относительно прямой
        //0 - принадлежит линии, > 0 - левее, < 0 - правее
        int findPointPos(PointF p, Point A, Point B)
        {
            float result = (B.X - A.X) * (p.Y - B.Y) - (B.Y - A.Y) * (p.X - B.X);
            return (int)result;
        }

        //очищаем экран и обнуляем список
        private void clearBtn_Click(object sender, EventArgs e)
        {
            g.Clear(Color.White);
            points.Clear();
            pictureBox1.Invalidate();
        }

    }

}
