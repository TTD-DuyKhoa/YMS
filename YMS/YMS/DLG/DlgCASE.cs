using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using System ;
using System.Collections.Generic ;
using System.ComponentModel ;
using System.Data ;
using System.Drawing ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using YMS.Parts ;

namespace YMS.DLG
{
  public partial class DlgCASE : System.Windows.Forms.Form
  {
    private System.Drawing.Color cellColor = System.Drawing.Color.Gray ; //セルに色付けを行う際の色み
    private System.Drawing.Color cellColor2 = System.Drawing.Color.Brown ; //セルに色付けを行う際の色み
    private int KuiIndexStartKO = 12 ; //鋼矢板・親杭のデータグリッドビューで杭1のインデックス
    private int KuiIndexStartSR = 14 ; //SMW・連続壁のデータグリッドビューで杭1のインデックス
    private int zanchiLenKO = 7 ; //鋼矢板・親杭のデータグリッドビューで残置長さのインデックス
    private int zanchiLenSR = 9 ; //SMW・連続壁のデータグリッドビューで残置長さのインデックス
    private int numIndexKO = 8 ; //鋼矢板・親杭のデータグリッドビューで本数のインデックス
    private int numIndexSR = 10 ; //SMW・連続壁のデータグリッドビューで本数のインデックス
    private int edabanIndexKO = 9 ; //鋼矢板・親杭のデータグリッドビューで現在の枝番のインデックス
    private int edabanIndexSR = 11 ; //SMW・連続壁のデータグリッドビューで現在の枝番のインデックス
    private int newEdabanIndexKO = 10 ; //鋼矢板・親杭のデータグリッドビューで新しい枝番のインデックス
    private int newEdabanIndexSR = 12 ; //SMW・連続壁のデータグリッドビューで新しい枝番のインデックス
    private int jointNumIndexKO = 11 ; //鋼矢板・親杭のデータグリッドビューでJoint数のインデックス
    private int jointNumIndexSR = 13 ; //SMW・連続壁のデータグリッドビューでJoint数のインデックス

    private RequestHandler m_Handler ;
    private ExternalEvent m_ExEvent ;
    public ClsCASE.CASECommand m_Command ;
    public ClsCASE.enKabeshu m_enKabeshu ;
    public string m_HighLightCASE ;
    public string m_HighLightEdaNum ;
    public List<OverrideGraphicSettings> m_originalSettingsList ;
    public List<ElementId> m_originalSettingsIdList ;

    private DataGridViewRow m_editPreviousDGV { get ; set ; }

    public List<ClsCASE.stCASEKoyaita> m_koyaitaDataList { get ; set ; }
    public List<ClsCASE.stCASEOyagui> m_oyaguiDataList { get ; set ; }
    public List<ClsCASE.stCASESMW> m_smwDataList { get ; set ; }
    public List<ClsCASE.stCASERenzokuKabe> m_kabeDataList { get ; set ; }

    public enum enMode
    {
      editMode,
      wariateMode,
    }

    public enMode m_mode { get ; set ; }
    public Document m_doc { get ; set ; }

    public DlgCASE( enMode mode )
    {
      InitializeComponent() ;
      m_mode = mode ;
    }

    public DlgCASE( enMode mode, Document doc, ExternalEvent exEvent, RequestHandler handler )
    {
      InitializeComponent() ;
      m_mode = mode ;
      m_doc = doc ;
      m_Handler = handler ;
      m_ExEvent = exEvent ;
      m_originalSettingsIdList = new List<ElementId>() ;
      m_originalSettingsList = new List<OverrideGraphicSettings>() ;
    }

    private void DlgCASE_Load( object sender, EventArgs e )
    {
      //初期値
      DgvComboboxAddItem() ;

      SetKoyaitaCASEData() ;
      SetOyaguiCASEData() ;
      SetSMWCASEData() ;
      SetRenzokukabeCASEData() ;

      if ( m_mode == enMode.editMode ) {
        //btnAdd.Enabled = false;
        btnAddEdaban.Enabled = false ;
      }

      //dgvKoyaita.Rows.Add("1", "1", KouyaitaColumnType.Items[0].ToString(), KouyaitaColumnSize.Items[0].ToString(), "10000",
      //    KouyaitaColumnMaterial.Items[0].ToString(), KouyaitaColumnRemain.Items[0].ToString(), "5000", "10", "A", "A", "3",
      //    "1", "2", "3", "4", "5", "6", "7", "8", "9", "10");
      //dgvOyakui.Rows.Add("1", "1", OyakuiColumnType.Items[0].ToString(), OyakuiColumnSize.Items[0].ToString(), "10000",
      //    OyakuiColumnMaterial.Items[0].ToString(), OyakuiColumnRemain.Items[0].ToString(), "5000", "10", "A", "A", "3",
      //    "1", "2", "3", "4", "5", "6", "7", "8", "9", "10");
      //dgvSMW.Rows.Add("1", "1", SMWColumnType.Items[0].ToString(), SMWColumnSize.Items[0].ToString(), "10000", "10000", "10000",
      //    SMWColumnMaterial.Items[0].ToString(), SMWColumnRemain.Items[0].ToString(), "5000", "10", "A", "A", "3",
      //    "1", "2", "3", "4", "5", "6", "7", "8", "9", "10");
      //dgvConWall.Rows.Add("1", "1", ConWallColumnType.Items[0].ToString(), ConWallColumnSize.Items[0].ToString(), "10000", "10000", "10000",
      //    ConWallColumnMaterial.Items[0].ToString(), ConWallColumnRemain.Items[0].ToString(), "5000", "10", "A", "A", "3",
      //    "1", "2", "3", "4", "5", "6", "7", "8", "9", "10");
    }

    private void DlgCASE_FormClosed( object sender, FormClosedEventArgs e )
    {
      //初期化処理
      MakeRequest( RequestId.End ) ;
      m_ExEvent.Dispose() ;
      m_ExEvent = null ;
      m_Handler = null ;
    }

    private void MakeRequest( RequestId request )
    {
      m_Handler.Request.Make( request ) ;
      m_ExEvent.Raise() ;
      DozeOff() ;
    }

    /// <summary>
    ///   DozeOff -> disable all controls (but the Exit button)
    /// </summary>
    /// 
    private void DozeOff()
    {
      EnableCommands( false ) ;
    }

    public void WakeUp()
    {
      EnableCommands( true ) ;
    }

    /// <summary>
    ///   Control enabler / disabler 
    /// </summary>
    ///
    private void EnableCommands( bool status )
    {
      foreach ( System.Windows.Forms.Control ctrl in this.Controls ) {
        ctrl.Enabled = status ;
      }

      if ( ! status ) {
        //this.btnCancel.Enabled = true;
      }

      if ( m_mode == enMode.editMode ) {
        //btnAdd.Enabled = false;
        btnAddEdaban.Enabled = false ;
      }
    }

    /// <summary>
    /// コンボボックスにアイテムを追加
    /// </summary>
    private void DgvComboboxAddItem()
    {
      KouyaitaComboboxAddItem() ;
      OyakuiComboboxAddItem() ;
      SMWComboboxAddItem() ;
      ConWallComboboxAddItem() ;
    }

    /// <summary>
    /// 鋼矢板でコンボボックスにアイテムを追加
    /// </summary>
    /// <returns></returns>
    private void KouyaitaComboboxAddItem()
    {
      List<string> lstStr = new List<string>() ;

      //タイプ
      lstStr = Master.ClsKouyaitaCsv.GetTypeList( false ) ;
      foreach ( string str in lstStr ) {
        KouyaitaColumnType.Items.Add( str ) ;
      }

      ////サイズ
      //lstStr = Master.ClsKouyaitaCsv.GetSizeList("鋼矢板", false);
      //lstStr.AddRange(Master.ClsKouyaitaCsv.GetSizeList("鋼矢板", true));
      //foreach (string str in lstStr)
      //{
      //    KouyaitaColumnSize.Items.Add(str);
      //}

      //材質
      lstStr = ClsGlobal.m_zaishitu ;
      foreach ( string str in lstStr ) {
        KouyaitaColumnMaterial.Items.Add( str ) ;
      }

      //残置
      lstStr = ClsGlobal.m_zanti ;
      foreach ( string str in lstStr ) {
        KouyaitaColumnRemain.Items.Add( str ) ;
      }
    }

    /// <summary>
    /// 親杭でコンボボックスにアイテムを追加
    /// </summary>
    /// <returns></returns>
    private void OyakuiComboboxAddItem()
    {
      List<string> lstStr = new List<string>() ;

      //タイプ
      lstStr = Master.ClsHBeamCsv.GetTypeList() ;
      foreach ( string str in lstStr ) {
        OyakuiColumnType.Items.Add( str ) ;
      }

      ////サイズ
      //lstStr = Master.ClsHBeamCsv.GetSizeList("H形鋼 広幅");
      //foreach (string str in lstStr)
      //{
      //    OyakuiColumnSize.Items.Add(str);
      //}

      //材質
      lstStr = ClsGlobal.m_zaishitu2 ;
      foreach ( string str in lstStr ) {
        OyakuiColumnMaterial.Items.Add( str ) ;
      }

      //残置
      lstStr = ClsGlobal.m_zanti ;
      foreach ( string str in lstStr ) {
        OyakuiColumnRemain.Items.Add( str ) ;
      }
    }

    /// <summary>
    /// SMWでコンボボックスにアイテムを追加
    /// </summary>
    /// <returns></returns>
    private void SMWComboboxAddItem()
    {
      List<string> lstStr = new List<string>() ;

      //タイプ
      lstStr = Master.ClsHBeamCsv.GetTypeList() ;
      foreach ( string str in lstStr ) {
        SMWColumnType.Items.Add( str ) ;
      }

      ////サイズ
      //lstStr = Master.ClsHBeamCsv.GetSizeList("H形鋼 広幅");
      //foreach (string str in lstStr)
      //{
      //    SMWColumnSize.Items.Add(str);
      //}

      //材質
      lstStr = ClsGlobal.m_zaishitu2 ;
      foreach ( string str in lstStr ) {
        SMWColumnMaterial.Items.Add( str ) ;
      }

      //残置
      lstStr = ClsGlobal.m_zanti ;
      foreach ( string str in lstStr ) {
        SMWColumnRemain.Items.Add( str ) ;
      }
    }

    /// <summary>
    /// 連続壁でコンボボックスにアイテムを追加
    /// </summary>
    /// <returns></returns>
    private void ConWallComboboxAddItem()
    {
      List<string> lstStr = new List<string>() ;

      //タイプ
      lstStr = Master.ClsHBeamCsv.GetTypeList() ;
      foreach ( string str in lstStr ) {
        ConWallColumnType.Items.Add( str ) ;
      }

      ////サイズ
      //lstStr = Master.ClsHBeamCsv.GetSizeList("H形鋼 広幅");
      //foreach (string str in lstStr)
      //{
      //    ConWallColumnSize.Items.Add(str);
      //}

      //材質
      lstStr = ClsGlobal.m_zaishitu2 ;
      foreach ( string str in lstStr ) {
        ConWallColumnMaterial.Items.Add( str ) ;
      }

      //残置
      lstStr = ClsGlobal.m_zanti ;
      foreach ( string str in lstStr ) {
        ConWallColumnRemain.Items.Add( str ) ;
      }
    }

    /// <summary>
    /// データグリッドビューのセルの値をstringで返す　nullの場合はstring.empty
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    private string GetCellVal( DataGridViewCell cell )
    {
      string res = string.Empty ;

      if ( cell == null ) {
        return res ;
      }

      if ( cell.Value == null ) {
        return res ;
      }

      res = cell.Value.ToString() ;
      return res ;
    }

