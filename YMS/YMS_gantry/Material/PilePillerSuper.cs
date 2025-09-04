using Autodesk.Revit.DB ;
using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using YMS_gantry.UI ;
using static YMS_gantry.DefineUtil ;

namespace YMS_gantry
{
  public abstract class PilePillerSuper : MaterialSuper
  {
    public static new string typeName = "支持杭" ;
    public static string exPileTypeName = "継ぎ足し杭" ;
    public static string sichuTypeName = "支柱" ;

    [MaterialProperty( "橋長No" )]
    public int m_KyoutyouNum { get ; set ; }

    [MaterialProperty( "幅員No" )]
    public int m_HukuinNum { get ; set ; }

    public PilePillerSuper() : base()
    {
    }

    public PilePillerSuper( ElementId _id, string koudaname, PilePillerData data, int kyoutyouNum = int.MinValue,
      int hukuinNum = int.MinValue ) : base( _id, koudaname, data.PillarMaterial, data.PillarSize )
    {
      this.m_KyoutyouNum = kyoutyouNum ;
      this.m_HukuinNum = hukuinNum ;
    }

    /// <summary>
    /// 配置位置算出
    /// </summary>
    /// <returns></returns>
    public static Dictionary<XYZ, string> CalcArrangementPoint( double LengKyoutyou, double LenHukuin, double firstDiff,
      List<double> hukuinPitch, List<double> kyoutyouPitch, XYZ basePnt, XYZ vecKyoutyou, XYZ vecHukuin )
    {
      Dictionary<XYZ, string> retList = new Dictionary<XYZ, string>() ;
      XYZ baseP = basePnt ;
      XYZ newP = baseP + vecHukuin.Normalize() * ( RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( firstDiff ) ) ;

      double kyouLeng = 0 ;
      for ( int kC = 0 ; kC < kyoutyouPitch.Count ; kC++ ) {
        newP += vecKyoutyou.Normalize() * ( RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( kyoutyouPitch[ kC ] ) ) ;
        for ( int hC = 0 ; hC < hukuinPitch.Count ; hC++ ) {
          newP = newP + vecHukuin.Normalize() * ( RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( hukuinPitch[ hC ] ) ) ;
          if ( ! retList.ContainsKey( newP ) ) {
            retList.Add( newP, $"{kC},{hC}" ) ;
          }
        }

        newP = baseP + vecHukuin.Normalize() * ( RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( firstDiff ) ) ;
        kyouLeng += kyoutyouPitch[ kC ] ;
        newP += vecKyoutyou.Normalize() * ( RevitUtil.ClsRevitUtil.ConvertDoubleGeo2Revit( kyouLeng ) ) ;
      }

      return retList ;
    }

