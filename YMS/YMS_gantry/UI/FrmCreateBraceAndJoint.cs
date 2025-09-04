using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Windows.Forms ;
using YMS_gantry.Command ;
using YMS_gantry.Master ;
using VMType = YMS_gantry.UI.FrmCreateBraceAndJointViewModel ;

namespace YMS_gantry.UI
{
  public partial class FrmCreateBraceAndJoint : Form
  {
    public VMType ViewModel { get ; } = new VMType() ;

    public FrmCreateBraceAndJoint()
    {
      InitializeComponent() ;
      InitializeBindings() ;
    }

    private void InitializeBindings()
    {
      var vm = ViewModel ;

      CmbKoudaiName.DataBindings.Add( nameof( ComboBox.DataSource ), vm, nameof( VMType.KodaiSet ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      CmbKoudaiName.DisplayMember = nameof( AllKoudaiFlatFrmData.KoudaiName ) ;
      CmbKoudaiName.DataBindings.Add( nameof( ComboBox.SelectedValue ), vm, nameof( VMType.SelectedKodai ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;

      ChkHasVerBraceKyoujiku.DataBindings.Add( nameof( CheckBox.Checked ), vm, nameof( VMType.VrtBraceHasK ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      ChkHasVerBraceHukuin.DataBindings.Add( nameof( CheckBox.Checked ), vm, nameof( VMType.VrtBraceHasH ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;

      CmbVerBraceSize.DataBindings.Add( nameof( ComboBox.DataSource ), vm, nameof( VMType.VrtBraceSizeList ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      CmbVerBraceSize.DisplayMember = nameof( ClsMasterCsv.Size ) ;
      CmbVerBraceSize.DataBindings.Add( nameof( ComboBox.SelectedValue ), vm, nameof( VMType.VrtBraceSize ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      // SelectedValueへの値変更では反映できないのでSelectedIndexを追加
      CmbVerBraceSize.DataBindings.Add( nameof( ComboBox.SelectedIndex ), vm, nameof( VMType.VrtBraceSizeIndex ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;

      radioButton8.DataBindings.Add( nameof( RadioButton.Checked ), vm, nameof( VMType.VrtBracePriorH ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      radioButton9.DataBindings.Add( nameof( RadioButton.Checked ), vm, nameof( VMType.VrtBracePriorK ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;

      numericUpDown2.DataBindings.Add( nameof( YmsNumericInteger.DoubleValue ), vm, nameof( VMType.VrtBrace1UpperXMm ),
        true, DataSourceUpdateMode.OnPropertyChanged ) ;
      numericUpDown4.DataBindings.Add( nameof( YmsNumericInteger.DoubleValue ), vm, nameof( VMType.VrtBrace1UpperYMm ),
        true, DataSourceUpdateMode.OnPropertyChanged ) ;
      numericUpDown6.DataBindings.Add( nameof( YmsNumericInteger.DoubleValue ), vm, nameof( VMType.VrtBrace1LowerXMm ),
        true, DataSourceUpdateMode.OnPropertyChanged ) ;
      numericUpDown5.DataBindings.Add( nameof( YmsNumericInteger.DoubleValue ), vm, nameof( VMType.VrtBrace1LowerYMm ),
        true, DataSourceUpdateMode.OnPropertyChanged ) ;

      numericUpDown12.DataBindings.Add( nameof( YmsNumericInteger.DoubleValue ), vm, nameof( VMType.VrtBrace2UpperXMm ),
        true, DataSourceUpdateMode.OnPropertyChanged ) ;
      numericUpDown11.DataBindings.Add( nameof( YmsNumericInteger.DoubleValue ), vm, nameof( VMType.VrtBrace2UpperYMm ),
        true, DataSourceUpdateMode.OnPropertyChanged ) ;
      numericUpDown10.DataBindings.Add( nameof( YmsNumericInteger.DoubleValue ), vm, nameof( VMType.VrtBrace2LowerXMm ),
        true, DataSourceUpdateMode.OnPropertyChanged ) ;
      numericUpDown9.DataBindings.Add( nameof( YmsNumericInteger.DoubleValue ), vm, nameof( VMType.VrtBrace2LowerYMm ),
        true, DataSourceUpdateMode.OnPropertyChanged ) ;

      //checkBox8.DataBindings.Add(nameof(CheckBox.Checked), vm, nameof(VMType.VrtBraceHasLowest), true, DataSourceUpdateMode.OnPropertyChanged);

      checkBox3.DataBindings.Add( nameof( CheckBox.Checked ), vm, nameof( VMType.VrtBraceHasRound ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      comboBox4.DataBindings.Add( nameof( ComboBox.Enabled ), vm, nameof( VMType.VrtBraceHasRound ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      comboBox4.DataBindings.Add( nameof( YMS_gantry.UI.YmsComboInteger._Number ), vm, nameof( VMType.VrtBraceRoundMm ),
        true, DataSourceUpdateMode.OnPropertyChanged ) ;

      RbtVerBraceWelding.DataBindings.Add( nameof( RadioButton.Checked ), vm, nameof( VMType.VrtBraceIsWelding ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      RbtVerBraceBolt.DataBindings.Add( nameof( RadioButton.Checked ), vm, nameof( VMType.VrtBraceIsBolt ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      RbtVerBraceSup.DataBindings.Add( nameof( RadioButton.Checked ), vm, nameof( VMType.VrtBraceIsMetal ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;

      radioButton12.DataBindings.Add( nameof( RadioButton.Checked ), vm, nameof( VMType.HrzBraceHasPlacing ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      radioButton13.DataBindings.Add( nameof( RadioButton.Checked ), vm, nameof( VMType.HrzBraceHasNotPlacing ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;

      comboBox2.DataBindings.Add( nameof( ComboBox.DataSource ), vm, nameof( VMType.HrzBraceSizeList ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      comboBox2.DisplayMember = nameof( ClsAngleCsv.Size ) ;
      comboBox2.DataBindings.Add( nameof( ComboBox.SelectedValue ), vm, nameof( VMType.HrzBraceSize ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      // SelectedValueへの値変更では反映できないのでSelectedIndexを追加
      comboBox2.DataBindings.Add( nameof( ComboBox.SelectedIndex ), vm, nameof( VMType.HrzBraceSizeIndex ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;

      radioButton10.DataBindings.Add( nameof( RadioButton.Checked ), vm, nameof( VMType.HrzBraceIsOhbikiInner ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      radioButton11.DataBindings.Add( nameof( RadioButton.Checked ), vm, nameof( VMType.HrzBraceIsLowerOhbiki ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;

      checkBox13.DataBindings.Add( nameof( CheckBox.Checked ), vm, nameof( VMType.HrzBraceHasTopPlate ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;

      numericUpDown16.DataBindings.Add( nameof( YmsNumericInteger.DoubleValue ), vm, nameof( VMType.HrzBraceXMm ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      numericUpDown15.DataBindings.Add( nameof( YmsNumericInteger.DoubleValue ), vm, nameof( VMType.HrzBraceYMm ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;

      RbtHorBraceWelding.DataBindings.Add( nameof( RadioButton.Checked ), vm, nameof( VMType.HrzBraceIsWelding ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      RbtHorBraceBolt.DataBindings.Add( nameof( RadioButton.Checked ), vm, nameof( VMType.HrzBraceIsBolt ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      RbtHolBraceSup.DataBindings.Add( nameof( RadioButton.Checked ), vm, nameof( VMType.HrzBraceIsMetal ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;

      checkBox2.DataBindings.Add( nameof( CheckBox.Checked ), vm, nameof( VMType.HrzJointHasK ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      checkBox1.DataBindings.Add( nameof( CheckBox.Checked ), vm, nameof( VMType.HrzJointHasH ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;

      comboBox5.DataBindings.Add( nameof( ComboBox.DataSource ), vm, nameof( VMType.HrzJointSizeList ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      comboBox5.DisplayMember = nameof( ClsChannelCsv.Size ) ;
      comboBox5.DataBindings.Add( nameof( ComboBox.SelectedValue ), vm, nameof( VMType.HrzJointSize ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      // SelectedValueへの値変更では反映できないのでSelectedIndexを追加
      comboBox5.DataBindings.Add( nameof( ComboBox.SelectedIndex ), vm, nameof( VMType.HrzJointSizeIndex ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;

      radioButton7.DataBindings.Add( nameof( RadioButton.Checked ), vm, nameof( VMType.HrzJointUpperIsH ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      radioButton4.DataBindings.Add( nameof( RadioButton.Checked ), vm, nameof( VMType.HrzJointLowerIsH ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;

      numericUpDown7.DataBindings.Add( nameof( YmsNumericInteger.DoubleValue ), vm, nameof( VMType.HrzJointStartMm ),
        true, DataSourceUpdateMode.OnPropertyChanged ) ;
      numericUpDown8.DataBindings.Add( nameof( YmsNumericInteger.DoubleValue ), vm, nameof( VMType.HrzJointEndMm ),
        true, DataSourceUpdateMode.OnPropertyChanged ) ;

      RbtHorJointWelding.DataBindings.Add( nameof( RadioButton.Checked ), vm, nameof( VMType.HrzJointIsWelding ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      RbtHorJointBolt.DataBindings.Add( nameof( RadioButton.Checked ), vm, nameof( VMType.HrzJointIsBolt ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      RbtHorJointSup.DataBindings.Add( nameof( RadioButton.Checked ), vm, nameof( VMType.HrzJointIsMetal ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;

      NmcHolJointDan.DataBindings.Add( nameof( YmsNumericInteger.DoubleValue ), vm, nameof( VMType.DanCount ), true,
        DataSourceUpdateMode.OnPropertyChanged ) ;
      NmcHolJointDist.DataBindings.Add( nameof( YmsNumericInteger.DoubleValue ), vm, nameof( VMType.DanBaseInterval ),
        true, DataSourceUpdateMode.OnPropertyChanged ) ;

      ColDansuNo.DataPropertyName = nameof( VMType.DansuRow.Index ) ;
      ColDansuSpan.DataPropertyName = nameof( VMType.DansuRow.IntervalMm ) ;
      chkSuiseitsunagiUmu.DataPropertyName = nameof( VMType.DansuRow.HasJoint ) ;
      chkSuichokuBraceUmu.DataPropertyName = nameof( VMType.DansuRow.HasBrace ) ;

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

      LoadExtensionData() ;
      LoadCalcFileData() ;

      //var radioButtons = ChildrenRecursive(this).Select(x => x as RadioButton).Where(x => x != null);
      //foreach (var radioButton in radioButtons)
      //{
      //    radioButton.Paint += (s, a) =>
      //    {
      //        if (s is RadioButton aa)
      //        {

      //        }
      //    };

      //}
    }

    private void button3_Click( object sender, EventArgs e )
    {
      CmdAllPutBraceTsunagi.DeleteAll( ViewModel ) ;
    }

    private void button1_Click( object sender, EventArgs e )
    {
      DialogResult = DialogResult.OK ;
    }

    private void button2_Click( object sender, EventArgs e )
    {
      DialogResult = DialogResult.Cancel ;
    }

    private void numericUpDown1_ValueChanged( object sender, EventArgs e )
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

    private static List<Control> ChildrenRecursive( Control parentControl )
    {
      var list = new List<Control>() ;
      foreach ( Control ctrl in parentControl.Controls ) {
        list.Add( ctrl ) ;
        list.AddRange( ChildrenRecursive( ctrl ) ) ;
      }

      return list ;
    }

    private void numericUpDown3_Validated( object sender, EventArgs e )
    {
      var danRows = ViewModel.DanRowSet.GetEnumerator() ;
      while ( danRows.MoveNext() ) {
        danRows.Current.IntervalMm = ViewModel.DanBaseInterval ;
      }

      this.DgvDansu.Refresh() ;
    }

    private void comboBox1_SelectedIndexChanged( object sender, EventArgs e )
    {
      LoadExtensionData() ;
      LoadCalcFileData() ;
    }

    /// <summary>
    /// 図面に登録した拡張データから初期値読み込み
    /// </summary>
    private void LoadExtensionData()
    {
      //MEMO
      //ViewModelのプロパティに代入する方法では、ユーザーによる構台の選択変更時、ダイアログのラジオボタンのチェックが反映されない（ViewModelのプロパティ自体には反映される）
      //ラジオボタンのCheckedプロパティに直接代入する方法では、ダイアログ起動時、ダイアログのラジオボタンのチェックが反映されない（すべて最左チェックとなる。また、ViewModelのプロパティには反映されなかった）
      //また、ユーザーによる構台の選択変更時も現在開いているタブのラジオボタンしか更新されない。
      //以下、解決の経緯
      //ViewModelのプロパティに代入とラジオボタンのCheckプロパティに代入を両方行うことで解決した。

      if ( ViewModel.SelectedKodai == null ) return ;
      var data = GantryUtil.GetKoudaiData( ViewModel.RevitDoc, ViewModel.SelectedKodai.KoudaiName ) ;
      if ( data.BraceTsunagiData == null ) return ;
      int tmpInt ; //TryParse用

      //段数設定
      ViewModel.DanCount = data.BraceTsunagiData.DanSetting.TsunagiDansu ;
      ViewModel.DanBaseInterval = data.BraceTsunagiData.DanSetting.BaseSpan ;

      ViewModel.DanRowSet.Clear() ;
      foreach ( var danSettingListData in data.BraceTsunagiData.DanSetting.DanSettingDataList ) {
        VMType.DansuRow dansuRow = new VMType.DansuRow() ;
        if ( int.TryParse( danSettingListData.Dan, out tmpInt ) ) dansuRow.Index = tmpInt ;
        dansuRow.IntervalMm = danSettingListData.Span ;
        dansuRow.HasJoint = danSettingListData.TsunagiUmu ;
        dansuRow.HasBrace = danSettingListData.BraceUmu ;

        ViewModel.DanRowSet.Add( dansuRow ) ;
      }

      //垂直ブレース
      ViewModel.VrtBraceHasK = data.BraceTsunagiData.VerticalBrace.KyoujikuUmu ;
      ViewModel.VrtBraceHasH = data.BraceTsunagiData.VerticalBrace.FukuinUmu ;
      if ( ViewModel.VrtBraceSizeList.Count != 0 ) {
        ViewModel.VrtBraceSize =
          ViewModel.VrtBraceSizeList.First( x => x.Size == data.BraceTsunagiData.VerticalBrace.Size ) ;
        ViewModel.VrtBraceSizeIndex = ViewModel.VrtBraceSizeList.IndexOf( ViewModel.VrtBraceSize ) ;
      }

      ViewModel.VrtBracePriorH = data.BraceTsunagiData.VerticalBrace.KachiFukuin ;
      ViewModel.VrtBracePriorK = data.BraceTsunagiData.VerticalBrace.KachiKyoujiku ;
      radioButton8.Checked = data.BraceTsunagiData.VerticalBrace.KachiFukuin ;
      radioButton9.Checked = data.BraceTsunagiData.VerticalBrace.KachiKyoujiku ;

      ViewModel.VrtBrace1UpperXMm = data.BraceTsunagiData.VerticalBrace.FirstHanareTopX ;
      ViewModel.VrtBrace1UpperYMm = data.BraceTsunagiData.VerticalBrace.FirstHanareTopY ;
      ViewModel.VrtBrace1LowerXMm = data.BraceTsunagiData.VerticalBrace.FirstHanareBottomX ;
      ViewModel.VrtBrace1LowerYMm = data.BraceTsunagiData.VerticalBrace.FirstHanareBottomY ;

      ViewModel.VrtBrace2UpperXMm = data.BraceTsunagiData.VerticalBrace.SecondLaterHanareTopX ;
      ViewModel.VrtBrace2UpperYMm = data.BraceTsunagiData.VerticalBrace.SecondLaterHanareTopY ;
      ViewModel.VrtBrace2LowerXMm = data.BraceTsunagiData.VerticalBrace.SecondLaterHanareBottomX ;
      ViewModel.VrtBrace2LowerYMm = data.BraceTsunagiData.VerticalBrace.SecondLaterHanareBottomY ;

      ViewModel.VrtBraceHasRound = data.BraceTsunagiData.VerticalBrace.RoundON ;
      if ( int.TryParse( data.BraceTsunagiData.VerticalBrace.RoundLength, out tmpInt ) )
        ViewModel.VrtBraceRoundMm = tmpInt ;

      ViewModel.VrtBraceIsWelding = data.BraceTsunagiData.VerticalBrace.ToritsukiYousetsu ;
      ViewModel.VrtBraceIsBolt = data.BraceTsunagiData.VerticalBrace.ToritsukiBolt ;
      ViewModel.VrtBraceIsMetal = data.BraceTsunagiData.VerticalBrace.ToritsukiTeiketsuKanagu ;
      RbtVerBraceWelding.Checked = data.BraceTsunagiData.VerticalBrace.ToritsukiYousetsu ;
      RbtVerBraceBolt.Checked = data.BraceTsunagiData.VerticalBrace.ToritsukiBolt ;
      RbtVerBraceSup.Checked = data.BraceTsunagiData.VerticalBrace.ToritsukiTeiketsuKanagu ;

      //水平ブレース
      ViewModel.HrzBraceHasPlacing = data.BraceTsunagiData.HorizontalBrace.HaichiOn ;
      ViewModel.HrzBraceHasNotPlacing = data.BraceTsunagiData.HorizontalBrace.HaichiOFF ;
      radioButton12.Checked = data.BraceTsunagiData.HorizontalBrace.HaichiOn ;
      radioButton13.Checked = data.BraceTsunagiData.HorizontalBrace.HaichiOFF ;
      if ( ViewModel.HrzBraceSizeList.Count != 0 ) {
        ViewModel.HrzBraceSize =
          ViewModel.HrzBraceSizeList.First( x => x.Size == data.BraceTsunagiData.HorizontalBrace.Size ) ;
        ViewModel.HrzBraceSizeIndex = ViewModel.HrzBraceSizeList.IndexOf( ViewModel.HrzBraceSize ) ;
      }

      ViewModel.HrzBraceIsOhbikiInner = data.BraceTsunagiData.HorizontalBrace.PositionTopFlaBottomAndBottomFlaTop ;
      ViewModel.HrzBraceIsLowerOhbiki = data.BraceTsunagiData.HorizontalBrace.PositionBottomFlaTopAndBottomFlaBottom ;
      radioButton10.Checked = data.BraceTsunagiData.HorizontalBrace.PositionTopFlaBottomAndBottomFlaTop ;
      radioButton11.Checked = data.BraceTsunagiData.HorizontalBrace.PositionBottomFlaTopAndBottomFlaBottom ;

      ViewModel.HrzBraceHasTopPlate = data.BraceTsunagiData.HorizontalBrace.PutTopPL ;

      ViewModel.HrzBraceXMm = data.BraceTsunagiData.HorizontalBrace.PutBanRangeX ;
      ViewModel.HrzBraceYMm = data.BraceTsunagiData.HorizontalBrace.PutBanRangeY ;

      ViewModel.HrzBraceIsWelding = data.BraceTsunagiData.HorizontalBrace.ToritsukiYousetsu ;
      ViewModel.HrzBraceIsBolt = data.BraceTsunagiData.HorizontalBrace.ToritsukiBolt ;
      ViewModel.HrzBraceIsMetal = data.BraceTsunagiData.HorizontalBrace.ToritsukiTeiketsuKanagu ;
      RbtHorBraceWelding.Checked = data.BraceTsunagiData.HorizontalBrace.ToritsukiYousetsu ;
      RbtHorBraceBolt.Checked = data.BraceTsunagiData.HorizontalBrace.ToritsukiBolt ;
      RbtHolBraceSup.Checked = data.BraceTsunagiData.HorizontalBrace.ToritsukiTeiketsuKanagu ;

      //水平ツナギ
      ViewModel.HrzJointHasK = data.BraceTsunagiData.HorizontalTsunagi.KyoujikuUmu ;
      ViewModel.HrzJointHasH = data.BraceTsunagiData.HorizontalTsunagi.FukuinUmu ;
      if ( ViewModel.HrzJointSizeList.Count != 0 ) {
        ViewModel.HrzJointSize =
          ViewModel.HrzJointSizeList.First( x => x.Size == data.BraceTsunagiData.HorizontalTsunagi.Size ) ;
        ViewModel.HrzJointSizeIndex = ViewModel.HrzJointSizeList.IndexOf( ViewModel.HrzJointSize ) ;
      }

      ViewModel.HrzJointUpperIsH = data.BraceTsunagiData.HorizontalTsunagi.PositionFukuinTopAndKyoujikuBottom ;
      ViewModel.HrzJointLowerIsH = data.BraceTsunagiData.HorizontalTsunagi.PositionFukuinBottomAndKyoujikuTop ;
      radioButton7.Checked = data.BraceTsunagiData.HorizontalTsunagi.PositionFukuinTopAndKyoujikuBottom ;
      radioButton4.Checked = data.BraceTsunagiData.HorizontalTsunagi.PositionFukuinBottomAndKyoujikuTop ;

      ViewModel.HrzJointStartMm = data.BraceTsunagiData.HorizontalTsunagi.TsukidashiStart ;
      ViewModel.HrzJointEndMm = data.BraceTsunagiData.HorizontalTsunagi.TsukidashiEnd ;

      ViewModel.HrzJointIsWelding = data.BraceTsunagiData.HorizontalTsunagi.ToritsukiYousetsu ;
      ViewModel.HrzJointIsBolt = data.BraceTsunagiData.HorizontalTsunagi.ToritsukiBolt ;
      ViewModel.HrzJointIsMetal = data.BraceTsunagiData.HorizontalTsunagi.ToritsukiTeiketsuKanagu ;
      RbtHorJointWelding.Checked = data.BraceTsunagiData.HorizontalTsunagi.ToritsukiYousetsu ;
      RbtHorJointBolt.Checked = data.BraceTsunagiData.HorizontalTsunagi.ToritsukiBolt ;
      RbtHorJointSup.Checked = data.BraceTsunagiData.HorizontalTsunagi.ToritsukiTeiketsuKanagu ;

      this.Update() ;
    }

    private void LoadCalcFileData()
    {
      if ( ViewModel.SelectedKodai == null ) return ;

      var isCalcData = false ;
      var data = GantryUtil.GetKoudaiData( ViewModel.RevitDoc, ViewModel.SelectedKodai.KoudaiName ) ;
      if ( data.CalcFileData != null ) {
        var dlgResult = MessageUtil.YesNo( "計算書取込データが存在します。取込データを展開しますか？", "ブレース・ツナギ材配置", this ) ;
        if ( dlgResult == DialogResult.Yes ) {
          isCalcData = true ;
        }
      }

      // 垂直ブレース 橋軸側有無
      if ( isCalcData && ! string.IsNullOrEmpty( data.CalcFileData.VBrace.LGExist ) ) {
        ViewModel.VrtBraceHasK = data.CalcFileData.VBrace.LGExist == "ON" ? true : false ;
      }

      // 垂直ブレース 幅員側有無
      if ( isCalcData && ! string.IsNullOrEmpty( data.CalcFileData.VBrace.TRExist ) ) {
        ViewModel.VrtBraceHasH = data.CalcFileData.VBrace.TRExist == "ON" ? true : false ;
      }

      // 垂直ブレース サイズ
      if ( isCalcData && ! string.IsNullOrEmpty( data.CalcFileData.VBrace.Size ) ) {
        ViewModel.VrtBraceSize =
          ViewModel.VrtBraceSizeList.FirstOrDefault( x => x.Size == data.CalcFileData.VBrace.Size ) ;
        ViewModel.VrtBraceSizeIndex = ViewModel.VrtBraceSizeList.IndexOf( ViewModel.VrtBraceSize ) ;
      }

      // 水平ブレース 配置有無
      if ( isCalcData && ! string.IsNullOrEmpty( data.CalcFileData.HBrace.Exist ) ) {
        ViewModel.HrzBraceHasPlacing = data.CalcFileData.HBrace.Exist == "ON" ? true : false ;
        ViewModel.HrzBraceHasNotPlacing = data.CalcFileData.HBrace.Exist == "OFF" ? true : false ;
      }

      // 水平ブレース サイズ
      if ( isCalcData && ! string.IsNullOrEmpty( data.CalcFileData.HBrace.Size ) ) {
        ViewModel.HrzBraceSize =
          ViewModel.HrzBraceSizeList.FirstOrDefault( x => x.Size == data.CalcFileData.HBrace.Size ) ;
        ViewModel.HrzBraceSizeIndex = ViewModel.HrzBraceSizeList.IndexOf( ViewModel.HrzBraceSize ) ;
      }

      // 水平ツナギ 橋軸側有無
      if ( isCalcData && ! string.IsNullOrEmpty( data.CalcFileData.JointMember.LGExist ) ) {
        ViewModel.HrzJointHasK = data.CalcFileData.JointMember.LGExist == "ON" ? true : false ;
      }

      // 水平ツナギ 幅員側有無
      if ( isCalcData && ! string.IsNullOrEmpty( data.CalcFileData.JointMember.TRExist ) ) {
        ViewModel.HrzJointHasH = data.CalcFileData.JointMember.TRExist == "ON" ? true : false ;
      }

      // 水平ツナギ サイズ
      if ( isCalcData && ! string.IsNullOrEmpty( data.CalcFileData.JointMember.Size ) ) {
        ViewModel.HrzJointSize =
          ViewModel.HrzJointSizeList.FirstOrDefault( x => data.CalcFileData.JointMember.Size.Contains( x.Size ) ) ;
        ViewModel.HrzJointSizeIndex = ViewModel.HrzJointSizeList.IndexOf( ViewModel.HrzJointSize ) ;
      }

      // 段数設定 水平ツナギ段数
      if ( isCalcData && data.CalcFileData.JointMember.Num != null ) {
        ViewModel.DanCount = data.CalcFileData.JointMember.Num.Value ;
      }

      // 段数設定 段数一覧
      if ( isCalcData && data.CalcFileData.JointMember.JointMemberPitch.Pitches != null ) {
        foreach ( var row in ViewModel.DanRowSet ) {
          var pitch = data.CalcFileData.JointMember.JointMemberPitch.Pitches.FirstOrDefault( x => x.No == row.Index ) ;
          if ( pitch == null ) continue ;

          row.Index = pitch.No ;
          row.IntervalMm = pitch.Value ;
          row.HasJoint = pitch.JMExist == "ON" ? true : false ;
          row.HasBrace = pitch.EVBExist == "ON" ? true : false ;
        }
      }
    }
  }
}