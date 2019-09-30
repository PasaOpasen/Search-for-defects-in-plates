/**************************************************************************
 *
 * Filename: PS5000ABlockForm.cs
 * 
 * Description:
 *   This is a GUI-based program that demonstrates how to use the
 *   PicoScope 5000 Series (ps5000a) driver API functions using .NET
 *   in order to collect a block of data.
 *
 * Supported PicoScope models:
 *
 *		PicoScope 5242A/B & 5442A/B
 *		PicoScope 5243A/B & 5443A/B
 *		PicoScope 5244A/B & 5444A/B
 * 
 * Examples:
 *    Collect a block of samples immediately
 *    Collect a block of samples when a trigger event occurs
 *    Collect a block using ETS
 *    Collect a stream of data immediately
 *    Collect a stream of data when a trigger event occurs
 *    Set Signal Generator, using built in or custom signals
 *    
 * Copyright (C) 2013 - 2017 Pico Technology Ltd. See LICENSE file for terms.   
 *    
 **************************************************************************/

using System;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using PS5000AImports;
using PicoPinnedArray;
using PicoStatus;
using System.IO;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;
using Библиотека_графики;
using System.Linq;
using System.IO.Ports;
using МатКлассы;

namespace PS5000A
{
    public partial class PS5000ABlockForm : Form
    {

        #region Поля
        private short _handle;
        public const int BUFFER_SIZE = 1024;
        public const int MAX_CHANNELS = 4;
        public const int QUAD_SCOPE = 4;
        public const int DUAL_SCOPE = 2;


        uint _timebase = 15;
        short _oversample = 1;
        bool _scaleVoltages = true;

        ushort[] inputRanges = { 10, 20, 50, 100, 200, 500, 1000, 2000, 5000, 10000, 20000, 50000 };
        bool _ready = false;
        short _trig = 0;
        uint _trigAt = 0;
        int _sampleCount = 0;
        uint _startIndex = 0;
        bool _autoStop;
        //private ChannelSettings[] _channelSettings;
        private int _channelCount;
        private Imports.Range _firstRange;
        private Imports.Range _lastRange;
        private int _digitalPorts;
        private Imports.ps5000aBlockReady _callbackDelegate;
        private string StreamFile = "stream.txt";
        private string BlockFile = "block.txt";
        double w0;
        double w1;
        int wcount;


        #endregion
        bool opened = false;
        int n_ignore = 28200;
        int usred = 1;
        public CSwitchInterface Switch_;
        public readonly int countPorts = 4;
        /// <summary>
        /// Конструктор
        /// </summary>
        public PS5000ABlockForm(double w0_, double w1_, int l_, int Scount = 8)
        {
            InitializeComponent();
            w0 = w0_;
            w1 = w1_;
            wcount = l_;
            sourcesCount = Scount;

            //     comboRangeA.DataSource = System.Enum.GetValues(typeof(Imports.Range));
            toolStripStatusLabel1.Text = "Готов к работе";
            toolStripStatusLabel2.Text = "";

            timer1.Interval = 300;
            timer1.Tick += new EventHandler(Timer1_Tick);

            SetDirects();
            SetParams();

            this.FormClosing += (object o, FormClosingEventArgs aa) =>
            {
                GetParams();
            };

            this.FormClosed += new FormClosedEventHandler((object o, FormClosedEventArgs a) =>
            {
                FurierTransformer.Dispose();
            });

            Hides();
        }

        #region Димас писал

