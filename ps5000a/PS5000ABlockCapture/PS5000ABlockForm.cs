﻿/**************************************************************************
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
using System.IO;
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
        int l;


        #endregion
        bool opened = false;
        int n_ignore = 28200;
        int usred = 1;
        public CSwitchInterface Switch_;
        public int countports = 4;
        /// <summary>
        /// Конструктор
        /// </summary>
        public PS5000ABlockForm(double w0_, double w1_, int l_, int Scount = 8)
        {
            InitializeComponent();
            w0 = w0_;
            w1 = w1_;
            l = l_;
            sourcesCount = Scount;

            //     comboRangeA.DataSource = System.Enum.GetValues(typeof(Imports.Range));


            toolStripStatusLabel1.Text = "Готов к работе";
            toolStripStatusLabel2.Text = "";

            timer1.Interval = 300;
            timer1.Tick += new EventHandler(Timer1_Tick);

            string p = "Замеры";
            Directory.CreateDirectory(p);
            globalbase = Path.Combine(Environment.CurrentDirectory, p);
            textBox12.Text = globalbase;
            SetForlders();
            SetFiles();

            listBox2.Items.Clear();
            for (int i = 0; i < countports; i++)
                listBox2.Items.Add(i);
            listBox2.SelectedIndex = countports / 2;
            checkBox1.Hide();
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



        private void Timer1_Tick(object Sender, EventArgs e)
        {
            //Debug.WriteLine(save);
            if (Label1 != null)
                toolStripStatusLabel1.Text = Label1;
            if (Label2 != null)
                toolStripStatusLabel2.Text = Label2;
            toolStripProgressBar1.Value = (int)(((double)save) / all * toolStripProgressBar1.Maximum);
            this.Refresh();
        }
        private string symbols = "ABCDEFGHIKLMNOPQRSTVXYZ";
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
                ArraysNames[i] = $"Array{symbols[i]}.txt";
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
                    n[i] = $"Замер {symbols[i]}";
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

        private void MakeTime()
        {
            Parallel.For(0, sourcesCount, (int k) => {
            //for (int k = 0; k < sourcesCount; k++)
                using (StreamWriter res = new StreamWriter(Path.Combine(fdiff[k], "time.txt")))
                {
                    for (int i = -countdo; i < countz; i++)                  
                        res.WriteLine(dt_ * i);                  
               }
            });
        }
        private async Task MakeDiffAsync(bool Normalize)
        {
            var ar = Enumerable.Range(0, sourcesCount).ToArray();

            await Task.Run(() =>
            {
                Parallel.For(0, sourcesCount, (int i) =>
                {
                    //  for (int i = 0; i < sourcesCount; i++)
                    //  {
                    var args = ar.Where(n => n != i).ToArray();
                    for (int j = 0; j < sourcesCount - 1; j++)
                    {

                        double max = 0;
                        using (StreamWriter res = new StreamWriter(Path.Combine(fdiff[i], ArraysNames[args[j]])))
                        {
                            using (StreamReader f0 = new StreamReader(Path.Combine(fwithout[i], ArraysNames[args[j]])))
                            using (StreamReader f1 = new StreamReader(Path.Combine(fwith[i], ArraysNames[args[j]])))
                            {

                                string s = f0.ReadLine();
                                while (s != null && s.Length > 0)
                                {
                                    double t = Convert.ToDouble(f1.ReadLine().Replace('.', ',')) - Convert.ToDouble(s.Replace('.', ','));
                                    if (max < t * t) max = t * t;
                                    //res.WriteLine(t);
                                    s = f0.ReadLine();
                                }
                            }
                            max = Math.Sqrt(max);
                            using (StreamReader f0 = new StreamReader(Path.Combine(fwithout[i], ArraysNames[args[j]])))
                            using (StreamReader f1 = new StreamReader(Path.Combine(fwith[i], ArraysNames[args[j]])))
                            {

                                string s = f0.ReadLine();
                                while (s != null && s.Length > 0)
                                {
                                    double t = Convert.ToDouble(f1.ReadLine().Replace('.', ',')) - Convert.ToDouble(s.Replace('.', ','));
                                    if (Normalize)
                                        res.WriteLine(t / max);
                                    else
                                        res.WriteLine(t);
                                    //res.WriteLine(t);
                                    s = f0.ReadLine();
                                }
                            }
                        }
                    }
                    //}
                });
            });
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
        private void button3_Click(object sender, EventArgs e)
        {
            if (opened) buttonOpen_Click(new object(), new EventArgs());
            this.Close();
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            globalbase = textBox12.Text;

            MakeTime();

            await MakeDiffAsync(true);


            for (int i = 0; i < sourcesCount; i++)
                if (checkBox2.Checked && !checkBox3.Checked)
                    Furier(fdiff[i], folderbase[i], i);
                else
            if (checkBox3.Checked)
                    ShowData(fdiff[i], folderbase[i], i);

        }
        #endregion

        /// <summary>
        /// Обратная связб от осциллографа
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
            countz = Convert.ToInt32(textBox10.Text);
            countdo = Convert.ToInt32(textBox13.Text);
            usred = Convert.ToInt32(textBox11.Text);
            dt_ = (double)(_timebase - 3) / 62500000.0; // 16 bit
        }
        private void buttonOpen_Click(object sender, EventArgs e)
        {
            //  n = Convert.ToUInt32(textBox10.Text);
            //    time_scale;
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
        int countz = 100;
        int countdo = 100;

        double dt_ = 104 * 1.0E-9;
        long[] masA;
        string[] ArraysNames;

        double Voltage_Range = 200; //max po amplitude
                                    /*
                                     * 
                                     * частоты от 0 Гц 10Mhz
                                     * dt =10^-7
                                     * df = 100;
                                     *
                                     */

        private string Label2;
        async void CalcTransform(double f0, double f1, int sc, string from, string to, int[] args, int sourcenumber)
        {
            double fl = f1 - f0;
            double df = (f1 - f0) / (sc - 1);
            int count_approx = sc;

            CFurieTransformer FTT = new CFurieTransformer();
            // FTT.LoadCfg("cfg.txt");
            FTT.t_0 = 0;
            FTT.t_n = dt_ * (countz + countdo);

            FTT.count_t = (int)(countz + countdo);
            FTT.dt = dt_;
            //параметры преобразования по частоте

            FTT.f_0 = f0;

            FTT.f_m = f1;

            FTT.count_f = count_approx;
            FTT.df = (FTT.f_m - FTT.f_0) / ((double)FTT.count_f - 1);

            FTT.w_0 = 2 * Math.PI * FTT.f_0;
            FTT.w_m = 2 * Math.PI * FTT.f_m;
            FTT.count_w = FTT.count_f;
            FTT.dw = (FTT.w_m - FTT.w_0) / ((double)FTT.count_w - 1);

            //параметры вычитания постоянной составляющей

            FTT.n_avg = (int)(countz + countdo) - 2;
            //параметры вычитания постоянной составляющей

            FTT.n_ignore = 300;
            //  FTT.LoadIn("ArrayA.txt");

            string[] froms = new string[sourcesCount];
            string[] tosABS = new string[sourcesCount];
            string[] tos = new string[sourcesCount];

            IProgress<int> progress = new Progress<int>((p) => { save = p; });
            all = sc /** (sourcesCount - 1)*sourcesCount*/;
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

                    FTT.LoadIn(froms[i]);
                    //сделать IProgress
                    FTT.GetSplainFT_old(progress);
                    FTT.SaveOut(tos[i]);
                    FTT.SaveOutAbs(tosABS[i]);
                    Label2 = $"Выполнено {i} из {sourcesCount - 1} для источника {symbols[sourcenumber]}";
                }
            });
            Label2 = null;
            timer1.Stop();
            toolStripProgressBar1.Value = 0;
            toolStripStatusLabel1.Text = $"Преобразование для источника {symbols[sourcenumber]} завершено. Данные записаны";
            toolStripStatusLabel2.Text = "";

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
            int dsd = listBox1.SelectedIndex/*textBox14.Text*/;
            Switch_.OpenPort(dsd);

            System.Threading.Thread.Sleep(500);
            textBoxUnitInfo.AppendText(Switch_.GetAccepted() + "\n");
        }

        string[] names_;
        private void button7_Click(object sender, EventArgs e)
        {
            names_ = SerialPort.GetPortNames();
            listBox1.Items.Clear();
            for (int i = 0; i < names_.Length; i++)
            {
                listBox1.Items.Add(names_[i]);
                //textBoxUnitInfo.AppendText(names_[i]);
                //textBoxUnitInfo.AppendText("\n");
            }

            if (listBox1.Items.Count > 0)
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBoxUnitInfo.AppendText(Switch_.GetAccepted() + "\n");
            // System.Threading.Thread.Sleep(500);
            Switch_.SendCmd(0, listBox2.SelectedIndex);
            //System.Threading.Thread.Sleep(500);
            textBoxUnitInfo.AppendText(Switch_.GetAccepted() + "\n");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBoxUnitInfo.AppendText(Switch_.GetAccepted() + "\n");
            //System.Threading.Thread.Sleep(500);
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

        private void RunAvg(ref double[] Array_, int l, int kernel_len = 20)
        {
            double kl = (double)kernel_len;
            double[] Array_buf = new double[l];
            for (int i = kernel_len / 2 + 1; i < l - (kernel_len / 2 + 1); i++)
            {
                double summ = 0;
                for (int j = -kernel_len / 2; j < kernel_len / 2; j++)
                {
                    summ += Array_[i + j];
                }
                summ /= kl;
                Array_buf[i] = summ;
            }
            for (int i = kernel_len / 2 + 1; i < l - (kernel_len / 2 + 1); i++)
            {
                Array_[i] = Array_buf[i];
            }
        }
        private string Label1;
        public async void DataSborCh(int id, string filename_)
        {
            double[] Array = new double[countz + countdo];
            double middleA = 0;
            double mid_ignore = 0;
            double[] arrA = new double[countz + countdo];
            textBoxUnitInfo.AppendText(Switch_.GetAccepted() + "\n");
            Switch_.SendCmd(1, id);
            textBoxUnitInfo.AppendText(Switch_.GetAccepted() + "\n");
            System.Threading.Thread.Sleep(2000);


            all = usred;
            save = 0;
            timer1.Start();
            Stopwatch wr = new Stopwatch();
            wr.Start();
            //          await Task.Run(() => { 
            for (int i = 0; i < usred; i++)//тут надо асинхронно
            {
                Label1 = $"Замер {i + 1} выполняется";

                start((uint)countz, (uint)countdo, 250);
                save = i + 1;
                Timer1_Tick(new object(), new EventArgs());
            }
            //});
            Debug.WriteLine(wr.ElapsedMilliseconds);

            Label1 = null;
            toolStripStatusLabel1.Text = "Замеры выполнены";


            timer1.Stop();
            toolStripProgressBar1.Value = 0;

            middleA = 0;
            mid_ignore = 0;
            for (int i = 0; i < (countz + countdo); i++)
            {
                arrA[i] = ((double)masA[i]) / ((double)usred) / ((double)32767) * Voltage_Range / 2.0;
                middleA += arrA[i];
            }
            middleA /= (double)(countz + countdo);
            for (int i = 0; i < n_ignore; i++)
            {
                Array[i] = 0;
                masA[i] = 0;
            }
            for (int i = n_ignore; i < (countz + countdo); i++)
            {
                Array[i] = arrA[i] - middleA;
                masA[i] = 0;
            }

            RunAvg(ref Array, countz + countdo, 20);


            using (StreamWriter fs = new StreamWriter(filename_))
            {
                for (int i = 0; i < (countz + countdo); i++)
                {
                    fs.WriteLine(Array[i].ToString().Replace(',', '.'));
                }
            }
        }


        public void DataSbor()
        {
            masA = new long[countz + countdo];
            int it = 0;
            int mx = sourcesCount * (sourcesCount - 1);

            for (int i = 0; i < sourcesCount; i++)
            {
                textBoxUnitInfo.AppendText(Switch_.GetAccepted() + "\n");
                Switch_.SendCmd(0, i);
                textBoxUnitInfo.AppendText(Switch_.GetAccepted() + "\n");
                System.Threading.Thread.Sleep(1000);

                for (int j = 0; j < sourcesCount; j++)
                    if (i != j)
                    {
                        DataSborCh(j, Path.Combine(ItFolder(i), ArraysNames[j]));
                        toolStripStatusLabel2.Text = $"Выполнено {it++} из {mx}";
                    }

            }

        }



        private async void buttonStart_Click(object sender, EventArgs e)
        {
            if (!opened)
                buttonOpen_Click(sender, e);
            globalbase = textBox12.Text;
            SetFiles();

            DataSbor();

            for (int i = 0; i < sourcesCount; i++)
                if (checkBox2.Checked && !checkBox3.Checked)
                    Furier(ItFolder(i), null, i);
                else
                           if (checkBox3.Checked)
                    ShowData(ItFolder(i), null, i);

        }

        /// <summary>
        ///------------------------------------------------------------------------
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        private void Furier(string from, string to = null, int number = 0)
        {
            if (to == null)
                to = from;
            InitParams();
            toolStripStatusLabel1.Text = "Запущено преобразование Фурье";

            double dddf = (w1 - w0) / 2 / Math.PI * 1.0E6 / l;

            int[] args = Enumerable.Range(0, sourcesCount).ToArray();

            CalcTransform(w0 / 2.0 / Math.PI * 1.0E6, w1 / 2.0 / Math.PI * 1.0E6, l, from, to, args.Where(n => n != number).ToArray(), number);
        }

        private void ShowData(string from, string to = null, int number = 0)
        {
            if (to == null)
                to = from;

            int counttt = sourcesCount * (sourcesCount - 1);
            string[] st = new string[counttt];
            string[] names = new string[counttt];
            int index = 0;
            for (int i = 0; i < sourcesCount; i++)
                for (int j = 0; j < sourcesCount; j++)
                    if (i != j)
                    {
                        st[index] = "from " + symbols[i] + " to " + symbols[j];
                        names[index] = Path.Combine(from, ArraysNames[j]);
                        index++;
                    }

            toolStripStatusLabel1.Text = "Строится график...";
            this.Refresh();

            string[] sst = new string[sourcesCount - 1], nnames = new string[sourcesCount - 1];
            for (int i = 0; i < (sourcesCount - 1); i++)
            {
                sst[i] = st[number * (sourcesCount - 1) + i];
                nnames[i] = names[number * (sourcesCount - 1) + i];
            }


            var form = new JustGrafic(sst, nnames,$"График от {symbols[number]}");

            void cmet(object sender, FormClosedEventArgs e)
            {
                if (checkBox2.Checked)
                    Furier(from, to, number);
            }

            form.FormClosed += new FormClosedEventHandler(cmet);
            //form.FormClosed += new FormClosedEventHandler(cmet);
            form.Show();

        }
    }
}