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
            var points = МатКлассы.Point.PointsParallel(new RealFunc(t => f(t)), count - 1, a, b);
            for (int i = 0; i < points.Length; i++)
                chart1.Series[0].Points.AddXY(points[i].x, points[i].y);
            Библиотека_графики.ForChart.SetToolTips(ref chart1);
        }
        public MostSimpleGrafic(Func<double, double>[] f, double a, double b, int count, string[] names)
        {
            InitializeComponent();
            for (int k = 0; k < f.Length; k++)
            {
                var points = МатКлассы.Point.PointsParallel(new RealFunc(t => f[k](t)), count - 1, a, b);
                for (int i = 0; i < points.Length; i++)
                    chart1.Series[k].Points.AddXY(points[i].x, points[i].y);
                chart1.Series[k].Name = names[k];
            }

            Библиотека_графики.ForChart.SetToolTips(ref chart1);
        }
    }
}
