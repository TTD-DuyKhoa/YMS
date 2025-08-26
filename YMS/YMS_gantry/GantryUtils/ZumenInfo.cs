using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMS_gantry.GantryUtils
{
    public class ZumenInfo
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

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

        public bool GetProjectInfo(Document doc)
        {
            try
            {
                ProjectInfo pInfo = doc.ProjectInformation;

                Parameter parameter = null;
                parameter = pInfo.LookupParameter(DefineUtil.ZumenInfo_Sekkeishishin);
                if (parameter != null)
                {
                    m_sekkeishishin = parameter.AsString();
                }

                parameter = pInfo.LookupParameter(DefineUtil.ZumenInfo_Tokuisaki);
                if (parameter != null)
                {
                    m_tokuisaki = parameter.AsString();
                }

                parameter = pInfo.LookupParameter(DefineUtil.ZumenInfo_GenbaName);
                if (parameter != null)
                {
                    m_genbaName = parameter.AsString();
                }

                parameter = pInfo.LookupParameter(DefineUtil.ZumenInfo_SekkeiNum);
                if (parameter != null)
                {
                    m_sekkeiNum = parameter.AsString();
                }
            }
            catch (OperationCanceledException e)
            {
                logger.Error(e.Message);
                return false;
            }

            return true;
        }
    }
}
