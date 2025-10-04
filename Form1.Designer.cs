namespace unidasmwrapper
{
    partial class Form1
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
            menuStrip1 = new MenuStrip();
            MI_Files = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            MI_Exit = new ToolStripMenuItem();
            MI_Select = new ToolStripMenuItem();
            MI_CPUType = new ToolStripMenuItem();
            MI_Architecture = new ToolStripMenuItem();
            MI_Setting = new ToolStripMenuItem();
            menuit_SetUnidasm = new ToolStripMenuItem();
            MI_Utility = new ToolStripMenuItem();
            MI_Unite8x2 = new ToolStripMenuItem();
            MI_Unite8x4 = new ToolStripMenuItem();
            MI_Unite16x2 = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            ts_unidasmchecker = new ToolStripStatusLabel();
            ts_SelectedCPU = new ToolStripStatusLabel();
            ts_Status = new ToolStripStatusLabel();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { MI_Files, MI_Select, MI_Setting, MI_Utility });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // MI_Files
            // 
            MI_Files.DropDownItems.AddRange(new ToolStripItem[] { toolStripSeparator1, MI_Exit });
            MI_Files.Name = "MI_Files";
            MI_Files.Size = new Size(67, 20);
            MI_Files.Text = "ファイル(&F)";
            MI_Files.Click += ファイルToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(177, 6);
            // 
            // MI_Exit
            // 
            MI_Exit.Name = "MI_Exit";
            MI_Exit.Size = new Size(180, 22);
            MI_Exit.Text = "終了(&X)";
            MI_Exit.Click += MI_Exit_Click;
            // 
            // MI_Select
            // 
            MI_Select.DropDownItems.AddRange(new ToolStripItem[] { MI_CPUType, MI_Architecture });
            MI_Select.Name = "MI_Select";
            MI_Select.Size = new Size(57, 20);
            MI_Select.Text = "選択(&L)";
            // 
            // MI_CPUType
            // 
            MI_CPUType.Enabled = false;
            MI_CPUType.Name = "MI_CPUType";
            MI_CPUType.Size = new Size(180, 22);
            MI_CPUType.Text = "CPU(&C)";
            // 
            // MI_Architecture
            // 
            MI_Architecture.Enabled = false;
            MI_Architecture.Name = "MI_Architecture";
            MI_Architecture.Size = new Size(180, 22);
            MI_Architecture.Text = "アーキテクチャ(&A)";
            // 
            // MI_Setting
            // 
            MI_Setting.DropDownItems.AddRange(new ToolStripItem[] { menuit_SetUnidasm });
            MI_Setting.Name = "MI_Setting";
            MI_Setting.Size = new Size(57, 20);
            MI_Setting.Text = "設定(&S)";
            // 
            // menuit_SetUnidasm
            // 
            menuit_SetUnidasm.Name = "menuit_SetUnidasm";
            menuit_SetUnidasm.Size = new Size(169, 22);
            menuit_SetUnidasm.Text = "unidasmの指定(&U)";
            menuit_SetUnidasm.Click += menuit_SetUnidasm_Click;
            // 
            // MI_Utility
            // 
            MI_Utility.DropDownItems.AddRange(new ToolStripItem[] { MI_Unite8x2, MI_Unite8x4, MI_Unite16x2 });
            MI_Utility.Name = "MI_Utility";
            MI_Utility.Size = new Size(97, 20);
            MI_Utility.Text = "ユーティリティ(&U))";
            // 
            // MI_Unite8x2
            // 
            MI_Unite8x2.Name = "MI_Unite8x2";
            MI_Unite8x2.Size = new Size(192, 22);
            MI_Unite8x2.Text = "ファイル連結(8bitx2)(&2)";
            MI_Unite8x2.Click += MI_Unite8x2_Click;
            // 
            // MI_Unite8x4
            // 
            MI_Unite8x4.Name = "MI_Unite8x4";
            MI_Unite8x4.Size = new Size(192, 22);
            MI_Unite8x4.Text = "ファイル連結(8bitx4)(&4)";
            // 
            // MI_Unite16x2
            // 
            MI_Unite16x2.Name = "MI_Unite16x2";
            MI_Unite16x2.Size = new Size(192, 22);
            MI_Unite16x2.Text = "ファイル連結(16bitx2)(&6)";
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { ts_unidasmchecker, ts_SelectedCPU, ts_Status });
            statusStrip1.Location = new Point(0, 428);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(800, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // ts_unidasmchecker
            // 
            ts_unidasmchecker.Name = "ts_unidasmchecker";
            ts_unidasmchecker.Size = new Size(111, 17);
            ts_unidasmchecker.Text = "unidasm: not found";
            // 
            // ts_SelectedCPU
            // 
            ts_SelectedCPU.Name = "ts_SelectedCPU";
            ts_SelectedCPU.Size = new Size(64, 17);
            ts_SelectedCPU.Text = "CPU=none";
            // 
            // ts_Status
            // 
            ts_Status.Name = "ts_Status";
            ts_Status.Size = new Size(42, 17);
            ts_Status.Text = "Status:";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Unidasm Wrapper";
            FormClosing += Form1_FormClosing;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel ts_unidasmchecker;
        private ToolStripMenuItem MI_Files;
        private ToolStripStatusLabel ts_Status;
        private ToolStripMenuItem MI_Setting;
        private ToolStripMenuItem MI_Select;
        private ToolStripMenuItem MI_CPUType;
        private ToolStripMenuItem MI_Architecture;
        private ToolStripMenuItem menuit_SetUnidasm;
        private ToolStripMenuItem 設定SToolStripMenuItem;
        private ToolStripStatusLabel ts_SelectedCPU;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem MI_Exit;
        private ToolStripMenuItem MI_Utility;
        private ToolStripMenuItem MI_Unite8x2;
        private ToolStripMenuItem MI_Unite8x4;
        private ToolStripMenuItem MI_Unite16x2;
    }
}
