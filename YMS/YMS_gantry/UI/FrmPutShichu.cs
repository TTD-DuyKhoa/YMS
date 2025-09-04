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
using YMS_gantry.Data ;
using YMS_gantry.GantryUtils ;
using YMS.gantry ;
using System.IO ;

namespace YMS_gantry.UI
{
  public partial class FrmPutShichu : System.Windows.Forms.Form
  {
    #region 定数

    /// <summary>
    /// Iniファイルのパス
    /// </summary>
    const string IniPath = "ini\\FrmPutShichu.ini" ;

    /// <summary>
    /// Iniファイルのセクション名
    /// </summary>
    const string sec = "FrmPutShichu" ;

    #endregion

    private RequestHandler m_Handler ;
    private ExternalEvent m_ExEvent ;

    private UIApplication _uiApp { get ; set ; }
    private Document doc { get ; set ; }

    public FrmPutShichu( UIApplication uiApp )
    {
      InitializeComponent() ;
      _uiApp = uiApp ;
      doc = uiApp.ActiveUIDocument.Document ;
    }

    public FrmPutShichu( ExternalEvent exEvent, RequestHandler handler, UIApplication uiApp )
    {
      InitializeComponent() ;
      _uiApp = uiApp ;
      doc = uiApp.ActiveUIDocument.Document ;

      m_Handler = handler ;
      m_ExEvent = exEvent ;
    }

    private void FrmPutShichu_Load( object sender, EventArgs e )
    {
      ////構台名をコンボボックスに追加
      //comboBox12.Items.Clear();
      //comboBox12.Items.AddRange(GantryUtil.GetAllKoudaiName(doc).ToArray());
      ////comboBox12.Text = comboBox12.Items[0].ToString();

      ////レベル追加
      //comboBox18.Items.Clear();
      //comboBox18.Items.AddRange(GantryUtil.GetAllLevelName(doc).ToArray());
      //comboBox18.Text = comboBox18.Items[0].ToString();

      // コンボボックス初期化
      InitComboBox() ;
      GetIniData() ;
      UpdatePLJointCtrl() ;
    }


    /// <summary>
    /// コンボボックス初期化
    /// </summary>
    private void InitComboBox()
    {
      List<string> lstStr = new List<string>() ;

      //イベント
      CmbShichuSizeType.SelectedIndexChanged += InitComboBoxKui ;

      //レベル名
      lstStr = GantryUtil.GetAllLevelName( doc ) ;
      ControlUtils.SetComboBoxItems( CmbLevel, lstStr ) ;
      this.CmbLevel.Items.Add( "部材選択" ) ;

      //全構台名
      lstStr = GantryUtil.GetAllKoudaiName( doc ) ;
      ControlUtils.SetComboBoxItems( CmbKoudaiName, lstStr ) ;

      //杭
      lstStr = new List<string>()
      {
        Master.ClsSichuCsv.HKou, Master.ClsSichuCsv.Yamadome, Master.ClsSichuCsv.Koukyoudo
      } ;
      ControlUtils.SetComboBoxItems( CmbShichuSizeType, lstStr ) ;

      //トッププレート
      lstStr = Master.ClsTopPlateCsv.GetSizeList() ;
      ControlUtils.SetComboBoxItems( CmbTopPlateSize, lstStr, true ) ;

      //材質
      lstStr = DefineUtil.ListMaterial ;
      ControlUtils.SetComboBoxItems( CmbMaterial, lstStr ) ;
    }

    /// <summary>
    /// コンボボックス初期化（杭）
    /// </summary>
    private void InitComboBoxKui( object sender, EventArgs e )
    {
      UpdateSizeList() ;
    }

    private void UpdateSizeList()
    {
      List<string> lstStr = new List<string>() ;

      //杭
      string type = CmbShichuSizeType.Text ;

      if ( type == Master.ClsSichuCsv.HKou || type == Master.ClsSichuCsv.Yamadome ||
           type == Master.ClsSichuCsv.Koukyoudo ) //H鋼
      {
        lstStr = Master.ClsSichuCsv.GetSizeList( type ) ;
      }

      ControlUtils.SetComboBoxItems( CmbShichuSize, lstStr ) ;
    }

