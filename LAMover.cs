using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using zamboni;


namespace LobbyActionMover
{
    public partial class LAMover : Form
    {
        public static string currentProgramFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        //public List<string> csvValues_string;
        public string PSO2Directory = "";
        public bool csvDataLoaded = false;
        public bool createBackups = false;
        public bool overWriteDirectly = false;

        public LAMover()
        {
            InitializeComponent();
            System.Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + currentProgramFolder); // Should fix ooz.dll problems
            csvValues = new List<LAData>();
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        public class LAData
        {
            public string[] valueData { get; set; }
            public string valueDescription { get; set; }
            /*
            public override string ToString()
            {
                return valueDescription;
            }
            */
        }
        public List<LAData> csvValues;
        private void loadAnimationsCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var filePath = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Select Animation .csv",
                Filter = ".csv (*.csv)|*.csv"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //Get the path of specified file
                filePath = openFileDialog.FileName;
                csvDataLoaded = true;

                using (var reader = new StreamReader(filePath))
                {
                    var gender = "Male";
                    var emoteVariation = 0;
                    var commandName = "";
                    csvValues = new List<LAData>();
                    //csvValues_string = new List<string>();
                    while (!reader.EndOfStream)

                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');

                        if (values.Length > 6)
                        {


                            //values[0].Trim(); // command name
                            //values[2].Trim(); // Normal Emote Name
                            //values[3].Trim(); // Base ICE
                            //values[4].Trim(); // NGS ICE
                            //
                            //
                            //values[5].Trim();//ngs Cast ice
                            //values[6].Trim();//ngs Caseal ice
                            emoteVariation++;
                            for (int i = 0; i < values.Length; i++)
                            {
                                if (values[i].Trim() == "Male" || values[i].Trim() == "Female")
                                {
                                    if (gender != values[i].Trim()) {
                                        gender = values[i].Trim();
                                        emoteVariation = 0;
                                    }
                                }
                            }
                            //values[8].Trim();//Male or female
                            if (values[0].Trim() != "") {
                                commandName = values[0].Trim();

                            }
                            else
                            {
                                values[0] = commandName;
                            }
                            var valueString = commandName + "-" + values[2].Trim() + "-" + gender + "-" + emoteVariation;

                            var lobbyactiondat = new LAData() {
                                valueData = values,
                                valueDescription = valueString,
                            };

                            csvValues.Add(lobbyactiondat);
                            //csvValues_string.Add(valueString);
                            listBox1.DisplayMember = "valueDescription";
                            listBox2.DisplayMember = "valueDescription";
                            listBox1.ValueMember = "valueData";
                            listBox2.ValueMember = "valueData";
                            listBox1.Items.Add(lobbyactiondat);
                            listBox2.Items.Add(lobbyactiondat);
                        }
                    }
                }
            }
        }

        private void setPSO2DirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog goodFolderDialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
                Title = "Select pso2_bin folder",
            };
            var pathIncorrect = true;
            while (pathIncorrect)
            {
                var goodfolder_result = goodFolderDialog.ShowDialog();
                if (goodfolder_result == CommonFileDialogResult.Cancel)
                    break;
                if (goodfolder_result == CommonFileDialogResult.Ok)
                {

                    PSO2Directory = goodFolderDialog.FileName;
                    var dir_info = new DirectoryInfo(PSO2Directory);
                    if (dir_info.Exists && dir_info.Name == "pso2_bin")
                    {
                        pathIncorrect = false;
                    }
                    else
                    {
                        MessageBox.Show("Incorrect folder, must be pso2_bin.");
                    }
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                listBox1.Items.Clear();
                foreach (LAData str in csvValues)
                {
                    if (str.valueDescription.Contains(textBox1.Text)) {
                        listBox1.Items.Add(str);
                    }
                }

            }
            else
            {
                foreach (LAData str in csvValues)
                {
                    listBox1.Items.Add(str);
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // same as textbox 1 but with 2
            if (textBox2.Text != "")
            {
                listBox2.Items.Clear();
                foreach (LAData str in csvValues)
                {
                    if (str.valueDescription.Contains(textBox2.Text))
                    {
                        listBox2.Items.Add(str);
                    }
                }

            }
            else
            {
                foreach (LAData str in csvValues)
                {
                    listBox2.Items.Add(str);
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        public string RemoveInvalidChars(string filename)
        {
            return string.Concat(filename.Split(Path.GetInvalidPathChars()));
        }
        private void button1_Click(object sender, EventArgs e) // Save and create replaced LA
        {



            if (csvDataLoaded)
            {
                if (PSO2Directory != "")
                {
                    var oldItem = (LAData)listBox1.SelectedItem;
                    var newItem = (LAData)listBox2.SelectedItem;
                    //values[3].Trim(); // Base ICE
                    //values[4].Trim(); // NGS ICE
                    //
                    //
                    //values[5].Trim();//ngs Cast ice
                    //values[6].Trim();//ngs Caseal ice
                    var newitemStringdescr = newItem.valueDescription.Split('-');
                    string newitemStringdescr_formatted = newitemStringdescr[1] + "_" + newitemStringdescr[2] + "_" + newitemStringdescr[3];
                    var olditemStringdescr = oldItem.valueDescription.Split('-');
                    string olditemStringdescr_formatted = olditemStringdescr[1] + "_" + olditemStringdescr[2] + "_" + olditemStringdescr[3];

                    string newReplacedLAFolder = RemoveInvalidChars(currentProgramFolder + "\\" + newitemStringdescr_formatted + "_over_" + olditemStringdescr_formatted);
                    if (!Directory.Exists(newReplacedLAFolder))
                        Directory.CreateDirectory(newReplacedLAFolder);
                    string newReplacedLAFolder_NGS = newReplacedLAFolder + "\\" + "win32reboot";
                    string newReplacedLAFolder_base = newReplacedLAFolder + "\\" + "win32";
                    if (!overWriteDirectly)
                    {
                        if (!Directory.Exists(newReplacedLAFolder_NGS))
                            Directory.CreateDirectory(newReplacedLAFolder_NGS);
                        if (!Directory.Exists(newReplacedLAFolder_base))
                            Directory.CreateDirectory(newReplacedLAFolder_base);
                    }
                    //Backups
                    string replacedLaFolder_backupfolder = newReplacedLAFolder + "\\" + "backup";
                    string replacedLaFolder_backupfolder_base = newReplacedLAFolder + "\\" + "backup" + "\\" + "win32";
                    string replacedLaFolder_backupfolder_NGS = newReplacedLAFolder + "\\" + "backup" + "\\" + "win32reboot";
                    if (createBackups)
                    {

                        if (!Directory.Exists(replacedLaFolder_backupfolder))
                            Directory.CreateDirectory(replacedLaFolder_backupfolder);
                        if (!Directory.Exists(replacedLaFolder_backupfolder_base))
                            Directory.CreateDirectory(replacedLaFolder_backupfolder_base);

                        if (!Directory.Exists(replacedLaFolder_backupfolder_NGS))
                            Directory.CreateDirectory(replacedLaFolder_backupfolder_NGS);
                    }
                    var exportTemp = currentProgramFolder + "\\temp";
                    if (!Directory.Exists(exportTemp))
                        Directory.CreateDirectory(exportTemp);
                    CleanFolder();
                    var oldItemaqms = ExtractTempICE(PSO2Directory + "\\data\\win32\\" + oldItem.valueData[3]);

                    var castfemaleAQM = "";
                    var castmaleAQM = "";
                    var humanAQM = "";
                    var camandcaf = false;
                    foreach (string str in oldItemaqms)
                    {
                        camandcaf = false;
                        Console.Out.WriteLine(str);
                        var fileNameString = Path.GetFileName(str).Trim();
                        if (fileNameString.Contains("cam") || fileNameString.Contains("caf")) // Dominant that should be used instead of lacf or lacm
                        {
                            camandcaf = true;
                        }
                        if ((fileNameString.StartsWith("pl_cas_lacf") && (!camandcaf)) || fileNameString.Contains("caf"))
                        {
                            castfemaleAQM = str;
                        }
                        if ((fileNameString.StartsWith("pl_cas_lacm") && (!camandcaf)) || fileNameString.Contains("cam"))
                        {
                            castmaleAQM = str;
                        }
                        if (fileNameString.StartsWith("pl_hum_la"))
                        {
                            humanAQM = str;
                        }
                    }
                    CleanFolder();
                    //pl_cas_lacf // cast female
                    //pl_cas_lacm // cast male
                    //pl_hum_la?? // human ?? is cf for female, or cm for male
                    var newItemsAqms = ExtractTempICE(PSO2Directory + "\\data\\win32\\" + newItem.valueData[3]);
                    var tempFolder = currentProgramFolder + "\\temp";
                    Console.WriteLine("CFAQM:" + castfemaleAQM);
                    Console.WriteLine("CMAQM:" + castmaleAQM);
                    Console.WriteLine("HUAQM:" + humanAQM);
                    Console.Out.WriteLine("NewItemAqms:");

                    foreach (string str in newItemsAqms)
                    {
                        camandcaf = false;
                        Console.Out.WriteLine(str);
                        var fileNameString = Path.GetFileName(str).Trim();

                        if (fileNameString.Contains("cam") || fileNameString.Contains("caf")) // Dominant that should be used instead of lacf or lacm
                        {
                            camandcaf = true;
                        }
                        if ((fileNameString.StartsWith("pl_cas_lacf") && (!camandcaf)) || fileNameString.Contains("caf"))
                        {
                            if (castfemaleAQM != "")
                                File.Copy(str, castfemaleAQM);
                        }
                        if ((fileNameString.StartsWith("pl_cas_lacm") && (!camandcaf)) || fileNameString.Contains("cam"))
                        {
                            if (castmaleAQM != "")
                                File.Copy(str, castmaleAQM);
                        }
                        if (fileNameString.StartsWith("pl_hum_la"))
                        {
                            File.Copy(str, humanAQM);
                        }
                        File.Delete(str);
                    }
                    if (createBackups)
                    {
                        File.Copy(PSO2Directory + "\\data\\win32\\" + oldItem.valueData[3], replacedLaFolder_backupfolder_base + "\\" + oldItem.valueData[3]);
                    }

                    var basedirectory = newReplacedLAFolder_base + "\\" + oldItem.valueData[3];

                    if (overWriteDirectly)
                    {
                        basedirectory = PSO2Directory + "\\data\\win32\\" + oldItem.valueData[3];
                    }
                    packIceFromDirectoryToFile(currentProgramFolder + "//temp",
                            ReadWhiteList(Path.Combine(currentProgramFolder, "group1.txt")),
                            false, false, true, false, basedirectory);

                    ///////////////////////////////////////Creating NGS files, oh boy
                    exportTemp = currentProgramFolder + "\\temp\\ngs";
                    if (!Directory.Exists(exportTemp))
                        Directory.CreateDirectory(exportTemp);

                    Console.WriteLine("---" + Path.Combine(currentProgramFolder, "group1.txt"));
                    Console.WriteLine("---" + exportTemp);
                    Console.WriteLine("---" + castfemaleAQM);

                    string valData;// 6 -- female cast location array value
                    string ngsIceSubFolder;
                    string ngsIceOrgName;
                    string tempNgsfolderstr;
                    string newFolderNGS;
                    ////////////////////////////////////// cast female
                    void ngsfileIcecreation (string genderaqm, int arrayvalue){
                        if (genderaqm != "")
                        {
                            valData = oldItem.valueData[arrayvalue].Trim();// 6 -- female cast location array value
                            ngsIceSubFolder = valData.Substring(0, 2);
                            ngsIceOrgName = valData.Substring(2, valData.Length - 2);

                            if (createBackups)
                            {
                                if (!Directory.Exists(replacedLaFolder_backupfolder_NGS + "\\" + ngsIceSubFolder))
                                    Directory.CreateDirectory(replacedLaFolder_backupfolder_NGS + "\\" + ngsIceSubFolder);
                                File.Copy(PSO2Directory + "\\data\\win32reboot\\" + ngsIceSubFolder + "\\" + ngsIceOrgName,
                                    replacedLaFolder_backupfolder_NGS + "\\" + ngsIceSubFolder + "\\" + ngsIceOrgName);
                            }

                            tempNgsfolderstr = exportTemp + "\\" + Path.GetFileName(genderaqm);

                            File.Copy(genderaqm, tempNgsfolderstr); // Copy AQM to temp ngs folder
                            if (!overWriteDirectly)
                            {
                                if (!Directory.Exists(newReplacedLAFolder_NGS + "\\" + ngsIceSubFolder))
                                    Directory.CreateDirectory(newReplacedLAFolder_NGS + "\\" + ngsIceSubFolder);// Create win32reboot subfolder
                            }

                            newFolderNGS = newReplacedLAFolder_NGS + "\\" + ngsIceSubFolder;

                            if (overWriteDirectly)
                            {
                                newFolderNGS = PSO2Directory + "\\data\\win32reboot\\" + ngsIceSubFolder;
                            }
                            packIceFromDirectoryToFile(exportTemp,
                                        ReadWhiteList(Path.Combine(currentProgramFolder, "group1.txt")), // Create ice
                                        false, false, true, false, newFolderNGS + "\\" + ngsIceOrgName);

                            File.Delete(tempNgsfolderstr); // Delete aqm and start again
                        }
                    }
                    ngsfileIcecreation(castfemaleAQM, 6);
                    ngsfileIcecreation(castmaleAQM, 5);
                    ngsfileIcecreation(humanAQM, 4);


                    //Clean Temp folders and such, or else it'll keep using the old files
                    CleanFolder();
                    if(!overWriteDirectly)
                        MessageBox.Show("Saved lobby action in program folder.");
                    else
                    {
                        MessageBox.Show("Saved lobby action in win32/win32reboot.\nReload LA's to see effect.");
                    }

                }
                else
                {
                    MessageBox.Show("The PSO2 Directory is not set.");
                }
            }
            else
            {
                MessageBox.Show("There is no CSV data loaded.");
            }
        }
        private static bool writeGroupToDirectory(byte[][] groupToWrite, string directory)
        {
            if (!Directory.Exists(directory) && groupToWrite != null && (uint)groupToWrite.Length > 0U)
                Directory.CreateDirectory(directory);
            for (int index = 0; index < groupToWrite.Length; ++index)
            {
                int int32_1 = BitConverter.ToInt32(groupToWrite[index], 16);
                string str = Encoding.ASCII.GetString(groupToWrite[index], 64, int32_1).TrimEnd(new char[1]);
                int int32_2 = BitConverter.ToInt32(groupToWrite[index], 12);
                int length = groupToWrite[index].Length - int32_2;
                byte[] bytes = new byte[length];
                Array.ConstrainedCopy((Array)groupToWrite[index], int32_2, (Array)bytes, 0, length);
                System.IO.File.WriteAllBytes(directory + "\\" + str, bytes);
                groupToWrite[index] = (byte[])null;
            }
            return groupToWrite != null && groupToWrite.Length != 0;
        }
        public string[] ExtractTempICE(string currFile) // Extract Ice and all of their files.
        {
            byte[] buffer = System.IO.File.ReadAllBytes(currFile);
            IceFile iceFile = IceFile.LoadIceFile((Stream)new MemoryStream(buffer));
            if (iceFile != null)
            {
                Path.GetFileName(currFile);
                string directoryName = Path.GetDirectoryName(currFile);
                var exportTemp = currentProgramFolder + "\\temp";
                if (!Directory.Exists(exportTemp))
                    Directory.CreateDirectory(exportTemp);

                bool group1Success = writeGroupToDirectory(iceFile.groupOneFiles, exportTemp);
                bool group2Success = writeGroupToDirectory(iceFile.groupTwoFiles, exportTemp);
                if (!group1Success && !group2Success)
                {
                    Console.Out.WriteLine("ERROR: Both groups not successfully exported.");
                    return new string[] { };
                }
                return Directory.GetFiles(currentProgramFolder + "\\temp");
            }
            Console.Out.WriteLine("ERROR: icefile null.");
            return new string[] { };

        }
        public void CleanFolder()
        {
            //Console.Out.WriteLine(currentProgramFolder + "\\temp");

            foreach (string file in Directory.GetFiles(currentProgramFolder + "\\temp"))
            {
                //Console.Out.WriteLine(file);
                File.Delete(file);
            }


        }





        public static void packIceFromDirectoryToFile(string path, List<string> group1WhiteList, bool searchSub, bool compress, bool useFolderGroup, bool forceUnencrypted = false, string newFilename = null)
        {
            if (!Directory.Exists(path))
                return;
            loadFilesFromDirectory(path, searchSub, group1WhiteList, out byte[][] groupOneIn, out byte[][] groupTwoIn, useFolderGroup, true);

            byte[] rawData = new IceV4File((new IceHeaderStructures.IceArchiveHeader()).GetBytes(), groupOneIn, groupTwoIn).getRawData(compress, forceUnencrypted);

            File.WriteAllBytes(newFilename, rawData);
        }
        public static void loadFilesFromDirectory(string path, bool searchSub, List<string> group1WhiteList, out byte[][] group1, out byte[][] group2, bool useFolderGroup, bool headerless = true)
        {
            string[] files;
            if (searchSub == true)
            {
                files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).ToArray();
            }
            else
            {
                files = Directory.GetFiles(path);
            }
            var group1temp = new List<byte[]>();
            var group2temp = new List<byte[]>();

            Parallel.ForEach(files, currfile =>
            {
                List<byte> file = new List<byte>(System.IO.File.ReadAllBytes(currfile));
                var realName = Path.GetFileName(currfile);
                //Add header as needed
                if (headerless == true)
                {
                    file.InsertRange(0, (new IceHeaderStructures.IceFileHeader(currfile, (uint)file.Count)).GetBytes());
                }

                //Sort basaed on loaded whitelist
                if (CheckIfGroup1(group1WhiteList, useFolderGroup, currfile, realName))
                {
                    group1temp.Add(file.ToArray());
                }
                else
                {
                    group2temp.Add(file.ToArray());
                }
            });

            group1 = group1temp.ToArray();
            group2 = group2temp.ToArray();
        }


        private static bool CheckIfGroup1(List<string> group1WhiteList, bool useFolderGroup, string currfile, string realName)
        {
            if (useFolderGroup && currfile.Contains("\\group"))
            {
                if (currfile.Contains("\\group1\\"))
                {
                    return true;
                }
            }
            else if (group1WhiteList.Any(v => realName.Contains(v)))
            {
                return true;
            }

            return false;
        }

        public static List<string> ReadWhiteList(string group1WhiteListName)
        {
            List<string> group1WhiteList = new List<string>();
            if (group1WhiteListName != null && File.Exists(group1WhiteListName))
            {
                group1WhiteList = new List<string>(File.ReadAllLines(group1WhiteListName));
                for (int i = group1WhiteList.Count - 1; i > 0; i--)
                {
                    if (group1WhiteList[i].Contains("//") || group1WhiteList[i].Contains(";") || group1WhiteList[i].Contains(" ") || group1WhiteList[i] == ""
                        || group1WhiteList[i].Contains(Encoding.UTF8.GetString(new byte[] { 9 })))
                    {
                        group1WhiteList.RemoveAt(i);
                    }
                }
            }

            return group1WhiteList;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            createBackups = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            overWriteDirectly = checkBox2.Checked;
        }
    }
}
