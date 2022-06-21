using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Timers;

namespace Synoxo.USBHidDevice
{
    public partial class Form1 : Form
    {
        #region 变量定义
        /// <summary>
        /// HID对象
        /// </summary>
        private DeviceManagement MyDeviceManagement = new DeviceManagement();
        private static System.Timers.Timer tmrReadTimeout;
        private static System.Timers.Timer tmrContinuousDataCollect;
    

        // 供异步线程调用显示过程信息
        private delegate void MarshalToForm(String action, String textToAdd);
        #endregion 变量定义

        #region 构造函数
        public Form1()
        {
            InitializeComponent();
        }
        #endregion 构造函数

        #region 函数定义
        ///  <summary>
        ///  Perform actions that must execute when the program starts.
        ///  </summary>
        private void Startup()
        {
            try
            {
                try
                {
                    //  在没有连接到HID设备前，不允许用户修改BuffSize
                    cmdInputReportBufferSize.Focus();
                    cmdInputReportBufferSize.Enabled = false;

                    if (DeviceManagement.IsWindowsXpOrLater())
                    {
                        chkUseControlTransfersOnly.Enabled = true;
                    }
                    else
                    {
                        // 早期Windows XP系统，不允许
                        //  disable the option to force Input and Output reports to use control transfers.
                        chkUseControlTransfersOnly.Enabled = false;
                    }
                }
                catch (Exception ex)
                {
                this.richTextBox_Msg.Text += ex.Message;
                }

                tmrContinuousDataCollect = new System.Timers.Timer(100);
                tmrContinuousDataCollect.Elapsed += new ElapsedEventHandler(OnDataCollect);
                tmrContinuousDataCollect.Stop();
                tmrContinuousDataCollect.SynchronizingObject = this;

                tmrReadTimeout = new System.Timers.Timer(10000);
                tmrReadTimeout.Elapsed += new ElapsedEventHandler(OnReadTimeout);
                tmrReadTimeout.SynchronizingObject = this;
                tmrReadTimeout.Stop();

                //  Default USB Vendor ID and Product ID:
                //this.MyDeviceManagement.RegisterForDeviceNotifications(
                txtVendorID.Text = "0x0683";
                txtProductID.Text = "0x5850";

                this.MyDeviceManagement.WhenUsbEvent += new DeviceManagement.usbEventsHandler(MyDeviceManagement_WhenUsbEvent);
            }
            catch (Exception ex)
            {
                this.richTextBox_Msg.Text += ex.Message;
            }
        }

        void MyDeviceManagement_WhenUsbEvent(object sender, USBEventArgs e)
        {
            string statur = "";
            switch (e.Status)
            {
                case USBDeviceStateEnum.Put_In:
                    statur = "--插入";
                    break;
                case USBDeviceStateEnum.Put_Out:
                    statur = "--拔出";
                    break;
            }
            if (this.MyDeviceManagement.DeviceCount > 0)
                this.richTextBox_Msg.Text += this.MyDeviceManagement[e.DeviceIndex].myDevicePathName + this.MyDeviceManagement[e.DeviceIndex].HidUsage.ToString() + statur + "\r\n";
        }

        ///  <summary>
        ///  用VID和PID查找HID设备
        ///  </summary>
        ///  <returns>
        ///   True： 找到设备
        ///  </returns>
        private Boolean FindTheHid()
        {
            int myVendorID = 0x03EB;
            int myProductID = 0x2013;
            if (this.txtVendorID.Text != null && this.txtProductID.Text != null)
            {
                int vid = 0;
                int pid = 0;
                try
                {
                    vid = Convert.ToInt32(this.txtVendorID.Text, 16);
                    pid = Convert.ToInt32(this.txtProductID.Text, 16);
                    myVendorID = vid;
                    myProductID = pid;
                }
                catch (SystemException e)
                {
                    this.richTextBox_Msg.Text += e.Message;
                }
            }
            return this.MyDeviceManagement.findHidDevices(ref myVendorID, ref myProductID);//, this);
        }

