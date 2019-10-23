using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using МатКлассы;
using static МатКлассы.Number;
using Библиотека_графики;
using static Functions;
using Defect2019;
using Point = МатКлассы.Point;
using static МатКлассы.Waves;
using System.IO;
using static РабКонсоль;
using JR.Utils.GUI.Forms;

namespace Практика_с_фортрана
{
    public partial class UGrafic : Form
    {
        #region конструктор и простые методы формы
        static UGrafic()
        {

        }

        public UGrafic()
        {
            InitializeComponent();
            SetSource();
            
            this.FormClosed += (object o, FormClosedEventArgs e) => HankelTuple = HankelTupleWith;

            label8.BackColor = Color.Transparent;

            colorDialog1.FullOpen = true;
            colorDialog1.Color = Color.Green;          
            listBox1.SelectedItem = "По лучу от центра окружности через точку";

            SetTimers();
            HideCancelControls();
            toolStripStatusLabel1.Text = "Ожидание команды";

            ClearSeries();

            SetMemoized();
            SetArrays();

            kGrafic.ReadModelData();
        }
        private void SetMemoized()
        {
            var uf = new Memoize<Tuple<double, double, double, Normal2D[], Func<double, Complex>>, Complex[]>((Tuple<double, double, double, Normal2D[], Func<double, Complex>> t) => UF(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5));
            UFMemoized = (double x, double y, double w, Normal2D[] n, Func<double, Complex> f) => uf.Value(new Tuple<double, double, double, Normal2D[], Func<double, Complex>>(x, y, w, n, f));

            var ur = new Memoize<Tuple<double, double, double, Normal2D[]>, Complex[]>((Tuple<double, double, double, Normal2D[]> t) => URes(t.Item1, t.Item2, t.Item3, t.Item4));
            UResMemoized = (double x, double y, double w, Normal2D[] n) => ur.Value(new Tuple<double, double, double, Normal2D[]>(x, y, w, n));

            this.FormClosing += (object o, FormClosingEventArgs e) =>
              {
                  uf.Dispose();
                  ur.Dispose();
              };
        }
        private void SetArrays() => CreateArrays(0);

        public System.Threading.CancellationTokenSource source;

        /// <summary>
        /// Задать источник
        /// </summary>
        /// <param name="s"></param>
        public void SetSource(Source? s = null)
        {
            if (s == null)
            {
                var p = new Circle(1, 1, 0.5);
                var norms = p.GetNormalsOnCircle(40);
                s = new Source(p, norms, GetFmas());
            }
            sourceIt = ((Source)(s)).dup;
            textBox8.Text = sourceIt.ToString();
        }
        public void SetTimers()
        {
            timer1.Interval = 1800;
            timer1.Tick += new EventHandler(Timer1_Tick);
            timer2 = new Timer() { Interval = 1800 };
            timer2.Tick += (object Sender, EventArgs e) => toolStripStatusLabel1.Text = $"Мемоизация PMRSN. Сохранено значений: {prmsnmem.Lenght}";
        }
        private void Timer1_Tick(object Sender, EventArgs e)
        {
            if (prbar.Sum() > 0)
            {
                toolStripStatusLabel1.Text = $"Начальная мемоизация завершена. Считаются значения функции u. Сохранено значений PMRSN: {prmsnmem.Lenght}. Осталось найти примерно {prbar.Length - prbar.Sum()} точек";
                timer2.Stop();
                IsManyPRMSN();
            }
            progressBar1.Value = (prbar.Sum().ToDouble() / prbar.Length * progressBar1.Maximum).ToInt();
        }

        /// <summary>
        /// Скрыть кнопки для отмены асинхронной операции
        /// </summary>
        public void HideCancelControls()
        {
            label8.Hide();
            button8.Hide();
        }
        /// <summary>
        /// Показать кнопки для отмены асинхронной операции
        /// </summary>
        public void ShowCancelControls()
        {
            label8.Show();
            button8.Show();
        }

