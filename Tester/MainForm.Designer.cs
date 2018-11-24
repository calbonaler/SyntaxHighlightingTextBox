namespace Tester
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
			this.txtCode = new Controls.WindowsForms.SyntaxHighlightingTextBox();
			this.SuspendLayout();
			// 
			// txtCode
			// 
			this.txtCode.AcceptsTab = true;
			this.txtCode.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtCode.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtCode.Font = new System.Drawing.Font("ＭＳ ゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.txtCode.HighlightTokenizer = null;
			this.txtCode.Location = new System.Drawing.Point(0, 0);
			this.txtCode.Name = "txtCode";
			this.txtCode.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
			this.txtCode.Size = new System.Drawing.Size(413, 343);
			this.txtCode.TabIndex = 0;
			this.txtCode.Text = "";
			this.txtCode.WordWrap = false;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 18F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(413, 343);
			this.Controls.Add(this.txtCode);
			this.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "MainForm";
			this.Text = "MainForm";
			this.ResumeLayout(false);

		}

		#endregion

		private Controls.WindowsForms.SyntaxHighlightingTextBox txtCode;



	}
}