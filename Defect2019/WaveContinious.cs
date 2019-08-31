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
using Point = МатКлассы.Point;
using static МатКлассы.Waves;
using static МатКлассы.Number;
using Работа2019;
using static PS5000A.PS5000ABlockForm;

namespace Defect2019
{
    public partial class WaveContinious : Form
    {
        public WaveContinious()
        {
            InitializeComponent();

            BaseConstruct();
        }
        public WaveContinious(Source[] mas) : this()
        {
            sourcesArray = mas.dup();
        }

        private void BaseConstruct()
        {
            button2.Hide();
            label9.Hide();
            checkBox1.Hide();
            toolStripStatusLabel1.Text = "Всё готово к запуску";

            timer1.Interval = 1500;
            timer1.Tick += new EventHandler(Timer1_Tick);
            toolStripStatusLabel2.Text = $"";

            timer2.Interval = 1800;
            timer2.Tick += new EventHandler(Timer2_Tick);

            toolTip1.AutoPopDelay = 7000;
            toolTip1.InitialDelay = 700;
            toolTip1.ReshowDelay = 400;
            toolTip1.ShowAlways = true;

            toolTip1.SetToolTip(this.button4, "Сгенерировать реальные f(w) и запустить построение после нахождения u(x,w)");
            toolTip1.SetToolTip(this.checkBox1, "Занимает около 20 сек на каждый график (не рекомендуется)");
            toolTip1.SetToolTip(this.checkBox3, "(Не рекомендуется.) Если снять флажок, обратится к уже имеющимся файлам или файлам, которые будут созданы с осциллографа. Если таких файлов нет, сообщит об ошибке после вычислений");


            if (File.Exists("Space.txt"))
                using (StreamReader f = new StreamReader("Space.txt"))
                {
                    textBox1.Text = f.ReadLine().Replace('.', ',').Split(' ')[1];
                    textBox3.Text = f.ReadLine().Replace('.', ',').Split(' ')[1];
                    textBox2.Text = f.ReadLine().Replace('.', ',').Split(' ')[1];
                    textBox4.Text = f.ReadLine().Replace('.', ',').Split(' ')[1];
                    numericUpDown1.Value = f.ReadLine().Replace('.', ',').Split(' ')[1].ToInt32();
                }
            if (File.Exists("LastTimeConfig.txt"))
                using (StreamReader f = new StreamReader("LastTimeConfig.txt"))
                {
                    textBox5.Text = f.ReadLine().Replace('.', ',').Split(' ')[1];
                    textBox6.Text = f.ReadLine().Replace('.', ',').Split(' ')[1];
                    numericUpDown2.Value = f.ReadLine().Replace('.', ',').Split(' ')[1].ToInt32();
                }


            MinSize = new Size(this.Size.Width, this.Size.Height / 2);
            MaxSize = new Size(MinSize.Width, MinSize.Height * 2);
            checkBox1_CheckedChanged(new object(), new EventArgs());
        }
        private Size MinSize, MaxSize;

        private int all, save = 0;
        private void Timer1_Tick(object Sender, EventArgs e)
        {
            toolStripProgressBar1.Value = (save.ToDouble() / all * toolStripProgressBar1.Maximum).ToInt();
            toolStripStatusLabel2.Text = $"({save} из {all})";
        }
        private void Timer2_Tick(object Sender, EventArgs e)
        {
            toolStripProgressBar1.Value = (OtherMethods.Saved.ToDouble() / OtherMethods.SaveCount * toolStripProgressBar1.Maximum).ToInt();

            toolStripStatusLabel2.Text = OtherMethods.info ?? $"({OtherMethods.Saved} из {OtherMethods.SaveCount}, {(OtherMethods.Saved.ToDouble() / OtherMethods.SaveCount * 100).ToString(3)}%)";
        }

        public double xmin, xmax, ymin, ymax, tmin, tmax;
        public int count, tcount;
        /// <summary>
        /// Имена png файлов
        /// </summary>
        public string[] filenames;
        /// <summary>
        /// Текущие источники
        /// </summary>
        public Source[] sourcesArray;

