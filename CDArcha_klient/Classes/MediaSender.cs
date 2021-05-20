using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.ComponentModel;

namespace CDArcha_klient.Classes
{
    public class MediaSender
    {
        #region Variables
        /// <summary>
        /// BackgroundWorker for sending the ISO file
        /// </summary>
        BackgroundWorker bgSender = new BackgroundWorker();

        /// <summary>
        /// Upload params to send to server
        /// </summary>
        string UploadId;
        string UploadHash;
        string QuickId;
        bool MediumReadProblem;
        bool ForcedUpload;
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
        public event SenderEventHandler OnProgress;

        /// <summary>
        /// Raised when a message appears (errors, etc.)
        /// </summary>
        public event SenderEventHandler OnMessage;

        /// <summary>
        /// Raised when the file is finished
        /// </summary>
        public event SenderEventHandler OnFinish;
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
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public MediaSender()
        {
            bgSender.WorkerSupportsCancellation = true;
            bgSender.DoWork += new DoWorkEventHandler(bgSender_DoWork);
            bgSender.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgSender_RunWorkerCompleted);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Starts the thread creating the ISO file
        /// </summary>
        void bgSender_DoWork(object sender, DoWorkEventArgs e)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            if (this.UploadHash != "")
            {
                long MediumSize = new System.IO.FileInfo(PathToIso).Length;
                HttpWebRequest httpWebRequest = HttpWebRequest.Create(Settings.UploadLink) as HttpWebRequest;
                WebHeaderCollection httpHdr = httpWebRequest.Headers;
                httpHdr.Add("x-cdarcha-mediaid:" + this.UploadId.Replace("-", "").ToLower());
                httpHdr.Add("x-cdarcha-checksum:" + this.UploadHash.Replace("-", "").ToLower());
                httpHdr.Add("x-cdarcha-mediasize:" + MediumSize.ToString());
                httpHdr.Add("x-cdarcha-quickid:" + this.QuickId);
                httpHdr.Add("x-cdarcha-filetype:iso");
                httpHdr.Add("x-cdarcha-mediareadproblem:" + (this.MediumReadProblem ? "1" : "0"));
                httpHdr.Add("x-cdarcha-forcedupload:" + (this.ForcedUpload ? "1" : "0"));
                httpWebRequest.Timeout = 9000000;
                httpWebRequest.Method = "POST";
                httpWebRequest.SendChunked = true;
                httpWebRequest.AllowWriteStreamBuffering = false;
                httpWebRequest.ContentType = "application/octet-stream";
                Stream st = httpWebRequest.GetRequestStream();

                try
                {

                    //Read buffer blocks from source and write them to the ISO file
                    long progress = 0;
                    using (FileStream fs = File.Open(PathToIso, FileMode.Open))
                    {
                        int bufferSize = (progress + BUFFER > MediumSize) ? Convert.ToInt32(MediumSize - progress) : BUFFER;
                        byte[] buffer = new byte[bufferSize];
                        while (fs.Read(buffer, 0, bufferSize) > 0)
                        {
                            if (bgSender.CancellationPending)
                            {
                                e.Cancel = true;
                                Stop();
                                break;
                            }

                            st.Write(buffer, 0, bufferSize);
                            progress += bufferSize;

                            if (OnProgress != null)
                            {
                                //Progress
                                int percent = Convert.ToInt32((progress * 100) / MediumSize);
                                EventSenderArgs eArgs = new EventSenderArgs(progress, percent);
                                OnProgress(eArgs);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (OnMessage != null)
                    {
                        st.Close();
                        httpWebRequest.Abort();
                        EventSenderArgs eArgs = new EventSenderArgs("Error while creating the image: " + ex.Message);
                        OnMessage(eArgs);
                    }
                }
                finally
                {
                    if (OnFinish != null)
                    {
                        st.Close();
                        httpWebRequest.Abort();
                        //WebResponse response = httpWebRequest.GetResponse();
                        EventSenderArgs eArgs = new EventSenderArgs(stopWatch.Elapsed);
                        OnFinish(eArgs);
                    }
                }
            }
        }

        /// <summary>
        /// When the file is finished
        /// </summary>
        void bgSender_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CloseAll();
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
        public SenderState SendMediaToServer(string source, string destination, string uploadId, string uploadHash, string quickId, bool mediumReadProblem, bool forcedUpload)
        {
            //Get medium size
            MediumSize = GetMediumLength(source);

            //Book upload params
            UploadId = uploadId;
            UploadHash = uploadHash;
            QuickId = quickId;
            MediumReadProblem = mediumReadProblem;
            ForcedUpload = forcedUpload;

            if (!string.IsNullOrEmpty(destination))
                PathToIso = source;

            //Create thread to create the ISO file
            bgSender.RunWorkerAsync();

            return SenderState.Running;
        }

        /// <summary>
        /// Aborts the creation of the image and deletes it
        /// </summary>
        public void Stop()
        {
            CloseAll();

            if (OnMessage != null)
            {
                EventSenderArgs e = new EventSenderArgs(@"Sending was canceled");
                OnMessage(e);
            }
        }

        /// <summary>
        /// Closes all streams and handles and frees resources
        /// </summary>
        private void CloseAll()
        {
            /*if (bgSender != null)
            {
                bgSender.Dispose();
            }*/
        }

        /// <summary>
        /// Size of media (CD/DVD)
        /// </summary>
        /// <param name="drive">Source drive</param>
        /// <returns>Size in bytes</returns>
        private long GetMediumLength(string drive)
        {
            return 1;
        }
        #endregion
    }

    #region Enumeration
    /// <summary>
    /// Returns state of ISO creation
    /// </summary>
    public enum SenderState
    {
        /// <summary>
        /// Creation running
        /// </summary>
        Running = 1,
        /// <summary>
        /// Invalid handle
        /// </summary>
        InvalidHandle = -1,
        /// <summary>
        /// The device is not ready
        /// </summary>
        NotReady = -5
    }
    #endregion

    #region EventSenderArgs
    public delegate void SenderEventHandler(EventSenderArgs e);

    /// <summary>
    /// Contains additional data for the event
    /// </summary>
    public class EventSenderArgs : EventArgs
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

        public EventSenderArgs(TimeSpan value)
            : base()
        {
            ElapsedTime = value;
        }

        public EventSenderArgs(long value, int percent)
            : base()
        {
            WrittenSize = value;
            ProgressPercent = percent;
        }

        public EventSenderArgs(string value)
            : base()
        {
            Message = value;
        }
    }
    #endregion

}
