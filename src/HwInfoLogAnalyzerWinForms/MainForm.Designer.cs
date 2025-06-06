namespace HwInfoLogAnalyzerWinForms
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            openLogFileButton = new Button();
            logFilePathLabel = new Label();
            runButton = new Button();
            plotView = new OxyPlot.WindowsForms.PlotView();
            parserInfoLabel = new Label();
            reloadFileButton = new Button();
            SuspendLayout();
            // 
            // openLogFileButton
            // 
            openLogFileButton.AutoSize = true;
            openLogFileButton.Location = new Point(12, 12);
            openLogFileButton.Name = "openLogFileButton";
            openLogFileButton.Size = new Size(125, 30);
            openLogFileButton.TabIndex = 0;
            openLogFileButton.Text = "Open Log File";
            openLogFileButton.UseVisualStyleBackColor = true;
            openLogFileButton.Click += openLogFileButton_Click;
            // 
            // logFilePathLabel
            // 
            logFilePathLabel.AutoSize = true;
            logFilePathLabel.Location = new Point(274, 16);
            logFilePathLabel.Name = "logFilePathLabel";
            logFilePathLabel.Size = new Size(0, 20);
            logFilePathLabel.TabIndex = 1;
            // 
            // runButton
            // 
            runButton.Enabled = false;
            runButton.Location = new Point(12, 48);
            runButton.Name = "runButton";
            runButton.Size = new Size(94, 29);
            runButton.TabIndex = 2;
            runButton.Text = "Start";
            runButton.UseVisualStyleBackColor = true;
            runButton.Click += runButton_Click;
            // 
            // plotView
            // 
            plotView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            plotView.Location = new Point(12, 83);
            plotView.Name = "plotView";
            plotView.PanCursor = Cursors.Hand;
            plotView.Size = new Size(985, 652);
            plotView.TabIndex = 3;
            plotView.Text = "plotView1";
            plotView.ZoomHorizontalCursor = Cursors.SizeWE;
            plotView.ZoomRectangleCursor = Cursors.SizeNWSE;
            plotView.ZoomVerticalCursor = Cursors.SizeNS;
            // 
            // parserInfoLabel
            // 
            parserInfoLabel.AutoSize = true;
            parserInfoLabel.Location = new Point(112, 52);
            parserInfoLabel.Name = "parserInfoLabel";
            parserInfoLabel.Size = new Size(0, 20);
            parserInfoLabel.TabIndex = 4;
            // 
            // reloadFileButton
            // 
            reloadFileButton.Enabled = false;
            reloadFileButton.Location = new Point(143, 12);
            reloadFileButton.Name = "reloadFileButton";
            reloadFileButton.Size = new Size(125, 29);
            reloadFileButton.TabIndex = 5;
            reloadFileButton.Text = "Reload File";
            reloadFileButton.UseVisualStyleBackColor = true;
            reloadFileButton.Click += reloadFileButton_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1009, 747);
            Controls.Add(reloadFileButton);
            Controls.Add(parserInfoLabel);
            Controls.Add(plotView);
            Controls.Add(runButton);
            Controls.Add(logFilePathLabel);
            Controls.Add(openLogFileButton);
            Name = "MainForm";
            Text = "HWiNFO Log Analyzer";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button openLogFileButton;
        private Label logFilePathLabel;
        private Button runButton;
        private OxyPlot.WindowsForms.PlotView plotView;
        private Label parserInfoLabel;
        private Button reloadFileButton;
    }
}
