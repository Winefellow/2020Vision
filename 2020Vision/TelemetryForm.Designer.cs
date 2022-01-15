
namespace Vision2020
{
    partial class TelemetryForm
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
            this.driverListBox = new System.Windows.Forms.PictureBox();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.splitter4 = new System.Windows.Forms.Splitter();
            this.circuitBoxBG = new System.Windows.Forms.PictureBox();
            this.ScreenUpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.TraceFileOpenLog = new System.Windows.Forms.OpenFileDialog();
            this.replayTimer = new System.Windows.Forms.Timer(this.components);
            this.lapLineBox = new System.Windows.Forms.PictureBox();
            this.circuitBox = new System.Windows.Forms.PictureBox();
            this.btnAddCompareLap = new System.Windows.Forms.Button();
            this.Analyze = new System.Windows.Forms.Button();
            this.RecordButton = new System.Windows.Forms.Button();
            this.replayButton = new System.Windows.Forms.Button();
            this.playerInfoBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.clickTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.driverListBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.circuitBoxBG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lapLineBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.circuitBox)).BeginInit();
            this.circuitBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.playerInfoBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // driverListBox
            // 
            this.driverListBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.driverListBox.Location = new System.Drawing.Point(933, 0);
            this.driverListBox.Name = "driverListBox";
            this.driverListBox.Size = new System.Drawing.Size(330, 609);
            this.driverListBox.TabIndex = 8;
            this.driverListBox.TabStop = false;
            this.driverListBox.Click += new System.EventHandler(this.DriverListBox_Click);
            this.driverListBox.Paint += new System.Windows.Forms.PaintEventHandler(this.DriverListBox_Paint);
            this.driverListBox.Resize += new System.EventHandler(this.driverListBox_Resize);
            // 
            // tbLog
            // 
            this.tbLog.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tbLog.Location = new System.Drawing.Point(0, 534);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ReadOnly = true;
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbLog.Size = new System.Drawing.Size(933, 75);
            this.tbLog.TabIndex = 9;
            // 
            // splitter2
            // 
            this.splitter2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter2.Location = new System.Drawing.Point(0, 531);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(933, 3);
            this.splitter2.TabIndex = 10;
            this.splitter2.TabStop = false;
            // 
            // splitter3
            // 
            this.splitter3.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.splitter3.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter3.Location = new System.Drawing.Point(930, 0);
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(3, 531);
            this.splitter3.TabIndex = 11;
            this.splitter3.TabStop = false;
            // 
            // splitter4
            // 
            this.splitter4.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.splitter4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter4.Location = new System.Drawing.Point(0, 395);
            this.splitter4.Name = "splitter4";
            this.splitter4.Size = new System.Drawing.Size(930, 3);
            this.splitter4.TabIndex = 10;
            this.splitter4.TabStop = false;
            // 
            // circuitBoxBG
            // 
            this.circuitBoxBG.Dock = System.Windows.Forms.DockStyle.Fill;
            this.circuitBoxBG.Location = new System.Drawing.Point(0, 0);
            this.circuitBoxBG.Name = "circuitBoxBG";
            this.circuitBoxBG.Size = new System.Drawing.Size(930, 531);
            this.circuitBoxBG.TabIndex = 13;
            this.circuitBoxBG.TabStop = false;
            // 
            // ScreenUpdateTimer
            // 
            this.ScreenUpdateTimer.Interval = 50;
            this.ScreenUpdateTimer.Tick += new System.EventHandler(this.ScreenUpdateTimer_Tick);
            // 
            // TraceFileOpenLog
            // 
            this.TraceFileOpenLog.FileName = "openFileDialog1";
            this.TraceFileOpenLog.Filter = "Log File|*.log|All files|*.*";
            // 
            // replayTimer
            // 
            this.replayTimer.Interval = 5;
            this.replayTimer.Tick += new System.EventHandler(this.replayTimer_Tick);
            // 
            // lapLineBox
            // 
            this.lapLineBox.BackColor = System.Drawing.Color.Black;
            this.lapLineBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lapLineBox.Location = new System.Drawing.Point(0, 398);
            this.lapLineBox.Name = "lapLineBox";
            this.lapLineBox.Size = new System.Drawing.Size(930, 133);
            this.lapLineBox.TabIndex = 18;
            this.lapLineBox.TabStop = false;
            this.lapLineBox.Visible = false;
            this.lapLineBox.SizeChanged += new System.EventHandler(this.lapLineBox_SizeChanged);
            this.lapLineBox.Paint += new System.Windows.Forms.PaintEventHandler(this.lapLineBox_Paint);
            // 
            // circuitBox
            // 
            this.circuitBox.Controls.Add(this.btnAddCompareLap);
            this.circuitBox.Controls.Add(this.Analyze);
            this.circuitBox.Controls.Add(this.RecordButton);
            this.circuitBox.Controls.Add(this.replayButton);
            this.circuitBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.circuitBox.Location = new System.Drawing.Point(0, 0);
            this.circuitBox.Name = "circuitBox";
            this.circuitBox.Size = new System.Drawing.Size(930, 395);
            this.circuitBox.TabIndex = 19;
            this.circuitBox.TabStop = false;
            this.circuitBox.Click += new System.EventHandler(this.circuitBox_Click);
            this.circuitBox.Paint += new System.Windows.Forms.PaintEventHandler(this.circuitBox_Paint);
            this.circuitBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.circuitBox_MouseDown);
            this.circuitBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.circuitBox_MouseUp_1);
            this.circuitBox.Resize += new System.EventHandler(this.circuitBox_Resize);
            // 
            // btnAddCompareLap
            // 
            this.btnAddCompareLap.Location = new System.Drawing.Point(3, 90);
            this.btnAddCompareLap.Name = "btnAddCompareLap";
            this.btnAddCompareLap.Size = new System.Drawing.Size(75, 23);
            this.btnAddCompareLap.TabIndex = 21;
            this.btnAddCompareLap.Text = "Lap Compare";
            this.btnAddCompareLap.UseVisualStyleBackColor = true;
            this.btnAddCompareLap.Click += new System.EventHandler(this.btnAddCompareLap_Click);
            // 
            // Analyze
            // 
            this.Analyze.Location = new System.Drawing.Point(3, 32);
            this.Analyze.Name = "Analyze";
            this.Analyze.Size = new System.Drawing.Size(75, 23);
            this.Analyze.TabIndex = 20;
            this.Analyze.Text = "Analyze";
            this.Analyze.UseVisualStyleBackColor = true;
            this.Analyze.Click += new System.EventHandler(this.Analyze_Click);
            // 
            // RecordButton
            // 
            this.RecordButton.Location = new System.Drawing.Point(3, 61);
            this.RecordButton.Name = "RecordButton";
            this.RecordButton.Size = new System.Drawing.Size(75, 23);
            this.RecordButton.TabIndex = 19;
            this.RecordButton.Text = "Record";
            this.RecordButton.UseVisualStyleBackColor = true;
            this.RecordButton.Click += new System.EventHandler(this.RecordButton_Click);
            // 
            // replayButton
            // 
            this.replayButton.Location = new System.Drawing.Point(3, 3);
            this.replayButton.Name = "replayButton";
            this.replayButton.Size = new System.Drawing.Size(75, 23);
            this.replayButton.TabIndex = 18;
            this.replayButton.Text = "Replay";
            this.replayButton.UseVisualStyleBackColor = true;
            this.replayButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // playerInfoBindingSource
            // 
            this.playerInfoBindingSource.DataSource = typeof(Vision2020.PlayerInfo);
            // 
            // clickTimer
            // 
            this.clickTimer.Interval = 5;
            this.clickTimer.Tick += new System.EventHandler(this.clickTimer_Tick);
            // 
            // TelemetryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1263, 609);
            this.Controls.Add(this.circuitBox);
            this.Controls.Add(this.splitter4);
            this.Controls.Add(this.lapLineBox);
            this.Controls.Add(this.circuitBoxBG);
            this.Controls.Add(this.splitter3);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.driverListBox);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TelemetryForm";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TelemetryForm_FormClosing);
            this.Shown += new System.EventHandler(this.TelemetryForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.driverListBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.circuitBoxBG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lapLineBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.circuitBox)).EndInit();
            this.circuitBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.playerInfoBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.BindingSource playerInfoBindingSource;
        private System.Windows.Forms.PictureBox driverListBox;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.Splitter splitter3;
        private System.Windows.Forms.Splitter splitter4;
        private System.Windows.Forms.PictureBox circuitBoxBG;
        private System.Windows.Forms.Timer ScreenUpdateTimer;
        private System.Windows.Forms.OpenFileDialog TraceFileOpenLog;
        private System.Windows.Forms.Timer replayTimer;
        private System.Windows.Forms.PictureBox lapLineBox;
        private System.Windows.Forms.PictureBox circuitBox;
        private System.Windows.Forms.Button btnAddCompareLap;
        private System.Windows.Forms.Button Analyze;
        private System.Windows.Forms.Button RecordButton;
        private System.Windows.Forms.Button replayButton;
        private System.Windows.Forms.Timer clickTimer;
    }
}

