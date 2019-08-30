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

namespace Работа2019
{
    public partial class Helper : Form
    {
        public Helper(double a=-1,double b=1, int n=10)
        {
            InitializeComponent();
            textBox2.Text = a.ToRString();
            textBox3.Text = b.ToRString();
            textBox4.Text = ((b - a) / (n - 1)).ToRString();


            label8.Hide();
        }
        double t0, T, dt0, dT, h;

        private void button1_Click(object sender, EventArgs e)
        {
            Read();
            label8.Text = "Вычисляется...";
            Search();

            label8.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        int count,c;
        double[] mas;

        private void Read()
        {
            t0 = textBox2.Text.ToDouble();
            T = textBox3.Text.ToDouble();
            h = textBox4.Text.ToDouble();
            c = (int)((T - t0) / h - 1);
            count = numericUpDown1.Value.ToInt32();

            double d1, d2;
            if (radioButton1.Checked)
            {
                dt0 = textBox5.Text.ToDouble();
                dT = textBox6.Text.ToDouble();
            }
            else
            {
                d1 = textBox7.Text.ToDouble();
                d2 = textBox8.Text.ToDouble();
                dt0 = d1- d2 ;
                dT = d2 + d1;
            }

            mas = textBox1.Text.Replace("\n", "").ToDoubleMas().Distinct().ToArray() ;

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Search()
        {
            int i = c;
            double[] res;
            bool b;
            while (true)
            {
                res = Expendator.Seq(t0, T, i);
                b = false;
                for(int k=0;k<mas.Length;k++)
                    if(res.Contains(mas[k]))
                    {
                        b = true;
                        break;
                    }
                if (!b)
                if(res.Where((double n)=>n>=dt0 && n<=dT).Count()>=count)
                    {
                        label8.Text = $"Число точек: {i}; шаг: {res[1]-res[0]}";
                        return;
                    }


                i++;
            }
        }

    }
}
