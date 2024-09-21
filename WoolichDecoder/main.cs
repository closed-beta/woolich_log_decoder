using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WoolichDecoder.Models;
using WoolichDecoder.Settings;

namespace WoolichDecoder
{
    public partial class WoolichFileDecoderForm : Form
    {
        public static class LogPrefix
        {
            private static readonly string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            public static string Prefix => $"{DateTime.Now.ToString(DateTimeFormat)} -- ";
        }

        string OpenFileName = string.Empty;

        WoolichMT09Log logs = new WoolichMT09Log();

        WoolichMT09Log exportLogs = new WoolichMT09Log();

        UserSettings userSettings;

        string outputFileNameWithoutExtension = string.Empty;
        string outputFileNameWithExtension = string.Empty;

        string logFolder = string.Empty;


        string[] autoTuneFilterOptions =
        {
            "ETV correction for MT-09",
            "Filter Out Gear 2",
            "Filter Out Idle RPM",
            "Filter Out Engine Braking in Gears 1-3",
            "Filter Out RPM in Gear 1"
        };

        // hours: 5
        // min: 6
        // seconds: 7
        // miliseconds: 8, 9 raw
        // rpm: 10, 11 raw
        // iap: 21 (x2.0156)
        // atm pressure: 23 (x2.0156)
        // battery: 41 (x13.65)
        // engine temp: 26 (-30)
        // inlet temp: 27 (-30)
        // injector duration: 28 * 0.5 (aka half)
        // ignition btdc?: 29 (-30)

        List<int> decodedColumns = new List<int>();

        // The first columns of each packet. 0, 1, 2 => 00, 01, 02 The 3 is the number of data items (93 or 253 so far) and 4 varies but is unknown
        List<int> knownPacketDefinitionColumns = new List<int> { 0, 1, 2, 3, 4 };

        // Used by the analysis.
        List<StaticPacketColumn> presumedStaticColumns = new List<StaticPacketColumn> { };

        List<int> analysisColumn = new List<int> { };

