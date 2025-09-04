using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using YMS.Master ;

namespace YMS.Parts
{
  public enum PutVec
  {
    Kougo,
    Const
  }

  public enum TensetuVec
  {
    HaraSide,
    SenakaSide
  }

  public enum VoidVec
  {
    Kussaku,
    Kabe
  }

  public class ClsKouyaita
  {
    #region 定数

    /// <summary>
    /// 最大ジョイント数
    /// </summary>
    public const int PileJointMax = 10 ;

    public const string KOUYAITA = "鋼矢板" ;
    public const string CORNERYAITA = "コーナー鋼矢板" ;
    public const string IKEIYAITA = "異形鋼矢板" ;
    public const string VOIDVEC = "掘削向き" ;

    #endregion


    #region プロパティ

    /// <summary>
    /// CASE番号
    /// </summary>
    public int m_case { get ; set ; }

    /// <summary>
    /// 枝番号
    /// </summary>
    public string m_edaNum { get ; set ; }

    /// <summary>
    /// 枝番号2
    /// </summary>
    public string m_edaNum2 { get ; set ; }

    /// <summary>
    /// 処理方法
    /// </summary>
    public int m_way { get ; set ; }

    /// <summary>
    /// 掘削深さ
    /// </summary>
    public int m_void { get ; set ; }

    /// <summary>
    /// 配置向き
    /// </summary>
    public PutVec m_putVec { get ; set ; }

    /// <summary>
    /// 鋼材：タイプ
    /// </summary>
    public string m_type { get ; set ; }

    /// <summary>
    /// 鋼材：サイズ
    /// </summary>
    public string m_size { get ; set ; }

    /// <summary>
    /// 鋼材：天端
    /// </summary>
    public int m_HTop { get ; set ; }

    /// <summary>
    /// 鋼材：全長
    /// </summary>
    public int m_HLen { get ; set ; }

    /// <summary>
    /// コーナー矢板：サイズ
    /// </summary>
    public string m_kouyaitaSize { get ; set ; }

    /// <summary>
    /// 材質
    /// </summary>
    public string m_zaishitu { get ; set ; }

    /// <summary>
    /// 異形矢板を使用
    /// </summary>
    public bool m_bIzyou { get ; set ; }

    /// <summary>
    /// 残置
    /// </summary>
    public string m_zanti { get ; set ; }

    /// <summary>
    /// 一部埋め殺し長さ
    /// </summary>
    public string m_zantiLength { get ; set ; }

    /// <summary>
    /// 交互配置フラグ
    /// </summary>
    public bool m_KougoFlg { get ; set ; }

    /// <summary>
    /// 箇所数１
    /// </summary>
    public int m_Kasho1 { get ; set ; }

    /// <summary>
    /// 箇所数２
    /// </summary>
    public int m_Kasho2 { get ; set ; }

    /// <summary>
    /// 固定方法1
    /// </summary>
    public Master.Kotei m_Kotei1 { get ; set ; }

    /// <summary>
    /// 固定方法2
    /// </summary>
    public Master.Kotei m_Kotei2 { get ; set ; }


    /// <summary>
    /// 杭長さ１
    /// </summary>
    public List<int> m_ListPileLength1 { get ; set ; }

    /// <summary>
    /// 杭長さ２
    /// </summary>
    public List<int> m_ListPileLength2 { get ; set ; }

    /// <summary>
    /// 添接板向き1
    /// </summary>
    public TensetuVec TensetuVec1 { get ; set ; }

    /// <summary>
    /// 添接板向き2
    /// </summary>
    public TensetuVec TensetuVec2 { get ; set ; }

    /// <summary>
    /// ボルト1(フランジ)
    /// </summary>
    public string m_BoltF1 { get ; set ; }

    /// <summary>
    /// ボルト1(フランジ数)
    /// </summary>
    public string m_BoltFNum1 { get ; set ; }

    /// <summary>
    /// ボルト1(ウェブ)
    /// </summary>
    public string m_BoltW1 { get ; set ; }

    /// <summary>
    /// ボルト1(ウェブ数)
    /// </summary>
    public string m_BoltWNum1 { get ; set ; }

    /// <summary>
    /// ボルト2(フランジ)
    /// </summary>
    public string m_BoltF2 { get ; set ; }

    /// <summary>
    /// ボルト2(フランジ数)
    /// </summary>
    public string m_BoltFNum2 { get ; set ; }

    /// <summary>
    /// ボルト2(ウェブ)
    /// </summary>
    public string m_BoltW2 { get ; set ; }

    /// <summary>
    /// ボルト2(ウェブ数)
    /// </summary>
    public string m_BoltWNum2 { get ; set ; }

    /// <summary>
    /// ボルトコーナー1(フランジ)
    /// </summary>
    public string m_BoltCornerF1 { get ; set ; }

    /// <summary>
    /// ボルトコーナー1(フランジ数)
    /// </summary>
    public string m_BoltCornerFNum1 { get ; set ; }

    /// <summary>
    /// ボルトコーナー1(ウェブ)
    /// </summary>
    public string m_BoltCornerW1 { get ; set ; }

    /// <summary>
    /// ボルトコーナー1(ウェブ数)
    /// </summary>
    public string m_BoltCornerWNum1 { get ; set ; }

    /// <summary>
    /// ボルトコーナー2(フランジ)
    /// </summary>
    public string m_BoltCornerF2 { get ; set ; }

    /// <summary>
    /// ボルトコーナー2(フランジ数)
    /// </summary>
    public string m_BoltCornerFNum2 { get ; set ; }

    /// <summary>
    /// ボルトコーナー2(ウェブ)
    /// </summary>
    public string m_BoltCornerW2 { get ; set ; }

    /// <summary>
    /// ボルトコーナー2(ウェブ数)
    /// </summary>
    public string m_BoltCornerWNum2 { get ; set ; }

    /// <summary>
    /// 杭のファミリ名称
    /// </summary>
    public string FamilyNameKui
    {
      get
      {
        return "杭_" + m_size ; // 外部から取得される
      }
    }

    /// <summary>
    /// カスタムデータを付ける際にデフォルトの向きで作成しているか
    /// </summary>
    public bool m_defaultVec { get ; set ; }

    /// <summary>
    /// 配置順
    /// </summary>
    public int m_oreder { get ; set ; }

    /// <summary>
    /// 一定方向時反転するか
    /// </summary>
    public bool m_ConstOpp { get ; set ; }

    /// <summary>
    /// 最初に作成された鋼矢板ID
    /// </summary>
    public ElementId m_FirstKouyaita { get ; set ; }

    /// <summary>
    /// 最後に1点作成された鋼矢板ID
    /// </summary>
    public ElementId m_LastKouyaita { get ; set ; }

    #endregion

    #region コンストラクタ

    public ClsKouyaita()
    {
      //初期化
      Init() ;
    }

    #endregion

    #region メソッド

