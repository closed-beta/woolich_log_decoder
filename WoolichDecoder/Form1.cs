using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WoolichDecoder.Models;
using WoolichDecoder.Settings;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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

        // Constructor for form???

        private bool IsFileLoaded()
        {
            if (string.IsNullOrEmpty(OpenFileName))
            {
                MessageBox.Show("Please open a file first.", "File Not Loaded", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            return true;
        }


        public WoolichFileDecoderForm()
        {
            InitializeComponent();
            cmbExportType.SelectedIndex = 0;

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
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 20, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 22, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 39, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 40, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 43, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 44, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 45, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 46, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 47, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 48, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 49, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 50, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 51, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 52, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 53, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 54, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 55, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 56, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 57, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 58, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 59, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 60, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 61, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 62, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 63, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 64, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 65, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 66, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 67, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 68, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 69, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 70, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 71, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 72, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 73, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 74, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 75, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 76, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 77, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 78, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 79, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 80, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 81, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 82, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 83, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 84, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 85, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 86, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 87, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 88, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 89, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 90, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 91, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 92, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 93, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 94, StaticValue = 0, File = string.Empty });
        }

        public void SetS1000RR_StaticColumns()
        {

            decodedColumns = new List<int>();

            presumedStaticColumns.Clear();


            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 150, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 151, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 152, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 153, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 154, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 155, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 156, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 157, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 158, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 159, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 160, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 161, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 162, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 163, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 164, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 165, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 166, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 167, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 168, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 169, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 170, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 171, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 172, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 173, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 174, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 175, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 176, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 177, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 178, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 179, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 180, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 181, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 182, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 183, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 184, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 185, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 186, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 187, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 188, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 189, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 190, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 191, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 192, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 193, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 194, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 195, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 196, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 197, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 198, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 199, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 200, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 201, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 202, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 203, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 204, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 205, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 206, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 207, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 208, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 209, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 210, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 211, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 212, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 213, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 214, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 215, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 216, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 217, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 218, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 219, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 220, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 221, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 222, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 223, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 224, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 225, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 226, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 227, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 228, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 229, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 230, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 231, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 232, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 233, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 234, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 235, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 236, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 237, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 238, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 239, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 240, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 241, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 242, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 243, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 244, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 245, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 246, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 247, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 248, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 249, StaticValue = 0, File = string.Empty });
            presumedStaticColumns.Add(new StaticPacketColumn() { Column = 250, StaticValue = 0, File = string.Empty });

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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

        private void btnOpenFile_Click(object sender, EventArgs e)
        {

            openWRLFileDialog.Title = "Select WRL file to inspect";

            if (string.IsNullOrWhiteSpace(this.openWRLFileDialog.InitialDirectory))
            {

                this.openWRLFileDialog.InitialDirectory = this.logFolder ?? Directory.GetCurrentDirectory();
            }

            this.openWRLFileDialog.Multiselect = false;
            this.openWRLFileDialog.Filter = "WRL files (*.wrl)|*.wrl|BIN files (*.bin)|*.bin|All files (*.*)|*.*";

            if (openWRLFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            };

            var filename = openWRLFileDialog.FileNames.FirstOrDefault();
            OpenFileName = filename;
            // clear any existing data
            logs.ClearPackets();
            Array.Clear(logs.PrimaryHeaderData, 0, logs.PrimaryHeaderData.Length);
            Array.Clear(logs.SecondaryHeaderData, 0, logs.SecondaryHeaderData.Length);

            string path = string.Empty;
            string binOutputFileName = string.Empty;
            string inputFileName = string.Empty;
            if (File.Exists(filename))
            {
                this.logFolder = Path.GetDirectoryName(filename);
                this.openWRLFileDialog.InitialDirectory = this.logFolder;

                outputFileNameWithoutExtension = Path.GetDirectoryName(filename) + "\\" + Path.GetFileNameWithoutExtension(filename);
                binOutputFileName = Path.GetDirectoryName(filename) + "\\" + Path.GetFileNameWithoutExtension(filename) + ".bin";
                // outputFileName = Path.GetFileNameWithoutExtension(filename) + ".sql";
                inputFileName = filename;

            }
            else
            {
                this.lblFileName.Text = "Error: File Not Found";
                return;
            }
            this.lblFileName.Text = inputFileName;

            FileStream fileStream = new FileStream(inputFileName, FileMode.Open, FileAccess.Read);
            BinaryReader binReader = new BinaryReader(fileStream, Encoding.ASCII);

            logs.PrimaryHeaderData = binReader.ReadBytes(logs.PrimaryHeaderLength);

            // Search for the byte sequence 01 02 5D 01
            byte[] searchPattern = new byte[] { 0x01, 0x02, 0x5D, 0x01 };
            long position = FindPatternInFile(inputFileName, searchPattern);

            if (position >= 0)
            {
                // Append information to txtLogging
                this.txtLogging.AppendText($"{LogPrefix.Prefix}Header marker was found at position {position} bytes." + Environment.NewLine);

                // If the pattern is found at a distance of logs.PrimaryHeaderLength + 1 bytes, skip reading the secondary header
                if (position != logs.PrimaryHeaderLength + 1)
                {
                    // Read the secondary header only if the sequence is not at position logs.PrimaryHeaderLength + 1
                    logs.SecondaryHeaderData = binReader.ReadBytes(logs.SecondaryHeaderLength);

                    exportLogs.SecondaryHeaderData = logs.SecondaryHeaderData;
                }
            }
            else
            {
                // If the pattern is not found, read the secondary header as usual
                logs.SecondaryHeaderData = binReader.ReadBytes(logs.SecondaryHeaderLength);
                exportLogs.SecondaryHeaderData = logs.SecondaryHeaderData;
            }

            exportLogs.PrimaryHeaderData = logs.PrimaryHeaderData;

            // Continue loading the data
            bool eof = false;

            while (!eof)
            {

                byte[] packetPrefixBytes = binReader.ReadBytes(logs.PacketPrefixLength);
                if (packetPrefixBytes.Length < 5)
                {
                    eof = true;
                    continue;
                }

                // It's wierd that I have to do - 2 but it works... I hope.
                int calculatedRemainingPacketBytes = (int)packetPrefixBytes[3] - 2;

                byte[] packetBytes = binReader.ReadBytes(calculatedRemainingPacketBytes);
                if (packetBytes.Length < calculatedRemainingPacketBytes)
                {
                    eof = true;
                    continue;
                }

                int totalPacketLength = (int)packetPrefixBytes[3] + 3;
                logs.AddPacket(packetPrefixBytes.Concat(packetBytes).ToArray(), totalPacketLength, (int)packetPrefixBytes[4]);

            }

            this.txtLogging.AppendText($"{LogPrefix.Prefix}Data Loaded and {logs.GetPacketCount()} packets found." + Environment.NewLine);

            // byte[] headerBytes = binReader.ReadBytes((int)fileStream.Length);
            // byte[] fileBytes = System.IO.File.ReadAllBytes(fileNameWithPath_); // this also works

            binReader.Close();
            fileStream.Close();


            // Trial output to file... This is the output cut down .bin file. 
            fileStream = File.Open(binOutputFileName, FileMode.Create);
            BinaryWriter binWriter = new BinaryWriter(fileStream);

            // push to disk
            binWriter.Flush();
            foreach (KeyValuePair<string, byte[]> packet in logs.GetPackets())
            {
                // byte[] outPacket = new byte[48];
                // Array.Copy(packet, outPacket, 48);

                binWriter.Write(packet.Value);
                // binWriter.Write(outPacket);
            }
            // binWriter.Write(fileBytes); // just feed it the contents verbatim
            binWriter.Flush();
            fileStream.Flush();
            binWriter.Close();

            fileStream.Close();

            string fileName = Path.GetFileName(binOutputFileName);
            this.txtLogging.AppendText($"{LogPrefix.Prefix}BIN file created and saved as: " + fileName + Environment.NewLine);

        }
        // Method to search for a pattern in a file
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

        /// <summary>
        /// This is intended to analyse a specific column and filter the changes down to just where that single field changes.
        /// It's basically redundant now but may serve a purpose in the future.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        


        private async void btnAnalyse_Click(object sender, EventArgs e)
        {
            if (!IsFileLoaded())
            {
                txtFeedback.AppendText("No file loaded." + Environment.NewLine);
                return;
            }

            txtFeedback.AppendText("Analysis started..." + Environment.NewLine);

            txtFeedback.AppendText($"Packet format: {logs.PacketFormat:X}" + Environment.NewLine);

            if (logs.PacketFormat == 0x01)
            {
                txtFeedback.AppendText("MT09 packet format detected. Setting static columns..." + Environment.NewLine);
                SetMT09_StaticColumns();
            }
            else if (logs.PacketFormat == 0x10)
            {
                txtFeedback.AppendText("S1000RR packet format detected. Setting static columns..." + Environment.NewLine);
                SetS1000RR_StaticColumns();
            }
            else
            {
                txtFeedback.AppendText("Unknown packet format. Clearing presumed static columns..." + Environment.NewLine);
                this.presumedStaticColumns.Clear();
            }

            if (!string.IsNullOrWhiteSpace(txtBreakOnChange.Text))
            {
                try
                {
                    int columnNumber = int.Parse(txtBreakOnChange.Text);
                    txtFeedback.AppendText($"Monitoring changes on column: {columnNumber}" + Environment.NewLine);
                    analysisColumn.Add(columnNumber);

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

                    if (columnFunctions.ContainsKey(columnNumber))
                    {
                        txtFeedback.AppendText($"Analyzing column {columnNumber}..." + Environment.NewLine);
                        txtFeedback.Clear();
                        lblExportPacketsCount.Text = string.Empty;
                        exportLogs.ClearPackets();

                        StringBuilder feedbackBuffer = new StringBuilder();
                        Func<byte[], double> conversionFunction = columnFunctions[columnNumber];

                        double? previousValue = null;
                        double? minValue = null;
                        double? maxValue = null;

                        var packets = logs.GetPackets();
                        int totalPackets = packets.Count;
                        int processedPackets = 0;

                        // Show progress bar and initialize it
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

                                // Update progress bar and label
                                this.Invoke(new Action(() =>
                                {
                                    progressBar.Value = Math.Min(progressPercentage, progressBar.Maximum);
                                    UpdateProgressLabel($"Analyzing... {progressPercentage}% completed");
                                }));

                                // Allow UI to update
                                Application.DoEvents();
                            }
                        });

                        feedbackBuffer.AppendLine($"Column {columnNumber} min value: {minValue}, max value: {maxValue}");
                        txtFeedback.AppendText(feedbackBuffer.ToString());
                    }
                    else
                    {
                        txtFeedback.AppendText("Column not supported for analysis. Displaying legend..." + Environment.NewLine);
                        DisplayLegend();
                    }
                }
                catch (Exception ex)
                {
                    txtFeedback.AppendText($"Error parsing column number: {ex.Message}" + Environment.NewLine);
                    MessageBox.Show("Wrong column number.");
                }
            }

            if (logs.GetPacketCount() == 0)
            {
                txtFeedback.AppendText("No packets in logs." + Environment.NewLine);
                MessageBox.Show("File not open");
            }

            if (exportLogs.GetPacketCount() > 0)
            {
                txtFeedback.AppendText($"Exporting {exportLogs.GetPacketCount()} packets..." + Environment.NewLine);
                lblExportPacketsCount.Text = $"{exportLogs.GetPacketCount()}";
                outputFileNameWithExtension = outputFileNameWithoutExtension + $"_C{txtBreakOnChange.Text.Trim()}.WRL";
                lblExportFilename.Text = outputFileNameWithExtension;
            }
            else
            {
                txtFeedback.AppendText("No packets to export." + Environment.NewLine);
                lblExportPacketsCount.Text = "No packets to export.";
                lblExportFilename.Text = "Export filename not defined";
            }

            txtFeedback.AppendText("Analysis completed." + Environment.NewLine);

            // Hide progress bar and reset label
            UpdateProgressLabel("Analysis finished.");
            System.Threading.Thread.Sleep(3000);
            progressBar.Visible = false;
            progressLabel.Visible = false;
            
        }

        private void DisplayLegend()
        {
            StringBuilder legend = new StringBuilder();
            legend.AppendLine("Wrong column number. Available:");
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


            txtFeedback.Clear();
            txtFeedback.AppendText(legend.ToString());
        }
        void log(string logData)
        {
            txtLogging.AppendText(logData + Environment.NewLine);
        }

        void clearLog()
        {
            txtLogging.Clear();
        }

        /// <summary>
        /// This is for exporting to a WRL file, the results of a column analysis. It requires a column to be analysed even though I check the export type.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        //private void btnExportTargetColumn_Click(object sender, EventArgs e)
        //{
        //    if (!IsFileLoaded())
        //        return;

        //    WoolichMT09Log exportItem = null;
        //    // "Export Full File",
        //    // "Export Analysis Only"
        //    if (cmbExportType.SelectedIndex == 0)
        //    {
        //        // "Export Full File",
        //        exportItem = logs;
        //        // Error condition.
        //        MessageBox.Show(
        //            "Export Analysis Only is supported at the moment.",
        //            "Export Not Supported",
        //            MessageBoxButtons.OK,
        //            MessageBoxIcon.Information
        //        );
        //    }
        //    else
        //    {
        //        // "Export Analysis Only"
        //        exportItem = exportLogs;
        //    }

        //    if (!string.IsNullOrWhiteSpace(txtBreakOnChange.Text))
        //    {
        //        string outputFileNameWithExtension = ""; 

        //        try
        //        {
        //            var columnToExport = int.Parse(txtBreakOnChange.Text.Trim());


        //            outputFileNameWithExtension = outputFileNameWithoutExtension + $"_C{columnToExport}.WRL";


        //            using (BinaryWriter binWriter = new BinaryWriter(File.Open(outputFileNameWithExtension, FileMode.Create)))
        //            {
        //                binWriter.Write(exportItem.PrimaryHeaderData);
        //                binWriter.Write(exportItem.SecondaryHeaderData);
        //                foreach (var packet in exportItem.GetPackets())
        //                {
        //                    binWriter.Write(packet.Value);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //            MessageBox.Show($"Error during file export: {ex.Message}");
        //        }

        //        log($"{LogPrefix.Prefix}Analysis WRL File saved as:" + Path.GetFileName(outputFileNameWithExtension));
        //        //log($"{LogPrefix.Prefix}Analysis WRL File saved as: " + Environment.NewLine + outputFileNameWithExtension);
        //    }


        //}

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
                var columnToExport = int.Parse(txtBreakOnChange.Text.Trim());

                outputFileNameWithExtension = outputFileNameWithoutExtension + $"_C{columnToExport}.WRL";

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


        private void cmbExportType_SelectedIndexChanged(object sender, EventArgs e)
        {

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
            if (!IsFileLoaded())
                return;

            WoolichMT09Log exportItem = null;
            var csvFileName = outputFileNameWithoutExtension + $".csv"; // Default file name

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

                // Get the column number if provided
                if (!string.IsNullOrWhiteSpace(txtBreakOnChange.Text))
                {
                    try
                    {
                        // Parse the column number from the text input
                        int columnNumber = int.Parse(txtBreakOnChange.Text.Trim());

                        // Modify the file name to include the column number for analysis
                        csvFileName = outputFileNameWithoutExtension + $"_C{columnNumber}.csv";
                    }
                    catch (Exception ex)
                    {
                        // Handle parsing errors if the input is not a valid integer
                        MessageBox.Show("Invalid column number. Please provide a valid column number for analysis.}" + ex.Message);
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

                            // Limiting the output for non-MT09 formats if count exceeds 100,000
                            if (count > 100000 && exportItem.PacketFormat != 0x01)
                            {
                                break;
                            }
                        }

                        outputFile.Close(); // Close the file after writing is complete
                    }

                    // Log success and update UI
                    Invoke(new Action(() => log($"{LogPrefix.Prefix}File in CSV format saved as: " + Path.GetFileName(csvFileName))));
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




        //private void btnAutoTuneExport_Click(object sender, EventArgs e)
        //{
        //    if (!IsFileLoaded())
        //        return;
        //    WoolichMT09Log exportItem = logs;

        //    if (exportItem.PacketFormat != 0x01)
        //    {
        //        MessageBox.Show("This bikes file cannot be adjusted by this software yet.");
        //        return;
        //    }

        //    string outputFileNameWithExtension = outputFileNameWithoutExtension + $"_AT.WRL";
        //    try
        //    {

        //        List<string> selectedFilterOptions = new List<string>();

        //        var selectedCount = this.aTFCheckedListBox.CheckedItems.Count;

        //        for (int i = 0; i < this.aTFCheckedListBox.CheckedItems.Count; i++)
        //        {
        //            selectedFilterOptions.Add(this.aTFCheckedListBox.CheckedItems[i].ToString());
        //        }

        //        // Trial output to file...
        //        BinaryWriter binWriter = new BinaryWriter(File.Open(outputFileNameWithExtension, FileMode.Create));
        //        // push to disk
        //        // binWriter.Flush();
        //        binWriter.Write(exportItem.PrimaryHeaderData);
        //        binWriter.Write(exportItem.SecondaryHeaderData);
        //        foreach (var packet in exportItem.GetPackets())
        //        {

        //            // break the reference
        //            byte[] exportPackets = packet.Value.ToArray();

        //            int diff = 0;
        //            // I'm going to change the approach to this... Rather than eliminate the record i'm going to make it one that woolich will filter out.
        //            // The reason for this is that we may drop an AFR record for a prior record.
        //            // Options are:
        //            // Temperature
        //            // gear
        //            // clutch
        //            // 0 RPM
        //            // I'm choosing gear first. Lets make it 0
        //            var outputGear = packet.Value.getGear();

        //            // {
        //            // 0 "MT09 ETV correction",
        //            // 1 "Remove Gear 2 logs",
        //            // 2 "Exclude below 1200 rpm",
        //            // 3 "Remove Gear 1, 2 & 3 engine braking",
        //            // 4 "Remove non launch gear 1"
        //            // };



        //            // "Remove Gear 2 logs"
        //            if (outputGear == 2 && selectedFilterOptions.Contains(autoTuneFilterOptions[1]))
        //            {
        //                // 2nd gear is just for slow speed tight corners.
        //                // 0 gear is neutral and is supposed to be filterable in autotune.

        //                // adjust the gear packet to make woolich autotune filter it out.
        //                byte newOutputGearByte = (byte)(exportPackets[24] & (~0b00000111));
        //                diff = diff + newOutputGearByte - outputGear;
        //                outputGear = newOutputGearByte;
        //                exportPackets[24] = newOutputGearByte;

        //                // continue;
        //            }

        //            // 4 "Remove non launch gear 1 - customizable"

        //            int minRPM = int.Parse(textBox2.Text);  // Read and convert textBox2
        //            int maxRPM = int.Parse(textBox3.Text);  // Read and convert textBox3

        //            if (outputGear == 1 && (packet.Value.getRPM() < minRPM || packet.Value.getRPM() > maxRPM) && selectedFilterOptions.Contains(autoTuneFilterOptions[4]))

        //            // 4 "Remove non launch gear 1"
        //            //if (outputGear == 1 && (packet.Value.getRPM() < 1000 || packet.Value.getRPM() > 4500) && selectedFilterOptions.Contains(autoTuneFilterOptions[4]))
        //            {
        //                // We don't want first gear but we do want launch RPM ranges
        //                // Exclude anything outside of the launch ranges.

        //                byte newOutputGearByte = (byte)(exportPackets[24] & (~0b00000111));
        //                diff = diff + newOutputGearByte - outputGear;
        //                outputGear = newOutputGearByte;
        //                exportPackets[24] = newOutputGearByte;



        //                // continue;
        //            }


        //            // Get rid of any RPM below defined by textBox1

        //            int rpmLimit = int.Parse(textBox1.Text);

        //            if (outputGear != 1 && packet.Value.getRPM() <= rpmLimit && selectedFilterOptions.Contains(autoTuneFilterOptions[2]))


        //            // Get rid of anything below 1200 RPM
        //            // 2 "Exclude below 1200 rpm"
        //            //if (outputGear != 1 && packet.Value.getRPM() <= 1200 && selectedFilterOptions.Contains(autoTuneFilterOptions[2]))
        //            {
        //                // We aren't interested in below idle changes.

        //                byte newOutputGearByte = (byte)(exportPackets[24] & (~0b00000111));
        //                diff = diff + newOutputGearByte - outputGear;
        //                outputGear = newOutputGearByte;
        //                exportPackets[24] = newOutputGearByte;

        //                // continue;
        //            }

        //            // This one is tricky due to wooliches error in decoding the etv packet.
        //            // 3 "Remove Gear 1, 2 & 3 engine braking",
        //            if (packet.Value.getCorrectETV() <= 1.2 && outputGear < 4 && selectedFilterOptions.Contains(autoTuneFilterOptions[3]))
        //            {
        //                // We aren't interested closed throttle engine braking.

        //                byte newOutputGearByte = (byte)(exportPackets[24] & (~0b00000111));
        //                diff = diff + newOutputGearByte - outputGear;
        //                outputGear = newOutputGearByte;
        //                exportPackets[24] = newOutputGearByte;

        //                // continue;
        //            }


        //            if (selectedFilterOptions.Contains(autoTuneFilterOptions[0]))
        //            {
        //                // adjust the etv packet to make woolich put it in the right place.
        //                double correctETV = exportPackets.getCorrectETV();
        //                byte hackedETVbyte = (byte)((correctETV * 1.66) + 38);
        //                diff = diff + hackedETVbyte - exportPackets[18];
        //                exportPackets[18] = hackedETVbyte;
        //            }
        //            exportPackets[95] += (byte)diff;

        //            binWriter.Write(exportPackets);
        //        }
        //        binWriter.Close();


        //        //log($"{LogPrefix.Prefix}Autotune WRL File saved as: " + outputFileNameWithExtension);
        //        log($"{LogPrefix.Prefix}Autotune WRL File saved as: " + Path.GetFileName(outputFileNameWithExtension));

        //    }
        //    catch
        //    {
        //        log($"{LogPrefix.Prefix}Autotune WRL File saving error" + Environment.NewLine);

        //    }
        //}

        // Utility function for testing the checksum. Not used anymore.

        private void btnAutoTuneExport_Click(object sender, EventArgs e)
        {
            // Check if a file is loaded
            if (!IsFileLoaded())
            {
                //MessageBox.Show("File not loaded. Please open file first.");
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
            string outputFileNameWithExtension = outputFileNameWithoutExtension + $"_{binaryString}_AT.WRL"; // Correctly defined only once

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

                        int minRPM = int.Parse(textBox2.Text);  // Read and convert minimum RPM from textBox2
                        int maxRPM = int.Parse(textBox3.Text);  // Read and convert maximum RPM from textBox3

                        if (outputGear == 1 && (packet.Value.getRPM() < minRPM || packet.Value.getRPM() > maxRPM) && this.aTFCheckedListBox.CheckedItems.Contains(autoTuneFilterOptions[4]))
                        {
                            byte newOutputGearByte = (byte)(exportPackets[24] & (~0b00000111));
                            diff = diff + newOutputGearByte - outputGear;
                            outputGear = newOutputGearByte;
                            exportPackets[24] = newOutputGearByte;
                        }

                        int rpmLimit = int.Parse(textBox1.Text);  // Read and convert RPM limit from textBox1

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
                // Get size from textBox4
                // Attempt to parse the size from the text box. If parsing fails, show an error message and exit.
                if (!int.TryParse(textBox4.Text.Trim(), out int size))
                {
                    MessageBox.Show("Invalid size. Please enter a valid number.");
                    return;
                }

                // Generate the file name based on the size
                // Create the file name using the size entered by the user.
                var outputFileNameWithExtension = outputFileNameWithoutExtension + $"_CRC.{size}.WRL";

                WoolichMT09Log exportItem = logs;

                // Write data to a binary file
                // Open a binary file stream for writing, and use it to save the header data and packets.
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
                // Log the message with the name of the file that was saved, using only the file name without the path.
                log($"{LogPrefix.Prefix}CRC saved as: " + Path.GetFileName(outputFileNameWithExtension));
            }
            catch (Exception ex)
            {
                // Log any error that occurs during the export process
                // If an error occurs, log a message with the error details.
                log($"Error while exporting CRC: {ex.Message}");
            }
        }


        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void WoolichFileDecoderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            userSettings.LogDirectory = this.logFolder;

            // save the user settings.
            userSettings.Save();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void aTFCheckedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}
