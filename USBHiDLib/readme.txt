///<summary>
/// Project: GenericHid
/// 
/// ***********************************************************************
/// Software License Agreement
///
/// Licensor grants any person obtaining a copy of this software ("You") 
/// a worldwide, royalty-free, non-exclusive license, for the duration of 
/// the copyright, free of charge, to store and execute the Software in a 
/// computer system and to incorporate the Software or any portion of it 
/// in computer programs You write.   
/// 
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
/// THE SOFTWARE.
/// ***********************************************************************
/// 
/// Author             
/// Jan Axelson        
/// 
/// This software was written using Visual Studio 2008 Standard Edition and building   
/// for the .NET Framework 2.0.
/// 
/// Purpose: 
/// Demonstrates USB communications with a generic HID-class device
/// 
/// Requirements:
/// Windows 98 or later and an attached USB generic Human Interface Device (HID).
/// 
/// Description:
/// Finds an attached device that matches the vendor and product IDs in the form's 
/// text boxes.
/// 
/// Retrieves the device's capabilities.
/// Sends and requests HID reports.
/// 
/// Uses RegisterDeviceNotification() and WM_DEVICE_CHANGE messages
/// to detect when a device is attached or removed.
/// RegisterDeviceNotification doesn't work under Windows 98 (not sure why).
/// 
/// A list box displays the data sent and received,
/// along with error and status messages.
/// Combo boxes select data to send, and 1-time or timed, periodic transfers.
/// 
/// You can change the size of the host's Input report buffer and request to use control
/// transfers only to exchange Input and Output reports.
/// 
/// To view additional debugging messages, in the Visual Studio development environment,
/// select the Debug build (Build > Configuration Manager > Active Solution Configuration)
/// and view the Output window (View > Other Windows > Output)
/// 
/// The application uses an asynchronous FileStream to read
/// Input reports asynchronously, so the application's main thread doesn't have to
/// wait for the device to return an Input report when the HID driver's buffer is empty. 
/// 
/// For code that finds a device and opens handles to it, see the FindTheHid routine in frmMain.cs.
/// For code that reads from the device, search for fileStreamDeviceData.BeginRead and GetInputReportData
/// in FrmMain.cs and GetInputReportViaControlTransfer and GetFeatureReport in Hid.cs.
/// For code that writes to the device, search for fileStreamDeviceData.Write in FrmMain.cs and 
/// SendOutputReportViaControlTransfer and SendFeatureReport in Hid.cs.
/// 
/// This project includes the following modules:
/// 
/// GenericHid.cs - runs the application.
/// FrmMain.cs - routines specific to the form.
/// Hid.cs - routines specific to HID communications.
/// DeviceManagement.cs - routines for obtaining a handle to a device from its GUID
/// and receiving device notifications. This routines are not specific to HIDs.
/// Debugging.cs - contains a routine for displaying API error messages.
/// HidDeclarations.cs - Declarations for API functions used by Hid.cs.
/// FileIODeclarations.cs - Declarations for file-related API functions.
/// DeviceManagementDeclarations.cs - Declarations for API functions used by DeviceManagement.cs.
/// DebuggingDeclarations.cs - Declarations for API functions used by Debugging.cs.
/// 
/// Companion device firmware for several device CPUs is available from www.Lvr.com/hidpage.htm
/// You can use any generic HID (not a system mouse or keyboard) that sends and receives reports.
/// 
/// V5.0
/// 3/30/11
/// Replaced ReadFile and WriteFile with FileStreams. Thanks to Joe Dunne and John on my Ports forum for tips on this.
/// Simplified Hid.cs.
/// Replaced the form timer with a system timer.
/// 
/// V4.6
/// 1/12/10
/// Supports Vendor IDs and Product IDs up to FFFFh.
///
/// V4.52
/// 11/10/09
/// Changed HIDD_ATTRIBUTES to use UInt16
/// 
/// V4.51
/// 2/11/09
/// Moved Free_ and similar to Finally blocks to ensure they execute.
/// 
/// V4.5
/// 2/9/09
/// Changes to support 64-bit systems, memory management, and other corrections. 
/// Big thanks to Peter Nielsen.
/// 
/// For more information about HIDs and USB, and additional example device firmware to use
/// with this application, visit Lakeview Research at http://www.Lvr.com .
/// Send comments, bug reports, etc. to jan@Lvr.com 
/// 
/// </summary>