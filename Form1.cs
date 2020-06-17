using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace 拉力机
{
    public partial class Form1 : Form
    {
        public int x = 0;
        string[] dates = new string[2048];
        public Form1()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if(button1.Text == "运行")
            {
                groupBox1.Enabled = false;
                groupBox2.Enabled = false;
                groupBox4.Enabled = false;
                button1.Text = "停止";

                byte[] com = new byte[1] {0x00};
                if (radioButton1.Checked)
                    com[0] += 0x20;
                else if(radioButton2.Checked)
                    com[0] += 0x30;
                if (radioButton3.Checked)
                    com[0] += 0x01;
                else if (radioButton4.Checked)
                    com[0] += 0x02;
                else if (radioButton5.Checked)
                    com[0] += 0x03;
                try
                {
                    serialPort1.Write(com,0,1);
                }
                catch
                {
                    MessageBox.Show("串口字符写入错误", "错误");   //弹出发送错误对话框
                }
            }
            else
            {
                try
                {
                    byte[] stop = new byte[1] { 0xFF };
                    serialPort1.Write(stop, 0, 1);
                }
                catch
                {
                    MessageBox.Show("串口字符写入错误", "错误");   //弹出发送错误对话框
                }
                groupBox1.Enabled = true;
                groupBox2.Enabled = true;
                groupBox4.Enabled = true;
                button1.Text = "运行";
            }
             
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "运行")
            {
                groupBox5.Enabled = false;
                groupBox6.Enabled = false;
                groupBox3.Enabled = false;
                button2.Text = "停止";
                byte[] com = new byte[1] { 0x00 };
                if (radioButton7.Checked)
                    com[0] += 0x00;
                else if (radioButton6.Checked)
                    com[0] += 0x10;
                if (radioButton10.Checked)
                    com[0] += 0x01;
                else if (radioButton9.Checked)
                    com[0] += 0x02;
                else if (radioButton8.Checked)
                    com[0] += 0x03;
                try
                {
                    serialPort1.Write(com, 0, 1);
                }
                catch
                {
                    MessageBox.Show("串口字符写入错误", "错误");   //弹出发送错误对话框
                }
            }
            else
            {
                try
                {
                    byte[] stop = new byte[1] { 0xFF };
                    serialPort1.Write(stop, 0, 1);
                }
                catch
                {
                    MessageBox.Show("串口字符写入错误", "错误");   //弹出发送错误对话框
                }
                groupBox5.Enabled = true;
                groupBox6.Enabled = true;
                groupBox3.Enabled = true;
                button2.Text = "运行";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (button3.Text == "打开串口")
            {
                try
                {
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = 115200;//Convert.ToInt32(comboBox1.Text);
                    serialPort1.Open();

                    //comboBox1.Enabled = false;
                    comboBox1.Enabled = false;
                    button3.Text = "关闭串口";
                    groupBox3.Enabled = true;
                    groupBox4.Enabled = true;
                }
                catch
                {
                    MessageBox.Show("打开串口失败", "错误");
                }
            }
            else              //如果当前串口设备是打开状态
            {
                try
                {
                    serialPort1.Close();    //关闭串口
                    comboBox1.Enabled = true;   //串口已经关闭了，将comboBox2设置为可操作
                    button3.Text = "打开串口";    //将串口开关按键的文字改为  “打开串口”
                    groupBox3.Enabled = false;
                    groupBox4.Enabled = false;
                }
                catch
                {
                    MessageBox.Show("关闭串口失败", "错误");   //弹出错误对话框
                }

            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            Updata_Serialport_Name(comboBox1);
            //设置显示范围
            ChartArea chartArea = chart1.ChartAreas[0];
            chartArea.AxisX.Minimum = 0;
            chartArea.AxisX.Maximum = 10d;
            chartArea.AxisY.Minimum = 0d;
            chartArea.AxisY.Maximum = 50d;
            chartArea.AxisX.Title = "位移 MM";
            chartArea.AxisY.Title = "拉力 KG";
            Series series = chart1.Series[0];
            // 画样条曲线（Spline）
            series.ChartType = SeriesChartType.Line;
            // 线宽2个像素
            series.BorderWidth = 2;
            // 线的颜色：红色
            series.Color = System.Drawing.Color.Red;
            // 图示上的文字
        }

        private void Updata_Serialport_Name(ComboBox MycomboBox)
        {
            string[] portList = System.IO.Ports.SerialPort.GetPortNames();

            MycomboBox.Items.Clear();
            try
            {
                comboBox1.Text = portList[0];
                for (int i = 0; i < portList.Length; i++)
                {
                    MycomboBox.Items.Add(portList[i]);
                }
            }

            catch
            {
                comboBox1.Text = "NoCOM";
            }
        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            Updata_Serialport_Name(comboBox1);
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            string str = serialPort1.ReadExisting();   //读取串口接收缓冲区字符串
            string[] parts = str.Split('*');
            string part1 = parts[0]; // distence
            string part2 = parts[1]; // presure
            dates[x++] = part1 + ','+part2;
            textBox1.AppendText("D:"+part1+"mm    P:"+ part2+"kg\r\n");             //在接收文本框中进行显示
            this.chart1.Series[0].Points.AddXY(double.Parse(part1), System.Math.Abs(double.Parse(part2)));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            this.chart1.Series[0].Points.Clear();
            x = 0;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {

                string fileName = Application.StartupPath + "\\" + "data" + ".csv";
                StreamWriter fileWriter = new StreamWriter(fileName, false, Encoding.ASCII);
                fileWriter.Write("Distence,Pressure\r\n");
                for (int j = 0; j < x; j++)
                {
                    fileWriter.Write("{0}" + "\r\n", dates[j]);
                }
                fileWriter.Flush();
                fileWriter.Close();
                MessageBox.Show("保存成功", "完成");
            }
            catch
            {
                MessageBox.Show("保存失败", "错误");
            }

        }
    }
}
