using System;
using System.Collections.Generic;
using МатКлассы;
using static МатКлассы.Number;
using static МатКлассы.Waves;
using Point = МатКлассы.Point;
using System.IO;
using static Functions;
using static РабКонсоль;

/// <summary>
/// Источник с дополнительными свойствами
/// </summary>
public class Source
{
    public Point Center { get; private set; }
    public Normal2D[] Norms;
    public Func<Point, bool> Filter;
    public Tuple<double[], Complex[]> Fmas;

    public enum Type { Circle,DCircle};
    public Type MeType;
    public double radius;

    public Source(Point center,Normal2D[] normals, Func<Point, bool> filter, Tuple<double[], Complex[]> fmas, Type type,double radius)
    {
        Center = center.dup;
        Norms = normals; //new Normal2D[normals.Length];
        //for (int i = 0; i < normals.Length; i++)
        //    Norms[i] = new Normal2D(normals[i]);
        Filter = new Func<Point, bool>(filter);
        Fmas = new Tuple<double[], Complex[]>(fmas.Item1.Dup(), fmas.Item2.Dup());

        this.radius = radius;
        MeType = type;

    }
    public Source(Source s) : this(s.Center,s.Norms,s.Filter,s.Fmas,s.MeType,s.radius) { }

    public Point[] MasForDraw()
    {
        Point[] mas = new Point[Norms.Length];
        for (int i = 0; i < mas.Length; i++)
            mas[i] = Norms[i].Position;
        return mas;
    }

    public override string ToString()
    {
        return $"type = {((MeType==Type.Circle)?"circle":"Dcircle")} \tcenter = ({Center.x}; {Center.y}) R = {radius} N.Count = {Norms.Length}";
    }

    public override bool Equals(object obj)
    {
        Point p = (obj as Source).Center;
        return (Center).Equals(p);
    }

    public override int GetHashCode()
    {
        return 659946564 + EqualityComparer<Point>.Default.GetHashCode(Center);
    }

    public static  string ToString( Source[] mas)
    {
        string s = "(x,y,r) = {";
        for (int i = 0; i < mas.Length-1; i++)
            s += $"{mas[i].Center.x},{mas[i].Center.y},{mas[i].radius}" + "} {";
        s += $"{mas[mas.Length - 1].Center.x},{mas[mas.Length - 1].Center.y},{mas[mas.Length - 1].radius}"+"}";
        return s;
    }

    /// <summary>
    /// Записать массив f(w) в файл
    /// </summary>
    /// <param name="filename"></param>
    public void FmasToFile()
    {
        string filename = $"f(w) from {this.Center.ToString()}.txt";
        using(StreamWriter fs=new StreamWriter(filename))
        {
            fs.WriteLine("w Re(f(w)) Im(f(w))");
            for (int i = 0; i < Fmas.Item1.Length; i++)
                fs.WriteLine($"{Fmas.Item1[i]} {Fmas.Item2[i].Re} {Fmas.Item2[i].Im}");
        }
    }

    /// <summary>
    /// Считать массив f(w) из файла
    /// </summary>
    /// <param name="filename"></param>
    public void FmasFromFile()
    {
        string filename = $"f(w) from {this.Center.ToString()}.txt";
        string s = "";
        using (StreamReader fs = new StreamReader(filename))
        {
            s = fs.ReadLine();
            string[] st;

            List<double> d = new List<double>();
            List<Complex> c = new List<Complex>();

            s = fs.ReadLine();
            while (s != null)
            {
                //s.Show();
                st = s.Split(new char[] { ' '}, StringSplitOptions.RemoveEmptyEntries);
                d.Add(st[0].ToDouble());
                c.Add(new Complex(st[1].ToDouble(), st[2].ToDouble()));
                //Console.WriteLine(new Complex(st[1].ToDouble(), st[2].ToDouble()).Conjugate);
                s = fs.ReadLine();
            }

            Fmas = new Tuple<double[], Complex[]>(Functions.Seqw(wbeg,wend,wcount), c.ToArray());
        }
    }
}