    /// <summary>
    /// 柱配置
    /// </summary>
    /// <returns></returns>
    public static ElementId CreatePile( Document doc, XYZ point, double angle, PilePillerData data, string koudaiName,
      XYZ adjustVec, Level level = null, double leveloffset = 0, FamilySymbol oriSym = null )
    {
      ElementId retId = ElementId.InvalidElementId ;
      FamilySymbol sym = oriSym ;
      if ( oriSym == null ) {
        string familyPath = Master.ClsHBeamCsv.GetPileFamilyPath( data.PillarSize ) ;
        if ( ! GantryUtil.GetFamilySymbol( doc, familyPath, typeName, out sym, true ) ) {
          return ElementId.InvalidElementId ;
        }

        sym = GantryUtil.DuplicateTypeWithNameRule( doc, koudaiName, sym, typeName ) ;
      }

      //パラメータの組み合わせ作成
      Dictionary<string, string> paramList = new Dictionary<string, string>() ;
      string jointF = GantryUtil.GetJointTypeName( data.jointDetailData.JointType, data.PillarSize ) ;
      FamilySymbol jSym = RevitUtil.ClsRevitUtil.GetFamilySymbol( doc, jointF, jointF ) ;

      if ( jSym != null ) {
        paramList.Add( DefineUtil.PARAM_JOINT_TYPE, jSym.Id.ToString() ) ;
      }

      //トッププレート
      paramList.Add( DefineUtil.PARAM_TOP_PLATE,
        ( ! data.ExtensionPile && data.HasTopPlate )
          ? ( (int) DefineUtil.PramYesNo.Yes ).ToString()
          : ( (int) DefineUtil.PramYesNo.No ).ToString() ) ;
      MaterialSize plsize = GantryUtil.GetKouzaiSize( data.topPlateData.PlateSize ) ;
      if ( plsize != null ) {
        paramList.Add( DefineUtil.PARAM_TOP_PLATE_W, plsize.Width.ToString() ) ;
        paramList.Add( DefineUtil.PARAM_TOP_PLATE_D, plsize.Height.ToString() ) ;
        paramList.Add( DefineUtil.PARAM_TOP_PLATE_T, plsize.Thick.ToString() ) ;
      }

      double cutLeng = data.PileCutLength /*+ data.exPillarHeadLeng*/ ;
      if ( data.IsCut && data.HasTopPlate ) {
        cutLeng += plsize.Thick ;
      }

      //paramList.Add(DefineUtil.PARAM_PILE_CUT_LENG, (data.IsCut) ? $"{cutLeng}" : "0");
      paramList.Add( DefineUtil.PARAM_PILE_CUT_LENG, cutLeng.ToString() ) ;
      paramList.Add( DefineUtil.PARAM_LENGTH, data.PillarWholeLength.ToString() ) ;
      paramList.Add( DefineUtil.PARAM_MATERIAL, data.PillarMaterial ) ;

      string plateName = "ﾄｯﾌﾟﾌﾟﾚｰﾄ_定形" ;
      string plateType = ( data.extensionPileData.topPlateData.PlateSize == "任意" )
        ? "ﾄｯﾌﾟﾌﾟﾚｰﾄ_任意"
        : data.topPlateData.PlateSize ;
      FamilySymbol pltSym = RevitUtil.ClsRevitUtil.GetFamilySymbolWithFuzzy( doc, plateName, plateType ) ;
      if ( pltSym == null ) {
        pltSym = RevitUtil.ClsRevitUtil.GetFamilySymbolWithFuzzy( doc, "ﾄｯﾌﾟﾌﾟﾚｰﾄ_任意", "ﾄｯﾌﾟﾌﾟﾚｰﾄ_任意" ) ;
      }

      if ( pltSym != null ) {
        paramList.Add( DefineUtil.PARAM_TOP_PLATE_SIZE, pltSym.Id.ToString() ) ;
      }

      //継ぎ手
      paramList.Add( DefineUtil.PARAM_JOINT_COUNT, data.pJointCount.ToString() ) ;
      ;
      for ( int i = 0 ; i < data.pJointPitch.Count ; i++ ) {
        if ( data.pJointPitch[ i ].Equals( 0 ) ) {
          continue ;
        }

        paramList.Add( $"杭{i + 1}", data.pJointPitch[ i ].ToString() ) ;
      }

      using ( SubTransaction tr = new SubTransaction( doc ) ) {
        tr.Start() ;
        ElementId CreatedID = RevitUtil.ClsRevitUtil.Create( doc, point, level.Id, sym ) ;
        RevitUtil.ClsRevitUtil.RotateElement( doc, CreatedID, Line.CreateBound( point, point + XYZ.BasisZ ), angle ) ;

        //高さ調整
        if ( data.HasTopPlate && ! data.IsCut ) {
          leveloffset -= plsize.Thick ;
        }

        if ( data.exPillarHeadLeng != 0 ) {
          leveloffset += data.exPillarHeadLeng ;
        }

        RevitUtil.ClsRevitUtil.SetParameter( doc, CreatedID, DefineUtil.PARAM_BASE_OFFSET,
          RevitUtil.ClsRevitUtil.CovertToAPI( leveloffset ) ) ;

        //タイプパラメータ設定
        foreach ( KeyValuePair<string, string> kv in paramList ) {
          GantryUtil.SetParameterValueByParameterName( doc.GetElement( CreatedID ) as FamilyInstance, kv.Key,
            kv.Value ) ;
        }

        retId = CreatedID ;
        tr.Commit() ;
      }

      return retId ;
    }

