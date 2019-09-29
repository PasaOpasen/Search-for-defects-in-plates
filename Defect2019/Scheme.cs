using System.Drawing;
using System.Windows.Forms;
using Point = МатКлассы.Point;

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

        public Scheme(string title="Схема эксперимента")
        {
            InitializeComponent();
            saveFileDialog1.Filter = "Image files(*.png)|*.png|All files(*.*)|*.*";
            this.Text = title;
           // SoundMethods.SetPositions();
        }

        public Scheme(Source[] mass, string title = "Схема эксперимента") : this(title)
        {
            mas = mass;
            CreateEmptyImageAndSetParams();

            DrawFigures();
        }

        public Scheme(Source[] mass,EllipseParam[] param, string title = "Схема эксперимента") : this(mass,title)
        {
            foreach(var p in param)
            {
                this.Add(p);
            }
            DrawFigures();
        }

        public Scheme(Source[] mass, Point beg, double lenx, double leny, string filename) : this(mass)
        {
            DrawImage(beg, lenx, leny, filename);
        }
        private void DrawImage(Point beg, double lenx, double leny, string filename)
        {
          var p = MyPointToPointF(beg);
            float cc = 15.0f / 11;
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
            
            int c = 1000;
            pictureBox1.BackgroundImage = new Bitmap((int)(xy * c),c);
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
                var step = rad/4.0f;
                g.DrawLine(new Pen(Brushes.Black, 4), new PointF(pp.X -  step, pp.Y -  step), new PointF(pp.X +  step, pp.Y +  step));
                g.DrawLine(new Pen(Brushes.Black, 4), new PointF(pp.X - step, pp.Y +  step), new PointF(pp.X +  step, pp.Y -  step));
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
          return  PointArrayToPointF(s);
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
    }
}