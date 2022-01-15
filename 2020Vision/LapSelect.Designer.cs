
namespace Vision2020
{
    partial class LapSelect
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
            this.lapList = new System.Windows.Forms.ListView();
            this.colCircuit = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSessionType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colNummer = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPlayername = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTeam = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLapTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tbFilter = new System.Windows.Forms.TextBox();
            this.lvSelected = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.OKButton = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lapList
            // 
            this.lapList.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.lapList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lapList.CheckBoxes = true;
            this.lapList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colCircuit,
            this.colSessionType,
            this.colNummer,
            this.colPlayername,
            this.colTeam,
            this.colLapTime});
            this.lapList.FullRowSelect = true;
            this.lapList.HideSelection = false;
            this.lapList.Location = new System.Drawing.Point(12, 37);
            this.lapList.Name = "lapList";
            this.lapList.Size = new System.Drawing.Size(574, 477);
            this.lapList.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.lapList.TabIndex = 0;
            this.lapList.UseCompatibleStateImageBehavior = false;
            this.lapList.View = System.Windows.Forms.View.Details;
            this.lapList.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lapList_ColumnClick);
            this.lapList.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lapList_ItemChecked);
            this.lapList.SelectedIndexChanged += new System.EventHandler(this.lapList_SelectedIndexChanged);
            // 
            // colCircuit
            // 
            this.colCircuit.Text = "Circuit";
            this.colCircuit.Width = 108;
            // 
            // colSessionType
            // 
            this.colSessionType.Text = "Type sessie";
            this.colSessionType.Width = 92;
            // 
            // colNummer
            // 
            this.colNummer.Text = "NR";
            this.colNummer.Width = 29;
            // 
            // colPlayername
            // 
            this.colPlayername.Text = "Naam";
            this.colPlayername.Width = 144;
            // 
            // colTeam
            // 
            this.colTeam.Text = "Team";
            // 
            // colLapTime
            // 
            this.colLapTime.Text = "Rondetijd";
            // 
            // tbFilter
            // 
            this.tbFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFilter.Location = new System.Drawing.Point(12, 11);
            this.tbFilter.Name = "tbFilter";
            this.tbFilter.Size = new System.Drawing.Size(574, 20);
            this.tbFilter.TabIndex = 2;
            // 
            // lvSelected
            // 
            this.lvSelected.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.lvSelected.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvSelected.CheckBoxes = true;
            this.lvSelected.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.lvSelected.FullRowSelect = true;
            this.lvSelected.HideSelection = false;
            this.lvSelected.HotTracking = true;
            this.lvSelected.HoverSelection = true;
            this.lvSelected.Location = new System.Drawing.Point(594, 37);
            this.lvSelected.Name = "lvSelected";
            this.lvSelected.Size = new System.Drawing.Size(574, 477);
            this.lvSelected.TabIndex = 3;
            this.lvSelected.UseCompatibleStateImageBehavior = false;
            this.lvSelected.View = System.Windows.Forms.View.Details;
            this.lvSelected.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvSelected_ItemChecked);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Circuit";
            this.columnHeader1.Width = 108;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Type sessie";
            this.columnHeader2.Width = 92;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "NR";
            this.columnHeader3.Width = 29;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Naam";
            this.columnHeader4.Width = 144;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Team";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Rondetijd";
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(1093, 520);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 4;
            this.OKButton.Text = "&OK";
            this.OKButton.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDelete.Location = new System.Drawing.Point(12, 520);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(141, 23);
            this.btnDelete.TabIndex = 5;
            this.btnDelete.Text = "&Delete checked items";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // LapSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1181, 552);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.lvSelected);
            this.Controls.Add(this.tbFilter);
            this.Controls.Add(this.lapList);
            this.Name = "LapSelect";
            this.Text = "LapSelect";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lapList;
        private System.Windows.Forms.ColumnHeader colCircuit;
        private System.Windows.Forms.ColumnHeader colSessionType;
        private System.Windows.Forms.ColumnHeader colNummer;
        private System.Windows.Forms.ColumnHeader colPlayername;
        private System.Windows.Forms.ColumnHeader colTeam;
        private System.Windows.Forms.ColumnHeader colLapTime;
        private System.Windows.Forms.TextBox tbFilter;
        private System.Windows.Forms.ListView lvSelected;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button btnDelete;
    }
}