    /// <summary>
    /// 継足し杭配置
    /// </summary>
    /// <param name=""></param>
    /// <param name="leveloffset"></param>
    /// <returns></returns>
    public static ElementId CreateExtraPile( Document doc, XYZ point, double angle, PilePillerData data,
      string koudainame, XYZ adjustVec, Level level = null, double leveloffset = 0, FamilySymbol oriSym = null )
    {
      ElementId retId = ElementId.InvalidElementId ;
      FamilySymbol sym = oriSym ;
      if ( oriSym == null ) {
        string familyPath = Master.ClsHBeamCsv.GetExPileFamilyPath( data.PillarSize ) ;
        //IFamilyLoadOptions iflop = IFamilyLoadOptions.
        if ( ! GantryUtil.GetFamilySymbol( doc, familyPath, exPileTypeName, out sym, true ) ) {
          return ElementId.InvalidElementId ;
        }

        sym = GantryUtil.DuplicateTypeWithNameRule( doc, koudainame, sym, exPileTypeName ) ;
      }

      //トッププレート
      MaterialSize plsize = GantryUtil.GetKouzaiSize( data.extensionPileData.topPlateData.PlateSize ) ;

      //パラメータの組み合わせ作成
      Dictionary<string, string> paramList = new Dictionary<string, string>() ;
      string jointF = GantryUtil.GetJointTypeName( data.extensionPileData.attachType, data.PillarSize ) ;
      FamilySymbol jSym = RevitUtil.ClsRevitUtil.GetFamilySymbol( doc, jointF, jointF ) ;
      if ( jSym != null ) {
        paramList.Add( DefineUtil.PARAM_JOINT_TYPE, jSym.Id.ToString() ) ;
      }

      paramList.Add( DefineUtil.PARAM_PILE_CUT_LENG, "0" ) ;
      paramList.Add( DefineUtil.PARAM_LENGTH, ( data.extensionPileData.Length.ToString() ) ) ;
      paramList.Add( DefineUtil.PARAM_MATERIAL, data.PillarMaterial ) ;

      //paramList.Add(DefineUtil.PARAM_TOP_PLATE, ((int)DefineUtil.PramYesNo.Yes).ToString());
      paramList.Add( DefineUtil.PARAM_TOP_PLATE,
        ( data.HasTopPlate )
          ? ( (int) DefineUtil.PramYesNo.Yes ).ToString()
          : ( (int) DefineUtil.PramYesNo.No ).ToString() ) ;
      paramList.Add( DefineUtil.PARAM_TOP_PLATE_W, plsize.Width.ToString() ) ;
      paramList.Add( DefineUtil.PARAM_TOP_PLATE_D, plsize.Height.ToString() ) ;
      paramList.Add( DefineUtil.PARAM_TOP_PLATE_T, plsize.Thick.ToString() ) ;

      string plateName = "ﾄｯﾌﾟﾌﾟﾚｰﾄ_定形" ;
      string plateType = ( data.extensionPileData.topPlateData.PlateSize == "任意" )
        ? "ﾄｯﾌﾟﾌﾟﾚｰﾄ_任意"
        : data.topPlateData.PlateSize ;
      FamilySymbol pltSym = RevitUtil.ClsRevitUtil.GetFamilySymbolWithFuzzy( doc, plateName, plateType ) ;
      if ( pltSym == null ) {
        pltSym = RevitUtil.ClsRevitUtil.GetFamilySymbolWithFuzzy( doc, "ﾄｯﾌﾟﾌﾟﾚｰﾄ_任意", "ﾄｯﾌﾟﾌﾟﾚｰﾄ_任意" ) ;
      }

      if ( pltSym != null ) {
        paramList.Add( DefineUtil.PARAM_TOP_PLATE_SIZE, pltSym.Id.ToString() ) ;
      }

      //継ぎ手
      paramList.Add( DefineUtil.PARAM_JOINT_COUNT, "1" ) ;
      double jointZ = data.extensionPileData.Length - plsize.Thick ;
      paramList.Add( "杭1", jointZ.ToString() ) ;

      using ( SubTransaction tr = new SubTransaction( doc ) ) {
        tr.Start() ;
        ElementId CreatedID = RevitUtil.ClsRevitUtil.Create( doc, point, level.Id, sym ) ;
        RevitUtil.ClsRevitUtil.RotateElement( doc, CreatedID, Line.CreateBound( point, point + XYZ.BasisZ ), angle ) ;

        //タイプパラメータ設定
        foreach ( KeyValuePair<string, string> kv in paramList ) {
          GantryUtil.SetParameterValueByParameterName( doc.GetElement( CreatedID ) as FamilyInstance, kv.Key,
            kv.Value ) ;
        }

        double btmOffset = leveloffset + ( data.HasTopPlate ? -plsize.Thick : 0 ) ;
        RevitUtil.ClsRevitUtil.SetParameter( doc, CreatedID, DefineUtil.PARAM_BASE_OFFSET,
          RevitUtil.ClsRevitUtil.CovertToAPI( btmOffset ) ) ;

        retId = CreatedID ;
        tr.Commit() ;
      }

      return retId ;
    }

