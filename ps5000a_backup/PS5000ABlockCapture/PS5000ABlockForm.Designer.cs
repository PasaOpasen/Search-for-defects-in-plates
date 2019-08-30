/**************************************************************************
 * 
 * Filename: PS5000ABlockForm.Designer.cs
 * 
 * Description:
 *  Windows Form Designer Class for PS5000ABlockCapture project.
 *  
 *  Copyright (C) 2016 - 2017 Pico Technology Ltd. See LICENSE file for terms.  
 *
 **************************************************************************/

namespace PS5000A
{
    partial class PS5000ABlockForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabUnit = new System.Windows.Forms.TabPage();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.textBox11 = new System.Windows.Forms.TextBox();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.tabChannels = new System.Windows.Forms.TabPage();
            this.comboRangeB = new System.Windows.Forms.ComboBox();
            this.comboRangeC = new System.Windows.Forms.ComboBox();
            this.comboRangeD = new System.Windows.Forms.ComboBox();
            this.comboRangeA = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.tabGetData = new System.Windows.Forms.TabPage();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox12 = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.buttonStart = new System.Windows.Forms.Button();
            this.tabAbout = new System.Windows.Forms.TabPage();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.textBoxUnitInfo = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.button3 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.label19 = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tabUnit.SuspendLayout();
            this.tabChannels.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabGetData.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabAbout.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOpen
            // 
            this.buttonOpen.Location = new System.Drawing.Point(22, 22);
            this.buttonOpen.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(160, 64);
            this.buttonOpen.TabIndex = 0;
            this.buttonOpen.Text = "Запуск";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabUnit);
            this.tabControl.Controls.Add(this.tabChannels);
            this.tabControl.Controls.Add(this.tabGetData);
            this.tabControl.Controls.Add(this.tabAbout);
            this.tabControl.Location = new System.Drawing.Point(18, 18);
            this.tabControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(701, 310);
            this.tabControl.TabIndex = 3;
            // 
            // tabUnit
            // 
            this.tabUnit.Controls.Add(this.label16);
            this.tabUnit.Controls.Add(this.label15);
            this.tabUnit.Controls.Add(this.textBox11);
            this.tabUnit.Controls.Add(this.textBox10);
            this.tabUnit.Controls.Add(this.label14);
            this.tabUnit.Controls.Add(this.textBox9);
            this.tabUnit.Controls.Add(this.label13);
            this.tabUnit.Controls.Add(this.buttonOpen);
            this.tabUnit.Location = new System.Drawing.Point(4, 29);
            this.tabUnit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabUnit.Name = "tabUnit";
            this.tabUnit.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabUnit.Size = new System.Drawing.Size(693, 277);
            this.tabUnit.TabIndex = 0;
            this.tabUnit.Text = "Модуль";
            this.tabUnit.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(259, 240);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(410, 16);
            this.label16.TabIndex = 10;
            this.label16.Text = "*Шаг по времени определяется как (timebase-2) /125000000 sek";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(258, 170);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(150, 20);
            this.label15.TabIndex = 9;
            this.label15.Text = "Число усреднений";
            // 
            // textBox11
            // 
            this.textBox11.Location = new System.Drawing.Point(435, 164);
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new System.Drawing.Size(100, 26);
            this.textBox11.TabIndex = 8;
            this.textBox11.Text = "50";
            this.textBox11.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox10
            // 
            this.textBox10.Location = new System.Drawing.Point(435, 123);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(100, 26);
            this.textBox10.TabIndex = 6;
            this.textBox10.Text = "50000";
            this.textBox10.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(300, 129);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(108, 20);
            this.label14.TabIndex = 5;
            this.label14.Text = "Число шагов";
            // 
            // textBox9
            // 
            this.textBox9.Location = new System.Drawing.Point(435, 85);
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new System.Drawing.Size(100, 26);
            this.textBox9.TabIndex = 4;
            this.textBox9.Text = "15";
            this.textBox9.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox9.TextChanged += new System.EventHandler(this.textBox9_TextChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(334, 88);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(74, 20);
            this.label13.TabIndex = 3;
            this.label13.Text = "timebase";
            // 
            // tabChannels
            // 
            this.tabChannels.Controls.Add(this.comboRangeB);
            this.tabChannels.Controls.Add(this.comboRangeC);
            this.tabChannels.Controls.Add(this.comboRangeD);
            this.tabChannels.Controls.Add(this.comboRangeA);
            this.tabChannels.Controls.Add(this.groupBox1);
            this.tabChannels.Location = new System.Drawing.Point(4, 29);
            this.tabChannels.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabChannels.Name = "tabChannels";
            this.tabChannels.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabChannels.Size = new System.Drawing.Size(693, 277);
            this.tabChannels.TabIndex = 1;
            this.tabChannels.Text = "Каналы";
            this.tabChannels.UseVisualStyleBackColor = true;
            // 
            // comboRangeB
            // 
            this.comboRangeB.FormattingEnabled = true;
            this.comboRangeB.Location = new System.Drawing.Point(19, 119);
            this.comboRangeB.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.comboRangeB.Name = "comboRangeB";
            this.comboRangeB.Size = new System.Drawing.Size(180, 28);
            this.comboRangeB.TabIndex = 3;
            this.comboRangeB.Text = "Range B";
            // 
            // comboRangeC
            // 
            this.comboRangeC.FormattingEnabled = true;
            this.comboRangeC.Location = new System.Drawing.Point(19, 161);
            this.comboRangeC.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.comboRangeC.Name = "comboRangeC";
            this.comboRangeC.Size = new System.Drawing.Size(180, 28);
            this.comboRangeC.TabIndex = 2;
            this.comboRangeC.Text = "Range C";
            // 
            // comboRangeD
            // 
            this.comboRangeD.FormattingEnabled = true;
            this.comboRangeD.Location = new System.Drawing.Point(19, 202);
            this.comboRangeD.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.comboRangeD.Name = "comboRangeD";
            this.comboRangeD.Size = new System.Drawing.Size(180, 28);
            this.comboRangeD.TabIndex = 1;
            this.comboRangeD.Text = "Range D";
            // 
            // comboRangeA
            // 
            this.comboRangeA.FormattingEnabled = true;
            this.comboRangeA.Location = new System.Drawing.Point(19, 78);
            this.comboRangeA.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.comboRangeA.Name = "comboRangeA";
            this.comboRangeA.Size = new System.Drawing.Size(180, 28);
            this.comboRangeA.TabIndex = 0;
            this.comboRangeA.Text = "Range A";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox7);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBox8);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.textBox5);
            this.groupBox1.Controls.Add(this.textBox4);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textBox6);
            this.groupBox1.Controls.Add(this.textBox3);
            this.groupBox1.Location = new System.Drawing.Point(220, 52);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(404, 190);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Имена для файлов (указать центры источников)";
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(254, 150);
            this.textBox7.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(97, 26);
            this.textBox7.TabIndex = 23;
            this.textBox7.Text = "0";
            this.textBox7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 35);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "f(w) from (";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(359, 153);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(14, 20);
            this.label10.TabIndex = 22;
            this.label10.Text = ")";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(128, 30);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(97, 26);
            this.textBox1.TabIndex = 4;
            this.textBox1.Text = "400";
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(233, 154);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(13, 20);
            this.label11.TabIndex = 21;
            this.label11.Text = ",";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(233, 33);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = ",";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(40, 156);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(80, 20);
            this.label12.TabIndex = 20;
            this.label12.Text = "f(w) from (";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(359, 32);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = ")";
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(128, 151);
            this.textBox8.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(97, 26);
            this.textBox8.TabIndex = 19;
            this.textBox8.Text = "3";
            this.textBox8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(254, 29);
            this.textBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(97, 26);
            this.textBox2.TabIndex = 8;
            this.textBox2.Text = "0";
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(254, 109);
            this.textBox5.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(97, 26);
            this.textBox5.TabIndex = 18;
            this.textBox5.Text = "200";
            this.textBox5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(128, 68);
            this.textBox4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(97, 26);
            this.textBox4.TabIndex = 9;
            this.textBox4.Text = "0";
            this.textBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(359, 112);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(14, 20);
            this.label7.TabIndex = 17;
            this.label7.Text = ")";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(40, 73);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 20);
            this.label6.TabIndex = 10;
            this.label6.Text = "f(w) from (";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(233, 113);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(13, 20);
            this.label8.TabIndex = 16;
            this.label8.Text = ",";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(233, 71);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(13, 20);
            this.label5.TabIndex = 11;
            this.label5.Text = ",";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(40, 115);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(80, 20);
            this.label9.TabIndex = 15;
            this.label9.Text = "f(w) from (";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(359, 70);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 20);
            this.label4.TabIndex = 12;
            this.label4.Text = ")";
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(128, 110);
            this.textBox6.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(97, 26);
            this.textBox6.TabIndex = 14;
            this.textBox6.Text = "0";
            this.textBox6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(254, 67);
            this.textBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(97, 26);
            this.textBox3.TabIndex = 13;
            this.textBox3.Text = "0";
            this.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tabGetData
            // 
            this.tabGetData.Controls.Add(this.checkBox3);
            this.tabGetData.Controls.Add(this.checkBox2);
            this.tabGetData.Controls.Add(this.button4);
            this.tabGetData.Controls.Add(this.groupBox2);
            this.tabGetData.Controls.Add(this.button2);
            this.tabGetData.Controls.Add(this.textBox12);
            this.tabGetData.Controls.Add(this.label17);
            this.tabGetData.Controls.Add(this.buttonStart);
            this.tabGetData.Location = new System.Drawing.Point(4, 29);
            this.tabGetData.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabGetData.Name = "tabGetData";
            this.tabGetData.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabGetData.Size = new System.Drawing.Size(693, 277);
            this.tabGetData.TabIndex = 3;
            this.tabGetData.Text = "Получение информации";
            this.tabGetData.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Checked = true;
            this.checkBox3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox3.Location = new System.Drawing.Point(202, 112);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(163, 24);
            this.checkBox3.TabIndex = 7;
            this.checkBox3.Text = "Строить графики";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(202, 142);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(275, 24);
            this.checkBox2.TabIndex = 6;
            this.checkBox2.Text = "Строить преобразование Фурье";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(418, 40);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(147, 56);
            this.button4.TabIndex = 5;
            this.button4.Text = "Создать разницу замеров";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButton2);
            this.groupBox2.Controls.Add(this.radioButton1);
            this.groupBox2.Location = new System.Drawing.Point(202, 21);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(184, 83);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Замер выполняется";
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(24, 51);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(120, 24);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.Text = "с дефектом";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(24, 21);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(127, 24);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "без дефекта";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(559, 210);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(115, 42);
            this.button2.TabIndex = 3;
            this.button2.Text = "Изменить";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox12
            // 
            this.textBox12.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox12.Location = new System.Drawing.Point(226, 210);
            this.textBox12.Multiline = true;
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new System.Drawing.Size(313, 42);
            this.textBox12.TabIndex = 2;
            this.textBox12.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(24, 221);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(196, 20);
            this.label17.TabIndex = 1;
            this.label17.Text = "Директория с замерами:";
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(18, 42);
            this.buttonStart.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(153, 46);
            this.buttonStart.TabIndex = 0;
            this.buttonStart.Text = "Пуск замера";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // tabAbout
            // 
            this.tabAbout.Controls.Add(this.label19);
            this.tabAbout.Controls.Add(this.listBox2);
            this.tabAbout.Controls.Add(this.listBox1);
            this.tabAbout.Controls.Add(this.button7);
            this.tabAbout.Controls.Add(this.button6);
            this.tabAbout.Controls.Add(this.label18);
            this.tabAbout.Controls.Add(this.button5);
            this.tabAbout.Controls.Add(this.button1);
            this.tabAbout.Controls.Add(this.textBoxUnitInfo);
            this.tabAbout.Location = new System.Drawing.Point(4, 29);
            this.tabAbout.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabAbout.Name = "tabAbout";
            this.tabAbout.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabAbout.Size = new System.Drawing.Size(693, 277);
            this.tabAbout.TabIndex = 2;
            this.tabAbout.Text = "Дополнительно";
            this.tabAbout.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(12, 197);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(128, 53);
            this.button7.TabIndex = 9;
            this.button7.Text = "Получить имена";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(12, 146);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(128, 45);
            this.button6.TabIndex = 8;
            this.button6.Text = "Подключится";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.label18.Location = new System.Drawing.Point(219, 17);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(38, 20);
            this.label18.TabIndex = 7;
            this.label18.Text = "Port";
            this.label18.Click += new System.EventHandler(this.label18_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(12, 78);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(128, 51);
            this.button5.TabIndex = 6;
            this.button5.Text = "подключить вход";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 17);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(128, 55);
            this.button1.TabIndex = 5;
            this.button1.Text = "подключить выход";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBoxUnitInfo
            // 
            this.textBoxUnitInfo.Location = new System.Drawing.Point(344, 0);
            this.textBoxUnitInfo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxUnitInfo.Multiline = true;
            this.textBoxUnitInfo.Name = "textBoxUnitInfo";
            this.textBoxUnitInfo.Size = new System.Drawing.Size(349, 277);
            this.textBoxUnitInfo.TabIndex = 3;
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 386);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(732, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Maximum = 10000;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(78, 359);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(346, 24);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.Text = "Построить графики nArrays при закрытии";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Location = new System.Drawing.Point(496, 336);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(218, 46);
            this.button3.TabIndex = 5;
            this.button3.Text = "Всё готово. Закрыть";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 20;
            this.listBox1.Location = new System.Drawing.Point(202, 174);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(76, 84);
            this.listBox1.TabIndex = 12;
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.ItemHeight = 20;
            this.listBox2.Location = new System.Drawing.Point(202, 45);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(76, 84);
            this.listBox2.TabIndex = 13;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.label19.Location = new System.Drawing.Point(217, 146);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(45, 20);
            this.label19.TabIndex = 14;
            this.label19.Text = "COM";
            // 
            // PS5000ABlockForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(732, 408);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "PS5000ABlockForm";
            this.Text = "PS5000ABlockCapture";
            this.tabControl.ResumeLayout(false);
            this.tabUnit.ResumeLayout(false);
            this.tabUnit.PerformLayout();
            this.tabChannels.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabGetData.ResumeLayout(false);
            this.tabGetData.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabAbout.ResumeLayout(false);
            this.tabAbout.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabUnit;
        private System.Windows.Forms.TabPage tabChannels;
        private System.Windows.Forms.ComboBox comboRangeA;
        private System.Windows.Forms.TabPage tabAbout;
        private System.Windows.Forms.ComboBox comboRangeB;
        private System.Windows.Forms.ComboBox comboRangeC;
        private System.Windows.Forms.ComboBox comboRangeD;
        private System.Windows.Forms.TabPage tabGetData;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.Timer timer1;
        public System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox textBox11;
        private System.Windows.Forms.TextBox textBox10;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBox9;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox textBoxUnitInfo;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox12;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.Label label19;
    }
}

