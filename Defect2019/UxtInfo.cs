using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Библиотека_графики;

namespace Работа2019
{
    public partial class UxtInfo : Form
    {
        public UxtInfo(double a, double h, double[] ur, double[] uz, string[] f1names, string[] f1filenames, string f2)
        {
            InitializeComponent();

            SetMaxis(a, h, ur, uz);

            tabControl1.TabPages.Clear();

            TabPage[] Tmas = new TabPage[f1names.Length];

            for (int i = 0; i < f1names.Length; i++)
            {
                ref TabPage page = ref Tmas[i];
                page = new TabPage
                {
                    Text = f1names[i]
                };
                WebBrowser wb = new WebBrowser();
                page.Controls.Add(wb);
                wb.Dock = DockStyle.Fill;
                wb.Navigate(f1filenames[i]);
            }
            tabControl1.TabPages.AddRange(Tmas);

            webBrowser2.Navigate(Environment.CurrentDirectory + @"\" + f2);
        }
        private void SetMaxis(double a, double h, double[] ur, double[] uz)
        {
            double m1 = 0, m2 = 0;
            int ind1 = 0, ind2 = 0;
            for (int i = 0; i < ur.Length; i++)
            {
                if (Math.Abs(ur[i]) > m1) { m1 = Math.Abs(ur[i]); ind1 = i; }
                if (Math.Abs(uz[i]) > m2) { m2 = Math.Abs(uz[i]); ind2 = i; }
            }
            textBox1.Text = $"Максимальное значение |ur| = {m1} при t = {a + ind1 * h}" + Environment.NewLine +
                $"Максимальное значение |uz| = {m2} при t = {a + ind2 * h}";
        }

        private void выделитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new TextBoxForm("Максимальные значения в дефекте", textBox1).Show();
        }
    }
}
