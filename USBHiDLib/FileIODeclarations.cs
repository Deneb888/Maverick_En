using Microsoft.Win32.SafeHandles; 
using System.Runtime.InteropServices;
using System.Threading;

///  <summary>
///  API declarations relating to file I/O.
///  </summary>

using System;

namespace Synoxo.USBHidDevice
{
	public class FileIO
	{
		public const Int32 FILE_SHARE_READ = 1;
		public const Int32 FILE_SHARE_WRITE = 2;
		public const uint GENERIC_READ = 0X80000000U;
		public const Int32 GENERIC_WRITE = 0X40000000;
		public const Int32 INVALID_HANDLE_VALUE = -1;
		public const Int32 OPEN_EXISTING = 3;

        /// <summary>
        /// 获取设备句柄
        /// </summary>
        /// <param name="lpFileName">由SetupDiGetDeviceInterfaceDetail函数返回的设备路径；A device path name returned by SetupDiGetDeviceInterfaceDetail</param>
        /// <param name="dwDesiredAccess">读写类型；The type of access requested (read/write)</param>
        /// <param name="dwShareMode">当设备句柄打开后允许的文件操作的FILE_SHARE属性 FILE_SHARE attributes to allow other processes to access the device while this handle is open</param>
        /// <param name="lpSecurityAttributes">一个安全的结构或者IntPtr.Zero；A Security structure or IntPtr.Zero</param>
        /// <param name="dwCreationDisposition">生成时的属性值，对设备用OPEN_EXISTING；A creation disposition value. Use OPEN_EXISTING for devices</param>
        /// <param name="dwFlagsAndAttributes">文件的标志和属性，对设备无效；Flags and attributes for files. Not used for devices</param>
        /// <param name="hTemplateFile">临时文件句柄，不用；Handle to a template file. Not used</param>
        /// <returns>设备句柄，用此可以获取所有HID、甚至系统键盘鼠标的信息，Returns: a handle without read or write access.This enables obtaining information about all HIDs, even system keyboards and mice.Separate handles are used for reading and writing.</returns>
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern SafeFileHandle CreateFile(String lpFileName, UInt32 dwDesiredAccess, Int32 dwShareMode, IntPtr lpSecurityAttributes, Int32 dwCreationDisposition, Int32 dwFlagsAndAttributes, Int32 hTemplateFile);
	}
} 
