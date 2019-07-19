using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Functions;
using МатКлассы;
using МатКлассы;
using static МатКлассы.Number;

namespace Defect2019
{
    public partial class PRMSN : Form
    {
        public PRMSN()
        {
            InitializeComponent();
            AfterChaigeData();
            ModelRead();
            Hides();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        Complex[] P, R, M, S, N;

        bool flag = false;

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (flag)
                ReDraw();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (flag)
                ReDraw();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Библиотека_графики.ForChart.SaveImageFromChart(chart1, "PRMSN");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            new DINN5().Show();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (flag)
                ReDraw();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (flag)
                ReDraw();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (flag)
                ReDraw();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (flag)
                ReDraw();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (flag)
                ReDraw();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (flag)
                ReDraw();
        }

        Complex[] arg;

        private void button1_Click(object sender, EventArgs e)
        {
            int c = Convert.ToInt32(numericUpDown1.Value);
            P = new Complex[c];
            R = new Complex[c];
            M = new Complex[c];
            S = new Complex[c];
            N = new Complex[c];
            arg = new Complex[c];
            double w = textBox5.Text.ToDouble();

            if (radioButton4.Checked)
            {
                Complex c1 = new Complex(textBox1.Text.ToDouble(), textBox2.Text.ToDouble());
                Complex c2 = new Complex(textBox3.Text.ToDouble(), textBox4.Text.ToDouble());
                Complex h = (c2 - c1) / (c - 1);

                Parallel.For(0, c, (int i) =>
                {
                    arg[i] = c1 + h * i;
                    WritePRMSN(i, arg[i], w);
                });
                tit = $"Отрезок [{c1}; {c2}]";
            }
            else
            {
                double end = textBox6.Text.ToDouble();
                double h = (end + 2 * РабКонсоль.tm) / (c - 1);
                Vectors v = PolesPoles(w);
                double min = v.Min * 0.5, max = v.Max * 1.5;
                if (end < max) end = max + 0.1;
                tit = $"[{0}; {min}] --- проход по R до спуска" + Environment.NewLine + $"[{min}; {min + РабКонсоль.tm}] --- спуск" + Environment.NewLine + $"[{min + РабКонсоль.tm}; {max + РабКонсоль.tm}] --- проход под R" + Environment.NewLine + $"[{max + РабКонсоль.tm}; {max + 2 * РабКонсоль.tm}] --- подъём" + Environment.NewLine + $"[{max + 2 * РабКонсоль.tm}; {end + 2 * РабКонсоль.tm}] --- остаток по R";


                int k = (int)(min / h);//chart1.Annotations[0].SetAnchor(new System.Windows.Forms.DataVisualization.Charting.DataPoint(2, 0)); chart1.Annotations[0].BringToFront();
                if (k <= c)
                    for (int i = 0; i < k; i++)
                    {
                        arg[i] = h * i;
                        WritePRMSN(i, arg[i], w);
                    }
                int k1 = k + (int)(РабКонсоль.tm / h);
                if (k1 <= c)
                    for (int i = k; i < k1; i++)
                    {
                        arg[i] = new Complex(h * (k - 1), h * (/*k1*/i - k));
                        WritePRMSN(i, arg[i], w);
                        arg[i] = arg[i].Re + arg[i].Im.Abs();
                    }
                int k2 = k1 + (int)((max - min) / h);
                if (k2 <= c)
                    for (int i = k1; i < k2; i++)
                    {
                        arg[i] = new Complex(min + (i - k1) * h, -РабКонсоль.tm);
                        WritePRMSN(i, arg[i], w);
                        arg[i] = arg[i].Re + arg[i].Im.Abs();
                    }
                int k3 = k2 + (int)(РабКонсоль.tm / h);
                if (k3 <= c)
                    for (int i = k2; i < k3; i++)
                    {
                        arg[i] = new Complex(max, -РабКонсоль.tm + (i - k2) * h);
                        WritePRMSN(i, arg[i], w);
                        arg[i] = arg[i].Re + 2 * РабКонсоль.tm - arg[i].Im.Abs();
                    }
                int k4 = k3 + (int)((end - max) / h);
                if (k4 <= c)
                    for (int i = k3; i < k4; i++)
                    {
                        arg[i] = new Complex(max + (i - k3) * h, 0);
                        WritePRMSN(i, arg[i], w);
                        arg[i] = arg[i].Re + +2 * РабКонсоль.tm;
                    }

            }


            for (int i = 0; i < 5; i++)
                chart1.Series[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;

            flag = true;
            ReDraw();
        }
        private void WritePRMSN(int i, Complex c, double w)
        {
            var v = Functions.PRMSN(c, w);
            P[i] = v[0];//$"P[{i}] = {P[i]} ".Show();
            R[i] = v[1];
            M[i] = v[2];
            S[i] = v[3];
            N[i] = v[4];
        }

        bool obr = false;
        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            obr = !obr;
            chart1.Series[5].IsVisibleInLegend = obr;

            if (flag)
                ReDraw();
        }

