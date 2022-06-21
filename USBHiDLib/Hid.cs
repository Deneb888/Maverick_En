using Microsoft.VisualBasic;
using Microsoft.Win32.SafeHandles; 
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

///  <summary>
///  For communicating with HID-class USB devices.
///  Includes routines for sending and receiving reports via control transfers and to 
///  retrieve information about and configure a HID.
///  </summary>
///  

namespace Synoxo.USBHidDevice
{    
    internal partial class Hid  
    {         
        //  Used in error messages.
        
        private const String MODULE_NAME = "Hid";

        private Debugging MyDebugging = new Debugging(); //  For viewing results of API calls via Debug.Write.

        public HIDP_CAPS Capabilities; 
        public HIDD_ATTRIBUTES DeviceAttributes; 
        
        ///  <summary>
        ///  清空等待中的报告输入队列
        ///  </summary>
        ///  <param name="hidHandle">设备句柄</param>
        ///  <returns>
        ///  成功：true, 失败：false.
        ///  </returns>
        public Boolean FlushQueue( SafeFileHandle hidHandle ) 
        {             
            Boolean success = false; 
            
            try 
            { 
                //  调用API函数: HidD_FlushQueue
                success = HidD_FlushQueue( hidHandle );
                Debug.WriteLine(MyDebugging.ResultOfAPICall("HidD_FlushQueue"));
                return success;                 
            } 
            catch ( Exception ex ) 
            { 
                DisplayException( MODULE_NAME, ex ); 
                throw ; 
            }             
        }         
        
        ///  <summary>
        ///  检索设备的参数性能信息，存储到结构中 
        ///  </summary>
        ///  <param name="hidHandle"> 设备句柄 </param>
        ///  <returns>
        ///  一个 HIDP_CAPS structure结构.
        ///  </returns>
        public HIDP_CAPS GetDeviceCapabilities( SafeFileHandle hidHandle ) 
        {             
            IntPtr preparsedData = new System.IntPtr(); 
            Int32 result = 0; 
            Boolean success = false;  

			try
			{
				success = HidD_GetPreparsedData(hidHandle, ref preparsedData);
                Debug.WriteLine(this.MyDebugging.ResultOfAPICall("HidD_GetPreparsedData"));

                //  调用API函数: HidP_GetCaps
                result = HidP_GetCaps(preparsedData, ref Capabilities);
                Debug.WriteLine(this.MyDebugging.ResultOfAPICall("HidP_GetCaps"));
                if ((result != 0))
				{
					Debug.WriteLine("");
					Debug.WriteLine("  Usage: " + Convert.ToString(Capabilities.Usage, 16));
					Debug.WriteLine("  Usage Page: " + Convert.ToString(Capabilities.UsagePage, 16));
					Debug.WriteLine("  Input Report Byte Length: " + Capabilities.InputReportByteLength);
					Debug.WriteLine("  Output Report Byte Length: " + Capabilities.OutputReportByteLength);
					Debug.WriteLine("  Feature Report Byte Length: " + Capabilities.FeatureReportByteLength);
					Debug.WriteLine("  Number of Link Collection Nodes: " + Capabilities.NumberLinkCollectionNodes);
					Debug.WriteLine("  Number of Input Button Caps: " + Capabilities.NumberInputButtonCaps);
					Debug.WriteLine("  Number of Input Value Caps: " + Capabilities.NumberInputValueCaps);
					Debug.WriteLine("  Number of Input Data Indices: " + Capabilities.NumberInputDataIndices);
					Debug.WriteLine("  Number of Output Button Caps: " + Capabilities.NumberOutputButtonCaps);
					Debug.WriteLine("  Number of Output Value Caps: " + Capabilities.NumberOutputValueCaps);
					Debug.WriteLine("  Number of Output Data Indices: " + Capabilities.NumberOutputDataIndices);
					Debug.WriteLine("  Number of Feature Button Caps: " + Capabilities.NumberFeatureButtonCaps);
					Debug.WriteLine("  Number of Feature Value Caps: " + Capabilities.NumberFeatureValueCaps);
					Debug.WriteLine("  Number of Feature Data Indices: " + Capabilities.NumberFeatureDataIndices);

					//  调用API函数: HidP_GetValueCaps
					Int32 vcSize = Capabilities.NumberInputValueCaps;
					Byte[] valueCaps = new Byte[vcSize];
					
					result = HidP_GetValueCaps(HidP_Input, valueCaps, ref vcSize, preparsedData);
                    Debug.WriteLine(this.MyDebugging.ResultOfAPICall("HidP_GetValueCaps"));

				}
			}
			catch (Exception ex)
			{
				DisplayException(MODULE_NAME, ex);
				throw;
			}
			finally
			{
				//  API function: HidD_FreePreparsedData
				if (preparsedData != IntPtr.Zero)
				{
					success = HidD_FreePreparsedData(preparsedData);
                    Debug.WriteLine(MyDebugging.ResultOfAPICall("HidD_FreePreparsedData"));
                }
			} 
            
            return Capabilities;             
        }

