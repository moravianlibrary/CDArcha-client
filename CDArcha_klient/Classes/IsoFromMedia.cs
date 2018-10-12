using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace CDArcha_klient.Classes
{
    public class IsoFromMedia : IsoFromMediaInterface
    {
        #region Variables
        /// <summary>
        /// BackgroundWorker for creating the ISO file
        /// </summary>
        BackgroundWorker bgCreator;

        /// <summary>
        /// FileStream for reading
        /// </summary>
        FileStream streamReader;

        /// <summary>
        /// FileStream for writing
        /// </summary>
        FileStream streamWriter;
        #endregion

        #region Constants
        /// <summary>
        /// 4MB block size
        /// </summary>
        const int BUFFER = 0x400000;

        /// <summary>
        /// 4 GB maximum size per file on FAT32 file system
        /// </summary>
        const long LIMIT = 4294967296;

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
        /// Path to the ISO file
        /// </summary>
        string PathToIso { get; set; }

        /// <summary>
        /// Size of the medium
        /// </summary>
        public long MediumSize { get; set; }

        /// <summary>
        /// Medium handle
        /// </summary>
        SafeFileHandle Handle { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public IsoFromMedia() //todo: vymazat
        {
            bgCreator = new BackgroundWorker();
            bgCreator.WorkerSupportsCancellation = true;
            bgCreator.DoWork += new DoWorkEventHandler(bgCreator_DoWork);
            //bgCreator.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgCreator_RunWorkerCompleted);
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
                streamReader = new FileStream(Handle, FileAccess.Read, BUFFER);
                streamWriter = new FileStream(PathToIso, FileMode.Create, FileAccess.Write, FileShare.Read, BUFFER);

                byte[] buffer = new byte[BUFFER];

                //Read buffer blocks from source and write them to the ISO file
                do
                {
                    /*if (bgCreator.CancellationPending)
                    {
                        e.Cancel = true;
                        Stop();
                        break;
                    }*/

                    streamReader.Read(buffer, 0, BUFFER);
                    streamWriter.Write(buffer, 0, BUFFER);
                    byte[] bufferClone = buffer.ToArray();

                    if (OnProgress != null)
                    {
                        //Progress in percent
                        int percent = Convert.ToInt32((streamWriter.Length * 100) / MediumSize);
                        EventIsoArgs eArgs = new EventIsoArgs(streamWriter.Position, percent);
                        OnProgress(eArgs);
                    }
                } while (streamReader.Position == streamWriter.Position);
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
        /// Creates an ISO image from media (CD/DVD)
        /// </summary>
        /// <param name="source">CD/DVD</param>
        /// <param name="destination">Path where the ISO file is to be stored</param>
        /// <returns>
        /// Running = Creation in progress
        /// InvalidHandle = Invalid handle
        /// NoDevice = The source is not a medium (CD/DVD)
        /// NotEnoughMemory = Not enough disk space
        /// LimitExceeded = Source exceeds FAT32 maximum file size of 4 GB (4096 MB)
        /// NotReady = The device is not ready
        /// </returns>
        public IsoState CreateIsoFromMedia(string source, string destination)
        {
            //Is the device ready?
            if (!new DriveInfo(source).IsReady)
                return IsoState.NotReady;

            //Source CD/DVD (aplikaciu je v plane pouzivat i na USB a floppy)
            //if (new DriveInfo(source).DriveType != DriveType.CDRom)
            //    return IsoState.NoDevice;

            //Get medium size
            MediumSize = GetMediumLength(source);

            //Check disk space
            long diskSize = new DriveInfo(System.IO.Path.GetPathRoot(destination)).AvailableFreeSpace;

            if (diskSize <= MediumSize)
                return IsoState.NotEnoughMemory;

            //Check capacity of > 4096 MB (NTFS)
            if (!CheckNTFS(destination) && MediumSize >= LIMIT)
                return IsoState.LimitExceeded;

            //Create handle
            Handle = Win32.CreateFile(source);

            if (!string.IsNullOrEmpty(destination))
                PathToIso = destination;

            //If invalid or closed handle
            if (Handle.IsInvalid || Handle.IsClosed)
                return IsoState.InvalidHandle;

            //Create thread to create the ISO file
            bgCreator.RunWorkerAsync();

            return IsoState.Running;
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

            if (streamReader != null)
            {
                streamReader.Close();
                streamReader.Dispose();
            }

            if (streamWriter != null)
            {
                streamWriter.Close();
                streamWriter.Dispose();
            }

            if (Handle != null)
            {
                Handle.Close();
                Handle.Dispose();
            }
        }

        /// <summary>
        /// Size of media (CD/DVD)
        /// </summary>
        /// <param name="drive">Source drive</param>
        /// <returns>Size in bytes</returns>
        public long GetMediumLength(string drive)
        {
            return new DriveInfo(drive).TotalSize;
        }

        /// <summary>
        /// Checks if filesystem is NTFS
        /// </summary>
        /// <param name="destination">Path to ISO file</param>
        /// <returns>True if NTFS</returns>
        public bool CheckNTFS(string destination)
        {
            return new DriveInfo(System.IO.Path.GetPathRoot(destination)).DriveFormat == "NTFS" ? true : false;
        }
        #endregion
    }

    #region EventIsoArgs
    public delegate void IsoEventHandler(EventIsoArgs e);

    /// <summary>
    /// Contains additional data for the event
    /// </summary>
    public class EventIsoArgs : EventArgs
    {
        /// <summary>
        /// Already written bytes
        /// </summary>
        public long WrittenSize { get; private set; }

        /// <summary>
        /// Progress in percent
        /// </summary>
        public int ProgressPercent { get; private set; }

        /// <summary>
        /// Elapsed time
        /// </summary>
        public TimeSpan ElapsedTime { get; private set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; private set; }

        public EventIsoArgs(TimeSpan value)
            : base()
        {
            ElapsedTime = value;
        }

        public EventIsoArgs(long value, int percent)
            : base()
        {
            WrittenSize = value;
            ProgressPercent = percent;
        }

        public EventIsoArgs(string value)
            : base()
        {
            Message = value;
        }
    }
    #endregion

    #region Win32
    /// <summary>
    /// Provices required functionality
    /// </summary>
    internal class Win32
    {
        /// <summary>
        /// Read access
        /// </summary>
        static uint GENERIC_READ = 0x80000000;

        /// <summary>
        /// Indicates that subsequent opening operations are successful only when read access to the object is requested
        /// </summary>
        static uint FILE_SHARE_READ = 0x1;

        /// <summary>
        /// Opens the file. Fails if file not exists
        /// </summary>
        static uint OPEN_EXISTING = 0x3;

        /// <summary>
        /// Indicates that the file has no other attributes. This attribute is valid only if it is used alone.
        /// </summary>
        static uint FILE_ATTRIBUTE_NORMAL = 0x00000080;

        /// <summary>
        /// Returns handle that can be used to access a file or device in different ways
        /// </summary>
        /// <param name="lpFileName">Name of file or device to be opened</param>
        /// <param name="dwDesiredAccess">Access to requested file or device</param>
        /// <param name="dwShareMode">Requested share mode of file or device</param>
        /// <param name="lpSecurityAttributes">Pointer to a security attribute</param>
        /// <param name="dwCreationDisposition">An action, which is performed on a file or a device if it is present or not present</param>
        /// <param name="dwFlagsAndAttributes">File/device attribute, FILE_ATTRIBUTE_NORMAL is most frequently used</param>
        /// <param name="hTemplateFile">Handle to a template file</param>
        /// <returns>A handle to the device/file</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        /// <summary>
        /// Creates the handle of the media
        /// </summary>
        /// <param name="device">Media (CD/DVD)</param>
        /// <returns>Handle of media</returns>
        public static SafeFileHandle CreateFile(string device)
        {
            //Check how drive letter was entered
            //e.g. Z:\ -> Z: else change nothing
            string devName = device.EndsWith(@"\") ? device.Substring(0, device.Length - 1) : device;

            //Create handle
            IntPtr devHandle = CreateFile(string.Format(@"\\.\{0}", devName), GENERIC_READ, FILE_SHARE_READ, IntPtr.Zero,
                OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero);

            return new SafeFileHandle(devHandle, true);
        }
    }
    #endregion
}
