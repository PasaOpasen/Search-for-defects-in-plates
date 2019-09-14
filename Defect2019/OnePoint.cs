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
            var UXT = Functions.GetUxtFunc(radioButton7,radioButton8,radioButton9);

            statusStrip1.Show();
            toolStripStatusLabel1.Text = "Готов к работе";

            if (checkBox1.Checked)
            {
                toolStripStatusLabel1.Text = "Ожидает новых данных";
                var t = new Task(() => IlushaMethod());
                await Task.Run(() => t.Start());
                t.Wait();
            }

            toolStripStatusLabel1.Text = "Считывает данные";
            FilesToSources();
            x = textBox1.Text.ToDouble();
            y = textBox2.Text.ToDouble();
            tmin = textBox3.Text.ToDouble();
            tmax = textBox4.Text.ToDouble();
            tcount = numericUpDown1.Value.ToInt32();

            double th = (tmax - tmin) / (tcount - 1), wh = (РабКонсоль.wend - РабКонсоль.wbeg) / (РабКонсоль.wcount - 1);

            double[] ur = new double[tcount], uz = new double[tcount];
            double[,] urs = new double[sources.Length, tcount], uzs = new double[sources.Length,tcount];
      //      Tuple<double, double> tmp;

            toolStripStatusLabel1.Text = "Вычисляет u(x,t)";
            //for (int i = 0; i < tcount; i++)
            //{
            //    tmp = Functions.Uxt(x, y, tmin + i * th, sources);
            //    ur[i] = tmp.Item1;
            //    uz[i] = tmp.Item2;
            //}

            await Task.Run(() =>
            {
                Parallel.For(0, tcount, (int i) =>
                {
                    double t = tmin + i * th;
                    if (t != 0)
                    {
                        //var tmpt = Functions.Uxt(x, y, t, sources);
                        //ur[i] = tmpt.Item1;
                        //uz[i] = tmpt.Item2;
                        for(int k=0;k<sources.Length;k++)
                        {
                            var tp = Functions.UxtOne(x, y, t, sources[k]);
                           var cor = new Number.Complex(x - sources[k].Center.x, y - sources[k].Center.y).Arg;
                            urs[k, i] = tp.Item1.Re;
                            uzs[k, i] = tp.Item2.Re;
                            ur[i] += urs[k, i];
                            uz[i] += uzs[k, i];
                        }
                    }
                });
            });

            using (StreamWriter f = new StreamWriter("OnePoint.txt"))
            {
                f.WriteLine("t ur uz");
                for (int i = 0; i < tcount; i++)
                    f.WriteLine($"{tmin + i * th} {ur[i]} {uz[i]}");
            }
            for(int k=0;k<sources.Length;k++)
                using (StreamWriter f = new StreamWriter($"OnePoint(f{k+1}).txt"))
                {
                    f.WriteLine("t ur uz");
                    for (int i = 0; i < tcount; i++)
                        f.WriteLine($"{tmin + i * th} {urs[k,i]} {uzs[k,i]}");
                }

            

            string s = $"x = {x}; y = {y}; t in [{tmin}; {tmax}]; tcount = {tcount}";
            using (StreamWriter f = new StreamWriter("OnePoint(info).txt"))
            {
                f.WriteLine(s);
                for (int i = 0; i < files.Length; i++) f.WriteLine(files[i]);
                for (int i = 0; i < files.Length; i++)
                    f.WriteLine($"OnePoint(w{i + 1}).txt");
                for (int i = 0; i < files.Length; i++)
                    f.WriteLine($"OnePoint(f{i + 1}).txt");
            }

            toolStripStatusLabel1.Text = "Вычисляет u(x,w)";
            //CVectors[] uf = new CVectors[РабКонсоль.wcount];
            Parallel.For(0, sources.Length, (int k) =>
            {
                using (StreamWriter f = new StreamWriter($"OnePoint(w{k + 1}).txt"))
                {
                    Tuple<Number.Complex,Number.Complex>[] mas =new Tuple<Number.Complex, Number.Complex>[РабКонсоль.wcount];
                    for (int i = 0; i < РабКонсоль.wcount; i++)
                    {
                        mas[i] = Functions.uxwMemoized(x, y, РабКонсоль.wbeg + i * wh, sources[k]);
                       // uf[i] = new CVectors(mas[i]) * sources[k].Fmas.Item2[i];
                    }
                    f.WriteLine("w uxRe uxIm uyRe uyIm uzRe uzIm");
                    for (int i = 0; i < РабКонсоль.wcount; i++)
                        f.WriteLine($"{РабКонсоль.wbeg + i * wh} {mas[i].Item1.Re} {mas[i].Item2.Im} {mas[i].Item1.Re} {mas[i].Item2.Im}");
                }
                //using (StreamWriter f = new StreamWriter($"OnePoint(uwf{k + 1}).txt"))
                //{
                //    f.WriteLine("w uxRe uxIm uyRe uyIm uzRe uzIm");
                //    for (int i = 0; i < РабКонсоль.wcount; i++)
                //        f.WriteLine($"{РабКонсоль.wbeg + i * wh} {uf[i][0].Re} {uf[i][0].Im} {uf[i][1].Re} {uf[i][1].Im} {uf[i][2].Re} {uf[i][2].Im}");
                //}

            });

            toolStripStatusLabel1.Text = "Запускает скрипт";
            OtherMethods.StartProcess2("OnePoint.r");

            //Process.Start("center = "+files[listBox1.SelectedIndex]+"; "+s + ".pdf");

            new UxtInfo(tmin, th, ur, uz, "center = " + files[listBox1.SelectedIndex] + "; " + s + ".pdf", s + ".pdf").Show();

            this.Close();
        }

        int tcount;

        public Source[] sources;
        public string[] files;
        public OnePoint(Source[] smas)
        {
            InitializeComponent();
            sources = smas;
            files = new string[sources.Length];

            toolTip1.AutoPopDelay = 4000;
            toolTip1.InitialDelay = 700;
            toolTip1.ReshowDelay = 400;
            toolTip1.ShowAlways = true;

            toolTip1.SetToolTip(this.checkBox1, "Если снять флажок, обратится к уже имеющимся файлам. Если таких файлов нет, сообщит об ошибке");

            listBox1.Items.Clear();
            string s;
            for (int i = 0; i < sources.Length; i++)
            {
                s = $"({sources[i].Center.x} , {sources[i].Center.y})";
                listBox1.Items.Add(s);
                files[i] = /*"center = " + */s;// + $"; x = {x}; y = {y}; t in [{tmin}; {tmax}]; tcount = {tcount}";
            }

            if (sources.Length > 1)
                listBox1.SelectedIndex = 1;
            else
                listBox1.SelectedIndex = 0;

            statusStrip1.Hide();
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
            }

            form.FormClosing += new FormClosingEventHandler(cmet);
            //form.FormClosed += new FormClosedEventHandler(cmet);
            form.ShowDialog();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            new ParametrsQu().ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e) => new Библиотека_графики.PdfOpen("Варианты метрик", "formula.pdf").ShowDialog();

        private void button5_Click(object sender, EventArgs e)
        {
            new Helper(textBox3, textBox4, numericUpDown1).Show();
        }

        /// <summary>
        /// Считать файлы и записать f(w) в имеющиеся источники
        /// </summary>
        private void FilesToSources()
        {
            try
            {
                string t;
                using (StreamReader r = new StreamReader("WhereData.txt"))
                    t = r.ReadLine().Replace("\n", "");
                Parallel.For(0, sources.Length, (int i) => sources[i].FmasFromFile());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Возникла ошибка при чтении файлов", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        Color[] colors = new Color[] { Color.Blue, Color.Green, Color.Red, Color.Black };

    }
}