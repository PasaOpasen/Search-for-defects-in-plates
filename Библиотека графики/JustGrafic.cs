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
using System.Diagnostics;


namespace Библиотека_графики
{
    public partial class JustGrafic : Form
    {
        private int ind = 1;
        public int step;
        public JustGrafic(string title="График")
        {
            InitializeComponent();
            this.Text = title;
            step = numericUpDown1.Value.ToInt32();

            this.FormClosing += new FormClosingEventHandler((object o, FormClosingEventArgs f) =>
            {
                mas = null;
                arr = null;
                GC.Collect(1);
            });
        }

        public JustGrafic(string[] names, string[] filenames,string title= "График") : this(title)
        {
            fnames = filenames;
            

            this.chart1.Series.Clear();

            for (int i = 0; i < names.Length; i++)
                this.chart1.Series.Add(names[i]);

            arr = new double[names.Length][];
            arr2 = new double[names.Length][];

           // ReadDataOld();
            ReadData();

            for (int i = 0; i < names.Length; i++)
            {
                this.chart1.Series[i].BorderWidth = 1;
                this.chart1.Series[i].Color = colors[i];
                this.chart1.Series[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                this.chart1.Series[i].BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
                this.chart1.Series[i].Font = new Font("Arial", 16);

                for (int k = 1; k <= arr[i].Length; k += step)
                    this.chart1.Series[i].Points.AddXY(k, arr[i][k - 1]);

            }

            this.CreateCheckBoxes();
            this.ReDraw();

            string s = "";
            xmin = 1;
            xmax = arr[0].Length;
            double x, y, xold = double.NaN, yold = double.NaN;

            Stopwatch sp = new Stopwatch();
            sp.Start();
            
            this.chart1.MouseMove += new MouseEventHandler((object o, MouseEventArgs arg) =>
            {
                if(arg.Location.X<chart1.Size.Width*0.95&&arg.Location.Y < chart1.Size.Height * 0.95)
                if (sp.ElapsedMilliseconds > 100)
                {
                    x = chart1.ChartAreas[0].AxisX.PixelPositionToValue(arg.Location.X);
                    y = chart1.ChartAreas[0].AxisY.PixelPositionToValue(arg.Location.Y);
                    if (x >= xmin && x <= xmax && y >= ymin && y <= ymax)
                    {
                        if (x != xold || y != yold)
                        {
                            s = $"n = {(int)x}  val = {y.ToString(3)}";//s.Show();
                            toolTip1.SetToolTip(chart1, s);
                            xold = x;
                            yold = y;
                        }

                    }
                    else
                        toolTip1.SetToolTip(chart1, "");

                }

            });
        }

        private void ReadDataOld()
        {
            Parallel.For(0, fnames.Length, (int i) =>
             {
                 using (StreamReader f = new StreamReader(fnames[i]))
                 {
                     var p = (f.ReadToEnd()).Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                     arr[i] = new double[p.Length];
                     arr2[i] = new double[p.Length];
                     for (int k = 0; k < arr[i].Length; k++)
                     {
                         arr[i][k] = p[k].Replace('.', ',').ToDouble();
                         arr2[i][k] = arr[i][k];
                     }
                 }
             });
        }

        private void ReadData()
        {
            Parallel.For(0, fnames.Length, (int i) =>
            {
                string p;
                List<double> l = new List<double>();
                using (StreamReader f = new StreamReader(fnames[i]))
                {

                    p = f.ReadLine();
                    while(p!=null && p.Length>0)
                    {
                        l.Add(Convert.ToDouble(p.Replace('.', ',')));
                        p = f.ReadLine();
                    }
                }

                arr[i] = l.ToArray();
                arr2[i] = l.ToArray();
            });
        }

        /// <summary>
        /// Создаёт форму по массиву названий. Предполагается, что данные хранятся в файлах вида $"{s[i]}.txt"
        /// </summary>
        /// <param name="names"></param>
        public JustGrafic(string[] names, string title = "График") : this(names, Expendator.Map(names, (string s) => s + ".txt"),title)
        {

        }
        Color[] colors = new Color[] { Color.Blue, Color.Green, Color.Red, Color.Black, Color.Yellow, Color.Violet, Color.SkyBlue };


        private void button1_Click(object sender, EventArgs e)
        {
            Библиотека_графики.ForChart.SaveImageFromChart(chart1, $"Изображение от {DateTime.Now.ToString().Replace(':', ' ')}", System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Png);
        }

        public string[] fnames;
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
        }
        public void Lims()
        {
            Библиотека_графики.ForChart.SetAxisesY(ref chart1);
            SetLimsY();
        }

