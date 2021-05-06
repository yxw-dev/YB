using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;

namespace excel
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// conn[0]连接.xls  , conn连接xlsx文件
        /// 使用时conn[0(1)] + PATH + conn[3]
        /// conn[3]参数说明：HDR：Yes这代表第一行是标题，不做为数据使用,系统默认的是YES
        ///                  IMEX：0写入； 1 ：只读； 2：可读可写
        /// </summary>
        string[] conn = new string[4] { "Provider=Microsoft.Jet.OLEDB.4.0;Data Source="
            , "Provider=Microsoft.Jet.OLEDB.4.0;Data Source="
            ,"; Extended Properties = 'Excel 8.0;HDR=No;IMEX=2;'"  , "; Extended Properties = 'Excel 12.0;HDR=Yes;IMEX=2;'" };
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //OleDbCommand
        }

        private void button2_Click(object sender, EventArgs e)
        {
            exlpath.ShowDialog();
            string path = exlpath.SelectedPath;
        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            string path = System.Windows.Forms.Application.StartupPath;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            { 
                path = System.IO.Path.GetFullPath(openFileDialog1.FileName);

                string camText = conn[0] + path + ";" + conn[2];
                OleDbConnection com = new OleDbConnection(camText);

                OleDbDataAdapter ada = new OleDbDataAdapter("select * from [sheet1$]", com);

                DataSet ds = new DataSet();

                ada.Fill(ds);

                com.Close();

                DataTableCollection dtc =  ds.Tables;
                DataTable dt = dtc[0];
                DataRowCollection drc = dt.Rows;

                foreach (DataRow row in drc)
                {
                    string showtext = null;
                    //for (int i = 0; i < dt.Columns.Count ; i++)
                    //{
                    showtext = showtext + " " + row[2];
                    //}
                    showtext = showtext + "\r\n";
                    textBox1.AppendText(showtext);
                }
            }
        }


        public static void DTToExcel(string Path, System.Data.DataTable dt)
        {
            string strCon = string.Empty;
            FileInfo file = new FileInfo(Path);
            string extension = file.Extension;
            switch (extension)
            {
                case ".xls":
                    strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Path + ";Extended Properties=Excel 8.0;";
                    //strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Path + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=0;'";
                    //strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Path + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=2;'";
                    break;
                case ".xlsx":
                    //strCon = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Path + ";Extended Properties=Excel 12.0;";
                    //strCon = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Path + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=2;'";    //出现错误了
                    strCon = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Path + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=0;'";
                    break;
                default:
                    strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Path + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=0;'";
                    break;
            }
            try
            {
                using (System.Data.OleDb.OleDbConnection con = new System.Data.OleDb.OleDbConnection(strCon))
                {
                    con.Open();
                    StringBuilder strSQL = new StringBuilder();
                    System.Data.OleDb.OleDbCommand cmd;
                    try
                    {
                        cmd = new System.Data.OleDb.OleDbCommand(string.Format("drop table {0}", dt.TableName), con);    //覆盖文件时可能会出现Table 'Sheet1' already exists.所以这里先删除了一下
                        cmd.ExecuteNonQuery();
                    }
                    catch { }
                    //创建表格字段
                    strSQL.Append("CREATE TABLE ").Append("[" + dt.TableName + "]");
                    strSQL.Append("(");
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        strSQL.Append("[" + dt.Columns[i].ColumnName + "] text,");
                    }
                    strSQL = strSQL.Remove(strSQL.Length - 1, 1);
                    strSQL.Append(")");

                    cmd = new System.Data.OleDb.OleDbCommand(strSQL.ToString(), con);
                    cmd.ExecuteNonQuery();
                    //添加数据
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        strSQL.Clear();
                        StringBuilder strvalue = new StringBuilder();
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            strvalue.Append("'" + dt.Rows[j].ToString().Replace("'", "''") + "'");
                            if (j != dt.Columns.Count - 1)
                            {
                                strvalue.Append(",");
                            }
                            else
                            {
                            }
                        }
                        cmd.CommandText = strSQL.Append(" insert into [" + dt.TableName + "] values (").Append(strvalue).Append(")").ToString();
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
            }
            catch { }
        }
    }
}
