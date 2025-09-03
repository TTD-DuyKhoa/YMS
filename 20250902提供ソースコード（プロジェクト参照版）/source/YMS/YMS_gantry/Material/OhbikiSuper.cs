using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMS_gantry.UI;
using static YMS_gantry.DefineUtil;

namespace YMS_gantry
{
    public abstract class OhbikiSuper : MaterialSuper
    {
        public static new string typeName = "大引";
        public static string shikiGetaName = "敷桁";

        [MaterialProperty("K")]
        public int m_K { get; set; }

        [MaterialProperty("始点側突出")]
        public double m_ExStartLen { get; set; }

        [MaterialProperty("終点側突出")]
        public double m_ExEndLen { get; set; }

        [MaterialProperty("取付サイド")]
        public string m_AttachSide { get; set; }

        public OhbikiSuper():base()
        {

        }

        public OhbikiSuper(ElementId id, string koudaname, OhbikiData data,int k = int.MinValue) : base(id, koudaname, data.OhbikiMaterial, data.OhbikiSize)
        {
            this.m_K = k;
            this.m_ExStartLen = data.exOhbikiStartLeng;
            this.m_ExEndLen = data.exOhbikiEndLeng;
            m_AttachSide = "";
        }

        public static double CalcLevelOffsetLevel(Level baseLevel, eBaseLevel bLevel, double offset, bool hasFukkou, double OhbikiH, double NedaH, int ohbikiC, bool isHOhbiki,double rotate, bool isNeedBtm = false)
        {
            double retOffset = 0;
            if (isHOhbiki)
            {
                if (bLevel == eBaseLevel.FukkouTop)
                {
                    retOffset = offset - (hasFukkou ? DefineUtil.FukkouBAN_THICK : 0) - NedaH - ((isNeedBtm) ? OhbikiH : (OhbikiH / 2));
                }
                else
                {
                    retOffset = (isNeedBtm) ? offset : offset + (OhbikiH / 2);
                }
            }
            else
            {
                if (bLevel == eBaseLevel.FukkouTop)
                {
                    retOffset = offset - (hasFukkou ? DefineUtil.FukkouBAN_THICK : 0) - NedaH;
                    if (RevitUtil.ClsGeo.GEO_EQ(rotate,180))
                    {
                        retOffset -= (OhbikiH * (ohbikiC - 1));
                    }
                    else
                    {
                        retOffset -= (OhbikiH * (ohbikiC )) - ((isNeedBtm) ? OhbikiH : (OhbikiH / 2));
                    }
                }
                else
                {
                    retOffset = (isNeedBtm) ? offset : offset + (OhbikiH / 2);
                }
            }

            return retOffset;
        }
    }


    [Serializable]
    [System.Xml.Serialization.XmlRoot("ohbikiData")]
    public class OhbikiData
    {
        /// <summary>
        /// 材質
        /// </summary>
        [System.Xml.Serialization.XmlElement("OhbikiMaterial")]
        public string OhbikiMaterial { get; set; }
        /// <summary>
        /// 大引タイプ
        /// </summary>
        [System.Xml.Serialization.XmlElement("OhbikiType")]
        public string OhbikiType { get; set; }
        /// <summary>
        /// 大引サイズ
        /// </summary>
        [System.Xml.Serialization.XmlElement("OhbikiSize")]
        public string OhbikiSize { get; set; }
        /// <summary>
        /// 基点側突出量
        /// </summary>
        [System.Xml.Serialization.XmlElement("exOhbikiStartLeng")]
        public double exOhbikiStartLeng { get; set; }
        /// <summary>
        /// 終点側突出量
        /// </summary>
        [System.Xml.Serialization.XmlElement("exOhbikiEndLeng")]
        public double exOhbikiEndLeng { get; set; }

        /// <summary>
        /// H鋼か
        /// </summary>
        [System.Xml.Serialization.XmlElement("isHkou")]
        public bool isHkou { get; set; }
        /// <summary>
        /// 取付数
        /// </summary>
        [System.Xml.Serialization.XmlElement("OhbikiDan")]
        public int OhbikiDan { get; set; }
        /// <summary>
        /// H鋼以外の取付タイプ
        /// </summary>
        [System.Xml.Serialization.XmlElement("OhbikiAttachWay")]
        public eAttachWay OhbikiAttachWay { get; set; }

        /// <summary>
        /// H鋼以外の取付方法
        /// </summary>
        [System.Xml.Serialization.XmlElement("OhbikiAttachType")]
        public eJoinType OhbikiAttachType { get; set; }

        /// <summary>
        /// H鋼以外のボルトタイプ
        /// </summary>
        [System.Xml.Serialization.XmlElement("OhbikiBoltType")]
        public eBoltType OhbikiBoltType { get; set; }

        /// <summary>
        /// H鋼以外のボルト本数
        /// </summary>
        [System.Xml.Serialization.XmlElement("OhbikiBoltCount")]
        public int OhbikiBoltCount { get; set; }


        public OhbikiData()
        {
            OhbikiMaterial = "SS400";
            OhbikiType = "";
            OhbikiSize = "";
            exOhbikiStartLeng = 0;
            exOhbikiEndLeng = 0;
            OhbikiAttachWay = eAttachWay.OneSide;
            OhbikiAttachType = eJoinType.Bolt;
            OhbikiBoltType = eBoltType.Normal;
            OhbikiBoltCount = 0;
            isHkou = true;
            OhbikiDan = 1;
        }
    }

    [Serializable]
    [System.Xml.Serialization.XmlRoot("shikigetaData")]
    public class ShikigetaData
    {
        /// <summary>
        /// 材質
        /// </summary>
        [System.Xml.Serialization.XmlElement("ShikigetaMaterial")]
        public string ShikigetaMaterial { get; set; }
        /// <summary>
        /// 敷桁タイプ
        /// </summary>
        [System.Xml.Serialization.XmlElement("ShikigetaType")]
        public string ShikigetaType { get; set; }
        /// <summary>
        /// 敷桁サイズ
        /// </summary>
        [System.Xml.Serialization.XmlElement("ShikigetaSize")]
        public string ShikigetaSize { get; set; }
        /// <summary>
        /// 基点側突出量
        /// </summary>
        [System.Xml.Serialization.XmlElement("exShikigetaStartLeng")]
        public double exShikigetaStartLeng { get; set; }
        /// <summary>
        /// 終点側突出量
        /// </summary>
        [System.Xml.Serialization.XmlElement("exShikigetaEndLeng")]
        public double exShikigetaEndLeng { get; set; }

        public ShikigetaData()
        {
            ShikigetaMaterial = "SS400";
            ShikigetaType = "";
            ShikigetaSize = "";
            exShikigetaStartLeng = 0;
            exShikigetaEndLeng = 0;
        }
    }
}

