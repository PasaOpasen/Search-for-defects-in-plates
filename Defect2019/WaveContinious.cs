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
using Point= МатКлассы.Point;
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
            //webBrowser1.Navigate(Environment.CurrentDirectory + @"\" + $"3D ur, uz(title , circle = ((1; 1), r = 0,5), t = 0,1, (xmin, xmax, xcount, ymin, ymax, ycount) = (-10, 100, 4, -20, 90, 4) ).pdf");
        }
        public WaveContinious(Source[] mas):this()
        {
            sources = new Source[mas.Length];
            for (int i = 0; i < sources.Length; i++)
                sources[i] = new Source(mas[i]);
        }

        private void BaseConstruct()
        {
            button2.Hide();
            toolStripStatusLabel1.Text = "Всё готово к запуску";

            timer1.Interval = 1500;
            timer1.Tick += new EventHandler(Timer1_Tick);
            toolStripStatusLabel2.Text = $"";

            timer2.Interval = 1800;
            timer2.Tick += new EventHandler(Timer2_Tick);

            toolTip1.AutoPopDelay = 4000;
            toolTip1.InitialDelay = 700;
            toolTip1.ReshowDelay = 400;
            toolTip1.ShowAlways = true;

            toolTip1.SetToolTip(this.button1, "Запустить построение с теоретическими значениями f(w)");
            toolTip1.SetToolTip(this.button4, "Сгенерировать реальные f(w) и запустить построение после нахождения u(x,w)");
            toolTip1.SetToolTip(this.groupBox3, "Это повлияет только на отображение во время промежуточного построения");
            toolTip1.SetToolTip(this.radioButton1, "Это повлияет только на отображение во время промежуточного построения");
            toolTip1.SetToolTip(this.radioButton2, "Это повлияет только на отображение во время промежуточного построения");
            toolTip1.SetToolTip(this.radioButton3, "В конфигурации Release достаточно быстро + отображение в progressbar");
            toolTip1.SetToolTip(this.radioButton4, "Предположительно на 30-60% быстрее, без progressbar. В этой версии также были использованы некоторые улучшения, которые не применялись в C#");
            toolTip1.SetToolTip(this.radioButton5, "Более простая версия с более вероятной совместимостью");
            toolTip1.SetToolTip(this.radioButton6, "Быстрее, но может не запуститься");
        }

        private int all, save = 0;
        private void Timer1_Tick(object Sender, EventArgs e)
        {
           toolStripProgressBar1.Value= (save.ToDouble() / all * toolStripProgressBar1.Maximum).ToInt();
            toolStripStatusLabel2.Text = $"({save} из {all})";
        }
        private void Timer2_Tick(object Sender, EventArgs e)
        {
            toolStripProgressBar1.Value = (OtherMethods.Saved.ToDouble() / OtherMethods.SaveCount * toolStripProgressBar1.Maximum).ToInt();
            toolStripStatusLabel2.Text = $"({OtherMethods.Saved} из {OtherMethods.SaveCount}, {OtherMethods.Saved.ToDouble()/OtherMethods.SaveCount*100}%)";
        }

        public double xmin, xmax, ymin, ymax, tmin, tmax;
        public int count, tcount;
        public string[] filenames;

        public Source[] sources;

        private void button2_Click(object sender, EventArgs e)
        {
            source.Cancel();
            toolStripStatusLabel1.Text = "Операция отменена (может быть, требуется подождать ещё немного)";
            toolStripProgressBar1.Value = 0;
            button2.Hide();
            FilenamesToFile();
        }

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

        System.Threading.CancellationTokenSource source;
        private void button1_Click(object sender, EventArgs e)
        {
            if (sources==null)
                OnePZ();
            else
                ManyPZ();
        }

        public async void button4_Click(object sender, EventArgs e)
        {
            button2.Hide();
            GetFields();
            FilesFromSources();//потом удалить

            var m=Functions.Seqw(РабКонсоль.wbeg, РабКонсоль.wend, РабКонсоль.wcount);
            (new Vectors(m)).Show();

            Action MyAct = () =>
            {
                if (radioButton3.Checked)
                    OtherMethods.Saveuxw(xmin, xmax, count, ymin, ymax, count, sources);
                else
                    OtherMethods.Saveuxw2(xmin, xmax, count, ymin, ymax, count, sources);
            };

            timer2.Start();
            toolStripStatusLabel1.Text = "Запущена одновременная генерация u(x,w) и f(w)";
            await Task.Run(
                () =>
                Parallel.Invoke(
                    MyAct,
                    () => IlushaMethod()
                    )
                );
            Timer2_Tick(sender, e);
            timer2.Stop();
            toolStripStatusLabel2.Text = "";

            toolStripStatusLabel1.Text = "Считывается f(w)";
            FilesToSources();
            button2.Show();
            toolStripStatusLabel1.Text = "Начинается построение";
            
            button1_Click(sender, e);
        }

        /// <summary>
        /// Считать файлы и записать f(w) в имеющиеся источники
        /// </summary>
        private void FilesToSources()
        {
            Parallel.For(0, sources.Length, (int i) => sources[i].FmasFromFile());
        }

        /// <summary>
        /// Записать f(w) от всех источников в файлы
        /// </summary>
        public void FilesFromSources()
        {
            Parallel.For(0, sources.Length, (int i) => sources[i].FmasToFile());
        }

        Color[] colors = new Color[] { Color.Blue, Color.Green, Color.Red, Color.Black };


        /// <summary>
        /// Вызвать форму и нарисовать специальные графики после её закрытия
        /// </summary>
        public void IlushaMethod()
        {
            var form = new PS5000A.PS5000ABlockForm(РабКонсоль.wbeg, РабКонсоль.wend, РабКонсоль.wcount);

            void cmet(object sender, FormClosingEventArgs e)
            {
                if(form.checkBox1.Checked)
                CreateArraysGrafic();
            }

            form.FormClosing += new FormClosingEventHandler(cmet);
            //form.FormClosed += new FormClosedEventHandler(cmet);
            form.ShowDialog();
            
        }

        /// <summary>
        /// Создание формы с графиками
        /// </summary>
        private  void CreateArraysGrafic()
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


        private void zlims(int count=1)
        {
            if (radioButton2.Checked)
            {
                toolStripStatusLabel1.Text = "Происходит пересчёт пределов по z";
                if (Forms.UG.listBox1.SelectedIndex != 3)
                {
                    Forms.UG.listBox1.SelectedIndex = 3;
                Forms.UG.listBox1_SelectedIndexChanged(new object(), new EventArgs());
                    Forms.UG.checkBox13.Checked = false;
                }

                Forms.UG.button1_Click(new object(), new EventArgs());
            

            double min1,min2, max1,max2;
            using(StreamReader fs=new StreamReader("zlims.txt"))
            {
                fs.ReadLine();
                string[] st = fs.ReadLine().Split(' ');
                min1 = st[0].ToDouble();min2= st[1].ToDouble();
                 st = fs.ReadLine().Split(' ');
                max1 = st[0].ToDouble(); max2 = st[1].ToDouble();
            }
            using(StreamWriter f=new StreamWriter("zlims.txt"))
            {
                f.WriteLine($"ur uz");
                f.WriteLine($"{min1*count} {min2 * count}");
                f.WriteLine($"{max1 * count} {max2 * count}");
            }
            }
        }

        /// <summary>
        /// Построение волны при одном актуаторе
        /// </summary>
        private async void OnePZ()
        {
            zlims();
            GetFields();

            Stopwatch ttt = new Stopwatch();
            ttt.Start();
            var e = Functions.uRes(40, 40,  2, Forms.UG.tuple.Item4);
            var time = ttt.ElapsedMilliseconds;
            ttt.Stop();
           // Times(time, РабКонсоль.wcount, count * count, 1);


            double th = (tmax - tmin) / (tcount - 1);

            timer1.Start();
            IProgress<int> progress = new Progress<int>((p) => { save = p; });
            toolStripStatusLabel1.Text = "Вычисления запущены";
            source = new System.Threading.CancellationTokenSource();
            System.Threading.CancellationToken token = source.Token;

            string gl = $"{((Forms.UG.radioButton3.Checked) ? "circle" : "dcircle")} = (({Forms.UG.tuple.Item1}; {Forms.UG.tuple.Item2}), r = {Forms.UG.tuple.Item3}), (xmin, xmax, xcount, ymin, ymax, ycount) = ({xmin}, {xmax}, {count}, {ymin}, {ymax}, {count})";
            using (StreamWriter fst = new StreamWriter("SurfaceMain.txt"))
            {
                fst.WriteLine(gl);
            }

            for (int i = 0; i < tcount; i++)
            {
                double t = tmin + i * th;
                toolStripStatusLabel1.Text = $"Построение графика при t = {t.ToString(4)} (построено {i} из {tcount})";
                string tit = $"{((Forms.UG.radioButton3.Checked) ? "circle" : "dcircle")} = (({Forms.UG.tuple.Item1}; {Forms.UG.tuple.Item2}), r = {Forms.UG.tuple.Item3}), t = {t.ToString(4)}, (xmin, xmax, xcount, ymin, ymax, ycount) = ({xmin}, {xmax}, {count}, {ymin}, {ymax}, {count})";
                
                string filename = "3D ur, uz.txt";
                button2.Show();
                progress = new Progress<int>((p) => { save = p; });
                await Task.Run(() =>
                {

                    МатКлассы.Waves.Circle.FieldToFile(filename,
                        (double x, double y/*, МатКлассы.Point normal*/) =>
                        {
                            var tt = Forms.UG.u(x, y, t, Forms.UG.tuple.Item4);
                            double cor = new Number.Complex(x - Forms.UG.tuple.Item1, y - Forms.UG.tuple.Item2).Arg;
                            return new Tuple<double, double>(tt[0] * Math.Cos(cor) + tt[1] * Math.Sin(cor), tt[2]);
                        },
                        xmin, xmax, count, ymin, ymax, count,
                        //fiCirc,
                        progress,//ref Forms.UG.prbar,
                        token,
                        Forms.UG.tuple.Item5,
                        tit,
                        false
                                );
                });
                if (source.IsCancellationRequested) return;

                Timer1_Tick(new object(), new EventArgs());
                StartProcess("3Duxt.r",tit);
                filenames[i] ="3D "+ tit+" .png";
                save = 0;                           
            }
            StartProcess("WavesSurface.r", gl, true);
            toolStripStatusLabel1.Text = $"Операции закончены";
            toolStripStatusLabel2.Text = $"";

            FilenamesToFile();
            new Anima(filenames).ShowDialog();
        }

        public async void ManyPZ()
        {
            zlims(sources.Length);
            GetFields();

            //Stopwatch tt=new Stopwatch();
            //tt.Restart();
            //var e = Functions.uRes(40, 40, 0, 2, sources[0].Norms);
            //tt.Stop();
            //var time = tt.ElapsedMilliseconds;
            
            //Times(time, РабКонсоль.wcount, count * count, sources.Length);

            double th = (tmax - tmin) / (tcount - 1);

            timer1.Start();
            IProgress<int> progress = new Progress<int>((p) => { save = p; });
            toolStripStatusLabel1.Text = "Вычисления запущены";
            source = new System.Threading.CancellationTokenSource();
            System.Threading.CancellationToken token = source.Token;

            StreamWriter ts = new StreamWriter("textnames.txt");
            StreamWriter pds = new StreamWriter("pdfnames.txt");
            StreamWriter pns = new StreamWriter("pngnames.txt");

            string gl = $"{Source.ToString(sources)}, (xmin, xmax, xcount, ymin, ymax, ycount) = ({xmin}, {xmax}, {count}, {ymin}, {ymax}, {count})";
            using (StreamWriter fst = new StreamWriter("SurfaceMain.txt"))
            {
                fst.WriteLine(gl);
            }
           // StartProcess("WavesSurface.r", gl, true);
            for (int i = 0; i < tcount; i++)
            {
                double t = tmin + i * th;
                toolStripStatusLabel1.Text = $"Построение графика при t = {t.ToString(4)} (построено {i} из {tcount})";
                string tit = $"{Source.ToString(sources)}, t = {t.ToString(4)}, (xmin, xmax, xcount, ymin, ymax, ycount) = ({xmin}, {xmax}, {count}, {ymin}, {ymax}, {count})";

                ts.WriteLine(tit + ".txt");
                pds.WriteLine($"3D ur, uz(title , {tit} ).pdf");
                pns.WriteLine("3D " + tit + " .png");

                string filename = "3D ur, uz.txt";
                button2.Show();
                progress = new Progress<int>((p) => { save = p; });
                await Task.Run(() =>
                {

                    МатКлассы.Waves.Circle.FieldToFile(filename,
                        (double x, double y) => Functions.Uxt(x, y, t, sources),
                        xmin, xmax, count, ymin, ymax, count,
                        //fiCirc,
                        progress,//ref Forms.UG.prbar,
                        token,
                        (Point p) =>
                        {
                            foreach (Source s in sources)
                                if (s.Filter(p))
                                    return true;
                            return false;
                        },
                        tit,
                        false
                                );
                });
                if (source.IsCancellationRequested) return;

                Timer1_Tick(new object(), new EventArgs());             
                StartProcess("3Duxt.r", tit);
                filenames[i] = "3D " + tit + " .png";
                save = 0;
            }

            timer1.Stop();
            Functions.ur.Dispose();
            FilenamesToFile();
            ts.Close();
            pds.Close();
            pns.Close();

            toolStripStatusLabel1.Text = $"Запущено построение анимации";
            toolStripStatusLabel2.Text = $"";


            StartProcess("WavesSurface.r", gl, true);
            toolStripStatusLabel1.Text = $"Построены u-surface. Создаётся массив кадров";

            if (radioButton5.Checked)
            {
                OtherMethods.StartProcess2("Truezlims.r");
                   HandScript(tcount);
            }
            else
            StartProcess("ReDraw3Duxt.r", gl, true);

            toolStripStatusLabel1.Text = $"Операции закончены";

            new Scheme(sources, new Point(xmin, ymin), xmax - xmin, ymax - ymin, gl + " (heatmap).png").Show();

            new Anima(filenames).ShowDialog();
        }


        private void FilenamesToFile()
        {
           
           using(StreamWriter fs=new StreamWriter("3D ur, uz(info).txt"))
            {
                var st = filenames.Where(n => (n!=null)&& n.Length > 0).ToArray();
                for (int i = 0; i < st.Length; i++)
                    fs.WriteLine(st[i]);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
          if(source!=null) source.Cancel();
            this.Close();
        }

        public void StartProcess(string fileName,string tit,bool global=false)
        {
            Process process = new Process();
            process.StartInfo.FileName = fileName;
            if (!global)
            {
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }
            
            process.EnableRaisingEvents = true;

            process.Exited += (sender, e) =>
            {
               //toolStripStatusLabel1.Text = "Диалог с 3D закончен полностью";
               if(global)
                    webBrowser1.Navigate(Environment.CurrentDirectory + @"\" + $"{tit} .pdf");
               else
                webBrowser1.Navigate(Environment.CurrentDirectory+@"\"+$"3D ur, uz(title , {tit} ).pdf");
                webBrowser1.Refresh();
            };
            process.Start();

            process.WaitForExit();
            //var inProcess = true;
            //while (inProcess)
            //{
            //    process.Refresh();
            //    if (process.HasExited)
            //    {
            //        inProcess = false;
            //    }
            //}
        }

        public void HandScript(int tc=10)
        {
            using (StreamReader read=new StreamReader("textnames.txt"))
            {
                int k = 1;
                string rs = read.ReadLine();
                while (rs != null)
                {
                    string[] ur, uz;
                    using(StreamReader urs=new StreamReader(rs.Replace(".txt", " (ur).txt")))
                    {
                        ur = urs.ReadLine().Split(' ');
                    }
                    using (StreamReader uzs = new StreamReader(rs.Replace(".txt", " (uz).txt")))
                    {
                        uz = uzs.ReadLine().Split(' ');
                    }
                    using(StreamWriter wr=new StreamWriter("3D ur, uz.txt"))
                    {
                        wr.WriteLine("ur uz");
                        for (int i = 0; i < ur.Length; i++)
                            wr.WriteLine($"{ur[i]} {uz[i]}".Replace('.',','));
                    }
                    using(StreamWriter tt=new StreamWriter("3D ur, uz(title).txt"))
                    {
                        tt.WriteLine(rs.Replace(".txt", ""));
                    }

                    OtherMethods.StartProcess2("3Duxt(better).r");
                    Debug.WriteLine($"Обработано {k++} файлов");

                    toolStripProgressBar1.Value = (((double)k - 1) / tc * toolStripProgressBar1.Maximum).ToInt32();
                    rs = read.ReadLine();
                }
            }

            toolStripProgressBar1.Value = 0;
        }

        public void Times(long speed,int wc,int setk,int sourcec)
        {
            long t = speed * wc * setk * sourcec;
            string s = $"Приблизительное время вычислений первой сетки равно (число источников)х(число точек в сетке)х(число частот)х(время вычисления u(x,w)) = {sourcec}*{setk}*{wc}*{speed} = {t} мс = {((double)t)/60000} мин";

            MessageBox.Show(s, "Это займёт время", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