    /// <summary>
    /// 杭コントロール更新
    /// </summary>
    public void InitPileControl()
    {
      string bk = string.Empty ;
      List<string> lstStr = new List<string>() ;

      if ( CmbJointAttach.Text == Master.ClsPileTsugiteCsv.KoteiHohoBolt ) {
        lstStr = Master.ClsBoltCsv.GetSizeList( Master.ClsBoltCsv.BoltTypeT ) ;
      }
      else {
        lstStr = new List<string>() { string.Empty } ;
      }

      lstStr = Master.ClsBoltCsv.GetSizeList( Master.ClsBoltCsv.BoltTypeT ) ;

      //ボルト1-1
      ControlUtils.SetComboBoxItems( CmbKuiJointBoltSizeF, lstStr ) ;

      //ボルト1-2
      ControlUtils.SetComboBoxItems( CmbKuiJointBoltSizeW, lstStr ) ;

      string strKotei = CmbJointAttach.Text ;
      if ( string.IsNullOrEmpty( strKotei ) )
        return ;

      //CSVから継手情報を取得
      string psize = CmbShichuSize.Text.Replace( "H", "H" ) ;
      Master.ClsPileTsugiteCsv pileCsv = Master.ClsPileTsugiteCsv.GetCls( strKotei, CmbShichuSize.Text ) ;
      if ( string.IsNullOrEmpty( pileCsv.PlateSizeFOut ) )
        return ;

      //各コントロールに値を設定
      MaterialSize size = GantryUtil.GetKouzaiSize( pileCsv.PlateSizeFOut ) ;
      if ( size != null ) {
        txtJointPlSizeFO1.Text = size.Thick.ToString() ;
        txtJointPlSizeFO2.Text = size.Height.ToString() ;
        txtJointPlSizeFO3.Text = size.Width.ToString() ;
        txtJointPlSizeFO4.Text = pileCsv.PlateNumFOut.ToString() ;
      }
      else {
        txtJointPlSizeFO1.Text = string.Empty ;
        txtJointPlSizeFO2.Text = string.Empty ;
        txtJointPlSizeFO3.Text = string.Empty ;
        txtJointPlSizeFO4.Text = string.Empty ;
      }

      size = GantryUtil.GetKouzaiSize( pileCsv.PlateSizeFIn ) ;
      if ( size != null ) {
        txtJointPlSizeFI1.Text = size.Thick.ToString() ;
        txtJointPlSizeFI2.Text = size.Height.ToString() ;
        txtJointPlSizeFI3.Text = size.Width.ToString() ;
        txtJointPlSizeFI4.Text = pileCsv.PlateNumFIn.ToString() ;
      }
      else {
        txtJointPlSizeFI1.Text = string.Empty ;
        txtJointPlSizeFI2.Text = string.Empty ;
        txtJointPlSizeFI3.Text = string.Empty ;
        txtJointPlSizeFI4.Text = string.Empty ;
      }

      size = GantryUtil.GetKouzaiSize( pileCsv.PlateSizeW ) ;
      if ( size != null ) {
        txtJointPlSizeW1.Text = size.Thick.ToString() ;
        txtJointPlSizeW2.Text = size.Height.ToString() ;
        txtJointPlSizeW3.Text = size.Width.ToString() ;
        txtJointPlSizeW4.Text = pileCsv.PlateNumW.ToString() ;
      }
      else {
        txtJointPlSizeW1.Text = string.Empty ;
        txtJointPlSizeW2.Text = string.Empty ;
        txtJointPlSizeW3.Text = string.Empty ;
        txtJointPlSizeW4.Text = string.Empty ;
      }

      CmbKuiJointBoltSizeF.Text = pileCsv.BoltSizeF ;
      txtJointBoltF.Text = pileCsv.BoltNumF.ToString() ;
      CmbKuiJointBoltSizeW.Text = pileCsv.BoltSizeW ;
      txtJointBoltW.Text = pileCsv.BoltNumW.ToString() ;
    }

