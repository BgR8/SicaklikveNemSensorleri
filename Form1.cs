using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace DHT22Sensor
{
    public partial class Form1 : Form
    {
        double temperature = 0, humidity = 0;
        bool updateData = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            buton_ac.Enabled = true;
            buton_kapat.Enabled = false;

            chart1.Series["Sıcaklık"].Points.AddXY(1, 1);
            chart1.Series["Nem"].Points.AddXY(1, 1);
        }

        private void comboBox_portLists_DropDown(object sender, EventArgs e)
        {
            string[] portLists = SerialPort.GetPortNames();
            comboBox_portLists.Items.Clear();
            comboBox_portLists.Items.AddRange(portLists);
        }

        private void buton_ac_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = comboBox_portLists.Text;
                serialPort1.BaudRate = Convert.ToInt32(comboBox_baudRate.Text);
                serialPort1.Open();

                buton_ac.Enabled = false;
                buton_kapat.Enabled = true;

                comboBox_portLists.Enabled = false;
                comboBox_baudRate.Enabled = false;

                chart1.Series["Sıcaklık"].Points.Clear();
                chart1.Series["Nem"].Points.Clear();

                MessageBox.Show("Arduino'ya bağlantı başarılı");
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void buton_kapat_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Close();

                buton_ac.Enabled = true;
                buton_kapat.Enabled = false;

                comboBox_portLists.Enabled = true;
                comboBox_baudRate.Enabled = true;

                chart1.Series["Sıcaklık"].Points.Clear();
                chart1.Series["Nem"].Points.Clear();

                MessageBox.Show("Arduino'ya bağlantı kapatıldı.");
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                serialPort1.Close();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort1.ReadTo("\n");
            Data_Parsing(data);
            this.BeginInvoke(new EventHandler(Show_Data));
        }

        private void Show_Data(object sender, EventArgs e)
        {
            if (updateData == true)
            {
                label_temperature.Text = string.Format("Sıcaklık = {0}°C", temperature.ToString());
                label_humidity.Text = string.Format("Nem = {0}%RH", humidity.ToString());

                chart1.Series["Temperature"].Points.Add(temperature);
                chart1.Series["Humidity"].Points.Add(humidity);
            }
        }

        private void Data_Parsing(string data)
        {
            sbyte indexOf_startDataCharacter = (sbyte)data.IndexOf("@");
            sbyte indexOfA = (sbyte)data.IndexOf("A");
            sbyte indexOfB = (sbyte)data.IndexOf("B");

            if(indexOfA!=-1 && indexOfB!=-1 && indexOf_startDataCharacter!=-1)
            {
                try
                {
                    string str_temperature = data.Substring(indexOf_startDataCharacter+1, (indexOfA - indexOf_startDataCharacter) - 1);
                    string str_humidty = data.Substring(indexOfA + 1, (indexOfB-indexOfA)-1 );

                    temperature = Convert.ToDouble(str_temperature);
                    humidity = Convert.ToDouble(str_humidty);

                    updateData = true;


                }
                catch (Exception)
                {

                }
            }
            else
            {
                updateData = false;
            }
        }
    }
}
