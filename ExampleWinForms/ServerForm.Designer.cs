namespace ExampleGUI
{
    partial class ServerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblMsgs = new System.Windows.Forms.Label();
            this.lblSend = new System.Windows.Forms.Label();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.spltMain = new System.Windows.Forms.SplitContainer();
            this.txtMessages = new System.Windows.Forms.TextBox();
            this.lstClients = new System.Windows.Forms.ListBox();
            this.lblClients = new System.Windows.Forms.Label();
            this.tblMain = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.spltMain)).BeginInit();
            this.spltMain.Panel1.SuspendLayout();
            this.spltMain.Panel2.SuspendLayout();
            this.spltMain.SuspendLayout();
            this.tblMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblMsgs
            // 
            this.lblMsgs.AutoSize = true;
            this.tblMain.SetColumnSpan(this.lblMsgs, 3);
            this.lblMsgs.Location = new System.Drawing.Point(3, 3);
            this.lblMsgs.Margin = new System.Windows.Forms.Padding(3);
            this.lblMsgs.Name = "lblMsgs";
            this.lblMsgs.Size = new System.Drawing.Size(95, 15);
            this.lblMsgs.TabIndex = 0;
            this.lblMsgs.Text = "Client messages:";
            // 
            // lblSend
            // 
            this.lblSend.AutoSize = true;
            this.lblSend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSend.Location = new System.Drawing.Point(3, 377);
            this.lblSend.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.lblSend.Name = "lblSend";
            this.lblSend.Size = new System.Drawing.Size(87, 17);
            this.lblSend.TabIndex = 2;
            this.lblSend.Text = "Send to clients:";
            // 
            // txtMessage
            // 
            this.txtMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMessage.Location = new System.Drawing.Point(96, 374);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(320, 23);
            this.txtMessage.TabIndex = 3;
            // 
            // btnSend
            // 
            this.btnSend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSend.Location = new System.Drawing.Point(422, 374);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 4;
            this.btnSend.Text = "&Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // spltMain
            // 
            this.tblMain.SetColumnSpan(this.spltMain, 3);
            this.spltMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spltMain.Location = new System.Drawing.Point(6, 27);
            this.spltMain.Margin = new System.Windows.Forms.Padding(6);
            this.spltMain.Name = "spltMain";
            // 
            // spltMain.Panel1
            // 
            this.spltMain.Panel1.Controls.Add(this.txtMessages);
            this.spltMain.Panel1MinSize = 150;
            // 
            // spltMain.Panel2
            // 
            this.spltMain.Panel2.Controls.Add(this.lstClients);
            this.spltMain.Panel2.Controls.Add(this.lblClients);
            this.spltMain.Panel2MinSize = 150;
            this.spltMain.Size = new System.Drawing.Size(488, 338);
            this.spltMain.SplitterDistance = 292;
            this.spltMain.SplitterWidth = 6;
            this.spltMain.TabIndex = 5;
            // 
            // txtMessages
            // 
            this.txtMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMessages.Location = new System.Drawing.Point(0, 0);
            this.txtMessages.MaxLength = 2147483647;
            this.txtMessages.Multiline = true;
            this.txtMessages.Name = "txtMessages";
            this.txtMessages.Size = new System.Drawing.Size(292, 338);
            this.txtMessages.TabIndex = 0;
            // 
            // lstClients
            // 
            this.lstClients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstClients.FormattingEnabled = true;
            this.lstClients.ItemHeight = 15;
            this.lstClients.Location = new System.Drawing.Point(0, 15);
            this.lstClients.Name = "lstClients";
            this.lstClients.Size = new System.Drawing.Size(190, 323);
            this.lstClients.TabIndex = 1;
            // 
            // lblClients
            // 
            this.lblClients.AutoSize = true;
            this.lblClients.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblClients.Location = new System.Drawing.Point(0, 0);
            this.lblClients.Name = "lblClients";
            this.lblClients.Size = new System.Drawing.Size(46, 15);
            this.lblClients.TabIndex = 0;
            this.lblClients.Text = "Clients:";
            // 
            // tblMain
            // 
            this.tblMain.ColumnCount = 3;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblMain.Controls.Add(this.lblMsgs, 0, 0);
            this.tblMain.Controls.Add(this.spltMain, 0, 1);
            this.tblMain.Controls.Add(this.btnSend, 2, 2);
            this.tblMain.Controls.Add(this.lblSend, 0, 2);
            this.tblMain.Controls.Add(this.txtMessage, 1, 2);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 0);
            this.tblMain.Margin = new System.Windows.Forms.Padding(0);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 3;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblMain.Size = new System.Drawing.Size(500, 400);
            this.tblMain.TabIndex = 6;
            // 
            // ServerForm
            // 
            this.AcceptButton = this.btnSend;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(500, 400);
            this.Controls.Add(this.tblMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ServerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Server";
            this.spltMain.Panel1.ResumeLayout(false);
            this.spltMain.Panel1.PerformLayout();
            this.spltMain.Panel2.ResumeLayout(false);
            this.spltMain.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spltMain)).EndInit();
            this.spltMain.ResumeLayout(false);
            this.tblMain.ResumeLayout(false);
            this.tblMain.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblMsgs;
        private System.Windows.Forms.Label lblSend;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.SplitContainer spltMain;
        private System.Windows.Forms.ListBox lstClients;
        private System.Windows.Forms.Label lblClients;
        private System.Windows.Forms.TableLayoutPanel tblMain;
        private System.Windows.Forms.TextBox txtMessages;
    }
}