        public System.Windows.Forms.DataVisualization.Charting.DataPoint[][] mas;
        public double[][] arr, arr2;
        private double xmin, xmax, ymin, ymax;
        private bool first = true;

        public void ReSaveMas()
        {
            //Parallel.For(0, chart1.Series.Count, (int i) => {
            //    //chart1.Series[i].Points.Clear();

            //    for (int k = 1; k <= arr[i].Length; k += step)
            //    {
            //        //this.chart1.Series[i].Points.AddXY(k, arr[i][k - 1]);
            //        arr2[i][k - 1] = arr[i][k - 1];
            //    }
            //});

            //Lims();
            ReDraw();

            //for (int i = 0; i < chart1.Series.Count; i++)
            //{
            //    chart1.Series[i].Points.Clear();
            //    for (int k = 1; k <= arr[i].Length; k += step)
            //        this.chart1.Series[i].Points.AddXY(k, arr2[i][k - 1]);
               
            //}
            //Lims();
           // Библиотека_графики.ForChart.SetToolTips(ref chart1);
        }
        public void Cancel()
        {
            for(int i=0;i<chart1.Series.Count;i++)
            {
                chart1.Series[i].Points.Clear();

                for (int k = 1; k <= arr[i].Length; k += step)
                {
                    this.chart1.Series[i].Points.AddXY(k, arr[i][k - 1]);
                    arr2[i][k - 1] = arr[i][k - 1];
                }
            }        

            Lims();

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
                                for (int i = 0; i < arr[k].Length; i += step)
                                    chart1.Series[k].Points.AddXY(i + 1, arr2[k][i]);
                                chart1.Series[k].IsVisibleInLegend = true;
                                if (step <= 30) this.Refresh();
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
            SetNullInTextBox2(this.Controls);

            Lims();
            //Библиотека_графики.ForChart.SetToolTips(ref chart1);
        }

        private void SetLimsY()
        {
            ymin = chart1.ChartAreas[0].AxisY.Minimum;
            ymax = chart1.ChartAreas[0].AxisY.Maximum;

            textBox1.Text = ymax.ToString(5);
            textBox2.Text = ymin.ToString(5);
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

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            step = numericUpDown1.Value.ToInt32();
            ReDraw();
        }

        private void вернутьИсходныеМассивыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        private void сохранитьИзображениеВРабочуюПапкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Библиотека_графики.ForChart.SaveImageFromChart(chart1, $"Изображение от {DateTime.Now.ToString().Replace(':',' ')}", System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Png);
        }

        private void сохранитьНовыеМассивыВИсходныеФайлыToolStripMenuItem_Click(object sender, EventArgs e)
        {
           // for (int i = 0; i < fnames.Length; i++)
                Parallel.For(0, fnames.Length, (int i) => { 
                using (StreamWriter t = new StreamWriter(fnames[i]))
                {
                    for (int j = 0; j < arr[i].Length; j++)
                        t.WriteLine(arr2[i][j].ToString().Replace(',', '.'));
                }
                });
        }

        private void опцииToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            сохранитьНовыеМассивыВИсходныеФайлыToolStripMenuItem_Click(sender, e);

            arr = null;
            arr2 = null;
            
            //this.Dispose();
            GC.Collect();
            this.Close();
        }

        private void открытьТрапецевидноеОкноToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Trapezi trap = new Trapezi(this);

            this.FormClosing += new FormClosingEventHandler((object o, FormClosingEventArgs a) =>
            {
                if (!trap.IsDisposed)
                    trap.Close();
            });
            trap.Show();
        }

    }
}