    private bool SetKoyaitaCASEData()
    {
      int cnt = 0 ;

      foreach ( ClsCASE.stCASEKoyaita item in m_koyaitaDataList ) {
        dgvKoyaita.Rows.Add() ;
        var row = dgvKoyaita.Rows[ cnt ] ;
        SetRowDataKoyaita( row, item ) ;
        SetDefaultCellColor( dgvKoyaita, row ) ;
        if ( item.idList.Count == 0 && m_mode == enMode.wariateMode )
          SetColorAndReadonyRow( row ) ;
        cnt++ ;
      }

      return true ;
    }

    private void SetRowDataKoyaita( DataGridViewRow row, ClsCASE.stCASEKoyaita item )
    {
      row.Cells[ 0 ].Value = item.CASE ;
      row.Cells[ 1 ].Value = item.NewCASE ;
      row.Cells[ 2 ].Value = item.buzaiType ;
      SetSizeKoyaita( item.buzaiType, row.Index ) ;
      row.Cells[ 3 ].Value = item.buzaiSize ;
      row.Cells[ 4 ].Value = item.TotalLength.ToString() ;
      row.Cells[ 5 ].Value = item.zaishitsu ;
      row.Cells[ 6 ].Value = item.zanchi ;
      row.Cells[ 7 ].Value = item.zanchiLength.ToString() ;
      row.Cells[ 8 ].Value = item.count.ToString() ;
      row.Cells[ 9 ].Value = item.Edaban ;
      row.Cells[ 10 ].Value = item.NewEdaban ;
      row.Cells[ 11 ].Value = item.JointNum.ToString() ;
      int kc = KuiIndexStartKO ;
      foreach ( int d in item.KuiLengthList ) {
        if ( kc + ClsCASE.PileJointMax <= kc ) {
          break ;
        }

        row.Cells[ kc ].Value = d.ToString() ;
        kc++ ;
      }

      row.Tag = item.idList.ToList() ;
    }

    private bool GetKoyaitaCASEData( ref List<ClsCASE.stCASEKoyaita> list )
    {
      list = new List<ClsCASE.stCASEKoyaita>() ;
      // int cnt = 0;
      int rCnt = dgvKoyaita.Rows.Count ;

      for ( int i = 0 ; i < rCnt ; i++ ) {
        ClsCASE.stCASEKoyaita item = new ClsCASE.stCASEKoyaita() ;
        double dt = 0.0 ;
        int nt = 0 ;
        var row = dgvKoyaita.Rows[ i ] ;
        item.CASE = GetCellVal( row.Cells[ 0 ] ) ; // = item.CASE;
        item.NewCASE = GetCellVal( row.Cells[ 1 ] ) ; // = item.NewCASE;
        item.buzaiType = GetCellVal( row.Cells[ 2 ] ) ; // = item.buzaiType;
        //SetSizeKoyaita(item.buzaiType, cnt);
        item.buzaiSize = GetCellVal( row.Cells[ 3 ] ) ; // = item.buzaiSize;

        if ( ! double.TryParse( GetCellVal( row.Cells[ 4 ] ), out dt ) ) {
          return false ;
        }

        item.TotalLength = dt ; // = item.TotalLength.ToString();
        item.zaishitsu = GetCellVal( row.Cells[ 5 ] ) ; // = item.zaishitsu;
        item.zanchi = GetCellVal( row.Cells[ 6 ] ) ; // = item.zanchi;
        item.zanchiLength = GetCellVal( row.Cells[ 7 ] ) ; // = item.zanchiLength.ToString();

        if ( ! int.TryParse( GetCellVal( row.Cells[ 8 ] ), out nt ) ) {
          return false ;
        }

        item.count = nt ;
        item.Edaban = GetCellVal( row.Cells[ 9 ] ) ; // = item.Edaban;
        item.NewEdaban = GetCellVal( row.Cells[ 10 ] ) ; // = item.NewEdaban;
        if ( ! int.TryParse( GetCellVal( row.Cells[ 11 ] ), out nt ) ) {
          return false ;
        }

        item.JointNum = nt ;
        // = item.JointNum.ToString();
        for ( int j = 12 ; j <= 21 ; j++ ) {
          string ts = GetCellVal( row.Cells[ j ] ) ;
          if ( int.TryParse( ts, out nt ) ) {
            item.KuiLengthList.Add( nt ) ;
          }
        }

        item.idList = (List<ElementId>) row.Tag ;
        list.Add( item ) ;
      }

      return true ;
    }

    private bool SetOyaguiCASEData()
    {
      int cnt = 0 ;

      foreach ( ClsCASE.stCASEOyagui item in m_oyaguiDataList ) {
        dgvOyakui.Rows.Add() ;
        var row = dgvOyakui.Rows[ cnt ] ;
        SetRowDataOyagui( row, item ) ;
        SetDefaultCellColor( dgvOyakui, row ) ;
        if ( item.idList.Count == 0 && m_mode == enMode.wariateMode )
          SetColorAndReadonyRow( row ) ;
        cnt++ ;
      }

      return true ;
    }

    private void SetRowDataOyagui( DataGridViewRow row, ClsCASE.stCASEOyagui item )
    {
      row.Cells[ 0 ].Value = item.CASE ;
      row.Cells[ 1 ].Value = item.NewCASE ;
      row.Cells[ 2 ].Value = item.buzaiType ;
      SetSizeOyagui( item.buzaiType, row.Index ) ;
      row.Cells[ 3 ].Value = item.buzaiSize ;
      row.Cells[ 4 ].Value = item.TotalLength.ToString() ;
      row.Cells[ 5 ].Value = item.zaishitsu ;
      row.Cells[ 6 ].Value = item.zanchi ;
      row.Cells[ 7 ].Value = item.zanchiLength.ToString() ;
      row.Cells[ 8 ].Value = item.count.ToString() ;
      row.Cells[ 9 ].Value = item.Edaban ;
      row.Cells[ 10 ].Value = item.NewEdaban ;
      row.Cells[ 11 ].Value = item.JointNum.ToString() ;
      int kc = KuiIndexStartKO ;
      foreach ( int d in item.KuiLengthList ) {
        if ( kc + ClsCASE.PileJointMax <= kc ) {
          break ;
        }

        row.Cells[ kc ].Value = d.ToString() ;
        kc++ ;
      }

      row.Tag = item.idList.ToList() ;
    }

    private bool GetOyaguiCASEData( ref List<ClsCASE.stCASEOyagui> list )
    {
      list = new List<ClsCASE.stCASEOyagui>() ;
      //int cnt = 0;
      int rCnt = dgvOyakui.Rows.Count ;

      for ( int i = 0 ; i < rCnt ; i++ ) {
        ClsCASE.stCASEOyagui item = new ClsCASE.stCASEOyagui() ;
        double dt = 0.0 ;
        int nt = 0 ;
        var row = dgvOyakui.Rows[ i ] ;
        item.CASE = GetCellVal( row.Cells[ 0 ] ) ; // = item.CASE;
        item.NewCASE = GetCellVal( row.Cells[ 1 ] ) ; // = item.NewCASE;
        item.buzaiType = GetCellVal( row.Cells[ 2 ] ) ; // = item.buzaiType;
        //SetSizeKoyaita(item.buzaiType, cnt);
        item.buzaiSize = GetCellVal( row.Cells[ 3 ] ) ; // = item.buzaiSize;

        if ( ! double.TryParse( GetCellVal( row.Cells[ 4 ] ), out dt ) ) {
          return false ;
        }

        item.TotalLength = dt ; // = item.TotalLength.ToString();
        item.zaishitsu = GetCellVal( row.Cells[ 5 ] ) ; // = item.zaishitsu;
        item.zanchi = GetCellVal( row.Cells[ 6 ] ) ; // = item.zanchi;
        item.zanchiLength = GetCellVal( row.Cells[ 7 ] ) ; // = item.zanchiLength.ToString();

        if ( ! int.TryParse( GetCellVal( row.Cells[ 8 ] ), out nt ) ) {
          return false ;
        }

        item.count = nt ;
        item.Edaban = GetCellVal( row.Cells[ 9 ] ) ; // = item.Edaban;
        item.NewEdaban = GetCellVal( row.Cells[ 10 ] ) ; // = item.NewEdaban;
        if ( ! int.TryParse( GetCellVal( row.Cells[ 11 ] ), out nt ) ) {
          return false ;
        }

        item.JointNum = nt ;
        // = item.JointNum.ToString();
        for ( int j = 12 ; j <= 21 ; j++ ) {
          string ts = GetCellVal( row.Cells[ j ] ) ;
          if ( int.TryParse( ts, out nt ) ) {
            item.KuiLengthList.Add( nt ) ;
          }
        }

        item.idList = (List<ElementId>) row.Tag ;
        list.Add( item ) ;
      }

      return true ;
    }

    private bool SetSMWCASEData()
    {
      int cnt = 0 ;

      foreach ( ClsCASE.stCASESMW item in m_smwDataList ) {
        dgvSMW.Rows.Add() ;
        var row = dgvSMW.Rows[ cnt ] ;
        SetRowDataSMW( row, item ) ;
        SetDefaultCellColor( dgvSMW, row ) ;
        if ( item.idList.Count == 0 && m_mode == enMode.wariateMode )
          SetColorAndReadonyRow( row ) ;
        cnt++ ;
      }

      return true ;
    }

    private void SetRowDataSMW( DataGridViewRow row, ClsCASE.stCASESMW item )
    {
      row.Cells[ 0 ].Value = item.CASE ;
      row.Cells[ 1 ].Value = item.NewCASE ;
      row.Cells[ 2 ].Value = item.buzaiType ;
      SetSizeSMW( item.buzaiType, row.Index ) ;
      row.Cells[ 3 ].Value = item.buzaiSize ;
      row.Cells[ 4 ].Value = item.TotalLength.ToString() ;
      row.Cells[ 5 ].Value = item.soilKei.ToString() ;
      row.Cells[ 6 ].Value = item.soilLength.ToString() ;
      row.Cells[ 7 ].Value = item.zaishitsu ;
      row.Cells[ 8 ].Value = item.zanchi ;
      row.Cells[ 9 ].Value = item.zanchiLength.ToString() ;
      row.Cells[ 10 ].Value = item.count.ToString() ;
      row.Cells[ 11 ].Value = item.Edaban ;
      row.Cells[ 12 ].Value = item.NewEdaban ;
      row.Cells[ 13 ].Value = item.JointNum.ToString() ;
      int kc = KuiIndexStartSR ;
      foreach ( int d in item.KuiLengthList ) {
        if ( kc + ClsCASE.PileJointMax <= kc ) {
          break ;
        }

        row.Cells[ kc ].Value = d.ToString() ;
        kc++ ;
      }

      row.Tag = item.idList.ToList() ;
    }