        ///  <summary>
        ///  Creates a 32-bit Usage from the Usage Page and Usage ID. 
        ///  Determines whether the Usage is a system mouse or keyboard.
        ///  Can be modified to detect other Usages.
        ///  </summary>
        ///  <param name="MyCapabilities"> a HIDP_CAPS structure retrieved with HidP_GetCaps. </param>
        ///  <returns>
        ///  A String describing the Usage.
        ///  </returns>
        public String GetHidUsage(HIDP_CAPS MyCapabilities)
        {
            Int32 usage = 0;
            String usageDescription = "";

            try
            {
                //  Create32-bit Usage from Usage Page and Usage ID.
                usage = MyCapabilities.UsagePage * 256 + MyCapabilities.Usage;
                if (usage == Convert.ToInt32(0X102))
                {
                    usageDescription = "mouse";
                }
                if (usage == Convert.ToInt32(0X106))
                {
                    usageDescription = "keyboard";
                }
            }
            catch (Exception ex)
            {
                DisplayException(MODULE_NAME, ex);
                throw;
            }

            return usageDescription;
        }

        ///  <summary>
		///  writes a Feature report to the device.
		///  </summary>
		///  
		///  <param name="outFeatureReportBuffer"> contains the report ID and report data. </param>
		///  <param name="hidHandle"> handle to the device.  </param>
		///  
		///  <returns>
		///   True on success. False on failure.
		///  </returns>            
		public Boolean SendFeatureReport(SafeFileHandle hidHandle, Byte[] outFeatureReportBuffer)
		{
			Boolean success = false;

			try
			{
				//  ***
				//  调用API函数: HidD_SetFeature
				success = HidD_SetFeature(hidHandle, outFeatureReportBuffer, outFeatureReportBuffer.Length);
                Debug.WriteLine(this.MyDebugging.ResultOfAPICall("HidD_SetFeature"));

				return success;
			}
			catch (Exception ex)
			{
				DisplayException(MODULE_NAME, ex);
				throw;
			}
		}             
		
		///  <summary>
		///  reads a Feature report from the device.
		///  </summary>
		///  
		///  <param name="hidHandle"> the handle for learning about the device and exchanging Feature reports. </param>	
		///  <param name="myDeviceDetected"> tells whether the device is currently attached.</param>
		///  <param name="inFeatureReportBuffer"> contains the requested report.</param>
		///  <param name="success"> read success</param>
		public Boolean GetFeatureReport(SafeFileHandle hidHandle, ref Byte[] inFeatureReportBuffer)
		{
			Boolean success;

			try
			{
				//  ***
				//  API function: HidD_GetFeature
				success = HidD_GetFeature(hidHandle, inFeatureReportBuffer, inFeatureReportBuffer.Length);
                Debug.WriteLine(this.MyDebugging.ResultOfAPICall("HidD_GetFeature"));
                return success;
			}
			catch (Exception ex)
			{
				DisplayException(MODULE_NAME, ex);
				throw;
			}
		}             

        ///  <summary>
            ///  Writes an Output report to the device using a control transfer.
            ///  </summary>
            ///  
            ///  <param name="outputReportBuffer"> contains the report ID and report data. </param>
            ///  <param name="hidHandle"> handle to the device.  </param>
            ///  
            ///  <returns>
            ///   True on success. False on failure.
            ///  </returns>            
        public Boolean SendOutputReportViaControlTransfer(SafeFileHandle hidHandle, Byte[] outputReportBuffer) 
            {                 
                Boolean success = false; 
                
                try
                {
                    //  调用API函数: HidD_SetOutputReport
                    success = HidD_SetOutputReport(hidHandle, outputReportBuffer, outputReportBuffer.Length+1);
                    Debug.WriteLine(this.MyDebugging.ResultOfAPICall("HidD_SetOutputReport"));
                    
                    return success;
                }
                catch ( Exception ex )
                {
                    DisplayException( MODULE_NAME, ex );
                    throw ;
                }
            }
 
