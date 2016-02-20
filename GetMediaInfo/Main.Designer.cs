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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.audio_info = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.general_info2 = new System.Windows.Forms.Label();
            this.general_info1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.video_info = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.sub_info = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.Associationstatus = new System.Windows.Forms.Label();
            this.Associate = new System.Windows.Forms.Button();
            this.RemoveAssociation = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.Version = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.GetMediaInfosDLL = new System.Windows.Forms.Label();
            this.StatusMediainfo = new System.Windows.Forms.Label();
            this.MediainfoDLL = new System.Windows.Forms.Label();
            this.StatusGetMediaInfos = new System.Windows.Forms.Label();
            this.filepathBox = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AllowDrop = true;
            this.panel1.Controls.Add(this.btnSalvaReport);
            this.panel1.Controls.Add(this.statusStrip1);
            this.panel1.Controls.Add(this.btnApriFileDialog);
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Controls.Add(this.filepathBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(403, 417);
            this.panel1.TabIndex = 0;
            this.panel1.DragDrop += new System.Windows.Forms.DragEventHandler(this.InputPath_DragDrop);
            this.panel1.DragEnter += new System.Windows.Forms.DragEventHandler(this.InputPath_DragEnter);
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
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 33);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 1;
            this.tabControl1.Size = new System.Drawing.Size(382, 354);
            this.tabControl1.TabIndex = 10;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tableLayoutPanel1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(374, 328);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Simple";
            this.tabPage2.UseVisualStyleBackColor = true;
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
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.tableLayoutPanel4);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(362, 101);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General Info";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(6, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(358, 16);
            this.label3.TabIndex = 2;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.general_info2, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.general_info1, 0, 0);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 36);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(364, 63);
            this.tableLayoutPanel4.TabIndex = 1;
            // 
            // general_info2
            // 
            this.general_info2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.general_info2.Location = new System.Drawing.Point(185, 0);
            this.general_info2.Name = "general_info2";
            this.general_info2.Size = new System.Drawing.Size(176, 63);
            this.general_info2.TabIndex = 1;
            // 
            // general_info1
            // 
            this.general_info1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.general_info1.Location = new System.Drawing.Point(3, 0);
            this.general_info1.Name = "general_info1";
            this.general_info1.Size = new System.Drawing.Size(176, 63);
            this.general_info1.TabIndex = 0;
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
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textBox2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(374, 328);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Standard";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            this.textBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox2.Location = new System.Drawing.Point(3, 3);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox2.Size = new System.Drawing.Size(368, 322);
            this.textBox2.TabIndex = 2;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.textBox3);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(374, 328);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Complete";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // textBox3
            // 
            this.textBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox3.Location = new System.Drawing.Point(3, 3);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox3.Size = new System.Drawing.Size(368, 322);
            this.textBox3.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.Version);
            this.tabPage3.Controls.Add(this.groupBox5);
            this.tabPage3.Controls.Add(this.label2);
            this.tabPage3.Controls.Add(this.tableLayoutPanel3);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(374, 328);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Info";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.Associationstatus);
            this.groupBox5.Controls.Add(this.Associate);
            this.groupBox5.Controls.Add(this.RemoveAssociation);
            this.groupBox5.Location = new System.Drawing.Point(7, 109);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(361, 50);
            this.groupBox5.TabIndex = 9;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "ContextMenu Options";
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
            // Version
            // 
            this.Version.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Version.AutoSize = true;
            this.Version.Location = new System.Drawing.Point(302, 312);
            this.Version.Name = "Version";
            this.Version.Size = new System.Drawing.Size(66, 13);
            this.Version.TabIndex = 5;
            this.Version.Text = "Version x.x.x";
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
            this.filepathBox.TextChanged += new System.EventHandler(this.filepathBox_TextChanged);
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
            this.Load += new System.EventHandler(this.GetMediaInfo_Load);
            this.Shown += new System.EventHandler(this.GetMediaInfo_Shown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSalvaReport;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button btnApriFileDialog;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label audio_info;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label general_info2;
        private System.Windows.Forms.Label general_info1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label video_info;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label sub_info;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TabPage tabPage3;
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
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label Associationstatus;
    }
}

