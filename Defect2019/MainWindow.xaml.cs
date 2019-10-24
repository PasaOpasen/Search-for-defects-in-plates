using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using МатКлассы;

namespace Defect2019
{
    public static class Global
    {
        public static DComplexFunc RRToC;
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Media.SoundPlayer SoundPlayer = new System.Media.SoundPlayer(Expendator.GetResource("rising-of-the-phoenix.wav", "Defect2019"));
        public MainWindow()
        {
            InitializeComponent();
            SoundPlayer.PlayLooping();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SoundPlayer.Stop();
            new kGrafic().Show();          
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            SoundPlayer.Stop();
            Forms.UG = new Практика_с_фортрана.UGrafic();
            Forms.UG.Show();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            SoundPlayer.Stop();
            Forms.Uform = new Uxt();
            Forms.Uform.Show();
        }
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            SoundPlayer.Stop();
        }
    }

}
