namespace LocalApp.WindowsForms
{
    partial class PathfindImageForm
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
            this.imageBox = new System.Windows.Forms.PictureBox();
            this.goButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.textBox = new System.Windows.Forms.RichTextBox();
            this.workingButton = new System.Windows.Forms.Button();
            this.runningBox = new System.Windows.Forms.RichTextBox();
            this.nodeBox = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // imageBox
            // 
            this.imageBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.imageBox.Cursor = System.Windows.Forms.Cursors.Cross;
            this.imageBox.Location = new System.Drawing.Point(12, 12);
            this.imageBox.Name = "imageBox";
            this.imageBox.Size = new System.Drawing.Size(635, 637);
            this.imageBox.TabIndex = 0;
            this.imageBox.TabStop = false;
            this.imageBox.Click += new System.EventHandler(this.imageBox_Click);
            // 
            // goButton
            // 
            this.goButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(0)))), ((int)(((byte)(144)))));
            this.goButton.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.goButton.FlatAppearance.BorderSize = 0;
            this.goButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.goButton.Font = new System.Drawing.Font("Lucida Console", 39.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.goButton.ForeColor = System.Drawing.Color.White;
            this.goButton.Location = new System.Drawing.Point(653, 479);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(319, 82);
            this.goButton.TabIndex = 2;
            this.goButton.Text = "Pathfind";
            this.goButton.UseVisualStyleBackColor = false;
            this.goButton.Click += new System.EventHandler(this.goButton_Click);
            // 
            // exitButton
            // 
            this.exitButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(144)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.exitButton.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.exitButton.FlatAppearance.BorderSize = 0;
            this.exitButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.exitButton.Font = new System.Drawing.Font("Lucida Console", 39.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exitButton.ForeColor = System.Drawing.Color.White;
            this.exitButton.Location = new System.Drawing.Point(653, 567);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(319, 82);
            this.exitButton.TabIndex = 3;
            this.exitButton.Text = "Exit";
            this.exitButton.UseVisualStyleBackColor = false;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // textBox
            // 
            this.textBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(12)))));
            this.textBox.Font = new System.Drawing.Font("Lucida Console", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox.ForeColor = System.Drawing.SystemColors.Window;
            this.textBox.Location = new System.Drawing.Point(653, 12);
            this.textBox.Name = "textBox";
            this.textBox.ReadOnly = true;
            this.textBox.Size = new System.Drawing.Size(319, 461);
            this.textBox.TabIndex = 4;
            this.textBox.Text = "Instructions:\n\n- Left Click to set a Start node\n- Right Click to set an End node\n" +
    "\nClick \"Pathfind\" to get from Start to End (this may take some time)";
            // 
            // workingButton
            // 
            this.workingButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(144)))), ((int)(((byte)(17)))));
            this.workingButton.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.workingButton.FlatAppearance.BorderSize = 0;
            this.workingButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.workingButton.Font = new System.Drawing.Font("Lucida Console", 39.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.workingButton.ForeColor = System.Drawing.Color.White;
            this.workingButton.Location = new System.Drawing.Point(653, 391);
            this.workingButton.Name = "workingButton";
            this.workingButton.Size = new System.Drawing.Size(319, 82);
            this.workingButton.TabIndex = 5;
            this.workingButton.Text = "Working";
            this.workingButton.UseVisualStyleBackColor = false;
            // 
            // runningBox
            // 
            this.runningBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(12)))));
            this.runningBox.Font = new System.Drawing.Font("Lucida Console", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.runningBox.ForeColor = System.Drawing.SystemColors.Window;
            this.runningBox.Location = new System.Drawing.Point(328, 12);
            this.runningBox.Name = "runningBox";
            this.runningBox.ReadOnly = true;
            this.runningBox.Size = new System.Drawing.Size(319, 461);
            this.runningBox.TabIndex = 6;
            this.runningBox.Text = "If you see this then an error has occured please see logs for more information.";
            // 
            // nodeBox
            // 
            this.nodeBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(12)))));
            this.nodeBox.Font = new System.Drawing.Font("Lucida Console", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nodeBox.ForeColor = System.Drawing.SystemColors.Window;
            this.nodeBox.Location = new System.Drawing.Point(12, 597);
            this.nodeBox.Name = "nodeBox";
            this.nodeBox.ReadOnly = true;
            this.nodeBox.Size = new System.Drawing.Size(281, 52);
            this.nodeBox.TabIndex = 7;
            this.nodeBox.Text = "NODE COUNT";
            // 
            // PathfindImageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(12)))));
            this.ClientSize = new System.Drawing.Size(984, 661);
            this.Controls.Add(this.nodeBox);
            this.Controls.Add(this.runningBox);
            this.Controls.Add(this.workingButton);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.goButton);
            this.Controls.Add(this.imageBox);
            this.Name = "PathfindImageForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.ViewImageForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imageBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox imageBox;
        private System.Windows.Forms.Button goButton;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.RichTextBox textBox;
        private System.Windows.Forms.Button workingButton;
        private System.Windows.Forms.RichTextBox runningBox;
        private System.Windows.Forms.RichTextBox nodeBox;
    }
}