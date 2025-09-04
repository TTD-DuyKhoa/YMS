using Autodesk.Revit.DB ;
using Autodesk.Revit.UI ;
using RevitUtil ;
using System ;
using System.Collections.Generic ;
using System.ComponentModel ;
using System.Data ;
using System.Drawing ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;
using YMS.Command ;
using YMS.Master ;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window ;
using static YMS.DLG.DlgWaritsuke ;
using Point = System.Drawing.Point ;

namespace YMS.DLG
{
  public partial class DlgWaritsuke : System.Windows.Forms.Form
  {
    public enum WaritukeCommand : int
    {
      CreateSyuzai, // 主材配置
      CreateMegaBeam, // メガビーム配置
      CreateHojoPiece, // 補助ピース配置
      CreateJack1, // ジャッキ1配置
      CreateJack2, // ジャッキ2配置
      CreateKirikaePieceS, // 切替ピース(始点)配置
      CreateKirikaePieceE, // 切替ピース(終点)配置
      SwitchNormal, // 通常モード
      SwitchSMH, // 高強度モード
      SwitchTwinBeam, // ツインビームモード
      CommandUndo, // 戻る
      CommandEnd, // 終了
    }

    public enum WaritsukeMode
    {
      Normal,
      SMH,
      TwinBeam
    }

    #region メンバ変数

    private Document m_doc { get ; set ; }

    private FamilyInstance m_InstBase { get ; set ; }

    public WaritsukeMode m_WaritsukeMode { get ; set ; }

    public WaritukeCommand m_Command { get ; set ; }

    public double m_Length { get ; set ; }

    public string m_Size { get ; set ; }

    public bool m_UseJackCover { get ; set ; }

    public bool m_isTop { get ; set ; }

    public Point m_LastDialogLocation = Point.Empty ;

    #endregion

    public DlgWaritsuke( Document doc, FamilyInstance instBase, WaritsukeMode mode, bool isTop, Point point )
    {
      InitializeComponent() ;
      m_doc = doc ;
      m_InstBase = instBase ;
      m_WaritsukeMode = mode ;
      m_isTop = isTop ;
      m_LastDialogLocation = point ;
      InitControl() ;
    }

