using System ;
using System.Collections.Generic ;
using System.ComponentModel ;
using System.Data ;
using System.Drawing ;
using System.Linq ;
using System.Reflection ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using YMS ;

namespace YMS_Schedule
{
  /// <summary>
  /// 積算設定画面
  /// </summary>
  public partial class FormSekisanSetting : Form
  {
    #region 定数

    /// <summary>
    /// Iniファイルのパス
    /// </summary>
    const string IniPath = "ini\\FormSekisanSetting.ini" ;

    /// <summary>
    /// Iniファイルのセクション名
    /// </summary>
    const string sec = "FormSekisanSetting" ;

    #endregion

    #region メンバ変数

    public ClsSekisanSetting m_ClsSekisaiSetting ;

    #endregion

    #region コンストラクタ

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public FormSekisanSetting()
    {
      InitializeComponent() ;
      m_ClsSekisaiSetting = new ClsSekisanSetting() ;

      ////初期値
      //SetDefaultData();

      cmbKiriToritsukeHoho.SelectedIndexChanged += EnableControl ;
      cmbKiriKoteiHoho.SelectedIndexChanged += EnableControl ;
      cmbHorToritsukeHoho.SelectedIndexChanged += EnableControl ;
      cmbHorBoltType.SelectedIndexChanged += EnableControl ;
      cmbVerToritsukeHoho.SelectedIndexChanged += EnableControl ;
      cmbVerKoteiHoho.SelectedIndexChanged += EnableControl ;
      cmbKetaShugetaKetauke.SelectedIndexChanged += EnableControl ;
      cmbKetaShugetaKetaukeSlope.SelectedIndexChanged += EnableControl ;
      cmbKetaKetaukeKetauke.SelectedIndexChanged += EnableControl ;
      cmbKuiKuiKetauke.SelectedIndexChanged += EnableControl ;

      //初期値
      SetDefaultData() ;
    }

    #endregion

    #region イベント処理

