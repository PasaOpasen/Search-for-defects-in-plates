using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Библиотека_графики;
using МатКлассы;
using static МатКлассы.Number;
using nzy3d_winformsDemo;


namespace Консоль
{
    public partial class Форма_консоли : Form
    {
        public Форма_консоли()
        {
            InitializeComponent();
            List<string> s = new List<string>();
            for (double t = 4; t >1; t -= 0.5)
                s.Add($"a = {t}");

            string[] names = s.ToArray();
            chart1.Series.Clear();
            Библиотека_графики.ForChart.CreatSeries(ref chart1, names, 3, System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point, System.Windows.Forms.DataVisualization.Charting.MarkerStyle.None,null);
            for (int j = 0; j < names.Length; j++)
            {
               a = 4 - j * 0.5;
                for (double i = 0; i < (2+Math.PI)/*4 * a*/; i += 0.06)
                    chart1.Series[j].Points.AddXY(u(i), v(i));
            }
            //chart1.Series.RemoveAt(1); chart1.Series.RemoveAt(1);
            //for (int i=0;i<7; i++)
            //{
            //    chart1.Series.Add($"Вейвлет {i + 1}");
            //    chart1.Series.Last().BorderWidth = 3;
            //    chart1.Series.Last().ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            //}
            //chart1.Series[4].Color = Color.Red;
        }

        private double beg = -3, end = 3, step = 0.005;

        private void button1_Click(object sender, EventArgs e)
        {
            ForChart.SaveImageFromChart(chart1);
            chart1.SaveImage("rect.emf", System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Emf);
        }

        double x0 = 2, y0 = 2,a=Math.PI;


        private async void button2_Click(object sender, EventArgs e)
        {
            

            DateTime time = DateTime.Now;
            //Func<double,double> f = t => Math.Sin(t*9) / Math.Exp(t * t / 3) * Math.Sign(Math.Max(0, (2*Math.PI- t.Abs())));
            Func<double,double> f = t => Math.Cos(t * 9)*t/ Math.Exp(t * t / 3) * Math.Sign(Math.Max(0, ( Math.PI/2 - t.Abs())));
            //Func<double,double> f = t => Math.Sin(20*Math.PI*t)* Math.Sign(Math.Max(0, (1 - t.Abs())));
            Func<double,double,Complex>[] f1 = new Func<double,double,Complex>[7];
            Func<double,double>[] f2 = new Func<double,double>[7];
            Wavelet[] w = new Wavelet[7];


            //await Task.Run(() =>
            //{
            //    for (int i = 0; i < 7; i++)
            //    {
            //        w[i] = new Wavelet((Wavelet.Wavelets)i);
            //        f1[i] = w[i].GetAnalys(f);
            //        f2[i] = w[i].GetSyntesis(f1[i]);
            //    }
            //}
            //);

            w[3] = new Wavelet((Wavelet.Wavelets)3);
            f1[3] = w[3].GetAnalys(f);
            f2[3] = w[3].GetSyntesis(/*f1[3]*/);

            //Wavelet w = new Wavelet();
            //var f1 = w.GetAnalys(f);
            //var f2 = w.GetSyntesis(f1);

            //ИнтеграцияСДругимиПрограммами.CreatTableInExcel((double a, double b) => f1[3](a, b).Re, 0.01, 5, 100, 0.01, 7, 100);
            
            var p = OptimizationDCompFunc.GetMaxOnRectangle((double a, double b) => f1[3](a, b).Abs, 0.01, 5, 0.01, 10,eps:1e-14,ogr:5,nodescount:15);

            new nzy3d_winformsDemo.Form1("", 0.01, 5, 70, 0.01, 10, 70, (double a, double b) => f1[3](a, b).Abs).Show();
           // ИнтеграцияСДругимиПрограммами.CreatTableInExcel(f1[3], 0.01, 3, 100, 0.01, 3, 100);
           
            //ИнтеграцияСДругимиПрограммами.CreatTableInExcel(w[3].Dic,"Test Page",Number.Complex.ComplMode.Abs);
            //c1Chart3D1.ChartGroups.Group0.ChartType = Chart3DTypeEnum.Surface;

            //double[,] z = new double[100, 100];
            //for (int s = 0; s < 31; s++)
            //    for (int n = 0; n < 21; n++)
            //    {
            //        z[s, n] = f1[3](0.01 + 0.01 * s, 0.01 + 0.01 * n).Re;
            //    }
            // create dataset and put it to the chart
            //Chart3DDataSetGrid gridset = new Chart3DDataSetGrid(0, 0, 1, 1, z);
            //c1Chart3D1.ChartGroups[0].ChartData.Set = gridset;

            //var o = w[3].Dic;
            //МатКлассы.Point[] u = o.Keys.ToArray();
            //var c = o.Values.ToArray();
            //Chart3DPoint[] points1 = new Chart3DPoint[o.Count];
            //for (int i = 0; i < o.Count; i++)
            //    points1[i] = new Chart3DPoint(u[i].x, u[i].y, 7/*(double)c[i].Value*/);
            //Chart3DDataSetPoint pointset = new Chart3DDataSetPoint();
            //pointset.AddSeries(points1);
            //c1Chart3D1.ChartGroups[0].ChartData.Set = pointset;

            List<double> tm = new List<double>(), m = new List<double>();
            for (double i = beg; i < end; i += step)
            {
                double tmp0 = f(i);//,tmp2=f2(i);$"f = {tmp0}  \tWf-1 = {tmp2} \teps = {(tmp2-tmp0)} \t f/f0 = {tmp0/tmp2}".Show();
                double[] mas = new double[7];

                await Task.Run(()=> 
                {
                    //for (int j = 0; j < 7; j++)
                    //    mas[j] = f2[j](i);
                    mas[3] = f2[3](i);
                });

                
                (new Vectors(mas)-tmp0).Show();

                chart1.Series[0].Points.AddXY(i, tmp0);
                for (int j = 0; j < 7; j++)
                    chart1.Series[j + 1].Points.AddXY(i, mas[j]);

                progressBar1.Value = (int)((i -beg) / (end-beg)*100);

                m.Add(mas[3]);
                tm.Add(tmp0);
            }
            label1.Text ="Время работы (минуты полторы ушло на Excel): "+ (DateTime.Now - time).ToString();
            label2.Text = "Точность в евклидовой норме: "+(new Vectors(m.ToArray()) - new Vectors(tm.ToArray())).Sort.EuqlidNorm.ToString();
            var s = new МатКлассы.Point(p.Item1, p.Item2);
            label3.Text = "Точка максимума: " + s.ToString();
            label4.Text="Максимальное по модулю значение: "+f1[3](p.Item1, p.Item2).ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }


        private double r = 1.0 / 3.0, h = 0.25,R=4;

        double u(double t)
        {
            //if (t < a) return x0 - a / 2 + t;
            //if (t < 2 * a) return x0 + a / 2;
            //if (t < 3 * a) return x0 + 2.5 * a - t;
            //return x0 - a / 2;
            //return a*((R-r)*Math.Cos(r/R*t) +h*Math.Cos(t-r/R*t));
            //if (t < Math.PI) return a * Math.Cos(t); return -a + (t - Math.PI) * 2 * a / Math.PI;

            //if (t <= 2 * a) return a - t; return -a + (t - 2 * a) * Math.PI / 2;
            if (t < Math.PI) return a * Math.Cos(t); return -a + (t - Math.PI) * a;
        }
        double v(double t)
        {
            //if (t < a) return y0 - a / 2;
            //if (t < 2 * a) return y0 -1.5*a +t;
            //if (t < 3 * a) return y0 + 0.5 * a;
            //return y0 +3.5*a-t;
            //return a*((R - r) * Math.Sin(r / R * t) - h * Math.Sin(t - r / R * t));
            //if (t < Math.PI) return a * Math.Sin(t) + 0.5 * (R - a); return 0.5 * (R - a);

            //if (t <= 2 * a) $"a = {a} t = {t} sqrt = {Math.Sqrt(t * (2 * a - t))}".Show();
            //if (t <= 2 * a) return Math.Sqrt(t*(2*a-t)) + 0.5 * (R - a); return 0.5 * (R - a); 

            if (t < Math.PI) return a * Math.Sin(t) + 0.5 * (R - a); return 0.5 * (R - a);
        }
    }
}