        int sourcesCount;
        string[] filenames;
        private int all, save = 0;
        private string globalbase;
        private string[] folderbase, fwith, fwithout, fdiff;
        /// <summary>
        /// Текущая папка, куда будут сохраняться результаты замера
        /// </summary>
        private string ItFolder(int number)
        {
            if (radioButton1.Checked)
                return fwithout[number];
            else return fwith[number];

        }
        private void SetDirects()
        {
            string p;
            if (File.Exists("LastDirectory.txt") && Expendator.IfDirectoryExists("LastDirectory.txt", out p))
            {
                globalbase = p;
            }
            else
            {
                p = "Замеры";
                Directory.CreateDirectory(p);
                globalbase = Path.Combine(Environment.CurrentDirectory, p);
            }

            textBox12.Text = globalbase;
            SetForlders();
            SetFiles();

            listBox2.Items.Clear();
            for (int i = 0; i < countPorts; i++)
                listBox2.Items.Add(i);
            listBox2.SelectedIndex = countPorts / 2;
        }
        /// <summary>
        /// Число связей между всеми парами источников
        /// </summary>
        private int CountOfEdges => sourcesCount * (sourcesCount - 1);
        private int CountSum => countBefore + countAfter;

        private void SetParams()
        {
            string GetNumberString(string s) => s.Split(' ')[1];

            if (File.Exists("FirstWindowConfig.txt"))
                using (StreamReader f = new StreamReader("FirstWindowConfig.txt"))
                {
                    textBox9.Text = GetNumberString(f.ReadLine());
                    textBox14.Text = GetNumberString(f.ReadLine());
                    textBox13.Text = GetNumberString(f.ReadLine());
                    textBox10.Text = GetNumberString(f.ReadLine());
                    textBox11.Text = GetNumberString(f.ReadLine());
                }
        }
        private void GetParams()
        {
            using (StreamWriter f = new StreamWriter("FirstWindowConfig.txt"))
            {
                f.WriteLine($"timebase= {textBox9.Text}");
                f.WriteLine($"counttrig= {textBox14.Text}");
                f.WriteLine($"countbefore= {textBox13.Text}");
                f.WriteLine($"countafter= {textBox10.Text}");
                f.WriteLine($"countmeans= {textBox11.Text}");
            }
        }

        private void Timer1_Tick(object Sender, EventArgs e)
        {
            if (label1String != null)
                toolStripStatusLabel1.Text = label1String;
            if (label2String != null)
                toolStripStatusLabel2.Text = label2String;
            toolStripProgressBar1.Value = (int)(((double)save) / all * toolStripProgressBar1.Maximum);
            this.Refresh();
        }
        private string Symbols = "ABCDEFGHIKLMNOPQRSTVXYZ";
        private void SetFiles()
        {
            filenames = new string[sourcesCount];
            ArraysNames = new string[sourcesCount];

            filenames[0] = $"f(w) from ({textBox1.Text} , {textBox2.Text}).txt";
            filenames[1] = $"f(w) from ({textBox4.Text} , {textBox3.Text}).txt";
            filenames[2] = $"f(w) from ({textBox6.Text} , {textBox5.Text}).txt";
            filenames[3] = $"f(w) from ({textBox8.Text} , {textBox7.Text}).txt";
            filenames[4] = $"f(w) from ({textBox16.Text} , {textBox18.Text}).txt";
            filenames[5] = $"f(w) from ({textBox20.Text} , {textBox22.Text}).txt";
            filenames[6] = $"f(w) from ({textBox21.Text} , {textBox19.Text}).txt";
            filenames[7] = $"f(w) from ({textBox17.Text} , {textBox15.Text}).txt";

            for (int i = 0; i < sourcesCount; i++)
                ArraysNames[i] = $"Array{Symbols[i]}.txt";
        }
        private void SetForlders()
        {
            string[] n = new string[sourcesCount];
            fwith = new string[sourcesCount];
            fwithout = new string[sourcesCount];
            fdiff = new string[sourcesCount];
            folderbase = new string[sourcesCount];

            using (StreamWriter fs = new StreamWriter("WhereData.txt"))
                for (int i = 0; i < n.Length; i++)
                {
                    n[i] = $"Замер {Symbols[i]}";
                    folderbase[i] = Path.Combine(globalbase, n[i]);

                    fwith[i] = Path.Combine(folderbase[i], "C дефектом");
                    fwithout[i] = Path.Combine(folderbase[i], "Без дефекта");
                    fdiff[i] = Path.Combine(folderbase[i], "Разница");
                    Directory.CreateDirectory(fwith[i]);
                    Directory.CreateDirectory(fwithout[i]);
                    Directory.CreateDirectory(fdiff[i]);

                    fs.WriteLine(folderbase[i]);
                }
        }

