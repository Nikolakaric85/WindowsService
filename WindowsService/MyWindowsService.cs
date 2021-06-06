using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;

namespace WindowsService
{
    public partial class MyWindowsService : ServiceBase
    {
        private readonly Timer timer;
        public MyWindowsService()
        {
            
            InitializeComponent();
            timer = new Timer(1000) { AutoReset = true };
            timer.Elapsed += Checking;
        }

        public void Checking(object sender, ElapsedEventArgs e)
        {
            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(@"C:\temp");
            fileSystemWatcher.EnableRaisingEvents = true;
            fileSystemWatcher.Created += FileSystemWatcher_Created;
            
        }

        protected override void OnStart(string[] args)
        {
            timer.Start();
        }

        private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (e.Name == "note.xml")
            {
             
              
                EventLog.WriteEntry("Ubačen je neki element", EventLogEntryType.Information);
            }
            else
            {
                EventLog.WriteEntry("Nije Hearbeat.txt fajl", EventLogEntryType.Information);
            }

            
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry("Stojjj", EventLogEntryType.Information);
        }
    }
}
