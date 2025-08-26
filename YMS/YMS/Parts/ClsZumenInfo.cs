using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS.Parts
{
    public class ClsZumenInfo
    {
        #region 定数
        const string inisec = "Path";
        const string inikey = "FOLDER";
        public const string iniName = "Path.ini";
        public const string inisecSetting = "Setting";
        public const string inikeyBASE_LIST_SELECT_COLOR_R = "BASE_LIST_SELECT_COLOR_R";
        public const string inikeyBASE_LIST_SELECT_COLOR_G = "BASE_LIST_SELECT_COLOR_G";
        public const string inikeyBASE_LIST_SELECT_COLOR_B = "BASE_LIST_SELECT_COLOR_B";
        public const string inikeyYOKOYAITA_KAKARISHIRO = "YOKOYAITA_KAKARISHIRO";
        public const string master = "master\\yamadome";

        #endregion
        #region プロパティ
        /// <summary>
        /// 設計指針
        /// </summary>
        public string m_sekkeishishin { get; set; }
        /// <summary>
        /// 得意先
        /// </summary>
        public string m_tokuisaki { get; set; }
        /// <summary>
        /// 現場名
        /// </summary>
        public string m_genbaName { get; set; }
        /// <summary>
        /// 設計番号
        /// </summary>
        public string m_sekkeiNum { get; set; }
        #endregion
        #region コンストラクタ
        public ClsZumenInfo()
        {
            //初期化
            Init();
        }
        #endregion

        #region メソッド
        public void Init()
        {
            m_sekkeishishin = string.Empty;
            m_tokuisaki = string.Empty;
            m_genbaName = string.Empty;
            m_sekkeiNum = string.Empty;
        }
        /// <summary>
        /// ファミリフォルダを選択する
        /// </summary>
        /// <param name="path">ファミリフォルダ</param>
        /// <returns></returns>
        public static bool SetFamilyFolder(ref string path)
        {
            path = string.Empty;
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                // ダイアログのタイトルを設定
                dialog.Description = "フォルダを選択してください";

                // ダイアログを表示し、ユーザーの選択結果を取得
                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // ユーザーがフォルダを選択した場合は、選択されたパスを取得
                    string selectedPath = dialog.SelectedPath;
                    
                    path = selectedPath;
                    return true;
                }
            }
            return false;

        }

        /// <summary>
        /// YMSフォルダを取得（このフォルダの下にファミリとマスターが入っている）
        /// </summary>
        /// <returns>YMSフォルダパス</returns>
        public static string GetYMSFolder()
        {
            string selectedPath = string.Empty;

            string iniPath = ClsIni.GetIniFilePath(iniName);
            selectedPath = ClsIni.GetIniFile(inisec, inikey, iniPath);

            return selectedPath;
        }

       

        /// <summary>
        /// ベース一覧ダイアログで選択状態のベースの色の設定値のRを取得
        /// </summary>
        /// <returns></returns>
        public static byte GetBaseListSelectedbaseColorR()
        {
            const byte cb = 147;

            string iniPath = ClsIni.GetIniFilePath(iniName);
            string res = ClsIni.GetIniFile(inisecSetting, inikeyBASE_LIST_SELECT_COLOR_R, iniPath);

            if (string.IsNullOrWhiteSpace(res))
            {
                //取得に失敗した場合は規定値の147を返す
                return cb;
            }

            byte r = 0;
            if (byte.TryParse(res, out r))
            {
                return r;
            }

            //取得に失敗した場合は規定値の147を返す
            return cb;
        }


        /// <summary>
        /// ベース一覧ダイアログで選択状態のベースの色の設定値のRを取得
        /// </summary>
        /// <returns></returns>
        public static byte GetBaseListSelectedbaseColorG()
        {
            const byte cb = 39;

            string iniPath = ClsIni.GetIniFilePath(iniName);
            string res = ClsIni.GetIniFile(inisecSetting, inikeyBASE_LIST_SELECT_COLOR_G, iniPath);

            if (string.IsNullOrWhiteSpace(res))
            {
                //取得に失敗した場合は規定値の39を返す
                return cb;
            }

            byte g = 0;
            if (byte.TryParse(res, out g))
            {
                return g;
            }

            //取得に失敗した場合は規定値の39を返す
            return cb;
        }

        /// <summary>
        /// ベース一覧ダイアログで選択状態のベースの色の設定値のRを取得
        /// </summary>
        /// <returns></returns>
        public static byte GetBaseListSelectedbaseColorB()
        {
            const byte cb = 143;

            string iniPath = ClsIni.GetIniFilePath(iniName);
            string res = ClsIni.GetIniFile(inisecSetting, inikeyBASE_LIST_SELECT_COLOR_B, iniPath);

            if (string.IsNullOrWhiteSpace(res))
            {
                //取得に失敗した場合は規定値の143を返す
                return cb;
            }

            byte b = 0;
            
            if (byte.TryParse(res, out b))
            {
                return b;
            }

            //取得に失敗した場合は規定値の143を返す
            return cb;
        }


        public bool SaveProjectInfo(Document doc)
        {
            using (Transaction t = new Transaction(doc, Guid.NewGuid().GetHashCode().ToString()))
            {
                try
                {
                    t.Start();

                    ProjectInfo pInfo = doc.ProjectInformation;

                    Parameter parameter = null;
                    string mess = null;
                    parameter = pInfo.LookupParameter(ClsGlobal.m_zumeninfoSekkeishishin);
                    if (parameter != null)
                    {
                        parameter.Set(m_sekkeishishin);//設計指針
                    }
                    else { mess = mess + "「" + ClsGlobal.m_zumeninfoSekkeishishin + "」"; }

                    parameter = pInfo.LookupParameter(ClsGlobal.m_zumeninfoTokuisaki);
                    if (parameter != null)
                    {
                        parameter.Set(m_tokuisaki);//得意先名
                    }
                    else { mess = mess + "「" + ClsGlobal.m_zumeninfoTokuisaki + "」"; }

                    parameter = pInfo.LookupParameter(ClsGlobal.m_zumeninfoGenbaName);
                    if (parameter != null)
                    {
                        parameter.Set(m_genbaName);//現場名
                    }
                    else { mess = mess + "「" + ClsGlobal.m_zumeninfoGenbaName + "」"; }

                    parameter = pInfo.LookupParameter(ClsGlobal.m_zumeninfoSekkeiNum);
                    if (parameter != null)
                    {
                        parameter.Set(m_sekkeiNum);//設計番号
                    }
                    else { mess = mess + "「" + ClsGlobal.m_zumeninfoSekkeiNum + "」"; }

                    if (mess != null)
                    {
                        mess += "がプロジェクト情報項目に存在しません。\n";
                        MessageBox.Show(mess);
                    }

                    t.Commit();

                }
                catch (OperationCanceledException e)
                {
                }
                catch (Exception e)
                {
                }
            }
            return true;
        }
        public bool GetProjectInfo(Document doc)
        {
            try
            {
                ProjectInfo pInfo = doc.ProjectInformation;

                Parameter parameter = null;
                parameter = pInfo.LookupParameter(ClsGlobal.m_zumeninfoSekkeishishin);
                if (parameter != null)
                {
                    m_sekkeishishin = parameter.AsString();//設計指針
                }

                parameter = pInfo.LookupParameter(ClsGlobal.m_zumeninfoTokuisaki);
                if (parameter != null)
                {
                    m_tokuisaki = parameter.AsString();//得意先名
                }

                parameter = pInfo.LookupParameter(ClsGlobal.m_zumeninfoGenbaName);
                if (parameter != null)
                {
                    m_genbaName = parameter.AsString(); ;//現場名
                }

                parameter = pInfo.LookupParameter(ClsGlobal.m_zumeninfoSekkeiNum);
                if (parameter != null)
                {
                    m_sekkeiNum = parameter.AsString();//設計番号
                }
            }
            catch (OperationCanceledException e)
            {
            }
            catch (Exception e)
            {
            }
            return true;
        }
        #endregion
    }
}
