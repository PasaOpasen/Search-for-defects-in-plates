﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Functions;
using Практика_с_фортрана;
using МатКлассы;
using static РабКонсоль;

namespace Defect2019
{
    public partial class ParametrsQu : Form
    {
        public ParametrsQu()
        {
            InitializeComponent();
            textBox6.Text = ro.ToString();
            textBox8.Text = h.ToString();
            textBox5.Text = lamda.ToString();
            textBox9.Text = mu.ToString();
            textBox1.Text = РабКонсоль.steproot.ToString();
            textBox2.Text = РабКонсоль.epsroot.ToString();
            textBox3.Text = РабКонсоль.polesBeg.ToString();
            textBox12.Text = РабКонсоль.polesEnd.ToString();
            numericUpDown1.Value = РабКонсоль.countroot;

            textBox4.Text = (wbeg * (1e6 / (2 * Math.PI))).ToString();
            textBox10.Text = (wend * (1e6 / (2 * Math.PI))).ToString();
            textBox11.Text = wc.ToString();
            numericUpDown2.Value = wcount;

            numericUpDown3.Value = РабКонсоль.animatime;
            numericUpDown4.Value = РабКонсоль.animacycles;
            numericUpDown5.Value = РабКонсоль.clastersCount;
            numericUpDown6.Value = РабКонсоль.cyclescount;

            textBox7.Text = BeeHiveAlgorithm.w.ToRString();
            textBox13.Text = BeeHiveAlgorithm.fp.ToRString();
            textBox14.Text = BeeHiveAlgorithm.fg.ToRString();

            this.FormClosing += (o, e) =>
            {
                if (!UGrafic.wchange)
                    Работа2019.SoundMethods.Back();
            };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ro = Convert.ToDouble(textBox6.Text);
            h = Convert.ToDouble(textBox8.Text);
            lamda = Convert.ToDouble(textBox5.Text);
            mu = Convert.ToDouble(textBox9.Text);
            РабКонсоль.steproot = Convert.ToDouble(textBox1.Text);
            РабКонсоль.epsroot = Convert.ToDouble(textBox2.Text);
            РабКонсоль.countroot = Convert.ToInt32(numericUpDown1.Value);
            РабКонсоль.polesBeg = Convert.ToDouble(textBox3.Text);
            РабКонсоль.polesEnd = Convert.ToDouble(textBox12.Text);


            wc = textBox11.Text.ToDouble();
            wbeg = textBox4.Text.ToDouble() * pimult2 * 1e-6;
            wend = textBox10.Text.ToDouble() * pimult2 * 1e-6;
            wcount = Convert.ToInt32(numericUpDown2.Value);
            UGrafic.wchange = true;

            РабКонсоль.animatime = Convert.ToInt32(numericUpDown3.Value);
            РабКонсоль.animacycles = Convert.ToInt32(numericUpDown4.Value);
            РабКонсоль.clastersCount = Convert.ToInt32(numericUpDown5.Value);
            РабКонсоль.cyclescount = Convert.ToInt32(numericUpDown6.Value);

            BeeHiveAlgorithm.w = textBox7.Text.ToDouble();
            BeeHiveAlgorithm.fp = textBox13.Text.ToDouble();
            BeeHiveAlgorithm.fg = textBox14.Text.ToDouble();

            AfterChaigeData();
            Работа2019.SoundMethods.OK();
            this.Close();
        }
    }
}