        private void button2_Click(object sender, EventArgs e)
        {
            source.Cancel();
            toolStripStatusLabel1.Text = "Операция отменена";
            toolStripStatusLabel2.Text = "";
            toolStripProgressBar1.Value = 0;
            button2.Hide();
            KillR();

            FilenamesArrayToFile("");

            GC.Collect();
            EndShows();

        }
        private void KillR()
        {
            List<string> name = new List<string> { "rterm", "Rscript" };//процесс, который нужно убить
            System.Diagnostics.Process[] etc = System.Diagnostics.Process.GetProcesses();//получим процессы
            foreach (System.Diagnostics.Process anti in etc)//обойдем каждый процесс
            {
                foreach (string s in name)
                {
                    if (anti.ProcessName.ToLower().Contains(s.ToLower())) //найдем нужный и убьем
                    {
                        anti.Kill();
                        name.Remove(s);
                    }
                }
            }
        }

        /// <summary>
        /// Считать боксы
        /// </summary>
        public void GetFields()
        {
            xmin = textBox1.Text.ToDouble();
            xmax = textBox2.Text.ToDouble();
            ymin = textBox3.Text.ToDouble();
            ymax = textBox4.Text.ToDouble();
            tmin = textBox5.Text.ToDouble();
            tmax = textBox6.Text.ToDouble();
            count = numericUpDown1.Value.ToInt32();
            tcount = numericUpDown2.Value.ToInt32();
            all = count * count;
            filenames = new string[tcount];
        }

        /// <summary>
        /// Записать текущую временную конфигурацию в файл
        /// </summary>
        private void SetLastTimeConfig()
        {
            using(StreamWriter f=new StreamWriter("LastTimeConfig.txt"))
            {
                f.WriteLine($"tmin= {tmin}");
                f.WriteLine($"tmax= {tmax}");
                f.WriteLine($"tcount= {tcount}");
            }
        }

        System.Threading.CancellationTokenSource source = new System.Threading.CancellationTokenSource();

        /// <summary>
        /// Вычисление u(x,t)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task UxtCalc()
        {
            label9.Show();
            пределыПоОсиZПриПромежуточномРисованииToolStripMenuItem.Enabled = false;
            ускоренныйПараллелизмДляUxtToolStripMenuItem.Enabled = false;
            //OtherMethods.CopyFiles();

            SetLastTimeConfig();
            label9.Text = "Копирование скриптов";
            OtherMethods.CopyFilesTxt();
            string[] dir = Expendator.GetStringArrayFromFile("WhereData.txt", true);
            for (int i = 0; i < dir.Length; i++)//между циклами можно включить задержку на время выбора параметров, хотя лучше не надо
            {
                save = 0;
                Timer1_Tick(new object(), new EventArgs());
                label9.Text = $"Обрабатывается{Environment.NewLine}замер {i + 1} (из {dir.Length})";
                var s = Source.GetSourcesWithFw(sourcesArray, dir[i]);
                toolStripStatusLabel1.Text = $"Считывается f(w) для замера {i + 1}";
                FilesToSources(s, dir[i]);

                await ManyPZAsync(s, dir[i]);
            }

            await AfterLoopsAct();
        }