        public int[] prbar;
        public int many;
        public void IsManyPRMSN()
        {
            int s = 0;
            switch (РабКонсоль.NodesCount)
            {
                case FuncMethods.DefInteg.GaussKronrod.NodesCount.GK15:
                    s = 15;
                    break;
                case FuncMethods.DefInteg.GaussKronrod.NodesCount.GK21:
                    s = 21;
                    break;
                case FuncMethods.DefInteg.GaussKronrod.NodesCount.GK31:
                    s = 31;
                    break;
                case FuncMethods.DefInteg.GaussKronrod.NodesCount.GK41:
                    s = 41;
                    break;
                case FuncMethods.DefInteg.GaussKronrod.NodesCount.GK51:
                    s = 51;
                    break;
                case FuncMethods.DefInteg.GaussKronrod.NodesCount.GK61:
                    s = 61;
                    break;
            }
            many = 1300000 * s / 31;
            if (prmsnmem.Lenght >= many)
            {
                errorProvider1.SetError(button8, "Достаточно много значений PMRSN");
            }
        }
        #endregion

        #region базовые поля и функции u(w), uxwMemoized(w)

        public Source sourceIt;

        double[] xval, uRval, uIval, umodval;
        double[] xvalz, uRz, uIz, uAz;
        double[] uRvalRes, uIvalRes, umodvalRes, uRzRes, uIzRes, uAzRes;

        Color color = Color.Blue;
        double beg, end;
        public Func<double, double, double, Normal2D[], Complex[]> U = (double x, double y, double w, Normal2D[] normal) =>
       {
           Vectors poles = PolesMasMemoized(w);
           double min = poles.Min * 0.5, max = poles.Max * 1.5;

           //подынтегральная функция
           FuncMethods.DefInteg.GaussKronrod.ComplexVectorFunc tmp = (Complex a, int n) => Ksum(a, x, y, w, normal, (Point t) => { return new CVectors(new Complex[] { t.x, t.y, 0 }); }).ComplexMas;

           //интеграл
           return FuncMethods.DefInteg.GaussKronrod.DINN5_GK(tmp, min, min, min, max, РабКонсоль.tm, РабКонсоль.tp, РабКонсоль.eps, РабКонсоль.pr, РабКонсоль.gr, 3, РабКонсоль.NodesCount).Div(pimult2);
       };

        public Func<double, double, double, Normal2D[], Complex[]> URes = (double x, double y, double w, Normal2D[] s) => (KsumRes(x, y, w, s, (Point t) => { return new Vectors(t.x, t.y, 0); })).ComplexMas;

        /// <summary>
        /// Мемоизированная u(x,w) по вычетам. Её мемоизация не помогает на 3D-графиках, так как там уже нет повторных вычислений при меняющемся времени
        /// </summary>
        public Func<double, double, double, Normal2D[], Complex[]> UResMemoized;
        public Func<double, double, double, Normal2D[], Complex> vz = (double x, double y, double w, Normal2D[] nor) => w * Forms.UG.UResMemoized(x, y, w, nor)[2];
        #endregion

        #region u(x,t)
        public static bool wchange = false;

        public Func<double, double, double, Normal2D[], Func<double, Complex>, Complex[]> UF = (double x, double y, double w, Normal2D[] normal, Func<double, Complex> f) => Expendator.Mult(Forms.UG.UResMemoized(x, y, w, normal), f(w));
        public Func<double, double, double, Normal2D[], Func<double, Complex>, Complex[]> UFMemoized;


        /// <summary>
        /// Итоговая функция (через тестовую функцию f)
        /// </summary>
        public Func<double, double, double, Func<double, Complex>, Normal2D[], double[]> uxt = (double x, double y, double t, Func<double, Complex> f, Normal2D[] normal) =>
         {
             double[] wmas = РабКонсоль.wmas;

             CVectors[] c = new CVectors[wcount];
             Parallel.For(0, wcount, (int i) =>
                {
                 c[i] = new CVectors(Expendator.Mult(Forms.UG.UResMemoized(x, y, wmas[i], normal), f(wmas[i])));
             });
             return ((c * Fi(wmas, t)).Re / Math.PI).DoubleMas;
         };
        /// <summary>
        /// Итоговая функция (через вычисленные массивы w и f(w))
        /// </summary>
        public Func<double, double, double, Tuple<double[], Complex[]>, Normal2D[], double[]> uxt2 = (double x, double y, double t, Tuple<double[], Complex[]> tuple, Normal2D[] normal) =>
        {
            double[] w = tuple.Item1;
            Complex[] fw = tuple.Item2;

            CVectors[] c = new CVectors[wcount];
            Parallel.For(0, wcount, (int i) =>
            {
                c[i] = new CVectors(Expendator.Mult(Forms.UG.UResMemoized(x, y, w[i], normal), fw[i]));
            });
            return ((c * Fi(w, t)).Re / Math.PI).DoubleMas;
        };

