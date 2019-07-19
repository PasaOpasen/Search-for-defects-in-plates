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
using Практика_с_фортрана;
using Defect2019;
using System.IO;
using System.Diagnostics;

namespace Работа2019
{
    public partial class OnePoint : Form
    {
        double x, y, tmin, tmax;

        private async void button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                var t = new Task(() => IlushaMethod());
                await Task.Run(()=>t.Start());
                t.Wait();               
            }

            FilesToSources();
            x = textBox1.Text.ToDouble();
            y = textBox2.Text.ToDouble();
            tmin = textBox3.Text.ToDouble();
            tmax = textBox4.Text.ToDouble();
            tcount = numericUpDown1.Value.ToInt32();

            double th = (tmax - tmin) / (tcount - 1),wh=(РабКонсоль.wend- РабКонсоль.wbeg)/(РабКонсоль.wcount-1);

            double[] ur = new double[tcount], uz = new double[tcount];
            Tuple<double, double> tmp;
            Number.Complex[][] mas = new Number.Complex[РабКонсоль.wcount][];
            for (int i = 0; i < tcount; i++)
            {
                tmp = Functions.Uxt(x, y, tmin + i * th, sources);
                ur[i] = tmp.Item1;
                uz[i] = tmp.Item2;
            }
            for (int i = 0; i < РабКонсоль.wcount; i++)
            {
                mas[i] = Functions.uRes(x, y, РабКонсоль.wbeg + i * wh, sources[1].Norms);
            }

            using (StreamWriter f = new StreamWriter("OnePoint.txt"))
            {
                f.WriteLine("t ur uz");
                for (int i = 0; i < tcount; i++)
                     f.WriteLine($"{tmin + i * th} {ur[i]} {uz[i]}");
            }
            string s =$"x = {x}; y = {y}; t in [{tmin}; {tmax}]; tcount = {tcount}";
            using (StreamWriter f = new StreamWriter("OnePoint(info).txt"))
            {
                  f.WriteLine(s);
                f.WriteLine($"x = {x}; y = {y}; w in [{РабКонсоль.wbeg}; {РабКонсоль.wend}]; wcount = {РабКонсоль.wcount}");
            }
            using (StreamWriter f = new StreamWriter("OnePoint(w).txt"))
            {
                f.WriteLine("w uxRe uxIm uyRe uyIm uzRe uzIm");
                for (int i = 0; i < РабКонсоль.wcount; i++)
                   f.WriteLine($"{РабКонсоль.wbeg + i * wh} {mas[i][0].Re} {mas[i][0].Im} {mas[i][1].Re} {mas[i][1].Im} {mas[i][2].Re} {mas[i][2].Im}");
            }


            OtherMethods.StartProcess2("OnePoint.r");
            Process.Start(s + ".pdf");
            this.Close();
        }

        int tcount;

        public Source[] sources;
        public OnePoint(Source[] smas)
        {
            InitializeComponent();
            sources = smas;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// Вызвать форму и нарисовать специальные графики после её закрытия
        /// </summary>
        public void IlushaMethod()
        {
            var form = new PS5000A.PS5000ABlockForm(РабКонсоль.wbeg, РабКонсоль.wend, РабКонсоль.wcount);

            void cmet(object sender, FormClosingEventArgs e)
            {
                if (form.checkBox1.Checked)
                    CreateArraysGrafic();
            }

            form.FormClosing += new FormClosingEventHandler(cmet);
            //form.FormClosed += new FormClosedEventHandler(cmet);
            form.ShowDialog();

        }
        /// <summary>
        /// Считать файлы и записать f(w) в имеющиеся источники
        /// </summary>
        private void FilesToSources()
        {
            Parallel.For(0, sources.Length, (int i) => sources[i].FmasFromFile());
        }
        Color[] colors = new Color[] { Color.Blue, Color.Green, Color.Red, Color.Black };
        /// <summary>
        /// Создание формы с графиками
        /// </summary>
        private void CreateArraysGrafic()
        {
            string[] fn = new string[4];
            fn[0] = "ArrayA";
            fn[1] = "ArrayB";
            fn[2] = "ArrayC";
            fn[3] = "ArrayD";

            var form = new JustGrafic();
            form.chart1.Series.Clear();

            for (int i = 0; i < sources.Length; i++)
                form.chart1.Series.Add(fn[i]);

            for (int i = 0; i < sources.Length; i++)
            {
                form.chart1.Series[i].BorderWidth = 1;
                form.chart1.Series[i].Color = colors[i];
                form.chart1.Series[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                form.chart1.Series[i].BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
                form.chart1.Series[i].Font = new Font("Arial", 16);

                using (StreamReader f = new StreamReader(fn[i] + ".txt"))
                {
                    int k = 1;
                    string s = f.ReadLine();
                    while (s != null)
                    {
                        form.chart1.Series[i].Points.AddXY(k++, s.Replace('.', ',').ToDouble());
                        s = f.ReadLine();
                    }
                }
            }

            form.CreateCheckBoxes();
            form.Lims();

            form.ShowDialog();
        }



    }
}