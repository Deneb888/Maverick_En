using System;
using System.Runtime.InteropServices; 

namespace Synoxo.USBHidDevice
{
	public partial class DeviceManagement
	{
		///<summary >
		// API declarations relating to device management (SetupDixxx and 
		// RegisterDeviceNotification functions).   
		/// </summary>

		// from dbt.h
		public const Int32 DBT_DEVICEARRIVAL = 0X8000;
		public const Int32 DBT_DEVICEREMOVECOMPLETE = 0X8004;
		public const Int32 DBT_DEVTYP_DEVICEINTERFACE = 5;
		public const Int32 DBT_DEVTYP_HANDLE = 6;
		public const Int32 DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;
		public const Int32 DEVICE_NOTIFY_SERVICE_HANDLE = 1;
		public const Int32 DEVICE_NOTIFY_WINDOW_HANDLE = 0;
		public const Int32 WM_DEVICECHANGE = 0X219;

		// from setupapi.h
		public const Int32 DIGCF_PRESENT = 2;
		public const Int32 DIGCF_DEVICEINTERFACE = 0X10;

		// Two declarations for the DEV_BROADCAST_DEVICEINTERFACE structure.

		// Use this one in the call to RegisterDeviceNotification() and
		// in checking dbch_devicetype in a DEV_BROADCAST_HDR structure:

		[StructLayout(LayoutKind.Sequential)]
		public class DEV_BROADCAST_DEVICEINTERFACE
		{
			public Int32 dbcc_size;
			public Int32 dbcc_devicetype;
			public Int32 dbcc_reserved;
			public Guid dbcc_classguid;
			public Int16 dbcc_name;
		}

