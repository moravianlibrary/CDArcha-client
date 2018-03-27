using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CDArcha_klient.Classes
{
    interface IsoFromMediaInterface
    {
        /// <summary>
        /// Create ISO from source drive to destination file
        /// </summary>
        IsoState CreateIsoFromMedia(string source, string destination);

        /// <summary>
        /// Stop processing
        /// </summary>
        void Stop();

        /// <summary>
        /// Worker
        /// </summary>
        void bgCreator_DoWork(object sender, DoWorkEventArgs e);
    }

    #region Enumeration
    public enum IsoState
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
        /// Unknown device
        /// </summary>
        NoDevice = -2,
        /// <summary>
        /// Not enough memory remaining
        /// </summary>
        NotEnoughMemory = -3,
        /// <summary>
        /// 4 GB maximum size per file on FAT32 file system
        /// </summary>
        LimitExceeded = -4,
        /// <summary>
        /// The device is not ready
        /// </summary>
        NotReady = -5
    }
    #endregion
}
