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
    public partial class Anima : Form
    {
        public Anima(string[] mas)
        {
            InitializeComponent();

            count = mas.Length-1;
            files = mas;

            timer1.Interval = РабКонсоль.animatime;
            timer1.Tick += new EventHandler(Timer1_Tick);

            pictureBox1.BackgroundImage = Bitmap.FromFile(files[0]);
            timer1.Start();
            this.FormClosing += new FormClosingEventHandler((object o,FormClosingEventArgs f)=> {
                timer1.Stop();
                files = null;
                GC.Collect();
            });
        }
        int count;
        string[] files;
        int k = 1;
        private void Timer1_Tick(object Sender, EventArgs e)
        {
            //if(pictureBox1.BackgroundImage != null)
            //{
            //    pictureBox1.BackgroundImage.Dispose();
            //}
            pictureBox1.BackgroundImage = Bitmap.FromFile(files[k++]);
            if (k == count)
            {
                k = 0;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}
