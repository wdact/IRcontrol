using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IRcontrol
{
    public partial class Form1 : Form
    {
        //я сделал
        public Form1()
        {
            InitializeComponent();
            FormClosed += Form1_FormClosed;
            
            try
            {
                port = new SerialPort("COM3");
                port.RtsEnable = true;
                port.DtrEnable = true;
                port.DataReceived += Port_DataReceived;
                port.Open();

                
            }
            catch (Exception ee)
            {
                MessageBox.Show("Оборудование не подключено!");
                Close();
            }

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (port != null && port.IsOpen)
                port.Close();
        }

        List<string> allCommands = new List<string>();
        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var port = (SerialPort)sender;
            var data = port.ReadExisting();
            switch(data.Trim())
            {
                case "Button1":
                    radioButton1.BeginInvoke(new Action(()=>radioButton1.Checked = true));
                    radioButton1_CheckedChanged(radioButton1, null);
                    button8_Click(null, null);
                    break;
                case "Button2":
                    radioButton2.BeginInvoke(new Action(() => radioButton2.Checked = true));
                    radioButton1_CheckedChanged(radioButton2, null);
                    button8_Click(null, null);
                    break;
                case "Button3":
                    radioButton3.BeginInvoke(new Action(() => radioButton3.Checked = true));
                    radioButton1_CheckedChanged(radioButton3, null);
                    button8_Click(null, null);
                    break;
                case "Button4":
                    radioButton4.BeginInvoke(new Action(() => radioButton4.Checked = true));
                    radioButton1_CheckedChanged(radioButton4, null);
                    button8_Click(null, null);
                    break;
                case "Button5":
                    radioButton5.BeginInvoke(new Action(() => radioButton5.Checked = true));
                    radioButton1_CheckedChanged(radioButton5, null);
                    button8_Click(null, null);
                    break;
                case "Button6":
                    radioButton6.BeginInvoke(new Action(() => radioButton6.Checked = true));
                    radioButton1_CheckedChanged(radioButton6, null);
                    button8_Click(null, null);
                    break;
            }
        }
        SerialPort port;

        private void button1_Click(object sender, EventArgs e)
        {
            if (port.IsOpen)
                port.WriteLine("p");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (port.IsOpen)
                port.WriteLine("G");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (port.IsOpen)
                port.WriteLine("H");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_lastChoosed)){
                MessageBox.Show("Сначала нужно выбрать назначаемую клавишу!");
                return;
            }
            if (port.IsOpen)
            {
                forWriteBase = null;
                port.DataReceived -= Port_DataReceived;
                port.DataReceived += RecieveForRecord;
                port.WriteLine("R");
            }
        }

        private string forWriteBase;
        private void RecieveForRecord(object sender, SerialDataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(forWriteBase))
                forWriteBase = _lastChoosed + "|";
            forWriteBase += port.ReadExisting().Trim()+" ";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (port.IsOpen)
            port.DataReceived -= RecieveForRecord;
            port.DataReceived += Port_DataReceived;
            port.WriteLine("Q");

           var allLines = File.ReadAllLines("BASE.txt").ToList();
            allLines.RemoveAll(f => f.Contains(_lastChoosed + "|"));
            allLines.Add(forWriteBase);
            File.WriteAllLines("BASE.txt", allLines);

        }

        private string _lastChoosed;
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            _lastChoosed = (sender as RadioButton).Text.Replace("Кнопка ", string.Empty);

        }

        private  async void button8_Click(object sender, EventArgs e)
        {
            if (port.IsOpen) 
            {
                
                        port.WriteLine("q");
                        var neededCommand = File.ReadAllLines("BASE.txt").FirstOrDefault(f => f.StartsWith($"{_lastChoosed}|"))?.Split('|').Last().Split(' ');
                if (neededCommand == null)
                {
                    port.WriteLine("228");////
                    return;
                }
                port.WriteLine("001");

                foreach (var f in neededCommand)
                        {
                            
                            port.WriteLine(f);
                            await Task.Delay(500);//
                }
                await Task.Delay(500);//
                port.WriteLine("228");
                        


            }
        }
    }
}
