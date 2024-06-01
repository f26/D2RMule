using D2RMuleLib;

namespace D2RMuleGUI
{
    public partial class FormMain : Form
    {
        List<D2SFile> d2sFiles = new List<D2SFile>();
        UInt32 countUniques = 0;
        UInt32 countRunewords = 0;
        UInt32 countRares = 0;
        UInt32 countCharms = 0;
        UInt32 countRunes = 0;
        UInt32 countPotions = 0;
        UInt32 countMisc = 0;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            ResizeControls();
            ReloadFiles();
        }

        private void ReloadFiles()
        {
            string folderPath = Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory) + @"\D2RMule\_\saves\";
            string targetExtension = ".d2s";
            string[] filenames = Directory.GetFiles(folderPath, $"*{targetExtension}");
            d2sFiles.Clear();

            // Iterate over the file names
            int counter = 0;

            foreach (string file in filenames)
            {
                Console.WriteLine("##############################################################");
                Console.WriteLine("Opening " + Path.GetFileName(file));
                D2SFile d2SFile = new D2SFile(file);
                this.d2sFiles.Add(d2SFile);
            }

            RefreshItemList();

            this.labelTotalCharacters.Text = "Total characters: " + filenames.Length.ToString();
            this.labelTotalItems.Text = "Total items: " + this.fastObjectListView1.GetItemCount().ToString();
        }

        private void RefreshItemList()
        {
            this.countCharms = this.countRares = this.countRunes = this.countRunewords = this.countUniques = this.countPotions = 0;
            this.fastObjectListView1.SuspendLayout();
            this.fastObjectListView1.Objects = null;
            this.stagedItems.Clear();
            foreach (D2SFile f in this.d2sFiles)
            {
                foreach (Item i in f.playerItems.items)
                {
                    StageItem(i);
                }
                foreach (Item i in f.corpseItems.items)
                {
                    StageItem(i);
                }
                foreach (Item i in f.mercItems.items)
                {
                    StageItem(i);
                }
                foreach (Item i in f.golemItem.items)
                {
                    StageItem(i);
                }
            }

            ApplyStagedItems();

            this.fastObjectListView1.ResumeLayout();
        }

        List<Item> stagedItems = new List<Item>();

        private void StageItem(Item i)
        {
            // Alwyays increment counters
            IncrementCounters(i);

            // Only add the item if it's checkbox is checked
            bool showItem = false;
            showItem |= (this.checkBoxShowCharms.Checked && i.isCharm());
            showItem |= (this.checkBoxShowRares.Checked && i.Quality == Quality.Rare);
            showItem |= (this.checkBoxShowRunes.Checked && i.isRune());
            showItem |= (this.checkBoxShowRunewords.Checked && i.IsRuneword);
            showItem |= (this.checkBoxShowUniques.Checked && i.Quality == Quality.Unique);
            showItem |= (this.checkBoxShowPotions.Checked && i.isPotion());

            if (this.checkBoxShowAll.Checked) showItem = true;
            if (showItem) this.stagedItems.Add(i);

        }

        private void ApplyStagedItems()
        {
            this.fastObjectListView1.AddObjects(stagedItems);
        }

        private void IncrementCounters(Item i)
        {
            if (i.Quality != null)
            {
                if (i.Quality.ToString().Contains("Unique"))
                    this.countUniques++;
                if (i.Quality.ToString().Contains("Rare"))
                    this.countRares++;
            }
            if (i.isCharm())
                this.countCharms++;
            if (i.isRune())
                this.countRunes++;
            if (i.IsRuneword)
                this.countRunewords++;
            if (i.isPotion())
                this.countPotions++;

            this.checkBoxShowCharms.Text = "Charms: " + this.countCharms.ToString();
            this.checkBoxShowRares.Text = "Rares: " + this.countRares.ToString();
            this.checkBoxShowRunes.Text = "Runes: " + this.countRunes.ToString();
            this.checkBoxShowRunewords.Text = "Runewords: " + this.countRunewords.ToString();
            this.checkBoxShowUniques.Text = "Uniques: " + this.countUniques.ToString();
            this.checkBoxShowPotions.Text = "Potions: " + this.countPotions.ToString();
        }

        private void checkBoxShowItem_CheckChanged(object sender, EventArgs e)
        {
            RefreshItemList();
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            ResizeControls();
        }

        private void ResizeControls()
        {
            this.fastObjectListView1.Width = this.ClientSize.Width - this.fastObjectListView1.Left * 2;
            this.fastObjectListView1.Height = this.ClientSize.Height - this.fastObjectListView1.Top - this.fastObjectListView1.Left;


        }

        private void fastObjectListView1_CellToolTipShowing(object sender, BrightIdeasSoftware.ToolTipShowingEventArgs e)
        {
            Item i = (Item)e.Item.RowObject;
            e.Text = i.rawBytes.Length.ToString() + " bytes";
        }
    }
}