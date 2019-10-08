using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using МатКлассы;
using static МатКлассы.Number;
using static Functions;

namespace Defect2019
{
    public partial class kGrafic : Form
    {
        public kGrafic()
        {
            InitializeComponent();

            chart1.Series[0].IsVisibleInLegend = false;
            radioButton1_CheckedChanged(new object(), new EventArgs());
            this.chart1.MouseClick += new MouseEventHandler(chart1_MouseClick);
            toolTip1.AutoPopDelay = 4700;

            timer1.Interval = 500;
            timer1.Tick += new EventHandler(Timer1_Tick);

            listBox1.SelectedIndex = 1;
            listBox2.SelectedIndex = 0;

            ReadModelData();
            SetWData();
        }
        public static Tuple<double, double[]>[] Model;
        public static void ReadModelData()
        {
            StreamReader fs = new StreamReader("poles.dat");
            List<Tuple<double, double[]>> list = new List<Tuple<double, double[]>>();

            string s = fs.ReadLine();
            s = fs.ReadLine();
            while (s != null)
            {
                s = s.Replace("+0", "+").Replace("-0", "-").Replace('.', ',');
                string[] st = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                list.Add(new Tuple<double, double[]>(st[0].ToDouble(), new double[] { st[2].ToDouble(), st[3].ToDouble(), st[4].ToDouble() }));
                s = fs.ReadLine();
            }
            fs.Close();
            Model = list.ToArray();
        }
        private void SetWData()
        {
            textBox4.Text = РабКонсоль.wbeg.ToRString();
            textBox5.Text = РабКонсоль.wend.ToRString();
            numericUpDown1.Value = РабКонсоль.wcount;
        }

        int[] prbar;
        private void Timer1_Tick(object Sender, EventArgs e)
        {
            progressBar1.Value = (prbar.Sum().ToDouble() / prbar.Length * progressBar1.Maximum).ToInt();
        }

        double[] args;
        Vectors[] mas, masN;
        Roots.MethodRoot method;
        bool half = false;

        private async void button1_Click(object sender, EventArgs e)
        {
            SetMethodAndHalf();

            if (radioButton1.Checked)
            {
                await rad1();
                may = true;
            }
            else
            {
                await rad2();
                may = false;
            }
        }
        private void SetMethodAndHalf()
        {
            string met = (string)listBox1.SelectedItem;
            half = false;
            switch (met)
            {
                case "Bisec":
                    method = Roots.MethodRoot.Bisec;
                    break;
                case "Brent":
                    method = Roots.MethodRoot.Brent;
                    break;
                case "Broyden":
                    method = Roots.MethodRoot.Broyden;
                    break;
                case "Secant":
                    method = Roots.MethodRoot.Secant;
                    break;
                case "NewtonRaphson":
                    method = Roots.MethodRoot.NewtonRaphson;
                    break;
                case "Combine":
                    method = Roots.MethodRoot.Combine;
                    break;
                case "Halfc (not supplemented)":
                    half = true;
                    break;
                default:
                    method = Roots.MethodRoot.Brent;
                    break;
            }
        }

        private void SimpleBeginAboutChart()
        {
            int ind = listBox2.SelectedIndex;
            chart1.Series.Clear();
            switch (ind)
            {
                case 0:
                    chart1.Series.Add("α: Δ(α,ω) = ΔPRMS(α) = 0");
                    chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
                    chart1.Series[0].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
                    chart1.Series[0].Color = Color.Blue;
                    chart1.Series.Add("α: Δ(α,ω) = ΔN(α) = 0");
                    chart1.Series[1].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
                    chart1.Series[1].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
                    chart1.Series[1].Color = Color.Red;
                    break;
                case 1:
                    chart1.Series.Add("α: 1/K33 = 0");
                    chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
                    chart1.Series[0].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
                    chart1.Series[0].Color = Color.Blue;
                    chart1.Series.Add("α: Δ(α,ω) = ΔN(α) =0 ");
                    chart1.Series[1].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
                    chart1.Series[1].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
                    chart1.Series[1].Color = Color.Red;
                    break;
                case 2:
                    chart1.Series.Add("α: 1/tr(K) = 0");
                    chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
                    chart1.Series[0].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
                    chart1.Series[0].Color = Color.Blue;
                    break;
                default:
                    chart1.Series.Add("α: 1/det(K) = 0");
                    chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
                    chart1.Series[0].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
                    chart1.Series[0].Color = Color.Blue;
                    break;
            }
            if (checkBox1.Checked)
            {
                chart1.Series.Add("Эталон");
                chart1.Series[2].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
                chart1.Series[2].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Star5;
                chart1.Series[2].Color = Color.FromArgb(100, Color.Green);
            }
            if (radioButton3.Checked) chart1.Titles[0].Text = "График ζn(ω)";
            if (radioButton4.Checked) chart1.Titles[0].Text = "График ζn(ω)/ω";
        }

