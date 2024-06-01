namespace D2RMuleGUI
{
    partial class FormInventory
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInventory));
            pictureBoxLeft = new PictureBox();
            listBoxCharacters = new ListBox();
            textBoxDirectory = new TextBox();
            buttonChangeDirectory = new Button();
            buttonRefresh = new Button();
            textBoxVaultFilter = new TextBox();
            toolStrip1 = new ToolStrip();
            toolStripDropDownButton1 = new ToolStripDropDownButton();
            exitToolStripMenuItem = new ToolStripMenuItem();
            toolStripDropDownButton2 = new ToolStripDropDownButton();
            toolStripMenuItemOptionAllToVaultIncludesInventory = new ToolStripMenuItem();
            toolStripMenuItemDupeOnVaultRemove = new ToolStripMenuItem();
            label2 = new Label();
            radioButtonClassic = new RadioButton();
            groupBox1 = new GroupBox();
            radioButtonExpansion = new RadioButton();
            groupBox2 = new GroupBox();
            radioButtonHardcore = new RadioButton();
            radioButtonNormal = new RadioButton();
            groupBox3 = new GroupBox();
            ((System.ComponentModel.ISupportInitialize)pictureBoxLeft).BeginInit();
            toolStrip1.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBoxLeft
            // 
            pictureBoxLeft.Enabled = false;
            pictureBoxLeft.Location = new Point(136, 112);
            pictureBoxLeft.Name = "pictureBoxLeft";
            pictureBoxLeft.Size = new Size(500, 1000);
            pictureBoxLeft.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxLeft.TabIndex = 0;
            pictureBoxLeft.TabStop = false;
            pictureBoxLeft.Visible = false;
            pictureBoxLeft.Click += pictureBoxLeft_Click;
            pictureBoxLeft.MouseMove += pictureBoxStash_MouseMove;
            // 
            // listBoxCharacters
            // 
            listBoxCharacters.FormattingEnabled = true;
            listBoxCharacters.ItemHeight = 15;
            listBoxCharacters.Location = new Point(8, 112);
            listBoxCharacters.Name = "listBoxCharacters";
            listBoxCharacters.Size = new Size(120, 469);
            listBoxCharacters.TabIndex = 5;
            listBoxCharacters.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            // 
            // textBoxDirectory
            // 
            textBoxDirectory.Location = new Point(8, 32);
            textBoxDirectory.Name = "textBoxDirectory";
            textBoxDirectory.Size = new Size(576, 23);
            textBoxDirectory.TabIndex = 6;
            textBoxDirectory.Text = "";
            // 
            // buttonChangeDirectory
            // 
            buttonChangeDirectory.Location = new Point(592, 32);
            buttonChangeDirectory.Name = "buttonChangeDirectory";
            buttonChangeDirectory.Size = new Size(32, 23);
            buttonChangeDirectory.TabIndex = 8;
            buttonChangeDirectory.Text = "...";
            buttonChangeDirectory.UseVisualStyleBackColor = true;
            buttonChangeDirectory.Click += buttonChangeDirectory_Click;
            // 
            // buttonRefresh
            // 
            buttonRefresh.Location = new Point(632, 32);
            buttonRefresh.Name = "buttonRefresh";
            buttonRefresh.Size = new Size(75, 23);
            buttonRefresh.TabIndex = 9;
            buttonRefresh.Text = "Refresh";
            buttonRefresh.UseVisualStyleBackColor = true;
            buttonRefresh.Click += buttonRefresh_Click;
            // 
            // textBoxVaultFilter
            // 
            textBoxVaultFilter.BackColor = Color.Black;
            textBoxVaultFilter.BorderStyle = BorderStyle.None;
            textBoxVaultFilter.Font = new Font("Formal436 BT", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBoxVaultFilter.ForeColor = Color.White;
            textBoxVaultFilter.Location = new Point(648, 120);
            textBoxVaultFilter.Name = "textBoxVaultFilter";
            textBoxVaultFilter.Size = new Size(100, 33);
            textBoxVaultFilter.TabIndex = 12;
            textBoxVaultFilter.TextAlign = HorizontalAlignment.Center;
            textBoxVaultFilter.Visible = false;
            textBoxVaultFilter.KeyDown += textBoxVaultFilter_KeyDown;
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripDropDownButton1, toolStripDropDownButton2 });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1449, 25);
            toolStrip1.TabIndex = 14;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            toolStripDropDownButton1.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripDropDownButton1.DropDownItems.AddRange(new ToolStripItem[] { exitToolStripMenuItem });
            toolStripDropDownButton1.Image = (Image)resources.GetObject("toolStripDropDownButton1.Image");
            toolStripDropDownButton1.ImageTransparentColor = Color.Magenta;
            toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            toolStripDropDownButton1.Size = new Size(38, 22);
            toolStripDropDownButton1.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(93, 22);
            exitToolStripMenuItem.Text = "Exit";
            // 
            // toolStripDropDownButton2
            // 
            toolStripDropDownButton2.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripDropDownButton2.DropDownItems.AddRange(new ToolStripItem[] { toolStripMenuItemOptionAllToVaultIncludesInventory, toolStripMenuItemDupeOnVaultRemove });
            toolStripDropDownButton2.Image = (Image)resources.GetObject("toolStripDropDownButton2.Image");
            toolStripDropDownButton2.ImageTransparentColor = Color.Magenta;
            toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            toolStripDropDownButton2.Size = new Size(62, 22);
            toolStripDropDownButton2.Text = "Options";
            // 
            // toolStripMenuItemOptionAllToVaultIncludesInventory
            // 
            toolStripMenuItemOptionAllToVaultIncludesInventory.CheckOnClick = true;
            toolStripMenuItemOptionAllToVaultIncludesInventory.Name = "toolStripMenuItemOptionAllToVaultIncludesInventory";
            toolStripMenuItemOptionAllToVaultIncludesInventory.Size = new Size(276, 22);
            toolStripMenuItemOptionAllToVaultIncludesInventory.Text = "\"All to Vault\" includes player inventory";
            // 
            // toolStripMenuItemDupeOnVaultRemove
            // 
            toolStripMenuItemDupeOnVaultRemove.CheckOnClick = true;
            toolStripMenuItemDupeOnVaultRemove.Name = "toolStripMenuItemDupeOnVaultRemove";
            toolStripMenuItemDupeOnVaultRemove.Size = new Size(276, 22);
            toolStripMenuItemDupeOnVaultRemove.Text = "Dupe on vault remove";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(136, 128);
            label2.Name = "label2";
            label2.Size = new Size(146, 15);
            label2.TabIndex = 15;
            label2.Text = "Select a character to begin";
            // 
            // radioButtonClassic
            // 
            radioButtonClassic.AutoCheck = false;
            radioButtonClassic.AutoSize = true;
            radioButtonClassic.Location = new Point(8, 24);
            radioButtonClassic.Name = "radioButtonClassic";
            radioButtonClassic.Size = new Size(61, 19);
            radioButtonClassic.TabIndex = 16;
            radioButtonClassic.TabStop = true;
            radioButtonClassic.Text = "Classic";
            radioButtonClassic.UseVisualStyleBackColor = true;
            radioButtonClassic.Click += radioButtonClassic_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(radioButtonExpansion);
            groupBox1.Controls.Add(radioButtonClassic);
            groupBox1.Location = new Point(8, 32);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(144, 72);
            groupBox1.TabIndex = 17;
            groupBox1.TabStop = false;
            groupBox1.Text = "Game";
            // 
            // radioButtonExpansion
            // 
            radioButtonExpansion.AutoCheck = false;
            radioButtonExpansion.AutoSize = true;
            radioButtonExpansion.Checked = true;
            radioButtonExpansion.Location = new Point(8, 48);
            radioButtonExpansion.Name = "radioButtonExpansion";
            radioButtonExpansion.Size = new Size(127, 19);
            radioButtonExpansion.TabIndex = 17;
            radioButtonExpansion.TabStop = true;
            radioButtonExpansion.Text = "Lord of Destruction";
            radioButtonExpansion.UseVisualStyleBackColor = true;
            radioButtonExpansion.Click += radioButtonExpansion_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(radioButtonHardcore);
            groupBox2.Controls.Add(radioButtonNormal);
            groupBox2.Location = new Point(160, 32);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(104, 72);
            groupBox2.TabIndex = 18;
            groupBox2.TabStop = false;
            groupBox2.Text = "Mode";
            // 
            // radioButtonHardcore
            // 
            radioButtonHardcore.AutoCheck = false;
            radioButtonHardcore.AutoSize = true;
            radioButtonHardcore.Location = new Point(8, 48);
            radioButtonHardcore.Name = "radioButtonHardcore";
            radioButtonHardcore.Size = new Size(74, 19);
            radioButtonHardcore.TabIndex = 17;
            radioButtonHardcore.TabStop = true;
            radioButtonHardcore.Text = "Hardcore";
            radioButtonHardcore.UseVisualStyleBackColor = true;
            radioButtonHardcore.Click += radioButtonHardcore_Click;
            // 
            // radioButtonNormal
            // 
            radioButtonNormal.AutoCheck = false;
            radioButtonNormal.AutoSize = true;
            radioButtonNormal.Checked = true;
            radioButtonNormal.Location = new Point(8, 24);
            radioButtonNormal.Name = "radioButtonNormal";
            radioButtonNormal.Size = new Size(65, 19);
            radioButtonNormal.TabIndex = 16;
            radioButtonNormal.TabStop = true;
            radioButtonNormal.Text = "Normal";
            radioButtonNormal.UseVisualStyleBackColor = true;
            radioButtonNormal.Click += radioButtonNormal_Click;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(textBoxDirectory);
            groupBox3.Controls.Add(buttonRefresh);
            groupBox3.Controls.Add(buttonChangeDirectory);
            groupBox3.Location = new Point(272, 32);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(720, 72);
            groupBox3.TabIndex = 19;
            groupBox3.TabStop = false;
            groupBox3.Text = "Directory";
            // 
            // FormInventory
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1449, 719);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(pictureBoxLeft);
            Controls.Add(toolStrip1);
            Controls.Add(listBoxCharacters);
            Controls.Add(textBoxVaultFilter);
            Controls.Add(label2);
            Name = "FormInventory";
            StartPosition = FormStartPosition.CenterParent;
            Text = "D2D2RMule";
            FormClosing += FormInventory_FormClosing;
            Load += FormInventory_Load;
            ResizeEnd += FormInventory_ResizeEnd;
            ((System.ComponentModel.ISupportInitialize)pictureBoxLeft).EndInit();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBoxLeft;
        private ListBox listBoxCharacters;
        private TextBox textBoxDirectory;
        private Button buttonChangeDirectory;
        private Button buttonRefresh;
        private TextBox textBoxVaultFilter;
        private ToolStrip toolStrip1;
        private ToolStripDropDownButton toolStripDropDownButton1;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripDropDownButton toolStripDropDownButton2;
        private ToolStripMenuItem toolStripMenuItemOptionAllToVaultIncludesInventory;
        private ToolStripMenuItem toolStripMenuItemDupeOnVaultRemove;
        private Label label2;
        private RadioButton radioButtonClassic;
        private GroupBox groupBox1;
        private RadioButton radioButtonExpansion;
        private GroupBox groupBox2;
        private RadioButton radioButtonHardcore;
        private RadioButton radioButtonNormal;
        private GroupBox groupBox3;
    }
}