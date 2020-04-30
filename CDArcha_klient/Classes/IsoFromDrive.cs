using System;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;

namespace CDArcha_klient.Classes
{
    public class IsoFromDrive : IsoFromDriveInterface
    {
        #region Variables
        /// <summary>
        /// BackgroundWorker for creating the ISO file
        /// </summary>
        BackgroundWorker bgCreator;
        #endregion

        #region Events
        /// <summary>
        /// Raised if progress value changes
        /// </summary>
        public event IsoEventHandler OnProgress;

        /// <summary>
        /// Raised when a message appears (errors, etc.)
        /// </summary>
        public event IsoEventHandler OnMessage;

        /// <summary>
        /// Raised when the file is finished
        /// </summary>
        public event IsoEventHandler OnFinish;
        #endregion

        #region Properties
        /// <summary>
        /// Source drive name
        /// </summary>
        String SourceDriveName { get; set; }

        /// <summary>
        /// Path to the ISO file
        /// </summary>
        string PathToIso { get; set; }

        /// <summary>
        /// Size of the medium
        /// </summary>
        public long MediumSize { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public IsoFromDrive() //todo: vymazat
        {
            bgCreator = new BackgroundWorker();
            bgCreator.WorkerSupportsCancellation = true;
            bgCreator.DoWork += new DoWorkEventHandler(bgCreator_DoWork);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Starts the thread creating the ISO file
        /// </summary>
        public void bgCreator_DoWork(object sender, DoWorkEventArgs e)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                DriveInfo driveInfo = new DriveInfo(System.IO.Path.GetPathRoot(SourceDriveName));
                if (driveInfo.IsReady)
                {
                    DiscUtils.Iso9660.CDBuilder builder = new DiscUtils.Iso9660.CDBuilder();
                    builder.UseJoliet = true;
                    builder.VolumeIdentifier = driveInfo.VolumeLabel;
                    DirectoryInfo di = new DirectoryInfo(SourceDriveName);
                    PopulateFromFolder(builder, di, di.FullName);
                    builder.Build(PathToIso);
                }
            }
            catch (Exception ex)
            {
                if (OnMessage != null)
                {
                    EventIsoArgs eArgs = new EventIsoArgs("Error while creating the image: " + ex.Message);
                    OnMessage(eArgs);
                }
            }
            finally
            {
                CloseAll();

                if (OnFinish != null)
                {
                    EventIsoArgs eArgs = new EventIsoArgs(stopWatch.Elapsed);
                    OnFinish(eArgs);
                }
            }
        }

        /// <summary>
        /// Creates an ISO image from drive
        /// </summary>
        /// <param name="source">Source drive containing files and folders to archivate</param>
        /// <param name="destination">Path where the ISO file is to be stored</param>
        /// <returns>
        /// Running = Creation in progress
        /// NotEnoughSpace = Not enough disk space on destination
        /// NotReady = The device is not ready
        /// </returns>
        public IsoFromDriveState CreateIsoFromDrive(string source, string destination)
        {
            //Is the device ready?
            if (!new DriveInfo(source).IsReady)
                return IsoFromDriveState.NotReady;

            //Get medium size
            MediumSize = GetMediumLength(source);

            //Check disk space
            long diskSize = new DriveInfo(System.IO.Path.GetPathRoot(destination)).AvailableFreeSpace;

            if (diskSize <= MediumSize)
                return IsoFromDriveState.NotEnoughSpace;

            if (!string.IsNullOrEmpty(source))
                SourceDriveName = source;

            if (!string.IsNullOrEmpty(destination))
                PathToIso = destination;

            //Create thread to create the ISO file
            bgCreator.RunWorkerAsync();

            return IsoFromDriveState.Running;
        }

        private static void PopulateFromFolder(DiscUtils.Iso9660.CDBuilder builder, DirectoryInfo di, string basePath)
        {
            foreach (FileInfo file in di.GetFiles())
            {
                builder.AddFile(file.FullName.Substring(basePath.Length), file.FullName);
            }

            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                PopulateFromFolder(builder, dir, basePath);
            }
        }

        /// <summary>
        /// Aborts the creation of the image and deletes it
        /// </summary>
        public void Stop()
        {
            CloseAll();

            if (File.Exists(PathToIso))
                File.Delete(PathToIso);

            if (OnMessage != null)
            {
                EventIsoArgs e = new EventIsoArgs(@"Creation of the images was canceled");
                OnMessage(e);
            }
        }

        /// <summary>
        /// Closes all streams and handles and frees resources
        /// </summary>
        public void CloseAll()
        {
            if (bgCreator != null)
            {
                bgCreator.CancelAsync();
                bgCreator.Dispose();
            }
        }

        /// <summary>
        /// Used space on drive
        /// </summary>
        /// <param name="drive">Source drive</param>
        /// <returns>Size in bytes</returns>
        public long GetMediumLength(string drive)
        {
            return new DriveInfo(drive).TotalSize;
        }
        #endregion
    }

}