    /// <summary>
    /// 基準レベルを指定した際の杭のオフセット値
    /// </summary>
    /// <param name="baseLevel"></param>
    /// <param name="bLevel"></param>
    /// <param name="offset"></param>
    /// <param name="OhbikiH"></param>
    /// <param name="NedaH"></param>
    /// <returns></returns>
    public static double CalcLevelOffsetLevel( Level baseLevel, AllKoudaiFlatFrmData data, double OhbikiH,
      double NedaH )
    {
      double retOffset = 0 ;
      if ( data.BaseLevel == eBaseLevel.FukkouTop ) {
        if ( ! data.pilePillerData.IsCut ) {
          double upperHeigh = NedaH + ( data.HasFukkouban ? FukkouBAN_THICK : 0 ) ;
          if ( data.ohbikiData.isHkou ) {
            upperHeigh += OhbikiH ;
          }

          retOffset = -( upperHeigh ) + data.LevelOffset ;
        }
        else if ( data.pilePillerData.IsCut ) {
          double dif = ( NedaH + ( data.HasFukkouban ? FukkouBAN_THICK : 0 ) ) - data.LevelOffset +
                       ( data.ohbikiData.isHkou ? OhbikiH : 0 ) ;
          if ( dif < 0 ) {
            retOffset = Math.Abs( dif ) ;
          }
        }
      }
      else if ( data.BaseLevel == eBaseLevel.OhbikiBtm ) {
        if ( data.pilePillerData.IsCut && ! data.pilePillerData.ExtensionPile && data.LevelOffset >= 0 ) {
          retOffset = data.LevelOffset ;
        }
        else if ( ! data.pilePillerData.IsCut && ! data.pilePillerData.ExtensionPile ) {
          retOffset = data.LevelOffset ;
        }
        else {
          retOffset = 0 ;
        }
      }

      return retOffset ;
    }

