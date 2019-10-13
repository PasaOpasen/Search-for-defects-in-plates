using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using МатКлассы;
using System.Diagnostics;

namespace Работа2019
{
    /// <summary>
    /// Параметры эллипса
    /// </summary>
    public class EllipseParam
    {
        internal readonly string name;
        internal Point focSource, focSensor;
        internal readonly Point xM;
        internal double L, a, b, tau;
        public System.Drawing.Color Color;
        public readonly bool right = true;

        private double Dist(Point p) => Point.Eudistance(focSensor, p) + Point.Eudistance(focSource, p);
        /// <summary>
        /// Функция, которая по точке находит расстояние эллипса (s) и от него возвращает значение
        /// </summary>
        public readonly Func<Point, double> GetValue;

        public EllipseParam(Point SourceCenter, Point SensorCenter, double s, System.Drawing.Color color, string Name="", Func<double, double> Val=null)
        {
            name = Name;

            L = Point.Eudistance(SourceCenter, SensorCenter);
            if (s < L)
            {
                //throw new ArgumentException($"s < L !!! ({s} < {L})");
                Debug.WriteLine($"s < L !!! ({s} < {L})");
                right = false;
            }
                

            focSource = SourceCenter;
            focSensor = SensorCenter;
            a = s / 2;
            b = Math.Sqrt(a * a - (L / 2).Sqr());
            Color = color;

            xM = new Point((focSensor.x + focSource.x) / 2, (focSensor.y + focSource.y) / 2);

            tau = Math.Atan((focSensor.y - focSource.y) / (focSensor.x - focSource.x));
            if (focSensor.x < focSource.x)
                tau += Math.PI;


            if (Val == null)
                GetValue = null;
            else
            GetValue = (Point p) => Val(Dist(p));
        }

        public Point[] GetPointArray(int count)
        {
            double cost = Math.Cos(tau), sint = Math.Sin(tau);
            var angles = Expendator.Seq(0, 2 * Math.PI, count, false);
            Point[] res = new Point[count];
            for (int i = 0; i < angles.Length; i++)
            {
                double sinf = Math.Sin(angles[i]), cosf = Math.Cos(angles[i]);
                res[i] = new Point(xM.x + a * cost * cosf - b * sint * sinf, xM.y + a * sint * cosf + b * cost * sinf);
            }
            return res;
        }
        public override string ToString()
        {
            return $"{name}: source = {focSource} sensor = {focSensor} s = {2*a} color = {Color}";
        }

        /// <summary>
        /// Записать коллекцию параметров в файл
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="list"></param>
        public static void WriteInFile(string filename, IEnumerable<EllipseParam> list) => Expendator.WriteInFile(filename, list.Select(l => l.ToString()).ToArray());

        /// <summary>
        /// Создать графики по эллипсам
        /// </summary>
        /// <param name="array"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="shortname"></param>
        /// <returns></returns>
        public async static Task GetSurfaces(EllipseParam[] array, NetOnDouble X,NetOnDouble Y, string shortname)
        {
            var mas = array.Where(e => e.GetValue != null).ToArray();
            if (mas.Length == 0)
                return;

            Func<double,double, double> S = (double x,double y) =>
               {
                   Point p = new Point(x, y);
                   double sum = mas[0].GetValue(p);
                   for (int i = 1; i < mas.Length; i++)
                       sum += mas[i].GetValue(p);
                   return sum;
               };

            await Библиотека_графики.Create3DGrafics.JustGetGraficInFilesAsync(shortname, shortname, S, X, Y, 
                new Progress<int>(), new System.Threading.CancellationToken(), 
                new Библиотека_графики.StringsForGrafic("Gauss"), Библиотека_графики.Create3DGrafics.GraficType.Pdf, true);
        }
    }
}
