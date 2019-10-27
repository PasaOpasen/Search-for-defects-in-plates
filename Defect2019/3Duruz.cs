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

            TextCopy();
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
        private int all = 0, save = 0;
        private DateTime time;

        private async void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Circle fiCirc = Forms.UG.sourceIt.GetCircle;

            double w = textBox4.Text.ToDouble();
            double x0 = textBox5.Text.ToDouble(), X = textBox6.Text.ToDouble(), y0 = textBox7.Text.ToDouble(), Y = textBox8.Text.ToDouble();
            int xc = Convert.ToInt32(numericUpDown1.Value);
            int yc = Convert.ToInt32(numericUpDown2.Value);
            string tit = $"{Forms.UG.sourceIt.ToShortString()}, {((radioButton1.Checked) ? "ω" : "t")} = {w}, (xmin, xmax, xcount, ymin, ymax, ycount) = ({x0}, {X}, {xc}, {y0}, {Y}, {yc})";
            tit.Show();
            all = yc * xc;

            Normal2D[] norms = Forms.UG.sourceIt.Norms;
            Func<МатКлассы.Point, bool> filter = Forms.UG.sourceIt.Filter;

            timer1.Start();
            IProgress<int> progress = new Progress<int>((p) => { save = p; });

            Forms.UG.toolStripStatusLabel1.Text = "Вычисления запущены";
            time = DateTime.Now;
            Forms.UG.ShowCancelControls();
            Forms.UG.source = new System.Threading.CancellationTokenSource();
            System.Threading.CancellationToken token = Forms.UG.source.Token;
            if (radioButton1.Checked)
                await Task.Run(() =>
                {
                    МатКлассы.Waves.Circle.FieldToFileParallel("3D ur, uz.txt",
                        (double x, double y) => uxwMemoized(x, y, w, Forms.UG.sourceIt),
                        x0, X, xc, y0, Y, yc,
                        progress,
                        token,
                        filter,
                        tit
                                );
                });
            else
                await Task.Run(() =>
                {
                    МатКлассы.Waves.Circle.FieldToFileOLD("3D ur, uz.txt", "",
                        (double x, double y) =>
                        {
                            var t = Forms.UG.u(x, y, w, norms);
                            double cor = new Number.Complex(x - fiCirc.center.x, y - fiCirc.center.y).Arg;
                            return (t[0] * Math.Cos(cor) + t[1] * Math.Sin(cor), t[2]);
                        },
                        x0, X, xc, y0, Y, yc,
                        progress,
                        token,
                        filter,
                        tit,
                        false
                                );
                });
            TimeSpan ts = DateTime.Now - time;
            timer1.Stop();

            Forms.UG.HideCancelControls();

            if (token.IsCancellationRequested)
            {
                Forms.UG.toolStripStatusLabel1.Text = "Произошла отмена операции из 3D"; Forms.UG.progressBar1.Value = 0;
            }
            else
            {
                Forms.UG.toolStripStatusLabel1.Text = $"Вычисления выполнены и записаны в файл. Время вычислений: {ts}, среднее время вычилений 10-ти точек (sec.): {ts.Milliseconds.ToDouble() * 10 / xc / yc}. Вызван скрипт R";

                await Task.Run(() =>
                {
                    if (radioButton1.Checked) StartProcess("3Duruz.R");
                    else StartProcess("3Duxt.R");
                });

                if (radioButton1.Checked)
                {
                    var names = new string[]
                     {
                    "Все трёхмерные поверхности в pdf",
                    "Abs(ur)",
                    "Abs(uz)"
                     };
                    var st = new string[]
                    {
                    "3D ur, uz.pdf",
                    "urAbs.html",
                    "uzAbs.html"
                    };
                    var form = new Библиотека_графики.ManyDocumentsShower("3D поверхности для одного источника", names, st);
                    form.StartPosition = FormStartPosition.CenterScreen;
                    form.Show();
                }
                else
                {
                    new Библиотека_графики.PdfOpen($"Найденные поверхности при t = {w}", "3D ur, uz(title , " + tit + " ).pdf").Show();
                }

            }
            this.Close();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            label3.Text = "ω =";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            label3.Text = "t =";
        }

        private void наКругToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Forms.UG.наКругToolStripMenuItem_Click(sender, e);
            TextCopy();
        }

        private void наПолумесяцToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Forms.UG.наПолумесяцToolStripMenuItem_Click(sender, e);
            TextCopy();
        }
        private void TextCopy()
        {
            textBox9.Text = Forms.UG.textBox8.Text;
        }

        public void StartProcess(string fileName)
        {
            Process process = new Process();
            process.StartInfo.FileName = fileName;
            process.EnableRaisingEvents = true;

            process.Exited += (sender, e) =>
            {
                Forms.UG.toolStripStatusLabel1.Text = "Диалог с 3D закончен полностью";
                this.Close();
            };

            process.Start();
            process.WaitForExit();
        }
    }
}
