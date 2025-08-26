using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YMS_gantry.Data
{
    [Serializable]
    [System.Xml.Serialization.XmlRoot("SB6BIM")]
    public class CalcFileData
    {
        #region プロパティ

        [System.Xml.Serialization.XmlElement("Base")]
        public Base Base;

        [System.Xml.Serialization.XmlElement("Girder")]
        public Girder Girder;

        [System.Xml.Serialization.XmlElement("CatchBeam")]
        public CatchBeam CatchBeam;

        [System.Xml.Serialization.XmlElement("Pile")]
        public Pile Pile;

        [System.Xml.Serialization.XmlElement("WallGrider")]
        public WallGrider WallGrider;

        [System.Xml.Serialization.XmlElement("CoverPlate")]
        public CoverPlate CoverPlate;

        [System.Xml.Serialization.XmlElement("HBrace")]
        public HBrace HBrace;

        [System.Xml.Serialization.XmlElement("VBrace")]
        public VBrace VBrace;

        [System.Xml.Serialization.XmlElement("JointMember")]
        public JointMember JointMember;

        [System.Xml.Serialization.XmlElement("Manage")]
        public Manage Manage;

        #endregion

        #region メソッド

        //public static CalcFileData NewInstance(CalcFileData calcFileData)
        //{
        //    var _calcFileData = new CalcFileData();

        //    var calcFileDataBase = new Base
        //    {
        //        Bw = calcFileData.Base.Bw,
        //        Type = calcFileData.Base.Type
        //    };
        //    _calcFileData.Base = calcFileDataBase;

        //    var calcFileDataGirderGirderPitch = new GirderPitch
        //    {
        //        Pitches = calcFileData.Girder.GirderPitch.Pitches
        //    };
        //    var calcFileDataGirder = new Girder
        //    {
        //        Size = calcFileData.Girder.Size,
        //        LL = calcFileData.Girder.LL,
        //        LR = calcFileData.Girder.LR,
        //        Kind = calcFileData.Girder.Kind,
        //        Num = calcFileData.Girder.Num,
        //        GirderPitch = calcFileDataGirderGirderPitch
        //    };
        //    _calcFileData.Girder = calcFileDataGirder;

        //    var calcFileDataCatchBeam = new CatchBeam
        //    {
        //        Size = calcFileData.CatchBeam.Size,
        //        LL = calcFileData.CatchBeam.LL,
        //        LR = calcFileData.CatchBeam.LR,
        //        Kind = calcFileData.CatchBeam.Kind,
        //        TATE = calcFileData.CatchBeam.TATE,
        //        FitMeth = calcFileData.CatchBeam.FitMeth,
        //        BoltNum = calcFileData.CatchBeam.BoltNum
        //    };
        //    _calcFileData.CatchBeam = calcFileDataCatchBeam;

        //    var calcFileDataPilePilePitch = new PilePitch
        //    {
        //        Pitches = calcFileData.Pile.PilePitch.Pitches
        //    };
        //    var calcFileDataPile = new Pile
        //    {
        //        Size = calcFileData.Pile.Size,
        //        Kind = calcFileData.Pile.Kind,
        //        Length = calcFileData.Pile.Length,
        //        AtamaL = calcFileData.Pile.AtamaL,
        //        DD = calcFileData.Pile.DD,
        //        Num = calcFileData.Pile.Num,
        //        PilePitch = calcFileDataPilePilePitch
        //    };
        //    _calcFileData.Pile = calcFileDataPile;

        //    var calcFileDataWallGrider = new WallGrider
        //    {
        //        Size = calcFileData.WallGrider.Size
        //    };
        //    _calcFileData.WallGrider = calcFileDataWallGrider;

        //    var calcFileDataCoverPlateCoverPlatePitch = new CoverPlatePitch
        //    {
        //        Sizes = calcFileData.CoverPlate.CoverPlatePitch.Sizes
        //    };
        //    var calcFileDataCoverPlate = new CoverPlate
        //    {
        //        Exist = calcFileData.CoverPlate.Exist,
        //        Kind = calcFileData.CoverPlate.Kind,
        //        Num = calcFileData.CoverPlate.Num,
        //        CoverPlatePitch = calcFileDataCoverPlateCoverPlatePitch
        //    };
        //    _calcFileData.CoverPlate = calcFileDataCoverPlate;

        //    var calcFileDataHBrace = new HBrace
        //    {
        //        Exist = calcFileData.HBrace.Exist,
        //        Size = calcFileData.HBrace.Size
        //    };
        //    _calcFileData.HBrace = calcFileDataHBrace;

        //    var calcFileDataVBrace = new VBrace
        //    {
        //        Exist = calcFileData.VBrace.Exist,
        //        Size = calcFileData.VBrace.Size,
        //        LGExist = calcFileData.VBrace.LGExist,
        //        TRExist = calcFileData.VBrace.TRExist
        //    };
        //    _calcFileData.VBrace = calcFileDataVBrace;

        //    var calcFileDataJointMemberJointMemberPitch = new JointMemberPitch
        //    {
        //        Pitches = calcFileData.JointMember.JointMemberPitch.Pitches
        //    };
        //    var calcFileDataJointMember = new JointMember
        //    {
        //        Exist = calcFileData.JointMember.Exist,
        //        Size = calcFileData.JointMember.Size,
        //        LGExist = calcFileData.JointMember.LGExist,
        //        TRExist = calcFileData.JointMember.TRExist,
        //        UnderExist = calcFileData.JointMember.UnderExist,
        //        Num = calcFileData.JointMember.Num,
        //        JointMemberPitch = calcFileDataJointMemberJointMemberPitch
        //    };
        //    _calcFileData.JointMember = calcFileDataJointMember;

        //    var calcFileDataManage = new Manage
        //    {
        //        Dno = calcFileData.Manage.Dno,
        //        Client = calcFileData.Manage.Client,
        //        Construction = calcFileData.Manage.Construction,
        //        Comment1 = calcFileData.Manage.Comment1,
        //        Comment21 = calcFileData.Manage.Comment21,
        //        Comment22 = calcFileData.Manage.Comment22,
        //        Comment23 = calcFileData.Manage.Comment23,
        //        Comment24 = calcFileData.Manage.Comment24,
        //        Comment25 = calcFileData.Manage.Comment25,
        //        Comment26 = calcFileData.Manage.Comment26,
        //        DesignDocName = calcFileData.Manage.DesignDocName
        //    };
        //    _calcFileData.Manage = calcFileDataManage;

        //    return _calcFileData;
        //}

        /// <summary>
        /// XMLからモデル情報を取得
        /// </summary>
        /// <param name="xmlFilePath">計算書取込ファイルパス</param>
        /// <param name="calcFileData">計算書取込モデル情報</param>
        /// <returns></returns>
        public static bool ReadXML(string xmlFilePath, ref CalcFileData calcFileData)
        {
            if (System.IO.File.Exists(xmlFilePath))
            {
                CalcFileData cfd = new CalcFileData();
                if (!ExStrageUtils.ReadXml<CalcFileData>(xmlFilePath, ref cfd))
                {
                    return false;
                }
                calcFileData = cfd;
            }

            return true;
        }

        #endregion
    }

    #region サブクラス

    #region Base

    [Serializable]
    [System.Xml.Serialization.XmlRoot("Base")]
    public class Base
    {
        [System.Xml.Serialization.XmlElement("Type")]
        public string Type { get; set; }

        [System.Xml.Serialization.XmlElement("Bw")]
        public int? Bw { get; set; }
    }

    #endregion

    #region Girder

    [Serializable]
    [System.Xml.Serialization.XmlRoot("Girder")]
    public class Girder
    {
        [System.Xml.Serialization.XmlElement("Size")]
        public string Size { get; set; }

        [System.Xml.Serialization.XmlElement("LL")]
        public int? LL { get; set; }

        [System.Xml.Serialization.XmlElement("LR")]
        public int? LR { get; set; }

        [System.Xml.Serialization.XmlElement("Kind")]
        public string Kind { get; set; }

        [System.Xml.Serialization.XmlElement("Num")]
        public int? Num { get; set; }

        [System.Xml.Serialization.XmlElement("GirderPitch")]
        public GirderPitch GirderPitch { get; set; }
    }

    [Serializable]
    [System.Xml.Serialization.XmlRoot("GirderPitch")]
    public class GirderPitch
    {
        [System.Xml.Serialization.XmlElement("Pitch")]
        public List<PitchOfGirder> Pitches { get; set; } = new List<PitchOfGirder>();
    }

    [Serializable]
    [System.Xml.Serialization.XmlRoot("PitchOfGirder")]
    public class PitchOfGirder
    {
        [System.Xml.Serialization.XmlAttribute("no")]
        public int No { get; set; }

        [System.Xml.Serialization.XmlText]
        public int Value { get; set; }
    }

    #endregion

    #region CatchBeam

    [Serializable]
    [System.Xml.Serialization.XmlRoot("CatchBeam")]
    public class CatchBeam
    {
        [System.Xml.Serialization.XmlElement("Size")]
        public string Size { get; set; }

        [System.Xml.Serialization.XmlElement("LL")]
        public int? LL { get; set; }

        [System.Xml.Serialization.XmlElement("LR")]
        public int? LR { get; set; }

        [System.Xml.Serialization.XmlElement("Kind")]
        public string Kind { get; set; }

        [System.Xml.Serialization.XmlElement("TATE")]
        public string TATE { get; set; }

        [System.Xml.Serialization.XmlElement("FitMeth")]
        public string FitMeth { get; set; }

        [System.Xml.Serialization.XmlElement("BoltNum")]
        public string BoltNum { get; set; }
    }

    #endregion

    #region Pile

    [Serializable]
    [System.Xml.Serialization.XmlRoot("Pile")]
    public class Pile
    {
        [System.Xml.Serialization.XmlElement("Size")]
        public string Size { get; set; }

        [System.Xml.Serialization.XmlElement("Kind")]
        public string Kind { get; set; }

        [System.Xml.Serialization.XmlElement("Length")]
        public int? Length { get; set; }

        [System.Xml.Serialization.XmlElement("AtamaL")]
        public int? AtamaL { get; set; }

        [System.Xml.Serialization.XmlElement("DD")]
        public int? DD { get; set; }

        [System.Xml.Serialization.XmlElement("Num")]
        public int? Num { get; set; }

        [System.Xml.Serialization.XmlElement("PilePitch")]
        public PilePitch PilePitch { get; set; }
    }

    [Serializable]
    [System.Xml.Serialization.XmlRoot("PilePitch")]
    public class PilePitch
    {
        [System.Xml.Serialization.XmlElement("Pitch")]
        public List<PitchOfPile> Pitches { get; set; } = new List<PitchOfPile>();
    }

    [Serializable]
    [System.Xml.Serialization.XmlRoot("PitchOfPile")]
    public class PitchOfPile
    {
        [System.Xml.Serialization.XmlAttribute("no")]
        public int No { get; set; }

        [System.Xml.Serialization.XmlText]
        public int Value { get; set; }
    }

    #endregion

    #region WallGrider

    [Serializable]
    [System.Xml.Serialization.XmlRoot("WallGrider")]
    public class WallGrider
    {
        [System.Xml.Serialization.XmlElement("Size")]
        public string Size { get; set; }

        [System.Xml.Serialization.XmlElement("Kind")]
        public string Kind { get; set; }
    }

    #endregion

    #region CoverPlate

    [Serializable]
    [System.Xml.Serialization.XmlRoot("CoverPlate")]
    public class CoverPlate
    {
        [System.Xml.Serialization.XmlElement("Exist")]
        public string Exist { get; set; }

        [System.Xml.Serialization.XmlElement("Kind")]
        public string Kind { get; set; }

        [System.Xml.Serialization.XmlElement("Num")]
        public int? Num { get; set; }

        [System.Xml.Serialization.XmlElement("CoverPlatePitch")]
        public CoverPlatePitch CoverPlatePitch { get; set; }
    }

    [Serializable]
    [System.Xml.Serialization.XmlRoot("CoverPlatePitch")]
    public class CoverPlatePitch
    {
        [System.Xml.Serialization.XmlElement("Size")]
        public List<Size> Sizes { get; set; } = new List<Size>();
    }

    [Serializable]
    [System.Xml.Serialization.XmlRoot("Size")]
    public class Size
    {
        [System.Xml.Serialization.XmlAttribute("no")]
        public int No { get; set; }

        [System.Xml.Serialization.XmlText]
        public string Value { get; set; }
    }

    #endregion

    #region HBrace

    [Serializable]
    [System.Xml.Serialization.XmlRoot("HBrace")]
    public class HBrace
    {
        [System.Xml.Serialization.XmlElement("Exist")]
        public string Exist { get; set; }

        [System.Xml.Serialization.XmlElement("Size")]
        public string Size { get; set; }
    }

    #endregion

    #region VBrace

    [Serializable]
    [System.Xml.Serialization.XmlRoot("VBrace")]
    public class VBrace
    {
        [System.Xml.Serialization.XmlElement("Exist")]
        public string Exist { get; set; }

        [System.Xml.Serialization.XmlElement("Size")]
        public string Size { get; set; }

        [System.Xml.Serialization.XmlElement("LGExist")]
        public string LGExist { get; set; }

        [System.Xml.Serialization.XmlElement("TRExist")]
        public string TRExist { get; set; }
    }

    #endregion

    #region JointMember

    [Serializable]
    [System.Xml.Serialization.XmlRoot("JointMember")]
    public class JointMember
    {
        [System.Xml.Serialization.XmlElement("Exist")]
        public string Exist { get; set; }

        [System.Xml.Serialization.XmlElement("Size")]
        public string Size { get; set; }

        [System.Xml.Serialization.XmlElement("LGExist")]
        public string LGExist { get; set; }

        [System.Xml.Serialization.XmlElement("TRExist")]
        public string TRExist { get; set; }

        [System.Xml.Serialization.XmlElement("UnderExist")]
        public string UnderExist { get; set; }

        [System.Xml.Serialization.XmlElement("Num")]
        public int? Num { get; set; }

        [System.Xml.Serialization.XmlElement("JointMemberPitch")]
        public JointMemberPitch JointMemberPitch { get; set; }
    }

    [Serializable]
    [System.Xml.Serialization.XmlRoot("JointMemberPitch")]
    public class JointMemberPitch
    {
        [System.Xml.Serialization.XmlElement("Pitch")]
        public List<PitchOfJointMember> Pitches { get; set; } = new List<PitchOfJointMember>();
    }

    [Serializable]
    [System.Xml.Serialization.XmlRoot("PitchOfJointMember")]
    public class PitchOfJointMember
    {
        [System.Xml.Serialization.XmlAttribute("JMExist")]
        public string JMExist { get; set; }

        [System.Xml.Serialization.XmlAttribute("VBExist")]
        public string EVBExist { get; set; }

        [System.Xml.Serialization.XmlAttribute("no")]
        public int No { get; set; }

        [System.Xml.Serialization.XmlText]
        public int Value { get; set; }
    }

    #endregion

    #region Manage

    [Serializable]
    [System.Xml.Serialization.XmlRoot("Manage")]
    public class Manage
    {
        [System.Xml.Serialization.XmlElement("Dno")]
        public string Dno { get; set; }

        [System.Xml.Serialization.XmlElement("Client")]
        public string Client { get; set; }

        [System.Xml.Serialization.XmlElement("Construction")]
        public string Construction { get; set; }

        [System.Xml.Serialization.XmlElement("Comment1")]
        public string Comment1 { get; set; }

        [System.Xml.Serialization.XmlElement("Comment21")]
        public string Comment21 { get; set; }

        [System.Xml.Serialization.XmlElement("Comment22")]
        public string Comment22 { get; set; }

        [System.Xml.Serialization.XmlElement("Comment23")]
        public string Comment23 { get; set; }

        [System.Xml.Serialization.XmlElement("Comment24")]
        public string Comment24 { get; set; }

        [System.Xml.Serialization.XmlElement("Comment25")]
        public string Comment25 { get; set; }

        [System.Xml.Serialization.XmlElement("Comment26")]
        public string Comment26 { get; set; }

        [System.Xml.Serialization.XmlElement("DesignDocName")]
        public string DesignDocName { get; set; }
    }

    #endregion

    #endregion

}
