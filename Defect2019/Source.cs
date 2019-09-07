using System;
using System.Collections.Generic;
using System.IO;
using МатКлассы;
using static МатКлассы.Number;
using static МатКлассы.Waves;
using Point = МатКлассы.Point;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

/// <summary>
/// Источник с дополнительными свойствами
/// </summary>
public class Source : Idup<Source>
{
    /// <summary>
    /// Центр источника
    /// </summary>
    public Point Center { get; private set; }

    /// <summary>
    /// Копия источника
    /// </summary>
    public Source dup => new Source(this);

    /// <summary>
    /// Массив нормалей
    /// </summary>
    public Normal2D[] Norms;

    /// <summary>
    /// Фильтр принадлежности к источнику
    /// </summary>
    public Func<Point, bool> Filter;

    /// <summary>
    /// Массив f(w)
    /// </summary>
    public Complex[] Fmas;

    public enum Type { Circle, DCircle };

    /// <summary>
    /// Тип источника
    /// </summary>
    public Type MeType;

    /// <summary>
    /// Радиус
    /// </summary>
    public double radius;

    /// <summary>
    /// Создать источник по основным характеристикам
    /// </summary>
    /// <param name="center"></param>
    /// <param name="normals"></param>
    /// <param name="filter"></param>
    /// <param name="fmas"></param>
    /// <param name="type"></param>
    /// <param name="radius"></param>
    public Source(Point center, Normal2D[] normals, Func<Point, bool> filter, Complex[] fmas, Type type, double radius)
    {
        Center = center.dup;
        Norms = normals; //new Normal2D[normals.Length];
        //for (int i = 0; i < normals.Length; i++)
        //    Norms[i] = new Normal2D(normals[i]);
        Filter = new Func<Point, bool>(filter);
        Fmas = fmas.Dup();

        this.radius = radius;
        MeType = type;
    }

    private Source(Source s) : this(s.Center, s.Norms, s.Filter, s.Fmas, s.MeType, s.radius)
    {
    }

    /// <summary>
    /// Массив приложений нормалей источника
    /// </summary>
    public Point[] NormsPositionArray
    {
        get
        {
            Point[] mas = new Point[Norms.Length];
            for (int i = 0; i < mas.Length; i++)
            {
                mas[i] = Norms[i].Position;
            }

            return mas;
        }
    }

    public override string ToString()
    {
        return $"type = {((MeType == Type.Circle) ? "circle" : "Dcircle")} \tcenter = ({Center.x}; {Center.y}) R = {radius} N.Count = {Norms.Length}";
    }

    /// <summary>
    /// Эквивалентность по центрам
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        Point p = (obj as Source).Center;
        return (Center).Equals(p);
    }

    public override int GetHashCode()
    {
        return 659946564 + EqualityComparer<Point>.Default.GetHashCode(Center);
    }

    /// <summary>
    /// Характеристика массива в одной строке
    /// </summary>
    /// <param name="mas"></param>
    /// <returns></returns>
    public static string ToString(Source[] mas)
    {
        string s = "(x,y,r) = {";
        for (int i = 0; i < mas.Length - 1; i++)
        {
            s += $"{mas[i].Center.x},{mas[i].Center.y},{mas[i].radius}" + "} {";
        }

        s += $"{mas[mas.Length - 1].Center.x},{mas[mas.Length - 1].Center.y},{mas[mas.Length - 1].radius}" + "}";
        return s;
    }

    /// <summary>
    /// Краткая запись источника
    /// </summary>
    /// <returns></returns>
    public string ToShortString()
    {
        return $"{((MeType == Type.Circle) ? "circle" : "Dcircle")} ({Center.x} , {Center.y}) R = {radius} N.Count = {Norms.Length}";
    }

    /// <summary>
    /// Записать массив f(w) в файл
    /// </summary>
    /// <param name="filename"></param>
    public void FmasToFile(string path = "")
    {
        string filename = $"f(w) from {this.Center.ToString()}.txt";
        using (StreamWriter fs = new StreamWriter(Path.Combine(path, filename)))
        {
            fs.WriteLine("w Re(f(w)) Im(f(w))");
            for (int i = 0; i < Fmas.Length; i++)
            {
                fs.WriteLine($"{РабКонсоль.wmas[i]} {Fmas[i].Re} {Fmas[i].Im}");
            }
        }
    }

    /// <summary>
    /// Считать массив f(w) из файла
    /// </summary>
    /// <param name="filename"></param>
    public void FmasFromFile(string path = "")
    {
        string filename = $"f(w) from {this.Center.ToString()}.txt";
        string s = "";
        using (StreamReader fs = new StreamReader(Path.Combine(path, filename)))
        {
            s = fs.ReadLine();
            string[] st;

            // List<double> d = new List<double>();
            List<Complex> c = new List<Complex>();

            s = fs.ReadLine();
            while (s != null)
            {
                //s.Show();
                st = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                // d.Add(st[0].ToDouble());
                c.Add(new Complex(st[1].ToDouble(), st[2].ToDouble()));
                //Console.WriteLine(new Complex(st[1].ToDouble(), st[2].ToDouble()).Conjugate);

                s = fs.ReadLine();
            }

            Fmas = c.ToArray();
            //Fmas = new Tuple<double[], Complex[]>(Functions.SeqWMemoized(wbeg, wend, wcount), c.ToArray().ToCVector().Normalize.ComplexMas);
        }
        System.Diagnostics.Debug.WriteLine($"Считано f(w) для источника {this.ToString()}" + Environment.NewLine + Fmas[0] + Environment.NewLine + Fmas[1] + Environment.NewLine + Fmas[2]);
    }

    /// <summary>
    /// Возвращает те источники, для которых существуют файлы f(w) в папке directory
    /// </summary>
    /// <param name="s"></param>
    /// <param name="directory"></param>
    /// <returns></returns>
    public static Source[] GetSourcesWithFw(Source[] s, string directory)
    {
        List<Source> t = new List<Source>();
        for (int i = 0; i < s.Length; i++)
        {
            if (File.Exists(Path.Combine(directory, $"f(w) from {s[i].Center}.txt")))
            {
                t.Add(s[i]);
            }
        }

        return t.ToArray();
    }

    /// <summary>
    /// Возвращает массив источников, для которых существуют f(w) в папке directory, + дополнительно считывает f(w)
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    public static Source[] GetSourcesWithReadFw(string directory,Source[] sourcesArray)
    {
        var s = Source.GetSourcesWithFw(sourcesArray, directory);
        FilesToSources(s, directory);
        return s;
    }

    /// <summary>
    /// Считать файлы и записать f(w) в имеющиеся источники
    /// </summary>
    private static void FilesToSources(Source[] sources, string path)
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
    public static void FilesFromSources(Source[] sources,string filename)
    {
        string t;
        using (StreamReader r = new StreamReader(filename))
            t = r.ReadLine().Replace("\r\n", "");
        Parallel.For(0, sources.Length, (int i) => sources[i].FmasToFile(t));
    }

    public static double GetMaxRadius(Source[] s)
    {
        double res = 0;
        for (int i = 0; i < s.Length; i++)
            if (s[i].radius > res)
                res = s[i].radius;
        return res;
    }
}