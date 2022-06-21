using Microsoft.Win32.SafeHandles;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices; 

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

using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Timers;

using System.Windows.Forms;

namespace Synoxo.USBHidDevice
{
    public class FrmMain  : System.Windows.Forms.Form 
    {         
        #region '"Windows Form Designer generated code "' 
        public FrmMain() : base() 
        {               
            // This call is required by the Windows Form Designer.
            InitializeComponent(); 
        } 
        // Form overrides dispose to clean up the component list.
        protected override void Dispose( bool Disposing ) 
        { 
            if ( Disposing ) 
            { 
                if ( !( components == null ) ) 
                { 
                    components.Dispose(); 
                } 
            } 
            base.Dispose( Disposing ); 
        } 
        
        // Required by the Windows Form Designer
        private System.ComponentModel.IContainer components; 
        public System.Windows.Forms.ToolTip ToolTip1;         
        public System.Windows.Forms.Button cmdContinuous; 
        public System.Windows.Forms.Button cmdOnce; 
        public System.Windows.Forms.GroupBox fraSendAndReceive; 
        public System.Windows.Forms.TextBox txtBytesReceived;
        public System.Windows.Forms.GroupBox fraBytesReceived;
		public System.Windows.Forms.GroupBox fraBytesToSend; 
        public System.Windows.Forms.ListBox lstResults; 
        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.
        // Do not modify it using the code editor.
        public System.Windows.Forms.GroupBox fraReportTypes; 
        public System.Windows.Forms.RadioButton optInputOutput; 
        public System.Windows.Forms.RadioButton optFeature; 
        public System.Windows.Forms.GroupBox fraInputReportBufferSize; 
        public System.Windows.Forms.TextBox txtInputReportBufferSize; 
        public System.Windows.Forms.Button cmdInputReportBufferSize; 
        public System.Windows.Forms.GroupBox fraDeviceIdentifiers; 
        public System.Windows.Forms.Label lblVendorID; 
        public System.Windows.Forms.TextBox txtVendorID; 
        public System.Windows.Forms.Label lblProductID; 
        public System.Windows.Forms.TextBox txtProductID;
		public System.Windows.Forms.Button cmdFindDevice;
        private RichTextBox richTextBox_bytesToSend;
        private Button button_Clear;
        private NumericUpDown numericUpDown_TimerOut;
        private Label label1; 
        public System.Windows.Forms.CheckBox chkUseControlTransfersOnly; 
       
