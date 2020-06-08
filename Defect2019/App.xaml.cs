using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

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
            //window.Title = $"Время последней компиляции: {DateTime.Now}";

            SetWebBrowserCompatiblityLevel("Работа2019.exe");

            //SetExeptions();
            CopyFiles();
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

        #region Correct js scripts
        private static void SetWebBrowserCompatiblityLevel(string Application_ExecutablePath)
        {
            string appName = Path.GetFileNameWithoutExtension(Application_ExecutablePath);
            int lvl = 1000 * GetBrowserVersion();
            bool fixVShost = File.Exists(Path.ChangeExtension(Application_ExecutablePath, ".vshost.exe"));

            WriteCompatiblityLevel("HKEY_LOCAL_MACHINE", appName + ".exe", lvl);
            if (fixVShost) WriteCompatiblityLevel("HKEY_LOCAL_MACHINE", appName + ".vshost.exe", lvl);

            WriteCompatiblityLevel("HKEY_CURRENT_USER", appName + ".exe", lvl);
            if (fixVShost) WriteCompatiblityLevel("HKEY_CURRENT_USER", appName + ".vshost.exe", lvl);
        }

        private static void WriteCompatiblityLevel(string root, string appName, int lvl)
        {
            try
            {
                Microsoft.Win32.Registry.SetValue(root + @"\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", appName, lvl);
            }
            catch (Exception)
            {
            }
        }

        public static int GetBrowserVersion()
        {
            string strKeyPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Internet Explorer";
            string[] ls = new string[] { "svcVersion", "svcUpdateVersion", "Version", "W2kVersion" };

            int maxVer = 0;
            for (int i = 0; i < ls.Length; ++i)
            {
                object objVal = Microsoft.Win32.Registry.GetValue(strKeyPath, ls[i], "0");
                string strVal = Convert.ToString(objVal);
                if (strVal != null)
                {
                    int iPos = strVal.IndexOf('.');
                    if (iPos > 0)
                        strVal = strVal.Substring(0, iPos);

                    int res = 0;
                    if (int.TryParse(strVal, out res))
                        maxVer = Math.Max(maxVer, res);
                }
            }

            return maxVer;
        }
        #endregion

        private static void CopyFiles()
        {
            string dir = Path.GetDirectoryName(Path.GetDirectoryName(Environment.CurrentDirectory));
            dir = Path.Combine(dir, "Resources");

            string name;
            void Make(string file)
            {
                name = Path.GetFileName(file);
                if (!File.Exists(name) || new System.IO.FileInfo(name).LastWriteTime < new System.IO.FileInfo(file).LastWriteTime)
                    File.Copy(file, name, true);
            }

            foreach (string file in Directory.EnumerateFiles(dir, "*.r"))
            {
                Make(file);
            }
            //foreach (string file in Directory.EnumerateFiles(dir, "*.txt"))
            //{
            //    Make(file);
            //}

        }
    }
}
