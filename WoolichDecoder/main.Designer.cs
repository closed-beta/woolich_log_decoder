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
            this.btnExportCSV = new System.Windows.Forms.Button();
            this.cmbExportType = new System.Windows.Forms.ComboBox();
            this.btnAutoTuneExport = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnMulti = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.CRCsize = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.btnExportCRCHack = new System.Windows.Forms.Button();
            this.btnExportTargetColumn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtBreakOnChange = new System.Windows.Forms.TextBox();
            this.btnAnalyse = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.aTFCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnRepair = new System.Windows.Forms.Button();
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
            this.cmbLogsLocation = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
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
            this.btnOpenFile.Size = new System.Drawing.Size(112, 28);
            this.btnOpenFile.TabIndex = 2;
            this.btnOpenFile.Text = "Open File";
            this.toolTip1.SetToolTip(this.btnOpenFile, "Open WRL File");
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
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
            this.lblExportPacketsCount.BackColor = System.Drawing.SystemColors.Window;
            this.lblExportPacketsCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblExportPacketsCount.Enabled = false;
            this.lblExportPacketsCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblExportPacketsCount.Location = new System.Drawing.Point(360, 43);
            this.lblExportPacketsCount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblExportPacketsCount.Name = "lblExportPacketsCount";
            this.lblExportPacketsCount.Size = new System.Drawing.Size(78, 20);
            this.lblExportPacketsCount.TabIndex = 9;
            this.lblExportPacketsCount.Text = "0";
            this.lblExportPacketsCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            // btnExportCSV
            // 
            this.btnExportCSV.Enabled = false;
            this.btnExportCSV.Location = new System.Drawing.Point(351, 426);
            this.btnExportCSV.Margin = new System.Windows.Forms.Padding(2);
            this.btnExportCSV.Name = "btnExportCSV";
            this.btnExportCSV.Size = new System.Drawing.Size(114, 23);
            this.btnExportCSV.TabIndex = 11;
            this.btnExportCSV.Text = "Export CSV";
            this.toolTip1.SetToolTip(this.btnExportCSV, "Export and save file in CSV format");
            this.btnExportCSV.UseVisualStyleBackColor = true;
            this.btnExportCSV.Click += new System.EventHandler(this.btnExportCSV_Click);
            // 
            // cmbExportType
            // 
            this.cmbExportType.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cmbExportType.Enabled = false;
            this.cmbExportType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.cmbExportType.FormattingEnabled = true;
            this.cmbExportType.Items.AddRange(new object[] {
            "Export Full File",
            "Export Analysis Only"});
            this.cmbExportType.Location = new System.Drawing.Point(179, 428);
            this.cmbExportType.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cmbExportType.Name = "cmbExportType";
            this.cmbExportType.Size = new System.Drawing.Size(166, 21);
            this.cmbExportType.TabIndex = 12;
            // 
            // btnAutoTuneExport
            // 
            this.btnAutoTuneExport.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.btnAutoTuneExport.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAutoTuneExport.Enabled = false;
            this.btnAutoTuneExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAutoTuneExport.Location = new System.Drawing.Point(351, 108);
            this.btnAutoTuneExport.Margin = new System.Windows.Forms.Padding(2);
            this.btnAutoTuneExport.Name = "btnAutoTuneExport";
            this.btnAutoTuneExport.Size = new System.Drawing.Size(115, 41);
            this.btnAutoTuneExport.TabIndex = 13;
            this.btnAutoTuneExport.Text = "Save";
            this.toolTip1.SetToolTip(this.btnAutoTuneExport, "Save Filtered Data to WRL File");
            this.btnAutoTuneExport.UseVisualStyleBackColor = false;
            this.btnAutoTuneExport.Click += new System.EventHandler(this.btnAutoTuneExport_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(176, 412);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Export Type:";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnMulti);
            this.panel1.Controls.Add(this.label18);
            this.panel1.Controls.Add(this.CRCsize);
            this.panel1.Controls.Add(this.label16);
            this.panel1.Controls.Add(this.btnExportCRCHack);
            this.panel1.Controls.Add(this.btnExportTargetColumn);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtBreakOnChange);
            this.panel1.Controls.Add(this.btnAnalyse);
            this.panel1.Controls.Add(this.lblExportPacketsCount);
            this.panel1.Location = new System.Drawing.Point(10, 469);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(455, 105);
            this.panel1.TabIndex = 16;
            // 
            // btnMulti
            // 
            this.btnMulti.Enabled = false;
            this.btnMulti.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnMulti.Location = new System.Drawing.Point(254, 7);
            this.btnMulti.Margin = new System.Windows.Forms.Padding(2);
            this.btnMulti.Name = "btnMulti";
            this.btnMulti.Size = new System.Drawing.Size(27, 28);
            this.btnMulti.TabIndex = 39;
            this.btnMulti.Text = "M";
            this.toolTip1.SetToolTip(this.btnMulti, "Find Max values in column for all WRL in folder");
            this.btnMulti.UseVisualStyleBackColor = true;
            this.btnMulti.Click += new System.EventHandler(this.btnMultiAnalyse_Click);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label18.Location = new System.Drawing.Point(287, 76);
            this.label18.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(60, 15);
            this.label18.TabIndex = 38;
            this.label18.Text = "CRC size:";
            // 
            // CRCsize
            // 
            this.CRCsize.Enabled = false;
            this.CRCsize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.CRCsize.Location = new System.Drawing.Point(390, 75);
            this.CRCsize.Margin = new System.Windows.Forms.Padding(2);
            this.CRCsize.Name = "CRCsize";
            this.CRCsize.Size = new System.Drawing.Size(48, 20);
            this.CRCsize.TabIndex = 37;
            this.CRCsize.Text = "100";
            this.CRCsize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label16.Location = new System.Drawing.Point(287, 45);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(53, 15);
            this.label16.TabIndex = 36;
            this.label16.Text = "Packets:";
            // 
            // btnExportCRCHack
            // 
            this.btnExportCRCHack.Enabled = false;
            this.btnExportCRCHack.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnExportCRCHack.Location = new System.Drawing.Point(13, 71);
            this.btnExportCRCHack.Margin = new System.Windows.Forms.Padding(2);
            this.btnExportCRCHack.Name = "btnExportCRCHack";
            this.btnExportCRCHack.Size = new System.Drawing.Size(237, 26);
            this.btnExportCRCHack.TabIndex = 19;
            this.btnExportCRCHack.Text = "Export CRC to WRL";
            this.btnExportCRCHack.UseVisualStyleBackColor = true;
            this.btnExportCRCHack.Click += new System.EventHandler(this.btnExportCRCHack_Click);
            // 
            // btnExportTargetColumn
            // 
            this.btnExportTargetColumn.Enabled = false;
            this.btnExportTargetColumn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnExportTargetColumn.Location = new System.Drawing.Point(13, 39);
            this.btnExportTargetColumn.Margin = new System.Windows.Forms.Padding(2);
            this.btnExportTargetColumn.Name = "btnExportTargetColumn";
            this.btnExportTargetColumn.Size = new System.Drawing.Size(237, 28);
            this.btnExportTargetColumn.TabIndex = 18;
            this.btnExportTargetColumn.Text = "Export Analysis to WRL";
            this.btnExportTargetColumn.UseVisualStyleBackColor = true;
            this.btnExportTargetColumn.Click += new System.EventHandler(this.btnExportTargetColumn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(285, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Analysis Column:";
            // 
            // txtBreakOnChange
            // 
            this.txtBreakOnChange.Enabled = false;
            this.txtBreakOnChange.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.txtBreakOnChange.Location = new System.Drawing.Point(390, 12);
            this.txtBreakOnChange.Margin = new System.Windows.Forms.Padding(2);
            this.txtBreakOnChange.Name = "txtBreakOnChange";
            this.txtBreakOnChange.Size = new System.Drawing.Size(48, 20);
            this.txtBreakOnChange.TabIndex = 16;
            this.txtBreakOnChange.Text = "35";
            this.txtBreakOnChange.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtBreakOnChange.TextChanged += new System.EventHandler(this.TxtBreakOnChange_TextChanged);
            // 
            // btnAnalyse
            // 
            this.btnAnalyse.Enabled = false;
            this.btnAnalyse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnAnalyse.Location = new System.Drawing.Point(13, 7);
            this.btnAnalyse.Margin = new System.Windows.Forms.Padding(2);
            this.btnAnalyse.Name = "btnAnalyse";
            this.btnAnalyse.Size = new System.Drawing.Size(237, 28);
            this.btnAnalyse.TabIndex = 15;
            this.btnAnalyse.Text = "Analyse";
            this.btnAnalyse.UseVisualStyleBackColor = true;
            this.btnAnalyse.Click += new System.EventHandler(this.btnAnalyse_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label3.Location = new System.Drawing.Point(9, 453);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(390, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "Log File Analysis Functions (To analyse a particular packet column)";
            // 
            // aTFCheckedListBox
            // 
            this.aTFCheckedListBox.Enabled = false;
            this.aTFCheckedListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.aTFCheckedListBox.FormattingEnabled = true;
            this.aTFCheckedListBox.Location = new System.Drawing.Point(10, 108);
            this.aTFCheckedListBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.aTFCheckedListBox.Name = "aTFCheckedListBox";
            this.aTFCheckedListBox.Size = new System.Drawing.Size(335, 79);
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
            this.label5.Size = new System.Drawing.Size(39, 15);
            this.label5.TabIndex = 19;
            this.label5.Text = "Log: ";
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
            // btnRepair
            // 
            this.btnRepair.Location = new System.Drawing.Point(126, 53);
            this.btnRepair.Margin = new System.Windows.Forms.Padding(2);
            this.btnRepair.Name = "btnRepair";
            this.btnRepair.Size = new System.Drawing.Size(112, 28);
            this.btnRepair.TabIndex = 40;
            this.btnRepair.Text = "Repair";
            this.toolTip1.SetToolTip(this.btnRepair, "Repair corrupted WRL ");
            this.btnRepair.UseVisualStyleBackColor = true;
            this.btnRepair.Click += new System.EventHandler(this.btnRepairWRLFile_Click);
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
            this.label14.Size = new System.Drawing.Size(129, 15);
            this.label14.TabIndex = 33;
            this.label14.Text = "Additional settings:";
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
            this.label17.Size = new System.Drawing.Size(77, 15);
            this.label17.TabIndex = 37;
            this.label17.Text = "Feedback: ";
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
            this.lblLogsLocation.Location = new System.Drawing.Point(9, 412);
            this.lblLogsLocation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLogsLocation.Name = "lblLogsLocation";
            this.lblLogsLocation.Size = new System.Drawing.Size(87, 13);
            this.lblLogsLocation.TabIndex = 41;
            this.lblLogsLocation.Text = "Logs location:";
            // 
            // cmbLogsLocation
            // 
            this.cmbLogsLocation.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cmbLogsLocation.Enabled = false;
            this.cmbLogsLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.cmbLogsLocation.FormattingEnabled = true;
            this.cmbLogsLocation.Items.AddRange(new object[] {
            "Current",
            "Work Directory"});
            this.cmbLogsLocation.Location = new System.Drawing.Point(10, 428);
            this.cmbLogsLocation.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cmbLogsLocation.Name = "cmbLogsLocation";
            this.cmbLogsLocation.Size = new System.Drawing.Size(166, 21);
            this.cmbLogsLocation.TabIndex = 42;
            // 
            // WoolichFileDecoderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1284, 586);
            this.Controls.Add(this.cmbLogsLocation);
            this.Controls.Add(this.lblLogsLocation);
            this.Controls.Add(this.btnRepair);
            this.Controls.Add(this.lblDirName);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.progressLabel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnAutoTuneExport);
            this.Controls.Add(this.cmbExportType);
            this.Controls.Add(this.btnExportCSV);
            this.Controls.Add(this.lblExportFilename);
            this.Controls.Add(this.txtFeedback);
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.btnOpenFile);
            this.Controls.Add(this.txtLogging);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.aTFCheckedListBox);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximumSize = new System.Drawing.Size(1300, 625);
            this.MinimumSize = new System.Drawing.Size(1300, 625);
            this.Name = "WoolichFileDecoderForm";
            this.Text = "Woolich File Decoder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WoolichFileDecoderForm_FormClosing);
            this.Load += new System.EventHandler(this.WoolichFileDecoder_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
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
        private System.Windows.Forms.Button btnExportCSV;
        private System.Windows.Forms.ComboBox cmbExportType;
        private System.Windows.Forms.Button btnAutoTuneExport;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnExportCRCHack;
        private System.Windows.Forms.Button btnExportTargetColumn;
        private System.Windows.Forms.Label label1;
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
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox CRCsize;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label lblDirName;
        private System.Windows.Forms.Button btnRepair;
        private System.Windows.Forms.Button btnMulti;
        private System.Windows.Forms.Label lblLogsLocation;
        private System.Windows.Forms.ComboBox cmbLogsLocation;
    }
}