        public static Func<double, Complex> basefunc = F1;
        public Func<double, double, double, Normal2D[], double[]> u = (double x, double y, double t, Normal2D[] normal) => Forms.UG.uxt(x, y, t, basefunc, normal);

        #endregion

        #region более сложные методы формы (нажатие на боксы и т п)
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            checkBox1_CheckedChanged(sender, e);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            checkBox1_CheckedChanged(sender, e);
        }

        private void UGrafic_Load(object sender, EventArgs e)
        {

        }

        private void CheckBoxesShowHide(bool show = true)
        {
            if (show)
            {
                checkBox1.Show();
                checkBox2.Show();
                checkBox3.Show();
                checkBox4.Show();
                checkBox5.Show();
                checkBox6.Show();
                checkBox8.Show();
                checkBox9.Show();
                checkBox11.Show();
                checkBox12.Show();
                checkBox13.Hide();
            }
            else
            {
                checkBox1.Hide(); checkBox1.Checked = false;
                checkBox2.Hide(); checkBox2.Checked = false;
                checkBox3.Hide(); checkBox3.Checked = false;
                checkBox4.Hide(); checkBox4.Checked = false;
                checkBox5.Hide(); checkBox5.Checked = false;
                checkBox6.Hide(); checkBox6.Checked = false;
                checkBox8.Hide(); checkBox8.Checked = false;
                checkBox9.Hide(); checkBox9.Checked = false;
                checkBox11.Hide(); checkBox11.Checked = false;
                checkBox12.Hide(); checkBox12.Checked = false;
                checkBox7.Checked = true;
                checkBox10.Checked = true;
                checkBox13.Show();
            }

        }

        public void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            void TShow()
            {
                    label5.Show();
                    textBox6.Show();
                    label3.Show();
                    textBox2.Show();
                    textBox3.Show();
            }
            void THide()
            {
                label5.Hide();
                textBox6.Hide();
                label3.Hide();
                textBox2.Hide();
                textBox3.Hide();
            }
            void Sdist()
            {
                    if (textBox2.Text.ToDouble() == 0) textBox2.Text = РабКонсоль.polesBeg.ToString();
                    label7.Text = "dist =";
            }
            void Sw()
            {
                    label7.Text = "w =";
                    textBox7.Text = "3";
            }

            switch (listBox1.SelectedIndex)
            {
                case 0:
                    TShow();
                    Sw();
                    CheckBoxesShowHide(true);
                    numericUpDown1.Value = 100;
                    break;
                case 1:
                    TShow();
                    Sdist();
                    textBox7.Text = "20";
                    CheckBoxesShowHide(true);
                    numericUpDown1.Value = 100;
                    break;
                case 3:
                    TShow();
                    textBox2.Text = "0,01";
                    textBox3.Text = "100";
                    Sdist();
                    textBox7.Text = "100";
                    CheckBoxesShowHide(false);
                    numericUpDown1.Value = 300;
                    break;
                default:
                    THide();
                    Sw();
                    CheckBoxesShowHide(true);
                    numericUpDown1.Value = 100;
                    break;
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            checkBox1_CheckedChanged(sender, e);
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            checkBox1_CheckedChanged(sender, e);
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            checkBox1_CheckedChanged(sender, e);
        }


