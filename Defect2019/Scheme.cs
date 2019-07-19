using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using МатКлассы;
using Point = МатКлассы.Point;

namespace Работа2019
{
    public partial class Scheme : Form
    {
        Graphics g;
        Pen pen;
        Font font;
        double wind=1.3,X,Y;
        double dwx,dwy;
        Point center;
        int pcount = 100;
        Source[] mas;
        float rad;

        public Scheme()
        {
            InitializeComponent();
        }

        public Scheme(Source[] mass):this()
        {
            mas = mass;
            CreateEmptyImageAndSetParams();

            DrawFigures();
        }

        public Scheme(Source[] mass,Point beg,double lenx,double leny,string filename) : this()
        {
            mas = mass;
            CreateEmptyImageAndSetParams();

            var p = DoublePToIntP(beg);
            float cc = 15.0f / 11;
            g.DrawImage(Image.FromFile(filename), p.X, p.Y, (float)(lenx/X* pictureBox1.BackgroundImage.Size.Width/*+ pictureBox1.BackgroundImage.Size.Width/14*0.5*/)*cc, (float)(leny/Y*pictureBox1.BackgroundImage.Size.Height/*- pictureBox1.BackgroundImage.Size.Height/14*0.5)*cc*/));

            DrawFigures();
        }

        void CreateEmptyImageAndSetParams()
        {
            
            var pmas = new Point[mas.Length];
            for (int i = 0; i < mas.Length; i++)
            {
                pmas[i] = mas[i].Center;
                //pmas[i].Show();
            }


            var tp = МатКлассы.Point.GetBigRect(pmas);
            //center = Point.Center(pmas);
            center = new Point((tp.Item1.x + tp.Item2.x) / 2, (tp.Item1.y + tp.Item2.y) / 2);

            double diam = mas[0].radius * 2;
            rad = (float)mas[0].radius;

            X = tp.Item2.x - tp.Item1.x + diam;
            Y = tp.Item2.y - tp.Item1.y + diam;

            X *= wind;
            Y *= wind;

            //pictureBox1.Width = (int)X;
            //pictureBox1.Height = (int)Y;
            //pictureBox1.Location = new System.Drawing.Point((int)(center.x-X/2), (int)(center.y-Y/2));

            //g = pictureBox1.CreateGraphics();
            double xy = X / Y;
            int c = 1000;
            pictureBox1.BackgroundImage = new Bitmap(c, (int)(xy * c));

            g = Graphics.FromImage(pictureBox1.BackgroundImage);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            pen = new Pen(Brushes.Red, 5);
            font = new Font("Arial", 14);
        }

        void DrawFigures()
        {
            for (int i = 0; i < mas.Length; i++)
            {
                DrawS(mas[i]);
                var pp = DoublePToIntP(mas[i].Center);
                g.DrawString(mas[i].Center.ToString(), font, Brushes.Blue, pp);
                var del = 4;
                g.DrawLine(new Pen(Brushes.Black, 4), new PointF(pp.X-rad/del,pp.Y-rad/del), new PointF(pp.X + rad / del, pp.Y + rad / del));
                g.DrawLine(new Pen(Brushes.Black, 4), new PointF(pp.X - rad / del, pp.Y + rad / del), new PointF(pp.X + rad / del, pp.Y - rad / del));
            }
                
            pictureBox1.Invalidate();
        }


        void DrawS(Source s)
        {
            //var mas = s.MasForDraw();
            //for(int i = 0; i < mas.Length-1; i++)
            //{
            //g.DrawLine(pen, DoublePToIntP(mas[i]), DoublePToIntP(mas[i+1]));

            //}
            //g.DrawLine(pen, DoublePToIntP(mas[mas.Length - 2]), DoublePToIntP(mas[mas.Length - 1]));
            g.DrawCurve(pen, SourceToFpoint(s));
        }

        PointF DoublePToIntP(Point p)
        {
            //return new PointF((float)p.x, (float)p.y);
            return new PointF((float)((p.x - center.x) / X * pictureBox1.BackgroundImage.Size.Width+ pictureBox1.BackgroundImage.Size.Width*0.5f), (float)((p.y - center.y) / Y * pictureBox1.BackgroundImage.Size.Height+ pictureBox1.BackgroundImage.Size.Height*0.5));
        }

        PointF[] SourceToFpoint(Source ss)
        {
            var s = ss.MasForDraw();
            PointF[] mas = new PointF[s.Length+1];
            for (int i = 0; i < s.Length; i++)
                mas[i] = DoublePToIntP(s[i]);
            mas[s.Length] = mas[0];
            return mas;
        }

    }
}
