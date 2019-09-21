using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using МатКлассы;
using System.IO;

namespace МатКлассы
{
    /// <summary>
    /// Класс с методами для создания 3D графиков
    /// </summary>
    public static class Create3DGrafics
    {

        private static async Task GetDataToFile(string shortname, Func<double, double, double> F, double[] x, double[] y, IProgress<int> progress, System.Threading.CancellationToken token, string title = "", string xlab = "x", string ylab = "y", string zlab = "z", bool parallel = true)
        {
            int len = x.Length;
            int[] k = new int[len * len];
            double[,] ur = new double[len, len];

            void InnerLoop(int i)
            {
                for (int j = 0; j < len; j++)
                {
                    if (token.IsCancellationRequested) return;
                    ur[i, j] = F(x[i], y[j]);
                    k[i * len + j] = 1;
                }
                progress.Report(k.Sum());
            }

            await Task.Run(() =>
            {
                //нахождение массивов
                if (parallel)
                    Parallel.For(0, len, (int i) =>
                    {
                        InnerLoop(i);
                    });
                else
                    for (int i = 0; i < len; i++)
                    {
                        InnerLoop(i);
                    }
            });


            var filenames = new string[]
            {
                shortname + "(args).txt",
                shortname + "(vals).txt",
                shortname + "(info).txt"
            };
            Expendator.WriteInFile("3D Grafics Data Adress.txt", filenames);

            Expendator.WriteInFile(filenames[2], new string[]
            {
                shortname,
                title,
                xlab,ylab,zlab
            });

            //запись в файлы            
            using (StreamWriter xs = new StreamWriter(filenames[0]))
            {

                xs.WriteLine("x y");
                for (int i = 0; i < len; i++)
                    xs.WriteLine($"{x[i]} {y[i]}");
            }

            using (StreamWriter ts = new StreamWriter(filenames[1]))
            {
                ts.WriteLine("vals");
                for (int i = 0; i < len; i++)
                    for (int j = 0; j < len; j++)
                        if (Double.IsNaN(ur[i, j]))
                            ts.WriteLine("NA");
                        else
                            ts.WriteLine($"{ur[i, j]}");
            }
        }

        private static async Task GetDataToFile(string shortname,  Func<double, double, double> F, double xmin, double xmax, double ymin, double ymax, int count, IProgress<int> progress, System.Threading.CancellationToken token, string title = "", string xlab = "x", string ylab = "y", string zlab = "z", bool parallel = true)
        {
            var x = Expendator.Seq(xmin, xmax, count);
            var y = Expendator.Seq(ymin, ymax, count);
            await GetDataToFile(shortname, F, x, y, progress, token, title, xlab, ylab, zlab, parallel);
        }

        /// <summary>
        /// Тип графика
        /// </summary>
        public enum GraficType
        {
            /// <summary>
            /// Только Pdf график через persp3D
            /// </summary>
            Pdf,
            /// <summary>
            /// Тепловая карта через ggplot2
            /// </summary>
            Png,
            /// <summary>
            /// Вращающийся график в браузере
            /// </summary>
            Html,
            /// <summary>
            /// Все три варианта
            /// </summary>
            PdfPngHtml,
            /// <summary>
            /// График в окне с возможностью вращения и масштабирования через nzy3d
            /// </summary>
            Window
        }

        /// <summary>
        /// Создать 3D график в нужной форме
        /// </summary>
        /// <param name="graficType">Тип графика</param>
        /// <param name="shortname">Имя файла с графиками (без расширения)</param>
        /// <param name="path">Директория, куда будут сохраняться файлы</param>
        /// <param name="F">Функция, чей график надо построить</param>
        /// <param name="xmin">Начало отрезка по первой координате</param>
        /// <param name="xmax">Конец отрезка по первой координате</param>
        /// <param name="ymin">Начало отрезка по второй координате</param>
        /// <param name="ymax">Конец отрезка по второй координате</param>
        /// <param name="count">Число точек в разбиении отрезка. В сетке будет count*count этих точек</param>
        /// <param name="progress">Объект для отслеживания прогресса</param>
        /// <param name="token">Объект для отмены операции</param>
        /// <param name="title">Название поверхности</param>
        /// <param name="xlab">Название оси X</param>
        /// <param name="ylab">Название оси Y</param>
        /// <param name="zlab">Название оси Z</param>
        /// <param name="parallel">Выполнять ли вычисления параллельно</param>
        public static async Task MakeGrafic(GraficType graficType, string shortname, Func<double, double, double> F, double xmin, double xmax, double ymin, double ymax, int count, IProgress<int> progress, System.Threading.CancellationToken token, string title = "", string xlab = "x", string ylab = "y", string zlab = "z", bool parallel = true)
        {
            await GetDataToFile(shortname, F, xmin, xmax, ymin, ymax, count, progress, token, title, xlab, ylab, zlab, parallel);

            if (graficType == GraficType.Window)
            {

            }
            else
            {
                GraficTypeToFile(graficType);
              //  await Task.Run(() => Expendator.StartProcessOnly("Magic3DScript.R"));
            }
        }

        private static void GraficTypeToFile(GraficType type)
        {
            string s = "";
            switch (type)
            {
                case GraficType.Html:
                    s = "html";
                    break;
                case GraficType.Pdf:
                    s = "pdf";
                    break;
                case GraficType.Png:
                    s = "png";
                    break;
                default:
                    s = "all";
                    break;
            }
            Expendator.WriteStringInFile("GraficType.txt", s);
        }
    }
}
