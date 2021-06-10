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
        private readonly Timer timer;
        public MyWindowsService()
        {

            InitializeComponent();
            timer = new Timer();
            //timer.Elapsed += Checking;
        }

        protected override void OnStart(string[] args)
        {
            // Set up a timer that triggers every minute.

            timer.Interval = 5000; 
            timer.Elapsed += new ElapsedEventHandler(this.Checking);
            timer.Start();
        }

        public void Checking(object sender, ElapsedEventArgs e)
        {
            // FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(@"\\192.168.2.254\Eurolog ICT\11. Software\Test_wsCoreda");
            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(@"C:\temp");
            fileSystemWatcher.EnableRaisingEvents = true;
            fileSystemWatcher.Created += FileSystemWatcher_Created;
            string[] lines = new string[] {"Proverava temp folder " + DateTime.Now.ToString() };
            File.AppendAllLines(@"C:\temp\log.txt", lines);
        }

        private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            string[] lines = new string[] { "Proverava koliko puta poziva metodu   " + DateTime.Now.ToString() };
            File.AppendAllLines(@"C:\temp\log2.txt", lines);

            if (e.Name == "Test_ok.txt")
            {
                try
                {
                    

                    string conString = @"Server=192.168.2.200\SQLEXPRESS01; Database=els; User ID=elsUser; password=els5577";
                    string insertQuery = "INSERT INTO Test_wsCoreda(line1, line2) VALUES(@line1, @line2)";

                    using (SqlConnection sqlConnection = new SqlConnection(conString))
                    {
                        SqlCommand command = new SqlCommand(insertQuery, sqlConnection);

                        command.Parameters.Add("@line1", SqlDbType.Char);
                        command.Parameters.Add("@line2", SqlDbType.DateTime);
                        sqlConnection.Open();

                        EventLog.WriteEntry("Status konekcije je " + sqlConnection.State, EventLogEntryType.Warning);

                        // string line;
                        // System.IO.StreamReader file = new System.IO.StreamReader(@"\\192.168.2.254\Eurolog ICT\11. Software\Test_wsCoreda\Test_ok.txt");
                        // System.IO.StreamReader file = new System.IO.StreamReader(@"C:\temp\Test_ok.txt");

                        string[] file = File.ReadAllLines(@"C:\temp\" + e.Name);

                        int result = 0;

                        for (int i = 0; i < file.Length; i++)
                        {
                            EventLog.WriteEntry("Ubacen je zapis " + file[i], EventLogEntryType.Information);

                            command.Parameters["@line1"].Value = file[i];
                            command.Parameters["@line2"].Value = DateTime.Now;
                            result = command.ExecuteNonQuery();
                            command.Dispose();

                        }

                        sqlConnection.Close();
                        EventLog.WriteEntry("Broj query " + result, EventLogEntryType.Information);
                        EventLog.WriteEntry("Duzina array " + file.Length, EventLogEntryType.Information);



                        if (result != 0)
                        {

                            string destinationPath = @"C:\temp\OK\" + e.Name;
                            File.Move(@"C:\temp\Test_ok.txt", destinationPath);
                            EventLog.WriteEntry("Ubačen je fajl " + e.Name, EventLogEntryType.Warning);
                        }



                        //while ((line = file.ReadLine()) != null)
                        //{
                        //    command.Parameters["@line1"].Value = line;
                        //    command.Parameters["@line2"].Value = DateTime.Now;
                        //    command.ExecuteNonQuery();
                        //}


                        //file.Close();
                    }

                    // string destinationPath = @"\\192.168.2.254\Eurolog ICT\11. Software\Test_wsCoreda\OK\" + e.Name;


                }
                catch (Exception exp)
                {
                    EventLog.WriteEntry("Nije prosao proces - poruka " + exp.Message, EventLogEntryType.Error);
                    EventLog.WriteEntry("Nije prosao proces - data " + exp.Data, EventLogEntryType.Error);
                    EventLog.WriteEntry("Nije prosao proces - source " + exp.Source, EventLogEntryType.Error);
                }

            }
            //else
            //{
            //    try
            //    {
            //        string destinationPath = @"C:\temp\Bad\" + DateTime.Now.ToString("dddd, dd MMMM yyyy, HH mm ss") + " " + e.Name;
            //        File.Move(@"C:\temp\Bad\" + e.Name, destinationPath);
            //        EventLog.WriteEntry("Neispravan fajl: " + e.Name, EventLogEntryType.Information);
            //    }
            //    catch (Exception)
            //    {
            //        EventLog.WriteEntry("Nije prosao proces", EventLogEntryType.Error);
            //    }

            //}


        }

        protected override void OnStop()
        {
           
            EventLog.WriteEntry("Stoj", EventLogEntryType.Information);
        }
    }
}



