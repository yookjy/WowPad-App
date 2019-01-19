using Microsoft.Phone.Info;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Apps.WowPad.Model
{
    public class Version<VersionContent> : ObservableCollection<VersionContent>
    {
        public Version(string version)
        {
            VersionNumber = string.Format("Version {0}", version);
        }

        public string VersionNumber { get; private set; }
    }
}
