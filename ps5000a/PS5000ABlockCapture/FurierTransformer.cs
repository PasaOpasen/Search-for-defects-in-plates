using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Globalization;
using System.Threading;
using System.IO.Ports;
using System.IO;
using System.Numerics;
using System.Diagnostics;
using МатКлассы;

namespace PS5000A
{
    public static class FurierTransformer
    {
        static readonly double _2PI = 2 * Math.PI;
        static string InFile;
        static string CfgFile;
        public static double dt;
        public static double t_0;
        public static double t_n;
        public static int count_t;
        public static double f_0;
        public static double f_m;
        public static int count_f;
        public static double df;
        public static double w_0;
        public static double w_m;
        public static int count_w;
        public static double dw;
        public static int n_avg;
        public static int n_ignore;
        static double avg;
        static double[] f;
        static Complex[] F;

        public static void FilterData(int n = 2)
        {
            double[] f_ = new double[count_t - 2 * n];
            for (int i = 0; i < count_t - 2 * n; i++)
            {
                f_[i] = 0;
                for (int j = 0; j < 2 * n + 1; j++)
                    f_[i] += f[i + j];
                f_[i] /= n * 2 + 1;
                f[i] = f_[i];
            }
        }

        public static void LoadCfg(string filename)
        {
            CfgFile = filename;
            using (StreamReader sr = new StreamReader(CfgFile, System.Text.Encoding.Default))
            {
                int IntGet() => Int32.Parse(sr.ReadLine());
                double DoubleGet() => Double.Parse(sr.ReadLine());

                //параметры сигнала по времени
                t_0 = DoubleGet();
                t_n = DoubleGet();
                count_t = IntGet();
                dt = (t_n - t_0) / (count_t - 1);
                //параметры преобразования по частоте
                f_0 = DoubleGet();
                f_m = DoubleGet();
                count_f = IntGet();
                df = (f_m - f_0) / (count_f - 1);

                w_0 = _2PI * f_0;
                w_m = _2PI * f_m;
                count_w = count_f;
                dw = (w_m - w_0) / (double)(count_w - 1);

                //параметры вычитания постоянной составляющей
                n_avg = IntGet();
                //параметры вычитания постоянной составляющей
                n_ignore = IntGet();
            }
        }
        public static void LoadIn(string filename)
        {
            InFile = filename;
            f = new double[count_t];
            using (StreamReader sr = new StreamReader(InFile))
                for (int i = 0; i < count_t; i++)
                    f[i] = Double.Parse(sr.ReadLine());

            avg = 0;
            for (int i = n_ignore; i < n_avg; i++)
                avg += f[i];

            avg /= n_avg;
            for (int i = 0; i < count_t; i++)
                f[i] -= avg;
        }

        public static void LoadIn2(string filename)
        {
            InFile = filename;
            f = new double[count_t];
            using (StreamReader sr = new StreamReader(InFile, System.Text.Encoding.Default))
            {
                for (int i = 0; i < count_t; i++)
                    f[i] = double.Parse(sr.ReadLine().Split('\t')[1]);
            }
            avg = 0;
            for (int i = n_ignore; i < n_avg; i++)
                avg += f[i];
            avg /= n_avg;
            for (int i = 0; i < count_t; i++)
                f[i] -= avg;
        }

        public static void LoadInDiff(string filename1, string filename2, int len)
        {
            count_t = len;
            using (StreamReader sr1 = new StreamReader(filename1, System.Text.Encoding.Default))
            using (StreamReader sr2 = new StreamReader(filename2, System.Text.Encoding.Default))
            {
                f = new double[count_t];
                for (int i = 0; i < count_t; i++)
                {
                    f[i] = -double.Parse(sr1.ReadLine().Replace('.', ','));
                    f[i] += double.Parse(sr2.ReadLine().Replace('.', ','));
                    if (i < n_ignore)
                    {
                        f[i] = 0;
                    }
                }
            }

            avg = 0;
            for (int i = n_ignore; i < n_avg; i++)
                avg += f[i];
            avg /= n_avg;
            for (int i = 0; i < count_t; i++)
                f[i] -= avg;
        }

