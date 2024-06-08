using BrightIdeasSoftware;
using Microsoft.VisualBasic;
using D2RMuleLib;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Formats.Asn1;
using System.Media;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Windows.Forms;
using System.Text.Json;

namespace D2RMuleGUI
{

    enum DisplaySection
    {
        None,
        Stash,
        Inventory,
        Mercenary,
        Cube,
        Vault
    }

    enum Hand
    {
        Main,
        Alt
    }

    enum EquippedSlot
    {
        Helm = 1,
        Amulet = 2,
        Armor = 3,
        LHand = 4,
        RHand = 5,
        LRing = 6,
        RRing = 7,
        Belt = 8,
        Boots = 9,
        Gloves = 10,
        LHandAlt = 11,
        RHandAlt = 12
    }

    public partial class FormInventory : Form
    {
        double ratio = 1.0;
        const int MARGIN = 8;

        Size STASH_SIZE = new Size(10, 10);
        Size CLASSIC_STASH_SIZE = new Size(6, 4);

        // All these rects are in pixels and relate to the background images.  If the background images are
        // modified, these values will have to be recalculated.  Note that the positions are all in pixels
        // and are in SOURCE IMAGE pixels.  If the picture boxe is not the exact same size as the source
        // images, the images will be scaled automatically to fit the pictureboxes, but these values still
        // need to be used to draw items on each page, as the original, full-sized images are the ones
        // loaded into memory and used to composite the displays.

        const int GRID_SIZE_PX = 98; // Size of each grid space in the stash/inventory/cube, includes border

        Rectangle leftHalf = new Rectangle(0, 0, 1162, 1507);
        Rectangle rightHalf = new Rectangle(1162, 0, 1162, 1507);

        Rectangle stashItemGrid = new Rectangle(90, 236, 10 * GRID_SIZE_PX, 10 * GRID_SIZE_PX);
        Rectangle stashClassicItemGrid = new Rectangle(288, 750, 6 * GRID_SIZE_PX, 4 * GRID_SIZE_PX);
        Rectangle stashSelectButton = new Rectangle(641, 59, 463, 72);
        Rectangle stashIngestAll = new Rectangle(487, 1399, 186, 38);

        Rectangle vaultItemGrid = new Rectangle(90, 236, 22 * GRID_SIZE_PX, 10 * GRID_SIZE_PX);
        Rectangle vaultSelectButton = new Rectangle(59, 59, 463, 72);
        Rectangle vaultLeftButton = new Rectangle(1243, 43, 80, 85);
        Rectangle vaultRightButton = new Rectangle(2137, 41, 80, 85);
        Rectangle vaultCenterText = new Rectangle(1356, 46, 759, 69);
        Rectangle vaultLeftText = new Rectangle(1356, 46, 150, 69);
        Rectangle vaultRightText = new Rectangle(2014, 46, 100, 69);
        Rectangle vaultFilterBox = new Rectangle(150, 170, 929, 66);

        Rectangle vaultFilterOptions = new Rectangle(72, 1248, 2193, 200);
        Rectangle vaultFilterAmulet = new Rectangle(521, 1302, 40, 38);
        Rectangle vaultFilterArmor = new Rectangle(521, 1347, 40, 38);
        Rectangle vaultFilterBelt = new Rectangle(521, 1392, 40, 38);
        Rectangle vaultFilterBoots = new Rectangle(719, 1302, 40, 38);
        Rectangle vaultFilterGloves = new Rectangle(719, 1347, 40, 38);
        Rectangle vaultFilterHelm = new Rectangle(719, 1392, 40, 38);
        Rectangle vaultFilterRing = new Rectangle(917, 1302, 40, 38);
        Rectangle vaultFilterShield = new Rectangle(917, 1347, 40, 38);
        Rectangle vaultFilterWeapon = new Rectangle(917, 1392, 40, 38);
        Rectangle vaultFilterRune = new Rectangle(1165, 1302, 40, 38);
        Rectangle vaultFilterCharm = new Rectangle(1165, 1347, 40, 38);
        Rectangle vaultFilterGem = new Rectangle(1165, 1392, 40, 38);
        Rectangle vaultFilterJewel = new Rectangle(1331, 1302, 40, 38);
        Rectangle vaultFilterQuest = new Rectangle(1331, 1347, 40, 38);
        Rectangle vaultFilterNormal = new Rectangle(1544, 1302, 40, 38);
        Rectangle vaultFilterMagic = new Rectangle(1544, 1347, 40, 38);
        Rectangle vaultFilterRare = new Rectangle(1544, 1392, 40, 38);
        Rectangle vaultFilterSet = new Rectangle(1759, 1302, 40, 38);
        Rectangle vaultFilterCrafted = new Rectangle(1759, 1347, 40, 38);
        Rectangle vaultFilterUnique = new Rectangle(1759, 1392, 40, 38);

        Rectangle vaultFilterRuneword = new Rectangle(1988, 1302, 40, 38);
        Rectangle vaultFilterSocketed = new Rectangle(1988, 1347, 40, 38);
        Rectangle vaultFilterEthereal = new Rectangle(1988, 1392, 40, 38);

        Rectangle inventoryButton = new Rectangle(1529, 51, 426, 81);
        Rectangle inventoryItemGrid = new Rectangle(1253, 898, 980, 392);
        Rectangle inventoryLeftHand = new Rectangle(1270, 232, 2 * GRID_SIZE_PX, 4 * GRID_SIZE_PX);
        Rectangle inventoryRightHand = new Rectangle(2022, 232, 2 * GRID_SIZE_PX, 4 * GRID_SIZE_PX);
        Rectangle inventoryAmulet = new Rectangle(1881, 353, GRID_SIZE_PX, GRID_SIZE_PX);
        Rectangle inventoryHelm = new Rectangle(1644, 185, 2 * GRID_SIZE_PX, 2 * GRID_SIZE_PX);
        Rectangle inventoryArmor = new Rectangle(1644, 428, 2 * GRID_SIZE_PX, 3 * GRID_SIZE_PX);
        Rectangle inventoryBelt = new Rectangle(1644, 769, 2 * GRID_SIZE_PX, GRID_SIZE_PX);
        Rectangle inventoryGloves = new Rectangle(1269, 671, 2 * GRID_SIZE_PX, 2 * GRID_SIZE_PX);
        Rectangle inventoryBoots = new Rectangle(2022, 671, 2 * GRID_SIZE_PX, 2 * GRID_SIZE_PX);
        Rectangle inventoryLeftRing = new Rectangle(1507, 769, GRID_SIZE_PX, GRID_SIZE_PX);
        Rectangle inventoryRightRing = new Rectangle(1881, 769, GRID_SIZE_PX, GRID_SIZE_PX);
        Rectangle inventorySwapHandsL = new Rectangle(1260, 177, 217, 45);
        Rectangle inventorySwapHandsR = new Rectangle(1260, 177, 217, 45);

        Rectangle cubeButton = new Rectangle(1999, 52, 273, 80);
        Rectangle cubeItemGrid = new Rectangle(1593, 537, 3 * GRID_SIZE_PX, 4 * GRID_SIZE_PX);

        Rectangle mercButton = new Rectangle(1214, 52, 271, 80);
        Rectangle mercHelm = new Rectangle(1643, 190, 2 * GRID_SIZE_PX, 2 * GRID_SIZE_PX);
        Rectangle mercArmor = new Rectangle(1643, 432, 2 * GRID_SIZE_PX, 3 * GRID_SIZE_PX);
        Rectangle mercLeftHand = new Rectangle(1270, 337, 2 * GRID_SIZE_PX, 4 * GRID_SIZE_PX);
        Rectangle mercRightHand = new Rectangle(2022, 337, 2 * GRID_SIZE_PX, 4 * GRID_SIZE_PX);

        // The lists of rectangles that contain all the tooltips for what is currently displayed on each panel
        List<ItemRect> leftPanelRects = new List<ItemRect>();
        List<ItemRect> rightPanelRects = new List<ItemRect>();

        // The various images
        Image stashOriginalImage;
        Image classicStashOriginalImage;
        Image vaultOriginalImage;
        Image inventoryOriginalImage;
        Image inventoryOriginalAltImage;
        Image classicInventoryOriginalImage;
        Image mercOriginalImage;
        Image vaultFilterCheckedImage;
        Image cubeOriginalImage;
        Image compositedImage;

        // Vault display-related trackers
        UInt32 vaultCurrentDisplayIndex = 0;
        UInt32 vaultTotalFilteredItems = 0;
        UInt32 vaultItemsOnCurrentPage = 0;
        Stack<uint> vaultDisplayIndexStack = new Stack<uint>();

        Dictionary<string, D2SFile> characterFiles = new Dictionary<string, D2SFile>();
        string currentlyDisplayed = "";

        DisplaySection leftPanelDisplay = DisplaySection.Stash;
        DisplaySection rightPanelDisplay = DisplaySection.Inventory;
        Hand displayedHand = Hand.Main;

        ToolTip tt = new ToolTip();

        Items vaultItems = new Items("__VAULT__");

        Brush ON_CURSOR_BRUSH = new SolidBrush(Color.FromArgb(64, 255, 165, 0));

        Item onCursor = new Item();
        DisplaySection itemPickedFrom = DisplaySection.None;

        public FormInventory()
        {
            InitializeComponent();
            tt.AutoPopDelay = 60000; // Set how long the tooltip will be displayed
            tt.InitialDelay = 200;   // Set initial delay before showing the tooltip
            tt.ReshowDelay = 200;    // Set the delay before showing the tooltip again if mouse stops
        }



        void LoadConfiguration()
        {
            D2RMuleSettings settings = D2RMuleSettings.Load();

            this.radioButtonClassic.Checked = false;
            this.radioButtonExpansion.Checked = true;

            if (settings.Game == D2RMuleSettings.GameType.Classic)
            {
                this.radioButtonClassic.Checked = true;
                this.radioButtonExpansion.Checked = false;
            }

            this.radioButtonNormal.Checked = true;
            this.radioButtonHardcore.Checked = false;
            if (settings.Mode == D2RMuleSettings.ModeType.Hardcore)
            {
                this.radioButtonNormal.Checked = false;
                this.radioButtonHardcore.Checked = true;
            }

            this.textBoxDirectory.Text = settings.SaveDirectory;
        }

        void SaveConfiguration()
        {
            D2RMuleSettings settings = new D2RMuleSettings();

            settings.Game = D2RMuleSettings.GameType.LordOfDestruction;
            if (this.radioButtonClassic.Checked)
                settings.Game = D2RMuleSettings.GameType.Classic;

            settings.Mode = D2RMuleSettings.ModeType.Normal;
            if (this.radioButtonHardcore.Checked)
                settings.Mode = D2RMuleSettings.ModeType.Hardcore;

            settings.SaveDirectory = this.textBoxDirectory.Text;

            settings.Save();
        }

        private void FormInventory_Load(object sender, EventArgs e)
        {
            this.Top = 100;
            this.Left = 100;

            LoadConfiguration();

            //this.textBoxDirectory.Text = Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory) + @"\D2RMule\_\saves\";

            // The base, clean backgrounds
            this.stashOriginalImage = Image.FromFile("images/stash.png");
            this.vaultOriginalImage = Image.FromFile("images/vault.png");
            this.inventoryOriginalImage = Image.FromFile("images/inventory_main.png");
            this.inventoryOriginalAltImage = Image.FromFile("images/inventory_alt.png");
            this.mercOriginalImage = Image.FromFile("images/merc.png");
            this.cubeOriginalImage = Image.FromFile("images/cube.png");
            this.vaultFilterCheckedImage = Image.FromFile("images/checked.png");
            this.classicInventoryOriginalImage = Image.FromFile("images/classic_inventory.png");
            this.classicStashOriginalImage = Image.FromFile("images/classic_stash.png");

            InitializeGUI();

            try
            {
                LoadVault();
            }
            catch (Exception ex)
            {
                MessageBox.Show("error reading in items");
            }

            // Resize the vault filter box to the right location (done here instead of on refresh because it
            // does not need to be done every time)
            PlaceVaultFilterBox();
        }

        private void PlaceVaultFilterBox()
        {
            int x = (int)(this.vaultFilterBox.Left * this.ratio) + this.pictureBoxLeft.Left;
            int y = (int)(this.vaultFilterBox.Top * this.ratio) + this.pictureBoxLeft.Top;
            int width = (int)(this.vaultFilterBox.Width * this.ratio);
            int height = (int)(this.vaultFilterBox.Height * this.ratio);
            this.textBoxVaultFilter.Location = new Point(x, y);
            this.textBoxVaultFilter.Size = new Size(width, height);
        }

        private void FormInventory_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                SaveVault();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error has been encountered while attempting to save the vault.");
                e.Cancel = true;
            }
            SaveModifiedFiles();
            SaveConfiguration();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.pictureBoxLeft.Enabled = true;
                this.pictureBoxLeft.Visible = true;
                this.textBoxVaultFilter.Visible = true;

                string folderPath = this.textBoxDirectory.Text;
                string fullPath = folderPath + this.listBoxCharacters.SelectedItem;
                this.currentlyDisplayed = fullPath;

                if (!this.characterFiles.ContainsKey(fullPath))
                {
                    D2SFile d2SFile = new D2SFile(currentlyDisplayed);
                    this.characterFiles.Add(fullPath, d2SFile);
                }

