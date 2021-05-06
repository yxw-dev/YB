using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
/// <summary>
/// 2019-12-9修改画图方式（从bmp创建graphics绘图）
/// </summary>
namespace Client
{
    class chart
    {
        public chart() { }
        public chart(Control picture)
        {
            Width = picture.Width;
            Height = picture.Height;
        }

        private int Width;
        private int Height;
        private Point[] data;

        public List<Point> points_top = new List<Point>();
        public List<Point> points_down = new List<Point>();
        public List<double> Data = new List<double>();

        public int  Add_data(double data , int length)  //doubule为测量结果数据，length为图标绘制横坐标个数。个数满后清零再添加
        {
            if (Data.Count < length)
                Data.Add(data);
            else
                return 0;
            return 1;
        }
        public void modify_data(double data, int length) //doubule为测量结果数据，length为图标绘制横坐标个数。个数满后循环左移
        {
            if (Data.Count < length)
                Data.Add(data);
            else
            {
                Data.RemoveAt(0);
                Data.Add(data);
            }
        }

        public void Draw(Control picture)
        {
            Graphics g = picture.CreateGraphics();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //g.Clear(Color.Transparent);
            Pen pen = new Pen(Color.Red);
            points_top = trans(Data);
            if (points_top.Count <= 1)
                return;
            g.DrawLines(pen , points_top.ToArray());
            points_down = imversion(points_top);
            g.DrawLines(pen, points_down.ToArray());
            g.DrawLine(pen, points_top.Last(), points_down.Last());
            
            SolidBrush brush = new SolidBrush(Color.Black);
            g.FillPolygon(brush , points_top.ToArray());
            g.FillPolygon(brush, points_down.ToArray());
        }

        public void Draw(List<double> length, Control picture)
        {
            Graphics g = picture.CreateGraphics();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //g.Clear(Color.Transparent);
            Pen pen = new Pen(Color.Red);
            if (length.Count <= 1)
                return;

            g.DrawLines(pen, list_toArray(length, picture.Width , picture.Height).ToArray());

            SolidBrush brush = new SolidBrush(Color.Black);
            g.FillPolygon(brush, list_toArray(length, picture.Width , picture.Height).ToArray()); 
        }

        public Bitmap Draw(List<double> length, int width, int height)
        {
            Bitmap bmp = new Bitmap(width , height);
            Graphics dra = Graphics.FromImage(bmp);

            dra.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //g.Clear(Color.Transparent);
            Pen pen = new Pen(Color.Red);
            if (length.Count <= 1)
                return bmp;

            dra.DrawLines(pen, list_toArray(length, width, height).ToArray());

            SolidBrush brush = new SolidBrush(Color.DimGray);
            data = list_toArray(length, width, height).ToArray();
            dra.FillPolygon(brush, data);
            return bmp;
        }

        /// <summary>
        /// 参数说明：y1:绘制图像的y坐标
        ///           y2：label所在y坐标
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="direction"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <param name="bmp"></param>
        public Bitmap Draw_Arrow(int x1 , int direction , int y1 , int y2 , Image bmp)
        {
            GC.Collect();
            Bitmap BMP = new Bitmap(bmp);
            Graphics draw_arr = Graphics.FromImage(BMP);
            Pen p = new Pen(Color.LightBlue);
            System.Drawing.Drawing2D.AdjustableArrowCap lineCap = new System.Drawing.Drawing2D.AdjustableArrowCap(4, 4, true);
            if (direction == 0)
            {
                p.CustomStartCap = lineCap;
                draw_arr.DrawLine(p, x1, y1, x1, y2);
            }
            else
            {
                if (Math.Abs(x1 - direction) < 80)
                {
                    //画两条竖直的直线
                    draw_arr.DrawLine(p, x1, y1, x1, y2);
                    draw_arr.DrawLine(p, direction, y1, direction, y2);

                    if (x1 < direction)
                    { //画两个带箭头的线
                        p.CustomEndCap = lineCap;
                        draw_arr.DrawLine(p, x1 - 20, y2 - 15, x1, y2 - 15);
                        p = new Pen(Color.LightBlue);
                        p.CustomStartCap = lineCap;
                        draw_arr.DrawLine(p, direction , y2 - 15, direction + 20, y2 - 15);
                    }
                    else
                    {
                        p.CustomStartCap = lineCap;
                        draw_arr.DrawLine(p, x1 , y2 - 15, x1 + 20, y2 - 15);
                        p = new Pen(Color.LightBlue);
                        p.CustomEndCap = lineCap;
                        draw_arr.DrawLine(p, direction - 20, y2 - 15, direction, y2 - 15);
                    }

                    //画连接两竖线的虚线
                    p = new Pen(Color.LightBlue);
                    p.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
                    p.DashPattern = new float[] { (float)2.5, (float)2.5 };
                    draw_arr.DrawLine(p, x1, y2 - 15, direction, y2 - 15);
                }
                else
                {
                    //画两条竖直的直线
                    draw_arr.DrawLine(p, x1, y1, x1, y2);
                    draw_arr.DrawLine(p, direction, y1, direction, y2);

                    //画连接两竖线的虚线,带箭头
                    p.CustomStartCap = lineCap;
                    p.CustomEndCap = lineCap;
                    p.DashPattern = new float[] { (float)2.5, (float)2.5 };
                    //p.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                    draw_arr.DrawLine(p, x1, y2 - 15, direction, y2 - 15);
                }
            }
            draw_arr.Dispose();
            return BMP;
        }

        public void Clear(Control picture)
        {
            Graphics c = picture.CreateGraphics();
            c.Clear(picture.BackColor);
            c.Dispose();
            GC.Collect();
        }

