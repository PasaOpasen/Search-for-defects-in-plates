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

namespace Работа2019
{
    public partial class R2Area : Form
    {
        private double xmin, xmax, ymin, ymax,xb,yb;
        public NetOnDouble X, Y;

        private void button1_Click(object sender, EventArgs e)
        {
            OK = true;
            SoundMethods.OK();
            this.Close();
        }

        public bool OK = false;

        public R2Area(NetOnDouble xNet, NetOnDouble yNet)
        {
            InitializeComponent();
            X = xNet.dup;
            Y = yNet.dup;
            SetParams();
            SetEvents();

            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisX.Crossing = 0;
            chart1.ChartAreas[0].AxisY.Crossing = 0;
        }

        private void SetParams()
        {
            double xrange = X.Range / 2;
            double yrange = Y.Range / 2;
            chart1.ChartAreas[0].AxisX.Minimum = xmin = Math.Min(X.Begin, X.End) - xrange;
            chart1.ChartAreas[0].AxisX.Maximum = xmax = Math.Max(X.Begin, X.End) + xrange;
            chart1.ChartAreas[0].AxisY.Minimum = ymin = Math.Min(Y.Begin, Y.End) - yrange;
            chart1.ChartAreas[0].AxisY.Maximum = ymax = Math.Max(Y.Begin, Y.End) + yrange;
        
            numericUpDown1.Value =Math.Min( X.Count,numericUpDown1.Maximum);
            numericUpDown2.Value = Math.Min(Y.Count, numericUpDown2.Maximum);
            textBox3.Text = (2 * xrange).ToRString();
            textBox4.Text = (2 * yrange).ToRString();

            textBox1.Text = X.Center.ToRString();
            textBox2.Text = Y.Center.ToRString();

             xb = Math.Abs(xmin - xmax);
             yb = Math.Abs(ymin - ymax);

            trackBar1.Value = GetTrack(trackBar1, (X.Center-xmin)/xb);
            trackBar2.Value = GetTrack(trackBar2, (Y.Center-ymin)/yb);
            trackBar3.Value = GetTrack(trackBar3, 2 * xrange/xb);
            trackBar4.Value = GetTrack(trackBar4, 2 * yrange/yb);

            DrawNets();
        }

        private void DrawNets()
        {
            double x = textBox1.Text.ToDouble();
            double y = textBox2.Text.ToDouble();
            double xr = textBox3.Text.ToDouble()/2;
            double yr = textBox4.Text.ToDouble()/2;
            int xn = numericUpDown1.Value.ToInt32();
            int yn = numericUpDown2.Value.ToInt32();
            X = new NetOnDouble(x - xr, x + xr, xn);
            Y = new NetOnDouble(y - yr, y + yr, yn);

            foreach (var c in chart1.Series)
                c.Points.Clear();

            chart1.Series[0].Points.AddXY(X.Begin, Y.Begin);
            chart1.Series[0].Points.AddXY(X.Begin, Y.End);
            chart1.Series[0].Points.AddXY(X.End, Y.End);
            chart1.Series[0].Points.AddXY(X.End, Y.Begin);
            chart1.Series[0].Points.AddXY(X.Begin, Y.Begin);

            foreach(var xs in X.Array)
                foreach(var ys in Y.Array)
                    chart1.Series[1].Points.AddXY(xs, ys);

            Библиотека_графики.ForChart.SetToolTips(ref chart1);
        }

        private static int GetTrack(TrackBar t, double val) => GetTrack(t.Minimum, t.Maximum, t.Maximum - t.Minimum + 1, val);
        private static int GetTrack(int min, int max, int count, double val)
        {
            val = min + (max - min) * val;

            var arr = Expendator.Seq(min, max, count);
            return  Array.IndexOf(arr, Vectors.Create(arr).BinaryApproxSearch(val));
        }


        private void SetEvents()
        {
            string t1()=>(xmin + GetVal(trackBar1, xb)).ToRString();
            string t2() =>(ymin + GetVal(trackBar2, yb)).ToRString();
            string t3()=>GetVal(trackBar3, xb).ToRString();
            string t4() =>GetVal(trackBar4, yb).ToRString();

            trackBar1.ValueChanged += (o, e) => textBox1.Text = t1();
            trackBar2.ValueChanged += (o, e) => textBox2.Text = t2();
            trackBar3.ValueChanged += (o, e) => textBox3.Text = t3();
            trackBar4.ValueChanged += (o, e) => textBox4.Text = t4();

            textBox1.TextChanged += (o, e) => { if (textBox1.Text != t1()) textBox1.BackColor = Color.Tomato; };
            textBox2.TextChanged += (o, e) => { if (textBox2.Text != t2()) textBox2.BackColor = Color.Tomato; };
            textBox3.TextChanged += (o, e) => { if (textBox3.Text != t3()) textBox3.BackColor = Color.Tomato; };
            textBox4.TextChanged += (o, e) => { if (textBox4.Text != t4()) textBox4.BackColor = Color.Tomato; };

            trackBar1.ValueChanged += (o, e) => DrawNets();
            trackBar2.ValueChanged += (o, e) => DrawNets();
            trackBar3.ValueChanged += (o, e) => DrawNets();
            trackBar4.ValueChanged += (o, e) => DrawNets();
            textBox1.TextChanged += (o, e) => DrawNets();
            textBox2.TextChanged += (o, e) => DrawNets();
            textBox3.TextChanged += (o, e) => DrawNets();
            textBox4.TextChanged += (o, e) => DrawNets();
            numericUpDown1.ValueChanged += (o, e) => DrawNets();
            numericUpDown2.ValueChanged += (o, e) => DrawNets();

            trackBar1.ValueChanged += (o, e) =>  textBox1.BackColor = Color.White;
            trackBar2.ValueChanged += (o, e) => textBox2.BackColor = Color.White;
            trackBar3.ValueChanged += (o, e) => textBox3.BackColor = Color.White;
            trackBar4.ValueChanged += (o, e) => textBox4.BackColor = Color.White;

        }
        private static double GetVal(TrackBar t, double maxval) => t.Value * maxval / t.Maximum;
    }
}