    public void Init()
    {
      m_case = 0 ;
      m_edaNum = string.Empty ;
      m_edaNum2 = string.Empty ;
      m_way = 0 ;
      m_void = 0 ;
      m_putVec = PutVec.Kougo ;
      m_size = string.Empty ;
      m_HTop = 0 ;
      m_HLen = 0 ;
      m_kouyaitaSize = string.Empty ;
      m_zaishitu = string.Empty ;
      m_bIzyou = false ;
      m_zanti = string.Empty ;
      m_zantiLength = string.Empty ;

      m_KougoFlg = false ;
      m_Kasho1 = 0 ;
      m_Kasho2 = 0 ;
      m_Kotei1 = Master.Kotei.Bolt ;
      m_Kotei2 = Master.Kotei.Bolt ;
      m_ListPileLength1 = new List<int>() ;
      m_ListPileLength2 = new List<int>() ;
      m_BoltF1 = string.Empty ;
      m_BoltFNum1 = string.Empty ;
      m_BoltW1 = string.Empty ;
      m_BoltWNum1 = string.Empty ;
      m_BoltF2 = string.Empty ;
      m_BoltFNum2 = string.Empty ;
      m_BoltW2 = string.Empty ;
      m_BoltWNum2 = string.Empty ;

      m_BoltCornerF1 = string.Empty ;
      m_BoltCornerFNum1 = string.Empty ;
      m_BoltCornerW1 = string.Empty ;
      m_BoltCornerWNum1 = string.Empty ;
      m_BoltCornerF2 = string.Empty ;
      m_BoltCornerFNum2 = string.Empty ;
      m_BoltCornerW2 = string.Empty ;
      m_BoltCornerWNum2 = string.Empty ;

      TensetuVec1 = TensetuVec.HaraSide ;
      TensetuVec2 = TensetuVec.HaraSide ;

      m_defaultVec = true ;
      m_oreder = -1 ;
      m_ConstOpp = false ;

      m_FirstKouyaita = null ;
      m_LastKouyaita = null ;
      return ;
    }

    /// <summary>
    /// Kouyaitaを作図する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="picPointS"></param>
    /// <param name="picPointE"></param>
    /// <param name="voidVec">最初に作図される向き</param>
    /// <param name="bIrizumi"></param>
    /// <param name="oppMessage">反転のメッセージを表示するか（自動はしない）</param>
    /// <returns></returns>
    public bool CreateKouyaita( Document doc, XYZ picPointS, XYZ picPointE, ElementId levelId, VoidVec voidVec,
      out VoidVec lastVoidVec, bool bIrizumi = true, bool oppMessage = true )
    {
      lastVoidVec = voidVec ;
      try {
        Line soilLine = Line.CreateBound( picPointS, picPointE ) ;

        XYZ soilDirection = soilLine.Direction ; //単位ベクトル
        double length = ClsRevitUtil.CovertFromAPI( soilLine.Length ) ; //芯の長さ

        int vec = (int) voidVec ;
        int oppVec = voidVec == VoidVec.Kussaku ? 1 : 0 ;

        string familyPathKui = Master.ClsKouyaitaCsv.GetFamilyPath( m_size ) ;
        string familyNameKui = ClsRevitUtil.GetFamilyName( familyPathKui ) ;

        FamilySymbol kouya = null ;
        if ( m_type == KOUYAITA ) {
          //鋼矢板
          if ( ! ClsRevitUtil.LoadFamilyData( doc, familyPathKui, out Family kouyaFam ) ) {
            return false ;
          }

          kouya = ClsRevitUtil.GetFamilySymbol( doc, familyNameKui, KOUYAITA ) ;
        }
        else {
          //鋼矢板以外
          if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, familyPathKui, familyNameKui, out kouya ) ) {
            return false ;
          }
        }

        //コーナー矢板
        FamilySymbol corner = null ;
        if ( ! oppMessage ) //自動の時の処理
        {
          string familyPathcorner = Master.ClsKouyaitaCsv.GetFamilyPath( m_kouyaitaSize ) ;
          string familyNameCorner = ClsRevitUtil.GetFamilyName( familyPathcorner ) ;
          if ( ! ClsRevitUtil.LoadFamilyData( doc, familyPathcorner, out Family cornerFam ) ) {
            return false ;
          }

          corner = ClsRevitUtil.GetFamilySymbol( doc, familyNameCorner, KOUYAITA ) ;
        }

        FamilySymbol sym = kouya ;

        int pitch = ClsCommonUtils.ChangeStrToInt( Master.ClsKouyaitaCsv.GetLength( m_size ) ) ;

        //H形鋼
        //ClsRevitUtil.SetTypeParameter(oya, ClsGlobal.m_length, ClsRevitUtil.CovertToAPI(m_HLen));
        string stH = ClsYMSUtil.GetKouzaiSizeSunpou1( familyNameKui, 1 ) ;
        double dEx = ClsCommonUtils.ChangeStrToDbl( stH ) / 2 ;

        //角度//X軸がプラスの時回転しすぎてしまうので分岐が必要
        double dAngle = soilDirection.AngleOnPlaneTo( XYZ.BasisX, XYZ.BasisZ ) ;
        XYZ picPoint = null ;
        //グループ化するIDを保持する変数の宣言(nullだとAdd出来ないため)
        ICollection<ElementId> selectionIds = new List<ElementId>() ;
        List<ElementId> selId = new List<ElementId>() ; //ボイド用

        double dCount = length / pitch ; //ダイアログソイルピッチずつ親杭、ソイルを配置するため何個置けるか

        int kougoPut = vec ;
        int cornerPitch = 0 ;

        string newtypeName = KOUYAITA ;
        int cornerFlag = 0 ;

        using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
          t.Start() ;
          FailureHandlingOptions failOpt = t.GetFailureHandlingOptions() ;
          failOpt.SetFailuresPreprocessor( new WarningSwallower() ) ;
          t.SetFailureHandlingOptions( failOpt ) ;

          //ダイアログ指定の値をSET
          ClsRevitUtil.SetTypeParameter( sym, "切断長さ", 0.0 ) ;
          ClsRevitUtil.SetTypeParameter( sym, "トッププレート", 0 ) ;