    private bool GetSMWCASEData( ref List<ClsCASE.stCASESMW> list )
    {
      list = new List<ClsCASE.stCASESMW>() ;
      //int cnt = 0;
      int rCnt = dgvSMW.Rows.Count ;

      for ( int i = 0 ; i < rCnt ; i++ ) {
        ClsCASE.stCASESMW item = new ClsCASE.stCASESMW() ;
        double dt = 0.0 ;
        int nt = 0 ;
        var row = dgvSMW.Rows[ i ] ;
        item.CASE = GetCellVal( row.Cells[ 0 ] ) ; // = item.CASE;
        item.NewCASE = GetCellVal( row.Cells[ 1 ] ) ; // = item.NewCASE;
        item.buzaiType = GetCellVal( row.Cells[ 2 ] ) ; // = item.buzaiType;
        //SetSizeKoyaita(item.buzaiType, cnt);
        item.buzaiSize = GetCellVal( row.Cells[ 3 ] ) ; // = item.buzaiSize;

        if ( ! double.TryParse( GetCellVal( row.Cells[ 4 ] ), out dt ) ) {
          return false ;
        }

        item.TotalLength = dt ; // = item.TotalLength.ToString();

        if ( ! double.TryParse( GetCellVal( row.Cells[ 5 ] ), out dt ) ) {
          return false ;
        }

        item.soilKei = dt ;

        if ( ! double.TryParse( GetCellVal( row.Cells[ 6 ] ), out dt ) ) {
          return false ;
        }

        item.soilLength = dt ;

        item.zaishitsu = GetCellVal( row.Cells[ 7 ] ) ; // = item.zaishitsu;
        item.zanchi = GetCellVal( row.Cells[ 8 ] ) ; // = item.zanchi;
        item.zanchiLength = GetCellVal( row.Cells[ 9 ] ) ; // = item.zanchiLength.ToString();

        if ( ! int.TryParse( GetCellVal( row.Cells[ 10 ] ), out nt ) ) {
          return false ;
        }

        item.count = nt ;
        item.Edaban = GetCellVal( row.Cells[ 11 ] ) ; // = item.Edaban;
        item.NewEdaban = GetCellVal( row.Cells[ 12 ] ) ; // = item.NewEdaban;
        if ( ! int.TryParse( GetCellVal( row.Cells[ 13 ] ), out nt ) ) {
          return false ;
        }

        item.JointNum = nt ;
        // = item.JointNum.ToString();
        for ( int j = 14 ; j <= 23 ; j++ ) {
          string ts = GetCellVal( row.Cells[ j ] ) ;
          if ( int.TryParse( ts, out nt ) ) {
            item.KuiLengthList.Add( nt ) ;
          }
        }

        item.idList = (List<ElementId>) row.Tag ;
        list.Add( item ) ;
      }

      return true ;
    }

    private bool SetRenzokukabeCASEData()
    {
      int cnt = 0 ;

      foreach ( ClsCASE.stCASERenzokuKabe item in m_kabeDataList ) {
        dgvConWall.Rows.Add() ;
        var row = dgvConWall.Rows[ cnt ] ;
        SetRowDataRenzokuKabe( row, item ) ;
        SetDefaultCellColor( dgvConWall, row ) ;
        if ( item.idList.Count == 0 && m_mode == enMode.wariateMode )
          SetColorAndReadonyRow( row ) ;
        cnt++ ;
      }

      return true ;
    }

    private void SetRowDataRenzokuKabe( DataGridViewRow row, ClsCASE.stCASERenzokuKabe item )
    {
      row.Cells[ 0 ].Value = item.CASE ;
      row.Cells[ 1 ].Value = item.NewCASE ;
      row.Cells[ 2 ].Value = item.buzaiType ;
      SetSizeRenzokukabe( item.buzaiType, row.Index ) ;
      row.Cells[ 3 ].Value = item.buzaiSize ;
      row.Cells[ 4 ].Value = item.TotalLength.ToString() ;
      row.Cells[ 5 ].Value = item.kabeThickness.ToString() ;
      row.Cells[ 6 ].Value = item.kabeThickness.ToString() ;
      row.Cells[ 7 ].Value = item.zaishitsu ;
      row.Cells[ 8 ].Value = item.zanchi ;
      row.Cells[ 9 ].Value = item.zanchiLength.ToString() ;
      row.Cells[ 10 ].Value = item.count.ToString() ;
      row.Cells[ 11 ].Value = item.Edaban ;
      row.Cells[ 12 ].Value = item.NewEdaban ;
      row.Cells[ 13 ].Value = item.JointNum.ToString() ;
      int kc = KuiIndexStartSR ;
      foreach ( int d in item.KuiLengthList ) {
        if ( kc + ClsCASE.PileJointMax <= kc ) {
          break ;
        }

        row.Cells[ kc ].Value = d.ToString() ;
        kc++ ;
      }

      row.Tag = item.idList.ToList() ;
    }


    private bool GetReanzokukabeCASEData( ref List<ClsCASE.stCASERenzokuKabe> list )
    {
      list = new List<ClsCASE.stCASERenzokuKabe>() ;
      // int cnt = 0;
      int rCnt = dgvConWall.Rows.Count ;

      for ( int i = 0 ; i < rCnt ; i++ ) {
        ClsCASE.stCASERenzokuKabe item = new ClsCASE.stCASERenzokuKabe() ;
        double dt = 0.0 ;
        int nt = 0 ;
        var row = dgvConWall.Rows[ i ] ;
        item.CASE = GetCellVal( row.Cells[ 0 ] ) ; // = item.CASE;
        item.NewCASE = GetCellVal( row.Cells[ 1 ] ) ; // = item.NewCASE;
        item.buzaiType = GetCellVal( row.Cells[ 2 ] ) ; // = item.buzaiType;
        //SetSizeKoyaita(item.buzaiType, cnt);
        item.buzaiSize = GetCellVal( row.Cells[ 3 ] ) ; // = item.buzaiSize;

        if ( ! double.TryParse( GetCellVal( row.Cells[ 4 ] ), out dt ) ) {
          return false ;
        }

        item.TotalLength = dt ; // = item.TotalLength.ToString();

        if ( ! double.TryParse( GetCellVal( row.Cells[ 5 ] ), out dt ) ) {
          return false ;
        }

        item.kabeThickness = dt ;

        if ( ! double.TryParse( GetCellVal( row.Cells[ 6 ] ), out dt ) ) {
          return false ;
        }

        item.kabeLength = dt ;

        item.zaishitsu = GetCellVal( row.Cells[ 7 ] ) ; // = item.zaishitsu;
        item.zanchi = GetCellVal( row.Cells[ 8 ] ) ; // = item.zanchi;
        item.zanchiLength = GetCellVal( row.Cells[ 9 ] ) ; // = item.zanchiLength.ToString();

        if ( ! int.TryParse( GetCellVal( row.Cells[ 10 ] ), out nt ) ) {
          return false ;
        }

        item.count = nt ;
        item.Edaban = GetCellVal( row.Cells[ 11 ] ) ; // = item.Edaban;
        item.NewEdaban = GetCellVal( row.Cells[ 12 ] ) ; // = item.NewEdaban;
        if ( ! int.TryParse( GetCellVal( row.Cells[ 13 ] ), out nt ) ) {
          return false ;
        }

        item.JointNum = nt ;
        // = item.JointNum.ToString();
        for ( int j = 14 ; j <= 23 ; j++ ) {
          string ts = GetCellVal( row.Cells[ j ] ) ;
          if ( int.TryParse( ts, out nt ) ) {
            item.KuiLengthList.Add( nt ) ;
          }
        }

        item.idList = (List<ElementId>) row.Tag ;
        list.Add( item ) ;
      }

      return true ;
    }

    /// <summary>
    /// セル色の変更
    /// </summary>
    /// <param name="dwg">対象のデータグリッドビュー</param>
    /// <param name="row">対象の列</param>
    private void SetDefaultCellColor( DataGridView dgw, DataGridViewRow row )
    {
      row.Cells[ 0 ].Style.BackColor = cellColor ;
      int numIndex = dgw.Name == dgvKoyaita.Name || dgw.Name == dgvOyakui.Name ? numIndexKO : numIndexSR ;
      row.Cells[ numIndex ].Style.BackColor = cellColor ;
      int edabanIndex = dgw.Name == dgvKoyaita.Name || dgw.Name == dgvOyakui.Name ? edabanIndexKO : edabanIndexSR ;
      row.Cells[ edabanIndex ].Style.BackColor = cellColor ;
    }

    /// <summary>
    /// 指定された行の色をブラウンに変更しReadonyに変更
    /// </summary>
    /// <param name="row"></param>
    private void SetColorAndReadonyRow( DataGridViewRow row )
    {
      foreach ( DataGridViewCell cell in row.Cells ) {
        cell.Style.BackColor = cellColor2 ;
        cell.ReadOnly = true ;
      }
    }

    /// <summary>
    /// 指定された行以外をブラウンに変更しReadonyに変更
    /// </summary>
    /// <param name="row"></param>
    private void SetColorAndReadonyRow( DataGridView dgw, DataGridViewRow targetRow )
    {
      foreach ( DataGridViewRow row in dgw.Rows ) {
        if ( targetRow.Index == row.Index ) {
          continue ;
        }

        SetColorAndReadonyRow( row ) ;
      }
    }

    /// <summary>
    /// 鋼矢板のリストをソート
    /// </summary>
    /// <param name="koyaitaList"></param>
    /// <returns></returns>
    public static List<ClsCASE.stCASEKoyaita> SortKoyaitaList( List<ClsCASE.stCASEKoyaita> koyaitaList )
    {
      List<ClsCASE.stCASEKoyaita> sortedKoyaitaList = new List<ClsCASE.stCASEKoyaita>() ;

      koyaitaList = koyaitaList.OrderBy( line => line.CASE ).ToList() ;
      List<ClsCASE.stCASEKoyaita> sortEdabanList = new List<ClsCASE.stCASEKoyaita>() ;
      for ( int i = 0 ; i < koyaitaList.Count() ; i++ ) {
        ClsCASE.stCASEKoyaita sortedCaseLine = koyaitaList[ i ] ;
        if ( i == 0 ) {
          sortEdabanList.Add( sortedCaseLine ) ;
          continue ;
        }

        ClsCASE.stCASEKoyaita sortedCaseLinePre = koyaitaList[ i - 1 ] ;
        if ( sortedCaseLine.CASE == sortedCaseLinePre.CASE ) {
          if ( i == koyaitaList.Count() - 1 ) {
            sortEdabanList.Add( sortedCaseLine ) ;
            sortEdabanList = sortEdabanList.OrderBy( line => line.Edaban ).ToList() ;
            sortedKoyaitaList.AddRange( sortEdabanList ) ;
          }
          else {
            sortEdabanList.Add( sortedCaseLine ) ;
          }
        }
        else {
          sortEdabanList = sortEdabanList.OrderBy( line => line.Edaban ).ToList() ;
          sortedKoyaitaList.AddRange( sortEdabanList ) ;
          if ( i == koyaitaList.Count() - 1 ) {
            sortedKoyaitaList.Add( sortedCaseLine ) ;
          }
          else {
            sortEdabanList.Clear() ;
            sortEdabanList.Add( sortedCaseLine ) ;
          }
        }
      }

      return sortedKoyaitaList ;
    }

    /// <summary>
    /// 親杭のリストをソート
    /// </summary>
    /// <param name="OyaguiList"></param>
    /// <returns></returns>
    public static List<ClsCASE.stCASEOyagui> SortOyaguiList( List<ClsCASE.stCASEOyagui> OyaguiList )
    {
      List<ClsCASE.stCASEOyagui> sortedOyaguiList = new List<ClsCASE.stCASEOyagui>() ;

      OyaguiList = OyaguiList.OrderBy( line => line.CASE ).ToList() ;
      List<ClsCASE.stCASEOyagui> sortEdabanList = new List<ClsCASE.stCASEOyagui>() ;
      for ( int i = 0 ; i < OyaguiList.Count() ; i++ ) {
        ClsCASE.stCASEOyagui sortedCaseLine = OyaguiList[ i ] ;
        if ( i == 0 ) {
          sortEdabanList.Add( sortedCaseLine ) ;
          continue ;
        }

        ClsCASE.stCASEOyagui sortedCaseLinePre = OyaguiList[ i - 1 ] ;
        if ( sortedCaseLine.CASE == sortedCaseLinePre.CASE ) {
          if ( i == OyaguiList.Count() - 1 ) {
            sortEdabanList.Add( sortedCaseLine ) ;
            sortEdabanList = sortEdabanList.OrderBy( line => line.Edaban ).ToList() ;
            sortedOyaguiList.AddRange( sortEdabanList ) ;
          }
          else {
            sortEdabanList.Add( sortedCaseLine ) ;
          }
        }
        else {
          sortEdabanList = sortEdabanList.OrderBy( line => line.Edaban ).ToList() ;
          sortedOyaguiList.AddRange( sortEdabanList ) ;
          if ( i == OyaguiList.Count() - 1 ) {
            sortedOyaguiList.Add( sortedCaseLine ) ;
          }
          else {
            sortEdabanList.Clear() ;
            sortEdabanList.Add( sortedCaseLine ) ;
          }
        }
      }

      return sortedOyaguiList ;
    }

