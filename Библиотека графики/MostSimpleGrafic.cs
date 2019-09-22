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

namespace Библиотека_графики
{
    public partial class MostSimpleGrafic : Form
    {
        public MostSimpleGrafic(Func<double, double> f, double a, double b, int count)
        {
            InitializeComponent();
            chart1.Series[0].IsVisibleInLegend = false;
            var points = МатКлассы.Point.Points(new RealFunc(t => f(t)), count - 1, a, b,true);
            for (int i = 0; i < points.Length; i++)
                chart1.Series[0].Points.AddXY(points[i].x, points[i].y);
            Библиотека_графики.ForChart.SetToolTips(ref chart1);
        }
        public MostSimpleGrafic(Func<double, double>[] f, double a, double b, int count, string[] names,bool parallel=true)
        {
            InitializeComponent();
            chart1.Series.Clear();
            for (int k = 0; k < f.Length; k++)
            {
                chart1.Series.Add(names[k]);
                chart1.Series[k].BorderWidth = 3;
                chart1.Series[k].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;

                var points = МатКлассы.Point.Points(new RealFunc(t => f[k](t)), count - 1, a, b,parallel);
                
                for (int i = 0; i < points.Length; i++)
                    chart1.Series[k].Points.AddXY(points[i].x, points[i].y);
            }

            Библиотека_графики.ForChart.SetToolTips(ref chart1);
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ForChart.SaveImageFromChart(chart1);
        }
    }
}
