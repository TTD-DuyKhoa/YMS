using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows.Forms ;

namespace YMS_gantry
{
  public class MessageUtil
  {
    /// <summary>
    /// 警告メッセージ表示
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="title"></param>
    /// <param name="frm"></param>
    public static void Warning( string msg, string title, Form frm = null )
    {
      MessageBox.Show( frm, msg, title, MessageBoxButtons.OK, MessageBoxIcon.Warning ) ;
    }

    /// <summary>
    /// 情報表示
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="title"></param>
    /// <param name="frm"></param>
    public static void Information( string msg, string title, Form frm = null )
    {
      MessageBox.Show( frm, msg, title, MessageBoxButtons.OK, MessageBoxIcon.Information ) ;
    }

    /// <summary>
    /// 警告メッセージ表示
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="title"></param>
    /// <param name="frm"></param>
    public static void Error( string msg, string title, Form frm = null )
    {
      MessageBox.Show( frm, msg, title, MessageBoxButtons.OK, MessageBoxIcon.Error ) ;
    }

    /// <summary>
    /// Yes,Noを返す
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="title"></param>
    /// <param name="frm"></param>
    /// <returns></returns>
    public static DialogResult YesNo( string msg, string title, Form frm = null )
    {
      return MessageBox.Show( frm, msg, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question ) ;
    }
  }
}