/**************************************************************************
 *
 * Filename: PS5000ABlockCapture.cs
 * 
 * Description:
 *   Provide the main entry point for the application.
 *   
 * Copyright (C) 2013 - 2017 Pico Technology Ltd. See LICENSE file for terms.
 *
 **************************************************************************/

using System;
using System.Windows.Forms;

namespace PS5000A
{
    public static class PS5000ABlockCapture
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Application.Run(new PS5000ABlockForm(0.01256637061435917295385057353312E+0, 7.9796453401180748256951141935299E-001, 250)); // 2k..127k
              Application.Run(new PS5000ABlockForm(0.01256637061435917295385057353312E+0, 1.2566370614359172953850573533118, 400)); //2k..200k
        }
    }
}
