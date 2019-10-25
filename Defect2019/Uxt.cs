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
using System.IO;

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
            FillZam();
        }
        public List<Source> examples = new List<Source>(), examples2 = new List<Source>(), examples3 = new List<Source>();
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
            const int n = 40;
            const double r = 8;

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
            void CentersToExapmlesDcircle(Point[] Centers, ref List<Source> expls)
            {
                for (int i = 0; i < Centers.Length; i++)
                {
                    Waves.DCircle c = new Waves.DCircle(Centers[i], 16, 5, arg: (135 * Math.PI / 180));
                    //  Waves.Normal2D[] norm = c.GetNormalsOnDCircle();
                    var fw = GetFmas();
                    expls.Add(new Source(c, fw));
                }
            }

            CentersToExapmles(centers, ref examples);
            CentersToExapmles(centers2, ref examples2);
            CentersToExapmlesDcircle(centers2, ref examples3);
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

        private void FillZam()
        {
            string where = Expendator.GetWordFromFile("WhereData.txt");
            where = Path.GetDirectoryName(where);
            textBox1.Text = where;

            string file = Path.Combine(where, "Описание.txt");
            textBox2.Text = (File.Exists(file)) ? Expendator.GetWordFromFile(file) : "";

        }

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

        private void реверсироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();

            for (int i = 0; i < sources.Count; i++)
                checkedListBox1.Items.Add(sources[sources.Count - 1 - i].ToString(), true);

            sources.Reverse();
            checkedListBox1.Update();
            SoundMethods.SetPositions();
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
            SoundMethods.Clear();
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
                var ind = checkedListBox1.CheckedIndices;
                List<Source> list = new List<Source>(ind.Count);

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
            AddExamples(examples.ToArray());
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
            const string mess = "Требуется подключение к Интернету. Будут загружены все пакеты R, необходимые программе. Загрузка может занимать несколько минут, по окончанию загрузки консоль закроется. Уже установленные пакеты могут загрузиться заново либо обновиться. Выполнить действие?";

            if (MessageBox.Show(mess, "Требуется подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SoundMethods.OK();
                await Task.Run(() => Expendator.StartProcessOnly(OtherMethods.GetResource("InstallPackages.r"), true));
            }
        }

        private void сортироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();
            sources.Sort();

            for (int i = 0; i < sources.Count; i++)
                checkedListBox1.Items.Add(sources[i].ToString(), true);
            checkedListBox1.Update();
            SoundMethods.SetPositions();
        }

        private void текущиеИсточникиполумесяцыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddExamples(examples3.ToArray());
        }

        private void изменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string res = folderBrowserDialog1.SelectedPath;
                res = Path.Combine(res, "WhereData.txt");
                if (!File.Exists(res) || !OtherMethods.ExistAllDirectoriesFromFile(res))
                    MessageBox.Show("В рабочем каталоге отсутствует файл \"WhereData.txt\", либо среди указанных в нём директорий есть несуществующие. Требуется выбрать корректный файл", "Нет пути или папки",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    File.Copy(res, Path.Combine(Environment.CurrentDirectory, "WhereData.txt"), true);
                    FillZam();
                }
            }
        }

        private void текущиеИсточникиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddExamples(examples2.ToArray());
        }

        private void AddExamples(Source[] array)
        {
            for (int i = 0; i < array.Length; i++)
                sources.Add(array[i]);
            Recostract();
            SoundMethods.TukTuk();
        }
    }
}
