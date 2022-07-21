using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComPort
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] sComPort = SerialPort.GetPortNames();

            Array.Sort(sComPort);

            cBoxComPort.Items.AddRange(sComPort);
            cBoxComPort.SelectedIndex = 0;

            string[] sBaudRate = { "9600", "14400", "19200", "38400", "57600", "115200", "128000" };

            cBoxBaudRate.Items.AddRange(sBaudRate);
            cBoxBaudRate.SelectedIndex = 0;

            string[] sDataBits = { "5", "6", "7", "8" };
            cBoxDataBits.Items.AddRange(sDataBits);
            cBoxDataBits.SelectedIndex = 0;

            string[] sStopBits = { "One", "Two" };
            cBoxStopBits.Items.AddRange(sStopBits);
            cBoxStopBits.SelectedIndex = 0;

            string[] sParityBits = { "None", "Odd", "Even" };
            cBoxParityBits.Items.AddRange(sParityBits);
            cBoxParityBits.SelectedIndex = 0;

            btnOpen.Enabled = true;
            btnClose.Enabled = false;

            chBoxDtrEnable.Checked = false;
            serialPort1.DtrEnable = false;

            chBoxRtsEnable.Checked = false;
            serialPort1.RtsEnable = false;

            btnSendData.Enabled = false;

            chBoxWriteLine.Checked = false;
            chBoxWrite.Checked = true;
            sendWith = "Write";

            lblStatusCom.Text = "OFF";
            lblStatusCom.ForeColor = Color.Red;

            chBoxAlwaysUpdate.Checked = true;
            chBoxAddToOldData.Checked = false;


        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = cBoxComPort.Text;
                serialPort1.BaudRate = Convert.ToInt32(cBoxBaudRate.Text);
                serialPort1.DataBits = Convert.ToInt32(cBoxDataBits.Text);
                serialPort1.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cBoxStopBits.Text);
                serialPort1.Parity = (Parity)Enum.Parse(typeof(Parity), cBoxParityBits.Text);

                serialPort1.Open();

                if (serialPort1.IsOpen)
                {
                    progressBar1.Value = 100;
                    tBoxLog.AppendText($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} :: PortName :: {serialPort1.PortName} /./ Connection Success~!");
                    tBoxLog.AppendText(Environment.NewLine);

                    btnOpen.Enabled = false;
                    btnClose.Enabled = true;

                    lblStatusCom.Text = "ON";
                    lblStatusCom.ForeColor = Color.Blue;
                }

            }
            catch (Exception ex)
            {
                //MessageBox.Show($"{ex}");
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                btnOpen.Enabled = true;
                btnClose.Enabled = false;

                lblStatusCom.Text = "OFF";
                lblStatusCom.ForeColor = Color.Red;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();

                if (!serialPort1.IsOpen)
                {
                    progressBar1.Value = 0;
                    tBoxLog.AppendText($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} :: PortName :: {serialPort1.PortName} /./ DisConnection Success~!");
                    tBoxLog.AppendText(Environment.NewLine);

                    btnOpen.Enabled = true;
                    btnClose.Enabled = false;

                    lblStatusCom.Text = "OFF";
                    lblStatusCom.ForeColor = Color.Red;
                }
            }
        }

        string dataOUT;
        private void btnSendData_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                dataOUT = tBoxDataOut.Text;

                //byte[] sByte = Encoding.ASCII.GetBytes($"{dataOUT}");
                //serialPort1.Write(sByte, 0, sByte.Length);

                //serialPort1.WriteLine(dataOUT);

                //if (sendWith == "WriteLine")
                if (chBoxWriteLine.Checked)
                {
                    serialPort1.WriteLine(dataOUT);
                }
                else if (chBoxWrite.Checked)
                //else if (sendWith == "Write")
                {
                    serialPort1.Write(dataOUT);
                }
            }
        }

        private void chBoxDTREnable_CheckedChanged(object sender, EventArgs e)
        {
            serialPort1.DtrEnable = chBoxDtrEnable.Checked;

            if (serialPort1.DtrEnable)
            {
                MessageBox.Show("DTR Enable", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            tBoxLog.AppendText($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} :: DtrEnable :: {serialPort1.DtrEnable}");
            tBoxLog.AppendText(Environment.NewLine);
            tBoxLog.ScrollToCaret();
        }

        private void chBoxRtsEnable_CheckedChanged(object sender, EventArgs e)
        {
            serialPort1.RtsEnable = chBoxRtsEnable.Checked;

            if (serialPort1.RtsEnable)
            {
                MessageBox.Show("RTS Enable", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            tBoxLog.AppendText($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} :: RtsEnable ::{serialPort1.RtsEnable}");
            tBoxLog.AppendText(Environment.NewLine);
            tBoxLog.ScrollToCaret();
        }

        private void btnClearDataOut_Click(object sender, EventArgs e)
        {
            if (tBoxDataOut.Text != "")
            {
                tBoxDataOut.Text = "";
            }
        }

        private void tBoxDataOut_KeyDown(object sender, KeyEventArgs e)
        {
            if (chBoxUsingEnter.Checked)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (serialPort1.IsOpen)
                    {
                        dataOUT = tBoxDataOut.Text;
                        //serialPort1.Write(dataOUT);

                        //if (sendWith == "WriteLine")
                        if (chBoxWriteLine.Checked)
                        {
                            serialPort1.WriteLine(dataOUT);
                        }
                        else if (chBoxWrite.Checked)
                        //else if (sendWith == "Write")
                        {
                            serialPort1.Write(dataOUT);
                        }
                    }
                }
            }
        }

        private void tBoxDataOut_TextChanged(object sender, EventArgs e)
        {
            int dataOUTLength = tBoxDataOut.TextLength;
            lblDataOutLength.Text = string.Format("{0:00}", dataOUTLength);

            if (chBoxUsingEnter.Checked)
            {
                tBoxDataOut.Text = tBoxDataOut.Text.Replace(Environment.NewLine, "");
            }
        }

        private void chBoxUsingButton_CheckedChanged(object sender, EventArgs e)
        {
            btnSendData.Enabled = chBoxUsingButton.Checked;
            tBoxDataOut.Focus();
        }

        private void chBoxUsingEnter_CheckedChanged(object sender, EventArgs e)
        {

            tBoxDataOut.Focus();
        }

        string sendWith;
        private void chBoxWriteLine_CheckedChanged(object sender, EventArgs e)
        {
            if (chBoxWriteLine.Checked)
            {
                sendWith = "WriteLine";
                chBoxWrite.Checked = false;
                chBoxWriteLine.Checked = true;
            }
        }

        private void chBoxWrite_CheckedChanged(object sender, EventArgs e)
        {
            if (chBoxWrite.Checked)
            {
                sendWith = "Write";
                chBoxWrite.Checked = true;
                chBoxWriteLine.Checked = false;
            }
        }

        string dataIN;

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            dataIN = serialPort1.ReadExisting();
            this.Invoke(new EventHandler(ShowData));
        }

        private void ShowData(object sender, EventArgs e)
        {
            int dataINLength = dataIN.Length;
            lblDataInLength.Text = string.Format("{0:00}", dataINLength);

            if (chBoxAlwaysUpdate.Checked)
            {
                tBoxDataIN.Text = dataIN;
            }
            else if (chBoxAddToOldData.Checked)
            {
                tBoxDataIN.Text += dataIN;
            }
        }

        private void chBoxAlwaysUpdate_CheckedChanged(object sender, EventArgs e)
        {
            if (chBoxAlwaysUpdate.Checked)
            {
                chBoxAlwaysUpdate.Checked = true;
                chBoxAddToOldData.Checked = false;
            }
            else
            {
                chBoxAddToOldData.Checked = true;
            }
        }

        private void chBoxBoxAddToOldData_CheckedChanged(object sender, EventArgs e)
        {
            if (chBoxAddToOldData.Checked)
            {
                chBoxAlwaysUpdate.Checked = false;
                chBoxAddToOldData.Checked = true;
            }
            else
            {
                chBoxAlwaysUpdate.Checked = true;
            }
        }

        private void btnClearDataIN_Click(object sender, EventArgs e)
        {
            if (tBoxDataIN.Text != "")

            {
                tBoxDataIN.Text = "";
            }
                
        }
    }
}

