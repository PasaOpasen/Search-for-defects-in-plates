using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Работа2019
{
    public partial class WaveletContinious : Form
    {
        private Source[] sources;
        public WaveletContinious(Source[] array)
        {
            InitializeComponent();
            sources = array;
        }
    }
}
