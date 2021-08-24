using System;
using System.Collections.Generic;
using System.Text;

namespace TeleTavleLibrary
{
    class LogEventArgs : EventArgs
    {
        public string Message { get;  set; }
        public DateTime Time { get; set; }
        public InformationType informationType { get; set; }
        public LogEventArgs()
        {

        }
        public LogEventArgs(string message, DateTime time, InformationType type)
        {
            this.Message = message;
            this.Time = time;
            this.informationType = type;
        }
    }
}