    public void InitControl()
    {
      string steelSize = string.Empty ;
      string yokoCount = string.Empty ;

      string baseName = m_InstBase.Name.Replace( "ベース", "" ) ;
      switch ( baseName ) {
        case "腹起" :
          steelSize = ClsRevitUtil.GetInstMojiParameter( m_doc, m_InstBase.Id, "鋼材サイズ" ) ;
          yokoCount = ClsRevitUtil.GetInstMojiParameter( m_doc, m_InstBase.Id, "横本数" ) ;

          grpHojoPiece.Enabled = true ;
          chkUseJackCover.Enabled = false ;
          btnSetJack1.Enabled = false ;
          btnSetJack2.Enabled = false ;
          btnSetKirikaePieceS.Enabled = false ;
          btnSetKirikaePieceE.Enabled = false ;

          rbnNormal.Enabled = true ;

          if ( steelSize == "35HA" || steelSize == "40HA" ) {
            rbnSMH.Enabled = true ;
          }
          else if ( steelSize == "35SMH" || steelSize == "40SMH" ) {
            rbnSMH.Enabled = true ;
          }
          else if ( steelSize == "80SMH" ) {
            rbnSMH.Enabled = true ;
          }
          else {
            rbnSMH.Enabled = false ;
          }

          if ( steelSize == "40HA" && yokoCount == "ダブル" ) {
            btnMegaBeam.Enabled = true ;
          }
          else if ( steelSize == "40SMH" && yokoCount == "ダブル" ) {
            btnMegaBeam.Enabled = true ;
          }
          else if ( steelSize == "80SMH" ) {
            btnMegaBeam.Enabled = true ;
          }
          else {
            btnMegaBeam.Enabled = false ;
          }

          rbnTwinBeam.Enabled = false ;

          break ;
        case "切梁" :
        case "斜梁" :
          steelSize = ClsRevitUtil.GetInstMojiParameter( m_doc, m_InstBase.Id, "鋼材サイズ" ) ;

          grpHojoPiece.Enabled = true ;
          chkUseJackCover.Enabled = true ;
          btnSetJack1.Enabled = true ;
          btnSetJack2.Enabled = true ;
          btnSetKirikaePieceS.Enabled = true ;
          btnSetKirikaePieceE.Enabled = true ;

          rbnNormal.Enabled = true ;

          if ( steelSize == "35HA" || steelSize == "40HA" ) {
            rbnSMH.Enabled = true ;
          }
          else if ( steelSize == "35SMH" || steelSize == "40SMH" ) {
            rbnSMH.Enabled = true ;
          }
          else if ( steelSize == "60SMH" ) {
            rbnSMH.Enabled = true ;
          }
          else {
            rbnSMH.Enabled = false ;
          }

          btnMegaBeam.Enabled = false ;

          if ( steelSize == "40HA" || steelSize == "50HA" ) {
            rbnTwinBeam.Enabled = true ;
          }
          else if ( steelSize == "40SMH" ) {
            rbnTwinBeam.Enabled = true ;
          }
          else if ( steelSize == "60SMH" ) {
            rbnTwinBeam.Enabled = true ;
          }
          else {
            rbnTwinBeam.Enabled = false ;
          }

          break ;
        case "斜梁繋ぎ材" :
        case "斜梁受け材" :
        case "斜梁火打" :
          steelSize = ClsRevitUtil.GetInstMojiParameter( m_doc, m_InstBase.Id, "鋼材サイズ" ) ;

          grpHojoPiece.Enabled = true ;
          chkUseJackCover.Enabled = false ;
          btnSetJack1.Enabled = false ;
          btnSetJack2.Enabled = false ;
          btnSetKirikaePieceS.Enabled = false ;
          btnSetKirikaePieceE.Enabled = false ;

          rbnNormal.Enabled = true ;

          rbnSMH.Enabled = false ;
          btnMegaBeam.Enabled = false ;
          rbnTwinBeam.Enabled = false ;
          break ;
        case "隅火打" :
          steelSize = ClsRevitUtil.GetInstMojiParameter( m_doc, m_InstBase.Id, "鋼材サイズ" ) ;

          grpHojoPiece.Enabled = true ;
          chkUseJackCover.Enabled = false ;
          btnSetJack1.Enabled = false ;
          btnSetJack2.Enabled = false ;
          btnSetKirikaePieceS.Enabled = true ;
          btnSetKirikaePieceE.Enabled = true ;

          rbnNormal.Enabled = true ;

          if ( steelSize == "35HA" || steelSize == "40HA" ) {
            rbnSMH.Enabled = true ;
          }
          else if ( steelSize == "35SMH" || steelSize == "40SMH" ) {
            rbnSMH.Enabled = true ;
          }
          else if ( steelSize == "60SMH" ) {
            rbnSMH.Enabled = true ;
          }
          else {
            rbnSMH.Enabled = false ;
          }

          btnMegaBeam.Enabled = false ;

          if ( steelSize == "40HA" || steelSize == "50HA" ) {
            rbnTwinBeam.Enabled = true ;
          }
          else if ( steelSize == "40SMH" ) {
            rbnTwinBeam.Enabled = true ;
          }
          else if ( steelSize == "60SMH" ) {
            rbnTwinBeam.Enabled = true ;
          }
          else {
            rbnTwinBeam.Enabled = false ;
          }

          break ;
        case "切梁火打" :
          steelSize = ClsRevitUtil.GetInstMojiParameter( m_doc, m_InstBase.Id, "鋼材サイズ(シングル)" ) ;
          if ( steelSize == "" ) {
            if ( m_isTop ) {
              steelSize = ClsRevitUtil.GetInstMojiParameter( m_doc, m_InstBase.Id, "鋼材サイズ(ダブル上)" ) ;
            }
            else {
              steelSize = ClsRevitUtil.GetInstMojiParameter( m_doc, m_InstBase.Id, "鋼材サイズ(ダブル下)" ) ;
            }
          }

          grpHojoPiece.Enabled = true ;
          chkUseJackCover.Enabled = false ;
          btnSetJack1.Enabled = false ;
          btnSetJack2.Enabled = false ;
          btnSetKirikaePieceS.Enabled = true ;
          btnSetKirikaePieceE.Enabled = true ;

          rbnNormal.Enabled = true ;

          if ( steelSize == "35HA" || steelSize == "40HA" ) {
            rbnSMH.Enabled = true ;
          }
          else if ( steelSize == "35SMH" || steelSize == "40SMH" ) {
            rbnSMH.Enabled = true ;
          }
          else if ( steelSize == "60SMH" ) {
            rbnSMH.Enabled = true ;
          }
          else {
            rbnSMH.Enabled = false ;
          }

          btnMegaBeam.Enabled = false ;

          if ( steelSize == "40HA" || steelSize == "50HA" ) {
            rbnTwinBeam.Enabled = true ;
          }
          else if ( steelSize == "40SMH" ) {
            rbnTwinBeam.Enabled = true ;
          }
          else if ( steelSize == "60SMH" ) {
            rbnTwinBeam.Enabled = true ;
          }
          else {
            rbnTwinBeam.Enabled = false ;
          }

          break ;
        case "切梁受け材" :
          break ;
        case "切梁繋ぎ材" :
          break ;
        case "火打繋ぎ材" :
          break ;
        case "切梁継ぎ" :
          if ( ClsRevitUtil.GetInstMojiParameter( m_doc, m_InstBase.Id, "構成" ) == "シングル" ) {
            steelSize = ClsRevitUtil.GetInstMojiParameter( m_doc, m_InstBase.Id, "鋼材サイズ(シングル)" ) ;
          }
          else {
            if ( m_isTop ) {
              steelSize = ClsRevitUtil.GetInstMojiParameter( m_doc, m_InstBase.Id, "切梁側/鋼材サイズ(ダブル)" ) ;
            }
            else {
              steelSize = ClsRevitUtil.GetInstMojiParameter( m_doc, m_InstBase.Id, "腹起側/鋼材サイズ(ダブル)" ) ;
            }
          }

          grpHojoPiece.Enabled = true ;
          chkUseJackCover.Enabled = false ;
          btnSetJack1.Enabled = false ;
          btnSetJack2.Enabled = false ;
          btnSetKirikaePieceS.Enabled = true ;
          btnSetKirikaePieceE.Enabled = true ;

          rbnNormal.Enabled = true ;

          if ( steelSize == "35HA" || steelSize == "40HA" ) {
            rbnSMH.Enabled = true ;
          }
          else if ( steelSize == "35SMH" || steelSize == "40SMH" ) {
            rbnSMH.Enabled = true ;
          }
          else if ( steelSize == "60SMH" ) {
            rbnSMH.Enabled = true ;
          }
          else {
            rbnSMH.Enabled = false ;
          }

          btnMegaBeam.Enabled = false ;

          if ( steelSize == "40HA" || steelSize == "50HA" ) {
            rbnTwinBeam.Enabled = true ;
          }
          else if ( steelSize == "40SMH" ) {
            rbnTwinBeam.Enabled = true ;
          }
          else if ( steelSize == "60SMH" ) {
            rbnTwinBeam.Enabled = true ;
          }
          else {
            rbnTwinBeam.Enabled = false ;
          }

          break ;
        default :
          break ;
      }

      InitHojoPieceList() ;
      InitSyuzaiButton() ;
    }

