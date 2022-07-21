namespace FinalSolution.src.local.forms
{
    partial class PreviewImage
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
            this.next = new System.Windows.Forms.Button();
            this.currentStage = new System.Windows.Forms.Label();
            this.back = new System.Windows.Forms.Button();
            this.nextStage = new System.Windows.Forms.Label();
            this.parameters = new System.Windows.Forms.ListBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.imageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // imageBox
            // 
            this.imageBox.Location = new System.Drawing.Point(12, 12);
            this.imageBox.Name = "imageBox";
            this.imageBox.Size = new System.Drawing.Size(500, 450);
            this.imageBox.TabIndex = 0;
            this.imageBox.TabStop = false;
            // 
            // next
            // 
            this.next.Font = new System.Drawing.Font("JetBrains Mono SemiBold", 24.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.next.Location = new System.Drawing.Point(518, 384);
            this.next.Name = "next";
            this.next.Size = new System.Drawing.Size(354, 78);
            this.next.TabIndex = 1;
            this.next.Text = "Next";
            this.next.UseVisualStyleBackColor = true;
            this.next.Click += new System.EventHandler(this.next_Click);
            // 
            // currentStage
            // 
            this.currentStage.AutoSize = true;
            this.currentStage.Font = new System.Drawing.Font("JetBrains Mono SemiBold", 14F, System.Drawing.FontStyle.Bold);
            this.currentStage.Location = new System.Drawing.Point(518, 12);
            this.currentStage.Name = "currentStage";
            this.currentStage.Size = new System.Drawing.Size(166, 25);
            this.currentStage.TabIndex = 2;
            this.currentStage.Text = "Current Stage:";
            // 
            // back
            // 
            this.back.Font = new System.Drawing.Font("JetBrains Mono SemiBold", 24.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.back.Location = new System.Drawing.Point(518, 300);
            this.back.Name = "back";
            this.back.Size = new System.Drawing.Size(354, 78);
            this.back.TabIndex = 3;
            this.back.Text = "Back";
            this.back.UseVisualStyleBackColor = true;
            this.back.Click += new System.EventHandler(this.back_Click);
            // 
            // nextStage
            // 
            this.nextStage.AutoSize = true;
            this.nextStage.Font = new System.Drawing.Font("JetBrains Mono SemiBold", 14F, System.Drawing.FontStyle.Bold);
            this.nextStage.Location = new System.Drawing.Point(518, 37);
            this.nextStage.Name = "nextStage";
            this.nextStage.Size = new System.Drawing.Size(133, 25);
            this.nextStage.TabIndex = 4;
            this.nextStage.Text = "Next Stage:";
            // 
            // parameters
            // 
            this.parameters.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.parameters.Font = new System.Drawing.Font("JetBrains Mono SemiBold", 14F, System.Drawing.FontStyle.Bold);
            this.parameters.FormattingEnabled = true;
            this.parameters.ItemHeight = 25;
            this.parameters.Location = new System.Drawing.Point(523, 65);
            this.parameters.Name = "parameters";
            this.parameters.Size = new System.Drawing.Size(349, 227);
            this.parameters.TabIndex = 6;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // PreviewImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 474);
            this.Controls.Add(this.parameters);
            this.Controls.Add(this.nextStage);
            this.Controls.Add(this.back);
            this.Controls.Add(this.currentStage);
            this.Controls.Add(this.next);
            this.Controls.Add(this.imageBox);
            this.Name = "PreviewImage";
            this.Text = "PreviewImage";
            this.Load += new System.EventHandler(this.PreviewImage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imageBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox imageBox;
        private System.Windows.Forms.Button next;
        private System.Windows.Forms.Label currentStage;
        private System.Windows.Forms.Button back;
        private System.Windows.Forms.Label nextStage;
        private System.Windows.Forms.ListBox parameters;
        private System.Windows.Forms.ImageList imageList1;
    }
}