    /// <summary>
    /// 柱配置
    /// </summary>
    /// <returns></returns>
    public static ElementId CreatePiller( Document doc, XYZ point, double angle, string type, PilePillerData data,
      string koudaiName, string BPLdata, bool hasBpl, Level level = null, double leveloffset = 0,
      FamilySymbol orSym = null )
    {
      ElementId retId = ElementId.InvalidElementId ;
      FamilySymbol sym = orSym ;
      if ( orSym == null ) {
        string familyPath = Master.ClsSichuCsv.GetFamilyPath( data.PillarSize ) ;
        if ( ! GantryUtil.GetFamilySymbol( doc, familyPath, sichuTypeName, out sym, true ) ) {
          return ElementId.InvalidElementId ;
        }

        sym = GantryUtil.DuplicateTypeWithNameRule( doc, koudaiName, sym, sichuTypeName ) ;
      }


      //パラメータの組み合わせ作成
      Dictionary<string, string> paramList = new Dictionary<string, string>() ;
      paramList.Add( DefineUtil.PARAM_LENGTH, data.PillarWholeLength.ToString() ) ;
      paramList.Add( DefineUtil.PARAM_MATERIAL, data.PillarMaterial ) ;

      if ( type == Master.ClsSichuCsv.HKou ) {
        string jointF = GantryUtil.GetJointTypeName( data.jointDetailData.JointType, data.PillarSize ) ;
        FamilySymbol jSym = RevitUtil.ClsRevitUtil.GetFamilySymbol( doc, jointF, jointF ) ;
        if ( jSym != null ) {
          paramList.Add( DefineUtil.PARAM_JOINT_TYPE, jSym.Id.ToString() ) ;
        }

        //トッププレート
        paramList.Add( DefineUtil.PARAM_TOP_PLATE,
          ( data.HasTopPlate )
            ? ( (int) DefineUtil.PramYesNo.Yes ).ToString()
            : ( (int) DefineUtil.PramYesNo.No ).ToString() ) ;
        MaterialSize plsize = GantryUtil.GetKouzaiSize( data.topPlateData.PlateSize ) ;
        paramList.Add( DefineUtil.PARAM_TOP_PLATE_W, plsize.Width.ToString() ) ;
        paramList.Add( DefineUtil.PARAM_TOP_PLATE_D, plsize.Height.ToString() ) ;
        paramList.Add( DefineUtil.PARAM_TOP_PLATE_T, plsize.Thick.ToString() ) ;

        double cutLengh = data.PileCutLength ;
        if ( data.HasTopPlate && data.IsCut ) {
          cutLengh += plsize.Thick ;
        }

        paramList.Add( DefineUtil.PARAM_PILE_CUT_LENG, ( data.IsCut ) ? $"{data.PileCutLength}" : "0" ) ;

        string plateName = "ﾄｯﾌﾟﾌﾟﾚｰﾄ_定形" ;
        string plateType = ( data.extensionPileData.topPlateData.PlateSize == "任意" )
          ? "ﾄｯﾌﾟﾌﾟﾚｰﾄ_任意"
          : data.topPlateData.PlateSize ;
        FamilySymbol pltSym = RevitUtil.ClsRevitUtil.GetFamilySymbolWithFuzzy( doc, plateName, plateType ) ;
        if ( pltSym == null ) {
          pltSym = RevitUtil.ClsRevitUtil.GetFamilySymbolWithFuzzy( doc, "ﾄｯﾌﾟﾌﾟﾚｰﾄ_任意", "ﾄｯﾌﾟﾌﾟﾚｰﾄ_任意" ) ;
        }

        if ( pltSym != null ) {
          paramList.Add( DefineUtil.PARAM_TOP_PLATE_SIZE, pltSym.Id.ToString() ) ;
        }

        //ベースプレート
        string[] bpl = BPLdata.Split( '_' ) ;
        paramList.Add( DefineUtil.PARAM_BASE_PLATE,
          ( hasBpl ) ? ( (int) DefineUtil.PramYesNo.Yes ).ToString() : ( (int) DefineUtil.PramYesNo.No ).ToString() ) ;
        if ( bpl.Length == 2 ) {
          pltSym = RevitUtil.ClsRevitUtil.GetFamilySymbolWithFuzzy( doc, bpl[ 0 ], bpl[ 1 ] ) ;
          if ( pltSym != null ) {
            paramList.Add( DefineUtil.PARAM_BASE_PLATE_SIZE, pltSym.Id.ToString() ) ;
          }
        }

        //継ぎ手
        paramList.Add( DefineUtil.PARAM_JOINT_COUNT, data.pJointCount.ToString() ) ;
        ;
        for ( int i = 0 ; i < data.pJointPitch.Count ; i++ ) {
          if ( data.pJointPitch[ i ].Equals( 0 ) ) {
            continue ;
          }

          paramList.Add( $"{i + 1}段目長さ", data.pJointPitch[ i ].ToString() ) ;
        }

        //高さ調整
        if ( /*!data.ExtensionPile &&*/ data.HasTopPlate && ! data.IsCut ) {
          leveloffset -= plsize.Thick ;
        }

        //if (data.ExtensionPile)
        //{
        //    leveloffset -= data.extensionPileData.Length;
        //}

        if ( ! data.ExtensionPile && data.IsCut ) {
          //leveloffset = leveloffset + Math.Abs(data.PileCutLength);
        }

        if ( data.exPillarHeadLeng != 0 ) {
          leveloffset += data.exPillarHeadLeng ;
        }
      }

      using ( SubTransaction tr = new SubTransaction( doc ) ) {
        tr.Start() ;
        ElementId CreatedID = RevitUtil.ClsRevitUtil.Create( doc, point, level.Id, sym ) ;
        //sym = GantryUtil.ChangeInstanceTypeID(doc, sym, CreatedID, GantryUtil.CreateTypeName(typeName, paramList));
        RevitUtil.ClsRevitUtil.RotateElement( doc, CreatedID, Line.CreateBound( point, point + XYZ.BasisZ ), angle ) ;

        RevitUtil.ClsRevitUtil.SetParameter( doc, CreatedID, DefineUtil.PARAM_BASE_OFFSET,
          RevitUtil.ClsRevitUtil.CovertToAPI( leveloffset ) ) ;

        //タイプパラメータ設定
        foreach ( KeyValuePair<string, string> kv in paramList ) {
          GantryUtil.SetParameterValueByParameterName( doc.GetElement( CreatedID ) as FamilyInstance, kv.Key,
            kv.Value ) ;
        }

        retId = CreatedID ;
        tr.Commit() ;
      }

      return retId ;
    }

