using System;

namespace CDArcha_klient.Classes
{

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

}
