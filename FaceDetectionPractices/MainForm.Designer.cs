
namespace FaceDetectionPractices
{
    partial class MainForm
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
            this.EyeTrackStartButton = new System.Windows.Forms.Button();
            this.StopButton = new System.Windows.Forms.Button();
            this.textBox = new System.Windows.Forms.TextBox();
            this.HeadCoordinateStartButton = new System.Windows.Forms.Button();
            this.HeadPoseEstimateStartButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // EyeTrackStartButton
            // 
            this.EyeTrackStartButton.Location = new System.Drawing.Point(650, 12);
            this.EyeTrackStartButton.Name = "EyeTrackStartButton";
            this.EyeTrackStartButton.Size = new System.Drawing.Size(160, 80);
            this.EyeTrackStartButton.TabIndex = 0;
            this.EyeTrackStartButton.Text = "EyeTrack Start";
            this.EyeTrackStartButton.UseVisualStyleBackColor = true;
            this.EyeTrackStartButton.Click += new System.EventHandler(this.EyeTrackStartButton_Click);
            // 
            // StopButton
            // 
            this.StopButton.Location = new System.Drawing.Point(650, 270);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(160, 80);
            this.StopButton.TabIndex = 1;
            this.StopButton.Text = "Stop";
            this.StopButton.UseVisualStyleBackColor = true;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(12, 12);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox.Size = new System.Drawing.Size(632, 529);
            this.textBox.TabIndex = 2;
            // 
            // HeadCoordinateStartButton
            // 
            this.HeadCoordinateStartButton.Location = new System.Drawing.Point(650, 98);
            this.HeadCoordinateStartButton.Name = "HeadCoordinateStartButton";
            this.HeadCoordinateStartButton.Size = new System.Drawing.Size(160, 80);
            this.HeadCoordinateStartButton.TabIndex = 3;
            this.HeadCoordinateStartButton.Text = "HeadCoordinate Start";
            this.HeadCoordinateStartButton.UseVisualStyleBackColor = true;
            this.HeadCoordinateStartButton.Click += new System.EventHandler(this.HeadCoordinateStartButton_Click);
            // 
            // HeadPoseEstimateStartButton
            // 
            this.HeadPoseEstimateStartButton.Location = new System.Drawing.Point(650, 184);
            this.HeadPoseEstimateStartButton.Name = "HeadPoseEstimateStartButton";
            this.HeadPoseEstimateStartButton.Size = new System.Drawing.Size(160, 80);
            this.HeadPoseEstimateStartButton.TabIndex = 4;
            this.HeadPoseEstimateStartButton.Text = "HeadPoseEstimate Start";
            this.HeadPoseEstimateStartButton.UseVisualStyleBackColor = true;
            this.HeadPoseEstimateStartButton.Click += new System.EventHandler(this.HeadPoseEstimateStartButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(822, 553);
            this.Controls.Add(this.HeadPoseEstimateStartButton);
            this.Controls.Add(this.HeadCoordinateStartButton);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.EyeTrackStartButton);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button EyeTrackStartButton;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Button HeadCoordinateStartButton;
        private System.Windows.Forms.Button HeadPoseEstimateStartButton;
    }
}

