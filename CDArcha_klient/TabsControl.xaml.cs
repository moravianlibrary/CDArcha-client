using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Linq;
using System.Reflection;
using System.Collections.Specialized;
using System.Security.Cryptography;
using WIA;
using DAP.Adorners;
using CDArcha_klient.Classes;
using Newtonsoft.Json;

namespace CDArcha_klient
{

    /// <summary>
    /// Interaction logic for TabsControl.xaml
    /// </summary>
    public partial class TabsControl : UserControl
    {
        #region attributes
        #region key binding commands
        RoutedCommand rotateLeftCommand = new RoutedCommand();
        RoutedCommand rotateRightCommand = new RoutedCommand();
        RoutedCommand rotate180Command = new RoutedCommand();
        RoutedCommand flipHorizontalCommand = new RoutedCommand();
        RoutedCommand cropCommand = new RoutedCommand();
        RoutedCommand deskewCommand = new RoutedCommand();
        #endregion

        // Only for debugging purposes
        //public static StringBuilder DEBUGLOG = new StringBuilder();

        // Metadata received from Z39.50 or X-Server
        private IList<Metadata> metadata;

        // Index of currently valid metadata from metadataList
        private int metadataIndex = 0;

        public GeneralRecord generalRecord;

        // Backup image for Ctrl+Z (Undo)
        private KeyValuePair<string, BitmapSource> backupImage = new KeyValuePair<string, BitmapSource>();

        private KeyValuePair<string, BitmapSource> redoImage = new KeyValuePair<string, BitmapSource>();

        private KeyValuePair<Guid, BitmapSource> workingImage = new KeyValuePair<Guid, BitmapSource>();

        // Used by sliders, because changing contrast or brightness is irreversible process
        private KeyValuePair<Guid, BitmapSource> sliderOriginalImage = new KeyValuePair<Guid, BitmapSource>();

        // GUID of Image that is currently selected
        private Guid selectedImageGuid;

        // GUID that corresponds to cover image
        private Guid coverGuid;

        // Dictionary containing GUID and file path of all loaded images
        private Dictionary<Guid, string> imagesFilePaths = new Dictionary<Guid, string>();

        // Dictionary containing GUID and dimensions of originals of all loaded images
        private Dictionary<Guid, Size> imagesOriginalSizes = new Dictionary<Guid, Size>();

        // Dictionary containing Guid of TOC image and its thumbnail with wrapping Grid
        private Dictionary<Guid, Grid> tocThumbnailGridsDictionary = new Dictionary<Guid, Grid>();

        // Dictionary containing Guid of AUTH image and its thumbnail with wrapping Grid
        private Dictionary<Guid, Grid> authThumbnailGridsDictionary = new Dictionary<Guid, Grid>();

        // Object responsible for cropping of images
        private CroppingAdorner cropper;

        // Chosen scanner device
        private Device activeScanner;

        // Background worker for downloading of metadata
        private BackgroundWorker metadataReceiverBackgroundWorker = new BackgroundWorker();

        // Background worker for uploading of metadata and media
        private BackgroundWorker uploaderBackgroundWorker = new BackgroundWorker();

        // Background worker for uploading of cover and toc images
        private BackgroundWorker uploaderCoverTocBackgroundWorker = new BackgroundWorker();

        // Background worker for downloading covers
        private BackgroundWorker okczCoverBackgroundWorker = new BackgroundWorker();

        // Background worker for checking hash and finishing upload
        private BackgroundWorker hashCheckBackgroundWorker = new BackgroundWorker();

        private Dictionary<string, byte[]> tocDescriptionsDictionaryToDelete;
        private Dictionary<string, byte[]> authDescriptionsDictionaryToDelete;
        private string coverFileNameToDelete;

        // WebClient for downloading of pdf version of toc
        private WebClient tocPdfWebClient = new WebClient();

        // Union tab showing
        private bool unionTabVisible = false;

        // Is input correct
        private bool inputCorrect = false;

        // Working with SaveSelected
        private bool saveSelectedMode = false;
        private int saveSelectedCount = 0;
        private Grid newestThumbnail;
        private Grid workingThumbnail;

        // Authority existence = authority could be scanned
        private bool authScannable = false;

        private static HttpClient httpClient = new HttpClient();

        // Posledni nactene pdf
        public string pdfFile = "";

        // Posledni ziskane mediaid pomocu /api/import
        public string workingMediaId = "";
        public string workingArchiveId = "";
        public string lastWorkingMediaId = "";

        // Posledni vypocteny hash
        public string tmpMediaHash = "";

        // Rychly ID vlozeneho media
        public string tmpQuickId = "";
        public bool tmpMediumReadProblem = false;
        public bool tmpForcedUpload = false;

        // Pocet pokusu o zjisteni kontrolniho souctu prave vlozeneho media na server
        public int hashCheckTryCount = 0;

        // Vybrane pismeno jednotky
        public static string selectedDriveName = "";

        // Posledne ziskane informace ohledne media ze serveru
        GetmediaJsonObject lastMediaInfo;

        // Posledne ziskane informace ohledne archivu (vcetne seznamu medii v archivu)
        List<JsonApiAllMedia> lastArchiveInfo;

        #endregion

        /// <summary>Constructor, creates new TabsControl based on given barcode</summary>
        /// <param name="barcode">barcode of the unit, that will be processed</param>
        public TabsControl(GeneralRecord record)
        {
            this.generalRecord = record;
            InitializeComponent();
            if (record is Monograph)
            {
                this.addUnionButton.IsEnabled = true;
                this.addUnionButton.Visibility = Visibility.Visible;

                this.partIsbnLabel.IsEnabled = true;
                this.partIsbnLabel.Visibility = Visibility.Visible;
                this.partIsbnTextBox.IsEnabled = true;
                this.partIsbnTextBox.Visibility = Visibility.Visible;
                this.partVolumeLabel.Visibility = Visibility.Hidden;
                this.partVolumeLabel.IsEnabled = false;
                this.partVolumeTextBox.Visibility = Visibility.Hidden;
                this.partVolumeTextBox.IsEnabled = false;
                this.partNumberLabel.Visibility = Visibility.Hidden;
                this.partNumberLabel.IsEnabled = false;
                this.partNumberTextBox.Visibility = Visibility.Hidden;
                this.partNumberTextBox.IsEnabled = false;
                this.partNameLabel.Visibility = Visibility.Hidden;
                this.partNameLabel.IsEnabled = false;
                this.partNameTextBox.Visibility = Visibility.Hidden;
                this.partNameTextBox.IsEnabled = false;
                // union record
                this.isbnLabel.IsEnabled = true;
                this.isbnLabel.Visibility = Visibility.Visible;
                this.isbnTextBox.IsEnabled = true;
                this.isbnTextBox.Visibility = Visibility.Visible;
                // part and union grid hidden by default
                this.gridIdentifiers.Visibility = Visibility.Hidden;
            }
            else if (record is Periodical)
            {
                this.addUnionButton.IsEnabled = false;
                this.addUnionButton.Visibility = Visibility.Collapsed;

                this.partIssnLabel.IsEnabled = true;
                this.partIssnLabel.Visibility = Visibility.Visible;
                this.partIssnTextBox.IsEnabled = true;
                this.partIssnTextBox.Visibility = Visibility.Visible;
                this.partVolumeLabel.Visibility = Visibility.Visible;
                this.partVolumeLabel.IsEnabled = true;
                this.partVolumeTextBox.Visibility = Visibility.Visible;
                this.partVolumeTextBox.IsEnabled = true;
                this.partNameTextBox.Visibility = Visibility.Hidden;
                this.partNameTextBox.IsEnabled = false;
                this.partNameLabel.Visibility = Visibility.Hidden;
                this.partNameLabel.IsEnabled = false;
                // part and union grid hidden by default
                this.gridIdentifiers.Visibility = Visibility.Hidden;

                //Tooltips
                this.partYearLabel.Content = "Rok skenovaného čísla";
                this.partYearTextBox.ToolTip = "Rok periodika";
                this.partNumberTextBox.ToolTip = "Číslo periodika";
                this.partVolumeTextBox.ToolTip = "Ročník (svazek)";
            }

            this.saveSelectedButton.Visibility = System.Windows.Visibility.Hidden;

            #region key binding commands initialization
            //rotateLeft
            CommandBinding cb = new CommandBinding(this.rotateLeftCommand, RotateLeftCommandExecuted, RotateLeftCommandCanExecute);
            this.CommandBindings.Add(cb);
            KeyGesture kg = new KeyGesture(Key.Left, ModifierKeys.Control);
            InputBinding ib = new InputBinding(this.rotateLeftCommand, kg);
            this.InputBindings.Add(ib);

            //rotateRight
            cb = new CommandBinding(this.rotateRightCommand, RotateRightCommandExecuted, RotateRightCommandCanExecute);
            this.CommandBindings.Add(cb);
            kg = new KeyGesture(Key.Right, ModifierKeys.Control);
            ib = new InputBinding(this.rotateRightCommand, kg);
            this.InputBindings.Add(ib);

            //rotate180
            cb = new CommandBinding(this.rotate180Command, Rotate180CommandExecuted, Rotate180CommandCanExecute);
            this.CommandBindings.Add(cb);
            kg = new KeyGesture(Key.R, ModifierKeys.Control);
            ib = new InputBinding(this.rotate180Command, kg);
            this.InputBindings.Add(ib);

            //flipHorizontal
            cb = new CommandBinding(this.flipHorizontalCommand, FlipHorizontalCommandExecuted, FlipHorizontalCommandCanExecute);
            this.CommandBindings.Add(cb);
            kg = new KeyGesture(Key.H, ModifierKeys.Control);
            ib = new InputBinding(this.flipHorizontalCommand, kg);
            this.InputBindings.Add(ib);

            //crop
            cb = new CommandBinding(this.cropCommand, CropCommandExecuted, CropCommandCanExecute);
            this.CommandBindings.Add(cb);
            kg = new KeyGesture(Key.C, ModifierKeys.Control);
            ib = new InputBinding(this.cropCommand, kg);
            this.InputBindings.Add(ib);

            //deskew
            cb = new CommandBinding(this.deskewCommand, DeskewCommandExecuted, DeskewCommandCanExecute);
            this.CommandBindings.Add(cb);
            kg = new KeyGesture(Key.D, ModifierKeys.Control);
            ib = new InputBinding(this.deskewCommand, kg);
            this.InputBindings.Add(ib);
            #endregion

            InitializeBackgroundWorkers();

            // ziskej seznam mechanik
            metadataReceiverBackgroundWorker.RunWorkerAsync(record);
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            ComboBox driveList = this.driveList;
            int firstDrive = 0;
            int i = 0;
            // floppy,cd,dvd a usb vloz do zoznamu
            foreach (System.IO.DriveInfo drive in drives)
            {
                if (drive.DriveType == System.IO.DriveType.CDRom || drive.DriveType == System.IO.DriveType.Removable)
                {
                    ComboboxItem item = new ComboboxItem();
                    item.Text = drive.Name;
                    item.Value = drive.Name;
                    driveList.Items.Add(item);
                    if (drive.DriveType == System.IO.DriveType.CDRom && firstDrive == 0)
                    {
                        firstDrive = i;
                    }
                    i++;
                }
            }
            driveList.SelectedIndex = firstDrive;
        }

        #region key bindings

        //rotateLeft
        private void RotateLeftCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void RotateLeftCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TabItem currentTab = tabControl.SelectedItem as TabItem;
            if (currentTab.Equals(this.scanningTabItem))
            {
                RotateLeft_Clicked(null, null);
            }
        }

