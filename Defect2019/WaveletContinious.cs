using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using МатКлассы;
using System.IO;

namespace Работа2019
{
    public partial class WaveletContinious : Form
    {
        private readonly double step = 16E-9;
        private readonly int pcount = 150000;

        private static string symbols = "ABCDEFGH";
        private Source[] sources;
        private double wmin, wmax, tmin, tmax, epsForWaveletValues=1e-8;
        private int wcount,tcount,byevery;
        private NetOnDouble W, T;

        public WaveletContinious(Source[] array)
        {
            InitializeComponent();
            sources = array;

            SetDefaultStrip();
            SetDataGrid();

            timer1.Tick += (o, e) =>
            {
                var d = Expendator.GetProcent(save, all, 2);
                toolStripLabel2.Text = $"{d}%";
                toolStripProgressBar1.Value = (int)(d / 100 * toolStripProgressBar1.Maximum);
            };
        }

        private void SetDefaultStrip()
        {
            toolStripLabel1.Text = "Готов";
            SetDefaltProgressBar();
        }
        private void SetDefaltProgressBar()
        {
            toolStripLabel2.Text = "";
            toolStripProgressBar1.Value = 0;
        }
        private void SetDataGrid()
        {
            dataGridView1.RowHeadersVisible = false;

            var column1 = new DataGridViewColumn();
            column1.HeaderText = "Источник"; //текст в шапке
            column1.Width = 450; //ширина колонки
            column1.ReadOnly = true; //значение в этой колонке нельзя править
            column1.Name = "source"; //текстовое имя колонки, его можно использовать вместо обращений по индексу
            column1.Frozen = true; //флаг, что данная колонка всегда отображается на своем месте
            column1.CellTemplate = new DataGridViewTextBoxCell(); //тип нашей колонки

            var column2 = new DataGridViewColumn();
            column2.HeaderText = "Имя";
            column2.Name = "name";
            column2.CellTemplate = new DataGridViewTextBoxCell();

            dataGridView1.Columns.Add(column1);
            dataGridView1.Columns.Add(column2);

            dataGridView1.AllowUserToAddRows = false; //запрешаем пользователю самому добавлять строки

            for (int i = 0; i < sources.Length; ++i)
            {
                //Добавляем строку, указывая значения колонок поочереди слева направо
                dataGridView1.Rows.Add(sources[i].ToShortString(), symbols[i]);
            }

            //for (int i = 0; i < 5; ++i)
            //{
            //    //Добавляем строку, указывая значения каждой ячейки по имени (можно использовать индекс 0, 1, 2 вместо имен)
            //    dataGridView1.Rows.Add();
            //    dataGridView1["name", dataGridView1.Rows.Count - 1].Value = "Пример 2, Товар " + i;
            //    dataGridView1["price", dataGridView1.Rows.Count - 1].Value = i * 1000;
            //    dataGridView1["count", dataGridView1.Rows.Count - 1].Value = i;
            //}

            ////А теперь простой пройдемся циклом по всем ячейкам
            //for (int i = 0; i < dataGridView1.Rows.Count; ++i)
            //{
            //    for (int j = 0; j < dataGridView1.Columns.Count; ++j)
            //    {
            //        //Значения ячеек хряняться в типе object
            //        //это позволяет хранить любые данные в таблице
            //        object o = dataGridView1[j, i].Value;
            //    }
            //}
        }
        private void GetData()
        {
            wmin = 1.0 / textBox2.Text.ToDouble() / 1000;//(2000*Math.PI);
            wmax = 1.0 / textBox1.Text.ToDouble() / 1000; //(2000 * Math.PI);
            tmin = textBox3.Text.ToDouble();
            tmax = textBox4.Text.ToDouble();
            wcount = numericUpDown1.Value.ToInt32();
            tcount= numericUpDown2.Value.ToInt32();
            byevery = numericUpDown3.Value.ToInt32();

            W = new NetOnDouble(wmin, wmax, wcount);
            T = new NetOnDouble(tmin, tmax, tcount);
            epsForWaveletValues= textBox5.Text.ToDouble();
        }


        int save = 0, all = 1;
        private async void button1_Click(object sender, EventArgs e)
        {
            OtherMethods.IlushaMethod(checkBox4);
            OtherMethods.PlaySound("Поехали");

            string[] names = new string[sources.Length];
            for (int i = 0; i < names.Length; i++)
                names[i] = $"Array{dataGridView1[1, i].Value}.txt";

            List<EllipseParam> param = new List<EllipseParam>();

            GetData();
            all = wcount * tcount;
            int alles = sources.Length * (sources.Length - 1);
            IProgress<int> progress = new Progress<int>((int val) => save = val);

            string[] wheredata = Expendator.GetStringArrayFromFile("WhereData.txt").Select(s => Path.Combine(s, "Разница")).ToArray();

            for (int i = 0; i < sources.Length; i++)
            {
                var itSource = sources[i];
                var otherSources = sources.Where(s => s != itSource).ToArray();
                var othernames = names.Where(n => n != names[i]).ToArray();
                var snames = symbols.Where(s => s != symbols[i]).ToArray();
                var ItElleps = new EllipseParam[sources.Length - 1];

                timer1.Start();
                for (int k = 0; k < otherSources.Length; k++)
                {
                    toolStripLabel1.Text = $"Замер {symbols[i]}, источник {snames[k]}, осталось {alles--}";
                    //await Task.Run(() => System.Threading.Thread.Sleep(1000));
                    var tuple = await Functions.GetMaximunFromArea(W,T, progress, new System.Threading.CancellationToken(),
                        tmin, step, pcount, othernames[k], Wavelet.Wavelets.LP, wheredata[i],byevery, epsForWaveletValues);

                    var s = Functions.GetFockS(tuple);
                    ItElleps[k] = new EllipseParam(otherSources[k].Center, itSource.Center, s, Библиотека_графики.Other.colors[i], $"{snames[k]} -> {symbols[i]}");            
                }
//                new Scheme(sources, ItElleps,$"Схема для замера {symbols[i]}").ShowDialog();
                param.AddRange(ItElleps);
                SetDefaltProgressBar();
                timer1.Stop();
                OtherMethods.PlaySound("ЗамерОбработан");
            }


            OtherMethods.PlaySound("СоздаемЭллипсы");
            MakeEllipses(param);
            SetDefaultStrip();
        }

        private void MakeEllipses(List<EllipseParam> param)
        {
            EllipseParam.WriteInFile("Ellipses.txt", param);
            new Scheme(sources, param.ToArray(),"Схема для всех замеров").Show();
        }
    }
}
