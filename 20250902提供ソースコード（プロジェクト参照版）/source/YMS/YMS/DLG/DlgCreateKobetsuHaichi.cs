using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YMS.Parts;

namespace YMS.DLG
{
    public partial class DlgCreateKobetsuHaichi : Form
    {
        /// <summary>
        /// Iniファイルのパス
        /// </summary>
        const string IniPath = "ini\\DlgCreateKobetsuHaichi.ini";
        /// <summary>
        /// Iniファイルのセクション名
        /// </summary>
        const string sec = "DlgCreateKobetsuHaichi";
        public string m_targetFolder { get; set; }
        //public string m_targetFolder2 { get; set; }

        private string masterName = "個別配置マスタ.csv"; //マスタ名
        private string shihokouFolderName = "支保工関係";
        private string yamadomekabeFoldername = "山留壁関係";
        private string koudaiFoldername = "構台関係";

        private string m_familyPath = ""; //ファミリパス
        private string m_familyType = ""; //ファミリタイプ

        private bool m_yamadome { get; set; }

        public DlgCreateKobetsuHaichi(bool yamadome)
        {
            InitializeComponent();
            m_yamadome = yamadome;
        }

        private void DlgCreateKobetsuHaichi_Load(object sender, EventArgs e)
        {
            m_targetFolder = yamadomekabeFoldername;
           
            AddComboxBunruiItem();
            GetIniData();
        }

        private void comboBoxBunrui_SelectedIndexChanged(object sender, EventArgs e)
        {
            string target = comboBoxBunrui.Text;
            //if (target == shihokouFolderName)
            //{
            //    m_targetFolder = shihokouFolderName;
            //}
            //else
            //{
            //    m_targetFolder = yamadomekabeFoldername;
            //}

            m_targetFolder = target;


            AddComboxFolder1Item();
        }

        private void comboBoxFolder1_SelectedIndexChanged(object sender, EventArgs e)
        {
            AddComboxFolder2Item();
            AddComboBoxFamily2Item();
            AddComboBoxType2Item();
        }

        private void comboBoxFolder2_SelectedIndexChanged(object sender, EventArgs e)
        {
            AddComboBoxFamily2Item();
            AddComboBoxType2Item();
        }

        private void comboBoxFamily_SelectedIndexChanged(object sender, EventArgs e)
        {
            AddComboBoxType2Item();
        }

        private void AddComboxBunruiItem()
        {
            comboBoxBunrui.Items.Clear();
            comboBoxBunrui.Items.Add(yamadomekabeFoldername);
            comboBoxBunrui.Items.Add(shihokouFolderName);

#if BUILD_REVIT2022
            comboBoxBunrui.Items.Add(koudaiFoldername);
#else
            comboBoxBunrui.Items.Add(koudaiFoldername);
#endif

            if (comboBoxBunrui.Items.Contains(m_targetFolder))
            {
                comboBoxBunrui.SelectedItem = m_targetFolder;
            }
            else
            {
                comboBoxBunrui.SelectedIndex = 0;
            }
        }

        private void AddComboxFolder1Item()
        {
            comboBoxFolder1.Items.Clear();
            string path = System.IO.Path.Combine(ClsZumenInfo.GetYMSFolder(), m_targetFolder);
            if (Directory.Exists(path))
            {
                string[] SubFolders = System.IO.Directory.GetDirectories(path);
                foreach (string subFolder in SubFolders)
                {
                    string dirName = new DirectoryInfo(subFolder).Name;
                    if (dirName == "子ﾌｧﾐﾘ") { continue; }
                    if (dirName == "孫ﾌｧﾐﾘ") { continue; }
                    if (dirName.Contains("仮鋼材")) { continue; } //#31939

                    int index = dirName.IndexOf("_") + 1;
                    string txt = dirName.Substring(index, dirName.Length - index);

                    comboBoxFolder1.Items.Add(txt);
                }
                comboBoxFolder1.SelectedIndex = 0;
            }
        }