    /// <summary>
    /// 基準レベルを指定した際の継足し杭のオフセット値
    /// </summary>
    /// <param name="baseLevel"></param>
    /// <param name="bLevel"></param>
    /// <param name="offset"></param>
    /// <param name="OhbikiH"></param>
    /// <param name="NedaH"></param>
    /// <returns></returns>
    public static double CalcLevelOffsetExtensionPileLevel( Level baseLevel, eBaseLevel bLevel, double offset,
      double OhbikiH, double NedaH )
    {
      double retOffset = offset ;
      if ( bLevel == eBaseLevel.FukkouTop ) {
        retOffset = -( DefineUtil.FukkouBAN_THICK + OhbikiH + NedaH ) + offset ;
      }

      return retOffset ;
    }

    /// <summary>
    /// 切断長さ計算
    /// </summary>
    /// <returns></returns>
    public static double CalcPileCutLength( Document doc, AllKoudaiFlatFrmData data, PilePillerData pData = null )
    {
      double fukkouThick = ( data.HasFukkouban ? FukkouBAN_THICK : 0 ) ;
      double nedaHeight = GantryUtil.GetKouzaiSize( data.nedaData.NedaSize ).Height ;
      double ohbikiHeight = GantryUtil.GetKouzaiSize( data.ohbikiData.OhbikiSize ).Height ;
      //double topPLthick = GantryUtil.GetKouzaiSize(data.pilePillerData.topPlateData.PlateSize).Thick;
      double offset = data.LevelOffset ;
      PilePillerData pileData = ( pData == null ) ? data.pilePillerData : pData ;
      double retLeng = 0 ;

      if ( data.BaseLevel == eBaseLevel.FukkouTop ) {
        if ( pileData.IsCut ) {
          if ( data.LevelOffset >= 0 ) {
            retLeng = ( fukkouThick + nedaHeight + ( data.ohbikiData.isHkou ? ohbikiHeight : 0 ) ) - offset ;
          }
          else {
            retLeng = ( fukkouThick + nedaHeight + ( data.ohbikiData.isHkou ? ohbikiHeight : 0 ) ) +
                      Math.Abs( offset ) ;
          }
        }
        else {
          retLeng = 0 ;
        }
      }
      else {
        if ( pileData.IsCut ) {
          if ( pileData.ExtensionPile ) {
            if ( data.LevelOffset >= 0 ) {
              retLeng = pileData.extensionPileData.Length - offset ;
            }
            else {
              retLeng = retLeng = pileData.extensionPileData.Length + Math.Abs( offset ) ;
            }
          }
          else {
            if ( data.LevelOffset >= 0 ) {
              retLeng = 0 ;
            }
            else {
              retLeng = Math.Abs( offset ) ;
            }
          }
        }
        else {
          retLeng = 0 ;
        }
      }

      if ( retLeng < 0 ) {
        retLeng = 0 ;
      }

      return retLeng ;
    }


    public static List<ElementId> CollectExtensionPile( PType pType )
    {
      List<ElementId> retlist = new List<ElementId>() ;
      try {
      }
      catch ( Exception ) {
        retlist = new List<ElementId>() ;
      }

      return retlist ;
    }
  }

  public enum PType
  {
    Pile = 0,
    ExPile = 1,
    Piller = 2
  }

  [Serializable]
  [System.Xml.Serialization.XmlRoot( "pilePillerData" )]
  public class PilePillerData
  {
    /// <summary>
    /// 材質
    /// </summary>
    [System.Xml.Serialization.XmlElement( "PillarMaterial" )]
    public string PillarMaterial { get ; set ; }

    /// <summary>
    /// 杭タイプ
    /// </summary>
    [System.Xml.Serialization.XmlElement( "PillarType" )]
    public string PillarType { get ; set ; }

    /// <summary>
    /// 杭サイズ
    /// </summary>
    [System.Xml.Serialization.XmlElement( "PillarSize" )]
    public string PillarSize { get ; set ; }

    /// <summary>
    /// 杭長
    /// </summary>
    [System.Xml.Serialization.XmlElement( "PillarLength" )]
    public double PillarLength { get ; set ; }

