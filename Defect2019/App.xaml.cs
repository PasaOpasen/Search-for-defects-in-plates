using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Defect2019
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        App()
        {
            InitializeComponent();
        }

        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        static void Main()
        {
            App app = new App();
            MainWindow window = new MainWindow();
            window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            window.Title = $"Время последней компиляции: {DateTime.Now}";

            SetExeptions();
            app.Run(window);
        }

        private static void SetExeptions()
        {
            AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs args) =>
            {
                OtherMethods.PlaySound("КритическаяОшибка");
                var ex = (args.ExceptionObject as Exception);
                if (MessageBox.Show($"Произошло исключение \" {ex.Message} \", которое не было перехвачено. Программа будет закрыта, стек исключения находится в файле Exeptions.txt",
                    "Неперехваченное исключение", MessageBoxButton.OK, MessageBoxImage.Exclamation) == MessageBoxResult.OK)
                {
                    МатКлассы.Expendator.WriteStringInFile("Exeptions.txt", (ex.StackTrace));
                    Environment.Exit(1);
                }

            };
        }
    }
}
