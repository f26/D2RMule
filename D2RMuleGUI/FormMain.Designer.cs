namespace D2RMuleGUI
{
    partial class FormMain
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
            fastObjectListView1 = new BrightIdeasSoftware.FastObjectListView();
            columnHeaderCharacterName = new BrightIdeasSoftware.OLVColumn();
            columnHeaderLocation = new BrightIdeasSoftware.OLVColumn();
            columnHeaderDisplayName = new BrightIdeasSoftware.OLVColumn();
            columnHeaderType = new BrightIdeasSoftware.OLVColumn();
            columnHeaderLevel = new BrightIdeasSoftware.OLVColumn();
            columnHeaderQuality = new BrightIdeasSoftware.OLVColumn();
            columnHeaderSockets = new BrightIdeasSoftware.OLVColumn();
            columnHeaderSocketedItems = new BrightIdeasSoftware.OLVColumn();
            columnHeaderTooltip = new BrightIdeasSoftware.OLVColumn();
            groupBox1 = new GroupBox();
            checkBoxShowPotions = new CheckBox();
            checkBoxShowAll = new CheckBox();
            checkBoxShowRunes = new CheckBox();
            checkBoxShowCharms = new CheckBox();
            checkBoxShowRares = new CheckBox();
            checkBoxShowRunewords = new CheckBox();
            checkBoxShowUniques = new CheckBox();
            labelTotalItems = new Label();
            labelTotalCharacters = new Label();
            ((System.ComponentModel.ISupportInitialize)fastObjectListView1).BeginInit();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // fastObjectListView1
            // 
            fastObjectListView1.CellEditUseWholeCell = false;
            fastObjectListView1.Columns.AddRange(new ColumnHeader[] { columnHeaderCharacterName, columnHeaderLocation, columnHeaderDisplayName, columnHeaderType, columnHeaderLevel, columnHeaderQuality, columnHeaderSockets, columnHeaderSocketedItems, columnHeaderTooltip });
            fastObjectListView1.FullRowSelect = true;
            fastObjectListView1.GridLines = true;
            fastObjectListView1.Location = new Point(8, 152);
            fastObjectListView1.Name = "fastObjectListView1";
            fastObjectListView1.ShowGroups = false;
            fastObjectListView1.Size = new Size(1560, 530);
            fastObjectListView1.TabIndex = 0;
            fastObjectListView1.View = View.Details;
            fastObjectListView1.VirtualMode = true;
            fastObjectListView1.CellToolTipShowing += fastObjectListView1_CellToolTipShowing;
            // 
            // columnHeaderCharacterName
            // 
            columnHeaderCharacterName.AspectName = "CharacterName";
            columnHeaderCharacterName.Text = "Character";
            columnHeaderCharacterName.TextAlign = HorizontalAlignment.Center;
            columnHeaderCharacterName.Width = 125;
            // 
            // columnHeaderLocation
            // 
            columnHeaderLocation.AspectName = "Location";
            columnHeaderLocation.Text = "Location";
            columnHeaderLocation.TextAlign = HorizontalAlignment.Center;
            columnHeaderLocation.Width = 100;
            // 
            // columnHeaderDisplayName
            // 
            columnHeaderDisplayName.AspectName = "DisplayName";
            columnHeaderDisplayName.Text = "Specialized Name";
            columnHeaderDisplayName.TextAlign = HorizontalAlignment.Center;
            columnHeaderDisplayName.Width = 250;
            // 
            // columnHeaderType
            // 
            columnHeaderType.AspectName = "Type";
            columnHeaderType.Text = "Base";
            columnHeaderType.TextAlign = HorizontalAlignment.Center;
            columnHeaderType.Width = 150;
            // 
            // columnHeaderLevel
            // 
            columnHeaderLevel.AspectName = "Level";
            columnHeaderLevel.Text = "iLevel";
            columnHeaderLevel.TextAlign = HorizontalAlignment.Center;
            // 
            // columnHeaderQuality
            // 
            columnHeaderQuality.AspectName = "Quality";
            columnHeaderQuality.Text = "Quality";
            columnHeaderQuality.TextAlign = HorizontalAlignment.Center;
            columnHeaderQuality.Width = 100;
            // 
            // columnHeaderSockets
            // 
            columnHeaderSockets.AspectName = "Sockets";
            columnHeaderSockets.Text = "Sockets";
            columnHeaderSockets.TextAlign = HorizontalAlignment.Center;
            columnHeaderSockets.Width = 75;
            // 
            // columnHeaderSocketedItems
            // 
            columnHeaderSocketedItems.AspectName = "SocketedItems";
            columnHeaderSocketedItems.Text = "Socketed Items";
            columnHeaderSocketedItems.TextAlign = HorizontalAlignment.Center;
            columnHeaderSocketedItems.Width = 200;
            // 
            // columnHeaderTooltip
            // 
            columnHeaderTooltip.AspectName = "Mods";
            columnHeaderTooltip.FillsFreeSpace = true;
            columnHeaderTooltip.Text = "Mods";
            columnHeaderTooltip.Width = 400;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(checkBoxShowPotions);
            groupBox1.Controls.Add(checkBoxShowAll);
            groupBox1.Controls.Add(checkBoxShowRunes);
            groupBox1.Controls.Add(checkBoxShowCharms);
            groupBox1.Controls.Add(checkBoxShowRares);
            groupBox1.Controls.Add(checkBoxShowRunewords);
            groupBox1.Controls.Add(checkBoxShowUniques);
            groupBox1.Controls.Add(labelTotalItems);
            groupBox1.Controls.Add(labelTotalCharacters);
            groupBox1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            groupBox1.Location = new Point(8, 8);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(984, 136);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Summary";
            // 
            // checkBoxShowPotions
            // 
            checkBoxShowPotions.AutoSize = true;
            checkBoxShowPotions.Checked = true;
            checkBoxShowPotions.CheckState = CheckState.Checked;
            checkBoxShowPotions.Location = new Point(480, 40);
            checkBoxShowPotions.Name = "checkBoxShowPotions";
            checkBoxShowPotions.Size = new Size(111, 25);
            checkBoxShowPotions.TabIndex = 18;
            checkBoxShowPotions.Text = "Potions: ----";
            checkBoxShowPotions.UseVisualStyleBackColor = true;
            checkBoxShowPotions.CheckedChanged += checkBoxShowItem_CheckChanged;
            // 
            // checkBoxShowAll
            // 
            checkBoxShowAll.AutoSize = true;
            checkBoxShowAll.Checked = true;
            checkBoxShowAll.CheckState = CheckState.Checked;
            checkBoxShowAll.Location = new Point(192, 40);
            checkBoxShowAll.Name = "checkBoxShowAll";
            checkBoxShowAll.Size = new Size(47, 25);
            checkBoxShowAll.TabIndex = 17;
            checkBoxShowAll.Text = "All";
            checkBoxShowAll.UseVisualStyleBackColor = true;
            checkBoxShowAll.CheckedChanged += checkBoxShowItem_CheckChanged;
            // 
            // checkBoxShowRunes
            // 
            checkBoxShowRunes.AutoSize = true;
            checkBoxShowRunes.Checked = true;
            checkBoxShowRunes.CheckState = CheckState.Checked;
            checkBoxShowRunes.Location = new Point(344, 104);
            checkBoxShowRunes.Name = "checkBoxShowRunes";
            checkBoxShowRunes.Size = new Size(103, 25);
            checkBoxShowRunes.TabIndex = 16;
            checkBoxShowRunes.Text = "Runes: ----";
            checkBoxShowRunes.UseVisualStyleBackColor = true;
            checkBoxShowRunes.CheckedChanged += checkBoxShowItem_CheckChanged;
            // 
            // checkBoxShowCharms
            // 
            checkBoxShowCharms.AutoSize = true;
            checkBoxShowCharms.Checked = true;
            checkBoxShowCharms.CheckState = CheckState.Checked;
            checkBoxShowCharms.Location = new Point(344, 72);
            checkBoxShowCharms.Name = "checkBoxShowCharms";
            checkBoxShowCharms.Size = new Size(114, 25);
            checkBoxShowCharms.TabIndex = 15;
            checkBoxShowCharms.Text = "Charms: ----";
            checkBoxShowCharms.UseVisualStyleBackColor = true;
            checkBoxShowCharms.CheckedChanged += checkBoxShowItem_CheckChanged;
            // 
            // checkBoxShowRares
            // 
            checkBoxShowRares.AutoSize = true;
            checkBoxShowRares.Checked = true;
            checkBoxShowRares.CheckState = CheckState.Checked;
            checkBoxShowRares.Location = new Point(344, 40);
            checkBoxShowRares.Name = "checkBoxShowRares";
            checkBoxShowRares.Size = new Size(99, 25);
            checkBoxShowRares.TabIndex = 14;
            checkBoxShowRares.Text = "Rares: ----";
            checkBoxShowRares.UseVisualStyleBackColor = true;
            checkBoxShowRares.CheckedChanged += checkBoxShowItem_CheckChanged;
            // 
            // checkBoxShowRunewords
            // 
            checkBoxShowRunewords.AutoSize = true;
            checkBoxShowRunewords.Checked = true;
            checkBoxShowRunewords.CheckState = CheckState.Checked;
            checkBoxShowRunewords.Location = new Point(192, 104);
            checkBoxShowRunewords.Name = "checkBoxShowRunewords";
            checkBoxShowRunewords.Size = new Size(139, 25);
            checkBoxShowRunewords.TabIndex = 13;
            checkBoxShowRunewords.Text = "Runewords: ----";
            checkBoxShowRunewords.UseVisualStyleBackColor = true;
            checkBoxShowRunewords.CheckedChanged += checkBoxShowItem_CheckChanged;
            // 
            // checkBoxShowUniques
            // 
            checkBoxShowUniques.AutoSize = true;
            checkBoxShowUniques.Checked = true;
            checkBoxShowUniques.CheckState = CheckState.Checked;
            checkBoxShowUniques.Location = new Point(192, 72);
            checkBoxShowUniques.Name = "checkBoxShowUniques";
            checkBoxShowUniques.Size = new Size(117, 25);
            checkBoxShowUniques.TabIndex = 12;
            checkBoxShowUniques.Text = "Uniques: ----";
            checkBoxShowUniques.UseVisualStyleBackColor = true;
            checkBoxShowUniques.CheckedChanged += checkBoxShowItem_CheckChanged;
            // 
            // labelTotalItems
            // 
            labelTotalItems.Location = new Point(16, 72);
            labelTotalItems.Name = "labelTotalItems";
            labelTotalItems.Size = new Size(152, 24);
            labelTotalItems.TabIndex = 3;
            labelTotalItems.Text = "Total items:";
            labelTotalItems.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelTotalCharacters
            // 
            labelTotalCharacters.Location = new Point(16, 40);
            labelTotalCharacters.Name = "labelTotalCharacters";
            labelTotalCharacters.Size = new Size(152, 24);
            labelTotalCharacters.TabIndex = 1;
            labelTotalCharacters.Text = "Total characters:";
            labelTotalCharacters.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1782, 766);
            Controls.Add(groupBox1);
            Controls.Add(fastObjectListView1);
            Name = "FormMain";
            Text = "Form1";
            Load += FormMain_Load;
            Resize += FormMain_Resize;
            ((System.ComponentModel.ISupportInitialize)fastObjectListView1).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private BrightIdeasSoftware.FastObjectListView fastObjectListView1;
        private BrightIdeasSoftware.OLVColumn columnHeaderCharacterName;
        private BrightIdeasSoftware.OLVColumn columnHeaderLocation;
        private BrightIdeasSoftware.OLVColumn columnHeaderDisplayName;
        private BrightIdeasSoftware.OLVColumn columnHeaderType;
        private BrightIdeasSoftware.OLVColumn columnHeaderLevel;
        private BrightIdeasSoftware.OLVColumn columnHeaderQuality;
        private BrightIdeasSoftware.OLVColumn columnHeaderSockets;
        private BrightIdeasSoftware.OLVColumn columnHeaderSocketedItems;
        private BrightIdeasSoftware.OLVColumn columnHeaderTooltip;
        private GroupBox groupBox1;
        private Label labelTotalCharacters;
        private Label label1;
        private Label label2;
        private Label labelTotalItems;
        private CheckBox checkBoxShowUniques;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label6;
        private Label label5;
        private Label label4;
        private Label label3;
        private CheckBox checkBox4;
        private CheckBox checkBox3;
        private CheckBox checkBoxShowRunewords;
        private CheckBox checkBoxShowCharms;
        private CheckBox checkBoxShowRares;
        private CheckBox checkBoxShowRunes;
        private CheckBox checkBoxShowAll;
        private CheckBox checkBoxShowPotions;
    }
}
