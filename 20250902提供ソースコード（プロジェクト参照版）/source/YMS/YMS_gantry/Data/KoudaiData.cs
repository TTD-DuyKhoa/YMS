using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMS_gantry.UI;
using static YMS_gantry.UI.FrmCreateSlopeViewModel;

namespace YMS_gantry.Data
{
    [Serializable]
    [System.Xml.Serialization.XmlRoot("koudaiData")]
    public class KoudaiData: GantryData
    {
        #region 定数
        /// <summary>
        /// スキーマ名称
        /// </summary>
        public new string SchemaName = "SchemaKoudai";
        #endregion

        #region プロパティ
        /// <summary>
        /// フラット構台一括配置情報
        /// </summary>
        [System.Xml.Serialization.XmlElement("AllKoudaiFlatData")]
        public AllKoudaiFlatFrmData AllKoudaiFlatData;

        /// <summary>
        /// スロープ作成情報
        /// </summary>
        [System.Xml.Serialization.XmlElement("SlopeData")]
        public KodaiSlopeModel SlopeData;

        /// <summary>
        /// ブレース・ツナギ一括配置情報
        /// </summary>
        [System.Xml.Serialization.XmlElement("BraceTsunagiData")]
        public BraceTsunagiData BraceTsunagiData;

        /// <summary>
        /// 計算書取込情報
        /// </summary>
        [System.Xml.Serialization.XmlElement("CalcFileData")]
        public CalcFileData CalcFileData;

        /// <summary>
        /// グループ名
        /// </summary>
        [System.Xml.Serialization.XmlElement("GroupName")]
        public string GroupName;

        /// <summary>
        /// 構台割り当て部材一覧
        /// </summary>
        [System.Xml.Serialization.XmlArray("ListWariateElementId")]
        [System.Xml.Serialization.XmlArrayItem("WariateElementId")]
        public List<int> ListWariateElementId;
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KoudaiData()
        {
            InitializeVariables();
        }
        /// <summary>
        /// 初期化
        /// </summary>
        private void InitializeVariables()
        {
            GroupName = string.Empty;
            ListWariateElementId = new List<int>();
        }
        /// <summary>
        /// 割り当て済ElementIdを取得
        /// </summary>
        public List<ElementId> GetListWariateElementId()
        {
            List<ElementId> lst = new List<ElementId>();
            foreach(int n in ListWariateElementId)
            {
                lst.Add(new ElementId(n));
            }
            return lst;
        }
        #endregion
    }
}
