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
using Point = МатКлассы.Point;
using static Functions;
using static РабКонсоль;
using Работа2019;

namespace Defect2019
{
    public partial class Uxt : Form
    {
        public Uxt()
        {
            InitializeComponent();

            timer1.Interval = 350;
            timer1.Tick += new EventHandler(Timer1_Tick);
            timer1.Start();
            Recostract();

            FillExamples();
        }
        public List<Source> examples = new List<Source>(), examples2 = new List<Source>();
        public Point[] centers = new Point[]
        {
            new Point(-150),
            new Point(150),
            new Point(-150,150),
            new Point(150,-150),
            new Point(150,0),
            new Point(0,150),
            new Point(-150,0),
            new Point(0,-150),
            new Point(0)
        };
        /// <summary>
        /// Текущие центры источников
        /// </summary>
        public Point[] centers2 = new Point[]
        {
            new Point(0, 0),
            new Point(200,0 ),
            new Point(400,0),
            new Point( 600,0),
            new Point(0, 200),
            new Point(200, 200),
            new Point(400, 200),
            new Point(600, 200)
        };
        private void FillExamples()
        {
            int n = 40;
            double r = 8;

            void CentersToExapmles(Point[] Centers, ref List<Source> expls)
            {
                for (int i = 0; i < Centers.Length; i++)
                {
                    Waves.Circle c = new Waves.Circle(new МатКлассы.Point(Centers[i]), r);
                    Waves.Normal2D[] norm = c.GetNormalsOnCircle(n);
                    var fw = GetFmas();
                    expls.Add(new Source(c, norm, fw));
                }
            }

            CentersToExapmles(centers, ref examples);
            CentersToExapmles(centers2, ref examples2);
        }
        /// <summary>
        /// Передел чек-листа
        /// </summary>
        public void Recostract()
        {
            checkedListBox1.Items.Clear();
            for (int i = 0; i < sources.Count; i++)
            {
                checkedListBox1.Items.Add(sources[i].ToString(), true);
            }
            checkedListBox1.Update();
        }
        public static List<Source> sources = new List<Source>();
        public static bool addnewsource = false;

        private void Timer1_Tick(object Sender, EventArgs e)
        {
            if (addnewsource)
            {
                $"Added source(s) at {DateTime.Now} (count of sources is {sources.Count})".Show();
                Recostract();
                addnewsource = false;
            }
        }

        private void параметрыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ParametrsQu().ShowDialog();
        }

        private void построитьПоследнююСохранённуюАнимациюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Forms.UG.анимацияПоПоследнимСохранённымДаннымToolStripMenuItem_Click(new object(), new EventArgs());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void реверсироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();

            for (int i = 0; i < sources.Count; i++)
                checkedListBox1.Items.Add(sources[sources.Count - 1 - i].ToString(), true);

            sources.Reverse();
            checkedListBox1.Update();
        }

        private void удалитьПоследнееВхождениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sources.Count > 0)
            {
                sources.RemoveAt(sources.Count - 1);
                Recostract();
                checkedListBox1.Update();
            }

        }

        private void очиститьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();
            sources.Clear();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new Sources().Show();
        }

        private void полумесяцToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new DC(true).Show();
        }

        private bool GetCheckedSources(out Source[] smas)
        {
            if (checkedListBox1.Items.Count > 0)
            {
                List<Source> list = new List<Source>();
                var ind = checkedListBox1.CheckedIndices;
                for (int i = 0; i < ind.Count; i++)
                    list.Add(sources[ind[i]]);
                if (list.Count == 0)
                    MessageBox.Show("Не отмечено ни одного источника. Отметьте хотя бы один источник", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    smas = list.ToArray();
                    return true;
                };
            }
            else
                MessageBox.Show("Нет ни одного источника. Добавьте источники через меню, вызываемое правой кнопкой мыши при щелчке на белом окне", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Stop);

            smas = null;
            return false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (GetCheckedSources(out Source[] smas))
            {
                if (radioButton1.Checked)
                    new WaveContinious(smas).Show();
                else
                    new WaveletContinious(smas).Show();
            }
                
        }

        private void вставитьГотовыйПримерToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < examples.Count; i++)
                sources.Add(examples[i]);
            Recostract();
        }

        private void посмотретьПолучающуюсяСхемуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GetCheckedSources(out Source[] smas))
                new Scheme(smas).Show();
        }

        private void быстрыйТестToolStripMenuItem_Click(object sender, EventArgs e)
        {
            текущиеИсточникиToolStripMenuItem_Click(sender, e);
            if (GetCheckedSources(out Source[] smas))
            {
                var form = new WaveContinious(smas);
                form.numericUpDown1.Value = 10;
                form.numericUpDown2.Value = 5;
                form.textBox5.Text = "1";
                form.textBox6.Text = "110";

                form.Show();
                form.button4_Click(sender, e);
            }

        }

        private void быстрыйТестToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            текущиеИсточникиToolStripMenuItem_Click(sender, e);
            if (GetCheckedSources(out Source[] smas))
            {
                var form = new WaveContinious(smas);
                form.numericUpDown1.Value = 10;
                form.numericUpDown2.Value = 5;
                form.textBox5.Text = "1";
                form.textBox6.Text = "110";

                form.Show();
                form.button4_Click(sender, e);
            }
        }

        private void вычислитьUxtВОднойТочкеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GetCheckedSources(out Source[] smas))
                new OnePoint(smas).ShowDialog();
        }

        private async void загрузитьНеобходимыеПакетыRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string mess = "Требуется подключение к Интернету. Будут загружены все пакеты R, необходимые программе. Загрузка может занимать несколько минут, по окончанию загрузки консоль закроется. Уже установленные пакеты могут загрузиться заново либо обновиться. Выполнить действие?";

            if (MessageBox.Show(mess, "Требуется подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                await Task.Run(() => Expendator.StartProcessOnly(OtherMethods.GetResource("InstallPackages.r"), true));
        }

        private void текущиеИсточникиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < examples2.Count; i++)
                sources.Add(examples2[i]);
            Recostract();
        }
    }
}
