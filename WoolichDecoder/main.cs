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
//using System.Drawing;



namespace WoolichDecoder
{
    public partial class WoolichFileDecoderForm : Form
    {
        string OpenFileName = string.Empty;

        WoolichMT09Log logs = new WoolichMT09Log();

        WoolichMT09Log exportLogs = new WoolichMT09Log();

        WoolichMT09Log yamaha = new WoolichMT09Log();

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
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            System.Globalization.CultureInfo.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
            InitializeComponent();
            cmbExportType.SelectedIndex = 0;
            cmbExportFormat.SelectedIndex = 0;
            cmbLogsLocation.SelectedIndex = 0;
            cmbBinDelete.SelectedIndex = 0;
            cmbExportMode.SelectedIndex = 0;
            cmbATFileName.SelectedIndex = 0;
            cmbExportType.SelectedIndexChanged += cmbExportType_Change;
            cmbExportMode.SelectedIndexChanged += cmbExportMode_Change;
            lblTotalPacketsCount.Text = "0";
            btnAnalyse.MouseEnter += (s, e) =>
            {
                //cmbExportType.SelectedIndex = 1;
                cmbExportFormat.Enabled = false;
                cmbExportType.Enabled = false;
                btnExport.Enabled = false;
            };
            btnAnalyse.MouseLeave += (s, e) =>
            {
                //cmbExportType.SelectedIndex = 0;
                cmbExportFormat.Enabled = true;
                cmbExportType.Enabled = true;
                btnExport.Enabled = true;
            };
        }
        private bool IsFileLoaded()
        {
            if (string.IsNullOrEmpty(OpenFileName))
            {
                var result = MessageBox.Show("Please open a WRL file before proceeding. Would you like to select a file now?",
                                              "Information",
                                              MessageBoxButtons.OKCancel,
                                              MessageBoxIcon.Information);

                if (result == DialogResult.OK)
                {
                    OpenFile_Click(this, EventArgs.Empty);
                    return !string.IsNullOrEmpty(OpenFileName);
                }
                return false;
            }
            return true;
        }
        public static class LogPrefix
        {
            private static readonly string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            public static string Prefix => $"{DateTime.Now.ToString(DateTimeFormat)} -- ";
        }
        public void CompareAFR_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "This function compares AFR(for Yamaha bikes) from Woolich Racing Tuned and Woolich File Decoder.\n\nTo proceed, you need two CSV files converted from the same WRL file in the Woolich Racing Tuned software and this application.",
                "Information",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Information);

            if (result == DialogResult.Cancel)
            {
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Select CSV File from Woolich Racing Tuned software"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string wrtFilePath = openFileDialog.FileName;

                openFileDialog.Title = "Select CSV File from this application";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string woolichFilePath = openFileDialog.FileName;

                    var wrtLines = File.ReadAllLines(wrtFilePath);
                    var woolichLines = File.ReadAllLines(woolichFilePath);

                    if (wrtLines.Length < 2 || woolichLines.Length < 2)
                    {
                        MessageBox.Show("Not enough data in one of the files.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var firstWrtColumns = wrtLines[1].Split(',').Length; // Skip header
                    var firstWoolichColumns = woolichLines[1].Split(',').Length; // Skip header

                    if (firstWrtColumns >= firstWoolichColumns)
                    {
                        MessageBox.Show("Something went wrong.\n\nCheck if you opened file from Woolich Racing Tuned software first.", "Files Loading Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    List<(string timestamp, double wrtAFR, double woolichAFR)> comparisons = new List<(string, double, double)>();

                    for (int attempts = 0; attempts < 20 && comparisons.Count < 5; attempts++)
                    {
                        int randomIndex = new Random().Next(1, wrtLines.Length);
                        var wrtLine = wrtLines[randomIndex].Split(',');

                        if (double.TryParse(wrtLine[4], out double wrtAFR))
                        {
                            string timestamp = wrtLine[0];

                            for (int j = 1; j < woolichLines.Length; j++)
                            {
                                var woolichLine = woolichLines[j].Split(',');
                                if (timestamp == woolichLine[0] && double.TryParse(woolichLine[9], out double woolichAFR))
                                {
                                    comparisons.Add((timestamp, wrtAFR, woolichAFR));
                                    break;
                                }
                            }
                        }
                    }

                    if (comparisons.Count < 5)
                    {
                        MessageBox.Show("Not enough matching timestamps found for comparison.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    StringBuilder comparisonResults = new StringBuilder();
                    comparisonResults.AppendLine("Time                 |  WRT     |  WFD      |  Diff");
                    comparisonResults.AppendLine("--------------------------------------------------");

                    double totalDifference = 0;
                    int validComparisons = 0;
                    double currentDivisor = double.Parse(AFRdivisor.Text);

                    foreach (var (timestamp, wrtAFR, woolichAFR) in comparisons)
                    {
                        double difference = Math.Abs(wrtAFR - woolichAFR);
                        totalDifference += woolichAFR / wrtAFR;
                        validComparisons++;

                        comparisonResults.AppendLine($"{timestamp.PadRight(15)} | {wrtAFR.ToString("F2").PadRight(9)} | {woolichAFR.ToString("F2").PadRight(10)} | {difference.ToString("F2")}");
                    }

                    if (validComparisons > 0)
                    {
                        double averageRatio = totalDifference / validComparisons;
                        double newDivisor = currentDivisor * averageRatio;

                        comparisonResults.AppendLine();
                        comparisonResults.AppendLine($"     Current AFR divisor: {currentDivisor}");
                        comparisonResults.AppendLine($"Suggested AFR divisor: {Math.Round(newDivisor, 2)}");
                    }
                    else
                    {
                        comparisonResults.AppendLine("No valid AFR comparisons could be made.");
                    }

                    MessageBox.Show(comparisonResults.ToString(), "AFR Comparison and Correction Factor", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private double GetAFRDivisor()
        {
            if (double.TryParse(AFRdivisor.Text, out double divisor) && divisor >= 1 && divisor <= 20)
            {
                return divisor;
            }

            MessageBox.Show("Invalid value. Setting to default (10.2).", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            AFRdivisor.Text = "10.2";
            return 10.2;
        }
        private void AFRdivisor_TextChanged(object sender, EventArgs e)
        {
            GetAFRDivisor();
        }
        private void feedback(string fbData)
        {
            string directoryPath = LogsDirectory();

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
            string directoryPath = LogsDirectory();

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
        private void LogSeparator()
        {
            log($"{LogPrefix.Prefix}-------------------------------------------------------------------------");
        }
        private void cmbExportType_Change(object sender, EventArgs e)
        {
            UpdateExportSettings();
        }
        private void cmbExportMode_Change(object sender, EventArgs e)
        {
            UpdateExportSettings();
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
        private void App_Load(object sender, EventArgs e)
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

            AFRdivisor.Text = userSettings.AFRdivisor;
            idleRPM.Text = userSettings.idleRPM;
            minRPM.Text = userSettings.minRPM;
            maxRPM.Text = userSettings.maxRPM;

            for (int i = 0; i < this.aTFCheckedListBox.Items.Count; i++)
            {
                aTFCheckedListBox.SetItemCheckState(i, CheckState.Checked);
            }

            toolTip1.SetToolTip(btnExport,
                            "Executes options from Mode, Type, and Format.\n\n" +
                            "Available functions include:\n" +
                            "- Converting to text format (CSV, TSV)\n" +
                            "- Analyzing specific packets in a selected column\n    and saving results in CSV, TSV, or WRL format.\n" +
                            "- Saving CRC as counted packets\n" +
                            "- Saving filtered autotune data");

        }
        private void App_Close(object sender, FormClosingEventArgs e)
        {
            userSettings.LogDirectory = this.logFolder;
            userSettings.AFRdivisor = AFRdivisor.Text;
            userSettings.idleRPM = idleRPM.Text;
            userSettings.minRPM = minRPM.Text;
            userSettings.maxRPM = maxRPM.Text;


            // save the user settings.
            userSettings.Save();
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
        private string LogsDirectory()
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
        private void ExportToCRC()
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
        private void ExportColumnToWRL()
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
        private void UpdateFormsStates()
        {
            bool isFileLoaded = IsFileLoaded();
            aTFCheckedListBox.Enabled = isFileLoaded;
            idleRPM.Enabled = isFileLoaded;
            minRPM.Enabled = isFileLoaded;
            maxRPM.Enabled = isFileLoaded;
            txtBreakOnChange.Enabled = isFileLoaded;
        }
        private async void Analyse_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBreakOnChange.Text))
            {
                txtFeedback.Clear();
                MessageBox.Show("Please enter a column number for export analysis.", "Column Number Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DisplayLegend_Yamaha();
                return;
            }

            if (cmbBinDelete.SelectedIndex == 2 && cmbExportMode.SelectedIndex == 1)
            {
                MessageBox.Show("This option is not available yet.", "Option Unavailable", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            double divisor = GetAFRDivisor();

            int columnNumber;
            try
            {
                columnNumber = int.Parse(txtBreakOnChange.Text);
            }
            catch (Exception)
            {
                txtFeedback.Clear();
                MessageBox.Show("Please enter a valid number for export analysis.", "Invalid Column Number", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DisplayLegend_Yamaha();
                return;
            }

            var columnFunctions = new Dictionary<int, (Func<byte[], double>, string)>
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
            { 42, (packet => WoolichConversions.getAFR(packet, divisor), "AFR") }
        };

            if (!columnFunctions.ContainsKey(columnNumber))
            {
                MessageBox.Show("Please enter a valid column number for export analysis.", "Unsupported Column Number", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DisplayLegend_Yamaha();
                return;
            }

            var (conversionFunction, columnName) = columnFunctions[columnNumber];
            lblExportPacketsCount.Text = "0";
            exportLogs.ClearPackets();
            DateTime startTime = DateTime.Now;

            if (cmbExportMode.SelectedIndex == 0)
            {
                if (!IsFileLoaded())
                    return;

                log($"{LogPrefix.Prefix}Start analyzing...");
                txtFeedback.Clear();
                feedback($"Analyzing column {columnNumber}...");
                var packets = logs.GetPackets();
                StringBuilder feedbackBuffer = new StringBuilder();

                double? previousValue = null;
                double? minValue = null;
                double? maxValue = null;

                int processedPackets = 0;
                int totalPackets = packets.Count;

                progressBar.Visible = true;
                progressLabel.Visible = true;
                progressBar.Value = 0;
                UpdateProgressLabel("Starting analysis...");

                await Task.Run(() =>
                {
                    int updateFrequency = Math.Max(1, totalPackets / 10); // Update every 10% packets
                    foreach (var packet in packets)
                    {
                        try
                        {
                            double currentValue = conversionFunction(packet.Value);
                            processedPackets++;

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
                                exportLogs.AddPacket(packet.Value, logs.PacketLength, logs.PacketFormat);
                            }
                        }
                        catch (Exception ex)
                        {
                            feedbackBuffer.AppendLine($"Error processing column {columnNumber}: {ex.Message}");
                        }

                        // Update progress every 10% processed packets
                        if (processedPackets % updateFrequency == 0)
                        {
                            int progressPercentage = (processedPackets * 100) / totalPackets;

                            this.Invoke(new Action(() =>
                            {
                                progressBar.Value = Math.Min(progressPercentage, progressBar.Maximum);
                                UpdateProgressLabel($"Analyzing... {progressPercentage}% completed");
                            }));
                        }

                        Application.DoEvents();
                    }
                });

                feedbackBuffer.AppendLine($"Column {columnNumber} min value: {minValue}, max value: {maxValue}");
                feedback(feedbackBuffer.ToString());

                LogSeparator();
                log($"{LogPrefix.Prefix}Analysis summary for column {columnNumber}:");
                log($"{LogPrefix.Prefix}Total packets analyzed: {processedPackets}");
                log($"{LogPrefix.Prefix}Packets exported: {exportLogs.GetPacketCount()}");
                log($"{LogPrefix.Prefix}Min value: {minValue}, Max value: {maxValue}");

                feedback($"{LogPrefix.Prefix}Exporting {exportLogs.GetPacketCount()}/{processedPackets} packets");
                lblExportPacketsCount.Text = $"{exportLogs.GetPacketCount()}";

                progressBar.Visible = false;
                progressLabel.Visible = false;
            }
            else if (cmbExportMode.SelectedIndex == 1)
            {
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
                            UpdateProgressLabel($"Converting files... {progressBar.Value}/{wrlFiles.Length} files processed");
                        }));

                        Application.DoEvents();
                    }

                    if (successfulConversions.Count == 0)
                    {
                        feedback("No successful BIN files found in the selected folder.");
                        return;
                    }

                    var results = new List<(string FileName, double MaxValue)>();
                    log($"{LogPrefix.Prefix}Starting Analysis...");

                    await Task.Run(() =>
                    {
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
                                        byte[] packetPrefixBytes = binReader.ReadBytes(logs.PacketPrefixLength);
                                        int remainingPacketBytes = packetPrefixBytes[3] - 2;

                                        if (fileStream.Length - fileStream.Position < remainingPacketBytes)
                                        {
                                            feedback($"File {binFile} does not have enough data for full packet. Skipping.");
                                            break;
                                        }

                                        byte[] packetBytes = binReader.ReadBytes(remainingPacketBytes);
                                        byte[] fullPacket = packetPrefixBytes.Concat(packetBytes).ToArray();

                                        double currentValue = conversionFunction(fullPacket);
                                        if (maxValue == null || currentValue > maxValue)
                                            maxValue = currentValue;
                                    }

                                    if (maxValue.HasValue)
                                    {
                                        results.Add((binFile, maxValue.Value));
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                feedback($"Error processing file {binFile}: {ex.Message}");
                            }

                            int processedFilesCount = results.Count;
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
                    if (sortedResults.Count > 0)
                    {
                        var resultText = $"{Environment.NewLine}Max {columnName} for each file:" + Environment.NewLine +
                            string.Join(Environment.NewLine, sortedResults.Select(r =>
                            {
                                var fullPath = Path.GetDirectoryName(r.FileName);
                                var directoryParts = fullPath.Split(Path.DirectorySeparatorChar).ToList();

                                var relevantDirectories = directoryParts.Skip(Math.Max(0, directoryParts.Count - 2)).ToArray();
                                var fileName = Path.GetFileName(r.FileName);
                                var resultPath = relevantDirectories.Length > 0
                                    ? string.Join("/", relevantDirectories) + "/" + fileName
                                    : fileName;
                                return resultPath + $" - {r.MaxValue}";
                            }));

                        feedback(resultText);

                        LogSummary("Analysis summary for multi-file analysis:", wrlFiles.Length, successfulConversions.Count, failedConversions.Count);

                        //feedback(Environment.NewLine + "Failed to convert:" + Environment.NewLine + string.Join(Environment.NewLine, failedConversions) + Environment.NewLine);
                        if (failedConversions?.Count > 0) feedback($"{Environment.NewLine}Failed to convert:{Environment.NewLine}{string.Join(Environment.NewLine, failedConversions)}{Environment.NewLine}");

                    }
                    else
                    {
                        feedback("No results found.");
                    }

                    DeleteBin(folderPath);

                    progressBar.Visible = false;
                    progressLabel.Visible = false;
                }
            }

            DateTime endTime = DateTime.Now;
            TimeSpan duration = endTime - startTime;
            string durationFormatted = duration.ToString(@"mm\:ss\.ff");
            log($"{LogPrefix.Prefix}Analysis completed in {durationFormatted}.");
            LogSeparator();
            UpdateProgressLabel("Analysis finished.");
            progressBar.Visible = false;
            progressLabel.Visible = false;
        }
        void LogSummary(string summaryTitle, int totalProcessed, int successful, int failed, string duration = null)
        {
            LogSeparator();
            log($"{LogPrefix.Prefix}{summaryTitle}");
            log($"{LogPrefix.Prefix}Total files processed: {totalProcessed}");
            log($"{LogPrefix.Prefix}Successfully processed: {successful}");
            log($"{LogPrefix.Prefix}Failed to process: {failed}");
            if (duration != null) { log($"{LogPrefix.Prefix}Total processing time: {duration}"); }
        }
        private void Repair_Click(object sender, EventArgs e)
        {
            byte[] prefix = logs.PacketPattern;

            // Check if packet pattern exists
            if (prefix == null || prefix.Length == 0)
            {
                MessageBox.Show("Please load a valid file to obtain the packet pattern.", "Missing Packet Pattern", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Exit if there is no packet pattern
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "C:\\";
                openFileDialog.Filter = "WRL files (*.wrl)|*.wrl|All files (*.*)|*.*";
                openFileDialog.Title = "Select WRL file to repair";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFileName = openFileDialog.FileName;
                    RepairWRL(selectedFileName);
                }
            }
        }
        private async void ExportDirToExcel()
        {
            double divisor = GetAFRDivisor();
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
                    int successfulCount = 0;
                    int failedCount = 0;

                    var stopwatch = new System.Diagnostics.Stopwatch();
                    stopwatch.Start();

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
                                    failedCount++;
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
                                        var exportLine = WoolichMT09Log.getCSV(packet.Value, packet.Key, exportItem.PacketFormat, this.presumedStaticColumns, new List<int>(), divisor);
                                        outputFile.WriteLine(string.Join(delimiter.ToString(), exportLine.Split(','))); // Dynamic delimiter
                                    }
                                }

                                processedFiles++;
                                successfulCount++;

                                Invoke(new Action(() => UpdateProgressLabel($"Processing file {processedFiles}/{totalFiles}")));
                                int progressPercentage = (processedFiles * 100) / totalFiles;
                                Invoke(new Action(() => progressBar.Value = Math.Min(progressPercentage, progressBar.Maximum)));
                                ExcelMinimal(exportFileName);
                            }
                            catch (Exception ex)
                            {
                                Invoke(new Action(() => log($"{LogPrefix.Prefix}Failed to process file {filePath}: {ex.Message}")));
                                failedCount++;
                            }
                        }

                        stopwatch.Stop();

                        // Calculate time in minutes and seconds
                        var elapsed = stopwatch.Elapsed;
                        string duration = $"{(int)elapsed.TotalMinutes} minutes {elapsed.Seconds} seconds";

                        // Logging results
                        LogSummary($"Export to {exportFormat.ToUpper()} summary for multi-file:", totalFiles, successfulCount, failedCount, duration);

                        Invoke(new Action(() => UpdateProgressLabel("Conversion completed successfully.")));
                        DeleteBin(rootFolderPath);
                        LogSeparator();
                        ClearBoxAndPackets();
                    });
                }
            }

            progressBar.Visible = false;
            progressLabel.Visible = false;
            UpdateProgressLabel("Export finished.");
            lblFileName.Text = "None";
            lblDirName.Text = "None";
            lblTotalPacketsCount.Text = "0";
        }
        private void ExportToAutoTune()
        {
            if (!IsFileLoaded())
            {
                return;
            }

            WoolichMT09Log exportItem = logs;

            // Check if the packet format is supported
            if (exportItem.PacketFormat != 0x01)
            {
                MessageBox.Show("This bike's file cannot be adjusted by this software yet.");
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
            string outputFileNameWithExtension;

            // Determine file name based on cmbATFileName selection
            if (cmbATFileName.SelectedItem.ToString() == "Binary")
            {
                outputFileNameWithExtension = Path.Combine(directoryPath, $"{baseFileName}_{binaryString}_AT.WRL");
            }
            else if (cmbATFileName.SelectedItem.ToString() == "Default")
            {
                outputFileNameWithExtension = Path.Combine(directoryPath, $"{baseFileName}_AT.WRL");
            }
            else
            {
                MessageBox.Show("Invalid file naming option selected.");
                return;
            }

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
        private void ExportDirToAutoTune()
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string directoryPath = folderDialog.SelectedPath;
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
                                ExportToAutoTune();
                                successfulCount++;
                            }
                            else
                            {
                                failedCount++;
                            }
                        }

                        stopwatch.Stop();

                        // Calculate time in minutes and seconds
                        var elapsed = stopwatch.Elapsed;
                        string duration = $"{(int)elapsed.TotalMinutes} minutes {elapsed.Seconds} seconds";

                        LogSummary("Autotune Export summary for multi-file:", wrlFiles.Length, successfulCount, failedCount, duration);

                        DeleteBin(directoryPath);
                        LogSeparator();
                        ClearBoxAndPackets();
                        lblFileName.Text = "None";
                        lblDirName.Text = "None";
                        OpenFileName = "";
                        lblTotalPacketsCount.Text = "0";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while processing the files: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void UpdateExportSettings()
        {
            // Define the options to display based on the selected index in cmbExportType
            string[][] options = new string[][]
            {
        new string[] { "csv", "tsv" },       // For "Export" (index 0)
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
                if (selectedTypeIndex == 0 || selectedTypeIndex == 1) // "Export" or "Analysis Only"
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
            cmbExportFormat.Enabled = (selectedTypeIndex < 2 || selectedTypeIndex == 3); // Enable for "Export", "Analysis Only", or "Autotune"

            // Make btnExport active when mode is set to 1 (Directory) or valid conditions
            btnExport.Enabled = cmbExportMode.SelectedIndex != -1 && cmbExportType.SelectedIndex != -1;
        }
        private void Export_Click(object sender, EventArgs e)
        {
            int exportTypeIndex = cmbExportType.SelectedIndex;
            int exportFormatIndex = cmbExportFormat.SelectedIndex;
            int exportModeIndex = cmbExportMode.SelectedIndex;

            // Dictionary mapping (mode, type, format) to corresponding export functions
            var exportActions = new Dictionary<(int mode, int type, int format), Action>
    {
        {(0, 0, 0), ExportToExcel},  // File, Export, csv
        {(0, 0, 1), ExportToExcel},  // File, Export, tsv
        {(0, 1, 0), ExportToExcel},  // File, Analysis Only, csv
        {(0, 1, 1), ExportToExcel},  // File, Analysis Only, tsv
        {(0, 1, 2), ExportColumnToWRL}, // File, Analysis Only, wrl
        {(0, 2, 0), ExportToCRC},  // File, CRC, wrl
        {(0, 3, 0), ExportToAutoTune}, // File, Autotune, wrl
        {(1, 0, 0), ExportDirToExcel}, // Directory, Export, csv
        {(1, 0, 1), ExportDirToExcel}, // Directory, Export, tsv
        {(1, 3, 0), ExportDirToAutoTune} // Directory, Autotune, wrl
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
        private void DeleteBin(string directoryPath)
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

                    if (File.Exists(binOutputFileName))
                    {
                        totalCount++;

                        try
                        {
                            File.Delete(binOutputFileName);
                            deletedCount++;
                        }
                        catch (Exception)
                        {
                            failedCount++;
                        }
                    }
                    else
                    {
                    }
                }

                log($"{LogPrefix.Prefix}Total BIN files checked: {totalCount}");
                log($"{LogPrefix.Prefix}Successfully deleted: {deletedCount}");
                if (failedCount > 0) log($"{LogPrefix.Prefix}Failed to delete: {failedCount}");
            }
            else
            {
                log($"{LogPrefix.Prefix}Deletion not enabled. Setting is on: {cmbBinDelete.Text}"); // Debug log
            }
        }
        private List<int> FindPattern(byte[] data, byte[] pattern, int length)
        {
            List<int> offsets = new List<int>();
            int patternLength = pattern.Length;
            int offset = 0;

            while (offset <= (data.Length - patternLength))
            {
                bool match = true;
                for (int i = 0; i < patternLength; i++)
                {
                    if (data[offset + i] != pattern[i])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    offsets.Add(offset);
                    offset += length;
                }
                else
                {
                    offset += 1;
                }
            }

            return offsets;
        }
        private byte[] FixPackets(byte[] data, List<int> offsets, int prefixLength, int interval, out bool needsRepair)
        {
            needsRepair = false;

            using (MemoryStream recoveredDataStream = new MemoryStream())
            {
                int headerLength = logs.PrimaryHeaderLength;
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
        private void ConvertWRLToBIN(string wrlFileName, string binFileName, List<string> successfulConversions, List<string> failedConversions)
        {
            bool conversionSuccessful = false;

            try
            {
                if (!File.Exists(wrlFileName))
                {
                    feedback($"Error: File Not Found - {Path.GetFileName(wrlFileName)}");
                    failedConversions.Add(wrlFileName);
                    return;
                }

                conversionSuccessful = LoadAndProcessFile(wrlFileName, binFileName, failedConversions);

                if (conversionSuccessful)
                {
                    successfulConversions.Add(binFileName);
                    //log($"{LogPrefix.Prefix}Conversion successful: {binFileName}");
                }
            }
            catch (Exception ex)
            {
                feedback($"Error converting file: {Path.GetFileName(wrlFileName)}. Exception: {ex.Message}");
                failedConversions.Add(wrlFileName);

                if (File.Exists(binFileName))
                {
                    try
                    {
                        File.Delete(binFileName);
                    }
                    catch (Exception deleteEx)
                    {
                        feedback($"Failed to delete corrupted BIN file {binFileName}: {deleteEx.Message}");
                    }
                }
            }
        }
        private bool LoadFile(string filename)
        {
            try
            {
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

                return LoadAndProcessFile(filename, binOutputFileName);
            }
            catch (Exception ex)
            {
                log($"An unexpected error occurred: {ex.Message}");
                return false;
            }
        }
        private void RepairWRL(string inputFileName)
        {
            string directory = Path.GetDirectoryName(inputFileName);
            string outputFileName = Path.Combine(directory, Path.GetFileNameWithoutExtension(inputFileName) + "_fixed.WRL");

            try
            {
                byte[] data = File.ReadAllBytes(inputFileName);
                log($"{LogPrefix.Prefix}Loading file: " + Path.GetFileName(inputFileName));
                log($"{LogPrefix.Prefix}Read data from file. Size: {data.Length} bytes."); // Log the size of the read data

                byte[] pattern = logs.PacketPattern;

                int patternLength = pattern.Length; // Length of the pattern
                int length = logs.PacketPattern[3] + 3; // Define an length for processing

                List<int> offsets = FindPattern(data, pattern, length);
                log($"{LogPrefix.Prefix}Total number of prefixes found: {offsets.Count}."); // Log the number of prefixes found

                // Check if any prefixes were found
                if (offsets.Count == 0)
                {
                    MessageBox.Show("No valid packet patterns found.\n\nThe file is either:\n- from different motorcycle model\n- too damaged to be repaired",
                        "Repair Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Exit if no prefixes are found
                }

                // Ask for confirmation about the motorcycle model
                DialogResult result = MessageBox.Show("The packet pattern is available.\n\nDo you confirm that the file you want to repair is from the same motorcycle model?",
                    "Confirm Motorcycle Model", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    log($"{LogPrefix.Prefix}File repair cancelled by the user."); // Log completion and file save information
                    return; // Exit if the user cancels
                }

                bool needsRepair;
                byte[] recoveredData = FixPackets(data, offsets, patternLength, length, out needsRepair);

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
        private void OpenFile_Click(object sender, EventArgs e)
        {
            try
            {
                openWRLFileDialog.Title = "Select WRL file to inspect";
                openWRLFileDialog.InitialDirectory = string.IsNullOrWhiteSpace(openWRLFileDialog.InitialDirectory)
                    ? logFolder ?? Directory.GetCurrentDirectory()
                    : openWRLFileDialog.InitialDirectory;

                openWRLFileDialog.Multiselect = false;

                openWRLFileDialog.Filter = "WRL files (*.wrl)|*.wrl|All files (*.*)|*.*";

                if (openWRLFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                var filename = openWRLFileDialog.FileNames.FirstOrDefault();
                OpenFileName = filename;
                UpdateFormsStates();

                if (!File.Exists(filename))
                {
                    lblFileName.Text = "Error: File Not Found";
                    return;
                }

                logFolder = Path.GetDirectoryName(filename);
                ClearBoxAndPackets();

                //log($"Attempting to load file: {filename}");

                if (LoadFile(filename))
                {
                    string binOutputFileName = Path.Combine(logFolder, Path.GetFileNameWithoutExtension(filename) + ".bin");

                    if (logs.GetPacketCount() == 0)
                    {
                        ConvertWRLToBIN(filename, binOutputFileName, new List<string>(), new List<string>());
                    }
                    else
                    {
                        //log("Packets already processed. Skipping conversion.");
                    }

                    lblFileName.Text = Path.GetFileName(filename);
                    lblDirName.Text = Path.GetDirectoryName(filename);
                    lblTotalPacketsCount.Text = logs.GetPacketCount().ToString();
                    cmbExportType.SelectedIndex = 0;
                }
                else
                {
                    //log("LoadFile returned false, indicating a potential file error.");
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
                                log("Failed to load the repaired file.");
                                MessageBox.Show("Repair operation failed or was not needed. The file could not be opened.", "Repair Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                string binOutputFileName = Path.Combine(logFolder, Path.GetFileNameWithoutExtension(repairedFile) + ".bin");

                                if (logs.GetPacketCount() == 0)
                                {
                                    ConvertWRLToBIN(repairedFile, binOutputFileName, new List<string>(), new List<string>());
                                }
                                else
                                {
                                    //log("Packets already processed after repair. Skipping conversion.");
                                }
                            }
                        }
                        else
                        {
                            log("Repaired file not found after attempting repair.");
                            MessageBox.Show("Repair operation was not successful. The repaired file does not exist.", "Repair Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log($"An unexpected error occurred: {ex.Message}"); // log the exception message
                MessageBox.Show($"An unexpected error occurred: {ex.Message}\n\n Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void ExcelMinimal(string filePath)
        {
            // Check if the file exists
            if (!File.Exists(filePath))
            {
                log($"Error: The file does not exist: {filePath}");
                return;
            }

            // Determine the separator based on the file extension
            char separator = Path.GetExtension(filePath).Equals(".csv", StringComparison.OrdinalIgnoreCase) ? ',' : '\t';

            // Read all lines from the file
            var lines = File.ReadAllLines(filePath).ToList();

            // Check the value of cmbTextFilter
            if (cmbTextFilter.SelectedIndex == 1) // Assuming '1' is the index where no operations are performed
            {
                //log("Info: No operations performed as cmbTextFilter is set to 1.");
                return;
            }

            // Columns to remove (indexes start from 1) - Yamaha  for time being only
            var columnsToRemove = new List<int> { 4, 13, 14, 15, 16, 18, 21, 23, 27, 28, 31, 32, 33, 35, 36, 37, 38, 39, 40, 41 };

            // Remove headers and corresponding data
            var processedLines = new List<string>();
            foreach (var line in lines)
            {
                var columns = line.Split(separator).ToList();

                // Remove specified columns
                for (int i = columnsToRemove.Count - 1; i >= 0; i--)
                {
                    int columnIndex = columnsToRemove[i] - 1; // Change to 0-indexed
                    if (columnIndex < columns.Count)
                    {
                        columns.RemoveAt(columnIndex);
                    }
                }

                processedLines.Add(string.Join(separator.ToString(), columns));
            }

            // Save the processed data to a new file
            //string newFilePath = Path.ChangeExtension(filePath, "_processed" + Path.GetExtension(filePath));
            File.WriteAllLines(filePath, processedLines);
            //File.WriteAllLines(newFilePath, processedLines);

            //log($"Info: Processed the file. Saved as: {newFilePath}");
        }
        private bool LoadAndProcessFile(string filename, string binOutputFileName, List<string> failedConversions = null)
        {
            try
            {
                logs.ClearPackets();
                Array.Clear(logs.PrimaryHeaderData, 0, logs.PrimaryHeaderData.Length);
                Array.Clear(logs.SecondaryHeaderData, 0, logs.SecondaryHeaderData.Length);

                var watch = System.Diagnostics.Stopwatch.StartNew();

                using (var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, 4194304, FileOptions.SequentialScan))
                {
                    logs.PrimaryHeaderData = new byte[logs.PrimaryHeaderLength];
                    fileStream.Read(logs.PrimaryHeaderData, 0, logs.PrimaryHeaderLength);
                    long currentOffset = logs.PrimaryHeaderLength;

                    log($"{LogPrefix.Prefix}Loading file: " + Path.GetFileName(filename));
                    log($"{LogPrefix.Prefix}Primary header read. Length: {logs.PrimaryHeaderLength} bytes.");

                    byte[] validationBytes = new byte[logs.PacketPrefixLength];
                    fileStream.Read(validationBytes, 0, logs.PacketPrefixLength);
                    currentOffset += logs.PacketPrefixLength;

                    bool secondHeaderDetected = validationBytes.All(b => b == 0x00) || validationBytes.All(b => b == 0xFF);

                    if (secondHeaderDetected)
                    {
                        fileStream.Seek(-logs.PacketPrefixLength, SeekOrigin.Current);
                        logs.SecondaryHeaderData = new byte[logs.SecondaryHeaderLength];
                        fileStream.Read(logs.SecondaryHeaderData, 0, logs.SecondaryHeaderLength);
                        currentOffset += logs.SecondaryHeaderLength;
                        log($"{LogPrefix.Prefix}Secondary header read. Length: {logs.SecondaryHeaderLength} bytes.");
                    }
                    else
                    {
                        fileStream.Seek(-logs.PacketPrefixLength, SeekOrigin.Current);
                        currentOffset -= logs.PacketPrefixLength;
                        log($"{LogPrefix.Prefix}No second header detected.");
                    }

                    int packetCount = 0;

                    while (true)
                    {
                        byte[] currentPacketPrefixBytes = new byte[logs.PacketPrefixLength];
                        int bytesRead = fileStream.Read(currentPacketPrefixBytes, 0, logs.PacketPrefixLength);

                        if (bytesRead < logs.PacketPrefixLength)
                            break;

                        bool isHeaderValid = true;
                        for (int i = 0; i < logs.PacketPattern.Length; i++)
                        {
                            if (i >= currentPacketPrefixBytes.Length || currentPacketPrefixBytes[i] != logs.PacketPattern[i])
                            {
                                isHeaderValid = false;
                                break;
                            }
                        }

                        if (isHeaderValid)
                        {
                            if (logs.PacketPattern == null || logs.PacketPattern.Length == 0)
                            {
                                logs.PacketPattern = currentPacketPrefixBytes.Take(5).ToArray();
                                log($"{LogPrefix.Prefix}Packet pattern saved: {BitConverter.ToString(logs.PacketPattern)}");
                            }

                            int remainingPacketBytes = currentPacketPrefixBytes[3] - 2;
                            if (remainingPacketBytes < 0)
                            {
                                log($"{LogPrefix.Prefix}Invalid remaining packet length: {remainingPacketBytes}. Skipping.");
                                continue;
                            }

                            byte[] packetBytes = new byte[remainingPacketBytes];
                            bytesRead = fileStream.Read(packetBytes, 0, remainingPacketBytes);

                            if (bytesRead < remainingPacketBytes)
                                break;

                            int totalPacketLength = currentPacketPrefixBytes[3] + 3;
                            byte[] fullPacket = new byte[totalPacketLength];
                            Buffer.BlockCopy(currentPacketPrefixBytes, 0, fullPacket, 0, currentPacketPrefixBytes.Length);
                            Buffer.BlockCopy(packetBytes, 0, fullPacket, currentPacketPrefixBytes.Length, packetBytes.Length);
                            logs.AddPacket(fullPacket, totalPacketLength, currentPacketPrefixBytes[4]);
                            packetCount++;
                        }
                        else
                        {
                            long currentPosition = fileStream.Position;
                            if (currentPosition < fileStream.Length)
                            {
                                fileStream.Seek(1, SeekOrigin.Current);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    log($"{LogPrefix.Prefix}Found {packetCount} packets.");
                    watch.Stop();
                    log($"{LogPrefix.Prefix}Processing time: {watch.ElapsedMilliseconds} ms");

                    if (cmbBinDelete.SelectedIndex != 2)
                    {
                        using (var binFileStream = new FileStream(binOutputFileName, FileMode.Create, FileAccess.Write, FileShare.None, 262144))
                        using (var binWriter = new BinaryWriter(binFileStream))
                        {
                            foreach (var packet in logs.GetPackets())
                            {
                                binWriter.Write(packet.Value);
                            }
                            log($"{LogPrefix.Prefix}Bin file created.");
                        }
                        return true;
                    }

                    return true;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                log($"{LogPrefix.Prefix}File is corrupted: {filename}.");
                failedConversions?.Add(filename);
                return false;
            }
            catch (Exception ex)
            {
                log($"{LogPrefix.Prefix}An unexpected error occurred with file {filename}: {ex.Message}.");
                failedConversions?.Add(filename);
                return false;
            }
        }
        private async void ExportToExcel()
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
                        MessageBox.Show("Please provide a valid column number for analysis", "Invalid column number.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Please provide a column number for analysis", "No Column Number.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            if (exportItem.GetPacketCount() == 0)
            {
                MessageBox.Show("No packets available for export.", "No Data Available", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            double divisor = double.TryParse(AFRdivisor.Text, out double parsedDivisor) && parsedDivisor >= 5 && parsedDivisor <= 20
                             ? parsedDivisor : 10; // Default divisor if invalid input

            await Task.Run(() =>
            {
                int packetCount = exportItem.GetPacketCount();
                List<int> combinedCols = new List<int>();
                StringBuilder sb = new StringBuilder(); // StringBuilder for buffering output

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
                    // Prepare the header
                    string exportHeader = exportItem.GetHeader(this.presumedStaticColumns, combinedCols);
                    sb.AppendLine(exportHeader); // Append header to StringBuilder

                    var packets = exportItem.GetPackets();
                    int totalPackets = packets.Count;
                    int processedPackets = 0;
                    int updateFrequency = Math.Max(1, totalPackets / 10); // Update every 10% packets

                    foreach (var packet in packets)
                    {
                        var exportLine = WoolichMT09Log.getCSV(packet.Value, packet.Key, exportItem.PacketFormat, this.presumedStaticColumns, combinedCols, divisor);
                        sb.AppendLine(string.Join(delimiter.ToString(), exportLine.Split(',')));

                        processedPackets++;

                        // Update progress
                        if (processedPackets % updateFrequency == 0)
                        {
                            int progressPercentage = (processedPackets * 100) / totalPackets;
                            Invoke(new Action(() => progressBar.Value = Math.Min(progressPercentage, progressBar.Maximum)));
                            Invoke(new Action(() => UpdateProgressLabel($"Exporting... {progressPercentage}% completed")));
                        }

                        // Limit output for non-MT09 formats if count exceeds 100,000
                        if (processedPackets > 100000 && exportItem.PacketFormat != 0x01)
                        {
                            break;
                        }
                    }

                    // Write all buffered data to the file at once
                    using (StreamWriter outputFile = new StreamWriter(exportFileName))
                    {
                        outputFile.Write(sb.ToString());
                    }

                    Invoke(new Action(() => log($"{LogPrefix.Prefix}Data exported to {exportFormat.ToUpper()} format: " + Path.GetFileName(exportFileName))));
                    ExcelMinimal(exportFileName);
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


    }
}

