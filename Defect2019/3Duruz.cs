using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using МатКлассы;
using МатКлассы;
using static Functions;
using static МатКлассы.Waves;

namespace Defect2019
{
    public partial class _3Duruz : Form
    {
        public _3Duruz()
        {
            InitializeComponent();
            timer1.Interval = 1500;
            timer1.Tick += new EventHandler(Timer1_Tick);

            //label10.Hide();
            //numericUpDown3.Hide();

            ChooseCircleOfNot();
        }
        private void Timer1_Tick(object Sender, EventArgs e)
        {
            if (save > 0)
            {
                Forms.UG.toolStripStatusLabel1.Text = $"Считаются значения функции u. Сохранено значений PMRSN: {prmsnmem.Lenght}. Осталось найти примерно {all - save} точек";
            }
            else
            {
                Forms.UG.toolStripStatusLabel1.Text = $"Сохранено значений PMRSN: {prmsnmem.Lenght}";
            }
            Forms.UG.IsManyPRMSN();

            Forms.UG.progressBar1.Value = (save.ToDouble() / all * Forms.UG.progressBar1.Maximum).ToInt();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private int all = 0, save = 0;
        private DateTime time;

        private async void button1_Click(object sender, EventArgs e)
        {
            this.Hide();

            Waves.Circle circle = new Waves.Circle(new МатКлассы.Point(textBox1.Text.ToDouble(), textBox2.Text.ToDouble()), textBox3.Text.ToDouble());
            double w = textBox4.Text.ToDouble();
            double x0 = textBox5.Text.ToDouble(), X = textBox6.Text.ToDouble(), y0 = textBox7.Text.ToDouble(), Y = textBox8.Text.ToDouble();//X.Show();
            int xc = Convert.ToInt32(numericUpDown1.Value);
            int yc = Convert.ToInt32(numericUpDown2.Value);
            string tit = $"{((radioButton3.Checked)?"circle":"dcircle")} = (({circle.center.x}; {circle.center.y}), r = {circle.radius}), {((radioButton1.Checked) ? "ω" : "t")} = {w}, (xmin, xmax, xcount, ymin, ymax, ycount) = ({x0}, {X}, {xc}, {y0}, {Y}, {yc})";
            tit.Show();
            all = yc * xc;

            Normal2D[] norms = (radioButton3.Checked)?circle.GetNormalsOnCircle(Convert.ToInt32(numericUpDown3.Value)):Forms.UG.dCircle.GetNormalsOnDCircle();

            Func<МатКлассы.Point, bool> filter;
            if (radioButton3.Checked) filter = (МатКлассы.Point po) => circle.ContainPoint(po);
            else filter = (МатКлассы.Point po) => Forms.UG.dCircle.ContainPoint(po);

            Circle fiCirc = (radioButton3.Checked) ? new Circle(circle) : new Circle(Forms.UG.dCircle.BigCircle); 

            SaveFileDialog savedialog = new SaveFileDialog();
            savedialog.Title = "Сохранить рисунок как...";
            savedialog.FileName =/* Environment.CurrentDirectory + "\\*/"3D ur, uz.txt";
            savedialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            savedialog.OverwritePrompt = true;
            savedialog.CheckPathExists = true;
            savedialog.ShowHelp = true;
            //if (savedialog.ShowDialog() == DialogResult.OK)
            //{
            //    try
            //    {
            timer1.Start();
            IProgress<int> progress = new Progress<int>((p) => { save = p; });

            Forms.UG.toolStripStatusLabel1.Text = "Вычисления запущены";
            time = DateTime.Now;
            Forms.UG.stopshow();
            //Forms.UG.timer1.Start(); Forms.UG.timer2.Start();
            Forms.UG.source = new System.Threading.CancellationTokenSource();
            System.Threading.CancellationToken token = Forms.UG.source.Token;
            if(radioButton1.Checked)
            await Task.Run(() =>
            {
                МатКлассы.Waves.Circle.FieldToFile(savedialog.FileName,
                    (double x, double y/*, МатКлассы.Point normal*/) =>
                    {
                        var t = Forms.UG.ujRes(x, y, w, norms);
                        double cor = new Number.Complex(x - fiCirc.center.x, y - fiCirc.center.y).Arg;
                        return new Tuple<Number.Complex, Number.Complex>(/*Number.Complex.Sqrt(*/t[0]*Math.Cos(cor) + t[1]*Math.Sin(cor)/*)*/, t[2]);
                    },
                    x0, X, xc, y0, Y, yc,
                    //fiCirc,
                    progress,//ref Forms.UG.prbar,
                    token,
                    filter,
                    tit,
                    Convert.ToInt32(numericUpDown3.Value)
                            );
            });
            else
            await Task.Run(() =>
            {
                МатКлассы.Waves.Circle.FieldToFile(savedialog.FileName,
                    (double x, double y/*, МатКлассы.Point normal*/) =>
                    {
                        var t = Forms.UG.u(x, y, w, norms);
                        double cor = new Number.Complex(x - fiCirc.center.x, y - fiCirc.center.y).Arg;
                        return new Tuple<double, double>(t[0] * Math.Cos(cor) + t[1] * Math.Sin(cor), t[2]);
                    },
                    x0, X, xc, y0, Y, yc,
                    //fiCirc,
                    progress,//ref Forms.UG.prbar,
                    token,
                    filter,
                    tit,
                    false
                            );
            });
            //Forms.UG.timer1.Stop(); Forms.UG.timer2.Stop();
            TimeSpan ts = DateTime.Now - time;
            timer1.Stop();

            Forms.UG.stophide();

            if (token.IsCancellationRequested)
            {
                Forms.UG.toolStripStatusLabel1.Text = "Произошла отмена операции из 3D"; Forms.UG.progressBar1.Value = 0;
            }
            else
            {
                Forms.UG.toolStripStatusLabel1.Text = $"Вычисления выполнены и записаны в файл. Время вычислений: {ts}, среднее время вычилений 10-ти точек (sec.): {ts.Milliseconds.ToDouble() * 10 / xc / yc}. Вызван скрипт R";

                if (radioButton1.Checked) StartProcess("3Duruz.R");
                else StartProcess("3Duxt.R");
            }

            //}
            //catch (Exception ee)
            //{
            //    MessageBox.Show("Произошла ошибка", ee.Message,
            //    MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            //}          
        }

        private void button10_Click(object sender, EventArgs e)
        {
            new DC().Show();
        }
        private void ChooseCircleOfNot()
        {
            if (radioButton3.Checked)
            {
                label1.Show();
                textBox1.Show();
                textBox2.Show();
                label2.Show();
                textBox3.Show();
                label10.Show();
                numericUpDown3.Show();

                button10.Hide();
            }
            else
            {
                label1.Hide();
                textBox1.Hide();
                textBox2.Hide();
                label2.Hide();
                textBox3.Hide();
                label10.Hide();
                numericUpDown3.Hide();

                button10.Show();
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            ChooseCircleOfNot();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            ChooseCircleOfNot();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            label3.Text = "ω =";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            label3.Text = "t =";
        }

        public void StartProcess(string fileName)
        {
            Process process = new Process();
            process.StartInfo.FileName = fileName;
            process.EnableRaisingEvents = true;

            process.Exited += (sender, e) =>
            {
                Console.WriteLine($"Процесс завершен с кодом {process.ExitCode}");
                Process.Start("3D ur, uz.pdf");
                Process.Start("urAbs.html");
                Forms.UG.toolStripStatusLabel1.Text = "Диалог с 3D закончен полностью";
                this.Close();
            };

            process.Start();
        }
    }
}
