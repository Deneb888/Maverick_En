using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.Runtime.InteropServices; 
using System.Windows.Forms;
using System.Threading;

namespace Synoxo.USBHidDevice
{
    public class DeviceInformation
    {
        public Boolean DeviceIsDetected;
        public String myDevicePathName;
        public HIDD_ATTRIBUTES DeviceAttributes;
        public HIDP_CAPS Capabilities;
        public IntPtr DeviceNotificationHandle;
        public int NumberOfInputBuffers;
        public Boolean ExclusiveAccess;
        public FileStream FileStreamDeviceData;
        public SafeFileHandle HidHandle;
        public String HidUsage;
    }

    public enum USBDeviceStateEnum
    {
        Put_In,
        Put_Out,
    }

    public class USBEventArgs : EventArgs
    {
        public int DeviceIndex;
        public USBDeviceStateEnum Status;
        public USBEventArgs(int index, USBDeviceStateEnum statue)
        {
            DeviceIndex = index;
            this.Status = statue;
        }
    }

    public partial class DeviceManagement : Control
    {
        #region 事件定义
        /// <summary>
        /// 定义USB插拔变化句柄
        /// </summary>
        /// <param name="sender">发送设备</param>
        /// <param name="e">事件</param>
        public delegate void usbEventsHandler(object sender, USBEventArgs e);

        /// <summary>
        /// 定义事件
        /// </summary>
        [field: NonSerialized()]
        public event usbEventsHandler WhenUsbEvent;
        #endregion 事件定义

        #region 变量定义
        private List<DeviceInformation> _deviceList = new List<DeviceInformation>();
        private Debugging MyDebugging = new Debugging(); //  For viewing results of API calls via Debug.Write.
        private Hid _hidObject = new Hid();
        private bool _transferInProgress = false;
        /// <summary>
        /// 当前的异步传输设备
        /// </summary>
        private DeviceInformation _nowAscDevice = null;
        /// <summary>
        /// 传输完成标志
        /// </summary>
        private bool _transferIsComplate = false;
        /// <summary>
        /// 使用的VID
        /// </summary>
        private int _deviceVID = 0x03EB;
        /// <summary>
        /// 设备的PID
        /// </summary>
        private int _devicePID = 0x2013;
        /// <summary>
        /// 查找设备正在忙碌中
        /// </summary>
        private bool _findHidIsBusy = false;
        /// <summary>
        /// 停止标志
        /// </summary>
        private bool _isStop = false;
        #endregion 变量定义

        #region 属性定义
        /// <summary>
        /// 按照排序索引变量
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DeviceInformation this[int index] { get { if (this._deviceList.Count > index) return this._deviceList[index]; else return null; } }
        /// <summary>
        /// 连接设备数量只读属性
        /// </summary>
        public int DeviceCount { get { if (this._deviceList != null) return this._deviceList.Count; else return 0; } }
        /// <summary>
        /// 按照路径名称索引变量
        /// </summary>
        /// <param name="deviceID"></param>
        /// <returns></returns>
        public DeviceInformation this[string deviceID]
        {
            get
            {
                for (int i = 0; i < this._deviceList.Count; i++)
                    if (this._deviceList[i].myDevicePathName.StartsWith(deviceID))
                        return this._deviceList[i];
                return null;
            }
        }

        public bool TransferInProgress { get { return this._transferInProgress; } set { this._transferInProgress = value; } }
        /// <summary>
        /// 停止标志
        /// </summary>
        public bool IsStop { get { return this._isStop; } set { this._isStop = value; } }
        #endregion 属性定义

        #region 构造函数
        /// <summary>
        /// 给定VID和PID的构造函数
        /// </summary>
        /// <param name="vid">VID</param>
        /// <param name="pid">PID</param>
        public DeviceManagement(int vid, int pid)
        {
            this._deviceVID = vid;
            this._devicePID = pid;
            this.findHidDevices();
        }

        public DeviceManagement() : this(0x03EB, 0x2013) { }
        #endregion 构造函数

        #region 函数定义
        /// <summary>
        /// 关闭给定的设备.
        /// </summary>
        private void CloseCommunications(DeviceInformation device)
        {
            if (device.FileStreamDeviceData != null)
            {
                device.FileStreamDeviceData.Close();
            }

            if ((device.HidHandle != null) && (!(device.HidHandle.IsInvalid)))
            {
                device.HidHandle.Close();
            }

            // The next attempt to communicate will get new handles and FileStreams.
            device.DeviceIsDetected = false;
            this._deviceList.Remove(device);
        }

