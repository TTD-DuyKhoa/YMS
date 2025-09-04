using Autodesk.Revit.DB ;
using System ;
using System.Collections.Generic ;
using System.ComponentModel ;
using System.Data ;
using System.Drawing ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using YMS_gantry.Master ;
using Form = System.Windows.Forms.Form ;
using VMType = YMS_gantry.UI.FrmCreateBraceAndJointViewModel ;
using YMS.gantry ;
using Autodesk.Revit.UI ;
using System.IO ;
using YMS_gantry.GantryUtils ;

namespace YMS_gantry.UI
{
  public partial class FrmPutHorizontalTsunagi : Form
  {
    #region 定数

    /// <summary>
    /// Iniファイルのパス
    /// </summary>
    const string IniPath = "ini\\FrmPutHorizontalTsunagi.ini" ;

    /// <summary>
    /// Iniファイルのセクション名
    /// </summary>
    const string sec = "FrmPutHorizontalTsunagi" ;

    #endregion

    private RequestHandler m_Handler ;
    private ExternalEvent m_ExEvent ;

    private UIApplication _uiApp { get ; set ; }
    private Document doc { get ; set ; }

    public VMType ViewModel { get ; } = new VMType() ;

    public FrmPutHorizontalTsunagi( ExternalEvent exEvent, RequestHandler handler, UIApplication uiApp )
    {
      InitializeComponent() ;
      InitializeBindings() ;
      _uiApp = uiApp ;
      doc = uiApp.ActiveUIDocument.Document ;

      m_Handler = handler ;
      m_ExEvent = exEvent ;
    }

    private void radioButton3_CheckedChanged( object sender, EventArgs e )
    {
      //tabControl1.Enabled = RbtPutWayMulti.Checked;

      if ( this.RbtPutWayMulti.Checked ) {
        this.NmcDanCount.Enabled = true ;
        this.NmcDanSpan.Enabled = true ;
        this.DgvDansu.Enabled = true ;
        return ;
      }

      this.NmcDanCount.Enabled = false ;
      this.NmcDanSpan.Enabled = false ;
      this.DgvDansu.Enabled = false ;
    }


