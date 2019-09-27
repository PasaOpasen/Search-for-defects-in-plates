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
        private double xmin, xmax, ymin, ymax;
        private NetOnDouble X, Y;

        private void button1_Click(object sender, EventArgs e)
        {
            OK = true;
            this.Close();
        }

        private bool OK = false;

        public R2Area(NetOnDouble xNet, NetOnDouble yNet)
        {
            InitializeComponent();
            X = xNet.dup;
            Y = yNet.dup;
            SetParams();
        }

        private void SetParams()
        {
            double xrange = X.Range / 2;
            double yrange = Y.Range / 2;
            chart1.ChartAreas[0].AxisX.Minimum = xmin = Math.Min(X.Begin, X.End) - xrange;
            chart1.ChartAreas[0].AxisX.Maximum = xmax = Math.Max(X.Begin, X.End) + xrange;
            chart1.ChartAreas[0].AxisY.Minimum = ymin = Math.Min(Y.Begin, Y.End) - yrange;
            chart1.ChartAreas[0].AxisY.Maximum = ymax = Math.Max(Y.Begin, Y.End) + yrange;

            DrawNets();

            numericUpDown1.Value = X.Count;
            numericUpDown2.Value = Y.Count;
            textBox3.Text = (2 * xrange).ToRString();
            textBox4.Text = (2 * yrange).ToRString();

            textBox1.Text = X.Center.ToRString();
            textBox2.Text = Y.Center.ToRString();

        }

        private void DrawNets()
        {
            foreach (var c in chart1.Series)
                c.Points.Clear();

            chart1.Series[0].Points.AddXY(X.Begin, Y.Begin);
            chart1.Series[0].Points.AddXY(X.Begin, Y.End);
            chart1.Series[0].Points.AddXY(X.End, Y.End);
            chart1.Series[0].Points.AddXY(X.End, Y.Begin);
            chart1.Series[0].Points.AddXY(X.Begin, Y.Begin);

            foreach(var x in X.Array)
                foreach(var y in Y.Array)
                    chart1.Series[1].Points.AddXY(x, y);

            Библиотека_графики.ForChart.SetToolTips(ref chart1);
        }

    }
}
