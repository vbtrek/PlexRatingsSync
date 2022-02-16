﻿namespace DS.PlexRatingsSync
{
	partial class Options2
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
      this.txtPlexDatabase = new System.Windows.Forms.TextBox();
      this.cboPlexAccount = new System.Windows.Forms.ComboBox();
      this.cmdPlexDatabase = new System.Windows.Forms.Button();
      this.lblPlexDatabase = new System.Windows.Forms.Label();
      this.PlexAccount = new System.Windows.Forms.Label();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.grdPlaylists = new System.Windows.Forms.DataGridView();
      this.colPlaylist = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colChecked = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      this.chkSyncPlaylists = new System.Windows.Forms.CheckBox();
      this.lblItunesLibrary = new System.Windows.Forms.Label();
      this.cmdItunesLibrary = new System.Windows.Forms.Button();
      this.txtItunesLibrary = new System.Windows.Forms.TextBox();
      this.chkRemoveEmptyPlaylists = new System.Windows.Forms.CheckBox();
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
      this.etchedLine2 = new DS.Controls.EtchedLine();
      this.etchedLine1 = new DS.Controls.EtchedLine();
      ((System.ComponentModel.ISupportInitialize)(this.grdPlaylists)).BeginInit();
      this.SuspendLayout();
      // 
      // txtPlexDatabase
      // 
      this.txtPlexDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtPlexDatabase.Location = new System.Drawing.Point(97, 40);
      this.txtPlexDatabase.Name = "txtPlexDatabase";
      this.txtPlexDatabase.Size = new System.Drawing.Size(444, 21);
      this.txtPlexDatabase.TabIndex = 1;
      this.txtPlexDatabase.TextChanged += new System.EventHandler(this.txtPlexDatabase_TextChanged);
      // 
      // cboPlexAccount
      // 
      this.cboPlexAccount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.cboPlexAccount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cboPlexAccount.FormattingEnabled = true;
      this.cboPlexAccount.Location = new System.Drawing.Point(97, 65);
      this.cboPlexAccount.Name = "cboPlexAccount";
      this.cboPlexAccount.Size = new System.Drawing.Size(444, 21);
      this.cboPlexAccount.TabIndex = 2;
      // 
      // cmdPlexDatabase
      // 
      this.cmdPlexDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.cmdPlexDatabase.Location = new System.Drawing.Point(547, 40);
      this.cmdPlexDatabase.Name = "cmdPlexDatabase";
      this.cmdPlexDatabase.Size = new System.Drawing.Size(29, 21);
      this.cmdPlexDatabase.TabIndex = 3;
      this.cmdPlexDatabase.Text = "...";
      this.cmdPlexDatabase.UseVisualStyleBackColor = true;
      this.cmdPlexDatabase.Click += new System.EventHandler(this.cmdPlexDatabase_Click);
      // 
      // lblPlexDatabase
      // 
      this.lblPlexDatabase.AutoSize = true;
      this.lblPlexDatabase.Location = new System.Drawing.Point(12, 44);
      this.lblPlexDatabase.Name = "lblPlexDatabase";
      this.lblPlexDatabase.Size = new System.Drawing.Size(79, 13);
      this.lblPlexDatabase.TabIndex = 4;
      this.lblPlexDatabase.Text = "Plex database:";
      // 
      // PlexAccount
      // 
      this.PlexAccount.AutoSize = true;
      this.PlexAccount.Location = new System.Drawing.Point(12, 68);
      this.PlexAccount.Name = "PlexAccount";
      this.PlexAccount.Size = new System.Drawing.Size(72, 13);
      this.PlexAccount.TabIndex = 5;
      this.PlexAccount.Text = "Plex account:";
      // 
      // openFileDialog
      // 
      this.openFileDialog.FileName = "openFileDialog";
      // 
      // grdPlaylists
      // 
      this.grdPlaylists.AllowUserToAddRows = false;
      this.grdPlaylists.AllowUserToDeleteRows = false;
      this.grdPlaylists.AllowUserToResizeRows = false;
      this.grdPlaylists.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.grdPlaylists.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
      this.grdPlaylists.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.grdPlaylists.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
      this.grdPlaylists.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.grdPlaylists.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colPlaylist,
            this.colChecked});
      this.grdPlaylists.Enabled = false;
      this.grdPlaylists.Location = new System.Drawing.Point(32, 175);
      this.grdPlaylists.Name = "grdPlaylists";
      this.grdPlaylists.RowHeadersVisible = false;
      this.grdPlaylists.Size = new System.Drawing.Size(544, 167);
      this.grdPlaylists.TabIndex = 6;
      // 
      // colPlaylist
      // 
      this.colPlaylist.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.colPlaylist.DataPropertyName = "Playlist";
      this.colPlaylist.FillWeight = 80F;
      this.colPlaylist.HeaderText = "Playlist";
      this.colPlaylist.Name = "colPlaylist";
      this.colPlaylist.ReadOnly = true;
      // 
      // colChecked
      // 
      this.colChecked.DataPropertyName = "Selected";
      this.colChecked.FillWeight = 20F;
      this.colChecked.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.colChecked.HeaderText = "Selected";
      this.colChecked.Name = "colChecked";
      // 
      // chkSyncPlaylists
      // 
      this.chkSyncPlaylists.AutoSize = true;
      this.chkSyncPlaylists.Location = new System.Drawing.Point(15, 119);
      this.chkSyncPlaylists.Name = "chkSyncPlaylists";
      this.chkSyncPlaylists.Size = new System.Drawing.Size(160, 17);
      this.chkSyncPlaylists.TabIndex = 10;
      this.chkSyncPlaylists.Text = "Sync iTunes playlists to Plex";
      this.chkSyncPlaylists.UseVisualStyleBackColor = true;
      this.chkSyncPlaylists.CheckedChanged += new System.EventHandler(this.chkSyncPlaylists_CheckedChanged);
      // 
      // lblItunesLibrary
      // 
      this.lblItunesLibrary.AutoSize = true;
      this.lblItunesLibrary.Enabled = false;
      this.lblItunesLibrary.Location = new System.Drawing.Point(29, 152);
      this.lblItunesLibrary.Name = "lblItunesLibrary";
      this.lblItunesLibrary.Size = new System.Drawing.Size(75, 13);
      this.lblItunesLibrary.TabIndex = 13;
      this.lblItunesLibrary.Text = "iTunes library:";
      // 
      // cmdItunesLibrary
      // 
      this.cmdItunesLibrary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.cmdItunesLibrary.Enabled = false;
      this.cmdItunesLibrary.Location = new System.Drawing.Point(547, 148);
      this.cmdItunesLibrary.Name = "cmdItunesLibrary";
      this.cmdItunesLibrary.Size = new System.Drawing.Size(29, 21);
      this.cmdItunesLibrary.TabIndex = 12;
      this.cmdItunesLibrary.Text = "...";
      this.cmdItunesLibrary.UseVisualStyleBackColor = true;
      this.cmdItunesLibrary.Click += new System.EventHandler(this.cmdItunesLibrary_Click);
      // 
      // txtItunesLibrary
      // 
      this.txtItunesLibrary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.txtItunesLibrary.Enabled = false;
      this.txtItunesLibrary.Location = new System.Drawing.Point(114, 148);
      this.txtItunesLibrary.Name = "txtItunesLibrary";
      this.txtItunesLibrary.Size = new System.Drawing.Size(427, 21);
      this.txtItunesLibrary.TabIndex = 11;
      this.txtItunesLibrary.TextChanged += new System.EventHandler(this.txtItunesLibrary_TextChanged);
      // 
      // chkRemoveEmptyPlaylists
      // 
      this.chkRemoveEmptyPlaylists.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.chkRemoveEmptyPlaylists.AutoSize = true;
      this.chkRemoveEmptyPlaylists.Enabled = false;
      this.chkRemoveEmptyPlaylists.Location = new System.Drawing.Point(32, 348);
      this.chkRemoveEmptyPlaylists.Name = "chkRemoveEmptyPlaylists";
      this.chkRemoveEmptyPlaylists.Size = new System.Drawing.Size(139, 17);
      this.chkRemoveEmptyPlaylists.TabIndex = 14;
      this.chkRemoveEmptyPlaylists.Text = "Remove empty Playlists";
      this.chkRemoveEmptyPlaylists.UseVisualStyleBackColor = true;
      // 
      // chkSyncRatings
      // 
      this.chkSyncRatings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.chkSyncRatings.AutoSize = true;
      this.chkSyncRatings.Location = new System.Drawing.Point(15, 398);
      this.chkSyncRatings.Name = "chkSyncRatings";
      this.chkSyncRatings.Size = new System.Drawing.Size(124, 17);
      this.chkSyncRatings.TabIndex = 15;
      this.chkSyncRatings.Text = "Sync Ratings to Plex";
      this.chkSyncRatings.UseVisualStyleBackColor = true;
      this.chkSyncRatings.CheckedChanged += new System.EventHandler(this.chkSyncRatings_CheckedChanged);
      // 
      // cmdCancel
      // 
      this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cmdCancel.Location = new System.Drawing.Point(503, 505);
      this.cmdCancel.Name = "cmdCancel";
      this.cmdCancel.Size = new System.Drawing.Size(75, 23);
      this.cmdCancel.TabIndex = 16;
      this.cmdCancel.Text = "Cancel";
      this.cmdCancel.UseVisualStyleBackColor = true;
      // 
      // cmdOk
      // 
      this.cmdOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.cmdOk.Location = new System.Drawing.Point(422, 505);
      this.cmdOk.Name = "cmdOk";
      this.cmdOk.Size = new System.Drawing.Size(75, 23);
      this.cmdOk.TabIndex = 17;
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
      this.cboSyncSource.Location = new System.Drawing.Point(323, 396);
      this.cboSyncSource.Name = "cboSyncSource";
      this.cboSyncSource.Size = new System.Drawing.Size(255, 21);
      this.cboSyncSource.TabIndex = 19;
      this.cboSyncSource.SelectedIndexChanged += new System.EventHandler(this.cboSyncSource_SelectedIndexChanged);
      // 
      // cboSyncMode
      // 
      this.cboSyncMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.cboSyncMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cboSyncMode.Enabled = false;
      this.cboSyncMode.FormattingEnabled = true;
      this.cboSyncMode.Location = new System.Drawing.Point(323, 423);
      this.cboSyncMode.Name = "cboSyncMode";
      this.cboSyncMode.Size = new System.Drawing.Size(255, 21);
      this.cboSyncMode.TabIndex = 20;
      // 
      // lblSyncSource
      // 
      this.lblSyncSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblSyncSource.AutoSize = true;
      this.lblSyncSource.Enabled = false;
      this.lblSyncSource.Location = new System.Drawing.Point(246, 399);
      this.lblSyncSource.Name = "lblSyncSource";
      this.lblSyncSource.Size = new System.Drawing.Size(69, 13);
      this.lblSyncSource.TabIndex = 21;
      this.lblSyncSource.Text = "Sync source:";
      // 
      // lblSyncMode
      // 
      this.lblSyncMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblSyncMode.AutoSize = true;
      this.lblSyncMode.Enabled = false;
      this.lblSyncMode.Location = new System.Drawing.Point(246, 426);
      this.lblSyncMode.Name = "lblSyncMode";
      this.lblSyncMode.Size = new System.Drawing.Size(63, 13);
      this.lblSyncMode.TabIndex = 22;
      this.lblSyncMode.Text = "Sync mode:";
      // 
      // lblClashWinner
      // 
      this.lblClashWinner.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblClashWinner.AutoSize = true;
      this.lblClashWinner.Enabled = false;
      this.lblClashWinner.Location = new System.Drawing.Point(246, 453);
      this.lblClashWinner.Name = "lblClashWinner";
      this.lblClashWinner.Size = new System.Drawing.Size(72, 13);
      this.lblClashWinner.TabIndex = 24;
      this.lblClashWinner.Text = "Clash winner:";
      // 
      // cboClashWinner
      // 
      this.cboClashWinner.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.cboClashWinner.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cboClashWinner.Enabled = false;
      this.cboClashWinner.FormattingEnabled = true;
      this.cboClashWinner.Location = new System.Drawing.Point(324, 450);
      this.cboClashWinner.Name = "cboClashWinner";
      this.cboClashWinner.Size = new System.Drawing.Size(255, 21);
      this.cboClashWinner.TabIndex = 23;
      // 
      // etchedLine4
      // 
      this.etchedLine4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.etchedLine4.Edge = DS.Controls.EtchEdge.Center;
      this.etchedLine4.Location = new System.Drawing.Point(-3, 479);
      this.etchedLine4.Name = "etchedLine4";
      this.etchedLine4.Size = new System.Drawing.Size(596, 21);
      this.etchedLine4.TabIndex = 18;
      this.etchedLine4.TextLabel = "";
      // 
      // etchedLine3
      // 
      this.etchedLine3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.etchedLine3.Edge = DS.Controls.EtchEdge.Center;
      this.etchedLine3.Location = new System.Drawing.Point(12, 371);
      this.etchedLine3.Name = "etchedLine3";
      this.etchedLine3.Size = new System.Drawing.Size(564, 21);
      this.etchedLine3.TabIndex = 9;
      this.etchedLine3.TextLabel = "Ratings ";
      // 
      // etchedLine2
      // 
      this.etchedLine2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.etchedLine2.Edge = DS.Controls.EtchEdge.Center;
      this.etchedLine2.Location = new System.Drawing.Point(12, 92);
      this.etchedLine2.Name = "etchedLine2";
      this.etchedLine2.Size = new System.Drawing.Size(564, 21);
      this.etchedLine2.TabIndex = 8;
      this.etchedLine2.TextLabel = "Playlists ";
      // 
      // etchedLine1
      // 
      this.etchedLine1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.etchedLine1.Edge = DS.Controls.EtchEdge.Center;
      this.etchedLine1.Location = new System.Drawing.Point(12, 12);
      this.etchedLine1.Name = "etchedLine1";
      this.etchedLine1.Size = new System.Drawing.Size(564, 21);
      this.etchedLine1.TabIndex = 7;
      this.etchedLine1.TextLabel = "Plex ";
      // 
      // Options2
      // 
      this.AcceptButton = this.cmdOk;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.cmdCancel;
      this.ClientSize = new System.Drawing.Size(590, 540);
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
      this.Controls.Add(this.chkRemoveEmptyPlaylists);
      this.Controls.Add(this.lblItunesLibrary);
      this.Controls.Add(this.cmdItunesLibrary);
      this.Controls.Add(this.txtItunesLibrary);
      this.Controls.Add(this.chkSyncPlaylists);
      this.Controls.Add(this.etchedLine3);
      this.Controls.Add(this.etchedLine2);
      this.Controls.Add(this.etchedLine1);
      this.Controls.Add(this.grdPlaylists);
      this.Controls.Add(this.PlexAccount);
      this.Controls.Add(this.lblPlexDatabase);
      this.Controls.Add(this.cmdPlexDatabase);
      this.Controls.Add(this.cboPlexAccount);
      this.Controls.Add(this.txtPlexDatabase);
      this.Font = new System.Drawing.Font("Tahoma", 8.25F);
      this.Name = "Options2";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Options2";
      this.Load += new System.EventHandler(this.Options2_Load);
      ((System.ComponentModel.ISupportInitialize)(this.grdPlaylists)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

		}

		#endregion
    private System.Windows.Forms.TextBox txtPlexDatabase;
    private System.Windows.Forms.ComboBox cboPlexAccount;
    private System.Windows.Forms.Button cmdPlexDatabase;
    private System.Windows.Forms.Label lblPlexDatabase;
    private System.Windows.Forms.Label PlexAccount;
    private System.Windows.Forms.OpenFileDialog openFileDialog;
    private System.Windows.Forms.DataGridView grdPlaylists;
    private Controls.EtchedLine etchedLine1;
    private Controls.EtchedLine etchedLine2;
    private Controls.EtchedLine etchedLine3;
    private System.Windows.Forms.CheckBox chkSyncPlaylists;
    private System.Windows.Forms.Label lblItunesLibrary;
    private System.Windows.Forms.Button cmdItunesLibrary;
    private System.Windows.Forms.TextBox txtItunesLibrary;
    private System.Windows.Forms.CheckBox chkRemoveEmptyPlaylists;
    private System.Windows.Forms.CheckBox chkSyncRatings;
    private System.Windows.Forms.Button cmdCancel;
    private System.Windows.Forms.Button cmdOk;
    private System.Windows.Forms.DataGridViewTextBoxColumn colPlaylist;
    private System.Windows.Forms.DataGridViewCheckBoxColumn colChecked;
    private Controls.EtchedLine etchedLine4;
    private System.Windows.Forms.ComboBox cboSyncSource;
    private System.Windows.Forms.ComboBox cboSyncMode;
    private System.Windows.Forms.Label lblSyncSource;
    private System.Windows.Forms.Label lblSyncMode;
    private System.Windows.Forms.Label lblClashWinner;
    private System.Windows.Forms.ComboBox cboClashWinner;
  }
}