    /// <summary>
    /// 支持杭長
    /// </summary>
    [System.Xml.Serialization.XmlElement( "PillarWholeLength" )]
    public double PillarWholeLength { get ; set ; }

    /// <summary>
    /// 杭切断有無
    /// </summary>
    [System.Xml.Serialization.XmlElement( "IsCut" )]
    public bool IsCut { get ; set ; }

    /// <summary>
    /// 杭切断有無
    /// </summary>
    [System.Xml.Serialization.XmlElement( "PileCutLength" )]
    public double PileCutLength { get ; set ; }

    /// <summary>
    /// 柱橋長ピッチ
    /// </summary>
    [System.Xml.Serialization.XmlElement( "KyoutyouPillarNum" )]
    public int KyoutyouPillarNum { get ; set ; }

    /// <summary>
    /// 柱幅員ピッチ
    /// </summary>
    [System.Xml.Serialization.XmlElement( "HukuinPillarNum" )]
    public int HukuinPillarNum { get ; set ; }

    /// <summary>
    /// 突出量
    /// </summary>
    [System.Xml.Serialization.XmlElement( "exPillarHeadLeng" )]
    public double exPillarHeadLeng { get ; set ; }

    /// <summary>
    /// ジョイント間隔
    /// </summary>
    [System.Xml.Serialization.XmlArray( "ListpJointPitch" )]
    [System.Xml.Serialization.XmlArrayItem( "pJointPitch" )]
    public List<double> pJointPitch { get ; set ; }

    /// <summary>
    /// ジョイント数
    /// </summary>
    [System.Xml.Serialization.XmlElement( "pJointCount" )]
    public int pJointCount { get ; set ; }

    /// <summary>
    /// ジョイント詳細
    /// </summary>
    [System.Xml.Serialization.XmlElement( "JointDetailData" )]
    public JointDetailData jointDetailData { get ; set ; }

    /// <summary>
    /// トッププレート有無
    /// </summary>
    [System.Xml.Serialization.XmlElement( "HasTopPlate" )]
    public bool HasTopPlate { get ; set ; }

    /// <summary>
    /// トッププレートデータ
    /// </summary>
    [System.Xml.Serialization.XmlElement( "topPlateData" )]
    public TopPlateData topPlateData { get ; set ; }

    /// <summary>
    /// 継足し杭有無
    /// </summary>
    [System.Xml.Serialization.XmlElement( "ExtensionPile" )]
    public bool ExtensionPile { get ; set ; }

    /// <summary>
    /// 継足し杭データ
    /// </summary>
    [System.Xml.Serialization.XmlElement( "extensionPileData" )]
    public ExtensionPileData extensionPileData { get ; set ; }

    public PilePillerData()
    {
      IsCut = false ;
      PillarMaterial = "SS400" ;
      PillarType = "" ;
      PillarSize = "" ;
      PillarLength = 0 ;
      PillarWholeLength = 0 ;
      PileCutLength = 0 ;
      KyoutyouPillarNum = int.MinValue ;
      HukuinPillarNum = int.MinValue ;
      exPillarHeadLeng = 0 ;
      pJointCount = 0 ;
      pJointPitch = new List<double>() ;
      jointDetailData = new JointDetailData() ;
      HasTopPlate = false ;
      topPlateData = new TopPlateData() ;
      ExtensionPile = false ;
      extensionPileData = new ExtensionPileData() ;
    }
  }

  [Serializable]
  [System.Xml.Serialization.XmlRoot( "jointDetailData" )]
  public class JointDetailData
  {
    /// <summary>
    /// ジョイント方法
    /// </summary>
    [System.Xml.Serialization.XmlElement( "JointType" )]
    public eJoinType JointType { get ; set ; }

    /// <summary>
    /// プレート（フランジ外側）
    /// </summary>
    [System.Xml.Serialization.XmlElement( "PlateFlangeOutSide" )]
    public JointPlateData PlateFlangeOutSide { get ; set ; }

    /// <summary>
    /// プレート（フランジ内側）
    /// </summary>
    [System.Xml.Serialization.XmlElement( "PlateFlangeInSide" )]
    public JointPlateData PlateFlangeInSide { get ; set ; }

    /// <summary>
    /// プレート（WEB側）
    /// </summary>
    [System.Xml.Serialization.XmlElement( "PlateWeb" )]
    public JointPlateData PlateWeb { get ; set ; }