    /// <summary>
    /// OKボタン押下処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnOK_Click( object sender, EventArgs e )
    {
      if ( ! CheckInput() ) {
        return ;
      }

      SetData() ;
      SetIniData() ;

      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    /// <summary>
    /// キャンセルボタン押下
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnCancel_Click( object sender, EventArgs e )
    {
      this.DialogResult = DialogResult.Cancel ;
      this.Close() ;
    }

    #endregion

    #region メソッド

    /// <summary>
    /// コントロール制御
    /// </summary>
    /// <returns></returns>
    private void EnableControl( object sender, EventArgs e )
    {
      //切梁受け
      bool bKiriPlate = true ;
      bool bKiriKoteiHoho = true ;
      bool bKiriBolt = true ;
      if ( cmbKiriToritsukeHoho.Text == "ブルマン" ) {
        bKiriPlate = false ;
        bKiriKoteiHoho = false ;
        bKiriBolt = false ;
      }
      else if ( cmbKiriToritsukeHoho.Text == "なし" ) {
        bKiriPlate = false ;
        //bKiriKoteiHoho = false;
        bKiriBolt = false ;
      }
      else if ( cmbKiriKoteiHoho.Text != "ボルト" ) {
        bKiriBolt = false ;
      }

      txtKiriKiriPlateHeight.Enabled = bKiriPlate ;
      txtKiriKiriPlateThickness.Enabled = bKiriPlate ;
      cmbKiriKoteiHoho.Enabled = bKiriKoteiHoho ;
      //txtKiriFlangeSideBoltNum.Enabled = bKiriBolt;
      //txtKiriWebSideBoltNum.Enabled = bKiriBolt;
      //txtKiriToritsukeHojoAndPile.Enabled = bKiriBolt;

      //水平ブレス
      bool bHorBolt = true ;
      if ( cmbHorToritsukeHoho.Text != "ボルト" ) {
        bHorBolt = false ;
      }

      cmbHorBoltType.Enabled = bHorBolt ;
      //txtHorBoltNum.Enabled = bHorBolt;

      //垂直ブレスまたは水平つばぎ
      bool bVerPlate = true ;
      bool bVerKoteiHoho = true ;
      bool bVerBolt = true ;
      if ( cmbVerToritsukeHoho.Text == "ブルマン" ) {
        bVerPlate = false ;
        bVerKoteiHoho = false ;
        bVerBolt = false ;
      }
      else if ( cmbVerToritsukeHoho.Text == "なし" ) {
        bVerPlate = false ;
        //bVerKoteiHoho = false;
        bVerBolt = false ;
      }
      else if ( cmbVerKoteiHoho.Text != "ボルト" ) {
        bVerBolt = false ;
      }

      txtVerHorPlateHeight.Enabled = bVerPlate ;
      txtVerVerPlateThickness.Enabled = bVerPlate ;
      cmbVerKoteiHoho.Enabled = bVerKoteiHoho ;
      txtVerPlateThickness.Enabled = bVerPlate ;
      //txtVerFlangeSideBoltNum.Enabled = bVerBolt;
      //txtVerWebSideBoltNum.Enabled = bVerBolt;
      //txtVerToritsukeHojoAndPile.Enabled = bVerBolt;

      //杭
      if ( cmbKetaShugetaKetauke.Text == "なし" || cmbKetaShugetaKetauke.Text == "ブルマン" ||
           cmbKetaShugetaKetauke.Text == "溶接" ) {
        txtKetaShugetaKetaukeNum.Enabled = false ;
      }
      else {
        txtKetaShugetaKetaukeNum.Enabled = true ;
      }

      if ( cmbKetaShugetaKetaukeSlope.Text == "なし" || cmbKetaShugetaKetaukeSlope.Text == "ブルマン" ||
           cmbKetaShugetaKetaukeSlope.Text == "溶接" ) {
        txtKetaShugetaKetaukeSlopeNum.Enabled = false ;
      }
      else {
        txtKetaShugetaKetaukeSlopeNum.Enabled = true ;
      }

      if ( cmbKetaKetaukeKetauke.Text == "なし" || cmbKetaKetaukeKetauke.Text == "ブルマン" ||
           cmbKetaKetaukeKetauke.Text == "溶接" ) {
        txtKetaKetaukeKetaukeNum.Enabled = false ;
      }
      else {
        txtKetaKetaukeKetaukeNum.Enabled = true ;
      }

      //杭
      if ( cmbKuiKuiKetauke.Text == "なし" || cmbKuiKuiKetauke.Text == "ブルマン" || cmbKuiKuiKetauke.Text == "溶接" ) {
        txtKuiKuiKetaukeNum.Enabled = false ;
      }
      else {
        txtKuiKuiKetaukeNum.Enabled = true ;
      }
    }

    /// <summary>
    /// 入力チェック
    /// </summary>
    /// <returns></returns>
    private bool CheckInput()
    {
      return true ;
    }

    /// <summary>
    /// データクラスにコントロールの情報をセットする
    /// </summary>
    /// <returns></returns>
    private bool SetData()
    {
      ClsSekisanSetting clsSS = new ClsSekisanSetting() ;
      try {
        clsSS.KariHaraOkoshi = RevitUtil.ClsCommonUtils.ChangeStrToDbl( txtKariHaraOkoshi.Text.ToString() ) ;
        clsSS.KariKiribari = RevitUtil.ClsCommonUtils.ChangeStrToDbl( txtKariKiribari.Text.ToString() ) ;
        clsSS.KariCornerHiuchi = RevitUtil.ClsCommonUtils.ChangeStrToDbl( txtKariCornerHiuchi.Text.ToString() ) ;
        clsSS.KariKiribariHiuchi = RevitUtil.ClsCommonUtils.ChangeStrToDbl( txtKariKiribariHiuchi.Text.ToString() ) ;
        clsSS.KariShichu = RevitUtil.ClsCommonUtils.ChangeStrToDbl( txtKariShichu.Text.ToString() ) ;
        clsSS.KariHoujou = RevitUtil.ClsCommonUtils.ChangeStrToDbl( txtKariHoujou.Text.ToString() ) ;

        clsSS.Urakome = chkUrakome.Checked ;

        clsSS.KiriToritsukeHoho = cmbKiriToritsukeHoho.Text.ToString() ;
        clsSS.KiriKiriPlateHeight = RevitUtil.ClsCommonUtils.ChangeStrToDbl( txtKiriKiriPlateHeight.Text.ToString() ) ;
        clsSS.KiriKiriPlateThickness =
          RevitUtil.ClsCommonUtils.ChangeStrToDbl( txtKiriKiriPlateThickness.Text.ToString() ) ;
        clsSS.KiriKoteiHoho = cmbKiriKoteiHoho.Text.ToString() ;
        clsSS.KiriFlangeSiedeBoltNum =
          RevitUtil.ClsCommonUtils.ChangeStrToInt( txtKiriFlangeSideBoltNum.Text.ToString() ) ;
        clsSS.KiriWebSiedeBoltNum = RevitUtil.ClsCommonUtils.ChangeStrToInt( txtKiriWebSideBoltNum.Text.ToString() ) ;
        clsSS.KiriToritsukeHojoAndPile =
          RevitUtil.ClsCommonUtils.ChangeStrToInt( txtKiriToritsukeHojoAndPile.Text.ToString() ) ;

        clsSS.HorToritsukeHoho = cmbHorToritsukeHoho.Text.ToString() ;
        clsSS.HorBoltType = cmbHorBoltType.Text.ToString() ;
        clsSS.HorBoltNum = RevitUtil.ClsCommonUtils.ChangeStrToInt( txtHorBoltNum.Text.ToString() ) ;

        clsSS.VerToritsukeHoho = cmbVerToritsukeHoho.Text.ToString() ;
        clsSS.VerHorPlateHeight = RevitUtil.ClsCommonUtils.ChangeStrToDbl( txtVerHorPlateHeight.Text.ToString() ) ;
        clsSS.VerVerPlateThickness =
          RevitUtil.ClsCommonUtils.ChangeStrToDbl( txtVerVerPlateThickness.Text.ToString() ) ;
        clsSS.VerPlateThickness = RevitUtil.ClsCommonUtils.ChangeStrToDbl( txtVerPlateThickness.Text.ToString() ) ;
        clsSS.VerKoteiHoho = cmbVerKoteiHoho.Text.ToString() ;
        clsSS.VerFlangeSiedeBoltNum =
          RevitUtil.ClsCommonUtils.ChangeStrToInt( txtVerFlangeSideBoltNum.Text.ToString() ) ;
        clsSS.VerWebSiedeBoltNum = RevitUtil.ClsCommonUtils.ChangeStrToInt( txtVerWebSideBoltNum.Text.ToString() ) ;
        clsSS.VerToritsukeHojoAndPile =
          RevitUtil.ClsCommonUtils.ChangeStrToInt( txtVerToritsukeHojoAndPile.Text.ToString() ) ;

        clsSS.KetaShugetaKetauke = cmbKetaShugetaKetauke.Text.ToString() ;
        clsSS.KetaShugetaKetaukeNum =
          RevitUtil.ClsCommonUtils.ChangeStrToInt( txtKetaShugetaKetaukeNum.Text.ToString() ) ;
        clsSS.KetaShugetaKetaukeSlope = cmbKetaShugetaKetaukeSlope.Text.ToString() ;
        clsSS.KetaShugetaKetaukeSlopeNum =
          RevitUtil.ClsCommonUtils.ChangeStrToInt( txtKetaShugetaKetaukeSlopeNum.Text.ToString() ) ;
        clsSS.KetaKetaukeKetauke = cmbKetaKetaukeKetauke.Text.ToString() ;
        clsSS.KetaKetaukeKetaukeNum =
          RevitUtil.ClsCommonUtils.ChangeStrToInt( txtKetaKetaukeKetaukeNum.Text.ToString() ) ;

        clsSS.KuiKuiKetauke = cmbKuiKuiKetauke.Text.ToString() ;
        clsSS.KuiKuiKetaukeNum = RevitUtil.ClsCommonUtils.ChangeStrToInt( txtKuiKuiKetaukeNum.Text.ToString() ) ;

        clsSS.YamdomeBoltSekisan = chkBxYamadomeSekisanBolt.Checked ;
        clsSS.KoudaiBoltSekisan = chkBxKoudaiSekisanBolt.Checked ;

        m_ClsSekisaiSetting = clsSS ;
      }
      catch {
        MessageBox.Show( "コントロールに不正な値が設定されています。" ) ;
      }

      return true ;
    }

    /// <summary>
    /// 初期値
    /// </summary>
    /// <returns></returns>
    private bool SetDefaultData()
    {
      try {
        GetIniData() ;

        var formSekisanSetting = typeof( FormSekisanSetting ) ;
        FieldInfo[] formSekisanSettingFields =
          formSekisanSetting.GetFields( BindingFlags.NonPublic | BindingFlags.Instance ) ;
        foreach ( FieldInfo fieldInfo in formSekisanSettingFields ) {
          if ( fieldInfo == null )
            continue ;
          if ( fieldInfo.Name.Contains( "txtKari" ) ) {
            if ( fieldInfo.FieldType == typeof( TextBox ) ) {
              TextBox txtKari = (TextBox) fieldInfo.GetValue( this ) ;
              if ( txtKari.Text == "none" )
                txtKari.Text = "0.0" ;
            }
          }
        }
        //txtKariHaraOkoshi.Text = "0.0";
        //txtKariKiribari.Text = "0.0";
        //txtKariCornerHiuchi.Text = "0.0";
        //txtKariKiribariHiuchi.Text = "0.0";
        //txtKariShichu.Text = "0.0";
        //txtKariHoujou.Text = "0.0";

        chkUrakome.Checked = true ;
        chkBxYamadomeSekisanBolt.Checked = false ; //#32048
        chkBxKoudaiSekisanBolt.Checked = false ; //#32048

        cmbKiriToritsukeHoho.SelectedIndex = 0 ;
        txtKiriKiriPlateHeight.Text = "0.0" ;
        txtKiriKiriPlateThickness.Text = "150.0" ;
        cmbKiriKoteiHoho.SelectedIndex = 0 ;
        txtKiriFlangeSideBoltNum.Text = "2" ;
        txtKiriWebSideBoltNum.Text = "2" ;
        txtKiriToritsukeHojoAndPile.Text = "2" ;

        cmbHorToritsukeHoho.SelectedIndex = 0 ;
        cmbHorBoltType.SelectedIndex = 0 ;
        txtHorBoltNum.Text = "2" ;

        cmbVerToritsukeHoho.SelectedIndex = 0 ;
        txtVerHorPlateHeight.Text = "0.0" ;
        txtVerVerPlateThickness.Text = "150.0" ;
        txtVerPlateThickness.Text = "150.0" ;
        cmbVerKoteiHoho.SelectedIndex = 0 ;
        txtVerFlangeSideBoltNum.Text = "2" ;
        txtVerWebSideBoltNum.Text = "2" ;
        txtVerToritsukeHojoAndPile.Text = "2" ;

        cmbKetaShugetaKetauke.SelectedIndex = 0 ;
        txtKetaShugetaKetaukeNum.Text = "4" ;
        cmbKetaShugetaKetaukeSlope.SelectedIndex = 0 ;
        txtKetaShugetaKetaukeSlopeNum.Text = "4" ;
        cmbKetaKetaukeKetauke.SelectedIndex = 0 ;
        txtKetaKetaukeKetaukeNum.Text = "2" ;

        cmbKuiKuiKetauke.SelectedIndex = 0 ;
        txtKuiKuiKetaukeNum.Text = "4" ;
      }
      catch {
      }

      return true ;
    }

    /// <summary>
    /// ダイアログデータをiniにセット
    /// </summary>
    private void SetIniData()
    {
      string iniPath = ClsIni.GetIniFilePath( IniPath ) ;

      //仮鋼材→割付部材
      ClsIni.WritePrivateProfileString( sec, txtKariHaraOkoshi.Name, txtKariHaraOkoshi.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, txtKariKiribari.Name, txtKariKiribari.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, txtKariCornerHiuchi.Name, txtKariCornerHiuchi.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, txtKariKiribariHiuchi.Name, txtKariKiribariHiuchi.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, txtKariShichu.Name, txtKariShichu.Text, iniPath ) ;
      ClsIni.WritePrivateProfileString( sec, txtKariHoujou.Name, txtKariHoujou.Text, iniPath ) ;
    }

    /// <summary>
    /// iniデータをダイアログにセット
    /// </summary>
    private void GetIniData()
    {
      string iniPath = ClsIni.GetIniFilePath( IniPath ) ;

      //仮鋼材→割付部材
      txtKariHaraOkoshi.Text = ClsIni.GetIniFile( sec, txtKariHaraOkoshi.Name, iniPath ) ;
      txtKariKiribari.Text = ClsIni.GetIniFile( sec, txtKariKiribari.Name, iniPath ) ;
      txtKariCornerHiuchi.Text = ClsIni.GetIniFile( sec, txtKariCornerHiuchi.Name, iniPath ) ;
      txtKariKiribariHiuchi.Text = ClsIni.GetIniFile( sec, txtKariKiribariHiuchi.Name, iniPath ) ;
      txtKariShichu.Text = ClsIni.GetIniFile( sec, txtKariShichu.Name, iniPath ) ;
      txtKariHoujou.Text = ClsIni.GetIniFile( sec, txtKariHoujou.Name, iniPath ) ;
    }

    #endregion

    private void cmbHorToritsukeHoho_SelectedIndexChanged( object sender, EventArgs e )
    {
    }

    private void cmbHorBoltType_SelectedIndexChanged( object sender, EventArgs e )
    {
    }

    private void checkBox1_CheckedChanged( object sender, EventArgs e )
    {
    }

    private void cmbKiriToritsukeHoho_SelectedIndexChanged( object sender, EventArgs e )
    {
    }

    private void cmbVerToritsukeHoho_SelectedIndexChanged( object sender, EventArgs e )
    {
    }
  }