    /// <summary>
    /// ダイアログデータをini二セット
    /// </summary>
    public void SetIniData()
    {
      string iniPath = ClsIni.GetIniFilePath( IniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtElement.Name, RbtElement.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtFree.Name, RbtFree.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbLevel.Name, CmbLevel.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcOffset.Name, NmcOffset.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbShichuSizeType.Name, CmbShichuSizeType.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbShichuSize.Name, CmbShichuSize.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbMaterial.Name, CmbMaterial.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcKuiLng.Name, NmcKuiLng.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcKuiWholeLng.Name, NmcKuiWholeLng.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, ChckKuiHasCut.Name, ChckKuiHasCut.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcKuiHeadLng.Name, NmcKuiHeadLng.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, ChkHasTopPlate.Name, ChkHasTopPlate.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbTopPlateSize.Name, CmbTopPlateSize.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, ChkHasBasePlate.Name, ChkHasBasePlate.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbBasePlateType.Name, CmbBasePlateType.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcBasePlateThick.Name, NmcBasePlateThick.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcJointCnt.Name, NmcJointCnt.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbJointAttach.Name, CmbJointAttach.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbKuiJointBoltSizeF.Name, CmbKuiJointBoltSizeF.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbKuiJointBoltSizeW.Name, CmbKuiJointBoltSizeW.Text, iniPath ) ;

      var rowSpanList = new List<string>() ;
      foreach ( DataGridViewRow row in DgvJointSpan.Rows ) {
        var rowSpanValue = row.Cells[ 1 ].Value?.ToString() ;
        if ( string.IsNullOrEmpty( rowSpanValue ) ) continue ;
        rowSpanList.Add( rowSpanValue ) ;
      }

      var rowSpan = string.Join( "|", rowSpanList ) ;
      ClsIni.WritePrivateProfileString( sec, DgvJointSpan.Name, rowSpan, iniPath ) ;
    }

