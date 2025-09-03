using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace YMS_Schedule
{
    class Env
    {
        public static string iniName = "YMS_Schedule.ini";

        public static string sectionRibbon = "ribbon";
        public static string keyTabName = "tabName";

        public static string sectionRibbonPanel = "ribbonPanel";
        public static string keyPanelName = "panelName";

        public static string sectionButton = "button";
        public static string keyButtonName = "buttonName";
        public static string keyImageS = "iconS";
        public static string keyImageL = "iconL";
        public static string keyDescription = "description";
        public static string keyCommand = "command";

        public static string sectionLog = "log";
        public static string showLog = "showlog";

        public static string sectionSharedParameterPath = "SharedParameterPath";
        static public string DummyFamilyPath = "DummyFamilyPath";

        public static string keyFilepath = "filePath";

        public static string sectionSchedule = "Schedule";
        public static string keyName = "name";

        public static string sectionParameterYMS = "ParameterYMS";
        public static string keyParameter1 = "parameter1";
        public static string keyParameter2 = "parameter2";
        public static string keyParameter3 = "parameter3";

        public static string sectionParameter = "Parameter";
        public static string keyId = "id";
        public static string keyDan = "dan";

        public static string sectionParameterAdd = "ParameterAdd";
        public static string sectionParameterPhase = "ParameterPhase";

        public static string sectionFilter = "Filter";
        public static string keyFilterDan = "dan";
        public static string keyFilter1 = "item1";
        public static string keyFilter2 = "item2";
        public static string keyFilter3 = "item3";

        public static string sectionProjectInfo = "ProjectInfo";
        public static string keyInfoDesign = "設計指針";
        public static string keyInfoCustomer = "得意先名称";
        public static string keyInfoName = "工事名称";
        public static string keyInfoClass = "工事区分";
        public static string keyInfoNo = "物件No.";
        public static string keyInfoBranch = "工事枝番";
        public static string keyInfoDate = "日付";
        public static string keyInfoTechnical = "技術担当者";
        public static string keyInfoSales = "営業担当者";
        public static string keyInfoComment = "数量総括表コメント";

        public static string sectionSeparator = "Separator";
        public static string keyProjectInfo = "ProjectInfo";
        public static string keySchedule = "Schedule";
        public static string keyEnd = "End";

        public static string ProductnameMaster = "ProductnameMaster";
        public static string keyYMS = "isYMS";

        public static string rKeyName = @"SOFTWARE\HIROSE\Hi-YMS数量表システム(Revit)";
        public static string rGetValueName = "path";

        public static string subFolderRevit = "Revit";
        public static string subFolderIcon = "icon";

        static public Encoding encoding = Encoding.GetEncoding("Shift_JIS");
        static public string separator = ",";

        static public string paramPhaseCreated = "構築フェーズ";
        static public string paramPhaseDemolished = "解体フェーズ";
        static public string paramLevel = "レベル";
        static public string paramFamily = "ファミリ";
        static public string paramFamilyAndType = "ファミリとタイプ";
        static public string paramVolume = "容積";

        static public string folderName = "YMS_Schedule";

        public static string sectionLevelCheck = "LevelCheck";
        public static string keyLogCreate = "LogCreate";
        public static string keyDialogShow = "DialogShow";
        public static string sectionLevelCheckParameter = "LevelCheckParameter";

        public static string productnameYMS()
        {
            return MiscWin.GetIniValue(iniPath(), Env.ProductnameMaster, Env.keyYMS);
        }
        public static string productnameMaster()
        {
            return MiscWin.GetIniValue(iniPath(), Env.ProductnameMaster, Env.keyFilepath);
        }
        public static string separatorProjectInfo()
        {
            return MiscWin.GetIniValue(iniPath(), Env.sectionSeparator, Env.keyProjectInfo);
        }
        public static string separatorSchedule()
        {
            return MiscWin.GetIniValue(iniPath(), Env.sectionSeparator, Env.keySchedule);
        }
        public static string separatorEnd()
        {
            return MiscWin.GetIniValue(iniPath(), Env.sectionSeparator, Env.keyEnd);
        }

        public static string dummyFamilyPath()
        {
            string fol = GetExecutingAssemblyDLLPath();
            string ini = MiscWin.GetIniValue(iniPath(), Env.DummyFamilyPath, Env.keyFilepath);
            string path = System.IO.Path.Combine(fol, ini);
            return path;//MiscWin.GetIniValue(iniPath(), Env.DummyFamilyPath, Env.keyFilepath);
        }

        /// <summary>
        /// DLL直下YMS_Scheduleフォルダ取得
        /// </summary>
        /// <returns></returns>
        public static string GetExecutingAssemblyYMSPath()
        {
            string apppath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string appFol = System.IO.Path.GetDirectoryName(apppath);
            string symbolFolpath = System.IO.Path.Combine(appFol, folderName);

            return symbolFolpath;
        }

        /// <summary>
        /// DLL階層フォルダ取得
        /// </summary>
        /// <returns></returns>
        public static string GetExecutingAssemblyDLLPath()
        {
            string apppath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string appFol = System.IO.Path.GetDirectoryName(apppath);

            return appFol;
        }

        public static Dictionary<string,string> projectInfo()
        {
            Dictionary<string, string> res = new Dictionary<string, string>();

            string path = iniPath();
            List<string> keys = new List<string>
                {
                 Env.keyInfoDesign
                ,Env.keyInfoCustomer
                ,Env.keyInfoName
                ,Env.keyInfoClass
                ,Env.keyInfoNo
                ,Env.keyInfoBranch
                ,Env.keyInfoDate
                ,Env.keyInfoTechnical
                ,Env.keyInfoSales
                ,Env.keyInfoComment
                };

            foreach(var x in keys)
            {
                res.Add(x, MiscWin.GetIniValue(path, Env.sectionProjectInfo, x));
            }

            return res;
        }
        public static List<string> parameterYMS()
        {
            List<string> res = new List<string>();
            string path = iniPath();
            res.Add(MiscWin.GetIniValue(path, Env.sectionParameterYMS, Env.keyParameter1));
            res.Add(MiscWin.GetIniValue(path, Env.sectionParameterYMS, Env.keyParameter2));
            res.Add(MiscWin.GetIniValue(path, Env.sectionParameterYMS, Env.keyParameter3));
            return res;
        }
        public static string sheduleName()
        {
            return MiscWin.GetIniValue(iniPath(), Env.sectionSchedule, Env.keyName);
        }

        public class ScheduleYMSFilter : Dictionary<string, string>
        {
        }

        public static ScheduleYMSFilter sheduleFilter(bool containPhase = true)
        {
            ScheduleYMSFilter res = new ScheduleYMSFilter();

            string path = iniPath();
            string sect = Env.sectionFilter;
            string key = "";
            
            key = Env.keyFilter1;
            res.Add(key, MiscWin.GetIniValue(path, sect, key));
            key = Env.keyFilter2;
            res.Add(key, MiscWin.GetIniValue(path, sect, key));
            key = Env.keyFilter3;
            res.Add(key, MiscWin.GetIniValue(path, sect, key));
            key = Env.keyFilterDan;
            res.Add(key, MiscWin.GetIniValue(path, sect, key));

            if (containPhase)
            {
                sect = Env.sectionParameterPhase;
                List<string> phase = MiscWin.GetKeys(sect, path);
                foreach (var x in phase)
                {
                    string value = MiscWin.GetIniValue(path, sect, x);
                    res.Add(value, value);
                }
            }

            return res;
        }

        public static string sharedParameterDan()
        {
            return MiscWin.GetIniValue(iniPath(), Env.sectionParameter, Env.keyDan);
        }

        public static string sharedParameterId()
        {
            return MiscWin.GetIniValue(iniPath(), Env.sectionParameter, Env.keyId);
        }

        public static string sharedParameterLevel()
        {
            return MiscWin.GetIniValue(iniPath(), Env.sectionParameter, Env.keyId);
        }

        public static string sharedParameterFile()
        {
            string path = MiscWin.GetIniValue(iniPath(), sectionSharedParameterPath, keyFilepath);
            if(! File.Exists(path)
            && !string.IsNullOrEmpty(path))
            {
                path = Env.pathAdjust(path);
            }
            return path;
        }

        public static List<string> parametersPhase()
        {
            List<string> res = new List<string>();
            string path = iniPath();
            string sectName = Env.sectionParameterPhase;
            List<string> lst = MiscWin.GetKeys(sectName, path);
            foreach (var x in lst)
            {
                res.Add(MiscWin.GetIniValue(path, sectName, x));
            }
            return res;
        }
        public static List<string> parametersAdd()
        {
            List<string> res = new List<string>();
            string path = iniPath();
            string sectName = Env.sectionParameterAdd;
            List<string> lst = MiscWin.GetKeys(sectName, path);
            foreach (var x in lst)
            {
                res.Add(MiscWin.GetIniValue(path, sectName, x));
            }
            return res;
        }

        public static List<string> sharedParameters(bool containPhase = true, bool containAdd = true)
        {
            List<string> res = new List<string>();
            string path= iniPath();
            string sectName = Env.sectionParameter;
            List<string> lst = MiscWin.GetKeys(sectName, path);
            foreach(var x in lst)
            {
                res.Add(MiscWin.GetIniValue(path, sectName, x));
            }

            if (containAdd)
            {
                res.AddRange(parametersAdd());
            }

            if (containPhase)
            {
                res.AddRange(parametersPhase());
            }

            return res;
        }
        static public string folderInstall()
        {
            //using (Microsoft.Win32.RegistryKey rKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(rKeyName))
            //{
            //    res = (string)rKey.GetValue(rGetValueName);
            //    rKey.Close();
            //}

            string apppath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string appFol = System.IO.Path.GetDirectoryName(apppath);
            //string folpath = System.IO.Path.Combine(appFol, "YMS_Schedule");
            string folpath = System.IO.Path.Combine(appFol, "数量data");

            return folpath;
        }

        static public string folderSettings()
        {
            return folderInstall();
        }

        static public string pathAdjust(string iPath)
        {
            string res = iPath;
            if (!string.IsNullOrEmpty(iPath)
            && !Path.IsPathRooted(iPath))
            {
                Uri u1 = new Uri(folderInstall());
                Uri u2 = new Uri(u1, iPath);
                res = u2.LocalPath;
            }
            return res;
        }

        static public string iniPath()
        {
            string fol = folderSettings();
            return Path.Combine(fol, iniName);
        }
        static public void ribbonButtonAdd(RibbonPanel panel, string btnName, string cmdName, string tooltip, string imageL, string imageS)
        {
            string iniPath = Env.iniPath();

            string btnImageL = string.Empty;
            if (!string.IsNullOrEmpty(imageL))
            {
                btnImageL = Env.pathAdjust(imageL);
            }

            string btnImageS = string.Empty;
            if (!string.IsNullOrEmpty(imageS))
            {
                btnImageS = Env.pathAdjust(imageS);
            }

            ribbonButtonAdd(panel, btnName, btnName, cmdName, tooltip, btnImageL, btnImageS);
        }
        static public void ribbonButtonAdd(RibbonPanel panel, string btnName, string btnText, string cmdName, string tooltip, string imageL, string imageS)
        {
            PushButtonData pbDataExtCmd1 = new PushButtonData(btnName, btnText, System.Reflection.Assembly.GetExecutingAssembly().Location, cmdName);
            PushButton pbExtCmd1 = panel.AddItem(pbDataExtCmd1) as PushButton;
            pbExtCmd1.ToolTip = tooltip;
            if (!string.IsNullOrEmpty(imageL))
            {
                pbExtCmd1.LargeImage = MiscWin.BmpImageSource(imageL);
            }
            if (!string.IsNullOrEmpty(imageS))
            {
                pbExtCmd1.Image = MiscWin.BmpImageSource(imageS);
            }
        }

        public static void levelCheck(ref bool bLog, ref bool bDialog) 
        {
            bLog = bDialog = false;

            List<string> res = new List<string>();
            string path = iniPath();
            string sectName = Env.sectionLevelCheck;

            string sLog = MiscWin.GetIniValue(path, sectName, Env.keyLogCreate);
            bLog = sLog.ToUpper().StartsWith("Y");

            string sDialog = MiscWin.GetIniValue(path, sectName, Env.keyDialogShow);
            bDialog = sDialog.ToUpper().StartsWith("Y");
        }

        public static List<string> levelCheckParam()
        {
            List<string> res = new List<string>();
            string path = iniPath();
            string sectName = Env.sectionLevelCheckParameter;
            List<string> lst = MiscWin.GetKeys(sectName, path);
            foreach (var x in lst)
            {
                res.Add(MiscWin.GetIniValue(path, sectName, x));
            }
            return res;
        }
    }
}