    /// <summary>
    /// 補助ピースのリスト
    /// </summary>
    private void InitHojoPieceList()
    {
      lstHojoPiece.Items.Clear() ;

      bool isSMH = false ;
      bool isTwinBeam = false ;
      if ( m_WaritsukeMode == DlgWaritsuke.WaritsukeMode.SMH ) {
        isSMH = true ;
      }
      else if ( m_WaritsukeMode == DlgWaritsuke.WaritsukeMode.TwinBeam ) {
        isTwinBeam = true ;
      }

      string typeName = m_InstBase.Name.Replace( "ベース", "" ) ;
      string steelSize = string.Empty ;
      switch ( typeName ) {
        case "腹起" :
        case "切梁" :
        case "斜梁" :
        case "斜梁受け材" :
        case "斜梁火打" :
          steelSize = ClsRevitUtil.GetInstMojiParameter( m_doc, m_InstBase.Id, "鋼材サイズ" ) ;
          if ( steelSize == "60SMH" || steelSize == "80SMH" ) {
            steelSize = "40HA" ;
          }

          break ;
        case "斜梁繋ぎ材" :
          steelSize = ClsRevitUtil.GetInstMojiParameter( m_doc, m_InstBase.Id, "サイズ" ) ;
          break ;
        case "切梁火打" :
          if ( ClsRevitUtil.GetInstMojiParameter( m_doc, m_InstBase.Id, "構成" ) == "シングル" ) {
            steelSize = ClsRevitUtil.GetInstMojiParameter( m_doc, m_InstBase.Id, "鋼材サイズ(シングル)" ) ;
          }
          else {
            if ( m_isTop ) {
              steelSize = ClsRevitUtil.GetInstMojiParameter( m_doc, m_InstBase.Id, "鋼材サイズ(ダブル上)" ) ;
            }
            else {
              steelSize = ClsRevitUtil.GetInstMojiParameter( m_doc, m_InstBase.Id, "鋼材サイズ(ダブル下)" ) ;
            }
          }

          break ;
        case "隅火打" :
          steelSize = ClsRevitUtil.GetInstMojiParameter( m_doc, m_InstBase.Id, "鋼材サイズ" ) ;
          break ;
        case "切梁継ぎ" :
          if ( ClsRevitUtil.GetInstMojiParameter( m_doc, m_InstBase.Id, "構成" ) == "シングル" ) {
            steelSize = ClsRevitUtil.GetInstMojiParameter( m_doc, m_InstBase.Id, "鋼材サイズ(シングル)" ) ;
          }
          else {
            if ( m_isTop ) {
              steelSize = ClsRevitUtil.GetInstMojiParameter( m_doc, m_InstBase.Id, "切梁側/鋼材サイズ(ダブル)" ) ;
            }
            else {
              steelSize = ClsRevitUtil.GetInstMojiParameter( m_doc, m_InstBase.Id, "腹起側/鋼材サイズ(ダブル)" ) ;
            }
          }

          break ;
        default :
          break ;
      }

      steelSize = steelSize.Replace( "SMH", "HA" ) ;

      foreach ( var piece in ClsSupportPieceCsv.GetSizeList( steelSize, isSMH, isTwinBeam ).ToArray() ) {
        if ( ! lstHojoPiece.Items.Cast<string>().Any( item => item.Equals( piece ) ) ) {
          lstHojoPiece.Items.Add( piece ) ;
        }
      }
    }