                this.currentlyDisplayed = fullPath;

                // Classic does not have equippable mercs, so don't allow view of merc
                if (!this.characterFiles[currentlyDisplayed].isExpansion &&
                    this.rightPanelDisplay == DisplaySection.Mercenary)
                {
                    this.rightPanelDisplay = DisplaySection.Inventory;
                }

                RefreshDisplayedItems();
            }
            catch (System.IO.FileNotFoundException ex)
            {
                // No character selected
                this.pictureBoxLeft.Enabled = false;
                this.pictureBoxLeft.Visible = false;
                this.textBoxVaultFilter.Visible = false;
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            InitializeGUI();
        }


        private void pictureBoxLeft_Click(object sender, EventArgs e)
        {
            var mouseEventArgs = e as MouseEventArgs;
            if (mouseEventArgs == null)
                return;

            // Only left/right mouse buttons are supported
            if (mouseEventArgs.Button != MouseButtons.Left && mouseEventArgs.Button != MouseButtons.Right)
                return;

            Point location = mouseEventArgs.Location;
            bool refreshNeeded = false;

            // NOTE: Having an item on the cursor prevents switching to the vault.  It is handled first and
            // nothing else is handled if an item is on the cursor.

            // Is an item being moved?
            if (itemPickedFrom != DisplaySection.None)
            {
                if (this.leftHalf.ContainsPoint(location))
                    refreshNeeded = HandleLeftSideClickWithItemOnCursor(mouseEventArgs);
                else
                    refreshNeeded = HandleRightSideClickWithItemOnCursor(mouseEventArgs);
            }
            // Switching to stash?
            else if (this.leftPanelDisplay != DisplaySection.Stash && this.stashSelectButton.ContainsPoint(location))
            {
                this.leftPanelDisplay = DisplaySection.Stash;
                this.rightPanelDisplay = DisplaySection.Inventory;
                PlayButtonSound();
                refreshNeeded = true;
            }
            // Switching to vault?
            else if (this.leftPanelDisplay != DisplaySection.Vault && this.vaultSelectButton.ContainsPoint(location))
            {
                // If we are attempting to switch to the vault with an item on the cursor, prevent it
                if (itemPickedFrom != DisplaySection.None)
                {
                    MessageBox.Show("Cannot switch to the vault when moving an item.  Finish the move first.");
                    return;
                }

                this.leftPanelDisplay = DisplaySection.Vault;
                this.rightPanelDisplay = DisplaySection.Vault;
                this.vaultCurrentDisplayIndex = 0;
                PlayButtonSound();
                refreshNeeded = true;
            }
            // Switching to inventory?
            else if (this.rightPanelDisplay != DisplaySection.Vault &&
                     this.rightPanelDisplay != DisplaySection.Inventory &&
                     this.inventoryButton.ContainsPoint(mouseEventArgs.Location))
            {
                this.rightPanelDisplay = DisplaySection.Inventory;
                PlayButtonSound();
                refreshNeeded = true;
            }
            // Switching to merc?
            else if (this.rightPanelDisplay != DisplaySection.Vault &&
                     this.rightPanelDisplay != DisplaySection.Mercenary &&
                     this.mercButton.ContainsPoint(mouseEventArgs.Location))
            {
                // Classic does not allow merc equip, so don't swap to it.
                if (this.characterFiles[currentlyDisplayed].isExpansion)
                {
                    this.rightPanelDisplay = DisplaySection.Mercenary;
                    PlayButtonSound();
                    refreshNeeded = true;
                }
                else
                {
                    PlayFailSound();
                    refreshNeeded = false;
                }

            }
            // Switching to cube?
            else if (this.rightPanelDisplay != DisplaySection.Vault &&
                     this.rightPanelDisplay != DisplaySection.Cube &&
                     this.cubeButton.ContainsPoint(mouseEventArgs.Location))
            {
                this.rightPanelDisplay = DisplaySection.Cube;
                PlayButtonSound();
                refreshNeeded = true;
            }
            // Was this click in the stash?
            else if (this.leftPanelDisplay == DisplaySection.Stash &&
                     this.leftHalf.ContainsPoint(mouseEventArgs.Location))
            {
                if (this.stashIngestAll.ContainsPoint(mouseEventArgs.Location))
                    refreshNeeded = HandleStashIngestButtonClick();
                else
                    refreshNeeded = HandlePossibleItemClickWithNoItemOnCursor(mouseEventArgs);
            }
            // Was this click in the vault?
            else if (this.leftPanelDisplay == DisplaySection.Vault)
            {
                if (this.vaultLeftButton.ContainsPoint(mouseEventArgs.Location))
                    refreshNeeded = HandleVaultLeftButton();
                else if (this.vaultRightButton.ContainsPoint(mouseEventArgs.Location))
                    refreshNeeded = HandleVaultRightButton();
                else if (this.vaultItemGrid.ContainsPoint(mouseEventArgs.Location))
                    refreshNeeded = HandleVaultItemClick(mouseEventArgs);
                else if (this.vaultFilterOptions.ContainsPoint(mouseEventArgs.Location))
                {
                    refreshNeeded = HandleVaultFilterClick(mouseEventArgs);
                    if (refreshNeeded)
                    {
                        this.vaultCurrentDisplayIndex = 0;
                        this.vaultDisplayIndexStack.Clear();
                        PlayButtonSound();
                    }
                }
            }
            // Was this click on the right side?
            else if (this.rightHalf.ContainsPoint(mouseEventArgs.Location))
            {
                if (this.rightPanelDisplay == DisplaySection.Inventory &&
                    (this.inventorySwapHandsL.ContainsPoint(mouseEventArgs.Location) ||
                     this.inventorySwapHandsR.ContainsPoint(mouseEventArgs.Location)))

                {
                    if (this.displayedHand == Hand.Main)
                        this.displayedHand = Hand.Alt;
                    else
                        this.displayedHand = Hand.Main;
                    PlayButtonSound();
                    refreshNeeded = true;
                }
                else
                {
                    refreshNeeded = HandlePossibleItemClickWithNoItemOnCursor(mouseEventArgs);
                }
            }

            if (refreshNeeded)
                RefreshDisplayedItems();
        }


        Point prevMouseLoc = new Point();
        Item hoveredItem = new Item();
        bool tooltipEmpty = true;
        private void pictureBoxStash_MouseMove(object sender, MouseEventArgs e)
        {
            HandleMouseMove(this.pictureBoxLeft, this.leftPanelRects, e);
        }

        private Image GetImage(Item item)
        {
            string customCode = "";
            if (item.HasCustomGraphics)
                customCode = item.CustomGraphicsIndex.ToString();

            string typeCode = item.TypeCode;
            if ((item.Quality == Quality.Unique || item.Quality == Quality.Set) &&
                typeCode != "rin" &&
                typeCode != "amu" &&
                typeCode != "jew")
            {
                // Some unique items have custom graphics
                customCode = "";
                switch (typeCode)
                {
                    // Armor
                    case "cm1": typeCode = "mss"; break;
                    case "cm2": typeCode = "torch"; break;
                    case "xar": typeCode = "corpsemourn"; break; // Courpsemourn, Griswold's Heart
                    case "hbt": typeCode = "sabot"; break; // Sigon's Sabot
                    case "ful": typeCode = "goldskin"; break; // Goldskin
                    case "xrs": typeCode = "haemosus"; break; // Haemosu's Adamant
                    case "aar": typeCode = "silks"; break; // Silks of the Victor
                    case "xtu": typeCode = "ironpelt"; break; // Iron Pelt
                    case "xrn": typeCode = "crown"; break; // Crown of Thieves
                    case "fhl": typeCode = "duskdeep"; break; // Duskdeep
                    case "uhm": typeCode = "ondals"; break; // Ondal's Almighty
                    case "xkp": typeCode = "rockstopper"; break; // Rockstopper
                    case "cap": typeCode = "bonnet"; break; // Biggin's Bonnet
                    case "bhm": typeCode = "wormskull"; break; // Wormskull
                    case "tow": typeCode = "bverrit"; break; // Bverrit Keep
                    case "uts":
                        if (item.Quality == Quality.Unique) typeCode = "ward"; // The Ward
                        else typeCode = "taebaek";  // Taebaek's Glory
                        break;
                    case "xpk": typeCode = "lance"; break; // Lance Guard
                    case "xsh": typeCode = "lidless"; break; // Lidless Wall
                    case "xml": typeCode = "mosers"; break; // Moser's Blessed Circle
                    case "buc": typeCode = "pelta"; break; // Pelta Lunata
                    case "xrg": typeCode = "stormchaser"; break; // Stormchaser
                    case "kit": typeCode = "steelclash"; break; // Steelclash, Milabrega's Orb
                    case "lrg": typeCode = "stormguild"; break; // Stormguild
                    case "spk": typeCode = "swordback"; break; // Swordback Hold
                    case "spl": typeCode = "umbral"; break; // Umbral Disk
                    case "bsh": typeCode = "eyeless"; break; // Wall of the Eyeless

                    // Weapons
                    case "9s8": typeCode = "athena"; break; // Athena's Wrath
                    case "9gw": typeCode = "blackhand"; break; // Blackhand Key
                    case "bsw": typeCode = "blacktongue"; break; // Blacktounge
                    case "scm": typeCode = "bloodcrescent"; break; // Blood Crescent
                    case "mst": typeCode = "bloodrise"; break; // Blood Crescent
                    case "mau": typeCode = "bonesob"; break; // Bonesob
                    case "gax": typeCode = "brainhew"; break; // Brainhew
                    case "7ma": typeCode = "dangoon"; break; // Dangoon's Teaching
                    case "axe": typeCode = "deathspade"; break; // Deathspade
                    case "rxb": typeCode = "doomspittle"; break; // Doomspittle
                    case "clb": typeCode = "felloak"; break; // Felloak
                    case "9cr": typeCode = "ginthers"; break; // Ginther's Rift
                    case "flc": typeCode = "gleam"; break; // Gleamscythe
                    case "bwn": typeCode = "gravenspine"; break; // Gravenspine
                    case "bsd": typeCode = "griswold"; break; // Griswold's Edge
                    case "hxb": typeCode = "hellcast"; break; // Hellcast
                    case "swb": typeCode = "hellclap"; break; // Hellclap
                    case "hfh": typeCode = "hellforge"; break; // Hellforge Hammer
                    case "lsd": typeCode = "hellplague"; break; // Hellplague
                    case "9sb": typeCode = "hexfire"; break; // Hexfire
                    case "mxb": typeCode = "ichor"; break; // Ichorstring
                    case "wnd": typeCode = "iros"; break; // Iro's Torch
                    case "gis": typeCode = "kinemils"; break; // Kinemils Awl
                    case "sbr": typeCode = "krintizs"; break; // Krintizs Skewer
                    case "8lb": typeCode = "kuko"; break; // Krintizs Skewer
                    case "8lx": typeCode = "langer"; break; // Langer Briser
                    case "cst": typeCode = "lazarus"; break; // Lazarus Spire
                    case "6cf": typeCode = "lazarus"; break; // Ondal's Almighty
                    case "lxb": typeCode = "lead"; break; // Leadcrow
                    case "7cr": typeCode = "lightsabre"; break; // Lightsabre
                    case "mpi": typeCode = "mindrend"; break; // Mindrend
                    case "7qr": typeCode = "natssci"; break; // Natalya's Mark
                    case "cbw": typeCode = "piercerib"; break; // Piercerib
                    case "9ls": typeCode = "plague"; break; // Plague Bearer
                    case "sbb": typeCode = "pullspite"; break; // Pullspite
                    case "8mx": typeCode = "pus"; break; // Pus Spiter
                    case "tri": typeCode = "razortine"; break; // Razortine
                    case "2hs": typeCode = "shadowfang"; break; // Shadowfang
                    case "7ca": typeCode = "shadowkiller"; break; // Shadowkiller
                    case "scy": typeCode = "soul"; break; // Soul Harvest
                    case "9br": typeCode = "soulfeast"; break; // Soulfeast Tine
                    case "9bt": typeCode = "stormrider"; break; // Stormrider
                    case "9bl": typeCode = "stormspike"; break; // Stormspike
                    case "spc": typeCode = "stoutnail"; break; // Stoutnail
                    case "btx": typeCode = "chieftan"; break; // The Chieftan
                    case "9gm": typeCode = "gavel"; break; // The Gavel of Pain
                    case "hax": typeCode = "gnasher"; break; // The Gnasher
                    case "kri": typeCode = "jade"; break; // The Jade Tan Do
                    case "9gi": typeCode = "minataur"; break; // The Minataur
                    case "gsd": typeCode = "patriarch"; break; // The Patriarch
                    case "9fb": typeCode = "tode"; break; // Todesfaelle Flamme
                    case "7bk": typeCode = "shrike"; break; // Warshrike
                    case "8s8": typeCode = "which"; break; // Whichwild String
                }
            }

            string filename = typeCode + customCode + ".png";
            string imageName = @"images\items\" + filename;

            if (!File.Exists(imageName) && D2DB.Instance().itemDB.ContainsKey(typeCode))
            {
                // Look for alternate graphic for the item. THere are image files for all normal items but
                // not for their exceptional/unique equivalents, since they use the same images.
                filename = D2DB.Instance().itemDB[typeCode].AlernateGfx + customCode + ".png";
                imageName = @"images\items\" + filename;
            }
            Image img;
            try
            {
                img = Image.FromFile(imageName);
            }
            catch (Exception ex)
            {
                img = new Bitmap(GRID_SIZE_PX, GRID_SIZE_PX);
                using (Graphics gfx = Graphics.FromImage(img))
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(128, 255, 0, 0)))
                {
                    gfx.FillRectangle(brush, 0, 0, img.Width, img.Height);
                }
            }

            //img = ScaleImage((Bitmap)img, item.Size.Width, item.Size.Height);


            if (item.IsEthereal)
            {
                img = ReduceOpacity(img);
            }
            // ASSUMPTION: Socket image is GRID_SIZE_PX tall/wide
            if (item.Sockets > 0)
            {
                Image socket = Image.FromFile(@"images\gemsocket.png");
                socket = ReduceOpacity(socket, 0.33f);

                List<Point> points = GetSocketCoords(item);

                // Draw the sockets
                foreach (Point p in points)
                {
                    img.DrawImage(socket, p);
                }

                for (int i = 0; i < item.socketedItems.Count(); i++)
                {
                    Image simg = GetImage(item.socketedItems[i]);
                    simg = ReduceOpacity(simg, 0.25f);
                    img.DrawImage(simg, points[i]);
                }

                // Draw the items


                //if (this.checkBoxItemCodes.Checked)
                //{
                //    using (Graphics gfx = Graphics.FromImage(img))
                //    {
                //        Font font = new Font("Tahoma", 12);
                //        Brush brush = new SolidBrush(Color.White);
                //        PointF point = new PointF(0, 0);
                //        gfx.DrawString(filename.Replace(".png", ""), font, brush, point);
                //    }
                //}


            }
            return img;
        }

        private List<Point> GetSocketCoords(Item item)
        {
            List<Point> socketCoords = new List<Point>();

            // Calculate center of item
            int x = (item.Size.Width * GRID_SIZE_PX) / 2;
            int y = (item.Size.Height * GRID_SIZE_PX) / 2;

            switch (Math.Max(item.Sockets, item.NumSocketedItems))
            {
                case 1:
                    socketCoords.Add(new Point(x - GRID_SIZE_PX / 2, y - GRID_SIZE_PX / 2));
                    break;
                case 2: // 1x2
                    socketCoords.Add(new Point(x - GRID_SIZE_PX / 2, y - GRID_SIZE_PX));
                    socketCoords.Add(new Point(x - GRID_SIZE_PX / 2, y));
                    break;
                case 3:
                    if (item.Size.Height > 2) // 1x3
                    {
                        socketCoords.Add(new Point(x - GRID_SIZE_PX / 2, y - (int)(GRID_SIZE_PX * 1.5)));
                        socketCoords.Add(new Point(x - GRID_SIZE_PX / 2, y - GRID_SIZE_PX / 2));
                        socketCoords.Add(new Point(x - GRID_SIZE_PX / 2, y + GRID_SIZE_PX / 2));
                    }
                    else // 2/1
                    {
                        socketCoords.Add(new Point(x - GRID_SIZE_PX, y - GRID_SIZE_PX));
                        socketCoords.Add(new Point(x, y - GRID_SIZE_PX));
                        socketCoords.Add(new Point(x - GRID_SIZE_PX / 2, y));
                    }
                    break;
                case 4:
                    if (item.Size.Height == 4) // 1x4
                    {
                        socketCoords.Add(new Point(x - GRID_SIZE_PX / 2, y - GRID_SIZE_PX * 2));
                        socketCoords.Add(new Point(x - GRID_SIZE_PX / 2, y - GRID_SIZE_PX));
                        socketCoords.Add(new Point(x - GRID_SIZE_PX / 2, y));
                        socketCoords.Add(new Point(x - GRID_SIZE_PX / 2, y + GRID_SIZE_PX));
                    }
                    else // 2x2
                    {
                        socketCoords.Add(new Point(x - GRID_SIZE_PX, y - GRID_SIZE_PX));
                        socketCoords.Add(new Point(x, y - GRID_SIZE_PX));
                        socketCoords.Add(new Point(x - GRID_SIZE_PX, y));
                        socketCoords.Add(new Point(x, y));
                    }
                    break;
                case 5: // 2/1/2
                    {
                        socketCoords.Add(new Point(x - GRID_SIZE_PX, y - (int)(GRID_SIZE_PX * 1.5)));
                        socketCoords.Add(new Point(x, y - (int)(GRID_SIZE_PX * 1.5)));
                        socketCoords.Add(new Point(x - GRID_SIZE_PX / 2, y - GRID_SIZE_PX / 2));
                        socketCoords.Add(new Point(x - GRID_SIZE_PX, y + GRID_SIZE_PX / 2));
                        socketCoords.Add(new Point(x, y + GRID_SIZE_PX / 2));
                        break;
                    }
                case 6: // 2x3
                    {
                        socketCoords.Add(new Point(x - GRID_SIZE_PX, y - (int)(GRID_SIZE_PX * 1.5)));
                        socketCoords.Add(new Point(x, y - (int)(GRID_SIZE_PX * 1.5)));
                        socketCoords.Add(new Point(x - GRID_SIZE_PX, y - GRID_SIZE_PX / 2));
                        socketCoords.Add(new Point(x, y - GRID_SIZE_PX / 2));
                        socketCoords.Add(new Point(x - GRID_SIZE_PX, y + GRID_SIZE_PX / 2));
                        socketCoords.Add(new Point(x, y + GRID_SIZE_PX / 2));
                        break;
                    }


            }

            return socketCoords;
        }

        private Image ReduceOpacity(Image image, float percent = 0.5f)
        {
            // Create a blank bitmap with the same dimensions as the image
            Bitmap bmp = new Bitmap(image.Width, image.Height);

            // Create a graphics object from the bitmap
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Create a color matrix with 50% opacity
                float[][] matrixItems =
                {
                    new float[] {1, 0, 0, 0, 0},
                    new float[] {0, 1, 0, 0, 0},
                    new float[] {0, 0, 1, 0, 0},
                    new float[] {0, 0, 0, percent, 0},
                    new float[] {0, 0, 0, 0, 1}
                };
                ColorMatrix colorMatrix = new ColorMatrix(matrixItems);

                // Create an image attribute with the color matrix
                ImageAttributes imageAttributes = new ImageAttributes();
                imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                // Draw the original image onto the blank bitmap with the image attribute
                g.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
            }

            return bmp;
        }


        private void RefreshDisplayedItems()
        {
            ResetBackgroundImages();

            if (this.leftPanelDisplay == DisplaySection.Vault)
            {
                DrawVaultContents();
            }
            else
            {
                // Not displaying vault.  Stash is always on left in this case.  What's on right can vary.
                DrawStashContents();

                if (this.rightPanelDisplay == DisplaySection.Inventory)
                    DrawInventoryContents();
                else if (this.rightPanelDisplay == DisplaySection.Mercenary)
                    DrawMercenaryContents();
                else if (this.rightPanelDisplay == DisplaySection.Cube)
                    DrawCubeContents();
            }
        }

        private void DrawMercenaryContents()
        {
            this.rightPanelRects.Clear();
            using (Graphics g = Graphics.FromImage(this.compositedImage))
            {
                this.rightPanelRects.Clear();

                foreach (Item item in this.characterFiles[currentlyDisplayed].mercItems.items)
                {
                    Image itemImage = GetImage(item);
                    Point itemPos = new Point(0, 0);

                    switch (item.Position.X)
                    {
                        case 1: itemPos = this.mercHelm.Location; break;
                        case 3: itemPos = this.mercArmor.Location; break;
                        case 4: // left hand
                        case 5: // right hand
                            if (item.Position.X == 4) itemPos = this.mercLeftHand.Location;
                            if (item.Position.X == 5) itemPos = this.mercRightHand.Location;
                            if (item.Size.Width == 1) itemPos.X += (int)Math.Round(0.5 * GRID_SIZE_PX);
                            if (item.Size.Height == 2) itemPos.Y += GRID_SIZE_PX;
                            if (item.Size.Height == 3) itemPos.Y += (int)Math.Round(0.5 * GRID_SIZE_PX);
                            break;
                    }
                    g.DrawImage(itemImage, itemPos);

                    ItemRect rect = new ItemRect(itemPos.X, itemPos.Y, item.Size.Width * GRID_SIZE_PX, item.Size.Height * GRID_SIZE_PX);
                    rect.Item = item;
                    this.rightPanelRects.Add(rect);
                }

                // If there's an item on the cursor it is not technically in this player's inventory, but is
                // drawn at its old position so the player can see where it used to be.
                if (itemPickedFrom == DisplaySection.Mercenary &&
                    onCursor.CharacterName == this.characterFiles[currentlyDisplayed].CharacterName)
                {
                    Point itemPos = GetEquippedItemPos(onCursor, true);
                    Image itemImage = GetImage(onCursor);
                    using (Graphics g2 = Graphics.FromImage(itemImage))
                        g2.FillRectangle(ON_CURSOR_BRUSH, new Rectangle(0, 0, itemImage.Width, itemImage.Height));
                    g.DrawImage(itemImage, itemPos);

                }

            }
        }

        private void DrawCubeContents()
        {
            this.rightPanelRects.Clear();
            using (Graphics g = Graphics.FromImage(this.compositedImage))
            {
                foreach (Item item in this.characterFiles[currentlyDisplayed].playerItems.items)
                {
                    if (item.Stash != Stash.HoradricCube) continue;
                    Image itemImage = GetImage(item);
                    int x = this.cubeItemGrid.Location.X + item.Position.X * GRID_SIZE_PX;
                    int y = this.cubeItemGrid.Location.Y + item.Position.Y * GRID_SIZE_PX;
                    g.DrawImage(itemImage, new Point(x, y));

                    ItemRect rect = new ItemRect(x, y, item.Size.Width * GRID_SIZE_PX, item.Size.Height * GRID_SIZE_PX);
                    rect.Item = item;
                    this.rightPanelRects.Add(rect);
                }

                // If there's an item on the cursor it is not technically in this player's inventory, but is
                // drawn at its old position so the player can see where it used to be.
                if (itemPickedFrom == DisplaySection.Cube &&
                    onCursor.CharacterName == this.characterFiles[currentlyDisplayed].CharacterName)
                {

                    int xPos = this.cubeItemGrid.X + onCursor.Position.X * GRID_SIZE_PX;
                    int yPos = this.cubeItemGrid.Y + onCursor.Position.Y * GRID_SIZE_PX;
                    Image itemImage = GetImage(onCursor);
                    using (Graphics g2 = Graphics.FromImage(itemImage))
                        g2.FillRectangle(ON_CURSOR_BRUSH, new Rectangle(0, 0, itemImage.Width, itemImage.Height));
                    g.DrawImage(itemImage, new Point(xPos, yPos));
                }
            }
        }

        private void DrawInventoryContents()
        {
            this.rightPanelRects.Clear();

            using (Graphics g = Graphics.FromImage(this.compositedImage))
            {
                // Draw items in player inventory
                foreach (Item item in this.characterFiles[currentlyDisplayed].playerItems.items)
                {
                    if (item.Stash != Stash.Inventory) continue;
                    Image itemImage = GetImage(item);
                    int x = this.inventoryItemGrid.Location.X + item.Position.X * GRID_SIZE_PX;
                    int y = this.inventoryItemGrid.Location.Y + item.Position.Y * GRID_SIZE_PX;
                    g.DrawImage(itemImage, new Point(x, y));

                    ItemRect rect = new ItemRect(x, y, item.Size.Width * GRID_SIZE_PX, item.Size.Height * GRID_SIZE_PX);
                    rect.Item = item;
                    this.rightPanelRects.Add(rect);
                }

                // Draw items equipped on player
                foreach (Item item in this.characterFiles[currentlyDisplayed].playerItems.items)
                {
                    if (item.Parent != D2RMuleLib.Parent.Equipped) continue;
                    Image itemImage = GetImage(item);

                    Point itemPos = GetEquippedItemPos(item);

                    bool drawItem = true;
                    if (this.displayedHand == Hand.Alt && (item.Position.X == 4 || item.Position.X == 5)) drawItem = false;
                    if (this.displayedHand == Hand.Main && (item.Position.X == 11 || item.Position.X == 12)) drawItem = false;

                    if (drawItem)
                    {
                        g.DrawImage(itemImage, itemPos);

                        ItemRect rect = new ItemRect(itemPos.X, itemPos.Y, item.Size.Width * GRID_SIZE_PX, item.Size.Height * GRID_SIZE_PX);
                        rect.Item = item;
                        this.rightPanelRects.Add(rect);
                    }
                }

                // If there's an item on the cursor it is not technically in this player's inventory, but is
                // drawn at its old position so the player can see where it used to be.
                if (itemPickedFrom == DisplaySection.Inventory &&
                    onCursor.CharacterName == this.characterFiles[currentlyDisplayed].CharacterName)
                {
                    if (onCursor.Stash == Stash.Inventory) // Is item in the player's inventory?
                    {
                        int xPos = this.inventoryItemGrid.X + onCursor.Position.X * GRID_SIZE_PX;
                        int yPos = this.inventoryItemGrid.Y + onCursor.Position.Y * GRID_SIZE_PX;
                        Image itemImage = GetImage(onCursor);
                        using (Graphics g2 = Graphics.FromImage(itemImage))
                            g2.FillRectangle(ON_CURSOR_BRUSH, new Rectangle(0, 0, itemImage.Width, itemImage.Height));
                        g.DrawImage(itemImage, new Point(xPos, yPos));
                    }
                    else // Item is equipped
                    {
                        Point itemPos = GetEquippedItemPos(onCursor);
                        Image itemImage = GetImage(onCursor);
                        using (Graphics g2 = Graphics.FromImage(itemImage))
                            g2.FillRectangle(ON_CURSOR_BRUSH, new Rectangle(0, 0, itemImage.Width, itemImage.Height));
                        g.DrawImage(itemImage, itemPos);
                    }
                }
            }
        }

        private Point GetEquippedItemPos(Item item, bool isMerc = false)
        {
            Point itemPos = new Point(0, 0);

            switch (item.Position.X)
            {
                case 1:
                    if (isMerc) return this.mercHelm.Location;
                    else return this.inventoryHelm.Location;
                case 2:
                    return this.inventoryAmulet.Location;
                case 3:
                    if (isMerc) return this.mercArmor.Location;
                    else return this.mercArmor.Location;
                case 4: // left hand
                case 5: // right hand
                case 11: // alt left hand
                case 12: // alt right hand
                    if (item.Position.X == 4 || item.Position.X == 11)
                    {
                        if (isMerc) itemPos = this.mercLeftHand.Location;
                        else itemPos = this.inventoryLeftHand.Location;
                    }
                    if (item.Position.X == 5 || item.Position.X == 12)
                    {
                        if (isMerc) itemPos = this.mercRightHand.Location;
                        else itemPos = this.inventoryRightHand.Location;
                    }
                    if (item.Size.Width == 1) itemPos.X += (int)Math.Round(0.5 * GRID_SIZE_PX);
                    if (item.Size.Height == 2) itemPos.Y += GRID_SIZE_PX;
                    if (item.Size.Height == 3) itemPos.Y += (int)Math.Round(0.5 * GRID_SIZE_PX);
                    return itemPos;
                case 6: return this.inventoryLeftRing.Location;
                case 7: return this.inventoryRightRing.Location;
                case 8: return this.inventoryBelt.Location;
                case 9: return this.inventoryBoots.Location;
                case 10: return this.inventoryGloves.Location;
                default:
                    throw new Exception("Unexpected equipped item position: " + item.Position.X.ToString());
            }

        }

        private List<ItemRect> GenerateCurrentPlayerStashRects()
        {
            Rectangle stashItemGrid = this.stashItemGrid;
            if (!this.characterFiles[currentlyDisplayed].isExpansion)
                stashItemGrid = this.stashClassicItemGrid;

            List<ItemRect> list = new List<ItemRect>();
            foreach (Item item in this.characterFiles[currentlyDisplayed].playerItems.items)
            {
                if (item.Stash != Stash.Stash) continue;
                ItemRect rect = new ItemRect(stashItemGrid.X + item.Position.X * GRID_SIZE_PX,
                    stashItemGrid.Y + item.Position.Y * GRID_SIZE_PX,
                    item.Size.Width * GRID_SIZE_PX,
                    item.Size.Height * GRID_SIZE_PX);
                rect.Item = item;
                list.Add(rect);
            }

            return list;
        }

        private List<ItemRect> GenerateCurrentPlayerInventoryRects()
        {
            List<ItemRect> list = new List<ItemRect>();
            foreach (Item item in this.characterFiles[currentlyDisplayed].playerItems.items)
            {
                if (item.Stash != Stash.Inventory) continue;
                ItemRect rect = new ItemRect(this.inventoryItemGrid.X + item.Position.X * GRID_SIZE_PX,
                    this.inventoryItemGrid.Y + item.Position.Y * GRID_SIZE_PX,
                    item.Size.Width * GRID_SIZE_PX,
                    item.Size.Height * GRID_SIZE_PX);
                rect.Item = item;
                list.Add(rect);
            }

            return list;
        }

        private List<ItemRect> GenerateCurrentCubeInventoryRects()
        {
            List<ItemRect> list = new List<ItemRect>();
            foreach (Item item in this.characterFiles[currentlyDisplayed].playerItems.items)
            {
                if (item.Stash != Stash.HoradricCube) continue;
                ItemRect rect = new ItemRect(this.cubeItemGrid.X + item.Position.X * GRID_SIZE_PX,
                    this.cubeItemGrid.Y + item.Position.Y * GRID_SIZE_PX,
                    item.Size.Width * GRID_SIZE_PX,
                    item.Size.Height * GRID_SIZE_PX);
                rect.Item = item;
                list.Add(rect);
            }

            return list;
        }

        private void DrawStashContents()
        {

            Rectangle stashItemGrid = this.stashItemGrid;
            if (!this.characterFiles[currentlyDisplayed].isExpansion)
            {
                stashItemGrid = this.stashClassicItemGrid;
            }

            // Bring picturebox to the front to ensure it is over the vault search box
            this.pictureBoxLeft.BringToFront();

            using (Graphics g = Graphics.FromImage(this.compositedImage))
            {
                foreach (Item item in this.characterFiles[currentlyDisplayed].playerItems.items)
                {
                    if (item.Stash != Stash.Stash) continue;
                    int xPos = stashItemGrid.X + item.Position.X * GRID_SIZE_PX;
                    int yPos = stashItemGrid.Y + item.Position.Y * GRID_SIZE_PX;
                    Image itemImage = GetImage(item);
                    g.DrawImage(itemImage, new Point(xPos, yPos));
                }

                // If there's an item on the cursor it is not technically in this player's stash, but is drawn at its old position
                // so the player can see where it used to be.
                if (itemPickedFrom == DisplaySection.Stash &&
                    onCursor.CharacterName == this.characterFiles[currentlyDisplayed].CharacterName)
                {
                    int xPos = stashItemGrid.X + onCursor.Position.X * GRID_SIZE_PX;
                    int yPos = stashItemGrid.Y + onCursor.Position.Y * GRID_SIZE_PX;
                    Image itemImage = GetImage(onCursor);
                    using (Graphics g2 = Graphics.FromImage(itemImage))
                    {
                        g2.FillRectangle(ON_CURSOR_BRUSH, new Rectangle(0, 0, itemImage.Width, itemImage.Height));
                    }

                    // Makes entire image transparent.  Not used, but saved in case its useful later
                    //ColorMatrix colorMatrix = new ColorMatrix();
                    //colorMatrix.Matrix33 = 0.5f; // 50% transparency
                    //ImageAttributes imgAttributes = new ImageAttributes();
                    //imgAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    //g.DrawImage(itemImage, new Rectangle(xPos, yPos, itemImage.Width, itemImage.Height), 0, 0, itemImage.Width, itemImage.Height, GraphicsUnit.Pixel, imgAttributes);

                    g.DrawImage(itemImage, new Point(xPos, yPos));
                }
            }

            this.leftPanelRects = GenerateCurrentPlayerStashRects();
        }



        private void DrawVaultContents()
        {
            this.leftPanelRects.Clear();
            this.rightPanelRects.Clear();
            this.vaultItemsOnCurrentPage = 0;
            this.vaultTotalFilteredItems = 0;
            this.textBoxVaultFilter.BringToFront();

            // Apply filtering
            List<Item> filteredItems = new List<Item>();
            UInt32 itemsSkipped = vaultCurrentDisplayIndex;
            foreach (Item item in this.vaultItems.items)
            {
                if (this.textBoxVaultFilter.Text != "" &&
                    !item.HoverText().ToLower().Contains(this.textBoxVaultFilter.Text.ToLower()))
                    continue;

                // Check for filter
                if (this.filterAmulet && !item.isAmulet()) continue;
                if (this.filterArmor && !item.isArmor()) continue;
                if (this.filterBelt && !item.isBelt()) continue;
                if (this.filterBoots && !item.isBoots()) continue;
                if (this.filterGloves && !item.isGloves()) continue;
                if (this.filterHelm && !item.isHelm()) continue;
                if (this.filterRing && !item.isRing()) continue;
                if (this.filterShield && !item.isShield()) continue;
                if (this.filterWeapon && !item.isWeapon()) continue;
                if (this.filterRune && !item.isRune()) continue;
                if (this.filterCharm && !item.isCharm()) continue;
                if (this.filterGem && !item.isGem()) continue;
                if (this.filterJewel && !item.isJewel()) continue;
                if (this.filterQuest && !item.isQuest()) continue;
                if (this.filterNormal && item.Quality != Quality.Normal) continue;
                if (this.filterMagic && item.Quality != Quality.Magic) continue;
                if (this.filterRare && item.Quality != Quality.Rare) continue;
                if (this.filterSet && item.Quality != Quality.Set) continue;
                if (this.filterCrafted && item.Quality != Quality.Crafted) continue;
                if (this.filterUnique && item.Quality != Quality.Unique) continue;
                if (this.filterRuneword && !item.IsRuneword) continue;
                if (this.filterSocketed && !item.IsSocketed) continue;
                if (this.filterEthereal && !item.IsEthereal) continue;

                vaultTotalFilteredItems++;
                if (itemsSkipped > 0)
                {
                    itemsSkipped--;
                    continue;
                }
                filteredItems.Add(item);
            }

            using (Graphics g = Graphics.FromImage(this.compositedImage))
            {
                // Dynamically place vault items (side note: This is like O^4 or some crazy shit, but # of
                // items on a vault page at any given time is low so lol)
                foreach (Item item in filteredItems)
                {
                    bool itemPlaced = false;

                    // Try to find a place for this item to fit
                    for (int y = 0; y <= 10 - item.Size.Height; y++)
                    {
                        for (int x = 0; x <= 22 - item.Size.Width; x++)
                        {
                            // Generate a rect for this item if it were placed at this spot
                            ItemRect r = new ItemRect(this.stashItemGrid.X + x * GRID_SIZE_PX,
                                                      this.stashItemGrid.Y + y * GRID_SIZE_PX,
                                                      item.Size.Width * GRID_SIZE_PX,
                                                      item.Size.Height * GRID_SIZE_PX);

                            // Does this rect collide with any other rects?
                            bool collides = false;
                            foreach (ItemRect ir in this.leftPanelRects)
                            {
                                if (ir.rect.IntersectsWith(r.rect))
                                {
                                    collides = true;
                                    break;
                                }
                            }
                            if (collides)
                                continue;

                            // If execution gets here, the item does not collide and can be dropped in this spot
                            Image itemImage = GetImage(item);
                            g.DrawImage(itemImage, new Point(r.rect.X, r.rect.Y));
                            r.Item = item;
                            this.leftPanelRects.Add(r);
                            itemPlaced = true;
                            vaultItemsOnCurrentPage++;
                            break;

                        }

                        if (itemPlaced)
                            break;
                    }
                }

                // Draw counters
                string text = "Items " + vaultCurrentDisplayIndex.ToString() + " to " + (vaultCurrentDisplayIndex + vaultItemsOnCurrentPage).ToString();
                DrawAndCenterText(g, vaultCenterText, text);
                DrawAndCenterText(g, vaultLeftText, vaultCurrentDisplayIndex.ToString());
                DrawAndCenterText(g, vaultRightText, (filteredItems.Count - itemsSkipped - vaultItemsOnCurrentPage).ToString());

                // Draw filtering states
                if (this.filterAmulet) g.DrawImage(this.vaultFilterCheckedImage, this.vaultFilterAmulet);
                if (this.filterAmulet) g.DrawImage(this.vaultFilterCheckedImage, this.vaultFilterAmulet);
                if (this.filterArmor) g.DrawImage(this.vaultFilterCheckedImage, this.vaultFilterArmor);
                if (this.filterBelt) g.DrawImage(this.vaultFilterCheckedImage, this.vaultFilterBelt);
                if (this.filterBoots) g.DrawImage(this.vaultFilterCheckedImage, this.vaultFilterBoots);
                if (this.filterGloves) g.DrawImage(this.vaultFilterCheckedImage, this.vaultFilterGloves);
                if (this.filterHelm) g.DrawImage(this.vaultFilterCheckedImage, this.vaultFilterHelm);
                if (this.filterRing) g.DrawImage(this.vaultFilterCheckedImage, this.vaultFilterRing);
                if (this.filterShield) g.DrawImage(this.vaultFilterCheckedImage, this.vaultFilterShield);
                if (this.filterWeapon) g.DrawImage(this.vaultFilterCheckedImage, this.vaultFilterWeapon);
                if (this.filterRune) g.DrawImage(this.vaultFilterCheckedImage, this.vaultFilterRune);
                if (this.filterCharm) g.DrawImage(this.vaultFilterCheckedImage, this.vaultFilterCharm);
                if (this.filterGem) g.DrawImage(this.vaultFilterCheckedImage, this.vaultFilterGem);
                if (this.filterJewel) g.DrawImage(this.vaultFilterCheckedImage, this.vaultFilterJewel);
                if (this.filterQuest) g.DrawImage(this.vaultFilterCheckedImage, this.vaultFilterQuest);
                if (this.filterNormal) g.DrawImage(this.vaultFilterCheckedImage, this.vaultFilterNormal);
                if (this.filterMagic) g.DrawImage(this.vaultFilterCheckedImage, this.vaultFilterMagic);
                if (this.filterRare) g.DrawImage(this.vaultFilterCheckedImage, this.vaultFilterRare);
                if (this.filterSet) g.DrawImage(this.vaultFilterCheckedImage, this.vaultFilterSet);
                if (this.filterCrafted) g.DrawImage(this.vaultFilterCheckedImage, this.vaultFilterCrafted);
                if (this.filterUnique) g.DrawImage(this.vaultFilterCheckedImage, this.vaultFilterUnique);
                if (this.filterRuneword) g.DrawImage(this.vaultFilterCheckedImage, this.vaultFilterRuneword);
                if (this.filterSocketed) g.DrawImage(this.vaultFilterCheckedImage, this.vaultFilterSocketed);
                if (this.filterEthereal) g.DrawImage(this.vaultFilterCheckedImage, this.vaultFilterEthereal);
            }
        }

        private void DrawAndCenterText(Graphics g, Rectangle rect, string text)
        {
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            Font font = new Font("Formal436 BT", 30, FontStyle.Regular, GraphicsUnit.Pixel);
            SolidBrush brush = new SolidBrush(Color.White);
            PointF position = new PointF(508, 1316); // Coordinates where the text will be drawn

            SizeF textSize = g.MeasureString(text, font);
            float x = rect.X + (rect.Width - textSize.Width) / 2;
            float y = rect.Y + (rect.Height - textSize.Height) / 2;
            g.DrawString(text, font, brush, new PointF(x, y));
        }

        private void InitializeGUI()
        {
            ResetBackgroundImages();
            this.listBoxCharacters.Items.Clear();
            if (!this.textBoxDirectory.Text.EndsWith("\\"))
                this.textBoxDirectory.Text += "\\";
            string folderPath = this.textBoxDirectory.Text;

            string[] files = Directory.GetFiles(folderPath, "*.d2s");
            foreach (string file in files)
            {
                string fullPath = folderPath + file;
                D2SFile d2SFile = new D2SFile(file);

                bool includeCharacter = true;

                if (this.radioButtonExpansion.Checked && !d2SFile.isExpansion)
                    includeCharacter = false;
                if (this.radioButtonClassic.Checked && d2SFile.isExpansion)
                    includeCharacter = false;
                if (this.radioButtonNormal.Checked && d2SFile.isHardcore)
                    includeCharacter = false;
                if (this.radioButtonHardcore.Checked && !d2SFile.isHardcore)
                    includeCharacter = false;
                if (includeCharacter)
                    this.listBoxCharacters.Items.Add(Path.GetFileName(file));
            }
        }

        private int _int(double val)
        {
            return (int)Math.Round(val);
        }

        private void ResetBackgroundImages()
        {
            // Size the picturebox accordingly.  Aspect aspect ratio is preserved.
            double aspectRatio = (double)this.vaultOriginalImage.Width / this.vaultOriginalImage.Height;
            double pictureBoxHeight = Math.Max(250, this.pictureBoxLeft.Height);
            double pictureBoxWidth = pictureBoxHeight * aspectRatio;

            // Resize the picturebox and form to fit
            this.pictureBoxLeft.Size = new Size(_int(pictureBoxWidth), _int(pictureBoxHeight));
            this.Width = this.pictureBoxLeft.Right + MARGIN + (this.Width - this.ClientSize.Width);
            this.Height = this.pictureBoxLeft.Bottom + MARGIN + (this.Height - this.ClientSize.Height);

            this.compositedImage = (Image)vaultOriginalImage.Clone();

            // Display a right side image only if we aren't already looking at the vault
            if (this.leftPanelDisplay != DisplaySection.Vault)
            {
                // Draw the stash on the left
                if (this.characterFiles.Count == 0 || this.characterFiles[currentlyDisplayed].isExpansion)
                {
                    this.compositedImage.DrawImage(this.stashOriginalImage);

                    // Draw the stash selector tab at the top left of the stash
                    Image tabStash = Image.FromFile("images/stash_tab.png");
                    this.compositedImage.DrawImage(tabStash, new Point(84, 164));
                }
                else
                {
                    this.compositedImage.DrawImage(this.classicStashOriginalImage);
                }

                // Draw the appropriate right side
                switch (this.rightPanelDisplay)
                {
                    case DisplaySection.Inventory:
                        if (this.displayedHand == Hand.Main)
                            this.compositedImage.DrawImage(this.inventoryOriginalImage);
                        else
                            this.compositedImage.DrawImage(this.inventoryOriginalAltImage);
                        break;
                    case DisplaySection.Mercenary:
                        this.compositedImage.DrawImage(this.mercOriginalImage);
                        break;
                    case DisplaySection.Cube:
                        this.compositedImage.DrawImage(this.cubeOriginalImage);
                        break;
                }
            }

            // Set the image to be displayed
            this.pictureBoxLeft.Image = this.compositedImage;

            // Determine the ratio of picture box to original images.  This is used when determining if a
            // click in picturebox coordinates matches up with an item in base image coordinates.
            double ratioX = (double)this.pictureBoxLeft.Width / vaultOriginalImage.Width;
            double ratioY = (double)this.pictureBoxLeft.Height / vaultOriginalImage.Height;
            this.ratio = (ratioX + ratioY) / 2;
            FormInventoryExtensions.ratio = (ratioX + ratioY) / 2;

            // Always clean up after yourself.  The GC doesn't run often enough on its own and memory usage
            // can balloon wildly after viewing several characters.
            GC.Collect();
        }

        private Point PictureboxPointToImagePoint(Point location)
        {
            return new Point(_int(location.X / this.ratio), _int(location.Y / this.ratio));
        }

        bool HandleVaultLeftButton()
        {
            if (vaultCurrentDisplayIndex == 0)
                return false;


            if (vaultDisplayIndexStack.Count > 0)
                vaultCurrentDisplayIndex = vaultDisplayIndexStack.Pop();
            else
                vaultCurrentDisplayIndex = 0;

            PlayButtonSound();
            return true;
        }

        bool HandleVaultRightButton()
        {
            if (vaultTotalFilteredItems == vaultCurrentDisplayIndex + vaultItemsOnCurrentPage)
                return false;

            vaultDisplayIndexStack.Push(vaultCurrentDisplayIndex);
            vaultCurrentDisplayIndex += vaultItemsOnCurrentPage;

            PlayButtonSound();
            return true;
        }

        public bool filterAmulet = false;
        public bool filterArmor = false;
        public bool filterBelt = false;
        public bool filterBoots = false;
        public bool filterGloves = false;
        public bool filterHelm = false;
        public bool filterRing = false;
        public bool filterShield = false;
        public bool filterWeapon = false;
        public bool filterRune = false;
        public bool filterCharm = false;
        public bool filterGem = false;
        public bool filterJewel = false;
        public bool filterQuest = false;
        public bool filterNormal = false;
        public bool filterMagic = false;
        public bool filterRare = false;
        public bool filterSet = false;
        public bool filterCrafted = false;
        public bool filterUnique = false;
        public bool filterRuneword = false;
        public bool filterSocketed = false;
        public bool filterEthereal = false;

        bool HandleVaultFilterClick(MouseEventArgs e)
        {
            string equipPropToSet = "";
            if (this.vaultFilterAmulet.ContainsPoint(e)) equipPropToSet = nameof(filterAmulet);
            if (this.vaultFilterArmor.ContainsPoint(e)) equipPropToSet = nameof(filterArmor);
            if (this.vaultFilterBelt.ContainsPoint(e)) equipPropToSet = nameof(filterBelt);
            if (this.vaultFilterBoots.ContainsPoint(e)) equipPropToSet = nameof(filterBoots);
            if (this.vaultFilterGloves.ContainsPoint(e)) equipPropToSet = nameof(filterGloves);
            if (this.vaultFilterHelm.ContainsPoint(e)) equipPropToSet = nameof(filterHelm);
            if (this.vaultFilterRing.ContainsPoint(e)) equipPropToSet = nameof(filterRing);
            if (this.vaultFilterShield.ContainsPoint(e)) equipPropToSet = nameof(filterShield);
            if (this.vaultFilterWeapon.ContainsPoint(e)) equipPropToSet = nameof(filterWeapon);

            if (this.vaultFilterRune.ContainsPoint(e)) { this.filterRune = !this.filterRune; return true; }
            if (this.vaultFilterCharm.ContainsPoint(e)) { this.filterCharm = !this.filterCharm; return true; }
            if (this.vaultFilterGem.ContainsPoint(e)) { this.filterGem = !this.filterGem; return true; }
            if (this.vaultFilterJewel.ContainsPoint(e)) { this.filterJewel = !this.filterJewel; return true; }
            if (this.vaultFilterQuest.ContainsPoint(e)) { this.filterQuest = !this.filterQuest; return true; }

            string qualityPropToSet = "";
            if (this.vaultFilterNormal.ContainsPoint(e)) qualityPropToSet = nameof(filterNormal);
            if (this.vaultFilterMagic.ContainsPoint(e)) qualityPropToSet = nameof(filterMagic);
            if (this.vaultFilterRare.ContainsPoint(e)) qualityPropToSet = nameof(filterRare);
            if (this.vaultFilterSet.ContainsPoint(e)) qualityPropToSet = nameof(filterSet);
            if (this.vaultFilterCrafted.ContainsPoint(e)) qualityPropToSet = nameof(filterCrafted);
            if (this.vaultFilterUnique.ContainsPoint(e)) qualityPropToSet = nameof(filterUnique);

            if (this.vaultFilterRuneword.ContainsPoint(e)) { this.filterRuneword = !this.filterRuneword; return true; }
            if (this.vaultFilterSocketed.ContainsPoint(e)) { this.filterSocketed = !this.filterSocketed; return true; }
            if (this.vaultFilterEthereal.ContainsPoint(e)) { this.filterEthereal = !this.filterEthereal; return true; }

            if (equipPropToSet != "")
            {
                Type t = this.GetType();
                FieldInfo finfo = t.GetField(equipPropToSet);

                if ((bool)finfo.GetValue(this) == true)
                    ClearEquipmentFilter();
                else
                {
                    ClearEquipmentFilter();
                    finfo.SetValue(this, true);
                }
                return true;
            }

            if (qualityPropToSet != "")
            {
                Type t = this.GetType();
                FieldInfo finfo = t.GetField(qualityPropToSet);

                if ((bool)finfo.GetValue(this) == true)
                    ClearQualityFilter();
                else
                {
                    ClearQualityFilter();
                    finfo.SetValue(this, true);
                }
                return true;
            }


            return false;
        }

        void ClearEquipmentFilter()
        {
            this.filterAmulet = this.filterArmor = this.filterBelt = this.filterBoots = this.filterGloves = this.filterHelm = this.filterRing = this.filterShield = this.filterWeapon = false;
        }

        void ClearQualityFilter()
        {
            this.filterNormal = this.filterMagic = this.filterRare = this.filterSet = this.filterCrafted = this.filterUnique = false;
        }

        bool HandleVaultItemClick(MouseEventArgs e)
        {
            // Only right clicking is allowed on vault items
            if (e.Button != MouseButtons.Right)
            {
                PlayFailSound();
                return false;
            }

            // Was an item clicked on?
            foreach (ItemRect rect in this.leftPanelRects)
            {
                // Is the cursor over this item?
                if (!rect.rect.ContainsPoint(e))
                    continue;

                List<ItemRect> stashRects = GenerateCurrentPlayerStashRects();

                Rectangle stashItemGrid = this.stashItemGrid;
                Size stashSize = this.STASH_SIZE;
                if (!this.characterFiles[currentlyDisplayed].isExpansion)
                {
                    stashSize = this.CLASSIC_STASH_SIZE;
                    stashItemGrid = this.stashClassicItemGrid;
                }

                // Attempt to place the item somewhere in the player's stash
                for (int y = 0; y < stashSize.Height; y++)
                {
                    for (int x = 0; x < stashSize.Width; x++)
                    {
                        // Determine the bounding rectangle that would go with the item if it was dropped here
                        Rectangle candidateRect = new Rectangle(stashItemGrid.X + x * GRID_SIZE_PX,
                            stashItemGrid.Y + y * GRID_SIZE_PX,
                            rect.Item.Size.Width * GRID_SIZE_PX,
                            rect.Item.Size.Height * GRID_SIZE_PX);

                        // Does this rectangle collide with any other?
                        bool collision = false;
                        foreach (ItemRect ir in stashRects)
                        {
                            if (candidateRect.IntersectsWith(ir.rect))
                            {
                                collision = true;
                                continue;
                            }
                        }

                        if (!collision)
                        {
                            // Ok now check if it would go outside the bounds of the item grid
                            if (x + rect.Item.Size.Width > stashSize.Width || y + rect.Item.Size.Height > stashSize.Height)
                                continue;

                            // This item fits on player stash at this position.  Move it.
                            Item item = rect.Item;
                            if (this.toolStripMenuItemDupeOnVaultRemove.Checked)
                            {
                                item = (Item)rect.Item.Clone();
                            }
                            else
                            {
                                this.vaultItems.items.Remove(rect.Item);
                            }

                            item.Position = new Point(x, y);
                            item.Parent = D2RMuleLib.Parent.Stored;
                            item.Stash = Stash.Stash;
                            item.CharacterName = this.characterFiles[currentlyDisplayed].CharacterName;
                            this.characterFiles[currentlyDisplayed].playerItems.items.Add(item);
                            this.characterFiles[currentlyDisplayed].Modified = true;
                            PlayVaultSound();
                            RefreshDisplayedItems();
                            return true;
                        }
                    }
                }

                // If execution gets here, we were unable to find a place to move it to
                //MessageBox.Show("Unable to move item to player stash, not enough space");
            }

            PlayFailSound();
            return false;
        }

        bool AttemptToPlaceInOppositePanel(Item item)
        {
            // Assume target is stash, this is overriden below if it isn't
            Rectangle targetGrid = this.stashClassicItemGrid;
            List<ItemRect> targetRects = GenerateCurrentPlayerStashRects();
            int targetMaxX = CLASSIC_STASH_SIZE.Width;
            int targetMaxY = CLASSIC_STASH_SIZE.Height;

            if (this.characterFiles[currentlyDisplayed].isExpansion)
            {
                targetGrid = this.stashItemGrid;
                targetMaxX = STASH_SIZE.Width;
                targetMaxY = STASH_SIZE.Height;
            }
            Stash targetStash = Stash.Stash;

            // If item is the stash, it must be going to inventory or cube (not allowed to go to merc)
            if (item.Stash == Stash.Stash)
            {
                switch (this.rightPanelDisplay)
                {
                    case DisplaySection.Inventory:
                        targetGrid = this.inventoryItemGrid;
                        targetRects = GenerateCurrentPlayerInventoryRects();
                        targetMaxY = 4;
                        targetStash = Stash.Inventory;
                        break;
                    case DisplaySection.Cube:
                        targetGrid = this.cubeItemGrid;
                        targetRects = GenerateCurrentCubeInventoryRects();
                        targetMaxX = 3;
                        targetMaxY = 4;
                        targetStash = Stash.HoradricCube;
                        break;
                    default:
                        // Not going to attempt to dynamically equip this, let user do that manually
                        PlayFailSound();
                        return false;
                }
            }

            // Attempt to place the item somewhere that it fits
            for (int y = 0; y < targetMaxY; y++)
            {
                for (int x = 0; x < targetMaxX; x++)
                {
                    // Determine the bounding rectangle that would go with the item if it was dropped here
                    Rectangle candidateRect = new Rectangle(targetGrid.X + x * GRID_SIZE_PX,
                        targetGrid.Y + y * GRID_SIZE_PX,
                        item.Size.Width * GRID_SIZE_PX,
                        item.Size.Height * GRID_SIZE_PX);

                    // Does this rectangle collide with any other?
                    bool collision = false;
                    foreach (ItemRect ir in targetRects)
                    {
                        if (candidateRect.IntersectsWith(ir.rect))
                        {
                            collision = true;
                            continue;
                        }
                    }
                    if (collision)
                        continue;

                    // No collision, now check if it would go outside the bounds of the grid
                    if (x + item.Size.Width > targetMaxX || y + item.Size.Height > targetMaxY)
                        continue;

                    // This item fits at this position. Update its properties
                    item.Position = new Point(x, y);
                    item.Parent = D2RMuleLib.Parent.Stored;
                    item.Stash = targetStash;

                    // If this is a merc item, it needs to be moved to the player item list
                    if (this.characterFiles[currentlyDisplayed].mercItems.items.Contains(item))
                    {
                        this.characterFiles[currentlyDisplayed].mercItems.items.Remove(item);
                        this.characterFiles[currentlyDisplayed].playerItems.items.Add(item);
                    }
                    this.characterFiles[currentlyDisplayed].Modified = true;
                    PlayItemSound(item);
                    return true;
                }
            }

            PlayFailSound();
            return false;
        }

        bool HandlePossibleItemClickWithNoItemOnCursor(MouseEventArgs e)
        {
            bool clickedOnItem = false;
            Item itemClicked = new Item();

            List<ItemRect> itemRects = this.leftPanelRects;
            if (this.rightHalf.ContainsPoint(e))
                itemRects = this.rightPanelRects;

            // Was an item clicked on to be picked up?
            foreach (ItemRect rect in itemRects)
            {
                if (!rect.rect.ContainsPoint(e))
                    continue;
                itemClicked = rect.Item;
                clickedOnItem = true;
                break;
            }
            if (!clickedOnItem)
                return false;

            // An item was clicked on.  Depending on modifier key and mouse button used, different actions:
            //   Left click, no modifier key  = "pick up" to cursor
            //   Left click, control key      = to other panel
            //   Right click, no modifier key = to vault

            if (e.Button == MouseButtons.Left && Control.ModifierKeys == Keys.Control)
            {
                return AttemptToPlaceInOppositePanel(itemClicked);
            }
            else
            {
                if (e.Button == MouseButtons.Left && Control.ModifierKeys == Keys.None)
                {
                    PlayPickupSound();
                    onCursor = itemClicked;
                    if (this.leftHalf.ContainsPoint(e))
                        itemPickedFrom = this.leftPanelDisplay;
                    else
                        itemPickedFrom = this.rightPanelDisplay;
                }
                else if (e.Button == MouseButtons.Right && Control.ModifierKeys == Keys.None)
                {
                    if (!isAllowedInVault(itemClicked)) return false;
                    this.vaultItems.items.Add(itemClicked);
                    SortVault();
                    PlayVaultSound();
                }
                else
                {
                    // Some other mouse button/modifier key combo, ignore this
                    return false;
                }

                // Item was either picked up or moved to the vault.  Either way, it needs to be removed
                // from wherever it originated from.
                if (this.rightHalf.ContainsPoint(e))
                {
                    if (this.rightPanelDisplay == DisplaySection.Inventory ||
                        this.rightPanelDisplay == DisplaySection.Cube)
                    {
                        // Item was on the player (cube is considered on the player)
                        this.characterFiles[currentlyDisplayed].playerItems.items.Remove(itemClicked);
                        this.characterFiles[currentlyDisplayed].Modified = true;
                        return true;
                    }
                    else
                    {
                        // Item was on the merc
                        this.characterFiles[currentlyDisplayed].mercItems.items.Remove(itemClicked);
                        this.characterFiles[currentlyDisplayed].Modified = true;
                        return true;
                    }
                }
                else
                {
                    // Item was in the stash
                    this.characterFiles[currentlyDisplayed].playerItems.items.Remove(itemClicked);
                    this.characterFiles[currentlyDisplayed].Modified = true;
                    return true;
                }
            }
        }

        bool HandleStashIngestButtonClick()
        {
            bool refreshNeeded = false;
            foreach (ItemRect rect in this.leftPanelRects)
            {
                if (!isAllowedInVault(rect.Item)) continue;
                this.vaultItems.items.Add(rect.Item);
                this.characterFiles[currentlyDisplayed].playerItems.items.Remove(rect.Item);
                this.characterFiles[currentlyDisplayed].Modified = true;
                refreshNeeded = true;
            }

            if (this.toolStripMenuItemOptionAllToVaultIncludesInventory.Checked)
            {
                foreach (ItemRect rect in this.rightPanelRects)
                {
                    if (!isAllowedInVault(rect.Item)) continue;
                    this.vaultItems.items.Add(rect.Item);
                    this.characterFiles[currentlyDisplayed].playerItems.items.Remove(rect.Item);
                    this.characterFiles[currentlyDisplayed].Modified = true;
                    refreshNeeded = true;
                }
            }

            SortVault();

            PlayVaultSound();

            return refreshNeeded;
        }

        void SortVault()
        {
            // Auto-sort by item type?
            //this.vaultItems.Sort();
        }

        bool isAllowedInVault(Item item)
        {
            return item.TypeCode != "box";
        }

        bool PlayerHasEquipped(EquippedSlot slot)
        {
            return HasEquippedInternal(this.characterFiles[currentlyDisplayed].playerItems.items, slot);
        }

        bool MercHasEquipped(EquippedSlot slot)
        {
            return HasEquippedInternal(this.characterFiles[currentlyDisplayed].mercItems.items, slot);
        }

        bool HasEquippedInternal(List<Item> items, EquippedSlot slot)
        {
            foreach (Item i in items)
            {
                if (i.Parent == D2RMuleLib.Parent.Equipped
                    && i.Position.X == (int)slot)
                    return true;
            }

            return false;
        }

        bool HandleInventoryClickWithItemOnCursor_Merc(MouseEventArgs e)
        {
            // TODO: Check character class on merc to see if they're allowed to hold this item?

            // Check if the item is being dropped in any of the equipped slots
            if (this.mercHelm.ContainsPoint(e) && onCursor.isHelm())
            {
                if (MercHasEquipped(EquippedSlot.Helm)) return false;
                onCursor.Position = new Point(1, 0);
                onCursor.Parent = D2RMuleLib.Parent.Equipped;
                onCursor.Stash = Stash.None;
                onCursor.CharacterName = this.characterFiles[currentlyDisplayed].CharacterName;
                this.characterFiles[currentlyDisplayed].mercItems.items.Add(onCursor);
                this.characterFiles[currentlyDisplayed].Modified = true;
                PlayItemSound(onCursor);
                onCursor = new Item();
                itemPickedFrom = DisplaySection.None;
                return true;
            }
            else if (this.inventoryArmor.ContainsPoint(e) && onCursor.isArmor())
            {
                if (MercHasEquipped(EquippedSlot.Armor)) return false;
                onCursor.Position = new Point(3, 0);
                onCursor.Parent = D2RMuleLib.Parent.Equipped;
                onCursor.Stash = Stash.None;
                onCursor.CharacterName = this.characterFiles[currentlyDisplayed].CharacterName;
                this.characterFiles[currentlyDisplayed].mercItems.items.Add(onCursor);
                this.characterFiles[currentlyDisplayed].Modified = true;
                PlayItemSound(onCursor);
                onCursor = new Item();
                itemPickedFrom = DisplaySection.None;
                return true;
            }
            else if (this.inventoryLeftHand.ContainsPoint(e) && (onCursor.isWeapon() || onCursor.isShield()))
            {
                if (MercHasEquipped(EquippedSlot.LHand)) return false;
                onCursor.Position = new Point(4, 0);
                onCursor.Parent = D2RMuleLib.Parent.Equipped;
                onCursor.Stash = Stash.None;
                onCursor.CharacterName = this.characterFiles[currentlyDisplayed].CharacterName;
                this.characterFiles[currentlyDisplayed].mercItems.items.Add(onCursor);
                this.characterFiles[currentlyDisplayed].Modified = true;
                PlayItemSound(onCursor);
                onCursor = new Item();
                itemPickedFrom = DisplaySection.None;
                return true;
            }
            else if (this.inventoryRightHand.ContainsPoint(e) && (onCursor.isWeapon() || onCursor.isShield()))
            {
                if (MercHasEquipped(EquippedSlot.RHand)) return false;
                onCursor.Position = new Point(5, 0);
                onCursor.Parent = D2RMuleLib.Parent.Equipped;
                onCursor.Stash = Stash.None;
                onCursor.CharacterName = this.characterFiles[currentlyDisplayed].CharacterName;
                this.characterFiles[currentlyDisplayed].mercItems.items.Add(onCursor);
                this.characterFiles[currentlyDisplayed].Modified = true;
                PlayItemSound(onCursor);
                onCursor = new Item();
                itemPickedFrom = DisplaySection.None;
                return true;
            }

            return false;
        }
        bool HandleInventoryClickWithItemOnCursor_Cube(MouseEventArgs e)
        {
            // Not an equipped slot, was this a click within the item grid?
            if (!this.cubeItemGrid.ContainsPoint(e))
                return false;

            // User clicked in the cube grid.  If the item is dropped right here, would it collide with
            // any other item?

            // First, determine what grid space was clicked on
            int gridX = (int)(((e.Location.X / this.ratio) - this.cubeItemGrid.Location.X) / GRID_SIZE_PX);
            int gridY = (int)(((e.Location.Y / this.ratio) - this.cubeItemGrid.Location.Y) / GRID_SIZE_PX);

            // Now determine the bounding rectangle that goes with the item if it were to be dropped right here
            Rectangle rect = new Rectangle(this.cubeItemGrid.X + gridX * GRID_SIZE_PX,
                                           this.cubeItemGrid.Y + gridY * GRID_SIZE_PX,
                                           onCursor.Size.Width * GRID_SIZE_PX,
                                           onCursor.Size.Height * GRID_SIZE_PX);

            // Does this rectangle collide with any other?
            foreach (ItemRect ir in this.rightPanelRects)
            {
                if (rect.IntersectsWith(ir.rect))
                    return false;
            }

            // Ok now check if it would go outside the bounds of the item grid
            if (gridX + onCursor.Size.Width > 3 || gridY + onCursor.Size.Height > 4)
                return false;

            // If execution gets here, the item does not collide with anything in the grid and it does
            // not exceed the boundaries of the grid, it can be dropped here.  This grid spot is used as
            // the upper left of the item.  The item is added to the player's item list
            onCursor.Position = new Point(gridX, gridY);
            onCursor.Parent = D2RMuleLib.Parent.Stored;
            onCursor.Stash = Stash.HoradricCube;
            onCursor.CharacterName = this.characterFiles[currentlyDisplayed].CharacterName;
            this.characterFiles[currentlyDisplayed].playerItems.items.Add(onCursor);
            this.characterFiles[currentlyDisplayed].Modified = true;
            PlayItemSound(onCursor);
            onCursor = new Item();
            itemPickedFrom = DisplaySection.None;
            return true;
        }
        bool HandleInventoryClickWithItemOnCursor_Player(MouseEventArgs e)
        {
            // Check if the item is being dropped in any of the equipped slots
            if (this.inventoryHelm.ContainsPoint(e) && onCursor.isHelm())
            {
                if (PlayerHasEquipped(EquippedSlot.Helm)) return false;
                if (this.characterFiles[currentlyDisplayed].CharacterClass != "Barbarian" && onCursor.isBarbHelm()) return false;
                if (this.characterFiles[currentlyDisplayed].CharacterClass != "Druid" && onCursor.isDruidHelm()) return false;

                onCursor.Position = new Point(1, 0);
                onCursor.Parent = D2RMuleLib.Parent.Equipped;
                onCursor.Stash = Stash.None;
                onCursor.CharacterName = this.characterFiles[currentlyDisplayed].CharacterName;
                this.characterFiles[currentlyDisplayed].playerItems.items.Add(onCursor);
                this.characterFiles[currentlyDisplayed].Modified = true;
                PlayItemSound(onCursor);
                onCursor = new Item();
                itemPickedFrom = DisplaySection.None;
                return true;
            }
            else if (this.inventoryAmulet.ContainsPoint(e) && onCursor.isAmulet())
            {
                if (PlayerHasEquipped(EquippedSlot.Amulet)) return false;
                onCursor.Position = new Point(2, 0);
                onCursor.Parent = D2RMuleLib.Parent.Equipped;
                onCursor.Stash = Stash.None;
                onCursor.CharacterName = this.characterFiles[currentlyDisplayed].CharacterName;
                this.characterFiles[currentlyDisplayed].playerItems.items.Add(onCursor);
                this.characterFiles[currentlyDisplayed].Modified = true;
                PlayItemSound(onCursor);
                onCursor = new Item();
                itemPickedFrom = DisplaySection.None;
                return true;
            }
            else if (this.inventoryArmor.ContainsPoint(e) && onCursor.isArmor())
            {
                if (PlayerHasEquipped(EquippedSlot.Armor)) return false;
                onCursor.Position = new Point(3, 0);
                onCursor.Parent = D2RMuleLib.Parent.Equipped;
                onCursor.Stash = Stash.None;
                onCursor.CharacterName = this.characterFiles[currentlyDisplayed].CharacterName;
                this.characterFiles[currentlyDisplayed].playerItems.items.Add(onCursor);
                this.characterFiles[currentlyDisplayed].Modified = true;
                PlayItemSound(onCursor);
                onCursor = new Item();
                itemPickedFrom = DisplaySection.None;
                return true;
            }
            else if (this.inventoryLeftRing.ContainsPoint(e) && onCursor.isRing())
            {
                if (PlayerHasEquipped(EquippedSlot.LRing)) return false;
                onCursor.Position = new Point(6, 0);
                onCursor.Parent = D2RMuleLib.Parent.Equipped;
                onCursor.Stash = Stash.None;
                onCursor.CharacterName = this.characterFiles[currentlyDisplayed].CharacterName;
                this.characterFiles[currentlyDisplayed].playerItems.items.Add(onCursor);
                this.characterFiles[currentlyDisplayed].Modified = true;
                PlayItemSound(onCursor);
                onCursor = new Item();
                itemPickedFrom = DisplaySection.None;
                return true;
            }
            else if (this.inventoryRightRing.ContainsPoint(e) && onCursor.isRing())
            {
                if (PlayerHasEquipped(EquippedSlot.RRing)) return false;
                onCursor.Position = new Point(7, 0);
                onCursor.Parent = D2RMuleLib.Parent.Equipped;
                onCursor.Stash = Stash.None;
                onCursor.CharacterName = this.characterFiles[currentlyDisplayed].CharacterName;
                this.characterFiles[currentlyDisplayed].playerItems.items.Add(onCursor);
                this.characterFiles[currentlyDisplayed].Modified = true;
                PlayItemSound(onCursor);
                onCursor = new Item();
                itemPickedFrom = DisplaySection.None;
                return true;
            }
            else if (this.inventoryBelt.ContainsPoint(e) && onCursor.isBelt())
            {
                if (PlayerHasEquipped(EquippedSlot.Belt)) return false;
                onCursor.Position = new Point(8, 0);
                onCursor.Parent = D2RMuleLib.Parent.Equipped;
                onCursor.Stash = Stash.None;
                onCursor.CharacterName = this.characterFiles[currentlyDisplayed].CharacterName;
                this.characterFiles[currentlyDisplayed].playerItems.items.Add(onCursor);
                this.characterFiles[currentlyDisplayed].Modified = true;
                PlayItemSound(onCursor);
                onCursor = new Item();
                itemPickedFrom = DisplaySection.None;
                return true;
            }
            else if (this.inventoryBoots.ContainsPoint(e) && onCursor.isBoots())
            {
                if (PlayerHasEquipped(EquippedSlot.Boots)) return false;
                onCursor.Position = new Point(9, 0);
                onCursor.Parent = D2RMuleLib.Parent.Equipped;
                onCursor.Stash = Stash.None;
                onCursor.CharacterName = this.characterFiles[currentlyDisplayed].CharacterName;
                this.characterFiles[currentlyDisplayed].playerItems.items.Add(onCursor);
                this.characterFiles[currentlyDisplayed].Modified = true;
                PlayItemSound(onCursor);
                onCursor = new Item();
                itemPickedFrom = DisplaySection.None;
                return true;
            }
            else if (this.inventoryGloves.ContainsPoint(e) && onCursor.isGloves())
            {
                if (PlayerHasEquipped(EquippedSlot.Gloves)) return false;
                onCursor.Position = new Point(10, 0);
                onCursor.Parent = D2RMuleLib.Parent.Equipped;
                onCursor.Stash = Stash.None;
                onCursor.CharacterName = this.characterFiles[currentlyDisplayed].CharacterName;
                this.characterFiles[currentlyDisplayed].playerItems.items.Add(onCursor);
                this.characterFiles[currentlyDisplayed].Modified = true;
                PlayItemSound(onCursor);
                onCursor = new Item();
                itemPickedFrom = DisplaySection.None;
                return true;
            }
            else if (this.inventoryLeftHand.ContainsPoint(e) && (onCursor.isWeapon() || onCursor.isShield()))
            {
                if (displayedHand == Hand.Main)
                {
                    if (PlayerHasEquipped(EquippedSlot.LHand)) return false;
                    onCursor.Position = new Point(4, 0);
                }
                else
                {
                    if (PlayerHasEquipped(EquippedSlot.LHandAlt)) return false;
                    onCursor.Position = new Point(11, 0);
                }

                onCursor.Parent = D2RMuleLib.Parent.Equipped;
                onCursor.Stash = Stash.None;
                onCursor.CharacterName = this.characterFiles[currentlyDisplayed].CharacterName;
                this.characterFiles[currentlyDisplayed].playerItems.items.Add(onCursor);
                this.characterFiles[currentlyDisplayed].Modified = true;
                PlayItemSound(onCursor);
                onCursor = new Item();
                itemPickedFrom = DisplaySection.None;
                return true;
            }
            else if (this.inventoryRightHand.ContainsPoint(e) && (onCursor.isWeapon() || onCursor.isShield()))
            {
                if (displayedHand == Hand.Main)
                {
                    if (PlayerHasEquipped(EquippedSlot.RHand)) return false;
                    onCursor.Position = new Point(5, 0);
                }
                else
                {
                    if (PlayerHasEquipped(EquippedSlot.RHandAlt)) return false;
                    onCursor.Position = new Point(12, 0);
                }

                onCursor.Parent = D2RMuleLib.Parent.Equipped;
                onCursor.Stash = Stash.None;
                onCursor.CharacterName = this.characterFiles[currentlyDisplayed].CharacterName;
                this.characterFiles[currentlyDisplayed].playerItems.items.Add(onCursor);
                this.characterFiles[currentlyDisplayed].Modified = true;
                PlayItemSound(onCursor);
                onCursor = new Item();
                itemPickedFrom = DisplaySection.None;
                return true;
            }

            // Not an equipped slot, was this a click within the item grid?
            if (!this.inventoryItemGrid.ContainsPoint(e))
                return false;

            // User clicked in the inventory grid.  If the item is dropped right here, would it collide with
            // any other item?

            // First, determine what grid space was clicked on
            int gridX = (int)(((e.Location.X / this.ratio) - this.inventoryItemGrid.Location.X) / GRID_SIZE_PX);
            int gridY = (int)(((e.Location.Y / this.ratio) - this.inventoryItemGrid.Location.Y) / GRID_SIZE_PX);

            // Now determine the bounding rectangle that goes with the item if it were to be dropped right here
            Rectangle rect = new Rectangle(this.inventoryItemGrid.X + gridX * GRID_SIZE_PX,
                                           this.inventoryItemGrid.Y + gridY * GRID_SIZE_PX,
                                           onCursor.Size.Width * GRID_SIZE_PX,
                                           onCursor.Size.Height * GRID_SIZE_PX);

            // Does this rectangle collide with any other?
            foreach (ItemRect ir in this.rightPanelRects)
            {
                if (rect.IntersectsWith(ir.rect))
                    return false;
            }

            // Ok now check if it would go outside the bounds of the item grid
            if (gridX + onCursor.Size.Width > 10 || gridY + onCursor.Size.Height > 4)
                return false;

            // If execution gets here, the item does not collide with anything in the grid and it does
            // not exceed the boundaries of the grid, it can be dropped here.  This grid spot is used as
            // the upper left of the item.  The item is added to the player's item list
            onCursor.Position = new Point(gridX, gridY);
            onCursor.Parent = D2RMuleLib.Parent.Stored;
            onCursor.Stash = Stash.Inventory;
            onCursor.CharacterName = this.characterFiles[currentlyDisplayed].CharacterName;
            this.characterFiles[currentlyDisplayed].playerItems.items.Add(onCursor);
            this.characterFiles[currentlyDisplayed].Modified = true;
            PlayItemSound(onCursor);
            onCursor = new Item();
            itemPickedFrom = DisplaySection.None;
            return true;
        }


        bool HandleRightSideClickWithItemOnCursor(MouseEventArgs e)
        {
            switch (rightPanelDisplay)
            {
                case DisplaySection.Inventory:
                    return HandleInventoryClickWithItemOnCursor_Player(e);
                case DisplaySection.Mercenary:
                    return HandleInventoryClickWithItemOnCursor_Merc(e);
                case DisplaySection.Cube:
                    return HandleInventoryClickWithItemOnCursor_Cube(e);
            }

            return false;
        }

        bool HandleLeftSideClickWithItemOnCursor(MouseEventArgs e)
        {
            bool isExpansion = this.characterFiles[currentlyDisplayed].isExpansion;

            // Was this click within the stash grid?
            if (isExpansion && !this.stashItemGrid.ContainsPoint(e))
                return false;
            if (!isExpansion && !this.stashClassicItemGrid.ContainsPoint(e))
                return false;

            Rectangle stashItemGrid = this.stashItemGrid;
            Size stashSize = this.STASH_SIZE;
            if (!isExpansion)
            {
                stashItemGrid = this.stashClassicItemGrid;
                stashSize = CLASSIC_STASH_SIZE;
            }


            // User clicked in the stash grid.  If the item is dropped right here, would it collide with
            // any other item?

            // First, determine what grid space was clicked on
            int gridX = (int)(((e.Location.X / this.ratio) - stashItemGrid.Location.X) / GRID_SIZE_PX);
            int gridY = (int)(((e.Location.Y / this.ratio) - stashItemGrid.Location.Y) / GRID_SIZE_PX);




            // Now determine the bounding rectangle that goes with the item if it were to be dropped right here
            Rectangle rect = new Rectangle(stashItemGrid.X + gridX * GRID_SIZE_PX,
                                           stashItemGrid.Y + gridY * GRID_SIZE_PX,
                                           onCursor.Size.Width * GRID_SIZE_PX,
                                           onCursor.Size.Height * GRID_SIZE_PX);

            // Does this rectangle collide with any other?
            foreach (ItemRect ir in this.leftPanelRects)
            {
                if (rect.IntersectsWith(ir.rect))
                    return false;
            }

            // Ok now check if it would go outside the bounds of the item grid
            if (gridX + onCursor.Size.Width > stashSize.Width || gridY + onCursor.Size.Height > stashSize.Height)
                return false;

            // If execution gets here, the item does not collide with anything in the grid and it does
            // not exceed the boundaries of the grid, it can be dropped here.  This grid spot is used as
            // the upper left of the item.  The item is added to the player's item list
            onCursor.Position = new Point(gridX, gridY);
            onCursor.Parent = D2RMuleLib.Parent.Stored;
            onCursor.Stash = Stash.Stash;
            onCursor.CharacterName = this.characterFiles[currentlyDisplayed].CharacterName; ;
            this.characterFiles[currentlyDisplayed].playerItems.items.Add(onCursor);
            this.characterFiles[currentlyDisplayed].Modified = true;
            PlayItemSound(onCursor);
            onCursor = new Item();
            itemPickedFrom = DisplaySection.None;
            return true;
        }

        void PlayVaultSound()
        {
            SoundPlayer soundPlayer = new SoundPlayer("sounds/rare.wav");
            soundPlayer.Play();
        }

        void PlayPickupSound()
        {
            SoundPlayer soundPlayer = new SoundPlayer("sounds/pickup.wav");
            soundPlayer.Play();
        }

        void PlayFailSound()
        {
            SoundPlayer soundPlayer = new SoundPlayer("sounds/cursor_fail.wav");
            soundPlayer.Play();
        }

        void PlayButtonSound()
        {
            SoundPlayer soundPlayer = new SoundPlayer("sounds/windowopen.wav");
            soundPlayer.Play();
        }

        void PlayItemSound(Item item)
        {
            string soundFile = "herb.wav";

            if (item.isRune()) soundFile = "rune.wav";
            else if (item.isRing()) soundFile = "ring.wav";
            else if (item.isAmulet()) soundFile = "amulet.wav";
            else if (item.isBelt()) soundFile = "belt.wav";
            else if (item.isTome()) soundFile = "book.wav";
            else if (item.isBoots())
            {
                if (item.isChainBoots()) soundFile = "bootschain.wav";
                else if (item.isMetalBoots()) soundFile = "bootsmetal.wav";
                else soundFile = "boots.wav";
            }
            else if (item.isHelm())
            {
                if (item.isLightHelm()) soundFile = "cap.wav";
                else soundFile = "helm.wav";
            }
            else if (item.isArmor())
            {
                if (item.isLightArmor()) soundFile = "lightarmor.wav";
                else if (item.isChainArmor()) soundFile = "chainarmor.wav";
                else soundFile = "platearmor.wav";
            }
            else if (item.isCharm()) soundFile = "charm.wav";
            else if (item.isGem())
            {
                if (item.isSkull()) soundFile = "skull.wav";
                else soundFile = "gem.wav";
            }
            else if (item.isGloves())
            {
                if (item.isChainGloves()) soundFile = "gloveschain.wav";
                else if (item.isMetalGloves()) soundFile = "glovesmetal.wav";
                else soundFile = "gloves.wav";
            }
            else if (item.isWeapon())
            {
                if (item.isBow()) soundFile = "bow.wav";
                else if (item.isCrossbow()) soundFile = "xbow.wav";
                else if (item.isWand()) soundFile = "wand.wav";
                else if (item.isJavelin()) soundFile = "javelins.wav";
                else if (item.isStaff() || item.isSorceressOrb()) soundFile = "staff.wav";
                else if (item.isSword()) soundFile = "sword.wav";
                else if (item.isLargeMetalWeapon()) soundFile = "largemetalweapon.wav";
                else if (item.isSmallMetalWeapon()) soundFile = "smallmetalweapon.wav";
                else if (item.isWoodWeaponLarge()) soundFile = "woodweaponlarge.wav";
            }
            if (item.isJewel()) soundFile = "jewel.wav";
            if (item.isKey()) soundFile = "key.wav";
            else if (item.isShield())
            {
                if (item.isNecroShield()) soundFile = "head.wav";
                else if (item.isWoodenShield()) soundFile = "woodshield.wav";
                else soundFile = "metalshield.wav";
            }
            else if (item.isPotion())
            {
                soundFile = "potionui.wav";
            }
            else if (item.isQuiver()) soundFile = "quiver.wav";
            if (item.isScroll()) soundFile = "scroll.wav";

            SoundPlayer soundPlayer = new SoundPlayer("sounds/" + soundFile);

            soundPlayer.Play();
        }



        bool clickedInRegion(MouseEventArgs e, PictureBox pb, Rectangle rect)
        {
            Point actualClickedPoint = new Point((int)Math.Round((double)e.Location.X / ratio),
                                                 (int)Math.Round((double)e.Location.Y / ratio));
            return rect.Contains(actualClickedPoint);
        }


        void HandleMouseMove(PictureBox pb, List<ItemRect> rects, MouseEventArgs e)
        {
            // NOTE: Setting a tooltip will re-trigger the mouse move even if the mouse hasn't moved.  This
            // can lead to a endless loop so we track both the mouse location and the last hovered item
            // to ensure we only change the tooltip if the mouse has moved and the item it's over has changed
            if (e.Location == prevMouseLoc)
                return;
            prevMouseLoc = e.Location;

            // The mouse has moved.  Is it over any items?
            foreach (ItemRect rect in this.leftPanelRects)
            {
                // Is the cursor over this item?
                if (!rect.rect.ContainsPoint(e))
                    continue;

                // Was the cursor over this item last time? If so, the tooltip doesn't need updating.
                if (hoveredItem == rect.Item)
                    return;

                // Update the tooltip
                tt.SetToolTip(pb, rect.Item.HoverText());
                hoveredItem = rect.Item;
                tooltipEmpty = false;
                return;
            }

            foreach (ItemRect rect in this.rightPanelRects)
            {
                // Is the cursor over this item?
                if (!rect.rect.ContainsPoint(e))
                    continue;

                // Was the cursor over this item last time? If so, the tooltip doesn't need updating.
                if (hoveredItem == rect.Item)
                    return;

                // Update the tooltip
                tt.SetToolTip(pb, rect.Item.HoverText());
                hoveredItem = rect.Item;
                tooltipEmpty = false;
                return;
            }

            // The cursor is over empty space or not even over the item grid.  Clear the tooltip only if it
            // has not already been cleared.

            if (tooltipEmpty)
                return;

            tooltipEmpty = true;
            hoveredItem = new Item();
            tt.SetToolTip(pb, "");
        }

        private void SaveModifiedFiles()
        {
            // Save any files that were modified
            foreach (string filename in this.characterFiles.Keys)
            {
                if (this.characterFiles[filename].Modified)
                {
                    this.characterFiles[filename].Save(filename);

                    // Reset modification flags
                    this.characterFiles[filename].Modified = false;
                    foreach (Item i in this.characterFiles[filename].playerItems.items)
                        i.Modified = false;
                    foreach (Item i in this.characterFiles[filename].mercItems.items)
                        i.Modified = false;
                }
            }
        }

        string GenerateVaultFilename()
        {

            string modeSuffix = "normal";
            if (this.radioButtonHardcore.Checked) modeSuffix = "hc";
            string gameSuffix = "_lod";
            if (this.radioButtonClassic.Checked) gameSuffix = "_classic";
            return "vault/" + modeSuffix + gameSuffix + ".vault";
        }

        private void LoadVault()
        {
            string vaultName = GenerateVaultFilename();
            this.vaultItems = new Items("__VAULT__", Items.ItemsType.Player);

            if (!File.Exists(vaultName))
                return;

            using (FileStream fileStream = new System.IO.FileStream(vaultName, FileMode.Open, FileAccess.Read))
            using (BinaryReader binReader = new System.IO.BinaryReader(fileStream, Encoding.ASCII))
            {
                this.vaultItems = new Items("__VAULT__", binReader, Items.ItemsType.Player, true);
                if (this.vaultItems.ErrorString != "")
                    MessageBox.Show("WARNING: " + this.vaultItems.ErrorString);
            }
        }
        private void SaveVault()
        {
            string vaultFilename = GenerateVaultFilename();

            if (vaultItems.items.Count == 0)
            {
                File.Delete(vaultFilename);
                return;
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                // The vault is just a normal player "item" section, but with item count being 4 bytes
                // instead of 2.  Chances of a vault having 65k items is low, but not zero, so I added the
                // special vault mode with a 32 bit count.
                vaultItems.magic = 0x4d4a;
                vaultItems.Save(memoryStream, true);
                memoryStream.Seek(0, SeekOrigin.Begin);
                using (FileStream fileStream = File.Create(vaultFilename))
                    memoryStream.CopyTo(fileStream);
            }
        }

        private void textBoxVaultFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.vaultCurrentDisplayIndex = 0;
                RefreshDisplayedItems();
            }
        }

        private void FormInventory_ResizeEnd(object sender, EventArgs e)
        {
            if (!this.pictureBoxLeft.Enabled) return;

            this.pictureBoxLeft.Height = this.ClientSize.Height - this.pictureBoxLeft.Top - MARGIN;
            RefreshDisplayedItems();
            PlaceVaultFilterBox();
        }

        const int WM_SYSCOMMAND = 0x0112;
        const int SC_MAXIMIZE = 0xF030;
        const int SC_RESTORE = 0xF120;
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_SYSCOMMAND)
            {
                if (m.WParam == (IntPtr)SC_MAXIMIZE || m.WParam == (IntPtr)SC_RESTORE)
                {
                    //the window has been maximized
                    this.OnResizeEnd(EventArgs.Empty);
                }
            }
        }

        private void radioButtonHardcore_Click(object sender, EventArgs e)
        {
            if (itemPickedFrom != DisplaySection.None)
            {
                MessageBox.Show("Item on cursor, can't do this");
                return;
            }

            if (this.radioButtonHardcore.Checked == false)
            {
                this.pictureBoxLeft.Visible = false;
                this.textBoxVaultFilter.Visible = false;
                SaveVault();
                SaveModifiedFiles();
                this.radioButtonNormal.Checked = false;
                this.radioButtonHardcore.Checked = true;
                LoadVault();
                InitializeGUI();
            }
        }

        private void radioButtonNormal_Click(object sender, EventArgs e)
        {
            if (itemPickedFrom != DisplaySection.None)
            {
                MessageBox.Show("Item on cursor, can't do this");
                return;
            }

            if (this.radioButtonNormal.Checked == false)
            {
                this.pictureBoxLeft.Visible = false;
                this.textBoxVaultFilter.Visible = false;
                SaveVault();
                SaveModifiedFiles();
                this.radioButtonHardcore.Checked = false;
                this.radioButtonNormal.Checked = true;
                LoadVault();
                InitializeGUI();
            }
        }

        private void radioButtonExpansion_Click(object sender, EventArgs e)
        {
            
            if (itemPickedFrom != DisplaySection.None)
            {
                MessageBox.Show("Item on cursor, can't do this");
                return;
            }

            if (this.radioButtonExpansion.Checked == false)
            {
                this.pictureBoxLeft.Visible = false;
                this.textBoxVaultFilter.Visible = false;
                SaveVault();
                SaveModifiedFiles();
                this.radioButtonClassic.Checked = false;
                this.radioButtonExpansion.Checked = true;
                LoadVault();
                InitializeGUI();
            }
        }

        private void radioButtonClassic_Click(object sender, EventArgs e)
        {
            if (itemPickedFrom != DisplaySection.None)
            {
                MessageBox.Show("Item on cursor, can't do this");
                return;
            }

            if (this.radioButtonClassic.Checked == false)
            {
                this.pictureBoxLeft.Visible = false;
                this.textBoxVaultFilter.Visible = false;
                SaveVault();
                SaveModifiedFiles();
                this.radioButtonExpansion.Checked = false;
                this.radioButtonClassic.Checked = true;
                LoadVault();
                InitializeGUI();
            }
        }

        private void buttonChangeDirectory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;

            this.textBoxDirectory.Text = dlg.SelectedPath;

            InitializeGUI();
        }
    }

    public static class FormInventoryExtensions
    {
        public static void DrawImage(this Image bg, Image img, Point p)
        {
            using (Graphics g = Graphics.FromImage(bg))
            {
                g.DrawImage(img, p);
            }
        }

        public static void DrawImage(this Image bg, Image img)
        {
            bg.DrawImage(img, new Point(0, 0));
        }

        public static double ratio;

        public static bool ContainsPoint(this Rectangle rect, MouseEventArgs e)
        {
            return rect.ContainsPoint(e.Location);
        }
        public static bool ContainsPoint(this Rectangle rect, Point point)
        {
            Point actualClickedPoint = new Point((int)Math.Round((double)point.X / ratio),
                                 (int)Math.Round((double)point.Y / ratio));
            return rect.Contains(actualClickedPoint);
        }
    }

    public class D2RMuleSettings
    {
        public enum GameType
        {
            Classic,
            LordOfDestruction
        }

        public enum ModeType
        {
            Normal,
            Hardcore
        }

        public const string SAVE_FILE = "d2rmule_config.json";

        public string SaveDirectory { get; set; }
        public GameType Game { get; set; } = GameType.LordOfDestruction;
        public ModeType Mode { get; set; } = ModeType.Normal;

        public void Save()
        {
            string jsonString = JsonSerializer.Serialize(this);
            File.WriteAllText(SAVE_FILE, jsonString);
        }

        public static D2RMuleSettings Load()
        {
            // If the file exists, attempt to load it
            if (File.Exists(SAVE_FILE))
            {
                var settings = JsonSerializer.Deserialize<D2RMuleSettings>(File.ReadAllText(SAVE_FILE));

                if (settings == null)
                    throw new Exception("Unable to deserialize");

                return settings;
            }

            // If execution gets here, the configuration file does not exist.  Generate a default config.
            D2RMuleSettings s = new D2RMuleSettings();
            string dir = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) +
                @"\Saved Games\Diablo II Resurrected";
            if (!Directory.Exists(dir))
            {
                dir = Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            }
            s.SaveDirectory = dir;
            return s;
        }
    }
}
