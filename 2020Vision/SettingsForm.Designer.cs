namespace Vision2020
{
    partial class SettingsForm
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
            this.cbDistance2Apex = new System.Windows.Forms.CheckBox();
            this.cbApexSpeed = new System.Windows.Forms.CheckBox();
            this.cbTimeSpent = new System.Windows.Forms.CheckBox();
            this.comboAnnounceSpeed = new System.Windows.Forms.DomainUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cbDistance2Apex
            // 
            this.cbDistance2Apex.AutoSize = true;
            this.cbDistance2Apex.Location = new System.Drawing.Point(12, 12);
            this.cbDistance2Apex.Name = "cbDistance2Apex";
            this.cbDistance2Apex.Size = new System.Drawing.Size(231, 20);
            this.cbDistance2Apex.TabIndex = 0;
            this.cbDistance2Apex.Text = "Annouce braking distance to apex";
            this.cbDistance2Apex.UseVisualStyleBackColor = true;
            // 
            // cbApexSpeed
            // 
            this.cbApexSpeed.AutoSize = true;
            this.cbApexSpeed.Location = new System.Drawing.Point(12, 38);
            this.cbApexSpeed.Name = "cbApexSpeed";
            this.cbApexSpeed.Size = new System.Drawing.Size(164, 20);
            this.cbApexSpeed.TabIndex = 1;
            this.cbApexSpeed.Text = "Announce apex speed";
            this.cbApexSpeed.UseVisualStyleBackColor = true;
            // 
            // cbTimeSpent
            // 
            this.cbTimeSpent.AutoSize = true;
            this.cbTimeSpent.Location = new System.Drawing.Point(12, 64);
            this.cbTimeSpent.Name = "cbTimeSpent";
            this.cbTimeSpent.Size = new System.Drawing.Size(158, 20);
            this.cbTimeSpent.TabIndex = 2;
            this.cbTimeSpent.Text = "Announce corner time";
            this.cbTimeSpent.UseVisualStyleBackColor = true;
            // 
            // comboAnnounceSpeed
            // 
            this.comboAnnounceSpeed.Items.Add("-2");
            this.comboAnnounceSpeed.Items.Add("-1");
            this.comboAnnounceSpeed.Items.Add("0");
            this.comboAnnounceSpeed.Items.Add("1");
            this.comboAnnounceSpeed.Items.Add("2");
            this.comboAnnounceSpeed.Items.Add("3");
            this.comboAnnounceSpeed.Items.Add("4");
            this.comboAnnounceSpeed.Items.Add("5");
            this.comboAnnounceSpeed.Items.Add("6");
            this.comboAnnounceSpeed.Location = new System.Drawing.Point(12, 90);
            this.comboAnnounceSpeed.Name = "comboAnnounceSpeed";
            this.comboAnnounceSpeed.Size = new System.Drawing.Size(120, 22);
            this.comboAnnounceSpeed.TabIndex = 3;
            this.comboAnnounceSpeed.Text = "domainUpDown1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(138, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "Announce speed";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(436, 142);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboAnnounceSpeed);
            this.Controls.Add(this.cbTimeSpent);
            this.Controls.Add(this.cbApexSpeed);
            this.Controls.Add(this.cbDistance2Apex);
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SettingsForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbDistance2Apex;
        private System.Windows.Forms.CheckBox cbApexSpeed;
        private System.Windows.Forms.CheckBox cbTimeSpent;
        private System.Windows.Forms.DomainUpDown comboAnnounceSpeed;
        private System.Windows.Forms.Label label1;
    }
}