        private async Task rad1()
        {
            SimpleBeginAboutChart();
            progressBar1.Value = 0;

            double tmin = РабКонсоль.polesBeg, tmax = РабКонсоль.polesEnd, eps = РабКонсоль.epsroot, step = РабКонсоль.steproot;
            double beg = Convert.ToDouble(textBox4.Text), end = Convert.ToDouble(textBox5.Text);
            FuncMethods.Optimization.EPS = eps;
            FuncMethods.Optimization.STEP = step;
            int itcount = РабКонсоль.countroot, k = Convert.ToInt32(numericUpDown1.Value);
            double h = (end - beg) / (k - 1);
            mas = new Vectors[k];
            masN = new Vectors[k];
            args = new double[k];
            prbar = new int[k];
            timer1.Enabled = true;

            int ind = listBox2.SelectedIndex;

            await Task.Run(() =>
            {
                Parallel.For(0, k, (int i) =>
                {
                    args[i] = beg + i * h;
                    ComplexFunc del;

                    switch (ind)
                    {
                        case 0:
                            del = (Complex a) => Deltass(a, args[i]);
                            break;
                        case 1:
                            del = (Complex a) => K(a, 1, 0, args[i])[2, 2].Reverse();
                            break;
                        case 2:
                            del = (Complex a) => K(a, 1, 0, args[i]).Track.Reverse();
                            break;
                        default:
                            del = (Complex a) => K(a, 1, 0, args[i]).Det.Reverse();
                            break;
                    }

                    if (!half)
                        mas[i] = Roots.OtherMethod(del, tmin, tmax, step, eps, method, checkBox4.Checked,countpoles:2);
                    else
                        mas[i] = FuncMethods.Optimization.Halfc(del, tmin, tmax, step, eps, itcount).DoubleMas.Where(n => del(n).Abs < eps).Distinct().ToArray();
                    //использовать ли корни N
                    masN[i] = DeltassNPosRoots(args[i], tmin, tmax);

                    prbar[i] = 1;
                });
            });

            ReDraw();
        }
        private void ReDraw()
        {
            SimpleBeginAboutChart();

            int k = args.Length;

            if (checkBox2.Checked)
                for (int i = 0; i < k; i++)
                {
                    if (radioButton3.Checked) for (int j = 0; j < mas[i].Deg; j++) chart1.Series[0].Points.AddXY(args[i], mas[i][j]);
                    if (radioButton4.Checked) for (int j = 0; j < mas[i].Deg; j++) chart1.Series[0].Points.AddXY(args[i], mas[i][j] / args[i]);
                }
            if (checkBox3.Checked)
                for (int i = 0; i < k; i++)
                {
                    if (radioButton3.Checked) for (int j = 0; j < masN[i].Deg; j++) chart1.Series[1].Points.AddXY(args[i], masN[i][j]);
                    if (radioButton4.Checked) for (int j = 0; j < masN[i].Deg; j++) chart1.Series[1].Points.AddXY(args[i], masN[i][j] / args[i]);
                }
            if (checkBox1.Checked)
                for (int i = 0; i < Model.Length; i++)
                {
                    if (radioButton3.Checked) for (int j = 0; j < Model[i].Item2.Length; j++) chart1.Series[2].Points.AddXY(Model[i].Item1, Model[i].Item2[j]);
                    if (radioButton4.Checked) for (int j = 0; j < Model[i].Item2.Length; j++) chart1.Series[2].Points.AddXY(Model[i].Item1, (new Vectors(Model[i].Item2) / Model[i].Item1).DoubleMas[j]);
                }

            Библиотека_графики.ForChart.SetToolTips(ref chart1);
        }
        private async Task rad2()
        {
            chart1.Series.Clear();
            double wold = Convert.ToDouble(textBox13.Text);
            double beg = Convert.ToDouble(textBox4.Text), end = Convert.ToDouble(textBox5.Text);
            int itcount = РабКонсоль.countroot, k = Convert.ToInt32(numericUpDown1.Value);
            double h = (end - beg) / (k - 1);
            double kt1 = k1(wold), kt2 = k2(wold);

            ComplexFunc s1 = (Complex alp) => sigma(alp * alp, kt1);
            ComplexFunc s2 = (Complex alp) => sigma(alp * alp, kt2);

            if (radioButton5.Checked)
            {
                othergraph("Δ", (Complex c) => Deltass(c, wold), beg, k, h);
            }
            if (radioButton6.Checked) { othergraph("σ1", s1, beg, k, h); }
            if (radioButton7.Checked) { othergraph("σ2", s2, beg, k, h); }
        }
        private void othergraph(string func, ComplexFunc f, double beg, int k, double hh)
        {
            prbar = new int[k];
            chart1.Series.Add($"Re {func}"); chart1.Series[0].Color = Color.Red;
            chart1.Series.Add($"Im {func}"); chart1.Series[1].Color = Color.Green;
            chart1.Series.Add($"Abs {func}"); chart1.Series[2].Color = Color.Blue;
            for (int i = 0; i < 3; i++)
            {
                chart1.Series[i].BorderWidth = 3;
                chart1.Series[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                chart1.Series[i].MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
                chart1.Series[i].ToolTip = "X = #VALX, Y = #VALY";
            }
            for (int i = 0; i < k; i++)
            {
                double arg = beg + i * hh;
                Number.Complex val = f(arg); //val.Show();
                chart1.Series[0].Points.AddXY(arg, val.Re);
                chart1.Series[1].Points.AddXY(arg, val.Im);
                chart1.Series[2].Points.AddXY(arg, val.Abs);
                prbar[i] = 1;
            }
            chart1.Titles[0].Text = $"График {func}(ω)";
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            groupBox2.Show();
            groupBox3.Hide();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            groupBox2.Hide();
            groupBox3.Show();
        }


        bool may = false;
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

            if (!(checkBox2.Checked || checkBox3.Checked)) checkBox3.Checked = true;
            if (may) ReDraw();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (!(checkBox2.Checked || checkBox3.Checked)) checkBox2.Checked = true;
            if (may) ReDraw();
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int v = listBox2.SelectedIndex;
            if (v <= 1)
            {
                checkBox2.Show();
                checkBox3.Show();
                checkBox2.Checked = true;
                checkBox3.Checked = true;
            }
            else
            {
                checkBox2.Hide();
                checkBox3.Hide();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (may) ReDraw();
        }

        private void параметрыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ParametrsQu().ShowDialog();
            SetWData();
        }

