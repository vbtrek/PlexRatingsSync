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
      this.etchedLine4 = new DS.Controls.EtchedLine();
      this.etchedLine3 = new DS.Controls.EtchedLine();
      this.etchedLine1 = new DS.Controls.EtchedLine();
      this.txtPlexPassword = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // txtPlexUsername
      // 
      this.txtPlexUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtPlexUsername.Location = new System.Drawing.Point(97, 40);
      this.txtPlexUsername.Name = "txtPlexUsername";
      this.txtPlexUsername.Size = new System.Drawing.Size(479, 21);
      this.txtPlexUsername.TabIndex = 2;
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
      this.chkSyncRatings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.chkSyncRatings.AutoSize = true;
      this.chkSyncRatings.Location = new System.Drawing.Point(15, 126);
      this.chkSyncRatings.Name = "chkSyncRatings";
      this.chkSyncRatings.Size = new System.Drawing.Size(124, 17);
      this.chkSyncRatings.TabIndex = 6;
      this.chkSyncRatings.Text = "Sync Ratings to Plex";
      this.chkSyncRatings.UseVisualStyleBackColor = true;
      this.chkSyncRatings.CheckedChanged += new System.EventHandler(this.chkSyncRatings_CheckedChanged);
      // 
      // cmdCancel
      // 
      this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cmdCancel.Location = new System.Drawing.Point(503, 233);
      this.cmdCancel.Name = "cmdCancel";
      this.cmdCancel.Size = new System.Drawing.Size(75, 23);
      this.cmdCancel.TabIndex = 15;
      this.cmdCancel.Text = "Cancel";
      this.cmdCancel.UseVisualStyleBackColor = true;
      // 
      // cmdOk
      // 
      this.cmdOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cmdOk.Location = new System.Drawing.Point(422, 233);
      this.cmdOk.Name = "cmdOk";
      this.cmdOk.Size = new System.Drawing.Size(75, 23);
      this.cmdOk.TabIndex = 14;
      this.cmdOk.Text = "OK";
      this.cmdOk.UseVisualStyleBackColor = true;
      this.cmdOk.Click += new System.EventHandler(this.CmdOk_Click);
      // 
      // cboSyncSource
      // 
      this.cboSyncSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.cboSyncSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cboSyncSource.Enabled = false;
      this.cboSyncSource.FormattingEnabled = true;
      this.cboSyncSource.Location = new System.Drawing.Point(323, 124);
      this.cboSyncSource.Name = "cboSyncSource";
      this.cboSyncSource.Size = new System.Drawing.Size(255, 21);
      this.cboSyncSource.TabIndex = 8;
      this.cboSyncSource.SelectedIndexChanged += new System.EventHandler(this.cboSyncSource_SelectedIndexChanged);
      // 
      // cboSyncMode
      // 
      this.cboSyncMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.cboSyncMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cboSyncMode.Enabled = false;
      this.cboSyncMode.FormattingEnabled = true;
      this.cboSyncMode.Location = new System.Drawing.Point(323, 151);
      this.cboSyncMode.Name = "cboSyncMode";
      this.cboSyncMode.Size = new System.Drawing.Size(255, 21);
      this.cboSyncMode.TabIndex = 10;
      this.cboSyncMode.SelectedIndexChanged += new System.EventHandler(this.cboSyncMode_SelectedIndexChanged);
      // 
      // lblSyncSource
      // 
      this.lblSyncSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblSyncSource.AutoSize = true;
      this.lblSyncSource.Enabled = false;
      this.lblSyncSource.Location = new System.Drawing.Point(246, 127);
      this.lblSyncSource.Name = "lblSyncSource";
      this.lblSyncSource.Size = new System.Drawing.Size(69, 13);
      this.lblSyncSource.TabIndex = 7;
      this.lblSyncSource.Text = "Sync source:";
      // 
      // lblSyncMode
      // 
      this.lblSyncMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblSyncMode.AutoSize = true;
      this.lblSyncMode.Enabled = false;
      this.lblSyncMode.Location = new System.Drawing.Point(246, 154);
      this.lblSyncMode.Name = "lblSyncMode";
      this.lblSyncMode.Size = new System.Drawing.Size(63, 13);
      this.lblSyncMode.TabIndex = 9;
      this.lblSyncMode.Text = "Sync mode:";
      // 
      // lblClashWinner
      // 
      this.lblClashWinner.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblClashWinner.AutoSize = true;
      this.lblClashWinner.Enabled = false;
      this.lblClashWinner.Location = new System.Drawing.Point(246, 181);
      this.lblClashWinner.Name = "lblClashWinner";
      this.lblClashWinner.Size = new System.Drawing.Size(72, 13);
      this.lblClashWinner.TabIndex = 11;
      this.lblClashWinner.Text = "Clash winner:";
      // 
      // cboClashWinner
      // 
      this.cboClashWinner.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.cboClashWinner.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cboClashWinner.Enabled = false;
      this.cboClashWinner.FormattingEnabled = true;
      this.cboClashWinner.Location = new System.Drawing.Point(324, 178);
      this.cboClashWinner.Name = "cboClashWinner";
      this.cboClashWinner.Size = new System.Drawing.Size(255, 21);
      this.cboClashWinner.TabIndex = 12;
      // 
      // etchedLine4
      // 
      this.etchedLine4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.etchedLine4.Edge = DS.Controls.EtchEdge.Center;
      this.etchedLine4.Location = new System.Drawing.Point(-3, 207);
      this.etchedLine4.Name = "etchedLine4";
      this.etchedLine4.Size = new System.Drawing.Size(596, 21);
      this.etchedLine4.TabIndex = 13;
      this.etchedLine4.TextLabel = "";
      // 
      // etchedLine3
      // 
      this.etchedLine3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.etchedLine3.Edge = DS.Controls.EtchEdge.Center;
      this.etchedLine3.Location = new System.Drawing.Point(12, 99);
      this.etchedLine3.Name = "etchedLine3";
      this.etchedLine3.Size = new System.Drawing.Size(564, 21);
      this.etchedLine3.TabIndex = 5;
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
      // txtPlexPassword
      // 
      this.txtPlexPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtPlexPassword.Location = new System.Drawing.Point(97, 65);
      this.txtPlexPassword.Name = "txtPlexPassword";
      this.txtPlexPassword.PasswordChar = '*';
      this.txtPlexPassword.Size = new System.Drawing.Size(479, 21);
      this.txtPlexPassword.TabIndex = 4;
      // 
      // Options3
      // 
      this.AcceptButton = this.cmdOk;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cmdCancel;
      this.ClientSize = new System.Drawing.Size(590, 268);
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
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "Options3";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Options";
      this.Load += new System.EventHandler(this.Options3_Load);
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
  }
}