        //rotateRight
        private void RotateRightCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void RotateRightCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TabItem currentTab = tabControl.SelectedItem as TabItem;
            if (currentTab.Equals(this.scanningTabItem))
            {
                RotateRight_Clicked(null, null);
            }
        }

        //rotate180
        private void Rotate180CommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void Rotate180CommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TabItem currentTab = tabControl.SelectedItem as TabItem;
            if (currentTab.Equals(this.scanningTabItem))
            {
                Rotate180_Clicked(null, null);
            }
        }

        //flipHorizontal
        private void FlipHorizontalCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void FlipHorizontalCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TabItem currentTab = tabControl.SelectedItem as TabItem;
            if (currentTab.Equals(this.scanningTabItem))
            {
                Flip_Clicked(null, null);
            }
        }

        //crop
        private void CropCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void CropCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TabItem currentTab = tabControl.SelectedItem as TabItem;
            if (currentTab.Equals(this.scanningTabItem))
            {
                Crop_Clicked(null, null);
            }
        }

        //deskew
        private void DeskewCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void DeskewCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            TabItem currentTab = tabControl.SelectedItem as TabItem;
            if (currentTab.Equals(this.scanningTabItem))
            {
                Deskew_Clicked(null, null);
            }
        }
        #endregion

        #region metadata tab controls

        // Shows all available metadata in new MetadataWindow
        private void showCompleteMetadataButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO - ak su metadata len jedny, preskocit
            if (this.metadata != null)
            {
                MetadataWindow metadataWindow = new MetadataWindow(this.metadata, this.metadataIndex);
                metadataWindow.ShowDialog();

                if (metadataWindow.DialogResult.HasValue && metadataWindow.DialogResult.Value)
                {
                    this.metadataIndex = metadataWindow.MetadataIndex;
                    int idx = metadataWindow.reorder[this.metadataIndex];
                    this.generalRecord.ImportFromMetadata(this.metadata[idx]);
                    FillTextboxes();
                }
            }
        }

        // Downloads metadata and cover and toc images
        private void DownloadMetadataButton_Click(object sender, RoutedEventArgs e)
        {
            this.downloadMetadataButton.IsEnabled = false;
            (Window.GetWindow(this) as MainWindow).AddMessageToStatusBar("Stahuji metadata.");
            this.metadataReceiverBackgroundWorker.RunWorkerAsync(this.generalRecord);
        }

        // On doubleclick, downloads pdf with toc and opens it in default viewer
        private void OriginalTocImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (!this.tocPdfWebClient.IsBusy && this.generalRecord.OriginalTocPdfLink != null)
                {
                    MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                    mainWindow.AddMessageToStatusBar("Stahuji pdf obsahu.");
                    using (tocPdfWebClient)
                    {
                        tocPdfWebClient.DownloadFileCompleted += new AsyncCompletedEventHandler(TocPdfDownloadCompleted);
                        tocPdfWebClient.DownloadFileAsync(new Uri(this.generalRecord.OriginalTocPdfLink), Settings.TemporaryFolder.TrimEnd('\\')
                            + "\\" + "orig_toc.pdf");
                    }
                }
            }
        }

        #region AsyncMethods

        // Sets background worker for receiving of metadata
        private void InitializeBackgroundWorkers()
        {
            uploaderBackgroundWorker.WorkerReportsProgress = false;
            uploaderBackgroundWorker.WorkerSupportsCancellation = true;
            uploaderBackgroundWorker.DoWork += new DoWorkEventHandler(UploaderBW_DoWork);
            uploaderBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UploaderBW_RunWorkerCompleted);

            uploaderCoverTocBackgroundWorker.WorkerReportsProgress = false;
            uploaderCoverTocBackgroundWorker.WorkerSupportsCancellation = true;
            uploaderCoverTocBackgroundWorker.DoWork += new DoWorkEventHandler(UploaderCoverTocBW_DoWork);
            uploaderCoverTocBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UploaderCoverTocBW_RunWorkerCompleted);

            metadataReceiverBackgroundWorker.WorkerSupportsCancellation = true;
            metadataReceiverBackgroundWorker.WorkerReportsProgress = false;
            metadataReceiverBackgroundWorker.DoWork += new DoWorkEventHandler(MetadataReceiverBW_DoWork);
            metadataReceiverBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(MetadataReceiverBW_RunWorkerCompleted);

            okczCoverBackgroundWorker.WorkerSupportsCancellation = false;
            okczCoverBackgroundWorker.WorkerReportsProgress = false;
            okczCoverBackgroundWorker.DoWork += new DoWorkEventHandler(RetrieveOriginalCoverAndTocInformation);
            okczCoverBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RetrieveOriginalCoverAndTocInformation_Completed);

            hashCheckBackgroundWorker.WorkerSupportsCancellation = true;
            hashCheckBackgroundWorker.DoWork += new DoWorkEventHandler(bgCheckHash_DoWork);
        }

        // Starts retrieving of metadata on background
        private void MetadataReceiverBW_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            GeneralRecord generalRecord = e.Argument as GeneralRecord;

            IList<Metadata> metadataList = MetadataRetriever.RetrieveMetadata(generalRecord);
            e.Result = metadataList;
        }

        // Called after worker ended job, shows status with which worker ended
        private void MetadataReceiverBW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow).RemoveMessageFromStatusBar("Stahuji metadata.");
            this.downloadMetadataButton.IsEnabled = true;
            if (e.Error != null)
            {
                MessageBoxDialogWindow.Show("Chyba při stahování metadat", e.Error.Message,
                    "OK", MessageBoxDialogWindow.Icons.Error);
            }
            else if (!e.Cancelled)
            {
                List<Metadata> metadataList = e.Result as List<Metadata>;
                if (metadataList != null)
                {
                    if (this.generalRecord is Monograph && (this.generalRecord as Monograph).IsUnionRequested)
                    {
                        SetUnionMetadataTab(true, metadataList);
                    }
                    else
                    {
                        this.metadata = metadataList;
                        int idx = 0;

                        // show metadata window if multiple records fetched
                        if (metadataList.Count > 1)
                        {
                            var metadataIndex = (this.generalRecord is Monograph) ? 0 : metadataList.Count - 1;
                            MetadataWindow metadataWindow = new MetadataWindow(metadataList, metadataIndex);
                            metadataWindow.ShowDialog();
                            if (metadataWindow.DialogResult.HasValue && metadataWindow.DialogResult.Value)
                            {
                                this.metadata = metadataList;
                                this.metadataIndex = metadataWindow.MetadataIndex;
                                idx = metadataWindow.reorder[this.metadataIndex];
                            }
                        }

                        this.generalRecord.ImportFromMetadata(this.metadata[idx]);
                        if ((this.generalRecord is Monograph) && (this.generalRecord as Monograph).HasIssn)
                        {
                            MessageBoxDialogWindow.Show("Špatný vstup", "Chystáte se skenovat novou monografii, ale v záznamu byl nalezen identifikátor ISSN. \nProsím vyhledejte záznam jako periodikum (zmáčkněte CTRL+N a v oknu zvolte záložku PERIODIKUM).",
                                "OK", MessageBoxDialogWindow.Icons.Error);
                        }

                        FillTextboxes();
                        this.showCompleteMetadataButton.IsEnabled = true;

                        if (this.generalRecord is Monograph)
                        {
                            // show union tab
                            if ((this.generalRecord as Monograph).listIdentifiers.Count > 2) showUnionTab();
                        }

                        // load archive content into tree view
                        reloadArchiveList_Click(null, null);
                    }
                }
            }
        }

        private void RetrieveOriginalCoverAndTocInformation(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            GeneralRecord record = e.Argument as GeneralRecord;

            this.authScannable = false;

            if (record == null)
            {
                return;
            }

            // union record has no cover or toc data
            if ((record is Monograph) && (record as Monograph).IsUnionRequested)
            {
                return;
            }

            RequestObject3 requestObject = new RequestObject3();

            if (record is Monograph)
            {
                var tmpRecord = record as Monograph;
                requestObject.isbn = string.IsNullOrWhiteSpace(tmpRecord.PartIsbn) ? null : tmpRecord.PartIsbn;
                requestObject.ismn = (string.IsNullOrWhiteSpace(tmpRecord.PartIsmn)) ? null : tmpRecord.PartIsmn;
                requestObject.oclc = (string.IsNullOrWhiteSpace(tmpRecord.PartOclc)) ? null : tmpRecord.PartOclc;
                if (!string.IsNullOrWhiteSpace(tmpRecord.PartCnb))
                {
                    requestObject.nbn = tmpRecord.PartCnb;
                }
                else if (!string.IsNullOrWhiteSpace(tmpRecord.PartUrn))
                {
                    requestObject.nbn = tmpRecord.PartUrn;
                }
                else if (!string.IsNullOrWhiteSpace(tmpRecord.PartCustom))
                {
                    requestObject.nbn = Settings.Sigla + "-" + tmpRecord.PartCustom;
                }
                requestObject.part_no = (string.IsNullOrWhiteSpace(record.PartNo)) ? null : record.PartNo;
                requestObject.part_name = (string.IsNullOrWhiteSpace(record.PartName)) ? null : record.PartName;
            }
            if (record is Periodical)
            {
                requestObject.isbn = (string.IsNullOrWhiteSpace(((Periodical)record).Issn)) ? null : ((Periodical)record).Issn;
                requestObject.ismn = (string.IsNullOrWhiteSpace(record.Ismn)) ? null : record.Ismn;
                requestObject.oclc = (string.IsNullOrWhiteSpace(record.Oclc)) ? null : record.Oclc;
                if (!string.IsNullOrWhiteSpace(record.Cnb))
                {
                    requestObject.nbn = record.Cnb;
                }
                else if (!string.IsNullOrWhiteSpace(record.Urn))
                {
                    requestObject.nbn = record.Urn;
                }
                else if (!string.IsNullOrWhiteSpace(record.Custom))
                {
                    requestObject.nbn = Settings.Sigla + "-" + record.Custom;
                }
                requestObject.part_year = (string.IsNullOrWhiteSpace(record.PartYear)) ? null : record.PartYear;
                requestObject.part_no = (string.IsNullOrWhiteSpace(record.PartNo)) ? null : record.PartNo;
                requestObject.part_volume = (string.IsNullOrWhiteSpace((record as Periodical).PartVolume)) ? null : (record as Periodical).PartVolume;
            }

            string jsonData = JsonConvert.SerializeObject(requestObject, Formatting.None,
            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            //http://www.obalkyknih.cz/api/books?books=[{"bibinfo":{"isbn":"9770231582002","part_year":"1976","part_volume":"1976","part_no":"2"}}]
            string urlParams = Uri.EscapeDataString(jsonData);
            //urlParams += "&type=medium&encsigla=" + Uri.EscapeDataString(EcryptSigla(Settings.Sigla));

            /*string urlCover = "http://cache.obalkyknih.cz/api/cover?multi=" + urlParams;
            string urlToc = "http://cache.obalkyknih.cz/api/toc/thumbnail?multi=" + urlParams;
            string urlPdf = "http://cache.obalkyknih.cz/api/toc/pdf?multi=" + urlParams;

            record.OriginalCoverImageLink = urlCover;
            record.OriginalTocThumbnailLink = urlToc;
            record.OriginalTocPdfLink = urlPdf;*/

            string urlString = "http://www.obalkyknih.cz/api/books?books=[{\"bibinfo\":" + jsonData + "}]";

            using (WebClient webClient = new WebClient())
            {
                webClient.Headers.Add("Referer", Settings.Z39ServerUrl ?? Settings.XServerUrl);
                Stream stream = webClient.OpenRead(urlString);
                StreamReader reader = new StreamReader(stream);
                string responseJson = reader.ReadToEnd();
                char[] endTrimChars = { '\n', ')', ']', ';' };
                //remove unwanted characters, from beginning remove string "obalky.callback([" and from end, it should remove string "]);\n"
                responseJson = responseJson.Replace("obalky.callback([", "").TrimEnd(endTrimChars);
                ResponseObject responseObject = JsonConvert.DeserializeObject<ResponseObject>(responseJson);
                if (responseObject != null)
                {
                    //assign values
                    record.OriginalCoverImageLink = responseObject.cover_medium_url;
                    record.OriginalTocThumbnailLink = responseObject.toc_thumbnail_url;
                    record.OriginalTocPdfLink = responseObject.toc_pdf_url;

                    e.Result = record;
                }
            }
        }

        private void RetrieveOriginalCoverAndTocInformation_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null && !e.Cancelled)
            {
                GeneralRecord generalRecord = e.Result as GeneralRecord;
                if (generalRecord != null)
                {

                    if (generalRecord.OriginalCoverImageLink != null)
                    {
                        if ((Window.GetWindow(this) as MainWindow) != null) (Window.GetWindow(this) as MainWindow).AddMessageToStatusBar("Stahuji obálku.");
                        using (WebClient coverWc = new WebClient())
                        {
                            coverWc.OpenReadCompleted += new OpenReadCompletedEventHandler(CoverDownloadCompleted);
                            coverWc.OpenReadAsync(new Uri(generalRecord.OriginalCoverImageLink));
                        }
                    }
                    else
                    {
                        this.originalCoverImage.Source = getIconSource("CDArcha_klient;component/Images/ok-empty.png");
                    }

                    if (generalRecord.OriginalTocThumbnailLink != null)
                    {
                        if ((Window.GetWindow(this) as MainWindow) != null) (Window.GetWindow(this) as MainWindow).AddMessageToStatusBar("Stahuji obsah.");
                        using (WebClient tocWc = new WebClient())
                        {
                            tocWc.OpenReadCompleted += new OpenReadCompletedEventHandler(TocDownloadCompleted);
                            tocWc.OpenReadAsync(new Uri(generalRecord.OriginalTocThumbnailLink));
                        }
                    }
                    else
                    {
                        this.originalTocImage.Source = getIconSource("CDArcha_klient;component/Images/ok-empty.png");
                    }
                }
                else
                {
                    this.originalCoverImage.Source = getIconSource("CDArcha_klient;component/Images/ok-empty.png");
                    this.originalTocImage.Source = getIconSource("CDArcha_klient;component/Images/ok-empty.png");
                }
            }
        }

        // Downloads cover and toc images
        private void DownloadCoverAndToc()
        {
            FillGeneralRecord();
            if (this.generalRecord is Periodical)
            {
                if (this.generalRecord.PartNo == null &&
                    (this.generalRecord.PartYear == null && (this.generalRecord as Periodical).PartVolume == null))
                {
                    MessageBoxDialogWindow.Show("Varování!", "Musíte vyplnit rok a číslo nebo ročník a číslo",
                        "OK", MessageBoxDialogWindow.Icons.Warning);
                    return;
                }
            }

            if (!okczCoverBackgroundWorker.IsBusy)
            {
                okczCoverBackgroundWorker.RunWorkerAsync(this.generalRecord);
            }
        }

        // Actions after cover image was downloaded - shows image
        void CoverDownloadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            //if user created new record
            if (Window.GetWindow(this) == null)
            {
                return;
            }

            (Window.GetWindow(this) as MainWindow).RemoveMessageFromStatusBar("Stahuji obálku.");
            if (e.Error == null && !e.Cancelled)
            {
                BitmapImage imgsrc = new BitmapImage();
                imgsrc.BeginInit();
                imgsrc.StreamSource = e.Result;
                imgsrc.EndInit();
                this.originalCoverImage.Source = imgsrc;
                this.expanderArchStep1.IsExpanded = true;
            }
        }

        // Actions after toc image was downloaded - shows image
        void TocDownloadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            //if user created new record
            if (Window.GetWindow(this) == null)
            {
                return;
            }
            MainWindow win = (Window.GetWindow(this) as MainWindow);
            if (win.IsInitialized) win.RemoveMessageFromStatusBar("Stahuji obsah.");
            if (e.Error == null && !e.Cancelled)
            {
                BitmapImage imgsrc = new BitmapImage();
                imgsrc.BeginInit();
                imgsrc.StreamSource = e.Result;
                imgsrc.EndInit();
                this.originalTocImage.Source = imgsrc;
                this.originalTocImage.IsEnabled = true;
                this.expanderArchStep1.IsExpanded = true;
            }
        }

        // Actions after pdf file was downloaded - opens it in default viewer
        void TocPdfDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            (Window.GetWindow(this) as MainWindow).RemoveMessageFromStatusBar("Stahuji pdf obsahu.");

            if (e.Error == null && !e.Cancelled)
            {
                System.Diagnostics.Process.Start(Settings.TemporaryFolder + @"orig_toc.pdf");
            }
            else if (e.Error != null && e.Error.GetBaseException() is WebException)
            {
                WebException we = (WebException)e.Error;
                HttpWebResponse response = (System.Net.HttpWebResponse)we.Response;
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    MessageBoxDialogWindow.Show("Chyba!", "Vybrané PDF neexistuje", "OK", MessageBoxDialogWindow.Icons.Error);
                }
            }
        }
        #endregion

        #region Metadata validation

        // Sets metadata from textBoxes to general record and starts validation
        private void FillGeneralRecord()
        {
            //title
            if (this.generalRecord is Monograph)
            {
                (this.generalRecord as Monograph).PartTitle =
                    string.IsNullOrWhiteSpace(this.partTitleTextBox.Text) ? null : this.partTitleTextBox.Text;
                this.generalRecord.Title =
                    string.IsNullOrWhiteSpace(this.titleTextBox.Text) ? null : this.titleTextBox.Text;
            }
            else
            {
                this.generalRecord.Title =
                    string.IsNullOrWhiteSpace(this.partTitleTextBox.Text) ? null : this.partTitleTextBox.Text;
            }
            //authors
            if (this.generalRecord is Monograph)
            {
                (this.generalRecord as Monograph).PartAuthors =
                    string.IsNullOrWhiteSpace(this.partAuthorTextBox.Text) ? null : this.partAuthorTextBox.Text;
                this.generalRecord.Authors =
                    string.IsNullOrWhiteSpace(this.authorTextBox.Text) ? null : this.authorTextBox.Text;
            }
            else
            {
                this.generalRecord.Authors =
                    string.IsNullOrWhiteSpace(this.authorTextBox.Text) ? null : this.authorTextBox.Text;
            }
            //year
            this.generalRecord.PartYear =
                string.IsNullOrWhiteSpace(this.partYearTextBox.Text) ? null : this.partYearTextBox.Text;
            this.generalRecord.Year =
                string.IsNullOrWhiteSpace(this.yearTextBox.Text) ? null : this.yearTextBox.Text;
            //volume
            if (this.generalRecord is Periodical)
            {
                (this.generalRecord as Periodical).PartVolume =
                    string.IsNullOrWhiteSpace(this.partVolumeTextBox.Text) ? null : this.partVolumeTextBox.Text;
            }
            //number
            this.generalRecord.PartNo =
                string.IsNullOrWhiteSpace(this.partNumberTextBox.Text) ? null : this.partNumberTextBox.Text;

            this.generalRecord.PartName =
                string.IsNullOrWhiteSpace(this.partNameTextBox.Text) ? null : this.partNameTextBox.Text;
            //isbn and issn
            if (this.generalRecord is Monograph)
            {
                (this.generalRecord as Monograph).PartIsbn =
                    string.IsNullOrWhiteSpace(this.partIsbnTextBox.Text) ? null : this.partIsbnTextBox.Text;
                (this.generalRecord as Monograph).Isbn =
                    string.IsNullOrWhiteSpace(this.isbnTextBox.Text) ? null : this.isbnTextBox.Text;
            }
            else
            {
                (this.generalRecord as Periodical).Issn =
                    string.IsNullOrWhiteSpace(this.partIssnTextBox.Text) ? null : this.partIssnTextBox.Text;
            }
            //cnb
            if (this.generalRecord is Monograph)
            {
                (this.generalRecord as Monograph).PartCnb =
                    string.IsNullOrWhiteSpace(this.partCnbTextBox.Text) ? null : this.partCnbTextBox.Text;
            }
            this.generalRecord.Cnb =
                string.IsNullOrWhiteSpace(this.cnbTextBox.Text) ? null : this.cnbTextBox.Text;
            //ismn
            if (this.generalRecord is Monograph)
            {
                (this.generalRecord as Monograph).PartIsmn =
                    string.IsNullOrWhiteSpace(this.partIsmnTextBox.Text) ? null : this.partIsmnTextBox.Text;
            }
            this.generalRecord.Ismn =
                string.IsNullOrWhiteSpace(this.ismnTextBox.Text) ? null : this.ismnTextBox.Text;
            //oclc
            if (this.generalRecord is Monograph)
            {
                (this.generalRecord as Monograph).PartOclc =
                    string.IsNullOrWhiteSpace(this.partOclcTextBox.Text) ? null : this.partOclcTextBox.Text;
            }
            this.generalRecord.Oclc =
                string.IsNullOrWhiteSpace(this.oclcTextBox.Text) ? null : this.oclcTextBox.Text;
            //urn
            if (this.generalRecord is Monograph)
            {
                (this.generalRecord as Monograph).PartUrn =
                    string.IsNullOrWhiteSpace(this.partUrnNbnTextBox.Text) ? null : this.partUrnNbnTextBox.Text;
            }
            this.generalRecord.Urn =
                string.IsNullOrWhiteSpace(this.urnNbnTextBox.Text) ? null : this.urnNbnTextBox.Text;
            //custom
            if (this.generalRecord is Monograph)
            {
                (this.generalRecord as Monograph).PartCustom =
                    string.IsNullOrWhiteSpace(this.partSiglaTextBox.Text) ? null : this.partSiglaTextBox.Text;
            }
            this.generalRecord.Custom =
                string.IsNullOrWhiteSpace(this.siglaTextBox.Text) ? null : this.siglaTextBox.Text;
        }

        // Sets metadata from metadata object to textBoxes and starts validation
        private void FillTextboxes()
        {
            GeneralRecord record = this.generalRecord;
            if (this.generalRecord is Periodical)
            {
                this.partTitleTextBox.Text = string.IsNullOrWhiteSpace(record.Title) ? "" : record.Title.Trim();
                this.partAuthorTextBox.Text = string.IsNullOrWhiteSpace(record.Authors) ? "" : record.Authors.Trim();
                this.partYearTextBox.Text = DateTime.Now.Year.ToString();
                this.yearTextBox.Text = record.Year;
                this.partVolumeTextBox.Text = (record as Periodical).PartVolume;
                this.partNumberTextBox.Text = record.PartNo;
                this.partNameTextBox.Text = "";
                this.partIssnTextBox.Text = (record as Periodical).Issn;
                this.partCnbTextBox.Text = record.Cnb;
                this.partOclcTextBox.Text = record.Oclc;
                this.partIsbnTextBox.Text = record.Ean;
                this.partIsmnTextBox.Text = record.Ismn;
                this.partUrnNbnTextBox.Text = record.Urn;
                // hide optional fields
                this.partCnbTextBox.Visibility = Visibility.Hidden;
                this.partOclcTextBox.Visibility = Visibility.Hidden;
                this.partIsmnTextBox.Visibility = Visibility.Hidden;
                this.partUrnNbnTextBox.Visibility = Visibility.Hidden;
                this.partCnbLabel.Visibility = Visibility.Hidden;
                this.partOclcLabel.Visibility = Visibility.Hidden;
                this.partIsbnLabel.Visibility = Visibility.Hidden;
                this.partIsmnLabel.Visibility = Visibility.Hidden;
                this.partUrnNbnLabel.Visibility = Visibility.Hidden;
                this.optionalAtributesLink.Visibility = Visibility.Visible;
            }
            else if (this.generalRecord is Monograph && !(this.generalRecord as Monograph).IsUnionRequested)
            {
                var tmpRecord = record as Monograph;
                this.partTitleTextBox.Text = string.IsNullOrWhiteSpace(tmpRecord.PartTitle) ? "" : tmpRecord.PartTitle.Trim();
                this.partAuthorTextBox.Text = string.IsNullOrWhiteSpace(tmpRecord.PartAuthors) ? "" : tmpRecord.PartAuthors.Trim();
                string year = tmpRecord.PartYear;
                year = year + ".";
                if (!char.IsDigit(year[0])) year = year.Substring(1);
                if (year.Length > 0 && !char.IsDigit(year[year.Length - 1])) year = year.Substring(0, year.Length - 1);
                this.partYearTextBox.Text = year;
                //this.partNumberTextBox.Text = tmpRecord.PartNo; //na zadost p.Zahorika automaticky nedoplnovat
                //this.partNameTextBox.Text = tmpRecord.PartName;
                this.partIsbnTextBox.Text = tmpRecord.PartIsbn;
                if (string.IsNullOrWhiteSpace(this.partIsbnTextBox.Text)) this.partIsbnTextBox.Text = tmpRecord.PartEan;
                this.partCnbTextBox.Text = tmpRecord.PartCnb;
                this.partOclcTextBox.Text = tmpRecord.PartOclc;
                this.partIsmnTextBox.Text = tmpRecord.PartIsmn;
                this.partUrnNbnTextBox.Text = tmpRecord.PartUrn;
                // show optional fields
                this.partCnbTextBox.Visibility = Visibility.Visible;
                this.partOclcTextBox.Visibility = Visibility.Visible;
                this.partIsmnTextBox.Visibility = Visibility.Visible;
                this.partUrnNbnTextBox.Visibility = Visibility.Visible;
                this.partCnbLabel.Visibility = Visibility.Visible;
                this.partOclcLabel.Visibility = Visibility.Visible;
                this.partIsbnLabel.Visibility = Visibility.Visible;
                this.partIsmnLabel.Visibility = Visibility.Visible;
                this.partUrnNbnLabel.Visibility = Visibility.Visible;
                this.optionalAtributesLink.Visibility = Visibility.Hidden;

                // multipart monography
                if (tmpRecord.listIdentifiers.Count > 2)
                {
                    this.gridIdentifiers.Visibility = Visibility.Visible;
                    ComboboxItem emptyItem = new ComboboxItem();
                    emptyItem.Text = "";
                    emptyItem.Value = null;

                    int unionComboIndex = -1;
                    int partComboIndex = -1;
                    int j = 0;
                    this.multipartIdentifierOwn.Items.Clear();
                    this.multipartIdentifierUnion.Items.Clear();
                    foreach (MetadataIdentifier identifier in tmpRecord.listIdentifiers)
                    {
                        ComboboxItem item = new ComboboxItem();
                        item.Text = identifier.IdentifierCode + " " + identifier.IdentifierDescription;
                        item.Value = identifier.IdentifierCode;
                        this.multipartIdentifierOwn.Items.Add(j == 0 ? emptyItem : item); // zero index have empty text item
                        this.multipartIdentifierUnion.Items.Add(item); // zero index have item "nebyl nalezen ISBN souborného záznamu"
                        if (tmpRecord.PartIsbn == identifier.IdentifierCode && partComboIndex == -1) partComboIndex = j;
                        if (tmpRecord.Isbn == identifier.IdentifierCode && unionComboIndex == -1) unionComboIndex = j;
                        j++;
                    }

                    // pokud je jako skenovany dokument zvoleny prvni zaznam, musi byt
                    if (partComboIndex == unionComboIndex) unionComboIndex = 0;

                    this.multipartIdentifierOwn.SelectedIndex = partComboIndex == -1 ? 1 : partComboIndex;
                    this.multipartIdentifierUnion.SelectedIndex = unionComboIndex;

                    // multipart monography visual workaround
                    this.partNumberLabel.Visibility = Visibility.Visible;
                    this.partNumberLabel.IsEnabled = true;
                    this.partNumberTextBox.Visibility = Visibility.Visible;
                    this.partNumberTextBox.IsEnabled = true;
                    this.partNameLabel.Visibility = Visibility.Visible;
                    this.partNameLabel.IsEnabled = true;
                    this.partNameTextBox.Visibility = Visibility.Visible;
                    this.partNameTextBox.IsEnabled = true;
                    this.gridCover.VerticalAlignment = VerticalAlignment.Top;
                    this.gridToc.VerticalAlignment = VerticalAlignment.Top;

                    // fill part/union texts
                    //if (partComboIndex > -1) this.partNameTextBox.Text = tmpRecord.listIdentifiers[partComboIndex].IdentifierDescription;
                    if (unionComboIndex > -1) this.isbnTextBox.Text = tmpRecord.listIdentifiers[unionComboIndex].IdentifierCode;
                    this.titleTextBox.Text = tmpRecord.PartTitle;
                    this.authorTextBox.Text = tmpRecord.PartAuthors;
                    this.yearTextBox.Text = tmpRecord.PartYear;
                    this.cnbTextBox.Text = tmpRecord.PartCnb;
                    this.oclcTextBox.Text = tmpRecord.PartOclc;
                    this.isbnTextBox.Text = tmpRecord.PartIsbn;
                    this.ismnTextBox.Text = tmpRecord.PartIsmn;
                    this.urnNbnTextBox.Text = tmpRecord.PartUrn;

                    //is minor by default
                    bool minorIsChecked = tmpRecord.listIdentifiers.Count > 2 ? true : false;
                    this.checkboxMinorPartName.IsChecked = minorIsChecked;
                    if (minorIsChecked)
                    {
                        this.partCnbTextBox.Text = "";
                        this.partOclcTextBox.Text = "";
                    }
                    else
                    {
                        this.partCnbTextBox.Text = this.cnbTextBox.Text;
                        this.partOclcTextBox.Text = this.oclcTextBox.Text;
                        this.cnbTextBox.Text = "";
                        this.oclcTextBox.Text = "";
                    }
                    this.partIsbnTextBox.Text = "";
                    this.partIsmnTextBox.Text = "";
                    this.partUrnNbnTextBox.Text = "";
                }
                else
                {
                    this.gridCover.VerticalAlignment = VerticalAlignment.Top;
                    this.gridToc.VerticalAlignment = VerticalAlignment.Top;
                }
            }
            else
            {
                this.titleTextBox.Text = record.Title;
                this.authorTextBox.Text = record.Authors;
                this.yearTextBox.Text = record.Year;
                this.isbnTextBox.Text = (record as Monograph).Isbn;
                this.oclcTextBox.Text = record.Oclc;
                this.cnbTextBox.Text = record.Cnb;
                this.urnNbnTextBox.Text = record.Urn;
            }
            // if all identifiers are empty, fill custom id
            if ((this.generalRecord is Periodical || !(this.generalRecord as Monograph).IsUnionRequested)
                && string.IsNullOrWhiteSpace(this.partIsbnTextBox.Text)
                && string.IsNullOrWhiteSpace(this.partIssnTextBox.Text)
                && string.IsNullOrWhiteSpace(this.partCnbTextBox.Text)
                && string.IsNullOrWhiteSpace(this.partOclcTextBox.Text)
                && string.IsNullOrWhiteSpace(this.partIsbnTextBox.Text)
                && string.IsNullOrWhiteSpace(this.partIsmnTextBox.Text)
                && string.IsNullOrWhiteSpace(this.partUrnNbnTextBox.Text)
                && !string.IsNullOrWhiteSpace(record.Custom))
            {
                this.partSiglaTextBox.Text = record.Custom;

                if (!Settings.DisableCustomIdentifierNotification)
                {
                    bool dontAskAgain;
                    MessageBoxDialogWindow.Show("Chybějící identifikátor",
                        "Záznam nemá žádný unikátní identifikátor, proto mu byl vytvořen jeden spojením báze a systémového čísla.\n"
                        + "Prosím zkontrolujte, zda je v poli 'Vlastní' opravdu uvedena správná báze a systémové číslo.\n"
                        + "Sigla bude AUTOMATICKY připojena při odeslání, nevyplňujte ji.\n"
                        + "Finální identifikátor při odeslání: " + Settings.Sigla + "-" + partSiglaTextBox.Text, out dontAskAgain,
                        "Příště nezobrazovat", "OK", MessageBoxDialogWindow.Icons.Warning);

                    Settings.DisableCustomIdentifierNotification = dontAskAgain;
                }
            }

            // automaticka korekce vstupu
            Regex rgx = new Regex("\\((.*)\\)");
            this.partIsbnTextBox.Text = rgx.Replace(this.partIsbnTextBox.Text, "");
            this.isbnTextBox.Text = rgx.Replace(this.isbnTextBox.Text, "");
            rgx = new Regex("\\]\\-");
            // disabled
            /*
            this.partYearTextBox.Text = rgx.Replace(this.partYearTextBox.Text, "");
            this.yearTextBox.Text = rgx.Replace(this.yearTextBox.Text, "");
            rgx = new Regex("[\\]\\]\\?]");
            */

            ValidateIdentifiers(null, null);
        }

        // Validates identifiers, highlights errors
        private void ValidateIdentifiers(object sender, TextChangedEventArgs e)
        {
            bool warn = true;
            bool warnTmp = true;
            GeneralRecord record = this.generalRecord;
            bool isMono = this.generalRecord is Monograph ? true : false;
            bool isMonoSingle = (isMono && !this.unionTabVisible) ? true : false;
            bool isMonoPart = (isMono && this.unionTabVisible) ? true : false;
            bool isSerial = this.generalRecord is Periodical ? true : false;

            // set title to scanning tab
            //this.thumbnailsTitleLabel.Content = this.partTitleTextBox.Text;
            // year [0-9,-]
            warnTmp = ShowYearAndVolumeWarningControl(this.partYearTextBox, this.partYearWarning);
            warn = !warnTmp ? false : warn;
            // volume [0-9,-]
            warnTmp = ShowYearAndVolumeWarningControl(this.partVolumeTextBox, this.partVolumeWarning);
            warn = !warnTmp ? false : warn;
            // year SZ [0-9,-]
            warnTmp = ShowYearAndVolumeWarningControl(this.yearTextBox, this.yearWarning);
            warn = (!warnTmp && isMonoPart) ? false : warn;
            // part number and part name
            warnTmp = ShowPartnoPartnameWarningControl(this.partNumberTextBox, this.partNameTextBox, this.partNumberWarning);
            warn = (!warnTmp && (isMonoPart || isSerial)) ? false : warn;
            // ISBN - ISBN10 or ISBN13
            warnTmp = ShowIdentifierWarningControl(IdentifierType.ISBN);
            warn = !warnTmp ? false : warn;
            // ISSN - 7 numbers + checksum
            warnTmp = ShowIdentifierWarningControl(IdentifierType.ISSN);
            warn = !warnTmp ? false : warn;
            // ISMN - 10 chars, old format + 13 chars 979-x
            warnTmp = ShowIdentifierWarningControl(IdentifierType.ISMN);
            warn = !warnTmp ? false : warn;
            // CNB - cnb + 9 numbers
            warnTmp = ShowIdentifierWarningControl(IdentifierType.CNB);
            warn = !warnTmp ? false : warn;
            // OCLC - variable-length numeric string
            warnTmp = ShowIdentifierWarningControl(IdentifierType.OCLC);
            warn = !warnTmp ? false : warn;
            // URN - no validation
            warnTmp = ShowIdentifierWarningControl(IdentifierType.URN);
            warn = !warnTmp ? false : warn;
            // Custom - no validation
            warnTmp = ShowIdentifierWarningControl(IdentifierType.CUSTOM);
            warn = !warnTmp ? false : warn;

            this.inputCorrect = warn;
        }

        // Validate volume and year (only digits, coma and minus sign allowed)
        private bool ShowYearAndVolumeWarningControl(TextBox textbox, Image warning)
        {
            bool ret = true;
            const string colorBorderError = "#A50100";
            const string colorBorderNormal = "#111111";
            string partYear = this.partYearTextBox.Text;
            string partVol = this.partVolumeTextBox.Text;
            string partNo = this.partNumberTextBox.Text;

            if (this.generalRecord is Periodical && string.IsNullOrWhiteSpace(partYear) && string.IsNullOrWhiteSpace(partVol) && string.IsNullOrWhiteSpace(partNo))
            {
                warning.ToolTip = "Vyplňte prosím vstupní data rok, ročník, případně číslo";
                warning.Visibility = Visibility.Visible;
                textbox.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(colorBorderError));
                return false;
            }

            if (!string.IsNullOrWhiteSpace(textbox.Text) && textbox.Name == "partVolumeTextBox" && partVol == partYear)
            {
                // v pripade, ze je rok zadan i na miste rocniku soucasne, chceme pouze zapis u roku
                this.partYearTextBox.Text = this.partVolumeTextBox.Text;
                this.partVolumeTextBox.Text = "";
                MessageBoxDialogWindow.Show("Automatická oprava vstupních dat", "V případě stejného data v poli roku a ročníku periodika prosím nechte ročník periodika prázdný.",
                    "OK", MessageBoxDialogWindow.Icons.Information);
                ret = false;
            }

            if (!string.IsNullOrWhiteSpace(textbox.Text) && (
                (textbox.Name == "partYearTextBox" && (partYear == partVol || partYear == partNo)) ||
                (textbox.Name == "partVolumeTextBox" && (partVol == partYear))
               ))
            {
                warning.ToolTip = "Rok, ročník, nebo číslo se nesmí shodovat";
                warning.Visibility = Visibility.Visible;
                textbox.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(colorBorderError));
                ret = false;
            }
            else if (textbox.Text.All(c => (char.IsWhiteSpace(c) || char.IsDigit(c) || '-'.Equals(c) || ','.Equals(c) || '['.Equals(c) || ']'.Equals(c) || '('.Equals(c) || ')'.Equals(c))) ||
                textbox.Name == "partVolumeTextBox")
            {
                warning.ToolTip = null;
                warning.Visibility = Visibility.Hidden;
                textbox.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(colorBorderNormal));
            }
            else
            {
                warning.ToolTip = "Pole musí obsahovat jenom číslice, čárky a pomlčky";
                warning.Visibility = Visibility.Visible;
                textbox.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(colorBorderError));
                ret = false;
            }

            return ret;
        }

        private bool ShowPartnoPartnameWarningControl(TextBox partNo, TextBox partName, Image warning)
        {
            bool ret = true;
            string partYear = this.partYearTextBox.Text;
            string partVol = this.partVolumeTextBox.Text;
            const string colorBorderError = "#A50100";
            const string colorBorderNormal = "#111111";

            // kontrola na prazdny text
            warning.Visibility = (partNo.Text == "" && partName.Text == "") ? Visibility.Visible : Visibility.Hidden;
            if (!string.IsNullOrWhiteSpace(partNo.Text))
            {
                // kontrola na stejny text
                warning.Visibility = (partNo.Text == partName.Text) ? Visibility.Visible : Visibility.Hidden;
                ret = (partNo.Text == partName.Text) ? false : true;
            }

            if (this.generalRecord is Periodical && string.IsNullOrWhiteSpace(partYear) && string.IsNullOrWhiteSpace(partVol) && string.IsNullOrWhiteSpace(partNo.Text))
            {
                // pokud neni vyplneny rok, rocnik, ani cislo
                warning.ToolTip = "Vyplňte prosím vstupní data rok, ročník, případně číslo";
                warning.Visibility = Visibility.Visible;
                partNo.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(colorBorderError));
                ret = false;
            }
            else
            {
                // normalni stav, je vyplnene alespon jedno z rok|rocnik|cislo
                warning.Visibility = Visibility.Hidden;
                partNo.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(colorBorderNormal));
            }

            // v pripade, ze je rok zadan i na miste cisla chceme pouze zapis u roku
            if (!string.IsNullOrWhiteSpace(partNo.Text) && partYear == partNo.Text)
            {
                this.partYearTextBox.Text = this.partNumberTextBox.Text;
                this.partNumberTextBox.Text = "";
                MessageBoxDialogWindow.Show("Automatická oprava vstupních dat", "V případě stejného data v poli ročníku a čísla periodika prosím nechte číslo periodika prázdné.",
                    "OK", MessageBoxDialogWindow.Icons.Information);
            }

            return ret;
        }

        // Shows right control element (warning sign | left arrow | right arrow | double arrow | none)
        private bool ShowIdentifierWarningControl(IdentifierType identifierType)
        {
            bool ret = true;
            const string colorBorderError = "#A50100";
            const string colorBorderNormal = "#111111";
            const string imageWarning = "/CDArcha_klient;component/Images/ok-icon-warning.png";
            const string imageLeftArrow = "/CDArcha_klient;component/Images/left-line-icon.png";
            const string imageRightArrow = "/CDArcha_klient;component/Images/right-line-icon.png";
            const string imageDoubleArrow = "/CDArcha_klient;component/Images/double-line-icon.png";
            const string tooltipLeftArrow = "Skopírovat pole souborného záznamu do pole části";
            const string tooltipRightArrow = "Skopírovat pole části do pole souborného záznamu";
            const string tooltipDoubleArrow = "Vyměnit pole části s polem souborného záznamu";

            string partError = null;
            string unionError = null;
            Image partImage = new Image();
            Image unionImage = new Image();
            TextBox partTextBox = new TextBox();
            TextBox unionTextBox = new TextBox();

            switch (identifierType)
            {
                case IdentifierType.CNB:
                    partTextBox = this.partCnbTextBox;
                    partImage = this.partCnbWarning;
                    partError = ValidateCnb(partTextBox.Text);

                    unionTextBox = this.cnbTextBox;
                    unionImage = this.cnbWarning;
                    unionError = ValidateCnb(unionTextBox.Text);
                    if (partError != null || unionError != null) ret = false;
                    break;
                case IdentifierType.CUSTOM:
                    partTextBox = this.partSiglaTextBox;
                    partImage = this.partSiglaWarning;

                    unionTextBox = this.siglaTextBox;
                    break;
                case IdentifierType.ISMN:
                    partTextBox = this.partIsmnTextBox;
                    partImage = this.partIsmnWarning;
                    partError = ValidateIsmn(partTextBox.Text);
                    if (partError != null) ret = false;
                    break;
                case IdentifierType.ISBN:
                    partTextBox = this.partIsbnTextBox;
                    partImage = this.partIsbnWarning;
                    partError = ValidateIsbn(partTextBox.Text);

                    unionTextBox = this.isbnTextBox;
                    unionImage = this.isbnWarning;
                    unionError = ValidateIsbn(unionTextBox.Text);
                    if (partError != null || unionError != null) ret = false;
                    break;
                case IdentifierType.ISSN:
                    partTextBox = this.partIssnTextBox;
                    partImage = this.partIssnWarning;
                    partError = ValidateIssn(partTextBox.Text);
                    if (partError != null) ret = false;
                    break;
                case IdentifierType.OCLC:
                    partTextBox = this.partOclcTextBox;
                    partImage = this.partOclcWarning;
                    partError = ValidateOclc(partTextBox.Text);

                    unionTextBox = this.oclcTextBox;
                    unionImage = this.oclcWarning;
                    unionError = ValidateOclc(unionTextBox.Text);
                    if (partError != null || unionError != null) ret = false;
                    break;
                case IdentifierType.URN:
                    partTextBox = this.partUrnNbnTextBox;
                    partImage = this.partUrnWarning;

                    unionTextBox = this.urnNbnTextBox;
                    break;
                default:
                    throw new ArgumentException("Argument " + identifierType + " is not supported");
            }

            bool isUnionGridVisible = this.collectionRecordGrid.IsVisible;

            // validate partGrid textbox
            if (partError == null)
            {
                if (!isUnionGridVisible ||
                    (string.IsNullOrWhiteSpace(partTextBox.Text) && string.IsNullOrWhiteSpace(unionTextBox.Text)))
                {
                    // part identifier is correct; union grid is invisible or both textboxes are empty- don't show anything
                    partImage.ToolTip = null;
                    partImage.Visibility = Visibility.Hidden;
                    partTextBox.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(colorBorderNormal));
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(partTextBox.Text) && string.IsNullOrWhiteSpace(unionTextBox.Text))
                    {
                        // part identifier is correct; union grid is visible partTextBox is set and unionTextBox is empty
                        // - show right arrow
                        partImage.Source = new BitmapImage(new Uri(imageRightArrow, UriKind.Relative));
                        partImage.ToolTip = tooltipRightArrow;
                    }
                    else if (!string.IsNullOrWhiteSpace(partTextBox.Text) && string.IsNullOrWhiteSpace(unionTextBox.Text))
                    {
                        // part identifier is correct; union grid is visible partTextBox is empty and unionTextBox is set
                        // - show left arrow
                        partImage.Source = new BitmapImage(new Uri(imageLeftArrow, UriKind.Relative));
                        partImage.ToolTip = tooltipLeftArrow;
                    }
                    else
                    {
                        // part identifier is correct; union grid is visible both textboxes are set - show double arrow
                        partImage.Source = new BitmapImage(new Uri(imageDoubleArrow, UriKind.Relative));
                        partImage.ToolTip = tooltipDoubleArrow;
                    }
                    partImage.MouseLeftButtonDown -= PartImageWarning_Click;
                    partImage.MouseLeftButtonDown += PartImageWarning_Click;
                    partImage.Cursor = Cursors.Hand;
                    partImage.Visibility = Visibility.Visible;
                    partTextBox.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(colorBorderNormal));
                }
            }
            else
            {
                // part identifier is incorrect - show warning icon
                partImage.Source = new BitmapImage(
                     new Uri(imageWarning, UriKind.Relative));
                partImage.ToolTip = partError;
                partImage.Cursor = Cursors.Arrow;
                partImage.MouseLeftButtonDown -= PartImageWarning_Click;
                partImage.Visibility = Visibility.Visible;
                partTextBox.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(colorBorderError));
                ret = false;
            }

            //validate unionGrid textBox
            if (isUnionGridVisible)
            {
                if (unionError == null)
                {
                    unionImage.ToolTip = null;
                    unionImage.Visibility = Visibility.Hidden;
                    unionTextBox.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(colorBorderNormal));
                }
                else
                {
                    unionImage.Source = new BitmapImage(
                        new Uri(imageWarning, UriKind.Relative));
                    unionImage.ToolTip = unionError;
                    unionImage.Visibility = Visibility.Visible;
                    unionTextBox.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(colorBorderError));
                    ret = false;
                }
            }

            return ret;
        }

        // Validates given isbn, returns error message if invalid or null if valid
        private string ValidateIsbn(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
            {
                return null;
            }

            string errorText = null;
            isbn = isbn.Replace("-", "");// remove all dashes (minus signs)
            isbn = String.Join("", isbn.Where(c => !char.IsWhiteSpace(c)));// remove all white spaces
            char[] isbnArray = isbn.ToCharArray();
            switch (isbn.Length)
            {
                case 10:
                    int sumIsbn = 0;
                    for (int i = 0; i < 9; i++)
                    {
                        if (char.IsDigit(isbnArray[i]))
                        {
                            int multiplier = 10 - i;
                            sumIsbn += multiplier * ((int)char.GetNumericValue(isbnArray[i]));
                        }
                        else
                        {
                            errorText = "ISBN obsahuje nečíselný znak";
                            break;
                        }
                    }
                    int checksumIsbn = (char.ToLower(isbnArray[9]) == 'x') ? 10 : (int)char.GetNumericValue(isbnArray[9]);
                    sumIsbn += checksumIsbn;
                    if ((sumIsbn % 11) != 0)
                    {
                        errorText = "Nesedí kontrolní znak";
                    }
                    break;
                case 11: // UPC 11 (doplni se 2 nuly na zacatek)
                case 12: // UPC 12 (doplni se 1 nula na zacatek)
                case 13:
                    sumIsbn = 0;
                    int pos = 0;
                    for (int i = isbn.Length - 1; i >= 0; i--)
                    {
                        pos = isbn.Length - i;
                        if (char.IsDigit(isbnArray[i]))
                        {
                            sumIsbn += (int)char.GetNumericValue(isbnArray[i]) * ((pos % 2) == 0 ? 3 : 1);
                        }
                        else
                        {
                            errorText = "ISBN obsahuje nečíselný znak";
                            break;
                        }
                    }
                    if ((sumIsbn % 10) != 0)
                    {
                        errorText = "Nesedí kontrolní znak";
                    }
                    break;
                default:
                    errorText = "ISBN musí obsahovat 10 nebo 13 číslic. UPC musí obsahovat 11 nebo 12 číslic.";
                    break;
            }
            return errorText;
        }

        // Validates given issn, returns error message if invalid or null if valid
        private string ValidateIssn(string issn)
        {
            if (string.IsNullOrWhiteSpace(issn))
            {
                return null;
            }

            string errorText = null;
            issn = issn.Replace("-", "").Trim();
            issn = String.Join("", issn.Where(c => !char.IsWhiteSpace(c)));// remove all white spaces
            char[] issnArray = issn.ToCharArray();
            if (issn.Length == 8)
            {
                int sumIssn = 0;
                for (int i = 0; i < issnArray.Length - 1; i++)
                {
                    if (char.IsDigit(issnArray[i]))
                    {
                        int multiplier = 8 - i;
                        sumIssn += (int)char.GetNumericValue(issnArray[i]) * multiplier;
                    }
                    else
                    {
                        errorText = "ISSN obsahuje nečíselný znak";
                        break;
                    }
                }
                int checksumIssn = (char.ToLower(issnArray[7]) == 'x') ? 10 : (int)char.GetNumericValue(issnArray[7]);
                sumIssn += checksumIssn;
                if ((sumIssn % 11) != 0)
                {
                    errorText = "Nesedí kontrolní znak";
                }
            }
            else
            {
                errorText = "ISSN musí obsahovat 8 čísel";
            }

            return errorText;
        }

        // Validates given ismn, returns error message if invalid or null if valid
        private string ValidateIsmn(string ismn)
        {
            if (string.IsNullOrWhiteSpace(ismn))
            {
                return null;
            }

            ismn = String.Join("", ismn.Where(c => !char.IsWhiteSpace(c)));// remove all white spaces
            if (ismn.StartsWith("979"))
            {
                if (string.IsNullOrWhiteSpace(this.partIsbnTextBox.Text))
                {
                    this.partIsbnTextBox.Text = this.partIsmnTextBox.Text;
                    this.partIsmnTextBox.Text = "";
                    return null;
                }
                return "Identifikátory začínající na 979 prosím vložte do pole ISBN / EAN / UPC";
            }

            return null;
        }

        // Validates given oclc, returns error message or null
        private string ValidateOclc(string oclc)
        {
            if (string.IsNullOrWhiteSpace(oclc))
            {
                return null;
            }

            oclc = String.Join("", oclc.Where(c => !char.IsWhiteSpace(c)));// remove all white spaces
            if (!oclc.StartsWith("(OCoLC)"))
            {
                return "OCLC nezačíná znaky (OCoLC)";
            }

            oclc = oclc.Substring(7);
            long tmp;
            if (!long.TryParse(oclc, out tmp))
            {
                return "OCLC obsahuje za znaky (OCoLC) další nečíselné znaky";
            }

            return null;
        }

        // Validates given cnb, returns error message or null
        private string ValidateCnb(string cnb)
        {
            if (string.IsNullOrWhiteSpace(cnb))
            {
                return null;
            }

            string errorText = null;

            cnb = String.Join("", cnb.Where(c => !char.IsWhiteSpace(c)));// remove all white spaces
            if (cnb.StartsWith("cnb"))
            {
                string cnbTmp = cnb.Substring(3);
                int tmp;
                if (int.TryParse(cnbTmp, out tmp))
                {
                    if (cnbTmp.Length != 9)
                    {
                        errorText = "ČNB musí obsahovat za znaky cnb přesně 9 číslic";
                    }
                }
                else
                {
                    errorText = "ČNB obsahuje za znaky cnb nečíselné znaky";
                }
            }
            else
            {
                errorText = "ČNB nezačíná znaky cnb";
            }

            return errorText;
        }

        // partNo and partName can't be null
        private string ValidatePartMono(string partNo, string partName)
        {
            if (!string.IsNullOrWhiteSpace(partNo) && !string.IsNullOrWhiteSpace(partName))
            {
                return "Vyplňte prosím označení části, nebo číslo části.";
            }
            else if (partNo == partName)
            {
                return "Označení části a název části nesmí být shodné.";
            }

            return null;
        }

        // partYear, partVol, partNo validation
        private string ValidatePartSerial(string partYear, string partVol, string partNo)
        {
            if (string.IsNullOrWhiteSpace(partYear) && string.IsNullOrWhiteSpace(partVol) && string.IsNullOrWhiteSpace(partNo))
            {
                return "Vyplňte prosím rok+číslo, nebo ročník+číslo v případě periodika vycházejícího více krát v roce, nebo rok+ročník v případě ročenky.";
            }
            else if (!string.IsNullOrWhiteSpace(partYear) && !string.IsNullOrWhiteSpace(partVol) && partYear == partVol)
            {
                return "Označení roku a ročníku periodika nesmí být shodné.";
            }
            else if (!string.IsNullOrWhiteSpace(partYear) && !string.IsNullOrWhiteSpace(partNo) && partYear == partNo)
            {
                return "Označení roku a čísla periodika nesmí být shodné.";
            }
            else if (!string.IsNullOrWhiteSpace(partVol) && !string.IsNullOrWhiteSpace(partNo) && partVol == partNo && partVol.Length == 4)
            {
                return "Označení ročníku a čísla periodika nesmí být shodné. \nV případě, že se jedná a ročenku vyplňte pouze ročník a označení čísla periodika ponechte prázdné.";
            }

            return null;
        }

        // Validates given ean, returns error message or null
        private string ValidateEan(string ean)
        {
            if (string.IsNullOrWhiteSpace(ean))
            {
                return null;
            }

            string errorText = null;

            ean = ean.Replace("-", "").Trim();
            ean = String.Join("", ean.Where(c => !char.IsWhiteSpace(c))); // remove all white spaces
            if (ean.Length == 12) ean = "0" + ean; // UPC is part of EAN
            char[] eanArray = ean.ToCharArray();
            if (ean.Length == 13)
            {
                long tmp;
                if (!long.TryParse(ean, out tmp))
                {
                    errorText = "EAN obsahuje nečíselný znak";
                }
                else
                {
                    int sumEan = 0;
                    for (int i = 0; i < 13; i += 2)
                    {
                        sumEan += (int)char.GetNumericValue(eanArray[i]);
                    }
                    for (int i = 1; i < 12; i += 2)
                    {
                        sumEan += (int)char.GetNumericValue(eanArray[i]) * 3;
                    }
                    if ((sumEan % 10) != 0)
                    {
                        errorText = "Nesedí kontrolní znak";
                    }
                }
            }
            else
            {
                errorText = "EAN musí mít 13 číslic. UPC musí mít 12 číslic.";
            }

            return errorText;
        }
        #endregion
        #endregion

        #region sending to ObalkyKnih

        // Checks everything and calls uploadWorker to upload archive
        public void SendToObalkyKnih()
        {
            if (uploaderBackgroundWorker.IsBusy)
            {
                MessageBoxDialogWindow.Show("Odesílání dat", "Počkejte, než se dokončí předchozí odesílání.",
                    "OK", MessageBoxDialogWindow.Icons.Error);
                return;
            }

            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            if (string.IsNullOrWhiteSpace(Settings.UserName))
            {
                MessageBoxDialogWindow.Show("Žádné přihlašovací údaje", "Nastavte přihlašovací údaje.",
                    "OK", MessageBoxDialogWindow.Icons.Error);
                return;
            }

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                // progress // log
                stepIcon1.Visibility = Visibility.Visible;
                stepIcon1.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-hourglass.png");

                logTextBox.Document.Blocks.Clear();
                logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Kompletace identifikátorů záznamů.\r");

                this.scanningTabDoneIcon.Visibility = Visibility.Hidden;
                this.scanningStartButton.Content = "SKENOVAT";
                this.scanningStartButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF6C3131"));
            }));

            // download images of cover and toc
            this.DownloadCoverAndToc();

            //data type
            bool isMono = this.generalRecord is Monograph ? true : false;
            bool isMonoSingle = (isMono && !this.unionTabVisible) ? true : false;
            bool isMonoPart = (isMono && this.unionTabVisible) ? true : false;
            bool isSerial = this.generalRecord is Periodical ? true : false;
            //validate
            string error = "";
            string isbn = isMonoPart ? this.isbnTextBox.Text : this.partIsbnTextBox.Text;
            string issn = isSerial ? this.partIssnTextBox.Text : null;
            string partIsbn = isMonoPart ? this.partIsbnTextBox.Text : null;
            string oclc = isMonoPart ? this.oclcTextBox.Text : this.partOclcTextBox.Text;
            string partOclc = isMonoPart ? this.partOclcTextBox.Text : null;
            string ismn = isMonoPart ? this.ismnTextBox.Text : this.partIsmnTextBox.Text;
            string partIsmn = isMonoPart ? this.partIsmnTextBox.Text : null;
            string cnb = isMonoPart ? this.cnbTextBox.Text : this.partCnbTextBox.Text;
            string partCnb = isMonoPart ? this.partCnbTextBox.Text : null;
            string urn = isMonoPart ? this.urnNbnTextBox.Text : this.partUrnNbnTextBox.Text;
            string partUrn = isMonoPart ? this.partUrnNbnTextBox.Text : null;
            string custom = isMonoPart ? this.siglaTextBox.Text : this.partSiglaTextBox.Text;
            string partCustom = isMonoPart ? this.partSiglaTextBox.Text : null;
            string partNo = (isMonoPart || isSerial) ? this.partNumberTextBox.Text : null;
            string partName = isMonoPart ? this.partNameTextBox.Text : null;
            string partYear = isSerial ? this.partYearTextBox.Text : null;
            string partVol = isSerial ? this.partVolumeTextBox.Text : null;
            string mediaNo = this.mediaNoTextBox.Text;
            if (isMonoPart) error += ValidatePartMono(partNo, partName);
            if (isSerial) error += ValidatePartSerial(partYear, partVol, partNo);

            // get record idents NVC + errors list
            var identBuildTuple = this.identifiersBuild();
            error = identBuildTuple.Item1;
            NameValueCollection nvc = identBuildTuple.Item2;

            nvc.Add("login", Settings.UserName);
            nvc.Add("password", Settings.Password);
            nvc.Add("version", Settings.VersionInfo);
            if (!string.IsNullOrEmpty(mediaNo))
            {
                mediaNo = String.Join("", mediaNo.Where(c => !char.IsWhiteSpace(c)));// remove all white spaces
                nvc.Add("media_no", mediaNo);
            }
            else
            {
                error += "Je potřeba zdata označení média!";
            }

            if (!string.IsNullOrWhiteSpace(error))
            {
                MessageBoxDialogWindow.Show("Chybný identifikátor",
                    "Některý z identifikátorů obsahuje chybu." + Environment.NewLine + error,
                    "OK", MessageBoxDialogWindow.Icons.Error);
                return;
            }

            if (string.IsNullOrEmpty(isbn) && string.IsNullOrEmpty(issn) && string.IsNullOrEmpty(cnb)
                && string.IsNullOrEmpty(oclc) && string.IsNullOrEmpty(ismn) && string.IsNullOrEmpty(urn)
                && string.IsNullOrEmpty(custom))
            {
                MessageBoxDialogWindow.Show("Žádný identifikátor",
                    "Vyplňte alespoň jeden identifikátor." + Environment.NewLine + error,
                    "OK", MessageBoxDialogWindow.Icons.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(this.titleTextBox.Text) && string.IsNullOrWhiteSpace(this.partTitleTextBox.Text))
            {
                MessageBoxDialogWindow.Show("Žádný název",
                    "Název musí být vyplněn." + Environment.NewLine + error,
                    "OK", MessageBoxDialogWindow.Icons.Error);
                return;
            }

            if ((string.IsNullOrWhiteSpace(this.authorTextBox.Text) || string.IsNullOrWhiteSpace(this.yearTextBox.Text))
                && (string.IsNullOrWhiteSpace(this.partAuthorTextBox.Text) || string.IsNullOrWhiteSpace(this.partYearTextBox.Text))
                && !Settings.DisableMissingAuthorYearNotification)
            {
                bool dontShowAgain;
                var result = MessageBoxDialogWindow.Show("Chybí základní informace.",
                    "Chybí autor nebo rok vydání. Opravdu chcete odeslat obálku bez toho?"
                    + Environment.NewLine + error, out dontShowAgain, "Příště se neptat a odeslat",
                    "Ano", "Ne", false, MessageBoxDialogWindow.Icons.Warning);
                if (result != true)
                {
                    return;
                }
                if (dontShowAgain)
                {
                    Settings.DisableMissingAuthorYearNotification = true;
                }
            }

            string title = isMonoPart ? this.titleTextBox.Text : this.partTitleTextBox.Text;
            nvc.Add("title", title);
            nvc.Add("authors", (isMonoPart ? this.authorTextBox.Text : this.partAuthorTextBox.Text) ?? "");
            nvc.Add("year", this.yearTextBox.Text ?? "");
            nvc.Add("ocr", (this.ocrCheckBox.IsChecked == true) ? "yes" : "no");
            string partTitle = isMonoPart ? this.partTitleTextBox.Text : null;
            string partAuthor = isMonoPart ? this.partAuthorTextBox.Text : null;
            partYear = (isMonoPart || isSerial) ? this.partYearTextBox.Text : null;
            string partVolume = isSerial ? this.partVolumeTextBox.Text : null;
            if (partTitle != null) nvc.Add("part_title", partTitle);
            if (partAuthor != null) nvc.Add("part_authors", partAuthor);
            if (partYear != null) nvc.Add("part_year", partYear);
            if (partVolume != null) nvc.Add("part_volume", partVolume);
            if (partNo != null) nvc.Add("part_no", partNo);
            if (partName != null) nvc.Add("part_name", partName);

            // monography part, or serial identification
            if (isSerial) nvc.Add("part_type", "serial");
            if (isMonoPart) nvc.Add("part_type", "mono");

            string metaXml = null;

            //metastream
            try
            {
                IPHostEntry host;
                string localIPv4 = "";
                string localIPv6 = "";
                host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIPv4 = ip.ToString();
                    }
                    if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        localIPv6 = ip.ToString();
                    }
                }

                XNamespace mets = "http://www.loc.gov/METS/";
                XNamespace mix = "http://www.loc.gov/mix/v20";
                XElement metsRoot = new XElement("mets",
                    new XAttribute(XNamespace.Xmlns + "mets", mets),
                    new XAttribute("aes57", "http://www.aes.org/publications/standards/search.cfm?docID=84"),
                    new XAttribute("premis", "info:lc/xmlns/premis-v2"),
                    new XAttribute("xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                    new XAttribute("mods", "http://www.loc.gov/mods/v3"),
                    new XAttribute("oai_dc", "http://www.openarchives.org/OAI/2.0/oai_dc/"),
                    new XAttribute("dc", "http://purl.org/dc/elements/1.1/"),
                    new XAttribute("xlink", "http://www.w3.org/1999/xlink"),
                    new XAttribute("LABEL", title),
                    new XAttribute("schemaLocation", "http://www.w3.org/2001/XMLSchema-instance http://www.w3.org/2001/XMLSchema.xsd http://www.loc.gov/METS/ http://www.loc.gov/standards/mets/mets.xsd http://www.aes.org/standards/schemas/aes57-2011-08-27.xsd info:lc/xmlns/premis-v2 http://www.loc.gov/standards/premis/premis.xsd"),
                    new XElement(mets + "metsHdr",
                        new XAttribute("CREATEDATE", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")),
                        new XAttribute("ID", "###uuid###"),
                        new XElement(mets + "agent",
                            new XAttribute("ROLE", "CREATOR"),
                            new XAttribute("TYPE", "ORGANIZATION"),
                            new XElement(mets + "name", "###organization###")
                        )
                    ),
                    new XElement(mets + "structMap",
                        new XAttribute("TYPE", "PHYSICAL"),
                        new XElement(mets + "div",
                            new XAttribute("TYPE", "DATACOLLECTION"),
                            new XElement(mets + "fptr",
                                new XAttribute("FILEID", "###filename###")
                            )
                        )
                    ),
                    new XElement(mets + "mdWrap",
                        new XAttribute("MDTYPE", "NISOIMG"),
                        new XAttribute("MIMETYPE", "text/xml"),
                        new XElement(mets + "xmlData",
                            new XElement("mix",
                                new XAttribute(XNamespace.Xmlns + "mix", mix),
                                new XElement(mix + "user", Settings.UserName),
                                new XElement(mix + "sigla", Settings.Sigla),
                                new XElement(mix + "client",
                                    new XElement(mix + "name", "CDArcha_klient"),
                                    new XElement(mix + "version", Assembly.GetEntryAssembly().GetName().Version),
                                    new XElement(mix + "local-IPv4-address", localIPv4),
                                    new XElement(mix + "local-IPv6-address", localIPv6)
                                )
                            )
                        )
                    )
                );

                XDocument xmlDoc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"), metsRoot);
                metaXml = xmlDoc.ToString();
            }
            catch (Exception)
            {
                MessageBoxDialogWindow.Show("Chybný metasoubor", "Nastala chyba při tvorbě metasouboru.",
                        "OK", MessageBoxDialogWindow.Icons.Error);
                return;
            }

            UploadParameters param = new UploadParameters();
            param.Url = Settings.ImportLink;
            param.Nvc = nvc;

            param.MetaXml = metaXml;

            // transformace MARCXML na MODS
            MetadataRetriever.transformMARC2MODS();
            //string marcXmlPath = System.IO.Path.Combine(Settings.TmpDir, "marcxml.xml");
            string modsPath = System.IO.Path.Combine(Settings.TmpDir, "mods.xml");
            /*if (File.Exists(marcXmlPath))
            {
                param.MarcXml = File.ReadAllText(marcXmlPath);
            }*/
            if (File.Exists(modsPath))
            {
                param.MetaMods = File.ReadAllText(modsPath);
            }

            //DEBUGLOG.AppendLine("SendToObalkyKnih part1: Total time: " + sw.ElapsedMilliseconds);
            this.uploaderBackgroundWorker.RunWorkerAsync(param);

            controlTabItem.IsEnabled = true;
            tabControl.SelectedItem = controlTabItem;
        }


        private Tuple<string, NameValueCollection> identifiersBuild()
        {
            NameValueCollection nvc = new NameValueCollection();
            string error = "";
            bool isMono = this.generalRecord is Monograph ? true : false;
            bool isMonoSingle = (isMono && !this.unionTabVisible) ? true : false;
            bool isMonoPart = (isMono && this.unionTabVisible) ? true : false;
            bool isSerial = this.generalRecord is Periodical ? true : false;
            string isbn = isMonoPart ? this.isbnTextBox.Text : this.partIsbnTextBox.Text;
            string issn = isSerial ? this.partIssnTextBox.Text : null;
            string partIsbn = isMonoPart ? this.partIsbnTextBox.Text : null;
            string oclc = isMonoPart ? this.oclcTextBox.Text : this.partOclcTextBox.Text;
            string partOclc = isMonoPart ? this.partOclcTextBox.Text : null;
            string ismn = isMonoPart ? this.ismnTextBox.Text : this.partIsmnTextBox.Text;
            string partIsmn = isMonoPart ? this.partIsmnTextBox.Text : null;
            string cnb = isMonoPart ? this.cnbTextBox.Text : this.partCnbTextBox.Text;
            string partCnb = isMonoPart ? this.partCnbTextBox.Text : null;
            string urn = isMonoPart ? this.urnNbnTextBox.Text : this.partUrnNbnTextBox.Text;
            string partUrn = isMonoPart ? this.partUrnNbnTextBox.Text : null;
            string custom = isMonoPart ? this.siglaTextBox.Text : this.partSiglaTextBox.Text;
            string partCustom = isMonoPart ? this.partSiglaTextBox.Text : null;
            string partNo = (isMonoPart || isSerial) ? this.partNumberTextBox.Text : null;
            string partName = isMonoPart ? this.partNameTextBox.Text : null;
            string partYear = isSerial ? this.partYearTextBox.Text : null;
            string partVol = isSerial ? this.partVolumeTextBox.Text : null;

            if (!string.IsNullOrEmpty(isbn))
            {
                isbn = String.Join("", isbn.Where(c => !char.IsWhiteSpace(c)));// remove all white spaces
                nvc.Add("isbn", isbn);
                error += ValidateIsbn(isbn);
            }
            if (!string.IsNullOrEmpty(partIsbn) && isMono)
            {
                partIsbn = String.Join("", partIsbn.Where(c => !char.IsWhiteSpace(c)));// remove all white spaces
                nvc.Add("part_isbn", partIsbn);
                error += ValidateIsbn(partIsbn);
            }
            if (!string.IsNullOrEmpty(issn))
            {
                issn = String.Join("", issn.Where(c => !char.IsWhiteSpace(c)));// remove all white spaces
                nvc.Add("issn", issn);
                error += ValidateIssn(issn);
            }
            if (!string.IsNullOrEmpty(oclc))
            {
                oclc = String.Join("", oclc.Where(c => !char.IsWhiteSpace(c)));// remove all white spaces
                nvc.Add("oclc", oclc);
                error += ValidateOclc(oclc);
            }
            if (!string.IsNullOrEmpty(partOclc))
            {
                partOclc = String.Join("", partOclc.Where(c => !char.IsWhiteSpace(c)));// remove all white spaces
                nvc.Add("part_oclc", partOclc);
                error += ValidateOclc(partOclc);
            }
            if (!string.IsNullOrEmpty(ismn))
            {
                ismn = String.Join("", ismn.Where(c => !char.IsWhiteSpace(c)));// remove all white spaces
                nvc.Add("ismn", ismn);
            }
            if (!string.IsNullOrEmpty(partIsmn))
            {
                partIsmn = String.Join("", partIsmn.Where(c => !char.IsWhiteSpace(c)));// remove all white spaces
                nvc.Add("part_ismn", partIsmn);
            }
            if (!string.IsNullOrEmpty(cnb))
            {
                cnb = String.Join("", cnb.Where(c => !char.IsWhiteSpace(c)));// remove all white spaces
                nvc.Add("nbn", cnb);
                error += ValidateCnb(cnb);
            }
            if (!string.IsNullOrEmpty(partCnb))
            {
                partCnb = String.Join("", partCnb.Where(c => !char.IsWhiteSpace(c)));// remove all white spaces
                nvc.Add("part_nbn", partCnb);
                error += ValidateCnb(partCnb);
            }
            if (!string.IsNullOrEmpty(urn))
            {
                if (nvc.Get("nbn") == null)
                {
                    nvc.Add("nbn", urn);
                }
            }
            if (!string.IsNullOrEmpty(partUrn))
            {
                if (nvc.Get("part_nbn") == null)
                {
                    nvc.Add("part_nbn", partUrn);
                }
            }
            if (!string.IsNullOrEmpty(custom))
            {
                if (nvc.Get("nbn") == null)
                {
                    nvc.Add("nbn", Settings.Sigla + "-" + custom);
                }
            }
            if (!string.IsNullOrEmpty(partCustom))
            {
                if (nvc.Get("part_nbn") == null)
                {
                    nvc.Add("part_nbn", Settings.Sigla + "-" + partCustom);
                }
            }

            return Tuple.Create(error, nvc);
        }


        // Method for uploading multipart/form-data
        // url where will be data posted, login, password
        //private void UploadFilesToRemoteUrl(string url, string coverFileName, List<string> tocFileNames, List<string> authFileNames, string metaXml, NameValueCollection nvc, DoWorkEventArgs e)
        private void UploadFilesToRemoteUrl(string url, NameValueCollection nvc, string metaXml, string marcXml, string metaMods, DoWorkEventArgs e)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            // Check version
            /*
            UpdateChecker updateChecker = new UpdateChecker();
            updateChecker.RetrieveUpdateInfo();
            if (!updateChecker.IsSupportedVersion)
            {
                throw new WebException("Používáte nepodporovanou verzi programu. Aktualizujte ho.",
                    WebExceptionStatus.ProtocolError);
            }
            */

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Chystání metadatových souborů MARC XML a MODS.\r");
            }));

            try
            {
                HttpWebRequest requestToServer = (HttpWebRequest)WebRequest.Create(url + "?version=" + Assembly.GetEntryAssembly().GetName().Version + "&login=" + Settings.UserName + "&password=" + Settings.Password);

                requestToServer.Timeout = 10000;

                // Define a boundary string
                string boundaryString = "CDArchaFormBoundary" + DateTime.Now.Ticks.ToString("x");

                // Turn off the buffering of data to be written, to prevent OutOfMemoryException when sending data
                requestToServer.SendChunked = true;
                requestToServer.AllowWriteStreamBuffering = false;
                // Specify that request is a HTTP post
                requestToServer.Method = WebRequestMethods.Http.Post;
                // Specify that the content type is a multipart request
                requestToServer.ContentType = "multipart/form-data; boundary=" + boundaryString;

                UTF8Encoding utf8 = new UTF8Encoding();
                string boundaryStringLine = "--" + boundaryString + "\r\n";

                string lastBoundaryStringLine = "--" + boundaryString + "--";
                byte[] lastBoundaryStringLineBytes = utf8.GetBytes(lastBoundaryStringLine);


                // TEXT PARAMETERS
                string formDataString = "";
                foreach (string key in nvc.Keys)
                {
                    formDataString += boundaryStringLine
                        + String.Format(
                    "Content-Disposition: form-data; name=\"{0}\"; filename=\"{0}\"\r\nContent-Type: text/plain\r\n\r\n{1}\r\n",
                    key,
                    nvc[key]);
                }
                byte[] formDataBytes = utf8.GetBytes(formDataString);

                // META PARAMETER
                string metaDataString = boundaryStringLine
                    + String.Format(
                    "Content-Disposition: form-data; name=\"{0}\"; "
                    + "filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n",
                    "meta", "meta.xml", "text/xml")
                    + metaXml + "\r\n";
                byte[] metaDataBytes = utf8.GetBytes(metaDataString);

                // MARC XML
                string marcXmlString = boundaryStringLine
                    + String.Format(
                    "Content-Disposition: form-data; name=\"{0}\"; "
                    + "filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n",
                    "marcxml", "marcxml.xml", "text/xml")
                    + marcXml + "\r\n";
                byte[] marcXmlBytes = utf8.GetBytes(marcXmlString);

                // MODS
                string metaModsString = boundaryStringLine
                    + String.Format(
                    "Content-Disposition: form-data; name=\"{0}\"; "
                    + "filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n",
                    "mods", "mods.xml", "text/xml")
                    + metaMods + "\r\n";
                byte[] metaModsBytes = utf8.GetBytes(metaModsString);

                // Calculate the total size of the HTTP request
                long totalRequestBodySize =
                    +lastBoundaryStringLineBytes.Length
                    + formDataBytes.Length
                    //+ coverSize
                    //+ tocSize
                    + metaDataBytes.Length
                    + marcXmlBytes.Length
                    + metaModsBytes.Length;

                // And indicate the value as the HTTP request content length
                requestToServer.ContentLength = totalRequestBodySize;


                // Write the http request body directly to the server
                using (Stream s = requestToServer.GetRequestStream())
                {
                    // Send text parameters
                    s.Write(formDataBytes, 0, formDataBytes.Length);

                    // Send meta
                    s.Write(metaDataBytes, 0, metaDataBytes.Length);

                    // Send MarcXML
                    s.Write(marcXmlBytes, 0, marcXmlBytes.Length);

                    // Send MODS
                    s.Write(metaModsBytes, 0, metaModsBytes.Length);

                    // Send the last part of the HTTP request body
                    s.Write(lastBoundaryStringLineBytes, 0, lastBoundaryStringLineBytes.Length);
                }

                //DEBUGLOG.AppendLine("UploadFilesToRemoteUrl (upload data): Total time: " + sw.ElapsedMilliseconds);

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Zasílání identifikátorů a metadat.\r");
                }));

                WebResponse response = requestToServer.GetResponse();
                StreamReader responseReader = new StreamReader(response.GetResponseStream());
                e.Result = responseReader.ReadToEnd();
                response.Close();
                requestToServer.Abort();

                //DEBUGLOG.AppendLine("UploadFilesToRemoteUrl: Total time: " + sw.ElapsedMilliseconds);
            }
            catch (WebException ex) {
                HttpStatusCode wRespStatusCode = ((HttpWebResponse)ex.Response).StatusCode;
                if (wRespStatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    MessageBoxDialogWindow.Show("Odesílání dat", "API odmítá spojení. Pravděpodobně se nezhoduje verze aplikace s verzí API.",
                    "OK", MessageBoxDialogWindow.Icons.Error);
                }
                else if (wRespStatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    MessageBoxDialogWindow.Show("Odesílání dat", "Špatné přihlašovací jméno, nebo heslo.",
                    "OK", MessageBoxDialogWindow.Icons.Error);
                }
            }
        }

        // Checks everything and calls uploadCoverTocWorker to upload cover and toc
        public void SendCoverToc()
        {
            if (uploaderCoverTocBackgroundWorker.IsBusy)
            {
                MessageBoxDialogWindow.Show("Odesílání dat", "Počkejte, než se dokončí předchozí odesílání obálek.",
                    "OK", MessageBoxDialogWindow.Icons.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(Settings.UserName))
            {
                MessageBoxDialogWindow.Show("Žádné přihlašovací údaje", "Nastavte přihlašovací údaje.",
                    "OK", MessageBoxDialogWindow.Icons.Error);
                return;
            }

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                this.scanningTabDoneIcon.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-hourglass.png");
                this.scanningTabDoneIcon.Visibility = Visibility.Visible;
                this.scanningStartButton.Content = "Zasílání dat...";
                this.scanningStartButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFCCCCCC"));
                logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Kompletace dat pro odesílání obálek a bookletů.\r");
            }));

            //validate
            string error = "";

            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("login", Settings.UserName);
            nvc.Add("password", Settings.Password);
            nvc.Add("version", Settings.VersionInfo);
            if (!string.IsNullOrEmpty(this.lastWorkingMediaId))
            {
                nvc.Add("id", this.lastWorkingMediaId);
            }
            else
            {
                error += "Chybí identifikace média. Skenování je možné pouze v kontextu s procesem archivace! Ujištěte se, že probíhá archivace, nebo máte načtena data o archivu.";
            }

            if (!string.IsNullOrWhiteSpace(error))
            {
                MessageBoxDialogWindow.Show("Před odeslaním byl nalezen problém",
                    "Před odeslaním dat byl nalezen tento problém:" + Environment.NewLine + error,
                    "OK", MessageBoxDialogWindow.Icons.Error);
                return;
            }

            string coverFileName = (this.coverGuid == Guid.Empty) ? null : this.imagesFilePaths[this.coverGuid];
            List<string> tocFileNames = new List<string>();

            // Save working image in memory to file
            if (this.workingImage.Key != Guid.Empty &&
                this.imagesFilePaths.ContainsKey(this.workingImage.Key))
            {
                this.sendButton.IsEnabled = false;
                try
                {
                    ImageTools.SaveToFile(this.workingImage.Value, this.imagesFilePaths[this.workingImage.Key]);
                }
                catch (Exception)
                {
                    MessageBoxDialogWindow.Show("Chyba!", "Nastal problém při ukládání obrázku do souboru.", "OK",
                        MessageBoxDialogWindow.Icons.Error);
                    this.sendButton.IsEnabled = true;
                    return;
                }
                this.sendButton.IsEnabled = true;
            }

            // where to save local images copy
            String uploadDirName = DateTime.Now.ToString("yyyyMMdd");
            String mainIdentifier = DateTime.Now.ToString("HHmmss");
            uploadDirName = uploadDirName + '_' + mainIdentifier;

            String localUploadDir = Settings.ScanOutputDir;
            localUploadDir = System.IO.Path.Combine(localUploadDir, uploadDirName);
            if (Settings.EnableLocalImageCopy && !System.IO.Directory.Exists(localUploadDir))
            {
                System.IO.Directory.CreateDirectory(localUploadDir);
            }

            //cover
            if (this.coverGuid == Guid.Empty && !Settings.DisableWithoutCoverNotification)
            {
                bool dontShowAgain;
                var result = MessageBoxDialogWindow.Show("Chybí obálka.", "Opravdu chcete odeslat data bez obálky ?",
                    out dontShowAgain, "Příště se neptat a odeslat", "Ano", "Ne", false,
                    MessageBoxDialogWindow.Icons.Warning);
                if (result != true)
                {
                    return;
                }
                if (dontShowAgain)
                {
                    Settings.DisableWithoutCoverNotification = true;
                }
            }
            if (this.coverGuid != Guid.Empty && Settings.EnableLocalImageCopy)
            {
                System.IO.File.Copy(this.imagesFilePaths[this.coverGuid], System.IO.Path.Combine(localUploadDir, mainIdentifier + ".tiff"), true);
            }

            //toc
            if (!this.tocImagesList.HasItems)
            {
                if (!Settings.DisableWithoutTocNotification)
                {
                    bool dontShowAgain;
                    var result = MessageBoxDialogWindow.Show("Chybí obsah/booklet", "Opravdu chcete odeslat data bez obsahu/bookletu ?",
                        out dontShowAgain, "Příště se neptat a odeslat", "Ano", "Ne", false,
                        MessageBoxDialogWindow.Icons.Warning);
                    if (result != true)
                    {
                        return;
                    }
                    if (dontShowAgain)
                    {
                        Settings.DisableWithoutTocNotification = true;
                    }
                }
            }
            else
            {
                int cnt = 1;
                foreach (var grid in tocImagesList.Items)
                {
                    Guid guid = Guid.Empty;
                    foreach (var record in this.tocThumbnailGridsDictionary)
                    {
                        if (record.Value.Equals(grid))
                        {
                            tocFileNames.Add(this.imagesFilePaths[record.Key]);

                            if (Settings.EnableLocalImageCopy)
                            {
                                System.IO.File.Copy(this.imagesFilePaths[record.Key], System.IO.Path.Combine(localUploadDir, "toc-" + cnt.ToString().PadLeft(2, '0') + "-" + mainIdentifier + ".tiff"), true);
                                cnt++;
                            }
                        }
                    }
                }
            }

            string metaXml = null;

            //metastream
            try
            {
                IPHostEntry host;
                string localIPv4 = "";
                string localIPv6 = "";
                host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIPv4 = ip.ToString();
                    }
                    if (ip.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        localIPv6 = ip.ToString();
                    }
                }

                bool isMono = this.generalRecord is Monograph ? true : false;
                bool isMonoPart = (isMono && this.unionTabVisible) ? true : false;
                string title = isMonoPart ? this.titleTextBox.Text : this.partTitleTextBox.Text;

                XNamespace mets = "http://www.loc.gov/METS/";
                XNamespace mix = "http://www.loc.gov/mix/v20";
                XElement metsRoot = new XElement("mets",
                    new XAttribute(XNamespace.Xmlns + "mets", mets),
                    new XAttribute("aes57", "http://www.aes.org/publications/standards/search.cfm?docID=84"),
                    new XAttribute("premis", "info:lc/xmlns/premis-v2"),
                    new XAttribute("xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                    new XAttribute("mods", "http://www.loc.gov/mods/v3"),
                    new XAttribute("oai_dc", "http://www.openarchives.org/OAI/2.0/oai_dc/"),
                    new XAttribute("dc", "http://purl.org/dc/elements/1.1/"),
                    new XAttribute("xlink", "http://www.w3.org/1999/xlink"),
                    new XAttribute("LABEL", title),
                    new XAttribute("schemaLocation", "http://www.w3.org/2001/XMLSchema-instance http://www.w3.org/2001/XMLSchema.xsd http://www.loc.gov/METS/ http://www.loc.gov/standards/mets/mets.xsd http://www.aes.org/standards/schemas/aes57-2011-08-27.xsd info:lc/xmlns/premis-v2 http://www.loc.gov/standards/premis/premis.xsd"),
                    new XElement(mets + "metsHdr",
                        new XAttribute("CREATEDATE", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")),
                        new XAttribute("ID", "###uuid###"),
                        new XElement(mets + "agent",
                            new XAttribute("ROLE", "CREATOR"),
                            new XAttribute("TYPE", "ORGANIZATION"),
                            new XElement(mets + "name", "###organization###")
                        )
                    ),
                    new XElement(mets + "structMap",
                        new XAttribute("TYPE", "PHYSICAL"),
                        new XElement(mets + "div",
                            new XAttribute("TYPE", "DATACOLLECTION"),
                            new XElement(mets + "fptr",
                                new XAttribute("FILEID", "###filename###")
                            )
                        )
                    ),
                    new XElement(mets + "mdWrap",
                        new XAttribute("MDTYPE", "NISOIMG"),
                        new XAttribute("MIMETYPE", "text/xml"),
                        new XElement(mets + "xmlData",
                            new XElement("mix",
                                new XAttribute(XNamespace.Xmlns + "mix", mix),
                                new XElement(mix + "user", Settings.UserName),
                                new XElement(mix + "sigla", Settings.Sigla),
                                new XElement(mix + "client",
                                    new XElement(mix + "name", "CDArcha_klient"),
                                    new XElement(mix + "version", Assembly.GetEntryAssembly().GetName().Version),
                                    new XElement(mix + "local-IPv4-address", localIPv4),
                                    new XElement(mix + "local-IPv6-address", localIPv6)
                                )
                            )
                        )
                    )
                );

                XDocument xmlDoc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"), metsRoot);
                metaXml = xmlDoc.ToString();
            }
            catch (Exception)
            {
                MessageBoxDialogWindow.Show("Chybný metasoubor", "Nastala chyba při tvorbě metasouboru.",
                        "OK", MessageBoxDialogWindow.Icons.Error);
                return;
            }

            UploadParameters param = new UploadParameters();
            param.Url = Settings.UploadCoverLink;
            param.Nvc = nvc;
            param.MetaXml = metaXml;

            param.TocFilePaths = tocFileNames;
            param.CoverFilePath = coverFileName;

            //DEBUGLOG.AppendLine("SendToObalkyKnih part1: Total time: " + sw.ElapsedMilliseconds);
            this.uploaderCoverTocBackgroundWorker.RunWorkerAsync(param);

            controlTabItem.IsEnabled = true;
            tabControl.SelectedItem = controlTabItem;
        }

        // Method for uploading multipart/form-data
        // url where will be data posted, login, password
        private void UploadCoverTocFilesToRemoteUrl(string url, NameValueCollection nvc, string MetaXml, string coverFileName, List<string> tocFileNames, DoWorkEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Vytváří se multipart/form-data request pro zaslání obálky a obsahu/bookletu.\r");
            }));

            try
            {
                HttpWebRequest requestToServer = (HttpWebRequest)WebRequest.Create(url);

                requestToServer.Timeout = 10000;

                // Define a boundary string
                string boundaryString = "CDArchaFormBoundary" + DateTime.Now.Ticks.ToString("x");

                // Turn off the buffering of data to be written, to prevent OutOfMemoryException when sending data
                requestToServer.SendChunked = true;
                requestToServer.AllowWriteStreamBuffering = false;
                // Specify that request is a HTTP post
                requestToServer.Method = WebRequestMethods.Http.Post;
                // Specify that the content type is a multipart request
                requestToServer.ContentType = "multipart/mixed; boundary=" + boundaryString;

                UTF8Encoding utf8 = new UTF8Encoding();
                string boundaryStringLine = "--" + boundaryString + "\r\n";

                string lastBoundaryStringLine = "\r\n--" + boundaryString + "--";
                byte[] lastBoundaryStringLineBytes = utf8.GetBytes(lastBoundaryStringLine);

                // TEXT PARAMETERS
                string formDataString = "";
                foreach (string key in nvc.Keys)
                {
                    formDataString += boundaryStringLine
                        + String.Format(
                        "Content-Disposition: form-data; name=\"{0}\"; filename=\"{0}\"\r\nContent-Type: text/plain\r\n\r\n{1}\r\n",
                        key, nvc[key]);
                }
                byte[] formDataBytes = utf8.GetBytes(formDataString);

                // COVER PARAMETER
                long coverSize = 0;
                string coverDescriptionString = boundaryStringLine
                    + String.Format(
                    "Content-Disposition: form-data; name=\"{0}\"; "
                    + "filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n",
                    "cover", "cover.tif", "image/tif");
                byte[] coverDescriptionBytes = utf8.GetBytes(coverDescriptionString);

                if (coverFileName != null)
                {
                    FileInfo fileInfo = new FileInfo(coverFileName);
                    coverSize = fileInfo.Length + coverDescriptionBytes.Length;
                }

                // TOC PARAMETERS
                int counter = 1;
                Dictionary<string, byte[]> tocDescriptionsDictionary = new Dictionary<string, byte[]>();
                long tocSize = 0;
                foreach (var fileName in tocFileNames)
                {
                    string tocDescription = "\r\n" + boundaryStringLine
                        + String.Format(
                        "Content-Disposition: form-data; name=\"{0}\"; "
                        + "filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n",
                        "toc_page_" + counter, "toc_page_" + counter + ".tif", "image/tif");
                    byte[] tocDescriptionBytes = utf8.GetBytes(tocDescription);

                    FileInfo fi = new FileInfo(fileName);
                    tocSize += fi.Length + tocDescriptionBytes.Length;

                    tocDescriptionsDictionary.Add(fileName, tocDescriptionBytes);
                    counter++;
                }

                // META PARAMETER
                string metaDataString = boundaryStringLine
                    + String.Format(
                    "Content-Disposition: form-data; name=\"{0}\"; "
                    + "filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n",
                    "meta", "meta.xml", "text/xml")
                    + MetaXml + "\r\n";
                byte[] metaDataBytes = utf8.GetBytes(metaDataString);


                // Calculate the total size of the HTTP request
                long totalRequestBodySize =
                    lastBoundaryStringLineBytes.Length
                    + formDataBytes.Length
                    + coverSize
                    + tocSize
                    + formDataBytes.Length;

                // And indicate the value as the HTTP request content length
                requestToServer.ContentLength = totalRequestBodySize;


                // Write the http request body directly to the server
                using (Stream s = requestToServer.GetRequestStream())
                {
                    // Send text parameters
                    s.Write(formDataBytes, 0, formDataBytes.Length);

                    // Send cover
                    if (coverFileName != null)
                    {
                        s.Write(coverDescriptionBytes, 0,
                            coverDescriptionBytes.Length);

                        byte[] buffer = File.ReadAllBytes(coverFileName);
                        s.Write(buffer, 0, buffer.Length);
                    }

                    // Send toc
                    foreach (var tocRecord in tocDescriptionsDictionary)
                    {
                        GC.Collect();

                        byte[] buffer = File.ReadAllBytes(tocRecord.Key);
                        s.Write(tocRecord.Value, 0, tocRecord.Value.Length);
                        s.Write(buffer, 0, buffer.Length);
                    }

                    // Send meta
                    s.Write(metaDataBytes, 0, metaDataBytes.Length);

                    // Send the last part of the HTTP request body
                    s.Write(lastBoundaryStringLineBytes, 0, lastBoundaryStringLineBytes.Length);
                }

                //DEBUGLOG.AppendLine("UploadFilesToRemoteUrl (upload data): Total time: " + sw.ElapsedMilliseconds);

                // Grab the response from the server. WebException will be thrown
                // when a HTTP OK status is not returned
                tocDescriptionsDictionaryToDelete = tocDescriptionsDictionary;
                coverFileNameToDelete = coverFileName;

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Zasílání obálky a obsahu/bookletu.\r");
                }));

                WebResponse response = requestToServer.GetResponse();
                StreamReader responseReader = new StreamReader(response.GetResponseStream());
                e.Result = responseReader.ReadToEnd();
                response.Close();
                requestToServer.Abort();
                //DEBUGLOG.AppendLine("UploadFilesToRemoteUrl: Total time: " + sw.ElapsedMilliseconds);
            }
            catch (Exception ex) { }
        }

        private BitmapImage getIconSource(string uri)
        {
            BitmapImage newIcon = new BitmapImage();
            newIcon.BeginInit();
            newIcon.UriSource = new Uri("pack://application:,,,/" + uri);
            newIcon.EndInit();
            return newIcon;
        }

        // Uploads files to cdarcha server in new thread
        private void UploaderBW_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            UploadParameters up = e.Argument as UploadParameters;
            if (!Settings.offlineMode)
            {
                UploadFilesToRemoteUrl(up.Url, up.Nvc, up.MetaXml, up.MarcXml, up.MetaMods, e);
                this.workingMediaId = (e.Result as string) ?? "";
                this.lastWorkingMediaId = this.workingMediaId;
            }
        }

        // Shows result of uploading process (OK or error message)
        private void UploaderBW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.zobrazitVysledek.Visibility = System.Windows.Visibility.Hidden;
            if (e.Error != null)
            {
                controlTabDoneIcon.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-ban.png");
                if (e.Error is WebException)
                {
                    string message = "";
                    if ((e.Error as WebException).Response != null)
                    {
                        HttpWebResponse response = (e.Error as WebException).Response as HttpWebResponse;
                        if (response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            message = "Chyba autorizace: Přihlašovací údaje nejsou správné.";
                        }
                        else if (response.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            message = "Chyba na straně serveru: " + response.StatusDescription;
                        }
                        else
                        {
                            message = response.StatusCode + ": " + response.StatusDescription;
                        }
                    }
                    else
                    {
                        message = (e.Error as WebException).Status + ": " + e.Error.Message;
                    }
                    MessageBoxDialogWindow.Show("Odesílání neúspěšné", message, "OK", MessageBoxDialogWindow.Icons.Error);

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        this.stepIcon1.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-warning.png");
                        logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Odesílání neúspěšné.\r");
                    }));
                }
                else
                {
                    MessageBoxDialogWindow.Show("Chyba odesílání",
                        "Počas odesílání nastala neznámá výjimka, je možné, že data nebyla odeslána.",
                        "OK", MessageBoxDialogWindow.Icons.Error);

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        this.stepIcon1.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-warning.png");
                        logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Počas odesílání nastala neznámá výjimka, je možné, že data nebyla odeslána.\r");
                    }));
                }
            }
            else if (!e.Cancelled && !Settings.offlineMode)
            {
                string response = (e.Result as string) ?? "";
                if (string.IsNullOrEmpty(response))
                {
                    controlTabDoneIcon.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-ban.png");
                    MessageBoxDialogWindow.Show("Zpracování nepotvrzené",
                        "Server nepotvrdil zpracování dat. Je možné, že data nebyla zpracována správně." + response,
                        "OK", MessageBoxDialogWindow.Icons.Warning);

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        this.stepIcon1.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-warning.png");
                        logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Server nepotvrdil zpracování dat.Je možné, že data nebyla zpracována správně. " + response + "\r");
                    }));
                }
                else
                {
                    // Prave uploadnute 
                    this.workingMediaId = response;
                    this.lastWorkingMediaId = this.workingMediaId;

                    FillControlMetadata();
                    this.controlTabItem.IsEnabled = true;
                    this.tabControl.SelectedItem = this.controlTabItem;

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        this.stepIcon1.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-done.png");
                        if (Settings.offlineMode)
                        {
                            this.stepIcon1.Visibility = Visibility.Visible;
                        }
                        this.stepIcon2.Visibility = Visibility.Visible;
                        this.stepIcon2.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-hourglass.png");

                        // progress // log
                        logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Odesílání identifikátorů záznamu a metadat úspěšné.\r");
                        logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Vytváření ISO na lokálny PC.\r");
                    }));

                    //Create ISO
                    IsoFromMedia iso = new IsoFromMedia();
                    iso.OnFinish += new IsoEventHandler(iso_OnFinish);
                    iso.OnMessage += new IsoEventHandler(iso_OnMessage);
                    iso.OnProgress += new IsoEventHandler(iso_OnProgress);

                    IsoState status = iso.CreateIsoFromMedia(@selectedDriveName, @Settings.tmpPathToIso);
                    if (status != IsoState.Running)
                    {
                        iso.Stop();
                        MessageBoxDialogWindow.Show("Oznam", "Vytváření ISO selhalo.", "OK", MessageBoxDialogWindow.Icons.Error);
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Vytváření ISO selhalo.\r");
                        }));
                    }
                }
            }
        }

        // Uploads cover + toc
        private void UploaderCoverTocBW_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            UploadParameters up = e.Argument as UploadParameters;
            if (!Settings.offlineMode)
            {
                UploadCoverTocFilesToRemoteUrl(up.Url, up.Nvc, up.MetaXml, up.CoverFilePath, up.TocFilePaths, e);
            }
        }

        // Shows result of uploading process (OK or error message)
        private void UploaderCoverTocBW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.zobrazitVysledek.Visibility = System.Windows.Visibility.Hidden;
            if (e.Error != null)
            {
                scanningTabDoneIcon.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-ban.png");
                this.scanningStartButton.Content = "Neúspěšné";
                this.scanningStartButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF6C3131"));
                if (e.Error is WebException)
                {
                    string message = "";
                    if ((e.Error as WebException).Response != null)
                    {
                        HttpWebResponse response = (e.Error as WebException).Response as HttpWebResponse;
                        if (response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            message = "Chyba autorizace: Přihlašovací údaje nejsou správné.";
                        }
                        else if (response.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            message = "Chyba na straně serveru: " + response.StatusDescription;
                        }
                        else
                        {
                            message = response.StatusCode + ": " + response.StatusDescription;
                        }
                    }
                    else
                    {
                        message = (e.Error as WebException).Status + ": " + e.Error.Message;
                    }
                    MessageBoxDialogWindow.Show("Odesílání neúspěšné", message, "OK", MessageBoxDialogWindow.Icons.Error);

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Odesílání obálky a obsahu/bookletu neúspěšné.\r");
                    }));
                }
                else
                {
                    MessageBoxDialogWindow.Show("Chyba odesílání",
                        "Během odesílání nastala neznámá výjimka, je možné, že data nebyla odeslána.",
                        "OK", MessageBoxDialogWindow.Icons.Error);

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Během odesílání nastala neznámá výjimka, je možné, že data nebyla odeslána.\r");
                    }));
                }
            }
            else if (!e.Cancelled && !Settings.offlineMode)
            {
                string response = (e.Result as string) ?? "";
                if (string.IsNullOrEmpty(response))
                {
                    scanningTabDoneIcon.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-ban.png");
                    this.scanningStartButton.Content = "Neúspěšné";
                    this.scanningStartButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF6C3131"));
                    MessageBoxDialogWindow.Show("Zpracování nepotvrzené",
                        "Server nepotvrdil zpracování dat. Je možné, že data nebyla zpracována správně." + response,
                        "OK", MessageBoxDialogWindow.Icons.Warning);

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Server nepotvrdil zpracování dat.Je možné, že data nebyla zpracována správně. " + response + "\r");
                    }));
                }
                else
                {
                    //delete sent files
                    foreach (var tocRecord in tocDescriptionsDictionaryToDelete)
                    {
                        GC.Collect();

                        if (File.Exists(tocRecord.Key))
                        {
                            File.Delete(tocRecord.Key);
                        }
                    }

                    if (coverFileNameToDelete != null)
                    {
                        if (File.Exists(coverFileNameToDelete))
                        {
                            File.Delete(coverFileNameToDelete);
                        }
                    }

                    //remove working images and reset controls
                    tocPagesNumber.Content = "0 stran";
                    coverThumbnail.IsEnabled = false;
                    coverGuid = Guid.Empty;
                    coverThumbnail.Source = new BitmapImage(
                    new Uri("/CDArcha_klient;component/Images/default-icon.png", UriKind.Relative));
                    int cnt = this.tocImagesList.Items.Count;
                    for (int i = cnt - 1; i >= 0; i--)
                    {
                        Guid guid = (from record in tocThumbnailGridsDictionary.ToList()
                                     where record.Value.Equals(this.tocImagesList.Items.GetItemAt(i))
                                     select record.Key).First();
                        this.tocImagesList.Items.RemoveAt(i);
                        this.tocThumbnailGridsDictionary.Remove(guid);
                        this.imagesFilePaths.Remove(guid);
                        this.imagesOriginalSizes.Remove(guid);
                    }
                    //tocImagesList = new MyListView();
                    imagesFilePaths = new Dictionary<Guid, string>();
                    tocThumbnailGridsDictionary = new Dictionary<Guid, Grid>();
                    this.selectedImageGuid = Guid.Empty;
                    this.selectedImage.Source = new BitmapImage(
                        new Uri("/CDArcha_klient;component/Images/default-icon.png", UriKind.Relative));

                    FillControlMetadata();
                    this.controlTabItem.IsEnabled = true;
                    this.tabControl.SelectedItem = this.controlTabItem;
                    this.DownloadCoverAndToc();

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        // progress // log
                        logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Odesílání obálky a obsahu/bookletu úspěšné.\r");
                        scanningTabDoneIcon.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-done.png");
                        this.scanningStartButton.Content = "Úspěšně odesláno";
                        this.scanningStartButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF6BA62A"));
                    }));
                }
            }
        }
        #endregion

        #region scanning functionality

        // Scans image
        private void ScanImage(DocumentType documentType, String format)
        {
            //Stopwatch totalTime = new Stopwatch();
            //Stopwatch partialTime = new Stopwatch();
            //totalTime.Start();
            //partialTime.Start();
            ICommonDialog dialog = new CommonDialog();

            //try to set active scanner
            if (!setActiveScanner())
            {
                return;
            }

            int dpi = (documentType == DocumentType.Cover) ? Settings.CoverDPI : Settings.TocDPI;
            if (Settings.EnableScanLowRes) dpi = Settings.LowResDPI;
            Item item;

            try
            {
                item = activeScanner.Items[1];
            }
            catch (System.Runtime.InteropServices.COMException e)
            {
                MessageBoxDialogWindow.ShowHyperlink("Chyba!", "Nepodařilo se připojit skenovací zařízení.", "Zobrazit nápovědu.", "OK", MessageBoxDialogWindow.Icons.Error);
                //tmp; known errors : 8021006B - this exception is thrown when you disconnect printer while application is running
                //after disconnecting the printer the application needs to be restarted otherwise this exception will be caught again
                return;
            }

            //Setting configuration of scanner (dpi, color)
            Object value;
            foreach (IProperty property in item.Properties)
            {
                try
                {
                    switch (property.PropertyID)
                    {
                        case 6146: //4 is Black-white,gray is 2, color 1
                            value = (documentType == DocumentType.Cover) ? Settings.CoverScanType : Settings.TocScanType;
                            property.set_Value(ref value);
                            break;
                        case 6147: //dots per inch/horizontal
                            value = dpi;
                            property.set_Value(ref value);
                            break;
                        case 6148: //dots per inch/vertical
                            value = dpi;
                            property.set_Value(ref value);
                            break;
                        case 4104: //BitsPerPixel
                            value = 24;
                            property.set_Value(ref value);
                            break;
                        case 6151: //HorizontalExtent
                            if (format == null) break;
                            value = Settings.EnableScanLowRes ? 1225 : 2450; // A4, A5
                            if (format == "A3") value = Settings.EnableScanLowRes ? 1700 : 3400;
                            property.set_Value(ref value);
                            break;
                        case 6152: // vertical extent
                            if (format == null) break;
                            value = Settings.EnableScanLowRes ? 1700 : 3400; // A4
                            if (format == "A5")
                                value = Settings.EnableScanLowRes ? 860 : 1720;
                            else if (format == "A3")
                                value = Settings.EnableScanLowRes ? 2400 : 4800;
                            property.set_Value(ref value);
                            break;
                        case 6157: // rotation
                            if (format == null) break;
                            value = 0;
                            if (format == "A5") value = 1;
                            property.set_Value(ref value);
                            break;
                        case 5130: //TimeDelay
                            value = 0;
                            property.set_Value(ref value);
                            break;
                        case 6161: //LampWarmUpTime
                            value = 0;
                            property.set_Value(ref value);
                            break;
                    }
                }
                catch (Exception) { }
            }

            ImageFile image = null;
            try
            {
                image = (ImageFile)dialog.ShowTransfer(item, Settings.EnableScanLowDataFlow ? FormatID.wiaFormatJPEG : FormatID.wiaFormatBMP, true);
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                MessageBoxDialogWindow.Show("Chyba!", "Skenování nebylo úspěšné.", "OK", MessageBoxDialogWindow.Icons.Error);
                return;
            }

            //long scanTime = partialTime.ElapsedMilliseconds;
            //partialTime.Restart();

            BitmapSource originalSizeImage = null;
            BitmapSource smallerSizeImage = null;
            using (MemoryStream ms = new MemoryStream((byte[])image.FileData.get_BinaryData()))
            {
                originalSizeImage = ImageTools.LoadFullSize(ms);
                originalSizeImage = ImageTools.ApplyAutoColorCorrections(originalSizeImage);
                smallerSizeImage = ImageTools.LoadGivenSizeFromBitmapSource(originalSizeImage, 800);
            }

            //long conversionTime = partialTime.ElapsedMilliseconds;
            //partialTime.Restart();

            // create unique identifier for image
            Guid guid = Guid.NewGuid();
            while (this.imagesFilePaths.ContainsKey(guid))
            {
                guid = Guid.NewGuid();
            }

            string filePrefix = "";
            if (documentType == DocumentType.Cover)
            {
                filePrefix = "obalkyknih-cover_";
            }
            else if (documentType == DocumentType.Toc)
            {
                filePrefix = "obalkyknih-toc_";
            }

            string newFileName = Settings.TemporaryFolder + filePrefix + "_" + guid + ".tif";

            Size originalSize = new Size(originalSizeImage.PixelWidth, originalSizeImage.PixelHeight);

            this.imagesFilePaths.Add(guid, newFileName);
            this.imagesOriginalSizes.Add(guid, originalSize);

            if (documentType == DocumentType.Cover)
            {
                AddCoverImage(smallerSizeImage, guid);
            }
            else if (documentType == DocumentType.Toc)
            {
                AddTocImage(smallerSizeImage, guid);
            }

            //set workingImage and save previous to file
            if (this.workingImage.Key != Guid.Empty && this.imagesFilePaths.ContainsKey(this.workingImage.Key))
            {
                try
                {
                    ImageTools.SaveToFile(this.workingImage.Value, this.imagesFilePaths[this.workingImage.Key]);
                }
                catch (Exception)
                {
                    MessageBoxDialogWindow.Show("Chyba!", "Nastal problém při ukládání obrázku do souboru.",
                        "OK", MessageBoxDialogWindow.Icons.Error);
                    return;
                }
            }
            this.workingImage = new KeyValuePair<Guid, BitmapSource>(guid, originalSizeImage);

            image = null;
            GC.Collect();
            //DEBUGLOG.AppendLine("ScanImage: Total time: " + totalTime.ElapsedMilliseconds + "; scanning time: " + scanTime + "; conversion time:" + conversionTime + "; rest: " + partialTime.ElapsedMilliseconds);
        }

        // Sets active scanner device automatically, show selection dialog, if more scanners, if no scanner device found, shows error window and returns false
        private bool setActiveScanner()
        {
            ICommonDialog dialog = new CommonDialog();
            if (activeScanner != null)
            {
                return true;
            }

            List<DeviceInfo> foundDevices = GetDevices();
            //MessageBoxDialogWindow.Show("Chyba!", foundDevices.Count.ToString(), "OK", MessageBoxDialogWindow.Icons.Error); //pocet skeneru
            if (foundDevices.Count == 1 && foundDevices[0].Type == WiaDeviceType.ScannerDeviceType)
            {
                try
                {
                    activeScanner = foundDevices[0].Connect();
                }
                catch (Exception)
                {
                    activeScanner = dialog.ShowSelectDevice(WiaDeviceType.ScannerDeviceType, true, true);
                }
                return true;
            }
            try
            {
                activeScanner = dialog.ShowSelectDevice(WiaDeviceType.ScannerDeviceType, true, true);
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                // known errors  : 0x80210015 - device not found / WIA disabled
                MessageBoxDialogWindow.ShowHyperlink("Chyba!", "Skenovací zařízení nebylo nalezeno.", "Zobrazit nápovědu.", "OK", MessageBoxDialogWindow.Icons.Error);
                return false;
            }
            return true;
        }

        // Gets the list of available WIA devices.
        private static List<DeviceInfo> GetDevices()
        {
            List<DeviceInfo> devices = new List<DeviceInfo>();
            WIA.DeviceManager manager = new WIA.DeviceManager();

            foreach (WIA.DeviceInfo info in manager.DeviceInfos)
            {
                devices.Add(info);
                //MessageBoxDialogWindow.Show("Debug", info.DeviceID, "OK", MessageBoxDialogWindow.Icons.Information); //debug
            }

            return devices;
        }
        #endregion

        #region scanning tab controls

        #region Scanning controllers

        // Scan cover image
        private void ScanCoverButton_Click(object sender, RoutedEventArgs e)
        {
            ScanButtonClicked(DocumentType.Cover, null);
        }

        private void scanA3CoverButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ScanButtonClicked(DocumentType.Cover, "A3");
        }

        private void scanA4CoverButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ScanButtonClicked(DocumentType.Cover, "A4");
        }

        private void scanA5CoverButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ScanButtonClicked(DocumentType.Cover, "A5");
        }

        // Scan toc image
        private void ScanTocButton_Click(object sender, RoutedEventArgs e)
        {
            ScanButtonClicked(DocumentType.Toc, null);
        }

        private void scanA3TocButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ScanButtonClicked(DocumentType.Toc, "A3");
        }

        private void scanA4TocButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ScanButtonClicked(DocumentType.Toc, "A4");
        }

        private void scanA5TocButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ScanButtonClicked(DocumentType.Toc, "A5");
        }

        // Unified scan function
        private void ScanButtonClicked(DocumentType documentType, String format)
        {
            DisableImageControllers();

            // backup old cover
            if (documentType == DocumentType.Cover && this.coverGuid != Guid.Empty)
            {
                string filePath = this.imagesFilePaths[this.coverGuid];
                if (this.workingImage.Key == this.coverGuid)
                {
                    backupImage = new KeyValuePair<string, BitmapSource>(filePath,
                        this.workingImage.Value);
                }
                else
                {
                    backupImage = new KeyValuePair<string, BitmapSource>(filePath,
                    ImageTools.LoadFullSize(filePath));
                }
                SignalLoadedBackup();
            }

            ScanImage(documentType, format);

            EnableImageControllers();
        }
        #endregion

        #region Load Image controllers

        // Shows ExernalImageLoadWindow
        private void LoadFromFile_Clicked(object sender, MouseButtonEventArgs e)
        {
            ExternalImageLoadWindow window = new ExternalImageLoadWindow();
            window.Image_Clicked += new MouseButtonEventHandler(LoadButtonClicked);
            window.Pdf_Clicked += new MouseButtonEventHandler(PdfLoadButtonClicked);
            window.ShowDialog();
        }

        private void BackupOldCover(DocumentType documentType)
        {
            if (documentType == DocumentType.Cover && this.coverGuid != Guid.Empty)
            {
                string filePath = this.imagesFilePaths[this.coverGuid];
                if (this.workingImage.Key == this.coverGuid)
                {
                    backupImage = new KeyValuePair<string, BitmapSource>(filePath,
                        this.workingImage.Value);
                }
                else
                {
                    backupImage = new KeyValuePair<string, BitmapSource>(filePath,
                    ImageTools.LoadFullSize(filePath));
                }
                SignalLoadedBackup();
            }
        }

        private void ImportFromFile(DocumentType documentType, string fileName)
        {
            Guid guid = Guid.NewGuid();
            while (this.imagesFilePaths.ContainsKey(guid))
            {
                guid = Guid.NewGuid();
            }

            LoadExternalImage(documentType, fileName, guid);
        }

        // Shows dialog for loading image
        private void LoadButtonClicked(object sender, MouseButtonEventArgs e)
        {
            DocumentType documentType;
            if ((sender as Image).Name.Equals("tocImage"))
            {
                documentType = DocumentType.Toc;
            }
            else
            {
                documentType = DocumentType.Cover;
            }

            // backup old cover
            BackupOldCover(documentType);

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Title = (documentType == DocumentType.Cover) ? "Načíst obálku" : "Načíst obsah";
            dlg.Filter = "image files (bmp;png;jpeg;wmp;gif;tiff)|*.png;*.bmp;*.jpeg;*.jpg;*.wmp;*.gif;*.tiff;*.tif";
            dlg.FilterIndex = 2;
            bool? result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                string fileName = dlg.FileName;
                DisableImageControllers();
                ImportFromFile(documentType, fileName);
                EnableImageControllers();
                this.contrastSlider.IsEnabled = true;
            }
        }

        // Loads image from external file
        private void LoadExternalImage(DocumentType documentType, string fileName, Guid guid)
        {
            //Stopwatch totalSW = new Stopwatch();
            //Stopwatch partialSW = new Stopwatch();
            //long preparationTime;
            //long loadingTime;
            //long colorCorrectionTime;
            //long imageSaveTime;
            //long thumbCreateTime;
            //long saveWorkingImageTime = 0;
            //totalSW.Start();
            //partialSW.Start();

            string filePrefix = "";
            if (documentType == DocumentType.Toc)
            {
                filePrefix = "obalkyknih-toc_";
            }
            else
            {
                filePrefix = "obalkyknih-cover_";
            }

            string newFileName = Settings.TemporaryFolder + filePrefix + "_" + guid + ".tif";
            BitmapSource originalSizeImage = null;
            BitmapSource smallerSizeImage = null;
            Size originalSize;
            try
            {
                //partialSW.Stop();
                //preparationTime = partialSW.ElapsedMilliseconds;
                //partialSW.Restart();
                originalSizeImage = ImageTools.LoadFullSize(fileName);

                //partialSW.Stop();
                //loadingTime = partialSW.ElapsedMilliseconds;
                //partialSW.Restart();

                originalSizeImage = ImageTools.ApplyAutoColorCorrections(originalSizeImage);

                //partialSW.Stop();
                //colorCorrectionTime = partialSW.ElapsedMilliseconds;
                //partialSW.Restart();

                originalSize = new Size(originalSizeImage.PixelWidth, originalSizeImage.PixelHeight);

                ImageTools.SaveToFile(originalSizeImage, newFileName);

                //partialSW.Stop();
                //imageSaveTime = partialSW.ElapsedMilliseconds;
                //partialSW.Restart();

                smallerSizeImage = ImageTools.LoadGivenSizeFromBitmapSource((BitmapSource)originalSizeImage, 800);

                //partialSW.Stop();
                //thumbCreateTime = partialSW.ElapsedMilliseconds;
                //partialSW.Restart();
            }
            catch (Exception ex)
            {
                MessageBoxDialogWindow.Show("Chyba načítání obrázku", "Nastala chyba během načítání souboru. Důvod: " + ex.Message,
                    "OK", MessageBoxDialogWindow.Icons.Error);
                return;
            }

            this.imagesFilePaths.Add(guid, newFileName);
            this.imagesOriginalSizes.Add(guid, originalSize);

            if (documentType == DocumentType.Toc)
            {
                AddTocImage(smallerSizeImage, guid);
            }
            else
            {
                AddCoverImage(smallerSizeImage, guid);
            }

            if (this.workingImage.Key != Guid.Empty && this.imagesFilePaths.ContainsKey(this.workingImage.Key))
            {
                try
                {
                    ImageTools.SaveToFile(this.workingImage.Value, this.imagesFilePaths[this.workingImage.Key]);
                }
                catch (Exception)
                {
                    MessageBoxDialogWindow.Show("Chyba!", "Nastal problém při ukládání obrázku do souboru.",
                        "OK", MessageBoxDialogWindow.Icons.Error);
                    return;
                }
            }
            this.workingImage = new KeyValuePair<Guid, BitmapSource>(guid, originalSizeImage);

            GC.Collect();
        }
        #endregion

        #region Main transformation controllers (rotation, deskew, crop, flip)

        // Save selected image
        private void SaveSelectedButton_Clicked(object sender, RoutedEventArgs e)
        {
            SaveSelected();
        }
        // Switches SelecteDMode
        private void SaveSelectedMode_Clicked(object sender, RoutedEventArgs e)
        {
            SwitchSaveSelectedMode();
        }

        // Rotates selected image by 90 degrees left
        private void RotateLeft_Clicked(object sender, MouseButtonEventArgs e)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            TransformImage(ImageTransforms.RotateLeft);
            //DEBUGLOG.AppendLine("Rotate: Total time: " + sw.ElapsedMilliseconds);
        }

        // Rotates selected image by 90 degrees right
        private void RotateRight_Clicked(object sender, MouseButtonEventArgs e)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            TransformImage(ImageTransforms.RotateRight);
            //DEBUGLOG.AppendLine("Rotate: Total time: " + sw.ElapsedMilliseconds);
        }

        // Rotates selected image by 180 degrees
        private void Rotate180_Clicked(object sender, MouseButtonEventArgs e)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            TransformImage(ImageTransforms.Rotate180);
            //DEBUGLOG.AppendLine("Rotate: Total time: " + sw.ElapsedMilliseconds);
        }

        // Flips selected image horizontally
        private void Flip_Clicked(object sender, MouseButtonEventArgs e)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            TransformImage(ImageTransforms.FlipHorizontal);
            //DEBUGLOG.AppendLine("Flip: Total time: " + sw.ElapsedMilliseconds);
        }

        // Crops selected image
        private void Crop_Clicked(object sender, MouseButtonEventArgs e)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            TransformImage(ImageTransforms.Crop);
            //DEBUGLOG.AppendLine("Crop: Total time: " + sw.ElapsedMilliseconds);
        }

        // Deskews selected image
        private void Deskew_Clicked(object sender, MouseButtonEventArgs e)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            TransformImage(ImageTransforms.Deskew);
            //DEBUGLOG.AppendLine("Deskew: Total time: " + sw.ElapsedMilliseconds);
        }

        // Applies contrast and brightness changes to original image
        private void SliderConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            TransformImage(ImageTransforms.CorrectColors);
            //DEBUGLOG.AppendLine("Color correction: Total time: " + sw.ElapsedMilliseconds);
        }

        //Saves selection
        private void SaveSelected()
        {
            if (saveSelectedMode == false || selectedImage.Source == coverThumbnail.Source)
            {
                return;
            }

            Guid selectedGuid = this.selectedImageGuid;
            if (selectedGuid == Guid.Empty)
            {
                return;
            }

            string filePath = this.imagesFilePaths[selectedGuid];

            // if working image is not selected image, save old working image and load new
            if (selectedGuid != this.workingImage.Key)
            {
                if (this.workingImage.Key != Guid.Empty && this.imagesFilePaths.ContainsKey(this.workingImage.Key))
                {
                    try
                    {
                        ImageTools.SaveToFile(this.workingImage.Value, this.imagesFilePaths[this.workingImage.Key]);
                    }
                    catch (Exception)
                    {
                        MessageBoxDialogWindow.Show("Chyba!", "Nastal problém při ukládání obrázku do souboru.",
                        "OK", MessageBoxDialogWindow.Icons.Error);
                        EnableImageControllers();
                        return;
                    }
                }
                // freeze previous workingImage because it somehow decreases memory footprint
                if (this.workingImage.Value != null && this.workingImage.Value.CanFreeze)
                {
                    this.workingImage.Value.Freeze();
                }
                this.workingImage = new KeyValuePair<Guid, BitmapSource>(selectedGuid, ImageTools.LoadFullSize(filePath));
            }
            // freeze previous workingImage because it somehow decreases memory footprint
            if (this.workingImage.Value != null && this.workingImage.Value.CanFreeze)
            {
                this.workingImage.Value.Freeze();
            }
            // freeze previous backupImage because it somehow decreases memory footprint
            if (this.backupImage.Value != null && this.backupImage.Value.CanFreeze)
            {
                this.backupImage.Value.Freeze();
            }
            // backup old image
            backupImage = new KeyValuePair<string, BitmapSource>(filePath, this.workingImage.Value);
            SignalLoadedBackup();

            //get new guid
            Guid guid = Guid.NewGuid();
            while (this.imagesFilePaths.ContainsKey(guid))
            {
                guid = Guid.NewGuid();
            }

            //get new temporary filename
            string newFileName = Settings.TemporaryFolder +
                "obalkyknih-toc_"
                + "_" + guid + ".tif";

            BitmapSource originalSizeImage = null;
            BitmapSource smallerSizeImage = null;
            Size originalSize;

            try
            {
                var croppedImage = cropper.BpsCrop(workingImage.Value);
                originalSizeImage = croppedImage;
                originalSize = new Size(originalSizeImage.PixelWidth, originalSizeImage.PixelHeight);

                ImageTools.SaveToFile(originalSizeImage, newFileName);

                smallerSizeImage = ImageTools.LoadGivenSizeFromBitmapSource((BitmapSource)originalSizeImage, 800);
            }
            catch (Exception ex)
            {
                MessageBoxDialogWindow.Show("Chyba načítání obrázku", "Nastala chyba během načítání souboru. Důvod: " + ex.Message,
                    "OK", MessageBoxDialogWindow.Icons.Error);
                return;
            }

            this.imagesFilePaths.Add(guid, newFileName);
            this.imagesOriginalSizes.Add(guid, originalSize);

            AddTocImage(smallerSizeImage, guid);
            saveSelectedCount++;
            newestThumbnail.IsEnabled = false;

            if (this.workingImage.Key != Guid.Empty && this.imagesFilePaths.ContainsKey(this.workingImage.Key))
            {
                try
                {
                    ImageTools.SaveToFile(this.workingImage.Value, this.imagesFilePaths[this.workingImage.Key]);
                }
                catch (Exception)
                {
                    MessageBoxDialogWindow.Show("Chyba!", "Nastal problém při ukládání obrázku do souboru.",
                        "OK", MessageBoxDialogWindow.Icons.Error);
                    return;
                }
            }

            GC.Collect();
        }

        private void SwitchSaveSelectedMode()
        {
            //not working with toc
            if (workingImage.Value == null || selectedImage.Source == coverThumbnail.Source)
            {
                return;
            }

            if (!saveSelectedMode)
            {
                workingThumbnail = (Grid)tocImagesList.SelectedItem;

                saveSelecteModeButton.ToolTip = "Vypnout výsekový režim";
                saveSelecteModeButton.Content = "Hotovo";
                saveSelectedCount = 0;

                saveSelectedButton.Visibility = System.Windows.Visibility.Visible;

                //SaveSelectedMode is set to true when user is saving cropped part of image, false otherwise
                saveSelectedMode = true;

                //Disable controls that changes workingImage
                scanCoverButton.IsEnabled = false;
                scanCoverA5.IsEnabled = false;
                scanCoverA4.IsEnabled = false;
                scanCoverA3.IsEnabled = false;
                scanTocButton.IsEnabled = false;
                scanTocA5.IsEnabled = false;
                scanTocA4.IsEnabled = false;
                scanTocA3.IsEnabled = false;
                loadFromFileLabel.IsEnabled = false;
                sendButton.IsEnabled = false;

                coverThumbnail.IsEnabled = false;

                foreach (var grid in this.tocThumbnailGridsDictionary.Values)
                {
                    grid.IsEnabled = false;
                }

                foreach (var imageControl in workingThumbnail.Children.OfType<Image>())
                {
                    imageControl.Visibility = System.Windows.Visibility.Hidden;
                }
            }
            else
            {
                saveSelecteModeButton.ToolTip = "Zapnout výsekový režim";
                saveSelecteModeButton.Content = "Výsekový režim";

                saveSelectedButton.Visibility = System.Windows.Visibility.Hidden;

                //SaveSelectedMode is set to true when user is saving cropped part of image, false otherwise
                saveSelectedMode = false;

                //Enable controls that changes workingImage
                scanCoverButton.IsEnabled = true;
                scanCoverA5.IsEnabled = true;
                scanCoverA4.IsEnabled = true;
                scanCoverA3.IsEnabled = true;
                scanTocButton.IsEnabled = true;
                scanTocA5.IsEnabled = true;
                scanTocA4.IsEnabled = true;
                scanTocA3.IsEnabled = true;
                loadFromFileLabel.IsEnabled = true;
                sendButton.IsEnabled = true;

                coverThumbnail.IsEnabled = true;

                foreach (var grid in this.tocThumbnailGridsDictionary.Values)
                {
                    grid.IsEnabled = true;
                }

                foreach (var imageControl in workingThumbnail.Children.OfType<Image>())
                {
                    imageControl.Visibility = System.Windows.Visibility.Visible;
                }

                if (saveSelectedCount > 0)
                {
                    delete(tocImagesList.Items.IndexOf(workingThumbnail));
                }
            }
        }

        // Applies given transformation to selected image
        private void TransformImage(ImageTransforms transformation)
        {
            Guid guid = this.selectedImageGuid;
            if (guid == Guid.Empty)
            {
                return;
            }

            DisableImageControllers();

            string filePath = this.imagesFilePaths[guid];

            // if working image is not selected image, save old working image and load new
            if (guid != this.workingImage.Key)
            {
                if (this.workingImage.Key != Guid.Empty && this.imagesFilePaths.ContainsKey(this.workingImage.Key))
                {
                    try
                    {
                        ImageTools.SaveToFile(this.workingImage.Value, this.imagesFilePaths[this.workingImage.Key]);
                    }
                    catch (Exception)
                    {
                        MessageBoxDialogWindow.Show("Chyba!", "Nastal problém při ukládání obrázku do souboru.",
                        "OK", MessageBoxDialogWindow.Icons.Error);
                        EnableImageControllers();
                        return;
                    }
                }
                // freeze previous workingImage because it somehow decreases memory footprint
                if (this.workingImage.Value != null && this.workingImage.Value.CanFreeze)
                {
                    this.workingImage.Value.Freeze();
                }
                this.workingImage = new KeyValuePair<Guid, BitmapSource>(guid, ImageTools.LoadFullSize(filePath));
            }

            // freeze previous workingImage because it somehow decreases memory footprint
            if (this.workingImage.Value != null && this.workingImage.Value.CanFreeze)
            {
                this.workingImage.Value.Freeze();
            }
            // freeze previous backupImage because it somehow decreases memory footprint
            if (this.backupImage.Value != null && this.backupImage.Value.CanFreeze)
            {
                this.backupImage.Value.Freeze();
            }
            // backup old image
            backupImage = new KeyValuePair<string, BitmapSource>(filePath, this.workingImage.Value);
            SignalLoadedBackup();

            // do transformation to working image
            switch (transformation)
            {
                case ImageTransforms.RotateLeft:
                    this.workingImage = new KeyValuePair<Guid, BitmapSource>(guid,
                        ImageTools.RotateImage(this.workingImage.Value, -90));
                    break;
                case ImageTransforms.RotateRight:
                    this.workingImage = new KeyValuePair<Guid, BitmapSource>(guid,
                        ImageTools.RotateImage(this.workingImage.Value, 90));
                    break;
                case ImageTransforms.Rotate180:
                    this.workingImage = new KeyValuePair<Guid, BitmapSource>(guid,
                        ImageTools.RotateImage(this.workingImage.Value, 180));
                    break;
                case ImageTransforms.FlipHorizontal:
                    this.workingImage = new KeyValuePair<Guid, BitmapSource>(guid,
                        ImageTools.FlipHorizontalImage(this.workingImage.Value));
                    break;
                case ImageTransforms.Deskew:
                    double skewAngle = ImageTools.GetDeskewAngle(this.selectedImage.Source as BitmapSource);
                    this.workingImage = new KeyValuePair<Guid, BitmapSource>(guid,
                        ImageTools.DeskewImage(this.workingImage.Value, skewAngle));
                    break;
                case ImageTransforms.Crop:
                    this.workingImage = new KeyValuePair<Guid, BitmapSource>(guid,
                        ImageTools.CropImage(this.workingImage.Value, this.cropper));
                    break;
                case ImageTransforms.CorrectColors:
                    BitmapSource tmp = ImageTools.AdjustContrast(this.workingImage.Value, (int)this.contrastSlider.Value);
                    tmp = ImageTools.AdjustGamma(tmp, (int)this.gammaSlider.Value);
                    tmp = ImageTools.AdjustBrightness(tmp, (int)this.gammaSlider.Value);
                    this.workingImage = new KeyValuePair<Guid, BitmapSource>(guid, tmp);
                    break;
            }

            // renew selected image
            this.selectedImage.Source = ImageTools.LoadGivenSizeFromBitmapSource(this.workingImage.Value, 800);
            this.sliderOriginalImage = new KeyValuePair<Guid, BitmapSource>(Guid.Empty, null);

            // renew thumbnail image
            //if (this.selectedImageGuid == this.coverGuid)
            this.coverThumbnail.Source = this.selectedImage.Source;
            if (this.tocThumbnailGridsDictionary.ContainsKey(this.selectedImageGuid))
            {
                (LogicalTreeHelper.FindLogicalNode(this.tocThumbnailGridsDictionary[this.selectedImageGuid],
                    "tocThumbnail") as Image).Source = this.selectedImage.Source;
            }

            // set new width and height
            this.imagesOriginalSizes[this.selectedImageGuid] = new Size(this.workingImage.Value.PixelWidth,
                this.workingImage.Value.PixelHeight);

            EnableImageControllers();

            // reset cropZone
            SetAppropriateCrop(Size.Empty, this.selectedImage.RenderSize, true);
            GC.Collect();
        }

        // Changes brightness of cover - only preview
        private void BrightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.selectedImageGuid == Guid.Empty)
            {
                return;
            }
            if (this.sliderOriginalImage.Key != this.selectedImageGuid)
            {
                this.sliderOriginalImage = new KeyValuePair<Guid, BitmapSource>(
                    this.selectedImageGuid, this.selectedImage.Source as BitmapSource);
            }
            BitmapSource tmp = ImageTools.AdjustContrast(this.sliderOriginalImage.Value, (int)this.contrastSlider.Value);
            tmp = ImageTools.AdjustGamma(tmp, this.gammaSlider.Value);
            this.selectedImage.Source = ImageTools.AdjustBrightness(tmp, (int)e.NewValue);
        }

        // Changes contrast of cover - only preview
        private void ContrastSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.selectedImageGuid == Guid.Empty)
            {
                return;
            }
            if (this.sliderOriginalImage.Key != this.selectedImageGuid)
            {
                this.sliderOriginalImage = new KeyValuePair<Guid, BitmapSource>(
                    this.selectedImageGuid, this.selectedImage.Source as BitmapSource);
            }
            BitmapSource tmp = ImageTools.AdjustContrast(this.sliderOriginalImage.Value, (int)e.NewValue);
            tmp = ImageTools.AdjustGamma(tmp, this.gammaSlider.Value);
            this.selectedImage.Source = ImageTools.AdjustBrightness(tmp, (int)this.brightnessSlider.Value);
        }

        // Changes contrast of cover - only preview
        private void GammaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.selectedImageGuid == Guid.Empty)
            {
                return;
            }
            if (this.sliderOriginalImage.Key != this.selectedImageGuid)
            {
                this.sliderOriginalImage = new KeyValuePair<Guid, BitmapSource>(
                    this.selectedImageGuid, this.selectedImage.Source as BitmapSource);
            }

            BitmapSource tmp = ImageTools.AdjustContrast(this.sliderOriginalImage.Value, (int)this.contrastSlider.Value);
            tmp = ImageTools.AdjustGamma(tmp, e.NewValue);
            this.selectedImage.Source = ImageTools.AdjustBrightness(tmp, (int)this.brightnessSlider.Value);
        }
        #endregion

        #region Thumbnail controllers

        // Adds new cover image
        private void AddCoverImage(BitmapSource bitmapSource, Guid guid)
        {
            this.imagesFilePaths.Remove(this.coverGuid);
            this.imagesOriginalSizes.Remove(this.coverGuid);
            this.coverGuid = guid;
            this.selectedImageGuid = guid;
            // add bitmapSource to images
            this.coverThumbnail.IsEnabled = true;
            this.coverThumbnail.Source = bitmapSource;
            this.selectedImage.Source = bitmapSource;
            // set border
            RemoveAllBorders();
            HideAllThumbnailControls();
            (this.coverThumbnail.Parent as Border).BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#6D8527"));
            this.coverThumbnail.IsEnabled = true;
            this.deleteCoverIcon.Visibility = Visibility.Visible;

            // set crop
            SetAppropriateCrop(Size.Empty, this.selectedImage.RenderSize, true);

            EnableImageControllers();
        }

        // Adds new TOC image to list of TOC images
        private void AddTocImage(BitmapSource bitmapSource, Guid guid)
        {
            #region construction of ListItem
            // create thumbnail with following structure
            //<ItemsControl>
            //    <Grid>
            //        <Image HorizontalAlignment="Left" Margin="0,-40,0,0" Stretch="Uniform" VerticalAlignment="Center" Source="/CDArcha_klient;component/Images/arrows/arrow_up.gif" Width="23"/>
            //        <Image HorizontalAlignment="Left" Margin="0,45,0,0" Stretch="Uniform" VerticalAlignment="Center" Source="/CDArcha_klient;component/Images/delete_24.png" Width="23"/>
            //        <Image HorizontalAlignment="Left" Margin="0,0,0,0" Stretch="Uniform" VerticalAlignment="Center" Source="/CDArcha_klient;component/Images/arrows/arrow_down.gif" Width="23"/>
            //        <Border>
            //            <Image HorizontalAlignment="Left" Margin="25,0,0,0" Stretch="Uniform" VerticalAlignment="Top" Source="/CDArcha_klient;component/Images/default-icon.png" />
            //        </Border>
            //    </Grid>
            //</ItemsControl>
            Image tocImage = new Image();
            tocImage.Name = "tocThumbnail";
            tocImage.MouseLeftButtonDown += Thumbnail_Clicked;
            tocImage.Source = bitmapSource;
            tocImage.Cursor = Cursors.Hand;
            tocImage.MouseEnter += Icon_MouseEnter;
            tocImage.MouseLeave += Icon_MouseLeave;

            Border tocImageBorder = new Border();
            tocImageBorder.BorderThickness = new Thickness(4);
            if (!saveSelectedMode)
            {
                //green border
                tocImageBorder.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#6D8527"));
            }
            else
            {
                tocImageBorder.BorderBrush = Brushes.Transparent;
            }
            tocImageBorder.Margin = new Thickness(50, 0, 50, 0);

            Image deleteImage = new Image();
            deleteImage.Name = "deleteThumbnail";
            deleteImage.VerticalAlignment = VerticalAlignment.Top;
            deleteImage.HorizontalAlignment = HorizontalAlignment.Right;
            deleteImage.Source = new BitmapImage(new Uri("/CDArcha_klient;component/Images/ok-icon-delete.png", UriKind.Relative));
            deleteImage.Margin = new Thickness(0, 0, 26, 0);
            deleteImage.Width = 18;
            deleteImage.Stretch = Stretch.None;
            deleteImage.Cursor = Cursors.Hand;
            deleteImage.MouseLeftButtonDown += TocThumbnail_Delete;

            Image moveUpImage = new Image();
            moveUpImage.Name = "moveUpThumbnail";
            moveUpImage.VerticalAlignment = VerticalAlignment.Top;
            moveUpImage.HorizontalAlignment = HorizontalAlignment.Right;
            moveUpImage.Source = new BitmapImage(new Uri("/CDArcha_klient;component/Images/ok-icon-up.png", UriKind.Relative));
            moveUpImage.Margin = new Thickness(0, 25, 26, 0);
            moveUpImage.Stretch = Stretch.None;
            moveUpImage.Width = 18;
            moveUpImage.Cursor = Cursors.Hand;
            moveUpImage.MouseLeftButtonDown += TocThumbnail_MoveUp;
            if (!this.tocImagesList.HasItems)
            {
                moveUpImage.Visibility = Visibility.Hidden;
            }


            Image moveDownImage = new Image();
            moveDownImage.Name = "moveDownThumbnail";
            moveDownImage.VerticalAlignment = VerticalAlignment.Top;
            moveDownImage.HorizontalAlignment = HorizontalAlignment.Right;
            moveDownImage.Source = new BitmapImage(new Uri("/CDArcha_klient;component/Images/ok-icon-down.png", UriKind.Relative));
            moveDownImage.Margin = new Thickness(0, 50, 26, 0); // 0 50 26 0
            moveDownImage.Width = 18;
            moveDownImage.Stretch = Stretch.None;
            moveDownImage.Cursor = Cursors.Hand;
            moveDownImage.MouseLeftButtonDown += TocThumbnail_MoveDown;
            moveDownImage.Visibility = Visibility.Hidden;

            Image moveIntoImage = new Image();
            moveIntoImage.Name = "moveIntoThumbnail";
            moveIntoImage.VerticalAlignment = VerticalAlignment.Top;
            moveIntoImage.HorizontalAlignment = HorizontalAlignment.Right;
            moveIntoImage.Source = new BitmapImage(new Uri("/CDArcha_klient;component/Images/ok-icon-up-down.png", UriKind.Relative));
            moveIntoImage.Margin = new Thickness(0, 75, 23, 0); //init
            moveIntoImage.Width = 23;
            moveIntoImage.Cursor = Cursors.Hand;
            moveIntoImage.MouseLeftButtonDown += TocThumbnail_MoveInto;
            if (tocImagesList.Items.Count < 2)
            {
                moveIntoImage.Visibility = Visibility.Hidden;
            }

            Grid gridWrapper = new Grid();
            gridWrapper.Margin = new Thickness(0, 10, 0, 10);
            gridWrapper.Name = "guid_" + guid.ToString().Replace("-", "");
            tocImageBorder.Child = tocImage;
            gridWrapper.Children.Add(tocImageBorder);
            gridWrapper.Children.Add(moveIntoImage);
            gridWrapper.Children.Add(moveUpImage);
            gridWrapper.Children.Add(moveDownImage);
            gridWrapper.Children.Add(deleteImage);
            #endregion

            // edit previously last item - enable moveDown arrow
            if (this.tocImagesList.HasItems)
            {
                var lastItem = this.tocImagesList.Items.OfType<Grid>().LastOrDefault();
                foreach (Image item in lastItem.Children.OfType<Image>())
                {
                    if (item.Name.Contains("moveDownThumbnail"))
                    {
                        item.IsEnabled = true;
                    }
                }
            }

            if (!saveSelectedMode)
            {
                RemoveAllBorders();
            }
            HideAllThumbnailControls();

            // add to list
            this.tocImagesList.Items.Add(gridWrapper);

            if (!saveSelectedMode)
            {
                this.tocImagesList.SelectedItem = gridWrapper;
            }
            else
            {
                //its saved so it can be disabled upon adding to list when working with other mode
                newestThumbnail = gridWrapper;
            }

            // assign "pointers" to these elements into dictionaries
            if (!saveSelectedMode)
            {
                this.selectedImageGuid = guid;
                this.selectedImage.Source = bitmapSource;
            }
            this.tocThumbnailGridsDictionary.Add(guid, gridWrapper);
            SetAppropriateCrop(Size.Empty, this.selectedImage.RenderSize, true);
            string pages = "";
            int pagesNumber = this.tocImagesList.Items.Count;
            switch (pagesNumber)
            {
                case 1:
                    pages = "strana";
                    break;
                case 2:
                case 3:
                case 4:
                    pages = "strany";
                    break;
                default:
                    pages = "stran";
                    break;
            }
            this.tocPagesNumber.Content = pagesNumber + " " + pages;
            EnableImageControllers();
            this.ocrCheckBox.IsChecked = true;


            (Window.GetWindow(this) as MainWindow).DeactivateUndo();
            (Window.GetWindow(this) as MainWindow).DeactivateRedo();
        }

        // Removes colored border from all thumbnails
        private void RemoveAllBorders()
        {
            (this.coverThumbnail.Parent as Border).BorderBrush = Brushes.Transparent;
            foreach (var grid in this.tocThumbnailGridsDictionary.Values)
            {
                grid.Children.OfType<Border>().First().BorderBrush = Brushes.Transparent;
            }
        }

        // Hides all thumbnail controls (arrows and delete icon)
        private void HideAllThumbnailControls()
        {
            this.deleteCoverIcon.Visibility = Visibility.Hidden;
            foreach (var grid in this.tocThumbnailGridsDictionary.Values)
            {
                foreach (var imageControl in grid.Children.OfType<Image>())
                {
                    imageControl.Visibility = Visibility.Hidden;
                }
            }
        }

        // Makes appropriate controls of toc thumbnail visible
        private void SetTocThumbnailControls(Guid guid)
        {
            Grid grid = this.tocThumbnailGridsDictionary[guid];
            // set delete icon visible
            (LogicalTreeHelper.FindLogicalNode(grid, "deleteThumbnail") as Image).Visibility = Visibility.Visible;
            Image moveUp = LogicalTreeHelper.FindLogicalNode(grid, "moveUpThumbnail") as Image;
            Image moveDown = LogicalTreeHelper.FindLogicalNode(grid, "moveDownThumbnail") as Image;
            Image moveInto = LogicalTreeHelper.FindLogicalNode(grid, "moveIntoThumbnail") as Image;
            if (!grid.Equals(this.tocImagesList.Items.GetItemAt(0)))
            {
                moveUp.Visibility = Visibility.Visible;
            }

            if (!grid.Equals(this.tocImagesList.Items.GetItemAt(this.tocImagesList.Items.Count - 1)))
            {
                moveDown.Visibility = Visibility.Visible;
            }

            if (tocImagesList.Items.Count > 2)
            {
                moveInto.Visibility = Visibility.Visible;
            }
        }

        // Sets the selectedImage
        private void Thumbnail_Clicked(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            this.selectedImage.Source = image.Source;
            SetAppropriateCrop(Size.Empty, this.selectedImage.RenderSize, true);

            RemoveAllBorders();
            HideAllThumbnailControls();

            // find out if new image is toc or cover and color the border
            if (image.Name.Equals("coverThumbnail"))
            {
                this.selectedImageGuid = this.coverGuid;
                (image.Parent as Border).BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#6D8527"));
                this.deleteCoverIcon.Visibility = Visibility.Visible;
            }
            else
            {
                Border border = (sender as Image).Parent as Border;
                string guidName = (border.Parent as Grid).Name;
                foreach (var key in this.imagesFilePaths.Keys)
                {
                    if (guidName.Contains(key.ToString().Replace("-", "")))
                    {
                        this.selectedImageGuid = key;
                    }
                }
                border.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#6D8527"));
                if (image.Name.Contains("tocThumbnail"))
                {
                    SetTocThumbnailControls(this.selectedImageGuid);
                }
            }

            EnableImageControllers();
            SetAppropriateCrop(Size.Empty, this.selectedImage.RenderSize, true);
        }

        // Sets selected TOC image from list of all TOC images
        private void TocThumbnail_MoveUp(object sender, MouseButtonEventArgs e)
        {
            int selectedIndex = this.tocImagesList.SelectedIndex;

            // check sanity of moving up
            if (selectedIndex <= 0 || selectedIndex > this.tocImagesList.Items.Count - 1)
            {
                return;
            }

            // get the grid
            var tmp = this.tocImagesList.Items.GetItemAt(selectedIndex);
            // move it
            this.tocImagesList.Items.RemoveAt(selectedIndex);
            this.tocImagesList.Items.Insert(selectedIndex - 1, tmp);

            this.tocImagesList.SelectedIndex = selectedIndex - 1;

            HideAllThumbnailControls();
            SetTocThumbnailControls(this.selectedImageGuid);
        }

        // Sets selected TOC image from list of all TOC images
        private void TocThumbnail_MoveDown(object sender, MouseButtonEventArgs e)
        {
            int selectedIndex = this.tocImagesList.SelectedIndex;

            // check sanity of moving down
            if (selectedIndex < 0 || selectedIndex >= this.tocImagesList.Items.Count - 1)
            {
                return;
            }

            // get the grid
            var tmp = this.tocImagesList.Items.GetItemAt(selectedIndex);
            // move it
            this.tocImagesList.Items.RemoveAt(selectedIndex);
            this.tocImagesList.Items.Insert(selectedIndex + 1, tmp);

            this.tocImagesList.SelectedIndex = selectedIndex + 1;

            HideAllThumbnailControls();
            SetTocThumbnailControls(this.selectedImageGuid);
        }

        //Sets selected TOC image from list of all TOC images
        private void TocThumbnail_MoveInto(object sender, MouseButtonEventArgs e)
        {
            int selectedIndex = this.tocImagesList.SelectedIndex;
            // get the grid
            var tmp = this.tocImagesList.Items.GetItemAt(selectedIndex);

            //asks user for input
            MoveScannedPageWindow window = new MoveScannedPageWindow(tocImagesList.Items.Count);
            window.ShowDialog();


            //move it to selected position if input value is valid
            if (window.DialogResult.HasValue && window.DialogResult.Value)
            {
                int theValue = window.moveIntoValue;
                tocImagesList.Items.RemoveAt(selectedIndex);
                tocImagesList.Items.Insert(--theValue, tmp);
                tocImagesList.SelectedIndex = theValue;

                HideAllThumbnailControls();
                SetTocThumbnailControls(selectedImageGuid);
            }

        }

        private void TocThumbnail_DeleteNoRemove()
        {
            int selectedIndex = this.tocImagesList.SelectedIndex;
            // sanity check
            if (selectedIndex < 0 || selectedIndex >= this.tocImagesList.Items.Count)
            {
                return;
            }

            bool dontShowAgain;
            bool? result = true;
            if (!Settings.DisableTocDeletionNotification)
            {
                result = MessageBoxDialogWindow.Show("Potvrzení odstranění", "Opravdu chcete odstranit vybraný obsah?",
                    out dontShowAgain, "Příště se neptat a rovnou odstranit", "Ano", "Ne", false,
                    MessageBoxDialogWindow.Icons.Question);
                if (result == true && dontShowAgain)
                {
                    Settings.DisableTocDeletionNotification = true;
                }
            }
            if (result == true)
            {
                delete(selectedIndex, false);
            }
        }

        //Removes image from TOC thumbnails
        private void TocThumbnail_Delete(object sender, MouseButtonEventArgs e)
        {
            int selectedIndex = this.tocImagesList.SelectedIndex;
            // sanity check
            if (selectedIndex < 0 || selectedIndex >= this.tocImagesList.Items.Count)
            {
                return;
            }

            bool dontShowAgain;
            bool? result = true;
            if (!Settings.DisableTocDeletionNotification)
            {
                result = MessageBoxDialogWindow.Show("Potvrzení odstranění", "Opravdu chcete odstranit vybraný obsah?",
                    out dontShowAgain, "Příště se neptat a rovnou odstranit", "Ano", "Ne", false,
                    MessageBoxDialogWindow.Icons.Question);
                if (result == true && dontShowAgain)
                {
                    Settings.DisableTocDeletionNotification = true;
                }
            }
            if (result == true)
            {
                delete(selectedIndex);
            }
        }

        private void CoverThumbnail_DeleteNoRemove()
        {
            (this.coverThumbnail.Parent as Border).BorderBrush = Brushes.Transparent;

            if (this.coverGuid != this.workingImage.Key)
            {
                this.backupImage = new KeyValuePair<string, BitmapSource>(this.imagesFilePaths[this.coverGuid],
                    ImageTools.LoadFullSize(this.imagesFilePaths[this.coverGuid]));
            }
            else
            {
                this.backupImage = new KeyValuePair<string, BitmapSource>(
                    this.imagesFilePaths[this.coverGuid], this.workingImage.Value);
            }
            SignalLoadedBackup();

            this.imagesFilePaths.Remove(this.coverGuid);
            this.imagesOriginalSizes.Remove(this.coverGuid);
            this.coverGuid = Guid.Empty;
            this.coverThumbnail.IsEnabled = false;
            this.deleteCoverIcon.Visibility = Visibility.Hidden;
            this.coverThumbnail.Source = new BitmapImage(
                    new Uri("/CDArcha_klient;component/Images/default-icon.png", UriKind.Relative));

            if (this.tocThumbnailGridsDictionary.Keys.Count > 0)
            {
                this.selectedImageGuid = this.tocThumbnailGridsDictionary.Keys.Last();
                this.selectedImage.Source = (LogicalTreeHelper.FindLogicalNode(
                    this.tocThumbnailGridsDictionary.Values.Last(), "tocThumbnail") as Image).Source;

                this.tocImagesList.SelectedItem = this.tocThumbnailGridsDictionary[this.selectedImageGuid];
                this.tocThumbnailGridsDictionary[this.selectedImageGuid].Children.OfType<Border>().First()
                    .BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#6D8527"));
                SetTocThumbnailControls(this.selectedImageGuid);

            }
            else
            {
                // set default image
                this.selectedImageGuid = Guid.Empty;
                this.selectedImage.Source = new BitmapImage(
                    new Uri("/CDArcha_klient;component/Images/default-icon.png", UriKind.Relative));
            }
            Mouse.OverrideCursor = null;

        }

        private void CoverThumbnail_Delete(object sender, MouseButtonEventArgs e)
        {
            bool dontShowAgain;
            bool? result = true;
            if (!Settings.DisableCoverDeletionNotification)
            {
                result = MessageBoxDialogWindow.Show("Potvrzení odstranění", "Opravdu chcete odstranit vybranou obálku?",
                    out dontShowAgain, "Příště se neptat a rovnou odstranit", "Ano", "Ne", false,
                    MessageBoxDialogWindow.Icons.Question);
                if (result == true && dontShowAgain)
                {
                    Settings.DisableCoverDeletionNotification = true;
                }
            }
            if (result == true)
            {
                DisableImageControllers();

                (this.coverThumbnail.Parent as Border).BorderBrush = Brushes.Transparent;

                if (this.coverGuid != this.workingImage.Key)
                {
                    this.backupImage = new KeyValuePair<string, BitmapSource>(this.imagesFilePaths[this.coverGuid],
                        ImageTools.LoadFullSize(this.imagesFilePaths[this.coverGuid]));
                }
                else
                {
                    this.backupImage = new KeyValuePair<string, BitmapSource>(
                        this.imagesFilePaths[this.coverGuid], this.workingImage.Value);
                }
                SignalLoadedBackup();

                try
                {
                    if (File.Exists(this.imagesFilePaths[this.coverGuid]))
                    {
                        File.Delete(this.imagesFilePaths[this.coverGuid]);
                    }
                }
                catch (Exception)
                {
                    MessageBoxDialogWindow.Show("Chyba mazání souboru.", "Nebylo možné zmazat soubor z disku.",
                        "OK", MessageBoxDialogWindow.Icons.Error);
                }

                this.imagesFilePaths.Remove(this.coverGuid);
                this.imagesOriginalSizes.Remove(this.coverGuid);
                this.coverGuid = Guid.Empty;
                this.coverThumbnail.IsEnabled = false;
                this.deleteCoverIcon.Visibility = Visibility.Hidden;
                this.coverThumbnail.Source = new BitmapImage(
                        new Uri("/CDArcha_klient;component/Images/default-icon.png", UriKind.Relative));

                if (this.tocThumbnailGridsDictionary.Keys.Count > 0)
                {
                    this.selectedImageGuid = this.tocThumbnailGridsDictionary.Keys.Last();
                    this.selectedImage.Source = (LogicalTreeHelper.FindLogicalNode(
                        this.tocThumbnailGridsDictionary.Values.Last(), "tocThumbnail") as Image).Source;

                    this.tocImagesList.SelectedItem = this.tocThumbnailGridsDictionary[this.selectedImageGuid];
                    this.tocThumbnailGridsDictionary[this.selectedImageGuid].Children.OfType<Border>().First()
                        .BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#6D8527"));
                    SetTocThumbnailControls(this.selectedImageGuid);

                    EnableImageControllers();
                }
                else
                {
                    // set default image
                    this.selectedImageGuid = Guid.Empty;
                    this.selectedImage.Source = new BitmapImage(
                        new Uri("/CDArcha_klient;component/Images/default-icon.png", UriKind.Relative));
                }
                Mouse.OverrideCursor = null;
            }
        }

        //delete TOC image at index
        private void delete(int index, bool removeFiles = true)
        {
            DisableImageControllers();

            Guid guid = (from record in tocThumbnailGridsDictionary.ToList()
                         where record.Value.Equals(this.tocImagesList.Items.GetItemAt(index))
                         select record.Key).First();

            if (guid != Guid.Empty)
            {
                if (guid != this.workingImage.Key)
                {
                    this.backupImage = new KeyValuePair<string, BitmapSource>(this.imagesFilePaths[guid],
                        ImageTools.LoadFullSize(this.imagesFilePaths[guid]));
                }
                else
                {
                    this.backupImage = new KeyValuePair<string, BitmapSource>(this.imagesFilePaths[guid], this.workingImage.Value);
                }
                SignalLoadedBackup();
            }

            try
            {
                if (removeFiles && File.Exists(this.imagesFilePaths[guid]))
                {
                    File.Delete(this.imagesFilePaths[guid]);
                }
            }
            catch (Exception)
            {
                MessageBoxDialogWindow.Show("Chyba mazání souboru.", "Nebylo možné zmazat soubor z disku.",
                    "OK", MessageBoxDialogWindow.Icons.Error);
            }

            this.tocImagesList.Items.RemoveAt(index);
            this.tocThumbnailGridsDictionary.Remove(guid);
            this.imagesFilePaths.Remove(guid);
            this.imagesOriginalSizes.Remove(guid);

            HideAllThumbnailControls();

            if (!this.tocImagesList.HasItems)
            {
                if (this.coverGuid == Guid.Empty)
                {
                    // set default image
                    this.selectedImageGuid = Guid.Empty;
                    this.selectedImage.Source = new BitmapImage(
                        new Uri("/CDArcha_klient;component/Images/default-icon.png", UriKind.Relative));

                    Mouse.OverrideCursor = null;
                }
                else
                {
                    this.selectedImageGuid = this.coverGuid;
                    (this.coverThumbnail.Parent as Border).BorderBrush = (SolidColorBrush)(new BrushConverter()
                        .ConvertFrom("#6D8527"));
                    this.deleteCoverIcon.Visibility = Visibility.Visible;
                    this.selectedImage.Source = coverThumbnail.Source;

                    EnableImageControllers();
                }
            }
            else
            {
                Grid grid = this.tocImagesList.Items.GetItemAt(tocImagesList.Items.Count - 1) as Grid;
                Image thumbnail = LogicalTreeHelper.FindLogicalNode(grid, "tocThumbnail") as Image;
                this.selectedImage.Source = thumbnail.Source;
                foreach (var guidKey in this.imagesFilePaths.Keys)
                {
                    if (grid.Name.Contains(guidKey.ToString().Replace("-", "")))
                    {
                        this.selectedImageGuid = guidKey;
                    }
                }
                SetTocThumbnailControls(this.selectedImageGuid);
                this.tocThumbnailGridsDictionary[this.selectedImageGuid].Children.OfType<Border>().First()
                    .BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#6D8527"));

                this.tocImagesList.SelectedItem = this.tocThumbnailGridsDictionary[this.selectedImageGuid];
                EnableImageControllers();
            }

            // set numer of pages
            string pages = "";
            int pagesNumber = this.tocImagesList.Items.Count;
            switch (pagesNumber)
            {
                case 1:
                    pages = "strana";
                    break;
                case 2:
                case 3:
                case 4:
                    pages = "strany";
                    break;
                default:
                    pages = "stran";
                    break;
            }
            this.tocPagesNumber.Content = pagesNumber + " " + pages;
        }

        #endregion

        #region Undo/Redo
        internal void UndoLastStep()
        {
            bool isCover = this.backupImage.Key.Contains(Settings.TemporaryFolder + "obalkyknih-cover_");

            // get guid of backuped image from imagePaths or return empty guid
            Guid guid = (from record in this.imagesFilePaths
                         where record.Value.Equals(this.backupImage.Key)
                         select record.Key).SingleOrDefault();

            // if empty guid, create new one with new file path
            if (guid == Guid.Empty)
            {
                while (this.imagesFilePaths.ContainsKey(guid) || guid == Guid.Empty)
                {
                    guid = Guid.NewGuid();
                }

                string newFileName = Settings.TemporaryFolder +
                ((isCover) ? "obalkyknih-cover_" : "obalkyknih-toc_")
                + "_" + guid + ".tif";
                this.imagesFilePaths.Add(guid, newFileName);
            }

            // reset redo
            this.redoImage = new KeyValuePair<string, BitmapSource>(null, null);

            // file was changed
            if (this.imagesFilePaths.ContainsKey(this.workingImage.Key) &&
                this.imagesFilePaths[this.workingImage.Key].Equals(this.backupImage.Key))
            {
                // save current version to redo
                this.redoImage = new KeyValuePair<string, BitmapSource>
                    (this.backupImage.Key, this.workingImage.Value);

                // renew current version from backup
                this.workingImage = new KeyValuePair<Guid, BitmapSource>
                    (guid, this.backupImage.Value);

                // set new OriginalSize
                this.imagesOriginalSizes.Remove(guid);
                this.imagesOriginalSizes.Add(guid, new Size(this.workingImage.Value.PixelWidth,
                    this.workingImage.Value.PixelHeight));

                Image thumbnail;
                if (isCover)
                {
                    thumbnail = this.coverThumbnail;
                }
                else
                {
                    thumbnail = LogicalTreeHelper.FindLogicalNode(
                        this.tocThumbnailGridsDictionary[guid], "tocThumbnail") as Image;
                }
                thumbnail.Source = ImageTools.LoadGivenSizeFromBitmapSource(
                        this.workingImage.Value, 800);
                Thumbnail_Clicked(thumbnail, null);
            }
            // file was deleted
            else if (isCover)
            {
                // cover image was replaced, so put current cover to redo
                if (this.imagesFilePaths.ContainsKey(this.workingImage.Key)
                    && this.coverGuid == this.workingImage.Key && this.coverGuid != Guid.Empty)
                {
                    this.redoImage = new KeyValuePair<string, BitmapSource>
                        (this.imagesFilePaths[guid], this.workingImage.Value);
                }

                // load backup to working image
                this.workingImage = new KeyValuePair<Guid, BitmapSource>(guid, this.backupImage.Value);

                // set new OriginalSize
                this.imagesOriginalSizes.Remove(guid);
                this.imagesOriginalSizes.Add(guid, new Size(this.workingImage.Value.PixelWidth,
                    this.workingImage.Value.PixelHeight));

                AddCoverImage(ImageTools.LoadGivenSizeFromBitmapSource(this.workingImage.Value, 800),
                        guid);

            }

            // reset backup
            this.backupImage = new KeyValuePair<string, BitmapSource>(null, null);

            (Window.GetWindow(this) as MainWindow).DeactivateUndo();
            if (this.redoImage.Value != null)
            {
                (Window.GetWindow(this) as MainWindow).ActivateRedo();
            }
        }

        internal void RedoLastStep()
        {
            bool isCover = this.redoImage.Key.Contains(Settings.TemporaryFolder + "obalkyknih-cover_");

            // get guid of backuped image from imagePaths or return empty guid
            Guid guid = (from record in this.imagesFilePaths
                         where record.Value.Equals(this.redoImage.Key)
                         select record.Key).SingleOrDefault();

            // if empty guid, create new one with new file path
            if (guid == Guid.Empty)
            {
                while (this.imagesFilePaths.ContainsKey(guid) || guid == Guid.Empty)
                {
                    guid = Guid.NewGuid();
                }

                string newFileName = Settings.TemporaryFolder +
                ((isCover) ? "obalkyknih-cover_" : "obalkyknih-toc_")
                + "_" + guid + ".tif";
                this.imagesFilePaths.Add(guid, newFileName);
            }

            // reset redo
            this.backupImage = new KeyValuePair<string, BitmapSource>(null, null);

            // file was changed
            if (this.imagesFilePaths.ContainsKey(this.workingImage.Key) &&
                this.imagesFilePaths[this.workingImage.Key].Equals(this.redoImage.Key))
            {
                // save current version to backup
                this.backupImage = new KeyValuePair<string, BitmapSource>
                    (this.redoImage.Key, this.workingImage.Value);

                // renew current version from redo
                this.workingImage = new KeyValuePair<Guid, BitmapSource>
                    (guid, this.redoImage.Value);

                // set new OriginalSize
                this.imagesOriginalSizes.Remove(guid);
                this.imagesOriginalSizes.Add(guid, new Size(this.workingImage.Value.PixelWidth,
                    this.workingImage.Value.PixelHeight));

                Image thumbnail;
                if (isCover)
                {
                    thumbnail = this.coverThumbnail;
                }
                else
                {
                    thumbnail = LogicalTreeHelper.FindLogicalNode(
                        this.tocThumbnailGridsDictionary[guid], "tocThumbnail") as Image;

                }
                thumbnail.Source = ImageTools.LoadGivenSizeFromBitmapSource(
                        this.workingImage.Value, 800);
                Thumbnail_Clicked(thumbnail, null);
            }
            // cover was replaced
            else if (isCover)
            {
                // save working image to file
                if (this.imagesFilePaths.ContainsKey(this.workingImage.Key)
                    && this.coverGuid == this.workingImage.Key && this.coverGuid != Guid.Empty)
                {
                    this.redoImage = new KeyValuePair<string, BitmapSource>
                        (this.imagesFilePaths[guid], this.workingImage.Value);
                }

                // load backup to working image
                this.workingImage = new KeyValuePair<Guid, BitmapSource>(guid, this.backupImage.Value);

                // set new OriginalSize
                this.imagesOriginalSizes.Remove(guid);
                this.imagesOriginalSizes.Add(guid, new Size(this.workingImage.Value.PixelWidth,
                    this.workingImage.Value.PixelHeight));

                AddCoverImage(ImageTools.LoadGivenSizeFromBitmapSource(this.workingImage.Value, 800),
                        guid);
            }

            // reset redo
            this.redoImage = new KeyValuePair<string, BitmapSource>(null, null);

            (Window.GetWindow(this) as MainWindow).DeactivateRedo();
            if (this.backupImage.Value != null)
            {
                (Window.GetWindow(this) as MainWindow).ActivateUndo();
            }
        }
        #endregion

        // Resetuje uzivatelske ovladacie prvky progresu kopirovania
        private void clearGuiProgress()
        {
            controlTabDoneIcon.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-hourglass.png");
            zobrazitVysledek.Visibility = Visibility.Hidden;
            stepIcon1.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-hourglass.png");
            stepIcon2.Visibility = Visibility.Hidden;
            stepIcon3.Visibility = Visibility.Hidden;
            stepIcon4.Visibility = Visibility.Hidden;
            stepIcon5.Visibility = Visibility.Hidden;
            step5Finish.Visibility = Visibility.Hidden;
            copyProgress.Value = 0;
            copyPercent.Content = "";
            copyTime.Content = "";
            copySize.Content = "";
            copyPercent.Content = "";
        }

        // Start archivation
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            this.tmpMediaHash = "";
            this.workingMediaId = "";
            clearGuiProgress();
            SendToObalkyKnih();
        }

        // Send cover,toc
        private void SendCoverToc_Click(object sender, RoutedEventArgs e)
        {
            SendCoverToc();
        }

        // Creates hover effect for transormation controllers
        private void Icon_MouseEnter(object sender, EventArgs e)
        {
            (sender as UIElement).Opacity = 0.7;
        }

        // Creates hover effect for transormation controllers
        private void Icon_MouseLeave(object sender, EventArgs e)
        {
            (sender as UIElement).Opacity = 1;
        }

        // Disables image editing controllers
        private void DisableImageControllers()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            this.rotateRightIcon.IsEnabled = false;
            this.rotate180Icon.IsEnabled = false;
            this.deskewIcon.IsEnabled = false;
            this.flipIcon.IsEnabled = false;
            this.cropIcon.IsEnabled = false;
            this.brightnessSlider.IsEnabled = false;
            this.contrastSlider.IsEnabled = false;
            this.gammaSlider.IsEnabled = false;
            this.sliderConfirmButton.IsEnabled = false;
        }

        // Enables image editing controllers
        private void EnableImageControllers()
        {
            Mouse.OverrideCursor = null;
            this.rotateLeftIcon.IsEnabled = true;
            this.rotateRightIcon.IsEnabled = true;
            this.rotate180Icon.IsEnabled = true;
            this.deskewIcon.IsEnabled = true;
            this.flipIcon.IsEnabled = true;
            this.cropIcon.IsEnabled = true;
            this.brightnessSlider.IsEnabled = true;
            this.contrastSlider.IsEnabled = true;
            this.gammaSlider.IsEnabled = true;
            this.sliderConfirmButton.IsEnabled = true;
            this.brightnessSlider.Value = 0;
            this.contrastSlider.Value = 0;
            this.gammaSlider.Value = 1;
        }

        private void SelectedImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetAppropriateCrop(e.PreviousSize, e.NewSize, false);
        }

        private void SetAppropriateCrop(Size previousSize, Size newSize, bool calculateCropZone)
        {
            if (this.selectedImageGuid == Guid.Empty)
            {
                AdornerLayer aly = AdornerLayer.GetAdornerLayer(this.selectedImage);
                if (aly.GetAdorners(this.selectedImage) != null)
                {
                    foreach (var adorner in aly.GetAdorners(this.selectedImage))
                    {
                        aly.Remove(adorner);
                    }
                }
                return;
            }
            BitmapSource source = this.selectedImage.Source as BitmapSource;
            double ratioNewSizeToThumbX = newSize.Width / source.PixelWidth;
            double ratioNewSizeToThumbY = newSize.Height / source.PixelHeight;

            // Coordinates where started previous cropZone
            Point previousCropZoneFrom = (this.cropper == null) ? new Point() :
                new Point(this.cropper.ClippingRectangle.X, this.cropper.ClippingRectangle.Y);
            // Size of previous cropZone
            Size previousCropZoneSize = (this.cropper == null) ? Size.Empty :
                new Size(this.cropper.ClippingRectangle.Width, this.cropper.ClippingRectangle.Height);
            // size of cropZone in full (not scaled) image
            Size originalSizeCropZone = (this.cropper == null) ? Size.Empty : this.cropper.CropZone;
            // real size of image (pixel size of this image on disk)
            Size originalSourceSize = this.imagesOriginalSizes[this.selectedImageGuid];
            // display size of cropZone, (pixel size of cropZone as displayed on screen) 

            Size newCropZoneSize = Size.Empty;
            Point newCropZoneFrom = new Point(0, 0);

            if (this.cropper == null) //adding first image
            {
                Rect calculatedCrop = ImageTools.FindCropZone(source);
                newCropZoneFrom = new Point(ratioNewSizeToThumbX * calculatedCrop.X, ratioNewSizeToThumbY * calculatedCrop.Y);
                newCropZoneSize = new Size(ratioNewSizeToThumbX * calculatedCrop.Width, ratioNewSizeToThumbY * calculatedCrop.Height);
            }
            else // adding image other than first
            {
                if (calculateCropZone) // try to automatically determine crop zone
                {
                    Rect calculatedCrop = ImageTools.FindCropZone(source);
                    if (originalSizeCropZone.Equals(Size.Empty) || originalSizeCropZone.Equals(new Size(0, 0)))
                    {
                        newCropZoneFrom = new Point(ratioNewSizeToThumbX * calculatedCrop.X, ratioNewSizeToThumbY * calculatedCrop.Y);
                        newCropZoneSize = new Size(ratioNewSizeToThumbX * calculatedCrop.Width, ratioNewSizeToThumbY * calculatedCrop.Height);

                    }
                    else
                    {

                        // count new X and Y coordinates for start of crop zone
                        double ratioOrigX = (double)originalSourceSize.Width / source.PixelWidth;
                        double ratioOrigY = (double)originalSourceSize.Height / source.PixelHeight;

                        double newFromX = ((ratioOrigX * calculatedCrop.X + originalSizeCropZone.Width) > originalSourceSize.Width) ?
                            (originalSourceSize.Width - originalSizeCropZone.Width) * (newSize.Width / originalSourceSize.Width)
                            : ratioNewSizeToThumbX * calculatedCrop.X;
                        double newFromY = ((ratioOrigY * calculatedCrop.Y + originalSizeCropZone.Height) > originalSourceSize.Height) ?
                            (originalSourceSize.Height - originalSizeCropZone.Height) * (newSize.Height / originalSourceSize.Height)
                            : ratioNewSizeToThumbY * calculatedCrop.Y;
                        newCropZoneFrom = new Point(newFromX, newFromY);
                        // set width and height of crop
                        newCropZoneSize = new Size((newSize.Width / originalSourceSize.Width) * originalSizeCropZone.Width,
                            (newSize.Height / originalSourceSize.Height) * originalSizeCropZone.Height);
                    }
                }
                else //image was resized, don't change crop zone, only resize it
                {
                    // set X and Y of crop
                    newCropZoneFrom = new Point((newSize.Width / previousSize.Width) * previousCropZoneFrom.X,
                        (newSize.Height / previousSize.Height) * previousCropZoneFrom.Y);
                    // set width and height of crop
                    newCropZoneSize = new Size((newSize.Width / previousSize.Width) * previousCropZoneSize.Width,
                        (newSize.Height / previousSize.Height) * previousCropZoneSize.Height);
                }
            }

            // limit to size of image
            if (newCropZoneSize.Height + newCropZoneFrom.Y > newSize.Height)
            {
                newCropZoneSize.Height = newSize.Height - newCropZoneFrom.Y;
            }
            if (newCropZoneSize.Width + newCropZoneFrom.X > newSize.Width)
            {
                newCropZoneSize.Width = newSize.Width - newCropZoneFrom.X;
            }

            ImageTools.AddCropToElement(this.selectedImage, ref this.cropper,
                    new Rect(newCropZoneFrom, newCropZoneSize));
        }

        private void SignalLoadedBackup()
        {
            this.redoImage = new KeyValuePair<string, BitmapSource>(null, null);
            (Window.GetWindow(this) as MainWindow).ActivateUndo();
            (Window.GetWindow(this) as MainWindow).DeactivateRedo();
        }
        #endregion

        #region control tab controls

        // Shows windows for barcode
        private void controlNewUnitButton_Click(object sender, RoutedEventArgs e)
        {
            // reset media tmp data
            tmpQuickId = "";
            lastMediaInfo = null;
            tmpMediumReadProblem = false;
            tmpForcedUpload = false;
            mediaInfoExists.Visibility = Visibility.Hidden;
            mediaInfoNew.Visibility = Visibility.Hidden;
            discName.Content = "není vložené";
            discVolno.Content = "";
            discSize.Content = "";
            closeArchive();
            (Window.GetWindow(this) as MainWindow).ShowNewUnitWindow();
        }

        // Writes metadata for output checking in controlTabItem
        private void FillControlMetadata()
        {
            int counter = 0;

            this.controlTitle.Content = this.partTitleTextBox.Text;
            this.controlAuthors.Content = this.partAuthorTextBox.Text;
            this.controlYear.Content = this.partYearTextBox.Text;

            #region identifiers
            if (!string.IsNullOrWhiteSpace(this.partIsbnTextBox.Text))
            {
                CreateIdentifierLabel("ISBN", this.partIsbnTextBox.Text, counter);
                counter++;
            }

            //issn

            if (!string.IsNullOrWhiteSpace(this.partCnbTextBox.Text))
            {
                CreateIdentifierLabel("ČNB", this.partCnbTextBox.Text, counter);
                counter++;
            }

            if (!string.IsNullOrWhiteSpace(this.partOclcTextBox.Text))
            {
                CreateIdentifierLabel("OCLC", this.partOclcTextBox.Text, counter);
                counter++;
            }

            if (!string.IsNullOrWhiteSpace(this.partIsmnTextBox.Text))
            {
                CreateIdentifierLabel("ISMN", this.partIsmnTextBox.Text, counter);
                counter++;
            }

            if (!string.IsNullOrWhiteSpace(this.partUrnNbnTextBox.Text))
            {
                CreateIdentifierLabel("URN:NBN", this.partUrnNbnTextBox.Text, counter);
                counter++;
            }

            if (!string.IsNullOrWhiteSpace(this.partSiglaTextBox.Text))
            {
                CreateIdentifierLabel("Vlastní", Settings.Sigla + "-" + this.partSiglaTextBox.Text, counter);
                counter++;
            }

            if (!string.IsNullOrWhiteSpace(this.partNumberTextBox.Text))
            {
                CreateIdentifierLabel("Číslo části", this.partNumberTextBox.Text, counter);
                counter++;
            }

            if (!string.IsNullOrWhiteSpace(this.partNameTextBox.Text))
            {
                CreateIdentifierLabel("Název části", this.partNameTextBox.Text, counter);
                counter++;
            }

            if (!string.IsNullOrWhiteSpace(this.partVolumeTextBox.Text))
            {
                CreateIdentifierLabel("Ročník vydání", this.partVolumeTextBox.Text, counter);
                counter++;
            }
            #endregion
        }

        private void CreateIdentifierLabel(string identifierName, string identifierValue, int counter)
        {
            Label label = new Label();
            label.FontFamily = (FontFamily)(new FontFamilyConverter()).ConvertFromInvariantString("Arial");
            label.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#858585"));
            label.Margin = new Thickness(0, counter * 20, 0, 0);
            label.Content = identifierName;

            Label label2 = new Label();
            label2.FontFamily = (FontFamily)(new FontFamilyConverter()).ConvertFromInvariantString("Arial");
            label2.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#cecece"));
            label2.Margin = new Thickness(70, counter * 20, 0, 0);
            label2.Content = identifierValue;

            this.controlIdentifiersGrid.Children.Add(label);
            this.controlIdentifiersGrid.Children.Add(label2);
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Keyboard.Focus(this.tabControl.SelectedItem as TabItem);
            MainWindow mainwin = Window.GetWindow(this) as MainWindow;

            if (this.tabControl.SelectedItem != null &&
                "scanningTabItem".Equals((this.tabControl.SelectedItem as TabItem).Name))
            {
                if (this.backupImage.Key != null)
                {
                    (Window.GetWindow(this) as MainWindow).ActivateUndo();
                }

                if (this.redoImage.Key != null)
                {
                    (Window.GetWindow(this) as MainWindow).ActivateRedo();
                }
            }
            else if (mainwin != null)
            {
                mainwin.DeactivateUndo();
                mainwin.DeactivateRedo();
            }
        }

        // shows fields for union (only for monograph)
        private void addUnionButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.generalRecord is Periodical)
            {
                return;
            }

            if (this.recordGrid.ColumnDefinitions.Count < 1)
            {
                CreateNewUnionWindow unionWindow = new CreateNewUnionWindow(this.generalRecord);
                unionWindow.ShowDialog();
                if (unionWindow.DialogResult.HasValue && unionWindow.DialogResult.Value)
                {
                    GeneralRecord generalRecord = unionWindow.GeneralRecord;
                    (this.generalRecord as Monograph).IsUnionRequested = true;
                    (Window.GetWindow(this) as MainWindow).AddMessageToStatusBar("Stahuji metadata.");
                    metadataReceiverBackgroundWorker.RunWorkerAsync(this.generalRecord);
                }
            }
            else
            {
                SetUnionMetadataTab(false, null);
            }
        }

        // add or remove union metadata tab
        private void SetUnionMetadataTab(bool addTab, List<Metadata> metadataList)
        {
            if (addTab)
            {
                if (metadataList.Count == 1)
                {
                    // one union metadata record
                    this.generalRecord.ImportFromMetadata(metadataList[0]);
                    FillTextboxes();
                }
                else
                {
                    // multiple union metadata records
                    MetadataWindow metadataWindow = new MetadataWindow(metadataList, 0);
                    metadataWindow.ShowDialog();

                    if (metadataWindow.DialogResult.HasValue && metadataWindow.DialogResult.Value)
                    {
                        this.metadataIndex = metadataWindow.MetadataIndex;
                        int idx = metadataWindow.reorder[this.metadataIndex];
                        this.generalRecord.ImportFromMetadata(metadataList[idx]);
                        FillTextboxes();
                    }
                    else
                        return;
                }

                (this.generalRecord as Monograph).IsUnionRequested = true;
                showUnionTab();
            }
            else
                hideUnionTab();
        }

        public void showUnionTab()
        {
            if (!this.unionTabVisible)
            {
                this.metadataCD.Width = (this.metadataCD.ActualWidth < this.tabControl.ActualWidth - 10) ?
                        new GridLength(this.metadataCD.ActualWidth * 2) : new GridLength(this.tabControl.ActualWidth - 10);
                this.collectionRecordGrid.Visibility = System.Windows.Visibility.Visible;
                this.collectionRecordGrid.IsEnabled = true;
                ColumnDefinition cd1 = new ColumnDefinition();
                cd1.Width = new GridLength(1, GridUnitType.Star);
                ColumnDefinition cd2 = new ColumnDefinition();
                cd2.Width = new GridLength(1, GridUnitType.Star);
                this.recordGrid.ColumnDefinitions.Add(cd1);
                this.recordGrid.ColumnDefinitions.Add(cd2);
                this.metadataCD.MinWidth = this.metadataCD.MinWidth * 2;
                this.partNameTextBox.Visibility = Visibility.Visible;
                this.partNameLabel.Visibility = Visibility.Visible;
                this.partNameTextBox.IsEnabled = true;
                this.partNumberTextBox.Visibility = Visibility.Visible;
                this.partNumberLabel.Visibility = Visibility.Visible;
                this.partNumberTextBox.IsEnabled = true;
                this.addUnionButton.Content = "Odpojit souborný záznam";
                this.unionTabVisible = true;
            }
        }

        public void hideUnionTab()
        {
            if (this.unionTabVisible)
            {
                this.metadataCD.Width = new GridLength(metadataCD.ActualWidth / 2);
                this.collectionRecordGrid.Visibility = Visibility.Collapsed;
                this.collectionRecordGrid.IsEnabled = false;
                this.recordGrid.ColumnDefinitions.RemoveRange(0, this.recordGrid.ColumnDefinitions.Count);
                this.metadataCD.MinWidth = this.metadataCD.MinWidth / 2;
                this.partNameTextBox.Visibility = Visibility.Hidden;
                this.partNameLabel.Visibility = Visibility.Hidden;
                this.partNameTextBox.IsEnabled = false;
                this.partNumberTextBox.Visibility = Visibility.Hidden;
                this.partNumberLabel.Visibility = Visibility.Hidden;
                this.partNumberTextBox.IsEnabled = false;
                this.partCnbTextBox.Visibility = Visibility.Visible;
                this.partCnbLabel.Visibility = Visibility.Visible;
                this.partCnbTextBox.IsEnabled = true;
                this.partOclcTextBox.Visibility = Visibility.Visible;
                this.partOclcLabel.Visibility = Visibility.Visible;
                this.partOclcTextBox.IsEnabled = true;
                this.addUnionButton.Content = "Připojit souborný záznam";
                this.unionTabVisible = false;
                this.partCnbTextBox.Text = this.generalRecord.Cnb ?? (this.generalRecord as Monograph).PartCnb;
                this.partOclcTextBox.Text = this.generalRecord.Oclc ?? (this.generalRecord as Monograph).PartOclc;
                this.multipartIdentifierUnion.SelectedIndex = 0;
            }
        }

        //TODO switch identifiers
        private void PartImageWarning_Click(object sender, MouseButtonEventArgs e)
        {
            Image clicked = (Image)sender;
            switch (clicked.Name)
            {
                case "partIsbnWarning":
                    if (string.IsNullOrWhiteSpace(this.partIsbnTextBox.Text))
                        this.partIsbnTextBox.Text = this.isbnTextBox.Text;
                    else if (string.IsNullOrWhiteSpace(this.isbnTextBox.Text))
                        this.isbnTextBox.Text = this.partIsbnTextBox.Text;
                    else
                    {
                        string tmp = this.isbnTextBox.Text;
                        this.isbnTextBox.Text = this.partIsbnTextBox.Text;
                        this.partIsbnTextBox.Text = tmp;
                    }
                    break;
                case "partCnbWarning":
                    if (string.IsNullOrWhiteSpace(this.partCnbTextBox.Text))
                        this.partCnbTextBox.Text = this.cnbTextBox.Text;
                    else if (string.IsNullOrWhiteSpace(this.cnbTextBox.Text))
                        this.cnbTextBox.Text = this.partCnbTextBox.Text;
                    else
                    {
                        string tmp = this.cnbTextBox.Text;
                        this.cnbTextBox.Text = this.partCnbTextBox.Text;
                        this.partCnbTextBox.Text = tmp;
                    }
                    break;
                case "partOclcWarning":
                    if (string.IsNullOrWhiteSpace(this.partOclcTextBox.Text))
                        this.partOclcTextBox.Text = this.oclcTextBox.Text;
                    else if (string.IsNullOrWhiteSpace(this.oclcTextBox.Text))
                        this.oclcTextBox.Text = this.partOclcTextBox.Text;
                    else
                    {
                        string tmp = this.oclcTextBox.Text;
                        this.oclcTextBox.Text = this.partOclcTextBox.Text;
                        this.partOclcTextBox.Text = tmp;
                    }
                    break;
                case "partIsmnWarning":
                    if (string.IsNullOrWhiteSpace(this.partIsmnTextBox.Text))
                        this.partIsmnTextBox.Text = this.ismnTextBox.Text;
                    else if (string.IsNullOrWhiteSpace(this.ismnTextBox.Text))
                        this.ismnTextBox.Text = this.partIsmnTextBox.Text;
                    else
                    {
                        string tmp = this.ismnTextBox.Text;
                        this.ismnTextBox.Text = this.partIsmnTextBox.Text;
                        this.partIsmnTextBox.Text = tmp;
                    }
                    break;
                case "partUrnWarning":
                    if (string.IsNullOrWhiteSpace(this.partUrnNbnTextBox.Text))
                        this.partUrnNbnTextBox.Text = this.urnNbnTextBox.Text;
                    else if (string.IsNullOrWhiteSpace(this.urnNbnTextBox.Text))
                        this.urnNbnTextBox.Text = this.partUrnNbnTextBox.Text;
                    else
                    {
                        string tmp = this.urnNbnTextBox.Text;
                        this.urnNbnTextBox.Text = this.partUrnNbnTextBox.Text;
                        this.partUrnNbnTextBox.Text = tmp;
                    }
                    break;
                case "partSiglaWarning":
                    if (string.IsNullOrWhiteSpace(this.partSiglaTextBox.Text))
                        this.partSiglaTextBox.Text = this.siglaTextBox.Text;
                    else if (string.IsNullOrWhiteSpace(this.siglaTextBox.Text))
                        this.siglaTextBox.Text = this.partSiglaTextBox.Text;
                    else
                    {
                        string tmp = this.siglaTextBox.Text;
                        this.siglaTextBox.Text = this.partSiglaTextBox.Text;
                        this.partSiglaTextBox.Text = tmp;
                    }
                    break;
            }
        }

        private void checkboxMinorPartName_Click(object sender, RoutedEventArgs e)
        {
            checkboxMinorChanger((bool)this.checkboxMinorPartName.IsChecked);
        }

        private void checkboxMinorChanger(bool isChecked)
        {
            if (!this.unionTabVisible) return;
            if (isChecked)
            {
                if (this.partCnbTextBox.Text != "") this.cnbTextBox.Text = this.partCnbTextBox.Text;
                if (this.partOclcTextBox.Text != "") this.oclcTextBox.Text = this.partOclcTextBox.Text;
                if (this.partIsmnTextBox.Text != "") this.ismnTextBox.Text = this.partIsmnTextBox.Text;
                if (this.partUrnNbnTextBox.Text != "") this.urnNbnTextBox.Text = this.partUrnNbnTextBox.Text;
                this.partCnbTextBox.Text = ""; this.partOclcTextBox.Text = ""; this.partIsmnTextBox.Text = ""; this.partUrnNbnTextBox.Text = "";
                this.partCnbTextBox.IsEnabled = false;
                this.partCnbTextBox.Visibility = Visibility.Hidden;
                this.partCnbLabel.Visibility = Visibility.Hidden;
                this.partCnbWarning.Visibility = Visibility.Hidden;
                this.partOclcTextBox.IsEnabled = false;
                this.partOclcTextBox.Visibility = Visibility.Hidden;
                this.partOclcLabel.Visibility = Visibility.Hidden;
                this.partOclcWarning.Visibility = Visibility.Hidden;
            }
            else
            {
                this.partCnbTextBox.IsEnabled = true;
                this.partCnbTextBox.Visibility = Visibility.Visible;
                this.partCnbLabel.Visibility = Visibility.Visible;
                this.partCnbWarning.Visibility = Visibility.Visible;
                this.partOclcTextBox.IsEnabled = true;
                this.partOclcTextBox.Visibility = Visibility.Visible;
                this.partOclcLabel.Visibility = Visibility.Visible;
                this.partOclcWarning.Visibility = Visibility.Visible;
                if (this.cnbTextBox.Text != "") this.partCnbTextBox.Text = this.cnbTextBox.Text;
                if (this.oclcTextBox.Text != "") this.partOclcTextBox.Text = this.oclcTextBox.Text;
                if (this.ismnTextBox.Text != "") this.partIsmnTextBox.Text = this.ismnTextBox.Text;
                if (this.urnNbnTextBox.Text != "") this.partUrnNbnTextBox.Text = this.urnNbnTextBox.Text;
                this.cnbTextBox.Text = ""; this.oclcTextBox.Text = ""; this.ismnTextBox.Text = ""; this.urnNbnTextBox.Text = "";
            }
        }

        private void checkboxMultipartChanger(bool isMonography)
        {
            if (isMonography)
            {
                var tmpRecord = this.generalRecord;
                this.checkboxMinorPartName.IsEnabled = true;
                this.multipartIdentifierUnion.IsEnabled = true;
                this.titleTextBox.Text = tmpRecord.Title == null ? (tmpRecord as Monograph).PartTitle : tmpRecord.Title;
                this.authorTextBox.Text = tmpRecord.Authors == null ? (tmpRecord as Monograph).PartAuthors : tmpRecord.Authors;
                this.isbnTextBox.Text = (tmpRecord as Monograph).Isbn;
                this.yearTextBox.Text = tmpRecord.Year == null ? (tmpRecord as Monograph).PartYear : tmpRecord.Year;
                this.cnbTextBox.Text = tmpRecord.Cnb == null ? (tmpRecord as Monograph).PartCnb : tmpRecord.Cnb;
                this.oclcTextBox.Text = tmpRecord.Oclc == null ? (tmpRecord as Monograph).PartOclc : tmpRecord.Oclc;
                this.ismnTextBox.Text = tmpRecord.Ismn == null ? (tmpRecord as Monograph).PartIsmn : tmpRecord.Ismn;
                this.urnNbnTextBox.Text = tmpRecord.Urn == null ? (tmpRecord as Monograph).PartUrn : tmpRecord.Urn;
                this.partNameTextBox.Text = (this.generalRecord as Monograph).PartName;
                this.partNumberLabel.Visibility = Visibility.Visible;
                this.partNumberTextBox.Visibility = Visibility.Visible;
                this.partNameLabel.Visibility = Visibility.Visible;
                this.partNameTextBox.Visibility = Visibility.Visible;
                checkboxMinorChanger((bool)this.checkboxMinorPartName.IsChecked);
                showUnionTab();
            }
            else
            {
                var tmpRecord = this.generalRecord;
                this.checkboxMinorPartName.IsEnabled = false;
                this.multipartIdentifierUnion.IsEnabled = false;
                this.titleTextBox.Text = ""; this.authorTextBox.Text = ""; this.yearTextBox.Text = ""; this.isbnTextBox.Text = ""; this.cnbTextBox.Text = ""; this.oclcTextBox.Text = ""; this.ismnTextBox.Text = ""; this.urnNbnTextBox.Text = ""; this.partNameTextBox.Text = "";
                this.partTitleTextBox.Text = tmpRecord.Title;
                this.partAuthorTextBox.Text = tmpRecord.Authors;
                this.partYearTextBox.Text = tmpRecord.Year;
                this.partCnbTextBox.Text = tmpRecord.Cnb;
                this.partOclcTextBox.Text = tmpRecord.Oclc;
                this.partIsmnTextBox.Text = tmpRecord.Ismn;
                this.partUrnNbnTextBox.Text = tmpRecord.Urn;
                this.partNumberLabel.Visibility = Visibility.Hidden;
                this.partNumberTextBox.Visibility = Visibility.Hidden;
                this.partNameLabel.Visibility = Visibility.Hidden;
                this.partNameTextBox.Visibility = Visibility.Hidden;
                hideUnionTab();
            }
        }

        private void radioMultipartCoedition_Click(object sender, RoutedEventArgs e)
        {
            checkboxMinorChanger(false);
            checkboxMultipartChanger(false);
        }

        private void radioMultipartMonography_Click(object sender, RoutedEventArgs e)
        {
            checkboxMultipartChanger(true);
        }

        private void multipartIdentifierOwn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //update part ISBN textbox and reload cover and toc
            var tmpRecord = this.generalRecord as Monograph;
            var selected = this.multipartIdentifierOwn.SelectedIndex;
            if (selected == -1) return;
            this.partIsbnTextBox.Text = tmpRecord.listIdentifiers[selected].IdentifierCode;
        }

        private void multipartIdentifierUnion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //update union ISBN textbox
            var tmpRecord = this.generalRecord as Monograph;
            var selected = this.multipartIdentifierUnion.SelectedIndex;
            if (selected == -1) return;
            this.isbnTextBox.Text = tmpRecord.listIdentifiers[selected].IdentifierCode;
            if (selected > 0) showUnionTab();
            this.checkboxMinorChanger((bool)this.checkboxMinorPartName.IsChecked);
        }

        private void controlSameUnitButton_Click(object sender, RoutedEventArgs e)
        {
            // switch to first tab
            tabControl.SelectedItem = this.metadataTabItem;

            // reset media tmp data
            tmpQuickId = "";
            lastMediaInfo = null;
            tmpMediumReadProblem = false;
            tmpForcedUpload = false;
            mediaInfoExists.Visibility = Visibility.Hidden;
            mediaInfoNew.Visibility = Visibility.Hidden;
            discName.Content = "není vložené";
            discVolno.Content = "";
            discSize.Content = "";

            // remove cover
            if (this.coverGuid != new Guid())
            {
                /*bool settingsOrig = Settings.DisableCoverDeletionNotification;
                Settings.DisableCoverDeletionNotification = true;
                CoverThumbnail_Delete(null, null);
                Settings.DisableCoverDeletionNotification = settingsOrig;*/
                CoverThumbnail_DeleteNoRemove();
            }

            // remove toc
            int cnt = this.tocImagesList.Items.Count;
            for (int i = 0; i < cnt; i++)
            {
                this.tocImagesList.SelectedIndex = i;
                TocThumbnail_DeleteNoRemove();
                //   TocThumbnail_Delete(null, null);
            }

            // cleanup part info
            this.partNumberTextBox.Text = null;
            this.partNameTextBox.Text = null;
        }

        private void reloadCoverAndToc(object sender, RoutedEventArgs e)
        {
            this.scanningStartButton.Content = "SKENOVAT";
            this.scanningStartButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF6C3131"));
            this.DownloadCoverAndToc();
        }

        private void optionalAtributesLink_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // show optional fields
            this.partCnbTextBox.Visibility = Visibility.Visible;
            this.partOclcTextBox.Visibility = Visibility.Visible;
            this.partIsmnTextBox.Visibility = Visibility.Visible;
            this.partUrnNbnTextBox.Visibility = Visibility.Visible;
            this.partCnbLabel.Visibility = Visibility.Visible;
            this.partOclcLabel.Visibility = Visibility.Visible;
            this.partIsmnLabel.Visibility = Visibility.Visible;
            this.partUrnNbnLabel.Visibility = Visibility.Visible;
            this.optionalAtributesLink.Visibility = Visibility.Hidden;
        }

        private void scanAuthorA3_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ScanButtonClicked(DocumentType.Auth, "A3");
        }

        private void PdfLoadButtonClicked(object sender, MouseButtonEventArgs e)
        {
            //unused
        }

        private void zobrazitVysledek_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.workingMediaId) && !Settings.offlineMode)
            {
                System.Diagnostics.Process.Start(Settings.ViewLink + "?media_id=" + this.workingMediaId);
            }
            else
            {
                System.IO.Directory.CreateDirectory(Settings.TmpDir);
                Process.Start(@Settings.TmpDir);
            }
        }
        #endregion

        private async void loadMedia_Click(object sender, RoutedEventArgs e)
        {
            tmpQuickId = "";
            lastMediaInfo = null;
            mediaInfoNew.Visibility = Visibility.Hidden;
            mediaInfoExists.Visibility = Visibility.Hidden;

            String driveName = this.driveList.Text;
            driveName = driveName.Remove(driveName.Length - 1);
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_LogicalDisk WHERE DeviceID = \"" + driveName + "\"");
                DriveInfo driveInfo = new DriveInfo(System.IO.Path.GetPathRoot(driveName));

                if (!driveInfo.IsReady)
                {
                    btnCreateWarning.Content = "Médium není načtené";
                    btnCreateWarning.Visibility = Visibility.Visible;
                    btnCreateWarningIcon.Visibility = Visibility.Visible;
                    mediaInfoExists.Visibility = Visibility.Hidden;
                    mediaInfoNew.Visibility = Visibility.Hidden;
                    discName.Content = "není vložené";
                    mediaInfoDateUpdate.Content = mediaInfoTitle.Content = mediaInfoAuthors.Content = mediaInfoYear.Content = mediaInfoSize.Content = discSize.Content = discVolno.Content = "";
                    return;
                }

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    //Double size = Convert.ToDouble(queryObj["Size"]);
                    Double size = Convert.ToDouble(driveInfo.TotalSize) - Convert.ToDouble(driveInfo.TotalFreeSpace);
                    String sizeText = "";
                    if (size > 999999999)
                    {
                        double sizeNumeric = size / 1000000000;
                        sizeText = sizeNumeric.ToString("0.00") + " GB";
                    }
                    else if (size > 999999)
                    {
                        double sizeNumeric = size / 1000000;
                        sizeText = sizeNumeric.ToString("0.00") + " MB";
                    }
                    else
                    {
                        sizeText = size.ToString() + " kB";
                    }
                    String serno = "";
                    if (queryObj["VolumeSerialNumber"] != null) serno = queryObj["VolumeSerialNumber"].ToString();
                    if (serno == "") serno = driveInfo.VolumeLabel;
                    this.discName.Content = driveInfo.VolumeLabel;
                    this.discSize.Content = sizeText;
                    this.discVolno.Content = serno;
                    tmpQuickId = serno + size.ToString();
                }

                // skry warning nevlozeneho media, prave bylo vlozene
                if (tmpQuickId != "" && !Settings.offlineMode)
                {
                    btnCreateWarning.Visibility = Visibility.Hidden;
                    btnCreateWarningIcon.Visibility = Visibility.Hidden;

                    // ziskat informace o mediu ze serveru na zaklade rychleho ID media
                    var content = await GetData(Settings.GetMediaLink + "/?quickid=" + tmpQuickId);

                    lastMediaInfo = JsonConvert.DeserializeObject<GetmediaJsonObject>(content);
                    if (lastMediaInfo.size != null)
                    {
                        mediaInfoExists.Visibility = Visibility.Visible;
                        mediaInfoDateUpdate.Content = lastMediaInfo.dtUpdate;
                        mediaInfoTitle.Content = lastMediaInfo.title;
                        mediaInfoAuthors.Content = lastMediaInfo.authors;
                        mediaInfoYear.Content = lastMediaInfo.year;
                        mediaInfoSize.Content = (long.Parse(lastMediaInfo.size) / 1000000) + " MB";
                        mediaInfoReadProblem.Content = (lastMediaInfo.mediaReadProblem == "1" ? "Ano" : "Ne");
                    }
                    else
                    {
                        mediaInfoNew.Visibility = Visibility.Visible;
                    }
                }
            }
            catch (ManagementException ex)
            {
                MessageBox.Show("Error getting media info: " + ex.Message);
                return;
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            bool start = false;
            if (tmpQuickId == "")
            {
                btnCreateWarning.Content = "Médium není načtené";
                btnCreateWarning.Visibility = Visibility.Visible;
                btnCreateWarningIcon.Visibility = Visibility.Visible;
            }
            else if (string.IsNullOrWhiteSpace(mediaNoTextBox.Text))
            {
                btnCreateWarning.Content = "Zadejte číslo disku";
                btnCreateWarning.Visibility = Visibility.Visible;
                btnCreateWarningIcon.Visibility = Visibility.Visible;
            }
            else if (!tmpMediumReadProblem && mediaInfoExists.Visibility == Visibility.Visible)
            {
                bool dontShowAgain;
                var result = MessageBoxDialogWindow.Show("Opakování přenosu",
                    "Toto médium je již v pořádku archivováno. Opakovat přenos?", out dontShowAgain, null,
                    "Ano", "Ne", false, MessageBoxDialogWindow.Icons.Error);
                if (result != false)
                {
                    tmpForcedUpload = true;
                    start = true;
                }
            }
            else
            {
                start = true;
            }

            if (start)
            {
                selectedDriveName = driveList.Text;
                tmpMediaHash = "";
                workingMediaId = "";
                hashCheckTryCount = 999;
                clearGuiProgress();
                SendToObalkyKnih();
            }
        }

        private void iso_OnFinish(EventIsoArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                this.stepIcon2.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-done.png");
                this.stepIcon3.Visibility = Visibility.Visible;
                this.stepIcon3.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-hourglass.png");

                // progress // log
                this.logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Ukládání ISO úspěšné.\r");
            }));

            byte[] hashValue;
            System.IO.FileStream oFileStream = null;

            // Vypocet kontrolniho souctu
            if (!Settings.offlineMode)
            {
                try
                {
                    System.Security.Cryptography.MD5CryptoServiceProvider oMD5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();
                    oFileStream = new System.IO.FileStream(Settings.tmpPathToIso, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
                    hashValue = oMD5Hasher.ComputeHash(oFileStream);
                    oFileStream.Close();

                    this.tmpMediaHash = System.BitConverter.ToString(hashValue);

                    // progress // log
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        this.logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Kontrolní součet lokálního ISO souboru: " + this.tmpMediaHash.Replace("-", "").ToLower() + "\r");
                    }));
                }
                catch (System.Exception ex)
                {
                    // progress // log
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        this.logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Error while calculating SHA1 checksum: " + ex.Message + "\r");
                    }));

                    MessageBox.Show("Error while calculating MD5 checksum: " + ex.Message);
                    return;
                }
            }

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                stepIcon3.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-done.png");
                if (Settings.offlineMode)
                {
                    stepIcon3.Visibility = Visibility.Visible;
                }
                stepIcon4.Visibility = Visibility.Visible;
                stepIcon4.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-hourglass.png");

                // progress // log
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    this.logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Výpočet kontrolního součtu ukončen.\r");
                }));
            }));

            if (!Settings.offlineMode)
            {
                // progress // log
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    this.logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Odesílám ISO soubor na server.\r");
                }));

                // Poslat na server
                doUploadIso(Settings.tmpPathToIso, this.workingMediaId, this.tmpMediaHash);
            }
            else
            {
                // V offline rezimu nezasilame na server
                finishProces();
            }
        }

        private void doUploadIso(String pathToIso, String workingMediaId, String tmpMediaHash)
        {
            MediaSender ms = new MediaSender();
            ms.OnFinish += new SenderEventHandler(ms_OnFinish);
            ms.OnMessage += new SenderEventHandler(ms_OnMessage);
            ms.OnProgress += new SenderEventHandler(ms_OnProgress);

            //Upload ISO
            SenderState status = ms.SendMediaToServer(pathToIso, Settings.ImportLink, this.workingMediaId, this.tmpMediaHash, this.tmpQuickId, this.tmpMediumReadProblem, this.tmpForcedUpload);

            if (status != SenderState.Running)
            {
                // progress // log
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    this.logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Upload ISO selhalo.\r");
                }));

                ms.Stop();
                MessageBox.Show("Upload ISO selhalo.");
            }
        }

        private void iso_OnMessage(EventIsoArgs e)
        {
            MessageBox.Show(e.Message);
        }

        private void iso_OnProgress(EventIsoArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                this.copyProgress.Value = e.ProgressPercent;
                this.copyPercent.Content = e.ProgressPercent.ToString() + "%";
                this.copyTime.Content = e.ElapsedTime.ToString();
                this.copySize.Content = Math.Round(e.WrittenSize / 1000000.0).ToString() + " MB";
            }));
        }

        private void ms_OnFinish(EventSenderArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                stepIcon4.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-done.png");
            }));

            finishProces();
        }

        private void ms_OnMessage(EventSenderArgs e)
        {
            MessageBox.Show(e.Message);
        }

        private void ms_OnProgress(EventSenderArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                this.copyProgress.Value = e.ProgressPercent;
                this.copyPercent.Content = e.ProgressPercent.ToString() + "%";
                this.copyTime.Content = e.ElapsedTime.ToString();
                this.copySize.Content = Math.Round(e.WrittenSize / 1000000.0).ToString() + " MB";
            }));
        }

        private void finishProces()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                stepIcon5.Visibility = Visibility.Visible;
                stepIcon5.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-hourglass.png");
                step5Finish.Visibility = Visibility.Visible;
                step5Finish.Content = "Čekám na dokončení na straně serveru: 60s";

                // progress // log
                this.logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Upload ISO ukončen.\r");
                this.logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Čekám na výpočet kontrolního souboru na sevreru max 60s.\r");
            }));

            this.hashCheckTryCount = 0;

            if (!Settings.offlineMode)
            {
                // Kontroluje spravnost kontrolneho suctu, pokud jsme ONLINE
                hashCheckBackgroundWorker.RunWorkerAsync();
            }
            else
            {
                // Koprujeme do docasneho uloziste
                string dt = DateTime.Now.ToString("yyyyMMddHHmmss");
                string destDir = System.IO.Path.Combine(Settings.TmpDir, dt);

                // Jsme offline, zkopirujeme soubory do adresare a koncime
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    stepIcon4.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-done.png");
                    stepIcon5.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-done.png");
                    step5Finish.Visibility = Visibility.Hidden;

                    // progress // log
                    this.logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Offline režim. Přesun ISO souboru do adresáře: " + destDir + "\r");
                }));

                System.IO.Directory.CreateDirectory(destDir);
                System.IO.File.Move(Settings.tmpPathToIso, System.IO.Path.Combine(destDir, "media.iso"));
                uploadProcesCompleted();
            }
        }

        // Ziskaj data z HTTP
        async Task<string> GetData(String urlString)
        {
            var url = new Uri(urlString);

            try
            {
                var result = await httpClient.GetStringAsync(url);
                string checkResult = result.ToString();
                //httpClient.Dispose();
                return checkResult;
            }
            catch (Exception ex)
            {
                string checkResult = "Error " + ex.ToString();
                //httpClient.Dispose();
                return checkResult;
            }
        }

        public async Task<string> PostAsync(String urlString, String data)
        {
            var url = new Uri(urlString);

            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.PostAsync(url, new StringContent(data));
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception ex)
            {
                string checkResult = "Error " + ex.ToString();
                return checkResult;
            }
        }

        // Kontrola spravnosti kontrolneho suctu - posledny krok pri posielani media na server
        private async void bgCheckHash_DoWork(object sender, DoWorkEventArgs e)
        {
            if (string.IsNullOrEmpty(this.workingMediaId))
            {
                return;
            }

            if (!Settings.offlineMode)
            {
                if (this.hashCheckTryCount >= 59)
                {
                    // max 60 pokusu = 60sec
                    uploadProcesFailed();
                    return;
                }
                this.hashCheckTryCount++;

                await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    int sec = 60 - this.hashCheckTryCount;
                    step5Finish.Content = "Čekám na dokončení na straně serveru: " + sec + "s";

                    // progress // log
                    this.logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  " + step5Finish.Content.ToString() + "\r");
                }));

                var content = await GetData(Settings.ChecksumLink + "/?mediaid=" + this.workingMediaId);

                try
                {
                    if (string.IsNullOrEmpty(this.workingMediaId))
                    {
                        return;
                    }
                    GethashJsonObject responseObject = JsonConvert.DeserializeObject<GethashJsonObject>(content);
                    if (responseObject != null && responseObject.status == "ok")
                    {
                        await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            // progress // log
                            this.logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Kontrolní součet vypočtený serverem: " + responseObject.hash + "\n");
                        }));

                        if (responseObject.hash == this.tmpMediaHash.Replace("-", "").ToLower())
                        {
                            // hotovo
                            uploadProcesCompleted();
                            this.hashCheckTryCount = 0;
                            return;
                        }
                        else
                        {
                            // hash is not the same = upload failed
                            uploadProcesFailed();
                            return;
                        }
                    }
                    else if (responseObject != null && responseObject.status == "error")
                    {
                        // something went wrong = upload failed
                        uploadProcesFailed();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    uploadProcesFailed();
                    return;
                }
            }
            else
            {
                uploadProcesCompleted();
                return;
            }

            Thread.Sleep(1000);
            if (this.hashCheckTryCount > 0 && this.hashCheckTryCount < 60)
            {
                if (!hashCheckBackgroundWorker.IsBusy)
                {
                    hashCheckBackgroundWorker.RunWorkerAsync();
                }
            }
        }

        private void uploadProcesFailed()
        {
            if (this.hashCheckTryCount == 999)
            {
                return;
            }
            this.hashCheckTryCount = 999;

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                // progress // log
                this.stepIcon5.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-warning.png");
                this.logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Proces archivace neúspěšný !\r");

                bool dontShowAgain;
                var result = MessageBoxDialogWindow.Show("Opakování přenosu",
                    "Proces archivace média selhal. Přejete si proces opakovat?", out dontShowAgain, null,
                    "Ano", "Ne", false, MessageBoxDialogWindow.Icons.Error);
                if (result != false)
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        clearGuiProgress();
                        stepIcon1.Visibility = Visibility.Visible;
                        stepIcon2.Visibility = Visibility.Visible;
                        stepIcon3.Visibility = Visibility.Visible;
                        stepIcon4.Visibility = Visibility.Visible;
                        stepIcon1.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-done.png");
                        stepIcon2.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-done.png");
                        stepIcon3.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-done.png");
                        stepIcon4.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-hourglass.png");
                    }));
                    doUploadIso(Settings.tmpPathToIso, this.workingMediaId, this.tmpMediaHash);
                }
                else
                {
                    //todo: ukonci archivaci tohoto media
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        copyProgress.Value = 0;
                        stepIcon1.Visibility = Visibility.Hidden;
                        stepIcon2.Visibility = Visibility.Hidden;
                        stepIcon3.Visibility = Visibility.Hidden;
                        stepIcon4.Visibility = Visibility.Hidden;
                        stepIcon5.Visibility = Visibility.Hidden;
                        step5Finish.Visibility = Visibility.Hidden;
                        copyPercent.Content = "";
                        controlTabDoneIcon.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-ban.png");
                    }));
                }
            }));
        }

        private void uploadProcesCompleted()
        {
            this.tmpMediaHash = "";
            this.workingMediaId = "";

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                stepIcon5.Visibility = Visibility.Visible;
                stepIcon5.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-done.png");
                controlTabDoneIcon.Source = getIconSource("CDArcha_klient;component/Images/ok-icon-done.png");
                zobrazitVysledek.Visibility = System.Windows.Visibility.Visible;
                step5Finish.Visibility = Visibility.Hidden;
                reloadArchiveList_Click(null, null);

                // progress // log
                this.logTextBox.AppendText(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + "  Proces archivace úspěšně ukončen.\r");
            }));
        }

        private void TabsControlUserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void showLogLink_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.logTextBox.Visibility != Visibility.Visible)
            {
                this.logTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                this.logTextBox.Visibility = Visibility.Visible;
            }
        }

        private void quitApp(object sender, RoutedEventArgs e)
        {
            closeArchive();
            System.Windows.Forms.Application.Exit();
        }

        private void chooseScanTab(object sender, RoutedEventArgs e)
        {
            scanningTabItem.IsEnabled = true;
            tabControl.SelectedItem = scanningTabItem;
        }

        private void reloadArchiveList_Click(object sender, RoutedEventArgs e)
        {
            var identBuildTuple = this.identifiersBuild();
            NameValueCollection nvc = identBuildTuple.Item2;
            UploadParameters param = new UploadParameters();
            DoWorkEventArgs res = new DoWorkEventArgs(null);
            UploadFilesToRemoteUrl(Settings.GetAllMediaListLink, nvc, null, null, null, res);
            archiveTree.Items.Clear();
            mediaNoTextBox.Text = "";
            archiveOp.Content = "";
            archiveState.Content = "";

            if (res.Result != null && !string.IsNullOrWhiteSpace(res.Result.ToString()))
            {
                List<JsonApiAllMedia> responseObject = JsonConvert.DeserializeObject<List<JsonApiAllMedia>>(res.Result.ToString());

                TreeViewItem root = new TreeViewItem();

                lastArchiveInfo = new List<JsonApiAllMedia>();
                foreach (JsonApiAllMedia obj in responseObject)
                {
                    JsonApiAllMedia lastArchiveInfoItem = new JsonApiAllMedia();
                    lastArchiveInfoItem.id = obj.id;
                    lastArchiveInfoItem.type = obj.type;
                    lastArchiveInfoItem.status = obj.status;
                    lastArchiveInfoItem.mediaNo = obj.mediaNo;
                    lastArchiveInfoItem.dtLastUpdate = obj.dtLastUpdate;
                    lastArchiveInfoItem.text = obj.text;
                    lastArchiveInfo.Add(lastArchiveInfoItem);

                    if (obj.type == "archive")
                    {
                        root.Header = obj.text;
                        root.Uid = obj.id;
                        switch (obj.status) {
                            case "0": archiveState.Content = "Rozpracovaný na klientovi"; break;
                            case "1": archiveState.Content = "Ukončený na klientovi"; break;
                            case "2": archiveState.Content = "Rozpracovaný na serveru"; break;
                            case "3": archiveState.Content = "Připravený k archivaci"; break;
                            case "4": archiveState.Content = "Archivovaný"; break;
                        }
                        if (obj.status != "0")
                        {
                            disableArchiveControls();
                        } else
                        {
                            enableArchiveControls();
                        }
                    } else
                    {
                        TreeViewItem item = new TreeViewItem();
                        item.Header = obj.text;
                        item.Uid = obj.id;
                        root.Items.Add(item);
                    }
                }

                root.IsExpanded = true;
                root.IsSelected = true;
                archiveTree.Items.Add(root);
            }
            else
            {
                archiveOp.Content = "Archiv neexistuje a bude vytvořen s první archivací";
                archiveState.Content = "Ještě neexistuje";
            }
        }

        private void archiveTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            archiveOp.Content = "";
            TreeViewItem sel = (TreeViewItem)this.archiveTree.SelectedItem;
            if (sel != null && tabControl.SelectedItem == metadataTabItem)
            {
                this.lastWorkingMediaId = sel.Uid;
                foreach (JsonApiAllMedia obj in lastArchiveInfo)
                {
                    if (obj.id == sel.Uid)
                    {
                        if (obj.type == "archive")
                        {
                            archiveOp.Content = "Můžete pokračovat v archivaci dalšího média archivu: " + sel.Uid;
                            mediaNoTextBox.Text = "";
                            scanArchiveItem.Content = "Skenovat obálku vybraného ARCHIVU";
                        }
                        else
                        {
                            archiveOp.Content = "Načteno médium: " + obj.mediaNo + ". Můžete archivaci média opakovat, nebo pokračovat v skenování.";
                            mediaNoTextBox.Text = obj.mediaNo;
                            scanArchiveItem.Content = "Skenovat obálku vybraného MÉDIA";
                        }
                    }
                }
            }
        }

        private void showArchiveItem_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.lastWorkingMediaId))
            {
                System.Diagnostics.Process.Start(Settings.ip + "/cdarcha/content/?id=" + this.lastWorkingMediaId);
            }
            else
            {
                MessageBox.Show("Není načten žádný záznam !");
            }
        }

        private void scanArchiveItem_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.lastWorkingMediaId))
            {
                scanningTabItem.IsEnabled = true;
                tabControl.SelectedItem = scanningTabItem;
            } else
            {
                MessageBox.Show("Není načten žádný záznam !");
            }
        }

        private void closeArchiveItem_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.lastWorkingMediaId))
            {
                archiveOp.Content = "Archiv byl označen jako ukončený.";
                archiveState.Content = "Ukončen na klientovi";
                closeArchive();
                disableArchiveControls();
            }
            else
            {
                MessageBox.Show("Není načten žádný záznam archivu !");
            }
        }

        private void closeArchive()
        {
            if (!string.IsNullOrEmpty(this.lastWorkingMediaId))
            {
                HttpWebRequest requestToServer = (HttpWebRequest)WebRequest.Create(Settings.CloseArchiveLink + this.lastWorkingMediaId);
                requestToServer.Timeout = 10000;
                requestToServer.Method = WebRequestMethods.Http.Get;
                requestToServer.ContentType = "text/plain";
                try
                {
                    WebResponse response = requestToServer.GetResponse();
                    requestToServer.Abort();
                    reloadArchiveList_Click(null, null);
                    this.lastWorkingMediaId = null;
                }
                catch (Exception e) { }
            }
        }

        private void disableArchiveControls()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                loadMedia.IsEnabled = false;
                btnCreate.IsEnabled = false;
                closeArchiveItem.IsEnabled = false;
                scanArchiveItem.IsEnabled = false;
                driveList.IsEnabled = false;
                mediaNoTextBox.IsEnabled = false;
            }));
        }

        private void enableArchiveControls()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                loadMedia.IsEnabled = true;
                btnCreate.IsEnabled = true;
                closeArchiveItem.IsEnabled = true;
                scanArchiveItem.IsEnabled = true;
                driveList.IsEnabled = true;
                mediaNoTextBox.IsEnabled = true;
            }));
        }
    }

    #region Custom WPF controls

    /// <summary>
    /// Custom ListView - changed not to catch events with arrow keys and scrolling,
    /// because of rotation key bindings
    /// </summary>
    public class MyListView : ListView
    {
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            return;
        }
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            e.Handled = true;

            var e2 = new MouseWheelEventArgs(e.MouseDevice,e.Timestamp,e.Delta);
            e2.RoutedEvent = UIElement.MouseWheelEvent;
            this.RaiseEvent(e2);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.Right
                    || e.Key == Key.Up || e.Key == Key.Down)
            {
                return;
            }
            base.OnKeyDown(e);
        }
    }
    /// <summary>
    /// Custom ScrollViewer - changed not to catch events with arrow keys,
    /// because of rotation key bindings
    /// </summary>
    public class MyScrollViewer : ScrollViewer
    {
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.Left || e.Key == Key.Right
                    || e.Key == Key.Up || e.Key == Key.Down)
                    return;
            }
            base.OnKeyDown(e);
        }
    }

    /// <summary>
    /// Custom GridSplitter - changed not to catch events with arrow keys,
    /// because of rotation key bindings
    /// </summary>
    public class MyGridSplitter : GridSplitter
    {
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.Left || e.Key == Key.Right
                    || e.Key == Key.Up || e.Key == Key.Down)
                    return;
            }
            base.OnKeyDown(e);
        }
    }

    public class ComboboxItem
    {
        public string Text { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
    #endregion

}
