using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using RevitUtil;

namespace YMS_Schedule
{
    public partial class FormScheduleFilter : System.Windows.Forms.Form
    {
        public Schedule sch { get; set; }
        public string[][] tbl { get; set; }
        public string[][] selectedTbl { get; set; }

        private string undefDialog = "(未定義)";
        public int indexItem1 { get; set; }
        public int indexItem2 { get; set; }
        public int indexItem3 { get; set; }
        public int indexDan { set; get; }
        public Dictionary<string, int> indexPhase { set; get; }

        bool close_permition;

        public FormScheduleFilter(Schedule sch_)
        {
            sch = sch_;

            indexPhase = new Dictionary<string, int>();

            InitializeComponent();
        }

        private void AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                TreeNode active = e.Node;     // チェックされたノード

                //// 上位ノードが Checked の場合、下位ノードのチェックを外せない
                //try
                //{
                //    TreeNode parent = active.Parent;
                //    if (parent != null)
                //    {
                //        if (parent.Checked && !active.Checked)
                //        {
                //            MessageBox.Show("上位と異なるCheck状態はとれません。");
                //            active.Checked = true;
                //            return;
                //        }
                //    }
                //}
                //catch
                //{ }

                // Check状態を全ての下位ノードに反映させる
                if (active.Nodes.Count > 0)
                {
                    CheckChildNodes(active, active.Checked);
                }
            }
        }

        // [CheckChildNodes]メソッド
        private void CheckChildNodes(TreeNode vnode, bool vchecked)
        {
            foreach (TreeNode node in vnode.Nodes)
            {
                node.Checked = vchecked;
                if (node.Nodes.Count > 0)
                {
                    CheckChildNodes(node, vchecked);
                }
            }
        }

        private void CheckNodes(TreeNode vnode, bool vchecked)
        {
            vnode.Checked = vchecked;
            CheckChildNodes(vnode, vchecked);
        }

        private void compensatePlus(string[][] tbl)
        {
            for(int i = 0; i < tbl.Length; i++)
            {
                compensatePlus(ref tbl[i]);
            }
        }
        private void compensatePlus(ref string[] vals)
        {
            if (string.IsNullOrEmpty(vals[indexItem1]))
            {
                vals[indexItem1] = undefDialog;
            }
            if (string.IsNullOrEmpty(vals[indexItem2]))
            {
                vals[indexItem2] = undefDialog;
            }
            if (string.IsNullOrEmpty(vals[indexItem3]))
            {
                vals[indexItem3] = undefDialog;
            }
        }
        private void FormScheduleFilter_Load(object sender, EventArgs e)
        {
            tbl = new string[sch.itms.Count][];
            for(int i = 0; i < sch.itms.Count; i++)
            {
                int n = sch.itms[i].items.Length;
                string[] row = new string[n];
                for(int j = 0; j < n; j++)
                {
                    row[j] = sch.itms[i].items[j];
                }
                tbl[i] = row;
            }

            //data location
            // indexDan = sch.h.FindIndex(x => x == Env.sharedParameterDan());
            indexDan = sch.h.FindIndex(x => x == Env.paramLevel);

            Env.ScheduleYMSFilter filter = Env.sheduleFilter();
            indexItem1 = sch.h.FindIndex(x => x == filter[Env.keyFilter1]);
            indexItem2 = sch.h.FindIndex(x => x == filter[Env.keyFilter2]);
            indexItem3 = sch.h.FindIndex(x => x == filter[Env.keyFilter3]);

            List<string> filterPhase = Env.parametersPhase();
            foreach(var x in filterPhase)
            {
                indexPhase.Add(x,sch.h.FindIndex(val => val == x));
            }

            //
            compensatePlus(tbl);

            IEnumerable<string> results;

            //item
            Dictionary<string, Dictionary<string, List<string>>> dataItem = new Dictionary<string, Dictionary<string, List<string>>>();
            results = (from fields in tbl
                       where fields[indexItem1] != ""
                       select String.Format("{0}", fields[indexItem1])
                       ).Distinct();
            if (results != null && results.Count() >= 1)
            {
                foreach(var x in results)
                {
                    dataItem.Add(x, new Dictionary<string, List<string>>());
                }
            }
            List<string> lst = dataItem.Keys.ToList();
            foreach (var x in dataItem.Keys.ToList())
            {
                results = (from fields in tbl
                           where fields[indexItem1] == x
                           && fields[indexItem2] != ""
                           select String.Format("{0}", fields[indexItem2])
                           ).Distinct();
                if (results != null && results.Count() >= 1)
                {
                    foreach(var y in results)
                    {
                        dataItem[x].Add(y,new List<string>());
                    }
                }
            }
            foreach (var x in dataItem.Keys)
            {
                foreach (var y in dataItem[x].Keys.ToList())
                {
                    results = (from fields in tbl
                                where fields[indexItem1] == x
                                && fields[indexItem2] == y
                                && fields[indexItem3] != ""
                               select String.Format("{0}", fields[indexItem3])
                                ).Distinct();
                    if (results != null && results.Count() >= 1)
                    {
                        dataItem[x][y] =  new List<string>(results.ToList());
                    }
                }
            }

            //dan
            List<string> dataDan = new List<string>();
            results =
                (from fields in tbl
                 where fields[indexDan] != ""
                 select String.Format("{0}", fields[indexDan])
                 ).Distinct();
            if (results != null && results.Count() >= 1)
            {
                dataDan.AddRange(results.ToList());
            }

            //Phase
            Dictionary<string, List<string>> dataPhase = new Dictionary<string, List<string>>();
            foreach (var x in filterPhase)
            {
                int index = indexPhase[x];
                results = (from fields in tbl
                           where fields[index] != ""
                           select String.Format("{0}", fields[index])
                    ).Distinct();
                if (results != null && results.Count() >= 1)
                {
                    dataPhase.Add(x, results.ToList());
                }
            }

            //
            foreach(var x in dataItem.Keys)
            {
                TreeNode tn = treeView1.Nodes.Add(x);
                foreach (var y in dataItem[x].Keys)
                {
                    TreeNode tn2 = tn.Nodes.Add(y);
                    foreach (var z in dataItem[x][y])
                    {
                        TreeNode tn3 = tn2.Nodes.Add(z);
                    }
                }
            }
            foreach (var x in dataDan)
            {
                listView1.Items.Add(x);
            }
            foreach (var x in dataPhase.Keys) { 
                TreeNode tn = treeView2.Nodes.Add(x);
                foreach (var y in dataPhase[x])
                {
                    TreeNode tn2 = tn.Nodes.Add(y);
                }
            }

            //check
            foreach (TreeNode x in treeView1.Nodes)
            {
                CheckNodes(x, true);
            }
            foreach (TreeNode x in treeView2.Nodes)
            {
                CheckNodes(x, true);
            }
            foreach (ListViewItem x in listView1.Items)
            {
                x.Checked = true;
            }

        }

        private void treeView2_AfterCheck(object sender, TreeViewEventArgs e)
        {
            AfterCheck(sender, e);
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            AfterCheck(sender, e);
        }

        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Control)
            {
                listView1.MultiSelect = true;
                foreach (ListViewItem item in listView1.Items)
                {
                    item.Selected = true;
                }
            }
        }

        public void AllLeavesChecked(System.Windows.Forms.TreeView treeView, List<TreeNode> checkedNodes)
        {
            foreach (TreeNode node in treeView.Nodes)
            {
                AllLeavesChecked(node, checkedNodes);
            }
        }
        public void AllLeavesChecked(TreeNode treeNode, List<TreeNode> checkedNodes)
        {
            if (treeNode.Nodes.Count == 0
            && treeNode.Checked
            )
            {
                checkedNodes.Add(treeNode);
            }
            else
            {
                foreach (TreeNode node in treeNode.Nodes)
                {
                    AllLeavesChecked(node, checkedNodes);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            close_permition = true;
            selectedTbl = null;

            IEnumerable<string> results;
            string s = Env.sharedParameterId();
            int indexId = sch.h.FindIndex(x => x == s);

            selectedTbl = new string[0][];

            //
            if (checkBox1.Checked)
            {
                List<string[]> tmp = new List<string[]>();

                List<TreeNode> checkedItem = new List<TreeNode>();
                AllLeavesChecked(treeView1, checkedItem);
                foreach(var x in checkedItem)
                {
                    string[] items = x.FullPath.Split(treeView1.PathSeparator[0]);
                    results = (from fields in tbl
                                where fields[indexItem1] == items[0]
                                && fields[indexItem2] == items[1]
                                && fields[indexItem3] == items[2]
                                select String.Format("{0}", fields[indexId])
                                );
                    if (results != null && results.Count() >= 1)
                    {
                        foreach (var y in results)
                        {
                            Item si = sch.itms.Find(v => v.elmId.ToString() == y);
                            tmp.Add(si.items);
                        }
                    }
                }
                selectedTbl = tmp.ToArray();
            }

            //
            if (checkBox2.Checked)
            {
                List<string[]> tmp = new List<string[]>();

                CheckedListViewItemCollection checkedDan = listView1.CheckedItems;

                foreach (ListViewItem x in checkedDan)
                {
                    results = (from fields in tbl
                               where fields[indexDan] == x.Text
                               select String.Format("{0}", fields[indexId])
                               );
                    if (results != null && results.Count() >= 1)
                    {
                        foreach (var y in results)
                        {
                            Item si = sch.itms.Find(v => v.elmId.ToString() == y);
                            tmp.Add(si.items);
                        }
                    }
                }
                List<string[]> tmpSel = new List<string[]>();
                for(int i = 0; i < selectedTbl.Length; i++)
                {
                    tmpSel.Add(selectedTbl[i]);
                }
                foreach (var x in tmp)
                {
                    if(! tmpSel.Exists(v => v[indexId] == x.ElementAt(indexId)))
                    {
                        Item si = sch.itms.Find(v => v.elmId.ToString() == x[indexId]);
                        tmpSel.Add(si.items);
                    }
                }
                selectedTbl = tmpSel.ToArray();
            }

            //
            if (checkBox3.Checked)
            {
                List<string[]> tmp = new List<string[]>();

                List<TreeNode> checkedPhase = new List<TreeNode>();
                AllLeavesChecked(treeView2, checkedPhase);

                //選択候補は1行に2列有
                Dictionary<string, int> dict = new Dictionary<string, int>();

                foreach (TreeNode x in checkedPhase)
                {
                    string[] items = x.FullPath.Split(treeView1.PathSeparator[0]);

                    results = (from fields in tbl
                               where fields[indexPhase[items[0]]] == x.Text
                               select String.Format("{0}", fields[indexId])
                               );
                    if (results != null && results.Count() >= 1)
                    {
                        foreach (var y in results)
                        {
                            if (! dict.ContainsKey(y))
                            {
                                dict.Add(y, 1);
                            }
                        }
                    }
                }

                foreach(var x in dict.Keys)
                {
                    Item si = sch.itms.Find(v => v.elmId.ToString() == x);
                    tmp.Add(si.items);
                }

                List<string[]> tmpSel = new List<string[]>();
                for (int i = 0; i < selectedTbl.Length; i++)
                {
                    tmpSel.Add(selectedTbl[i]);
                }
                foreach (var x in tmp)
                {
                    if (!tmpSel.Exists(v => v[indexId] == x.ElementAt(indexId)))
                    {
                        Item si = sch.itms.Find(v => v.elmId.ToString() == x[indexId]);
                        tmpSel.Add(si.items);
                    }
                }
                selectedTbl = tmpSel.ToArray();
            }

            close_permition = (selectedTbl.Count() >= 1);
        }

        private void FormScheduleFilter_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!close_permition)
            {
                MessageBox.Show("出力対象がありません");
                close_permition = true;
                e.Cancel = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            close_permition = true;
        }
    }
}