        private void label8_Click(object sender, EventArgs e)
        {
        string message = "При неправильном выборе параметра tm или отрезка обхода полюсов интеграл может проходить через полюс (либо слишком близко), из-за чего происходят NaN от деления на 0 или бесконечности от суммирования больших чисел." + Environment.NewLine +
            "В этом случае метод интегрирования дробит шаг и начинает считать намного больше значений, в следствие этого значений PMRSN сохраняется слишком много. Из-за близости полюсов программа работает в разы дольше, а в конечных данных появляются выбросы." + Environment.NewLine +
            "В этом случае, опираясь на число PMRSN, следует прервать программу и изменить параметры интегрирования. Число значений PMRSN (при фикс. частоте и GK31) бывает: хорошее (3-4к), среднее (8-10к), плохое (>12к). Программу однозначно следует прерывать при числе PMRSN, большем 16к, если только не взята большая размерность. Если частота изменяется, указанные числа умножаются на количество разных частот, а при изменении GK - на соответсвующее отношение." + Environment.NewLine +
            "Этот способ поможет только 1 раз, так как сохранённые значения не удаляются (зачем?). Данные сотрутся автоматически при изменении условий задачи";

            MessageBox.Show(message, "Когда отменять асинхронную операцию?", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        public void SourceKill() => source.Cancel();

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

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();
        }

        private void графикиPRMSNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new PRMSN_Memoized().Show();
        }

