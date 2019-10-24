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
using System.Media;
using static МатКлассы.Number;

namespace Работа2019
{
    public partial class OnePoint : Form
    {
        double x, y, tmin, tmax, th, wh;
        int tcount;
        int save = 0, all = 1;

        double[] ur, uz;
        double[,] urs, uzs;

        private void ReadData()
        {
            x = textBox1.Text.ToDouble();
            y = textBox2.Text.ToDouble();
            tmin = textBox3.Text.ToDouble();
            tmax = textBox4.Text.ToDouble();
            tcount = numericUpDown1.Value.ToInt32();

            th = (tmax - tmin) / (tcount - 1);
            wh = (РабКонсоль.wend - РабКонсоль.wbeg) / (РабКонсоль.wcount - 1);

            ur = new double[tcount]; uz = new double[tcount];
            urs = new double[sources.Length, tcount]; uzs = new double[sources.Length, tcount];
        }
        private async Task ReadFwsAsync()
        {
            if (checkBox1.Checked)
            {
                toolStripStatusLabel1.Text = "Ожидает новых данных";
                var t = Task.Run(() => IlushaMethod());
                await t;
            }

            toolStripStatusLabel1.Text = "Считывает данные";

            Sarray = Source.GetSourcesWithReadFw(textBox5.Text, sources);
            if (Sarray.Length == 0)
                Sarray = GetPathWithSources();

            toolStripStatusLabel1.Text = "Данные считаны";
        }


        private async void button1_Click(object sender, EventArgs e)
        {
            await ReadFwsAsync();
            ReadData();

            toolStripStatusLabel1.Text = "Вычисляет u(x,t)";

            timer1.Start();
            await CalcUxtAsync();
            toolStripProgressBar1.Value = toolStripProgressBar1.Maximum;
            timer1.Stop();

            var s = WriteUxt();

            toolStripStatusLabel1.Text = "Вычисляет u(x,w)";

            await CalcUxwAsync();

            toolStripStatusLabel1.Text = "Запускает скрипт";
            await Task.Run(() => OtherMethods.StartProcess2("OnePoint.r"));
            OtherMethods.PlaySound("ВычисленияЗавершены");

            GetUxtInfo(s);
            this.Close();
        }
        private void GetUxtInfo(string s)
        {
            var range = Enumerable.Range(0, Sarray.Length);
            var names = range.Select(n => Sarray[n].Center.ToString()).ToArray();
            var filenames = range.Select(n => Path.Combine(Environment.CurrentDirectory, "center = " + names[n] + "; " + s + ".pdf")).ToArray();

            new UxtInfo(tmin, th, ur, uz, names, filenames, s + ".pdf").Show();
        }

        private async Task CalcUxtAsync()
        {
            all = tcount;
            save = 0;
            int[] sum = new int[all];

            if (radioButton7.Checked)
                await Task.Run(() =>
                {
                    Parallel.For(0, tcount, (int i) =>
                    {
                        double t = tmin + i * th;
                        
                        if (t != 0)
                        {
                            double urt = 0, uzt = 0;
                            for (int k = 0; k < Sarray.Length; k++)
                            {
                                var tp = Functions.UxtOne(x, y, t, Sarray[k]);
                                urs[k, i] = tp.Item1.Re;
                                uzs[k, i] = tp.Item2.Re;
                                urt += urs[k, i];
                                uzt += uzs[k, i];

                            }
                            ur[i] += urt * urt;
                            uz[i] += uzt * uzt;
                        }
                        sum[i] = 1;
                        save = sum.Sum();
                    });
                });
            if (radioButton8.Checked)
                await Task.Run(() =>
                {
                    Parallel.For(0, tcount, (int i) =>
                    {
                        double t = tmin + i * th;
                        
                        if (t != 0)
                        {
Complex urt = 0, uzt = 0;
                            for (int k = 0; k < Sarray.Length; k++)
                            {
                                var tp = Functions.UxtOne(x, y, t, Sarray[k]);
                                urs[k, i] = tp.Item1.Re;
                                uzs[k, i] = tp.Item2.Re;
                                urt += tp.Item1;
                                uzt += tp.Item2;

                            }
                            ur[i] += urt.Abs;
                            uz[i] += uzt.Abs;
                        }
                        sum[i] = 1;
                        save = sum.Sum();
                    });
                });
            if (radioButton9.Checked)
                await Task.Run(() =>
                {
                    Parallel.For(0, tcount, (int i) =>
                    {
                        double t = tmin + i * th;
                        
                        if (t != 0)
                        {
double urt = 0, uzt = 0;
                            for (int k = 0; k < Sarray.Length; k++)
                            {
                                var tp = Functions.UxtOne(x, y, t, Sarray[k]);
                                urs[k, i] = tp.Item1.Re;
                                uzs[k, i] = tp.Item2.Re;
                                urt += urs[k, i];
                                uzt += uzs[k, i];

                            }
                            ur[i] += urt;
                            uz[i] += uzt;
                        }
                        sum[i] = 1;
                        save = sum.Sum();
                    });
                });
        }