        /// <summary>
        /// 关闭设备通讯
        /// </summary>
        public void CloseCommunications()
        {
            if (this._deviceList.Count > 0)
            {
                for (int i = 0; i < this._deviceList.Count; i++)
                {
                    this.CloseCommunications(this._deviceList[i]);
                }
            }
        }

        /// <summary>
        /// 停止接收设备信息
        /// </summary>
        public void StopReceiveDeviceNotificationHandle()
        {
            for (int i = 0; i < this._deviceList.Count; i++)
            {
                if (this._deviceList[i].DeviceNotificationHandle != IntPtr.Zero)
                {
                    this.StopReceivingDeviceNotifications(this._deviceList[i].DeviceNotificationHandle);
                    this._deviceList[i].DeviceNotificationHandle = IntPtr.Zero;
                }
            }
        }

        /// <summary>
        /// 关闭所有设备
        /// 停止设备接收信息，并关闭所有通讯
        /// </summary>
        public void ShutDown()
        {
            for (int i = 0; i < this._deviceList.Count; i++)
            {
                if (this._deviceList[i].DeviceIsDetected)
                {
                    this.StopReceivingDeviceNotifications(this._deviceList[i].DeviceNotificationHandle);
                }
            }
            this.CloseCommunications();
        }

        ///  <summary>
        /// 查询设备缓冲区大小 
        ///  </summary>
        public void GetInputReportBufferSize(DeviceInformation findDevice)
        {
            Int32 numberOfInputBuffers = 0;
            Boolean success;
            try
            {
                //  Get the number of input buffers.
                success = _hidObject.GetNumberOfInputBuffers(findDevice.HidHandle, ref findDevice.NumberOfInputBuffers);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
        }

        ///  <summary>
        ///  设置输入缓冲区大小Set the number of Input buffers (the number of Input reports 
        ///  the host will store) from the value in the text box.
        ///  </summary>
        public void SetInputReportBufferSize(DeviceInformation device, int numberOfInputBuffers)
        {
            try
            {
                //  Set the number of buffers.
                _hidObject.SetNumberOfInputBuffers(device.HidHandle, numberOfInputBuffers);

                //  Verify and display the result.
                this.GetInputReportBufferSize(device);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
        }

        ///  <summary>
        ///  根据给定的VendorID和ProductID查找设备，并将其事件注册到窗体.
        ///  </summary>
        ///  <returns>
        ///   探测到返回True，没有返回False
        ///  </returns>
        public Boolean findHidDevices(ref int myVendorID, ref int myProductID)//, Form FrmMy
        {
            Boolean deviceFound = false;
            String[] devicePathName = new String[128];
            String functionName = "";
            Guid hidGuid = Guid.Empty;
            Int32 memberIndex = 0;
            Boolean success = false;
            // 检查是否在重入
            if (this._findHidIsBusy == true)
                return false;
            else
                this._findHidIsBusy = true;

            if (this._deviceVID != myVendorID) this._deviceVID = myVendorID;
            if (this._devicePID != myVendorID) this._devicePID = myProductID;

            try
            {
                DeviceInformation device = new DeviceInformation();
                device.DeviceIsDetected = false;
                //  CloseCommunications();
                //  调用API 函数: 'HidD_GetHidGuid
                Hid.HidD_GetHidGuid(ref hidGuid);
                Debug.WriteLine("在函数FindTheHid中" + this.MyDebugging.ResultOfAPICall("HidD_GetHidGuid"));

                //  获取连接的HID路径名称数组 
                deviceFound = this.FindDeviceFromGuid(hidGuid, ref devicePathName);

                //  如果至少有一个HID，获取每个设备的Vendor ID，Product ID，直到发现一个或者所有设备
                if (deviceFound)
                {
                    memberIndex = 0;
                    do
                    {
                        // 调用API函数CreateFile
                        if (devicePathName[memberIndex] != null && this[devicePathName[memberIndex]] != null)
                        {
                            if (this[devicePathName[memberIndex]] != null && this[devicePathName[memberIndex]].DeviceIsDetected)
                            {
                                memberIndex++;
                                continue;   // 继续查找下一个设备
                            }
                            else
                            {
                                device = this[devicePathName[memberIndex]];
                            }
                        }

                        // 在不使用Read/write权限的情况下打开句柄，以获取HID的信息, 甚至系统键盘鼠标等
                        device.HidHandle = FileIO.CreateFile(devicePathName[memberIndex], 0, FileIO.FILE_SHARE_READ | FileIO.FILE_SHARE_WRITE, IntPtr.Zero, FileIO.OPEN_EXISTING, 0, 0);
                        Debug.WriteLine("在函数FindTheHid中" + this.MyDebugging.ResultOfAPICall("CreateFile"));

                        if (!device.HidHandle.IsInvalid)
                        {
                            //  返回的句柄是合法的，确定此设备是否我们要找的设备
                            //  设置DeviceAttributes中的数据字节结构
                            _hidObject.DeviceAttributes.Size = Marshal.SizeOf(_hidObject.DeviceAttributes);

                            //  调用API函数 HidD_GetAttributes
                            success = Hid.HidD_GetAttributes(device.HidHandle, ref _hidObject.DeviceAttributes);
                            Debug.WriteLine("在函数FindTheHid中" + this.MyDebugging.ResultOfAPICall("HidD_GetAttributes"));
                            if (success)
                            {
                                //Debug.WriteLine("  HIDD_ATTRIBUTES 结构被成功填充.");
                                //Debug.WriteLine("  结构大小: " + MyHid.DeviceAttributes.Size);
                                //Debug.WriteLine("  Vendor ID: " + Convert.ToString(MyHid.DeviceAttributes.VendorID, 16));
                                //Debug.WriteLine("  Product ID: " + Convert.ToString(MyHid.DeviceAttributes.ProductID, 16));
                                //Debug.WriteLine("  版本号: " + Convert.ToString(MyHid.DeviceAttributes.VersionNumber, 16));
                                //  确定是否我们要找的设备
                                if ((_hidObject.DeviceAttributes.VendorID == myVendorID) && (_hidObject.DeviceAttributes.ProductID == myProductID))
                                {
                                    //  窗口列表框中显示设备信息
                                    Debug.WriteLine(" 探测到设备:" + " Vendor ID = " + Convert.ToString(_hidObject.DeviceAttributes.VendorID, 16) + " Product ID = " + Convert.ToString(_hidObject.DeviceAttributes.ProductID, 16));

                                    device.DeviceIsDetected = true;

                                    // 为OnDeviceChange()函数保存设备路径名
                                    if (this[devicePathName[memberIndex]] == null)
                                    {
                                        device.myDevicePathName = devicePathName[memberIndex];
                                        device.DeviceAttributes = _hidObject.DeviceAttributes;
                                        this._deviceList.Add(device);
                                        device = new DeviceInformation();
                                        _hidObject = new Hid();
                                        device.DeviceIsDetected = false;
                                        Debug.WriteLine("将设备" + device.myDevicePathName + "加入到设备列表中");
                                    }
                                }
                                else
                                {
                                    //  如果不匹配关闭设备句柄
                                    device.DeviceIsDetected = false;
                                    device.HidHandle.Close();
                                }
                            }
                            else
                            {
                                //  提示获取设备信息出现问题.
                                Debug.WriteLine("Error in filling HIDD_ATTRIBUTES structure.");
                                device.DeviceIsDetected = false;
                                device.HidHandle.Close();
                            }
                        }

                        //  继续查找，直到我们发现给定设备或者检查完成
                        memberIndex = memberIndex + 1;
                    }
                    while (!((device.DeviceIsDetected || (memberIndex == devicePathName.Length))));
                }

                if (this._deviceList.Count > 0)
                {
                    for (int i = 0; i < this._deviceList.Count; i++)
                    {
                        DeviceInformation findDevice = this._deviceList[i];
                        if (findDevice.Capabilities.InputReportByteLength > 0)
                            continue;
                        // 如果探测到设备
                        // 注册关注设备事件  Register to receive notifications if the device is removed or attached.
                        success = this.RegisterForDeviceNotifications(findDevice.myDevicePathName, this.Handle/*FrmMy.Handle*/, hidGuid, ref findDevice.DeviceNotificationHandle);

                        Debug.WriteLine("RegisterForDeviceNotifications = " + success);

                        //  获取设备信息
                        findDevice.Capabilities = _hidObject.GetDeviceCapabilities(findDevice.HidHandle);

                        if (success)
                        {
                            //  确定是否鼠标或键盘设备
                            findDevice.HidUsage = _hidObject.GetHidUsage(_hidObject.Capabilities);

                            //  获取报告的缓冲区大小
                            GetInputReportBufferSize(findDevice);
                            this.ReOpenDeviceFileStreamHandler(findDevice);

                        }
                    }
                    for (int i = this._deviceList.Count - 1; i >= 0; i--)
                    {
                        if (!this._deviceList[i].DeviceIsDetected)
                            this._deviceList.RemoveAt(i);
                    }
                }
                else
                {
                    //  没有检测到设备
                    Debug.WriteLine("没发现设备.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            this._findHidIsBusy = false;
            return this._deviceList.Count > 0;
        }

        /// <summary>
        /// 查找HID设备
        /// </summary>
        /// <returns></returns>
        public Boolean findHidDevices()
        {
            return this.findHidDevices(ref this._deviceVID, ref this._devicePID);
        }

        /// <summary>
        /// 重新打开设备文件流
        /// </summary>
        /// <param name="findDevice"></param>
        private void ReOpenDeviceFileStreamHandler(DeviceInformation findDevice)
        {
            //关闭句柄并用读/写模式重新打开
            findDevice.HidHandle.Close();
            findDevice.HidHandle = FileIO.CreateFile(findDevice.myDevicePathName, FileIO.GENERIC_READ | FileIO.GENERIC_WRITE, FileIO.FILE_SHARE_READ | FileIO.FILE_SHARE_WRITE, IntPtr.Zero, FileIO.OPEN_EXISTING, 0, 0);
            Debug.WriteLine("在函数ReOpenDeviceFileStreamHandler中" + this.MyDebugging.ResultOfAPICall("CreateFile"));

            if (findDevice.HidHandle.IsInvalid)
            {
                findDevice.ExclusiveAccess = true;
                Debug.WriteLine("此设备是一个系统" + findDevice.HidUsage + "." + "对于此设备，Windows 2000 和 Windows XP 才能使用 Input 和 Output reports功能.");
            }
            else
            {
                if (findDevice.Capabilities.InputReportByteLength > 0)
                {
                    //  Set the size of the Input report buffer. 
                    Byte[] inputReportBuffer = null;

                    inputReportBuffer = new Byte[findDevice.Capabilities.InputReportByteLength];
                    findDevice.FileStreamDeviceData = new FileStream(findDevice.HidHandle, FileAccess.Read | FileAccess.Write, inputReportBuffer.Length, false);
                }

                //  Flush any waiting reports in the input buffer. (optional)
                _hidObject.FlushQueue(findDevice.HidHandle);
            }
        }

        ///  <summary>
        ///  Called when a WM_DEVICECHANGE message has arrived,
        ///  indicating that a device has been attached or removed.
        ///  </summary>
        ///  <param name="m"> a message with information about the device </param>
        public void OnDeviceChange(Message m)
        {
            Debug.WriteLine("WM_DEVICECHANGE");
            try
            {
                if ((m.WParam.ToInt32() == DeviceManagement.DBT_DEVICEARRIVAL))
                {
                    //  如果 WParam 中包含 DBT_DEVICEARRIVAL, 设备被连接上.
                    Debug.WriteLine("A device has been attached.");
                    bool findDevice = false;
                    // 依次查找是哪一个设备连接上了
                    for (int i = 0; i < this._deviceList.Count; i++)
                    {
                        DeviceInformation device = this._deviceList[i];
                        //  验证是否正在通讯的设备
                        if (this.DeviceNameMatch(m, device.myDevicePathName))
                        {
                            device.DeviceIsDetected = true;
                            this.ReOpenDeviceFileStreamHandler(device);
                            Debug.WriteLine("探测到设备连接：" + device.myDevicePathName);
                            findDevice = true;
                            if (this.WhenUsbEvent != null && this[i]!=null)
                            {
                                this.WhenUsbEvent(this[i], new USBEventArgs(i, USBDeviceStateEnum.Put_In));
                            }
                            break;
                        }
                    }
                    if (!findDevice)
                    {
                        this.findHidDevices(ref this._deviceVID, ref this._devicePID);
                    }
                }
                else if ((m.WParam.ToInt32() == DeviceManagement.DBT_DEVICEREMOVECOMPLETE))
                {
                    // 入托 WParam 中包含 DBT_DEVICEREMOVAL，有设备被移除
                    Debug.WriteLine("A device has been removed.");
                    // 验证是否我们正在通讯的设备
                    for (int i = 0; i < this._deviceList.Count; i++)
                    {
                        DeviceInformation device = this._deviceList[i];
                        if (this.DeviceNameMatch(m, device.myDevicePathName))
                        {
                            Debug.WriteLine("探测到我们的设备被移除：" + device.myDevicePathName);
                            //  设置 MyDeviceDetected 为假，阻止下一次数据传输,
                            //  再次调用 FindTheHid()以便查找设备并重新获得设备句柄
                            device.DeviceIsDetected = false;
                            device.HidHandle.Close();
                            if (this.WhenUsbEvent != null && this[i]!=null)
                            {
                                this.WhenUsbEvent(this[i], new USBEventArgs(i, USBDeviceStateEnum.Put_Out));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 从窗体的消息中获取USB设备消息
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            //  The OnDeviceChange routine processes WM_DEVICECHANGE messages.
            if (m.Msg == DeviceManagement.WM_DEVICECHANGE)
            {
                OnDeviceChange(m);
            }
            base.WndProc(ref m);
        }

        ///  <summary>
        ///  Retrieves Input report data and status information.
        ///  This routine is called automatically when myInputReport.Read
        ///  returns. Calls several marshaling routines to access the main form.
        ///  </summary>
        ///  <param name="ar"> an object containing status information about 
        ///  the asynchronous operation. </param>
        private void GetInputReportData(IAsyncResult ar)
        {
            String byteValue = "";
            Int32 count = 0;
            Byte[] inputReportBuffer = null;

            if (_nowAscDevice != null)
            {
                try
                {
                    inputReportBuffer = (byte[])ar.AsyncState;

                    _nowAscDevice.FileStreamDeviceData.EndRead(ar);
                    if ((ar.IsCompleted))
                    {
                        Debug.WriteLine("An Input report has been read.");
                        Debug.WriteLine(" Input Report ID: " + String.Format("{0:X2} ", inputReportBuffer[0]));
                        Debug.WriteLine(" Input Report Data:");

                        for (count = 0; count <= inputReportBuffer.Length - 1; count++)
                        {
                            byteValue += String.Format(" {0:X2} ", inputReportBuffer[count]);
                        }
                        Debug.WriteLine(byteValue);
                        _transferIsComplate = true;
                    }
                    else
                    {
                        Debug.WriteLine("读取输入报告失败！");
                        _transferIsComplate = false;
                    }
                    _transferInProgress = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    _transferIsComplate = false;
                }
            }
        }

        /// <summary>
        /// 将报告数组发送到指定序号的设备
        /// </summary>
        /// <param name="index">设备序号</param>
        /// <param name="useControlTransfersOnly">true:当设备有中断输入节点时使用，false:异步输入使用</param>
        /// <param name="outDatas"></param>
        /// <returns></returns>
        public bool WriteReportToDevice(int index, bool useControlTransfersOnly, byte[] outDatas)
        {
            Byte[] outputReportBuffer = null;
            Boolean success = false;
            DeviceInformation device = this[index];
            this._isStop = false;
            try
            {
                success = false;

                //  验证设备句柄的合法性，否则不能发送报告
                //  (as for a mouse or keyboard under Windows 2000/XP.)
                if (device!=null && device.DeviceIsDetected && !device.HidHandle.IsInvalid)
                {
                    //  如果设备没有Output报告，也不能发送
                    if (device.Capabilities.OutputReportByteLength > 0)
                    {
                        //  设置输出缓冲区大小   
                        outputReportBuffer = new Byte[device.Capabilities.OutputReportByteLength];

                        int packgeLength = outputReportBuffer.Length - 1;
                        int packgeCount = outDatas.Length / packgeLength;
                        if (outDatas.Length % (outputReportBuffer.Length - 1) != 0)
                            packgeCount++;
                        for (int i = 0; i < packgeCount; i++)
                        {
                            //  在缓冲区的第一个字节设置报告的标识
                            outputReportBuffer[0] = (byte)i;
                            //  将发送数据拷贝到缓冲区
                            if ((outDatas.Length - i * packgeLength) > packgeLength)
                                Array.Copy(outDatas, i * packgeLength, outputReportBuffer, 1, packgeLength);
                            else
                                Array.Copy(outDatas, i * packgeLength, outputReportBuffer, 1, outDatas.Length - i * packgeLength);

                            //  写一个报告
                            if (useControlTransfersOnly)
                            {
                                // 既然HID有输出节点中断，使用Control 发送一个报告，Use a control transfer to send the report, even if the HID has an interrupt OUT endpoint.
                                success = _hidObject.SendOutputReportViaControlTransfer(device.HidHandle, outputReportBuffer);
                            }
                            else
                            {
                                // 如果HID有输出中断节点，主机将使用中断传输Report， If the HID has an interrupt OUT endpoint, the host uses an interrupt transfer to send the report. 
                                // 如果没有，就使用控制传输。 If not, the host uses a control transfer.
                                if (device.FileStreamDeviceData.CanWrite)
                                {
                                    device.FileStreamDeviceData.Write(outputReportBuffer, 0, outputReportBuffer.Length);
                                    success = true;
                                }
                            }
                            if (success)
                            {
                                Debug.WriteLine("一个报告已经输出，输出报告ID: " + String.Format("{0:X2} ", outputReportBuffer[0]));
                                string outed = "输出报告数据：";
                                for (int x = 1; x < outputReportBuffer.Length; x++)
                                {
                                    outed += String.Format(" {0:X2} ", outputReportBuffer[x]);
                                }
                                Debug.WriteLine(outed);
                            }
                            else
                            {
                                CloseCommunications(device);
                                Debug.WriteLine("输出报告失败！");
                            }
                            if (this._isStop)
                                break;
                        }
                    }
                    else
                    {
                        Debug.WriteLine("HID设备没有输出缓冲区");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
            return success;
        }

        public bool WriteReportToDevice(int index, byte[] outDatas)
        {
            return this.WriteReportToDevice(index, false, outDatas);
        }

        /// <summary>
        /// 从指定设备读取数据到给定的字节数组
        /// </summary>
        /// <param name="index">设备序号</param>
        /// <param name="useControlTransfersOnly">true:当设备有中断输入节点时使用，false:异步输入使用</param>
        /// <param name="inputDatas"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool ReadReportFromDevice(int index, bool useControlTransfersOnly, ref byte[] inputDatas, int timeout)
        {
            String byteValue = null;
            Int32 count = 0;
            Byte[] inputReportBuffer = null;
            Boolean success = false;

            DeviceInformation device = this[index];

            this._isStop = false;
            try
            {
                success = false;

                //  验证设备句柄的合法性，否则不能发送报告
                //  (as for a mouse or keyboard under Windows 2000/XP.)
                if (device!=null && device.DeviceIsDetected && !device.HidHandle.IsInvalid)
                {
                    //  探测HID设备是否有输入报告.
                    //  (The HID spec requires all HIDs to have an interrupt IN endpoint,
                    //  which suggests that all HIDs must support Input reports.)
                    if (device.Capabilities.InputReportByteLength > 0)
                    {
                        //  Set the size of the Input report buffer. 
                        inputReportBuffer = new Byte[device.Capabilities.InputReportByteLength];

                        // 计算分包数
                        int packgeLength = inputReportBuffer.Length-1;
                        int packgeCount = inputDatas.Length / packgeLength;
                        if (inputDatas.Length % packgeLength != 0)
                            packgeCount++;
                        // 分包读取
                        for (int i = 0; i < packgeCount; i++)
                        {
                            _transferIsComplate = false;
                            if (useControlTransfersOnly)
                            {
                                //  Read a report using a control transfer.
                                _transferIsComplate = _hidObject.GetInputReportViaControlTransfer(device.HidHandle, ref inputReportBuffer);

                                if (_transferIsComplate)
                                {
                                    Debug.WriteLine("读入一个报告，报告的ID: " + String.Format("{0:X2} ", inputReportBuffer[0]));

                                    //  Display the report data received in the form's list box.
                                    string received = "";
                                    for (count = 0; count <= inputReportBuffer.Length - 1; count++)
                                    {
                                        received += String.Format(" {0:X2} ", inputReportBuffer[count]);
                                    }
                                    Debug.WriteLine(" 报告数据:" + received);
                                }
                                else
                                {
                                    CloseCommunications(device);
                                    Debug.WriteLine("试图读取输入失败");
                                }
                            }
                            else
                            {
                                //  用中断传输模式读取报告，Read a report using interrupt transfers.                
                                // 使能异步传输，不用主程序的线程， To enable reading a report without blocking the main thread, this
                                //  application uses an asynchronous delegate.
                                IAsyncResult ar = null;
                                _transferInProgress = true;
                                _transferIsComplate = false;
                                if (device.FileStreamDeviceData.CanRead)
                                {
                                    _nowAscDevice = device;
                                    //device.fileStreamDeviceData
                                    ar=device.FileStreamDeviceData.BeginRead(inputReportBuffer, 0, inputReportBuffer.Length, new AsyncCallback(GetInputReportData), inputReportBuffer);
                                    // 计算超时时间
                                    DateTime start = DateTime.Now;
                                    while (true)
                                    {
                                        Application.DoEvents();
                                        if (!_transferInProgress)
                                        {
                                            break;
                                        }
                                        TimeSpan span = DateTime.Now - start;
                                        if (span.TotalMilliseconds > timeout)
                                        {
                                            device.FileStreamDeviceData.Close();//.EndRead(ar);
                                            Debug.WriteLine("发生读取超时错误!");
                                            break;
                                        }
                                        if (this._isStop)
                                            break;
                                    }
                                }
                                else
                                {
                                    CloseCommunications(device);
                                    Debug.WriteLine("读取输入报告失败.");
                                }
                            }
                            success = _transferIsComplate;
                            if (_transferIsComplate)
                            {
                                if ((inputDatas.Length - i * packgeLength) > packgeLength)
                                    Array.Copy(inputReportBuffer, 1, inputDatas, i * packgeLength, packgeLength);
                                else
                                    Array.Copy(inputReportBuffer, 1, inputDatas, i * packgeLength, inputDatas.Length - i * packgeLength);
                            }
                            if (this._isStop)
                                break;
                        }
                    }
                    else
                    {
                        Debug.WriteLine("不要试图读取没有输入功能的设备");
                        Debug.WriteLine("此HID没有输入报告");
                        success = false;
                    }
                }
                else
                {
                    Debug.WriteLine("非法文件句柄或者设备未在线，或者是鼠标、键盘设备");
                    Debug.WriteLine("没有发送或接收任何报告");
                    success = false;
                }
                return success;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }

        public bool ReadReportFromDevice(int index, ref byte[] inputDatas, int timeout)
        {
            return this.ReadReportFromDevice(index, false, ref inputDatas, timeout);
        }

        /// <summary>
        /// 输出报告并读取上传的报告
        /// </summary>
        /// <param name="device">设备信息对象</param>
        /// <param name="useControlTransfersOnly">true:仅使用控制传输，false:使用文件方式传输</param>
        /// <param name="outDatas">输出数据数组</param>
        /// <param name="inputDatas">读入的数据数组</param>
        /// <param name="timeout">中断异步读取时的超时时间</param>
        /// <returns></returns>
        public bool InputAndOutputReports(int deviceIndex, bool useControlTransfersOnly, byte[] outDatas, ref byte[] inputDatas, int timeout)
        {
            bool success = this.WriteReportToDevice(deviceIndex, useControlTransfersOnly, outDatas);
            if (success)
            {
                Thread.Sleep(50);
                success = this.ReadReportFromDevice(deviceIndex, useControlTransfersOnly, ref inputDatas, timeout);
            }

#if DEBUG
            Debug.Assert(success);
#endif

            return success;
        }

        public bool WriteAndReadReportFromDevice(int deviceIndex, byte[] outDatas, ref byte[] inputDatas, int timeout)
        {
            return this.InputAndOutputReports(deviceIndex, false, outDatas, ref inputDatas, timeout);
        }

        /// <summary>
        /// 向设备输出FeasureReport
        /// </summary>
        /// <param name="index">设备序号</param>
        /// <param name="outputDatas">数据</param>
        /// <returns></returns>
        public bool WriteFeasureReportToDevice(int index, byte[] outputDatas)
        {
            DeviceInformation device = this[index];
            Byte[] outFeatureReportBuffer = null;
            Boolean success = false;
            Int32 count = 0;
            String byteValue = null;
            if (device != null && device.DeviceIsDetected && !device.HidHandle.IsInvalid)
            {
                if ((device.Capabilities.FeatureReportByteLength > 0))
                {
                    //  The HID has a Feature report.
                    //  Set the size of the Feature report buffer. 
                    //  Subtract 1 from the value in the Capabilities structure because 
                    //  the array begins at index 0.
                    outFeatureReportBuffer = new Byte[device.Capabilities.FeatureReportByteLength];

                    //  Store the report ID in the buffer:
                    outFeatureReportBuffer[0] = 0;

                    //  Store the report data following the report ID.
                    //  Use the data in the combo boxes on the form.
                    if (outputDatas.Length > outFeatureReportBuffer.Length - 1)
                        Array.Copy(outputDatas, 0, outFeatureReportBuffer, 1, outFeatureReportBuffer.Length - 1);
                    else
                        outputDatas.CopyTo(outFeatureReportBuffer, 1);

                    //  Write a report to the device
                    success = _hidObject.SendFeatureReport(device.HidHandle, outFeatureReportBuffer);

                    if (success)
                    {
                        Debug.WriteLine("A Feature report has been written.");

                        //  Display the report data sent in the form's list box.
                        Debug.WriteLine(" Feature Report ID: " + String.Format("{0:X2} ", outFeatureReportBuffer[0]));
                        Debug.WriteLine(" Feature Report Data:");

                        for (count = 0; count <= outFeatureReportBuffer.Length - 1; count++)
                        {
                            byteValue += String.Format(" {0:X2} ", outFeatureReportBuffer[count]);
                        }
                        Debug.WriteLine(byteValue);
                    }
                    else
                    {
                        CloseCommunications(device);
                        Debug.WriteLine("The attempt to send a Feature report failed.");
                    }
                }
                else
                {
                    Debug.WriteLine("The HID doesn't have a Feature report.");
                }
            }
            else
            {
                Debug.WriteLine("没发现 HID设备");
            }
            return success;
        }

        /// <summary>
        /// 读取Feature报告
        /// </summary>
        /// <param name="index">设备序号</param>
        /// <param name="inputDatas">读取到的数据</param>
        /// <returns></returns>
        public bool ReadFeatureReportsFromDevice(int index, ref byte[] inputDatas)
        {
            Byte[] inFeatureReportBuffer = null;
            Boolean success = false;
            DeviceInformation device = this[index];
            String byteValue = null;
            Int32 count = 0;

            //  从设备中读取报告.
            //  Set the size of the Feature report buffer. 
            //  Subtract 1 from the value in the Capabilities structure because 
            //  the array begins at index 0.
            if (device != null && device.DeviceIsDetected && !device.HidHandle.IsInvalid)
            {
                if (device.Capabilities.FeatureReportByteLength > 0)
                {
                    inFeatureReportBuffer = new Byte[device.Capabilities.FeatureReportByteLength];

                    //  读取报告.
                    success = _hidObject.GetFeatureReport(device.HidHandle, ref inFeatureReportBuffer);

                    if (success)
                    {
                        // 将读入的数据拷贝到输入缓冲区
                        if (inputDatas.Length > inFeatureReportBuffer.Length - 1)
                            Array.Copy(inFeatureReportBuffer, 1, inputDatas, 0, inFeatureReportBuffer.Length - 1);
                        else
                            Array.Copy(inFeatureReportBuffer, 1, inputDatas, 0, inputDatas.Length);

                        Debug.WriteLine("A Feature report has been read.");

                        //  Display the report data received in the form's list box.
                        Debug.WriteLine(" Feature Report ID: " + String.Format("{0:X2} ", inFeatureReportBuffer[0]));
                        Debug.WriteLine(" Feature Report Data:");
                        for (count = 0; count <= inFeatureReportBuffer.Length - 1; count++)
                        {
                            //  Display bytes as 2-character Hex strings.
                            byteValue += String.Format(" {0:X2} ", inFeatureReportBuffer[count]);
                        }
                        Debug.WriteLine(byteValue);
                    }
                    else
                    {
                        CloseCommunications(device);
                        Debug.WriteLine("The attempt to read a Feature report failed.");
                    }
                }
                else
                {
                    Debug.WriteLine("The HID doesn't have a Feature report.");
                }
            }
            else
            {
                Debug.WriteLine("没发现 HID设备");
            }
            return success;
        }

        ///  <summary>
        ///  发送和接收 Feature 报告，然后接收一个
        ///  Assumes report ID = 0 for both reports.
        ///  </summary>
        public bool InputAndOutputFeatureReports(int index, byte[] outputDatas, ref byte[] inputDatas)
        {
            bool success = this.WriteFeasureReportToDevice(index, outputDatas);
            if (success)
            {
                success = this.ReadFeatureReportsFromDevice(index, ref inputDatas);
                return success;
            }
            return success;
        }

        ///  <summary>
        ///  Find out if the current operating system is Windows XP or later.
        ///  (Windows XP or later is required for HidD_GetInputReport and HidD_SetInputReport.)
        ///  </summary>
        public static Boolean IsWindowsXpOrLater()
        {
            try
            {
                OperatingSystem myEnvironment = Environment.OSVersion;

                //  Windows XP is version 5.1.

                System.Version versionXP = new System.Version(5, 1);

                if (myEnvironment.Version >= versionXP)
                {
                    Debug.WriteLine("The OS is Windows XP or later.");
                    return true;
                }
                else
                {
                    Debug.WriteLine("The OS is earlier than Windows XP.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Hid.DisplayException("IsWindowsXpOrLater", ex);
                throw;
            }
        }

        ///  <summary>
        ///  Find out if the current operating system is Windows 98 Gold (original version).
        ///  Windows 98 Gold does not support the following:
        ///  Interrupt OUT transfers (WriteFile uses control transfers and Set_Report).
        ///  HidD_GetNumInputBuffers and HidD_SetNumInputBuffers
        ///  (Not yet tested on a Windows 98 Gold system.)
        ///  </summary>
        public static Boolean IsWindows98Gold()
        {
            Boolean result = false;
            try
            {
                OperatingSystem myEnvironment = Environment.OSVersion;

                //  Windows 98 Gold is version 4.10 with a build number less than 2183.

                System.Version version98SE = new System.Version(4, 10, 2183);

                if (myEnvironment.Version < version98SE)
                {
                    Debug.WriteLine("The OS is Windows 98 Gold.");
                    result = true;
                }
                else
                {
                    Debug.WriteLine("The OS is more recent than Windows 98 Gold.");
                    result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Hid.DisplayException("IsWindows98Gold", ex);
                throw;
            }
        }

        /// <summary>
        /// 设备是否连接
        /// </summary>
        /// <param name="index">设备序号</param>
        /// <returns>true:连接</returns>
        public bool IsDeviceAttached(int index)
        {
            if (this._deviceList.Count > index)
                return this[index].DeviceIsDetected;
            else
                return true;
        }
        #endregion 函数定义
    }
}