        private static Complex Expi(double val) => new Complex(Math.Cos(val), Math.Sin(val));
        /// <summary>
        /// Мемоизированная версия преобразования Фурье
        /// </summary>
        /// <param name="progress"></param>
        public static void GetSplainFT_new(IProgress<int> progress = null)
        {
            F = new Complex[count_w];
            int[] s = new int[count_w];
            bool isnotnull = progress != null;

            Parallel.For(0, count_w, (int i) =>
            {
                Complex result = 0;
                double w = argi[i];
                double A = AAMemoized(i);
                for (int j = n_ignore; j < count_t; j++)
                    result += f[j] * Expi(w * argj[j - n_ignore]) * A;

                F[i] = result;
                s[i]++;
                if (isnotnull && i % 7 == 0)
                    progress.Report(s.Sum());
            });
        }
        /// <summary>
        /// Оптимизированная версия преобразования Фурье
        /// </summary>
        /// <param name="progress"></param>
        public static void GetSplainFT_old(IProgress<int> progress = null)
        {
            F = new Complex[count_w];
            int[] s = new int[count_w];
            bool isnotnull = progress != null;

            Parallel.For(0, count_w, (int i) =>
            {
                Complex result = 0;
                double w = (dw * i + w_0);
                double dtw = dt * w;
                double A = 2.0 * (1.0 - Math.Cos(dtw)) / (dtw * w);
                for (int j = n_ignore; j < count_t; j++)
                    result += f[j] * Expi(w * (dt * j + t_0)) * A;

                F[i] = result;
                s[i]++;

                if (isnotnull && i % 7 == 0)
                    progress.Report(s.Sum());
            });
        }