  /// <summary>
  /// 積算設定クラス
  /// </summary>
  public class ClsSekisanSetting
  {
    /// <summary>
    /// [仮鋼材→割付部材]腹起
    /// </summary>
    public double KariHaraOkoshi { get ; set ; }

    /// <summary>
    /// [仮鋼材→割付部材]切梁
    /// </summary>
    public double KariKiribari { get ; set ; }

    /// <summary>
    /// [仮鋼材→割付部材]隅火打
    /// </summary>
    public double KariCornerHiuchi { get ; set ; }

    /// <summary>
    /// [仮鋼材→割付部材]切梁火打
    /// </summary>
    public double KariKiribariHiuchi { get ; set ; }

    /// <summary>
    /// [仮鋼材→割付部材]支柱
    /// </summary>
    public double KariShichu { get ; set ; }

    /// <summary>
    /// [仮鋼材→割付部材]方丈
    /// </summary>
    public double KariHoujou { get ; set ; }

    /// <summary>
    /// [裏込め材]コーナーピース用を積算するかどうか
    /// </summary>
    public bool Urakome { get ; set ; }

    /// <summary>
    /// [切梁受け]取付方法
    /// </summary>
    public string KiriToritsukeHoho { get ; set ; }

    /// <summary>
    /// [切梁受け]固定方法
    /// </summary>
    public string KiriKoteiHoho { get ; set ; }