    /// <summary>
    /// SMWのリストをソート
    /// </summary>
    /// <param name="SMWList"></param>
    /// <returns></returns>
    public static List<ClsCASE.stCASESMW> SortSMWList( List<ClsCASE.stCASESMW> SMWList )
    {
      List<ClsCASE.stCASESMW> sortedSMWList = new List<ClsCASE.stCASESMW>() ;

      SMWList = SMWList.OrderBy( line => line.CASE ).ToList() ;
      List<ClsCASE.stCASESMW> sortEdabanList = new List<ClsCASE.stCASESMW>() ;
      for ( int i = 0 ; i < SMWList.Count() ; i++ ) {
        ClsCASE.stCASESMW sortedCaseLine = SMWList[ i ] ;
        if ( i == 0 ) {
          sortEdabanList.Add( sortedCaseLine ) ;
          continue ;
        }

        ClsCASE.stCASESMW sortedCaseLinePre = SMWList[ i - 1 ] ;
        if ( sortedCaseLine.CASE == sortedCaseLinePre.CASE ) {
          if ( i == SMWList.Count() - 1 ) {
            sortEdabanList.Add( sortedCaseLine ) ;
            sortEdabanList = sortEdabanList.OrderBy( line => line.Edaban ).ToList() ;
            sortedSMWList.AddRange( sortEdabanList ) ;
          }
          else {
            sortEdabanList.Add( sortedCaseLine ) ;
          }
        }
        else {
          sortEdabanList = sortEdabanList.OrderBy( line => line.Edaban ).ToList() ;
          sortedSMWList.AddRange( sortEdabanList ) ;
          if ( i == SMWList.Count() - 1 ) {
            sortedSMWList.Add( sortedCaseLine ) ;
          }
          else {
            sortEdabanList.Clear() ;
            sortEdabanList.Add( sortedCaseLine ) ;
          }
        }
      }

      return sortedSMWList ;
    }

    /// <summary>
    /// 連続壁のリストをソート
    /// </summary>
    /// <param name="renzokuKabeList"></param>
    /// <returns></returns>
    public static List<ClsCASE.stCASERenzokuKabe> SortRenzokuKabeList( List<ClsCASE.stCASERenzokuKabe> renzokuKabeList )
    {
      List<ClsCASE.stCASERenzokuKabe> sortedRenzokuKabeList = new List<ClsCASE.stCASERenzokuKabe>() ;

      renzokuKabeList = renzokuKabeList.OrderBy( line => line.CASE ).ToList() ;
      List<ClsCASE.stCASERenzokuKabe> sortEdabanList = new List<ClsCASE.stCASERenzokuKabe>() ;
      for ( int i = 0 ; i < renzokuKabeList.Count() ; i++ ) {
        ClsCASE.stCASERenzokuKabe sortedCaseLine = renzokuKabeList[ i ] ;
        if ( i == 0 ) {
          sortEdabanList.Add( sortedCaseLine ) ;
          continue ;
        }

        ClsCASE.stCASERenzokuKabe sortedCaseLinePre = renzokuKabeList[ i - 1 ] ;
        if ( sortedCaseLine.CASE == sortedCaseLinePre.CASE ) {
          if ( i == renzokuKabeList.Count() - 1 ) {
            sortEdabanList.Add( sortedCaseLine ) ;
            sortEdabanList = sortEdabanList.OrderBy( line => line.Edaban ).ToList() ;
            sortedRenzokuKabeList.AddRange( sortEdabanList ) ;
          }
          else {
            sortEdabanList.Add( sortedCaseLine ) ;
          }
        }
        else {
          sortEdabanList = sortEdabanList.OrderBy( line => line.Edaban ).ToList() ;
          sortedRenzokuKabeList.AddRange( sortEdabanList ) ;
          if ( i == renzokuKabeList.Count() - 1 ) {
            sortedRenzokuKabeList.Add( sortedCaseLine ) ;
          }
          else {
            sortEdabanList.Clear() ;
            sortEdabanList.Add( sortedCaseLine ) ;
          }
        }
      }

      return sortedRenzokuKabeList ;
    }

    /// <summary>
    /// 指定された「現在のケース」、「現在の枝版」の値を両方合致する行の下に、鋼矢板のデータを挿入する
    /// </summary>
    /// <param name="CASE">現在のケース</param>
    /// <param name="edaban">現在の枝番</param>
    /// <param name="renzokuKabe">挿入する情報</param>
    /// <returns></returns>
    public bool SetDataKoyaita( string CASE, string edaban, ClsCASE.stCASEKoyaita koyaita )
    {
      DataGridViewRow targetRow = new DataGridViewRow() ;
      foreach ( DataGridViewRow row in dgvKoyaita.Rows ) {
        if ( GetCellVal( row.Cells[ 0 ] ) == CASE && GetCellVal( row.Cells[ edabanIndexKO ] ) == edaban ) {
          targetRow = row ;
        }
      }

      if ( targetRow == null ) {
        return false ;
      }

      int insertIndex = targetRow.Index + 1 ;
      dgvKoyaita.Rows.Insert( insertIndex, 1 ) ;
      var insertRow = dgvKoyaita.Rows[ insertIndex ] ;
      SetRowDataKoyaita( insertRow, koyaita ) ;
      SetDefaultCellColor( dgvKoyaita, insertRow ) ;
      SetColorAndReadonyRow( dgvKoyaita, insertRow ) ;
      return true ;
    }

    /// <summary>
    /// 指定された「現在のケース」、「現在の枝版」の値を両方合致する行の下に、親杭のデータを挿入する
    /// </summary>
    /// <param name="CASE">現在のケース</param>
    /// <param name="edaban">現在の枝番</param>
    /// <param name="renzokuKabe">挿入する情報</param>
    /// <returns></returns>
    public bool SetDataOyagui( string CASE, string edaban, ClsCASE.stCASEOyagui oyagui )
    {
      DataGridViewRow targetRow = new DataGridViewRow() ;
      foreach ( DataGridViewRow row in dgvOyakui.Rows ) {
        if ( GetCellVal( row.Cells[ 0 ] ) == CASE && GetCellVal( row.Cells[ edabanIndexKO ] ) == edaban ) {
          targetRow = row ;
        }
      }

      if ( targetRow == null ) {
        return false ;
      }

      int insertIndex = targetRow.Index + 1 ;
      dgvOyakui.Rows.Insert( insertIndex, 1 ) ;
      var insertRow = dgvOyakui.Rows[ insertIndex ] ;
      SetRowDataOyagui( insertRow, oyagui ) ;
      SetDefaultCellColor( dgvOyakui, insertRow ) ;
      SetColorAndReadonyRow( dgvOyakui, insertRow ) ;
      return true ;
    }

    /// <summary>
    /// 指定された「現在のケース」、「現在の枝版」の値を両方合致する行の下に、SMWのデータを挿入する
    /// </summary>
    /// <param name="CASE">現在のケース</param>
    /// <param name="edaban">現在の枝番</param>
    /// <param name="renzokuKabe">挿入する情報</param>
    /// <returns></returns>
    public bool SetDataSMW( string CASE, string edaban, ClsCASE.stCASESMW SMW )
    {
      DataGridViewRow targetRow = new DataGridViewRow() ;
      foreach ( DataGridViewRow row in dgvSMW.Rows ) {
        if ( GetCellVal( row.Cells[ 0 ] ) == CASE && GetCellVal( row.Cells[ edabanIndexKO ] ) == edaban ) {
          targetRow = row ;
        }
      }

      if ( targetRow == null ) {
        return false ;
      }

      int insertIndex = targetRow.Index + 1 ;
      dgvSMW.Rows.Insert( insertIndex, 1 ) ;
      var insertRow = dgvSMW.Rows[ insertIndex ] ;
      SetRowDataSMW( insertRow, SMW ) ;
      SetDefaultCellColor( dgvSMW, insertRow ) ;
      SetColorAndReadonyRow( dgvSMW, insertRow ) ;
      return true ;
    }

    /// <summary>
    /// 指定された「現在のケース」、「現在の枝版」の値を両方合致する行の下に、連続壁のデータを挿入する
    /// </summary>
    /// <param name="CASE">現在のケース</param>
    /// <param name="edaban">現在の枝番</param>
    /// <param name="renzokuKabe">挿入する情報</param>
    /// <returns></returns>
    public bool SetDataRenzokuKabe( string CASE, string edaban, ClsCASE.stCASERenzokuKabe renzokuKabe )
    {
      DataGridViewRow targetRow = new DataGridViewRow() ;
      foreach ( DataGridViewRow row in dgvConWall.Rows ) {
        if ( GetCellVal( row.Cells[ 0 ] ) == CASE && GetCellVal( row.Cells[ edabanIndexKO ] ) == edaban ) {
          targetRow = row ;
        }
      }

      if ( targetRow == null ) {
        return false ;
      }

      int insertIndex = targetRow.Index + 1 ;
      dgvConWall.Rows.Insert( insertIndex, 1 ) ;
      var insertRow = dgvConWall.Rows[ insertIndex ] ;
      SetRowDataRenzokuKabe( insertRow, renzokuKabe ) ;
      SetDefaultCellColor( dgvConWall, insertRow ) ;
      SetColorAndReadonyRow( dgvConWall, insertRow ) ;
      return true ;
    }

    private bool SetSizeKoyaita( string type, int row )
    {
      try {
        //KouyaitaColumnSize.Items.Clear();

        List<string> lstStr = new List<string>() ;
        //サイズ
        lstStr = Master.ClsKouyaitaCsv.GetSizeList( type, false ) ;
        lstStr.AddRange( Master.ClsKouyaitaCsv.GetSizeList( type, true ) ) ;
        DataGridViewComboBoxCell comboBoxCell = new DataGridViewComboBoxCell() ;
        comboBoxCell.Items.AddRange( lstStr.ToArray() ) ;
        dgvKoyaita.Rows[ row ].Cells[ "KouyaitaColumnSize" ] = comboBoxCell ;

        if ( lstStr.Count > 0 ) {
          dgvKoyaita.Rows[ row ].Cells[ "KouyaitaColumnSize" ].Value = lstStr[ 0 ] ;
        }
      }
      catch {
        return false ;
      }

      return true ;
    }

    private bool SetSizeOyagui( string type, int row )
    {
      try {
        //OyakuiColumnSize.Items.Clear();

        List<string> lstStr = new List<string>() ;

        //サイズ
        lstStr = Master.ClsHBeamCsv.GetSizeList( type ) ;
        DataGridViewComboBoxCell comboBoxCell = new DataGridViewComboBoxCell() ;
        comboBoxCell.Items.AddRange( lstStr.ToArray() ) ;
        dgvOyakui.Rows[ row ].Cells[ "OyakuiColumnSize" ] = comboBoxCell ;

        if ( lstStr.Count > 0 ) {
          dgvOyakui.Rows[ row ].Cells[ "OyakuiColumnSize" ].Value = lstStr[ 0 ] ;
        }
      }
      catch {
        return false ;
      }

      return true ;
    }

    private bool SetSizeSMW( string type, int row )
    {
      try {
        //SMWColumnSize.Items.Clear();

        List<string> lstStr = new List<string>() ;

        //サイズ
        lstStr = Master.ClsHBeamCsv.GetSizeList( type ) ;
        DataGridViewComboBoxCell comboBoxCell = new DataGridViewComboBoxCell() ;
        comboBoxCell.Items.AddRange( lstStr.ToArray() ) ;
        dgvSMW.Rows[ row ].Cells[ "SMWColumnSize" ] = comboBoxCell ;

        if ( lstStr.Count > 0 ) {
          dgvSMW.Rows[ row ].Cells[ "SMWColumnSize" ].Value = lstStr[ 0 ] ;
        }
      }
      catch {
        return false ;
      }

      return true ;
    }

