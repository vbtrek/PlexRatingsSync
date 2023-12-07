namespace DS.PlexRatingsSync
{
	partial class Options3
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Options3));
      this.txtPlexUsername = new System.Windows.Forms.TextBox();
      this.lblPlexUsername = new System.Windows.Forms.Label();
      this.lblPlexPassword = new System.Windows.Forms.Label();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.chkSyncRatings = new System.Windows.Forms.CheckBox();
      this.cmdCancel = new System.Windows.Forms.Button();
      this.cmdOk = new System.Windows.Forms.Button();
      this.cboSyncSource = new System.Windows.Forms.ComboBox();
      this.cboSyncMode = new System.Windows.Forms.ComboBox();
      this.lblSyncSource = new System.Windows.Forms.Label();
      this.lblSyncMode = new System.Windows.Forms.Label();
      this.lblClashWinner = new System.Windows.Forms.Label();
      this.cboClashWinner = new System.Windows.Forms.ComboBox();
      this.txtPlexPassword = new System.Windows.Forms.TextBox();
      this.txtPlexUri = new System.Windows.Forms.TextBox();
      this.lblPlexUri = new System.Windows.Forms.Label();
      this.chkSendEmail = new System.Windows.Forms.CheckBox();
      this.lblSendToName = new System.Windows.Forms.Label();
      this.txtSendToName = new System.Windows.Forms.TextBox();
      this.lblSendToEmail = new System.Windows.Forms.Label();
      this.txtSendToEmail = new System.Windows.Forms.TextBox();
      this.lblFromEmail = new System.Windows.Forms.Label();
      this.txtFromEmail = new System.Windows.Forms.TextBox();
      this.lblFromName = new System.Windows.Forms.Label();
      this.txtFromName = new System.Windows.Forms.TextBox();
      this.lblSmtpServer = new System.Windows.Forms.Label();
      this.txtSmtpServer = new System.Windows.Forms.TextBox();
      this.lblSmtpPort = new System.Windows.Forms.Label();
      this.txtSmtpPort = new System.Windows.Forms.TextBox();
      this.chkUseSsl = new System.Windows.Forms.CheckBox();
      this.lblSecurity = new System.Windows.Forms.Label();
      this.cboSecurity = new System.Windows.Forms.ComboBox();
      this.lblSmtpUsername = new System.Windows.Forms.Label();
      this.txtSmtpUsername = new System.Windows.Forms.TextBox();
      this.lblSmtpPassword = new System.Windows.Forms.Label();
      this.txtSmtpPassword = new System.Windows.Forms.TextBox();
      this.grdMappings = new System.Windows.Forms.DataGridView();
      this.etchedLine2 = new DS.Controls.EtchedLine();
      this.etchedLine4 = new DS.Controls.EtchedLine();
      this.etchedLine3 = new DS.Controls.EtchedLine();
      this.etchedLine1 = new DS.Controls.EtchedLine();
      this.label1 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.grdMappings)).BeginInit();
      this.SuspendLayout();
      // 
      // txtPlexUsername
      // 
      this.txtPlexUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtPlexUsername.Location = new System.Drawing.Point(107, 40);
      this.txtPlexUsername.Name = "txtPlexUsername";
      this.txtPlexUsername.Size = new System.Drawing.Size(471, 21);
      this.txtPlexUsername.TabIndex = 2;
      this.txtPlexUsername.Validating += new System.ComponentModel.CancelEventHandler(this.txtPlexUsername_Validating);
      // 
      // lblPlexUsername
      // 
      this.lblPlexUsername.AutoSize = true;
      this.lblPlexUsername.Location = new System.Drawing.Point(12, 44);
      this.lblPlexUsername.Name = "lblPlexUsername";
      this.lblPlexUsername.Size = new System.Drawing.Size(81, 13);
      this.lblPlexUsername.TabIndex = 1;
      this.lblPlexUsername.Text = "Plex username:";
      // 
      // lblPlexPassword
      // 
      this.lblPlexPassword.AutoSize = true;
      this.lblPlexPassword.Location = new System.Drawing.Point(12, 68);
      this.lblPlexPassword.Name = "lblPlexPassword";
      this.lblPlexPassword.Size = new System.Drawing.Size(80, 13);
      this.lblPlexPassword.TabIndex = 3;
      this.lblPlexPassword.Text = "Plex password:";
      // 
      // openFileDialog
      // 
      this.openFileDialog.FileName = "openFileDialog";
      // 
      // chkSyncRatings
      // 
      this.chkSyncRatings.AutoSize = true;
      this.chkSyncRatings.Location = new System.Drawing.Point(15, 244);
      this.chkSyncRatings.Name = "chkSyncRatings";
      this.chkSyncRatings.Size = new System.Drawing.Size(124, 17);
      this.chkSyncRatings.TabIndex = 8;
      this.chkSyncRatings.Text = "Sync Ratings to Plex";
      this.chkSyncRatings.UseVisualStyleBackColor = true;
      this.chkSyncRatings.CheckedChanged += new System.EventHandler(this.chkSyncRatings_CheckedChanged);
      // 
      // cmdCancel
      // 
      this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cmdCancel.Location = new System.Drawing.Point(503, 621);
      this.cmdCancel.Name = "cmdCancel";
      this.cmdCancel.Size = new System.Drawing.Size(75, 23);
      this.cmdCancel.TabIndex = 17;
      this.cmdCancel.Text = "Cancel";
      this.cmdCancel.UseVisualStyleBackColor = true;
      // 
      // cmdOk
      // 
      this.cmdOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cmdOk.Location = new System.Drawing.Point(422, 621);
      this.cmdOk.Name = "cmdOk";
      this.cmdOk.Size = new System.Drawing.Size(75, 23);
      this.cmdOk.TabIndex = 16;
      this.cmdOk.Text = "OK";
      this.cmdOk.UseVisualStyleBackColor = true;
      this.cmdOk.Click += new System.EventHandler(this.CmdOk_Click);
      // 
      // cboSyncSource
      // 
      this.cboSyncSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.cboSyncSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cboSyncSource.Enabled = false;
      this.cboSyncSource.FormattingEnabled = true;
      this.cboSyncSource.Location = new System.Drawing.Point(294, 242);
      this.cboSyncSource.Name = "cboSyncSource";
      this.cboSyncSource.Size = new System.Drawing.Size(282, 21);
      this.cboSyncSource.TabIndex = 10;
      this.cboSyncSource.SelectedIndexChanged += new System.EventHandler(this.cboSyncSource_SelectedIndexChanged);
      // 
      // cboSyncMode
      // 
      this.cboSyncMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.cboSyncMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cboSyncMode.Enabled = false;
      this.cboSyncMode.FormattingEnabled = true;
      this.cboSyncMode.Location = new System.Drawing.Point(294, 269);
      this.cboSyncMode.Name = "cboSyncMode";
      this.cboSyncMode.Size = new System.Drawing.Size(282, 21);
      this.cboSyncMode.TabIndex = 12;
      this.cboSyncMode.SelectedIndexChanged += new System.EventHandler(this.cboSyncMode_SelectedIndexChanged);
      // 
      // lblSyncSource
      // 
      this.lblSyncSource.AutoSize = true;
      this.lblSyncSource.Enabled = false;
      this.lblSyncSource.Location = new System.Drawing.Point(217, 245);
      this.lblSyncSource.Name = "lblSyncSource";
      this.lblSyncSource.Size = new System.Drawing.Size(69, 13);
      this.lblSyncSource.TabIndex = 9;
      this.lblSyncSource.Text = "Sync source:";
      // 
      // lblSyncMode
      // 
      this.lblSyncMode.AutoSize = true;
      this.lblSyncMode.Enabled = false;
      this.lblSyncMode.Location = new System.Drawing.Point(217, 272);
      this.lblSyncMode.Name = "lblSyncMode";
      this.lblSyncMode.Size = new System.Drawing.Size(63, 13);
      this.lblSyncMode.TabIndex = 11;
      this.lblSyncMode.Text = "Sync mode:";
      // 
      // lblClashWinner
      // 
      this.lblClashWinner.AutoSize = true;
      this.lblClashWinner.Enabled = false;
      this.lblClashWinner.Location = new System.Drawing.Point(217, 299);
      this.lblClashWinner.Name = "lblClashWinner";
      this.lblClashWinner.Size = new System.Drawing.Size(72, 13);
      this.lblClashWinner.TabIndex = 13;
      this.lblClashWinner.Text = "Clash winner:";
      // 
      // cboClashWinner
      // 
      this.cboClashWinner.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.cboClashWinner.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cboClashWinner.Enabled = false;
      this.cboClashWinner.FormattingEnabled = true;
      this.cboClashWinner.Location = new System.Drawing.Point(294, 296);
      this.cboClashWinner.Name = "cboClashWinner";
      this.cboClashWinner.Size = new System.Drawing.Size(282, 21);
      this.cboClashWinner.TabIndex = 14;
      // 
      // txtPlexPassword
      // 
      this.txtPlexPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtPlexPassword.Location = new System.Drawing.Point(107, 65);
      this.txtPlexPassword.Name = "txtPlexPassword";
      this.txtPlexPassword.PasswordChar = '*';
      this.txtPlexPassword.Size = new System.Drawing.Size(471, 21);
      this.txtPlexPassword.TabIndex = 4;
      this.txtPlexPassword.Validating += new System.ComponentModel.CancelEventHandler(this.txtPlexPassword_Validating);
      // 
      // txtPlexUri
      // 
      this.txtPlexUri.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtPlexUri.Location = new System.Drawing.Point(107, 92);
      this.txtPlexUri.Name = "txtPlexUri";
      this.txtPlexUri.Size = new System.Drawing.Size(471, 21);
      this.txtPlexUri.TabIndex = 6;
      // 
      // lblPlexUri
      // 
      this.lblPlexUri.AutoSize = true;
      this.lblPlexUri.Location = new System.Drawing.Point(12, 95);
      this.lblPlexUri.Name = "lblPlexUri";
      this.lblPlexUri.Size = new System.Drawing.Size(84, 13);
      this.lblPlexUri.TabIndex = 5;
      this.lblPlexUri.Text = "Private Plex Uri:";
      // 
      // chkSendEmail
      // 
      this.chkSendEmail.AutoSize = true;
      this.chkSendEmail.Location = new System.Drawing.Point(15, 360);
      this.chkSendEmail.Name = "chkSendEmail";
      this.chkSendEmail.Size = new System.Drawing.Size(196, 17);
      this.chkSendEmail.TabIndex = 19;
      this.chkSendEmail.Text = "Send Email Summary Of Unmatched";
      this.chkSendEmail.UseVisualStyleBackColor = true;
      this.chkSendEmail.CheckedChanged += new System.EventHandler(this.chkSendEmail_CheckedChanged);
      // 
      // lblSendToName
      // 
      this.lblSendToName.AutoSize = true;
      this.lblSendToName.Location = new System.Drawing.Point(12, 387);
      this.lblSendToName.Name = "lblSendToName";
      this.lblSendToName.Size = new System.Drawing.Size(80, 13);
      this.lblSendToName.TabIndex = 20;
      this.lblSendToName.Text = "Send To Name:";
      // 
      // txtSendToName
      // 
      this.txtSendToName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtSendToName.Location = new System.Drawing.Point(97, 383);
      this.txtSendToName.Name = "txtSendToName";
      this.txtSendToName.Size = new System.Drawing.Size(192, 21);
      this.txtSendToName.TabIndex = 21;
      // 
      // lblSendToEmail
      // 
      this.lblSendToEmail.AutoSize = true;
      this.lblSendToEmail.Location = new System.Drawing.Point(12, 414);
      this.lblSendToEmail.Name = "lblSendToEmail";
      this.lblSendToEmail.Size = new System.Drawing.Size(77, 13);
      this.lblSendToEmail.TabIndex = 22;
      this.lblSendToEmail.Text = "Send To Email:";
      // 
      // txtSendToEmail
      // 
      this.txtSendToEmail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtSendToEmail.Location = new System.Drawing.Point(97, 410);
      this.txtSendToEmail.Name = "txtSendToEmail";
      this.txtSendToEmail.Size = new System.Drawing.Size(192, 21);
      this.txtSendToEmail.TabIndex = 23;
      // 
      // lblFromEmail
      // 
      this.lblFromEmail.AutoSize = true;
      this.lblFromEmail.Location = new System.Drawing.Point(298, 414);
      this.lblFromEmail.Name = "lblFromEmail";
      this.lblFromEmail.Size = new System.Drawing.Size(62, 13);
      this.lblFromEmail.TabIndex = 26;
      this.lblFromEmail.Text = "From Email:";
      // 
      // txtFromEmail
      // 
      this.txtFromEmail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtFromEmail.Location = new System.Drawing.Point(383, 410);
      this.txtFromEmail.Name = "txtFromEmail";
      this.txtFromEmail.Size = new System.Drawing.Size(193, 21);
      this.txtFromEmail.TabIndex = 27;
      // 
      // lblFromName
      // 
      this.lblFromName.AutoSize = true;
      this.lblFromName.Location = new System.Drawing.Point(298, 387);
      this.lblFromName.Name = "lblFromName";
      this.lblFromName.Size = new System.Drawing.Size(65, 13);
      this.lblFromName.TabIndex = 24;
      this.lblFromName.Text = "From Name:";
      // 
      // txtFromName
      // 
      this.txtFromName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtFromName.Location = new System.Drawing.Point(383, 383);
      this.txtFromName.Name = "txtFromName";
      this.txtFromName.Size = new System.Drawing.Size(193, 21);
      this.txtFromName.TabIndex = 25;
      // 
      // lblSmtpServer
      // 
      this.lblSmtpServer.AutoSize = true;
      this.lblSmtpServer.Location = new System.Drawing.Point(298, 440);
      this.lblSmtpServer.Name = "lblSmtpServer";
      this.lblSmtpServer.Size = new System.Drawing.Size(72, 13);
      this.lblSmtpServer.TabIndex = 28;
      this.lblSmtpServer.Text = "SMTP Server:";
      // 
      // txtSmtpServer
      // 
      this.txtSmtpServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtSmtpServer.Location = new System.Drawing.Point(384, 437);
      this.txtSmtpServer.Name = "txtSmtpServer";
      this.txtSmtpServer.Size = new System.Drawing.Size(192, 21);
      this.txtSmtpServer.TabIndex = 29;
      // 
      // lblSmtpPort
      // 
      this.lblSmtpPort.AutoSize = true;
      this.lblSmtpPort.Location = new System.Drawing.Point(298, 467);
      this.lblSmtpPort.Name = "lblSmtpPort";
      this.lblSmtpPort.Size = new System.Drawing.Size(60, 13);
      this.lblSmtpPort.TabIndex = 30;
      this.lblSmtpPort.Text = "SMTP Port:";
      // 
      // txtSmtpPort
      // 
      this.txtSmtpPort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtSmtpPort.Location = new System.Drawing.Point(384, 464);
      this.txtSmtpPort.Name = "txtSmtpPort";
      this.txtSmtpPort.Size = new System.Drawing.Size(192, 21);
      this.txtSmtpPort.TabIndex = 31;
      // 
      // chkUseSsl
      // 
      this.chkUseSsl.AutoSize = true;
      this.chkUseSsl.Location = new System.Drawing.Point(383, 491);
      this.chkUseSsl.Name = "chkUseSsl";
      this.chkUseSsl.Size = new System.Drawing.Size(64, 17);
      this.chkUseSsl.TabIndex = 32;
      this.chkUseSsl.Text = "Use SSL";
      this.chkUseSsl.UseVisualStyleBackColor = true;
      // 
      // lblSecurity
      // 
      this.lblSecurity.AutoSize = true;
      this.lblSecurity.Enabled = false;
      this.lblSecurity.Location = new System.Drawing.Point(298, 517);
      this.lblSecurity.Name = "lblSecurity";
      this.lblSecurity.Size = new System.Drawing.Size(50, 13);
      this.lblSecurity.TabIndex = 33;
      this.lblSecurity.Text = "Security:";
      // 
      // cboSecurity
      // 
      this.cboSecurity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.cboSecurity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cboSecurity.FormattingEnabled = true;
      this.cboSecurity.Location = new System.Drawing.Point(383, 514);
      this.cboSecurity.Name = "cboSecurity";
      this.cboSecurity.Size = new System.Drawing.Size(193, 21);
      this.cboSecurity.TabIndex = 34;
      // 
      // lblSmtpUsername
      // 
      this.lblSmtpUsername.AutoSize = true;
      this.lblSmtpUsername.Location = new System.Drawing.Point(298, 544);
      this.lblSmtpUsername.Name = "lblSmtpUsername";
      this.lblSmtpUsername.Size = new System.Drawing.Size(86, 13);
      this.lblSmtpUsername.TabIndex = 35;
      this.lblSmtpUsername.Text = "SMTP Usernane:";
      // 
      // txtSmtpUsername
      // 
      this.txtSmtpUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtSmtpUsername.Location = new System.Drawing.Point(384, 541);
      this.txtSmtpUsername.Name = "txtSmtpUsername";
      this.txtSmtpUsername.Size = new System.Drawing.Size(192, 21);
      this.txtSmtpUsername.TabIndex = 36;
      // 
      // lblSmtpPassword
      // 
      this.lblSmtpPassword.AutoSize = true;
      this.lblSmtpPassword.Location = new System.Drawing.Point(298, 571);
      this.lblSmtpPassword.Name = "lblSmtpPassword";
      this.lblSmtpPassword.Size = new System.Drawing.Size(86, 13);
      this.lblSmtpPassword.TabIndex = 37;
      this.lblSmtpPassword.Text = "SMTP Password:";
      // 
      // txtSmtpPassword
      // 
      this.txtSmtpPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtSmtpPassword.Location = new System.Drawing.Point(384, 568);
      this.txtSmtpPassword.Name = "txtSmtpPassword";
      this.txtSmtpPassword.PasswordChar = '*';
      this.txtSmtpPassword.Size = new System.Drawing.Size(192, 21);
      this.txtSmtpPassword.TabIndex = 38;
      // 
      // grdMappings
      // 
      this.grdMappings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.grdMappings.Location = new System.Drawing.Point(107, 119);
      this.grdMappings.Name = "grdMappings";
      this.grdMappings.Size = new System.Drawing.Size(471, 92);
      this.grdMappings.TabIndex = 39;
      // 
      // etchedLine2
      // 
      this.etchedLine2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.etchedLine2.Edge = DS.Controls.EtchEdge.Center;
      this.etchedLine2.Location = new System.Drawing.Point(12, 333);
      this.etchedLine2.Name = "etchedLine2";
      this.etchedLine2.Size = new System.Drawing.Size(564, 21);
      this.etchedLine2.TabIndex = 18;
      this.etchedLine2.TextLabel = "Email";
      // 
      // etchedLine4
      // 
      this.etchedLine4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.etchedLine4.Edge = DS.Controls.EtchEdge.Center;
      this.etchedLine4.Location = new System.Drawing.Point(-3, 595);
      this.etchedLine4.Name = "etchedLine4";
      this.etchedLine4.Size = new System.Drawing.Size(596, 21);
      this.etchedLine4.TabIndex = 15;
      this.etchedLine4.TextLabel = "";
      // 
      // etchedLine3
      // 
      this.etchedLine3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.etchedLine3.Edge = DS.Controls.EtchEdge.Center;
      this.etchedLine3.Location = new System.Drawing.Point(12, 217);
      this.etchedLine3.Name = "etchedLine3";
      this.etchedLine3.Size = new System.Drawing.Size(564, 21);
      this.etchedLine3.TabIndex = 7;
      this.etchedLine3.TextLabel = "Ratings ";
      // 
      // etchedLine1
      // 
      this.etchedLine1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.etchedLine1.Edge = DS.Controls.EtchEdge.Center;
      this.etchedLine1.Location = new System.Drawing.Point(12, 12);
      this.etchedLine1.Name = "etchedLine1";
      this.etchedLine1.Size = new System.Drawing.Size(564, 21);
      this.etchedLine1.TabIndex = 0;
      this.etchedLine1.TextLabel = "Plex ";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 121);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(89, 13);
      this.label1.TabIndex = 40;
      this.label1.Text = "Folder Mappings:";
      // 
      // Options3
      // 
      this.AcceptButton = this.cmdOk;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cmdCancel;
      this.ClientSize = new System.Drawing.Size(590, 656);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.grdMappings);
      this.Controls.Add(this.lblSmtpPassword);
      this.Controls.Add(this.txtSmtpPassword);
      this.Controls.Add(this.lblSmtpUsername);
      this.Controls.Add(this.txtSmtpUsername);
      this.Controls.Add(this.lblSecurity);
      this.Controls.Add(this.cboSecurity);
      this.Controls.Add(this.chkUseSsl);
      this.Controls.Add(this.lblSmtpPort);
      this.Controls.Add(this.txtSmtpPort);
      this.Controls.Add(this.lblSmtpServer);
      this.Controls.Add(this.txtSmtpServer);
      this.Controls.Add(this.lblFromEmail);
      this.Controls.Add(this.txtFromEmail);
      this.Controls.Add(this.lblFromName);
      this.Controls.Add(this.txtFromName);
      this.Controls.Add(this.lblSendToEmail);
      this.Controls.Add(this.txtSendToEmail);
      this.Controls.Add(this.lblSendToName);
      this.Controls.Add(this.txtSendToName);
      this.Controls.Add(this.chkSendEmail);
      this.Controls.Add(this.etchedLine2);
      this.Controls.Add(this.txtPlexUri);
      this.Controls.Add(this.lblPlexUri);
      this.Controls.Add(this.txtPlexPassword);
      this.Controls.Add(this.lblClashWinner);
      this.Controls.Add(this.cboClashWinner);
      this.Controls.Add(this.lblSyncMode);
      this.Controls.Add(this.lblSyncSource);
      this.Controls.Add(this.cboSyncMode);
      this.Controls.Add(this.cboSyncSource);
      this.Controls.Add(this.etchedLine4);
      this.Controls.Add(this.cmdOk);
      this.Controls.Add(this.cmdCancel);
      this.Controls.Add(this.chkSyncRatings);
      this.Controls.Add(this.etchedLine3);
      this.Controls.Add(this.etchedLine1);
      this.Controls.Add(this.lblPlexPassword);
      this.Controls.Add(this.lblPlexUsername);
      this.Controls.Add(this.txtPlexUsername);
      this.Font = new System.Drawing.Font("Tahoma", 8.25F);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "Options3";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Options";
      this.Load += new System.EventHandler(this.Options3_Load);
      ((System.ComponentModel.ISupportInitialize)(this.grdMappings)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

		}

		#endregion
    private System.Windows.Forms.TextBox txtPlexUsername;
    private System.Windows.Forms.Label lblPlexUsername;
    private System.Windows.Forms.Label lblPlexPassword;
    private System.Windows.Forms.OpenFileDialog openFileDialog;
    private Controls.EtchedLine etchedLine1;
    private Controls.EtchedLine etchedLine3;
    private System.Windows.Forms.CheckBox chkSyncRatings;
    private System.Windows.Forms.Button cmdCancel;
    private System.Windows.Forms.Button cmdOk;
    private Controls.EtchedLine etchedLine4;
    private System.Windows.Forms.ComboBox cboSyncSource;
    private System.Windows.Forms.ComboBox cboSyncMode;
    private System.Windows.Forms.Label lblSyncSource;
    private System.Windows.Forms.Label lblSyncMode;
    private System.Windows.Forms.Label lblClashWinner;
    private System.Windows.Forms.ComboBox cboClashWinner;
    private System.Windows.Forms.TextBox txtPlexPassword;
    private System.Windows.Forms.TextBox txtPlexUri;
    private System.Windows.Forms.Label lblPlexUri;
    private Controls.EtchedLine etchedLine2;
    private System.Windows.Forms.CheckBox chkSendEmail;
    private System.Windows.Forms.Label lblSendToName;
    private System.Windows.Forms.TextBox txtSendToName;
    private System.Windows.Forms.Label lblSendToEmail;
    private System.Windows.Forms.TextBox txtSendToEmail;
    private System.Windows.Forms.Label lblFromEmail;
    private System.Windows.Forms.TextBox txtFromEmail;
    private System.Windows.Forms.Label lblFromName;
    private System.Windows.Forms.TextBox txtFromName;
    private System.Windows.Forms.Label lblSmtpServer;
    private System.Windows.Forms.TextBox txtSmtpServer;
    private System.Windows.Forms.Label lblSmtpPort;
    private System.Windows.Forms.TextBox txtSmtpPort;
    private System.Windows.Forms.CheckBox chkUseSsl;
    private System.Windows.Forms.Label lblSecurity;
    private System.Windows.Forms.ComboBox cboSecurity;
    private System.Windows.Forms.Label lblSmtpUsername;
    private System.Windows.Forms.TextBox txtSmtpUsername;
    private System.Windows.Forms.Label lblSmtpPassword;
    private System.Windows.Forms.TextBox txtSmtpPassword;
    private System.Windows.Forms.DataGridView grdMappings;
    private System.Windows.Forms.Label label1;
  }
}