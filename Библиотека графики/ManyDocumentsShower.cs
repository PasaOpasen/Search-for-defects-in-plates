using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Библиотека_графики
{
    public partial class ManyDocumentsShower : Form
    {
        public ManyDocumentsShower(string maintitle, string[] titles, string[] documentnames, bool addCurrendDirectory = true)
        {
            InitializeComponent();

            string getname(string name) => (addCurrendDirectory) ? Path.Combine(Environment.CurrentDirectory, name) : name;

            this.Text = maintitle;
            tabControl1.TabPages.Clear();

            TabPage[] Tmas = new TabPage[titles.Length];


            for (int i = 0; i < titles.Length; i++)
            {
                Tmas[i] = new TabPage();
                //Tmas[i].BackColor = Other.colors[i];
                Tmas[i].Text = titles[i];

                string p = Path.GetExtension(documentnames[i]);
                string name = getname(documentnames[i]);

                if (p == ".pdf" || p == ".html")
                {
                    WebBrowser wb = new WebBrowser();
                    Tmas[i].Controls.Add(wb);
                    wb.Dock = DockStyle.Fill;
                    wb.Navigate(name);
                }
                else if (p == ".png" || p == ".bitmap")
                {
                    PictureBox pb = new PictureBox();
                    Tmas[i].Controls.Add(pb);
                    pb.BackgroundImage = Image.FromFile(name);
                    pb.BackgroundImageLayout = ImageLayout.Stretch;
                    pb.Dock = DockStyle.Fill;
                }
            }
            tabControl1.TabPages.AddRange(Tmas);
        }
    }
}