    private void InitializeBindings()
    {
      var vm = ViewModel ;

      CmbKoudaiName.DataBindings.Add( nameof( System.Windows.Forms.ComboBox.DataSource ), vm, nameof( VMType.KodaiSet ),
        true, DataSourceUpdateMode.OnPropertyChanged ) ;
      CmbKoudaiName.DisplayMember = nameof( AllKoudaiFlatFrmData.KoudaiName ) ;
      CmbKoudaiName.DataBindings.Add( nameof( System.Windows.Forms.ComboBox.SelectedValue ), vm,
        nameof( VMType.SelectedKodai ), true, DataSourceUpdateMode.OnPropertyChanged ) ;

      CmbSize.DataBindings.Add( nameof( System.Windows.Forms.ComboBox.DataSource ), vm,
        nameof( VMType.HrzJointSizeList ), true, DataSourceUpdateMode.OnPropertyChanged ) ;
      CmbSize.DisplayMember = nameof( ClsChannelCsv.Size ) ;
      CmbSize.DataBindings.Add( nameof( System.Windows.Forms.ComboBox.SelectedValue ), vm,
        nameof( VMType.HrzJointSize ), true, DataSourceUpdateMode.OnPropertyChanged ) ;

      RbtPutWayMulti.DataBindings.Add( nameof( RadioButton.Checked ), vm, nameof( VMType.PlaceIsMultiple ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      RbtPutWaySingle.DataBindings.Add( nameof( RadioButton.Checked ), vm, nameof( VMType.PlaceIsSingle ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;

      NmcSLng.DataBindings.Add( nameof( YmsNumericInteger.DoubleValue ), vm, nameof( VMType.HrzJointStartMm ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      NmcELng.DataBindings.Add( nameof( YmsNumericInteger.DoubleValue ), vm, nameof( VMType.HrzJointEndMm ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;

      RbtAttachWayWelding.DataBindings.Add( nameof( RadioButton.Checked ), vm, nameof( VMType.HrzJointIsWelding ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      RbtAttachWayBolt.DataBindings.Add( nameof( RadioButton.Checked ), vm, nameof( VMType.HrzJointIsBolt ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      RbtAttachWayTeiketsu.DataBindings.Add( nameof( RadioButton.Checked ), vm, nameof( VMType.HrzJointIsMetal ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;

      NmcDanCount.DataBindings.Add( nameof( YmsNumericInteger.DoubleValue ), vm, nameof( VMType.DanCount ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      NmcDanSpan.DataBindings.Add( nameof( YmsNumericInteger.DoubleValue ), vm, nameof( VMType.DanBaseInterval ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;

      ColDansuNo.DataPropertyName = nameof( VMType.DansuRow.Index ) ;
      ColDansuSpan.DataPropertyName = nameof( VMType.DansuRow.IntervalMm ) ;
      chkTsunagiUmu.DataPropertyName = nameof( VMType.DansuRow.HasJoint ) ;

      DgvDansu.DataBindings.Add( nameof( DataGridView.DataSource ), vm, nameof( VMType.DanRowSet ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      DgvDansu.AutoGenerateColumns = false ;
    }

    protected override void OnLoad( EventArgs e )
    {
      base.OnLoad( e ) ;

      if ( ViewModel.SelectedKodai == null ) {
        ViewModel.SelectedKodai = ViewModel.KodaiSet.FirstOrDefault() ;
      }

      //ClsAngleCsv.GetCsv();
      ViewModel.VrtBraceSizeList.Clear() ;
      ViewModel.HrzBraceSizeList.Clear() ;
      var braceRecords = ClsMasterCsv.Shared.Where( x => x.IsBrace ) ;
      foreach ( var x in braceRecords ) {
        ViewModel.VrtBraceSizeList.Add( x ) ;
        ViewModel.HrzBraceSizeList.Add( x ) ;
      }

      ViewModel.VrtBraceSize = ViewModel.VrtBraceSizeList.FirstOrDefault() ;
      ViewModel.HrzBraceSize = ViewModel.HrzBraceSizeList.FirstOrDefault() ;

      ViewModel.HrzJointSizeList.Clear() ;
      var jointRecords = ClsMasterCsv.Shared.Where( x => x.IsHrzJoint ).ToArray() ;
      var displayJointRecords = jointRecords.Where( x => x is ClsHBeamCsv )
        .Concat( jointRecords.Where( x => x is ClsChannelCsv ) ).Concat( jointRecords.Where( x => x is ClsAngleCsv ) ) ;
      foreach ( var x in displayJointRecords ) {
        ViewModel.HrzJointSizeList.Add( x ) ;
      }

      ViewModel.HrzJointSize = ViewModel.HrzJointSizeList.FirstOrDefault() ;

      GetIniData() ;
    }

    private void button3_Click( object sender, EventArgs e )
    {
      //初期化処理
      MakeRequest( RequestId.End ) ;
      m_ExEvent.Dispose() ;
      m_ExEvent = null ;
      m_Handler = null ;

      this.DialogResult = DialogResult.Cancel ;
      this.Close() ;
    }

    private void button1_Click( object sender, EventArgs e )
    {
      MakeRequest( RequestId.HorizontalTsunagi ) ;
      SetIniData() ;
      DialogResult = DialogResult.OK ;
    }

    private void numericUpDown14_ValueChanged( object sender, EventArgs e )
    {
      if ( ! ( sender is YmsNumericInteger numeric ) ) {
        return ;
      }

      var dstCount = (int) numeric.Value ;
      var srcCount = ViewModel.DanRowSet.Count() ;

      if ( dstCount < srcCount ) {
        for ( int i = 0 ; i < srcCount - dstCount ; i++ ) {
          ViewModel.DanRowSet.RemoveAt( ViewModel.DanRowSet.Count() - 1 ) ;
        }
      }
      else if ( srcCount < dstCount ) {
        for ( int i = 0 ; i < dstCount - srcCount ; i++ ) {
          ViewModel.DanRowSet.Add( new VMType.DansuRow
          {
            Index = ViewModel.DanRowSet.Count() + 1,
            IntervalMm = ViewModel.DanBaseInterval,
            HasBrace = true,
            HasJoint = true,
          } ) ;
        }
      }
    }

    /// <summary>
    /// ダイアログデータをini二セット
    /// </summary>
    public void SetIniData()
    {
      string iniPath = ClsIni.GetIniFilePath( IniPath ) ;
      ClsIni.WritePrivateProfileString( sec, CmbSize.Name, CmbSize.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtPutWayMulti.Name, RbtPutWayMulti.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtPutWaySingle.Name, RbtPutWaySingle.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcSLng.Name, NmcSLng.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcELng.Name, NmcELng.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtAttachWayWelding.Name, RbtAttachWayWelding.Checked.ToString(),
        iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtAttachWayBolt.Name, RbtAttachWayBolt.Checked.ToString(), iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, RbtAttachWayTeiketsu.Name, RbtAttachWayTeiketsu.Checked.ToString(),
        iniPath ) ;
      //ClsIni.WritePrivateProfileString(sec, radioButton1.Name, radioButton1.Checked.ToString(), iniPath);
      //ClsIni.WritePrivateProfileString(sec, radioButton2.Name, radioButton2.Checked.ToString(), iniPath);
      ClsIni.WritePrivateProfileString( sec, NmcDanCount.Name, NmcDanCount.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, NmcDanSpan.Name, NmcDanSpan.Text, iniPath ) ;

      var rowDanSettingList = new List<string>() ;
      foreach ( DataGridViewRow row in DgvDansu.Rows ) {
        var rowSpanValue = row.Cells[ 1 ].Value?.ToString() ;
        var rowHasTsunagiValue = row.Cells[ 2 ].Value?.ToString() ;
        if ( string.IsNullOrEmpty( rowSpanValue ) || string.IsNullOrEmpty( rowHasTsunagiValue ) ) continue ;
        rowDanSettingList.Add( $"{rowSpanValue}:{rowHasTsunagiValue}" ) ;
      }

      var rowDanSetting = string.Join( "|", rowDanSettingList ) ;
      ClsIni.WritePrivateProfileString( sec, DgvDansu.Name, rowDanSetting, iniPath ) ;
    }

    /// <summary>
    /// iniデータをダイアログにセット
    /// </summary>
    public void GetIniData()
    {
      string iniPath = ClsIni.GetIniFilePath( IniPath ) ;
      if ( File.Exists( iniPath ) ) {
        CmbSize.Text = ClsIni.GetIniFile( sec, CmbSize.Name, iniPath ) ;
        RbtPutWayMulti.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtPutWayMulti.Name, iniPath ) ) ;
        RbtPutWaySingle.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtPutWaySingle.Name, iniPath ) ) ;
        //NmcSLng.Text = ClsIni.GetIniFile(sec, NmcSLng.Name, iniPath);
        ViewModel.HrzJointStartMm = int.Parse( ClsIni.GetIniFile( sec, NmcSLng.Name, iniPath ) ) ;
        //NmcELng.Text = ClsIni.GetIniFile(sec, NmcELng.Name, iniPath);
        ViewModel.HrzJointEndMm = int.Parse( ClsIni.GetIniFile( sec, NmcELng.Name, iniPath ) ) ;
        RbtAttachWayWelding.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtAttachWayWelding.Name, iniPath ) ) ;
        RbtAttachWayBolt.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtAttachWayBolt.Name, iniPath ) ) ;
        RbtAttachWayTeiketsu.Checked = bool.Parse( ClsIni.GetIniFile( sec, RbtAttachWayTeiketsu.Name, iniPath ) ) ;
        //radioButton1.Checked = bool.Parse(ClsIni.GetIniFile(sec, radioButton1.Name, iniPath));
        //radioButton2.Checked = bool.Parse(ClsIni.GetIniFile(sec, radioButton2.Name, iniPath));
        //NmcDanCount.Text = ClsIni.GetIniFile(sec, NmcDanCount.Name, iniPath);
        ViewModel.DanCount = int.Parse( ClsIni.GetIniFile( sec, NmcDanCount.Name, iniPath ) ) ;
        //NmcDanSpan.Text = ClsIni.GetIniFile(sec, NmcDanSpan.Name, iniPath);
        ViewModel.DanBaseInterval = int.Parse( ClsIni.GetIniFile( sec, NmcDanSpan.Name, iniPath ) ) ;

        var dgvDansuValue = ClsIni.GetIniFile( sec, DgvDansu.Name, iniPath ) ;

        if ( ! string.IsNullOrEmpty( dgvDansuValue ) && dgvDansuValue != "none" ) {
          var dgvDansuArray = dgvDansuValue.Split( '|' ) ;

          ViewModel.DanRowSet.Clear() ;
          for ( var i = 0 ; i < dgvDansuArray.Count() ; i++ ) {
            var valueArray = dgvDansuArray[ i ].Split( ':' ) ;

            VMType.DansuRow dansuRow = new VMType.DansuRow() ;
            dansuRow.Index = i + 1 ;
            dansuRow.IntervalMm = int.Parse( valueArray[ 0 ] ) ;
            dansuRow.HasJoint = bool.Parse( valueArray[ 1 ] ) ;

            ViewModel.DanRowSet.Add( dansuRow ) ;
          }
        }
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

    //一括配置時の値参照ボタン
    private void button2_Click( object sender, EventArgs e )
    {
      var koudaiData = GantryUtil.GetKoudaiData( doc, CmbKoudaiName.Text ) ;
      if ( koudaiData == null || koudaiData.BraceTsunagiData == null ||
           koudaiData.BraceTsunagiData.HorizontalTsunagi == null || koudaiData.BraceTsunagiData.DanSetting == null ) {
        MessageUtil.Information( "一括配置情報が存在しません。", this.Text ) ;
        return ;
      }
      else {
        if ( MessageUtil.YesNo( "一括配置情報が存在します。展開しますか？", this.Text ) == DialogResult.No )
          return ;
        else {
          int tmpInt ; //TryParse用
          //ダイアログに一括配置情報を展開
          //「ツナギ」タブ
          var hrzTsunagidata = koudaiData.BraceTsunagiData.HorizontalTsunagi ;
          if ( ViewModel.HrzJointSizeList.Count != 0 ) {
            ViewModel.HrzJointSize = ViewModel.HrzJointSizeList.First( x => x.Size == hrzTsunagidata.Size ) ;
            ViewModel.HrzJointSizeIndex = ViewModel.HrzJointSizeList.IndexOf( ViewModel.HrzJointSize ) ;
            CmbSize.SelectedIndex = ViewModel.HrzJointSizeIndex ;
          }

          ViewModel.HrzJointStartMm = hrzTsunagidata.TsukidashiStart ;
          ViewModel.HrzJointEndMm = hrzTsunagidata.TsukidashiEnd ;
          ViewModel.HrzJointIsWelding = hrzTsunagidata.ToritsukiYousetsu ;
          ViewModel.HrzJointIsBolt = hrzTsunagidata.ToritsukiBolt ;
          ViewModel.HrzJointIsMetal = hrzTsunagidata.ToritsukiTeiketsuKanagu ;
          RbtAttachWayWelding.Checked = hrzTsunagidata.ToritsukiYousetsu ;
          RbtAttachWayBolt.Checked = hrzTsunagidata.ToritsukiBolt ;
          RbtAttachWayTeiketsu.Checked = hrzTsunagidata.ToritsukiTeiketsuKanagu ;

          //「段数設定」タブ
          var danSettingData = koudaiData.BraceTsunagiData.DanSetting ;
          ViewModel.DanCount = danSettingData.TsunagiDansu ;
          ViewModel.DanBaseInterval = danSettingData.BaseSpan ;

          ViewModel.DanRowSet.Clear() ;
          foreach ( var danSettingListData in danSettingData.DanSettingDataList ) {
            VMType.DansuRow dansuRow = new VMType.DansuRow() ;
            if ( int.TryParse( danSettingListData.Dan, out tmpInt ) ) dansuRow.Index = tmpInt ;
            dansuRow.IntervalMm = danSettingListData.Span ;
            dansuRow.HasJoint = danSettingListData.TsunagiUmu ;
            //dansuRow.HasBrace = danSettingListData.BraceUmu;

            ViewModel.DanRowSet.Add( dansuRow ) ;
          }
        }
      }
    }
  }
}