namespace Synoxo.USBHidDevice
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cmdFindDevice = new System.Windows.Forms.Button();
            this.fraDeviceIdentifiers = new System.Windows.Forms.GroupBox();
            this.comboBox_DeviceList = new System.Windows.Forms.ComboBox();
            this.txtProductID = new System.Windows.Forms.TextBox();
            this.lblProductID = new System.Windows.Forms.Label();
            this.txtVendorID = new System.Windows.Forms.TextBox();
            this.lblVendorID = new System.Windows.Forms.Label();
            this.fraInputReportBufferSize = new System.Windows.Forms.GroupBox();
            this.cmdInputReportBufferSize = new System.Windows.Forms.Button();
            this.txtInputReportBufferSize = new System.Windows.Forms.TextBox();
            this.fraReportTypes = new System.Windows.Forms.GroupBox();
            this.chkUseControlTransfersOnly = new System.Windows.Forms.CheckBox();
            this.optFeature = new System.Windows.Forms.RadioButton();
            this.optInputOutput = new System.Windows.Forms.RadioButton();
            this.fraSendAndReceive = new System.Windows.Forms.GroupBox();
            this.button_Exit = new System.Windows.Forms.Button();
            this.numericUpDown_TimerOut = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdContinuous = new System.Windows.Forms.Button();
            this.cmdOnce = new System.Windows.Forms.Button();
            this.fraBytesReceived = new System.Windows.Forms.GroupBox();
            this.txtBytesReceived = new System.Windows.Forms.TextBox();
            this.fraBytesToSend = new System.Windows.Forms.GroupBox();
            this.richTextBox_bytesToSend = new System.Windows.Forms.RichTextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.richTextBox_Msg = new System.Windows.Forms.RichTextBox();
            this.fraDeviceIdentifiers.SuspendLayout();
            this.fraInputReportBufferSize.SuspendLayout();
            this.fraReportTypes.SuspendLayout();
            this.fraSendAndReceive.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_TimerOut)).BeginInit();
            this.fraBytesReceived.SuspendLayout();
            this.fraBytesToSend.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdFindDevice
            // 
            this.cmdFindDevice.Location = new System.Drawing.Point(12, 97);
            this.cmdFindDevice.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdFindDevice.Name = "cmdFindDevice";
            this.cmdFindDevice.Size = new System.Drawing.Size(104, 29);
            this.cmdFindDevice.TabIndex = 18;
            this.cmdFindDevice.Text = "Find My Device";
            this.cmdFindDevice.Click += new System.EventHandler(this.cmdFindDevice_Click);
            // 
            // fraDeviceIdentifiers
            // 
            this.fraDeviceIdentifiers.Controls.Add(this.comboBox_DeviceList);
            this.fraDeviceIdentifiers.Controls.Add(this.cmdFindDevice);
            this.fraDeviceIdentifiers.Controls.Add(this.txtProductID);
            this.fraDeviceIdentifiers.Controls.Add(this.lblProductID);
            this.fraDeviceIdentifiers.Controls.Add(this.txtVendorID);
            this.fraDeviceIdentifiers.Controls.Add(this.lblVendorID);
            this.fraDeviceIdentifiers.Location = new System.Drawing.Point(14, 15);
            this.fraDeviceIdentifiers.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.fraDeviceIdentifiers.Name = "fraDeviceIdentifiers";
            this.fraDeviceIdentifiers.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.fraDeviceIdentifiers.Size = new System.Drawing.Size(243, 134);
            this.fraDeviceIdentifiers.TabIndex = 17;
            this.fraDeviceIdentifiers.TabStop = false;
            this.fraDeviceIdentifiers.Text = "Device Identifiers";
            // 
            // comboBox_DeviceList
            // 
            this.comboBox_DeviceList.FormattingEnabled = true;
            this.comboBox_DeviceList.Location = new System.Drawing.Point(122, 96);
            this.comboBox_DeviceList.Name = "comboBox_DeviceList";
            this.comboBox_DeviceList.Size = new System.Drawing.Size(115, 25);
            this.comboBox_DeviceList.TabIndex = 2;
            // 
            // txtProductID
            // 
            this.txtProductID.Location = new System.Drawing.Point(140, 51);
            this.txtProductID.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtProductID.Name = "txtProductID";
            this.txtProductID.Size = new System.Drawing.Size(83, 25);
            this.txtProductID.TabIndex = 3;
            this.txtProductID.Text = "0x5850";
            this.txtProductID.TextChanged += new System.EventHandler(this.txtProductID_TextChanged);
            // 
            // lblProductID
            // 
            this.lblProductID.Location = new System.Drawing.Point(19, 51);
            this.lblProductID.Name = "lblProductID";
            this.lblProductID.Size = new System.Drawing.Size(115, 21);
            this.lblProductID.TabIndex = 2;
            this.lblProductID.Text = "Product ID (hex):";
            this.lblProductID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtVendorID
            // 
            this.txtVendorID.Location = new System.Drawing.Point(140, 22);
            this.txtVendorID.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtVendorID.Name = "txtVendorID";
            this.txtVendorID.Size = new System.Drawing.Size(83, 25);
            this.txtVendorID.TabIndex = 1;
            this.txtVendorID.Text = "0x0683";
            this.txtVendorID.TextChanged += new System.EventHandler(this.txtVendorID_TextChanged);
            // 
            // lblVendorID
            // 
            this.lblVendorID.Location = new System.Drawing.Point(19, 22);
            this.lblVendorID.Name = "lblVendorID";
            this.lblVendorID.Size = new System.Drawing.Size(115, 21);
            this.lblVendorID.TabIndex = 0;
            this.lblVendorID.Text = "Vendor ID (hex):";
            this.lblVendorID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // fraInputReportBufferSize
            // 
            this.fraInputReportBufferSize.Controls.Add(this.cmdInputReportBufferSize);
            this.fraInputReportBufferSize.Controls.Add(this.txtInputReportBufferSize);
            this.fraInputReportBufferSize.Location = new System.Drawing.Point(263, 15);
            this.fraInputReportBufferSize.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.fraInputReportBufferSize.Name = "fraInputReportBufferSize";
            this.fraInputReportBufferSize.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.fraInputReportBufferSize.Size = new System.Drawing.Size(170, 134);
            this.fraInputReportBufferSize.TabIndex = 16;
            this.fraInputReportBufferSize.TabStop = false;
            this.fraInputReportBufferSize.Text = "Input Report Buffer Size";
            // 
            // cmdInputReportBufferSize
            // 
            this.cmdInputReportBufferSize.Location = new System.Drawing.Point(6, 77);
            this.cmdInputReportBufferSize.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdInputReportBufferSize.Name = "cmdInputReportBufferSize";
            this.cmdInputReportBufferSize.Size = new System.Drawing.Size(153, 29);
            this.cmdInputReportBufferSize.TabIndex = 1;
            this.cmdInputReportBufferSize.Text = "Change Buffer Size";
            this.cmdInputReportBufferSize.Click += new System.EventHandler(this.cmdInputReportBufferSize_Click);
            // 
            // txtInputReportBufferSize
            // 
            this.txtInputReportBufferSize.Location = new System.Drawing.Point(28, 30);
            this.txtInputReportBufferSize.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtInputReportBufferSize.Name = "txtInputReportBufferSize";
            this.txtInputReportBufferSize.Size = new System.Drawing.Size(97, 25);
            this.txtInputReportBufferSize.TabIndex = 0;
            // 
            // fraReportTypes
            // 
            this.fraReportTypes.Controls.Add(this.chkUseControlTransfersOnly);
            this.fraReportTypes.Controls.Add(this.optFeature);
            this.fraReportTypes.Controls.Add(this.optInputOutput);
            this.fraReportTypes.Location = new System.Drawing.Point(448, 15);
            this.fraReportTypes.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.fraReportTypes.Name = "fraReportTypes";
            this.fraReportTypes.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.fraReportTypes.Size = new System.Drawing.Size(289, 134);
            this.fraReportTypes.TabIndex = 15;
            this.fraReportTypes.TabStop = false;
            this.fraReportTypes.Text = "Report Options";
            // 
            // chkUseControlTransfersOnly
            // 
            this.chkUseControlTransfersOnly.Location = new System.Drawing.Point(28, 60);
            this.chkUseControlTransfersOnly.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkUseControlTransfersOnly.Name = "chkUseControlTransfersOnly";
            this.chkUseControlTransfersOnly.Size = new System.Drawing.Size(233, 30);
            this.chkUseControlTransfersOnly.TabIndex = 2;
            this.chkUseControlTransfersOnly.Text = "只使用控制模式";
            // 
            // optFeature
            // 
            this.optFeature.Location = new System.Drawing.Point(6, 96);
            this.optFeature.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.optFeature.Name = "optFeature";
            this.optFeature.Size = new System.Drawing.Size(326, 30);
            this.optFeature.TabIndex = 1;
            this.optFeature.Text = "用Feqture Report 函数发送数据和读取相应";
            // 
            // optInputOutput
            // 
            this.optInputOutput.Checked = true;
            this.optInputOutput.Location = new System.Drawing.Point(9, 30);
            this.optInputOutput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.optInputOutput.Name = "optInputOutput";
            this.optInputOutput.Size = new System.Drawing.Size(273, 30);
            this.optInputOutput.TabIndex = 0;
            this.optInputOutput.TabStop = true;
            this.optInputOutput.Text = "用Report函数发送数据和读取相应";
            // 
            // fraSendAndReceive
            // 
            this.fraSendAndReceive.BackColor = System.Drawing.SystemColors.Control;
            this.fraSendAndReceive.Controls.Add(this.button_Exit);
            this.fraSendAndReceive.Controls.Add(this.numericUpDown_TimerOut);
            this.fraSendAndReceive.Controls.Add(this.label1);
            this.fraSendAndReceive.Controls.Add(this.cmdContinuous);
            this.fraSendAndReceive.Controls.Add(this.cmdOnce);
            this.fraSendAndReceive.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fraSendAndReceive.ForeColor = System.Drawing.SystemColors.ControlText;
            this.fraSendAndReceive.Location = new System.Drawing.Point(17, 335);
            this.fraSendAndReceive.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.fraSendAndReceive.Name = "fraSendAndReceive";
            this.fraSendAndReceive.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.fraSendAndReceive.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fraSendAndReceive.Size = new System.Drawing.Size(720, 60);
            this.fraSendAndReceive.TabIndex = 14;
            this.fraSendAndReceive.TabStop = false;
            this.fraSendAndReceive.Text = "Send and Receive Data";
            // 
            // button_Exit
            // 
            this.button_Exit.Location = new System.Drawing.Point(592, 20);
            this.button_Exit.Name = "button_Exit";
            this.button_Exit.Size = new System.Drawing.Size(122, 28);
            this.button_Exit.TabIndex = 12;
            this.button_Exit.Text = "退出";
            this.button_Exit.UseVisualStyleBackColor = true;
            this.button_Exit.Click += new System.EventHandler(this.button_Exit_Click);
            // 
            // numericUpDown_TimerOut
            // 
            this.numericUpDown_TimerOut.Location = new System.Drawing.Point(82, 20);
            this.numericUpDown_TimerOut.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
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
            this.numericUpDown_TimerOut.Size = new System.Drawing.Size(78, 23);
            this.numericUpDown_TimerOut.TabIndex = 11;
            this.numericUpDown_TimerOut.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown_TimerOut.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 24);
            this.label1.TabIndex = 10;
            this.label1.Text = "Timeout";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmdContinuous
            // 
            this.cmdContinuous.BackColor = System.Drawing.SystemColors.Control;
            this.cmdContinuous.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdContinuous.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdContinuous.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdContinuous.Location = new System.Drawing.Point(370, 20);
            this.cmdContinuous.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdContinuous.Name = "cmdContinuous";
            this.cmdContinuous.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdContinuous.Size = new System.Drawing.Size(141, 30);
            this.cmdContinuous.TabIndex = 9;
            this.cmdContinuous.Text = "连续发送";
            this.cmdContinuous.UseVisualStyleBackColor = false;
            this.cmdContinuous.Click += new System.EventHandler(this.cmdContinuous_Click);
            // 
            // cmdOnce
            // 
            this.cmdOnce.BackColor = System.Drawing.SystemColors.Control;
            this.cmdOnce.Cursor = System.Windows.Forms.Cursors.Default;
            this.cmdOnce.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdOnce.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdOnce.Location = new System.Drawing.Point(205, 20);
            this.cmdOnce.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdOnce.Name = "cmdOnce";
            this.cmdOnce.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmdOnce.Size = new System.Drawing.Size(122, 30);
            this.cmdOnce.TabIndex = 8;
            this.cmdOnce.Text = "发送一次";
            this.cmdOnce.UseVisualStyleBackColor = false;
            this.cmdOnce.Click += new System.EventHandler(this.cmdOnce_Click);
            // 
            // fraBytesReceived
            // 
            this.fraBytesReceived.BackColor = System.Drawing.SystemColors.Control;
            this.fraBytesReceived.Controls.Add(this.txtBytesReceived);
            this.fraBytesReceived.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fraBytesReceived.ForeColor = System.Drawing.SystemColors.ControlText;
            this.fraBytesReceived.Location = new System.Drawing.Point(384, 157);
            this.fraBytesReceived.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.fraBytesReceived.Name = "fraBytesReceived";
            this.fraBytesReceived.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.fraBytesReceived.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fraBytesReceived.Size = new System.Drawing.Size(353, 170);
            this.fraBytesReceived.TabIndex = 13;
            this.fraBytesReceived.TabStop = false;
            this.fraBytesReceived.Text = "接收到的数据字节";
            // 
            // txtBytesReceived
            // 
            this.txtBytesReceived.AcceptsReturn = true;
            this.txtBytesReceived.BackColor = System.Drawing.SystemColors.Window;
            this.txtBytesReceived.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtBytesReceived.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBytesReceived.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBytesReceived.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtBytesReceived.Location = new System.Drawing.Point(3, 20);
            this.txtBytesReceived.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtBytesReceived.MaxLength = 0;
            this.txtBytesReceived.Multiline = true;
            this.txtBytesReceived.Name = "txtBytesReceived";
            this.txtBytesReceived.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtBytesReceived.Size = new System.Drawing.Size(347, 146);
            this.txtBytesReceived.TabIndex = 5;
            this.toolTip1.SetToolTip(this.txtBytesReceived, "显示接收到的数据字节，16进制格式，空格分隔");
            // 
            // fraBytesToSend
            // 
            this.fraBytesToSend.BackColor = System.Drawing.SystemColors.Control;
            this.fraBytesToSend.Controls.Add(this.richTextBox_bytesToSend);
            this.fraBytesToSend.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fraBytesToSend.ForeColor = System.Drawing.SystemColors.ControlText;
            this.fraBytesToSend.Location = new System.Drawing.Point(14, 157);
            this.fraBytesToSend.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.fraBytesToSend.Name = "fraBytesToSend";
            this.fraBytesToSend.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.fraBytesToSend.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.fraBytesToSend.Size = new System.Drawing.Size(364, 170);
            this.fraBytesToSend.TabIndex = 12;
            this.fraBytesToSend.TabStop = false;
            this.fraBytesToSend.Text = "发送数据字节";
            // 
            // richTextBox_bytesToSend
            // 
            this.richTextBox_bytesToSend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox_bytesToSend.Location = new System.Drawing.Point(3, 20);
            this.richTextBox_bytesToSend.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.richTextBox_bytesToSend.Name = "richTextBox_bytesToSend";
            this.richTextBox_bytesToSend.Size = new System.Drawing.Size(358, 146);
            this.richTextBox_bytesToSend.TabIndex = 0;
            this.richTextBox_bytesToSend.Text = "";
            this.toolTip1.SetToolTip(this.richTextBox_bytesToSend, "发送数据字节，16进制格式，用空格分隔");
            // 
            // richTextBox_Msg
            // 
            this.richTextBox_Msg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_Msg.Location = new System.Drawing.Point(19, 402);
            this.richTextBox_Msg.Name = "richTextBox_Msg";
            this.richTextBox_Msg.Size = new System.Drawing.Size(933, 168);
            this.richTextBox_Msg.TabIndex = 18;
            this.richTextBox_Msg.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(962, 584);
            this.Controls.Add(this.richTextBox_Msg);
            this.Controls.Add(this.fraDeviceIdentifiers);
            this.Controls.Add(this.fraInputReportBufferSize);
            this.Controls.Add(this.fraReportTypes);
            this.Controls.Add(this.fraSendAndReceive);
            this.Controls.Add(this.fraBytesReceived);
            this.Controls.Add(this.fraBytesToSend);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.fraDeviceIdentifiers.ResumeLayout(false);
            this.fraDeviceIdentifiers.PerformLayout();
            this.fraInputReportBufferSize.ResumeLayout(false);
            this.fraInputReportBufferSize.PerformLayout();
            this.fraReportTypes.ResumeLayout(false);
            this.fraSendAndReceive.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_TimerOut)).EndInit();
            this.fraBytesReceived.ResumeLayout(false);
            this.fraBytesReceived.PerformLayout();
            this.fraBytesToSend.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Button cmdFindDevice;
        public System.Windows.Forms.GroupBox fraDeviceIdentifiers;
        public System.Windows.Forms.TextBox txtProductID;
        public System.Windows.Forms.Label lblProductID;
        public System.Windows.Forms.TextBox txtVendorID;
        public System.Windows.Forms.Label lblVendorID;
        public System.Windows.Forms.GroupBox fraInputReportBufferSize;
        public System.Windows.Forms.Button cmdInputReportBufferSize;
        public System.Windows.Forms.TextBox txtInputReportBufferSize;
        public System.Windows.Forms.GroupBox fraReportTypes;
        public System.Windows.Forms.CheckBox chkUseControlTransfersOnly;
        public System.Windows.Forms.RadioButton optFeature;
        public System.Windows.Forms.RadioButton optInputOutput;
        public System.Windows.Forms.GroupBox fraSendAndReceive;
        private System.Windows.Forms.NumericUpDown numericUpDown_TimerOut;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Button cmdContinuous;
        public System.Windows.Forms.Button cmdOnce;
        public System.Windows.Forms.GroupBox fraBytesReceived;
        public System.Windows.Forms.TextBox txtBytesReceived;
        public System.Windows.Forms.GroupBox fraBytesToSend;
        private System.Windows.Forms.RichTextBox richTextBox_bytesToSend;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button button_Exit;
        private System.Windows.Forms.RichTextBox richTextBox_Msg;
        private System.Windows.Forms.ComboBox comboBox_DeviceList;
    }
}

