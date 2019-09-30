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
            string BeeHiveAdress = @"C:\Users\крендель\Desktop\Code\Defect2019\bin\Debug\Максимумы с эллипсов";
            string NotBeeHiveAdress = @"C:\Users\крендель\Desktop\Code\Defect2019\bin\Release\Максимумы с эллипсов";
            string Symbols = "ABCDEFGH";

            string[] files = new string[Symbols.Length * (Symbols.Length - 1)];
            int k = 0;
            for (int i = 0; i < Symbols.Length; i++)
                for (int j = 0; j < Symbols.Length; j++)
                    if (i != j)
                        files[k++] = $"{Symbols[i]}to{Symbols[j]}(MaxCoordinate).txt";

            double[] Get(string path) => files.Select(s => Expendator.GetStringArrayFromFile(Path.Combine(path, s))[2].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2].Replace('.',',').ToDouble()).ToArray();

            var vec1 = Get(BeeHiveAdress);
            var vec2 = Get(NotBeeHiveAdress);

            using (StreamWriter r = new StreamWriter("bee.txt"))
                for (int i = 0; i < vec1.Length; i++)
                {
                    r.WriteLine($"{vec1[i]} {vec2[i]}");
                    Console.WriteLine($"{vec1[i]} \t{vec2[i]} " + ((vec1[i]>=vec2[i])?"\tЛучше  +":"\tХучше  -"));
                }

            File.Copy(Expendator.GetResource("TestBee.r", "OptimisationTest"), "TestBee.r", true);
            Expendator.StartProcessOnly("TestBee.r");
            Process.Start("bee.png");

            Console.ReadKey();
        }
    }
}
