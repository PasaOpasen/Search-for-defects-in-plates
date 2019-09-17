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
using System.Diagnostics;

namespace Работа2019
{
    public partial class Helper : Form
    {
        double tmin, tmax, dtmin, dtmax, hmax;
        int count, c, tcount = 0;
        /// <summary>
        /// Массив значений, которые не должны войти в разбиение
        /// </summary>
        double[] mas;

        public Helper(double a = -1, double b = 1, int n = 10)
        {
            InitializeComponent();
            textBox2.Text = a.ToRString();
            textBox3.Text = b.ToRString();
            textBox4.Text = ((b - a) / (n - 1)).ToRString();

            SetDt(a, b, 1.2);
            SetEvents();
            label8.Hide();
        }
        private void SetDt(double a, double b, double h)
        {
            double tmp = (b - a) * 0.66;
            double d1 = a + tmp, d2 = d1 + h;

            textBox5.Text = d1.ToRString();
            textBox6.Text = d2.ToRString();
            textBox7.Text = ((d1 + d2) / 2).ToRString();
            textBox8.Text = Math.Abs((d1 - d2) / 2).ToRString();
        }
        private void SetEvents()
        {
            void SomeEvent() => label8.Hide();
            var ev =new EventHandler( (object o, EventArgs e) => SomeEvent());
            textBox1.TextChanged += ev;
            textBox2.TextChanged += ev;
            textBox3.TextChanged += ev;
            textBox4.TextChanged += ev;
            textBox5.TextChanged += ev;
            textBox6.TextChanged += ev;
            textBox7.TextChanged += ev;
            textBox8.TextChanged += ev;
            numericUpDown1.ValueChanged += ev;
        }

        public Helper(TextBox tminBox, TextBox tmaxBox, NumericUpDown tcountBox) : this(tminBox.Text.ToDouble(), tmaxBox.Text.ToDouble(), tcountBox.Value.ToInt32())
        {
            this.FormClosing += (object o, FormClosingEventArgs e) =>
              {
                  if (tcount > 0)
                  {
                      var st = new string[]
                      {
                          "Установленные данные:",
                          $"\ttmin = {tmin}",
                          $"\ttmax = {tmax}",
                          $"\ttcount = {tcount}",
                          "Зафиксировать их в вызывающей форме?"
                      };
                      if (MessageBox.Show(Expendator.StringArrayToString(st), "Исправить данные на форме?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                      {
                          tminBox.Text = tmin.ToRString();
                          tmaxBox.Text = tmax.ToRString();
                          tcountBox.Value = tcount;
                      }
                  }
              };
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Read();
            label8.Text = "Вычисляется...";

            if (CheckDt())
            {
            Search();
            label8.Show();
            }
        }


        private void Read()
        {
            tmin = textBox2.Text.ToDouble();
            tmax = textBox3.Text.ToDouble();
            count = numericUpDown1.Value.ToInt32();
            ReadDt();

            hmax = Math.Min( textBox4.Text.ToDouble(),(dtmax-dtmin)/(count-1));          
            c = (int)((tmax - tmin) / hmax - 1);
            
            mas = textBox1.Text.Replace("\n", "").ToDoubleMas().Distinct().ToArray();

        }
        private void ReadDt()
        {
            double d1, d2;
            if (radioButton1.Checked)
            {
                dtmin = textBox5.Text.ToDouble();
                dtmax = textBox6.Text.ToDouble();
            }
            else
            {
                d1 = textBox7.Text.ToDouble();
                d2 = textBox8.Text.ToDouble();
                dtmin = d1 - d2;
                dtmax = d2 + d1;
            }
        }
        private bool CheckDt()
        {
            //if (dtmin >= tmin && dtmax <= tmax)
            if(!(dtmax<=tmin || dtmin>=tmax))
                return true;
            MessageBox.Show("Отрезок с указанными точками не имеет непрерывного пересечения с исходным отрезком. Перепроверьте данные", "Ошибка в данных", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            return false;
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
                res = Expendator.Seq(tmin, tmax, i);
                b = false;
                for (int k = 0; k < mas.Length; k++)
                    if (res.Contains(mas[k]))
                    {
                        b = true;
                        break;
                    }
                if (!b)
                    if (res.Where((double n) => n >= dtmin && n <= dtmax).Count() >= count)
                    {
                        label8.Text = $"Число точек: {i}; шаг: {res[1] - res[0]}";
                        tcount = i;
                        return;
                    }

                i++;
            }
        }

    }
}
