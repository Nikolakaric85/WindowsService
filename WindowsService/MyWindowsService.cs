using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Timers;

namespace WindowsService
{
    public partial class MyWindowsService : ServiceBase
    {
        public MyWindowsService()
        {
            InitializeComponent();
            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(@"C:\temp");
            fileSystemWatcher.EnableRaisingEvents = true;
            fileSystemWatcher.Created += OnCreated;
        }

        protected override void OnStart(string[] args)
        {

        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {

            if (e.Name == "Test_ok.txt")
            {
                EventLog.WriteEntry("Kreiran je fajl " + e.Name, EventLogEntryType.Information);

                string conString = @"Server=192.168.2.200\SQLEXPRESS01; Database=els; User ID=elsUser; password=els5577";
                string insertQuery = "INSERT INTO Test_wsCoreda(line1, line2) VALUES(@line1, @line2)";

                using (SqlConnection sqlConnection = new SqlConnection(conString))
                {
                    using (SqlCommand command = new SqlCommand(insertQuery, sqlConnection))
                    {

                        command.Parameters.Add("@line1", SqlDbType.Char);
                        command.Parameters.Add("@line2", SqlDbType.DateTime);
                        sqlConnection.Open();
                        EventLog.WriteEntry("Status konekcije je " + sqlConnection.State, EventLogEntryType.Warning);

                        string line;
                        System.IO.StreamReader file = new System.IO.StreamReader(@"C:\temp\Test_ok.txt");

                        while ((line = file.ReadLine()) != null)
                        {
                            command.Parameters["@line1"].Value = line;
                            command.Parameters["@line2"].Value = DateTime.Now;
                            command.ExecuteNonQuery();
                        }
                        command.Dispose();
                        file.Close();
                    }
                    sqlConnection.Close();
                    EventLog.WriteEntry("Status konekcije je " + sqlConnection.State, EventLogEntryType.Information);
                }

                string destinationPath = @"C:\temp\OK\" + e.Name+ " " + DateTime.Now.ToString("dddd, dd MMMM yyyy, HH mm ss") + ".txt";
                File.Move(@"C:\temp\Test_ok.txt", destinationPath);
            }

            else
            {
                EventLog.WriteEntry("Neispravan fajl " + e.Name, EventLogEntryType.Warning);
                string destinationPath = @"C:\temp\Bad\" + DateTime.Now.ToString("dddd, dd MMMM yyyy, HH mm ss") + " " + e.Name;
                File.Move(@"C:\temp\" + e.Name, destinationPath);
            }
        }

        protected override void OnStop()
        {
            EventLog.WriteEntry("Stoj", EventLogEntryType.Information);
        }
    }
}

/*
    string conString = @"Server=192.168.2.200\SQLEXPRESS01; Database=els; User ID=elsUser; password=els5577";
   string insertQuery = "INSERT INTO Test_wsCoreda(line1, line2) VALUES(@line1, @line2)";
 */


/*
 https://stackoverflow.com/questions/33482366/read-line-by-line-from-text-file-and-save-to-database
 */


/*
 
  ako ima gresaka 

        int result = command.ExecuteNonQuery();

 */