        public WoolichFileDecoderForm()
        {
            InitializeComponent();
            cmbExportType.SelectedIndex = 0;
            cmbExportFormat.SelectedIndex = 0;
            cmbLogsLocation.SelectedIndex = 0;
            cmbBinDelete.SelectedIndex = 0;
            cmbExportMode.SelectedIndex = 0;
            cmbExportType.SelectedIndexChanged += cmbExportType_Change;
            cmbExportMode.SelectedIndexChanged += cmbExportMode_Change;
        }
        private bool IsFileLoaded()
        {
            if (string.IsNullOrEmpty(OpenFileName))
            {
                MessageBox.Show("Please open a file before proceeding.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            return true;
        }
        private void cmbExportType_Change(object sender, EventArgs e)
        {
            // Update cmbExportFormat options based on the selected value in cmbExportType
            UpdateCmbExport();
        }
        private void cmbExportMode_Change(object sender, EventArgs e)
        {
            UpdateCmbExport();
        }
        public void SetMT09_StaticColumns()
        {
            decodedColumns = new List<int> {
                5, 6, 7, 8, 9, // Time
                10, 11, // RPM
                12, 13, // true TPS 
                // 14, unknown
                // 15, woolich tps
                // 16, 17 unknown 
                // 18 etv
                // 19 ?
                // 20 ?
                21, // iap pressure
                23, // atm pressures
                41, // battery 
                31, 32, 33, 34, 35, 36, // speeds
                // 37, 38, // combined but havent worked out what or how yet. Goes high even when just idling.
                26, 27, // temperatures
                28, // injector duration
                29, // ignition 
            };

            presumedStaticColumns.Clear();

            int[] columns = new int[] { 20, 22, 39, 40, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58,
                            59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78,
                            79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94 };

            foreach (int column in columns)
            {
                presumedStaticColumns.Add(new StaticPacketColumn { Column = column, StaticValue = 0, File = string.Empty });
            }

        }
        public void SetS1000RR_StaticColumns()
        {

            decodedColumns = new List<int>();

            presumedStaticColumns.Clear();

            int[] columns = new int[] { 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169,
                            170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189,
                            190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209,
                            210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229,
                            230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249,
                            250 };

            foreach (int column in columns)
            {
                presumedStaticColumns.Add(new StaticPacketColumn { Column = column, StaticValue = 0, File = string.Empty });
            }

        }
        private void TxtBreakOnChange_TextChanged(object sender, EventArgs e)
        {
            exportLogs.ClearPackets();
            lblExportPacketsCount.Text = "0";
        }
        private void WoolichFileDecoder_Load(object sender, EventArgs e)
        {
            userSettings = new UserSettings();


            string logFileLocation = userSettings.LogDirectory;
            //Since there is no default value for FormText.
            if (logFileLocation != null)
            {
                this.logFolder = logFileLocation;
            }
            else
            {
                this.logFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            this.aTFCheckedListBox.Items.AddRange(autoTuneFilterOptions.ToArray());


            for (int i = 0; i < this.aTFCheckedListBox.Items.Count; i++)
            {
                aTFCheckedListBox.SetItemCheckState(i, CheckState.Checked);
            }

        }
        private void WoolichFileDecoder_Close(object sender, FormClosingEventArgs e)
        {
            userSettings.LogDirectory = this.logFolder;

            // save the user settings.
            userSettings.Save();
        }
        private List<int> FindPrefixes(byte[] data, byte[] prefix)
        {
            List<int> offsets = new List<int>();
            int prefixLength = prefix.Length;
            int interval = 96;
            int offset = 0;

            while (offset <= (data.Length - prefixLength))
            {
                bool match = true;
                for (int i = 0; i < prefixLength; i++)
                {
                    if (data[offset + i] != prefix[i])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    offsets.Add(offset);
                    offset += interval;
                }
                else
                {
                    offset += 1;
                }
            }

            return offsets;
        }
        private byte[] ProcessPackets(byte[] data, List<int> offsets, int prefixLength, int interval, out bool needsRepair)
        {
            needsRepair = false;

            using (MemoryStream recoveredDataStream = new MemoryStream())
            {
                int headerLength = 353;
                byte[] header = new byte[headerLength];
                Array.Copy(data, 0, header, 0, headerLength);

                recoveredDataStream.Write(header, 0, header.Length);

                int previousOffset = -1;

                foreach (int currentOffset in offsets)
                {
                    if (previousOffset != -1)
                    {
                        int distance = currentOffset - previousOffset;
                        if (distance != interval)
                        {
                            log($"{LogPrefix.Prefix}Gap of {distance} bytes, offsets {previousOffset} - {currentOffset}. Fixed");
                            needsRepair = true;
                            previousOffset = currentOffset;
                        }
                    }

                    if (currentOffset >= headerLength)
                    {
                        int packetLength = Math.Min(interval, data.Length - currentOffset);
                        byte[] packetData = new byte[packetLength];
                        Array.Copy(data, currentOffset, packetData, 0, packetLength);
                        if (packetLength == interval)
                        {
                            recoveredDataStream.Write(packetData, 0, packetData.Length);
                            previousOffset = currentOffset;
                        }
                    }
                }

                return recoveredDataStream.ToArray();
            }
        }
        private long FindPatternInFile(string filePath, byte[] pattern)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);

                for (long i = 0; i < buffer.Length - pattern.Length; i++)
                {
                    bool match = true;
                    for (int j = 0; j < pattern.Length; j++)
                    {
                        if (buffer[i + j] != pattern[j])
                        {
                            match = false;
                            break;
                        }
                    }

                    if (match)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        private void ClearBoxAndPackets()
        {
            txtBreakOnChange.Text = string.Empty;

            exportLogs.ClearPackets();

            lblExportPacketsCount.Text = "0";

            lblExportFilename.Text = string.Empty;

        }
        private void DisplayLegend_Yamaha()
        {
            StringBuilder legend = new StringBuilder();
            legend.AppendLine("Available column numbers:");
            legend.AppendLine("10: RPM");
            legend.AppendLine("12: True TPS");
            legend.AppendLine("15: Woolich TPS");
            legend.AppendLine("18: Correct ETV");
            legend.AppendLine("21: IAP");
            legend.AppendLine("23: ATM Pressure");
            legend.AppendLine("24: Gear");
            legend.AppendLine("26: Engine Temperature");
            legend.AppendLine("27: Inlet Temperature");
            legend.AppendLine("28: Injector Duration");
            legend.AppendLine("29: Ignition Offset");
            legend.AppendLine("31: Speedo");
            legend.AppendLine("33: Front Wheel Speed");
            legend.AppendLine("35: Rear Wheel Speed");
            legend.AppendLine("41: Battery Voltage");
            legend.AppendLine("42: AFR");

            feedback(legend.ToString());
        }
        private string GetDirectoryPath()
        {

            if (cmbLogsLocation.SelectedIndex == 0)
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
            else if (cmbLogsLocation.SelectedIndex == 1)
            {
                return lblDirName.Text;
            }
            else
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }
        private void feedback(string fbData)
        {
            string directoryPath = GetDirectoryPath();

            if (string.IsNullOrEmpty(directoryPath))
            {
                return;
            }

            if (!Directory.Exists(directoryPath))
            {
                try
                {
                    Directory.CreateDirectory(directoryPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while creating the directory: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            txtFeedback.AppendText(fbData + Environment.NewLine);

            string feedbackFilePath = Path.Combine(directoryPath, "feedback.txt");

            try
            {
                File.AppendAllText(feedbackFilePath, $"{fbData}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving the feedback: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void log(string logData)
        {
            string directoryPath = GetDirectoryPath();

            if (string.IsNullOrEmpty(directoryPath))
            {
                return;
            }

            if (!Directory.Exists(directoryPath))
            {
                try
                {
                    Directory.CreateDirectory(directoryPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while creating the directory: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            txtLogging.AppendText(logData + Environment.NewLine);

            string logFilePath = Path.Combine(directoryPath, "log.txt");

            try
            {
                File.AppendAllText(logFilePath, $"{logData}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving the log: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void UpdateProgressLabel(string text)
        {
            if (progressLabel.InvokeRequired)
            {
                progressLabel.Invoke(new Action(() => progressLabel.Text = text));
            }
            else
            {
                progressLabel.Text = text;
            }
        }
        private void ExportCRCHack()
        {
            if (!IsFileLoaded())
                return;

            try
            {
                if (!int.TryParse(CRCsize.Text.Trim(), out int size))
                {
                    MessageBox.Show(
                        "Please enter a valid number.",
                        "Invalid size",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }

                string baseFileName = Path.GetFileNameWithoutExtension(lblFileName.Text.Trim());
                string directoryPath = lblDirName.Text.Trim();
                outputFileNameWithExtension = Path.Combine(directoryPath, $"{baseFileName}_CRC.{size}.WRL");

                WoolichMT09Log exportItem = logs;

                using (BinaryWriter binWriter = new BinaryWriter(File.Open(outputFileNameWithExtension, FileMode.Create)))
                {
                    binWriter.Write(exportItem.PrimaryHeaderData);
                    binWriter.Write(exportItem.SecondaryHeaderData);

                    var packets = exportItem.GetPackets().Take(size);

                    foreach (var packet in packets)
                    {
                        binWriter.Write(packet.Value);
                    }
                }

                log($"{LogPrefix.Prefix}CRC saved as: " + Path.GetFileName(outputFileNameWithExtension));
            }
            catch (Exception ex)
            {
                log($"Error while exporting CRC: {ex.Message}");
            }
        }
        private void ExportTargetColumn()
        {
            if (!IsFileLoaded())
                return;

            WoolichMT09Log exportItem = null;

            if (cmbExportType.SelectedIndex == 0)
            {
                exportItem = logs;

                MessageBox.Show(
                    "Export Analysis Only is supported at the moment.",
                    "Export Not Supported",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }
            else
            {
                exportItem = exportLogs;
            }

            if (string.IsNullOrWhiteSpace(txtBreakOnChange.Text))
            {
                MessageBox.Show(
                    "Please enter a column number for export analysis.",
                    "Column Number Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            if (exportItem == null || !exportItem.GetPackets().Any())
            {
                MessageBox.Show(
                    "No packets available for export.",
                    "No Data Available",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            string outputFileNameWithExtension = "";

            try
            {
                string baseFileName = Path.GetFileNameWithoutExtension(lblFileName.Text.Trim());
                string directoryPath = lblDirName.Text.Trim();
                var columnToExport = int.Parse(txtBreakOnChange.Text.Trim());
                outputFileNameWithExtension = Path.Combine(directoryPath, $"{baseFileName}_C{columnToExport}.WRL");
                using (BinaryWriter binWriter = new BinaryWriter(File.Open(outputFileNameWithExtension, FileMode.Create)))
                {
                    binWriter.Write(exportItem.PrimaryHeaderData);
                    binWriter.Write(exportItem.SecondaryHeaderData);
                    foreach (var packet in exportItem.GetPackets())
                    {
                        binWriter.Write(packet.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during file export: {ex.Message}");
            }

            log($"{LogPrefix.Prefix}Analysis WRL File saved as: " + Path.GetFileName(outputFileNameWithExtension));
        }
        private async void ExportToText()
        {
            if (!IsFileLoaded())
            {
                return;
            }

            WoolichMT09Log exportItem = null;

            string directoryPath = lblDirName.Text.Trim();

            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                MessageBox.Show("Directory path is not defined.");
                return;
            }

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(lblFileName.Text.Trim());

            if (string.IsNullOrWhiteSpace(fileNameWithoutExtension))
            {
                MessageBox.Show("File name is not defined.");
                return;
            }

            string exportFormat = cmbExportFormat.SelectedItem.ToString().ToLower();
            string fileExtension = exportFormat == "tsv" ? ".tsv" : ".csv";
            char delimiter = exportFormat == "tsv" ? '\t' : ',';

            var exportFileName = Path.Combine(directoryPath, fileNameWithoutExtension + fileExtension);

            if (cmbExportType.SelectedIndex == 0)
            {
                exportItem = logs;
            }
            else
            {
                exportItem = exportLogs;

                if (!string.IsNullOrWhiteSpace(txtBreakOnChange.Text))
                {
                    try
                    {
                        int columnNumber = int.Parse(txtBreakOnChange.Text.Trim());
                        exportFileName = Path.Combine(directoryPath, fileNameWithoutExtension + $"_C{columnNumber}" + fileExtension);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(
                            "Please provide a valid column number for analysis",
                            "Invalid column number.",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                        return;
                    }
                }
                else
                {
                    MessageBox.Show(
                            "Please provide a column number for analysis",
                            "No Column Number.",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
);
                    return;
                }
            }

            if (exportItem.GetPacketCount() == 0)
            {
                MessageBox.Show(
                    "No packets available for export.",
                    "No Data Available",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (!Directory.Exists(directoryPath))
            {
                try
                {
                    Directory.CreateDirectory(directoryPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to create directory: {directoryPath}. Error: {ex.Message}");
                    return;
                }
            }

            progressBar.Visible = true;
            progressLabel.Visible = true;
            UpdateProgressLabel("Starting export...");

            await Task.Run(() =>
            {
                int packetCount = exportItem.GetPacketCount();
                int count = 0;
                List<int> combinedCols = new List<int>();

                // Set columns based on packet format
                if (exportItem.PacketFormat == 0x01)
                {
                    SetMT09_StaticColumns();
                }
                else if (exportItem.PacketFormat == 0x10)
                {
                    SetS1000RR_StaticColumns();
                }
                else
                {
                    this.presumedStaticColumns.Clear();
                }

                try
                {
                    using (StreamWriter outputFile = new StreamWriter(exportFileName))
                    {
                        string exportHeader = exportItem.GetHeader(this.presumedStaticColumns, combinedCols);
                        outputFile.WriteLine(string.Join(delimiter.ToString(), exportHeader.Split(','))); // Use the correct delimiter

                        var packets = exportItem.GetPackets();
                        int totalPackets = packets.Count;
                        int processedPackets = 0;

                        foreach (var packet in packets)
                        {
                            var exportLine = WoolichMT09Log.getCSV(packet.Value, packet.Key, exportItem.PacketFormat, this.presumedStaticColumns, combinedCols);
                            outputFile.WriteLine(string.Join(delimiter.ToString(), exportLine.Split(','))); // Use the correct delimiter
                            outputFile.Flush();

                            processedPackets++;
                            int progressPercentage = (processedPackets * 100) / totalPackets;
                            Invoke(new Action(() => progressBar.Value = Math.Min(progressPercentage, progressBar.Maximum)));
                            Invoke(new Action(() => UpdateProgressLabel($"Exporting... {progressPercentage}% completed")));

                            count++;

                            // Limit output for non-MT09 formats if count exceeds 100,000
                            if (count > 100000 && exportItem.PacketFormat != 0x01)
                            {
                                break;
                            }
                        }
                    }
                    Invoke(new Action(() => log($"{LogPrefix.Prefix}Data exported to {exportFormat.ToUpper()} format: " + Path.GetFileName(exportFileName))));
                    Invoke(new Action(() => UpdateProgressLabel("Export completed successfully.")));
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() => MessageBox.Show($"An error occurred: {ex.Message}")));
                    Invoke(new Action(() => UpdateProgressLabel("Error occurred during export.")));
                }
                finally
                {
                    Invoke(new Action(() => UpdateProgressLabel("Export finished.")));
                    System.Threading.Thread.Sleep(3000);
                    Invoke(new Action(() => progressBar.Visible = false));
                    Invoke(new Action(() => progressLabel.Visible = false));
                }
            });
        }
        private void UpdateFormsStates()
        {
            bool isFileLoaded = IsFileLoaded();
            btnAnalyse.Enabled = isFileLoaded;
            aTFCheckedListBox.Enabled = isFileLoaded;
            idleRPM.Enabled = isFileLoaded;
            minRPM.Enabled = isFileLoaded;
            maxRPM.Enabled = isFileLoaded;
            lblExportPacketsCount.Enabled = isFileLoaded;
            txtBreakOnChange.Enabled = isFileLoaded;
            txtFeedback.Enabled = isFileLoaded;
            txtLogging.Enabled = isFileLoaded;
            cmbLogsLocation.Enabled = isFileLoaded;
        }
        private void btnAnalyse_Click(object sender, EventArgs e)
        {
            int selectedIndex = cmbExportMode.SelectedIndex;

            if (selectedIndex == 0)
            {
                if (!IsFileLoaded())
                    return;
                Analyse();
            }
            else if (selectedIndex == 1)
            {
                MultiAnalyse();
            }
        }
        private async void Analyse()
        {
            if (string.IsNullOrWhiteSpace(txtBreakOnChange.Text))
            {
                txtFeedback.Clear();
                MessageBox.Show(
                    "Please enter a column number for export analysis.",
                    "Column Number Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                DisplayLegend_Yamaha();
                return;
            }

            int columnNumber;
            try
            {
                columnNumber = int.Parse(txtBreakOnChange.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(
                     "Please enter a valid number for export analysis.",
                     "Column Number Required",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Information
                );

                txtFeedback.Clear();
                ClearBoxAndPackets();
                DisplayLegend_Yamaha();
                return;
            }

            // Mapping of column numbers to corresponding analysis functions
            var columnFunctions = new Dictionary<int, Func<byte[], double>>()
        {
            { 10, packet => WoolichConversions.getRPM(packet) },    // RPM
            { 12, packet => WoolichConversions.getTrueTPS(packet) }, // True TPS
            { 15, packet => WoolichConversions.getWoolichTPS(packet) }, // Woolich TPS
            { 18, packet => WoolichConversions.getCorrectETV(packet) }, // Correct ETV
            { 21, packet => WoolichConversions.getIAP(packet) }, // IAP
            { 23, packet => WoolichConversions.getATMPressure(packet) }, // ATM Pressure
            { 24, packet => WoolichConversions.getGear(packet) }, // Gear
            { 26, packet => WoolichConversions.getEngineTemperature(packet) }, // Engine Temperature
            { 27, packet => WoolichConversions.getInletTemperature(packet) }, // Inlet Temperature
            { 28, packet => WoolichConversions.getInjectorDuration(packet) }, // Injector Duration
            { 29, packet => WoolichConversions.getIgnitionOffset(packet) }, // Ignition Offset
            { 31, packet => WoolichConversions.getSpeedo(packet) }, // Speedo
            { 33, packet => WoolichConversions.getFrontWheelSpeed(packet) }, // Front Wheel Speed
            { 35, packet => WoolichConversions.getRearWheelSpeed(packet) }, // Rear Wheel Speed
            { 41, packet => WoolichConversions.getBatteryVoltage(packet) }, // Battery Voltage
            { 42, packet => WoolichConversions.getAFR(packet) } // AFR
        };

            // Check if the column number is supported in the analysis
            if (!columnFunctions.ContainsKey(columnNumber))
            {
                // If not supported, display a message, log it, and clear data
                MessageBox.Show(
                    "Please enter a valid column number for export analysis.",
                    "Unsupported Column Number",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                txtFeedback.Clear();
                ClearBoxAndPackets();
                DisplayLegend_Yamaha();
                return;
            }

            feedback($"{LogPrefix.Prefix}Monitoring changes on column: {columnNumber}");
            analysisColumn.Add(columnNumber);

            feedback($"{LogPrefix.Prefix}Analysing column {columnNumber}...");
            lblExportPacketsCount.Text = string.Empty;
            exportLogs.ClearPackets();
            DateTime startTime = DateTime.Now;
            log($"{LogPrefix.Prefix}Start Analysing...");

            StringBuilder feedbackBuffer = new StringBuilder();
            Func<byte[], double> conversionFunction = columnFunctions[columnNumber];

            double? previousValue = null;
            double? minValue = null;
            double? maxValue = null;

            var packets = logs.GetPackets();
            int totalPackets = packets.Count;
            int processedPackets = 0;

            progressBar.Visible = true;
            progressLabel.Visible = true;
            progressBar.Value = 0;
            UpdateProgressLabel("Starting analysis...");

            await Task.Run(() =>
            {
                foreach (KeyValuePair<string, byte[]> packet in packets)
                {
                    try
                    {
                        double currentValue = conversionFunction(packet.Value);

                        if (previousValue == null)
                        {
                            minValue = currentValue;
                            maxValue = currentValue;
                            previousValue = currentValue;
                            feedbackBuffer.AppendLine($"Initial value in column {columnNumber}: {currentValue}");
                            continue;
                        }

                        if (currentValue > maxValue) maxValue = currentValue;
                        if (currentValue < minValue) minValue = currentValue;

                        if (currentValue != previousValue)
                        {
                            feedbackBuffer.AppendLine($"Column {columnNumber} changed from {previousValue} to {currentValue}");
                            previousValue = currentValue;

                            if (analysisColumn.Contains(columnNumber))
                            {
                                exportLogs.AddPacket(packet.Value, logs.PacketLength, logs.PacketFormat);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        feedbackBuffer.AppendLine($"Error processing column {columnNumber}: {ex.Message}");
                    }

                    processedPackets++;
                    int progressPercentage = (processedPackets * 100) / totalPackets;

                    this.Invoke(new Action(() =>
                    {
                        progressBar.Value = Math.Min(progressPercentage, progressBar.Maximum);
                        UpdateProgressLabel($"Analyzing... {progressPercentage}% completed");
                    }));

                    Application.DoEvents();
                }
            });

            feedbackBuffer.AppendLine($"Column {columnNumber} min value: {minValue}, max value: {maxValue}");
            feedback(feedbackBuffer.ToString());

            feedback($"{LogPrefix.Prefix}Exporting {exportLogs.GetPacketCount()}/{processedPackets} packets");
            lblExportPacketsCount.Text = $"{exportLogs.GetPacketCount()}";

            DateTime endTime = DateTime.Now;
            TimeSpan duration = endTime - startTime;
            string durationFormatted = duration.ToString(@"mm\:ss\.ff");
            log($"{LogPrefix.Prefix}Analysis completed in {durationFormatted}.");
            UpdateProgressLabel("Analysis finished.");
            System.Threading.Thread.Sleep(3000);
            progressBar.Visible = false;
            progressLabel.Visible = false;
        }
        private async void MultiAnalyse()
        {
            if (string.IsNullOrWhiteSpace(txtBreakOnChange.Text))
            {
                txtFeedback.Clear();
                MessageBox.Show("Column number is empty. Please enter a valid column number." + Environment.NewLine);
                DisplayLegend_Yamaha();

                return;
            }

            int columnNumber;
            try
            {
                columnNumber = int.Parse(txtBreakOnChange.Text);
            }
            catch (Exception)
            {
                txtFeedback.Clear();
                MessageBox.Show($"Invalid column number. Please enter a valid number." + Environment.NewLine);
                DisplayLegend_Yamaha();
                return;
            }

            var columnFunctions = new Dictionary<int, (Func<byte[], double>, string)>()
        {
            { 10, (packet => WoolichConversions.getRPM(packet), "RPM") },
            { 12, (packet => WoolichConversions.getTrueTPS(packet), "True TPS") },
            { 15, (packet => WoolichConversions.getWoolichTPS(packet), "Woolich TPS") },
            { 18, (packet => WoolichConversions.getCorrectETV(packet), "Correct ETV") },
            { 21, (packet => WoolichConversions.getIAP(packet), "IAP") },
            { 23, (packet => WoolichConversions.getATMPressure(packet), "ATM Pressure") },
            { 24, (packet => WoolichConversions.getGear(packet), "Gear") },
            { 26, (packet => WoolichConversions.getEngineTemperature(packet), "Engine Temperature") },
            { 27, (packet => WoolichConversions.getInletTemperature(packet), "Inlet Temperature") },
            { 28, (packet => WoolichConversions.getInjectorDuration(packet), "Injector Duration") },
            { 29, (packet => WoolichConversions.getIgnitionOffset(packet), "Ignition Offset") },
            { 31, (packet => WoolichConversions.getSpeedo(packet), "Speedo") },
            { 33, (packet => WoolichConversions.getFrontWheelSpeed(packet), "Front Wheel Speed") },
            { 35, (packet => WoolichConversions.getRearWheelSpeed(packet), "Rear Wheel Speed") },
            { 41, (packet => WoolichConversions.getBatteryVoltage(packet), "Battery Voltage") },
            { 42, (packet => WoolichConversions.getAFR(packet), "AFR") }
        };

            if (!columnFunctions.ContainsKey(columnNumber))
            {
                txtFeedback.Clear();
                MessageBox.Show("Unsupported column number. Please enter a valid column number." + Environment.NewLine);
                DisplayLegend_Yamaha();


                return;
            }

            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() != DialogResult.OK)
                    return;

                string folderPath = folderDialog.SelectedPath;
                lblDirName.Text = folderPath;

                var wrlFiles = Directory.GetFiles(folderPath, "*.wrl", SearchOption.AllDirectories);
                log($"{LogPrefix.Prefix}Total WRL files found: {wrlFiles.Length}");

                var successfulConversions = new List<string>();
                var failedConversions = new List<string>();

                DateTime startTime = DateTime.Now;
                log($"{LogPrefix.Prefix}Starting Multi File Conversion...");

                progressBar.Visible = true;
                progressLabel.Visible = true;
                progressBar.Maximum = wrlFiles.Length;
                progressBar.Value = 0;
                UpdateProgressLabel("Converting files...");

                foreach (var wrlFile in wrlFiles)
                {
                    string binFile = Path.Combine(Path.GetDirectoryName(wrlFile), Path.GetFileNameWithoutExtension(wrlFile) + ".bin");
                    ConvertWRLToBIN(wrlFile, binFile, successfulConversions, failedConversions);

                    this.Invoke(new Action(() =>
                    {
                        progressBar.Value++;
                        UpdateProgressLabel($"Converting files... {progressBar.Value}/{progressBar.Maximum} files processed");
                    }));

                    Application.DoEvents();
                }

                int totalFiles = wrlFiles.Length;
                int convertedFiles = successfulConversions.Count;
                int failedFiles = failedConversions.Count;

                log($"{LogPrefix.Prefix}Conversion completed.");
                log($"{LogPrefix.Prefix}Successfully converted {convertedFiles} files.");
                log($"{LogPrefix.Prefix}{failedFiles} files failed.");

                if (successfulConversions.Count == 0)
                {
                    feedback("No successful BIN files found in the selected folder.");
                    return;
                }

                var results = new List<(string FileName, double MaxValue)>();
                var (conversionFunction, columnName) = columnFunctions[columnNumber];

                log($"{LogPrefix.Prefix}Starting Analysis...");
                UpdateProgressLabel("Starting analysis...");

                await Task.Run(() =>
                {
                    int processedFilesCount = 0;

                    foreach (var binFile in successfulConversions)
                    {
                        double? maxValue = null;

                        try
                        {
                            using (var fileStream = new FileStream(binFile, FileMode.Open, FileAccess.Read))
                            using (var binReader = new BinaryReader(fileStream))
                            {
                                while (fileStream.Position < fileStream.Length)
                                {
                                    if (fileStream.Length - fileStream.Position < logs.PacketPrefixLength)
                                    {
                                        feedback($"File {binFile} does not have enough data for packet prefix.");
                                        break;
                                    }

                                    byte[] packetPrefixBytes = binReader.ReadBytes(logs.PacketPrefixLength);
                                    if (packetPrefixBytes.Length != logs.PacketPrefixLength)
                                    {
                                        feedback($"Incomplete packet prefix in {binFile}. Skipping.");
                                        break;
                                    }

                                    int remainingPacketBytes = packetPrefixBytes[3] - 2;

                                    if (fileStream.Length - fileStream.Position < remainingPacketBytes)
                                    {
                                        feedback($"File {binFile} does not have enough data for full packet. Skipping.");
                                        break;
                                    }

                                    byte[] packetBytes = binReader.ReadBytes(remainingPacketBytes);

                                    if (packetBytes.Length != remainingPacketBytes)
                                    {
                                        feedback($"Incomplete packet data in {binFile}. Skipping.");
                                        break;
                                    }

                                    byte[] fullPacket = packetPrefixBytes.Concat(packetBytes).ToArray();

                                    double currentValue = conversionFunction(fullPacket);

                                    if (maxValue == null || currentValue > maxValue)
                                    {
                                        maxValue = currentValue;
                                    }
                                }

                                if (maxValue.HasValue)
                                {
                                    var directoryInfo = new DirectoryInfo(Path.GetDirectoryName(binFile));
                                    var lastTwoDirs = Path.Combine(directoryInfo.Parent.Name, directoryInfo.Name);
                                    results.Add((Path.Combine(lastTwoDirs, Path.GetFileName(binFile)), maxValue.Value));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            feedback($"Error processing file {binFile}: {ex.Message}");
                        }

                        processedFilesCount++;
                        int progressPercentage = (int)((double)processedFilesCount / successfulConversions.Count * 100);

                        this.Invoke(new Action(() =>
                        {
                            progressBar.Value = Math.Min(processedFilesCount, progressBar.Maximum);
                            UpdateProgressLabel($"Analyzing... {progressPercentage}% completed");
                        }));

                        Application.DoEvents();
                    }
                });

                var sortedResults = results.OrderByDescending(r => r.MaxValue).ToList();

                DateTime endTime = DateTime.Now;
                TimeSpan duration = endTime - startTime;
                string durationFormatted = duration.ToString(@"mm\:ss\.ff");
                log($"{LogPrefix.Prefix}Analysis completed in {durationFormatted}.");

                if (sortedResults.Count > 0)
                {
                    var resultText = $"{Environment.NewLine}Found max {columnName}:{Environment.NewLine}" +
                                     string.Join(Environment.NewLine, sortedResults.Select(r => $"{r.FileName.Replace('\\', '/')} - {r.MaxValue}"));
                    feedback(resultText);
                }
                else
                {
                    feedback("No results found.");
                }

                if (failedConversions.Count > 0)
                {
                    var failedFilesText = $"{Environment.NewLine}Files that could not be converted:{Environment.NewLine}" +
                                          string.Join(Environment.NewLine, failedConversions.Select(f => f.Replace('\\', '/')));
                    feedback(failedFilesText);
                }

                UpdateProgressLabel("Analysis finished.");
                System.Threading.Thread.Sleep(3000);
                progressBar.Visible = false;
                progressLabel.Visible = false;
            }
        }
        private void RepairWRL(string inputFileName)
        {
            string directory = Path.GetDirectoryName(inputFileName);

            string outputFileName = Path.Combine(directory, Path.GetFileNameWithoutExtension(inputFileName) + "_fixed.WRL");

            try
            {
                byte[] data = File.ReadAllBytes(inputFileName);
                log($"{LogPrefix.Prefix}Read data from file. Size: {data.Length} bytes."); // Log the size of the read data

                byte[] prefix = { 0x01, 0x02, 0x5D, 0x01 };
                int prefixLength = prefix.Length; // Length of the prefix
                int interval = 96; // Define an interval for processing

                List<int> offsets = FindPrefixes(data, prefix);
                log($"{LogPrefix.Prefix}Total number of prefixes found: {offsets.Count}."); // Log the number of prefixes found

                bool needsRepair;
                byte[] recoveredData = ProcessPackets(data, offsets, prefixLength, interval, out needsRepair);

                if (needsRepair)
                {
                    File.WriteAllBytes(outputFileName, recoveredData);
                    log($"{LogPrefix.Prefix}File repaired and saved to:{Environment.NewLine}{outputFileName}."); // Log completion and file save information
                }
                else
                {
                    log($"{LogPrefix.Prefix}No repair needed for file: {Path.GetFileName(inputFileName)}.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during repair: {ex.Message}", "Repair Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnRepairWRL_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "WRL files (*.WRL)|*.WRL|All files (*.*)|*.*"; // Filter for WRL files and all files
                openFileDialog.Title = "Select a WRL File"; // Title of the dialog

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string inputFileName = openFileDialog.FileName;

                    RepairWRL(inputFileName);
                }
            }
        }
        private async void ExportDirToText()
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select the folder containing WRL files";
                folderDialog.ShowNewFolderButton = false;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string rootFolderPath = folderDialog.SelectedPath;

                    var wrlFiles = Directory.EnumerateFiles(rootFolderPath, "*.wrl", SearchOption.AllDirectories);

                    progressBar.Visible = true;
                    progressLabel.Visible = true;
                    UpdateProgressLabel("Starting conversion...");

                    int totalFiles = wrlFiles.Count();
                    int processedFiles = 0;

                    string exportFormat = cmbExportFormat.SelectedItem.ToString().ToLower();
                    string fileExtension = exportFormat == "tsv" ? ".tsv" : ".csv"; // Choose .csv or .tsv
                    char delimiter = exportFormat == "tsv" ? '\t' : ','; // Tab for TSV, comma for CSV

                    await Task.Run(() =>
                    {
                        foreach (var filePath in wrlFiles)
                        {
                            try
                            {
                                bool fileLoaded = LoadFile(filePath);
                                if (!fileLoaded)
                                {
                                    Invoke(new Action(() => log($"{LogPrefix.Prefix}Skipping file {filePath}.")));
                                    continue;
                                }

                                string directoryPath = Path.GetDirectoryName(filePath);
                                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                                var exportFileName = Path.Combine(directoryPath, fileNameWithoutExtension + fileExtension);

                                WoolichMT09Log exportItem = logs; // Assuming 'logs' is the loaded log item
                                using (StreamWriter outputFile = new StreamWriter(exportFileName))
                                {
                                    string exportHeader = exportItem.GetHeader(this.presumedStaticColumns, new List<int>());
                                    outputFile.WriteLine(string.Join(delimiter.ToString(), exportHeader.Split(','))); // Dynamic delimiter

                                    var packets = exportItem.GetPackets();
                                    foreach (var packet in packets)
                                    {
                                        var exportLine = WoolichMT09Log.getCSV(packet.Value, packet.Key, exportItem.PacketFormat, this.presumedStaticColumns, new List<int>());
                                        outputFile.WriteLine(string.Join(delimiter.ToString(), exportLine.Split(','))); // Dynamic delimiter
                                    }
                                }

                                processedFiles++;
                                Invoke(new Action(() => UpdateProgressLabel($"Processing file {processedFiles}/{totalFiles}")));
                                int progressPercentage = (processedFiles * 100) / totalFiles;
                                Invoke(new Action(() => progressBar.Value = Math.Min(progressPercentage, progressBar.Maximum)));
                            }
                            catch (Exception ex)
                            {
                                Invoke(new Action(() => log($"{LogPrefix.Prefix}Failed to process file {filePath}: {ex.Message}")));
                            }
                        }

                        Invoke(new Action(() => log($"{LogPrefix.Prefix}Conversion completed for {processedFiles} files.")));
                        Invoke(new Action(() => UpdateProgressLabel("Conversion completed successfully.")));
                    });
                }
            }

            progressBar.Visible = false;
            progressLabel.Visible = false;
            UpdateProgressLabel("Export finished.");
        }
        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            try
            {
                openWRLFileDialog.Title = "Select WRL file to inspect";
                openWRLFileDialog.InitialDirectory = string.IsNullOrWhiteSpace(openWRLFileDialog.InitialDirectory)
                    ? logFolder ?? Directory.GetCurrentDirectory()
                    : openWRLFileDialog.InitialDirectory;

                openWRLFileDialog.Multiselect = false;
                openWRLFileDialog.Filter = "WRL files (*.wrl)|*.wrl|BIN files (*.bin)|*.bin|All files (*.*)|*.*";

                if (openWRLFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                ClearBoxAndPackets();
                var filename = openWRLFileDialog.FileNames.FirstOrDefault();
                OpenFileName = filename;
                UpdateFormsStates();

                if (!File.Exists(filename))
                {
                    lblFileName.Text = "Error: File Not Found";
                    return;
                }

                logFolder = Path.GetDirectoryName(filename);

                if (!LoadFile(filename))
                {
                    HandleCorruptedFile();
                    return;
                }

                string binOutputFileName = Path.Combine(logFolder, Path.GetFileNameWithoutExtension(filename) + ".bin");
                ConvertWRLToBIN(filename, binOutputFileName, new List<string>(), new List<string>());

                lblFileName.Text = Path.GetFileName(filename);
                lblDirName.Text = Path.GetDirectoryName(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}\n\n Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void HandleCorruptedFile()
        {
            DialogResult result = MessageBox.Show("The file is corrupted or has an invalid format.\n\nWould you like to attempt repair?", "File Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

            if (result == DialogResult.Yes)
            {
                RepairWRL(OpenFileName);

                string repairedFile = Path.Combine(logFolder, Path.GetFileNameWithoutExtension(OpenFileName) + "_fixed.WRL");
                if (File.Exists(repairedFile))
                {
                    logs.ClearPackets();
                    Array.Clear(logs.PrimaryHeaderData, 0, logs.PrimaryHeaderData.Length);
                    Array.Clear(logs.SecondaryHeaderData, 0, logs.SecondaryHeaderData.Length);
                    OpenFileName = repairedFile;

                    ClearBoxAndPackets();
                    logFolder = Path.GetDirectoryName(repairedFile);
                    lblFileName.Text = Path.GetFileName(repairedFile);
                    lblDirName.Text = Path.GetDirectoryName(repairedFile);

                    if (!LoadFile(repairedFile))
                    {
                        MessageBox.Show("Repair operation failed or was not needed. The file could not be opened.", "Repair Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        string binOutputFileName = Path.Combine(logFolder, Path.GetFileNameWithoutExtension(repairedFile) + ".bin");
                        ConvertWRLToBIN(repairedFile, binOutputFileName, new List<string>(), new List<string>());
                    }
                }
            }
        }
        private bool LoadFile(string filename)
        {
            try
            {
                logs.ClearPackets();
                Array.Clear(logs.PrimaryHeaderData, 0, logs.PrimaryHeaderData.Length);
                Array.Clear(logs.SecondaryHeaderData, 0, logs.SecondaryHeaderData.Length);

                if (!File.Exists(filename))
                {
                    lblFileName.Text = "Error: File Not Found";
                    return false;
                }

                logFolder = Path.GetDirectoryName(filename);
                openWRLFileDialog.InitialDirectory = logFolder;

                string binOutputFileName = Path.Combine(logFolder, Path.GetFileNameWithoutExtension(filename) + ".bin");
                lblFileName.Text = Path.GetFileName(filename);
                lblDirName.Text = Path.GetDirectoryName(filename);

                using (var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                using (var binReader = new BinaryReader(fileStream, Encoding.ASCII))
                {
                    logs.PrimaryHeaderData = binReader.ReadBytes(logs.PrimaryHeaderLength);

                    byte[] searchPattern = { 0x01, 0x02, 0x5D, 0x01 };
                    long position = FindPatternInFile(filename, searchPattern);

                    if (position >= 0)
                    {
                        log($"{LogPrefix.Prefix}Header marker was found at position {position} bytes.");

                        if (position != logs.PrimaryHeaderLength + 1)
                        {
                            logs.SecondaryHeaderData = binReader.ReadBytes(logs.SecondaryHeaderLength);
                            exportLogs.SecondaryHeaderData = logs.SecondaryHeaderData;
                        }
                    }
                    else
                    {
                        logs.SecondaryHeaderData = binReader.ReadBytes(logs.SecondaryHeaderLength);
                        exportLogs.SecondaryHeaderData = logs.SecondaryHeaderData;
                    }

                    exportLogs.PrimaryHeaderData = logs.PrimaryHeaderData;

                    while (true)
                    {
                        byte[] packetPrefixBytes = binReader.ReadBytes(logs.PacketPrefixLength);
                        if (packetPrefixBytes.Length < 5)
                            break;

                        int remainingPacketBytes = packetPrefixBytes[3] - 2;
                        byte[] packetBytes = binReader.ReadBytes(remainingPacketBytes);

                        if (packetBytes.Length < remainingPacketBytes)
                            break;

                        int totalPacketLength = packetPrefixBytes[3] + 3;
                        logs.AddPacket(packetPrefixBytes.Concat(packetBytes).ToArray(), totalPacketLength, packetPrefixBytes[4]);
                    }

                    log($"{LogPrefix.Prefix}Data Loaded and {logs.GetPacketCount()} packets found.");
                }

                using (var fileStream = new FileStream(binOutputFileName, FileMode.Create))
                using (var binWriter = new BinaryWriter(fileStream))
                {
                    foreach (var packet in logs.GetPackets())
                    {
                        binWriter.Write(packet.Value);
                    }
                }

                string fileName = Path.GetFileName(binOutputFileName);
                log($"{LogPrefix.Prefix}BIN file created and saved as: {fileName}");
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                log($"{LogPrefix.Prefix}File is corrupted or has an invalid format: {filename}.");
                return false;
            }
            catch (Exception)
            {
                log($"{LogPrefix.Prefix}An unexpected error occurred with file {filename}.");
                return false;
            }
        }
        private void ExportAutoTune()
        {
            if (!IsFileLoaded())
            {

                return;
            }

            WoolichMT09Log exportItem = logs;

            // Check if the packet format is supported
            if (exportItem.PacketFormat != 0x01)
            {
                MessageBox.Show("This bikes file cannot be adjusted by this software yet.");
                return;
            }

            // Define the filter options and their corresponding binary values
            var filterOptionMap = new Dictionary<string, int>
    {
        { autoTuneFilterOptions[0], 0 }, // MT09 ETV correction
        { autoTuneFilterOptions[1], 1 }, // Remove Gear 2 logs
        { autoTuneFilterOptions[2], 2 }, // Exclude below 1200 rpm
        { autoTuneFilterOptions[3], 3 }, // Remove Gear 1, 2 & 3 engine braking
        { autoTuneFilterOptions[4], 4 }  // Remove non-launch gear 1
    };

            // Create a binary string based on selected filter options
            char[] binaryArray = new char[5];
            for (int i = 0; i < 5; i++)
            {
                string filterOption = autoTuneFilterOptions[i];
                binaryArray[i] = this.aTFCheckedListBox.CheckedItems.Contains(filterOption) ? '1' : '0';
            }

            string binaryString = new string(binaryArray);
            string baseFileName = Path.GetFileNameWithoutExtension(lblFileName.Text.Trim());
            string directoryPath = lblDirName.Text.Trim();
            string outputFileNameWithExtension = Path.Combine(directoryPath, $"{baseFileName}_{binaryString}_AT.WRL");

            try
            {
                using (BinaryWriter binWriter = new BinaryWriter(File.Open(outputFileNameWithExtension, FileMode.Create)))
                {
                    binWriter.Write(exportItem.PrimaryHeaderData);
                    binWriter.Write(exportItem.SecondaryHeaderData);

                    foreach (var packet in exportItem.GetPackets())
                    {
                        byte[] exportPackets = packet.Value.ToArray();
                        int diff = 0;

                        var outputGear = packet.Value.getGear();

                        if (outputGear == 2 && this.aTFCheckedListBox.CheckedItems.Contains(autoTuneFilterOptions[1]))
                        {
                            byte newOutputGearByte = (byte)(exportPackets[24] & (~0b00000111));
                            diff = diff + newOutputGearByte - outputGear;
                            outputGear = newOutputGearByte;
                            exportPackets[24] = newOutputGearByte;
                        }

                        int minRPM = int.Parse(this.minRPM.Text);
                        int maxRPM = int.Parse(this.maxRPM.Text);

                        if (outputGear == 1 && (packet.Value.getRPM() < minRPM || packet.Value.getRPM() > maxRPM) && this.aTFCheckedListBox.CheckedItems.Contains(autoTuneFilterOptions[4]))
                        {
                            byte newOutputGearByte = (byte)(exportPackets[24] & (~0b00000111));
                            diff = diff + newOutputGearByte - outputGear;
                            outputGear = newOutputGearByte;
                            exportPackets[24] = newOutputGearByte;
                        }

                        int rpmLimit = int.Parse(idleRPM.Text);

                        if (outputGear != 1 && packet.Value.getRPM() <= rpmLimit && this.aTFCheckedListBox.CheckedItems.Contains(autoTuneFilterOptions[2]))
                        {
                            byte newOutputGearByte = (byte)(exportPackets[24] & (~0b00000111));
                            diff = diff + newOutputGearByte - outputGear;
                            outputGear = newOutputGearByte;
                            exportPackets[24] = newOutputGearByte;
                        }

                        if (packet.Value.getCorrectETV() <= 1.2 && outputGear < 4 && this.aTFCheckedListBox.CheckedItems.Contains(autoTuneFilterOptions[3]))
                        {
                            byte newOutputGearByte = (byte)(exportPackets[24] & (~0b00000111));
                            diff = diff + newOutputGearByte - outputGear;
                            outputGear = newOutputGearByte;
                            exportPackets[24] = newOutputGearByte;
                        }

                        if (this.aTFCheckedListBox.CheckedItems.Contains(autoTuneFilterOptions[0]))
                        {
                            double correctETV = exportPackets.getCorrectETV();
                            byte hackedETVbyte = (byte)((correctETV * 1.66) + 38);
                            diff = diff + hackedETVbyte - exportPackets[18];
                            exportPackets[18] = hackedETVbyte;
                        }
                        exportPackets[95] += (byte)diff;

                        binWriter.Write(exportPackets);
                    }
                }

                log($"{LogPrefix.Prefix}Autotune WRL File saved as: " + Path.GetFileName(outputFileNameWithExtension));
            }
            catch (Exception ex)
            {

                log($"{LogPrefix.Prefix}Autotune WRL File saving error: {ex.Message}");
            }
        }
        private void ConvertWRLToBIN(string wrlFileName, string binFileName, List<string> successfulConversions, List<string> failedConversions)
        {
            bool conversionSuccessful = false;

            try
            {
                using (var fileStream = new FileStream(wrlFileName, FileMode.Open, FileAccess.Read))
                using (var binReader = new BinaryReader(fileStream, Encoding.ASCII))
                using (var binFileStream = new FileStream(binFileName, FileMode.Create))
                using (var binWriter = new BinaryWriter(binFileStream))
                {
                    logs.PrimaryHeaderData = binReader.ReadBytes(logs.PrimaryHeaderLength);

                    byte[] searchPattern = { 0x01, 0x02, 0x5D, 0x01 };
                    long position = FindPatternInFile(wrlFileName, searchPattern);

                    if (position >= 0)
                    {
                        if (position != logs.PrimaryHeaderLength + 1)
                        {
                            logs.SecondaryHeaderData = binReader.ReadBytes(logs.SecondaryHeaderLength);
                            exportLogs.SecondaryHeaderData = logs.SecondaryHeaderData;
                        }
                    }
                    else
                    {
                        logs.SecondaryHeaderData = binReader.ReadBytes(logs.SecondaryHeaderLength);
                        exportLogs.SecondaryHeaderData = logs.SecondaryHeaderData;
                    }

                    exportLogs.PrimaryHeaderData = logs.PrimaryHeaderData;

                    while (true)
                    {
                        byte[] packetPrefixBytes = binReader.ReadBytes(logs.PacketPrefixLength);
                        if (packetPrefixBytes.Length < logs.PacketPrefixLength)
                            break;

                        int remainingPacketBytes = packetPrefixBytes[3] - 2;
                        byte[] packetBytes = binReader.ReadBytes(remainingPacketBytes);

                        if (packetBytes.Length < remainingPacketBytes)
                            break;

                        byte[] packet = packetPrefixBytes.Concat(packetBytes).ToArray();
                        binWriter.Write(packet);
                    }

                    conversionSuccessful = true;
                }

                if (conversionSuccessful)
                {
                    successfulConversions.Add(binFileName);
                }
            }
            catch (Exception)
            {
                feedback($"ERROR converting file: {Path.GetFileName(wrlFileName)}");

                failedConversions.Add(wrlFileName);

                if (File.Exists(binFileName))
                {
                    try
                    {
                        File.Delete(binFileName);
                        feedback($"Deleted corrupted BIN file: {Path.GetFileName(binFileName)}");
                    }
                    catch (Exception deleteEx)
                    {
                        feedback($"Failed to delete corrupted BIN file {binFileName}: {deleteEx.Message}");
                    }
                }
            }
        }
        private void ExportDirAutoTune()
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportDirAutoTune(folderDialog.SelectedPath);
                }
            }
        }
        private void ExportDirAutoTune(string directoryPath)
        {
            try
            {
                var wrlFiles = Directory.GetFiles(directoryPath, "*.wrl", SearchOption.AllDirectories);

                if (wrlFiles.Length == 0)
                {
                    MessageBox.Show("No WRL files found in the selected directory.", "No Files", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                int successfulCount = 0;
                int failedCount = 0;
                var stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                foreach (var wrlFile in wrlFiles)
                {
                    OpenFileName = wrlFile;

                    bool fileLoaded = LoadFile(wrlFile);
                    if (fileLoaded)
                    {
                        ExportAutoTune();
                        successfulCount++;
                    }
                    else
                    {
                        log($"{LogPrefix.Prefix}Failed to load file: {wrlFile}");
                        failedCount++;
                    }
                }

                stopwatch.Stop();

                // Calculate time in minutes and seconds
                var elapsed = stopwatch.Elapsed;
                int minutes = elapsed.Minutes;
                int seconds = elapsed.Seconds;

                log($"{LogPrefix.Prefix}Total files processed: {wrlFiles.Length}");
                log($"{LogPrefix.Prefix}Successfully processed: {successfulCount}");
                log($"{LogPrefix.Prefix}Failed to process: {failedCount}");
                log($"{LogPrefix.Prefix}Total processing time: {minutes} minutes {seconds} seconds");

                DeleteBinFiles(directoryPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while processing the files: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void UpdateCmbExport()
        {
            // Define the options to display based on the selected index in cmbExportType
            string[][] options = new string[][]
            {
        new string[] { "csv", "tsv" },       // For "Full File" (index 0)
        new string[] { "csv", "tsv", "wrl" }, // For "Analysis Only" (index 1)
        new string[] { "wrl" },               // For "CRC" (index 2)
        new string[] { "wrl" }                // For "Autotune" (index 3)
            };

            // Clear existing items in cmbExportFormat
            cmbExportFormat.Items.Clear();

            // Set items based on the selected index in cmbExportType
            int selectedTypeIndex = cmbExportType.SelectedIndex;
            if (selectedTypeIndex >= 0 && selectedTypeIndex < options.Length)
            {
                cmbExportFormat.Items.AddRange(options[selectedTypeIndex]);
                cmbExportFormat.SelectedIndex = 0; // Default to the first option

                // Check if the cmbExportMode should be enabled or disabled
                if (selectedTypeIndex == 0 || selectedTypeIndex == 1) // "Full File" or "Analysis Only"
                {
                    cmbExportMode.Enabled = true;
                    if (cmbExportMode.SelectedIndex == -1)
                    {
                        cmbExportMode.SelectedIndex = 0;
                    }
                }
                else if (selectedTypeIndex == 2 || selectedTypeIndex == 3) // "CRC" or "Autotune"
                {
                    cmbExportMode.Enabled = true; // Keep enabled for Autotune
                }

                // Disable cmbExportFormat if "CRC" is selected
                if (selectedTypeIndex == 2)
                {
                    cmbExportFormat.Enabled = false;
                }

                // For "Autotune", gray out the cmbExportFormat and enable aTFCheckedListBox
                if (selectedTypeIndex == 3) // Autotune
                {
                    cmbExportFormat.Enabled = false;  // Disable the export format combo box
                    cmbExportFormat.SelectedIndex = 0; // Set to WRL as default

                    // Enable the AutoTune filter options (aTFCheckedListBox)
                    aTFCheckedListBox.Enabled = true;
                    idleRPM.Enabled = true;
                    minRPM.Enabled = true;
                    maxRPM.Enabled = true;
                }
                else
                {
                    // For other types, enable the export format selection
                    //cmbExportFormat.Enabled = true;

                    // Disable the AutoTune filter options (aTFCheckedListBox) for other export types
                    aTFCheckedListBox.Enabled = false;
                    idleRPM.Enabled = false;
                    minRPM.Enabled = false;
                    maxRPM.Enabled = false;
                }
            }

            // Additional settings if Directory mode is selected in cmbExportMode
            if (cmbExportMode.SelectedIndex == 1) // Directory mode
            {
                // Check if "Autotune" is selected in cmbExportType
                if (selectedTypeIndex == 3) // Autotune
                {
                    // Set export format to WRL only
                    cmbExportFormat.Items.Clear();
                    cmbExportFormat.Items.Add("wrl");
                    cmbExportFormat.SelectedIndex = 0; // Default to WRL
                    cmbExportFormat.Enabled = false; // Disable format combo box for Autotune
                }
                else
                {
                    // Clear format options and set to CSV/TSV for non-Autotune options
                    cmbExportFormat.Items.Clear();
                    cmbExportFormat.Items.Add("csv");
                    cmbExportFormat.Items.Add("tsv");
                    cmbExportFormat.SelectedIndex = 0; // Default to the first option
                    cmbExportFormat.Enabled = true; // Enable format selection
                }
            }

            // Enable or disable the CRCsize textbox based on selected Type
            CRCsize.Enabled = (selectedTypeIndex == 2); // Enable if CRC is selected

            // Ensure cmbExportType and cmbExportFormat are enabled if cmbExportMode is not 1
            cmbExportType.Enabled = true;
            cmbExportFormat.Enabled = (selectedTypeIndex < 2 || selectedTypeIndex == 3); // Enable for "Full File", "Analysis Only", or "Autotune"

            // Make btnExport active when mode is set to 1 (Directory) or valid conditions
            btnExport.Enabled = cmbExportMode.SelectedIndex != -1 && cmbExportType.SelectedIndex != -1;
        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            int exportTypeIndex = cmbExportType.SelectedIndex;
            int exportFormatIndex = cmbExportFormat.SelectedIndex;
            int exportModeIndex = cmbExportMode.SelectedIndex;

            // Dictionary mapping (mode, type, format) to corresponding export functions
            var exportActions = new Dictionary<(int mode, int type, int format), Action>
    {
        {(0, 0, 0), ExportToText},  // File, Full File, csv
        {(0, 0, 1), ExportToText},  // File, Full File, tsv
        {(0, 1, 0), ExportToText},  // File, Analysis Only, csv
        {(0, 1, 1), ExportToText},  // File, Analysis Only, tsv
        {(0, 1, 2), ExportTargetColumn}, // File, Analysis Only, wrl
        {(0, 2, 0), ExportCRCHack},  // File, CRC, wrl
        {(0, 3, 0), ExportAutoTune}, // File, Autotune, wrl
        {(1, 0, 0), ExportDirToText}, // Directory, Full File, csv
        {(1, 0, 1), ExportDirToText}, // Directory, Full File, tsv
        {(1, 3, 0), ExportDirAutoTune} // Directory, Autotune, wrl
    };

            // Try to find the action for the selected combination of mode, type, and format
            if (exportActions.TryGetValue((exportModeIndex, exportTypeIndex, exportFormatIndex), out Action exportAction))
            {
                exportAction.Invoke();
            }
            else
            {
                MessageBox.Show(
                    "Option is not supported at the moment.",
                    "Not Supported",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }
        private void DeleteBinFiles(string directoryPath)
        {
            if (cmbBinDelete.Text == "Delete")
            {
                var wrlFiles = Directory.GetFiles(directoryPath, "*.wrl", SearchOption.AllDirectories);
                int totalCount = 0;
                int deletedCount = 0;
                int failedCount = 0;

                foreach (var wrlFile in wrlFiles)
                {
                    string binOutputFileName = Path.Combine(Path.GetDirectoryName(wrlFile),
                        Path.GetFileNameWithoutExtension(wrlFile) + ".bin");

                    //log($"{LogPrefix.Prefix}Checking for BIN file: {binOutputFileName}"); // Debug log

                    if (File.Exists(binOutputFileName))
                    {
                        //log($"{LogPrefix.Prefix}Found BIN file: {binOutputFileName}"); // Debug log
                        totalCount++;

                        try
                        {
                            File.Delete(binOutputFileName);
                            deletedCount++;
                            //log($"{LogPrefix.Prefix}Deleted BIN file: {binOutputFileName}");
                        }
                        catch (Exception)
                        {
                            failedCount++;
                            //log($"{LogPrefix.Prefix}Failed to delete BIN file: {ex.Message}");
                        }
                    }
                    else
                    {
                        //log($"{LogPrefix.Prefix}BIN file does not exist, skipping: {binOutputFileName}"); // Debug log
                    }
                }

                log($"{LogPrefix.Prefix}Total BIN files checked: {totalCount}");
                log($"{LogPrefix.Prefix}Successfully deleted: {deletedCount}");
                log($"{LogPrefix.Prefix}Failed to delete: {failedCount}");
            }
            else
            {
                log($"{LogPrefix.Prefix}Deletion not enabled. Setting is on: {cmbBinDelete.Text}"); // Debug log
            }
        }







    }
}