        private void сохранитьИзображениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double tmin = РабКонсоль.polesBeg, tmax = РабКонсоль.polesEnd, eps = РабКонсоль.epsroot, step = РабКонсоль.steproot;
            double beg = Convert.ToDouble(textBox4.Text), end = Convert.ToDouble(textBox5.Text);

            int itcount = РабКонсоль.countroot, k = Convert.ToInt32(numericUpDown1.Value);

            string name = $"tmin={tmin} tmax={tmax} eps={eps} step={step} kcount={k}  ([{beg},{end}])";
            SaveFileDialog savedialog = new SaveFileDialog();
            savedialog.Title = "Сохранить рисунок как...";
            savedialog.FileName = name;
            savedialog.Filter = "Image files (*.png)|*.png|All files (*.*)|*.*";

            savedialog.OverwritePrompt = true;
            savedialog.CheckPathExists = true;
            savedialog.ShowHelp = true;
            if (savedialog.ShowDialog() == DialogResult.OK)
            {
                Работа2019.SoundMethods.OK();
                try
                {
                    chart1.SaveImage(savedialog.FileName.Substring(0, savedialog.FileName.IndexOf(".png")) + ".emf", System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Emf);
                    chart1.SaveImage(savedialog.FileName, System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Png);
                }
                catch (Exception ee)
                {
                    MessageBox.Show("Рисунок не сохранён", ee.Message,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void исправитьВнутренниеПараметрыНаЗаданныеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            РабКонсоль.wbeg = textBox4.Text.ToDouble();
            РабКонсоль.wend = textBox5.Text.ToDouble();
            РабКонсоль.wcount = numericUpDown1.Value.ToInt32();
            AfterChaigeData();
        }

        double xOne = 0;
        double xTwo = 0;
        private void chart1_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                var result = chart1.HitTest(e.X, e.Y);
                if (result.PointIndex >= 0)
                {
                    xOne = result.Series.Points[result.PointIndex].XValue;
                    xTwo = result.Series.Points[result.PointIndex].YValues[0];
                    Complex PRMS = Deltass(xTwo, xOne), N = DeltassN(xTwo, xOne);
                    toolTip1.Show($"ω = {xOne} α = {xTwo} deltaPRMS = {PRMS} deltaN = {N} |delta| = {(N * PRMS).Abs}", chart1);
                }
            }
            finally { }
        }
     
    }
}