        private void параметрыЗадачиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ParametrsQu().ShowDialog();
        }

        private void параметрыПодсчётаИнтегралаDINNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new DINN5().ShowDialog();
        }

        private void распространениеВолныВПространствеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTuple();
            new WaveContinious(new Source[] { sourceIt }).ShowDialog();
        }

        public async void анимацияПоПоследнимСохранённымДаннымToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists("3D ur, uz(info).txt"))
            {
                MessageBox.Show("Не найдено файла со списком изображений", $"Нет списка \"3D ur, uz(info).txt\"", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            string[] st = Expendator.GetStringArrayFromFile("3D ur, uz(info).txt", true);
            for (int i = 0; i < st.Length; i++)
                if (!File.Exists(st[i]))
                {
                    FlexibleMessageBox.Show($"Не найдено файла \"{st[i]}\" из списка изображений. Анимация не готова", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    if (FilesUrUzExist(st) && MessageBox.Show($"Поскольку все нужные текстовые файлы в наличии, изображения можно восстановить. Создать анимацию? (может занять около 15 минут)", "Перерисовка", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        await Task.Run(() => Expendator.StartProcessOnly("ReDraw3Duxt2.r", true));
                        break;
                    }
                    else
                        return;
                }


            new Anima(st).Show();
        }

        private static bool FilesUrUzExist(string[] pngnames)
        {
            string s;
            for (int i = 0; i < pngnames.Length; i++)
            {
                s = pngnames[i].Replace(".png", "").Replace("3D ", "");
                if (!File.Exists(s + "(ur).txt") || !File.Exists(s + "(ur).txt"))
                    return false;
            }
            return true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SourceKill();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ReDraw();
        }

        #endregion

        public Tuple<double, double, double, Normal2D[], Func<МатКлассы.Point, bool>> tuple;

        internal void наКругToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Sources(true).ShowDialog();
        }

        internal void наПолумесяцToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new DC().ShowDialog();
        }

        private void сохранитьИзображениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = $"Функции ur,uz";
            SaveFileDialog savedialog = new SaveFileDialog
            {
                Title = "Сохранить рисунок как...",
                FileName = name,
                Filter = "Image files (*.png)|*.png|All files (*.*)|*.*",

                OverwritePrompt = true,
                CheckPathExists = true,
                ShowHelp = true
            };
            if (savedialog.ShowDialog() == DialogResult.OK)
            {
               Работа2019.SoundMethods.OK();
                try
                {
                    chart1.SaveImage(savedialog.FileName, System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Png);
                    chart1.SaveImage(savedialog.FileName.Substring(0, savedialog.FileName.IndexOf(".png")) + ".emf", System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Emf);

                    StreamWriter fs = new StreamWriter(savedialog.FileName.Substring(0, savedialog.FileName.IndexOf(".png")) + ".txt");
                    for (int i = 0; i < xval.Length; i++)
                        fs.WriteLine($"{xval[i]} {uRval[i]} {uIval[i]} {umodval[i]} {uRz[i]} {uIz[i]} {uAz[i]} {uRvalRes[i]} {uIvalRes[i]} {umodvalRes[i]} {uRzRes[i]} {uIzRes[i]} {uAzRes[i]}");

                    fs.Close();
                }
                catch (Exception ee)
                {
                    MessageBox.Show("Рисунок не сохранён", ee.Message,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void изменитьЦветФонаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // установка цвета формы
            this.color = colorDialog1.Color;
            chart1.BackColor = this.color;
        }

        public void SetTuple()
        {
            tuple = new Tuple<double, double, double, Normal2D[], Func<Point, bool>>(sourceIt.Center.x, sourceIt.Center.y, sourceIt.radius, sourceIt.Norms, sourceIt.Filter);
        }

        private void dГрафикКомпонентаВолныToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Запущен диалог относительно 3D графика";
            new _3Duruz().ShowDialog();
        }

        private void ClearSeries()
        {
            for (int i = 0; i < chart1.Series.Count; i++)
            {
                chart1.Series[i].Points.Clear();
                chart1.Series[i].IsVisibleInLegend = false;
            }
        }
        private double GetCorner() => textBox6.Text.ToDouble() * (Math.PI / 180);
        private Tuple<Complex[], Complex[]> GetTupF(double x, double y, double w, Normal2D[] n) => new Tuple<Complex[], Complex[]>(U(x, y, w, n), UResMemoized(x, y, w, n));
        private void CreateArrays(int k)
        {
            xval = new double[k]; uRval = new double[k]; uIval = new double[k]; umodval = new double[k];
            xvalz = new double[k]; uRz = new double[k]; uIz = new double[k]; uAz = new double[k];
            uRvalRes = new double[k]; uIvalRes = new double[k]; umodvalRes = new double[k];
            uRzRes = new double[k]; uIzRes = new double[k]; uAzRes = new double[k];
        }
        public async void button1_Click(object sender, EventArgs e)
        {
            HankelTuple = HankelTupleClear;
            ClearSeries();
            toolStripStatusLabel1.Text = "Чтение данных и генерация переменных";

            int k = Convert.ToInt32(numericUpDown1.Value);
            CreateArrays(k);

            double cor = GetCorner();
            beg = textBox2.Text.ToDouble();
            end = textBox3.Text.ToDouble();
            Waves.Circle circle = sourceIt.GetCircle;

            double h = (end - beg) / (k - 1);
            Waves.Normal2D[] norms = sourceIt.Norms;
            Waves.Normal2D N = circle.GetNormal(cor);
            double w = textBox7.Text.ToDouble();

            prbar = new int[k]; timer1.Enabled = true;
            int ind = listBox1.SelectedIndex;
            toolStripStatusLabel1.Text = "Мемоизация PMRSN (занимает неопределённое время)"; timer2.Start();
            DateTime t1 = DateTime.Now;

            ShowCancelControls();

            source = new System.Threading.CancellationTokenSource();
            System.Threading.CancellationToken token = source.Token;
            await Task.Run(() =>
            {
                for (int ie = 0; ie < xval.Length; ie++)
                    xval[ie] = beg + ie * h;

                switch (ind)
                {
                    case 0:
                        Parallel.For(0, k, (int i) =>
                        {
                            if (token.IsCancellationRequested) { toolStripStatusLabel1.Text = "Асинхронная операция была отменена"; return; }
                            SetIElemForAll(i, GetTupF(N.Position.x + N.n.x * xval[i], N.Position.y + N.n.y * xval[i], w, norms), cor);
                        });
                        break;
                    case 1:
                        Parallel.For(0, k, (int i) =>
                        {
                            if (token.IsCancellationRequested) { toolStripStatusLabel1.Text = "Асинхронная операция была отменена"; return; }
                            SetIElemForAll(i, GetTupF(N.Position.x + N.n.x * w, N.Position.y + N.n.y * w, xval[i], norms), cor);
                        });
                        break;
                    case 3:
                        Parallel.For(0, k, (int i) =>
                        {
                            if (token.IsCancellationRequested) { toolStripStatusLabel1.Text = "Асинхронная операция была отменена"; return; }
                            Complex[] tmp2 = u(N.Position.x + N.n.x * w, N.Position.y + N.n.y * w, xval[i]/** ThU / SpU*/, norms).ToComplex();
                            SetIElemForAll(i, new Complex[3], tmp2, cor);
                        });
                        Setzlim(uRvalRes, uRzRes);
                        break;
                    default:
                        beg = 0;
                        end = pimult2;
                        Parallel.For(0, k, (int i) =>
                        {
                            if (token.IsCancellationRequested) { toolStripStatusLabel1.Text = "Асинхронная операция была отменена"; return; }
                            xval[i] = beg + i * h;
                            SetIElemForAll(i, GetTupF(norms[i].Position.x, norms[i].Position.y, w, norms), norms[i].Corner);
                        });
                        break;
                }
            });

            if (ind == 3 && checkBox13.Checked) DrawUXW(N.Position.x + N.n.x * w, N.Position.y + N.n.y * w, 0, norms, cor);

            HideCancelControls();

            switch (ind)
            {
                case 0:
                    chart1.Titles[0].Text = $"(x,y): [{N.Position.x}; {N.Position.y}]...[{N.Position.x + N.n.x * xval[k - 1]}; {N.Position.y + N.n.y * xval[k - 1]}], w = {w}";
                    break;
                case 1:
                    chart1.Titles[0].Text = $"(x,y) = ({N.Position.x + N.n.x * w}; {N.Position.y + N.n.y * w}), w = {beg} ... {end}";
                    break;
                case 3:
                    chart1.Titles[0].Text = $"(x,y) = ({N.Position.x + N.n.x * w}; {N.Position.y + N.n.y * w}), t = {beg} ... {end}";
                    break;
                default:
                    chart1.Titles[0].Text = $"(x,y) in ({sourceIt.Center}; {sourceIt.radius}), w = {w}";
                    break;
            }

            timer2.Stop();
            timer1.Stop();
            toolStripStatusLabel1.Text = $"Вычисления закончены. Время: {DateTime.Now - t1}";
            Expendator.WriteInFile($"ur, uz (last)", new Vectors(xval), new Vectors(uRval), new Vectors(uIval), new Vectors(umodval), new Vectors(uRz), new Vectors(uIz), new Vectors(uAz));
            pictureBox1.Hide();
            progressBar1.Value = progressBar1.Maximum;
            ReDraw();

            HankelTuple = HankelTupleWith;
        }

        private void ReDraw()
        {
            ClearSeries();

            var list = new List<double>(uRval.Length);
            if (checkBox1.Checked) { chart1.Series[0].Points.DataBindXY(xval, uRval); chart1.Series[0].IsVisibleInLegend = true; list.AddRange(uRval); }
            if (checkBox2.Checked) { chart1.Series[2].Points.DataBindXY(xval, umodval); chart1.Series[2].IsVisibleInLegend = true; list.AddRange(umodval); }
            if (checkBox3.Checked) { chart1.Series[1].Points.DataBindXY(xval, uIval); chart1.Series[1].IsVisibleInLegend = true; list.AddRange(uIval); }
            if (checkBox4.Checked) { chart1.Series[4].Points.DataBindXY(xval, uIz); chart1.Series[4].IsVisibleInLegend = true; list.AddRange(uIz); }
            if (checkBox5.Checked) { chart1.Series[5].Points.DataBindXY(xval, uAz); chart1.Series[5].IsVisibleInLegend = true; list.AddRange(uAz); }
            if (checkBox6.Checked) { chart1.Series[3].Points.DataBindXY(xval, uRz); chart1.Series[3].IsVisibleInLegend = true; list.AddRange(uRz); }

            if (checkBox7.Checked) { chart1.Series[6].Points.DataBindXY(xval, uRvalRes); chart1.Series[6].IsVisibleInLegend = true; list.AddRange(uRvalRes); }
            if (checkBox8.Checked) { chart1.Series[7].Points.DataBindXY(xval, uIvalRes); chart1.Series[7].IsVisibleInLegend = true; list.AddRange(uIvalRes); }
            if (checkBox9.Checked) { chart1.Series[8].Points.DataBindXY(xval, umodvalRes); chart1.Series[8].IsVisibleInLegend = true; list.AddRange(umodvalRes); }
            if (checkBox10.Checked) { chart1.Series[9].Points.DataBindXY(xval, uRzRes); chart1.Series[9].IsVisibleInLegend = true; list.AddRange(uRzRes); }
            if (checkBox11.Checked) { chart1.Series[10].Points.DataBindXY(xval, uIzRes); chart1.Series[10].IsVisibleInLegend = true; list.AddRange(uIzRes); }
            if (checkBox12.Checked) { chart1.Series[11].Points.DataBindXY(xval, uAzRes); chart1.Series[11].IsVisibleInLegend = true; list.AddRange(uAzRes); }

            ForChart.SetAxisesY(ref chart1);
            ForChart.SetToolTips(ref chart1);
        }
        private void SetIElemForAll(int i, Complex[] tmp, Complex[] tmp2, double phi = 0)
        {
            double sin=Math.Sin(phi), cos=Math.Cos(phi);

            Complex ur = tmp[0] *cos  + tmp[1] *sin ;
            Complex uz = tmp[2];
            uRval[i] = ur.Re; uIval[i] = ur.Im; umodval[i] = ur.Abs;
            uRz[i] = uz.Re; uIz[i] = uz.Im; uAz[i] = uz.Abs;

            ur = tmp2[0] * cos + tmp2[1] * sin;
            uz = tmp2[2];
            uRvalRes[i] = ur.Re; uIvalRes[i] = ur.Im; umodvalRes[i] = ur.Abs;
            uRzRes[i] = uz.Re; uIzRes[i] = uz.Im; uAzRes[i] = uz.Abs;

            prbar[i] = 1;
        }
        private void SetIElemForAll(int i, Tuple<Complex[], Complex[]> tup, double phi = 0) => SetIElemForAll(i, tup.Item1, tup.Item2, phi);

        public Color[] colors = new Color[] { Color.Blue, Color.Red, Color.Green, Color.Black, Color.Gold };
        private void DrawUXW(double x, double y, double z, Normal2D[] norm, double corner)
        {
            toolStripStatusLabel1.Text = "Генерация дополнительных графиков";

            var form = new Defect2019.JustGrafic();

            form.chart1.Series.Clear();

            int countpoint = wcount;
            double ep = (wend - wbeg) / countpoint;

            form.chart1.Series.Add($"|ur(x,w)|");
            form.chart1.Series.Add($"|uz(x,w)|");
            form.chart1.Series.Add($"|f(w)|");
            form.chart1.Series.Add($"|ur(x,w) f(w)|");
            form.chart1.Series.Add($"|uz(x,w) f(w)|");

            for (int i = 0; i < 5; i++)
            {
                form.chart1.Series[i].BorderWidth = 4;
                form.chart1.Series[i].Color = colors[i];
                form.chart1.Series[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                form.chart1.Series[i].BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
                form.chart1.Series[i].Font = new Font("Arial", 16);
            }

            double[] w = SeqWMemoized(wbeg, wend, wcount);
            for (int i = 0; i < wcount; i++)
            {
                var c = Forms.UG.UResMemoized(x, y, w[i], norm);
                form.chart1.Series[0].Points.AddXY(w[i], (c[0] * Math.Cos(corner) + c[1] * Math.Sin(corner)).Abs);
                form.chart1.Series[1].Points.AddXY(w[i], c[2].Abs);
                form.chart1.Series[2].Points.AddXY(w[i], basefunc(w[i]).Abs);
                form.chart1.Series[3].Points.AddXY(w[i], form.chart1.Series[2].Points[i].YValues[0] * form.chart1.Series[0].Points[i].YValues[0]);
                form.chart1.Series[4].Points.AddXY(w[i], form.chart1.Series[2].Points[i].YValues[0] * form.chart1.Series[1].Points[i].YValues[0]);
            }

            form.CreateCheckBoxes();
            form.Lims();

            form.ShowDialog();
        }


        private static void Setzlim(double[] rmas, double[] zmas)
        {
            using (StreamWriter f = new StreamWriter("zlims.txt"))
            {
                f.WriteLine($"ur uz");
                f.WriteLine($"{rmas.Min()} {zmas.Min()}");
                f.WriteLine($"{rmas.Max()} {zmas.Max()}");
            }
        }
    }
}
