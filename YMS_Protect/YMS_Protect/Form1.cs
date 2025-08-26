using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS_Protect
{
    public partial class Form1 : Form
    {
        private string ChkLockword = "YMS2023SecuritySystem"; // 文字列を難読化してください。 

        private string rKeyName = @"SOFTWARE\YMS\Pro";        // 操作するレジストリ・キーの名前

        private string rValueName = "value";                  // 取得処理を行う対象となるレジストリの値の名前

        private string rResultName = "res";

        public const int OK = 0;
        public const int NG = 1;
        public const int ETC = 2;

        public Form1()
        {
            InitializeComponent();
        }

        public int nShouldShowForm()
        {
            string strProInfo = "";
            string strDatFile = "";

            if (IntPtr.Size == 4)
            {
                passwordProtect1.DllFileName = "ProFuncNET32.dll";
                strProInfo = "ProInfo32.dll";
                strDatFile = "YMS_Protect.ProInfo32.dll.dat";
            }
            if (IntPtr.Size == 8)
            {
                passwordProtect1.DllFileName = "ProFuncNET64.dll";
                strProInfo = "ProInfo64.dll";
                strDatFile = "YMS_Protect.ProInfo64.dll.dat";
            }

            // リソースとして埋め込んだプロテクト情報のストリームを取得(PrNETSetProtect.exeで作成されたdatファイル)
            // 現在実行中のアセンブリを取得
            var assm = Assembly.GetExecutingAssembly();
            using (var stream = assm.GetManifestResourceStream(strDatFile))
            {
                // ストリームの内容をMemoryStreamにコピー
                var length = (int)stream.Length;
                var reader = new BinaryReader(stream);
                var memoryStream = new MemoryStream(length);

                memoryStream.Write(reader.ReadBytes(length), 0, length);

                passwordProtect1.Stream = memoryStream;
            }

            // 初期化（必ず実行してください）
            int ret = passwordProtect1.Initialize("YMS_Protect.exe", strProInfo);
            if (ret != 0)
            {
                switch (ret)
                {
                    case -1: MessageBox.Show("使用できません。"); break;
                    case 1: MessageBox.Show("サービスが動作していません。"); break;
                    case 2: MessageBox.Show("情報保持ファイルの読み込みエラーです。"); break;
                    case 3: MessageBox.Show("ロックワードが不正です。"); break;
                    case 4: MessageBox.Show("ローカルディスクで実行してください。"); break;
                    case 5: MessageBox.Show("エラーです。"); break;
                }
                // プロテクト情報の更新（必ず実行してください）
                passwordProtect1.Finish();
                return ETC;
            }

            string s = passwordProtect1.GetLockword();
            // 必ず「LockWord」のチェックを行ってください。
            // 又、アプリケーションをダンプしてロックワードの取得の手がかりを与えないように下記のようにチェックすると良いでしょう
            if (s != ChkLockword)
            {
                MessageBox.Show("ロックワードが不正です");
                // プロテクト情報の更新（必ず実行してください）
                passwordProtect1.Finish();
                return ETC;
            }

            // プロテクトの評価
            string sMsg = string.Empty;
            ret = passwordProtect1.QueryProtect();
            switch (ret)
            {
                case -1: sMsg = "使用不可"; break;
                case 0: sMsg = "プロテクトなし"; return OK;
                case 1: sMsg = "プロテクト中"; break;
                case 2: sMsg = "使用回数オーバー"; break;
                case 3: sMsg = "使用日数オーバー"; break;
                case 4: sMsg = "使用月数オーバー"; break;
                case 5: sMsg = "使用期限オーバー"; break;
                case 6: sMsg = "日付が戻されました。\nパスワードの再発行が必要です。"; break;
                case 7: sMsg = "PCが変更されました。\nパスワードの再発行が必要です。"; break;
                case 8: sMsg = "プロテクト情報保持ファイルが不正です。\nパスワードの再発行が必要です。"; break;
                case 9: sMsg = "ロックワードが不正です。"; break;
                case 10: sMsg = "パスワードが不正です。\nパスワードの再発行が必要です。"; break;
                case 11: sMsg = "プロテクト情報が正しく取得できませんでした。\nパスワードの再発行が必要です。"; break;
                case 12: sMsg = "プロテクト情報が不正です。\nパスワードの再発行が必要です。"; break;
            }

            MessageBox.Show(sMsg);

            return NG;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int ret = passwordProtect1.QueryProtect();
            if (passwordProtect1.ProtectWay != 0)
            {
                // 標準プロテクト以外
                if (ret == 0)
                {
                    // 特に処理は必要ない
                }
                else if (ret == 1 || ret == 2 || ret == 3 || ret == 4 || ret == 5)
                {
                    string sPassword = string.Empty;
                    sPassword = GetDecryptedPassword();

                    //パスワードのチェック
                    if (!passwordProtect1.SetPassword(sPassword))
                    {
                        //MessageBox.Show("使用期限が過ぎました");
                        // レジストリのパスワードを空にする
                        //if (!SetRegistry(string.Empty))
                        //{
                        //    MessageBox.Show("レジストリの初期化に失敗しました");
                        //    DialogResult = DialogResult.Cancel;
                        //    this.Close();
                        //}
                    }
                }
                else if (ret == 6 || ret == 7 || ret == 8 || ret == 10 || ret == 11 || ret == 12)
                {
                    // レジストリのパスワードを空にする
                    if (!SetRegistry(string.Empty))
                    {
                        MessageBox.Show("レジストリの初期化に失敗しました");
                        DialogResult = DialogResult.Cancel;
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("プロテクト情報が正しく取得できませんでした");
                    DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }

            textBox1.Text = passwordProtect1.QueryPcIDofPassword();

            //既にパスワードでロック解除済かどうかの確認
            string strPassword = "";
            strPassword = GetDecryptedPassword();
            if (passwordProtect1.SetPassword(strPassword))
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // プロテクト情報の更新（必ず実行してください）
            passwordProtect1.Finish();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string plainPassword = textBox2.Text;
            if (passwordProtect1.SetPassword(plainPassword))
            {
                if (SetEncryptedPassword(plainPassword))
                {
                    // パスワードの登録成功
                    MessageBox.Show("認証に成功しました");
                    DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private bool SetRegistry(string strPassword)
        {

            // レジストリの設定と削除
            try
            {
                // レジストリ・キーを新規作成して開く
                RegistryKey rKey = Registry.CurrentUser.CreateSubKey(rKeyName);

                // レジストリの値を設定
                rKey.SetValue(rValueName, strPassword);

                // 開いたレジストリを閉じる
                rKey.Close();

                //設定したレジストリの値をコンソールに表示
                //Console.WriteLine(location);
            }
            catch (Exception ex)
            {
                // レジストリ・キーが存在しない
                //Console.WriteLine(ex.Message);
                return false;
            }
            return true;

        }

        private bool GetRegistry(ref string strPassword)
        {
            // レジストリの取得
            try
            {
                // レジストリ・キーのパスを指定してレジストリを開く
                RegistryKey rKey = Registry.CurrentUser.OpenSubKey(rKeyName);

                // レジストリの値を取得
                string location = (string)rKey.GetValue(rValueName);

                // 開いたレジストリ・キーを閉じる
                rKey.Close();

                // コンソールに取得したレジストリの値を表示
                strPassword = location;
            }
            catch (NullReferenceException)
            {
                // レジストリ・キーまたは値が存在しない
                //Console.WriteLine("レジストリ［" + rKeyName
                //   + "］の［" + rGetValueName + "］がありません！");
                return false;

            }
            return true;
        }

        private bool SetResultRegistry(string strPassword)
        {

            // レジストリの設定と削除
            try
            {
                // レジストリ・キーを新規作成して開く
                RegistryKey rKey = Registry.CurrentUser.CreateSubKey(rKeyName);

                // レジストリの値を設定
                rKey.SetValue(rResultName, strPassword);

                // 開いたレジストリを閉じる
                rKey.Close();

                //設定したレジストリの値をコンソールに表示
                //Console.WriteLine(location);
            }
            catch (Exception ex)
            {
                // レジストリ・キーが存在しない
                //Console.WriteLine(ex.Message);
                return false;
            }
            return true;

        }

        // 暗号鍵と初期ベクトル (IV)
        private static readonly string key = "12345678qwertyui";
        private static readonly string iv = "1234567890123456";

        // パスワードをAESで暗号化してレジストリに登録
        private bool SetEncryptedPassword(string plainPassword)
        {
            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(key);
                    aesAlg.IV = Encoding.UTF8.GetBytes(iv);

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(plainPassword);
                            }
                        }

                        // 暗号化されたバイト配列をBase64文字列に変換してレジストリに登録
                        string encryptedPassword = Convert.ToBase64String(msEncrypt.ToArray());
                        if (!SetRegistry(encryptedPassword))
                        {
                            MessageBox.Show("パスワードの登録に失敗しました");
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("暗号化エラー: " + ex.Message);
                return false;
            }
        }

        // 暗号化されたパスワードを復号化して取得
        private string GetDecryptedPassword()
        {
            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(key);
                    aesAlg.IV = Encoding.UTF8.GetBytes(iv);

                    aesAlg.Padding = PaddingMode.PKCS7;

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    string encryptedPassword = "";
                    GetRegistry(ref encryptedPassword); // レジストリから暗号化されたパスワード取得
                    byte[] encryptedBytes = Convert.FromBase64String(encryptedPassword);

                    using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("復号化エラー: " + ex.Message);
                return null;
            }
        }
    }
}
