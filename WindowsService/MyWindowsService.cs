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
            timer = new Timer(1000) { AutoReset = true };
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

                        string line;
                        //  System.IO.StreamReader file = new System.IO.StreamReader(@"\\192.168.2.254\Eurolog ICT\11. Software\Test_wsCoreda\Test_ok.txt");
                        System.IO.StreamReader file = new System.IO.StreamReader(@"C:\temp\Test_ok.txt");


                        while ((line = file.ReadLine()) != null)
                        {
                            command.Parameters["@line1"].Value = line;
                            command.Parameters["@line2"].Value = DateTime.Now;
                            command.ExecuteNonQuery();
                        }

                        sqlConnection.Close();
                        file.Close();
                    }

                    // string destinationPath = @"\\192.168.2.254\Eurolog ICT\11. Software\Test_wsCoreda\OK\" + e.Name;
                    string destinationPath = @"C:\temp\OK\" + e.Name;
                    File.Move(@"C:\temp\Test_ok.txt", destinationPath);
                    EventLog.WriteEntry("Ubačen je fajl " + e.Name, EventLogEntryType.Warning);

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
            EventLog.WriteEntry("Stoj", EventLogEntryType.Information);
        }
    }
}


/*
 https://stackoverflow.com/questions/33482366/read-line-by-line-from-text-file-and-save-to-database
 */

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

/*
 * 
 * ako ima gresaka 
 * 
 using(SqlConnection connection = new SqlConnection(_connectionString))
{
    String query = "INSERT INTO dbo.SMS_PW (id,username,password,email) VALUES (@id,@username,@password, @email)";

    using(SqlCommand command = new SqlCommand(query, connection))
    {
        command.Parameters.AddWithValue("@id", "abc");
        command.Parameters.AddWithValue("@username", "abc");
        command.Parameters.AddWithValue("@password", "abc");
        command.Parameters.AddWithValue("@email", "abc");

        connection.Open();
        int result = command.ExecuteNonQuery();

        // Check Error
        if(result < 0)
            Console.WriteLine("Error inserting data into Database!");
    }
}
 
 */