        /// <summary>
        /// Суммирование замеров, построение анимации и прочие заключительные действия
        /// </summary>
        /// <returns></returns>
        private async Task AfterLoopsAct()
        {
            label9.Show();
            label9.Text = $"Суммирование{Environment.NewLine}по замерам";
            toolStripStatusLabel1.Text = $"Запущено построение u-surfaces";
            toolStripStatusLabel2.Text = $"";
            BegShow();
            string gl = $"{Source.ToString(sourcesArray)}, (xmin, xmax, xcount, ymin, ymax) = ({xmin}, {xmax}, {count}, {ymin}, {ymax})";

            await Task.Run(() =>
            {
                StreamWriter ts = new StreamWriter("textnames.txt");
                StreamWriter pds = new StreamWriter("pdfnames.txt");

                Expendator.WriteStringInFile("SurfaceMain.txt", gl);

                double th = (tmax - tmin) / (tcount - 1);
                for (int i = 0; i < tcount; i++)
                {
                    double t = tmin + i * th;
                    if (t == 0)
                        continue;
                    string tit = $"{Source.ToString(sourcesArray)}, t = {t.ToString(4)}, (xmin, xmax, xcount, ymin, ymax) = ({xmin}, {xmax}, {count}, {ymin}, {ymax})";
                    ts.WriteLine(tit + ".txt");
                    pds.WriteLine($"3D ur, uz(title , {tit} ).pdf");
                    filenames[i] = "3D " + tit + " .png";
                }
                FilenamesArrayToFile();
                ts.Close();
                pds.Close();

                Parallel.Invoke(
                    () => SSum(),
                    () =>
                    {
                        if (Functions.ur.Lenght > 0)
                        {
                        Functions.ur.Dispose();
                        GC.Collect();
                        }

                    });


                Expendator.CopyFiles(Expendator.GetWordFromFile("WhereData.txt"), Environment.CurrentDirectory, "3D ur, uz(x).txt", "3D ur, uz(y).txt");
            });

            label9.Hide();

            if (source.IsCancellationRequested) return;
            await Task.Run(() => StartProcess("OnlySurface.r", global: true));
            new Библиотека_графики.PdfOpen("Полученные u-surfaces", Path.Combine(Environment.CurrentDirectory, $"{gl} ")).Show();

            if (MessageBox.Show("Создавать анимацию? (может занять до 15 минут)", "Анимация", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                toolStripStatusLabel1.Text = $"Построены u-surface. Создаётся массив кадров";
                await Task.Run(() => OtherMethods.StartProcessOnly("ReDraw3Duxt2.r", true));

                if (source.IsCancellationRequested) return;
                new Anima(filenames).ShowDialog();
            }

            toolStripStatusLabel1.Text = $"Операции закончены";
            new Scheme(sourcesArray, new Point(xmin, ymin), xmax - xmin, ymax - ymin, gl + " (heatmap).png").Show();

            button4.Show();
            EndShows();
        }

        /// <summary>
        /// Просуммировать все замеры
        /// </summary>
        private void SSum()
        {
            var p = Expendator.GetStringArrayFromFile("WhereData.txt");
            string[] names = Expendator.GetStringArrayFromFile("textnames.txt");
            string[][] fnames = new string[p.Length][];
            for (int k = 0; k < p.Length; k++)
            {
                string[] tmp = Expendator.GetStringArrayFromFile(Path.Combine(p[k], "textnames.txt"));
                fnames[k] = new string[names.Length];
                for (int i = 0; i < names.Length; i++)
                    fnames[k][i] = Path.Combine(p[k], tmp[i]);
            }

            int len = count * count;
            Parallel.For(0, names.Length, (int i) =>
            {
                Vectors v = new Vectors(len);
                Vectors v1 = new Vectors(len);
                for (int k = 0; k < p.Length; k++)
                {
                    v += Vectors.VectorFromFile(fnames[k][i].Replace(".txt", " (ur).txt"));
                    v1 += Vectors.VectorFromFile(fnames[k][i].Replace(".txt", " (uz).txt"));
                }

                v.ToFile(names[i].Replace(".txt", " (ur).txt"));
                v1.ToFile(names[i].Replace(".txt", " (uz).txt"));

            });

            string name = Expendator.GetWordFromFile("SurfaceMain.txt");
            names = new string[p.Length];
            for (int i = 0; i < names.Length; i++)
                names[i] = Path.Combine(p[i], Expendator.GetWordFromFile(Path.Combine(p[i], "SurfaceMain.txt")));

            Vectors vv1= new Vectors(len), vv2= new Vectors(len);
            for (int k = 0; k < p.Length; k++)
            {
                vv1+= Vectors.VectorFromFile(names[k] + " (ur).txt").Norming;
                vv2 += Vectors.VectorFromFile(names[k] + " (uz).txt").Norming;
            }
            (vv1 / p.Length).ToFile(name + " (ur).txt");
            (vv2 / p.Length).ToFile(name + " (uz).txt");

        }

        /// <summary>
        /// Вычислить/прочесть u(x,w), сделать замеры или не сделать
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void button4_Click(object sender, EventArgs e)
        {
            толькоПросуммироватьToolStripMenuItem.Visible = false;
            button2.Hide();
            button4.Hide();
            checkBox4.Hide();
            GetFields();

            if (checkBox3.Checked)
                FilesFromSources(sourcesArray);
            checkBox3.Hide();
            groupBox1.Hide();

            timer2.Start();
            toolStripStatusLabel1.Text = "Выполняется генерация u(x,w) и f(w)";
            await Task.Run(
                () =>
                Parallel.Invoke(
                    ()=>OtherMethods.Saveuxw3(xmin, xmax, count, ymin, ymax, sourcesArray),
                    () => { if (checkBox4.Checked) IlushaMethod(); }
                    )
                );
            Timer2_Tick(sender, e);
            timer2.Stop();
            toolStripStatusLabel2.Text = "";

            button2.Show();
            toolStripStatusLabel1.Text = "Начинается построение";

            другиеПараметрыToolStripMenuItem.Visible = false;
              await    UxtCalc();
            толькоПросуммироватьToolStripMenuItem.Visible = true;
            другиеПараметрыToolStripMenuItem.Visible = true;

            toolStripProgressBar1.Value = 0;
        }

        /// <summary>
        /// Считать файлы и записать f(w) в имеющиеся источники
        /// </summary>
        private void FilesToSources(Source[] sources, string path)
        {
            try
            {
                Parallel.For(0, sources.Length, (int i) => sources[i].FmasFromFile(path));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Возникла ошибка при чтении файлов", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Записать f(w) от всех источников в файлы
        /// </summary>
        public void FilesFromSources(Source[] sources)
        {
            string t;
            using (StreamReader r = new StreamReader("WhereData.txt"))
                t = r.ReadLine().Replace("\r\n", "");
            Parallel.For(0, sources.Length, (int i) => sources[i].FmasToFile(t));
        }

        /// <summary>
        /// Вызвать форму Илуши
        /// </summary>
        public void IlushaMethod()
        {
            var form = new PS5000A.PS5000ABlockForm(РабКонсоль.wbeg, РабКонсоль.wend, РабКонсоль.wcount);

            void cmet(object sender, FormClosingEventArgs e)
            {                
            }

            form.FormClosing += new FormClosingEventHandler(cmet);

            form.ShowDialog();
        }

        /// <summary>
        /// Задание пределов по осям
        /// </summary>
        /// <param name="count"></param>
        /// <param name="path"></param>
        private void ZlimsCalculate(int count = 1, string path = null)
        {
            path =path?? Environment.CurrentDirectory;

            if (высчитатьИИспользоватьНовыеЗначенияToolStripMenuItem.Checked)
            {
                toolStripStatusLabel1.Text = "Происходит пересчёт пределов по z";
                if (Forms.UG.listBox1.SelectedIndex != 3)
                {
                    Forms.UG.listBox1.SelectedIndex = 3;
                    Forms.UG.listBox1_SelectedIndexChanged(new object(), new EventArgs());
                    Forms.UG.checkBox13.Checked = false;
                }

                Forms.UG.button1_Click(new object(), new EventArgs());


                double min1, min2, max1, max2;
                using (StreamReader fs = new StreamReader(Path.Combine(path, "zlims.txt")))
                {
                    fs.ReadLine();
                    string[] st = fs.ReadLine().Split(' ');
                    min1 = st[0].ToDouble(); min2 = st[1].ToDouble();
                    st = fs.ReadLine().Split(' ');
                    max1 = st[0].ToDouble(); max2 = st[1].ToDouble();
                }
                using (StreamWriter f = new StreamWriter(Path.Combine(path, "zlims.txt")))
                {
                    f.WriteLine($"ur uz");
                    f.WriteLine($"{min1 * count} {min2 * count}");
                    f.WriteLine($"{max1 * count} {max2 * count}");
                }
            }

            if (автоопределениеToolStripMenuItem.Checked)
                Expendator.WriteStringInFile(Path.Combine(path, "AutoLims.txt"), "yes");
            else
                Expendator.WriteStringInFile(Path.Combine(path, "AutoLims.txt"), "no");
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void button5_Click(object sender, EventArgs e)
        {
            new Helper(textBox5.Text.ToDouble(), textBox6.Text.ToDouble(), numericUpDown2.Value.ToInt32()).Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            new Библиотека_графики.PdfOpen("Варианты метрик", "formula").ShowDialog();
        }

        /// <summary>
        /// Скрыть основные элементы
        /// </summary>
        public void BegShow()
        {
            groupBox1.Hide();
            groupBox2.Hide();
            пределыПоОсиZПриПромежуточномРисованииToolStripMenuItem.Enabled = false;
            ускоренныйПараллелизмДляUxtToolStripMenuItem.Enabled = false;
            groupBox6.Hide();
            checkBox1.Hide();
            checkBox2.Hide();
            // checkBox2.Checked = true;
            checkBox3.Hide();
        }

        /// <summary>
        /// Вернуть скрытые элементы
        /// </summary>
        public void EndShows()
        {
            groupBox1.Show();
            groupBox2.Show();
            пределыПоОсиZПриПромежуточномРисованииToolStripMenuItem.Enabled = true;
            ускоренныйПараллелизмДляUxtToolStripMenuItem.Enabled = true;
            groupBox6.Show();
            checkBox1.Show();
            checkBox2.Show();
            // checkBox2.Checked = true;
            checkBox3.Show();
            checkBox4.Show();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                webBrowser1.Show();
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.Size = new Size(MaxSize.Width, MaxSize.Height);
            }
            else
            {
                webBrowser1.Hide();
                this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                MaxSize = new Size(this.Size.Width, this.Size.Height);
                this.Size = new Size(MinSize.Width, MinSize.Height);
            }
        }

        private void сохранённыеЗначенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (сохранённыеЗначенияToolStripMenuItem.Checked)
            {
                высчитатьИИспользоватьНовыеЗначенияToolStripMenuItem.Checked = false;
                автоопределениеToolStripMenuItem.Checked = false;
            }
            else
            {
                сохранённыеЗначенияToolStripMenuItem.Checked = true;
                высчитатьИИспользоватьНовыеЗначенияToolStripMenuItem.Checked = false;
                автоопределениеToolStripMenuItem.Checked = false;
            }
        }

        private async void толькоПросуммироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetFields();
            checkBox4.Hide();
           await AfterLoopsAct();
        }

        private void даToolStripMenuItem_Click(object sender, EventArgs e)
        {
            даToolStripMenuItem.Checked = true;
            нетToolStripMenuItem.Checked = false;
            checkBox1.Checked = false;
            checkBox1.Hide();
        }

        private void нетToolStripMenuItem_Click(object sender, EventArgs e)
        {
            нетToolStripMenuItem.Checked = true;
            даToolStripMenuItem.Checked = false;
            checkBox1.Show();
        }

        private void пределыПоОсиZПриПромежуточномРисованииToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void автоопределениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (автоопределениеToolStripMenuItem.Checked)
            {
                сохранённыеЗначенияToolStripMenuItem.Checked = false;
                высчитатьИИспользоватьНовыеЗначенияToolStripMenuItem.Checked = false;
            }
            else
            {
                автоопределениеToolStripMenuItem.Checked = true;
                сохранённыеЗначенияToolStripMenuItem.Checked = false;
                высчитатьИИспользоватьНовыеЗначенияToolStripMenuItem.Checked = false;
            }
        }