    /// <summary>
    /// iniデータをダイアログにセット
    /// </summary>
    public void GetIniData()
    {
      string iniPath = ClsIni.GetIniFilePath( IniPath ) ;
      if ( File.Exists( iniPath ) ) {
        RbtElement.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtElement.Name, iniPath ) ) ;
        RbtFree.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtFree.Name, iniPath ) ) ;
        CmbLevel.Text = ClsIni.GetIniFile( sec, CmbLevel.Name, iniPath ) ;
        NmcOffset.Text = ClsIni.GetIniFile( sec, NmcOffset.Name, iniPath ) ;
        CmbShichuSizeType.Text = ClsIni.GetIniFile( sec, CmbShichuSizeType.Name, iniPath ) ;
        CmbShichuSize.Text = ClsIni.GetIniFile( sec, CmbShichuSize.Name, iniPath ) ;
        CmbMaterial.Text = ClsIni.GetIniFile( sec, CmbMaterial.Name, iniPath ) ;
        NmcKuiLng.Text = ClsIni.GetIniFile( sec, NmcKuiLng.Name, iniPath ) ;
        NmcKuiWholeLng.Text = ClsIni.GetIniFile( sec, NmcKuiWholeLng.Name, iniPath ) ;
        ChckKuiHasCut.Checked = bool.Parse( ClsIni.GetIniFile( sec, ChckKuiHasCut.Name, iniPath ) ) ;
        NmcKuiHeadLng.Text = ClsIni.GetIniFile( sec, NmcKuiHeadLng.Name, iniPath ) ;
        ChkHasTopPlate.Checked = bool.Parse( ClsIni.GetIniFile( sec, ChkHasTopPlate.Name, iniPath ) ) ;
        CmbTopPlateSize.Text = ClsIni.GetIniFile( sec, CmbTopPlateSize.Name, iniPath ) ;
        ChkHasBasePlate.Checked = bool.Parse( ClsIni.GetIniFile( sec, ChkHasBasePlate.Name, iniPath ) ) ;
        CmbBasePlateType.Text = ClsIni.GetIniFile( sec, CmbBasePlateType.Name, iniPath ) ;
        NmcBasePlateThick.Text = ClsIni.GetIniFile( sec, NmcBasePlateThick.Name, iniPath ) ;
        NmcJointCnt.Text = ClsIni.GetIniFile( sec, NmcJointCnt.Name, iniPath ) ;
        CmbJointAttach.Text = ClsIni.GetIniFile( sec, CmbJointAttach.Name, iniPath ) ;
        CmbKuiJointBoltSizeF.Text = ClsIni.GetIniFile( sec, CmbKuiJointBoltSizeF.Name, iniPath ) ;
        CmbKuiJointBoltSizeW.Text = ClsIni.GetIniFile( sec, CmbKuiJointBoltSizeW.Name, iniPath ) ;

        var dgvJointSpanValue = ClsIni.GetIniFile( sec, DgvJointSpan.Name, iniPath ) ;
        if ( ! string.IsNullOrEmpty( dgvJointSpanValue ) && dgvJointSpanValue != "none" ) {
          var dgvJointSpanArray = dgvJointSpanValue.Split( '|' ) ;

          for ( var i = 0 ; i < dgvJointSpanArray.Count() ; i++ ) {
            DgvJointSpan.Rows[ i ].Cells[ 1 ].Value = dgvJointSpanArray[ i ] ;
          }
        }
      }
    }

    private void comboBox12_SelectedIndexChanged( object sender, EventArgs e )
    {
      string koudaiName = this.CmbKoudaiName.Text ;
      KoudaiData data = GantryUtil.GetKoudaiData( doc, koudaiName ) ;
      if ( data.AllKoudaiFlatData != null && data.AllKoudaiFlatData.KoudaiName != "" ) {
        AllKoudaiFlatFrmData kdata = data.AllKoudaiFlatData ;
        this.CmbShichuSize.Text = kdata.pilePillerData.PillarSize ;
        this.CmbMaterial.Text = kdata.pilePillerData.PillarMaterial ;
        this.CmbLevel.Text = kdata.SelectedLevel ;
        this.NmcOffset.Value = (decimal) kdata.LevelOffset ;
        this.NmcKuiLng.Value = (decimal) kdata.pilePillerData.PillarLength ;
        this.ChckKuiHasCut.Checked = kdata.pilePillerData.IsCut ;
        this.NmcKuiWholeLng.Value = (decimal) kdata.pilePillerData.PillarWholeLength ;

        this.ChkHasTopPlate.Checked = kdata.pilePillerData.HasTopPlate ;
        this.CmbTopPlateSize.Text = kdata.pilePillerData.topPlateData.PlateSize ;

        //if (true)
        //{
        //    string[] plateSize = kdata.pilePillerData.topPlateData.PlateSize.Split('x');
        //    this.numericUpDown6.Value = decimal.Parse(plateSize[0]);
        //    this.numericUpDown7.Value = decimal.Parse(plateSize[1]);
        //    this.numericUpDown8.Value = decimal.Parse(plateSize[2]);
        //}
        this.NmcKuiHeadLng.Value = (decimal) kdata.pilePillerData.exPillarHeadLeng ;

        //継手詳細
        this.NmcJointCnt.Value = kdata.pilePillerData.pJointCount ;
        this.CmbJointAttach.Text =
          kdata.pilePillerData.jointDetailData.JointType == DefineUtil.eJoinType.Bolt ? "ボルト" : "溶接" ;

        DgvJointSpan.Rows.Clear() ;
        foreach ( double pitch in kdata.pilePillerData.pJointPitch ) {
          if ( pitch == 0 ) {
            continue ;
          }

          DgvJointSpan.Rows.Add( new string[] { $"{DgvJointSpan.Rows.Count + 1}", $"{pitch}" } ) ;
        }
      }

      CalcPileLength() ;
    }

    private void numericUpDown1_ValueChanged( object sender, EventArgs e )
    {
      decimal currentValue = NmcJointCnt.Value ;

      // 以前の値が存在する場合、比較を行う
      if ( NmcJointCnt.Tag != null ) {
        decimal previousValue = decimal.Parse( NmcJointCnt.Tag.ToString() ) ;

        if ( currentValue > previousValue ) {
          int dif = (int) ( currentValue - previousValue ) ;
          // 増加した場合の処理
          for ( int i = 0 ; i < dif ; i++ ) {
            object[] aR = new object[] { $"{DgvJointSpan.Rows.Count + 1}", 0 } ;
            DgvJointSpan.Rows.Add( aR ) ;
          }
        }
        else if ( currentValue < previousValue && DgvJointSpan.Rows.Count > 0 ) {
          // 減少した場合の処理
          int dif = (int) ( previousValue - currentValue ) ;
          if ( dif > 0 ) {
            for ( int i = 0 ; i < dif ; i++ ) {
              DgvJointSpan.Rows.RemoveAt( DgvJointSpan.Rows.Count - 1 ) ;
            }
          }
          else {
            DgvJointSpan.Rows.Clear() ;
          }
        }
        else {
          // 値が変化していない場合の処理
        }
      }

      // 現在の値を以前の値として保存
      NmcJointCnt.Tag = currentValue ;
    }

    private void button1_Click( object sender, EventArgs e )
    {
      if ( ! CheckInpurValues() ) {
        return ;
      }

      MakeRequest( RequestId.Shichu ) ;
      SetIniData() ;
      this.DialogResult = DialogResult.OK ;
    }

    private void button2_Click( object sender, EventArgs e )
    {
      //初期化処理
      MakeRequest( RequestId.End ) ;
      m_ExEvent.Dispose() ;
      m_ExEvent = null ;
      m_Handler = null ;

      this.DialogResult = DialogResult.Cancel ;
      this.Close() ;
    }

    /// <summary>
    /// NumericupDownの空回避用処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void numeriUpDown_Leave( object sender, EventArgs e )
    {
      NumericUpDown nmc = (NumericUpDown) sender ;
      if ( nmc.Text == "" ) {
        nmc.Value = 0 ;
        nmc.Text = "0" ;
      }

      base.OnLostFocus( e ) ;
    }

    private void comboBox17_SelectedIndexChanged( object sender, EventArgs e )
    {
      string str = CmbTopPlateSize.Text ;
      //bool bE = true;
      //if (str != Master.ClsTopPlateCsv.FreeSize)
      //{
      //    bE = false;
      //}
      //numericUpDown6.Enabled = bE;
      //numericUpDown7.Enabled = bE;
      //numericUpDown8.Enabled = bE;
    }

    private void comboBox7_SelectedIndexChanged( object sender, EventArgs e )
    {
      InitPileControl() ;
    }

    private void CmbSize_SelectedIndexChanged( object sender, EventArgs e )
    {
      InitPileControl() ;
    }

    private void CalcPileLength()
    {
      AllKoudaiFlatFrmData kData = GantryUtil.GetKoudaiData( doc, this.CmbKoudaiName.Text ).AllKoudaiFlatData ;
      if ( kData == null ) {
        return ;
      }

      double fukkouThick = DefineUtil.FukkouBAN_THICK ;

      double nedaHeight = ( kData.nedaData.NedaSize != "" )
        ? GantryUtil.GetKouzaiSize( kData.nedaData.NedaSize ).Height
        : 0 ;
      double ohbikiHeight = ( kData.ohbikiData.OhbikiSize != "" )
        ? GantryUtil.GetKouzaiSize( kData.ohbikiData.OhbikiSize ).Height
        : 0 ;
      double topPLthick = ( kData.pilePillerData.topPlateData.PlateSize != "" )
        ? GantryUtil.GetKouzaiSize( kData.pilePillerData.topPlateData.PlateSize ).Thick
        : 0 ;
      double offset = (double) this.NmcOffset.Value ;
      bool isRbtKenchiku = kData.KoujiType == DefineUtil.eKoujiType.Kenchiku ;
      bool isFukkouTop = kData.BaseLevel == DefineUtil.eBaseLevel.FukkouTop ;
      double retLeng = (double) this.NmcKuiLng.Value ;
      if ( isFukkouTop ) {
        if ( this.ChckKuiHasCut.Checked ) {
          if ( this.NmcOffset.Value < 0 ) {
            retLeng = retLeng + ( fukkouThick + nedaHeight + ohbikiHeight ) + Math.Abs( offset ) ;
          }
          else {
            retLeng = retLeng + ( fukkouThick + nedaHeight + ohbikiHeight ) - offset ;
          }
        }
      }
      else {
        if ( this.ChckKuiHasCut.Checked ) {
          if ( this.NmcOffset.Value < 0 ) {
            retLeng = retLeng + Math.Abs( offset ) ;
          }
        }
      }

      if ( retLeng >= 0 ) {
        this.NmcKuiWholeLng.Value = (decimal) retLeng ;
      }
    }

    private void NmcOffset_ValueChanged( object sender, EventArgs e )
    {
      CalcPileLength() ;
    }

    private void NmcKuiLng_ValueChanged( object sender, EventArgs e )
    {
      CalcPileLength() ;
    }

    private void CmbShichuSizeType_SelectedIndexChanged( object sender, EventArgs e )
    {
      UpdateSizeList() ;

      UpdatePLJointCtrl() ;
    }

    private void UpdatePLJointCtrl()
    {
      //山留材はPL,継手の選択不可
      if ( this.CmbShichuSizeType.Text.Contains( "山留材" ) ) {
        this.groupBox1.Enabled = false ;
        this.groupBox3.Enabled = false ;
        this.groupBox5.Enabled = false ;
        this.NmcJointCnt.Value = 0 ;
        this.DgvJointSpan.Rows.Clear() ;
      }
      else {
        this.groupBox1.Enabled = true ;
        this.groupBox3.Enabled = true ;
        this.groupBox5.Enabled = true ;
      }
    }


    private bool CheckInpurValues()
    {
      List<string> errMsg = new List<string>() ;

      if ( string.IsNullOrEmpty( CmbKoudaiName.Text ) ) {
        errMsg.Add( "構台名を指定してください" ) ;
      }

      if ( string.IsNullOrEmpty( CmbMaterial.Text ) ) {
        errMsg.Add( "材質を設定してください" ) ;
      }

      if ( string.IsNullOrEmpty( CmbShichuSize.Text ) ) {
        errMsg.Add( "サイズを設定してください" ) ;
      }

      if ( RbtFree.Checked && string.IsNullOrEmpty( CmbLevel.Text ) ) {
        errMsg.Add( "配置するレベルを指定してください" ) ;
      }

      if ( NmcKuiLng.Value <= 0 ) {
        errMsg.Add( "長さを指定してください" ) ;
      }

      if ( ChkHasTopPlate.Checked && string.IsNullOrEmpty( CmbTopPlateSize.Text ) ) {
        errMsg.Add( "トッププレートサイズを指定してください" ) ;
      }

      if ( NmcJointCnt.Value > 0 ) {
        foreach ( DataGridViewRow row in DgvJointSpan.Rows ) {
          double span = RevitUtil.ClsCommonUtils.ChangeStrToDbl( row.Cells[ 1 ].Value.ToString() ) ;
          if ( span <= 0 ) {
            errMsg.Add( "継手スパンが０に設定されています" ) ;
            break ;
          }
        }

        if ( CmbJointAttach.Text == "ボルト" ) {
          if ( string.IsNullOrEmpty( CmbKuiJointBoltSizeF.Text ) ) {
            errMsg.Add( "継手フランジ側ボルト種類を指定してください" ) ;
          }

          if ( string.IsNullOrEmpty( CmbKuiJointBoltSizeW.Text ) ) {
            errMsg.Add( "継手ウェブ側ボルト種類を指定してください" ) ;
          }
        }
      }

      if ( errMsg.Count > 0 ) {
        FrmErrorInformation frm = new FrmErrorInformation( errMsg, this ) ;
        return false ;
      }
      else {
        return true ;
      }
    }

    #region "モードレス対応"

    /// <summary>
    ///   WakeUp -> enable all controls
    /// </summary>
    /// 
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
      //if (!status)
      //{
      //    this.button3.Enabled = true;
      //}
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

    #endregion

    private void CmbJointAttach_SelectedIndexChanged( object sender, EventArgs e )
    {
      InitPileControl() ;
    }

    private void NmcKuiWholeLng_ValueChanged( object sender, EventArgs e )
    {
    }

    private void RbtElement_CheckedChanged( object sender, EventArgs e )
    {
      ChngePlaceType() ;
    }

    private void RbtFree_CheckedChanged( object sender, EventArgs e )
    {
      ChngePlaceType() ;
    }

    private void ChngePlaceType()
    {
      if ( this.RbtElement.Checked ) {
        this.CmbLevel.Enabled = false ;
        this.NmcOffset.Enabled = false ;
        if ( CmbLevel.Items.Contains( "部材選択" ) ) {
          CmbLevel.Items.Remove( "部材選択" ) ;
        }
      }
      else {
        this.CmbLevel.Enabled = true ;
        this.NmcOffset.Enabled = true ;
        if ( ! CmbLevel.Items.Contains( "部材選択" ) ) {
          CmbLevel.Items.Add( "部材選択" ) ;
        }
      }
    }
  }
}