		///  <summary>
		///  reads an Input report from the device using a control transfer.
		///  </summary>
		///  
		///  <param name="hidHandle"> the handle for learning about the device and exchanging Feature reports. </param>
		///  <param name="myDeviceDetected"> tells whether the device is currently attached. </param>
		///  <param name="inputReportBuffer"> contains the requested report. </param>
		///  <param name="success"> read success </param>
		public Boolean GetInputReportViaControlTransfer(SafeFileHandle hidHandle, ref Byte[] inputReportBuffer)
		{
			Boolean success;

			try
			{
				//  调用API 函数: HidD_GetInputReport
				success = HidD_GetInputReport(hidHandle, inputReportBuffer, inputReportBuffer.Length+1);
                Debug.WriteLine(MyDebugging.ResultOfAPICall("HidD_GetInputReport"));
				return success;
			}
			catch (Exception ex)
			{
				DisplayException(MODULE_NAME, ex);
				throw;
			}
		} 
		        
        ///  <summary>
        ///  sets the number of input reports the host will store.
        ///  Requires Windows XP or later.
        ///  </summary>
        ///  
        ///  <param name="hidDeviceObject"> a handle to the device.</param>
        ///  <param name="numberBuffers"> the requested number of input reports.  </param>
        ///  
        ///  <returns>
        ///  True on success. False on failure.
        ///  </returns>
        public Boolean SetNumberOfInputBuffers(SafeFileHandle hidDeviceObject, Int32 numberBuffers)
            {
                try
                {
                    if (!DeviceManagement.IsWindows98Gold())
                    {
                        //  ***
                        //  调用API函数: HidD_SetNumInputBuffers
                        HidD_SetNumInputBuffers(hidDeviceObject, numberBuffers);
                        Debug.WriteLine(this.MyDebugging.ResultOfAPICall("HidD_SetNumInputBuffers"));
                        return true;
                    }
                    else
                    {
                        //  Not supported under Windows 98 Gold.
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    DisplayException(MODULE_NAME, ex);
                    throw;
                }
            }
                
        ///  <summary>
        ///  Retrieves the number of Input reports the host can store.
        ///  </summary>
        ///  
        ///  <param name="hidDeviceObject"> a handle to a device  </param>
        ///  <param name="numberOfInputBuffers"> an integer to hold the returned value. </param>
        ///  
        ///  <returns>
        ///  True on success, False on failure.
        ///  </returns>
        public Boolean GetNumberOfInputBuffers( SafeFileHandle hidDeviceObject, ref Int32 numberOfInputBuffers ) 
        {             
            Boolean success = false;

            try
            {
                if (!((DeviceManagement.IsWindows98Gold())))
                {
                    //  ***
                    //  调用API函数: HidD_GetNumInputBuffers
                    success = HidD_GetNumInputBuffers(hidDeviceObject, ref numberOfInputBuffers);
                    Debug.WriteLine(this.MyDebugging.ResultOfAPICall("HidD_GetNumInputBuffers"));
                }
                else
                {
                    //  Under Windows 98 Gold, the number of buffers is fixed at 2.

                    numberOfInputBuffers = 2;
                    success = true;
                }

                return success;
            }
            catch (Exception ex)
            {
                DisplayException( MODULE_NAME, ex ); 
                throw ; 
            }                       
        }

        ///  <summary>
        ///  Provides a central mechanism for exception handling.
        ///  Displays a message box that describes the exception.
        ///  </summary>
        ///  
        ///  <param name="moduleName">  the module where the exception occurred. </param>
        ///  <param name="e"> the exception </param>
        public static void DisplayException( String moduleName, Exception e ) 
        {             
            String message = null; 
            String caption = null; 
            
            //  Create an error message.
            message = "Exception: " + e.Message + ControlChars.CrLf + "Module: " + moduleName + ControlChars.CrLf + "Method: " + e.TargetSite.Name; 
            caption = "Unexpected Exception"; 
            
            MessageBox.Show( message, caption, MessageBoxButtons.OK );
            Debug.WriteLine(message);             
        }         
    } 
} 