        /// <summary>
        /// 刷新设备列表
        /// </summary>
        private void RefreshDeviceList()
        {
            if (this.MyDeviceManagement.DeviceCount > 0)
            {
                this.comboBox_DeviceList.Items.Clear();
                for (int i = 0; i < this.MyDeviceManagement.DeviceCount; i++)
                {
                    this.comboBox_DeviceList.Items.Add(this.MyDeviceManagement[i].myDevicePathName.ToString());
                }
                this.comboBox_DeviceList.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 将给定的数组转换成符合格式要求的字符串
        /// </summary>
        /// <param name="data">给定的数组</param>
        /// <param name="startIndex">起始字节序号</param>
        /// <param name="length">转换长度</param>
        /// <param name="prefix">前缀，例如 0x </param>
        /// <param name="splitString">分隔符</param>
        /// <param name="fromBase">进制10或16,其它值一律按16进制处理</param>
        /// <returns>字符串</returns>
        public string BytesToString(byte[] data, int startIndex, int length, string prefix, string splitString, int fromBase)
        {
            string _return = "";
            if (data == null)
                return _return;
            for (int i = 0; i < length; i++)
            {
                if (startIndex + i < data.Length)
                {
                    switch (fromBase)
                    {
                        case 10:
                            _return += string.Format("{0}{1:d3}", prefix, data[i + startIndex]);
                            break;
                        default:
                            _return += string.Format("{0}{1:X2}", prefix, data[i + startIndex]);
                            break;
                    }
                    if (i < data.Length - 1)
                        _return += splitString;
                }
            }
            return _return;
        }

        /// <summary>
        /// 将给定的字符串按照给定的分隔符和进制转换成字节数组
        /// </summary>
        /// <param name="str">给定的字符串</param>
        /// <param name="splitString">分隔符</param>
        /// <param name="fromBase">给定的进制</param>
        /// <returns>转换后的字节数组</returns>
        public byte[] StringToBytes(string str, string[] splitString, int fromBase)
        {
            string[] strBytes = str.Split(splitString, StringSplitOptions.RemoveEmptyEntries);
            if (strBytes == null || strBytes.Length == 0)
                return null;
            byte[] _return = new byte[strBytes.Length];
            for (int i = 0; i < strBytes.Length; i++)
            {
                try
                {
                    _return[i] = Convert.ToByte(strBytes[i], fromBase);
                }
                catch (SystemException)
                {
                    MessageBox.Show("发现不可转换的字符串->" + strBytes[i]);
                }
            }
            return _return;
        }

        ///  <summary>
        ///  定义异步线程发送Msg函数
        ///  </summary>
        ///  <param name="action"> 命令标识 ID </param>
        ///  <param name="formText"> 命令内容. </param>
        private void AccessForm(String action, String formText)
        {
            if (this.richTextBox_Msg != null && !this.richTextBox_Msg.IsDisposed)
            {
                switch (action.ToUpper())
                {
                    case "CLEAR":
                        this.richTextBox_Msg.Clear();
                        break;
                    case "ADDLINE":
                        if (!formText.EndsWith("\n"))
                            formText += "\r\n";
                        this.richTextBox_Msg.Text += formText;
                        break;
                    case "ADD":
                        this.richTextBox_Msg.Text += formText;
                        break;
                }
            }
        }

        ///  <summary>
        ///  异步线程显示过程信息函数 
        ///  </summary>
        ///  <param name="action"> 命令字符串标识 </param>
        ///  <param name="textToDisplay"> 命令内容 </param>
        private void MyMarshalToForm(String action, String textToDisplay)
        {
            object[] args = { action, textToDisplay };
            MarshalToForm MarshalToFormDelegate = null;
            //  The AccessForm routine contains the code that accesses the form.
            MarshalToFormDelegate = new MarshalToForm(AccessForm);
            //  Execute AccessForm, passing the parameters in args.
            base.Invoke(MarshalToFormDelegate, args);
        }

        ///  <summary>
        ///  Called if the user changes the Vendor ID or Product ID in the text box.
        ///  </summary>
        private void DeviceHasChanged()
        {
            try
            {
                this.MyDeviceManagement.StopReceiveDeviceNotificationHandle();
                this.cmdInputReportBufferSize.Enabled = false;
            }
            catch (Exception ex)
            {
                this.richTextBox_Msg.Text += ex.Message + "\r\n";
                throw;
            }
        }
        ///  <summary>
        ///  搜集设备数据.
        ///  </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        ///  <remarks>
        ///  此计数器只有在 "连续发送" 模式下才会使能，实现连续传输
        ///  </remarks>
        private void OnDataCollect(object source, ElapsedEventArgs e)
        {
            try
            {
                if (this.MyDeviceManagement.TransferInProgress == false)
                {
                    ReadAndWriteToDevice();
                }
            }
            catch (Exception ex)
            {
                this.richTextBox_Msg.Text += ex.Message + "\r\n";
                throw;
            }
        }

        /// <summary>
        /// 系统超时计数器
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnReadTimeout(object source, ElapsedEventArgs e)
        {
            MyMarshalToForm("ADDLINE", "读取报告超时!");
            this.MyDeviceManagement.CloseCommunications();
            tmrReadTimeout.Stop();
            //  Enable requesting another transfer.
        }
		        

        ///  <summary>
        ///  Sends an Output report, then retrieves an Input report.
        ///  Assumes report ID = 0 for both reports.
        ///  </summary>
        private byte[] ReadAndWriteToDevice()
        {
            byte[] outdatas = new byte[16];
            outdatas[0] = 0x55;
            outdatas[1] = 0x2;
            outdatas[2] = 0x1;
            outdatas[3] = 0x00;
            byte[] inputdatas = new byte[16];
            byte[] inputs = this.StringToBytes(this.richTextBox_bytesToSend.Text, new string[] { ",", " " }, 16);
            if (inputs != null && inputs.Length > 0)
            {
                outdatas = inputs;
            }
            //this.txtBytesReceived.Clear();
            //Application.DoEvents();
            if (this.optInputOutput.Checked)
                this.MyDeviceManagement.InputAndOutputReports(0, this.chkUseControlTransfersOnly.Checked, outdatas, ref inputdatas, (int)this.numericUpDown_TimerOut.Value);
            else
                this.MyDeviceManagement.InputAndOutputFeatureReports(0, outdatas, ref inputdatas);
            this.txtBytesReceived.Clear();
            Application.DoEvents();
            this.txtBytesReceived.Text = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
            return inputdatas;
        }         

        #endregion 函数定义

        #region 事件响应函数定义
        private void cmdContinuous_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmdContinuous.Text == "连续发送")
                {
                    //  Start doing periodic transfers.
                    if (!(cmdOnce.Enabled))
                    {
                        AccessForm("AddItemToListBox", "A previous transfer hasn't completed. Please try again.");
                    }
                    else
                    {
                        cmdOnce.Enabled = false;
                        //  Change the command button's text to "Cancel Continuous"
                        cmdContinuous.Text = "停止发送";
                        //  Enable the timer event to trigger a set of transfers.
                        tmrContinuousDataCollect.Enabled = true;
                        tmrContinuousDataCollect.Start();
                        ReadAndWriteToDevice();
                    }
                }
                else
                {
                    //  Stop doing continuous transfers.
                    //  Change the command button's text to "Continuous"
                    cmdContinuous.Text = "连续发送";
                    // D isable the timer that triggers the transfers.
                    tmrContinuousDataCollect.Enabled = false;
                    tmrContinuousDataCollect.Stop();
                    cmdOnce.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                this.richTextBox_Msg.Text += ex.Message + "\r\n";
                throw;
            }
        }

