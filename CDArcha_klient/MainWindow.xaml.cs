﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CDArcha_klient.Classes;


namespace CDArcha_klient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region key bindings commands
        public static RoutedCommand newUnitCommand = new RoutedCommand();
        public static RoutedCommand undoCommand = new RoutedCommand();
        public static RoutedCommand redoCommand = new RoutedCommand();
        #endregion

        //downloads info about update on background
        private BackgroundWorker updateInfoBackgroundWorker = new BackgroundWorker();
        //checks info about update and supported versions
        private UpdateChecker updateChecker = new UpdateChecker();
        // used for differentiation between calls from Menu and automatic call after start
        private bool showIsLatestVersionPopup = false;
        //signals is this version is supported
        private bool? isAllowedVersion = null;
        //signals finished version check
        private bool isVersionCheckFinished = false;
        //client for downloading of update
        private WebClient webClient = new WebClient();
        //force shutdown - do not show confirmation message
        private bool forceShutDown = false;
       
        /// <summary>constructor - initializes components and key bindings</summary>
        public MainWindow()
        {
            InitializeComponent();
            InitializeBackgroundWorkers();

            if (!Directory.Exists(Settings.TemporaryFolder))
            {
                Directory.CreateDirectory(Settings.TemporaryFolder);
            }

            // Pokus o zapis do docasneho adresare definovaneho v nastavenich jako zkouska schopnosti zapisu
            try
            {
                System.IO.File.WriteAllText(Path.Combine(Settings.TmpDir, "test.txt"), "test zapisu");
                System.IO.File.Delete(Path.Combine(Settings.TmpDir, "test.txt"));
            }
            catch (Exception)
            {
                Settings.TmpDir = Settings.TemporaryFolder;
            }

            // Promazani docasnych souboru
            string targetDirectory = Path.GetDirectoryName(Settings.tmpPathToIso);
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
            {
                if (fileName.IndexOf("working") >= 0)
                {
                    File.Delete(fileName);
                }
            }

            //Show version changes
            ShowVersionChanges();

            this.webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCompleted);
            this.webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressChanged);
            
            this.programVersionLabel.Content = Settings.Version.Major.ToString()
                + "." + Settings.Version.Minor.ToString();

            this.versionStateLabel.Content = "Získávání informací";
            this.updateInfoBackgroundWorker.RunWorkerAsync();

            // Zjisteni poctu docasne lokalne ulozenych medii
            var directoryInfo = new System.IO.DirectoryInfo(Settings.TmpDir);
            int directoryCount = directoryInfo.GetDirectories().Length;
            if (directoryCount > 0)
            {
                showTmpFolderBtn.Visibility = Visibility.Visible;
                tmpFolderLabel.Visibility = Visibility.Visible;
            }

            #region commandBindings

            //new - Ctrl + N
            CommandBinding cb = new CommandBinding(newUnitCommand, NewUnitExecuted, NewUnitCanExecute);
            this.CommandBindings.Add(cb);
            KeyGesture kg = new KeyGesture(Key.N, ModifierKeys.Control);
            InputBinding ib = new InputBinding(newUnitCommand, kg);
            this.InputBindings.Add(ib);
            
            //undo - Ctrl + Z
            cb = new CommandBinding(undoCommand, UndoExecuted, UndoCanExecute);
            this.CommandBindings.Add(cb);
            kg = new KeyGesture(Key.Z, ModifierKeys.Control);
            ib = new InputBinding(undoCommand, kg);
            this.InputBindings.Add(ib);
            
            //redo - Ctrl + Y
            cb = new CommandBinding(redoCommand, RedoExecuted, RedoCanExecute);
            this.CommandBindings.Add(cb);
            kg = new KeyGesture(Key.Y, ModifierKeys.Control);
            ib = new InputBinding(redoCommand, kg);
            this.InputBindings.Add(ib);
            #endregion
        }

        // Downloads update
        private void DownloadUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            DownloadUpdateFile();
        }

        #region menu items
        
        // Shows CreateNewUnitWindows
        private void NewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowNewUnitWindow();
        }
        
        // Download update-info from update server
        private void UpdateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.updateInfoBackgroundWorker.IsBusy)
            {
                return;
            }
            this.showIsLatestVersionPopup = true;
            this.updateInfoBackgroundWorker.RunWorkerAsync();
        }

        // Shows confirmation dialog and exits application
        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Shows CredentialsWindow
        private void CredentialsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Window credentialsWindow = new CredentialsWindow();
            credentialsWindow.ShowDialog();
        }

        // Shows SettingsWindow
        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Window settingsWindows = new SettingsWindow();
            settingsWindows.ShowDialog();
        }

        //Shows AboutWindow
        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Window aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }

        // Shows HelpWindow
        private void HelpMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Window helpWindow = new HelpWindow();
            helpWindow.Show();
        }

        // Navigate to OnlineHelpWindow
        private void OnlineHelpMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/");
        }
        #endregion

        #region command bindings methods

        //Ctrl+N  - Shows CreateNewUnitWindow
        private void NewUnitCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        //Ctrl+N  - Shows CreateNewUnitWindow
        private void NewUnitExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ShowNewUnitWindow();
        }


        //Ctrl+Z - Undo last step
        private void UndoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        //Ctrl+Z - Undo last step
        private void UndoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.undoMenuItem.IsEnabled)
            {
                UndoMenuItem_Click(null, null);
            }
        }

        //Ctrl+Z - Undo last step
        private void RedoCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        //Ctrl+Z - Undo last step
        private void RedoExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.redoMenuItem.IsEnabled)
            {
                RedoMenuItem_Click(null, null);
            }
        }
        #endregion

        #region functionality

        /// <summary>Shows important new changes</summary>
        private void ShowVersionChanges()
        {
        }

        /// <summary>Adds message to StatusBar</summary>
        /// <param name="text">Message that will be added to StatusBar</param>
        internal void AddMessageToStatusBar(string text)
        {
            this.metadataDownloadTextBox.Text += text + " ";
            if (!string.IsNullOrWhiteSpace(this.metadataDownloadTextBox.Text))
            {
                this.metadataDownloadTextBox.Visibility = Visibility.Visible;
            }
        }

        /// <summary>Removes message from StatusBar if it contains this message</summary>
        /// <param name="text">message that will be removed from StatusBar</param>
        internal void RemoveMessageFromStatusBar(string text)
        {
            if (!string.IsNullOrEmpty(this.metadataDownloadTextBox.Text))
            {
                this.metadataDownloadTextBox.Text =
                    this.metadataDownloadTextBox.Text.Replace(text + " ", "");
            }
            // check if new string is empty
            if (string.IsNullOrWhiteSpace(this.metadataDownloadTextBox.Text))
            {
                this.metadataDownloadTextBox.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>Enables Undo MenuItem</summary>
        internal void ActivateUndo()
        {
            this.undoMenuItem.IsEnabled = true;
        }

        /// <summary>Disables Undo MenuItem</summary>
        internal void DeactivateUndo()
        {
            this.undoMenuItem.IsEnabled = false;
        }

        /// <summary>Enables Redo MenuItem</summary>
        internal void ActivateRedo()
        {
            this.redoMenuItem.IsEnabled = true;
        }

        /// <summary>Disables Undo MenuItem</summary>
        internal void DeactivateRedo()
        {
            this.redoMenuItem.IsEnabled = false;
        }

        // Shows confirmation message and shutdown application
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (forceShutDown || Settings.DisableClosingConfirmation)
            {
                return;
            }
            bool dontAskAgain;
            if (MessageBoxDialogWindow.Show("Potvrzení", "Opravdu chcete ukončit program?",
                out dontAskAgain, "Příště se neptat a zavřít", "Ano", "Ne", true, MessageBoxDialogWindow.Icons.Question) == true)
            {
                if (dontAskAgain)
                {
                    Settings.DisableClosingConfirmation = true;
                }
                Application.Current.Shutdown();
            }
            else
            {
                e.Cancel = true;
            }
        }

        // Opens CreateNewUnitWindow
        internal void ShowNewUnitWindow()
        {
            try
            {
                foreach (string fileName in Directory.GetFiles(Settings.TemporaryFolder))
                {
                    // images handled after uploading
                    if (fileName.Equals("CDArcha_klient_setup.exe"))
                    {
                        File.Delete(fileName);
                    }
                }
            }
            // don't care if some file can't be deleted right now
            catch (Exception) { }

            if (this.isVersionCheckFinished && (Settings.NeverDownloadUpdates || Settings.offlineMode) && isAllowedVersion != false) isAllowedVersion = true; //debug
            if (isAllowedVersion == null)
            {
                MessageBoxDialogWindow.Show("Kontrola verze", "Kontrola verze zatím neskončila, počkejte prosím.",
                    "OK", MessageBoxDialogWindow.Icons.Error);
            }
            else if (isAllowedVersion == true)
            {
                if (!Settings.ForceUpdate || this.updateChecker.IsUpdateAvailable)
                {
                    CreateNewUnitWindow newWindow = new CreateNewUnitWindow();
                    newWindow.ShowDialog();

                    if (newWindow.DialogResult.HasValue && newWindow.DialogResult.Value)
                    {
                        AddMessageToStatusBar("Stahuji metadata.");
            
                        GeneralRecord generalRecord = newWindow.GeneralRecord;

                        this.dockPanel.Children.Remove(this.mainGrid);
                        foreach (UIElement element in this.dockPanel.Children)
                        {
                            if (element is TabsControl)
                            {
                                this.dockPanel.Children.Remove(element);
                                break;
                            }
                        }
                        this.dockPanel.Children.Add(new TabsControl(generalRecord));
                    }
                }
                else
                {
                    MessageBoxDialogWindow.Show("Zastaralá verze",
                        "Tato verze programu není aktuální a administrátor vynutil používání jenom aktuální verze."
                        + "Prosím nainstalujte aktualizaci.", "OK", MessageBoxDialogWindow.Icons.Error);
                }
            }
            else
            {
                MessageBoxDialogWindow.Show("Nepodporovaná verze",
                    "Tato verze programu není podporována, prosím nainstalujte aktualizaci.",
                    "OK", MessageBoxDialogWindow.Icons.Error);
            }
        }

        // Action after clicking on progress bar or label in StatusBar, starts update if downloaded
        private void UpdateDownload_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ShutDownAndStartUpdate();
            }
        }

        //Shows confirmation dialog, starts update and exits application
        private void ShutDownAndStartUpdate()
        {
            if (this.updateDownloadProgressBar.Value == 100)
            {
                var result = MessageBoxDialogWindow.Show("Aktualizace stažena",
                    "Aktualizace byla stažena, přejete si nyní ukončit program a aktualizovat?",
                    "Yes", "No", true, MessageBoxDialogWindow.Icons.Question);

                if (result == true)
                {
                    try
                    {
                        //start update in new process
                        Process updateProcess = new Process();
                        updateProcess.StartInfo.FileName = this.updateChecker.FilePath;
                        updateProcess.StartInfo.UseShellExecute = false;
                        updateProcess.Start();

                        //close application
                        this.forceShutDown = true;
                        Application.Current.Shutdown();
                    }
                    catch (Exception)
                    {
                        MessageBoxDialogWindow.Show("Aktualizace stažena",
                            "Nepodařilo se spustit aktualizaci, ukončete program a spusťte ji ručně.",
                            "OK", MessageBoxDialogWindow.Icons.Error);
                    }
                }
            }
            else
            {
                MessageBoxDialogWindow.Show("Stahování nekompletní", "Aktualizace zatím nebyla stažena.", "OK", MessageBoxDialogWindow.Icons.Information);
            }
        }

        #region background downloads

        // Initializes updateInfoBackgroundWorker 
        private void InitializeBackgroundWorkers()
        {
            this.updateInfoBackgroundWorker.WorkerSupportsCancellation = true;
            this.updateInfoBackgroundWorker.WorkerReportsProgress = false;
            this.updateInfoBackgroundWorker.DoWork += new DoWorkEventHandler(UpdateInfoBackgroundWorker_DoWork);
            this.updateInfoBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UpdateInfoBackgroundWorker_RunWorkerCompleted);
        }

        // Starts RetrieveInfo method of UpdateChecker in new thread
        private void UpdateInfoBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (this.isVersionCheckFinished)
            {
                this.updateInfoBackgroundWorker.CancelAsync();
            }
            else
            {
                updateChecker.RetrieveUpdateInfo();
            }
        }

        // Complex actions after update-info was retrieved
        private void UpdateInfoBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.isVersionCheckFinished = true;
            if (e.Error != null)
            {
                this.versionStateLabel.Content = "";
                if (e.Error.InnerException is FormatException)
                {
                    MessageBoxDialogWindow.Show("Chybné informace o aktualizaci",
                        "Informace o aktualizaci mají nesprávný formát. Kontaktujte prosím administrátory servru obalkyknih.cz.",
                        "OK", MessageBoxDialogWindow.Icons.Error);
                }
                /*else if (!Settings.NeverDownloadUpdates)
                {
                    var result = MessageBoxDialogWindow.Show("Chyba stahování informací o aktualizaci",
                        "Při stahování informací o aktualizaci se vyskytla chyba. Po stisknutí OK se zahájí další pokus.",
                        "OK", MessageBoxDialogWindow.Icons.Error);
                    if (result == true)
                    {
                        updateInfoBackgroundWorker.RunWorkerAsync();
                    }
                }*/
                else
                {
                    Settings.offlineMode = true;
                    this.versionStateLabel.Content = "Verzi není možné zjistit";
                    this.versionStateLabel.Foreground = Brushes.Red;
                    this.versionStateLabel.FontWeight = FontWeights.Bold;
                    this.offlineModeLabel.Visibility = Visibility.Visible;
                }
            }
            else if (!e.Cancelled)
            {
                this.latestVersionLabel.Content = this.updateChecker.AvailableVersion.Major.ToString()
                    + "." + this.updateChecker.AvailableVersion.Minor.ToString();
                this.latestDateLabel.Content = this.updateChecker.AvailableVersionDate;

                if (!updateChecker.IsSupportedVersion)
                {
                    // suppose that if it is not supported, it can't be up to date
                    this.versionStateImage.Visibility = Visibility.Visible;
                    this.versionStateLabel.Content = "Verze není aktuální";
                    this.programSupportLabel.Foreground = Brushes.Red;
                    this.programSupportLabel.Content = "Nepodporováno";
                    isAllowedVersion = false;
                    this.downloadUpdateButton.Visibility = Settings.DisableUpdate ? Visibility.Hidden : Visibility.Visible;
                    if (Settings.DisableUpdate)
                    {
                        MessageBoxDialogWindow.Show("Nepodporovaná verze",
                            "Vaše verze programu je v seznamu nepodporovaných verzí a nelze s ní pracovat."
                            + "Požádejte administrátora o instalaci nové verze.",
                            "OK", MessageBoxDialogWindow.Icons.Error);
                    }
                    else
                    {
                        bool dontShowAgain = false;
                        bool? result = true;
                        if (!Settings.AlwaysDownloadUpdates)
                        {
                            result = MessageBoxDialogWindow.Show("Nepodporovaná verze",
                                "Vaše verze programu je v seznamu nepodporovaných verzí a nelze s ní pracovat, musíte stáhnout aktualizaci. Chcete ji stáhnout nyní?",
                                out dontShowAgain, "Příště se neptat a stáhnout", "Ano", "Ne", true, MessageBoxDialogWindow.Icons.Warning);
                        }
                        if (result == true)
                        {
                            if(dontShowAgain)
                            {
                                Settings.AlwaysDownloadUpdates = true;
                            }
                            DownloadUpdateFile();
                        }
                    }
                }
                else
                {
                    this.programSupportLabel.Content = "Podporováno";
                    isAllowedVersion = true;
                    if (this.updateChecker.IsUpdateAvailable)
                    {
                        this.versionStateImage.Visibility = Visibility.Visible;
                        this.versionStateLabel.Content = "Verze není aktuální";
                        this.downloadUpdateButton.Visibility = Settings.DisableUpdate ? Visibility.Hidden : Visibility.Visible;
                        if (!Settings.DisableUpdate && (!Settings.NeverDownloadUpdates || Settings.offlineMode || showIsLatestVersionPopup))
                        {
                            bool dontShowAgain;
                            var result = MessageBoxDialogWindow.Show("Aktualizace", "Aktualizace je k dispozici, chcete ji stáhnout?",
                                out dontShowAgain, "Příště se neptat a použít tuto volbu", "Ano", "Ne", true, MessageBoxDialogWindow.Icons.Question);
                            if (result == true)
                            {
                                if (dontShowAgain)
                                {
                                    Settings.AlwaysDownloadUpdates = true;
                                }
                                DownloadUpdateFile();
                            }
                            else if (dontShowAgain)
                            {
                                Settings.NeverDownloadUpdates = true;
                            }
                        }
                    }
                    else
                    {
                        this.versionStateLabel.Content = "Verze je aktuální";
                        this.downloadUpdateButton.Visibility = Visibility.Hidden;
                        if (this.showIsLatestVersionPopup)
                        {
                            MessageBoxDialogWindow.Show("Aktualizace", "Verze je aktuální",
                                "OK", MessageBoxDialogWindow.Icons.Information);
                        }
                    }
                }
            }
        }

        // Downloads update file from link in DownloadLink property of UpdateChecker
        private void DownloadUpdateFile()
        {
            this.downloadUpdateButton.IsEnabled = false;
            if (this.webClient.IsBusy)
            {
                webClient.CancelAsync();
            }
            string downloadLink = this.updateChecker.DownloadLink;
            if (downloadLink != null)
            {
                this.updateDownloadTextBox.Visibility = Visibility.Visible;
                this.updateDownloadProgressBar.Value = 0;
                this.updateDownloadProgressBar.Visibility = Visibility.Visible;

                this.webClient.DownloadFileAsync(new Uri(downloadLink), this.updateChecker.FilePath);
            }
        }

        // Shows progress of downloading of update file
        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.updateDownloadProgressBar.Value = e.ProgressPercentage;
        }

        // Complex actions after update file was downloaded
        private void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.downloadUpdateButton.IsEnabled = true;
            if (e.Error != null)
            {
                MessageBoxDialogWindow.Show("Chyba při stahování aktualizace",
                    "Počas stahování aktualizace došlo k chybě. Zkuste spustit stahování opakovaně.",
                    "OK", MessageBoxDialogWindow.Icons.Error);
                this.updateDownloadTextBox.Visibility = Visibility.Collapsed;
                this.updateDownloadProgressBar.Visibility = Visibility.Collapsed;
            }
            else if (e.Cancelled)
            {
                MessageBoxDialogWindow.Show("Stahování přerušeno", "Stahování bylo přerušeno.",
                    "OK", MessageBoxDialogWindow.Icons.Warning);
                this.updateDownloadTextBox.Visibility = Visibility.Collapsed;
                this.updateDownloadProgressBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                //check checksum
                try
                {
                    string checksum;
                    using (FileStream stream = File.OpenRead(this.updateChecker.FilePath))
                    {
                        SHA256Managed sha = new SHA256Managed();
                        byte[] checksumBytes = sha.ComputeHash(stream);
                        checksum = BitConverter.ToString(checksumBytes).Replace("-", String.Empty);
                    }

                    if (!this.updateChecker.Checksum.ToLower().Equals(checksum.ToLower()))
                    {
                        File.Delete(this.updateChecker.FilePath);
                        MessageBoxDialogWindow.Show("Aktualizace poškozena",
                            "Kontrolní součet stažené aktualizace se neshoduje, soubor byl smazán, stáhněte aktualizaci opakovaně.",
                            "OK", MessageBoxDialogWindow.Icons.Error);
                        this.updateDownloadProgressBar.Value = 0;
                        this.updateDownloadProgressBar.Visibility = Visibility.Collapsed;
                        this.updateDownloadTextBox.Visibility = Visibility.Collapsed;
                        return;
                    }
                }
                catch (Exception)
                {
                    MessageBoxDialogWindow.Show("Chyba kontroly aktualizace",
                        "Při kontrole integrity aktualizace došlo k chybě, stáhněte aktualizaci opakovaně.",
                        "OK", MessageBoxDialogWindow.Icons.Error);
                    this.updateDownloadProgressBar.Value = 0;
                    this.updateDownloadProgressBar.Visibility = Visibility.Collapsed;
                    this.updateDownloadTextBox.Visibility = Visibility.Collapsed;
                    return;
                }

                ShutDownAndStartUpdate();
            }
        }
        #endregion

        // Undo last step
        private void UndoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            (LogicalTreeHelper.FindLogicalNode(this, "TabsControlUserControl") as TabsControl).UndoLastStep();
        }

        // Redo last step
        private void RedoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            (LogicalTreeHelper.FindLogicalNode(this, "TabsControlUserControl") as TabsControl).RedoLastStep();
        }
        #endregion        

        private void OnlineHelpMenuItem_Click(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/");
        }

        private void showTmpFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            System.IO.Directory.CreateDirectory(Settings.TmpDir);
            Process.Start(@Settings.TmpDir);
        }

    }
}