        public List<Point> trans(List<double> length)
        {
            List<Point> t = new List<Point>();
            double[] temp = length.ToArray();
            for (int i = 0; i < temp.Length; i++)
            {
                Point k = new Point(Width / 18 + 2*i, (int)(Height / 2 - temp[i]*Height/100));
                t.Add(k);
            }
            return t;
        }
        /// <summary>
        /// 传入完整list，转化为picturebox上坐标用于绘图
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public List<Point> list_toArray(List<double> length  , int width , int height)
        {
            List<Point> t = new List<Point>();
            double[] temp = length.ToArray();

            //根据最大长度和控件尺寸确定坐标系最小刻度
            double Y = (int)(length.Max() * 2);
            double X = (int)(length.Count * 1.05);
            double ratio_x = width / X;
            double ratio_y = height / Y;

            double youyi = width * 0.02;
            if (youyi > 30)
                youyi = 20;

            for (int i = 0; i < temp.Length; i++)
            {
                Point k = new Point((int)(ratio_x * i + youyi), (int)((height + temp[i] * ratio_y) / 2));
                t.Add(k);
            }
            for (int i = length.Count; i > 0; i--)
            {
                Point k = new Point((int)(ratio_x * (i - 1) + youyi), (int)((height - temp[i - 1] * ratio_y) / 2));
                t.Add(k);
            }
            return t;
        }
        public int picrtureX_to_pictureY(List<double> length , int con_width , int con_heigth ,int x , bool top)
        {
            double[] temp = length.ToArray(); int res = 0;

            //根据最大长度和控件尺寸确定坐标系最小刻度
            double Y = (int)(length.Max() * 2);
            double X = (int)(length.Count * 1.05);
            double ratio_x = con_width / X;
            double ratio_y = con_heigth / Y;

            double youyi = con_width * 0.02;
            if (!top)
            {
                res = (int)((con_heigth + temp[x] * ratio_y) / 2);
            }
            else
            {
                res = (int)((con_heigth - temp[x - 1] * ratio_y) / 2);
            }
            return res;
        }
        public List<Point> imversion(List<Point> temp)
        {
            List<Point> m = new List<Point>();
            Point[] t = temp.ToArray();
            for (int i = 0; i < temp.Count; i++)
            {
                Point k = new Point(t[i].X, Height - t[i].Y);
                m.Add(k);
            }
            return m;
        }


        public Bitmap Dral_lines(List<double> data , int img_width , int img_height , double ave , double danwei)
        {
            Bitmap bmp = new Bitmap(img_width, img_height);
            Graphics dra = Graphics.FromImage(bmp);
            dra.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //g.Clear(Color.Transparent);
            Pen pen = new Pen(Color.Red);
            if (data.Count <= 1)
                return bmp;

            int zhongxin = img_height / 2;

            double x_coor_width = img_width / 540;

            List<Point> dat = new List<Point>();
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i] < (ave / 2))
                    continue;
                Point temp = new Point((int)(i * x_coor_width) , (int)((data[i] - ave) / danwei + zhongxin));
                dat.Add(temp);
            }
            dra.DrawLines(pen, dat.ToArray());
            return bmp;
        }
        ///误差计算
        public double average(List<double> Data)
        {
            return Data.Average();
        }

        public double[] residual(List<double> Data)
        {
            double ave = Data.Average();
            double[] temp = new double[Data.Count];
            for (int j = 0; j < Data.Count; j++)
            {
                temp[j] = Data[j] - ave;
            }
            return temp;
        }

        /// <summary>
        /// 求取残差平方和
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public double que_sum(double[] data)
        {
            double res = 0.0;
            for (int i = 0; i < data.Length; i++)
            {
                res = res + Math.Pow(data[i], 2);
            }
            return res;
        }

        /// <summary>
        /// 求残差的模的和
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public double sum_residual(List<double> Data)
        {
            double average = Data.Average();
            List<double> temp = Data;
            for (int i = 0; i < Data.Count; i++)
            {
                temp[i] = Math.Abs(Data[i] - average);
            }
            return temp.Sum();
        }

        /// <summary>
        /// 求标准差，默认贝叶斯公式求（时间长）、0为别捷尔公式、1为极差法（n《20内可用）、2为最大误差法（n《10可用）
        /// </summary>
        public double deviation(List<double> Data, int method)
        {
            double res = -1.0;
            switch (method)
            {
                case 0:
                    res = 1.253 * sum_residual(Data) / Math.Sqrt(Data.Count * (Data.Count - 1));
                    return res;

                case 1:
                    if (Data.Count > 20)
                        return res = -2.0;
                    else
                        res = (Data.Max() - Data.Min()) / Dn[Data.Count];
                    return res;

                case 2:
                    if (Data.Count > 10)
                        return res = -2.0;
                    else
                        res = Math.Abs(residual(Data).Max()) / Kn[Data.Count];
                    return res;

                default:
                    res = Math.Sqrt(que_sum(residual(Data)) / (Data.Count - 1));
                    return res;
            }
        }

        private double[] Dn = { 0, 0, 1.13, 1.69, 2.33, 2.53, 2.70, 2.85, 2.97, 3.08, 3.17, 3.26, 3.34, 3.47, 3.59, 3.64, 3.69, 3.74 };/// <summary>
                                                                                                                                       /// 极差法求标准差，σ = 极差 / Dn
                                                                                                                                       /// </summary>
        private double[] Kn = { 0, 0, 1.77, 1.02, 0.83, 0.74, 0.68, 0.64, 0.61, 0.59, 0.57 };//最大误差法求标准差，σ = 残差（最大） * Kn
    }
}
