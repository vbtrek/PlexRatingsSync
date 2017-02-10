namespace DS.PlexRatingsSync
{
    partial class Options
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Options));
            this.grdConfig = new DS.Controls.PropertyGridEx();
            this.SuspendLayout();
            // 
            // grdConfig
            // 
            this.grdConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.grdConfig.DocCommentDescription.AccessibleName = "";
            this.grdConfig.DocCommentDescription.AutoEllipsis = true;
            this.grdConfig.DocCommentDescription.Cursor = System.Windows.Forms.Cursors.Default;
            this.grdConfig.DocCommentDescription.Location = new System.Drawing.Point(3, 18);
            this.grdConfig.DocCommentDescription.Name = "";
            this.grdConfig.DocCommentDescription.Size = new System.Drawing.Size(558, 39);
            this.grdConfig.DocCommentDescription.TabIndex = 1;
            this.grdConfig.DocCommentImage = null;
            // 
            // 
            // 
            this.grdConfig.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.grdConfig.DocCommentTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.grdConfig.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this.grdConfig.DocCommentTitle.Name = "";
            this.grdConfig.DocCommentTitle.Size = new System.Drawing.Size(558, 15);
            this.grdConfig.DocCommentTitle.TabIndex = 0;
            this.grdConfig.DocCommentTitle.UseMnemonic = false;
            this.grdConfig.DrawFlatToolbar = true;
            this.grdConfig.HelpSectionNumberOfLines = 4;
            this.grdConfig.Location = new System.Drawing.Point(12, 12);
            this.grdConfig.Name = "grdConfig";
            this.grdConfig.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.grdConfig.SelectedObject = ((object)(resources.GetObject("grdConfig.SelectedObject")));
            this.grdConfig.ShowCustomProperties = true;
            this.grdConfig.Size = new System.Drawing.Size(564, 417);
            this.grdConfig.TabIndex = 0;
            this.grdConfig.ToolbarVisible = false;
            // 
            // 
            // 
            this.grdConfig.ToolStrip.AccessibleName = "ToolBar";
            this.grdConfig.ToolStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
            this.grdConfig.ToolStrip.AllowMerge = false;
            this.grdConfig.ToolStrip.AutoSize = false;
            this.grdConfig.ToolStrip.CanOverflow = false;
            this.grdConfig.ToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.grdConfig.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.grdConfig.ToolStrip.Location = new System.Drawing.Point(0, 1);
            this.grdConfig.ToolStrip.Name = "";
            this.grdConfig.ToolStrip.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this.grdConfig.ToolStrip.Size = new System.Drawing.Size(276, 25);
            this.grdConfig.ToolStrip.TabIndex = 1;
            this.grdConfig.ToolStrip.TabStop = true;
            this.grdConfig.ToolStrip.Text = "PropertyGridToolBar";
            this.grdConfig.ToolStrip.Visible = false;
            this.grdConfig.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.grdConfig_PropertyValueChanged);
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(588, 441);
            this.Controls.Add(this.grdConfig);
            this.Name = "Options";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.Load += new System.EventHandler(this.Options_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private DS.Controls.PropertyGridEx grdConfig;
    }
}