        private void AddComboxFolder2Item()
        {
            comboBoxFolder2.Items.Clear();

            string path = System.IO.Path.Combine(ClsZumenInfo.GetYMSFolder(), m_targetFolder);
            string targetPath = "";
            if (Directory.Exists(path))
            {
                string[] SubFolders = System.IO.Directory.GetDirectories(path);
                foreach (string subFolder in SubFolders)
                {
                    string dirName = new DirectoryInfo(subFolder).Name;
                    if (dirName == "子ﾌｧﾐﾘ") { continue; }
                    if (dirName == "孫ﾌｧﾐﾘ") { continue; }

                    int index = dirName.IndexOf("_") + 1;
                    string substringDirName = dirName.Substring(index, dirName.Length - index);
                    if (substringDirName == comboBoxFolder1.Text)
                    {
                        targetPath = subFolder;
                        break;
                    }
                }
            }

            string[] allSubFolders = System.IO.Directory.GetDirectories(targetPath);
            foreach (string subFolder in allSubFolders)
            {
                string dirName = new DirectoryInfo(subFolder).Name;
                if (dirName == "子ﾌｧﾐﾘ") { continue; }
                if (dirName == "孫ﾌｧﾐﾘ") { continue; }

                int index = dirName.IndexOf("_") + 1;
                string txt = dirName.Substring(index, dirName.Length - index);

                comboBoxFolder2.Items.Add(txt);
            }

            if (comboBoxFolder2.Items.Count == 0)
            {
                comboBoxFolder2.Text = "";
                comboBoxFolder2.Enabled = false;
            }
            else if (comboBoxFolder2.Items.Count == 1)
            {
                comboBoxFolder2.SelectedIndex = 0;
                comboBoxFolder2.Enabled = false;
            }
            else
            {
                comboBoxFolder2.Enabled = true;
                comboBoxFolder2.SelectedIndex = 0;
            }
        }

