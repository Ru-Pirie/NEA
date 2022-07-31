namespace FinalSolution.src.local.forms
{
    partial class PathfindImage
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
            this.imageBox = new System.Windows.Forms.PictureBox();
            this.pathfindButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.instruction = new System.Windows.Forms.Label();
            this.clearButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // imageBox
            // 
            this.imageBox.Location = new System.Drawing.Point(12, 12);
            this.imageBox.Name = "imageBox";
            this.imageBox.Size = new System.Drawing.Size(500, 450);
            this.imageBox.TabIndex = 2;
            this.imageBox.TabStop = false;
            this.imageBox.Click += new System.EventHandler(this.imageBox_Click);
            // 
            // pathfindButton
            // 
            this.pathfindButton.Font = new System.Drawing.Font("JetBrains Mono SemiBold", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pathfindButton.Location = new System.Drawing.Point(518, 384);
            this.pathfindButton.Name = "pathfindButton";
            this.pathfindButton.Size = new System.Drawing.Size(177, 78);
            this.pathfindButton.TabIndex = 3;
            this.pathfindButton.Text = "Pathfind";
            this.pathfindButton.UseVisualStyleBackColor = true;
            this.pathfindButton.Visible = false;
            this.pathfindButton.Click += new System.EventHandler(this.pathfindButton_Click);
            // 
            // exitButton
            // 
            this.exitButton.Font = new System.Drawing.Font("JetBrains Mono SemiBold", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exitButton.Location = new System.Drawing.Point(695, 384);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(177, 78);
            this.exitButton.TabIndex = 4;
            this.exitButton.Text = "Exit";
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // instruction
            // 
            this.instruction.AutoSize = true;
            this.instruction.Font = new System.Drawing.Font("JetBrains Mono SemiBold", 14F, System.Drawing.FontStyle.Bold);
            this.instruction.Location = new System.Drawing.Point(518, 12);
            this.instruction.Name = "instruction";
            this.instruction.Size = new System.Drawing.Size(144, 25);
            this.instruction.TabIndex = 5;
            this.instruction.Text = "Instruction:";
            // 
            // clearButton
            // 
            this.clearButton.Font = new System.Drawing.Font("JetBrains Mono SemiBold", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clearButton.Location = new System.Drawing.Point(518, 339);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(354, 45);
            this.clearButton.TabIndex = 6;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Visible = false;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // PathfindImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 474);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.instruction);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.pathfindButton);
            this.Controls.Add(this.imageBox);
            this.Name = "PathfindImage";
            this.Text = "PathfindImage";
            ((System.ComponentModel.ISupportInitialize)(this.imageBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox imageBox;
        private System.Windows.Forms.Button pathfindButton;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Label instruction;
        private System.Windows.Forms.Button clearButton;
    }
}