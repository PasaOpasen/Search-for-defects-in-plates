using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Библиотека_графики
{
    public partial class Trapezi : Form
    {
        double beg = 0.01, end = 0.58, s = 0.04;
        int h;
        double t => (100 - trackBar1.Value - trackBar3.Value - 2 * trackBar2.Value);
        public Trapezi(JustGrafic g)
        {
            InitializeComponent();
            
            gr = g;
            chart1.Series[0].IsVisibleInLegend = false;
            h = g.arr[0].Length;
            ReDraw();
            Библиотека_графики.ForChart.SetToolTips(ref chart1);
            trackBar1.Value =(int) (100 * beg);
            trackBar2.Value = (int)(100 * s);
            trackBar3.Value = (int)(100 * end);

            trackBar1.Scroll += trackBar1_Scroll;
            trackBar2.Scroll += trackBar2_Scroll;
            trackBar3.Scroll += trackBar3_Scroll;
            trackBar1_Scroll(new object(), new EventArgs());
            trackBar2_Scroll(new object(), new EventArgs());
            trackBar3_Scroll(new object(), new EventArgs());


            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "{0:0,}K";
        }
        JustGrafic gr;
        void ReDraw()
        {
            chart1.Series[0].Points.Clear();
            chart1.Series[0].Points.AddXY(0, 0);
            chart1.Series[0].Points.AddXY(beg*h, 0);
            chart1.Series[0].Points.AddXY((beg+s)*h, 1);
            chart1.Series[0].Points.AddXY((1-end-s)*h, 1);
            chart1.Series[0].Points.AddXY((1-end)*h, 0);
            chart1.Series[0].Points.AddXY(h, 0);
        }
        void SetParams()
        {
            beg = trackBar1.Value / 100.0;
            end = trackBar3.Value / 100.0;
            s = trackBar2.Value / 100.0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            int t1 = (int)(beg * h);
            int t2 = (int)((beg+s) * h);
            int t3 = (int)((1.0-end-s) * h);
            int t4 = (int)((1.0-end) * h);

            int t12 = t2 - t1;
            double tg= (t12==0) ? 0 : 1.0 / t12;

            await Task.Run(() => { 
            Parallel.For(0, gr.arr2.GetLength(0), (int i) => { 
            //for(int i = 0; i < gr.arr2.GetLength(0); i++)
          //  {
                for (int k = 0; k <= t1; k++)
                    gr.arr2[i][k] = 0.0;
                for (int k = t4; k < h; k++)
                    gr.arr2[i][k] = 0.0;

                for (int k = t1+1; k < t2; k++)
                    gr.arr2[i][k]*= (k-t1)*tg;
                for (int k = t3+1; k < t4; k++)
                    gr.arr2[i][k] *=1.0- (k - t3) * tg;
          //  }
});
            });
            gr.ReSaveMas();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (t >= 0)
            {
                SetParams();
                ReDraw();
            }
            else
                trackBar1.Value = 100 - trackBar2.Value * 2 - trackBar3.Value;
            label1.Text = $"До трапеции{Environment.NewLine}({trackBar1.Value*h/100})";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            gr.Cancel();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if (t >= 0)
            {
                SetParams();
                ReDraw();
            }
            else
                trackBar2.Value = (100 - trackBar1.Value - trackBar3.Value)/2;
            label2.Text = "Под"+Environment.NewLine+"боковой" +Environment.NewLine+"стороной" +$"{Environment.NewLine}({trackBar2.Value * h / 100})";
        }
        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            if (t >= 0)
            {
                SetParams();
                ReDraw();
            }
            else
                trackBar3.Value = 100 - trackBar2.Value * 2 - trackBar1.Value;
            label3.Text = $"После трапеции{Environment.NewLine}({trackBar3.Value * h / 100})";
        }
    }
}