          for ( int i = 0 ; i < dCount ; i++ ) {
            if ( ! bIrizumi && i == 0 ) {
              continue ;
            } //出隅の時は別で出隅分作成するため始点を飛ばす

            picPoint = new XYZ(
              picPointS.X + ( ClsRevitUtil.CovertToAPI( pitch * ( i + kougoPut - cornerFlag ) + cornerPitch ) *
                              soilDirection.X ),
              picPointS.Y + ( ClsRevitUtil.CovertToAPI( pitch * ( i + kougoPut - cornerFlag ) + cornerPitch ) *
                              soilDirection.Y ), picPointS.Z ) ;

            if ( ! oppMessage ) //自動の時の処理
            {
              if ( i == 0 ) {
                cornerPitch = ClsCommonUtils.ChangeStrToInt( Master.ClsKouyaitaCsv.GetLength( m_kouyaitaSize ) ) ;
                sym = corner ;
                newtypeName = CORNERYAITA ;
                cornerFlag = 1 ;
              }
              else if ( dCount < i + 1 ) //最後
              {
                sym = corner ;
                newtypeName = CORNERYAITA ;
              }
              else if ( dCount < i + 2 ) //最後の一個手前
              {
                cornerPitch *= 2 ;
                sym = kouya ;
                newtypeName = KOUYAITA ;
                cornerFlag = 2 ;
              }
              else {
                sym = kouya ;
                newtypeName = KOUYAITA ;
              }
            }


            ElementId CreatedOyaID = ClsRevitUtil.Create( doc, picPoint, levelId, sym ) ; //親杭作成
            ClsRevitUtil.SetParameter( doc, CreatedOyaID, ClsGlobal.m_refLvTop, ClsRevitUtil.CovertToAPI( m_HTop ) ) ;
            ClsVoid.SetVoidDep( doc, CreatedOyaID, m_void ) ;

            Line axis = Line.CreateBound( picPoint, picPoint + XYZ.BasisZ ) ;
            ClsRevitUtil.RotateElement( doc, CreatedOyaID, axis, -dAngle ) ;
            selectionIds.Add( CreatedOyaID ) ;
            selId.Add( CreatedOyaID ) ;

            ////交互対応
            int nKougo = ( m_KougoFlg && ( i % 2 ) == 1 ? 2 : 1 ) ;
            sym = ClsRevitUtil.ChangeTypeID( doc, sym, CreatedOyaID,
              newtypeName + "_" + m_case + "_" + m_edaNum + "_" + nKougo.ToString() ) ;

            //ジョイント数
            int nKasho = 0 ;
            if ( nKougo == 1 ) nKasho = m_Kasho1 ;
            else nKasho = m_Kasho2 ;

            ClsRevitUtil.SetTypeParameter( sym, "ジョイント数", nKasho ) ;
            if ( m_zanti == "一部埋め殺し" )
              ClsRevitUtil.SetTypeParameter( sym, "残置/引抜", m_zanti + "(" + m_zantiLength + ")" ) ;
            else
              ClsRevitUtil.SetTypeParameter( sym, "残置/引抜", m_zanti ) ;
            ClsRevitUtil.SetTypeParameter( sym, "材質", m_zaishitu ) ;

            //杭長さ
            ClsRevitUtil.SetTypeParameter( sym, "長さ", ClsRevitUtil.CovertToAPI( m_HLen ) ) ;
            List<int> pileLengths = new List<int>() ;
            if ( nKougo == 1 ) pileLengths = m_ListPileLength1 ;
            else pileLengths = m_ListPileLength2 ;

            for ( int j = 0 ; j < PileJointMax ; j++ ) {
              int nPileLength = 0 ;
              if ( j < pileLengths.Count ) {
                nPileLength = pileLengths[ j ] ;
              }

              ClsRevitUtil.SetTypeParameter( sym, "杭" + ( j + 1 ).ToString(),
                ClsRevitUtil.CovertToAPI( nPileLength ) ) ;
            }

            if ( m_putVec == PutVec.Kougo ) {
              if ( i % 2 == oppVec ) {
                ClsRevitUtil.RotateElement( doc, CreatedOyaID, axis, Math.PI ) ;
                kougoPut = 0 ;
              }
              else {
                kougoPut = 1 ;
              }
            }
          }

          //変更が加わるメッセージ原因不明
          t.Commit() ;

          if ( oppMessage ) {
            //反転
            //DialogResult dResult = MessageBox.Show("反転しますか？",
            //    "確認",
            //    MessageBoxButtons.YesNo);
            //ダイアログを表示
            YMS.DLG.DlgOppKouyaita oppKouyaita = new DLG.DlgOppKouyaita() ;
            oppKouyaita.ShowDialog() ;
            DialogResult dResult = oppKouyaita.dialogResult ;
            if ( dResult == DialogResult.Retry ) {
              t.Start() ;
              ClsRevitUtil.Delete( doc, selId ) ;


              //反転処理
              kougoPut = oppVec ;
              for ( int i = 0 ; i < dCount ; i++ ) {
                if ( ! bIrizumi && i == 0 ) {
                  continue ;
                } //出隅の時は別で出隅分作成するため始点を飛ばす

                picPoint = new XYZ(
                  picPointS.X + ( ClsRevitUtil.CovertToAPI( pitch * ( i + kougoPut ) ) * soilDirection.X ),
                  picPointS.Y + ( ClsRevitUtil.CovertToAPI( pitch * ( i + kougoPut ) ) * soilDirection.Y ),
                  picPointS.Z ) ;


                ElementId CreatedOyaID = ClsRevitUtil.Create( doc, picPoint, levelId, kouya ) ; //親杭作成
                ClsRevitUtil.SetParameter( doc, CreatedOyaID, ClsGlobal.m_refLvTop,
                  ClsRevitUtil.CovertToAPI( m_HTop ) ) ;
                ClsVoid.SetVoidDep( doc, CreatedOyaID, m_void ) ;

                Line axis = Line.CreateBound( picPoint, picPoint + XYZ.BasisZ ) ;
                ClsRevitUtil.RotateElement( doc, CreatedOyaID, axis, -dAngle ) ;
                selectionIds.Add( CreatedOyaID ) ;
                selId.Add( CreatedOyaID ) ;

                ////交互対応
                int nKougo = ( m_KougoFlg && ( i % 2 ) == 1 ? 2 : 1 ) ;
                kouya = ClsRevitUtil.ChangeTypeID( doc, kouya, CreatedOyaID,
                  KOUYAITA + "_" + m_case + "_" + m_edaNum + "_" + nKougo.ToString() ) ;

                //ジョイント数
                int nKasho = 0 ;
                if ( nKougo == 1 ) nKasho = m_Kasho1 ;
                else nKasho = m_Kasho2 ;

                ClsRevitUtil.SetTypeParameter( kouya, "ジョイント数", nKasho ) ;

                //杭長さ
                ClsRevitUtil.SetTypeParameter( kouya, "長さ", ClsRevitUtil.CovertToAPI( m_HLen ) ) ;
                List<int> pileLengths = new List<int>() ;
                if ( nKougo == 1 ) pileLengths = m_ListPileLength1 ;
                else pileLengths = m_ListPileLength2 ;

                for ( int j = 0 ; j < PileJointMax ; j++ ) {
                  int nPileLength = 0 ;
                  if ( j < pileLengths.Count ) {
                    nPileLength = pileLengths[ j ] ;
                  }

                  ClsRevitUtil.SetTypeParameter( kouya, "杭" + ( j + 1 ).ToString(),
                    ClsRevitUtil.CovertToAPI( nPileLength ) ) ;
                }

                if ( m_putVec == PutVec.Kougo ) {
                  if ( i % 2 == vec ) {
                    ClsRevitUtil.RotateElement( doc, CreatedOyaID, axis, Math.PI ) ;
                    kougoPut = 0 ;
                  }
                  else {
                    kougoPut = 1 ;
                  }
                }
              }

              t.Commit() ;
            }
          }