        [ System.Diagnostics.DebuggerStepThrough() ]
        private void InitializeComponent() 
        {
            this.components = new System.ComponentModel.Container();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.fraSendAndReceive = new System.Windows.Forms.GroupBox();
            this.cmdContinuous = new System.Windows.Forms.Button();
            this.cmdOnce = new System.Windows.Forms.Button();
            this.fraBytesReceived = new System.Windows.Forms.GroupBox();
            this.txtBytesReceived = new System.Windows.Forms.TextBox();
            this.fraBytesToSend = new System.Windows.Forms.GroupBox();
            this.richTextBox_bytesToSend = new System.Windows.Forms.RichTextBox();
            this.lstResults = new System.Windows.Forms.ListBox();
            this.fraReportTypes = new System.Windows.Forms.GroupBox();
            this.chkUseControlTransfersOnly = new System.Windows.Forms.CheckBox();
            this.optFeature = new System.Windows.Forms.RadioButton();
            this.optInputOutput = new System.Windows.Forms.RadioButton();
            this.fraInputReportBufferSize = new System.Windows.Forms.GroupBox();
            this.cmdInputReportBufferSize = new System.Windows.Forms.Button();
            this.txtInputReportBufferSize = new System.Windows.Forms.TextBox();
            this.fraDeviceIdentifiers = new System.Windows.Forms.GroupBox();
            this.txtProductID = new System.Windows.Forms.TextBox();
            this.lblProductID = new System.Windows.Forms.Label();
            this.txtVendorID = new System.Windows.Forms.TextBox();
            this.lblVendorID = new System.Windows.Forms.Label();
            this.cmdFindDevice = new System.Windows.Forms.Button();
            this.button_Clear = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDown_TimerOut = new System.Windows.Forms.NumericUpDown();
            this.fraSendAndReceive.SuspendLayout();
            this.fraBytesReceived.SuspendLayout();
            this.fraBytesToSend.SuspendLayout();
            this.fraReportTypes.SuspendLayout();
            this.fraInputReportBufferSize.SuspendLayout();
            this.fraDeviceIdentifiers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_TimerOut)).BeginInit();
            this.SuspendLayout();
            // 
            // fraSendAndReceive
            // 
            this.fraSendAndReceive.BackColor = System.Drawing.SystemColors.Control;
            this.fraSendAndReceive.Controls.Add(this.numericUpDown_TimerOut);
            this.fraSendAndReceive.Controls.Add(this.label1);
            this.fraSendAndReceive.Controls.Add(this.cmdContinuous);
            this.fraSendAndReceive.Controls.Add(this.cmdOnce);
            this.fraSendAndReceive.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fraSendAndReceive.ForeColor = System.Drawing.SystemColors.ControlText;
            this.fraSendAndReceive.Location = new System.Drawing.Point(608, 48);
            this.fraSendAndReceive.Name = "fraSendAndReceive";
            this.fraSendAndReceive.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fraSendAndReceive.Size = new System.Drawing.Size(176, 216);
            this.fraSendAndReceive.TabIndex = 7;
            this.fraSendAndReceive.TabStop = false;
            this.fraSendAndReceive.Text = "Send and Receive Data";
            // 
            // cmdContinuous
            // 
            this.cmdContinuous.BackColor = System.Drawing.SystemColors.Control;
            this.cmdContinuous.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdContinuous.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdContinuous.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdContinuous.Location = new System.Drawing.Point(16, 168);
            this.cmdContinuous.Name = "cmdContinuous";
            this.cmdContinuous.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdContinuous.Size = new System.Drawing.Size(121, 36);
            this.cmdContinuous.TabIndex = 9;
            this.cmdContinuous.Text = "Continuous";
            this.cmdContinuous.UseVisualStyleBackColor = false;
            this.cmdContinuous.Click += new System.EventHandler(this.cmdContinuous_Click);
            // 
            // cmdOnce
            // 
            this.cmdOnce.BackColor = System.Drawing.SystemColors.Control;
            this.cmdOnce.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdOnce.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdOnce.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdOnce.Location = new System.Drawing.Point(16, 92);
            this.cmdOnce.Name = "cmdOnce";
            this.cmdOnce.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdOnce.Size = new System.Drawing.Size(121, 36);
            this.cmdOnce.TabIndex = 8;
            this.cmdOnce.Text = "Once";
            this.cmdOnce.UseVisualStyleBackColor = false;
            this.cmdOnce.Click += new System.EventHandler(this.cmdOnce_Click);
            // 
            // fraBytesReceived
            // 
            this.fraBytesReceived.BackColor = System.Drawing.SystemColors.Control;
            this.fraBytesReceived.Controls.Add(this.txtBytesReceived);
            this.fraBytesReceived.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fraBytesReceived.ForeColor = System.Drawing.SystemColors.ControlText;
            this.fraBytesReceived.Location = new System.Drawing.Point(208, 128);
            this.fraBytesReceived.Name = "fraBytesReceived";
            this.fraBytesReceived.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fraBytesReceived.Size = new System.Drawing.Size(128, 136);
            this.fraBytesReceived.TabIndex = 4;
            this.fraBytesReceived.TabStop = false;
            this.fraBytesReceived.Text = "Bytes Received";
            // 
            // txtBytesReceived
            // 
            this.txtBytesReceived.AcceptsReturn = true;
            this.txtBytesReceived.BackColor = System.Drawing.SystemColors.Window;
            this.txtBytesReceived.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtBytesReceived.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBytesReceived.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtBytesReceived.Location = new System.Drawing.Point(6, 24);
            this.txtBytesReceived.MaxLength = 0;
            this.txtBytesReceived.Multiline = true;
            this.txtBytesReceived.Name = "txtBytesReceived";
            this.txtBytesReceived.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtBytesReceived.Size = new System.Drawing.Size(106, 100);
            this.txtBytesReceived.TabIndex = 5;
            // 
            // fraBytesToSend
            // 
            this.fraBytesToSend.BackColor = System.Drawing.SystemColors.Control;
            this.fraBytesToSend.Controls.Add(this.richTextBox_bytesToSend);
            this.fraBytesToSend.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fraBytesToSend.ForeColor = System.Drawing.SystemColors.ControlText;
            this.fraBytesToSend.Location = new System.Drawing.Point(16, 128);
            this.fraBytesToSend.Name = "fraBytesToSend";
            this.fraBytesToSend.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fraBytesToSend.Size = new System.Drawing.Size(181, 136);
            this.fraBytesToSend.TabIndex = 1;
            this.fraBytesToSend.TabStop = false;
            this.fraBytesToSend.Text = "Bytes to Send";
            // 
            // richTextBox_bytesToSend
            // 
            this.richTextBox_bytesToSend.Location = new System.Drawing.Point(12, 24);
            this.richTextBox_bytesToSend.Name = "richTextBox_bytesToSend";
            this.richTextBox_bytesToSend.Size = new System.Drawing.Size(158, 100);
            this.richTextBox_bytesToSend.TabIndex = 0;
            this.richTextBox_bytesToSend.Text = "";
            // 
            // lstResults
            // 
            this.lstResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstResults.BackColor = System.Drawing.SystemColors.Window;
            this.lstResults.Cursor = System.Windows.Forms.Cursors.Default;
            this.lstResults.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstResults.ForeColor = System.Drawing.SystemColors.WindowText;
            this.lstResults.HorizontalScrollbar = true;
            this.lstResults.ItemHeight = 14;
            this.lstResults.Location = new System.Drawing.Point(12, 275);
            this.lstResults.Name = "lstResults";
            this.lstResults.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lstResults.Size = new System.Drawing.Size(772, 228);
            this.lstResults.TabIndex = 0;
            // 
            // fraReportTypes
            // 
            this.fraReportTypes.Controls.Add(this.chkUseControlTransfersOnly);
            this.fraReportTypes.Controls.Add(this.optFeature);
            this.fraReportTypes.Controls.Add(this.optInputOutput);
            this.fraReportTypes.Location = new System.Drawing.Point(344, 128);
            this.fraReportTypes.Name = "fraReportTypes";
            this.fraReportTypes.Size = new System.Drawing.Size(248, 136);
            this.fraReportTypes.TabIndex = 8;
            this.fraReportTypes.TabStop = false;
            this.fraReportTypes.Text = "Report Options";
            // 
            // chkUseControlTransfersOnly
            // 
            this.chkUseControlTransfersOnly.Location = new System.Drawing.Point(24, 48);
            this.chkUseControlTransfersOnly.Name = "chkUseControlTransfersOnly";
            this.chkUseControlTransfersOnly.Size = new System.Drawing.Size(200, 24);
            this.chkUseControlTransfersOnly.TabIndex = 2;
            this.chkUseControlTransfersOnly.Text = "Use ControlTransfers Only";
            // 
            // optFeature
            // 
            this.optFeature.Location = new System.Drawing.Point(8, 80);
            this.optFeature.Name = "optFeature";
            this.optFeature.Size = new System.Drawing.Size(234, 24);
            this.optFeature.TabIndex = 1;
            this.optFeature.Text = "Exchange Feature Reports";
            // 
            // optInputOutput
            // 
            this.optInputOutput.Checked = true;
            this.optInputOutput.Location = new System.Drawing.Point(8, 24);
            this.optInputOutput.Name = "optInputOutput";
            this.optInputOutput.Size = new System.Drawing.Size(234, 24);
            this.optInputOutput.TabIndex = 0;
            this.optInputOutput.TabStop = true;
            this.optInputOutput.Text = "Exchange Input and Output Reports";
            // 
            // fraInputReportBufferSize
            // 
            this.fraInputReportBufferSize.Controls.Add(this.cmdInputReportBufferSize);
            this.fraInputReportBufferSize.Controls.Add(this.txtInputReportBufferSize);
            this.fraInputReportBufferSize.Location = new System.Drawing.Point(248, 16);
            this.fraInputReportBufferSize.Name = "fraInputReportBufferSize";
            this.fraInputReportBufferSize.Size = new System.Drawing.Size(208, 96);
            this.fraInputReportBufferSize.TabIndex = 9;
            this.fraInputReportBufferSize.TabStop = false;
            this.fraInputReportBufferSize.Text = "Input Report Buffer Size";
            // 
            // cmdInputReportBufferSize
            // 
            this.cmdInputReportBufferSize.Location = new System.Drawing.Point(96, 32);
            this.cmdInputReportBufferSize.Name = "cmdInputReportBufferSize";
            this.cmdInputReportBufferSize.Size = new System.Drawing.Size(96, 56);
            this.cmdInputReportBufferSize.TabIndex = 1;
            this.cmdInputReportBufferSize.Text = "Change Buffer Size";
            this.cmdInputReportBufferSize.Click += new System.EventHandler(this.cmdInputReportBufferSize_Click);
            // 
            // txtInputReportBufferSize
            // 
            this.txtInputReportBufferSize.Location = new System.Drawing.Point(16, 40);
            this.txtInputReportBufferSize.Name = "txtInputReportBufferSize";
            this.txtInputReportBufferSize.Size = new System.Drawing.Size(56, 20);
            this.txtInputReportBufferSize.TabIndex = 0;
            // 
            // fraDeviceIdentifiers
            // 
            this.fraDeviceIdentifiers.Controls.Add(this.txtProductID);
            this.fraDeviceIdentifiers.Controls.Add(this.lblProductID);
            this.fraDeviceIdentifiers.Controls.Add(this.txtVendorID);
            this.fraDeviceIdentifiers.Controls.Add(this.lblVendorID);
            this.fraDeviceIdentifiers.Location = new System.Drawing.Point(16, 16);
            this.fraDeviceIdentifiers.Name = "fraDeviceIdentifiers";
            this.fraDeviceIdentifiers.Size = new System.Drawing.Size(208, 96);
            this.fraDeviceIdentifiers.TabIndex = 10;
            this.fraDeviceIdentifiers.TabStop = false;
            this.fraDeviceIdentifiers.Text = "Device Identifiers";
            // 
            // txtProductID
            // 
            this.txtProductID.Location = new System.Drawing.Point(120, 56);
            this.txtProductID.Name = "txtProductID";
            this.txtProductID.Size = new System.Drawing.Size(72, 20);
            this.txtProductID.TabIndex = 3;
            this.txtProductID.Text = "2013";
            this.txtProductID.TextChanged += new System.EventHandler(this.txtProductID_TextChanged);
            // 
            // lblProductID
            // 
            this.lblProductID.Location = new System.Drawing.Point(16, 56);
            this.lblProductID.Name = "lblProductID";
            this.lblProductID.Size = new System.Drawing.Size(112, 23);
            this.lblProductID.TabIndex = 2;
            this.lblProductID.Text = "Product ID (hex):";
            // 
            // txtVendorID
            // 
            this.txtVendorID.Location = new System.Drawing.Point(120, 24);
            this.txtVendorID.Name = "txtVendorID";
            this.txtVendorID.Size = new System.Drawing.Size(72, 20);
            this.txtVendorID.TabIndex = 1;
            this.txtVendorID.Text = "03EB";
            this.txtVendorID.TextChanged += new System.EventHandler(this.txtVendorID_TextChanged);
            // 
            // lblVendorID
            // 
            this.lblVendorID.Location = new System.Drawing.Point(16, 24);
            this.lblVendorID.Name = "lblVendorID";
            this.lblVendorID.Size = new System.Drawing.Size(112, 23);
            this.lblVendorID.TabIndex = 0;
            this.lblVendorID.Text = "Vendor ID (hex):";
            // 
            // cmdFindDevice
            // 
            this.cmdFindDevice.Location = new System.Drawing.Point(464, 48);
            this.cmdFindDevice.Name = "cmdFindDevice";
            this.cmdFindDevice.Size = new System.Drawing.Size(136, 40);
            this.cmdFindDevice.TabIndex = 11;
            this.cmdFindDevice.Text = "Find My Device";
            this.cmdFindDevice.Click += new System.EventHandler(this.cmdFindDevice_Click);
            // 
            // button_Clear
            // 
            this.button_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button_Clear.Location = new System.Drawing.Point(325, 515);
            this.button_Clear.Name = "button_Clear";
            this.button_Clear.Size = new System.Drawing.Size(94, 31);
            this.button_Clear.TabIndex = 12;
            this.button_Clear.Text = "Clear";
            this.button_Clear.UseVisualStyleBackColor = true;
            this.button_Clear.Click += new System.EventHandler(this.button_Clear_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(19, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 19);
            this.label1.TabIndex = 10;
            this.label1.Text = "Timeout";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numericUpDown_TimerOut
            // 
            this.numericUpDown_TimerOut.Location = new System.Drawing.Point(22, 59);
            this.numericUpDown_TimerOut.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown_TimerOut.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDown_TimerOut.Name = "numericUpDown_TimerOut";
            this.numericUpDown_TimerOut.Size = new System.Drawing.Size(115, 20);
            this.numericUpDown_TimerOut.TabIndex = 11;
            this.numericUpDown_TimerOut.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown_TimerOut.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // FrmMain
            // 
            this.ClientSize = new System.Drawing.Size(800, 561);
            this.Controls.Add(this.button_Clear);
            this.Controls.Add(this.cmdFindDevice);
            this.Controls.Add(this.fraDeviceIdentifiers);
            this.Controls.Add(this.fraInputReportBufferSize);
            this.Controls.Add(this.fraReportTypes);
            this.Controls.Add(this.fraSendAndReceive);
            this.Controls.Add(this.fraBytesReceived);
            this.Controls.Add(this.fraBytesToSend);
            this.Controls.Add(this.lstResults);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Location = new System.Drawing.Point(21, 28);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Generic HID Tester";
            this.Closed += new System.EventHandler(this.frmMain_Closed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.fraSendAndReceive.ResumeLayout(false);
            this.fraBytesReceived.ResumeLayout(false);
            this.fraBytesReceived.PerformLayout();
            this.fraBytesToSend.ResumeLayout(false);
            this.fraReportTypes.ResumeLayout(false);
            this.fraInputReportBufferSize.ResumeLayout(false);
            this.fraInputReportBufferSize.PerformLayout();
            this.fraDeviceIdentifiers.ResumeLayout(false);
            this.fraDeviceIdentifiers.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_TimerOut)).EndInit();
            this.ResumeLayout(false);

        }         
        #endregion 
        
        //private IntPtr deviceNotificationHandle; 
        //private Boolean exclusiveAccess;
        //private FileStream fileStreamDeviceData;
        //private SafeFileHandle hidHandle; 
        //private String hidUsage; 
        //private Boolean myDeviceDetected; 
        //private String myDevicePathName;
        //private Boolean transferInProgress = false;
        
        private DeviceManagement MyDeviceManagement = new DeviceManagement(); 

		private static System.Timers.Timer tmrReadTimeout;
		private static System.Timers.Timer tmrContinuousDataCollect;
    
        public FrmMain FrmMy; 
        
        //  This delegate has the same parameters as AccessForm.
        //  Used in accessing the application's form from a different thread.
        private delegate void MarshalToForm( String action, String textToAdd );
        
        ///  <summary>
        ///  Uses a series of API calls to locate a HID-class device
        ///  by its Vendor ID and Product ID.
        ///  </summary>
        ///  <returns>
        ///   True if the device is detected, False if not detected.
        ///  </returns>
        private Boolean FindTheHid() 
        {
            int myVendorID =   0x03EB;
            int myProductID = 0x2013;
            if (this.txtVendorID.Text != null && this.txtProductID.Text != null)
            {
                int vid = 0;
                int pid = 0;
                try
                {
                    vid = Convert.ToInt32(this.txtVendorID.Text,16);
                    pid = Convert.ToInt32(this.txtProductID.Text,16);
                    myVendorID = vid;
                    myProductID = pid;
                }
                catch (SystemException e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return this.MyDeviceManagement.findHidDevices(ref myVendorID, ref myProductID);//, this);
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
        ///  Performs various application-specific functions that
        ///  involve accessing the application's form.
        ///  </summary>
        ///  <param name="action"> a String that names the action to perform on the form</param>
        ///  <param name="formText"> text that the form displays or the code uses for 
        ///  another purpose. Actions that don't use text ignore this parameter. </param>
        private void AccessForm( String action, String formText ) 
        {             
            try 
            { 
                //  Select an action to perform on the form:
                switch ( action ) 
                {
                    case "AddItemToListBox":
                        lstResults.Items.Add( formText ); 
                        
                        break;
                    case "AddItemToTextBox":
                        txtBytesReceived.SelectedText = formText + "\r\n"; 
                        
                        break;
                    case "EnableCmdOnce":
                        //  If it's a single transfer, re-enable the command button.
                        if ( cmdContinuous.Text == "Continuous" ) 
                        { 
                            cmdOnce.Enabled = true; 
                        }                         
                        break;
                    case "ScrollToBottomOfListBox":
                        lstResults.SelectedIndex = lstResults.Items.Count - 1; 
                        
                        break;
                    case "TextBoxSelectionStart":
                        txtBytesReceived.SelectionStart = formText.Length; 
                        
                        break;
                    default:
                        
                        break;
                } 
            } 
            catch ( Exception ex ) 
            { 
                DisplayException( this.Name, ex ); 
                throw ; 
            }             
        }         
        
		/// <summary>
		/// Close the handle and FileStreams for a device.
		/// </summary>
		private void CloseCommunications()
		{
            this.MyDeviceManagement.CloseCommunications();
		}

         ///  <summary>
        ///  Start or stop a series of periodic transfers.
        ///  </summary>
        private void cmdContinuous_Click( System.Object eventSender, System.EventArgs eventArgs ) 
        {             
            try 
            { 
                if ( cmdContinuous.Text == "Continuous" ) 
                {                     
                    //  Start doing periodic transfers.
                    if ( !( cmdOnce.Enabled ) ) 
                    {                         
                        AccessForm( "AddItemToListBox", "A previous transfer hasn't completed. Please try again." ); 
                    } 
                    else 
                    {                         
                        cmdOnce.Enabled = false; 
                        //  Change the command button's text to "Cancel Continuous"
                        cmdContinuous.Text = "Cancel Continuous"; 
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
                    cmdContinuous.Text = "Continuous"; 
                    // D isable the timer that triggers the transfers.
					tmrContinuousDataCollect.Enabled = false;
					tmrContinuousDataCollect.Stop(); 
                    cmdOnce.Enabled = true; 
                }                 
            } 
            catch ( Exception ex ) 
            { 
                DisplayException( this.Name, ex ); 
                throw ; 
            } 
        }         
        
        ///  <summary>
        ///  Set the number of Input reports the host will store.
        ///  </summary>
        private void cmdInputReportBufferSize_Click( System.Object sender, System.EventArgs e ) 
        {             
            try 
            { 
                SetInputReportBufferSize();                 
            } 
            catch ( Exception ex ) 
            { 
                DisplayException( this.Name, ex ); 
                throw ; 
            } 
        } 
                
        ///  <summary>
        ///  Search for a specific device.
        ///  </summary>
        private void cmdFindDevice_Click( System.Object sender, System.EventArgs e ) 
        {            
            try 
            {
                if (FindTheHid())
                {
                    this.lstResults.Items.Add("发现设备");
                    for (int i = 0; i < this.MyDeviceManagement.DeviceCount; i++)
                    {
                        this.lstResults.Items.Add(this.MyDeviceManagement[i].myDevicePathName);
                    }
                }
            } 
            catch ( Exception ex ) 
            { 
                DisplayException( this.Name, ex ); 
                throw ; 
            }             
        }         
        
        ///  <summary>
        ///  Attempt to write a report and read a report.
        ///  </summary>
        private void cmdOnce_Click( System.Object eventSender, System.EventArgs eventArgs ) 
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
            catch ( Exception ex ) 
            { 
                DisplayException( this.Name, ex ); 
            }
            this.cmdOnce.Enabled = true;
        }        
        
        ///  <summary>
        ///  Called if the user changes the Vendor ID or Product ID in the text box.
        ///  </summary>
        private void DeviceHasChanged() 
        {             
            try 
            {
                this.MyDeviceManagement.StopReceiveDeviceNotificationHandle();
             } 
            catch ( Exception ex ) 
            { 
                DisplayException( this.Name, ex ); 
                throw ; 
            } 
        }         
        
        ///  <summary>
        ///  Sends a Feature report, then retrieves one.
        ///  Assumes report ID = 0 for both reports.
        ///  </summary>
        private void ExchangeFeatureReports() 
        {             
            byte[] outDatas=new byte[3];
            outDatas[0] = 0x55;
            outDatas[1] = 0x1;
            outDatas[2] = 0x0;
            byte[] inDatas = new byte[5];
            this.MyDeviceManagement.InputAndOutputFeatureReports(0, new byte[3], ref inDatas);
        }         
        
        ///  <summary>
        ///  Sends an Output report, then retrieves an Input report.
        ///  Assumes report ID = 0 for both reports.
        ///  </summary>
        private byte[] ExchangeInputAndOutputReports() 
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
            this.txtBytesReceived.Clear();
            if (this.optInputOutput.Checked)
                this.MyDeviceManagement.InputAndOutputReports(0, this.chkUseControlTransfersOnly.Checked, outdatas, ref inputdatas, (int)this.numericUpDown_TimerOut.Value);
            else
                this.MyDeviceManagement.InputAndOutputFeatureReports( 0, outdatas, ref inputdatas);

            this.txtBytesReceived.Text = this.BytesToString(inputdatas, 0, inputdatas.Length, "0x", " ", 16);
            return inputdatas;
        }         
        
        ///  <summary>
        ///  Perform shutdown operations.
        ///  </summary>
        private void frmMain_Closed( System.Object eventSender, System.EventArgs eventArgs ) 
        {             
            try 
            { 
                Shutdown();                 
            } 
            catch ( Exception ex ) 
            { 
                DisplayException( this.Name, ex ); 
                throw ; 
            } 
        }         
        
        ///  <summary>
        ///  Perform startup operations.
        ///  </summary>
        private void frmMain_Load( System.Object eventSender, System.EventArgs eventArgs ) 
        {             
            try 
            { 
                FrmMy = this; 
                Startup();                 
            } 
            catch ( Exception ex ) 
            { 
                DisplayException( this.Name, ex ); 
                throw ; 
            }             
        }         
        
        ///  <summary>
        ///  Retrieves a Vendor ID and Product ID in hexadecimal 
        ///  from the form's text boxes and converts the text to Int16s.
        ///  </summary>
        ///  <param name="myVendorID"> the Vendor ID</param>
        ///  <param name="myProductID"> the Product ID</param>
		private void GetVendorAndProductIDsFromTextBoxes(ref Int32 myVendorID, ref Int32 myProductID) 
        {             
            try 
            {
				myVendorID = Int32.Parse(txtVendorID.Text, NumberStyles.AllowHexSpecifier); 
				myProductID = Int32.Parse(txtProductID.Text, NumberStyles.AllowHexSpecifier);                
            } 
            catch ( Exception ex ) 
            { 
                DisplayException( this.Name, ex ); 
                throw ; 
            }             
        }         
        
        ///  <summary>
        ///  Initialize the elements on the form.
        ///  </summary>
        private void InitializeDisplay() 
        {           
            try 
            { 
                //  Don't allow the user to select an input report buffer size until there is
                //  a handle to a HID.
                cmdInputReportBufferSize.Focus(); 
                cmdInputReportBufferSize.Enabled = false; 
                
                if ( DeviceManagement.IsWindowsXpOrLater() ) 
                {                     
                    chkUseControlTransfersOnly.Enabled = true;                     
                } 
                else 
                { 
                    //  If the operating system is earlier than Windows XP,
                    //  disable the option to force Input and Output reports to use control transfers.
                    chkUseControlTransfersOnly.Enabled = false;                     
                } 
                lstResults.Items.Add( "For a more detailed event log, view debug statements in Visual Studio's Output window:" ); 
                lstResults.Items.Add( "Click Build > Configuration Manager > Active Solution Configuration > Debug > Close." ); 
                lstResults.Items.Add( "Then click View > Output." ); 
                lstResults.Items.Add( "" );                 
            } 
            catch ( Exception ex ) 
            { 
                DisplayException( this.Name, ex ); 
                throw ; 
            }             
        }         
        
        ///  <summary>
        ///  Enables accessing a form's controls from another thread 
        ///  </summary>
        ///  <param name="action"> a String that names the action to perform on the form </param>
        ///  <param name="textToDisplay"> text that the form displays or the code uses for 
        ///  another purpose. Actions that don't use text ignore this parameter.  </param>
        private void MyMarshalToForm( String action, String textToDisplay ) 
        {             
            object[] args = { action, textToDisplay }; 
            MarshalToForm MarshalToFormDelegate = null; 
            //  The AccessForm routine contains the code that accesses the form.
            MarshalToFormDelegate = new MarshalToForm( AccessForm ); 
            //  Execute AccessForm, passing the parameters in args.
            base.Invoke( MarshalToFormDelegate, args );             
        }

		///  <summary>
		///  Exchange data with the device.
		///  </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		///  <remarks>
		///  The timer is enabled only if cmdContinous has been clicked, 
		///  selecting continous (periodic) transfers.
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
				DisplayException(this.Name, ex);
				throw;
			}
		}
		
		/// <summary>
		/// ystem timer timeout if read via interrupt transfer doesn't return.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		private void OnReadTimeout(object source, ElapsedEventArgs e)
		{
			MyMarshalToForm("AddItemToListBox", "The attempt to read a report timed out.");
			CloseCommunications();
			tmrReadTimeout.Stop();
			//  Enable requesting another transfer.
			MyMarshalToForm("EnableCmdOnce", "");
			MyMarshalToForm("ScrollToBottomOfListBox", ""); 			
		}
		        
        ///  <summary>
        ///  Initiates exchanging reports. 
        ///  The application sends a report and requests to read a report.
        ///  </summary>
        private void ReadAndWriteToDevice() 
        {
            this.ExchangeInputAndOutputReports();
        } 
                
        ///  <summary>
        ///  Scroll to the bottom of the list box and trim as needed.
        ///  </summary>
        private void ScrollToBottomOfListBox() 
        {            
            try 
            { 
                Int32 count = 0; 
                lstResults.SelectedIndex = lstResults.Items.Count - 1; 
                //  If the list box is getting too large, trim its contents by removing the earliest data.
                if ( lstResults.Items.Count > 1000 ) 
                {                     
                    for ( count=1; count <= 500; count++ ) 
                    { 
                        lstResults.Items.RemoveAt( 4 ); 
                    }                    
                }                 
            } 
            catch ( Exception ex ) 
            { 
                DisplayException( this.Name, ex ); 
                throw ; 
            } 
        } 
                
        ///  <summary>
        ///  Set the number of Input buffers (the number of Input reports 
        ///  the host will store) from the value in the text box.
        ///  </summary>
        private void SetInputReportBufferSize() 
        {
            this.MyDeviceManagement.SetInputReportBufferSize(this.MyDeviceManagement[0], Convert.ToInt32(this.txtInputReportBufferSize.Text));
        }         
        
        ///  <summary>
        ///  Perform actions that must execute when the program ends.
        ///  </summary>
        private void Shutdown() 
        {
            this.MyDeviceManagement.ShutDown();
        } 
                
        ///  <summary>
        ///  Perform actions that must execute when the program starts.
        ///  </summary>
        private void Startup() 
        {            
            try 
            {
                InitializeDisplay();			
								
				tmrContinuousDataCollect = new System.Timers.Timer(1000);
				tmrContinuousDataCollect.Elapsed  += new ElapsedEventHandler(OnDataCollect);
				tmrContinuousDataCollect.Stop();
				tmrContinuousDataCollect.SynchronizingObject = this;

				tmrReadTimeout = new System.Timers.Timer(5000);			
				tmrReadTimeout.Elapsed += new ElapsedEventHandler(OnReadTimeout);
                tmrReadTimeout.SynchronizingObject = this;
				tmrReadTimeout.Stop();
                
                //  Default USB Vendor ID and Product ID:
                //this.MyDeviceManagement.RegisterForDeviceNotifications(
                txtVendorID.Text = "16C0";//"03EB"; 
                txtProductID.Text = "05DF";

                this.MyDeviceManagement.WhenUsbEvent += new DeviceManagement.usbEventsHandler(MyDeviceManagement_WhenUsbEvent);
            } 
            catch ( Exception ex ) 
            { 
                DisplayException( this.Name, ex ); 
                throw ; 
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
                this.lstResults.Items.Add(this.MyDeviceManagement[e.DeviceIndex].myDevicePathName + this.MyDeviceManagement[e.DeviceIndex].HidUsage + statur);
        }

        ///  <summary>
        ///  The Product ID has changed in the text box. Call a routine to handle it.
        ///  </summary>
        private void txtProductID_TextChanged( System.Object sender, System.EventArgs e ) 
        {            
            try 
            { 
                DeviceHasChanged();                 
            } 
            catch ( Exception ex ) 
            { 
                DisplayException( this.Name, ex ); 
                throw ; 
            } 
        } 
                
        ///  <summary>
        ///  The Vendor ID has changed in the text box. Call a routine to handle it.
        ///  </summary>
        private void txtVendorID_TextChanged( System.Object sender, System.EventArgs e ) 
        {            
            try 
            { 
                DeviceHasChanged();                 
            } 
            catch ( Exception ex ) 
            { 
                DisplayException( this.Name, ex ); 
                throw ; 
            }             
        }         
        
        ///  <summary>
        ///  Finalize method.
        ///  </summary>
        ~FrmMain() 
        { 
        } 
                
        ///  <summary>
        ///  Provides a central mechanism for exception handling.
        ///  Displays a message box that describes the exception.
        ///  </summary>
        ///  <param name="moduleName"> the module where the exception occurred. </param>
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

        private void button_Clear_Click(object sender, EventArgs e)
        {
            this.lstResults.Items.Clear();
        } 
    }      
} 