        private async Task MakeTimeAsync()
        {
            await Task.Run(() =>
            Parallel.For(0, sourcesCount, (int k) =>
            {
                using (StreamWriter res = new StreamWriter(Path.Combine(fdiff[k], "time.txt")))
                    for (int i = -countBefore; i < countAfter; i++)
                        res.WriteLine(dt * i);
            })
            );
        }
        private async Task MakeDiffAsync(bool Normalize)
        {
            var ar = Enumerable.Range(0, sourcesCount).ToArray();

            all = CountOfEdges;
            int[] tmp = new int[all];
            save = 0;
            timer1.Start();
            await Task.Run(() =>
            {
                Parallel.For(0, sourcesCount, (int i) =>
                {
                    var args = ar.Where(n => n != i).ToArray();
                    for (int j = 0; j < sourcesCount - 1; j++)
                    {
                        double max;
                        if (Normalize)
                        {
                            max = 0;
                            using (StreamReader f0 = new StreamReader(Path.Combine(fwithout[i], ArraysNames[args[j]])))
                            using (StreamReader f1 = new StreamReader(Path.Combine(fwith[i], ArraysNames[args[j]])))
                            {
                                string s = f0.ReadLine();
                                while (s != null && s.Length > 0)
                                {
                                    double t = Convert.ToDouble(f1.ReadLine().Replace('.',',')) - Convert.ToDouble(s.Replace('.', ','));
                                    if (max < t * t) max = t * t;
                                    s = f0.ReadLine();
                                }
                            }
                            max = Math.Sqrt(max);
                        }
                        else
                            max = 1.0;

                        using (StreamWriter res = new StreamWriter(Path.Combine(fdiff[i], ArraysNames[args[j]])))
                        {
                            using (StreamReader f0 = new StreamReader(Path.Combine(fwithout[i], ArraysNames[args[j]])))
                            using (StreamReader f1 = new StreamReader(Path.Combine(fwith[i], ArraysNames[args[j]])))
                            {
                                string s = f0.ReadLine();
                                while (s != null && s.Length > 0)
                                {
                                    double t = Convert.ToDouble(f1.ReadLine().Replace('.', ',')) - Convert.ToDouble(s.Replace('.', ','));
                                    res.WriteLine(t / max);
                                    s = f0.ReadLine();
                                }
                            }
                        }

                        tmp[i * (sourcesCount - 1) + j]++;
                        save = tmp.Sum();
                    }
                });
            });
            save = 0;
            Timer1_Tick(new object(), new EventArgs());
            timer1.Stop();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox12.Text = folderBrowserDialog1.SelectedPath;
                globalbase = textBox12.Text;
                SetForlders();
            }
        }