          //最後に作られた鋼矢板の向き
          lastVoidVec = kougoPut == 1 ? VoidVec.Kussaku : VoidVec.Kabe ;
        }
      }
      catch ( Exception ex ) {
        TaskDialog.Show( "Info Message", ex.Message ) ;
        string message = ex.Message ;
        MessageBox.Show( message ) ;
      }

      return true ;
    }

    /// <summary>
    /// Kouyaitaを作図する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="picPointS"></param>
    /// <param name="picPointE"></param>
    /// <param name="voidVec">最初に作図される向き</param>
    /// <param name="bIrizumi">出隅の時はfalse：コーナー矢板を作成しない</param>
    /// <param name="oppMessage">コーナーを作成するかのフラグ</param>
    /// <returns></returns>
    public List<ElementId> CreateKouyaita1( Document doc, XYZ picPointS, XYZ picPointE, ElementId levelId,
      VoidVec voidVec, ref VoidVec lastVoidVec, bool createCorner = false, bool bIrizumi = true,
      ElementId kabeShinId = null )
    {
      List<ElementId> createKouyaitaList = new List<ElementId>() ;
      try {
        Line line = Line.CreateBound( picPointS, picPointE ) ;

        XYZ dir = line.Direction ; //単位ベクトル
        double length = ClsRevitUtil.CovertFromAPI( line.Length ) ; //芯の長さ

        int vec = (int) voidVec ;
        int oppVec = voidVec == VoidVec.Kussaku ? 1 : 0 ;

        string familyPathKui = Master.ClsKouyaitaCsv.GetFamilyPath( m_size ) ;
        string familyNameKui = ClsRevitUtil.GetFamilyName( familyPathKui ) ;

        FamilySymbol kouya = null ;
        if ( m_type == KOUYAITA ) {
          //鋼矢板
          if ( ! ClsRevitUtil.LoadFamilyData( doc, familyPathKui, out Family kouyaFam ) ) {
            return null ;
          }

          kouya = ClsRevitUtil.GetFamilySymbol( doc, familyNameKui, KOUYAITA ) ;
          if ( kouya == null ) {
            //鋼矢板以外
            if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, familyPathKui, familyNameKui, out kouya ) ) {
              return null ;
            }
          }
        }
        else {
          //鋼矢板以外
          if ( ! ClsRevitUtil.LoadFamilySymbolData( doc, familyPathKui, familyNameKui, out kouya ) ) {
            return null ;
          }
        }

        //コーナー矢板
        FamilySymbol corner = null ;
        if ( createCorner && m_kouyaitaSize != "なし" ) {
          string familyPathcorner = Master.ClsKouyaitaCsv.GetFamilyPath( m_kouyaitaSize ) ;
          string familyNameCorner = ClsRevitUtil.GetFamilyName( familyPathcorner ) ;
          if ( ! ClsRevitUtil.LoadFamilyData( doc, familyPathcorner, out Family cornerFam ) ) {
            //return null;
          }

          //下記の場合コーナーは作成しないがコーナー矢板は読み込んで置く
          if ( m_size.Contains( "J" ) || m_size.Contains( "L" ) || m_way != 0 ) {
            corner = null ;
          }
          else {
            corner = ClsRevitUtil.GetFamilySymbol( doc, familyNameCorner, KOUYAITA ) ;
          }
        }

        //異形矢板
        FamilySymbol ikei = null ;
        if ( m_bIzyou && ( m_size == "SP-3" || m_size == "SP-4" ) && m_way == 0 ) {
          string symbolFolpath = ClsZumenInfo.GetYMSFolder() ;

          string familyPathIkei =
            System.IO.Path.Combine( symbolFolpath, "山留壁関係\\02_鋼矢板\\ﾌｧﾐﾘ\\ﾊﾞﾁ矢板_" + m_size + ".rfa" ) ;
          string familyNameIkei = ClsRevitUtil.GetFamilyName( familyPathIkei ) ;
          if ( ! ClsRevitUtil.LoadFamilyData( doc, familyPathIkei, out Family ikeiFamily ) ) {
            return null ;
          }

          ikei = ClsRevitUtil.GetFamilySymbol( doc, familyNameIkei, KOUYAITA ) ;
        }


        int pitch = ClsCommonUtils.ChangeStrToInt( Master.ClsKouyaitaCsv.GetLength( m_size ) ) ;
        //角度
        double dAngle = dir.AngleOnPlaneTo( XYZ.BasisX, XYZ.BasisZ ) ;

        XYZ picPoint = null ;
        //グループ化するIDを保持する変数の宣言(nullだとAdd出来ないため)
        ICollection<ElementId> selectionIds = new List<ElementId>() ;
        List<ElementId> selId = new List<ElementId>() ; //ボイド用

        double addIkeiyaita = ClsGeo.FloorAtDigitAdjust( 1, length ) % pitch ;
        double dCount = ClsGeo.FloorAtDigitAdjust( 1, length ) / pitch ; //配置個数

        int nCount = (int) dCount ;
        if ( nCount < dCount && ikei == null ) //異形矢板にチェックがある場合は増やさない
          nCount += 1 ;
        if ( ikei != null && 200 < addIkeiyaita )
          nCount += 1 ;
        int createType = nCount % 2 ;

        //異形矢板長さ決め
        if ( addIkeiyaita <= 200 )
          addIkeiyaita += pitch ;

        int kougoPut = vec ;
        if ( createCorner && lastVoidVec == VoidVec.Kussaku ) //createCorner && 
        {
          kougoPut = oppVec ;
          oppVec = vec ;
        }

        FamilySymbol sym = kouya ;
        string newtypeName = m_type ;

        int cornerPitch = 0 ;
        int cornerFlag = 0 ;
        int ikeiFlag = 0 ;
        double ikeiPitch = 0.0 ;
        bool ikeiFirst = true ;

        bool lastCornerFlag = false ;
        //コーナー継手
        string cornerTsugite = string.Empty ;
        int cornerNumeric = ClsCommonUtils.ConvertToNumeric( m_edaNum ) ;
        cornerNumeric += 1 ;
        string cornerEdaNum = ClsCommonUtils.ConvertToAlphabet( cornerNumeric ) ;

        //一定作図の時
        if ( ! m_defaultVec && m_way != 0 && m_putVec == PutVec.Const ) {
          kougoPut = 1 ;
        }
        else if ( m_way != 0 && m_putVec == PutVec.Const ) {
          kougoPut = 0 ;
        }

        int kougoCheck = 0 ;

        using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
          t.Start() ;
          FailureHandlingOptions failOpt = t.GetFailureHandlingOptions() ;
          failOpt.SetFailuresPreprocessor( new WarningSwallower() ) ;
          t.SetFailureHandlingOptions( failOpt ) ;
          if ( kabeShinId != null )
            ClsRevitUtil.SetMojiParameter( doc, kabeShinId, "壁", ClsKouyaita.KOUYAITA ) ;
          //ダイアログ指定の値をSET
          ClsRevitUtil.SetTypeParameter( kouya, "切断長さ", 0.0 ) ;
          ClsRevitUtil.SetTypeParameter( kouya, "トッププレート", 0 ) ;

          int putPtn = 0 ;
          for ( int i = 0 ; i < nCount ; i++ ) {
            picPoint = new XYZ(
              picPointS.X +
              ( ClsRevitUtil.CovertToAPI( pitch * ( i + kougoPut - cornerFlag - ikeiFlag ) + cornerPitch + ikeiPitch ) *
                dir.X ),
              picPointS.Y +
              ( ClsRevitUtil.CovertToAPI( pitch * ( i + kougoPut - cornerFlag - ikeiFlag ) + cornerPitch + ikeiPitch ) *
                dir.Y ), picPointS.Z ) ;


            string boltOrTugite = string.Empty ;
            if ( m_Kotei1 == Kotei.Bolt ) {
              boltOrTugite = "ﾎﾞﾙﾄ" ;
            }
            else {
              boltOrTugite = "溶接" ;
            }

            string haraOrSenaka = string.Empty ;
            string haraOrSenakaType = string.Empty ;
            if ( TensetuVec1 == TensetuVec.HaraSide ) {
              haraOrSenaka = "腹側" ;
              haraOrSenakaType = "内側補強" ;
            }
            else {
              haraOrSenaka = "背中側" ;
              haraOrSenakaType = "外側補強" ;
            }

            string tugitename = familyNameKui.Replace( "鋼矢板_", "" ) + boltOrTugite + "継手_" + haraOrSenaka ;

            //ボルト
            string boltF = m_BoltF1 ;
            string boltFNum = m_BoltFNum1 ;
            string boltW = m_BoltW1 ;
            string boltWNum = m_BoltWNum1 ;

            if ( createCorner && corner != null ) //自動の時の処理
            {
              if ( i == 0 && lastVoidVec == VoidVec.Kussaku && bIrizumi ) {
                cornerFlag += 1 ;
                cornerPitch = ClsCommonUtils.ChangeStrToInt( Master.ClsKouyaitaCsv.GetLength( m_kouyaitaSize ) ) *
                              cornerFlag ;
                sym = corner ;
                newtypeName = CORNERYAITA ;
                //コーナー継手左
                cornerTsugite = "右" ;
                tugitename = corner.FamilyName.Replace( "鋼矢板_", "" ) + boltOrTugite + "継手_" + haraOrSenaka ;
                cornerNumeric = ClsCommonUtils.ConvertToNumeric( cornerEdaNum ) ;
                cornerNumeric += 1 ;
                cornerEdaNum = ClsCommonUtils.ConvertToAlphabet( cornerNumeric ) ;
              }
              else if ( nCount <= i + 1 &&
                        ( createType == 0 && lastCornerFlag ||
                          ( createType == 1 && lastVoidVec == VoidVec.Kussaku ) ) && bIrizumi ) //最後
              {
                sym = corner ;
                newtypeName = CORNERYAITA ;
                //コーナー継手右
                cornerTsugite = "左" ;
                tugitename = corner.FamilyName.Replace( "鋼矢板_", "" ) + boltOrTugite + "継手_" + haraOrSenaka ;
                cornerNumeric = ClsCommonUtils.ConvertToNumeric( cornerEdaNum ) ;
                cornerNumeric += 1 ;
                cornerEdaNum = ClsCommonUtils.ConvertToAlphabet( cornerNumeric ) ;
              }
              else if ( nCount <= i + 2 &&
                        ( createType == 0 && cornerFlag == 0 ||
                          ( createType == 1 && lastVoidVec == VoidVec.Kussaku ) ) && bIrizumi ) //最後の一個手前
              {
                lastCornerFlag = true ;
                cornerFlag += 1 ;
                cornerPitch = ClsCommonUtils.ChangeStrToInt( Master.ClsKouyaitaCsv.GetLength( m_kouyaitaSize ) ) *
                              cornerFlag ;
                sym = kouya ;
                newtypeName = m_type ;
                cornerTsugite = string.Empty ;
              }
              else if ( nCount <= i + 3 && ikei != null && ikeiFirst ) //異形矢板
              {
                ikeiFirst = false ;
                ikeiPitch = addIkeiyaita ;
                ikeiFlag = 1 ;
                sym = ikei ;
                newtypeName = IKEIYAITA ;
                cornerTsugite = string.Empty ;
                cornerNumeric = ClsCommonUtils.ConvertToNumeric( cornerEdaNum ) ;
                cornerNumeric += 1 ;
                cornerEdaNum = ClsCommonUtils.ConvertToAlphabet( cornerNumeric ) ;
              }
              else if ( nCount <= i + 4 && ikei != null && ikeiFirst ) //異形矢板手前処理
              {
                if ( kougoPut != 1 ) {
                  ikeiFlag = 1 ;
                  ikeiPitch = addIkeiyaita ;
                }

                sym = kouya ;
                newtypeName = m_type ;
                cornerTsugite = string.Empty ;
              }
              else {
                sym = kouya ;
                newtypeName = m_type ;
                cornerTsugite = string.Empty ;
              }
            }

            ElementId CreatedD = ClsRevitUtil.Create( doc, picPoint, levelId, sym ) ; //鋼矢板作成
            ClsRevitUtil.SetParameter( doc, CreatedD, ClsGlobal.m_refLvTop, ClsRevitUtil.CovertToAPI( m_HTop ) ) ;
            ClsVoid.SetVoidDep( doc, CreatedD, m_void ) ;
            SetVoidVec( doc, CreatedD, kougoPut ) ;
            ClsKabeShin.SetKabeShinId( doc, CreatedD, kabeShinId ) ;
            createKouyaitaList.Add( CreatedD ) ;
            ClsRevitUtil.CustomDataSet( doc, CreatedD, "ピッチ", pitch ) ;

            ClsKabeShin.SetPutOrder( doc, CreatedD, putPtn ) ;
            if ( putPtn == 0 )
              putPtn = 1 ;
            else
              putPtn = 0 ;

            Line axis = Line.CreateBound( picPoint, picPoint + XYZ.BasisZ ) ;
            ClsRevitUtil.RotateElement( doc, CreatedD, axis, -dAngle ) ;
            selectionIds.Add( CreatedD ) ;
            selId.Add( CreatedD ) ;

            ////交互対応
            string edaban = m_edaNum ;
            //ジョイント数
            int nKasho = m_Kasho1 ;
            Kotei kotei = m_Kotei1 ;
            //杭長さ
            List<int> pileLengths = m_ListPileLength1 ;

            if ( m_KougoFlg && ( i % 2 ) == 1 ) //交互時の枝番タブ２
            {
              edaban = m_edaNum2 ;
              nKasho = m_Kasho2 ;
              kotei = m_Kotei2 ;
              pileLengths = m_ListPileLength2 ;
              boltF = m_BoltF2 ;
              boltFNum = m_BoltFNum2 ;
              boltW = m_BoltW2 ;
              boltWNum = m_BoltWNum2 ;
            }

            //コーナー矢板用枝番
            if ( cornerTsugite != string.Empty ) {
              boltF = m_BoltCornerF1 ;
              boltFNum = m_BoltCornerFNum1 ;
              boltW = m_BoltCornerW1 ;
              boltWNum = m_BoltCornerWNum1 ;
              edaban = cornerEdaNum ;
            }

            //異形矢板
            if ( newtypeName == IKEIYAITA ) {
              boltF = m_BoltCornerF1 ;
              boltFNum = m_BoltCornerFNum1 ;
              boltW = m_BoltCornerW1 ;
              boltWNum = m_BoltCornerWNum1 ;
              edaban = cornerEdaNum ;
              ClsRevitUtil.SetParameter( doc, CreatedD, "W", ClsRevitUtil.CovertToAPI( addIkeiyaita ) ) ;
            }

            ClsYMSUtil.SetKotei( doc, CreatedD, kotei ) ;
            ClsYMSUtil.SetBoltFlange( doc, CreatedD, boltF, ClsCommonUtils.ChangeStrToInt( boltFNum ) ) ;
            ClsYMSUtil.SetBoltWeb( doc, CreatedD, boltW, ClsCommonUtils.ChangeStrToInt( boltWNum ) ) ;

            sym = ClsRevitUtil.ChangeTypeID( doc, sym, CreatedD,
              newtypeName + cornerTsugite + "_" + m_case + "_" + edaban ) ;
            ClsRevitUtil.SetTypeParameter( sym, "CASE", m_case.ToString() ) ;
            ClsRevitUtil.SetTypeParameter( sym, "枝番", edaban ) ;

            ClsRevitUtil.SetTypeParameter( sym, "ジョイント数", nKasho ) ;
            ClsRevitUtil.SetTypeParameter( sym, "ピッチ", pitch.ToString() ) ;
            if ( m_zanti == "一部埋め殺し" )
              ClsRevitUtil.SetTypeParameter( sym, "残置/引抜", m_zanti + "(" + m_zantiLength + ")" ) ;
            else
              ClsRevitUtil.SetTypeParameter( sym, "残置/引抜", m_zanti ) ;

            ClsRevitUtil.SetTypeParameter( sym, "材質", m_zaishitu ) ;


            FamilySymbol tugiteSym = ClsRevitUtil.GetFamilySymbol( doc, tugitename, haraOrSenakaType ) ;
            if ( tugiteSym != null ) {
              ClsRevitUtil.SetTypeParameter( sym, "継手", tugiteSym.Id ) ;
            }


            if ( newtypeName == CORNERYAITA ) {
              if ( cornerTsugite == "右" ) {
                ClsRevitUtil.SetTypeParameter( sym, "左側", 0 ) ;
                ClsRevitUtil.SetTypeParameter( sym, "右側", 1 ) ;
              }
            }

            //杭長さ
            ClsRevitUtil.SetTypeParameter( sym, "長さ", ClsRevitUtil.CovertToAPI( m_HLen ) ) ;

            for ( int j = 0 ; j < PileJointMax ; j++ ) {
              int nPileLength = 0 ;
              if ( j < pileLengths.Count ) {
                nPileLength = pileLengths[ j ] ;
              }

              ClsRevitUtil.SetTypeParameter( sym, "杭" + ( j + 1 ).ToString(),
                ClsRevitUtil.CovertToAPI( nPileLength ) ) ;
            }

            if ( m_putVec == PutVec.Kougo ) {
              if ( i % 2 == oppVec ) {
                ClsRevitUtil.RotateElement( doc, CreatedD, axis, Math.PI ) ;
                kougoPut = 0 ;
              }
              else {
                kougoPut = 1 ;
              }
            }
            else {
              if ( ! m_defaultVec ) {
                ClsRevitUtil.RotateElement( doc, CreatedD, axis, Math.PI ) ;
              }
            }

            if ( m_FirstKouyaita == null && kougoPut == 1 ) {
              m_FirstKouyaita = CreatedD ;
              kougoCheck = kougoPut ;
            }

            if ( kougoCheck == kougoPut )
              m_LastKouyaita = CreatedD ;
          }

          //変更が加わるメッセージ原因不明
          t.Commit() ;

          //最後に作られた鋼矢板の向き
          lastVoidVec = kougoPut == 1 ? VoidVec.Kussaku : VoidVec.Kabe ;
        }
      }
      catch ( Exception ex ) {
        TaskDialog.Show( "Info Message", ex.Message ) ;
        string message = ex.Message ;
        MessageBox.Show( message ) ;
      }

      return createKouyaitaList ;
    }

    /// <summary>
    /// Kouyaitaを作図する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="picPointS"></param>
    /// <param name="picPointE"></param>
    /// <param name="soilDirection"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public ElementId CreateSingleKouyaita( Document doc, XYZ picPoint, XYZ dir, ElementId levelId, VoidVec lastVoidVec,
      ElementId kabeShinId = null )
    {
      ElementId kouyaita = null ;
      try {
        string familyPathKui = Master.ClsKouyaitaCsv.GetFamilyPath( m_size ) ;
        string familyNameKui = ClsRevitUtil.GetFamilyName( familyPathKui ) ;

        //鋼矢板
        if ( ! ClsRevitUtil.LoadFamilyData( doc, familyPathKui, out Family kouyaFam ) ) {
          return null ;
        }

        FamilySymbol kouya = ClsRevitUtil.GetFamilySymbol( doc, familyNameKui, KOUYAITA ) ;

        int pitch = ClsCommonUtils.ChangeStrToInt( Master.ClsKouyaitaCsv.GetLength( m_size ) ) ;
        //角度
        double dAngle = dir.AngleOnPlaneTo( XYZ.BasisX, XYZ.BasisZ ) ;
        FamilySymbol sym = kouya ;
        string newtypeName = m_type ;

        //コーナー継手
        string cornerTsugite = string.Empty ;

        //コーナー矢板
        FamilySymbol corner = null ;
        string familyPathcorner = Master.ClsKouyaitaCsv.GetFamilyPath( m_kouyaitaSize ) ;
        string familyNameCorner = ClsRevitUtil.GetFamilyName( familyPathcorner ) ;
        if ( m_kouyaitaSize != "なし" ) {
          if ( ! ClsRevitUtil.LoadFamilyData( doc, familyPathcorner, out Family cornerFam ) ) {
            //return null;
          }

          corner = ClsRevitUtil.GetFamilySymbol( doc, familyNameCorner, KOUYAITA ) ;

          if ( lastVoidVec == VoidVec.Kussaku ) {
            sym = corner ;
            newtypeName = CORNERYAITA ;
            //コーナー継手左
            cornerTsugite = "右" ;
          }
          else if ( lastVoidVec == VoidVec.Kabe ) {
            sym = corner ;
            newtypeName = CORNERYAITA ;
            //コーナー継手右
            cornerTsugite = "左" ;
          }
        }

        using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
          t.Start() ;
          FailureHandlingOptions failOpt = t.GetFailureHandlingOptions() ;
          failOpt.SetFailuresPreprocessor( new WarningSwallower() ) ;
          t.SetFailureHandlingOptions( failOpt ) ;

          //ダイアログ指定の値をSET
          ClsRevitUtil.SetTypeParameter( kouya, "切断長さ", 0.0 ) ;
          ClsRevitUtil.SetTypeParameter( kouya, "トッププレート", 0 ) ;


          ElementId CreatedD = ClsRevitUtil.Create( doc, picPoint, levelId, sym ) ; //鋼矢板作成
          ClsRevitUtil.SetParameter( doc, CreatedD, ClsGlobal.m_refLvTop, ClsRevitUtil.CovertToAPI( m_HTop ) ) ;
          ClsVoid.SetVoidDep( doc, CreatedD, m_void ) ;
          SetVoidVec( doc, CreatedD, lastVoidVec == VoidVec.Kussaku ? 1 : 0 ) ;
          kouyaita = CreatedD ;

          ClsKabeShin.SetPutOrder( doc, CreatedD, m_oreder ) ;
          ClsKabeShin.SetKabeShinId( doc, CreatedD, kabeShinId ) ;

          Line axis = Line.CreateBound( picPoint, picPoint + XYZ.BasisZ ) ;
          ClsRevitUtil.RotateElement( doc, CreatedD, axis, -dAngle ) ;


          ////交互対応
          string edaban = m_edaNum ;
          //ジョイント数
          int nKasho = m_Kasho1 ;
          Kotei kotei = m_Kotei1 ;
          //杭長さ
          List<int> pileLengths = m_ListPileLength1 ;

          //if (m_KougoFlg && (i % 2) == 1)//交互時の枝番タブ２
          //{
          //    int numeric = ClsCommonUtils.ConvertToNumeric(m_edaNum);
          //    numeric += 1;
          //    edaban = ClsCommonUtils.ConvertToAlphabet(numeric);
          //    nKasho = m_Kasho2;
          //    pileLengths = m_ListPileLength2;
          //}
          //ボルト
          string boltF = m_BoltF1 ;
          string boltFNum = m_BoltFNum1 ;
          string boltW = m_BoltW1 ;
          string boltWNum = m_BoltWNum1 ;

          ClsYMSUtil.SetKotei( doc, CreatedD, kotei ) ;
          ClsYMSUtil.SetBoltFlange( doc, CreatedD, boltF, ClsCommonUtils.ChangeStrToInt( boltFNum ) ) ;
          ClsYMSUtil.SetBoltWeb( doc, CreatedD, boltW, ClsCommonUtils.ChangeStrToInt( boltWNum ) ) ;

          sym = ClsRevitUtil.ChangeTypeID( doc, sym, CreatedD,
            newtypeName + cornerTsugite + "_" + m_case + "_" + edaban ) ;
          ClsRevitUtil.SetTypeParameter( sym, "CASE", m_case.ToString() ) ;
          ClsRevitUtil.SetTypeParameter( sym, "枝番", edaban ) ;

          ClsRevitUtil.SetTypeParameter( sym, "ジョイント数", nKasho ) ;
          ClsRevitUtil.SetTypeParameter( sym, "ピッチ", pitch.ToString() ) ;
          if ( m_zanti == "一部埋め殺し" )
            ClsRevitUtil.SetTypeParameter( sym, "残置/引抜", m_zanti + "(" + m_zantiLength + ")" ) ;
          else
            ClsRevitUtil.SetTypeParameter( sym, "残置/引抜", m_zanti ) ;
          ClsRevitUtil.SetTypeParameter( sym, "材質", m_zaishitu ) ;

          if ( newtypeName == CORNERYAITA ) {
            if ( cornerTsugite == "右" ) {
              ClsRevitUtil.SetTypeParameter( sym, "左側", 0 ) ;
              ClsRevitUtil.SetTypeParameter( sym, "右側", 1 ) ;
            }
          }

          //杭長さ
          ClsRevitUtil.SetTypeParameter( sym, "長さ", ClsRevitUtil.CovertToAPI( m_HLen ) ) ;

          for ( int j = 0 ; j < PileJointMax ; j++ ) {
            int nPileLength = 0 ;
            if ( j < pileLengths.Count ) {
              nPileLength = pileLengths[ j ] ;
            }

            ClsRevitUtil.SetTypeParameter( sym, "杭" + ( j + 1 ).ToString(), ClsRevitUtil.CovertToAPI( nPileLength ) ) ;
          }

          //変更が加わるメッセージ原因不明
          t.Commit() ;
        }
      }
      catch ( Exception ex ) {
        TaskDialog.Show( "Info Message", ex.Message ) ;
        string message = ex.Message ;
        MessageBox.Show( message ) ;
      }

      return kouyaita ;
    }

    public void FlipKouyaita( Document doc, ElementId id )
    {
      FamilyInstance inst = doc.GetElement( id ) as FamilyInstance ;
      XYZ point = ( inst.Location as LocationPoint ).Point ;
      XYZ dir = inst.HandOrientation ;
      m_size = Master.ClsKouyaitaCsv.GetSizePileFamilyPathInName( inst.Symbol.FamilyName ) ;
      double dist =
        ClsRevitUtil.CovertToAPI( ClsCommonUtils.ChangeStrToInt( Master.ClsKouyaitaCsv.GetLength( m_size ) ) ) ;
      XYZ movePoint = new XYZ( dir.X * dist, dir.Y * dist, dir.Z ) ;

      using ( Transaction t = new Transaction( doc, Guid.NewGuid().GetHashCode().ToString() ) ) {
        t.Start() ;
        Line axis = Line.CreateBound( point, point + XYZ.BasisZ ) ;
        ClsRevitUtil.RotateElement( doc, id, axis, Math.PI ) ;
        inst.Location.Move( movePoint ) ;
        t.Commit() ;
      }
    }

    /// <summary>
    /// 対象のファミリに作成した向きはどちらなのかをカスタムデータとして持たせる
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="id">ファミリ</param>
    /// <param name="voidVec">0:掘削側,1:壁側</param>
    /// <returns></returns>
    public bool SetVoidVec( Document doc, ElementId id, int voidVec )
    {
      if ( ! m_defaultVec ) {
        if ( voidVec == 0 )
          voidVec = 1 ;
        else if ( voidVec == 1 )
          voidVec = 0 ;
      }

      //一定方向かつ反転が指示されたとき
      if ( m_ConstOpp ) {
        if ( voidVec == 0 )
          voidVec = 1 ;
        else if ( voidVec == 1 )
          voidVec = 0 ;
      }

      return ClsRevitUtil.CustomDataSet( doc, id, VOIDVEC, voidVec ) ;
    }

    /// <summary>
    /// 対象のファミリから作成した向きののカスタムデータを取得する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="id">ファミリ</param>
    /// <returns>0:掘削側,1:壁側</returns>
    public static int GetVoidvec( Document doc, ElementId id )
    {
      ClsRevitUtil.CustomDataGet( doc, id, VOIDVEC, out int voidVec ) ;
      return voidVec ;
    }

    /// <summary>
    /// 指定したIDからクラスデータを取得する
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public void SetParameter( Document doc, ElementId id )
    {
      FamilyInstance inst = doc.GetElement( id ) as FamilyInstance ;
      FamilySymbol sym = inst.Symbol ;

      m_case = ClsCommonUtils.ChangeStrToInt( ClsRevitUtil.GetTypeParameterString( sym, "CASE" ) ) ;
      m_edaNum = ClsRevitUtil.GetTypeParameterString( sym, "枝番" ) ;

      //m_way = 0;
      m_void = (int) ClsVoid.GetVoidDep( doc, id ) ;
      //m_refPDist = 0;
      m_zanti = ClsRevitUtil.GetTypeParameterString( sym, "残置/引抜" ) ;
      if ( m_zanti.Contains( "一部埋め殺し" ) ) {
        string[] split = m_zanti.Split( '(' ) ;
        m_zanti = split[ 0 ] ;
        string[] split1 = split[ 1 ].Split( ')' ) ;
        m_zantiLength = split1[ 0 ] ;
      }

      m_zaishitu = ClsRevitUtil.GetTypeParameterString( sym, "材質" ) ;

      m_type = Master.ClsKouyaitaCsv.GetTypePileFamilyPathInName( sym.FamilyName ) ;
      m_size = Master.ClsKouyaitaCsv.GetSizePileFamilyPathInName( sym.FamilyName ) ;
      m_HTop = (int) ClsRevitUtil.CovertFromAPI( ClsRevitUtil.GetParameterDouble( doc, id, ClsGlobal.m_refLvTop ) ) ;
      m_HLen = (int) ClsRevitUtil.CovertFromAPI( ClsRevitUtil.GetTypeParameter( sym, ClsGlobal.m_length ) ) ;


      //m_KougoFlg = false;

      //m_Kasho2 = 0;
      m_ListPileLength1 = new List<int>() ;
      for ( int i = 0 ; i < PileJointMax ; i++ ) {
        int kuiLength =
          (int) ClsRevitUtil.CovertFromAPI( ClsRevitUtil.GetTypeParameter( sym, "杭" + ( i + 1 ).ToString() ) ) ;
        if ( kuiLength <= 0 )
          break ;
        m_ListPileLength1.Add( kuiLength ) ;
      }

      m_Kasho1 = m_ListPileLength1.Count() - 1 ;
      //m_ListPileLength2 = new List<int>();

      m_oreder = ClsKabeShin.GetPutOrder( doc, id ) ;

      //固定方法
      var kotei = ClsYMSUtil.GetKotei( doc, id ) ;
      m_Kotei1 = ( kotei == ClsPileTsugiteCsv.KoteiHohoBolt ? Kotei.Bolt : Kotei.Yousetsu ) ;
      ( var BoltF1, int BoltFNum1 ) = ClsYMSUtil.GetBoltFlange( doc, id ) ;
      m_BoltF1 = BoltF1 ;
      m_BoltFNum1 = BoltFNum1.ToString() ;
      ( var BoltW1, int BoltWNum1 ) = ClsYMSUtil.GetBoltWeb( doc, id ) ;
      m_BoltW1 = BoltW1 ;
      m_BoltWNum1 = BoltWNum1.ToString() ;

      return ;
    }

    /// <summary>
    /// 鋼矢板　の杭 のみを単独選択
    /// </summary>
    /// <param name="uidoc"></param>
    /// <param name="id">選択した 鋼矢板 の杭 のID</param>
    /// <param name="message">指示</param>
    /// <returns></returns>
    public static bool PickObject( UIDocument uidoc, ref ElementId id, string message = KOUYAITA )
    {
      return ClsRevitUtil.PickObjectPartFilter( uidoc, message + "を選択してください", KOUYAITA, ref id ) ;
    }

    /// <summary>
    /// 鋼矢板 の杭 のみを複数選択
    /// </summary>
    /// <param name="uidoc"></param>
    /// <param name="id">選択した 鋼矢板 の杭 のID</param>
    /// <param name="message">指示</param>
    /// <returns></returns>
    public static bool PickObjects( UIDocument uidoc, ref List<ElementId> ids, string message = KOUYAITA )
    {
      return ClsRevitUtil.PickObjectsPartFilter( uidoc, message + "を選択してください", KOUYAITA, ref ids ) ;
    }

    /// <summary>
    /// 図面上の鋼矢板を全て取得する
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    public static List<ElementId> GetAllKouyaitaList( Document doc )
    {
      //図面上の鋼矢板を全て取得
      List<ElementId> idList = ClsRevitUtil.GetSelectCreatedFamilyInstanceList( doc, KOUYAITA, true ) ;
      return idList ;
    }

    /// <summary>
    /// 壁に補助線を作成
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="pointS">開始位置</param>
    /// <param name="pointE">終了位置</param>
    /// <param name="dH">H形鋼サイズ/2(+:外側, -:内側)</param>
    /// <param name="dEx">補助線の延長</param>
    /// <returns>補助線</returns>
    public static ModelLine CreateKabeHojyoLine( Document doc, XYZ pointS, XYZ pointE, double dH, double dEx )
    {
      XYZ Direction = Line.CreateBound( pointS, pointE ).Direction ;
      //補助線の作成
      XYZ hojyoS = new XYZ( pointS.X + ( dH * Direction.Y ) - ( dEx * Direction.X ),
        pointS.Y - ( dH * Direction.X ) - ( dEx * Direction.Y ), pointS.Z ) ;
      XYZ hojyoE = new XYZ( pointE.X + ( dH * Direction.Y ) + ( dEx * Direction.X ),
        pointE.Y - ( dH * Direction.X ) + ( dEx * Direction.Y ), pointE.Z ) ;
      Line hojyo = Line.CreateBound( hojyoS, hojyoE ) ;
      if ( dH > 0 ) //補助線の内外を判別するためにDirectionを反対にする
      {
        hojyo = Line.CreateBound( hojyoE, hojyoS ) ;
      }

      XYZ mid = 0.5 * ( hojyoS + hojyoE ) ;
      //作成した壁に補助線を引く
      Plane plane = Plane.CreateByNormalAndOrigin( XYZ.BasisZ, mid ) ;
      SketchPlane sketchPlane = SketchPlane.Create( doc, plane ) ;
      ModelLine modelLine = doc.Create.NewModelCurve( hojyo, sketchPlane ) as ModelLine ;
      return modelLine ;
    }

    /// <summary>
    /// 壁に補助線を作成
    /// </summary>
    /// <param name="doc">ドキュメント</param>
    /// <param name="pointS">開始位置</param>
    /// <param name="pointE">終了位置</param>
    /// <param name="dH">H形鋼サイズ/2(+:外側, -:内側)</param>
    /// <param name="dEx">補助線の延長</param>
    /// <returns>補助線</returns>
    public static ModelLine CreatePointHojyoLine( Document doc, XYZ pointS, XYZ dirH, XYZ Direction, double dH,
      double dEx )
    {
      //補助線の作成
      XYZ hojyoS = new XYZ( pointS.X + ( dH * dirH.X ) - ( dEx * Direction.X ),
        pointS.Y + ( dH * dirH.Y ) - ( dEx * Direction.Y ), pointS.Z ) ;
      XYZ hojyoE = new XYZ( pointS.X + ( dH * dirH.X ) + ( dEx * Direction.X ),
        pointS.Y + ( dH * dirH.Y ) + ( dEx * Direction.Y ), pointS.Z ) ;
      Line hojyo = Line.CreateBound( hojyoS, hojyoE ) ;
      if ( dH > 0 ) //補助線の内外を判別するためにDirectionを反対にする
      {
        hojyo = Line.CreateBound( hojyoE, hojyoS ) ;
      }

      XYZ mid = 0.5 * ( hojyoS + hojyoE ) ;
      //作成した壁に補助線を引く
      Plane plane = Plane.CreateByNormalAndOrigin( XYZ.BasisZ, mid ) ;
      SketchPlane sketchPlane = SketchPlane.Create( doc, plane ) ;
      ModelLine modelLine = doc.Create.NewModelCurve( hojyo, sketchPlane ) as ModelLine ;
      return modelLine ;
    }

    /// <summary>
    /// 中点から指定量離れた位置の始終点を返す
    /// </summary>
    /// <param name="pointMid">中点</param>
    /// <param name="Direction"></param>
    /// <param name="dEx"></param>
    /// <returns></returns>
    public static (XYZ, XYZ) GetStEdToMidPoint( XYZ pointMid, XYZ Direction, double dEx )
    {
      //補助線の作成
      XYZ hojyoS = new XYZ( pointMid.X - ( dEx * Direction.X ), pointMid.Y - ( dEx * Direction.Y ), pointMid.Z ) ;
      XYZ hojyoE = new XYZ( pointMid.X + ( dEx * Direction.X ), pointMid.Y + ( dEx * Direction.Y ), pointMid.Z ) ;
      return ( hojyoS, hojyoE ) ;
    }

    #endregion
  }
}