    private void InitSyuzaiButton()
    {
      switch ( m_WaritsukeMode ) {
        case WaritsukeMode.Normal :
          rbnNormal.Enabled = true ;
          rbnNormal.Checked = true ;
          btn10.Enabled = true ;
          btn15.Enabled = true ;
          btn20.Enabled = true ;
          btn25.Enabled = true ;
          btn30.Enabled = true ;
          btn35.Enabled = true ;
          btn40.Enabled = true ;
          btn45.Enabled = true ;
          btn50.Enabled = true ;
          btn55.Enabled = true ;
          btn60.Enabled = true ;
          btn65.Enabled = true ;
          btn70.Enabled = true ;
          btn75.Enabled = false ;
          btn80.Enabled = false ;
          btn85.Enabled = false ;
          btn90.Enabled = false ;
          break ;
        case WaritsukeMode.SMH :
          rbnSMH.Enabled = true ;
          rbnSMH.Checked = true ;
          btn10.Enabled = true ;
          btn15.Enabled = true ;
          btn20.Enabled = true ;
          btn25.Enabled = true ;
          btn30.Enabled = true ;
          btn35.Enabled = true ;
          btn40.Enabled = true ;
          btn45.Enabled = true ;
          btn50.Enabled = true ;
          btn55.Enabled = true ;
          btn60.Enabled = true ;
          btn65.Enabled = true ;
          btn70.Enabled = true ;
          btn75.Enabled = false ;
          btn80.Enabled = false ;
          btn85.Enabled = false ;
          btn90.Enabled = false ;
          break ;
        case WaritsukeMode.TwinBeam :
          rbnTwinBeam.Enabled = true ;
          rbnTwinBeam.Checked = true ;
          btn10.Enabled = false ;
          btn15.Enabled = false ;
          btn20.Enabled = false ;
          btn25.Enabled = false ;
          btn30.Enabled = false ;
          btn35.Enabled = false ;
          btn40.Enabled = true ;
          btn45.Enabled = false ;
          btn50.Enabled = true ;
          btn55.Enabled = false ;
          btn60.Enabled = true ;
          btn65.Enabled = false ;
          btn70.Enabled = true ;
          btn75.Enabled = false ;
          btn80.Enabled = true ;
          btn85.Enabled = false ;
          btn90.Enabled = true ;
          break ;
        default :
          break ;
      }
    }