        private void AddComboBoxFamily2Item()
        {
            comboBoxFamily.Items.Clear();

            string path = System.IO.Path.Combine(ClsZumenInfo.GetYMSFolder(), m_targetFolder);
            string targetPath = "";
            if (Directory.Exists(path))
            {
                string[] SubFolders = System.IO.Directory.GetDirectories(path);

                foreach (string subFolder in SubFolders)
                {
                    string dirName = new DirectoryInfo(subFolder).Name;
                    if (dirName == "子ﾌｧﾐﾘ") { continue; }
                    if (dirName == "孫ﾌｧﾐﾘ") { continue; }

                    int index = dirName.IndexOf("_") + 1;
                    string substringDirName = dirName.Substring(index, dirName.Length - index);
                    if (substringDirName == comboBoxFolder1.Text)
                    {
                        targetPath = subFolder;
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(comboBoxFolder2.Text))
            {
                targetPath = targetPath + "\\" + comboBoxFolder2.Text;
            }

            if (Directory.Exists(targetPath))
            {
                string[] allSubFolders = System.IO.Directory.GetDirectories(targetPath, "*", System.IO.SearchOption.AllDirectories);
                if(allSubFolders.Length != 0)
                {
                    //サブフォルダ内にさらにサブフォルダがある場合
                    foreach (string subFolder in allSubFolders)
                    {
                        string dirName = new DirectoryInfo(subFolder).Name;
                        if (dirName == "子ﾌｧﾐﾘ") { continue; }
                        if (dirName == "孫ﾌｧﾐﾘ") { continue; }

                        string[] files = Directory.GetFiles(subFolder, "*.rfa");

                        foreach (string file in files)
                        {
                            string fileName = new DirectoryInfo(file).Name;
                            string name = Path.GetFileNameWithoutExtension(fileName);
                            comboBoxFamily.Items.Add(name);
                        }
                    }
                }
                else
                {
                    string[] files = Directory.GetFiles(targetPath, "*.rfa");

                    foreach (string file in files)
                    {
                        string fileName = new DirectoryInfo(file).Name;
                        string name = Path.GetFileNameWithoutExtension(fileName);
                        comboBoxFamily.Items.Add(name);
                    }
                }
            }

            if (comboBoxFamily.Items.Count == 0)
            {
                //エラー回避
                //サブフォルダ内にさらにサブフォルダがある場合、ファミリは取得できない　一旦保留
                comboBoxFolder2.Text = "";
            }
            else
            {
                comboBoxFamily.SelectedIndex = 0;
            }
        }

        private void AddComboBoxType2Item()
        {
            comboBoxType.Items.Clear();

            // 読み込みたいCSVファイルのパスを指定して開く
            string csvOath = System.IO.Path.Combine(ClsZumenInfo.GetYMSFolder(), ClsZumenInfo.GetYMSFolder(), masterName);

            //CSVファイルに書き込むときに使うEncoding
            System.Text.Encoding enc =
                System.Text.Encoding.GetEncoding("Shift_JIS");


            List<string> lists = new List<string>();
            if (File.Exists(csvOath))
            {
                StreamReader sr = new StreamReader(csvOath, enc);

                while (!sr.EndOfStream)
                {
                    // CSVファイルの一行を読み込む
                    string line = sr.ReadLine();

                    // 配列からリストに格納する
                    lists.Add(line);
                }
            }

            foreach (string list in lists)
            {
                string familyName = RevitUtil.ClsRevitUtil.GetFamilyName(list);
                if (familyName == comboBoxFamily.Text)
                {
                    string[] values = list.Split(',');
                    for (int i = 1; i < values.Length; i++)
                    {
                        if (string.IsNullOrEmpty(values[i]) || string.IsNullOrWhiteSpace(values[i]))
                        {
                            continue;
                        }
                        comboBoxType.Items.Add(values[i]);
                    }
                    break;
                }
            }

            if (comboBoxType.Items.Count == 0)
            {
                comboBoxType.Enabled = false;
                comboBoxType.Text = "";
            }
            else
            {
                comboBoxType.Enabled = true;
                comboBoxType.SelectedIndex = 0;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string path = System.IO.Path.Combine(ClsZumenInfo.GetYMSFolder(), m_targetFolder);
            string[] SubFolders = System.IO.Directory.GetDirectories(path);
            string targetPath = "";
            foreach (string subFolder in SubFolders)
            {
                string dirName = new DirectoryInfo(subFolder).Name;
                if (dirName == "子ﾌｧﾐﾘ") { continue; }
                if (dirName == "孫ﾌｧﾐﾘ") { continue; }

                int index = dirName.IndexOf("_") + 1;
                string substringDirName = dirName.Substring(index, dirName.Length - index);
                if (substringDirName == comboBoxFolder1.Text)
                {
                    targetPath = subFolder;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(comboBoxFolder2.Text))
            {
                targetPath = System.IO.Path.Combine(targetPath, comboBoxFolder2.Text);
            }

            //サブフォルダ内にさらにサブフォルダがある場合
            string[] allSubFolders = System.IO.Directory.GetDirectories(targetPath, "*", System.IO.SearchOption.AllDirectories);
            if(allSubFolders.Length != 0)
            {
                foreach (string subFolder in allSubFolders)
                {
                    if (File.Exists(System.IO.Path.Combine(subFolder, comboBoxFamily.Text + ".rfa")))
                    {
                        targetPath = subFolder;
                        break;
                    }
                }
            }

            m_familyPath = System.IO.Path.Combine(targetPath, comboBoxFamily.Text + ".rfa");
            m_familyType = comboBoxType.Text;

            SetIniData();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public string GetFamilyPath()
        {
            return m_familyPath;
        }

        public string GetFamilyType()
        {
            return m_familyType;
        }

        public void SetIniData()
        {
            string iniPath = ClsIni.GetIniFilePath(IniPath);
            if (!m_yamadome)
                iniPath = ClsIni.GetKoudaiIniFilePath(IniPath);


            ClsIni.WritePrivateProfileString(sec, "bunrui", comboBoxBunrui.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, "folder", comboBoxFolder1.Text, iniPath);

            ClsIni.WritePrivateProfileString(sec, "subfolder", comboBoxFolder2.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, "family", comboBoxFamily.Text, iniPath);
            ClsIni.WritePrivateProfileString(sec, "type", comboBoxType.Text, iniPath);
        }

        public void GetIniData()
        {
            string iniPath = ClsIni.GetIniFilePath(IniPath);
            if(!m_yamadome)
                iniPath = ClsIni.GetKoudaiIniFilePath(IniPath);

            string bunrui = ClsIni.GetIniFile(sec, "bunrui", iniPath);
            string folder = ClsIni.GetIniFile(sec, "folder", iniPath);
            string subfolder = ClsIni.GetIniFile(sec, "subfolder", iniPath);
            string family = ClsIni.GetIniFile(sec, "family", iniPath);
            string type = ClsIni.GetIniFile(sec, "type", iniPath);

            if (!string.IsNullOrWhiteSpace(bunrui))
            {
                if (comboBoxBunrui.Items.Contains(bunrui))
                {
                    comboBoxBunrui.Text = bunrui;
                }
            }

            if (!string.IsNullOrWhiteSpace(folder))
            {
                if (comboBoxFolder1.Items.Contains(folder))
                {
                    comboBoxFolder1.Text = folder;
                }
            }

            if (!string.IsNullOrWhiteSpace(subfolder))
            {
                if (comboBoxFolder2.Items.Contains(subfolder))
                {
                    comboBoxFolder2.Text = subfolder;
                }
            }

            if (!string.IsNullOrWhiteSpace(family))
            {
                if (comboBoxFamily.Items.Contains(family))
                {
                    comboBoxFamily.Text = family;
                }
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                if (comboBoxType.Items.Contains(type))
                {
                    comboBoxType.Text = bunrui;
                }
            }

        }
    }
}
