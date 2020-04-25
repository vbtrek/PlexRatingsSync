namespace DS.PlexRatingsSync
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
         this.progressBar1 = new System.Windows.Forms.ProgressBar();
         this.lblStatus = new System.Windows.Forms.Label();
         this.bwProcess = new System.ComponentModel.BackgroundWorker();
         this.lblProgress = new System.Windows.Forms.Label();
         this.menuStrip1 = new System.Windows.Forms.MenuStrip();
         this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuOptions = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
         this.cmdSync = new System.Windows.Forms.Button();
         this.chkSyncRatings = new System.Windows.Forms.CheckBox();
         this.chkSyncPlaylists = new System.Windows.Forms.CheckBox();
         this.menuStrip1.SuspendLayout();
         this.SuspendLayout();
         // 
         // progressBar1
         // 
         this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.progressBar1.Location = new System.Drawing.Point(12, 107);
         this.progressBar1.Name = "progressBar1";
         this.progressBar1.Size = new System.Drawing.Size(465, 23);
         this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
         this.progressBar1.TabIndex = 0;
         // 
         // lblStatus
         // 
         this.lblStatus.AutoSize = true;
         this.lblStatus.Location = new System.Drawing.Point(12, 83);
         this.lblStatus.Name = "lblStatus";
         this.lblStatus.Size = new System.Drawing.Size(48, 13);
         this.lblStatus.TabIndex = 1;
         this.lblStatus.Text = "lblStatus";
         // 
         // bwProcess
         // 
         this.bwProcess.WorkerReportsProgress = true;
         this.bwProcess.WorkerSupportsCancellation = true;
         this.bwProcess.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwProcess_DoWork);
         this.bwProcess.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bwProcess_ProgressChanged);
         this.bwProcess.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwProcess_RunWorkerCompleted);
         // 
         // lblProgress
         // 
         this.lblProgress.AutoSize = true;
         this.lblProgress.Location = new System.Drawing.Point(12, 142);
         this.lblProgress.Name = "lblProgress";
         this.lblProgress.Size = new System.Drawing.Size(59, 13);
         this.lblProgress.TabIndex = 2;
         this.lblProgress.Text = "lblProgress";
         // 
         // menuStrip1
         // 
         this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile});
         this.menuStrip1.Location = new System.Drawing.Point(0, 0);
         this.menuStrip1.Name = "menuStrip1";
         this.menuStrip1.Size = new System.Drawing.Size(489, 24);
         this.menuStrip1.TabIndex = 6;
         this.menuStrip1.Text = "menuStrip1";
         // 
         // mnuFile
         // 
         this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuOptions,
            this.mnuExit});
         this.mnuFile.Name = "mnuFile";
         this.mnuFile.Size = new System.Drawing.Size(37, 20);
         this.mnuFile.Text = "File";
         // 
         // mnuOptions
         // 
         this.mnuOptions.Name = "mnuOptions";
         this.mnuOptions.Size = new System.Drawing.Size(116, 22);
         this.mnuOptions.Text = "Options";
         this.mnuOptions.Click += new System.EventHandler(this.mnuOptions_Click);
         // 
         // mnuExit
         // 
         this.mnuExit.Name = "mnuExit";
         this.mnuExit.Size = new System.Drawing.Size(116, 22);
         this.mnuExit.Text = "Exit";
         this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
         // 
         // cmdSync
         // 
         this.cmdSync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.cmdSync.Location = new System.Drawing.Point(375, 27);
         this.cmdSync.Name = "cmdSync";
         this.cmdSync.Size = new System.Drawing.Size(102, 44);
         this.cmdSync.TabIndex = 7;
         this.cmdSync.Text = "Start sync";
         this.cmdSync.UseVisualStyleBackColor = true;
         this.cmdSync.Click += new System.EventHandler(this.cmdSync_Click);
         // 
         // chkSyncRatings
         // 
         this.chkSyncRatings.AutoSize = true;
         this.chkSyncRatings.Location = new System.Drawing.Point(12, 31);
         this.chkSyncRatings.Name = "chkSyncRatings";
         this.chkSyncRatings.Size = new System.Drawing.Size(88, 17);
         this.chkSyncRatings.TabIndex = 8;
         this.chkSyncRatings.Text = "Sync Ratings";
         this.chkSyncRatings.UseVisualStyleBackColor = true;
         this.chkSyncRatings.CheckedChanged += new System.EventHandler(this.chkSyncRatings_CheckedChanged);
         // 
         // chkSyncPlaylists
         // 
         this.chkSyncPlaylists.AutoSize = true;
         this.chkSyncPlaylists.Location = new System.Drawing.Point(12, 54);
         this.chkSyncPlaylists.Name = "chkSyncPlaylists";
         this.chkSyncPlaylists.Size = new System.Drawing.Size(124, 17);
         this.chkSyncPlaylists.TabIndex = 9;
         this.chkSyncPlaylists.Text = "Sync iTunes playlists";
         this.chkSyncPlaylists.UseVisualStyleBackColor = true;
         this.chkSyncPlaylists.CheckedChanged += new System.EventHandler(this.chkSyncPlaylists_CheckedChanged);
         // 
         // Main
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(489, 177);
         this.Controls.Add(this.chkSyncPlaylists);
         this.Controls.Add(this.chkSyncRatings);
         this.Controls.Add(this.cmdSync);
         this.Controls.Add(this.lblProgress);
         this.Controls.Add(this.lblStatus);
         this.Controls.Add(this.progressBar1);
         this.Controls.Add(this.menuStrip1);
         this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MainMenuStrip = this.menuStrip1;
         this.MaximizeBox = false;
         this.Name = "Main";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.Text = "Plex Ratings/Playlists Sync";
         this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
         this.Load += new System.EventHandler(this.Main_Load);
         this.menuStrip1.ResumeLayout(false);
         this.menuStrip1.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblStatus;
        private System.ComponentModel.BackgroundWorker bwProcess;
        private System.Windows.Forms.Label lblProgress;
      private System.Windows.Forms.MenuStrip menuStrip1;
      private System.Windows.Forms.ToolStripMenuItem mnuFile;
      private System.Windows.Forms.ToolStripMenuItem mnuOptions;
      private System.Windows.Forms.ToolStripMenuItem mnuExit;
      private System.Windows.Forms.Button cmdSync;
      private System.Windows.Forms.CheckBox chkSyncRatings;
      private System.Windows.Forms.CheckBox chkSyncPlaylists;
   }
}

