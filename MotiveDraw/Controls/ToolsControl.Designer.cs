namespace MotiveDraw.Controls
{
    partial class ToolsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btRect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btRect
            // 
            this.btRect.Location = new System.Drawing.Point(19, 32);
            this.btRect.Name = "btRect";
            this.btRect.Size = new System.Drawing.Size(75, 69);
            this.btRect.TabIndex = 0;
            this.btRect.Text = "Rect";
            this.btRect.UseVisualStyleBackColor = true;
            // 
            // ToolsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btRect);
            this.Name = "ToolsControl";
            this.Size = new System.Drawing.Size(150, 871);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btRect;
    }
}
