namespace Lfz.AutoUpdater
{
    partial class FrmDownloadProgress
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmDownloadProgress));
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel3 = new System.Windows.Forms.Panel();
            this.buttonOk = new System.Windows.Forms.Button();
            this.pnDownlad = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.label3 = new System.Windows.Forms.Label();
            this.lbTotal2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lbFileName2 = new System.Windows.Forms.Label();
            this.lbRece = new System.Windows.Forms.Label();
            this.lbTotalKb = new System.Windows.Forms.Label();
            this.progressBarCurrent = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.labelCurrent = new System.Windows.Forms.Label();
            this.lbFrom = new System.Windows.Forms.Label();
            this.lbFileName = new System.Windows.Forms.Label();
            this.lbAppName = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.labelCurrentItem = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel3.SuspendLayout();
            this.pnDownlad.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitter2
            // 
            this.splitter2.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter2.Location = new System.Drawing.Point(0, 201);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(532, 2);
            this.splitter2.TabIndex = 12;
            this.splitter2.TabStop = false;
            // 
            // splitter1
            // 
            this.splitter1.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 63);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(532, 2);
            this.splitter1.TabIndex = 11;
            this.splitter1.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.buttonOk);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 203);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(532, 42);
            this.panel3.TabIndex = 9;
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(385, 9);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(83, 23);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "取消";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.OnCancel);
            // 
            // pnDownlad
            // 
            this.pnDownlad.Controls.Add(this.panel2);
            this.pnDownlad.Controls.Add(this.lbRece);
            this.pnDownlad.Controls.Add(this.lbTotalKb);
            this.pnDownlad.Controls.Add(this.progressBarCurrent);
            this.pnDownlad.Controls.Add(this.label1);
            this.pnDownlad.Controls.Add(this.labelCurrent);
            this.pnDownlad.Controls.Add(this.lbFrom);
            this.pnDownlad.Controls.Add(this.lbFileName);
            this.pnDownlad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnDownlad.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.pnDownlad.Location = new System.Drawing.Point(0, 63);
            this.pnDownlad.Name = "pnDownlad";
            this.pnDownlad.Size = new System.Drawing.Size(532, 182);
            this.pnDownlad.TabIndex = 10;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.progressBar2);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.lbTotal2);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.lbFileName2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(532, 182);
            this.panel2.TabIndex = 8;
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(15, 98);
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(492, 18);
            this.progressBar2.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 11;
            this.label3.Text = "处理进度：";
            // 
            // lbTotal2
            // 
            this.lbTotal2.AutoSize = true;
            this.lbTotal2.Location = new System.Drawing.Point(68, 45);
            this.lbTotal2.Name = "lbTotal2";
            this.lbTotal2.Size = new System.Drawing.Size(83, 12);
            this.lbTotal2.TabIndex = 10;
            this.lbTotal2.Text = "总大小（Kb）:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "总大小：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(245, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(137, 12);
            this.label5.TabIndex = 7;
            this.label5.Text = "来源：www.ziyouchi.com";
            // 
            // lbFileName2
            // 
            this.lbFileName2.AutoSize = true;
            this.lbFileName2.Location = new System.Drawing.Point(14, 16);
            this.lbFileName2.Name = "lbFileName2";
            this.lbFileName2.Size = new System.Drawing.Size(155, 12);
            this.lbFileName2.TabIndex = 8;
            this.lbFileName2.Text = "Name:  KnightsWarrior.exe";
            // 
            // lbRece
            // 
            this.lbRece.AutoSize = true;
            this.lbRece.Location = new System.Drawing.Point(68, 64);
            this.lbRece.Name = "lbRece";
            this.lbRece.Size = new System.Drawing.Size(95, 12);
            this.lbRece.TabIndex = 7;
            this.lbRece.Text = "已经下载（KB）:";
            // 
            // lbTotalKb
            // 
            this.lbTotalKb.AutoSize = true;
            this.lbTotalKb.Location = new System.Drawing.Point(68, 38);
            this.lbTotalKb.Name = "lbTotalKb";
            this.lbTotalKb.Size = new System.Drawing.Size(83, 12);
            this.lbTotalKb.TabIndex = 6;
            this.lbTotalKb.Text = "总大小（Kb）:";
            // 
            // progressBarCurrent
            // 
            this.progressBarCurrent.Location = new System.Drawing.Point(15, 79);
            this.progressBarCurrent.Name = "progressBarCurrent";
            this.progressBarCurrent.Size = new System.Drawing.Size(438, 12);
            this.progressBarCurrent.Step = 1;
            this.progressBarCurrent.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "总大小：";
            // 
            // labelCurrent
            // 
            this.labelCurrent.AutoSize = true;
            this.labelCurrent.Location = new System.Drawing.Point(14, 64);
            this.labelCurrent.Name = "labelCurrent";
            this.labelCurrent.Size = new System.Drawing.Size(53, 12);
            this.labelCurrent.TabIndex = 3;
            this.labelCurrent.Text = "已下载：";
            // 
            // lbFrom
            // 
            this.lbFrom.AutoSize = true;
            this.lbFrom.Location = new System.Drawing.Point(245, 9);
            this.lbFrom.Name = "lbFrom";
            this.lbFrom.Size = new System.Drawing.Size(137, 12);
            this.lbFrom.TabIndex = 0;
            this.lbFrom.Text = "来源：www.ziyouchi.com";
            // 
            // lbFileName
            // 
            this.lbFileName.AutoSize = true;
            this.lbFileName.Location = new System.Drawing.Point(14, 9);
            this.lbFileName.Name = "lbFileName";
            this.lbFileName.Size = new System.Drawing.Size(155, 12);
            this.lbFileName.TabIndex = 0;
            this.lbFileName.Text = "Name:  KnightsWarrior.exe";
            // 
            // lbAppName
            // 
            this.lbAppName.AutoSize = true;
            this.lbAppName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbAppName.Location = new System.Drawing.Point(11, 8);
            this.lbAppName.Name = "lbAppName";
            this.lbAppName.Size = new System.Drawing.Size(85, 13);
            this.lbAppName.TabIndex = 0;
            this.lbAppName.Text = "拓创升级助手";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(450, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(79, 55);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.lbAppName);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(532, 63);
            this.panel1.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(389, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "自动升级将耗费几分钟时间，你可以在升级过程使用电脑从事其它工作。";
            // 
            // labelCurrentItem
            // 
            this.labelCurrentItem.AutoSize = true;
            this.labelCurrentItem.Location = new System.Drawing.Point(86, 85);
            this.labelCurrentItem.Name = "labelCurrentItem";
            this.labelCurrentItem.Size = new System.Drawing.Size(0, 12);
            this.labelCurrentItem.TabIndex = 7;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FrmDownloadProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 245);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.pnDownlad);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.labelCurrentItem);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmDownloadProgress";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "拓创升级助手-文件下载中";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.panel3.ResumeLayout(false);
            this.pnDownlad.ResumeLayout(false);
            this.pnDownlad.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Panel pnDownlad;
        private System.Windows.Forms.ProgressBar progressBarCurrent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelCurrent;
        private System.Windows.Forms.Label lbAppName;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelCurrentItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbFrom;
        private System.Windows.Forms.Label lbFileName;
        private System.Windows.Forms.Label lbTotalKb;
        private System.Windows.Forms.Label lbRece;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbTotal2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbFileName2;
        private System.Windows.Forms.ProgressBar progressBar2;
        private System.Windows.Forms.Timer timer1;
    }
}