        private void высчитатьИИспользоватьНовыеЗначенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (высчитатьИИспользоватьНовыеЗначенияToolStripMenuItem.Checked)
            {
                сохранённыеЗначенияToolStripMenuItem.Checked = false;
                автоопределениеToolStripMenuItem.Checked = false;
            }
            else
            {
                высчитатьИИспользоватьНовыеЗначенияToolStripMenuItem.Checked = true;
                сохранённыеЗначенияToolStripMenuItem.Checked = false;
                автоопределениеToolStripMenuItem.Checked = false;
            }

        }

        /// <summary>
        /// Задать параметры метрики в файл
        /// </summary>
        /// <param name="s"></param>
        private void MetricSet(string s = "max", string path = null)
        {
            path = path ?? Environment.CurrentDirectory;

            Expendator.WriteStringInFile(Path.Combine(path, "MetrixSumOrMax.txt"), s);
            Expendator.WriteStringInFile("MetrixSumOrMax.txt", s);
        }

        /// <summary>
        /// Определить и зафиксировать текущую метрику
        /// </summary>
        /// <param name="path"></param>
        private void Metrics(string path)
        {
            MetricSet("max", path);
            if (!radioButton9.Checked)
            {
                MetricSet("sum", path);
                if (radioButton7.Checked)
                    Uxt = Functions.Uxt1;
                if (radioButton8.Checked)
                    Uxt = Functions.Uxt2;
            }
        }

