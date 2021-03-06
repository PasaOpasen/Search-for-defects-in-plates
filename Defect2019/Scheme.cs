﻿using System;
using System.Drawing;
using System.Windows.Forms;
using Point = МатКлассы.Point;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using МатКлассы;
using System.Threading.Tasks;
using System.Threading;
using JR.Utils.GUI.Forms;
using System.Diagnostics;

namespace Работа2019
{
    public partial class Scheme : Form
    {
        private Graphics g;
        private Pen pen;
        private Font font;
        private double WindowCoef = 1.1;

        /// <summary>
        /// Ширина окна
        /// </summary>
        private double X;
        /// <summary>
        /// Высота окна
        /// </summary>
        private double Y;
        private Point center;
        private Source[] mas;
        private float rad;
        private EllipseParam[] ellipses;
        private string[] ellipseArray;
        private System.Media.SoundPlayer SoundPlayer = new System.Media.SoundPlayer(Expendator.GetResource("gamestartup.wav", "Defect2019"));

        public Scheme(string title = "Схема эксперимента")
        {
            InitializeComponent();
            saveFileDialog1.Filter = "Image files(*.png)|*.png|All files(*.*)|*.*";
            this.Text = title;

            decimal shift = (decimal)РабКонсоль.timeshift;
            numericUpDown1.Minimum = shift * 0.75m;
            numericUpDown1.Maximum = shift * 1.5m;
            numericUpDown1.Value = shift;
            numericUpDown1.Increment = shift / 70;
            numericUpDown1.DecimalPlaces = 15;


           // SoundMethods.CS();
            groupBox6.Hide();

            SoundPlayer.Play();
            this.FormClosing += (o, e) =>
            {
                SoundPlayer.Stop();
            };
        }

        public Scheme(Source[] mass, string title = "Схема эксперимента") : this(title)
        {
            mas = mass;
            CreateEmptyImageAndSetParams();
            DrawFigures();
        }

        public Scheme(Source[] mass, EllipseParam[] param, string title = "Схема эксперимента") : this(mass, title)
        {
            JustDrawEllipses(param);
        }

        public Scheme(string[] array, string title = "Схема эксперимента") : this(GetSources(array), title)
        {
            ellipseArray = array;
            JustDrawEllipses(array);

            void ProoveEllipses()
            {
                var arr = ellipses.Select(el => el.right).ToArray();
                if (arr.Contains(false))               
                      numericUpDown1.BackColor = Color.Red;                                  
                else
                    numericUpDown1.BackColor = Color.White;
                label2.Text = $"Сломалось: {arr.Count(i=>!i)} из {arr.Length}";
            }

            numericUpDown1.ValueChanged += (o, e) =>
            {
                try
                {
                    Convert.ToDouble(numericUpDown1.Value);
                }
                catch
                {
                    numericUpDown1.BackColor = Color.Yellow;
                    return;
                }

                g.Clear(Color.White);
                DrawFigures();

                ellipses = GetEllipses(array);
                DrawEllipses(ellipses);
                ProoveEllipses();
            };

            ProoveEllipses();
        }
        private void JustDrawEllipses(string[] array) => JustDrawEllipses(GetEllipses(array));
        private void JustDrawEllipses(EllipseParam[] array)
        {
            ellipses = array;
            groupBox6.Show();          
            DrawEllipses(ellipses);
        }