    /// <summary>
    /// [切梁受け]切梁用プレート高さ
    /// </summary>
    public double KiriKiriPlateHeight { get ; set ; }

    /// <summary>
    /// [切梁受け]切梁用プレート厚み
    /// </summary>
    public double KiriKiriPlateThickness { get ; set ; }

    /// <summary>
    /// [切梁受け]フランジ側ボルト本数
    /// </summary>
    public int KiriFlangeSiedeBoltNum { get ; set ; }

    /// <summary>
    /// [切梁受け]ウェブ側ボルト本数
    /// </summary>
    public int KiriWebSiedeBoltNum { get ; set ; }

    /// <summary>
    /// [切梁受け]取付補助材と杭
    /// </summary>
    public int KiriToritsukeHojoAndPile { get ; set ; }

    /// <summary>
    /// [水平ブレス]取付方法
    /// </summary>
    public string HorToritsukeHoho { get ; set ; }

    /// <summary>
    /// [水平ブレス]ボルト種類
    /// </summary>
    public string HorBoltType { get ; set ; }

    /// <summary>
    /// [水平ブレス]一か所あたりのボルト本数
    /// </summary>
    public int HorBoltNum { get ; set ; }

    /// <summary>
    /// [垂直ブレスまたは水平つなぎ]取付方法
    /// </summary>
    public string VerToritsukeHoho { get ; set ; }