    /// <summary>
    /// フランジボルト
    /// </summary>
    [System.Xml.Serialization.XmlElement( "FlangeBolt" )]
    public JointBoltData FlangeBolt { get ; set ; }

    /// <summary>
    /// ウェブ側ボルト
    /// </summary>
    [System.Xml.Serialization.XmlElement( "WebBolt" )]
    public JointBoltData WebBolt { get ; set ; }

    public JointDetailData()
    {
      JointType = eJoinType.Bolt ;
      PlateFlangeOutSide = new JointPlateData() ;
      PlateFlangeInSide = new JointPlateData() ;
      PlateWeb = new JointPlateData() ;
      FlangeBolt = new JointBoltData() ;
      WebBolt = new JointBoltData() ;
    }
  }

  [Serializable]
  [System.Xml.Serialization.XmlRoot( "topPlateData" )]
  public class TopPlateData
  {
    /// <summary>
    /// サイズ
    /// </summary>
    [System.Xml.Serialization.XmlElement( "PlateSize" )]
    public string PlateSize { get ; set ; }
  }

  [Serializable]
  [System.Xml.Serialization.XmlRoot( "extensionPileData" )]
  public class ExtensionPileData
  {
    /// <summary>
    /// 長さ
    /// </summary>
    [System.Xml.Serialization.XmlElement( "Length" )]
    public double Length { get ; set ; }

    /// <summary>
    /// 締結方法
    /// </summary>
    [System.Xml.Serialization.XmlElement( "attachType" )]
    public eJoinType attachType { get ; set ; }

    /// <summary>
    /// プレート（フランジ外側）
    /// </summary>
    [System.Xml.Serialization.XmlElement( "PlateFlangeOutSide" )]
    public JointPlateData PlateFlangeOutSide { get ; set ; }

    /// <summary>
    /// プレート（フランジ内側）
    /// </summary>
    [System.Xml.Serialization.XmlElement( "PlateFlangeInSide" )]
    public JointPlateData PlateFlangeInSide { get ; set ; }

    /// <summary>
    /// プレート（WEB側）
    /// </summary>
    [System.Xml.Serialization.XmlElement( "PlateWeb" )]
    public JointPlateData PlateWeb { get ; set ; }

    /// <summary>
    /// フランジボルト
    /// </summary>
    [System.Xml.Serialization.XmlElement( "FlangeBolt" )]
    public JointBoltData FlangeBolt { get ; set ; }

    /// <summary>
    /// ウェブ側ボルト
    /// </summary>
    [System.Xml.Serialization.XmlElement( "WebBolt" )]
    public JointBoltData WebBolt { get ; set ; }

    /// <summary>
    /// トッププレート
    /// </summary>
    [System.Xml.Serialization.XmlElement( "topPlateData" )]
    public TopPlateData topPlateData { get ; set ; }

    public ExtensionPileData()
    {
      Length = 0 ;
      attachType = eJoinType.Welding ;
      topPlateData = new TopPlateData() ;
      PlateFlangeOutSide = new JointPlateData() ;
      PlateFlangeInSide = new JointPlateData() ;
      PlateWeb = new JointPlateData() ;
      FlangeBolt = new JointBoltData() ;
      WebBolt = new JointBoltData() ;
    }
  }

  [Serializable]
  [System.Xml.Serialization.XmlRoot( "jointBoltData" )]
  public class JointBoltData
  {
    /// <summary>
    /// フランジプレートサイズ
    /// </summary>
    [System.Xml.Serialization.XmlElement( "BoltSize" )]
    public string BoltSize { get ; set ; } = "" ;

    /// <summary>
    /// 枚数
    /// </summary>
    [System.Xml.Serialization.XmlElement( "BoltCount" )]
    public int BoltCount { get ; set ; } = 0 ;
  }

  [Serializable]
  [System.Xml.Serialization.XmlRoot( "jointPlateData" )]
  public class JointPlateData
  {
    /// <summary>
    /// プレートサイズ
    /// </summary>
    [System.Xml.Serialization.XmlElement( "PlateSize" )]
    public string PlateSize { get ; set ; }

    /// <summary>
    /// プレート数（フランジ外側）
    /// </summary>
    [System.Xml.Serialization.XmlElement( "PlateCount" )]
    public int PlateCount { get ; set ; }
  }
}