        private static Source[] GetSources(string[] array)
        {
            List<Point> plist = new List<Point>(array.Length * 2);
            string[] st;
            for (int i = 0; i < array.Length; i++)
            {
                st = array[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                plist.Add(new Point(st[0].ToDouble(), st[1].ToDouble()));
                plist.Add(new Point(st[2].ToDouble(), st[3].ToDouble()));
            }
            var centers = plist.Distinct().Select(p => new Waves.Circle(p, 8));
            return centers.Select(p => new Source(p, p.GetNormalsOnCircle(30), Array.Empty<Number.Complex>())).ToArray();
        }
        public EllipseParam[] GetEllipses(string[] array) => GetEllipses(array, (double)numericUpDown1.Value);
        public EllipseParam[] GetEllipses(string[] array,double ts)
        {
            EllipseParam[] param = new EllipseParam[array.Length];

            double sd = textBox6.Text.ToDouble();

            Parallel.For(0, array.Length, (int i) =>
            {
                string[] st = array[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                var Vgb = (st[4].ToDouble(), st[5].ToDouble());
                double s = Vgb.Item1 * (Vgb.Item2 - ts /*(2.5*st[10].ToDouble()+5e-5)*/);
                param[i] = new EllipseParam(new Point(st[0].ToDouble(), st[1].ToDouble()),
                    new Point(st[2].ToDouble(), st[3].ToDouble()), s,
                    Библиотека_графики.Other.colors[st[6].ToInt32()], $"{st[7]} {st[8]} {st[9]}", FuncMethods.GaussBell2(s, sd * s));
            });
            return param;
        }

        public void DrawEllipses(EllipseParam[] param)
        {
            foreach (var p in param)
            {
                this.Add(p);
            }
            
            DrawFigures();//так сделано, чтобы эллипсы не накрывали источники и информацию о них
        }

        public Scheme(Source[] mass, Point beg, double lenx, double leny, string filename) : this(mass)
        {
            DrawImage(beg, lenx, leny, filename);
        }
        private void DrawImage(Point beg, double lenx, double leny, string filename)
        {
            var p = MyPointToPointF(beg);
            const float cc = 13.0f / 11;
            g.DrawImage(Image.FromFile(filename), p.X, p.Y, (float)(lenx / X * pictureBox1.BackgroundImage.Size.Width/*+ pictureBox1.BackgroundImage.Size.Width/14*0.5*/) * cc, (float)(leny / Y * pictureBox1.BackgroundImage.Size.Height/*- pictureBox1.BackgroundImage.Size.Height/14*0.5)*cc*/));
        }

        public void Add(EllipseParam p)
        {
            var pens = new Pen(p.Color, 2);

            g.DrawCurve(pens, EllipseToFpoint(p));
        }

        /// <summary>
        /// Инициализирует некоторые параметры и создаёт окно нужного размера
        /// </summary>
        private void CreateEmptyImageAndSetParams()
        {
            var pmas = new Point[mas.Length];
            for (int i = 0; i < mas.Length; i++)
            {
                pmas[i] = mas[i].Center;
            }

            var tp = МатКлассы.Point.GetBigRect(pmas);
            center = new Point((tp.Item1.x + tp.Item2.x) / 2, (tp.Item1.y + tp.Item2.y) / 2);

            rad = (float)Source.GetMaxRadius(mas);
            double diam = rad * 2;

            X = tp.Item2.x - tp.Item1.x + diam;
            Y = tp.Item2.y - tp.Item1.y + diam;
            double xy = X / Y;
            X *= WindowCoef;
            Y *= WindowCoef;

            const int c = 1000;
            pictureBox1.BackgroundImage = new Bitmap((int)(xy * c), c);
            this.Size = new Size((int)(this.Size.Height * xy), this.Size.Height);

            g = Graphics.FromImage(pictureBox1.BackgroundImage);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            pen = new Pen(Brushes.Red, 5);
            font = new Font("Arial", 18);
        }

        private void DrawFigures()
        {
            for (int i = 0; i < mas.Length; i++)
            {
                DrawSource(mas[i]);
                var pp = MyPointToPointF(mas[i].Center);
                g.DrawString(mas[i].Center.ToString(), font, Brushes.Blue, pp);
                var step = rad / 4.0f;
                g.DrawLine(new Pen(Brushes.Black, 4), new PointF(pp.X - step, pp.Y - step), new PointF(pp.X + step, pp.Y + step));
                g.DrawLine(new Pen(Brushes.Black, 4), new PointF(pp.X - step, pp.Y + step), new PointF(pp.X + step, pp.Y - step));
            }

            pictureBox1.Invalidate();
        }

        /// <summary>
        /// Рисует источник
        /// </summary>
        /// <param name="s"></param>
        private void DrawSource(Source s)
        {
            //var mas = s.NormsPositionArray();
            //for(int i = 0; i < mas.Length-1; i++)
            //{
            //g.DrawLine(pen, MyPointToPointF(mas[i]), MyPointToPointF(mas[i+1]));

            //}
            //g.DrawLine(pen, MyPointToPointF(mas[mas.Length - 2]), MyPointToPointF(mas[mas.Length - 1]));
            g.DrawCurve(pen, SourceToFpoint(s));
        }

        private PointF MyPointToPointF(Point p)
        {
            //return new PointF((float)p.x, (float)p.y);
            PointF ps = new PointF((float)((p.x - center.x) / X * pictureBox1.BackgroundImage.Size.Width + pictureBox1.BackgroundImage.Size.Width * 0.5f), (float)((p.y - center.y) / Y * pictureBox1.BackgroundImage.Size.Height + pictureBox1.BackgroundImage.Size.Height * 0.5));

            //ps = new PointF(ps.X, ((float)Y - ps.Y)+pictureBox1.BackgroundImage.Size.Height*0.75f);
            return ps;
        }

        private void сохранитьToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            pictureBox1.BackgroundImage.Save(saveFileDialog1.FileName);
        }

        /// <summary>
        /// Преобразовать источник в массив точек (для рисования)
        /// </summary>
        /// <param name="ss"></param>
        /// <returns></returns>
        private PointF[] SourceToFpoint(Source ss)
        {
            var s = ss.NormsPositionArray;
            return PointArrayToPointF(s);
        }

        /// <summary>
        /// Преобразовать EllipseParam в массив точек для рисования
        /// </summary>
        /// <param name="ss"></param>
        /// <returns></returns>
        private PointF[] EllipseToFpoint(EllipseParam el)
        {
            var s = el.GetPointArray(45);

            return PointArrayToPointF(s);
        }

        private PointF[] PointArrayToPointF(Point[] s)
        {
            PointF[] mas = new PointF[s.Length + 1];
            for (int i = 0; i < s.Length; i++)
            {
                mas[i] = MyPointToPointF(s[i]);
            }

            mas[s.Length] = mas[0];
            return mas;
        }

        private void groupBox6_Enter(object sender, System.EventArgs e)
        {

        }
        private async Task MakeEllipsesAsync(EllipseParam[] param)
        {
            var tmp = GetTwoNets();

            await EllipseParam.GetSurfaces(param.ToArray(), tmp.Item1, tmp.Item2, "EllipseSurface");
        }
        private Tuple<NetOnDouble,NetOnDouble> GetTwoNets()
        {
            NetOnDouble XX = new NetOnDouble(textBox7.Text.ToDouble(), textBox8.Text.ToDouble(), numericUpDown7.Value.ToInt32());
            NetOnDouble YY = new NetOnDouble(textBox9.Text.ToDouble(), textBox10.Text.ToDouble(), numericUpDown7.Value.ToInt32());
            return new Tuple<NetOnDouble, NetOnDouble>(XX, YY);
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Text = "Wait...";

            double sd = textBox6.Text.ToDouble();
            for (int i = 0; i < ellipses.Length; i++)
                ellipses[i] = new EllipseParam(ellipses[i].focSensor,
                    ellipses[i].focSource, ellipses[i].a * 2,
                    ellipses[i].Color, ellipses[i].name, FuncMethods.GaussBell2(2 * ellipses[i].a, sd * 2 * ellipses[i].a));
            await MakeEllipsesAsync(ellipses);
            new Библиотека_графики.PdfOpen("Поверхность для эллипсов", "EllipseSurface.pdf").Show();

            button1.Text = "Run";
        }

        private void подробнееToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var arr = ellipses.Where(el => !el.right).Select(s=>s.ToString()).ToArray();
            var ss = Expendator.StringArrayToString(arr);
            FlexibleMessageBox.Show(ss, "Информация о неудавшихся эллипсах");
        }


