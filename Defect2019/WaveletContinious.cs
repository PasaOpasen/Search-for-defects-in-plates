﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using МатКлассы;

namespace Работа2019
{
    public partial class WaveletContinious : Form
    {
        private readonly double step = 16E-9;
        private readonly int pcount = 150000;

        private static string symbols = "ABCDEFGH";
        private Source[] sources;
        private double wmin, wmax, tmin, tmax;
        private int count;

        public WaveletContinious(Source[] array)
        {
            InitializeComponent();
            sources = array;


            SetDefaultStrip();
            SetDataGrid();

        }

        private void SetDefaultStrip()
        {
            toolStripLabel1.Text = "Готов";
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
            wmin = textBox1.Text.ToDouble();
            wmax = textBox2.Text.ToDouble();
            tmin = textBox3.Text.ToDouble();
            tmax = textBox4.Text.ToDouble();
            count = numericUpDown1.Value.ToInt32();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string[] names = new string[sources.Length];
            for (int i = 0; i < names.Length; i++)
                names[i] = $"Array{dataGridView1[1, i].Value}.txt";

            List<EllipseParam> param = new List<EllipseParam>();

            GetData();
            int all = count * count;
            IProgress<int> progress = new Progress<int>((int val) => 
            {
                var d = Expendator.GetProcent(val, all, 2);
                toolStripLabel2.Text = $"{d}%";
                toolStripProgressBar1.Value =(int)( d / 100 * toolStripProgressBar1.Maximum);
            });








            SetDefaultStrip();
        }
    }
}