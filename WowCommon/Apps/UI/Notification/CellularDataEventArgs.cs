using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apps.UI.Notification
{
    public class CellularDataEventArgs : EventArgs
    {
        public long usage { get; set; }

        public CellularDataEventArgs(long usage)
        {
            this.usage = usage;
        }
    }
}
