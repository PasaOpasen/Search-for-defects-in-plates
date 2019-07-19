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


namespace Defect2019
{
    public partial class JustGrafic : Form
    {
        private int ind = 1;
        public JustGrafic()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler((object o, FormClosingEventArgs f) =>
            {
                mas = null;
                GC.Collect();
            });

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Библиотека_графики.ForChart.SaveImageFromChart(chart1, $"Изображение {ind++}", System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Png);
        }

        public void CreateCheckBoxes()
        {
            bool str(string text)
            {
                string s = text.Substring(8, text.Length - 8);
                int k = Convert.ToInt32(s);//$"{k - 1} {chart1.Series.Count}".Show();
                return k - 1 < chart1.Series.Count;
            }

            void SetNullInTextBox(Control.ControlCollection control)
            {
                foreach (Control _control in control)
                {
                    if (_control is CheckBox)
                    {
                        if (str(_control.Name))
                        {
                            //"yes".Show();
                            (_control as CheckBox).Checked = true;
                            _control.Show();
                        }
                        else
                        {
                            //"No".Show();
                            (_control as CheckBox).Checked = false;
                            _control.Hide();
                        }
                    }
                    if (_control.Controls.Count > 0)
                    {
                        SetNullInTextBox(_control.Controls);
                    }
                }
            }

            SetNullInTextBox(this.Controls);


            //for (int i = 0; i < chart1.Series.Count; i++)
            //{
            //    CheckBox c =(CheckBox) this.Controls["checkBox" + (i + 1).ToString()];
            //    c.Checked = true;
            //    c.Show();
            //}
            //for (int i = chart1.Series.Count; i < 10; i++)
            //{
            //    CheckBox c = (CheckBox)this.Controls["checkBox" + (i + 1).ToString()];
            //    c.Checked = false;
            //    c.Hide();
            //}
        }
        public void Lims()
        {
            Библиотека_графики.ForChart.SetAxisesY(ref chart1);
            SetLimsY();
        }

        System.Windows.Forms.DataVisualization.Charting.DataPoint[][] mas;
        private bool first = true;
        private void SaveMas()
        {
            mas = new System.Windows.Forms.DataVisualization.Charting.DataPoint[chart1.Series.Count][];
            for (int i = 0; i < chart1.Series.Count; i++)
                mas[i] = chart1.Series[i].Points.ToArray();
        }

        private void ReDraw()
        {
            int str(string text)
            {
                string s = text.Substring(8, text.Length - 8);
                int k = Convert.ToInt32(s);//$"{k - 1} {chart1.Series.Count}".Show();
                return k - 1;
            }

            void SetNullInTextBox2(Control.ControlCollection control)
            {
                foreach (Control _control in control)
                {
                    if (_control is CheckBox)
                    {
                        int k = str(_control.Name);
                        if (k < chart1.Series.Count)
                        {
                            chart1.Series[k].Points.Clear();
                            if ((_control as CheckBox).Checked)
                            {
                                for (int i = 0; i < mas[k].Length; i++)
                                    chart1.Series[k].Points.AddXY(mas[k][i].XValue, mas[k][i].YValues[0]);
                                chart1.Series[k].IsVisibleInLegend = true;
                            }
                            else
                            {
                                chart1.Series[k].IsVisibleInLegend = false;
                            }
                        }
                    }

                    if (_control.Controls.Count > 0)
                    {
                        SetNullInTextBox2(_control.Controls);
                    }
                }
            }

            if (first)
            {
                SaveMas();
                first = false;
            }
            SetNullInTextBox2(this.Controls);

            Lims();
            Библиотека_графики.ForChart.SetToolTips(ref chart1);
        }

        private void SetLimsY()
        {
            textBox1.Text = chart1.ChartAreas[0].AxisY.Maximum.ToString();
            textBox2.Text = chart1.ChartAreas[0].AxisY.Minimum.ToString();
        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisY.Maximum = textBox1.Text.ToDouble();
            chart1.ChartAreas[0].AxisY.Minimum = textBox2.Text.ToDouble();
        }
    }
}
