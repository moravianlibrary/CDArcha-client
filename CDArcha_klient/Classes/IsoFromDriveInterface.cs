using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CDArcha_klient.Classes
{
    interface IsoFromDriveInterface
    {
        /// <summary>
        /// Create ISO from source drive to destination file
        /// </summary>
        IsoFromDriveState CreateIsoFromDrive(string source, string destination);

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
    public enum IsoFromDriveState
    {
        /// <summary>
        /// Creation running
        /// </summary>
        Running = 1,
        /// <summary>
        /// Not enough memory remaining
        /// </summary>
        NotEnoughSpace = -1,
        /// <summary>
        /// The device is not ready
        /// </summary>
        NotReady = -2
    }
    #endregion
}
