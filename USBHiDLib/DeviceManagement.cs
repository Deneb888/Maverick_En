///  <summary>
///  Routines for detecting devices and receiving device notifications.
///  </summary>
  
using System;
using System.Runtime.InteropServices; 
using System.Windows.Forms;
using System.Diagnostics;

namespace Synoxo.USBHidDevice
{
    /// <summary>
    /// 本类从Control派生，主要考虑到要从Window获得系统消息的方法
    /// 以便通知用户USB插拔事件
    /// </summary>
	public partial class DeviceManagement
	{
		///  <summary>
		///  比较设备的路径名，用于发现最近连接的或者移除的正在通讯的匹配设备名称
        ///  Compares two device path names. Used to find out if the device name 
		///  of a recently attached or removed device matches the name of a 
		///  device the application is communicating with.
		///  </summary>
		///  <param name="m"> 一个 WM_DEVICECHANGE 消息. 调用 RegisterDeviceNotification 函数
		///  以便 WM_DEVICECHANGE 消息可以传递到 OnDeviceChange 例行程序.. </param>
		///  <param name="mydevicePathName"> a device pathname returned by 
		///  SetupDiGetDeviceInterfaceDetail in an SP_DEVICE_INTERFACE_DETAIL_DATA structure. </param>
		///  <returns>
		///  名称匹配返回True，否则返回False
		///  </returns>
		public Boolean DeviceNameMatch(Message m, String mydevicePathName)
		{
			Int32 stringSize;

			try
			{
				DEV_BROADCAST_DEVICEINTERFACE_1 devBroadcastDeviceInterface = new DEV_BROADCAST_DEVICEINTERFACE_1();
				DEV_BROADCAST_HDR devBroadcastHeader = new DEV_BROADCAST_HDR();

				// Message 的 LParam 参数  is 指向 DEV_BROADCAST_HDR 结构的指针.
				Marshal.PtrToStructure(m.LParam, devBroadcastHeader);

				if ((devBroadcastHeader.dbch_devicetype == DBT_DEVTYP_DEVICEINTERFACE))
				{
					// dbch_devicetype 参数指出了应用设备的接口 
					// 因此 LParam 参数实际是以DEV_BROADCAST_HDR开始的 DEV_BROADCAST_INTERFACE结构  

					// 通过减去结构中不是dbch_name部分的32字节后得到了字符的个数，由于一个字符两个字节，所以再除以2
					stringSize = System.Convert.ToInt32((devBroadcastHeader.dbch_size - 32) / 2);

					//  devBroadcastDeviceInterface中的dbcc_name 参数包含了设备名称 
					// 去掉前后的空格以匹配字符串长度         
					devBroadcastDeviceInterface.dbcc_name = new Char[stringSize + 1];

					// 将数据从不受管理的数据块传输到受管理的devBroadcastDeviceInterface对象中
					Marshal.PtrToStructure(m.LParam, devBroadcastDeviceInterface);

					// 将设备名用String类型存储
					String DeviceNameString = new String(devBroadcastDeviceInterface.dbcc_name, 0, stringSize);

					// 比较新连接的设备名字和我们已知的设备(mydevicePathName)名字，匹配返回true
					if ((String.Compare(DeviceNameString, mydevicePathName, true) == 0))
					{
						return true;
					}
					else
					{
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				throw;
			}

			return false;
		}

		///  <summary>
		///  Use SetupDi API functions to retrieve the device path name of an
		///  attached device that belongs to a device interface class.
		///  </summary>
		///  <param name="myGuid"> an interface class GUID. </param>
		///  <param name="devicePathName"> a pointer to the device path name of an attached device. </param>
		///  <returns>	True if a device is found, False if not</returns>
        public Boolean FindDeviceFromGuid(System.Guid myGuid, ref String[] devicePathName)
        {
            Int32 bufferSize = 0;
            IntPtr detailDataBuffer = IntPtr.Zero;
            Boolean deviceFound;
            IntPtr deviceInfoSet = new System.IntPtr();
            Boolean lastDevice = false;
            Int32 memberIndex = 0;
            SP_DEVICE_INTERFACE_DATA MyDeviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();
            Boolean success;

            try
            {
                //  调用API 函数
                deviceInfoSet = SetupDiGetClassDevs(ref myGuid, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);
                Debug.WriteLine("在函数FindDeviceFromGuid中" + this.MyDebugging.ResultOfAPICall("SetupDiGetClassDevs"));

                deviceFound = false;
                memberIndex = 0;

                // The cbSize element of the MyDeviceInterfaceData structure must be set to
                // the structure's size in bytes. 
                // The size is 28 bytes for 32-bit code and 32 bits for 64-bit code.
                MyDeviceInterfaceData.cbSize = Marshal.SizeOf(MyDeviceInterfaceData);

                do
                {
                    // Begin with 0 and increment through the device information set until no more devices are available.

                    // 调用API函数
                    success = SetupDiEnumDeviceInterfaces(deviceInfoSet, IntPtr.Zero, ref myGuid, memberIndex, ref MyDeviceInterfaceData);

                    Debug.WriteLine("在函数FindDeviceFromGuid中" + this.MyDebugging.ResultOfAPICall("SetupDiEnumDeviceInterfaces"));
                    // Find out if a device information set was retrieved.
                    if (!success)
                    {
                        lastDevice = true;
                    }
                    else
                    {
                        // 调用API函数 
                        success = SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref MyDeviceInterfaceData, IntPtr.Zero, 0, ref bufferSize, IntPtr.Zero);
                        Debug.WriteLine("在函数FindDeviceFromGuid中" + this.MyDebugging.ResultOfAPICall("SetupDiGetDeviceInterfaceDetail"));

                        // 使用返回的缓冲区大小为SP_DEVICE_INTERFACE_DETAIL_DATA结构分配内存
                        detailDataBuffer = Marshal.AllocHGlobal(bufferSize);

                        // Store cbSize in the first bytes of the array. The number of bytes varies with 32- and 64-bit systems.
                        Marshal.WriteInt32(detailDataBuffer, (IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8);

                        // 再次调用SetupDiGetDeviceInterfaceDetail 函数，这次传递一个指针给DetailDataBuffer，返回请求的缓冲区大小
                        success = SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref MyDeviceInterfaceData, detailDataBuffer, bufferSize, ref bufferSize, IntPtr.Zero);
                        Debug.WriteLine("在函数FindDeviceFromGuid中" + this.MyDebugging.ResultOfAPICall("SetupDiGetDeviceInterfaceDetail"));

                        // 略过 cbsize (4 bytes) 以获得devicePathName的地址
                        IntPtr pDevicePathName = new IntPtr(detailDataBuffer.ToInt32() + 4);

                        // 获取devicePathName字符串.
                        devicePathName[memberIndex] = Marshal.PtrToStringAuto(pDevicePathName);

                        if (detailDataBuffer != IntPtr.Zero)
                        {
                            // 释放由 AllocHGlobal 分配的内存
                            Marshal.FreeHGlobal(detailDataBuffer);
                        }
                        deviceFound = true;
                    }
                    memberIndex++;
                }
                while (!((lastDevice == true)));

                return deviceFound;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                //  调用API函数释放内存
                if (deviceInfoSet != IntPtr.Zero)
                {
                    SetupDiDestroyDeviceInfoList(deviceInfoSet);
                    Debug.WriteLine("在函数FindDeviceFromGuid中" + this.MyDebugging.ResultOfAPICall("SetupDiDestroyDeviceInfoList"));
                }
            }
        }

		///  <summary>
		///  当设备插入或拔出时请求接收消息
        ///  Requests to receive a notification when a device is attached or removed.
		///  </summary>
		///  <param name="devicePathName"> 设备句柄 handle to a device. </param>
		///  <param name="formHandle"> 接收事件的窗体句柄 handle to the window that will receive device events. </param>
		///  <param name="classGuid"> 设备接口 GUID device interface GUID. </param>
		///  <param name="deviceNotificationHandle"> 返回设备事件句柄 returned device notification handle. </param>
		///  <returns>
		///  成功返回True
		///  </returns>
		public Boolean RegisterForDeviceNotifications(String devicePathName, IntPtr formHandle, Guid classGuid, ref IntPtr deviceNotificationHandle)
		{
			// A DEV_BROADCAST_DEVICEINTERFACE header holds information about the request.
			DEV_BROADCAST_DEVICEINTERFACE devBroadcastDeviceInterface = new DEV_BROADCAST_DEVICEINTERFACE();
			IntPtr devBroadcastDeviceInterfaceBuffer = IntPtr.Zero;
			Int32 size = 0;

			try
			{
				// Set the parameters in the DEV_BROADCAST_DEVICEINTERFACE structure.
				// Set the size.
				size = Marshal.SizeOf(devBroadcastDeviceInterface);
				devBroadcastDeviceInterface.dbcc_size = size;

				// Request to receive notifications about a class of devices.
				devBroadcastDeviceInterface.dbcc_devicetype = DBT_DEVTYP_DEVICEINTERFACE;

				devBroadcastDeviceInterface.dbcc_reserved = 0;

				// Specify the interface class to receive notifications about.
				devBroadcastDeviceInterface.dbcc_classguid = classGuid;

				// Allocate memory for the buffer that holds the DEV_BROADCAST_DEVICEINTERFACE structure.
				devBroadcastDeviceInterfaceBuffer = Marshal.AllocHGlobal(size);

				// Copy the DEV_BROADCAST_DEVICEINTERFACE structure to the buffer.
				// Set fDeleteOld True to prevent memory leaks.
				Marshal.StructureToPtr(devBroadcastDeviceInterface, devBroadcastDeviceInterfaceBuffer, true);

                // 调用API函数
				deviceNotificationHandle = RegisterDeviceNotification(formHandle, devBroadcastDeviceInterfaceBuffer, DEVICE_NOTIFY_WINDOW_HANDLE);
                Debug.WriteLine("在函数RegisterForDeviceNotifications中" + this.MyDebugging.ResultOfAPICall("RegisterDeviceNotification"));

				// 从非管理的devBroadcastDeviceInterfaceBuffer数据块集合到受管理的devBroadcastDeviceInterface对象 
                // Marshal data from the unmanaged block devBroadcastDeviceInterfaceBuffer to the managed object devBroadcastDeviceInterface
				Marshal.PtrToStructure(devBroadcastDeviceInterfaceBuffer, devBroadcastDeviceInterface);

				if ((deviceNotificationHandle.ToInt32() == IntPtr.Zero.ToInt32()))
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				throw;
			}
			finally
			{
				if (devBroadcastDeviceInterfaceBuffer != IntPtr.Zero)
				{
					// 释放由AllocHGlobal函数分配的内存空间。 Free the memory allocated previously by AllocHGlobal.
					Marshal.FreeHGlobal(devBroadcastDeviceInterfaceBuffer);
				}
			}
		}

		///  <summary>
		///  Requests to stop receiving notification messages when a device in an
		///  interface class is attached or removed.
		///  </summary>
		///  <param name="deviceNotificationHandle"> handle returned previously by
		///  RegisterDeviceNotification. </param>
		public void StopReceivingDeviceNotifications(IntPtr deviceNotificationHandle)
		{
			try
			{
				//  调用API函数
				DeviceManagement.UnregisterDeviceNotification(deviceNotificationHandle);
                Debug.WriteLine("在函数StopReceivingDeviceNotifications中" + this.MyDebugging.ResultOfAPICall("UnregisterDeviceNotification"));
            }
			catch (Exception ex)
			{
				throw;
			}
		}
	}
}