    public static bool ShowSelectDialogHaraokoshi()
    {
      TaskDialog td = new TaskDialog( "割付ベースの選択" ) ;
      td.MainInstruction = "横ダブルのベースが選択されました" + "\n" + "掘削側を割付しますか？" + Environment.NewLine + "はい：掘削側　いいえ：壁側" ;
      td.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No ;
      td.DefaultButton = TaskDialogResult.No ;

      TaskDialogResult result = td.Show() ;
      if ( result == TaskDialogResult.Yes ) {
        return true ;
      }
      else if ( result == TaskDialogResult.No ) {
        return false ;
      }

      return false ;
    }

    public static bool ShowSelectDialogCornerHiuchi()
    {
      TaskDialog td = new TaskDialog( "上下段の選択" ) ;
      td.MainInstruction = "上段を割付しますか？" ;
      td.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No ;
      td.DefaultButton = TaskDialogResult.Yes ;

      TaskDialogResult result = td.Show() ;
      if ( result == TaskDialogResult.Yes ) {
        return true ;
      }
      else if ( result == TaskDialogResult.No ) {
        return false ;
      }

      return false ;
    }

    public static bool ShowSelectDialogCornerHiuchi2()
    {
      TaskDialog td = new TaskDialog( "上下段の選択" ) ;
      td.MainInstruction = "上段に配置しますか？\n" + "（はい:上段　いいえ:下段）" ;
      td.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No ;
      td.DefaultButton = TaskDialogResult.Yes ;

      TaskDialogResult result = td.Show() ;
      if ( result == TaskDialogResult.Yes ) {
        return true ;
      }
      else if ( result == TaskDialogResult.No ) {
        return false ;
      }

      return false ;
    }

    public static bool ShowSelectDialogKiribariTsugi( string target1, string target2 )
    {
      TaskDialog td = new TaskDialog( "割付の選択" ) ;
      td.MainInstruction = target1 + "に配置しますか？\n" + "（はい:" + target1 + "いいえ:" + target2 + "）" ;
      td.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No ;
      td.DefaultButton = TaskDialogResult.Yes ;

      TaskDialogResult result = td.Show() ;
      if ( result == TaskDialogResult.Yes ) {
        return true ;
      }
      else if ( result == TaskDialogResult.No ) {
        return false ;
      }

      return false ;
    }

    #region コントロールイベント