        //public static void SaveOut_old(string filename)
        //{
        //    try
        //    {
        //        OutFile = filename;
        //        using (StreamWriter sw = new StreamWriter(OutFile, false, System.Text.Encoding.Default))
        //        {
        //            string line = "w Re(f(w)) Im(f(w))";
        //            sw.WriteLine(line);
        //            for (int i = 0; i < count_w; i++)
        //            {
        //                Complex w = dw * i + w_0;
        //                line = (w.Real / 2.0 / Math.PI).ToString() + " " + F[i].Real.ToString() + " " + F[i].Imaginary.ToString();
        //                sw.WriteLine(line);
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }
        //}
        private static double ToW(int i) => (dw * i + w_0) / _2PI;
        public static void SaveIn(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
                for (int i = 0; i < count_t; i++)
                    sw.WriteLine(f[i].ToString().Replace(",", "."));
        }
        public static void SaveOut(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.WriteLine("w Re(f(w)) Im(f(w))");
                for (int i = 0; i < count_w; i++)
                    sw.WriteLine($"{ToW(i).ToString().Replace(",", ".")} {F[i].Real.ToString().Replace(",", ".")} {(-F[i].Imaginary).ToString().Replace(",", ".")}");
            }
        }
        public static void SaveOutAbs(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.WriteLine("w Re(f(w)) Im(f(w))");
                for (int i = 0; i < count_w; i++)
                    sw.WriteLine($"{ToW(i).ToString().Replace(",", ".")} {F[i].Magnitude.ToString().Replace(",", ".")}");
            }
        }


        static Memoize<Tuple<int, int>, Complex> Dictionary;
        static Func<int, int, Complex> Fury = (int i, int j) =>
             {
                 double w = (dw * i + w_0);
                 return Expi(w * (dt * j + t_0)) * AAMemoized(i);
             };
        static Func<int, int, Complex> FuryMemoized;
        static Memoize<int, double> DictionaryA;
        static Func<int, double> AA = (int i) =>
         {
             double w = (dw * i + w_0);
             double dtw = dt * w;
             return 2.0 * (1.0 - Math.Cos(dtw)) / (dtw * w);
         };
        static Func<int, double> AAMemoized;
        static double[] argi, argj;
        //static Complex[,] tmpArray;

        /// <summary>
        /// Пересоздать словарь для дальнейшего использования в нескольких циклах
        /// </summary>
        public static void CreateNewGen()
        {
            Dictionary = new Memoize<Tuple<int, int>, Complex>((Tuple<int, int> t) => Fury(t.Item1, t.Item2));
            FuryMemoized = (int i, int j) => Dictionary.Value(new Tuple<int, int>(i, j));

            DictionaryA = new Memoize<int, double>(i => AA(i),count_w);
            AAMemoized = DictionaryA.Value;

            argj = new double[count_t - n_ignore];
            for (int i = 0; i < argj.Length; i++)
                argj[i] = t_0 + dt * (i + n_ignore);

            argi = new double[count_w];
            for (int i = 0; i < argi.Length; i++)
                argi[i] = dw * i + w_0;

            //CreateTmpArray();
        }
        //private static void CreateTmpArray()
        //{
        //    tmpArray = new Complex[argi.Length, argj.Length];
        //    Parallel.For(0, argi.Length, (int i) =>
        //    {
        //        double w = argi[i];
        //    double A = AAMemoized(i);
        //        for (int j = 0; j < argj.Length; j++)
        //            tmpArray[i, j] = Expi(w * argj[j - n_ignore]) * A;
        //    });
        //}

        /// <summary>
        /// Освободить ресурсы
        /// </summary>
        public static void Dispose()
        {
            f = null;
            F = null;
            argi = null;
            argj = null;
            // tmpArray = null;
            if (Dictionary != null)
                Dictionary.Dispose();
            if (DictionaryA != null)
                DictionaryA.Dispose();
            GC.Collect();
        }
    }

    public class CSwitchInterface
    {

        public const int E_OK = 0;
        public const int E_TIMEOUT = 1;
        public const int E_CONNECTION = 2;
        public const int E_CONNECTION_LOST = 3;
        public const int E_TRANSMISSION_FAIL = 4;
        public SerialPort port;
        public string Receive_str()
        {
            if (port.BytesToRead > 0)
            {
                return port.ReadLine();
            }
            else return "";
        }
        public void OpenPort()
        {

            // получаем список доступных портов 
            string[] ports = SerialPort.GetPortNames();
            Console.WriteLine("Выберите порт:");
            // выводим список портов
            for (int i = 0; i < ports.Length; i++)
                Console.WriteLine("[" + i.ToString() + "] " + ports[i].ToString());
            port = new SerialPort();
            // читаем номер из консоли
            string n = Console.ReadLine();
            int num = int.Parse(n);
            try
            {
                // настройки порта
                port.PortName = ports[num];
                port.BaudRate = 57600;
                port.DataBits = 8;
                port.Parity = System.IO.Ports.Parity.None;
                port.StopBits = System.IO.Ports.StopBits.One;
                port.ReadTimeout = 1000;
                port.WriteTimeout = 1000;
                port.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: невозможно открыть порт:" + e.ToString());
                return;
            }
        }
        public void OpenPort(int num)
        {

            // получаем список доступных портов 
            string[] ports = SerialPort.GetPortNames();
            try
            {
                port = new SerialPort();
                // настройки порта
                port.PortName = ports[num];
                port.BaudRate = 57600;
                port.DataBits = 8;
                port.Parity = System.IO.Ports.Parity.None;
                port.StopBits = System.IO.Ports.StopBits.One;
                port.ReadTimeout = 1000;
                port.WriteTimeout = 1000;
                port.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: невозможно открыть порт:" + e.ToString());
                return;
            }
        }
        public void SendCmd(int status /*старшая цифра - команда */, int addr /*младшая цифра - адресс */)
        {
            port.Write(status.ToString() + addr.ToString());
        }
        //отправка команды ввиде строки
        public void SendCmd(string s)
        {
            port.Write(s);
        }
        public void SetOut(int addr)
        {
            SendCmd(0, addr);
        }
        public void SetIn(int addr)
        {
            SendCmd(1, addr);
        }
        //отправка команды из консоли
        // старшая цифра - команда младшая цифра - адресс 
        public void SendCmd2()
        {
            port.Write(Console.ReadLine());
        }
        public void ClosePort_()
        {
            if (port.IsOpen) port.Close();
        }

        public string GetAccepted()
        {
            if (port.BytesToRead > 0)
            {
                return port.ReadLine();
            }
            return "";
        }
    }
}