        private double GetOptimumShift(string[] array)
        {
            var tmp = GetTwoNets();
            var xmas = tmp.Item1.Array;
            var ymas = tmp.Item2.Array;
            int count = xmas.Length;
            double[,] net = new double[count, count];
            const double coef = -10;

            Func<double, double> func = (double shift) =>
               {
                   var p = GetEllipses(array, shift);
                   if (p.Count(ps => !ps.right) > 18)//условие, чтобы много элиипсов всё же не сломались
                       return Double.MaxValue;

                   Func<double, double, double> func2 = EllipseParam.GetGaussFunc(p);
                   //Parallel.For(0, count, (int i) => {
                   for (int i = 0; i < count; i++)
                       for (int j = 0; j < count; j++)
                           net[i, j] = func2(xmas[i], ymas[j]);
                   //});
                   return Math.Exp(coef* net.Max());
               };

            var tmpp = BeeHiveAlgorithm.GetGlobalMin(func,
                min: (double)numericUpDown1.Minimum,
                max: (double)numericUpDown1.Maximum,
                eps: 1e-10,
                countpoints: 120,
                maxcountstep: 50,
                maxiter: 120);
            Debug.WriteLine($"Максимум функции на сетке: {Math.Log(tmpp.Item2) / coef}");

            return tmpp.Item1;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            button2.Text = "Ищется...";
            double t = await Task.Run(() => GetOptimumShift(ellipseArray));
            numericUpDown1.Value = (decimal)t;
            button2.Text = "Поискать сдвиг";
            this.Refresh();
        }

        private void установитьТочныеГраницыПрямоугольникаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var r = GetSourceRect();
            textBox7.Text = r.Item1.x.ToString();
            textBox8.Text = r.Item2.x.ToString();
            textBox9.Text = r.Item1.y.ToString();
            textBox10.Text = r.Item2.y.ToString();
        }
        private Tuple<Point, Point> GetSourceRect() => Point.GetBigRect(mas.Select(s => s.Center).ToArray());

        private void установитьГраницыПрямоугольника10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var r = GetSourceRect();
            var center = (r.Item1 + r.Item2) * 0.5;
            double lx = (r.Item1.x - r.Item2.x).Abs() / 2 * 0.9;
            double ly = (r.Item1.y - r.Item2.y).Abs() / 2 * 0.9;


            textBox7.Text = (center.x - lx).ToString();
            textBox8.Text = (center.x + lx).ToString();
            textBox9.Text = (center.y - ly).ToString();
            textBox10.Text = (center.y + ly).ToString();
        }
    }
}