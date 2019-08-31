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
using static Functions;
using static Forms;
using static РабКонсоль;

namespace Defect2019
{
    public partial class Sources : Form
    {
        public Sources()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double x = textBox1.Text.ToDouble();
            double y = textBox4.Text.ToDouble();
            double r = textBox5.Text.ToDouble();
            int n = numericUpDown2.Value.ToInt32();

            Waves.Circle c = new Waves.Circle(new МатКлассы.Point(x, y), r);
            Waves.Normal2D[] norm = c.GetNormalsOnCircle(n);

            double[] w = SeqWMemoized(wbeg, wend, wcount);
            var fw = w.Map((double d) => Functions.F1(d) + new Number.Complex(RandomNumbers.NextDouble2(0, 0.01), RandomNumbers.NextDouble2(0, 0.01)));

            Source s = new Source(c.center, norm,
                (МатКлассы.Point p)=>c.ContainPoint(p),
                 fw,
                Source.Type.Circle,
                r);

            Uxt.sources.Add(s);
            Uform.Recostract();

            this.Close();
        }
    }
}
