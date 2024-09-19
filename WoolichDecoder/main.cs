﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WoolichDecoder.Models;
using WoolichDecoder.Settings;
using static System.Resources.ResXFileRef;

namespace WoolichDecoder
{
    public partial class WoolichFileDecoderForm : Form
    {
        public static class LogPrefix
        {
            // Date and Time format
            private static readonly string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

            // Generate prefix with date, time and string
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
            cmbExportMode.SelectedIndex = 0;
            cmbExportType.SelectedIndexChanged += cmbExportType_SelectedIndexChanged;
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

        private void cmbExportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update cmbExportFormat options based on the selected value in cmbExportType
            UpdateExportFormatOptions();
        }

        private void UpdateExportFormatOptions()
        {
            // Define the options to display based on the selected index
            string[][] options = new string[][]
            {
        new string[] { "csv", "tsv" },   // For "Full File"
        new string[] { "csv", "tsv", "wrl" }, // For "Analysis Only"
        new string[] { "wrl" }          // For "CRC"
            };

            // Clear existing items in cmbExportFormat
            cmbExportFormat.Items.Clear();

            // Set items based on the selected index
            int selectedIndex = cmbExportType.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < options.Length)
            {
                cmbExportFormat.Items.AddRange(options[selectedIndex]);
                cmbExportFormat.SelectedIndex = 0; // Default to the first option

                // Disable the ComboBox if "CRC" is selected
                cmbExportFormat.Enabled = (selectedIndex != 2);
            }
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
            // Clear packets in exportLogs
            exportLogs.ClearPackets();
            // Reset the packet count (if displayed in a label)
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
            needsRepair = false; // Assume no repair needed by default

