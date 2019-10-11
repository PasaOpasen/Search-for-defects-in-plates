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
using System.IO;
using System.Diagnostics;
using Accord.Math;

namespace Работа2019
{
    public partial class WaveletContinious : Form
    {
        private readonly double timestep = 16E-9, globalTimeMin = -0.0004;
        private readonly int tickCount = 150000;
        private readonly string DefSymbols = "ABCDEFGH";
        private readonly Wavelet.Wavelets MyWavelet = Wavelet.Wavelets.LP;

        private static string symbols;
        private Source[] sources;
        private double wmin, wmax, tmin, tmax, epsForWaveletValues = 1e-8,sd;
        private int wcount, tcount, byevery, pointcount, pointmax, pointmax2;
        private NetOnDouble Wnet, Tnet;

        public WaveletContinious(Source[] array)
        {
            InitializeComponent();
            sources = array;

            SetDefaultStrip();
            SetDataGrid();

            timer1.Tick += (o, e) =>
            {
                var d = Expendator.GetProcent(save, all, 2);
                toolStripLabel2.Text = $"{d}%";
                toolStripProgressBar1.Value = (int)(d / 100 * toolStripProgressBar1.Maximum);
            };
            groupBox3.Hide();
        }

        private void SetDefaultStrip()
        {
            toolStripLabel1.Text = "Готов";
            SetDefaltProgressBar();
        }
        private void SetDefaltProgressBar()
        {
            toolStripLabel2.Text = "";
            toolStripProgressBar1.Value = 0;
        }
        private void SetDataGrid()
        {
            dataGridView1.RowHeadersVisible = false;

            var column1 = new DataGridViewColumn();
            column1.HeaderText = "Источник"; //текст в шапке
            column1.Width = 450; //ширина колонки
            column1.ReadOnly = true; //значение в этой колонке нельзя править
            column1.Name = "source"; //текстовое имя колонки, его можно использовать вместо обращений по индексу
            column1.Frozen = true; //флаг, что данная колонка всегда отображается на своем месте
            column1.CellTemplate = new DataGridViewTextBoxCell(); //тип нашей колонки

            var column2 = new DataGridViewColumn();
            column2.HeaderText = "Имя";
            column2.Name = "name";
            column2.CellTemplate = new DataGridViewTextBoxCell();

            dataGridView1.Columns.Add(column1);
            dataGridView1.Columns.Add(column2);

            dataGridView1.AllowUserToAddRows = false; //запрешаем пользователю самому добавлять строки

            for (int i = 0; i < sources.Length; ++i)
            {
                //Добавляем строку, указывая значения колонок поочереди слева направо
                dataGridView1.Rows.Add(sources[i].ToShortString(), DefSymbols[i]);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            new Defect2019.ParametrsQu().ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GetData();
            var form = new R2Area(Wnet, Tnet);
            form.FormClosing += (o, s) =>
            {
                if (form.OK)
                {
                    textBox2.Text = (1.0 / (form.X.Begin * 1000)).ToString();
                    textBox1.Text = (1.0 / (form.X.End * 1000)).ToString();
                    textBox3.Text = form.Y.Begin.ToString();
                    textBox4.Text = form.Y.End.ToString();
                    numericUpDown1.Value = form.X.Count;
                    numericUpDown1.Value = form.Y.Count;
                    GetData();
                }
                else
                    SoundMethods.Back();
            };

            form.Show();
        }

        private async void toolStripButton2_Click(object sender, EventArgs e)
        {
            const int howmany = 40;

            if (MessageBox.Show($"Для текущего прямоугольника будут произведены все построения на сетке {howmany}*{howmany}, затем будут показаны некоторые результаты в случайном порядке. Это займёт время. Взглянув на результаты, пользователь должен сам обрезать исходный прямоугольник. Выполнить?",
                "Тестирование для опеределения области", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                return;

            OtherMethods.PlaySound("Поехали");
            SetDir();
            SetSymbols();
            GetData();

            var tmp = GetDataPaths();
            string[] names = tmp.Item1;
            string[] wheredata = tmp.Item2;

            all = howmany * howmany;
            int alles = sources.Length * (sources.Length - 1);
            IProgress<int> progress = new Progress<int>((int val) => save = val);

            for (int i = 0; i < sources.Length; i++)
            {
                var itSource = sources[i];
                var otherSources = sources.Without(itSource);
                var othernames = names.Without(names[i]);
                var snames = new string(symbols.ToCharArray().Without(symbols[i]));

                timer1.Start();
                for (int k = 0; k < otherSources.Length; k++)
                {
                    string savename = $"{snames[k]} -> {symbols[i]}";

                    toolStripLabel1.Text = $"Замер {symbols[i]}, источник {snames[k]}, осталось {alles--}";

                    var tuple = await Functions.GetMaximunFromArea(
                        new NetOnDouble(Wnet, 40), new NetOnDouble(Tnet, 40),
                        progress, new System.Threading.CancellationToken(),
                       globalTimeMin, timestep, tickCount, othernames[k], Path.Combine(Environment.CurrentDirectory, savename.Replace(" -> ", "to")),
                       MyWavelet, wheredata[i], 32, epsForWaveletValues);
                }
                SetDefaltProgressBar();
                timer1.Stop();
                OtherMethods.PlaySound("ЗамерОбработан");
            }

            ShowPdfs();

            SetDefaultStrip();
            OtherMethods.PlaySound("ТестированиеОкончено");
        }

        private Tuple<string[], string[]> GetDataPaths()
        {
            string[] names = Enumerable.Range(0, sources.Length).Select(i => $"Array{dataGridView1[1, i].Value}.txt").ToArray();
            string[] wheredata = Expendator.GetStringArrayFromFile("WhereData.txt").Select(s => Path.Combine(s, "Разница")).ToArray();
            return new Tuple<string[], string[]>(names, wheredata);
        }

        private void ShowPdfs()
        {
            List<string> L = new List<string>();
            for (int i = 0; i < symbols.Length; i++)
                for (int j = 0; j < symbols.Length; j++)
                    L.Add($"{symbols[i]}to{symbols[j]}");
            var arrt = L.Where(s => s[0] != s[3]).ToArray();

            var mas = Enumerable.Range(0, 28).Select(i => RandomNumbers.NextNumber(sources.Length * (sources.Length - 1))).Distinct().ToArray();
            var arr = mas.Select(i => arrt[i]).ToArray();

            new Библиотека_графики.ManyDocumentsShower("Поверхности на негустой сетке", arr, arr.Select(i => i + ".pdf").ToArray()).Show();
            button2_Click(new object(), new EventArgs());
        }


        private void GetData()
        {
            wmin = 1.0 / textBox2.Text.ToDouble() / 1000;
            wmax = 1.0 / textBox1.Text.ToDouble() / 1000;
            tmin = textBox3.Text.ToDouble();
            tmax = textBox4.Text.ToDouble();
            wcount = numericUpDown1.Value.ToInt32();
            tcount = numericUpDown2.Value.ToInt32();
            byevery = numericUpDown3.Value.ToInt32();
            pointcount = numericUpDown4.Value.ToInt32();
            pointmax = numericUpDown5.Value.ToInt32();
            pointmax2 = numericUpDown6.Value.ToInt32();

            Wnet = new NetOnDouble(wmin, wmax, wcount);
            Tnet = new NetOnDouble(tmin, tmax, tcount);
            epsForWaveletValues = textBox5.Text.ToDouble();

            all = wcount * tcount;
        }
        private void SetSymbols()
        {
            DialogResult MB(string text) => MessageBox.Show(text, "Ошибка в именах", MessageBoxButtons.OK, MessageBoxIcon.Error);

            try
            {
                symbols = new string(Enumerable.Range(0, sources.Length).Select(i => Convert.ToChar(dataGridView1[1, i].Value)).ToArray());
                if (symbols.Length != symbols.Distinct().Count())
                {
                    MB("Найдены совпадающие имена. Будут использованы имена по умолчанию");
                    symbols = DefSymbols;
                }
            }
            catch
            {
                MB("Минимум одно из заданных имён не является допустимым. Используйте только символы. Сейчас будут использованы имена по умолчанию");
                symbols = new string(DefSymbols.ToCharArray());
            }
            finally
            {
                for (int i = 0; i < sources.Length; i++)
                    dataGridView1[1, i].Value = symbols[i];
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            MakeEllipses(Expendator.GetStringArrayFromFile(Path.Combine("Максимумы с эллипсов", "Params.txt"), true));
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            groupBox3.Show();
            groupBox4.Hide();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            groupBox3.Hide();
            groupBox4.Show();
        }

        int save = 0, all = 1;
        string dir;

        private void SetDir()
        {
            dir = Path.Combine(Environment.CurrentDirectory, "Максимумы с эллипсов");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        private void TransformArea(string ABCfile, string Arrayfile, double tmin, double step)
        {
            TransformWariety(ABCfile);
            TransformTime(Arrayfile, tmin, step);

            this.Refresh();
        }
        private void TransformWariety(string ABCfile)
        {
            double w = 0, max = 0, tmp;
            using (StreamReader r = new StreamReader(ABCfile))
            {
                string s = r.ReadLine();
                string[] st;
                while (true)
                {
                    s = r.ReadLine();
                    if (s == null)
                        break;
                    st = s.Replace('.', ',').Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    tmp = new Number.Complex(Convert.ToDouble(st[1]), Convert.ToDouble(st[2])).Abs;
                    if (tmp > max)
                    {
                        max = tmp;
                        w = Convert.ToDouble(st[0]);
                    }
                }
            }
            w /= 1000;
            textBox1.Text = (w - 5).ToString();
            textBox2.Text = (w + 5).ToString();
        }
        private void TransformTime(string filename, double tmin, double step)
        {
            var arr = File.ReadLines(filename).Select(p => Convert.ToDouble(p.Replace('.', ','))).ToArray();
            const double crosstalk = 0.0001;
            const double crosstalkend=0.45e-3;
            if (tmin < crosstalk)
            {
                arr = arr.Slice(Math.Round((crosstalk - tmin) / step).ToInt(), /*arr.Length - 1*/Math.Round((crosstalkend-tmin )/ step).ToInt());
                tmin = crosstalk;
            }

            const int maxi = 16384;
            int how = arr.Length / maxi;
            double[] arr2 = new double[maxi];
            for (int i = 0; i < arr2.Length; i++)
                arr2[i] = arr[i * how];

            Accord.Math.HilbertTransform.FHT(arr2, FourierTransform.Direction.Forward);

            double t = tmin + Array.IndexOf(arr2, arr2.Max()) * how * step;
            const double dt = 3e-5;
            textBox3.Text = (t - dt).ToString();
            textBox4.Text = (t + dt).ToString();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            OtherMethods.IlushaMethod(checkBox4);
            OtherMethods.PlaySound("Поехали");
            SetDir();
            SetSymbols();

            var tmp = GetDataPaths();
            string[] names = tmp.Item1;
            string[] wheredata = tmp.Item2;

            List<string> pathellipse = new List<string>(sources.Length * (sources.Length - 1));

            int alles = sources.Length * (sources.Length - 1);
            IProgress<int> progress = new Progress<int>((int val) => save = val);

            for (int i = 0; i < sources.Length; i++)
            {
                var itSource = sources[i];
                var otherSources = sources.Without(itSource);
                var othernames = names.Without(names[i]);
                var snames = new string(symbols.ToCharArray().Without(symbols[i]));

                timer1.Start();
                for (int k = 0; k < otherSources.Length; k++)
                {
                    TransformArea(Path.Combine(wheredata[i], $"{snames[k]}.txt"), Path.Combine(wheredata[i], othernames[k]), globalTimeMin, timestep);
                    GetData();
                    string savename = $"{snames[k]} -> {symbols[i]}";

                    toolStripLabel1.Text = $"Замер {symbols[i]}, источник {snames[k]}, осталось {alles--}";

                    var tuple = (radioButton1.Checked) ? await Functions.GetMaximunFromArea(Wnet, Tnet, progress, new System.Threading.CancellationToken(),
                        globalTimeMin, timestep, tickCount, othernames[k], Path.Combine(dir, savename.Replace(" -> ", "to")),
                        MyWavelet, wheredata[i], byevery, epsForWaveletValues) :
                       await Functions.GetMaximunFromArea(wmin, wmax, tmin, tmax,
                        globalTimeMin, timestep, tickCount, othernames[k], Path.Combine(dir, savename.Replace(" -> ", "to")),
                        MyWavelet, wheredata[i], byevery, epsForWaveletValues,
                        pointcount, pointmax, pointmax2);

                    pathellipse.Add($"{otherSources[k].Center.x} {otherSources[k].Center.y} {itSource.Center.x} {itSource.Center.y} {Functions.Vg2(tuple.Item1).ToRString()} {tuple.Item2.ToRString()} {i} {savename} {tuple.Item1}");
                }
                Expendator.WriteInFile(Path.Combine("Максимумы с эллипсов", "Params.txt"), pathellipse.ToArray());
                SetDefaltProgressBar();
                timer1.Stop();
                OtherMethods.PlaySound("ЗамерОбработан");
            }


            OtherMethods.PlaySound("СоздаемЭллипсы");
            MakeEllipses(pathellipse.ToArray());         
            SetDefaultStrip();
        }


        //Scheme scheme = null;
        //private void AddToScheme(EllipseParam p)
        //{
        //    if (scheme == null)
        //    {
        //        scheme = new Scheme(sources, new EllipseParam[] { p });
        //        scheme.StartPosition = FormStartPosition.CenterScreen;
        //        scheme.Show();
        //    }
        //    else
        //    {
        //        scheme.Add(p);
        //        scheme.Refresh();
        //    }
        //}
        private void MakeEllipses(string[] param)
        {
            Functions.SetMinTimeShift(textBox6.Text.ToDouble());
            new Scheme(param, "Схема для всех замеров").Show();
        }
    }
}