    /// <summary>
    /// [垂直ブレスまたは水平つなぎ]固定方法
    /// </summary>
    public string VerKoteiHoho { get ; set ; }

    /// <summary>
    /// [垂直ブレスまたは水平つなぎ]水平つなぎ用プレート高さ
    /// </summary>
    public double VerHorPlateHeight { get ; set ; }

    /// <summary>
    /// [垂直ブレスまたは水平つなぎ]垂直ブレス用プレート厚み
    /// </summary>
    public double VerVerPlateThickness { get ; set ; }

    /// <summary>
    /// [垂直ブレスまたは水平つなぎ]プレート厚み
    /// </summary>
    public double VerPlateThickness { get ; set ; }

    /// <summary>
    /// [垂直ブレスまたは水平つなぎ]フランジ側ボルト本数
    /// </summary>
    public int VerFlangeSiedeBoltNum { get ; set ; }

    /// <summary>
    /// [垂直ブレスまたは水平つなぎ]ウェブ側ボルト本数
    /// </summary>
    public int VerWebSiedeBoltNum { get ; set ; }

    /// <summary>
    /// [垂直ブレスまたは水平つなぎ]取付補助材と杭
    /// </summary>
    public int VerToritsukeHojoAndPile { get ; set ; }


    /// <summary>
    /// [桁受]主桁（根太）―桁受（大引）
    /// </summary>
    public string KetaShugetaKetauke { get ; set ; }