    private bool SetSizeRenzokukabe( string type, int row )
    {
      try {
        //ConWallColumnSize.Items.Clear();

        List<string> lstStr = new List<string>() ;

        lstStr = Master.ClsHBeamCsv.GetSizeList( type ) ;
        DataGridViewComboBoxCell comboBoxCell = new DataGridViewComboBoxCell() ;
        comboBoxCell.Items.AddRange( lstStr.ToArray() ) ;
        dgvConWall.Rows[ row ].Cells[ "ConWallColumnSize" ] = comboBoxCell ;

        if ( lstStr.Count > 0 ) {
          dgvConWall.Rows[ row ].Cells[ "ConWallColumnSize" ].Value = lstStr[ 0 ] ;
        }
      }
      catch {
        return false ;
      }

      return true ;
    }

    /// <summary>
    /// 数値チェック
    /// </summary>
    /// <returns></returns>
    private bool CheackNumericValue()
    {
      //if(CheackNumericValueKoyaita() &&
      //   CheackNumericValueOyakui() &&
      //   CheackNumericValueSMW() &&
      //   CheackNumericValueConWall())
      //{
      //    return true;
      //}
      //return false;
      return true ;
    }

    /// <summary>
    /// 鋼矢板で数値チェック
    /// </summary>
    /// <returns></returns>
    private bool CheackNumericValueKoyaita()
    {
      foreach ( DataGridViewRow row in dgvKoyaita.Rows ) {
        foreach ( DataGridViewCell cell in row.Cells ) {
          int cellIndex = cell.ColumnIndex ;
          if ( cellIndex == KouyaitaColumnLength.Index || cellIndex == KouyaitaColumnRemainLen.Index ||
               cellIndex == KouyaitaColumnJointNum.Index || cellIndex == KouyaitaColumnStake1.Index ||
               cellIndex == KouyaitaColumnStake2.Index || cellIndex == KouyaitaColumnStake3.Index ||
               cellIndex == KouyaitaColumnStake4.Index || cellIndex == KouyaitaColumnStake5.Index ||
               cellIndex == KouyaitaColumnStake6.Index || cellIndex == KouyaitaColumnStake7.Index ||
               cellIndex == KouyaitaColumnStake8.Index || cellIndex == KouyaitaColumnStake9.Index ||
               cellIndex == KouyaitaColumnStake10.Index ) {
            int intNum = 0 ;
            double doubleNum = 0 ;
            if ( ! int.TryParse( GetCellVal( cell ), out intNum ) &&
                 ! double.TryParse( GetCellVal( cell ), out doubleNum ) ) {
              return false ;
            }
          }
        }
      }

      return true ;
    }

    /// <summary>
    /// 親杭で数値チェック
    /// </summary>
    /// <returns></returns>
    private bool CheackNumericValueOyakui()
    {
      foreach ( DataGridViewRow row in dgvOyakui.Rows ) {
        foreach ( DataGridViewCell cell in row.Cells ) {
          int cellIndex = cell.ColumnIndex ;
          if ( cellIndex == OyakuiColumnLength.Index || cellIndex == OyakuiColumnRemainLen.Index ||
               cellIndex == OyakuiColumnJointNum.Index || cellIndex == OyakuiColumnStake1.Index ||
               cellIndex == OyakuiColumnStake2.Index || cellIndex == OyakuiColumnStake3.Index ||
               cellIndex == OyakuiColumnStake4.Index || cellIndex == OyakuiColumnStake5.Index ||
               cellIndex == OyakuiColumnStake6.Index || cellIndex == OyakuiColumnStake7.Index ||
               cellIndex == OyakuiColumnStake8.Index || cellIndex == OyakuiColumnStake9.Index ||
               cellIndex == OyakuiColumnStake10.Index ) {
            int intNum = 0 ;
            double doubleNum = 0 ;
            if ( ! int.TryParse( GetCellVal( cell ), out intNum ) &&
                 ! double.TryParse( GetCellVal( cell ), out doubleNum ) ) {
              return false ;
            }
          }
        }
      }

      return true ;
    }

    /// <summary>
    /// SMWで数値チェック
    /// </summary>
    /// <returns></returns>
    private bool CheackNumericValueSMW()
    {
      foreach ( DataGridViewRow row in dgvSMW.Rows ) {
        foreach ( DataGridViewCell cell in row.Cells ) {
          int cellIndex = cell.ColumnIndex ;
          if ( cellIndex == SMWColumnLength.Index || cellIndex == SMWColumnSoilDia.Index ||
               cellIndex == SMWColumnSoilLen.Index || cellIndex == SMWColumnRemainLen.Index ||
               cellIndex == SMWColumnJointNum.Index || cellIndex == SMWColumnStake1.Index ||
               cellIndex == SMWColumnStake2.Index || cellIndex == SMWColumnStake3.Index ||
               cellIndex == SMWColumnStake4.Index || cellIndex == SMWColumnStake5.Index ||
               cellIndex == SMWColumnStake6.Index || cellIndex == SMWColumnStake7.Index ||
               cellIndex == SMWColumnStake8.Index || cellIndex == SMWColumnStake9.Index ||
               cellIndex == SMWColumnStake10.Index ) {
            int intNum = 0 ;
            double doubleNum = 0 ;
            if ( ! int.TryParse( GetCellVal( cell ), out intNum ) &&
                 ! double.TryParse( GetCellVal( cell ), out doubleNum ) ) {
              return false ;
            }
          }
        }
      }

      return true ;
    }

    /// <summary>
    /// 連続壁で数値チェック
    /// </summary>
    /// <returns></returns>
    private bool CheackNumericValueConWall()
    {
      foreach ( DataGridViewRow row in dgvConWall.Rows ) {
        foreach ( DataGridViewCell cell in row.Cells ) {
          int cellIndex = cell.ColumnIndex ;
          if ( cellIndex == ConWallColumnLength.Index || cellIndex == ConWallColumnWallLen.Index ||
               cellIndex == ConWallColumnWallThick.Index || cellIndex == ConWallColumnRemainLen.Index ||
               cellIndex == ConWallColumnJointNum.Index || cellIndex == ConWallColumnStake1.Index ||
               cellIndex == ConWallColumnStake2.Index || cellIndex == ConWallColumnStake3.Index ||
               cellIndex == ConWallColumnStake4.Index || cellIndex == ConWallColumnStake5.Index ||
               cellIndex == ConWallColumnStake6.Index || cellIndex == ConWallColumnStake7.Index ||
               cellIndex == ConWallColumnStake8.Index || cellIndex == ConWallColumnStake9.Index ||
               cellIndex == ConWallColumnStake10.Index ) {
            int intNum = 0 ;
            double doubleNum = 0 ;
            if ( ! int.TryParse( GetCellVal( cell ), out intNum ) &&
                 ! double.TryParse( GetCellVal( cell ), out doubleNum ) ) {
              return false ;
            }
          }
        }
      }

      return true ;
    }

    /// <summary>
    /// 整数チェック
    /// </summary>
    /// <returns></returns>
    private bool CheackIntValue()
    {
      //if (CheackIntValueKoyaita() &&
      //   CheackIntValueOyakui() &&
      //   CheackIntValueSMW() &&
      //   CheackIntValueConWall())
      //{
      //    return true;
      //}
      //return false;

      return true ;
    }

    /// <summary>
    /// 鋼矢板で整数チェック
    /// </summary>
    /// <returns></returns>
    private bool CheackIntValueKoyaita()
    {
      foreach ( DataGridViewRow row in dgvKoyaita.Rows ) {
        foreach ( DataGridViewCell cell in row.Cells ) {
          int cellIndex = cell.ColumnIndex ;
          if ( cellIndex == KouyaitaColumnJointNum.Index ) {
            int intNum = 0 ;
            if ( ! int.TryParse( GetCellVal( cell ), out intNum ) ) {
              return false ;
            }
          }
        }
      }

      return true ;
    }

    /// <summary>
    /// 親杭で整数チェック
    /// </summary>
    /// <returns></returns>
    private bool CheackIntValueOyakui()
    {
      foreach ( DataGridViewRow row in dgvOyakui.Rows ) {
        foreach ( DataGridViewCell cell in row.Cells ) {
          int cellIndex = cell.ColumnIndex ;
          if ( cellIndex == OyakuiColumnJointNum.Index ) {
            int intNum = 0 ;
            if ( ! int.TryParse( GetCellVal( cell ), out intNum ) ) {
              return false ;
            }
          }
        }
      }

      return true ;
    }

    /// <summary>
    /// SMWで整数チェック
    /// </summary>
    /// <returns></returns>
    private bool CheackIntValueSMW()
    {
      foreach ( DataGridViewRow row in dgvSMW.Rows ) {
        foreach ( DataGridViewCell cell in row.Cells ) {
          int cellIndex = cell.ColumnIndex ;
          if ( cellIndex == SMWColumnJointNum.Index ) {
            int intNum = 0 ;
            if ( ! int.TryParse( GetCellVal( cell ), out intNum ) ) {
              return false ;
            }
          }
        }
      }

      return true ;
    }

    /// <summary>
    /// 連続壁で整数チェック
    /// </summary>
    /// <returns></returns>
    private bool CheackIntValueConWall()
    {
      foreach ( DataGridViewRow row in dgvConWall.Rows ) {
        foreach ( DataGridViewCell cell in row.Cells ) {
          int cellIndex = cell.ColumnIndex ;
          if ( cellIndex == ConWallColumnJointNum.Index ) {
            int intNum = 0 ;
            if ( ! int.TryParse( GetCellVal( cell ), out intNum ) ) {
              return false ;
            }
          }
        }
      }

      return true ;
    }


    private void buttonOK_Click( object sender, EventArgs e )
    {
      ////不正な値を検出
      //if (!CheackNumericValue() || !CheackIntValue())
      //{
      //    MessageBox.Show("不正な値が存在します。");
      //}
      List<ClsCASE.stCASEKoyaita> data = new List<ClsCASE.stCASEKoyaita>() ;
      if ( ! GetKoyaitaCASEData( ref data ) ) {
        MessageBox.Show( "不正な値が存在します。" ) ;
      }

      m_koyaitaDataList = data ;

      List<ClsCASE.stCASEOyagui> data2 = new List<ClsCASE.stCASEOyagui>() ;
      if ( ! GetOyaguiCASEData( ref data2 ) ) {
        MessageBox.Show( "不正な値が存在します。" ) ;
      }

      m_oyaguiDataList = data2 ;

      List<ClsCASE.stCASESMW> data3 = new List<ClsCASE.stCASESMW>() ;
      if ( ! GetSMWCASEData( ref data3 ) ) {
        MessageBox.Show( "不正な値が存在します。" ) ;
      }

      m_smwDataList = data3 ;


      List<ClsCASE.stCASERenzokuKabe> data4 = new List<ClsCASE.stCASERenzokuKabe>() ;
      if ( ! GetReanzokukabeCASEData( ref data4 ) ) {
        MessageBox.Show( "不正な値が存在します。" ) ;
      }

      m_kabeDataList = data4 ;

      if ( m_mode == enMode.editMode ) {
        m_Command = ClsCASE.CASECommand.Change ;
        MakeRequest( RequestId.CASE ) ;
      }
      else {
        m_Command = ClsCASE.CASECommand.Wariate ;
        MakeRequest( RequestId.CASE ) ;
      }
      //this.DialogResult = DialogResult.OK;
      //this.Close();
    }

    private void btnCancel_Click( object sender, EventArgs e )
    {
      m_Command = ClsCASE.CASECommand.Close ;
      MakeRequest( RequestId.CASE ) ;
      //this.DialogResult = DialogResult.Cancel;
      //this.Close();
    }

