using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class deal
    {
        public deal(string text, string or , List<double> le)
        {
            data = text;
            ord = or;
            length = le;
        }

        public void run(string text, string or, List<double> le)
        {
            data = text;
            ord = or;
            length = new List<double>();

            if (or.ToUpper().Substring(0 , 1) == "R")
            {
                length = string_to_list(data);
            }

            //Thread th = new Thread(deal_thread);
            //th.IsBackground = true;
            //th.Start();
        }

        private void deal_thread()
        {
            ord.ToUpper();
            switch (ord)
            {
                //1.正常读取数据（一次最多500个可能要100次）
                case "RD":
                    length = string_to_list(data);
                    break;
                case "RDE":
                    length = string_to_list(data);
                    break;
                case "RDS":
                    length = string_to_list(data);
                    break;

                //2.正常写入数据（控制电机，置急停位）
                case "WR":
                    length = string_to_list(data);
                    break;
                case "WRE":
                    length = string_to_list(data);
                    break;
                case "WRS":
                    length = string_to_list(data);
                    break;

                //切换cpu工作模式
                case "M":
                    string res = data.Substring(0, 1);
                    break;

                //2.正常以执行
                case "2":
                    break;

                //3.错误未执行
                case "3":
                    break;
            }
        }

        private List<double> string_to_list(string temp)
        {
            string regu = @"\d{10}";

            List<double> tt = new List<double>();
            string tem = null;
            foreach (Match math in Regex.Matches(temp, regu))
            {
                tt.Add(double.Parse(math.ToString()));
                //textBox3.AppendText(math.ToString() + "\r\n");
            }
            return tt;
        }
        public List<double> length;
        private string data;
        private string ord;
    }
}