        private async Task CalcUxwAsync()
        {
            await Task.Run(() =>
            {
                Parallel.For(0, Sarray.Length, (int k) =>
                {
                    using (StreamWriter f = new StreamWriter($"OnePoint(w{k + 1}).txt"))
                    {
                        var mas = Enumerable.Range(0, РабКонсоль.wcount).Select(i => Functions.uxwMemoized(x, y, РабКонсоль.wbeg + i * wh, Sarray[k])).ToArray();

                        f.WriteLine("w urRe urIm uzRe uzIm");
                        for (int i = 0; i < РабКонсоль.wcount; i++)
                            f.WriteLine($"{РабКонсоль.wbeg + i * wh} {mas[i].Item1.Re} {mas[i].Item1.Im} {mas[i].Item2.Re} {mas[i].Item2.Im}");
                    }
                });
            });
        }

        private string WriteUxt()
        {
            using (StreamWriter f = new StreamWriter("OnePoint.txt"))
            {
                f.WriteLine("t ur uz");
                for (int i = 0; i < tcount; i++)
                    f.WriteLine($"{tmin + i * th} {ur[i]} {uz[i]}");
            }
            for (int k = 0; k < Sarray.Length; k++)
                using (StreamWriter f = new StreamWriter($"OnePoint(f{k + 1}).txt"))
                {
                    f.WriteLine("t ur uz");
                    for (int i = 0; i < tcount; i++)
                        f.WriteLine($"{tmin + i * th} {urs[k, i]} {uzs[k, i]}");
                }

            string s = $"x = {x}; y = {y}; t in [{tmin}; {tmax}]; tcount = {tcount}";
            files2 = Source.GetCenters(Sarray);
            using (StreamWriter f = new StreamWriter("OnePoint(info).txt"))
            {
                f.WriteLine(s);
                for (int i = 0; i < files2.Length; i++) f.WriteLine(files2[i]);
                for (int i = 0; i < files2.Length; i++)
                    f.WriteLine($"OnePoint(w{i + 1}).txt");
                for (int i = 0; i < files2.Length; i++)
                    f.WriteLine($"OnePoint(f{i + 1}).txt");
            }

            return s;
        }

        public Source[] sources, Sarray;
        public string[] files, files2;
        public OnePoint(Source[] smas)
        {
            InitializeComponent();
            sources = smas;
            files = new string[sources.Length];
            textBox5.Text = Environment.CurrentDirectory;

            SetToolTip();
            FillListAndFileArrays();

            timer1.Interval = 250;
            timer1.Tick += (object o, EventArgs e) =>
             {
                 toolStripProgressBar1.Value = (Expendator.GetProcent(save, all) / 100 * toolStripProgressBar1.Maximum).ToInt();
                 this.Refresh();
             };
        }
        private void SetToolTip()
        {
            toolTip1.AutoPopDelay = 4000;
            toolTip1.InitialDelay = 700;
            toolTip1.ReshowDelay = 400;
            toolTip1.ShowAlways = true;
            toolTip1.SetToolTip(this.checkBox1, "Если снять флажок, обратится к уже имеющимся файлам. Если таких файлов нет, сообщит об ошибке");
        }
        private void FillListAndFileArrays()
        {
            string s, f;
            for (int i = 0; i < sources.Length; i++)
            {
                s = $"({sources[i].Center.x} , {sources[i].Center.y})";
                files[i] = s;
                f = $"f(w) from {s}.txt";
                if (File.Exists(f))
                    File.Delete(f);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// Вызвать форму и нарисовать специальные графики после её закрытия
        /// </summary>
        public static void IlushaMethod()
        {
            var form = new PS5000A.PS5000ABlockForm(РабКонсоль.wbeg, РабКонсоль.wend, РабКонсоль.wcount);
            form.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new ParametrsQu().ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Sarray = GetPathWithSources();
        }

        private Source[] GetPathWithSources()
        {
            toolStripStatusLabel1.Text = "Требуется выбрать папку с данными f(w)";

            Source[] arr;
            string path;
            int i = 0;
            while (i < 4)
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    SoundMethods.OK();
                    path = folderBrowserDialog1.SelectedPath;
                    arr = Source.GetSourcesWithReadFw(path, sources, true);
                    if (arr.Length > 0)
                        return arr;
                }
                SystemSounds.Beep.Play();
                i++;
            }
            MessageBox.Show("За несколько попыток так и не была выбрана папка, содержащая файлы с данным для указанных источников. Перепроверьте данные и повторите поиск", "Ошибка в выборе папки", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return null;
        }

        private void button6_Click(object sender, EventArgs e) => new Библиотека_графики.PdfOpen("Варианты метрик", "formula.pdf").ShowDialog();

        private void button5_Click(object sender, EventArgs e)
        {
            new Helper(textBox3, textBox4, numericUpDown1).Show();
        }
    }
}