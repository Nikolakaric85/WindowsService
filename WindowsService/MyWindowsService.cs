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
using System.Data.SqlClient;

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
            if (e.Name == "Test_ok.txt")
            {
                try
                {
                    string conString = "Server=NIKOLA-PC\\DATALAB55;Database=test; User ID=sa;password=123";
                    string insertQuery = "INSERT INTO test1(line1, line2) VALUES(@line1, @line2)";

                    using (SqlConnection sqlConnection = new SqlConnection(conString))
                    {
                        SqlCommand command = new SqlCommand(insertQuery, sqlConnection);

                        command.Parameters.Add("@line1", SqlDbType.NVarChar);
                        command.Parameters.Add("@line2", SqlDbType.NVarChar);
                        sqlConnection.Open();
                    

                        

                   // EventLog.WriteEntry("Status konekcije je " + sqlConnection.State, EventLogEntryType.Information);

                    string line;
                    System.IO.StreamReader file = new System.IO.StreamReader(@"C:\temp\Test_ok.txt");

                        int counter = 0;

                        // da se uradi da se nadje max counter pa onda for petlja pa da tako upise u bazu da ne bi bilo ponavljanja

                    while ((line = file.ReadLine()) != null)
                    {
                        EventLog.WriteEntry("Ubačen je red " + line, EventLogEntryType.Information);
                        command.Parameters["@line1"].Value = line;
                        command.Parameters["@line2"].Value = DateTime.Now;

                        command.ExecuteNonQuery();
                            counter++;
                    }
                        EventLog.WriteEntry("Broj redova " + counter, EventLogEntryType.Warning);


                        file.Close();
                    }

                    string destinationPath = @"C:\temp\OK\" + e.Name;
                    File.Move(@"C:\temp\Test_ok.txt", destinationPath);
                    EventLog.WriteEntry("Ubačen je fajl " + e.Name, EventLogEntryType.Warning);
                }
                catch (Exception)
                {

                    //EventLog.WriteEntry("Nije prosao proces", EventLogEntryType.Error);
                }

            }
            else
            {
                try
                {
                    string destinationPath = @"C:\temp\Bad\" + DateTime.Now.ToString("dddd, dd MMMM yyyy, HH mm ss") + " " + e.Name;
                    File.Move(@"C:\temp\" + e.Name, destinationPath);
                    EventLog.WriteEntry("Neispravan fajl: " + e.Name, EventLogEntryType.Information);
                }
                catch (Exception)
                {
                    EventLog.WriteEntry("Nije prosao proces", EventLogEntryType.Error);
                }

            }


        }

        protected override void OnStop()
        {
            EventLog.WriteEntry("Stojjj", EventLogEntryType.Information);
        }
    }
}


/*
  if (e.Name == "Test_ok.txt")
            {
                try
                {
                    // string destinationPath = "C:\\temp\\OK\\note " + DateTime.Now.ToString("dddd, dd MMMM yyyy, HH mm ss") + ".xml";

                    string line;

                    List<string> myList = new List<string>();

                    System.IO.StreamReader file = new System.IO.StreamReader(@"C:\temp\Test_ok.txt");

                    while ((line = file.ReadLine()) != null)
                    {
                        EventLog.WriteEntry("Ubačen je red " + line, EventLogEntryType.Information);
                        myList.Add(line);
                    }

                    File.WriteAllLines(@"C:\temp\OK\note.txt", myList);
                    file.Close();

                    string destinationPath = @"C:\temp\OK\" + e.Name;
                    File.Move(@"C:\temp\Test_ok.txt", destinationPath);
                    EventLog.WriteEntry("Ubačen je fajl " + e.Name, EventLogEntryType.Warning);
                }
                catch (Exception)
                {

                    //EventLog.WriteEntry("Nije prosao proces", EventLogEntryType.Error);
                }

            }
            else
            {
                try
                {
                    string destinationPath = @"C:\temp\Bad\" + DateTime.Now.ToString("dddd, dd MMMM yyyy, HH mm ss") + " " + e.Name;
                    File.Move(@"C:\temp\" + e.Name, destinationPath);
                    EventLog.WriteEntry("Neispravan fajl: " + e.Name, EventLogEntryType.Information);
                }
                catch (Exception)
                {
                    EventLog.WriteEntry("Nije prosao proces", EventLogEntryType.Error);
                }

            }
 
 */