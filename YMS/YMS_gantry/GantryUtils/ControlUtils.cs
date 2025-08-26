using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YMS_gantry.GantryUtils
{
    public class ControlUtils
    {
        /// <summary>
        /// コンボボックスのリストを設定
        /// </summary>
        /// <param name="cmb"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static bool SetComboBoxItems(ComboBox cmb , List<string>items, bool isSlectedDefaultIndex = true)
        {
            try
            {
                string bk = cmb.Text;
                cmb.Items.Clear();
                foreach (string str in items)
                {
                    cmb.Items.Add(str);
                }
                if (items.Contains(bk)) cmb.Text = bk;
                else if (items.Count == 0) cmb.Text = string.Empty;
                else if (isSlectedDefaultIndex) cmb.SelectedIndex = 0;
            }
            catch (System.Exception e)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// コンボボックスのリストを設定
        /// </summary>
        /// <param name="cmb"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static bool SetListBoxItems(ListBox lst, List<string> items, bool isSlectedDefaultIndex = true)
        {
            try
            {
                string bk = lst.SelectedItem==null?string.Empty: lst.SelectedItem.ToString();
                lst.Items.Clear();
                foreach (string str in items)
                {
                    lst.Items.Add(str);
                }
                if (items.Contains(bk)) lst.SelectedItem = bk;
                else if (isSlectedDefaultIndex) lst.SelectedIndex = 0;
            }
            catch (System.Exception e)
            {
                return false;
            }
            return true;
        }
    }
}
