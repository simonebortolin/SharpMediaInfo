using System.IO;

namespace GetMediaInfo
{
    partial class GetMediaInfo
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GetMediaInfo));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSalvaReport = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnApriFileDialog = new System.Windows.Forms.Button();
            this.TabReport = new System.Windows.Forms.TabControl();
            this.TabSimple = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.audio_info = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.GeneralInfo = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.video_info = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.sub_info = new System.Windows.Forms.Label();
            this.TabStandard = new System.Windows.Forms.TabPage();
            this.TextBoxStandard = new System.Windows.Forms.TextBox();
            this.TabComplete = new System.Windows.Forms.TabPage();
            this.TextBoxComplete = new System.Windows.Forms.TextBox();
            this.TabInfo = new System.Windows.Forms.TabPage();
            this.Setting = new System.Windows.Forms.GroupBox();
            this.DisableSimpleInfo = new System.Windows.Forms.CheckBox();
            this.AutoSaveReport = new System.Windows.Forms.CheckBox();
            this.Version = new System.Windows.Forms.Label();
            this.ContextMenu = new System.Windows.Forms.GroupBox();
            this.Associationstatus = new System.Windows.Forms.Label();
            this.Associate = new System.Windows.Forms.Button();
            this.RemoveAssociation = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.GetMediaInfosDLL = new System.Windows.Forms.Label();
            this.StatusMediainfo = new System.Windows.Forms.Label();
            this.MediainfoDLL = new System.Windows.Forms.Label();
            this.StatusGetMediaInfos = new System.Windows.Forms.Label();
            this.filepathBox = new System.Windows.Forms.TextBox();
            this.MilliSec = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.TabReport.SuspendLayout();
            this.TabSimple.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.TabStandard.SuspendLayout();
            this.TabComplete.SuspendLayout();
            this.TabInfo.SuspendLayout();
            this.Setting.SuspendLayout();
            this.ContextMenu.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AllowDrop = true;
            this.panel1.Controls.Add(this.btnSalvaReport);
            this.panel1.Controls.Add(this.statusStrip1);
            this.panel1.Controls.Add(this.btnApriFileDialog);
            this.panel1.Controls.Add(this.TabReport);
            this.panel1.Controls.Add(this.filepathBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(403, 417);
            this.panel1.TabIndex = 0;
            this.panel1.DragDrop += new System.Windows.Forms.DragEventHandler(this.ApriFileDragDrop);
            this.panel1.DragEnter += new System.Windows.Forms.DragEventHandler(this.ApriFileDragEnter);
            // 
            // btnSalvaReport
            // 
            this.btnSalvaReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSalvaReport.Location = new System.Drawing.Point(315, 5);
            this.btnSalvaReport.Name = "btnSalvaReport";
            this.btnSalvaReport.Size = new System.Drawing.Size(75, 23);
            this.btnSalvaReport.TabIndex = 13;
            this.btnSalvaReport.Text = "Save Report";
            this.btnSalvaReport.UseVisualStyleBackColor = true;
            this.btnSalvaReport.Click += new System.EventHandler(this.SalvaReportSuFile);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 395);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(403, 22);
            this.statusStrip1.TabIndex = 11;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(39, 17);
            this.toolStripStatusLabel1.Text = "Ready";
            // 
            // btnApriFileDialog
            // 
            this.btnApriFileDialog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApriFileDialog.Location = new System.Drawing.Point(245, 5);
            this.btnApriFileDialog.Name = "btnApriFileDialog";
            this.btnApriFileDialog.Size = new System.Drawing.Size(64, 23);
            this.btnApriFileDialog.TabIndex = 12;
            this.btnApriFileDialog.Text = "Open File";
            this.btnApriFileDialog.UseVisualStyleBackColor = true;
            this.btnApriFileDialog.Click += new System.EventHandler(this.ApriFileDialog);
            // 
            // TabReport
            // 
            this.TabReport.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TabReport.Controls.Add(this.TabSimple);
            this.TabReport.Controls.Add(this.TabStandard);
            this.TabReport.Controls.Add(this.TabComplete);
            this.TabReport.Controls.Add(this.TabInfo);
            this.TabReport.Location = new System.Drawing.Point(12, 33);
            this.TabReport.Name = "TabReport";
            this.TabReport.SelectedIndex = 1;
            this.TabReport.Size = new System.Drawing.Size(382, 354);
            this.TabReport.TabIndex = 10;
            // 
            // TabSimple
            // 
            this.TabSimple.Controls.Add(this.tableLayoutPanel1);
            this.TabSimple.Location = new System.Drawing.Point(4, 22);
            this.TabSimple.Name = "TabSimple";
            this.TabSimple.Padding = new System.Windows.Forms.Padding(3);
            this.TabSimple.Size = new System.Drawing.Size(374, 328);
            this.TabSimple.TabIndex = 1;
            this.TabSimple.Text = "Simple";
            this.TabSimple.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox4, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22.22222F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22.22222F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22.22222F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(368, 322);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.audio_info);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(3, 181);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(362, 65);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Track Audio";
            // 
            // audio_info
            // 
            this.audio_info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.audio_info.Location = new System.Drawing.Point(3, 16);
            this.audio_info.Name = "audio_info";
            this.audio_info.Size = new System.Drawing.Size(356, 46);
            this.audio_info.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.GeneralInfo);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(362, 101);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General Info";
            // 
            // GeneralInfo
            // 
            this.GeneralInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GeneralInfo.Location = new System.Drawing.Point(3, 16);
            this.GeneralInfo.Name = "GeneralInfo";
            this.GeneralInfo.Size = new System.Drawing.Size(356, 82);
            this.GeneralInfo.TabIndex = 2;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.video_info);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 110);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(362, 65);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Track Video";
            // 
            // video_info
            // 
            this.video_info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.video_info.Location = new System.Drawing.Point(3, 16);
            this.video_info.Name = "video_info";
            this.video_info.Size = new System.Drawing.Size(356, 46);
            this.video_info.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.sub_info);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(3, 252);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(362, 67);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Subtitle";
            // 
            // sub_info
            // 
            this.sub_info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sub_info.Location = new System.Drawing.Point(3, 16);
            this.sub_info.Name = "sub_info";
            this.sub_info.Size = new System.Drawing.Size(356, 48);
            this.sub_info.TabIndex = 0;
            // 
            // TabStandard
            // 
            this.TabStandard.Controls.Add(this.TextBoxStandard);
            this.TabStandard.Location = new System.Drawing.Point(4, 22);
            this.TabStandard.Name = "TabStandard";
            this.TabStandard.Padding = new System.Windows.Forms.Padding(3);
            this.TabStandard.Size = new System.Drawing.Size(374, 328);
            this.TabStandard.TabIndex = 0;
            this.TabStandard.Text = "Standard";
            this.TabStandard.UseVisualStyleBackColor = true;
            // 
            // TextBoxStandard
            // 
            this.TextBoxStandard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBoxStandard.Location = new System.Drawing.Point(3, 3);
            this.TextBoxStandard.Multiline = true;
            this.TextBoxStandard.Name = "TextBoxStandard";
            this.TextBoxStandard.ReadOnly = true;
            this.TextBoxStandard.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TextBoxStandard.Size = new System.Drawing.Size(368, 322);
            this.TextBoxStandard.TabIndex = 0;
            // 
            // TabComplete
            // 
            this.TabComplete.Controls.Add(this.TextBoxComplete);
            this.TabComplete.Location = new System.Drawing.Point(4, 22);
            this.TabComplete.Name = "TabComplete";
            this.TabComplete.Padding = new System.Windows.Forms.Padding(3);
            this.TabComplete.Size = new System.Drawing.Size(374, 328);
            this.TabComplete.TabIndex = 3;
            this.TabComplete.Text = "Complete";
            this.TabComplete.UseVisualStyleBackColor = true;
            // 
            // TextBoxComplete
            // 
            this.TextBoxComplete.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBoxComplete.Location = new System.Drawing.Point(3, 3);
            this.TextBoxComplete.Multiline = true;
            this.TextBoxComplete.Name = "TextBoxComplete";
            this.TextBoxComplete.ReadOnly = true;
            this.TextBoxComplete.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TextBoxComplete.Size = new System.Drawing.Size(368, 322);
            this.TextBoxComplete.TabIndex = 0;
            // 
            // TabInfo
            // 
            this.TabInfo.Controls.Add(this.Setting);
            this.TabInfo.Controls.Add(this.Version);
            this.TabInfo.Controls.Add(this.ContextMenu);
            this.TabInfo.Controls.Add(this.label2);
            this.TabInfo.Controls.Add(this.tableLayoutPanel3);
            this.TabInfo.Location = new System.Drawing.Point(4, 22);
            this.TabInfo.Name = "TabInfo";
            this.TabInfo.Padding = new System.Windows.Forms.Padding(3);
            this.TabInfo.Size = new System.Drawing.Size(374, 328);
            this.TabInfo.TabIndex = 2;
            this.TabInfo.Text = "Info";
            this.TabInfo.UseVisualStyleBackColor = true;
            // 
            // Setting
            // 
            this.Setting.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Setting.Controls.Add(this.MilliSec);
            this.Setting.Controls.Add(this.DisableSimpleInfo);
            this.Setting.Controls.Add(this.AutoSaveReport);
            this.Setting.Location = new System.Drawing.Point(6, 166);
            this.Setting.Name = "Setting";
            this.Setting.Size = new System.Drawing.Size(362, 72);
            this.Setting.TabIndex = 10;
            this.Setting.TabStop = false;
            this.Setting.Text = "Setting";
            // 
            // DisableSimpleInfo
            // 
            this.DisableSimpleInfo.AutoSize = true;
            this.DisableSimpleInfo.Location = new System.Drawing.Point(7, 34);
            this.DisableSimpleInfo.Name = "DisableSimpleInfo";
            this.DisableSimpleInfo.Size = new System.Drawing.Size(116, 17);
            this.DisableSimpleInfo.TabIndex = 2;
            this.DisableSimpleInfo.Text = "Disable Simple Info";
            this.DisableSimpleInfo.UseVisualStyleBackColor = true;
            this.DisableSimpleInfo.CheckedChanged += new System.EventHandler(this.DisableSimpleInfo_CheckedChanged);
            // 
            // AutoSaveReport
            // 
            this.AutoSaveReport.AutoSize = true;
            this.AutoSaveReport.Location = new System.Drawing.Point(7, 19);
            this.AutoSaveReport.Name = "AutoSaveReport";
            this.AutoSaveReport.Size = new System.Drawing.Size(111, 17);
            this.AutoSaveReport.TabIndex = 1;
            this.AutoSaveReport.Text = "Auto Save Report";
            this.AutoSaveReport.UseVisualStyleBackColor = true;
            // 
            // Version
            // 
            this.Version.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Version.AutoSize = true;
            this.Version.Location = new System.Drawing.Point(297, 312);
            this.Version.Name = "Version";
            this.Version.Size = new System.Drawing.Size(74, 13);
            this.Version.TabIndex = 5;
            this.Version.Text = "Version x.x.x.x";
            this.Version.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // ContextMenu
            // 
            this.ContextMenu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ContextMenu.Controls.Add(this.Associationstatus);
            this.ContextMenu.Controls.Add(this.Associate);
            this.ContextMenu.Controls.Add(this.RemoveAssociation);
            this.ContextMenu.Location = new System.Drawing.Point(7, 109);
            this.ContextMenu.Name = "ContextMenu";
            this.ContextMenu.Size = new System.Drawing.Size(361, 50);
            this.ContextMenu.TabIndex = 9;
            this.ContextMenu.TabStop = false;
            this.ContextMenu.Text = "ContextMenu Options";
            // 
            // Associationstatus
            // 
            this.Associationstatus.AutoSize = true;
            this.Associationstatus.Location = new System.Drawing.Point(162, 25);
            this.Associationstatus.Name = "Associationstatus";
            this.Associationstatus.Size = new System.Drawing.Size(0, 13);
            this.Associationstatus.TabIndex = 9;
            // 
            // Associate
            // 
            this.Associate.Location = new System.Drawing.Point(6, 20);
            this.Associate.Name = "Associate";
            this.Associate.Size = new System.Drawing.Size(72, 23);
            this.Associate.TabIndex = 7;
            this.Associate.Text = "Add";
            this.Associate.UseVisualStyleBackColor = true;
            this.Associate.Click += new System.EventHandler(this.Associate_Click);
            // 
            // RemoveAssociation
            // 
            this.RemoveAssociation.Location = new System.Drawing.Point(84, 20);
            this.RemoveAssociation.Name = "RemoveAssociation";
            this.RemoveAssociation.Size = new System.Drawing.Size(72, 23);
            this.RemoveAssociation.TabIndex = 8;
            this.RemoveAssociation.Text = "Remove";
            this.RemoveAssociation.UseVisualStyleBackColor = true;
            this.RemoveAssociation.Click += new System.EventHandler(this.RemoveAssociation_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(7, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(361, 46);
            this.label2.TabIndex = 6;
            this.label2.Text = "Developed by .\r\nCredits to  for GetMediaInfos.dll, the help, the" +
    " development consultancy, the compiling for all Windows x86/x64.";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 53.22581F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 46.77419F));
            this.tableLayoutPanel3.Controls.Add(this.GetMediaInfosDLL, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.StatusMediainfo, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.MediainfoDLL, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.StatusGetMediaInfos, 1, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(6, 13);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(248, 39);
            this.tableLayoutPanel3.TabIndex = 4;
            // 
            // GetMediaInfosDLL
            // 
            this.GetMediaInfosDLL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GetMediaInfosDLL.Location = new System.Drawing.Point(3, 0);
            this.GetMediaInfosDLL.Name = "GetMediaInfosDLL";
            this.GetMediaInfosDLL.Size = new System.Drawing.Size(126, 19);
            this.GetMediaInfosDLL.TabIndex = 0;
            this.GetMediaInfosDLL.Text = "Status GetMediaInfos.dll";
            // 
            // StatusMediainfo
            // 
            this.StatusMediainfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StatusMediainfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusMediainfo.Location = new System.Drawing.Point(135, 19);
            this.StatusMediainfo.Name = "StatusMediainfo";
            this.StatusMediainfo.Size = new System.Drawing.Size(110, 20);
            this.StatusMediainfo.TabIndex = 3;
            // 
            // MediainfoDLL
            // 
            this.MediainfoDLL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MediainfoDLL.Location = new System.Drawing.Point(3, 19);
            this.MediainfoDLL.Name = "MediainfoDLL";
            this.MediainfoDLL.Size = new System.Drawing.Size(126, 20);
            this.MediainfoDLL.TabIndex = 1;
            this.MediainfoDLL.Text = "Status MediaInfo.dll";
            // 
            // StatusGetMediaInfos
            // 
            this.StatusGetMediaInfos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StatusGetMediaInfos.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusGetMediaInfos.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.StatusGetMediaInfos.Location = new System.Drawing.Point(135, 0);
            this.StatusGetMediaInfos.Name = "StatusGetMediaInfos";
            this.StatusGetMediaInfos.Size = new System.Drawing.Size(110, 19);
            this.StatusGetMediaInfos.TabIndex = 2;
            // 
            // filepathBox
            // 
            this.filepathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filepathBox.Location = new System.Drawing.Point(12, 7);
            this.filepathBox.Name = "filepathBox";
            this.filepathBox.Size = new System.Drawing.Size(227, 20);
            this.filepathBox.TabIndex = 14;
            this.filepathBox.TextChanged += new System.EventHandler(this.ApriFileTextBox);
            // 
            // MilliSec
            // 
            this.MilliSec.AutoSize = true;
            this.MilliSec.Location = new System.Drawing.Point(7, 49);
            this.MilliSec.Name = "MilliSec";
            this.MilliSec.Size = new System.Drawing.Size(115, 17);
            this.MilliSec.TabIndex = 3;
            this.MilliSec.Text = "Active milliseconds";
            this.MilliSec.UseVisualStyleBackColor = true;
            // 
            // GetMediaInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 417);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(419, 456);
            this.Name = "GetMediaInfo";
            this.Text = "Get MediaInfo";
            this.Load += new System.EventHandler(this.PrimoAvvio);
            this.Shown += new System.EventHandler(this.GetMediaInfo_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.TabReport.ResumeLayout(false);
            this.TabSimple.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.TabStandard.ResumeLayout(false);
            this.TabStandard.PerformLayout();
            this.TabComplete.ResumeLayout(false);
            this.TabComplete.PerformLayout();
            this.TabInfo.ResumeLayout(false);
            this.TabInfo.PerformLayout();
            this.Setting.ResumeLayout(false);
            this.Setting.PerformLayout();
            this.ContextMenu.ResumeLayout(false);
            this.ContextMenu.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSalvaReport;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button btnApriFileDialog;
        private System.Windows.Forms.TabControl TabReport;
        private System.Windows.Forms.TabPage TabSimple;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label audio_info;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label GeneralInfo;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label video_info;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label sub_info;
        private System.Windows.Forms.TabPage TabStandard;
        private System.Windows.Forms.TextBox TextBoxStandard;
        private System.Windows.Forms.TabPage TabComplete;
        private System.Windows.Forms.TextBox TextBoxComplete;
        private System.Windows.Forms.TabPage TabInfo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label Version;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label GetMediaInfosDLL;
        private System.Windows.Forms.Label StatusMediainfo;
        private System.Windows.Forms.Label MediainfoDLL;
        private System.Windows.Forms.Label StatusGetMediaInfos;
        private System.Windows.Forms.TextBox filepathBox;
        private System.Windows.Forms.Button Associate;
        private System.Windows.Forms.Button RemoveAssociation;
        private System.Windows.Forms.GroupBox ContextMenu;
        private System.Windows.Forms.Label Associationstatus;
        private System.Windows.Forms.GroupBox Setting;
        private System.Windows.Forms.CheckBox DisableSimpleInfo;
        private System.Windows.Forms.CheckBox AutoSaveReport;
        private System.Windows.Forms.CheckBox MilliSec;
    }
}

