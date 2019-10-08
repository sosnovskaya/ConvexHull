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
        private SolidBrush brushBlack = new SolidBrush(Color.Black);
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
            points = points.OrderBy(p => p.X).ToList();//сортируем по X,чтобы левая оболочка была слева,а првая справа
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

            // finding the upper tangent 
            int inda = leftmostRight, indb = rihtmostLeft;
            bool done = false;
            while (!done)
            {
                done = true;
                while (orientation(righthull[indb], lefthull[inda], lefthull[(inda + 1) % n1]) >= 0)
                    inda = (inda + 1) % n1;

                while (orientation(lefthull[inda], righthull[indb], righthull[(n2 + indb - 1) % n2]) <= 0)
                {
                    indb = (n2 + indb - 1) % n2;
                    done = false;
                }
            }

            int uppera = inda, upperb = indb;
            g.DrawLine(penProcess, lefthull[uppera], righthull[upperb]);
            pictureBox1.Invalidate();
            pictureBox1.Refresh();
            System.Threading.Thread.Sleep(1000);

            inda = leftmostRight; indb = rihtmostLeft;
            done = false;
            while (!done)//finding the lower tangent 
            {
                done = true;
                while (orientation(lefthull[inda], righthull[indb], righthull[(indb + 1) % n2]) >= 0)
                    indb = (indb + 1) % n2;

                while (orientation(righthull[indb], lefthull[inda], lefthull[(n1 + inda - 1) % n1]) <= 0)
                {
                    inda = (n1 + inda - 1) % n1;
                    done = false;
                }
            }
            int lowera = inda, lowerb = indb;
            g.DrawLine(penProcess, lefthull[lowera], righthull[lowerb]);
            pictureBox1.Invalidate();
            pictureBox1.Refresh();
            System.Threading.Thread.Sleep(1000);

            int ind = uppera;
            ConvexHull.Add(lefthull[uppera]);
            while (ind != lowera)
            {
                ind = (n1 + ind - 1) % n1;
                ConvexHull.Add(lefthull[ind]);
            }
            ind = lowerb;
            ConvexHull.Add(righthull[lowerb]);
            while (ind != upperb)
            {
                ind = (n2 + ind - 1) % n2;
                ConvexHull.Add(righthull[ind]);
            }
            ConvexHull = SortAnticlockwise(ConvexHull);

            return ConvexHull;
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
            vertices = SortAnticlockwise(vertices);//сортирует точки против часовой стрелки
           // vertices = vertices.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();
            //Point[] array = vertices.ToArray();
            //Array.Sort(array, new ClockwiseComparer());
            //vertices = array.ToList();
            //double[] angles = new double[array.Length];
            //for (int i = 0; i < array.Length; i++)
                //angles[i] = Math.Atan2(array[i].Y, array[i].X);
            //Array.Sort(angles, array);
            //vertices = vertices.OrderBy(p => Math.Atan2(p.X, p.Y)).ToList();//упорядочиваем по часовой
            return vertices;
        }

        List<Point> SortAnticlockwise(List<Point> points)
        {
            Point center = new Point(0,0);
            int n = points.Count;
            for(int i = 0;i < n; i++)
            {
                center.X += points[i].X;
                center.Y += points[i].Y;
            }
            center.X /= n;
            center.Y /= n;
            for(int i = 0;i < n; i++)
            {
                for(int j = 0; j < n; j++)
                {
                    if (j == i)
                        continue;
                    if (Less(points[i],points[j],center))
                    {
                        var temp = points[i];
                        points[i] = points[j];
                        points[j] = temp;
                    }
                }
            }
            return points;
        }

        bool Less(Point a,Point b,Point center)
        {
            if (a.X - center.X >= 0 && b.X - center.X < 0)
                return true;
            if (a.X - center.X < 0 && b.X - center.X >= 0)
                return false;
            if (a.X - center.X == 0 && b.X - center.X == 0)
            {
                if (a.Y - center.Y >= 0 || b.Y - center.Y >= 0)
                    return a.Y > b.Y;
                return b.Y > a.Y;
            }

            // compute the cross product of vectors (center -> a) x (center -> b)
            int det = (a.X - center.X) * (b.Y - center.Y) - (b.X - center.X) * (a.Y - center.Y);
            if (det < 0)
                return true;
            if (det > 0)
                return false;

            // points a and b are on the same line from the center
            // check which point is closer to the center
            int d1 = (a.X - center.X) * (a.X - center.X) + (a.Y - center.Y) * (a.Y - center.Y);
            int d2 = (b.X - center.X) * (b.X - center.X) + (b.Y - center.Y) * (b.Y - center.Y);
            return d1 > d2;
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
    /*
    public class ClockwiseComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            var point1 = (Point)x;
            var point2 = (Point)y;

            return Math.Atan2(point1.Y, point1.X).CompareTo(Math.Atan2(point2.Y, point2.X));
        }
    }
    */
}
