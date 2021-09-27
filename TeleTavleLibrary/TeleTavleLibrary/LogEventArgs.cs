using System;

namespace TeleTavleLibrary
{
    public class LogEventArgs : EventArgs
    {
        public string Message { get;  set; }
        public DateTime Time { get; set; }
        public InformationType InformationType { get; set; }
        public LogEventArgs()
        {

        }
        public LogEventArgs(string message, InformationType type)
        {
            this.Message = message;
            this.Time = DateTime.Now;
            this.InformationType = type;
        }
    }
}
