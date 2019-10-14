using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using МатКлассы;
using System.Diagnostics;
using System.IO;

namespace OptimisationTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string BeeHiveAdress = @"C:\Users\user1\source\repos\CodeIt\Defect2019\bin\Release\EllipseData";
            string NotBeeHiveAdress = @"C:\Users\user1\Desktop\EllipseData";
            string Symbols = "ABCDEFGH";

            string[] files = new string[Symbols.Length * (Symbols.Length - 1)];
            int k = 0;
            for (int i = 0; i < Symbols.Length; i++)
                for (int j = 0; j < Symbols.Length; j++)
                    if (i != j)
                        files[k++] = $"{Symbols[i]}to{Symbols[j]}(MaxCoordinate).txt";

            double[] Get(string path) => files.Select(s => Expendator.GetStringArrayFromFile(Path.Combine(path, s))[2].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2].Replace('.', ',').ToDouble()).ToArray();

            var vec1 = Get(BeeHiveAdress);
            var vec2 = Get(NotBeeHiveAdress);

            double s1 = 0, s2 = 0;

            using (StreamWriter r = new StreamWriter("bee.txt"))
                for (int i = 0; i < vec1.Length; i++)
                {
                    r.WriteLine($"{vec1[i]} {vec2[i]}");
                    Console.WriteLine($"{vec1[i]} \t{vec2[i]} " + ((vec1[i] >= vec2[i]) ? "\tЛучше  +" : "\tХучше  -"));
                    double tmp = vec1[i] - vec2[i];
                    if (tmp < 0)
                        s2 -= tmp;
                    else
                        s1 += tmp;
                }
            Console.WriteLine($"Выигрыш = \t{s1} ({Expendator.GetProcent(s1, s1 + s2)}%)");
            Console.WriteLine($"Проигрыш = \t{s2} ({Expendator.GetProcent(s2, s1 + s2)}%)");

            File.Copy(Expendator.GetResource("TestBee.r", "OptimisationTest"), "TestBee.r", true);
            Expendator.StartProcessOnly("TestBee.r");
            Process.Start("bee.png");

            Console.ReadKey();
        }
    }
}
