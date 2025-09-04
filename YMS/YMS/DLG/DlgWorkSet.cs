using System ;
using System.Collections.Generic ;
using System.ComponentModel ;
using System.Data ;
using System.Drawing ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;

namespace YMS.DLG
{
  public partial class DlgWorkSet : Form
  {
    /// <summary>
    /// ワークセット一覧
    /// </summary>
    public List<string> ListWorkSet ;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public DlgWorkSet()
    {
      InitializeComponent() ;
    }

    /// <summary>
    /// ロード処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DlgWorkSet_Load( object sender, EventArgs e )
    {
      //データグリッドに初期値を設定
      SetDataGridView() ;
    }

    /// <summary>
    /// OKボタン押下処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnOK_Click( object sender, EventArgs e )
    {
      SetData() ;
      this.DialogResult = DialogResult.OK ;
      this.Close() ;
    }

    /// <summary>
    /// キャンセルボタン押下処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnCancel_Click( object sender, EventArgs e )
    {
      this.DialogResult = DialogResult.Cancel ;
      this.Close() ;
    }

    /// <summary>
    /// DGVのデータをメンバ変数にセットする
    /// </summary>
    public void SetDataGridView()
    {
      List<string> lstWorkSet = new List<string>()
      {
        "共有レベルと通芯",
        "その他",
        "ベース",
        "SMW",
        "仮鋼材"
      } ;

      foreach ( string workSet in lstWorkSet ) {
        DataGridViewRow row = new DataGridViewRow() ;
        row.CreateCells( dgvWorkSet ) ;
        row.Cells[ 0 ].Value = workSet ;
        row.Cells[ 0 ].ReadOnly = true ;
        row.Cells[ 0 ].Style.BackColor = Color.FromArgb( 200, 200, 200 ) ;
        dgvWorkSet.Rows.Add( row ) ;
      }
    }

    /// <summary>
    /// DGVのデータをメンバ変数にセットする
    /// </summary>
    public void SetData()
    {
      List<string> lstData = new List<string>() ;
      foreach ( DataGridViewRow row in dgvWorkSet.Rows ) {
        string str = Convert.ToString( row.Cells[ 0 ].Value ) ;
        if ( ! string.IsNullOrEmpty( str ) ) {
          lstData.Add( str ) ;
        }
      }

      ListWorkSet = lstData ;
    }
  }
}