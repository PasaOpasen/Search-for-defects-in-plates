using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Библиотека_графики;
using МатКлассы;
using static МатКлассы.Number;

namespace Консоль
{
    public static class WaveletTest
    {
        public static void Start(Func<double,double> func, 
            Wavelet.Wavelets wavelets, double omega,FuncMethods.DefInteg.GaussKronrod.NodesCount nodesCount,
NetOnDouble X, NetOnDouble Y, NetOnDouble T)
        {
            Wavelet.countNodes = nodesCount;
            var wavel = Wavelet.Create(wavelets, omega);

            Func<double, double, Complex> f = wavel.GetAnalys(func);

            IProgress<int> progress = new Progress<int>(number => Console.WriteLine(Expendator.GetProcent(number, X.Count * Y.Count).ToString(2) + "%"));
            Create3DGrafics.MakeGrafic(Create3DGrafics.GraficType.PdfPngHtml, $"wavelets{wavelets}",
                (a, b) => f(a, b).Abs, X,Y,
                progress, new System.Threading.CancellationToken(),
                $"Wavelet {wavelets}", "a", "b", "|values|", true);


            Func<double, double> func2 = wavel.GetSyntesis();

            new MostSimpleGrafic(new Func<double, double>[]
            {
               func,
               func2
            }, 
            T,
            new string[] {
               "Исходная функция",
               "Её преобразование туда-сюда"
            },
               true).ShowDialog();
        }


        public static void Start(double begin,double step,int count,string filename,string path, 
            Wavelet.Wavelets wavelets, double omega, FuncMethods.DefInteg.GaussKronrod.NodesCount nodesCount,
            NetOnDouble X, NetOnDouble Y)
        {
            Wavelet.countNodes = nodesCount;
            var wavel = Wavelet.Create(wavelets, omega);

            var mas = Point.CreatePointArray(begin, step, count, filename, path);
            Func<double, double, Complex> f = wavel.GetAnalys(mas);

            IProgress<int> progress = new Progress<int>(number => Console.WriteLine(Expendator.GetProcent(number, X.Count * Y.Count).ToString(2) + "%"));
            Create3DGrafics.MakeGrafic(Create3DGrafics.GraficType.PdfPngHtml, $"wavelets{wavelets}",
                (a, b) => f(a, b).Abs, X,Y,
                progress, new System.Threading.CancellationToken(),
                $"Wavelet {wavelets}", "a", "b", "|values|", true);

            //Func<double, double> func2 = wavel.GetSyntesis();
            //new MostSimpleGrafic( func2, mas,    "Преобразование",    true).ShowDialog();
        }
    }
}
