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
using  Point = МатКлассы.Point;
using static МатКлассы.Waves;
using Практика_с_фортрана;
using static РабКонсоль;
using static Functions;
using static Forms;

namespace Defect2019
{
    public partial class DC : Form
    {
        static DC()
        {

        }
        static readonly double corner=Math.PI/6.0;
        bool tosource;
        double argstep;

        private void SetTextBoxesAndOtherData(out DCircle es,DCircle dCircle)
        {
            es = new DCircle(dCircle);
            textBox4.Text = es.Center.x.ToString();
            textBox5.Text = es.Center.y.ToString();
            var t = es.DiamsAndArg;
            textBox1.Text = t.Item1.ToString();
            textBox2.Text = t.Item2.ToString();
            textBox3.Text = t.Item3.ToString();
            numericUpDown1.Value = es.FirstNomnalsCount;
            numericUpDown2.Value = es.SecondNomnalsCount;
        }
        public DC(bool toS=false)
        {
            InitializeComponent();          
            DCircle es;
            if (!toS)
            {
                if (Forms.UG.dCircle != null)
                    SetTextBoxesAndOtherData(out es, Forms.UG.dCircle);
                else
                    es = DCircle.Example.dup;

            }
            else              
                SetTextBoxesAndOtherData(out es, DCircle.Example);
               
            tosource = toS;
            SetTrack();
            Draw(es.GetArraysForDraw(Convert.ToInt32(numericUpDown1.Value), Convert.ToInt32(numericUpDown2.Value)), es, Convert.ToInt32(numericUpDown3.Value));
        }

        public DC(Tuple<Point[],Point[]> tuple, DCircle circle, int randomcount = 40)
        {
            InitializeComponent();
            SetTrack();
            Draw(tuple,circle, randomcount);
        }
        int counter = 0;
        private void SetTrack(int val = 17)
        {
            argstep = 2 * Math.PI / (trackBar1.Maximum+1);
            trackBar1.Value = val;
            textBox3.Text = (trackBar1.Value* argstep).ToString();

            var evnt=new EventHandler((object o, EventArgs e) => 
            {
                textBox3.Text = (trackBar1.Value* argstep).ToString();
                button1_Click(o, e);
            });
            trackBar1.ValueChanged += evnt;
            evnt(new object(), new EventArgs());
            
        }
        /// <summary>
        /// Нарисовать полумесяц, стрелки нормалей на нём и набор случайных нормалей
        /// </summary>
        /// <param name="tuple"></param>
        /// <param name="dcircle"></param>
        /// <param name="randomcount"></param>
        private void Draw(Tuple<Point[], Point[]> tuple, DCircle dcircle, int randomcount=100)
        {
            counter = 0;  
            for (int i = 1; i < chart1.Series.Count; i++)
                chart1.Series.RemoveAt(i--);

            Point[] p = tuple.Item1;
            Point[] n = tuple.Item2;
            chart1.Series[0].Points.Clear();
            for (int i = 0; i < p.Length; i++)
            {
                chart1.Series[0].Points.AddXY(p[i].x, p[i].y);
                MakeArrow(p[i], n[i], Color.Red, dcircle);
            }

            RandArrows(dcircle, randomcount);
        }

        private void MakeArrow(Point beg, Point Normal,Color col,DCircle circle)
        {
            chart1.Series.Add(counter++.ToString());
            chart1.Series.Last().IsVisibleInLegend = false;
            chart1.Series.Last().ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            chart1.Series.Last().BorderWidth = 2;
            chart1.Series.Last().Color = col;

            double r = Normal.Abs,s=r/4;
            double cor = Math.Acos( (Normal.x)/r)*Math.Sign(Normal.y);
            Point p1 = new Point(s * Math.Cos(-Math.PI - corner), s * Math.Sin(-Math.PI - corner)).Turn(Point.Zero, cor);
            Point p2 = new Point(s * Math.Cos(-Math.PI + corner) , s * Math.Sin(-Math.PI + corner) ).Turn(Point.Zero, cor);

            chart1.Series.Last().Points.AddXY(beg.x, beg.y);
            chart1.Series.Last().Points.AddXY(beg.x+ Normal.x, beg.y+Normal.y);
            chart1.Series.Last().Points.AddXY(beg.x + Normal.x + p1.x, beg.y + Normal.y + p1.y);
            chart1.Series.Last().Points.AddXY(beg.x + Normal.x, beg.y + Normal.y);
            chart1.Series.Last().Points.AddXY(beg.x + Normal.x + p2.x, beg.y + Normal.y + p2.y);
        }


        private double GetWindowsCoef()
        {
            double res= textBox6.Text.ToDouble();
            if(res<=1.9)
            {
                textBox6.Text = "2";
                return 2;
            }
            return res;
        }
        private void RandArrows( DCircle circle,int count = 100)
        {
            if (count == 0)
                return;

            chart1.Series.Add(counter++.ToString());
            chart1.Series.Last().IsVisibleInLegend = false;
            chart1.Series.Last().ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            chart1.Series.Last().MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Star5;
            chart1.Series.Last().MarkerSize = 8;
            chart1.Series.Last().BorderWidth = 8;
            chart1.Series.Last().Color = Color.Green;
            int n = chart1.Series.Count - 1;
            double cof = GetWindowsCoef();

            int c = 0;
            Random r = new Random();
            double x, y, q=circle.Radius*cof;
            Point f,p;
            while (c < count)
            {
                x =-q+ r.NextDouble()*q*2+circle.Center.x;
                y = -q +r.NextDouble() * q*2+circle.Center.y;
                p = new Point(x, y);

                if (!circle.ContainPoint(p,1.05))
                {
                    chart1.Series[n].Points.AddXY(x, y);
                    f = circle.GetNormal(p, 0.1*q);
                    MakeArrow(new Point(p.x - f.x, p.y - f.y), f, Color.Blue,circle);
                    c++;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Waves.DCircle c = new Waves.DCircle(new Point(textBox4.Text.ToDouble(), textBox5.Text.ToDouble()), textBox1.Text.ToDouble(), textBox2.Text.ToDouble(), textBox3.Text.ToDouble(),
                Convert.ToInt32(numericUpDown1.Value), Convert.ToInt32(numericUpDown2.Value));

            Draw(c.GetArraysForDraw(Convert.ToInt32(numericUpDown1.Value), Convert.ToInt32(numericUpDown2.Value)), c, Convert.ToInt32(numericUpDown3.Value));
            //c.Center.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Waves.DCircle c = new Waves.DCircle(new Point(textBox4.Text.ToDouble(), textBox5.Text.ToDouble()), textBox1.Text.ToDouble(), textBox2.Text.ToDouble(), textBox3.Text.ToDouble(),
            Convert.ToInt32(numericUpDown1.Value), Convert.ToInt32(numericUpDown2.Value));

            if (!tosource)
                Forms.UG.dCircle = new DCircle(c);
            else
            {
                double[] w = SeqWMemoized(wbeg, wend, wcount);
                var fw = w.Map((double d) => Functions.F1(d) + new Number.Complex(RandomNumbers.NextDouble2(0, 0.01), RandomNumbers.NextDouble2(0, 0.01)));

                Uxt.sources.Add(new Source(c.Center,
                    c.GetNormalsOnDCircle(),
                    (Point p) => c.ContainPoint(p),
                      fw,
                    Source.Type.DCircle,
                    c.BigCircle.radius));

                Uform.Recostract();
            }

            this.Close();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            button1_Click(sender, e);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            button1_Click(sender, e);
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            button1_Click(sender, e);
        }
    }
}
