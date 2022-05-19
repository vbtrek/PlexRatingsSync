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
      this.lblTotals = new System.Windows.Forms.Label();
      this.lblPlex = new System.Windows.Forms.Label();
      this.lblItunes = new System.Windows.Forms.Label();
      this.cmdOptions = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // progressBar1
      // 
      this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.progressBar1.Location = new System.Drawing.Point(12, 75);
      this.progressBar1.Name = "progressBar1";
      this.progressBar1.Size = new System.Drawing.Size(465, 23);
      this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
      this.progressBar1.TabIndex = 0;
      // 
      // lblStatus
      // 
      this.lblStatus.AutoSize = true;
      this.lblStatus.Location = new System.Drawing.Point(9, 59);
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
      // lblTotals
      // 
      this.lblTotals.AutoSize = true;
      this.lblTotals.Location = new System.Drawing.Point(9, 103);
      this.lblTotals.Name = "lblTotals";
      this.lblTotals.Size = new System.Drawing.Size(46, 13);
      this.lblTotals.TabIndex = 2;
      this.lblTotals.Text = "lblTotals";
      // 
      // lblPlex
      // 
      this.lblPlex.AutoSize = true;
      this.lblPlex.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblPlex.Location = new System.Drawing.Point(9, 9);
      this.lblPlex.Name = "lblPlex";
      this.lblPlex.Size = new System.Drawing.Size(49, 13);
      this.lblPlex.TabIndex = 3;
      this.lblPlex.Text = "lblPlex";
      // 
      // lblItunes
      // 
      this.lblItunes.AutoSize = true;
      this.lblItunes.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblItunes.Location = new System.Drawing.Point(9, 26);
      this.lblItunes.Name = "lblItunes";
      this.lblItunes.Size = new System.Drawing.Size(61, 13);
      this.lblItunes.TabIndex = 4;
      this.lblItunes.Text = "lblItunes";
      // 
      // cmdOptions
      // 
      this.cmdOptions.Location = new System.Drawing.Point(402, 9);
      this.cmdOptions.Name = "cmdOptions";
      this.cmdOptions.Size = new System.Drawing.Size(75, 23);
      this.cmdOptions.TabIndex = 5;
      this.cmdOptions.Text = "Options";
      this.cmdOptions.UseVisualStyleBackColor = true;
      this.cmdOptions.Click += new System.EventHandler(this.cmdOptions_Click);
      // 
      // Main
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(489, 124);
      this.Controls.Add(this.cmdOptions);
      this.Controls.Add(this.lblItunes);
      this.Controls.Add(this.lblPlex);
      this.Controls.Add(this.lblTotals);
      this.Controls.Add(this.lblStatus);
      this.Controls.Add(this.progressBar1);
      this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.Name = "Main";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Plex Ratings/Playlists Sync";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
      this.Load += new System.EventHandler(this.Main_Load);
      this.Shown += new System.EventHandler(this.Main_Shown);
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblStatus;
        private System.ComponentModel.BackgroundWorker bwProcess;
        private System.Windows.Forms.Label lblTotals;
        private System.Windows.Forms.Label lblPlex;
        private System.Windows.Forms.Label lblItunes;
        private System.Windows.Forms.Button cmdOptions;
    }
}

