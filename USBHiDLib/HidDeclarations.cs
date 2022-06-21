using Microsoft.Win32.SafeHandles; 
using System;
using System.Runtime.InteropServices;

namespace Synoxo.USBHidDevice
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HIDD_ATTRIBUTES
    {
        public Int32 Size;
        public UInt16 VendorID;
        public UInt16 ProductID;
        public UInt16 VersionNumber;
    }

    public struct HIDP_CAPS
    {
        public Int16 Usage;
        public Int16 UsagePage;
        public Int16 InputReportByteLength;
        public Int16 OutputReportByteLength;
        public Int16 FeatureReportByteLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
        public Int16[] Reserved;
        public Int16 NumberLinkCollectionNodes;
        public Int16 NumberInputButtonCaps;
        public Int16 NumberInputValueCaps;
        public Int16 NumberInputDataIndices;
        public Int16 NumberOutputButtonCaps;
        public Int16 NumberOutputValueCaps;
        public Int16 NumberOutputDataIndices;
        public Int16 NumberFeatureButtonCaps;
        public Int16 NumberFeatureValueCaps;
        public Int16 NumberFeatureDataIndices;
    }

    //  If IsRange is false, UsageMin is the Usage and UsageMax is unused.
    //  If IsStringRange is false, StringMin is the String index and StringMax is unused.
    //  If IsDesignatorRange is false, DesignatorMin is the designator index and DesignatorMax is unused.
    public struct HidP_Value_Caps
    {
        public Int16 UsagePage;
        public Byte ReportID;
        public Int32 IsAlias;
        public Int16 BitField;
        public Int16 LinkCollection;
        public Int16 LinkUsage;
        public Int16 LinkUsagePage;
        public Int32 IsRange;
        public Int32 IsStringRange;
        public Int32 IsDesignatorRange;
        public Int32 IsAbsolute;
        public Int32 HasNull;
        public Byte Reserved;
        public Int16 BitSize;
        public Int16 ReportCount;
        public Int16 Reserved2;
        public Int16 Reserved3;
        public Int16 Reserved4;
        public Int16 Reserved5;
        public Int16 Reserved6;
        public Int32 LogicalMin;
        public Int32 LogicalMax;
        public Int32 PhysicalMin;
        public Int32 PhysicalMax;
        public Int16 UsageMin;
        public Int16 UsageMax;
        public Int16 StringMin;
        public Int16 StringMax;
        public Int16 DesignatorMin;
        public Int16 DesignatorMax;
        public Int16 DataIndexMin;
        public Int16 DataIndexMax;
    }

    internal partial class Hid
    {
        //  API declarations for HID communications.

        //  from hidpi.h
        //  Typedef enum defines a set of integer constants for HidP_Report_Type

        public const Int16 HidP_Input = 0;
        public const Int16 HidP_Output = 1;
        public const Int16 HidP_Feature = 2;

        /// <summary>
        /// 移除等待在缓冲区中的报告
        /// </summary>
        /// <param name="HidDeviceObject">指向设备的句柄</param>
        /// <returns>成功：True , 失败：False</returns>
        [DllImport("hid.dll", SetLastError = true)]
        public static extern Boolean HidD_FlushQueue(SafeFileHandle HidDeviceObject);

        /// <summary>
        /// 释放由HidD_GetPreparsedData函数申请的缓冲区
        /// </summary>
        /// <param name="PreparsedData">指向HidD_GetPreparsedData函数返回的PreparsedData数据结构指针</param>
        /// <returns>成功：True , 失败：False</returns>
        [DllImport("hid.dll", SetLastError = true)]
        public static extern Boolean HidD_FreePreparsedData(IntPtr PreparsedData);

        /// <summary>
        /// 从HID设备获取界面类的GUID；Retrieves the interface class GUID for the HID class.
        /// </summary>
        /// <param name="HidGuid">系统分配给设备的GUID字符串；A System.Guid object for storing the GUID</param>
        [DllImport("hid.dll", SetLastError = true)]
        public static extern void HidD_GetHidGuid(ref System.Guid HidGuid);

        /// <summary>
        /// 获取设备属性
        /// </summary>
        /// <param name="HidDeviceObject">设备句柄</param>
        /// <param name="Attributes">HIDD_ATTRIBUTES结构</param>
        /// <returns>成功：True , 失败：False</returns>
        [DllImport("hid.dll", SetLastError = true)]
        public static extern Boolean HidD_GetAttributes(SafeFileHandle HidDeviceObject, ref HIDD_ATTRIBUTES Attributes);

        /// <summary>
        /// 目的: 获取设备性能参数
        /// 对于标准设备，例如：手柄，你可以找出设备的特殊参数
        /// 对于软件知道容量的客户设备，不需要此函数
        /// </summary>
        /// <param name="PreparsedData">指向HidD_GetPreparsedData返回的结构指针</param>
        /// <param name="Capabilities">指向HIDP_CAPS structure的结构指针</param>
        /// <returns>成功：True , 失败：False</returns>
        [DllImport("hid.dll", SetLastError = true)]
        public static extern Int32 HidP_GetCaps(IntPtr PreparsedData, ref HIDP_CAPS Capabilities);

        /// <summary>
        /// 试着发送特性报告至设备；Attempts to send a Feature report to the device
        /// </summary>
        /// <param name="HidDeviceObject">HID设备句柄</param>
        /// <param name="lpReportBuffer">指向报告ID和报告体的缓冲区指针</param>
        /// <param name="ReportBufferLength">缓冲区大小</param>
        /// <returns>成功：True , 失败：False</returns>
        [DllImport("hid.dll", SetLastError = true)]
        public static extern Boolean HidD_SetFeature(SafeFileHandle HidDeviceObject, Byte[] lpReportBuffer, Int32 ReportBufferLength);

        /// <summary>
        /// 试着读取设备的报告的属性
        /// </summary>
        /// <param name="HidDeviceObject">HID设备句柄</param>
        /// <param name="lpReportBuffer">包含报告ID和内容的缓冲区指针</param>
        /// <param name="ReportBufferLength">缓冲区长度</param>
        /// <returns>成功：True , 失败：False</returns>
        [DllImport("hid.dll", SetLastError = true)]
        public static extern Boolean HidD_GetFeature(SafeFileHandle HidDeviceObject, Byte[] lpReportBuffer, Int32 ReportBufferLength);

        /// <summary>
        /// 试着用控制传输发送输出报告到设备
        /// 需要Win XP及以后的版本支持
        /// </summary>
        /// <param name="HidDeviceObject">HID设备句柄</param>
        /// <param name="lpReportBuffer">指向报告ID和报告体的指针</param>
        /// <param name="ReportBufferLength">报告长度</param>
        /// <returns>成功：True , 失败：False</returns>
        [DllImport("hid.dll", SetLastError = true)]
        public static extern Boolean HidD_SetOutputReport(SafeFileHandle HidDeviceObject, Byte[] lpReportBuffer, Int32 ReportBufferLength);

        /// <summary>
        /// 用控制传输（control transfer）试着从HID设备读取输入的报告，仅支持Windows XP以后的版本
        /// </summary>
        /// <param name="HidDeviceObject">HID设备句柄</param>
        /// <param name="lpReportBuffer">包含报告ID和内容的缓冲区指针</param>
        /// <param name="ReportBufferLength">缓冲区长度</param>
        /// <returns>成功：True , 失败：False</returns>
        [DllImport("hid.dll", SetLastError = true)]
        public static extern Boolean HidD_GetInputReport(SafeFileHandle HidDeviceObject, Byte[] lpReportBuffer, Int32 ReportBufferLength);

        /// <summary>
        /// Sets the number of Input reports the host can store
        /// 设置主机能存储的输入报告的数量
        /// </summary>
        /// <param name="HidDeviceObject">HID设备句柄</param>
        /// <param name="NumberBuffers">缓冲区数量；An integer to hold the number of buffers</param>
        /// <returns>成功：True , 失败：False</returns>
        [DllImport("hid.dll", SetLastError = true)]
        public static extern Boolean HidD_SetNumInputBuffers(SafeFileHandle HidDeviceObject, Int32 NumberBuffers);

        /// <summary>
        /// 获取Host能存储的输入报告的数值，不支持Windows 98 Gold，如果缓冲区满，又有新的报告到达，主机则丢掉缓冲区的报告(ldest report)
        /// </summary>
        /// <param name="HidDeviceObject">HID设备句柄，a handle to a device</param>
        /// <param name="NumberBuffers">保持缓冲区数值的整数；integer to hold the number of buffers</param>
        /// <returns>成功：True , 失败：False</returns>
        [DllImport("hid.dll", SetLastError = true)]
        public static extern Boolean HidD_GetNumInputBuffers(SafeFileHandle HidDeviceObject, ref Int32 NumberBuffers);

        /// <summary>
        /// API函数: HidD_GetPreparsedData
        /// 目的: 获取指向关于器件性能信息结构的指针.
        /// HidP_GetCaps 和其它 API 函数需要一个指向缓冲区的指针.
        /// </summary>
        /// <param name="HidDeviceObject">一个由CreateFile函数返回的文件句柄</param>
        /// <param name="PreparsedData">一个指向Buff的指针</param>
        /// <returns>成功：True , 失败：False</returns>
        [DllImport("hid.dll", SetLastError = true)]
        public static extern Boolean HidD_GetPreparsedData(SafeFileHandle HidDeviceObject, ref IntPtr PreparsedData);

        /// <summary>
        /// 重新获取一个 HidP_ValueCaps结构阵列，每个结构定义一个值得容量，此程序未用
        /// retrieves a buffer containing an array of HidP_ValueCaps structures.
        ///  Each structure defines the capabilities of one value.
        ///  This application doesn't use this data.
        /// </summary>
        /// <param name="ReportType">在hidpi.h中的报告枚举类型；A report type enumerator from hidpi.h</param>
        /// <param name="ValueCaps">返回阵列缓冲区指针；A pointer to a buffer for the returned array</param>
        /// <param name="ValueCapsLength">设备的HidP_Caps结构存储区；The NumberInputValueCaps member of the device's HidP_Caps structure</param>
        /// <param name="PreparsedData">由HidD_GetPreparsedData函数返回的PreparsedData数据结构指针，A pointer to the PreparsedData structure returned by HidD_GetPreparsedData</param>
        /// <returns>成功：True , 失败：False</returns>
        [DllImport("hid.dll", SetLastError = true)]
        public static extern Int32 HidP_GetValueCaps(Int32 ReportType, Byte[] ValueCaps, ref Int32 ValueCapsLength, IntPtr PreparsedData);
    }
}