        Func<double, double, double, Source[], Tuple<double, double>> Uxt = Functions.Uxt3;
        public async Task ManyPZAsync(Source[] sources, string path)
        {
            ZlimsCalculate(sources.Length, path);
            groupBox2.Hide();
            groupBox6.Hide();
            GetFields();

            Metrics(path);

            double th = (tmax - tmin) / (tcount - 1);

            all = tcount;
            timer1.Start();
            IProgress<int> progress = new Progress<int>((p) => { save = p; });
            toolStripStatusLabel1.Text = "Вычисления запущены";         
            System.Threading.CancellationToken token = source.Token;

            StreamWriter ts = new StreamWriter(Path.Combine(path, "textnames.txt"));
            StreamWriter pds = new StreamWriter(Path.Combine(path, "pdfnames.txt"));
            string gl = $"{Source.ToString(sources)}, (xmin, xmax, xcount, ymin, ymax) = ({xmin}, {xmax}, {count}, {ymin}, {ymax})";

            Expendator.WriteStringInFile(Path.Combine(path, "SurfaceMain.txt"), gl);

            double[] xmas = Expendator.Seq(xmin, xmax, count);
            double[] ymas = Expendator.Seq(ymin, ymax, count);

                Func<Point, bool> Filt = (Point point) =>
                       {
                           for (int q = 0; q < sources.Length; q++)
                               if (sources[q].Filter(point))
                                   return true;
                           return false;
                       };
            string filename = "3D ur, uz.txt";

            async Task SlowUxt()
            {
                double[,] ur = new double[count, count], uz = new double[count, count];
                for (int i = 0; i < tcount; i++)
                {
                    double t = tmin + i * th;
                    if (t == 0)
                        continue;

                    toolStripStatusLabel1.Text = $"Построение графика при t = {t.ToString(3)}";
                    string tit = $"{Source.ToString(sources)}, t = {t.ToString(4)}, (xmin, xmax, xcount, ymin, ymax) = ({xmin}, {xmax}, {count}, {ymin}, {ymax})";

                    ts.WriteLine(tit + ".txt");
                    pds.WriteLine($"3D ur, uz(title , {tit} ).pdf");
                    button2.Show();
                    //progress = new Progress<int>((p) => { save = p; });

                    await Task.Run(() =>
                    {
                        МатКлассы.Waves.Circle.FieldToFile(filename, path,
                            (double x, double y) => Uxt(x, y, t, sources),
                            xmas, ymas, ref ur, ref uz,
                            token,
                            Filt,
                            tit,
                            true
                                    );
                    });

                    if (source.IsCancellationRequested)
                        return;

                    if (checkBox1.Checked)                   
                        await Task.Run(() => StartProcess("3Duxt.r", tit, false, path));
                    
                    filenames[i] = "3D " + tit + " .png";
                    save = i + 1;
                    if (source.IsCancellationRequested) return;
                }

                ur = null; uz = null;
            }
            async Task FastUxt()
            {
                save = 0;
                string[] tsmas = new string[tcount],pdsmas=new string[tcount];
                toolStripStatusLabel1.Text = $"Построение u(x,t) с усиленным параллелизмом";
                int[] kmas = new int[tcount];
              
                await Task.Run(() =>
                {
                    OtherMethods.CalcUXT(xmas, ymas, sources);

                    Parallel.For(0, tcount, (int i) =>
                    //for (int i = 0; i < tcount; i++)
                    {
                        double t = tmin + i * th;
                        if (t != 0)
                        {
                        string tit = $"{Source.ToString(sources)}, t = {t.ToString(4)}, (xmin, xmax, xcount, ymin, ymax) = ({xmin}, {xmax}, {count}, {ymin}, {ymax})";

                            tsmas[i]=tit + ".txt";
                            pdsmas[i] = $"3D ur, uz(title , {tit} ).pdf";
 filenames[i] = "3D " + tit + " .png";
                            
                        МатКлассы.Waves.Circle.FieldToFile(path,(double x, double y) => Uxt(x, y, t, sources),
                                xmas, ymas,
                                token,
                                Filt,
                                tit   );

                        if (source.IsCancellationRequested)
                            return;
                           
                        }
                        kmas[i] = 1;

                        save = kmas.Sum();
                        if (source.IsCancellationRequested) return;
    
                    }
                    );
                });

                tsmas = tsmas.Where(n => n != null).ToArray();
                pdsmas=pdsmas.Where(n => n != null).ToArray();
                for (int i = 0; i < tsmas.Length; i++)
                {
                    ts.WriteLine(tsmas[i]);
                    pds.WriteLine(pdsmas[i]);
                }
            }

            WriteXY(filename, path, xmas, ymas);          
            //выбор параллельного или последовательного методов
            if (даToolStripMenuItem.Checked)
                await FastUxt();
            else await SlowUxt();


            Timer1_Tick(new object(), new EventArgs());
            checkBox1.Hide();
            timer1.Stop();
            FilenamesArrayToFile(path);
            ts.Close();
            pds.Close();
            button2.Hide();

            await Animate(gl, path);
        }
        private async Task Animate(string gl,string path)
        {
            toolStripStatusLabel1.Text = $"Запущено построение поверхностей";
            toolStripStatusLabel2.Text = $"";

            if (source.IsCancellationRequested) return;

            label9.Text = $"Очистка...{Environment.NewLine}Построение...";
            await Task.Run(() =>
                Parallel.Invoke(
                () => StartProcess("WavesSurface.r", gl, true, path),
                () =>
                {
                    Functions.cmas.Dispose();
                    GC.Collect();
                })
                );

            // new Библиотека_графики.PdfOpen("Полученные u-surfaces", Path.Combine(Environment.CurrentDirectory, $"{gl} ")).Show();

            if (source.IsCancellationRequested) return;
            checkBox2.Hide();
            if (checkBox2.Checked)
            {
                toolStripStatusLabel1.Text = $"Построены u-surface. Создаётся массив кадров";
                await Task.Run(() => OtherMethods.StartProcessOnly("ReDraw3Duxt2.r", true, path));

                if (source.IsCancellationRequested) return;
                //new Anima(filenames).ShowDialog();
            }
        }