    private void btn10_Click( object sender, EventArgs e )
    {
      m_Command = WaritukeCommand.CreateSyuzai ;
      m_Length = 1.0 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn15_Click( object sender, EventArgs e )
    {
      m_Command = WaritukeCommand.CreateSyuzai ;
      m_Length = 1.5 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn20_Click( object sender, EventArgs e )
    {
      m_Command = WaritukeCommand.CreateSyuzai ;
      m_Length = 2.0 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn25_Click( object sender, EventArgs e )
    {
      m_Command = WaritukeCommand.CreateSyuzai ;
      m_Length = 2.5 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn30_Click( object sender, EventArgs e )
    {
      m_Command = WaritukeCommand.CreateSyuzai ;
      m_Length = 3.0 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn35_Click( object sender, EventArgs e )
    {
      m_Command = WaritukeCommand.CreateSyuzai ;
      m_Length = 3.5 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn40_Click( object sender, EventArgs e )
    {
      m_Command = WaritukeCommand.CreateSyuzai ;
      m_Length = 4.0 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn45_Click( object sender, EventArgs e )
    {
      m_Command = WaritukeCommand.CreateSyuzai ;
      m_Length = 4.5 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn50_Click( object sender, EventArgs e )
    {
      m_Command = WaritukeCommand.CreateSyuzai ;
      m_Length = 5.0 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn55_Click( object sender, EventArgs e )
    {
      m_Command = WaritukeCommand.CreateSyuzai ;
      m_Length = 5.5 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn60_Click( object sender, EventArgs e )
    {
      m_Command = WaritukeCommand.CreateSyuzai ;
      m_Length = 6.0 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn65_Click( object sender, EventArgs e )
    {
      m_Command = WaritukeCommand.CreateSyuzai ;
      m_Length = 6.5 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn70_Click( object sender, EventArgs e )
    {
      m_Command = WaritukeCommand.CreateSyuzai ;
      m_Length = 7.0 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn75_Click( object sender, EventArgs e )
    {
      m_Command = WaritukeCommand.CreateSyuzai ;
      m_Length = 7.5 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn80_Click( object sender, EventArgs e )
    {
      m_Command = WaritukeCommand.CreateSyuzai ;
      m_Length = 8.0 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn85_Click( object sender, EventArgs e )
    {
      m_Command = WaritukeCommand.CreateSyuzai ;
      m_Length = 8.5 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btn90_Click( object sender, EventArgs e )
    {
      m_Command = WaritukeCommand.CreateSyuzai ;
      m_Length = 9.0 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btnMegaBeam_Click( object sender, EventArgs e )
    {
      m_Command = WaritukeCommand.CreateMegaBeam ;
      m_Length = 0.0 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btnSetHojoPiece_Click( object sender, EventArgs e )
    {
      if ( lstHojoPiece.SelectedItem != null ) {
        m_Command = WaritukeCommand.CreateHojoPiece ;
        m_Size = lstHojoPiece.SelectedItem.ToString() ;
        m_Length = 0.0 ;
        this.DialogResult = DialogResult.OK ;
        this.Close() ;
      }
    }

    private void btnSetJack1_Click( object sender, EventArgs e )
    {
      m_Command = WaritukeCommand.CreateJack1 ;
      m_UseJackCover = chkUseJackCover.Checked ;
      m_Length = 0.0 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btnSetJack2_Click( object sender, EventArgs e )
    {
      m_Command = WaritukeCommand.CreateJack2 ;
      m_UseJackCover = chkUseJackCover.Checked ;
      m_Length = 0.0 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btnSetKirikaePieceS_Click( object sender, EventArgs e )
    {
      m_Command = WaritukeCommand.CreateKirikaePieceS ;
      m_Length = 0.0 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btnSetKirikaePieceE_Click( object sender, EventArgs e )
    {
      m_Command = WaritukeCommand.CreateKirikaePieceE ;
      m_Length = 0.0 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btnUndo_Click( object sender, EventArgs e )
    {
      m_Command = WaritukeCommand.CommandUndo ;
      m_Length = 0.0 ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    private void btnEnd_Click( object sender, EventArgs e )
    {
      m_Command = WaritukeCommand.CommandEnd ;
      m_Length = 0.0 ;
      this.DialogResult = DialogResult.Cancel ;
      this.Close() ;
    }

    private void rbnNormal_CheckedChanged( object sender, EventArgs e )
    {
      m_WaritsukeMode = WaritsukeMode.Normal ;
      m_Command = WaritukeCommand.SwitchNormal ;
      //InitHojoPieceList();
      //InitSyuzaiButton();
      this.DialogResult = DialogResult.OK ;
      if ( this.Visible ) {
        this.Close() ;
      }
    }

    private void rbnSMH_CheckedChanged( object sender, EventArgs e )
    {
      m_WaritsukeMode = WaritsukeMode.SMH ;
      m_Command = WaritukeCommand.SwitchSMH ;
      //InitHojoPieceList();
      //InitSyuzaiButton();
      this.DialogResult = DialogResult.OK ;
      if ( this.Visible ) {
        this.Close() ;
      }
    }

    private void rbnTwinBeam_CheckedChanged( object sender, EventArgs e )
    {
      m_WaritsukeMode = WaritsukeMode.TwinBeam ;
      m_Command = WaritukeCommand.SwitchTwinBeam ;
      //InitHojoPieceList();
      //InitSyuzaiButton();
      this.DialogResult = DialogResult.OK ;
      if ( this.Visible ) {
        this.Close() ;
      }
    }

    #endregion

    private void DlgWaritsuke_Load( object sender, EventArgs e )
    {
      // 前回の位置情報があれば、それを使用してダイアログを配置する
      if ( m_LastDialogLocation != Point.Empty ) {
        this.StartPosition = FormStartPosition.Manual ;
        this.Location = m_LastDialogLocation ;
      }
    }

    private void DlgWaritsuke_FormClosed( object sender, FormClosedEventArgs e )
    {
      // ダイアログが閉じられたときに位置情報を保存する
      m_LastDialogLocation = this.Location ;
    }

    public void SetRemainingLength( double remainingLength )
    {
      txtRemainingLength.Text = "残り長さ " + remainingLength.ToString() ;
    }
  }
}