    private void dgvKoyaita_CellLeave( object sender, DataGridViewCellEventArgs e )
    {
      //var cellEditing = dgvKoyaita.Rows[e.RowIndex].Cells[e.ColumnIndex];
      //if (cellEditing.OwningColumn.Name == "NewKouyaitaColumnCase")
      //{
      //    MessageBox.Show("鋼矢板CASE編集");
      //}
      //else if (cellEditing.OwningColumn.Name == "NewKouyaitaColumnBranchNum")
      //{
      //    MessageBox.Show("鋼矢板枝番編集");
      //}
    }

    /// <summary>
    /// CASE変更処理
    /// </summary>
    /// <param name="dgv"></param>
    /// <param name="row"></param>
    /// <returns>CASEの値を変更=TRUE,CASEの値を未変更=FALSE</returns>
    private bool ChangeCASE( DataGridView dgv, DataGridViewRow row )
    {
      int existRowIndex = 0 ;

      string nowCASE = GetCellVal( row.Cells[ 0 ] ) ;
      string newCASE = GetCellVal( row.Cells[ 1 ] ) ;

      //編集前のCASEと編集後のCASEが同じ値であれば処理を終了
      if ( newCASE == GetCellVal( m_editPreviousDGV.Cells[ 1 ] ) ) {
        return false ;
      }

      if ( CheackExistCASE( dgv, newCASE, row.Index, out existRowIndex ) ) {
        DialogResult dr = MessageBox.Show( "CASE" + newCASE + "は既に存在しています。CASE" + newCASE + "の情報を適用しますか？", "CASE変更",
          MessageBoxButtons.YesNo ) ;
        if ( dr == DialogResult.Yes ) {
          DataGridViewRow sameCASERow = dgv.Rows[ existRowIndex ] ;

          //変更するセルの行の始まりのインデックスと終わりのインデックス
          int startIndex = 2 ;
          int endIndex = dgv.Name == dgvKoyaita.Name || dgv.Name == dgvOyakui.Name ? zanchiLenKO : zanchiLenSR ;

          //CASEが同じ行の値を、CASEを変更する行の値に反映
          for ( int i = startIndex ; i <= endIndex ; i++ ) {
            row.Cells[ i ].Value = GetCellVal( sameCASERow.Cells[ i ] ) ;
          }

          //同じ枝番がCASE○○に存在するか
          int edabanIndex = dgv.Name == dgvKoyaita.Name || dgv.Name == dgvOyakui.Name ? edabanIndexKO : edabanIndexSR ;
          string edaban = GetCellVal( row.Cells[ edabanIndex ] ) ;
          if ( CheackExistCASEAndEdaban( dgv, newCASE, edaban, row.Index, out existRowIndex ) ) {
            //CASEが同じ行のジョイント数と杭の1,2,3～の値を、CASEを変更する行の値に反映　
            int jointNumIndex = dgv.Name == dgvKoyaita.Name || dgv.Name == dgvOyakui.Name
              ? jointNumIndexKO
              : jointNumIndexSR ;
            for ( int i = jointNumIndex ; i < row.Cells.Count ; i++ ) {
              row.Cells[ i ].Value = GetCellVal( sameCASERow.Cells[ i ] ) ;
            }
          }

          return true ;
        }
        else {
          //新しいCASEの値を現在のCASEの値に変更
          row.Cells[ 1 ].Value = nowCASE ;
          return false ;
        }
      }

      return true ;
    }

    /// <summary>
    /// 枝番号変更処理
    /// </summary>
    /// <param name="dgv"></param>
    /// <param name="row"></param>
    /// <returns>枝番号の値を変更=TRUE,枝番号の値を未変更=FALSE</returns>
    private bool ChangeEdaban( DataGridView dgv, DataGridViewRow row )
    {
      int existRowIndex = 0 ;

      int edabanIndex = dgv.Name == dgvKoyaita.Name || dgv.Name == dgvOyakui.Name ? edabanIndexKO : edabanIndexSR ;
      string nowEdaban = GetCellVal( row.Cells[ edabanIndex ] ) ;
      int newEdabanIndex = dgv.Name == dgvKoyaita.Name || dgv.Name == dgvOyakui.Name
        ? newEdabanIndexKO
        : newEdabanIndexSR ;
      string newEdaban = GetCellVal( row.Cells[ newEdabanIndex ] ) ;

      string nowCASE = GetCellVal( row.Cells[ 0 ] ) ;

      //編集前のCASEと編集後のCASEが同じ値であれば処理を終了
      if ( newEdaban == GetCellVal( m_editPreviousDGV.Cells[ newEdabanIndex ] ) ) {
        return false ;
      }

      if ( CheackExistCASEAndEdaban( dgv, nowCASE, newEdaban, row.Index, out existRowIndex ) ) {
        DialogResult dr =
          MessageBox.Show(
            "CASE" + nowCASE + "-" + newEdaban + "は既に存在しています。CASE" + nowCASE + "-" + newEdaban + "の情報を適用しますか？", "枝番号変更",
            MessageBoxButtons.YesNo ) ;
        if ( dr == DialogResult.Yes ) {
          DataGridViewRow sameCASERow = dgv.Rows[ existRowIndex ] ;

          //枝番号が同じ行のジョイント数と杭の1,2,3～の値を、枝番号を変更する行の値に反映　
          int jointNumIndex = dgv.Name == dgvKoyaita.Name || dgv.Name == dgvOyakui.Name
            ? jointNumIndexKO
            : jointNumIndexSR ;
          for ( int i = jointNumIndex ; i < row.Cells.Count ; i++ ) {
            row.Cells[ i ].Value = GetCellVal( sameCASERow.Cells[ i ] ) ;
          }

          return true ;
        }
        else {
          //新しいCASEの値を現在のCASEの値に変更
          row.Cells[ newEdabanIndex ].Value = nowEdaban ;
          return false ;
        }
      }

      return true ;
    }

    private bool ChangeTypeToZanchiLen( DataGridView dgv, DataGridViewRow row )
    {
      int startIndex = 2 ;
      int endIndex = dgv.Name == dgvKoyaita.Name || dgv.Name == dgvOyakui.Name ? zanchiLenKO : zanchiLenSR ;

      //編集前の列と、編集後の列でセル数が異なれば処理を終了
      //または、編集前の列と、編集後の列で値が変わらなければ終了
      if ( row.Cells.Count != m_editPreviousDGV.Cells.Count ) {
        return false ;
      }

      bool isOk = false ;
      for ( int i = startIndex ; i <= endIndex ; i++ ) {
        if ( GetCellVal( row.Cells[ i ] ) != GetCellVal( m_editPreviousDGV.Cells[ i ] ) ) {
          isOk = true ;
          break ;
        }
      }

      if ( ! isOk ) {
        return false ;
      }

      int existRowIndex = 0 ;
      if ( CheackExistTypeToZanchiLen( dgv, row, out existRowIndex, true ) ) {
        DataGridViewRow sameCASERow = dgv.Rows[ existRowIndex ] ;
        DialogResult dr = MessageBox.Show( "CASE" + GetCellVal( sameCASERow.Cells[ 0 ] ) + "の設定値と同一です。変更してよろしいですか?",
          "CASE変更", MessageBoxButtons.YesNo ) ;
        if ( dr == DialogResult.Yes ) {
          return true ;
        }
        else {
          for ( int i = startIndex ; i <= endIndex ; i++ ) {
            row.Cells[ i ].Value = GetCellVal( m_editPreviousDGV.Cells[ i ] ) ;
          }

          return false ;
        }
      }

      return true ;
    }

    private bool CheackExistCASE( DataGridView dgv, string CASE, int targetRowIndex, out int existRowIndex )
    {
      foreach ( DataGridViewRow row in dgv.Rows ) {
        if ( row.Index == targetRowIndex ) {
          continue ;
        }

        if ( GetCellVal( row.Cells[ 0 ] ) == CASE ) {
          existRowIndex = row.Index ;
          return true ;
        }
      }

      existRowIndex = 0 ;
      return false ;
    }

    private bool CheackExistCASEAndEdaban( DataGridView dgv, string CASE, string edaban, int targetRowIndex,
      out int existRowIndex )
    {
      int edabanIndex = dgv.Name == dgvKoyaita.Name || dgv.Name == dgvOyakui.Name ? edabanIndexKO : edabanIndexSR ;
      foreach ( DataGridViewRow row in dgv.Rows ) {
        if ( row.Index == targetRowIndex ) {
          continue ;
        }

        if ( GetCellVal( row.Cells[ 0 ] ) == CASE && GetCellVal( row.Cells[ edabanIndex ] ) == edaban ) {
          existRowIndex = row.Index ;
          return true ;
        }
      }

      existRowIndex = 0 ;
      return false ;
    }

    private bool CheackExistTypeToZanchiLen( DataGridView dgv, DataGridViewRow targetRow, out int existRowIndex,
      bool isExceptSameCase = false )
    {
      //チェックするセルの行の始まりのインデックスと終わりのインデックス
      int startIndex = 2 ;
      int endIndex = dgv.Name == dgvKoyaita.Name || dgv.Name == dgvOyakui.Name ? zanchiLenKO : zanchiLenSR ;
      foreach ( DataGridViewRow row in dgv.Rows ) {
        bool isExist = true ;
        ;
        if ( row.Index == targetRow.Index ) {
          continue ;
        }

        for ( int i = startIndex ; i <= endIndex ; i++ ) {
          if ( GetCellVal( targetRow.Cells[ i ] ) != GetCellVal( row.Cells[ i ] ) ) {
            isExist = false ;
            break ;
          }
        }

        if ( isExist ) {
          if ( isExceptSameCase && GetCellVal( targetRow.Cells[ 0 ] ) == GetCellVal( row.Cells[ 0 ] ) ) {
            continue ;
          }

          existRowIndex = row.Index ;
          return true ;
        }
      }

      existRowIndex = 0 ;
      return false ;
    }

    /// <summary>
    /// DGV内にある同一CASEに存在する枝番Listを取得
    /// </summary>
    /// <param name="dgv"></param>
    /// <param name="CASE"></param>
    /// <returns></returns>
    private List<string> CheackCASEInEdaNum( DataGridView dgv, string CASE )
    {
      List<string> edaNumList = new List<string>() ;
      int edabanIndex = dgv.Name == dgvKoyaita.Name || dgv.Name == dgvOyakui.Name ? edabanIndexKO : edabanIndexSR ;
      foreach ( DataGridViewRow row in dgv.Rows ) {
        if ( GetCellVal( row.Cells[ 0 ] ) == CASE ) {
          edaNumList.Add( GetCellVal( row.Cells[ edabanIndex ] ) ) ;
        }
      }

      return edaNumList ;
    }

    private void dgvOyakui_CellLeave( object sender, DataGridViewCellEventArgs e )
    {
      //var cellEditing = dgvOyakui.Rows[e.RowIndex].Cells[e.ColumnIndex];
      //if (cellEditing.OwningColumn.Name == "NewOyakuiColumnCase")
      //{
      //    MessageBox.Show("親杭CASE編集");
      //}
      //else if (cellEditing.OwningColumn.Name == "NewOyakuiColumnBranchNum")
      //{
      //    MessageBox.Show("親杭枝番編集");
      //}
    }

    private void dgvSMW_CellLeave( object sender, DataGridViewCellEventArgs e )
    {
      //var cellEditing = dgvSMW.Rows[e.RowIndex].Cells[e.ColumnIndex];
      //if (cellEditing.OwningColumn.Name == "NewSMWColumnCase")
      //{
      //    MessageBox.Show("SMWCASE編集");
      //}
      //else if (cellEditing.OwningColumn.Name == "NewSMWColumnBranchNum")
      //{
      //    MessageBox.Show("SMW枝番編集");
      //}
    }