        string tit = "";

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            Hides();
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            Hides();
        }

        private void ReDraw()
        {
            for (int i = 0; i < chart1.Series.Count; i++)
                chart1.Series[i].Points.Clear();
            chart1.Titles[1].Text = tit;


            if (radioButton1.Checked)
            {
                chart1.Titles[0].Text = "Re";
                for (int i = 0; i < arg.Length; i++)
                {
                    double a = arg[i].Re;
                    if (arg[i].Abs != 0)
                    {
                        if (checkBox1.Checked) chart1.Series[0].Points.AddXY(a, P[i].Re);
                        if (checkBox2.Checked) chart1.Series[1].Points.AddXY(a, R[i].Re);
                        if (checkBox3.Checked) chart1.Series[2].Points.AddXY(a, M[i].Re);
                        if (checkBox4.Checked) chart1.Series[3].Points.AddXY(a, S[i].Re);
                        if (checkBox5.Checked) chart1.Series[4].Points.AddXY(a, N[i].Re);
                    }
                }

                if (checkBox6.Checked)
                    for (int i = 0; i < Model.Length; i++)
                    {
                        double d = Model[i].Item1;
                        Complex[] mas = Model[i].Item2;
                        for (int j = 0; j < 5; j++)
                            chart1.Series[5].Points.AddXY(d, mas[j].Re);
                    }
            }
            else if (radioButton2.Checked)
            {
                chart1.Titles[0].Text = "Im";
                for (int i = 0; i < arg.Length; i++)
                {
                    double a = arg[i].Re;
                    if (arg[i].Abs != 0)
                    {
                        if (checkBox1.Checked) chart1.Series[0].Points.AddXY(a, P[i].Im);
                        if (checkBox2.Checked) chart1.Series[1].Points.AddXY(a, R[i].Im);
                        if (checkBox3.Checked) chart1.Series[2].Points.AddXY(a, M[i].Im);
                        if (checkBox4.Checked) chart1.Series[3].Points.AddXY(a, S[i].Im);
                        if (checkBox5.Checked) chart1.Series[4].Points.AddXY(a, N[i].Im);
                    }
                }
                if (checkBox6.Checked)
                    for (int i = 0; i < Model.Length; i++)
                    {
                        double d = Model[i].Item1;
                        Complex[] mas = Model[i].Item2;
                        for (int j = 0; j < 5; j++)
                            chart1.Series[5].Points.AddXY(d, mas[j].Im);
                    }
            }
            else
            {
                chart1.Titles[0].Text = "Abs";
                for (int i = 0; i < arg.Length; i++)
                {
                    double a = arg[i].Re;
                    if (arg[i].Abs != 0)
                    {
                        if (checkBox1.Checked) chart1.Series[0].Points.AddXY(a, P[i].Abs);
                        if (checkBox2.Checked) chart1.Series[1].Points.AddXY(a, R[i].Abs);
                        if (checkBox3.Checked) chart1.Series[2].Points.AddXY(a, M[i].Abs);
                        if (checkBox4.Checked) chart1.Series[3].Points.AddXY(a, S[i].Abs);
                        if (checkBox5.Checked) chart1.Series[4].Points.AddXY(a, N[i].Abs);
                    }
                }
                if (checkBox6.Checked)
                    for (int i = 0; i < Model.Length; i++)
                    {
                        double d = Model[i].Item1;
                        Complex[] mas = Model[i].Item2;
                        for (int j = 0; j < 5; j++)
                            chart1.Series[5].Points.AddXY(d, mas[j].Abs);
                    }
            }

            Библиотека_графики.ForChart.SetToolTips(ref chart1);
            Библиотека_графики.ForChart.SetAxisesY(ref chart1);
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            int c = Args.Length;
            P = new Complex[c];
            R = new Complex[c];
            M = new Complex[c];
            S = new Complex[c];
            N = new Complex[c];
            arg = new Complex[c];
            await Task.Run(()=> { 
            Parallel.For(0, c, (int i) => { 

                WritePRMSN(i, Args[i], 1);
                arg[i] = Model[i].Item1;
            
});
            });
            checkBox6.Checked=true;
            for (int i = 0; i < 5; i++)
                chart1.Series[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            ReDraw();
        }

        private Tuple<double, Complex[]>[] Model;
        private Complex[] Args;
        private void ModelRead()
        {
            StreamReader fs = new StreamReader("GM_test.dat");
            List<Complex[]> list = new List<Complex[]>();
            Complex Ar, P, R, M, S, N;

            string s = fs.ReadLine();
            s = fs.ReadLine();
            while (s != null)
            {
                s = s.Replace('.', ',');//s.Show();
                string[] st = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                Ar = new Complex(st[0].ToDouble(), st[1].ToDouble());
                P = new Complex(st[2].ToDouble(), st[3].ToDouble());
                R = new Complex(st[4].ToDouble(), st[5].ToDouble());
                M = new Complex(st[6].ToDouble(), st[7].ToDouble());
                S = new Complex(st[8].ToDouble(), st[9].ToDouble());
                N = new Complex(st[10].ToDouble(), st[11].ToDouble());

                //$"{Ar} {P} {R} {M} {S} {N}".Show();"".Show();
                //$"{Ar} {Ar.Im.Abs()}".Show(); "".Show();

                list.Add(new Complex[] { Ar, P, R, M, S, N });
                //Tuple<double, double[]> t = new Tuple<double, double[]>(list.Last().Item1,list.Last().Item2);
                //$"{t.Item1} {t.Item2[0]} {t.Item2[1]} {t.Item2[2]}".Show();
                s = fs.ReadLine();
            }


            var l2 = new List<Tuple<double, Complex[]>>();
            int k = 0;
            while (list[k][0].Im == 0)
            {
                l2.Add(new Tuple<double, Complex[]>(list[k][0].Re, new Complex[] { list[k][1], list[k][2], list[k][3], list[k][4], list[k][5] }));
                k++;
            }
            while (list[k][0].Im.Abs() < 0.02)
            {
                l2.Add(new Tuple<double, Complex[]>(list[k][0].Re + list[k][0].Im.Abs(), new Complex[] { list[k][1], list[k][2], list[k][3], list[k][4], list[k][5] }));
                k++;
            }
            while (list[k][0].Im == -0.02)
            {
                l2.Add(new Tuple<double, Complex[]>(list[k][0].Re + 0.02, new Complex[] { list[k][1], list[k][2], list[k][3], list[k][4], list[k][5] }));
                k++;
            }
            while (list[k][0].Im < 0)
            {
                l2.Add(new Tuple<double, Complex[]>(list[k][0].Re + 0.02 + list[k][0].Im.Abs(), new Complex[] { list[k][1], list[k][2], list[k][3], list[k][4], list[k][5] }));
                k++;
            }
            while (k < list.Count)
            {

                l2.Add(new Tuple<double, Complex[]>(list[k][0].Re + 0.04, new Complex[] { list[k][1], list[k][2], list[k][3], list[k][4], list[k][5] }));
                k++;
            }

            Model = l2.ToArray();
            Args = new Complex[list.Count];
            for (int i = 0; i < Args.Length; i++)
                Args[i] = new Complex(list[i][0]);

            fs.Close();
        }

        private void Hides()
        {
            if (radioButton4.Checked)
            {
                textBox1.Show();
                textBox2.Show();
                textBox3.Show();
                textBox4.Show();

                label1.Hide();
                textBox6.Hide();
                checkBox6.Hide();
                button5.Hide();
            }
            else
            {
                textBox1.Hide();
                textBox2.Hide();
                textBox3.Hide();
                textBox4.Hide();

                label1.Show();
                textBox6.Show();
                checkBox6.Show();
                button5.Show();
            }
        }
    }
}