    /// <summary>
    /// [桁受]主桁（根太）―桁受（大引）ボルト本数
    /// </summary>
    public int KetaShugetaKetaukeNum { get ; set ; }

    /// <summary>
    /// [桁受]主桁（根太）―桁受（大引）スロープの場合
    /// </summary>
    public string KetaShugetaKetaukeSlope { get ; set ; }

    /// <summary>
    /// [桁受]主桁（根太）―桁受（大引）スロープの場合　本数
    /// </summary>
    public int KetaShugetaKetaukeSlopeNum { get ; set ; }

    /// <summary>
    /// [桁受]桁受（ﾁｬﾝﾈﾙ）―桁受（ﾁｬﾝﾈﾙ）の場合
    /// </summary>
    public string KetaKetaukeKetauke { get ; set ; }

    /// <summary>
    /// [桁受]桁受（ﾁｬﾝﾈﾙ）―桁受（ﾁｬﾝﾈﾙ）の場合　本数
    /// </summary>
    public int KetaKetaukeKetaukeNum { get ; set ; }

    /// <summary>
    /// [杭]主桁（根太）杭―桁受（大引）
    /// </summary>
    public string KuiKuiKetauke { get ; set ; }

    /// <summary>
    /// [杭]主桁（根太）杭―桁受（大引）本数
    /// </summary>
    public int KuiKuiKetaukeNum { get ; set ; }

    /// <summary>
    /// 山留ボルト積算を行うか否か
    /// </summary>
    public bool YamdomeBoltSekisan { get ; set ; }

    /// <summary>
    /// 構台ボルト積算を行うか否か
    /// </summary>
    public bool KoudaiBoltSekisan { get ; set ; }
  }
}