        private void cmdInputReportBufferSize_Click(object sender, EventArgs e)
        {
            try
            {
                this.MyDeviceManagement.SetInputReportBufferSize(this.MyDeviceManagement[this.comboBox_DeviceList.SelectedIndex], Convert.ToInt32(this.txtInputReportBufferSize.Text));
            }
            catch (Exception ex)
            {
                this.richTextBox_Msg.Text += ex.Message + "\r\n";
                throw;
            }
        }

        private void cmdFindDevice_Click(object sender, EventArgs e)
        {
            try
            {
                if (FindTheHid())
                {
                    this.richTextBox_Msg.Text+="发现设备->\r\n";
                    for (int i = 0; i < this.MyDeviceManagement.DeviceCount; i++)
                    {
                        this.richTextBox_Msg.Text += this.MyDeviceManagement[i].myDevicePathName +"\r\n";
                    }
                    this.RefreshDeviceList();
                    this.cmdInputReportBufferSize.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                this.richTextBox_Msg.Text += ex.Message + "\r\n";
                throw;
            }
        }

        private void cmdOnce_Click(object sender, EventArgs e)
        {
            this.cmdOnce.Enabled = false;
            try
            {
                //  Don't allow another transfer request until this one completes.
                //  Move the focus away from cmdOnce to prevent the focus from 
                //  switching to the next control in the tab order on disabling the button.
                fraSendAndReceive.Focus();
                cmdOnce.Enabled = false;
                ReadAndWriteToDevice();
            }
            catch (Exception ex)
            {
                this.richTextBox_Msg.Text += ex.Message + "\r\n";
            }
            this.cmdOnce.Enabled = true;
        }

        private void txtVendorID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DeviceHasChanged();
            }
            catch (Exception ex)
            {
                this.richTextBox_Msg.Text += ex.Message + "\r\n";
                throw;
            }
        }

        private void txtProductID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DeviceHasChanged();
            }
            catch (Exception ex)
            {
                this.richTextBox_Msg.Text += ex.Message + "\r\n";
                throw;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                Startup();
            }
            catch (Exception ex)
            {
                this.richTextBox_Msg.Text += ex.Message + "\r\n";
                throw;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                this.MyDeviceManagement.ShutDown();
            }
            catch (Exception ex)
            {
                this.richTextBox_Msg.Text += ex.Message + "\r\n";
                throw;
            }
        }

        #endregion 事件响应函数定义

        private void button_Exit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

    }
}