		// Use this to read the dbcc_name String and classguid:
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public class DEV_BROADCAST_DEVICEINTERFACE_1
		{
			public Int32 dbcc_size;
			public Int32 dbcc_devicetype;
			public Int32 dbcc_reserved;
			[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
			public Byte[] dbcc_classguid;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
			public Char[] dbcc_name;
		}

		[StructLayout(LayoutKind.Sequential)]
		public class DEV_BROADCAST_HDR
		{
			public Int32 dbch_size;
			public Int32 dbch_devicetype;
			public Int32 dbch_reserved;
		}

		public struct SP_DEVICE_INTERFACE_DATA
		{
			public Int32 cbSize;
			public System.Guid InterfaceClassGuid;
			public Int32 Flags;
			public IntPtr Reserved;
		}

		public struct SP_DEVICE_INTERFACE_DETAIL_DATA
		{
			public Int32 cbSize;
			public String DevicePath;
		}

		public struct SP_DEVINFO_DATA
		{
			public Int32 cbSize;
			public System.Guid ClassGuid;
			public Int32 DevInst;
			public Int32 Reserved;
		}

        /// <summary>
        ///  当接口类连接或者拔出时，请求接收通知信息
        /// Request to receive notification messages when a device in an interface class is attached or removed.
        /// </summary>
        /// <param name="hRecipient">接收设备事件的窗口句柄
        /// Handle to the window that will receive device events</param>
        /// <param name="NotificationFilter">发送事件的设备结构指针
        /// Pointer to a DEV_BROADCAST_DEVICEINTERFACE to specify the type of device to send notifications for.</param>
        /// <param name="Flags">DEVICE_NOTIFY_WINDOW_HANDLE indicates the handle is a window handle.</param>
        /// <returns>Device notification handle or NULL on failure.</returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, Int32 Flags);

        /// <summary>
        /// 停止接收系统消息
        /// </summary>
        /// <param name="Handle">Handle returned previously by RegisterDeviceNotification.</param>
        /// <returns>True on success</returns>
		[DllImport("user32.dll", SetLastError = true)]
		public static extern Boolean UnregisterDeviceNotification(IntPtr Handle);

		[DllImport("setupapi.dll", SetLastError = true)]
		public static extern Int32 SetupDiCreateDeviceInfoList(ref System.Guid ClassGuid, Int32 hwndParent);

        /// <summary>
        /// 释放为DeviceInfoSet返回的SetupDiGetClassDevs分配的内存
        /// </summary>
        /// <param name="DeviceInfoSet">由SetupDiGetClassDevs 返回的 DeviceInfoSet</param>
        /// <returns>True on success</returns>
		[DllImport("setupapi.dll", SetLastError = true)]
		public static extern Int32 SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        ///  <summary>
        ///  Retrieves a handle to a SP_DEVICE_INTERFACE_DATA structure for a device.
        ///  On return, MyDeviceInterfaceData contains the handle to a
        ///  SP_DEVICE_INTERFACE_DATA structure for a detected device.
        ///  </summary>
        ///  <param name="DeviceInfoSet">DeviceInfoSet returned by SetupDiGetClassDevs</param>
        ///  <param name="DeviceInfoData">Optional SP_DEVINFO_DATA structure that defines a device instance that is a member of a device information set</param> 
        ///  <param name="InterfaceClassGuid">Device interface GUID</param>
        ///  <param name="MemberIndex">Index to specify a device in a device information set</param>
        ///  <param name="DeviceInterfaceData">指向设备的SP_DEVICE_INTERFACE_DATA结构句柄的指针</param>
        ///  <returns>True on success.</returns>
		[DllImport("setupapi.dll", SetLastError = true)]
		public static extern Boolean SetupDiEnumDeviceInterfaces(IntPtr DeviceInfoSet, IntPtr DeviceInfoData, ref System.Guid InterfaceClassGuid, Int32 MemberIndex, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData);

        ///  <summary>
        ///   为指定类型的设备重新得到设备信息
        ///   Retrieves a device information set for a specified group of devices.
        ///  </summary>
        ///  <param name="ClassGuid">SetupDiEnumDeviceInterfaces uses the device information set Interface class GUID.</param>
        ///  <param name="Enumerator">Null to retrieve information for all device instances.</param>
        ///  <param name="hwndParent">Optional handle to a top-level window (unused here)</param>
        ///  <param name="Flags">Flags to limit the returned information to currently present devices 
        ///  and devices that expose interfaces in the class specified by the GUID
        ///  </param>
        ///  <returns>Handle to a device information set for the devices.</returns>
		[DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern IntPtr SetupDiGetClassDevs(ref System.Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, Int32 Flags);

        /// <summary>
        /// 重申请一个SP_DEVICE_INTERFACE_DETAIL_DATA结构以存储设备信息
        /// 要获得信息，得调用此函数两次,第一次得到结构大小,第二次得到指向数据的指针
        /// </summary>
        /// <param name="DeviceInfoSet">由SetupDiGetClassDevs函数返回的设备信息</param>
        /// <param name="DeviceInterfaceData">由SetupDiEnumDeviceInterfaces函数返回的SP_DEVICE_INTERFACE_DATA数据结构</param>
        /// <param name="DeviceInterfaceDetailData">返回的接收特殊界面信息的指向SP_DEVICE_INTERFACE_DETAIL_DATA的数据指针</param>
        /// <param name="DeviceInterfaceDetailDataSize">SP_DEVICE_INTERFACE_DETAIL_DATA数据结构的大小</param>
        /// <param name="RequiredSize">指向将要接收返回的请求的SP_DEVICE_INTERFACE_DETAIL_DATA 数据结构大小的指针</param>
        /// <param name="DeviceInfoData">返回的设备接收信息的SP_DEVINFO_DATA结构指针</param>
        /// <returns>成功返回true</returns>
		[DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern Boolean SetupDiGetDeviceInterfaceDetail(IntPtr DeviceInfoSet, ref SP_DEVICE_INTERFACE_DATA DeviceInterfaceData, IntPtr DeviceInterfaceDetailData, Int32 DeviceInterfaceDetailDataSize, ref Int32 RequiredSize, IntPtr DeviceInfoData);
	}
}
