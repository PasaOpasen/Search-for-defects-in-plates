using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading;
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

            button6.Hide();
            button5.Hide();
            button9.Hide();

            label8.BackColor = Color.Transparent;
            button7.Hide();

            colorDialog1.FullOpen = true;
            colorDialog1.Color = Color.Green;
            ForChart.SetToolTips(ref chart1);
            listBox1.SelectedItem = "По лучу от центра окружности через точку";
            timer1.Interval = 1800;
            timer1.Tick += new EventHandler(Timer1_Tick);

            stophide();

            timer2 = new Timer() { Interval = 1800 };
            timer2.Tick += new EventHandler(GetLenDic);

            toolStripStatusLabel1.Text = "Ожидание команды";

            for (int i = 0; i < chart1.Series.Count; i++)
            {
                chart1.Series[i].IsVisibleInLegend = false;
            }
            //label9.Hide();
            //numericUpDown2.Hide();
            CircleOrNot();

            var uf = new Memoize<Tuple<double, double, double,  Normal2D[], Func<double, Complex>>, Complex[]>((Tuple<double,  double, double, Normal2D[], Func<double, Complex>> t)=>_ufw(t.Item1,t.Item2,t.Item3,t.Item4,t.Item5));
            ufw = (double x, double y,  double w, Normal2D[] n, Func<double, Complex> f) => uf.Value(new Tuple<double, double,  double, Normal2D[], Func<double, Complex>>(x, y, w, n, f));

            var ur = new Memoize<Tuple<double, double,  double, Normal2D[]>,Complex[]>((Tuple<double, double,  double, Normal2D[]> t) => _ujRes(t.Item1, t.Item2, t.Item3, t.Item4));
            ujRes = (double x, double y, double w, Normal2D[] n) => ur.Value(new Tuple<double, double,  double, Normal2D[]>(x, y, w, n));

            kGrafic.ReadModelData();
        }

        public System.Threading.CancellationTokenSource source;

        public void CircleOrNot()
        {
            if (radioButton3.Checked)
            {
                label1.Show();
                textBox1.Show();
                textBox4.Show();
                label2.Show();
                textBox5.Show();
                label9.Show();
                numericUpDown2.Show();

                button10.Hide();
            }
            else
            {
                label1.Hide();
                textBox1.Hide();
                textBox4.Hide();
                label2.Hide();
                textBox5.Hide();
                label9.Hide();
                numericUpDown2.Hide();

                button10.Show();
            }
        }

        /// <summary>
        /// Скрыть кнопки для отмены асинхронной операции
        /// </summary>
        public void stophide()
        {
            label8.Hide();
            button8.Hide();
        }
        /// <summary>
        /// Показать кнопки для отмены асинхронной операции
        /// </summary>
        public void stopshow()
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
            many = 13000 * s / 31;
            if (prmsnmem.Lenght >= many)
            {
                errorProvider1.SetError(button8, "Достаточно много значений PMRSN");
            }
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
        private void GetLenDic(object Sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = $"Мемоизация PMRSN. Сохранено значений: {prmsnmem.Lenght}";
        }

        #endregion

        #region базовые поля и функции u(w), uxwMemoized(w)

        /// <summary>
        /// Полумесяц
        /// </summary>
        public Waves.DCircle dCircle = new DCircle(new Point(1, 2));
        double[] xval = new double[0], uRval = new double[0], uIval = new double[0], umodval = new double[0];
        double[] xvalz = new double[0], uRz = new double[0], uIz = new double[0], uAz = new double[0];
        double[] uRvalRes = new double[0], uIvalRes = new double[0], umodvalRes = new double[0], uRzRes = new double[0], uIzRes = new double[0], uAzRes = new double[0];

        int width = 3; Color color = Color.Blue;
        double fix, beg, end;
        public Func<double, double, double, Normal2D[], Complex[]> uj = (double x, double y,  double w, Normal2D[] normal) =>
       {
           Vectors poles = PolesMasMemoized(w);
           double min = poles.Min * 0.5, max = poles.Max * 1.5;//min.Show();

            //подынтегральная функция
            FuncMethods.DefInteg.GaussKronrod.ComplexVectorFunc tmp = (Complex a, int n) =>
              {
                   //CVectors S=new CVectors(3);
                   //for(int i=0;i<normal.Length;i++)
                   //{
                   //    S+= K(a, x-normal[i].Position.x, y - normal[i].Position.y, z, w)*new CVectors(new Complex[] { normal[i].n.x, normal[i].n.y, 0 });//S.Show();
                   //}
                   return (Ksum(a, x, y, w, normal, (Point t) => { return new CVectors(new Complex[] { t.x, t.y, 0 }); })).ComplexMas;
              };

            //интеграл
            return FuncMethods.DefInteg.GaussKronrod.DINN5_GK(tmp, min, min, min, max, РабКонсоль.tm, РабКонсоль.tp, РабКонсоль.eps, РабКонсоль.pr, РабКонсоль.gr, 3, РабКонсоль.NodesCount).Div(2 * Math.PI);
       };

        //public Func<double, double, double, double, Source,Tuple<Complex,Complex>> uj = (double x, double y, double z, double w, Normal2D[] normal) =>
        //{
        //    Vectors poles = PolesMasMemoized(w);
        //    double min = poles.Min * 0.5, max = poles.Max * 1.5;//min.Show();
        //    FuncMethods.DefInteg.GaussKronrod.ComplexVectorFunc tmp;
        //    CVectors res =new CVectors(3);
        //    double xx, yy;

        //    //подынтегральная функция
        //    for(int i = 0; i < normal.Length; i++)
        //    {
        //        xx = x - normal[i].Position.x;
        //        yy = y - normal[i].Position.y;
        //    tmp = (Complex a, int n) =>
        //    {
        //        return (K(a,xx , yy, z, w)*new CVectors(new Complex[] { normal[i].n.x, normal[i].n.y, 0 })).ComplexMas;
        //    };
        //        res += new CVectors(FuncMethods.DefInteg.GaussKronrod.DINN5_GK(tmp, min, min, min, max, РабКонсоль.tm, РабКонсоль.tp, РабКонсоль.eps, РабКонсоль.pr, РабКонсоль.gr, 3, РабКонсоль.NodesCount).Div(2 * Math.PI));
        //    }

        //    //интеграл
        //    return res.ComplexMas;
        //};

        public Func<double, double, double, Normal2D[], Complex[]> _ujRes = (double x, double y,  double w, Normal2D[] s) =>
      {
            return KsumRes(x, y,  w, s, (Point t) => { return new Vectors( t.x, t.y, 0 ); }).ComplexMas;
      };
        /// <summary>
        /// Мемоизированная u(x,w) по вычетам. Её мемоизация не помогает на 3D-графиках, так как там уже нет повторных вычислений при меняющемся времени
        /// </summary>
        public Func<double, double,  double, Normal2D[],Complex[]> ujRes;
        public Func<double, double,  double, Normal2D[], Complex> vz = (double x, double y, double w, Normal2D[] nor) => w * Forms.UG.ujRes(x, y, w, nor)[2];
        //public Func<double, double, double, double, Normal2D[], Complex> vz = (double x, double y, double z, double w, Normal2D[] nor) => w * Forms.UG.ujRes(x, y, z, w, nor)[2];
        #endregion

        #region u(x,t)

       // public static double[] wmas = SeqWMemoized(wbeg, wend, wcount);
        public static bool wchange = false;

        public Func<double, double, double,  Normal2D[],Func<double,Complex>, Complex[]> _ufw = (double x, double y, double w, Normal2D[] normal,Func<double, Complex> f) => Expendator.Mult(Forms.UG.ujRes(x, y, w, normal), f(w));
        public Func<double, double, double, Normal2D[], Func<double, Complex>, Complex[]> ufw;


        /// <summary>
        /// Итоговая функция (через тестовую функцию f)
        /// </summary>
        public Func<double, double, double,  Func<double, Complex>, Normal2D[], double[]> uxt = (double x, double y,  double t, Func<double, Complex> f, Normal2D[] normal) =>
          {
            //  if (wchange)
            //  {
              double[] wmas= SeqWMemoized(wbeg, wend, wcount);
                //  wchange = false;
             // }

              CVectors[] c = new CVectors[wcount];
              Parallel.For(0,wcount,(int i)=>
              {
                  c[i] =new CVectors( Expendator.Mult(Forms.UG.ujRes(x, y,  wmas[i], normal), f(wmas[i])));
              });
              return ((c * Fi(wmas, t)).Re / Math.PI).DoubleMas;
          };
        /// <summary>
        /// Итоговая функция (через вычисленные массивы w и f(w))
        /// </summary>
        public Func<double, double, double,  Tuple<double[],Complex[]>, Normal2D[], double[]> uxt2 = (double x, double y,  double t,  Tuple<double[],Complex[]> tuple, Normal2D[] normal) =>
        {
            double[] w = tuple.Item1;
            Complex[] fw = tuple.Item2;

            CVectors[] c = new CVectors[wcount];
            //tex: ${\bar c}= f({\bar w}) \cdot u(x,y,z,{\bar w}) $ покомпонентно
            Parallel.For(0, wcount, (int i) =>
            {
                c[i] = new CVectors(Expendator.Mult(Forms.UG.ujRes(x, y, w[i], normal), fw[i]));
            });
            return ((c * Fi(w, t)).Re / Math.PI).DoubleMas;
        };

        public static Func<double, Complex> basefunc = F1;
        public Func<double, double, double, Normal2D[], double[]> u = (double x, double y, double t, Normal2D[] normal) => Forms.UG.uxt(x, y,  t, basefunc, normal);

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

        private void CheckBoxesShowHide(bool show=true)
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
            int i = listBox1.SelectedIndex;
            switch (i)
            {
                case 0:
                    label5.Show();
                    textBox6.Show();
                    label3.Show();
                    textBox2.Show();
                    textBox3.Show();
                    label7.Text = "w =";
                    textBox7.Text = "3";
                    //label7.Show();
                    //textBox7.Show();
                    CheckBoxesShowHide(true);
                    numericUpDown1.Value = 100;
                    break;
                case 1:
                    label5.Show();
                    textBox6.Show();
                    if (textBox2.Text.ToDouble() == 0) textBox2.Text = РабКонсоль.polesBeg.ToString();
                    label3.Show();
                    textBox2.Show();
                    textBox3.Show();
                    label7.Text = "dist =";
                    textBox7.Text = "20";
                    //label7.Hide();
                    //textBox7.Hide();
                    CheckBoxesShowHide(true);
                    numericUpDown1.Value = 100;
                    break;
                case 3:
                    label5.Show();
                    textBox6.Show();
                    if (textBox2.Text.ToDouble() == 0) textBox2.Text = РабКонсоль.polesBeg.ToString();
                    label3.Show();
                    textBox2.Show();textBox2.Text = "0,01";
                    textBox3.Show();textBox3.Text = "100";
                    label7.Text = "dist =";
                    textBox7.Text = "100";
                    CheckBoxesShowHide(false);
                    numericUpDown1.Value = 300;
                    break;
                default:
                    label5.Hide();
                    textBox6.Hide();
                    label3.Hide();
                    textBox2.Hide();
                    textBox3.Hide();
                    label7.Text = "w =";
                    textBox7.Text = "3";
                    //label7.Show();
                    //textBox7.Show();
                    CheckBoxesShowHide(true);
                    numericUpDown1.Value = 100;
                    break;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            new ParametrsQu().Show();
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

        private void button7_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Запущен диалог относительно 3D графика";
            //timer1.Start();timer2.Start();
            //prbar = new int[1];
            new _3Duruz().ShowDialog();
            //timer1.Stop(); timer2.Stop();
            //toolStripStatusLabel1.Text = "Вычисления выполнены и записаны в файл. Запущен script из R";
        }

        string message = "При неправильном выборе параметра tm или отрезка обхода полюсов интеграл может проходить через полюс (либо слишком близко), из-за чего происходят NaN от деления на 0 или бесконечности от суммирования больших чисел." + Environment.NewLine +
            "В этом случае метод интегрирования дробит шаг и начинает считать намного больше значений, в следствие этого значений PMRSN сохраняется слишком много. Из-за близости полюсов программа работает в разы дольше, а в конечных данных появляются выбросы." + Environment.NewLine +
            "В этом случае, опираясь на число PMRSN, следует прервать программу и изменить параметры интегрирования. Число значений PMRSN (при фикс. частоте и GK31) бывает: хорошее (3-4к), среднее (8-10к), плохое (>12к). Программу однозначно следует прерывать при числе PMRSN, большем 16к, если только не взята большая размерность. Если частота изменяется, указанные числа умножаются на количество разных частот, а при изменении GK - на соответсвующее отношение." + Environment.NewLine +
            "Этот способ поможет только 1 раз, так как сохранённые значения не удаляются (зачем?). Данные сотрутся автоматически при изменении условий задачи";
        private void label8_Click(object sender, EventArgs e)
        {
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

        private void button9_Click(object sender, EventArgs e)
        {
            new PRMSN_Memoized().Show();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            CircleOrNot();
        }

        private void графикиPRMSNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button9_Click(new object(), new EventArgs());
        }

        private void параметрыЗадачиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button6_Click(new object(), new EventArgs());
        }

        private void параметрыПодсчётаИнтегралаDINNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button5_Click(new object(), new EventArgs());
        }

        private void распространениеВолныВПространствеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTuple();
            new WaveContinious().ShowDialog();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            CircleOrNot();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            new DC().ShowDialog();
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
                        await Task.Run(() => OtherMethods.StartProcessOnly("ReDraw3Duxt2.r", true));
                        break;
                       }
                    else
                        return;
                }
                    

            new Anima(st).Show();
        }

        private bool FilesUrUzExist(string[] pngnames)
        {
            string s;
            for (int i = 0; i < pngnames.Length; i++)
            {
                s = pngnames[i].Replace(".png", "").Replace("3D ","");
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
            //if (!(checkBox1.Checked || checkBox2.Checked || checkBox3.Checked || checkBox4.Checked || checkBox5.Checked || checkBox6.Checked)) { checkBox2.Checked = true; checkBox5.Checked = true; }
            ReDraw();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            radio();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            radio();
        }
        #endregion


        public Tuple<double, double, double, Normal2D[],Func<МатКлассы.Point, bool>> tuple;

        public void SetTuple()
        {
            double r = textBox5.Text.ToDouble();
            Point center = new Point(textBox1.Text.ToDouble(), textBox4.Text.ToDouble());
            Waves.Circle circle = new Waves.Circle(center, r);
            if (radioButton4.Checked) circle = new Circle(dCircle.BigCircle);
            Waves.Normal2D[] norms = (radioButton3.Checked) ? circle.GetNormalsOnCircle(Convert.ToInt32(numericUpDown2.Value)) : dCircle.GetNormalsOnDCircle();

            Func<МатКлассы.Point, bool> filter;
            if (radioButton3.Checked) filter = (МатКлассы.Point po) => circle.ContainPoint(po);
            else filter = (МатКлассы.Point po) => dCircle.ContainPoint(po);

            tuple = new Tuple<double, double, double, Normal2D[], Func<Point, bool>>(circle.center.x,circle.center.y,circle.radius,norms,new Func<Point, bool>(filter));
        }

        public async void button1_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Чтение данных и генерация переменных";
            for (int i = 0; i < chart1.Series.Count; i++)
            {
                chart1.Series[i].Points.Clear();
                chart1.Series[i].IsVisibleInLegend = false;
            }

            //simpleSound.Stop();
            button3.Show();

            //РабКонсоль.SetPolesDef();

            int k = Convert.ToInt32(numericUpDown1.Value);//k.Show();
            xval = new double[k]; uRval = new double[k]; uIval = new double[k]; umodval = new double[k];
            xvalz = new double[k]; uRz = new double[k]; uIz = new double[k]; uAz = new double[k];
            uRvalRes = new double[k]; uIvalRes = new double[k]; umodvalRes = new double[k];
            uRzRes = new double[k]; uIzRes = new double[k]; uAzRes = new double[k];

            double cor = textBox6.Text.ToDouble() * Math.PI / 180;
            beg = textBox2.Text.ToDouble();
            end = textBox3.Text.ToDouble();
            double r = textBox5.Text.ToDouble();
            Point center = new Point(textBox1.Text.ToDouble(), textBox4.Text.ToDouble());
            Waves.Circle circle = new Waves.Circle(center, r);

            double h = (end - beg) / (k - 1);
            Waves.Normal2D[] norms = (radioButton3.Checked) ? circle.GetNormalsOnCircle(Convert.ToInt32(numericUpDown2.Value)) : dCircle.GetNormalsOnDCircle();
            Waves.Normal2D N = (radioButton3.Checked) ? circle.GetNormal(cor) : dCircle.BigCircle.GetNormal(cor);
            double w = textBox7.Text.ToDouble();

            //массив нормалей
            //int NN = Convert.ToInt32(numericUpDown2.Value);
            //Point[] Nmas = Waves.Normal2D.NormalsToPoins(circle.GetNormalsOnCircle(NN));

            prbar = new int[k]; timer1.Enabled = true;
            int ind = listBox1.SelectedIndex;
            toolStripStatusLabel1.Text = "Мемоизация PMRSN (занимает неопределённое время)"; timer2.Start();
            DateTime t1 = DateTime.Now;

            stopshow();

            source = new System.Threading.CancellationTokenSource();
            System.Threading.CancellationToken token = source.Token;
            await Task.Run(() =>
            {
                switch (ind)
                {
                    case 0:
                        Parallel.For(0, k, (int i) =>
                        {
                            if (token.IsCancellationRequested) { toolStripStatusLabel1.Text = "Асинхронная операция была отменена"; return; }
                            xval[i] = beg + i * h;
                            Complex[] tmp = uj(N.Position.x + N.n.x * xval[i], N.Position.y + N.n.y * xval[i],  w, norms);
                            Complex[] tmp2 = ujRes(N.Position.x + N.n.x * xval[i], N.Position.y + N.n.y * xval[i],  w, norms);
                            SetIElemForAll(i, tmp, tmp2, cor);
                        });
                        //chart1.Titles[0].Text = $"(x,y): [{N.Position.x}; {N.Position.x + N.n.x * xval[k-1]}]...[{N.Position.y}; {N.Position.y + N.n.y * xval[k - 1]}], z = 0, w = {w}";
                        break;
                    case 1:
                        Parallel.For(0, k, (int i) =>
                        {
                            if (token.IsCancellationRequested) { toolStripStatusLabel1.Text = "Асинхронная операция была отменена"; return; }
                            xval[i] = beg + i * h;
                            Complex[] tmp = uj(N.Position.x + N.n.x * w, N.Position.y + N.n.y * w,  xval[i], norms);
                            Complex[] tmp2 = ujRes(N.Position.x + N.n.x * w, N.Position.y + N.n.y * w,  xval[i], norms);
                            SetIElemForAll(i, tmp, tmp2, cor);
                        });
                        //chart1.Titles[0].Text = $"(x,y) = ({N.Position.x}; {N.Position.y}), z = 0, w = {beg} ... {end}";
                        break;
                    case 3:
                        Parallel.For(0, k, (int i) =>
                        {
                            if (token.IsCancellationRequested) { toolStripStatusLabel1.Text = "Асинхронная операция была отменена"; return; }
                            xval[i] = beg + i * h;
                            Complex[] tmp = new Complex[3];
                            Complex[] tmp2 = u(N.Position.x + N.n.x * w, N.Position.y + N.n.y * w,  xval[i]/** ThU / SpU*/, norms).ToComplex();
                            SetIElemForAll(i, tmp, tmp2, cor);                   
                        });
                        //chart1.Titles[0].Text = $"(x,y) = ({N.Position.x}; {N.Position.y}), z = 0, w = {beg} ... {end}";
                        Setzlim(uRvalRes, uRzRes);

                        break;
                    default://тут надо хорошо подумать!!!!!!!!
                        beg = 0;
                        end = 2 * Math.PI;
                        //Complex[][] mm = new Complex[k][];
                        Parallel.For(0, k, (int i) =>
                        {
                            if (token.IsCancellationRequested) { toolStripStatusLabel1.Text = "Асинхронная операция была отменена"; return; }
                            xval[i] = beg + i * h;
                            Complex[] tmp = uj(norms[i].Position.x, norms[i].Position.y,  w, norms);
                            Complex[] tmp2 = ujRes(norms[i].Position.x, norms[i].Position.y,  w, norms);
                            SetIElemForAll(i, tmp, tmp2, norms[i].Corner);
                        });
                        //chart1.Titles[0].Text = $"(x,y) in ({center}; {r}), z = 0, w = {w}";
                        break;
                }
            });

            if (ind==3&& checkBox13.Checked) DrawUXW(N.Position.x + N.n.x * w, N.Position.y + N.n.y * w, 0, norms,cor);

            stophide();

            switch (ind)
            {
                case 0:
                    chart1.Titles[0].Text = $"(x,y): [{N.Position.x}; {N.Position.y}]...[{N.Position.x + N.n.x * xval[k - 1]}; {N.Position.y + N.n.y * xval[k - 1]}], z = 0, w = {w}";
                    break;
                case 1:
                    chart1.Titles[0].Text = $"(x,y) = ({N.Position.x + N.n.x * w}; {N.Position.y + N.n.y * w}), z = 0, w = {beg} ... {end}";
                    break;
                case 3:
                    chart1.Titles[0].Text = $"(x,y) = ({N.Position.x + N.n.x * w}; {N.Position.y + N.n.y * w}), z = 0, t = {beg} ... {end}";
                    break;
                ////////////
                default://тут надо хорошо подумать!!!!!!!!
                    chart1.Titles[0].Text = $"(x,y) in ({center}; {r}), z = 0, w = {w}";
                    break;
            }

            timer2.Stop();
            timer1.Stop();
            toolStripStatusLabel1.Text = $"Вычисления закончены. Время: {DateTime.Now - t1}";
            Expendator.WriteInFile($"ur, uz (last)", new Vectors(xval), new Vectors(uRval), new Vectors(uIval), new Vectors(umodval), new Vectors(uRz), new Vectors(uIz), new Vectors(uAz));
            pictureBox1.Hide();
            progressBar1.Value = progressBar1.Maximum;
            ReDraw();
        }

        private void ReDraw()
        {
            for (int i = 0; i < chart1.Series.Count; i++)
            {
                chart1.Series[i].Points.Clear();
                chart1.Series[i].IsVisibleInLegend = false;
            }

            var list = new List<double>();
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

            //simpleSound.PlayLooping();
            if (list.Count > 0)
            {
                double max = list.Max(), min = list.Min(), t = 0.05;
                chart1.ChartAreas[0].AxisY.Minimum = (min > 0) ? min * (1 - t) : min * (1 + t);
                chart1.ChartAreas[0].AxisY.Maximum = (max > 0) ? max * (1 + t) : max * (1 - t);
            }

        }
        private void SetIElemForAll(int i, Complex[] tmp, Complex[] tmp2, double phi = 0)
        {
            Complex ur = /*Complex.Sqrt(*/tmp[0]/*.Sqr()*/* Math.Cos(phi) + tmp[1]/*.Sqr()*//*)*/* Math.Sin(phi);
            //Complex ur = tmp[0].Sqr() + tmp[1].Sqr();

            Complex uz = tmp[2];
            uRval[i] = ur.Re; uIval[i] = ur.Im; umodval[i] = ur.Abs;
            uRz[i] = uz.Re; uIz[i] = uz.Im; uAz[i] = uz.Abs;

            ur = /*Complex.Sqrt(*/tmp2[0]/*.Sqr()*/* Math.Cos(phi) + tmp2[1]/*.Sqr()*//*)*/* Math.Sin(phi);
            uz = tmp2[2];
            uRvalRes[i] = ur.Re; uIvalRes[i] = ur.Im; umodvalRes[i] = ur.Abs;
            uRzRes[i] = uz.Re; uIzRes[i] = uz.Im; uAzRes[i] = uz.Abs;

            prbar[i] = 1;
        }

        public Color[] colors = new Color[] { Color.Blue, Color.Red, Color.Green, Color.Black, Color.Gold };
        private void DrawUXW(double x,double y,double z, Normal2D[] norm,double corner)
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

            for(int i = 0; i < 5; i++)
            {
            form.chart1.Series[i].BorderWidth = 4;
            form.chart1.Series[i].Color = colors[i];
            form.chart1.Series[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            form.chart1.Series[i].BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
                form.chart1.Series[i].Font=new Font("Arial", 16);
            }

            double[] w = SeqWMemoized(wbeg, wend, wcount);
            for (int i=0;i<wcount;i++)
            {
                //  Debug.WriteLine($"u(x = ({x}; {y}), w = {w[i]}) = {Forms.UG.ujRes(x, y, z, w[i], norm)[2].Abs}");
                var c = Forms.UG.ujRes(x, y, w[i], norm);
                form.chart1.Series[0].Points.AddXY(w[i], (c[0]*Math.Cos(corner)+c[1]*Math.Sin(corner)).Abs);
                form.chart1.Series[1].Points.AddXY(w[i], c[2].Abs);
                form.chart1.Series[2].Points.AddXY(w[i], basefunc(w[i]).Abs);
                form.chart1.Series[3].Points.AddXY(w[i], form.chart1.Series[2].Points[i].YValues[0] * form.chart1.Series[0].Points[i].YValues[0]);
                form.chart1.Series[4].Points.AddXY(w[i], form.chart1.Series[2].Points[i].YValues[0]* form.chart1.Series[1].Points[i].YValues[0]);
            }

            form.CreateCheckBoxes();
            form.Lims();

            form.ShowDialog();
        }


        private void button5_Click(object sender, EventArgs e)
        {
            new DINN5().Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string name = $"Функции ur,uz";
            SaveFileDialog savedialog = new SaveFileDialog();
            savedialog.Title = "Сохранить рисунок как...";
            savedialog.FileName = name;
            savedialog.Filter = "Image files (*.png)|*.png|All files (*.*)|*.*";

            savedialog.OverwritePrompt = true;
            savedialog.CheckPathExists = true;
            savedialog.ShowHelp = true;
            if (savedialog.ShowDialog() == DialogResult.OK)
            {
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

        private void button4_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // установка цвета формы
            this.color = colorDialog1.Color;
            chart1.BackColor = this.color;

            //chart1.Series[0].Color = colorDialog1.Color;
            //if (uRval != null) { chart1.Series[0].Points.Clear(); chart1.Series[0].Points.DataBindXY(xval, uRval); }
        }

        SoundPlayer simpleSound = new SoundPlayer(@"1.wav");

        private void button2_Click(object sender, EventArgs e)
        {
            simpleSound.Stop();
            //source.Cancel();    
            //source.Dispose();
            this.Close();
        }

        private void radio()
        {
            if (radioButton1.Checked)
            {
                button7.Hide();
                listBox1.Show();
                label5.Show();
                textBox6.Show();
                label7.Show();
                textBox7.Show();
                label3.Show();
                textBox2.Show();
                textBox3.Show();
                label4.Show();
                numericUpDown1.Show();
                //label9.Show();
                //numericUpDown2.Show();
            }
            else
            {
                button7.Show();
                listBox1.Hide();
                label5.Hide();
                textBox6.Hide();
                label7.Hide();
                textBox7.Hide();
                label3.Hide();
                textBox2.Hide();
                textBox3.Hide();
                label4.Hide();
                numericUpDown1.Hide();
                //label9.Hide();
                //numericUpDown2.Hide();
            }
        }

        private void Setzlim(double[] rmas,double[] zmas)
        {
            using(StreamWriter f=new StreamWriter("zlims.txt"))
            {
                f.WriteLine($"ur uz");
                f.WriteLine($"{rmas.Min()} {zmas.Min()}");
                f.WriteLine($"{rmas.Max()} {zmas.Max()}");
            }
        }
    }
}