        private void WriteXY(string filename,string path,double[] xmas,double[] ymas)
        {
            string se = filename.Substring(0, filename.Length - 4);//-.txt
            StreamWriter xs = new StreamWriter(Path.Combine(path, se + "(x).txt"));
            StreamWriter ys = new StreamWriter(Path.Combine(path, se + "(y).txt"));
            xs.WriteLine("x"); ys.WriteLine("y");
            for (int i = 0; i < count; i++)
            {
                xs.WriteLine(xmas[i]);
                ys.WriteLine(ymas[i]);
            }
            xs.Close();
            ys.Close();
        }

        private void FilenamesArrayToFile(string path = null)
        {
            path = path ?? Environment.CurrentDirectory;

            var st = filenames.Where(n => (n != null) && n.Length > 0).ToArray();
            if (st.Length > 0)
            {
                using (StreamWriter fs = new StreamWriter(Path.Combine(path, "3D ur, uz(info).txt")))
                {
                    for (int i = 0; i < st.Length; i++)
                        fs.WriteLine(st[i]);
                }
                var p = st.Where(n => File.Exists(n) && (new FileInfo(n).Length > 0)).ToArray();
                if (p.Length > 8)
                    new Anima(p).Show();
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (source != null) source.Cancel();
            // this.Close();
        }

        public void StartProcess(string fileName, string tit=null, bool global = false, string path = null)
        {      
           path =path??Environment.CurrentDirectory;

            Process process = new Process();
            process.StartInfo.FileName = Path.Combine(path, fileName);
            process.StartInfo.WorkingDirectory = path;

            if (!global)
            {
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }

            process.EnableRaisingEvents = true;

            process.Exited += (sender, e) =>
            {
                if (!global)
                {
                    webBrowser1.Navigate(Path.Combine(path, $"3D ur, uz(title , {tit} ).pdf"));
                webBrowser1.Refresh();
                }

            };

            Debug.WriteLine(process.StartInfo.FileName);
            process.Start();
            process.WaitForExit();

        }

    }
}
