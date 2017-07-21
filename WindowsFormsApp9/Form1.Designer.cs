namespace WindowsFormsApp9
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSync = new System.Windows.Forms.Button();
            this.btnApm = new System.Windows.Forms.Button();
            this.btnTap = new System.Windows.Forms.Button();
            this.btnGetMovies = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSync
            // 
            this.btnSync.Location = new System.Drawing.Point(12, 12);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(75, 23);
            this.btnSync.TabIndex = 0;
            this.btnSync.Text = "Sync";
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // btnApm
            // 
            this.btnApm.Location = new System.Drawing.Point(93, 12);
            this.btnApm.Name = "btnApm";
            this.btnApm.Size = new System.Drawing.Size(75, 23);
            this.btnApm.TabIndex = 1;
            this.btnApm.Text = "APM";
            this.btnApm.UseVisualStyleBackColor = true;
            this.btnApm.Click += new System.EventHandler(this.btnApm_Click);
            // 
            // btnTap
            // 
            this.btnTap.Location = new System.Drawing.Point(174, 12);
            this.btnTap.Name = "btnTap";
            this.btnTap.Size = new System.Drawing.Size(75, 23);
            this.btnTap.TabIndex = 2;
            this.btnTap.Text = "TAP";
            this.btnTap.UseVisualStyleBackColor = true;
            this.btnTap.Click += new System.EventHandler(this.btnTap_Click);
            // 
            // btnGetMovies
            // 
            this.btnGetMovies.Location = new System.Drawing.Point(12, 41);
            this.btnGetMovies.Name = "btnGetMovies";
            this.btnGetMovies.Size = new System.Drawing.Size(75, 23);
            this.btnGetMovies.TabIndex = 3;
            this.btnGetMovies.Text = "Get Movies";
            this.btnGetMovies.UseVisualStyleBackColor = true;
            this.btnGetMovies.Click += new System.EventHandler(this.btnGetMovies_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 326);
            this.Controls.Add(this.btnGetMovies);
            this.Controls.Add(this.btnTap);
            this.Controls.Add(this.btnApm);
            this.Controls.Add(this.btnSync);
            this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.Button btnApm;
        private System.Windows.Forms.Button btnTap;
        private System.Windows.Forms.Button btnGetMovies;
    }
}

