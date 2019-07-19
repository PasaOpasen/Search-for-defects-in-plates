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

        /// <summary>
        /// Конструктор
        /// </summary>
        public PS5000ABlockForm(double w0_, double w1_, int l_)
        {
            InitializeComponent();
            w0 = w0_;
            w1 = w1_;
            l = l_;

            comboRangeA.DataSource = System.Enum.GetValues(typeof(Imports.Range));
            comboRangeB.DataSource = System.Enum.GetValues(typeof(Imports.Range));
            comboRangeC.DataSource = System.Enum.GetValues(typeof(Imports.Range));
            comboRangeD.DataSource = System.Enum.GetValues(typeof(Imports.Range));


            toolStripStatusLabel1.Text = "Готов к работе";

            timer1.Interval = 300;
            timer1.Tick += new EventHandler(Timer1_Tick);
        }
        string[] filenames;
        private int all, save = 0;
        private void Timer1_Tick(object Sender, EventArgs e)
        {
            toolStripProgressBar1.Value = (int)(((double)save) / all * toolStripProgressBar1.Maximum);

        }
        private void SetFiles()
        {
            filenames = new string[4];
            filenames[0] = $"f(w) from ({textBox1.Text} , {textBox2.Text}).txt";
            filenames[1] = $"f(w) from ({textBox4.Text} , {textBox3.Text}).txt";
            filenames[2] = $"f(w) from ({textBox6.Text} , {textBox5.Text}).txt";
            filenames[3] = $"f(w) from ({textBox8.Text} , {textBox7.Text}).txt";
        }


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


        private void buttonOpen_Click(object sender, EventArgs e)
        {
        


        n = Convert.ToUInt32(textBox10.Text);
            //    time_scale;
            _timebase = Convert.ToUInt32(textBox9.Text);
            countz = Convert.ToInt32(textBox11.Text);
        dt_ = (double)_timebase / 125000000.0;


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

            Imports.DeviceResolution resolution = Imports.DeviceResolution.PS5000A_DR_14BIT;
            //Imports.DeviceResolution resolution = Imports.DeviceResolution.PS5000A_DR_8BIT;


            if (_handle > 0)
            {
                Imports.CloseUnit(_handle);
                textBoxUnitInfo.Text = "";
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
        }

        void start(uint sampleCount = 50000, int write_every=100)
        {
            {
                uint status;
                int ms;
                status = Imports.MemorySegments(_handle, 4, out ms);

                Voltage_Range = 200;
                status = Imports.SetChannel(_handle, Imports.Channel.ChannelA, 1, Imports.Coupling.PS5000A_DC, Imports.Range.Range_200mV, 0);
                status = Imports.SetChannel(_handle, Imports.Channel.ChannelB, 1, Imports.Coupling.PS5000A_DC, Imports.Range.Range_200mV, 0);
                status = Imports.SetChannel(_handle, Imports.Channel.ChannelC, 1, Imports.Coupling.PS5000A_DC, Imports.Range.Range_200mV, 0);
                status = Imports.SetChannel(_handle, Imports.Channel.ChannelD, 1, Imports.Coupling.PS5000A_DC, Imports.Range.Range_200mV, 0);

                short enable =1;
                uint delay = 0;
                short threshold = 25000;
                short auto = 22222;

                status = Imports.SetBandwidthFilter(_handle, Imports.Channel.ChannelA, Imports.BandwidthLimiter.PS5000A_BW_20MHZ);
                status = Imports.SetBandwidthFilter(_handle, Imports.Channel.ChannelB, Imports.BandwidthLimiter.PS5000A_BW_20MHZ);
                status = Imports.SetBandwidthFilter(_handle, Imports.Channel.ChannelC, Imports.BandwidthLimiter.PS5000A_BW_20MHZ);
                status = Imports.SetBandwidthFilter(_handle, Imports.Channel.ChannelD, Imports.BandwidthLimiter.PS5000A_BW_20MHZ);


                status = Imports.SetSimpleTrigger(_handle, enable, Imports.Channel.External, threshold, Imports.ThresholdDirection.Rising, delay, auto);
                _ready = false;
                _callbackDelegate = BlockCallback;
                _channelCount = 4;
                string data;
                int x;

               // textMessage.Clear();
              //  textData.Clear();

                bool retry;
                
                PinnedArray<short>[] minPinned = new PinnedArray<short>[_channelCount];
                PinnedArray<short>[] maxPinned = new PinnedArray<short>[_channelCount];

                int timeIndisposed;
                short[] minBuffersA = new short[sampleCount];
                short[] maxBuffersA = new short[sampleCount];
                short[] minBuffersB = new short[sampleCount];
                short[] maxBuffersB = new short[sampleCount];
                short[] minBuffersC = new short[sampleCount];
                short[] maxBuffersC = new short[sampleCount];
                short[] minBuffersD = new short[sampleCount];
                short[] maxBuffersD = new short[sampleCount];
                minPinned[0] = new PinnedArray<short>(minBuffersA);
                maxPinned[0] = new PinnedArray<short>(maxBuffersA);
                minPinned[1] = new PinnedArray<short>(minBuffersB);
                maxPinned[1] = new PinnedArray<short>(maxBuffersB);
                minPinned[2] = new PinnedArray<short>(minBuffersC);
                maxPinned[2] = new PinnedArray<short>(maxBuffersC);
                minPinned[3] = new PinnedArray<short>(minBuffersD);
                maxPinned[3] = new PinnedArray<short>(maxBuffersD);
                status = Imports.SetDataBuffers(_handle, Imports.Channel.ChannelA, maxBuffersA, minBuffersA, (int)sampleCount, 0, Imports.RatioMode.None);
                status = Imports.SetDataBuffers(_handle, Imports.Channel.ChannelB, maxBuffersB, minBuffersB, (int)sampleCount, 0, Imports.RatioMode.None);
                status = Imports.SetDataBuffers(_handle, Imports.Channel.ChannelC, maxBuffersC, minBuffersC, (int)sampleCount, 0, Imports.RatioMode.None);
                status = Imports.SetDataBuffers(_handle, Imports.Channel.ChannelD, maxBuffersD, minBuffersD, (int)sampleCount, 0, Imports.RatioMode.None);
                // textMessage.AppendText("BlockData\n");

                /*Find the maximum number of samples and the time interval(in nanoseconds).
                * If the function returns PICO_OK, the timebase will be used.
                */
                int timeInterval;
                int maxSamples;
                while (Imports.GetTimebase(_handle, _timebase, (int)sampleCount, out timeInterval, out maxSamples, 0) != 0)
                {
                    //textMessage.AppendText("Timebase selection\n");
                    _timebase++;

                }
              //  textMessage.AppendText("Timebase Set\n");

                /* Start it collecting, then wait for completion*/
                _ready = false;
                _callbackDelegate = BlockCallback;

                do
                {
                    retry = false;
                    status = Imports.RunBlock(_handle, 0, (int)sampleCount, _timebase, out timeIndisposed, 0, _callbackDelegate, IntPtr.Zero);
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

             //   textMessage.AppendText("Waiting for Data\n");

                while (!_ready)
                {
                    Thread.Sleep(30);
                }

                Imports.Stop(_handle);

                if (_ready)
                {
                    short overflow;
                    status = Imports.GetValues(_handle, 0, ref sampleCount, 1, Imports.RatioMode.None, 0, out overflow);

                    if (status == (short)StatusCodes.PICO_OK)
                    {
                    //    textMessage.AppendText("Have Data\n");
                        for (x = 0; x < sampleCount; x++)
                        {
                            //if (x%write_every==0)
                            //{
                            //data = maxBuffersA[x].ToString();
                            ////textData.AppendText(data+" \n");
                            //}

                            masA[x] += maxBuffersA[x];
                            masB[x] += maxBuffersB[x];
                            masC[x] += maxBuffersC[x];
                            masD[x] += maxBuffersD[x];
                        }


                    }
                    else
                    {
                      //  textMessage.AppendText("No Data\n");

                    }
                }
                else
                {
                   // textMessage.AppendText("data collection aborted\n");
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
        double T0; double OffsetT; uint n=50000; int time_scale;int countz=100;
        double dt_ = 104 * 1.0E-9;
        long[] masA;
        long[] masB;
        long[] masC;
        long[] masD;
        double[] arrA;
        double[] arrB;
        double[] arrC;
        double[] arrD;
        double Voltage_Range = 200; //max po amplitude
        /*
         * 
         * частоты от 0 Гц 10Mhz
         * dt =10^-7
         * df = 100;
         *
         */
        Complex[] Transform_dataA;
        Complex[] Transform_dataB;
        Complex[] Transform_dataC;
        Complex[] Transform_dataD;
        async void CalcTransform(double f0, double f1 , int sc)
        {
            double fl = f1 - f0;
            double df = (f1 - f0) / (sc - 1);
            int count_approx = sc;
            Transform_dataA = new Complex[count_approx];
            Transform_dataB = new Complex[count_approx];
            Transform_dataC = new Complex[count_approx];
            Transform_dataD = new Complex[count_approx];

            all = sc;
            timer1.Start();

            for(int i = 0; i < 100; i++)
            {
                arrA[i] = 0;
                arrB[i] = 0;
                arrC[i] = 0;
                arrD[i] = 0;
            }
            double averA=0, averB=0, averC=0, averD=0;
            for (int i = 100; i < 150; i++)
            {
                averA+=arrA[i];
                averB += arrB[i];
                averC += arrC[i];
                averD += arrD[i];
            }
            averA /= 50;
            averB /= 50;
            averC /= 50;
            averD /= 50;
            for (int i = 0; i < n; i++)
            {
                arrA[i] -= averA;
                arrB[i] -= averB;
                arrC[i] -= averC;
                arrD[i] -= averD;
            }

            //  await Task.Run(() => { 
            for (int j = 0; j<count_approx; j++)
            {
              //  save = j;
                //if ((j % 100) == 0)     textData.AppendText($"{j} / {count_approx}\n");
                for (int j1= 0;j1<n; j1++)
                {
                    Complex buf = new Complex (0,(f0 + df * j) * j1 * dt_*2*Math.PI);
                    Transform_dataA[j] += arrA[j1] * Complex.Exp(buf)* dt_;
                    Transform_dataB[j] += arrB[j1] * Complex.Exp(buf) * dt_;
                    Transform_dataC[j] += arrC[j1] * Complex.Exp(buf) * dt_;
                    Transform_dataD[j] += arrD[j1] * Complex.Exp(buf) * dt_;
                       
                    }
            }
          //  });
            timer1.Stop();
            toolStripStatusLabel1.Text = "Преобразование завершено. Данные записываются в файл";

            //  string filename = filenames[0];//"TransDmitryi.txt";
      //первый 
            using (StreamWriter fs = new StreamWriter(filenames[0]))
            {
                fs.WriteLine("w Re(f(w)) Im(f(w))");
                for (int i = 0; i < count_approx; i++)
                {
                    //if ((i%100 )==0)   textData.AppendText($"{i} / {count_approx}\n");

                    double freq = (f0 + df * i) * 2* Math.PI * 1.0E-6;

                    fs.WriteLine($"{freq} {Transform_dataA[i].Real} {-Transform_dataA[i].Imaginary}");
                }

            }
            string filename2 = "TransA.txt";
            using (StreamWriter fs = new StreamWriter(filename2))
            { 
                for (int i = 0; i < count_approx; i++)
                {
                    //if ((i % 100) == 0) textData.AppendText($"{i} / {count_approx}\n");
                    fs.WriteLine((f0 + df * i).ToString() + " "+ Transform_dataA[i].Magnitude.ToString());
                }
            }

            //второй
            using (StreamWriter fs = new StreamWriter(filenames[1]))
            {
                fs.WriteLine("w Re(f(w)) Im(f(w))");
                for (int i = 0; i < count_approx; i++)
                { 
                    double freq = (f0 + df * i) * 2 * Math.PI * 1.0E-6; 
                    fs.WriteLine($"{freq} {Transform_dataB[i].Real} {-Transform_dataB[i].Imaginary}");

                    //fs.WriteLine($"{freq} {Transform_dataB[i].Real} {-Transform_dataB[i].Imaginary}");
                    //fs.WriteLine($"{freq} {Transform_dataB[i].Real} {Transform_dataB[i].Imaginary}");
                    //fs.WriteLine(" ");
                }

            }
            filename2 = "TransB.txt";
            using (StreamWriter fs = new StreamWriter(filename2))
            {
                for (int i = 0; i < count_approx; i++)
                {
                    fs.WriteLine((f0 + df * i).ToString() + " " + Transform_dataB[i].Magnitude.ToString());
                }
            }
            //третий
            using (StreamWriter fs = new StreamWriter(filenames[2]))
            {
                fs.WriteLine("w Re(f(w)) Im(f(w))");
                for (int i = 0; i < count_approx; i++)
                {
                    double freq = (f0 + df * i) * 2 * Math.PI * 1.0E-6;
                    fs.WriteLine($"{freq} {Transform_dataC[i].Real} {-Transform_dataC[i].Imaginary}");
                }

            }
            filename2 = "TransC.txt";
            using (StreamWriter fs = new StreamWriter(filename2))
            {
                for (int i = 0; i < count_approx; i++)
                {
                    fs.WriteLine((f0 + df * i).ToString() + " " + Transform_dataC[i].Magnitude.ToString());
                }
            }
            //четвертый
            using (StreamWriter fs = new StreamWriter(filenames[3]))
            {
                fs.WriteLine("w Re(f(w)) Im(f(w))");
                for (int i = 0; i < count_approx; i++)
                {
                    double freq = (f0 + df * i) * 2 * Math.PI * 1.0E-6;
                    fs.WriteLine($"{freq} {Transform_dataD[i].Real} {-Transform_dataD[i].Imaginary}");
                }
            }
            filename2 = "TransD.txt";
            using (StreamWriter fs = new StreamWriter(filename2))
            {
                for (int i = 0; i < count_approx; i++)
                {
                    fs.WriteLine((f0 + df * i).ToString() + " " + Transform_dataD[i].Magnitude.ToString());
                }
            }

        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private async void buttonStart_Click(object sender, EventArgs e)
        {

            SetFiles();
            arrA = new double[n];
            arrB = new double[n];
            arrC = new double[n];
            arrD = new double[n];
            masA = new long[n];
            masB = new long[n];
            masC = new long[n];
            masD = new long[n];
            all = countz;
            timer1.Start();
            for(int i=0;i<countz;i++)
            {
               toolStripStatusLabel1.Text=$"Замер {i+1} выполняется";
                save = i;
                await System.Threading.Tasks.Task.Run(() =>start(n,250));
                
            }
            toolStripStatusLabel1.Text = "Замеры выполнены";
            timer1.Stop();

            double middleA = 0;
            double middleB = 0;
            double middleC = 0;
            double middleD = 0;
            for (int i = 0; i < n; i++)
            {

                arrA[i] = ((double)masA[i]) / ((double)countz) / ((double)32767) * Voltage_Range ;
                middleA += arrA[i];
                arrB[i] = ((double)masB[i]) / ((double)countz) / ((double)32767) * Voltage_Range ;
                middleB += arrB[i];
                arrC[i] = ((double)masC[i]) / ((double)countz) / ((double)32767) * Voltage_Range ;
                middleC += arrC[i];
                arrD[i] = ((double)masD[i]) / ((double)countz) / ((double)32767) * Voltage_Range;
                middleD += arrD[i];
            }
            middleA /= (double)n;
            middleB /= (double)n;
            middleC /= (double)n;
            middleD /= (double)n;
            for (int i = 0; i < n; i++)
            {
                arrA[i] -= middleA;
                arrB[i] -= middleB;
                arrC[i] -= middleC;
                arrD[i] -= middleD;
            }

            string filenameA = "ArrayA.txt";
            string filenameB = "ArrayB.txt";
            string filenameC = "ArrayC.txt";
            string filenameD = "ArrayD.txt";
            using (StreamWriter fs=new StreamWriter(filenameA))
            {
                for (int i = 0; i < n; i++)
                    fs.WriteLine(arrA[i].ToString().Replace(',','.'));
            }
            using (StreamWriter fs = new StreamWriter(filenameB))
            {
                for (int i = 0; i < n; i++)
                    fs.WriteLine(arrB[i].ToString().Replace(',', '.'));
            }
            using (StreamWriter fs = new StreamWriter(filenameC))
            {
                for (int i = 0; i < n; i++)
                    fs.WriteLine(arrC[i].ToString().Replace(',', '.'));
            }
            using (StreamWriter fs = new StreamWriter(filenameD))
            {
                for (int i = 0; i < n; i++)
                    fs.WriteLine(arrD[i].ToString().Replace(',', '.'));
            }

            toolStripStatusLabel1.Text = "Запущено преобразование Фурье";
            double dddf =( w1 / 2 / Math.PI * 1.0E6 - w0 / 2 / Math.PI * 1.0E6)/ (double)l;
            CalcTransform(w0/ 2 / Math.PI*1.0E6, w1 / 2 / Math.PI * 1.0E6, l);
            // CalcTransform(5E3, 1.1E5, 2E2);

            buttonOpen_Click(new object(), new EventArgs());
            //Process.Start(filenames[0]);
            //Process.Start(filenames[1]);
            //Process.Start(filenames[2]);
            //Process.Start(filenames[3]);
            this.Close();
        }

    }
}