            using (MemoryStream recoveredDataStream = new MemoryStream())
            {
                int headerLength = 353;
                byte[] header = new byte[headerLength];
                Array.Copy(data, 0, header, 0, headerLength);

                // Write the header to the stream initially
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

                    if (currentOffset >= headerLength) // Ensure we only process packets after the header
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

                // Return the recovered data with the header included if gaps were found
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
                        return i; // Return the position where the pattern is found
                    }
                }
            }
            return -1; // Pattern not found
        }
        private void ClearBoxAndPackets()
        {
            // Clear the text box
            txtBreakOnChange.Text = string.Empty;

            // Clear packets in exportLogs
            exportLogs.ClearPackets();

            // Reset the packet count (if displayed in a label)
            lblExportPacketsCount.Text = "0";

            // Optionally, clear the export file name label
            lblExportFilename.Text = string.Empty;

        }
        private void DisplayLegend()
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
            // Check if cmbLogsLocation has a valid selection
            if (cmbLogsLocation.SelectedIndex == 0) // Assuming 0 is the application's directory
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
            else if (cmbLogsLocation.SelectedIndex == 1) // Assuming 1 is the open file directory
            {
                return lblDirName.Text; // Use the directory of the open file
            }
            else
            {
                // Default to application's directory if no valid selection is made
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }
        private void feedback(string fbData)
        {
            // Get the directory path
            string directoryPath = GetDirectoryPath();

            // Ensure the directory path is valid
            if (string.IsNullOrEmpty(directoryPath))
            {
                return; // Exit the function if the directory path is invalid
            }

            // Ensure the directory exists or try to create it
            if (!Directory.Exists(directoryPath))
            {
                try
                {
                    Directory.CreateDirectory(directoryPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while creating the directory: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Exit the function if directory creation fails
                }
            }

            // Append the feedback data to the TextBox
            txtFeedback.AppendText(fbData + Environment.NewLine);

            // Define the path to the feedback file
            string feedbackFilePath = Path.Combine(directoryPath, "feedback.txt");

            try
            {
                // Write the feedback data to the file
                File.AppendAllText(feedbackFilePath, $"{fbData}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                // Display an error message if there is a problem writing to the file
                MessageBox.Show($"An error occurred while saving the feedback: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void log(string logData)
        {
            // Get the directory path
            string directoryPath = GetDirectoryPath();

            // Ensure the directory path is valid
            if (string.IsNullOrEmpty(directoryPath))
            {
                return; // Exit the function if the directory path is invalid
            }

            // Ensure the directory exists or try to create it
            if (!Directory.Exists(directoryPath))
            {
                try
                {
                    Directory.CreateDirectory(directoryPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while creating the directory: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Exit the function if directory creation fails
                }
            }

            // Append the log data to the TextBox
            txtLogging.AppendText(logData + Environment.NewLine);

            // Define the path to the log file
            string logFilePath = Path.Combine(directoryPath, "log.txt");

            try
            {
                // Write the log data to the file
                File.AppendAllText(logFilePath, $"{logData}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                // Display an error message if there is a problem writing to the file
                MessageBox.Show($"An error occurred while saving the log: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnExportTargetColumn_Click(object sender, EventArgs e)
        {
            if (!IsFileLoaded())
                return;

            WoolichMT09Log exportItem = null;

            // Check if the selected export type is supported
            if (cmbExportType.SelectedIndex == 0)
            {
                // "Export Full File"
                exportItem = logs;

                // Inform the user that only "Export Analysis Only" is supported
                MessageBox.Show(
                    "Export Analysis Only is supported at the moment.",
                    "Export Not Supported",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return; // Exit the method if the export type is not supported
            }
            else
            {
                // "Export Analysis Only"
                exportItem = exportLogs;
            }

            // Check if there are any packets to process
            if (exportItem == null || !exportItem.GetPackets().Any())
            {
                // Show a message prompting the user to select and analyze data
                MessageBox.Show(
                    "No packets available for export.",
                    "No Data Available",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return; // Exit the method if no packets are available
            }

            // Check if a valid column number is provided
            if (string.IsNullOrWhiteSpace(txtBreakOnChange.Text))
            {
                // Prompt the user to enter a column number
                MessageBox.Show(
                    "Please enter a column number for export analysis.",
                    "Column Number Required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return; // Exit the method if no column number is provided
            }

            string outputFileNameWithExtension = "";

            try
            {
                // Extract the base file name from lblFileName
                string baseFileName = Path.GetFileNameWithoutExtension(lblFileName.Text.Trim());
                string directoryPath = lblDirName.Text.Trim();

                // Get the column number from the text box
                var columnToExport = int.Parse(txtBreakOnChange.Text.Trim());

                // Construct the output file name
                //outputFileNameWithExtension = $"{baseFileName}_C{columnToExport}.WRL";
                outputFileNameWithExtension = Path.Combine(directoryPath, $"{baseFileName}_C{columnToExport}.WRL");


                // Write data to a binary file
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
                // Handle exceptions and provide feedback
                MessageBox.Show($"Error during file export: {ex.Message}");
            }

            // Log the successful export
            log($"{LogPrefix.Prefix}Analysis WRL File saved as: " + Path.GetFileName(outputFileNameWithExtension));
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
        private async void btnExportCSV_Click(object sender, EventArgs e)
        {
            // Check if a file is loaded
            if (!IsFileLoaded())
            {
                return;
            }

            WoolichMT09Log exportItem = null;

            // Get the directory path from lblDirName
            string directoryPath = lblDirName.Text.Trim();

            // Check if the directory path is valid
            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                MessageBox.Show("Directory path is not defined.");
                return;
            }

            // Get the file name from lblFileName and remove the extension
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(lblFileName.Text.Trim());

            // Check if the file name is valid
            if (string.IsNullOrWhiteSpace(fileNameWithoutExtension))
            {
                MessageBox.Show("File name is not defined.");
                return;
            }

            // Set the CSV file name
            var csvFileName = Path.Combine(directoryPath, fileNameWithoutExtension + ".csv");

            // "Export Full File" (Index 0) or "Export Analysis Only" (Index 1)
            if (cmbExportType.SelectedIndex == 0)
            {
                // "Export Full File" option selected
                exportItem = logs;
            }
            else
            {
                // "Export Analysis Only" option selected
                exportItem = exportLogs;

                // Check if a valid column number is provided
                if (!string.IsNullOrWhiteSpace(txtBreakOnChange.Text))
                {
                    try
                    {
                        // Parse the column number from the text input
                        int columnNumber = int.Parse(txtBreakOnChange.Text.Trim());

                        // Modify the file name to include the column number for analysis
                        csvFileName = Path.Combine(directoryPath, fileNameWithoutExtension + $"_C{columnNumber}.csv");
                    }
                    catch (Exception ex)
                    {
                        // Handle parsing errors if the input is not a valid integer
                        MessageBox.Show("Invalid column number. Please provide a valid column number for analysis. " + ex.Message);
                        return; // Exit function if the column number is invalid
                    }
                }
                else
                {
                    // No column number provided, show a warning message
                    MessageBox.Show("No column number provided for analysis.");
                    return;
                }
            }

            // Check if there are any packets to export
            if (exportItem.GetPacketCount() == 0)
            {
                // Display a message if there are no packets to export
                MessageBox.Show("No packets available for export.");
                return;
            }

            // Log the file name for debugging purposes
            Console.WriteLine($"Exporting to file: {csvFileName}");

            // Check if the directory exists; if not, create it
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

            // Proceed with export
            progressBar.Visible = true;
            progressLabel.Visible = true;
            UpdateProgressLabel("Starting export...");

            // Run the export process asynchronously using Task.Run
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
                    using (StreamWriter outputFile = new StreamWriter(csvFileName))
                    {
                        // Write the header
                        string csvHeader = exportItem.GetHeader(this.presumedStaticColumns, combinedCols);
                        outputFile.WriteLine(csvHeader);

                        var packets = exportItem.GetPackets();
                        int totalPackets = packets.Count;
                        int processedPackets = 0;

                        // Write packet data to CSV
                        foreach (var packet in packets)
                        {
                            var exportLine = WoolichMT09Log.getCSV(packet.Value, packet.Key, exportItem.PacketFormat, this.presumedStaticColumns, combinedCols);
                            outputFile.WriteLine(exportLine);
                            outputFile.Flush();

                            // Update progress bar and label
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

                    // Log success and update UI
                    Invoke(new Action(() => log($"{LogPrefix.Prefix}Data exported to CSV format: " + Path.GetFileName(csvFileName))));
                    Invoke(new Action(() => UpdateProgressLabel("Export completed successfully.")));
                }
                catch (Exception ex)
                {
                    // Handle any errors during file writing
                    Invoke(new Action(() => MessageBox.Show($"An error occurred: {ex.Message}")));
                    Invoke(new Action(() => UpdateProgressLabel("Error occurred during export.")));
                }
                finally
                {
                    // Hide progress bar and reset labels
                    Invoke(new Action(() => UpdateProgressLabel("Export finished.")));
                    System.Threading.Thread.Sleep(3000);
                    Invoke(new Action(() => progressBar.Visible = false));
                    Invoke(new Action(() => progressLabel.Visible = false));
                }
            });
        }
        private void btnAutoTuneExport_Click(object sender, EventArgs e)
        {
            // Check if a file is loaded
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

            // Extract the base file name from lblFileName
            string baseFileName = Path.GetFileNameWithoutExtension(lblFileName.Text.Trim());
            string directoryPath = lblDirName.Text.Trim();

            // Create the output file name based on the extracted base file name and binary string
            string outputFileNameWithExtension = Path.Combine(directoryPath, $"{baseFileName}_{binaryString}_AT.WRL");


            try
            {
                // Write data to the output file
                using (BinaryWriter binWriter = new BinaryWriter(File.Open(outputFileNameWithExtension, FileMode.Create)))
                {
                    binWriter.Write(exportItem.PrimaryHeaderData);
                    binWriter.Write(exportItem.SecondaryHeaderData);

                    foreach (var packet in exportItem.GetPackets())
                    {
                        byte[] exportPackets = packet.Value.ToArray();
                        int diff = 0;

                        var outputGear = packet.Value.getGear();

                        // Apply filters based on selected options
                        if (outputGear == 2 && this.aTFCheckedListBox.CheckedItems.Contains(autoTuneFilterOptions[1]))
                        {
                            byte newOutputGearByte = (byte)(exportPackets[24] & (~0b00000111));
                            diff = diff + newOutputGearByte - outputGear;
                            outputGear = newOutputGearByte;
                            exportPackets[24] = newOutputGearByte;
                        }

                        int minRPM = int.Parse(this.minRPM.Text);  // Read and convert minimum RPM
                        int maxRPM = int.Parse(this.maxRPM.Text);  // Read and convert maximum RPM

                        if (outputGear == 1 && (packet.Value.getRPM() < minRPM || packet.Value.getRPM() > maxRPM) && this.aTFCheckedListBox.CheckedItems.Contains(autoTuneFilterOptions[4]))
                        {
                            byte newOutputGearByte = (byte)(exportPackets[24] & (~0b00000111));
                            diff = diff + newOutputGearByte - outputGear;
                            outputGear = newOutputGearByte;
                            exportPackets[24] = newOutputGearByte;
                        }

                        int rpmLimit = int.Parse(idleRPM.Text);  // Read and convert RPM limit

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

                // Log success message
                log($"{LogPrefix.Prefix}Autotune WRL File saved as: " + Path.GetFileName(outputFileNameWithExtension));
            }
            catch (Exception ex)
            {
                // Log any errors that occur during file saving
                log($"{LogPrefix.Prefix}Autotune WRL File saving error: {ex.Message}");
            }
        }
        private void btnExportCRCHack_Click(object sender, EventArgs e)
        {
            if (!IsFileLoaded())
                return;

            try
            {
                // Get size from textBox
                if (!int.TryParse(CRCsize.Text.Trim(), out int size))
                {
                    MessageBox.Show("Invalid size. Please enter a valid number.");
                    return;
                }

                // Extract the base file name from lblFilename (excluding any file extension)
                string baseFileName = Path.GetFileNameWithoutExtension(lblFileName.Text.Trim());
                string directoryPath = lblDirName.Text.Trim();


                // Generate the file name based on the size
                outputFileNameWithExtension = Path.Combine(directoryPath, $"{baseFileName}_CRC.{size}.WRL");


                WoolichMT09Log exportItem = logs;

                // Write data to a binary file
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

                // Log information about the saved file
                log($"{LogPrefix.Prefix}CRC saved as: " + Path.GetFileName(outputFileNameWithExtension));
            }
            catch (Exception ex)
            {
                // Log any error that occurs during the export process
                log($"Error while exporting CRC: {ex.Message}");
            }
        }
        private void WoolichFileDecoderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            userSettings.LogDirectory = this.logFolder;

            // save the user settings.
            userSettings.Save();
        }
        private void UpdateButtonStates()
        {
            bool isFileLoaded = IsFileLoaded();
            btnAnalyse.Enabled = isFileLoaded;
            btnAutoTuneExport.Enabled = isFileLoaded;
            btnExportCRCHack.Enabled = isFileLoaded;
            btnExportCSV.Enabled = isFileLoaded;
            btnExportTargetColumn.Enabled = isFileLoaded;
            cmbExportType.Enabled = isFileLoaded;
            aTFCheckedListBox.Enabled = isFileLoaded;
            idleRPM.Enabled = isFileLoaded;
            minRPM.Enabled = isFileLoaded;
            maxRPM.Enabled = isFileLoaded;
            lblExportPacketsCount.Enabled = isFileLoaded;
            txtBreakOnChange.Enabled = isFileLoaded;
            CRCsize.Enabled = isFileLoaded;
            txtFeedback.Enabled = isFileLoaded;
            txtLogging.Enabled = isFileLoaded;
            btnMulti.Enabled = isFileLoaded;
            cmbLogsLocation.Enabled = isFileLoaded;
            cmbExportFormat.Enabled = isFileLoaded;
        }
        private void btnAnalyse_Click(object sender, EventArgs e)
        {
            // Get the selected index from the ComboBox (cmbExportMode)
            int selectedIndex = cmbExportMode.SelectedIndex;

            // Check the index and call the corresponding function
            if (selectedIndex == 0)
            {
                if (!IsFileLoaded())
                    return;
                Analyse();  // Call Analyse function if index is 0
            }
            else if (selectedIndex == 1)
            {
                MultiAnalyse();  // Call MultiAnalyse function if index is 1
            }
        }
        private async void Analyse()
        {
            // Check if the textbox contains a valid column number
            if (string.IsNullOrWhiteSpace(txtBreakOnChange.Text))
            {
                // Display a message if the column number is empty and log it
                txtFeedback.Clear();
                MessageBox.Show("Column number is empty. Please enter a valid column number." + Environment.NewLine);
                DisplayLegend();
                return;
            }

            int columnNumber;
            try
            {
                // Try to parse the textbox input as an integer
                columnNumber = int.Parse(txtBreakOnChange.Text);
            }
            catch (Exception)
            {
                // If parsing fails, display an error message and log the exception
                MessageBox.Show("Invalid column number. Please enter a valid number.");
                txtFeedback.Clear();
                //log($"{LogPrefix.Prefix}Error parsing column number." + Environment.NewLine);
                ClearBoxAndPackets();   // Clear text box and packets
                DisplayLegend();        // Show legend for valid columns
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
                MessageBox.Show("Unsupported column number. Please enter a valid column number.");
                //log($"{LogPrefix.Prefix}Column not supported for analysis.");
                txtFeedback.Clear();
                ClearBoxAndPackets();  // Clear text box and packets
                DisplayLegend();        // Show legend for valid columns
                return;
            }

            // Proceed with analysis if column number is valid
            feedback($"{LogPrefix.Prefix}Monitoring changes on column: {columnNumber}");
            analysisColumn.Add(columnNumber);

            // Begin analysis for the specific column
            feedback($"{LogPrefix.Prefix}Analysing column {columnNumber}...");
            lblExportPacketsCount.Text = string.Empty;
            exportLogs.ClearPackets(); // Clear the export log
            DateTime startTime = DateTime.Now;
            log($"{LogPrefix.Prefix}Start Analysing...");

            StringBuilder feedbackBuffer = new StringBuilder();
            Func<byte[], double> conversionFunction = columnFunctions[columnNumber];

            double? previousValue = null;
            double? minValue = null;
            double? maxValue = null;

            // Get all packets from the logs for analysis
            var packets = logs.GetPackets();
            int totalPackets = packets.Count;
            int processedPackets = 0;

            // Show the progress bar and initialize it
            progressBar.Visible = true;
            progressLabel.Visible = true;
            progressBar.Value = 0;
            UpdateProgressLabel("Starting analysis...");

            // Run the analysis in a separate task to avoid blocking the UI
            await Task.Run(() =>
            {
                foreach (KeyValuePair<string, byte[]> packet in packets)
                {
                    try
                    {
                        // Perform conversion and get the current value for the column
                        double currentValue = conversionFunction(packet.Value);

                        // Initialize min and max values on the first packet
                        if (previousValue == null)
                        {
                            minValue = currentValue;
                            maxValue = currentValue;
                            previousValue = currentValue;
                            feedbackBuffer.AppendLine($"Initial value in column {columnNumber}: {currentValue}");
                            continue;
                        }

                        // Update min and max values as the analysis proceeds
                        if (currentValue > maxValue) maxValue = currentValue;
                        if (currentValue < minValue) minValue = currentValue;

                        // If the value changes, log the change and add packet to export
                        if (currentValue != previousValue)
                        {
                            feedbackBuffer.AppendLine($"Column {columnNumber} changed from {previousValue} to {currentValue}");
                            previousValue = currentValue;

                            // Export the packet if the column is being monitored
                            if (analysisColumn.Contains(columnNumber))
                            {
                                exportLogs.AddPacket(packet.Value, logs.PacketLength, logs.PacketFormat);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log any errors encountered during analysis
                        feedbackBuffer.AppendLine($"Error processing column {columnNumber}: {ex.Message}");
                    }

                    // Update the progress of the analysis
                    processedPackets++;
                    int progressPercentage = (processedPackets * 100) / totalPackets;

                    // Update progress bar and label in the UI
                    this.Invoke(new Action(() =>
                    {
                        progressBar.Value = Math.Min(progressPercentage, progressBar.Maximum);
                        UpdateProgressLabel($"Analyzing... {progressPercentage}% completed");
                    }));

                    // Allow the UI to update during the analysis loop
                    Application.DoEvents();
                }
            });

            // Once analysis is completed, log min and max values
            feedbackBuffer.AppendLine($"Column {columnNumber} min value: {minValue}, max value: {maxValue}");
            feedback(feedbackBuffer.ToString());

            // Continue with further operations, such as exporting the results
            feedback($"{LogPrefix.Prefix}Exporting packets...");
            lblExportPacketsCount.Text = $"{exportLogs.GetPacketCount()}";

            // Finalize and log completion of the analysis
            //log($"{LogPrefix.Prefix}Analysis completed.");

            DateTime endTime = DateTime.Now;
            TimeSpan duration = endTime - startTime;
            string durationFormatted = duration.ToString(@"mm\:ss\.ff");
            log($"{LogPrefix.Prefix}Analysis completed in {durationFormatted}.");
            UpdateProgressLabel("Analysis finished.");
            System.Threading.Thread.Sleep(3000); // Allow time for user to see completion status
            progressBar.Visible = false;
            progressLabel.Visible = false; // Hide progress UI elements
        }
        private async void MultiAnalyse()
        //private async void btnMultiAnalyse_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBreakOnChange.Text))
            {
                txtFeedback.Clear();
                MessageBox.Show("Column number is empty. Please enter a valid column number." + Environment.NewLine);
                DisplayLegend();
 
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
                DisplayLegend();
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
                DisplayLegend();
                
                
                return;
            }

            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() != DialogResult.OK)
                    return;

                string folderPath = folderDialog.SelectedPath;
                lblDirName.Text = folderPath;

                // Step 1: Find WRL files and inform the user
                var wrlFiles = Directory.GetFiles(folderPath, "*.wrl", SearchOption.AllDirectories);
                log($"{LogPrefix.Prefix}Total WRL files found: {wrlFiles.Length}");

                // Step 2: Convert WRL files to BIN and inform the user about the conversion progress
                var successfulConversions = new List<string>();
                var failedConversions = new List<string>();

                DateTime startTime = DateTime.Now;
                log($"{LogPrefix.Prefix}Starting Multi File Conversion...");

                // Initialize progress bar
                progressBar.Visible = true;
                progressLabel.Visible = true;
                progressBar.Maximum = wrlFiles.Length; // Set maximum value for progress bar
                progressBar.Value = 0;
                UpdateProgressLabel("Converting files...");

                foreach (var wrlFile in wrlFiles)
                {
                    string binFile = Path.Combine(Path.GetDirectoryName(wrlFile), Path.GetFileNameWithoutExtension(wrlFile) + ".bin");
                    ConvertWRLToBIN(wrlFile, binFile, successfulConversions, failedConversions);

                    // Update progress bar
                    this.Invoke(new Action(() =>
                    {
                        progressBar.Value++;
                        UpdateProgressLabel($"Converting files... {progressBar.Value}/{progressBar.Maximum} files processed");
                    }));

                    Application.DoEvents(); // Ensure the UI updates
                }

                int totalFiles = wrlFiles.Length;
                int convertedFiles = successfulConversions.Count;
                int failedFiles = failedConversions.Count;

                log($"{LogPrefix.Prefix}Conversion completed.");
                log($"{LogPrefix.Prefix}Successfully converted {convertedFiles} files.");
                log($"{LogPrefix.Prefix}{failedFiles} files failed.");



                // Step 3: Analyze converted files
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
                    int processedFilesCount = 0; // Counter for processed files

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

                        // Update the progress bar only if the percentage has changed
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

                // Provide final feedback and results
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
                        //feedback($"Header marker was found at position {position} bytes.");

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

                    conversionSuccessful = true; // Set the flag to true if no exceptions were thrown
                }

                if (conversionSuccessful)
                {
                    feedback($"Converted file: {Path.GetFileName(wrlFileName)}.");
                    // Add to successful conversions
                    successfulConversions.Add(binFileName);
                }
            }
            catch (Exception)
            {
                feedback($"ERROR converting file: {Path.GetFileName(wrlFileName)}");
                // Add to failed conversions
                failedConversions.Add(wrlFileName);

                // If the BIN file was created before the error occurred, attempt to delete it
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
        private void RepairWRLFile(string inputFileName)
        {
            // Get the directory of the selected file
            string directory = Path.GetDirectoryName(inputFileName);

            // Define the output file name by appending "_fixed" to the original file name
            string outputFileName = Path.Combine(directory, Path.GetFileNameWithoutExtension(inputFileName) + "_fixed.WRL");

            try
            {
                // Read all bytes from the selected file
                byte[] data = File.ReadAllBytes(inputFileName);
                log($"{LogPrefix.Prefix}Read data from file. Size: {data.Length} bytes."); // Log the size of the read data

                // Define the prefix to search for in the data
                byte[] prefix = { 0x01, 0x02, 0x5D, 0x01 };
                int prefixLength = prefix.Length; // Length of the prefix
                int interval = 96; // Define an interval for processing

                // Find the offsets of the specified prefix in the data
                List<int> offsets = FindPrefixes(data, prefix);
                log($"{LogPrefix.Prefix}Total number of prefixes found: {offsets.Count}."); // Log the number of prefixes found

                // Process the data packets using the found offsets and prefix information
                bool needsRepair;
                byte[] recoveredData = ProcessPackets(data, offsets, prefixLength, interval, out needsRepair);

                if (needsRepair)
                {
                    // Write the processed data to the new file in the same directory as the input file
                    File.WriteAllBytes(outputFileName, recoveredData);
                    log($"{LogPrefix.Prefix}File repaired and saved to:{Environment.NewLine}{outputFileName}."); // Log completion and file save information
                }
                else
                {
                    // Inform the user that the file is already valid
                    log($"{LogPrefix.Prefix}No repair needed for file: {Path.GetFileName(inputFileName)}.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during repair: {ex.Message}", "Repair Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnRepairWRLFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "WRL files (*.WRL)|*.WRL|All files (*.*)|*.*"; // Filter for WRL files and all files
                openFileDialog.Title = "Select a WRL File"; // Title of the dialog

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the selected file's name
                    string inputFileName = openFileDialog.FileName;

                    // Call the repair function with the selected file
                    RepairWRLFile(inputFileName);
                }
            }
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

                // Clear any existing data
                logs.ClearPackets();
                UpdateButtonStates();
                Array.Clear(logs.PrimaryHeaderData, 0, logs.PrimaryHeaderData.Length);
                Array.Clear(logs.SecondaryHeaderData, 0, logs.SecondaryHeaderData.Length);

                if (!File.Exists(filename))
                {
                    lblFileName.Text = "Error: File Not Found";
                    return;
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

                    // Search for the byte sequence 01 02 5D 01
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
            }
            catch (ArgumentOutOfRangeException)
            {
                // Inform the user that the file is corrupted
                DialogResult result = MessageBox.Show("The file is corrupted or has an invalid format.\n\nWould you like to attempt repair?", "File Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                // If user chooses to repair the file
                if (result == DialogResult.Yes)
                {
                    // Call the repair function and pass the file name
                    RepairWRLFile(OpenFileName);

                    // After attempting repair, automatically open the repaired file without re-opening the dialog
                    string repairedFile = Path.Combine(logFolder, Path.GetFileNameWithoutExtension(OpenFileName) + "_fixed.WRL");
                    if (File.Exists(repairedFile))
                    {
                        
                        logs.ClearPackets();
                        Array.Clear(logs.PrimaryHeaderData, 0, logs.PrimaryHeaderData.Length);
                        Array.Clear(logs.SecondaryHeaderData, 0, logs.SecondaryHeaderData.Length);
                        OpenFileName = repairedFile; // Set the repaired file as the new file to open
 
                        // Load the repaired file
                        ClearBoxAndPackets();

                        logFolder = Path.GetDirectoryName(repairedFile);
                        lblFileName.Text = Path.GetFileName(repairedFile);
                        lblDirName.Text = Path.GetDirectoryName(repairedFile);

                        string binOutputFileName = Path.Combine(logFolder, Path.GetFileNameWithoutExtension(repairedFile) + ".bin");

                        using (var fileStream = new FileStream(repairedFile, FileMode.Open, FileAccess.Read))
                        using (var binReader = new BinaryReader(fileStream, Encoding.ASCII))
                        {
                            logs.PrimaryHeaderData = binReader.ReadBytes(logs.PrimaryHeaderLength);

                            // Search for the byte sequence 01 02 5D 01
                            byte[] searchPattern = { 0x01, 0x02, 0x5D, 0x01 };
                            long position = FindPatternInFile(repairedFile, searchPattern);

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
                        log($"{LogPrefix.Prefix}Repaired BIN file created and saved as: {fileName}");
                    }
                    else
                    {
                        MessageBox.Show("Repair operation failed or was not needed. The file could not be opened.", "Repair Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                // Show a generic error message if an unexpected error occurs
                MessageBox.Show($"An unexpected error occurred: {ex.Message}\n\n Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}


        


