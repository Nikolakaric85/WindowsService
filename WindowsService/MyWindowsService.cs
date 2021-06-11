using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;

namespace WindowsService
{
    public partial class MyWindowsService : ServiceBase
    {
        public MyWindowsService()
        {
            InitializeComponent();
           // FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(@"\\192.168.2.254\Eurolog ICT\11. Software\Test_wsCoreda");
            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(@"E:\wsTest");
           // FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(@"C:\temp");
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
                bool success = false;
                int retry = 0;

                while (!success && retry < 3)
                {

                    try
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
                             //  using (StreamReader file = new StreamReader(@"\\192.168.2.254\Eurolog ICT\11. Software\Test_wsCoreda" + e.Name))
                                // using (StreamReader file = new StreamReader(@"C:\temp\" + e.Name))
                                 using (StreamReader file = new StreamReader(@"E:\wsTest\" + e.Name))
                                {
                                    while ((line = file.ReadLine()) != null)
                                    {
                                        command.Parameters["@line1"].Value = line;
                                        command.Parameters["@line2"].Value = DateTime.Now;
                                        command.ExecuteNonQuery();
                                    }
                                    command.Dispose();
                                    file.Dispose();
                                    file.Close();
                                }

                                Thread.Sleep(1000);
                            }
                            sqlConnection.Close();
                            //  EventLog.WriteEntry("Status konekcije je " + sqlConnection.State, EventLogEntryType.Information);

                        }

                        string destinationPath = @"E:\wsTest\OK\" + e.Name + " " + DateTime.Now.ToString("dddd, dd MMMM yyyy, HH mm ss") + ".txt";
                        File.Move(@"E:\wsTest" + e.Name, destinationPath);

                        //string destinationPath = @"C:\temp\OK\" + e.Name + " " + DateTime.Now.ToString("dddd, dd MMMM yyyy, HH mm ss") + ".txt";
                        //File.Move(@"C:\temp\Test_ok.txt", destinationPath);

                        success = true;


                    }//kraj try

                    catch (Exception exp)
                    {
                        retry++;

                        if (retry >= 3)
                        {
                            EventLog.WriteEntry("Broj pokusaja  " + retry, EventLogEntryType.Warning);
                            moveBadFile(e.Name);

                        }
                        EventLog.WriteEntry("Poruka  " + exp.Message, EventLogEntryType.Warning);

                    }

                } //kraj while (!success && retry < 3)


            }
            else
            {
                //EventLog.WriteEntry("Neispravan fajl " + e.Name, EventLogEntryType.Warning);
                //string destinationPath = @"C:\temp\Bad\" + e.Name + " " + DateTime.Now.ToString("dddd, dd MMMM yyyy, HH mm ss") + ".txt";
                //File.Move(@"C:\temp\" + e.Name, destinationPath);

                //EventLog.WriteEntry("Neispravan fajl " + e.Name, EventLogEntryType.Warning);
                //string destinationPath = @"\\192.168.2.254\Eurolog ICT\11. Software\Test_wsCoreda\Bad\" + e.Name + " " + DateTime.Now.ToString("dddd, dd MMMM yyyy, HH mm ss") + ".txt";
                //File.Move(@"\\192.168.2.254\Eurolog ICT\11. Software\Test_wsCoreda" + e.Name, destinationPath);

                EventLog.WriteEntry("Neispravan fajl " + e.Name, EventLogEntryType.Warning);
                string destinationPath = @"E:\wsTest\Bad\" + e.Name + " " + DateTime.Now.ToString("dddd, dd MMMM yyyy, HH mm ss") + ".txt";
                File.Move(@"E:\wsTest\" + e.Name, destinationPath);
            }
        }

        public void moveBadFile(string name)
        {
            //// EventLog.WriteEntry("Broj pokusaja " + retry, EventLogEntryType.Warning);
            //EventLog.WriteEntry("Neispravan fajl " + name, EventLogEntryType.Warning);
            //string destinationP = @"C:\temp\Bad\" + DateTime.Now.ToString("dddd, dd MMMM yyyy, HH mm ss") + " " + name;
            //File.Move(@"C:\temp\" + name, destinationP);

            //string destinationPath = @"\\192.168.2.254\Eurolog ICT\11. Software\Test_wsCoreda\Bad\" + name + " " + DateTime.Now.ToString("dddd, dd MMMM yyyy, HH mm ss") + ".txt";
            //File.Move(@"\\192.168.2.254\Eurolog ICT\11. Software\Test_wsCoreda" + name, destinationPath);

            string destinationPath = @"E:\wsTest\Bad\" + name + " " + DateTime.Now.ToString("dddd, dd MMMM yyyy, HH mm ss") + ".txt";
            File.Move(@"E:\wsTest\Bad\" + name, destinationPath);
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


    string destinationPath = @"C:\temp\OK\" + e.Name + " " + DateTime.Now.ToString("dddd, dd MMMM yyyy, HH mm ss") + ".txt";
    File.Move(@"C:\temp\Test_ok.txt", destinationPath);



 */


/*
 https://stackoverflow.com/questions/33482366/read-line-by-line-from-text-file-and-save-to-database
 */


/*
 
  ako ima gresaka 

        int result = command.ExecuteNonQuery();

 */