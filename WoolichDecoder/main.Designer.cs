namespace WoolichDecoder
{
    partial class WoolichFileDecoderForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WoolichFileDecoderForm));
            this.txtLogging = new System.Windows.Forms.TextBox();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.openWRLFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.lblFileName = new System.Windows.Forms.Label();
            this.txtFeedback = new System.Windows.Forms.TextBox();
            this.lblExportPacketsCount = new System.Windows.Forms.Label();
            this.lblExportFilename = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.cmbExportType = new System.Windows.Forms.ComboBox();
            this.lblExportType = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblTotalPackets = new System.Windows.Forms.Label();
            this.lblTotalPacketsCount = new System.Windows.Forms.Label();
            this.lblPackets = new System.Windows.Forms.Label();
            this.lblAnalysisCol = new System.Windows.Forms.Label();
            this.txtBreakOnChange = new System.Windows.Forms.TextBox();
            this.btnAnalyse = new System.Windows.Forms.Button();
            this.lblCRCsize = new System.Windows.Forms.Label();
            this.CRCsize = new System.Windows.Forms.TextBox();
            this.btnRepair = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.aTFCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cmbLogsLocation = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbBinDelete = new System.Windows.Forms.ComboBox();
            this.cmbATFileName = new System.Windows.Forms.ComboBox();
            this.btnAFR = new System.Windows.Forms.Button();
            this.AFRdivisor = new System.Windows.Forms.TextBox();
            this.cmbTextFilter = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.idleRPM = new System.Windows.Forms.TextBox();
            this.minRPM = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.maxRPM = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label14 = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.progressLabel = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.lblDirName = new System.Windows.Forms.Label();
            this.lblLogsLocation = new System.Windows.Forms.Label();
            this.cmbExportFormat = new System.Windows.Forms.ComboBox();
            this.lblExportFormat = new System.Windows.Forms.Label();
            this.lblExportMode = new System.Windows.Forms.Label();
            this.cmbExportMode = new System.Windows.Forms.ComboBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblBinDelete = new System.Windows.Forms.Label();
            this.lblATFileName = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtLogging
            // 
            this.txtLogging.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLogging.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtLogging.Location = new System.Drawing.Point(876, 58);
            this.txtLogging.Margin = new System.Windows.Forms.Padding(2);
            this.txtLogging.Multiline = true;
            this.txtLogging.Name = "txtLogging";
            this.txtLogging.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLogging.Size = new System.Drawing.Size(397, 516);
            this.txtLogging.TabIndex = 1;
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(10, 53);
            this.btnOpenFile.Margin = new System.Windows.Forms.Padding(2);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(108, 28);
            this.btnOpenFile.TabIndex = 2;
            this.btnOpenFile.Text = "Open File";
            this.toolTip1.SetToolTip(this.btnOpenFile, "Open WRL File");
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.OpenFile_Click);
            // 
            // openWRLFileDialog
            // 
            this.openWRLFileDialog.AddExtension = false;
            this.openWRLFileDialog.DefaultExt = "WRL";
            // 
            // lblFileName
            // 
            this.lblFileName.AutoSize = true;
            this.lblFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblFileName.Location = new System.Drawing.Point(119, 27);
            this.lblFileName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(33, 13);
            this.lblFileName.TabIndex = 3;
            this.lblFileName.Text = "None";
            // 
            // txtFeedback
            // 
            this.txtFeedback.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFeedback.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtFeedback.Location = new System.Drawing.Point(471, 59);
            this.txtFeedback.Margin = new System.Windows.Forms.Padding(2);
            this.txtFeedback.Multiline = true;
            this.txtFeedback.Name = "txtFeedback";
            this.txtFeedback.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtFeedback.Size = new System.Drawing.Size(401, 516);
            this.txtFeedback.TabIndex = 5;
            // 
            // lblExportPacketsCount
            // 
            this.lblExportPacketsCount.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblExportPacketsCount.BackColor = System.Drawing.SystemColors.Control;
            this.lblExportPacketsCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblExportPacketsCount.Enabled = false;
            this.lblExportPacketsCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblExportPacketsCount.Location = new System.Drawing.Point(364, 13);
            this.lblExportPacketsCount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblExportPacketsCount.Name = "lblExportPacketsCount";
            this.lblExportPacketsCount.Size = new System.Drawing.Size(78, 20);
            this.lblExportPacketsCount.TabIndex = 9;
            this.lblExportPacketsCount.Text = "0";
            this.lblExportPacketsCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.lblExportPacketsCount, "Found packets.");
            this.lblExportPacketsCount.UseMnemonic = false;
            // 
            // lblExportFilename
            // 
            this.lblExportFilename.AutoSize = true;
            this.lblExportFilename.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblExportFilename.Location = new System.Drawing.Point(129, 62);
            this.lblExportFilename.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblExportFilename.Name = "lblExportFilename";
            this.lblExportFilename.Size = new System.Drawing.Size(132, 13);
            this.lblExportFilename.TabIndex = 10;
            this.lblExportFilename.Text = "Export Filename undefined";
            this.lblExportFilename.Visible = false;
            // 
            // btnExport
            // 
            this.btnExport.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnExport.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.btnExport.Location = new System.Drawing.Point(330, 4);
            this.btnExport.Margin = new System.Windows.Forms.Padding(1, 1, 3, 1);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(121, 37);
            this.btnExport.TabIndex = 11;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.Export_Click);
            // 
            // cmbExportType
            // 
            this.cmbExportType.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cmbExportType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.cmbExportType.FormattingEnabled = true;
            this.cmbExportType.Items.AddRange(new object[] {
            "Export to",
            "Analysis Only",
            "CRC",
            "Autotune"});
            this.cmbExportType.Location = new System.Drawing.Point(102, 19);
            this.cmbExportType.Margin = new System.Windows.Forms.Padding(1);
            this.cmbExportType.Name = "cmbExportType";
            this.cmbExportType.Size = new System.Drawing.Size(160, 21);
            this.cmbExportType.TabIndex = 12;
            this.cmbExportType.Text = "Export to";
            // 
            // lblExportType
            // 
            this.lblExportType.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblExportType.AutoSize = true;
            this.lblExportType.Location = new System.Drawing.Point(99, 4);
            this.lblExportType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblExportType.Name = "lblExportType";
            this.lblExportType.Size = new System.Drawing.Size(39, 13);
            this.lblExportType.TabIndex = 15;
            this.lblExportType.Text = "Type:";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.lblTotalPackets);
            this.panel1.Controls.Add(this.lblTotalPacketsCount);
            this.panel1.Controls.Add(this.lblPackets);
            this.panel1.Controls.Add(this.lblAnalysisCol);
            this.panel1.Controls.Add(this.txtBreakOnChange);
            this.panel1.Controls.Add(this.btnAnalyse);
            this.panel1.Controls.Add(this.lblExportPacketsCount);
            this.panel1.Location = new System.Drawing.Point(10, 504);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(455, 70);
            this.panel1.TabIndex = 16;
            // 
            // lblTotalPackets
            // 
            this.lblTotalPackets.AutoSize = true;
            this.lblTotalPackets.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblTotalPackets.Location = new System.Drawing.Point(282, 44);
            this.lblTotalPackets.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTotalPackets.Name = "lblTotalPackets";
            this.lblTotalPackets.Size = new System.Drawing.Size(76, 13);
            this.lblTotalPackets.TabIndex = 38;
            this.lblTotalPackets.Text = "Total Packets:";
            // 
            // lblTotalPacketsCount
            // 
            this.lblTotalPacketsCount.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblTotalPacketsCount.BackColor = System.Drawing.SystemColors.Control;
            this.lblTotalPacketsCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTotalPacketsCount.Enabled = false;
            this.lblTotalPacketsCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblTotalPacketsCount.Location = new System.Drawing.Point(364, 40);
            this.lblTotalPacketsCount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTotalPacketsCount.Name = "lblTotalPacketsCount";
            this.lblTotalPacketsCount.Size = new System.Drawing.Size(78, 20);
            this.lblTotalPacketsCount.TabIndex = 37;
            this.lblTotalPacketsCount.Text = "0";
            this.lblTotalPacketsCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.lblTotalPacketsCount, "Found packets.");
            this.lblTotalPacketsCount.UseMnemonic = false;
            // 
            // lblPackets
            // 
            this.lblPackets.AutoSize = true;
            this.lblPackets.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblPackets.Location = new System.Drawing.Point(309, 17);
            this.lblPackets.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPackets.Name = "lblPackets";
            this.lblPackets.Size = new System.Drawing.Size(49, 13);
            this.lblPackets.TabIndex = 36;
            this.lblPackets.Text = "Packets:";
            // 
            // lblAnalysisCol
            // 
            this.lblAnalysisCol.AutoSize = true;
            this.lblAnalysisCol.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblAnalysisCol.Location = new System.Drawing.Point(118, 17);
            this.lblAnalysisCol.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblAnalysisCol.Name = "lblAnalysisCol";
            this.lblAnalysisCol.Size = new System.Drawing.Size(86, 13);
            this.lblAnalysisCol.TabIndex = 17;
            this.lblAnalysisCol.Text = "Analysis Column:";
            // 
            // txtBreakOnChange
            // 
            this.txtBreakOnChange.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBreakOnChange.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtBreakOnChange.Location = new System.Drawing.Point(212, 14);
            this.txtBreakOnChange.Margin = new System.Windows.Forms.Padding(2);
            this.txtBreakOnChange.Name = "txtBreakOnChange";
            this.txtBreakOnChange.Size = new System.Drawing.Size(48, 20);
            this.txtBreakOnChange.TabIndex = 16;
            this.txtBreakOnChange.Text = "35";
            this.txtBreakOnChange.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.txtBreakOnChange, "Which column to analyse.");
            this.txtBreakOnChange.TextChanged += new System.EventHandler(this.TxtBreakOnChange_TextChanged);
            // 
            // btnAnalyse
            // 
            this.btnAnalyse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnAnalyse.Location = new System.Drawing.Point(13, 9);
            this.btnAnalyse.Margin = new System.Windows.Forms.Padding(2);
            this.btnAnalyse.Name = "btnAnalyse";
            this.btnAnalyse.Size = new System.Drawing.Size(82, 28);
            this.btnAnalyse.TabIndex = 15;
            this.btnAnalyse.Text = "Analyse";
            this.toolTip1.SetToolTip(this.btnAnalyse, "Analyse single file or directory depends on Mode settigs in Export and Convert.");
            this.btnAnalyse.UseVisualStyleBackColor = true;
            this.btnAnalyse.Click += new System.EventHandler(this.Analyse_Click);
            // 
            // lblCRCsize
            // 
            this.lblCRCsize.AutoSize = true;
            this.lblCRCsize.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblCRCsize.Location = new System.Drawing.Point(5, 383);
            this.lblCRCsize.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCRCsize.Name = "lblCRCsize";
            this.lblCRCsize.Size = new System.Drawing.Size(69, 15);
            this.lblCRCsize.TabIndex = 38;
            this.lblCRCsize.Text = "CRC size:";
            // 
            // CRCsize
            // 
            this.CRCsize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CRCsize.Enabled = false;
            this.CRCsize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.CRCsize.Location = new System.Drawing.Point(8, 400);
            this.CRCsize.Margin = new System.Windows.Forms.Padding(2);
            this.CRCsize.Name = "CRCsize";
            this.CRCsize.Size = new System.Drawing.Size(66, 20);
            this.CRCsize.TabIndex = 37;
            this.CRCsize.Text = "100";
            this.CRCsize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.CRCsize, "How many packets to export.");
            // 
            // btnRepair
            // 
            this.btnRepair.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnRepair.Location = new System.Drawing.Point(122, 53);
            this.btnRepair.Margin = new System.Windows.Forms.Padding(2);
            this.btnRepair.Name = "btnRepair";
            this.btnRepair.Size = new System.Drawing.Size(137, 28);
            this.btnRepair.TabIndex = 40;
            this.btnRepair.Text = "Repair";
            this.toolTip1.SetToolTip(this.btnRepair, "Repair corrupted WRL ");
            this.btnRepair.UseVisualStyleBackColor = true;
            this.btnRepair.Click += new System.EventHandler(this.Repair_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label3.Location = new System.Drawing.Point(10, 488);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(386, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "log File Analysis Functions (To analyse a particular packet column)";
            // 
            // aTFCheckedListBox
            // 
            this.aTFCheckedListBox.Enabled = false;
            this.aTFCheckedListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.aTFCheckedListBox.FormattingEnabled = true;
            this.aTFCheckedListBox.Location = new System.Drawing.Point(10, 108);
            this.aTFCheckedListBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.aTFCheckedListBox.Name = "aTFCheckedListBox";
            this.aTFCheckedListBox.Size = new System.Drawing.Size(249, 79);
            this.aTFCheckedListBox.TabIndex = 17;
            this.toolTip1.SetToolTip(this.aTFCheckedListBox, "Tick required settings. More information at: https://github.com/mt09sp/woolich_lo" +
        "g_decoder");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label4.Location = new System.Drawing.Point(10, 90);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(166, 15);
            this.label4.TabIndex = 18;
            this.label4.Text = "Autotune export options: ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label5.Location = new System.Drawing.Point(873, 41);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 15);
            this.label5.TabIndex = 19;
            this.label5.Text = "log: ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label6.Location = new System.Drawing.Point(76, 24);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 15);
            this.label6.TabIndex = 20;
            this.label6.Text = "File:";
            // 
            // toolTip1
            // 
            this.toolTip1.Tag = "";
            // 
            // cmbLogsLocation
            // 
            this.cmbLogsLocation.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cmbLogsLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.cmbLogsLocation.FormattingEnabled = true;
            this.cmbLogsLocation.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmbLogsLocation.Items.AddRange(new object[] {
            "Default",
            "Work Directory"});
            this.cmbLogsLocation.Location = new System.Drawing.Point(90, 3);
            this.cmbLogsLocation.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cmbLogsLocation.Name = "cmbLogsLocation";
            this.cmbLogsLocation.Size = new System.Drawing.Size(101, 21);
            this.cmbLogsLocation.TabIndex = 42;
            this.cmbLogsLocation.Text = "Default";
            this.toolTip1.SetToolTip(this.cmbLogsLocation, "Default is set for application directory.");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 422);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 13);
            this.label1.TabIndex = 43;
            this.label1.Text = "Export and Convert:";
            this.toolTip1.SetToolTip(this.label1, "Export to WRL and convert to CSV. Single and Massive conversion available.");
            // 
            // cmbBinDelete
            // 
            this.cmbBinDelete.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cmbBinDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.cmbBinDelete.FormattingEnabled = true;
            this.cmbBinDelete.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmbBinDelete.Items.AddRange(new object[] {
            "Save",
            "Delete"});
            this.cmbBinDelete.Location = new System.Drawing.Point(90, 57);
            this.cmbBinDelete.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cmbBinDelete.Name = "cmbBinDelete";
            this.cmbBinDelete.Size = new System.Drawing.Size(101, 21);
            this.cmbBinDelete.TabIndex = 45;
            this.cmbBinDelete.Text = "Default";
            this.toolTip1.SetToolTip(this.cmbBinDelete, "Delete BIN files in Directory Mode.");
            // 
            // cmbATFileName
            // 
            this.cmbATFileName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cmbATFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.cmbATFileName.FormattingEnabled = true;
            this.cmbATFileName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmbATFileName.Items.AddRange(new object[] {
            "Default",
            "Binary"});
            this.cmbATFileName.Location = new System.Drawing.Point(90, 30);
            this.cmbATFileName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cmbATFileName.Name = "cmbATFileName";
            this.cmbATFileName.Size = new System.Drawing.Size(101, 21);
            this.cmbATFileName.TabIndex = 47;
            this.cmbATFileName.Text = "Default";
            this.toolTip1.SetToolTip(this.cmbATFileName, "Autotune Output Filename format.");
            // 
            // btnAFR
            // 
            this.btnAFR.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnAFR.Location = new System.Drawing.Point(2, 3);
            this.btnAFR.Margin = new System.Windows.Forms.Padding(1);
            this.btnAFR.Name = "btnAFR";
            this.btnAFR.Size = new System.Drawing.Size(76, 21);
            this.btnAFR.TabIndex = 49;
            this.btnAFR.Text = "Calibrate";
            this.toolTip1.SetToolTip(this.btnAFR, "Calibrate AFR between Woolich software and Decoder. Mitigate difference in CSV/TS" +
        "V files.");
            this.btnAFR.UseVisualStyleBackColor = true;
            this.btnAFR.Click += new System.EventHandler(this.CompareAFR_Click);
            // 
            // AFRdivisor
            // 
            this.AFRdivisor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.AFRdivisor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.AFRdivisor.Location = new System.Drawing.Point(81, 3);
            this.AFRdivisor.Margin = new System.Windows.Forms.Padding(1);
            this.AFRdivisor.Name = "AFRdivisor";
            this.AFRdivisor.Size = new System.Drawing.Size(55, 20);
            this.AFRdivisor.TabIndex = 50;
            this.AFRdivisor.Text = "10.2";
            this.AFRdivisor.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.AFRdivisor, "AFR Divisor");
            this.AFRdivisor.TextChanged += new System.EventHandler(this.AFRdivisor_TextChanged);
            // 
            // cmbTextFilter
            // 
            this.cmbTextFilter.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cmbTextFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.cmbTextFilter.FormattingEnabled = true;
            this.cmbTextFilter.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmbTextFilter.Items.AddRange(new object[] {
            "Minimal",
            "Full"});
            this.cmbTextFilter.Location = new System.Drawing.Point(90, 84);
            this.cmbTextFilter.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cmbTextFilter.Name = "cmbTextFilter";
            this.cmbTextFilter.Size = new System.Drawing.Size(101, 21);
            this.cmbTextFilter.TabIndex = 50;
            this.cmbTextFilter.Text = "Minimal";
            this.toolTip1.SetToolTip(this.cmbTextFilter, "Delete BIN files in Directory Mode.");
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label7.Location = new System.Drawing.Point(38, 6);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(103, 13);
            this.label7.TabIndex = 21;
            this.label7.Text = "Filter out Idle RPM <";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label8.Location = new System.Drawing.Point(12, 10);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(129, 13);
            this.label8.TabIndex = 22;
            this.label8.Text = "Filter out RPM in Gear 1 <\r\n";
            // 
            // idleRPM
            // 
            this.idleRPM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.idleRPM.Enabled = false;
            this.idleRPM.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.idleRPM.Location = new System.Drawing.Point(150, 3);
            this.idleRPM.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.idleRPM.MaxLength = 2000;
            this.idleRPM.Name = "idleRPM";
            this.idleRPM.Size = new System.Drawing.Size(55, 20);
            this.idleRPM.TabIndex = 23;
            this.idleRPM.Text = "1200";
            this.idleRPM.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // minRPM
            // 
            this.minRPM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.minRPM.Enabled = false;
            this.minRPM.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.minRPM.Location = new System.Drawing.Point(150, 7);
            this.minRPM.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.minRPM.Name = "minRPM";
            this.minRPM.Size = new System.Drawing.Size(55, 20);
            this.minRPM.TabIndex = 24;
            this.minRPM.Text = "1000";
            this.minRPM.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label9.Location = new System.Drawing.Point(213, 6);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(31, 13);
            this.label9.TabIndex = 25;
            this.label9.Text = "RPM";
            // 
            // maxRPM
            // 
            this.maxRPM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.maxRPM.Enabled = false;
            this.maxRPM.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.maxRPM.Location = new System.Drawing.Point(150, 33);
            this.maxRPM.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.maxRPM.Name = "maxRPM";
            this.maxRPM.Size = new System.Drawing.Size(55, 20);
            this.maxRPM.TabIndex = 26;
            this.maxRPM.Text = "4500";
            this.maxRPM.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label11.Location = new System.Drawing.Point(128, 36);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(13, 13);
            this.label11.TabIndex = 28;
            this.label11.Text = ">";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label12.Location = new System.Drawing.Point(213, 36);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(31, 13);
            this.label12.TabIndex = 29;
            this.label12.Text = "RPM";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label13.Location = new System.Drawing.Point(213, 10);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(31, 13);
            this.label13.TabIndex = 30;
            this.label13.Text = "RPM";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.label13);
            this.panel2.Controls.Add(this.maxRPM);
            this.panel2.Controls.Add(this.label12);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.minRPM);
            this.panel2.Location = new System.Drawing.Point(10, 248);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(249, 63);
            this.panel2.TabIndex = 31;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.idleRPM);
            this.panel3.Controls.Add(this.label9);
            this.panel3.Controls.Add(this.label7);
            this.panel3.Location = new System.Drawing.Point(10, 214);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(249, 28);
            this.panel3.TabIndex = 32;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label14.Location = new System.Drawing.Point(10, 196);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(96, 15);
            this.label14.TabIndex = 33;
            this.label14.Text = "RPM settings:";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(559, 261);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(287, 23);
            this.progressBar.TabIndex = 34;
            this.progressBar.Visible = false;
            // 
            // progressLabel
            // 
            this.progressLabel.AutoSize = true;
            this.progressLabel.BackColor = System.Drawing.SystemColors.Window;
            this.progressLabel.Location = new System.Drawing.Point(556, 242);
            this.progressLabel.MinimumSize = new System.Drawing.Size(170, 16);
            this.progressLabel.Name = "progressLabel";
            this.progressLabel.Size = new System.Drawing.Size(170, 16);
            this.progressLabel.TabIndex = 35;
            this.progressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.progressLabel.Visible = false;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label17.Location = new System.Drawing.Point(468, 42);
            this.label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(73, 15);
            this.label17.TabIndex = 37;
            this.label17.Text = "feedback: ";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label19.Location = new System.Drawing.Point(7, 9);
            this.label19.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(104, 15);
            this.label19.TabIndex = 38;
            this.label19.Text = "Work Directory:";
            // 
            // lblDirName
            // 
            this.lblDirName.AutoSize = true;
            this.lblDirName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblDirName.Location = new System.Drawing.Point(119, 11);
            this.lblDirName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblDirName.Name = "lblDirName";
            this.lblDirName.Size = new System.Drawing.Size(33, 13);
            this.lblDirName.TabIndex = 39;
            this.lblDirName.Text = "None";
            // 
            // lblLogsLocation
            // 
            this.lblLogsLocation.AutoSize = true;
            this.lblLogsLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblLogsLocation.Location = new System.Drawing.Point(12, 6);
            this.lblLogsLocation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLogsLocation.Name = "lblLogsLocation";
            this.lblLogsLocation.Size = new System.Drawing.Size(70, 13);
            this.lblLogsLocation.TabIndex = 41;
            this.lblLogsLocation.Text = "Logs location";
            // 
            // cmbExportFormat
            // 
            this.cmbExportFormat.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cmbExportFormat.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.cmbExportFormat.FormattingEnabled = true;
            this.cmbExportFormat.Items.AddRange(new object[] {
            "csv",
            "tsv",
            "wrl"});
            this.cmbExportFormat.Location = new System.Drawing.Point(266, 19);
            this.cmbExportFormat.Margin = new System.Windows.Forms.Padding(1);
            this.cmbExportFormat.Name = "cmbExportFormat";
            this.cmbExportFormat.Size = new System.Drawing.Size(62, 21);
            this.cmbExportFormat.TabIndex = 43;
            this.cmbExportFormat.Text = "csv";
            // 
            // lblExportFormat
            // 
            this.lblExportFormat.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblExportFormat.AutoSize = true;
            this.lblExportFormat.Location = new System.Drawing.Point(263, 4);
            this.lblExportFormat.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblExportFormat.Name = "lblExportFormat";
            this.lblExportFormat.Size = new System.Drawing.Size(49, 13);
            this.lblExportFormat.TabIndex = 44;
            this.lblExportFormat.Text = "Format:";
            // 
            // lblExportMode
            // 
            this.lblExportMode.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblExportMode.AutoSize = true;
            this.lblExportMode.Location = new System.Drawing.Point(0, 4);
            this.lblExportMode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblExportMode.Name = "lblExportMode";
            this.lblExportMode.Size = new System.Drawing.Size(42, 13);
            this.lblExportMode.TabIndex = 46;
            this.lblExportMode.Text = "Mode:";
            // 
            // cmbExportMode
            // 
            this.cmbExportMode.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.cmbExportMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.cmbExportMode.FormattingEnabled = true;
            this.cmbExportMode.Items.AddRange(new object[] {
            "File",
            "Directory"});
            this.cmbExportMode.Location = new System.Drawing.Point(3, 19);
            this.cmbExportMode.Margin = new System.Windows.Forms.Padding(1);
            this.cmbExportMode.Name = "cmbExportMode";
            this.cmbExportMode.Size = new System.Drawing.Size(95, 21);
            this.cmbExportMode.TabIndex = 45;
            this.cmbExportMode.Text = "File";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.lblExportMode);
            this.panel4.Controls.Add(this.cmbExportMode);
            this.panel4.Controls.Add(this.btnExport);
            this.panel4.Controls.Add(this.lblExportFormat);
            this.panel4.Controls.Add(this.cmbExportType);
            this.panel4.Controls.Add(this.cmbExportFormat);
            this.panel4.Controls.Add(this.lblExportType);
            this.panel4.Location = new System.Drawing.Point(8, 438);
            this.panel4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(456, 45);
            this.panel4.TabIndex = 32;
            // 
            // lblBinDelete
            // 
            this.lblBinDelete.AutoSize = true;
            this.lblBinDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblBinDelete.Location = new System.Drawing.Point(36, 60);
            this.lblBinDelete.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBinDelete.Name = "lblBinDelete";
            this.lblBinDelete.Size = new System.Drawing.Size(46, 13);
            this.lblBinDelete.TabIndex = 44;
            this.lblBinDelete.Text = "Bin Files";
            // 
            // lblATFileName
            // 
            this.lblATFileName.AutoSize = true;
            this.lblATFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblATFileName.Location = new System.Drawing.Point(16, 33);
            this.lblATFileName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblATFileName.Name = "lblATFileName";
            this.lblATFileName.Size = new System.Drawing.Size(66, 13);
            this.lblATFileName.TabIndex = 46;
            this.lblATFileName.Text = "AT Filename";
            // 
            // panel5
            // 
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Controls.Add(this.cmbTextFilter);
            this.panel5.Controls.Add(this.label15);
            this.panel5.Controls.Add(this.cmbBinDelete);
            this.panel5.Controls.Add(this.lblATFileName);
            this.panel5.Controls.Add(this.cmbATFileName);
            this.panel5.Controls.Add(this.lblLogsLocation);
            this.panel5.Controls.Add(this.lblBinDelete);
            this.panel5.Controls.Add(this.cmbLogsLocation);
            this.panel5.Location = new System.Drawing.Point(265, 214);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(201, 115);
            this.panel5.TabIndex = 32;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.Location = new System.Drawing.Point(266, 196);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 15);
            this.label2.TabIndex = 48;
            this.label2.Text = "File settings:";
            // 
            // panel6
            // 
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Controls.Add(this.AFRdivisor);
            this.panel6.Controls.Add(this.btnAFR);
            this.panel6.Location = new System.Drawing.Point(10, 329);
            this.panel6.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(144, 29);
            this.panel6.TabIndex = 32;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label10.Location = new System.Drawing.Point(10, 314);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(85, 15);
            this.label10.TabIndex = 49;
            this.label10.Text = "AFR Divisor:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label15.Location = new System.Drawing.Point(28, 87);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(54, 13);
            this.label15.TabIndex = 48;
            this.label15.Text = "CSV/TSV";
            // 
            // WoolichFileDecoderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1284, 586);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblCRCsize);
            this.Controls.Add(this.CRCsize);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblDirName);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.btnRepair);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblExportFilename);
            this.Controls.Add(this.txtFeedback);
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.btnOpenFile);
            this.Controls.Add(this.txtLogging);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.aTFCheckedListBox);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel6);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximumSize = new System.Drawing.Size(1300, 625);
            this.MinimumSize = new System.Drawing.Size(1300, 625);
            this.Name = "WoolichFileDecoderForm";
            this.Text = "Woolich File Decoder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.App_Close);
            this.Load += new System.EventHandler(this.App_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtLogging;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.OpenFileDialog openWRLFileDialog;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.TextBox txtFeedback;
        private System.Windows.Forms.Label lblExportPacketsCount;
        private System.Windows.Forms.Label lblExportFilename;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.ComboBox cmbExportType;
        private System.Windows.Forms.Label lblExportType;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblAnalysisCol;
        private System.Windows.Forms.TextBox txtBreakOnChange;
        private System.Windows.Forms.Button btnAnalyse;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckedListBox aTFCheckedListBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox idleRPM;
        private System.Windows.Forms.TextBox minRPM;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox maxRPM;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label progressLabel;
        private System.Windows.Forms.Label lblPackets;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox CRCsize;
        private System.Windows.Forms.Label lblCRCsize;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label lblDirName;
        private System.Windows.Forms.Button btnRepair;
        private System.Windows.Forms.Label lblLogsLocation;
        private System.Windows.Forms.ComboBox cmbLogsLocation;
        private System.Windows.Forms.ComboBox cmbExportFormat;
        private System.Windows.Forms.Label lblExportFormat;
        private System.Windows.Forms.Label lblExportMode;
        private System.Windows.Forms.ComboBox cmbExportMode;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbBinDelete;
        private System.Windows.Forms.Label lblBinDelete;
        private System.Windows.Forms.ComboBox cmbATFileName;
        private System.Windows.Forms.Label lblATFileName;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblTotalPackets;
        private System.Windows.Forms.Label lblTotalPacketsCount;
        private System.Windows.Forms.Button btnAFR;
        private System.Windows.Forms.TextBox AFRdivisor;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox cmbTextFilter;
    }
}