    private void dgvConWall_CellLeave( object sender, DataGridViewCellEventArgs e )
    {
      //var cellEditing = dgvConWall.Rows[e.RowIndex].Cells[e.ColumnIndex];
      //if (cellEditing.OwningColumn.Name == "NewConWallColumnCase")
      //{
      //    MessageBox.Show("連続壁CASE編集");
      //}
      //else if (cellEditing.OwningColumn.Name == "NewConWallColumnBranchNum")
      //{
      //    MessageBox.Show("連続壁枝番編集");
      //}
    }

    private void dgvKoyaita_CellBeginEdit( object sender, DataGridViewCellCancelEventArgs e )
    {
      m_editPreviousDGV = new DataGridViewRow() ;
      m_editPreviousDGV.CreateCells( dgvKoyaita ) ;

      foreach ( DataGridViewCell cell in dgvKoyaita.Rows[ e.RowIndex ].Cells ) {
        m_editPreviousDGV.Cells[ cell.ColumnIndex ].Value = GetCellVal( cell ) ;
      }
    }

    private void dgvKoyaita_CellEndEdit( object sender, DataGridViewCellEventArgs e )
    {
      DataGridViewCell cellEditing = dgvKoyaita.Rows[ e.RowIndex ].Cells[ e.ColumnIndex ] ;
      DataGridViewRow targetRow = dgvKoyaita.Rows[ e.RowIndex ] ;
      switch ( cellEditing.OwningColumn.Name ) {
        case "NewKouyaitaColumnCase" :
          ChangeCASE( dgvKoyaita, targetRow ) ;
          break ;
        case "KouyaitaColumnType" :
          ChangeTypeToZanchiLen( dgvKoyaita, targetRow ) ;
          break ;
        case "KouyaitaColumnSize" :
          ChangeTypeToZanchiLen( dgvKoyaita, targetRow ) ;
          break ;
        case "KouyaitaColumnLength" :
          ChangeTypeToZanchiLen( dgvKoyaita, targetRow ) ;
          break ;
        case "KouyaitaColumnMaterial" :
          ChangeTypeToZanchiLen( dgvKoyaita, targetRow ) ;
          break ;
        case "KouyaitaColumnRemain" :
          ChangeTypeToZanchiLen( dgvKoyaita, targetRow ) ;
          break ;
        case "KouyaitaColumnRemainLen" :
          ChangeTypeToZanchiLen( dgvKoyaita, targetRow ) ;
          break ;
        case "NewKouyaitaColumnBranchNum" :
          ChangeEdaban( dgvKoyaita, targetRow ) ;
          break ;
      }
      //else if (cellEditing.OwningColumn.Name == "NewKouyaitaColumnBranchNum")
      //{
      //    MessageBox.Show("鋼矢板枝番編集");
      //}
    }

    private void dgvOyakui_CellBeginEdit( object sender, DataGridViewCellCancelEventArgs e )
    {
      m_editPreviousDGV = new DataGridViewRow() ;
      m_editPreviousDGV.CreateCells( dgvOyakui ) ;

      foreach ( DataGridViewCell cell in dgvOyakui.Rows[ e.RowIndex ].Cells ) {
        m_editPreviousDGV.Cells[ cell.ColumnIndex ].Value = GetCellVal( cell ) ;
      }
    }

    private void dgvOyakui_CellEndEdit( object sender, DataGridViewCellEventArgs e )
    {
      DataGridViewCell cellEditing = dgvOyakui.Rows[ e.RowIndex ].Cells[ e.ColumnIndex ] ;
      DataGridViewRow targetRow = dgvOyakui.Rows[ e.RowIndex ] ;
      switch ( cellEditing.OwningColumn.Name ) {
        case "NewOyakuiColumnCase" :
          ChangeCASE( dgvOyakui, targetRow ) ;
          break ;
        case "OyakuiColumnType" :
          ChangeTypeToZanchiLen( dgvOyakui, targetRow ) ;
          break ;
        case "OyakuiColumnSize" :
          ChangeTypeToZanchiLen( dgvOyakui, targetRow ) ;
          break ;
        case "OyakuiColumnLength" :
          ChangeTypeToZanchiLen( dgvOyakui, targetRow ) ;
          break ;
        case "OyakuiColumnMaterial" :
          ChangeTypeToZanchiLen( dgvOyakui, targetRow ) ;
          break ;
        case "OyakuiColumnRemain" :
          ChangeTypeToZanchiLen( dgvOyakui, targetRow ) ;
          break ;
        case "OyakuiColumnRemainLen" :
          ChangeTypeToZanchiLen( dgvOyakui, targetRow ) ;
          break ;
        case "NewOyakuiColumnBranchNum" :
          ChangeEdaban( dgvOyakui, targetRow ) ;
          break ;
      }
      //else if (cellEditing.OwningColumn.Name == "NewOyakuiColumnBranchNum")
      //{
      //    MessageBox.Show("親杭枝番編集");
      //}
    }

    private void dgvSMW_CellBeginEdit( object sender, DataGridViewCellCancelEventArgs e )
    {
      m_editPreviousDGV = new DataGridViewRow() ;
      m_editPreviousDGV.CreateCells( dgvSMW ) ;

      foreach ( DataGridViewCell cell in dgvSMW.Rows[ e.RowIndex ].Cells ) {
        m_editPreviousDGV.Cells[ cell.ColumnIndex ].Value = GetCellVal( cell ) ;
      }
    }

    private void dgvSMW_CellEndEdit( object sender, DataGridViewCellEventArgs e )
    {
      DataGridViewCell cellEditing = dgvSMW.Rows[ e.RowIndex ].Cells[ e.ColumnIndex ] ;
      DataGridViewRow targetRow = dgvSMW.Rows[ e.RowIndex ] ;
      switch ( cellEditing.OwningColumn.Name ) {
        case "NewSMWColumnCase" :
          ChangeCASE( dgvSMW, targetRow ) ;
          break ;
        case "SMWColumnType" :
          ChangeTypeToZanchiLen( dgvSMW, targetRow ) ;
          break ;
        case "SMWColumnSize" :
          ChangeTypeToZanchiLen( dgvSMW, targetRow ) ;
          break ;
        case "SMWColumnLength" :
          ChangeTypeToZanchiLen( dgvSMW, targetRow ) ;
          break ;
        case "SMWColumnSoilDia" :
          ChangeTypeToZanchiLen( dgvSMW, targetRow ) ;
          break ;
        case "SMWColumnSoilLen" :
          ChangeTypeToZanchiLen( dgvSMW, targetRow ) ;
          break ;
        case "SMWColumnMaterial" :
          ChangeTypeToZanchiLen( dgvSMW, targetRow ) ;
          break ;
        case "SMWColumnRemain" :
          ChangeTypeToZanchiLen( dgvSMW, targetRow ) ;
          break ;
        case "SMWColumnRemainLen" :
          ChangeTypeToZanchiLen( dgvSMW, targetRow ) ;
          break ;
        case "NewSMWColumnBranchNum" :
          ChangeEdaban( dgvSMW, targetRow ) ;
          break ;
      }
      //else if (cellEditing.OwningColumn.Name == "NewSMWColumnBranchNum")
      //{
      //    MessageBox.Show("SMW枝番編集");
      //}
    }

    private void dgvConWall_CellBeginEdit( object sender, DataGridViewCellCancelEventArgs e )
    {
      m_editPreviousDGV = new DataGridViewRow() ;
      m_editPreviousDGV.CreateCells( dgvConWall ) ;

      foreach ( DataGridViewCell cell in dgvConWall.Rows[ e.RowIndex ].Cells ) {
        m_editPreviousDGV.Cells[ cell.ColumnIndex ].Value = GetCellVal( cell ) ;
      }
    }

    private void dgvConWall_CellEndEdit( object sender, DataGridViewCellEventArgs e )
    {
      DataGridViewCell cellEditing = dgvConWall.Rows[ e.RowIndex ].Cells[ e.ColumnIndex ] ;
      DataGridViewRow targetRow = dgvConWall.Rows[ e.RowIndex ] ;
      switch ( cellEditing.OwningColumn.Name ) {
        case "NewConWallColumnCase" :
          ChangeCASE( dgvConWall, targetRow ) ;
          break ;
        case "ConWallColumnType" :
          ChangeTypeToZanchiLen( dgvConWall, targetRow ) ;
          break ;
        case "ConWallColumnSize" :
          ChangeTypeToZanchiLen( dgvConWall, targetRow ) ;
          break ;
        case "ConWallColumnLength" :
          ChangeTypeToZanchiLen( dgvConWall, targetRow ) ;
          break ;
        case "ConWallColumnWallThick" :
          ChangeTypeToZanchiLen( dgvConWall, targetRow ) ;
          break ;
        case "ConWallColumnWallLen" :
          ChangeTypeToZanchiLen( dgvConWall, targetRow ) ;
          break ;
        case "ConWallColumnMaterial" :
          ChangeTypeToZanchiLen( dgvConWall, targetRow ) ;
          break ;
        case "ConWallColumnRemain" :
          ChangeTypeToZanchiLen( dgvConWall, targetRow ) ;
          break ;
        case "ConWallColumnRemainLen" :
          ChangeTypeToZanchiLen( dgvConWall, targetRow ) ;
          break ;
        case "NewConWallColumnBranchNum" :
          ChangeEdaban( dgvConWall, targetRow ) ;
          break ;
      }
      //else if (cellEditing.OwningColumn.Name == "NewConWallColumnBranchNum")
      //{
      //    MessageBox.Show("連続壁枝番編集");
      //}
    }

    private void dgvKoyaita_CurrentCellDirtyStateChanged( object sender, EventArgs e )
    {
      var dataGridView = sender as DataGridView ;
      if ( dataGridView.IsCurrentCellDirty ) {
        dataGridView.CommitEdit( DataGridViewDataErrorContexts.Commit ) ;
      }
    }

    private void dgvOyakui_CurrentCellDirtyStateChanged( object sender, EventArgs e )
    {
      var dataGridView = sender as DataGridView ;
      if ( dataGridView.IsCurrentCellDirty ) {
        dataGridView.CommitEdit( DataGridViewDataErrorContexts.Commit ) ;
      }
    }

    private void dgvSMW_CurrentCellDirtyStateChanged( object sender, EventArgs e )
    {
      var dataGridView = sender as DataGridView ;
      if ( dataGridView.IsCurrentCellDirty ) {
        dataGridView.CommitEdit( DataGridViewDataErrorContexts.Commit ) ;
      }
    }

    private void dgvConWall_CurrentCellDirtyStateChanged( object sender, EventArgs e )
    {
      var dataGridView = sender as DataGridView ;
      if ( dataGridView.IsCurrentCellDirty ) {
        dataGridView.CommitEdit( DataGridViewDataErrorContexts.Commit ) ;
      }
    }

    private void dgvKoyaita_CellValueChanged( object sender, DataGridViewCellEventArgs e )
    {
      if ( e.RowIndex < 0 ) {
        return ;
      }

      var cellEditing = dgvKoyaita.Rows[ e.RowIndex ].Cells[ e.ColumnIndex ] ;
      if ( cellEditing.OwningColumn.Name == "NewKouyaitaColumnCase" ) {
        // MessageBox.Show(cellEditing.Value.ToString() + "　鋼矢板CASE");
      }
      else if ( cellEditing.OwningColumn.Name == "NewKouyaitaColumnBranchNum" ) {
        //MessageBox.Show(cellEditing.Value.ToString() + "　鋼矢板枝番");
      }
      else if ( cellEditing.OwningColumn.Name == "KouyaitaColumnType" ) {
        var str = cellEditing.Value.ToString() ;
        SetSizeKoyaita( str, e.RowIndex ) ;
      }
    }

