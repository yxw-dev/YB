using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace Client
{
    public partial class Form1 : Form
    {
        string mould = "05022654\r\n01466526\r\n05984512\r\n06415288";
        chart plot = new chart();

        public Form1()
        {
            InitializeComponent();
            ini_ini();
            Control.CheckForIllegalCrossThreadCalls = false;
        }
        public void ini_ini()
        {
            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\configure.ini"))
            {
                file = new IniFile(System.Windows.Forms.Application.StartupPath + "\\configure.ini");
                int num = int.Parse(file.IniFileRead("操作个数", "个数"));
                for (int i = 0; i < num; i++)
                {
                    string name = file.IniFileRead("操作名称", i.ToString());
                    string or = file.IniFileRead("操作指令", i.ToString());
                    order.Add(name, or);
                    Add_Button(name, groupBox1);
                }
            }
            else
            {
                file = new IniFile(System.Windows.Forms.Application.StartupPath + "\\configure.ini");
            }
        }

        List<double> dat = new List<double>();

        Socket client; IniFile file;
        private void button1_Click(object sender, EventArgs e)
        {
            client = new Socket(AddressFamily.InterNetwork , SocketType.Stream , ProtocolType.Tcp);
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(textBox1.Text), int.Parse(textBox2.Text));
            //client.ReceiveTimeout = 2000;
            //client.SendTimeout = 1000;
            try
            {
                client.Connect(ip);
            }
            catch
            {
                int p = 1;
            }
            write_textbox("连接成功");

            Thread re = new Thread(receive);    
            re.IsBackground = true;
            re.Start(client);
        }
        private byte[] buffer = new byte[1024 * 1024];
        //public List<double> data = new List<double>();
        Queue<string> ord_que = new Queue<string>();
        private void receive(object o)
        {
            while (true)
            {
                //Socket temp = client.Accept();
                int num = client.Receive(buffer);
                if (num == 0)
                    break;

                string temp = Encoding.UTF8.GetString(buffer, 0, num);

                string q = ord_que.Dequeue();
                deal d = new deal(temp, q , dat);
                d.run(temp, q, dat);
                dat.AddRange(d.length);
                u++;
                DateTime ti2 = DateTime.Now;
                TimeSpan sp = ti2.Subtract(ti1);
                write_textbox(client.RemoteEndPoint.ToString() + ":" + Encoding.UTF8.GetString(buffer , 0 , num));
                //write_textbox(sp.TotalMilliseconds.ToString());
;            }
        }
        public static int u = 0;
        private void write_textbox(string str)
        {
            textBox3.AppendText(str + "\r\n");
        }

        public void send_ord(string text)
        {
            byte[] b = new byte[1024 * 1024];
            b = Encoding.UTF8.GetBytes(text + "\r");

            //从text中提取出命令
            string order = text[0].ToString();int num = 0;
            for (int i = 1; i < text.Length; i++)
            {
                if ((text[i] < 90 && 65 < text[i]) | (text[i] < 122) && (97 < text[i]))
                    order += text[i];
                else
                {
                    break;
                    //i = text.Length;
                }
            }
            ord_que.Enqueue(text);
            client.Send(b);
        }

        static DateTime ti1;
        private void button2_Click(object sender, EventArgs e)
        {
            ti1 = DateTime.Now;
            send_ord(textBox4.Text);
        }

        List<Button> bu = new List<Button>();
        Dictionary<string, string> order = new Dictionary<string, string>();
        public void Add_Button(string text , Control e)
        {
            System.Windows.Forms.Button button = new System.Windows.Forms.Button();
            button.Name = bu.Count.ToString();
            button.Location = new Point(6, bu.Count * 25 + 18);
            button.Text = text;
            button.Font = new Font(button.Font.FontFamily, 10);
            //button.BackColor = Color.Yellow;
            button.AutoSize = false;
            button.Width = 80;
            button.Height = 25;
            button.TextAlign = ContentAlignment.MiddleCenter;
            e.Controls.Add(button);
            bu.Add(button);
            //add_lab.Add(button);
            //picturebox.Controls.Add(button);
            button.Click += new EventHandler(dyn_lab);
        }

        private void dyn_lab(object sender, System.EventArgs e)
        {
            System.Windows.Forms.Button temp = sender as System.Windows.Forms.Button;
            send_ord(order[temp.Text]);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            button5.Visible = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            button5.Visible = false;
            if (textBox5.Text == "")
                return;
            Add_Button(textBox5.Text , groupBox1);
            
            order.Add(textBox5.Text , textBox6.Text);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //删了重建
            File.Delete(System.Windows.Forms.Application.StartupPath + "\\configure.ini");
            Thread.Sleep(50);
            file = new IniFile(System.Windows.Forms.Application.StartupPath + "\\configure.ini");
            file.IniFileWrite("操作个数", "个数", order.Count.ToString());
            for (int i = 0; i < order.Count; i++)
            {
                file.IniFileWrite("操作名称", i.ToString(),bu[i].Text);
                file.IniFileWrite("操作指令", i.ToString(), order[bu[i].Text]);
            }
        }

        double[] RRR = null;
        private void button6_Click(object sender, EventArgs e)
        {
            if (dat.Count < 5)
            {
                MessageBox.Show("数据个数小于40000个");
                return;
            }
            dat = todouble(dat);
            int[] num = new int[7] { 5000, 10000, 15000, 20000, 25000, 30000, 35000 };
            
            string str = "总数据个数：" + RRR.Count().ToString() + "有效数据个数：" + weed(dat).Count.ToString() + "位置数据" + RRR[num[0]].ToString() +
                "," + RRR[num[1]].ToString() + "," + RRR[num[2]].ToString() + "," + RRR[num[3]].ToString() + "," + RRR[num[4]].ToString() + "," +
                RRR[num[5]].ToString() + "," + RRR[num[6]].ToString();

            write_textbox(str);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (bu.Count == 0)
                return;

            //删除按钮列表中最后一个
                //删除字典最后一对键-值
                 string key = order.Last().Key.ToString();
                if (key != "")
                {
                    key.Remove(0);
                    key.Remove(key.Count() - 1);
                }
                 order.Remove(key);


                //删除按钮
                bu.Last().Dispose();
                bu.RemoveAt(bu.Count - 1);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (dat.Count == 0)
                return;
            double x = e.X;
            if (x < 20 | x > pictureBox1.Width)
            {
                label3.Text = "0";
                return;
            }

            double rai = (x - 20) / (pictureBox1.Width - 20);

            double res;
            int num = (int)(rai * dat.Count * 1.05);
            if (num > dat.Count)
                res = 0;
            else
                res = dat[num];
            label3.Text = res.ToString();
        }

        public static int tim = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (tim < 100)
            {

                if (tim == 0)
                {
                    send_ord("rds zf" + (tim * 500) + ".d 500");
                    tim++;
                }
                else
                {
                    send_ord("rds zf" + (tim * 1000) + ".d 500");
                    tim++;
                }
            }
            else
            {
                write_textbox("读取完毕");
                RRR = dat.ToArray();
                tim = 0;
                timer1.Enabled = false;
            }
            //if (dat.Count < 50000)
            //    send_ord("rdf");
            //else
            //    timer1.Enabled = false;
        }
        double ave = 0;
        List<double> data1 = new List<double>();
        public void create_data()
        {
            Random re = new Random();
            int tiao = re.Next(0, 15);
            double d = 20 + (double)tiao / 10;

            if (data1.Count < 540)
                data1.Add(d);
            else
            {
                data1.RemoveAt(0);
                data1.Add(d);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            HSSFWorkbook wor = new HSSFWorkbook();
            ISheet sheet = wor.CreateSheet("sheet1");
            List <int> da= new List<int>();

            //for (int i = 0; i < 50000; i++)
            //{
            //    da.Add(i);
            //}

            //da = RRR.ToList();
            da = out_d;
            DateTime o1 = DateTime.Now;
            for (int k = 0; k < da.Count; k++)
            {
                IRow row = sheet.CreateRow(k);
                ICell cell = row.CreateCell(0);
                cell.SetCellValue(da[k]);
            }
            using (FileStream fs = File.OpenWrite("1.xlsx"))
            {
                wor.Write(fs);
            }
            DateTime o2 = DateTime.Now;
            TimeSpan sp1 = o2.Subtract(o1);
            textBox3.AppendText(sp1.TotalMilliseconds.ToString() + "\r\n");
        }

        public List<double> weed(List<double> leng)
        {
            for (int i = 0; i < leng.Count; i++)
            {
                if (i == 0)
                    continue;
                else if (i == leng.Count - 1)
                {
                    break;
                }
                else
                {
                    double sub1 = 0; double sub2 = 0;
                    sub1 = leng[i] - leng[i - 1];
                    sub2 = leng[i] - leng[i + 1];
                    if (sub1 > 1 && sub2 > 1
)
                        leng.RemoveAt(i);
                }
            }
            return leng;
        }

        public List<double> todouble(List<double> leng)
        {
            List<double> st = new List<double>();

            for (int i = 0; i < leng.Count; i++)
            {
                leng[i] = leng[i] / 100000;
            }
            st = leng;
            return st;
        }
        public List<int> step(List<double> leng)
        {
            List<int> st = new List<int>();
            double step = 50000; //设置跳变系数，现设置超过1mm跳变为跳动点

            for (int i = 0; i < leng.Count - 1; i++)
            {
                if ((Math.Abs(leng[i + 1] - leng[i])) > step && (Math.Abs(leng[i + 1] - leng[i]) < (10 * step)))
                {
                    st.Add(i);
                }
                else
                {

                }
            }
            return st;
        }

        private void button8_Click(object sender, EventArgs e)
        {
           dat.Clear();
           timer1.Enabled = true;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            List<double> test = new List<double>();
            for (int i = 0; i < 99; i++)
            {
                test.Add(00500200);
            }
            for (int k = 0; k < 99; k++)
            {
                test.Add(00510200);
            }
            for (int k = 0; k < 99; k++)
            {
                test.Add(00480200);
            }
            List<int> res = new List<int>();
            res = step(test);
          }

        private void button10_Click(object sender, EventArgs e)
        {
            List<double> draw = weed_to(RRR);

            //int time1 = draw.Count;
            for (int i = 0; i < draw.Count; i++)
            {
                if (draw[i] == 0)
                {
                    draw.RemoveAt(i);
                    i--;
                }
            }

            pictureBox1.Image = plot.Draw(draw, pictureBox1.Width, pictureBox1.Height);

            int[] num_dat = new int[7] { 5000, 10000, 15000, 20000, 25000, 30000, 35000 };
            double X = (int)(draw.Count * 1.05);
            double ratio_x = pictureBox1.Width / X;
            double youyi = pictureBox1.Width * 0.02;
            if (youyi > 30)
                youyi = 20;

            Graphics dra = Graphics.FromImage(pictureBox1.Image);
            dra.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //g.Clear(Color.Transparent);
            Pen pen = new Pen(Color.Green);
            for (int t = 0; t < num_dat.Count(); t++)
            {
                dra.DrawLine(pen, (float)(ratio_x * num_dat[t]) , 0 , (float)(ratio_x * num_dat[t]) , pictureBox1.Height - 3);
            }
        }

        /// <summary>
        /// 使用前一位替换掉原始数据中的粗大值
        /// </summary>
        public List<double> weed_to(double[] leng)
        {
            List<double> rt = new List<double>();

            for (int i = 0; i < leng.Count(); i++)
            {
                if (i == 0)
                    continue;
                else if (i == leng.Count() - 1)
                {
                    break;
                }
                else
                {
                    if (leng[i] > 6500000)
                    {
                        for (int t = 0; t < 8; t++)
                        {
                            if (leng[i + t] < 6500000)
                            {
                                leng[i] = leng[i + t];
                                break;
                            }
                        }
                    }
                }
            }
            rt = leng.ToList();
            return rt;
        }

        List<int> out_d = new List<int>();
        private void button11_Click(object sender, EventArgs e)
        {
            List<double> data_out = RRR.ToList();

            for (int i = 0; i < data_out.Count; i++)
            {
                if (data_out[i] == 0)
                    data_out.RemoveAt(i);
            }

            out_d = step(data_out);
        }
    }
}