/*
  public partial class MyWindowsService : ServiceBase
    {
        private readonly Timer timer;
        public MyWindowsService()
        {

            InitializeComponent();
            timer = new Timer(6000) { AutoReset = true };
            timer.Elapsed += Checking;
        }

        public void Checking(object sender, ElapsedEventArgs e)
        {
            // FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(@"\\192.168.2.254\Eurolog ICT\11. Software\Test_wsCoreda");
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
                    string conString = @"Server=192.168.2.200\SQLEXPRESS01; Database=els; User ID=elsUser; password=els5577";
                    string insertQuery = "INSERT INTO Test_wsCoreda(line1, line2) VALUES(@line1, @line2)";

                    using (SqlConnection sqlConnection = new SqlConnection(conString))
                    {
                        SqlCommand command = new SqlCommand(insertQuery, sqlConnection);

                        command.Parameters.Add("@line1", SqlDbType.Char);
                        command.Parameters.Add("@line2", SqlDbType.DateTime);
                        sqlConnection.Open();

                        EventLog.WriteEntry("Status konekcije je " + sqlConnection.State, EventLogEntryType.Warning);

                       // string line;
                       // System.IO.StreamReader file = new System.IO.StreamReader(@"\\192.168.2.254\Eurolog ICT\11. Software\Test_wsCoreda\Test_ok.txt");
                       // System.IO.StreamReader file = new System.IO.StreamReader(@"C:\temp\Test_ok.txt");

                        

                        string[] file = File.ReadAllLines(@"C:\temp\" + e.Name);

                        int result = 0;

                        for (int i = 0; i < file.Length; i++)
                        {
                            EventLog.WriteEntry("Ubacen je zapis " + file[i], EventLogEntryType.Information);

                            command.Parameters["@line1"].Value = file[i];
                            command.Parameters["@line2"].Value = DateTime.Now;
                            result = command.ExecuteNonQuery();
                            command.Dispose();

                        }
                       
                        sqlConnection.Close();
                        EventLog.WriteEntry("Broj query " + result, EventLogEntryType.Information);
                        EventLog.WriteEntry("Duzina array " + file.Length, EventLogEntryType.Information);



                        if (result != 0)
                        {

                            string destinationPath = @"C:\temp\OK\" + e.Name;
                            File.Move(@"C:\temp\Test_ok.txt", destinationPath);
                            EventLog.WriteEntry("Ubačen je fajl " + e.Name, EventLogEntryType.Warning);
                        }

                      

                        //while ((line = file.ReadLine()) != null)
                        //{
                        //    command.Parameters["@line1"].Value = line;
                        //    command.Parameters["@line2"].Value = DateTime.Now;
                        //    command.ExecuteNonQuery();
                        //}


                        //file.Close();
                    }

                    // string destinationPath = @"\\192.168.2.254\Eurolog ICT\11. Software\Test_wsCoreda\OK\" + e.Name;


                }
                catch (Exception exp)
                {
                    EventLog.WriteEntry("Nije prosao proces - poruka " + exp.Message, EventLogEntryType.Error);
                    EventLog.WriteEntry("Nije prosao proces - data " + exp.Data, EventLogEntryType.Error);
                    EventLog.WriteEntry("Nije prosao proces - source " + exp.Source, EventLogEntryType.Error);
                }

            }
            else
            {
                try
                {
                    string destinationPath = @"C:\temp\Bad\" + DateTime.Now.ToString("dddd, dd MMMM yyyy, HH mm ss") + " " + e.Name;
                    File.Move(@"C:\temp\Bad\" + e.Name, destinationPath);
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
            timer.Stop();
            EventLog.WriteEntry("Stoj", EventLogEntryType.Information);
        }
    }
 */















/*
 https://stackoverflow.com/questions/33482366/read-line-by-line-from-text-file-and-save-to-database
 */


/*
 
  ako ima gresaka 

        int result = command.ExecuteNonQuery();

 */