    private void dgvOyakui_CellValueChanged( object sender, DataGridViewCellEventArgs e )
    {
      if ( e.RowIndex < 0 ) {
        return ;
      }

      var cellEditing = dgvOyakui.Rows[ e.RowIndex ].Cells[ e.ColumnIndex ] ;
      if ( cellEditing.OwningColumn.Name == "NewOyakuiColumnCase" ) {
        // MessageBox.Show(cellEditing.Value.ToString() + "　親杭CASE");
      }
      else if ( cellEditing.OwningColumn.Name == "NewOyakuiColumnBranchNum" ) {
        // MessageBox.Show(cellEditing.Value.ToString() + "　親杭枝番");
      }
      else if ( cellEditing.OwningColumn.Name == "OyakuiColumnType" ) {
        var str = cellEditing.Value.ToString() ;
        //dgvConWall["OyakuiColumnSize", e.RowIndex].Value = "";
        SetSizeOyagui( str, e.RowIndex ) ;
      }
    }

    private void dgvSMW_CellValueChanged( object sender, DataGridViewCellEventArgs e )
    {
      if ( e.RowIndex < 0 ) {
        return ;
      }

      var cellEditing = dgvSMW.Rows[ e.RowIndex ].Cells[ e.ColumnIndex ] ;
      if ( cellEditing.OwningColumn.Name == "NewSMWColumnCase" ) {
        // MessageBox.Show(cellEditing.Value.ToString() + "　SMWCASE");
      }
      else if ( cellEditing.OwningColumn.Name == "NewSMWColumnBranchNum" ) {
        // MessageBox.Show(cellEditing.Value.ToString() + "　SMW枝番");
      }
      else if ( cellEditing.OwningColumn.Name == "SMWColumnType" ) {
        var str = cellEditing.Value.ToString() ;
        //dgvConWall["SMWColumnSize", e.RowIndex].Value = "";
        SetSizeSMW( str, e.RowIndex ) ;
      }
    }

    private void dgvConWall_CellValueChanged( object sender, DataGridViewCellEventArgs e )
    {
      if ( e.RowIndex < 0 ) {
        return ;
      }

      var cellEditing = dgvConWall.Rows[ e.RowIndex ].Cells[ e.ColumnIndex ] ;
      if ( cellEditing.OwningColumn.Name == "NewConWallColumnCase" ) {
        //MessageBox.Show(cellEditing.Value.ToString() + "　連続壁CASE");
      }
      else if ( cellEditing.OwningColumn.Name == "NewConWallColumnBranchNum" ) {
        //MessageBox.Show(cellEditing.Value.ToString() + "　連続壁枝番");
      }
      else if ( cellEditing.OwningColumn.Name == "ConWallColumnType" ) {
        var str = cellEditing.Value.ToString() ;
        // dgvConWall["ConWallColumnSize", e.RowIndex].Value = "";
        SetSizeRenzokukabe( str, e.RowIndex ) ;
      }
    }

    /// <summary>
    /// 枝番号追加機能　選択された行の下に新たに行を追加する（枝番号は適当なカタカナ）。その際杭1,2,3～の情報を逆順にして挿入する。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAddEdaban_Click( object sender, EventArgs e )
    {
      var dataGridView = sender as DataGridView ;

      if ( dgvKoyaita.CurrentRow != null && dgvKoyaita.Visible ) {
        dataGridView = dgvKoyaita ;
      }
      else if ( dgvOyakui.CurrentRow != null && dgvOyakui.Visible ) {
        dataGridView = dgvOyakui ;
      }
      else if ( dgvSMW.CurrentRow != null && dgvSMW.Visible ) {
        dataGridView = dgvSMW ;
      }
      else if ( dgvConWall.CurrentRow != null && dgvConWall.Visible ) {
        dataGridView = dgvConWall ;
      }

      //行が選択されていない、複数行選択されている場合は処理を終了する
      if ( dataGridView != null ) {
        int targetIndex = dataGridView.CurrentRow.Index + 1 ;
        var row1 = dataGridView.Rows[ targetIndex - 1 ] ;
        List<ElementId> idList = (List<ElementId>) row1.Tag ;
        if ( idList == null || idList.Count == 0 )
          return ;

        dataGridView.Rows.Insert( targetIndex, 1 ) ;
        var row = dataGridView.Rows[ targetIndex ] ;
        foreach ( DataGridViewCell cell in dataGridView.CurrentRow.Cells ) {
          row.Cells[ cell.ColumnIndex ].Value = GetCellVal( cell ) ;
        }

        //枝番号は現状では適当な値を設定する
        int edabanIndex = dataGridView.Name == dgvKoyaita.Name || dataGridView.Name == dgvOyakui.Name
          ? edabanIndexKO
          : edabanIndexSR ;
        int newEdabanIndex = dataGridView.Name == dgvKoyaita.Name || dataGridView.Name == dgvOyakui.Name
          ? newEdabanIndexKO
          : newEdabanIndexSR ;
        //row.Cells[edabanIndex].Value = "テスト";
        string CASE = row.Cells[ 0 ].Value.ToString() ;
        string edaNum = row.Cells[ edabanIndex ].Value.ToString() ;
        List<string> edaNumList = CheackCASEInEdaNum( dataGridView, CASE ) ;
        row.Cells[ newEdabanIndex ].Value = ClsCASE.GetEdabanAlphabet( edaNum, edaNumList ) ;

        //杭1,2,3～の値を逆順にする
        List<string> kuiValues = new List<string>() ;
        int kuiIndexStart = dataGridView.Name == dgvKoyaita.Name || dataGridView.Name == dgvOyakui.Name
          ? KuiIndexStartKO
          : KuiIndexStartSR ;
        for ( int i = kuiIndexStart ; i < row.Cells.Count ; i++ ) {
          if ( row.Cells[ i ].Value != null ) {
            string txt = row.Cells[ i ].Value.ToString() ;
            if ( string.IsNullOrEmpty( txt ) || string.IsNullOrWhiteSpace( txt ) ) {
              break ;
            }

            kuiValues.Add( txt ) ;
          }
          else {
            break ;
          }
        }

        kuiValues.Reverse() ;
        int kuiIndex = kuiIndexStart ;
        for ( int i = 0 ; i < kuiValues.Count ; i++ ) {
          row.Cells[ kuiIndex ].Value = kuiValues[ i ] ;
          kuiIndex++ ;
        }

        SetDefaultCellColor( dataGridView, row ) ;

        Document doc = m_doc ;

        ( List<ElementId> id1List, List<ElementId> id2List ) = ClsCASE.SetOrderKabe( doc, idList ) ;

        row1.Tag = id1List.ToList() ;
        row.Tag = id2List.ToList() ;
      }
      else {
        MessageBox.Show( "1行選択してください。" ) ;
      }
    }

    private void btnAdd_Click( object sender, EventArgs e )
    {
      DataGridView dataGridView = new DataGridView() ;

      if ( dgvKoyaita.Visible ) {
        dataGridView = dgvKoyaita ;
      }
      else if ( dgvOyakui.Visible ) {
        dataGridView = dgvOyakui ;
      }
      else if ( dgvSMW.Visible ) {
        dataGridView = dgvSMW ;
      }
      else if ( dgvConWall.Visible ) {
        dataGridView = dgvConWall ;
      }

      DataGridViewRow addDataGridViewRow = new DataGridViewRow() ;
      addDataGridViewRow.CreateCells( dataGridView ) ;
      dataGridView.Rows.Add( addDataGridViewRow ) ;

      int dgvRowIndex = dataGridView.Rows.Count - 1 ;
      if ( dgvKoyaita.Visible ) {
        dataGridView.Rows[ dgvRowIndex ].Cells[ 2 ].Value = KouyaitaColumnType.Items[ 0 ].ToString() ;
        dataGridView.Rows[ dgvRowIndex ].Cells[ 5 ].Value = KouyaitaColumnMaterial.Items[ 0 ].ToString() ;
        dataGridView.Rows[ dgvRowIndex ].Cells[ 6 ].Value = KouyaitaColumnRemain.Items[ 0 ].ToString() ;
      }
      else if ( dgvOyakui.Visible ) {
        dataGridView.Rows[ dgvRowIndex ].Cells[ 2 ].Value = OyakuiColumnType.Items[ 0 ].ToString() ;
        dataGridView.Rows[ dgvRowIndex ].Cells[ 5 ].Value = OyakuiColumnMaterial.Items[ 0 ].ToString() ;
        dataGridView.Rows[ dgvRowIndex ].Cells[ 6 ].Value = OyakuiColumnRemain.Items[ 0 ].ToString() ;
      }
      else if ( dgvSMW.Visible ) {
        dataGridView.Rows[ dgvRowIndex ].Cells[ 2 ].Value = SMWColumnType.Items[ 0 ].ToString() ;
        dataGridView.Rows[ dgvRowIndex ].Cells[ 5 ].Value = SMWColumnMaterial.Items[ 0 ].ToString() ;
        dataGridView.Rows[ dgvRowIndex ].Cells[ 6 ].Value = SMWColumnRemain.Items[ 0 ].ToString() ;
      }
      else if ( dgvConWall.Visible ) {
        dataGridView.Rows[ dgvRowIndex ].Cells[ 2 ].Value = ConWallColumnType.Items[ 0 ].ToString() ;
        dataGridView.Rows[ dgvRowIndex ].Cells[ 5 ].Value = ConWallColumnMaterial.Items[ 0 ].ToString() ;
        dataGridView.Rows[ dgvRowIndex ].Cells[ 6 ].Value = ConWallColumnRemain.Items[ 0 ].ToString() ;
      }

      SetDefaultCellColor( dataGridView, dataGridView.Rows[ dgvRowIndex ] ) ;
    }

    private void dgvKoyaita_CellClick( object sender, DataGridViewCellEventArgs e )
    {
      int nRowIndex = e.RowIndex ;
      if ( nRowIndex < 0 ) return ;
      m_Command = ClsCASE.CASECommand.HighLight ;
      m_enKabeshu = ClsCASE.enKabeshu.koyauita ;
      m_HighLightCASE = dgvKoyaita[ 0, nRowIndex ].Value.ToString() ;
      m_HighLightEdaNum = dgvKoyaita[ edabanIndexKO, nRowIndex ].Value.ToString() ;
      //MakeRequest(RequestId.CASE);
    }

    private void dgvOyakui_CellClick( object sender, DataGridViewCellEventArgs e )
    {
      int nRowIndex = e.RowIndex ;
      if ( nRowIndex < 0 ) return ;
      m_Command = ClsCASE.CASECommand.HighLight ;
      m_enKabeshu = ClsCASE.enKabeshu.oyagui ;
      m_HighLightCASE = dgvOyakui[ 0, nRowIndex ].Value.ToString() ;
      m_HighLightEdaNum = dgvOyakui[ edabanIndexKO, nRowIndex ].Value.ToString() ;
      //MakeRequest(RequestId.CASE);
    }

    private void dgvSMW_CellClick( object sender, DataGridViewCellEventArgs e )
    {
      int nRowIndex = e.RowIndex ;
      if ( nRowIndex < 0 ) return ;
      m_Command = ClsCASE.CASECommand.HighLight ;
      m_enKabeshu = ClsCASE.enKabeshu.smw ;
      m_HighLightCASE = dgvSMW[ 0, nRowIndex ].Value.ToString() ;
      m_HighLightEdaNum = dgvSMW[ edabanIndexSR, nRowIndex ].Value.ToString() ;
      //MakeRequest(RequestId.CASE);
    }

    private void dgvConWall_CellClick( object sender, DataGridViewCellEventArgs e )
    {
      int nRowIndex = e.RowIndex ;
      if ( nRowIndex < 0 ) return ;
      m_Command = ClsCASE.CASECommand.HighLight ;
      m_enKabeshu = ClsCASE.enKabeshu.renzokukabe ;
      m_HighLightCASE = dgvConWall[ 0, nRowIndex ].Value.ToString() ;
      m_HighLightEdaNum = dgvConWall[ edabanIndexSR, nRowIndex ].Value.ToString() ;
      //MakeRequest(RequestId.CASE);
    }

    private void btnHighLight_Click( object sender, EventArgs e )
    {
      MakeRequest( RequestId.CASE ) ;
    }
  }
}