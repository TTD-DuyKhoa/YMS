using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS_Protect
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            int exitCode = 1;

            Form1 f1 = new Form1();
            switch (f1.nShouldShowForm())
            {
                case Form1.OK:
                    exitCode = 0;
                    break;
                case Form1.NG:
                    DialogResult dialogResult = f1.ShowDialog();
                    // ここで終了コードを設定
                    exitCode = GetExitCodeFromDialogResult(dialogResult);
                    break;
                case Form1.ETC:
                    exitCode = 2;
                    break;
                default:
                    exitCode = 2;
                    break;
            }

            Environment.Exit(exitCode);
        }

        private static int GetExitCodeFromDialogResult(DialogResult dialogResult)
        {
            switch (dialogResult)
            {
                case DialogResult.OK:
                    return 0; // 正常終了
                case DialogResult.Cancel:
                    return 1; // キャンセル終了
                default:
                    return 2; // その他の終了コード
            }
        }
    }
}