        private bool SetGlobalBase()
        {
            if (Directory.Exists(textBox12.Text))
            {
                globalbase = textBox12.Text;
                Expendator.WriteStringInFile("LastDirectory.txt", globalbase);
            }
            else
            {
                MessageBox.Show("Указанной директории не существует!", "Ошибка в пути", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            return true;
        }

        private void Hides()
        {
            groupBox3.Hide();
            groupBox4.Hide();
            button6.Hide();
            listBox1.Hide();
            //label19.Hide();
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            InitParams();
            if (!SetGlobalBase())
                return;

            await MakeTimeAsync();

            toolStripStatusLabel1.Text = "Создаётся разность для каждого замера";
            await MakeDiffAsync(Normalize: true);
            new System.Media.SoundPlayer(Properties.Resources.РазницаГотова).Play();

            await FurierOrShowForm(i => fdiff[i], i => folderbase[i]);
            SygnalOfEndCalc();

            await Task.Run(() => Thread.Sleep(300));

            this.Close();
        }
        #endregion

        /// <summary>
        /// Обратная связь от осциллографа
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="status"></param>
        /// <param name="pVoid"></param>
        private void BlockCallback(short handle, short status, IntPtr pVoid)
        {
            // flag to say done reading data
            if (status != (short)StatusCodes.PICO_CANCELLED)
                _ready = true;
        }


        private uint SetTrigger(Imports.TriggerChannelProperties[] channelProperties,
            short nChannelProperties,
            Imports.TriggerConditions[] triggerConditions,
            short nTriggerConditions,
            Imports.ThresholdDirection[] directions,
            uint delay,
            short auxOutputEnabled,
            int autoTriggerMs)
        {
            uint status;

            status = Imports.SetTriggerChannelProperties(_handle, channelProperties, nChannelProperties, auxOutputEnabled,
                                                   autoTriggerMs);
            if (status != StatusCodes.PICO_OK)
            {
                return status;
            }

            status = Imports.SetTriggerChannelConditions(_handle, triggerConditions, nTriggerConditions);

            if (status != StatusCodes.PICO_OK)
            {
                return status;
            }

            if (directions == null)
            {
                directions = new Imports.ThresholdDirection[] { Imports.ThresholdDirection.None,
                                Imports.ThresholdDirection.None, Imports.ThresholdDirection.None, Imports.ThresholdDirection.None,
                                Imports.ThresholdDirection.None, Imports.ThresholdDirection.None};
            }

            status = Imports.SetTriggerChannelDirections(_handle,
                                                               directions[(int)Imports.Channel.ChannelA],
                                                               directions[(int)Imports.Channel.ChannelB],
                                                               directions[(int)Imports.Channel.ChannelC],
                                                               directions[(int)Imports.Channel.ChannelD],
                                                               directions[(int)Imports.Channel.External],
                                                               directions[(int)Imports.Channel.Aux]);
            if (status != StatusCodes.PICO_OK)
            {
                return status;
            }

            status = Imports.SetTriggerDelay(_handle, delay);

            if (status != StatusCodes.PICO_OK)
            {
                return status;
            }

            return status;
        }

        private void InitParams()
        {
            n_ignore = Convert.ToInt32(textBox14.Text);
            _timebase = Convert.ToUInt32(textBox9.Text);
            countAfter = Convert.ToInt32(textBox10.Text);
            countBefore = Convert.ToInt32(textBox13.Text);
            usred = Convert.ToInt32(textBox11.Text);
            dt = (_timebase - 3) / 62_500_000.0; // 16 bit
        }
        private void buttonOpen_Click(object sender, EventArgs e)
        {
            InitParams();

            StringBuilder UnitInfo = new StringBuilder(80);

            short handle;

            string[] description = {
                           "Driver Version    ",
                           "USB Version       ",
                           "Hardware Version  ",
                           "Variant Info      ",
                           "Serial            ",
                           "Cal Date          ",
                           "Kernel Ver        ",
                           "Digital Hardware  ",
                           "Analogue Hardware "
                         };

            Imports.DeviceResolution resolution = Imports.DeviceResolution.PS5000A_DR_16BIT;
            //Imports.DeviceResolution resolution = Imports.DeviceResolution.PS5000A_DR_8BIT;


            if (_handle > 0)
            {
                Imports.CloseUnit(_handle);
                //   textBoxUnitInfo.Text = "";
                _handle = 0;
                buttonOpen.Text = "Open";
            }
            else
            {
                uint status = Imports.OpenUnit(out handle, null, resolution);

                if (handle > 0)
                {
                    _handle = handle;

                    if (status == StatusCodes.PICO_POWER_SUPPLY_NOT_CONNECTED || status == StatusCodes.PICO_USB3_0_DEVICE_NON_USB3_0_PORT)
                    {
                        status = Imports.ChangePowerSource(_handle, status);
                    }
                    else if (status != StatusCodes.PICO_OK)
                    {
                        MessageBox.Show("Cannot open device error code: " + status.ToString(), "Error Opening Device", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                    }
                    else
                    {
                        // Do nothing - power supply connected
                    }

                    textBoxUnitInfo.Text = "Handle            " + _handle.ToString() + "\r\n";

                    for (int i = 0; i < 9; i++)
                    {
                        short requiredSize;
                        Imports.GetUnitInfo(_handle, UnitInfo, 80, out requiredSize, (uint)i);
                        textBoxUnitInfo.AppendText(description[i] + UnitInfo + "\r\n");
                    }
                    buttonOpen.Text = "Закрыть";
                }
            }
            opened = true;

            buttonOpen.Text = "Запущено";
            groupBox4.Show();
        }

        void start(uint sampleCountAfter = 50000, uint sampleCountBefore = 50000, int write_every = 100)
        {
            {
                uint all_ = sampleCountAfter + sampleCountBefore;
                uint status;
                int ms;
                status = Imports.MemorySegments(_handle, 1, out ms);

                Voltage_Range = 200;
                status = Imports.SetChannel(_handle, Imports.Channel.ChannelA, 1, Imports.Coupling.PS5000A_AC, Imports.Range.Range_200mV, 0);
                //status = Imports.SetChannel(_handle, Imports.Channel.ChannelA, 1, Imports.Coupling.PS5000A_DC, Imports.Range.Range_200mV, 0);

                short enable = 1;
                uint delay = 0;
                short threshold = 25000;
                short auto = 22222;

                status = Imports.SetBandwidthFilter(_handle, Imports.Channel.ChannelA, Imports.BandwidthLimiter.PS5000A_BW_20MHZ);
                status = Imports.SetSimpleTrigger(_handle, enable, Imports.Channel.External, threshold, Imports.ThresholdDirection.Rising, delay, auto);
                _ready = false;
                _callbackDelegate = BlockCallback;
                _channelCount = 1;
                //string data;
                int x;


                bool retry;

                PinnedArray<short>[] minPinned = new PinnedArray<short>[_channelCount];
                PinnedArray<short>[] maxPinned = new PinnedArray<short>[_channelCount];

                int timeIndisposed;
                short[] minBuffersA = new short[all_];
                short[] maxBuffersA = new short[all_];
                minPinned[0] = new PinnedArray<short>(minBuffersA);
                maxPinned[0] = new PinnedArray<short>(maxBuffersA);
                status = Imports.SetDataBuffers(_handle, Imports.Channel.ChannelA, maxBuffersA, minBuffersA, (int)sampleCountAfter + (int)sampleCountBefore, 0, Imports.RatioMode.None);

                //int timeInterval;
                //int maxSamples;
                //while (Imports.GetTimebase(_handle, _timebase, (int)sampleCount, out timeInterval, out maxSamples, 0) != 0)
                //{
                //    _timebase++;
                //}
                _ready = false;
                _callbackDelegate = BlockCallback;
                do
                {
                    retry = false;
                    status = Imports.RunBlock(_handle, (int)sampleCountBefore, (int)sampleCountAfter, _timebase, out timeIndisposed, 0, _callbackDelegate, IntPtr.Zero);
                    if (status == (short)StatusCodes.PICO_POWER_SUPPLY_CONNECTED || status == (short)StatusCodes.PICO_POWER_SUPPLY_NOT_CONNECTED || status == (short)StatusCodes.PICO_POWER_SUPPLY_UNDERVOLTAGE)
                    {
                        status = Imports.ChangePowerSource(_handle, status);
                        retry = true;
                    }
                    else
                    {
                        //  textMessage.AppendText("Run Block Called\n");
                    }
                }
                while (retry);
                while (!_ready)
                {
                    Thread.Sleep(30);
                }
                Imports.Stop(_handle);
                if (_ready)
                {
                    short overflow;
                    status = Imports.GetValues(_handle, 0, ref all_, 1, Imports.RatioMode.None, 0, out overflow);

                    if (status == (short)StatusCodes.PICO_OK)
                    {
                        for (x = 0; x < all_; x++)
                            masA[x] += maxBuffersA[x] + minBuffersA[x];//=========================================================!
                    }

                }

                Imports.Stop(_handle);
                foreach (PinnedArray<short> p in minPinned)
                {
                    if (p != null)
                        p.Dispose();
                }
                foreach (PinnedArray<short> p in maxPinned)
                {
                    if (p != null)
                        p.Dispose();
                }
            }
        }
        int countAfter = 100;
        int countBefore = 100;

        double dt = 104 * 1.0E-9;
        long[] masA;
        string[] ArraysNames;

        /// <summary>
        /// Maxinum of an amplitude
        /// </summary>
        double Voltage_Range = 200;

        private string label2String;
        private void CreateFurierTransform(double f0, double f1, int sc)
        {
            FurierTransformer.w_0 = f0 * 1e6;
            FurierTransformer.w_m = f1 * 1e6;
            double Freq(double w) => w / 2.0 / Math.PI * 1.0E6;

            f0 = Freq(f0); f1 = Freq(f1);
            int count_approx = sc;

            FurierTransformer.t_0 = 0;
            FurierTransformer.count_t = (CountSum);
            FurierTransformer.t_n = dt * FurierTransformer.count_t;
            FurierTransformer.dt = dt;
            FurierTransformer.f_0 = f0;
            FurierTransformer.f_m = f1;
            FurierTransformer.count_f = count_approx;
            FurierTransformer.count_w = count_approx;
            FurierTransformer.df = (f1 - f0) / (sc - 1);
            FurierTransformer.dw = (FurierTransformer.w_m - FurierTransformer.w_0) / ((double)FurierTransformer.count_w - 1);

            FurierTransformer.n_avg = (CountSum) - 2;
            FurierTransformer.n_ignore = n_ignore;

            FurierTransformer.CreateNewGen();
        }
        async Task CalcTransformAsync(int sc, string from, string to, int[] args, int sourcenumber)
        {
            string[] froms = new string[sourcesCount];
            string[] tosABS = new string[sourcesCount];
            string[] tos = new string[sourcesCount];

            IProgress<int> progress = new Progress<int>((p) => { save = p; });
            all = sc;
            toolStripStatusLabel2.Text = "";
            timer1.Start();

            await Task.Run(() =>
            {
                for (int i = 0; i < tos.Length - 1; i++)
                {
                    save = 0;
                    froms[i] = Path.Combine(from, ArraysNames[args[i]]);
                    tosABS[i] = Path.Combine(to, "Abs_" + filenames[args[i]]);
                    tos[i] = Path.Combine(to, filenames[args[i]]);

                    MakeTransform(froms[i], tos[i], progress);
                    label2String = $"Выполнено {i + 1} из {sourcesCount - 1} для источника {Symbols[sourcenumber]}";
                }
            });
            label2String = null;
            timer1.Stop();
            toolStripProgressBar1.Value = 0;
            toolStripStatusLabel1.Text = $"Преобразование для источника {Symbols[sourcenumber]} завершено. Данные записаны";
            toolStripStatusLabel2.Text = "";

        }

        private void MakeTransform(string from, string to, IProgress<int> progress)
        {
            FurierTransformer.LoadIn(from);
            //FurierTransformer.GetSplainFT_old(progress);
            FurierTransformer.GetSplainFT_new(progress);
            FurierTransformer.SaveOut(to);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void PS5000ABlockForm_Load(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            Switch_ = new CSwitchInterface();
            int dsd = listBox1.SelectedIndex;
            Switch_.OpenPort(dsd);

            Thread.Sleep(500);
            textBoxUnitInfo.AppendText(Switch_.GetAccepted() + "\n");

            groupBox3.Show();
            checkBox2.Checked = false;
            checkBox3.Checked = false;
        }

        string[] names_;
        private void button7_Click(object sender, EventArgs e)
        {
            names_ = SerialPort.GetPortNames();
            listBox1.Items.Clear();
            for (int i = 0; i < names_.Length; i++)
                listBox1.Items.Add(names_[i]);

            if (listBox1.Items.Count > 0)
                listBox1.SelectedIndex = listBox1.Items.Count - 1;

            listBox1.Show();
            //label19.Show();
            button6.Show();
        }

        //private void label19_Click(object sender, EventArgs e)
        //{

        //}

        private void textBox14_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBoxUnitInfo.AppendText(Switch_.GetAccepted() + "\n");
            // Thread.Sleep(500);
            Switch_.SendCmd(0, listBox2.SelectedIndex);
            //Thread.Sleep(500);
            textBoxUnitInfo.AppendText(Switch_.GetAccepted() + "\n");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBoxUnitInfo.AppendText(Switch_.GetAccepted() + "\n");
            //Thread.Sleep(500);
            Switch_.SendCmd(1, listBox2.SelectedIndex);
            textBoxUnitInfo.AppendText(Switch_.GetAccepted() + "\n");
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void RunAvg(ref double[] Array_, int wcount, int kernel_len = 20)
        {
            double kl = kernel_len;
            double[] Array_buf = new double[wcount];
            for (int i = kernel_len / 2 + 1; i < wcount - (kernel_len / 2 + 1); i++)
            {
                double summ = 0;
                for (int j = -kernel_len / 2; j < kernel_len / 2; j++)
                {
                    summ += Array_[i + j];
                }
                Array_buf[i] = summ / kl;
            }
            for (int i = kernel_len / 2 + 1; i < wcount - (kernel_len / 2 + 1); i++)
            {
                Array_[i] = Array_buf[i];
            }
        }
        private string label1String;
        public async Task GetDataMiniAsync(int id, string filename_)
        {
            int countSum = CountSum;
            double[] Array = new double[countSum];
            double[] arrA = new double[countSum];

            textBoxUnitInfo.AppendText(Switch_.GetAccepted() + "\n");
            Switch_.SendCmd(1, id);
            textBoxUnitInfo.AppendText(Switch_.GetAccepted() + "\n");
            Application.DoEvents();
            Thread.Sleep(2000);

            all = usred;
            save = 0;
            timer1.Start();
            //          await Task.Run(() => { 
            for (int i = 0; i < usred; i++)//тут надо асинхронно
            {
                label1String = $"Замер {i + 1} выполняется";
                start((uint)countAfter, (uint)countBefore, 250);
                save = i + 1;
                Timer1_Tick(new object(), new EventArgs());
                Application.DoEvents();
                Thread.Sleep(1);
            }
            //});

            label1String = null;
            toolStripStatusLabel1.Text = "Замеры выполнены";
            timer1.Stop();
            toolStripProgressBar1.Value = 0;

            await Task.Run(() => {

                double middleA = 0;
                double coef = Voltage_Range / ((double)usred) / 32767 / 2.0;
                for (int i = 0; i < countSum; i++)
                {
                    arrA[i] = masA[i] * coef;
                    middleA += arrA[i];
                }
                middleA /= countSum;
                for (int i = 0; i < n_ignore; i++)
                {
                    Array[i] = 0;
                    masA[i] = 0;
                }
                for (int i = n_ignore; i < countSum; i++)
                {
                    Array[i] = arrA[i] - middleA;
                    masA[i] = 0;
                }

                RunAvg(ref Array, countSum, 20);

                using (StreamWriter fs = new StreamWriter(filename_))
                    for (int i = 0; i < countSum; i++)
//                        fs.WriteLine(Array[i]);
                        fs.WriteLine(Array[i].ToString().Replace(',','.')); //тут изменил

            });
        }


        public async Task GetDataAsync()
        {
            masA = new long[CountSum];
            int it = 0;
            int mx = CountOfEdges;

            for (int i = 0; i < sourcesCount; i++)
            {
                textBoxUnitInfo.AppendText(Switch_.GetAccepted() + "\n");
                Switch_.SendCmd(0, i);
                textBoxUnitInfo.AppendText(Switch_.GetAccepted() + "\n");
                Thread.Sleep(2000);

                for (int j = 0; j < sourcesCount; j++)
                    if (i != j)
                    {
                        await GetDataMiniAsync(j, Path.Combine(ItFolder(i), ArraysNames[j]));
                        toolStripStatusLabel2.Text = $"Выполнено {++it} из {mx} ({Expendator.GetProcent(it, mx/*,2*/)}%)";
                    }
                toolStripStatusLabel2.Text = "";

                FurierOrShowIteration(ItFolder(i), null, i);
            }

        }

        private async void buttonStart_Click(object sender, EventArgs e)
        {
            InitParams();
            CreateFurierTransform(w0, w1, wcount);
            if (!SetGlobalBase())
                return;

            SetFiles();

            await GetDataAsync();

            new System.Media.SoundPlayer(Properties.Resources.ЗамерыСделаны).Play();

            //await FurierOrShowForm(ItFolder, (_) => null);
            SygnalOfEndCalc();
        }

        /// <summary>
        /// Запустить циклом преобразования Фурье и/или формы
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private async Task FurierOrShowForm(Func<int, string> from, Func<int, string> to)
        {
            CreateFurierTransform(w0, w1, wcount);

            for (int i = 0; i < sourcesCount; i++)
                await FurierOrShowIteration(from(i), to(i), i);
        }
        private async Task FurierOrShowIteration(string from, string to, int number)
        {
            if (checkBox2.Checked && !checkBox3.Checked)
                await FurierAsync(from, to, number);
            else
            if (checkBox3.Checked)
                await ShowDataAsync(from, to, number);
        }

        /// <summary>
        /// Сигнализирует о завершении всех операций
        /// </summary>
        private void SygnalOfEndCalc()
        {
            toolStripStatusLabel1.Text = $"Все вычисления завершены";
            new System.Media.SoundPlayer(Properties.Resources.ВычисленияЗавершены).Play();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new Библиотека_графики.PdfOpen("Алгоритм работы", Expendator.GetResource("algolIlusha.pdf", "PS5000ABlockCapture")).Show();
        }

        /// <summary>
        /// Запускает преобразование Фурье
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        private async Task FurierAsync(string from, string to = null, int number = 0)
        {
            to = to ?? from;
            toolStripStatusLabel1.Text = $"Запущено преобразование Фурье для источника {Symbols[number]}";

            await CalcTransformAsync(wcount, from, to, Enumerable.Range(0, sourcesCount).Where(n => n != number).ToArray(), number);
            new System.Media.SoundPlayer(Properties.Resources.Преобразование_готово).Play();
        }
        /// <summary>
        /// Запускает форму с графиками и возможным запуском преобразования Фурье при закрытии
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        private Task ShowDataAsync(string from, string to = null, int number = 0)
        {
            to = to ?? from;

            var arrays = GetSTandNAMESforGrafic(from, number);

            toolStripStatusLabel1.Text = "Строится график...";
            var tcs = new TaskCompletionSource<bool>();
            var form = new JustGrafic(arrays.Item1, arrays.Item2, $"График от {Symbols[number]}", dt, countBefore);

            form.FormClosed += async (object sender, FormClosedEventArgs e) =>
            {
                if (checkBox2.Checked)
                    await FurierAsync(from, to, number);

                tcs.SetResult(true);
            };
            form.Show();
            return tcs.Task;
        }

        private Tuple<string[], string[]> GetSTandNAMES(string from)
        {
            int counttt = CountOfEdges;
            string[] st = new string[counttt];
            string[] names = new string[counttt];
            int index = 0;
            for (int i = 0; i < sourcesCount; i++)
                for (int j = 0; j < sourcesCount; j++)
                    if (i != j)
                    {
                        st[index] = "from " + Symbols[i] + " to " + Symbols[j];
                        names[index] = Path.Combine(from, ArraysNames[j]);
                        index++;
                    }
            return new Tuple<string[], string[]>(st, names);
        }

        private Tuple<string[], string[]> GetSTandNAMESforGrafic(string from, int number)
        {
            var stnames = GetSTandNAMES(from);
            string[] st = stnames.Item1;
            string[] names = stnames.Item2;

            string[] sst = new string[sourcesCount - 1], nnames = new string[sourcesCount - 1];
            for (int i = 0; i < (sourcesCount - 1); i++)
            {
                sst[i] = st[number * (sourcesCount - 1) + i];
                nnames[i] = names[number * (sourcesCount - 1) + i];
            }

            return new Tuple<string[], string[]>(sst, nnames);
        }
    }
}