using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using System.Net.Mail;
using System.Net;
using System.Configuration;
using Microsoft.SqlServer.Dts.Runtime;
using System.Collections;
using System.Reflection;

namespace JobSchedulerAgent
{
    public partial class Service1 : ServiceBase
    {
        public bool TaskRunning = false;

        System.Threading.Thread InitThread;
        System.Threading.Thread ServiceThread;

        string myMachineName = string.Empty;
        private bool ServiceTerminating = false;
        EventLog el;
        DataSet dsJobs = new DataSet();
        public int SleepDelay = 60000;
        ScriptRunner vbScript = new ScriptRunner();

        int MaxThreads = 1;

        List<Variable> Variables = new List<Variable>();
        List<Task> Tasks = new List<Task>();

        int MachineID = 0;
        string MailServer = "localhost";
        string MailID = string.Empty;
        string MailPassword = string.Empty;
        string NotifyEmail = string.Empty;

        bool IsDebug = false;
        bool RefreshTasksActive = true;
        bool JobActive = false;

        DataClass.TParams[] Parms;
        DataClass db = new DataClass(ConfigurationManager.ConnectionStrings["JobScheduler"].ConnectionString);

        public Service1()
        {
            InitializeComponent();
            if (Environment.UserInteractive)
            {
                string[] args = { "" };
                OnStart(args);
            }
        }

        protected override void OnStart(string[] args)
        {
            this.AutoLog = true;
            myMachineName = System.Environment.MachineName;

            // Get List of Categories and Counters to capture
            try
            {
                InitThread = new System.Threading.Thread(new System.Threading.ThreadStart(InitialLoad));
                InitThread.Start();
                ServiceThread = new System.Threading.Thread(new System.Threading.ThreadStart(ServiceAgent));
                ServiceThread.Start();
            }
            catch (Exception ex)
            {
                EventLog e = new EventLog("Application", myMachineName, "JobScheduler");
                e.WriteEntry("OnStart - Service failed due to " + ex.Message, EventLogEntryType.Error);
                // Failed - so stop service
                this.Stop();
            }
        }

        protected override void OnStop()
        {
            ServiceTerminating = true;
            EventLog el = new EventLog("Application", this.myMachineName, "JobScheduler");
            string LogMsg = "JobScheduler Batch Agent Service Ended.";
            el.WriteEntry(LogMsg, EventLogEntryType.Information);
            if (InitThread.IsAlive)
            {
                InitThread.Abort();
            }
            // Wait for Thread to terminate
            while (TaskRunning)
            {
                Thread.Sleep(500);
            }
            //TaskRunning = false;

            if (!Environment.UserInteractive)
            {
                this.ExitCode = 0;
                //this.Stop();
            }
        }

        private void InitialLoad()
        {
            try
            {
                // Get Machine Name
                myMachineName = System.Environment.MachineName;

                if (!EventLog.SourceExists("JobScheduler"))
                {
                    EventLog.CreateEventSource("JobScheduler", "Application");
                }
                string LogMsg = "JobScheduler Batch Agent Service Started.";
                el = new EventLog("Application", myMachineName, "JobScheduler");
                el.Source = "JobScheduler Batch Agent";
                el.WriteEntry(LogMsg, EventLogEntryType.Information);
                el.WriteEntry("JobScheduler Batch Agent starting", EventLogEntryType.Information);

                while (!ServiceTerminating)
                {
                    System.Threading.Thread.Sleep(SleepDelay);  // Wait for 1 minutes for next test
                }
                EventLog e = new EventLog("Application", myMachineName, "JobScheduler Batch Agent");
                e.WriteEntry("Initial Load - Dataset Load is complete", EventLogEntryType.Information);
                TaskRunning = false;

            }
            catch (Exception ex)
            {
                EventLog e = new EventLog("Application", myMachineName, "JobScheduler Batch Agent");
                e.WriteEntry("Initial Load - Service failed due to " + ex.Message, EventLogEntryType.Error);
                // Failed - so stop service
                this.Stop();
            }
        }

        private void refreshTasks()
        {
            EventLog el = new EventLog("Application", myMachineName, "JobScheduler");
            string LogMsg = "Refreshing Dataset";
            //el.Source = "Performance Logging";
            //el.WriteEntry(LogMsg, EventLogEntryType.Information);
            var host = Dns.GetHostEntry(Dns.GetHostName());

            IEnumerable<IPAddress> IPList = (from ip in host.AddressList where !IPAddress.IsLoopback(ip) select ip).ToList();
            string IP4Address = string.Empty;
            string IP6Address = string.Empty;
            foreach (IPAddress a in IPList)
            {
                if (a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    IP6Address = a.ToString();
                }
                else
                {
                    IP4Address = a.ToString();
                }
            }
            // Grab Job/Tasks to be run
            Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "GetRun"),
                    new DataClass.TParams("ServerName", 50, SqlDbType.VarChar, host.HostName)
            };
            dsJobs = db.DIRunSPretDs(Parms, "sp_SchedulerJob");

            // Extract the Variables
            if (dsJobs == null)
            {
                return;
            }
            if (dsJobs.Tables.Count == 0)
            {
                // Could not find any information for this computer
                return;
            }
            RefreshTasksActive = true;
            while (JobActive)
            {
                Thread.Sleep(500);
            }
            if (dsJobs.Tables[1] != null)
            {
                Variables.Clear();
                foreach (DataRow dr in dsJobs.Tables[1].Rows)
                {
                    Variables.Add(new Variable(dr["VariableName"].ToString(), dr["VariableValue"].ToString()));
                    if (dr["VariableName"].ToString() == "NotifyEmail")
                    {
                        NotifyEmail = dr["VariableValue"].ToString();
                        Utilities.EmailNotify = NotifyEmail;
                    }
                }
            }
        
            if (dsJobs.Tables[2] != null)
            {
                if (dsJobs.Tables[2].Rows.Count > 0)
                {
                    DataRow drSvr = dsJobs.Tables[2].Rows[0];
                    MachineID = int.Parse(drSvr["SchedulerServerID"].ToString());
                    MailServer = drSvr["MailServer"].ToString();
                    MailID = drSvr["MailID"].ToString();
                    MailPassword = drSvr["MailPassword"].ToString();
                    if (drSvr["MaxThreads"].ToString() != string.Empty)
                    {
                        MaxThreads = int.Parse(drSvr["MaxThreads"].ToString());
                    }

                }
            }
            if (dsJobs.Tables[0] != null)
            {
                Tasks.Clear();
                foreach (DataRow dr in dsJobs.Tables[0].Rows)
                {
                    try
                    {
                        int? SchedulerIntervalID = null;
                        if (dr["SchedulerIntervalID"].ToString() != string.Empty)
                        {
                            SchedulerIntervalID = int.Parse(dr["SchedulerIntervalID"].ToString());
                        }
                        Tasks.Add(new Task(dr["SchedulerJobID"].ToString(), dr["SchedulerTaskID"].ToString(), dr["TaskInformation"].ToString().Replace("<![CDATA[", "").Replace("]]>", ""),
                            dr["NextRunTime"].ToString(), dr["MaxInstances"].ToString(),
                            dr["MaxMinutes"].ToString(), dr["TaskTypeName"].ToString(), dr["TaskTypeCode"].ToString(),
                            SchedulerIntervalID));
                    }
                    catch (Exception ex)
                    {
                        string Err = ex.Message;
                        Utilities.SendEmail(System.Environment.MachineName + " Agent Failure", ex.ToString());
                    }
                }
            }
            RefreshTasksActive = false;
        }


        /// <summary>
        ///  TODO::: need to enhance the following to enable multiple threads to the MaxThread limit
        /// </summary>
        private void ServiceAgent()
        {
            while (!ServiceTerminating)
            {
                refreshTasks();
                DateTime StartTime = DateTime.Now;
                DateTime EndTime = StartTime;
                if (Tasks.Count > 0)
                {
                    int TasksExecuting = 0;
                    int CurrentJobID = 0;
                    int CurrentLogID = 0;
                    JobActive = true;
                    foreach (Task t in Tasks)
                    {
                        if (CurrentJobID == 0)
                        {
                            CurrentJobID = t.JobID;
                        }
                        if (CurrentJobID != t.JobID)
                        {
                            // Job must be complete - Flag as completed and calculate next run time.
                            UpdateJobAndCalcNextRun(CurrentJobID, CurrentLogID, myMachineName);
                        }
                        // First Find the Job Log ID
                        Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "GetLogID"),
                                new DataClass.TParams("SchedulerJobID", 4, SqlDbType.Int, t.JobID),
                                new DataClass.TParams("ExecutionScheduled", 8, SqlDbType.DateTime, t.NextRunTime)
                        };
                        DataSet dsJobLog = db.DIRunSPretDs(Parms, "sp_SchedulerJobLog");
                        t.SchedulerJobLogId = int.Parse(dsJobLog.Tables[0].Rows[0]["SchedulerJobLogId"].ToString());
                        CurrentLogID = (int)t.SchedulerJobLogId;
                        CurrentJobID = t.JobID;

                        // then run task
                        if (RefreshTasksActive)
                        {
                            JobActive = false;
                            while (RefreshTasksActive)
                            {
                                Thread.Sleep(500);
                            }
                        }
                        int SchedulerTaskID = t.TaskID;

                        DateTime ExecutionScheduled = t.NextRunTime;
                        string StandardOutput = string.Empty;
                        string StandardError = string.Empty;
                        DateTime ExecutionStart = DateTime.Now;
                        DateTime ExecutionEnd = DateTime.Now;
                        string ExecutionCommandLine = string.Empty;
                        DataSet dsTask = Utilities.ConvertXmlToDataSet(t.TaskInformation);
                        TaskRunning = true;
                        // Build Execution agent based on type of task
                        switch (t.TaskTypeCode)
                        {
                            case "EMAIL":
                                {
                                    // Process Email task
                                    string TaskEmail = t.TaskInformation;

                                    if (TaskEmail != string.Empty)
                                    {
                                        try
                                        {
                                            // Get information
                                            string From = dsTask.Tables["Email"].Rows[0]["FromAddress"].ToString();
                                            string SendTo = dsTask.Tables["Email"].Rows[0]["ToAddress"].ToString();
                                            string Subject = dsTask.Tables["Email"].Rows[0]["Subject"].ToString();
                                            string Body = dsTask.Tables["Email"].Rows[0]["Body"].ToString();
                                            // Now send email
                                            MailMessage email = new MailMessage(From, SendTo.Replace(";", ","));
                                            email.Subject = Subject;
                                            email.Body = Body;
                                            SmtpClient client = new SmtpClient(MailServer);
                                            client.UseDefaultCredentials = false;
                                            client.Credentials = new System.Net.NetworkCredential(MailID, MailPassword);
                                            client.Send(email);
                                            StandardOutput = "Successful";
                                        }
                                        catch (Exception ex)
                                        {
                                            StandardError = ex.ToString();
                                            Utilities.SendEmail(System.Environment.MachineName + " Email Task Failed", ex.ToString());
                                        }
                                    }
                                    break;
                                }
                            case "EXECUTE":
                                {
                                    DataRow drT = dsTask.Tables["Execute"].Rows[0];
                                    // Use ProcessStartInfo class
                                    ProcessStartInfo startInfo = new ProcessStartInfo();
                                    startInfo.CreateNoWindow = false;
                                    startInfo.UseShellExecute = false;
                                    startInfo.FileName = drT["Path"].ToString() + "\\" + drT["Command"].ToString();
                                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                    startInfo.Arguments = drT["Parms"].ToString();
                                    startInfo.RedirectStandardError = true;
                                    startInfo.RedirectStandardOutput = true;
                                    ExecutionCommandLine = startInfo.FileName + " " + drT["Parms"].ToString();

                                    try
                                    {
                                        // Start the process with the info we specified.
                                        // Call WaitForExit and then the using statement will close.
                                        using (Process exeProcess = Process.Start(startInfo))
                                        {

                                            StandardOutput = exeProcess.StandardOutput.ReadToEnd();

                                            StandardError = exeProcess.StandardError.ReadToEnd();
                                            exeProcess.WaitForExit();
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        StandardError = ex.ToString();
                                        Utilities.SendEmail(System.Environment.MachineName + " Execute Task Failed", ex.ToString());
                                        // Log error.
                                    }
                                    break;
                                }
                            case "SQL":
                                {
                                    try
                                    {
                                        DataRow drT = dsTask.Tables["SQL"].Rows[0];
                                        DataClass db = new DataClass(drT["Connection"].ToString());
                                        DataSet dsResults = db.DIRunSQLretDs(drT["Code"].ToString());
                                        StandardOutput = dsResults.GetXml();
                                        StandardError = string.Empty;
                                    }
                                    catch (Exception ex)
                                    {
                                        StandardOutput += Environment.NewLine + "Task Failed";
                                        StandardError = "Exception: " + ex.Message;
                                        Utilities.SendEmail(System.Environment.MachineName + " SQL Task Failed", ex.ToString());
                                    }
                                    break;
                                }
                            case "SP":
                                {
                                    // Database run
                                    try
                                    {
                                        DataRow drT = dsTask.Tables["SP"].Rows[0];
                                        DataClass db = new DataClass(drT["Connection"].ToString());
                                        // Create Variable list

                                        DataClass.TParams[] Parms = new DataClass.TParams[dsTask.Tables["Parms"].Rows.Count];
                                        int i = 0;
                                        foreach (DataRow drP in dsTask.Tables["Parms"].Rows)
                                        {
                                            Parms[i].Name = drP["Name"].ToString();
                                            // Evaluate Data Type in Record
                                            SqlDbType dbt = SqlDbType.VarChar;
                                            switch (drP["DataType"].ToString())
                                            {
                                                case "BigInt":
                                                    {
                                                        dbt = SqlDbType.BigInt;
                                                        Parms[i].Size = 8;
                                                        break;
                                                    }
                                                case "Binary":
                                                    {
                                                        dbt = SqlDbType.Binary;
                                                        Parms[i].Size = 8;
                                                        break;
                                                    }
                                                case "Bit":
                                                    {
                                                        dbt = SqlDbType.Bit;
                                                        Parms[i].Size = 1;
                                                        break;
                                                    }
                                                case "Char":
                                                    {
                                                        dbt = SqlDbType.Char;
                                                        break;
                                                    }
                                                case "Date":
                                                    {
                                                        dbt = SqlDbType.Date;
                                                        Parms[i].Size = 3;
                                                        break;
                                                    }
                                                case "DateTime":
                                                    {
                                                        dbt = SqlDbType.DateTime;
                                                        Parms[i].Size = 8;
                                                        break;
                                                    }
                                                case "DateTime2":
                                                    {
                                                        dbt = SqlDbType.DateTime2;
                                                        Parms[i].Size = 8;
                                                        break;
                                                    }
                                                case "DateTimeOffset":
                                                    {
                                                        dbt = SqlDbType.DateTimeOffset;
                                                        Parms[i].Size = 10;
                                                        break;
                                                    }
                                                case "Decimal":
                                                    {
                                                        dbt = SqlDbType.Decimal;
                                                        Parms[i].Size = 17;
                                                        break;
                                                    }
                                                case "Float":
                                                    {
                                                        dbt = SqlDbType.Float;
                                                        Parms[i].Size = 8;
                                                        break;
                                                    }
                                                case "Image":
                                                    {
                                                        dbt = SqlDbType.Image;
                                                        Parms[i].Size = drP["Value"].ToString().Length;
                                                        break;
                                                    }
                                                case "Int":
                                                    {
                                                        dbt = SqlDbType.Int;
                                                        Parms[i].Size = 4;
                                                        break;
                                                    }
                                                case "Money":
                                                    {
                                                        dbt = SqlDbType.Money;
                                                        Parms[i].Size = 8;
                                                        break;
                                                    }
                                                case "NChar":
                                                    {
                                                        dbt = SqlDbType.NChar;
                                                        Parms[i].Size = drP["Value"].ToString().Length;
                                                        break;
                                                    }
                                                case "NText":
                                                    {
                                                        dbt = SqlDbType.NText;
                                                        Parms[i].Size = drP["Value"].ToString().Length;
                                                        break;
                                                    }
                                                case "NVarChar":
                                                    {
                                                        dbt = SqlDbType.NVarChar;
                                                        Parms[i].Size = drP["Value"].ToString().Length;
                                                        break;
                                                    }
                                                case "Real":
                                                    {
                                                        dbt = SqlDbType.Real;
                                                        Parms[i].Size = 4;
                                                        break;
                                                    }
                                                case "SmallDateTime":
                                                    {
                                                        dbt = SqlDbType.SmallDateTime;
                                                        Parms[i].Size = 4;
                                                        break;
                                                    }
                                                case "SmallInt":
                                                    {
                                                        dbt = SqlDbType.SmallInt;
                                                        Parms[i].Size = 2;
                                                        break;
                                                    }
                                                case "SmallMoney":
                                                    {
                                                        dbt = SqlDbType.SmallMoney;
                                                        Parms[i].Size = 4;
                                                        break;
                                                    }
                                                case "Structured":
                                                    {
                                                        dbt = SqlDbType.Structured;
                                                        Parms[i].Size = drP["Value"].ToString().Length;
                                                        break;
                                                    }
                                                case "Text":
                                                    {
                                                        dbt = SqlDbType.Text;
                                                        Parms[i].Size = drP["Value"].ToString().Length;
                                                        break;
                                                    }
                                                case "Time":
                                                    {
                                                        dbt = SqlDbType.Time;
                                                        Parms[i].Size = 5;
                                                        break;
                                                    }
                                                case "TimeStamp":
                                                    {
                                                        dbt = SqlDbType.Timestamp;
                                                        Parms[i].Size = drP["Value"].ToString().Length;
                                                        break;
                                                    }
                                                case "TinyInt":
                                                    {
                                                        dbt = SqlDbType.TinyInt;
                                                        Parms[i].Size = 1;
                                                        break;
                                                    }
                                                case "UDT":
                                                    {
                                                        dbt = SqlDbType.Udt;
                                                        Parms[i].Size = drP["Value"].ToString().Length;
                                                        break;
                                                    }
                                                case "UniqueIdentifier":
                                                    {
                                                        dbt = SqlDbType.UniqueIdentifier;
                                                        Parms[i].Size = 16;
                                                        break;
                                                    }
                                                case "VarBinary":
                                                    {
                                                        dbt = SqlDbType.VarBinary;
                                                        Parms[i].Size = drP["Value"].ToString().Length;
                                                        break;
                                                    }
                                                case "VarChar":
                                                    {
                                                        dbt = SqlDbType.VarChar;
                                                        Parms[i].Size = drP["Value"].ToString().Length;
                                                        break;
                                                    }
                                                case "Variant":
                                                    {
                                                        dbt = SqlDbType.Variant;
                                                        Parms[i].Size = drP["Value"].ToString().Length;
                                                        break;
                                                    }
                                                case "Xml":
                                                    {
                                                        dbt = SqlDbType.Xml;
                                                        Parms[i].Size = drP["Value"].ToString().Length;
                                                        break;
                                                    }
                                            }
                                            Parms[i].dtDataType = dbt;
                                            Parms[i].oValue = drP["Value"].ToString();
                                            i++;
                                        }
                                        DataSet dsResults = db.DIRunSPretDs(Parms, drT["Name"].ToString());
                                        StandardOutput = dsResults.GetXml();
                                        StandardError = string.Empty;
                                    }
                                    catch (Exception ex)
                                    {
                                        StandardOutput += Environment.NewLine + "Task Failed";
                                        StandardError = "Exception: " + ex.Message;
                                        Utilities.SendEmail(System.Environment.MachineName + " SQL SP Task Failed", ex.ToString());
                                    }
                                    break;
                                }
                            case "FILECOPY":
                                {
                                    try
                                    {
                                        DataRow drF = dsTask.Tables["Copy"].Rows[0];
                                        string SourceDirectory = drF["SourcePath"].ToString();
                                        string DestinationDirectory = drF["DestPath"].ToString();
                                        string FileList = drF["List"].ToString();
                                        int CopyCount = 0;
                                        foreach (string FileName in FileList.Split(",".ToCharArray()))
                                        {
                                            File.Copy(SourceDirectory + "\\" + FileName.Trim(), DestinationDirectory + "\\" + FileName.Trim());
                                            CopyCount++;
                                        }
                                        StandardOutput += Environment.NewLine + "File Copy Successful - " + CopyCount.ToString() + " Files Copied.";
                                    }
                                    catch (Exception ex)
                                    {
                                        StandardOutput += Environment.NewLine + "Task Failed";
                                        StandardError = "Exception: " + ex.Message;
                                        Utilities.SendEmail(System.Environment.MachineName + " File Copy Task Failed", ex.ToString());
                                    }
                                    break;
                                }
                            case "SCRIPT":
                                {
                                    try
                                    {
                                        DataRow drScript = dsTask.Tables["Script"].Rows[0];
                                        string Language = drScript["Language"].ToString();
                                        string Script = drScript["Code"].ToString();
                                        if (Language == "VB")
                                        {
                                            vbScript.setScript(Script);
                                            Hashtable results = vbScript.runScript();
                                            foreach (string s in results.Keys)
                                            {
                                                StandardOutput += s + " " + results[s] + Environment.NewLine;
                                            }
                                        }
                                        else if (Language == "C#")
                                        {
                                            // Run C# Script
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        StandardOutput += Environment.NewLine + "Task Failed";
                                        StandardError = "Exception: " + ex.Message;
                                        Utilities.SendEmail(System.Environment.MachineName + " Script Task Failed", ex.ToString());
                                    }
                                    break;
                                }
                            case "DTS":
                                {
                                    try
                                    {
                                        DataRow drDTS = dsTask.Tables["DTS"].Rows[0];

                                        string pkgLocation = drDTS["Path"].ToString();
                                        Application app = new Application();
                                        Package pkg = app.LoadPackage(pkgLocation, null);
                                        DTSExecResult pkgResults = pkg.Execute();
                                        StandardOutput += pkgResults.ToString();
                                    }
                                    catch (Exception ex)
                                    {
                                        StandardOutput += Environment.NewLine + "DTS Failed";
                                        StandardError = "Exception: " + ex.Message;
                                        Utilities.SendEmail(System.Environment.MachineName + " DTS Package Failed", ex.ToString());
                                    }
                                    break;
                                }
                            case "DLL":
                                {
                                    try
                                    {
                                        DataRow drDLL = dsTask.Tables["DLL"].Rows[0];
                                        string Path = drDLL["Path"].ToString();
                                        string Class = drDLL["Class"].ToString();
                                        string Method = drDLL["Method"].ToString();
                                        Assembly a = Assembly.LoadFile(Path);
                                        Type p = a.GetType(Class);
                                        MethodInfo mi = p.GetMethod(Method);
                                        mi.Invoke(null, new object[] { });  // Need to eventually handle parameters
                                        StandardOutput = string.Empty;
                                        StandardError = string.Empty;
                                    }
                                    catch (Exception ex)
                                    {
                                        StandardError = "Exception: " + ex.Message;
                                        Utilities.SendEmail(System.Environment.MachineName + " DLL Execution Failed", ex.ToString());
                                    }
                                    break;
                                }
                        }
                        ExecutionEnd = DateTime.Now;
                        TaskRunning = false;
                        // Update Job Log
                        Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "UpdateEx"),
                                new DataClass.TParams("SchedulerJobLogId", 4, SqlDbType.Int, t.SchedulerJobLogId),
                                new DataClass.TParams("ExecutionStart", 8, SqlDbType.DateTime, ExecutionStart),
                                new DataClass.TParams("ExecutionEnd", 8, SqlDbType.DateTime, ExecutionEnd)
                        };
                        db.DIRunSP(Parms, "sp_SchedulerJobLog");
                        // Send Back Results
                        Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Add"),
                                new DataClass.TParams("SchedulerTaskID", 4, SqlDbType.Int, t.TaskID),
                                new DataClass.TParams("ExecutionScheduled", 8, SqlDbType.DateTime, t.NextRunTime),
                                new DataClass.TParams("SchedulerJobLogId", 4, SqlDbType.Int, t.SchedulerJobLogId),
                                new DataClass.TParams("SchedulerServerID", 4, SqlDbType.Int, MachineID),
                                new DataClass.TParams("StandardOutput", StandardOutput.Length, SqlDbType.VarChar, StandardOutput),
                                new DataClass.TParams("StandardError", StandardError.Length, SqlDbType.VarChar, StandardError),
                                new DataClass.TParams("ExecutionStart", 8, SqlDbType.DateTime, ExecutionStart),
                                new DataClass.TParams("ExecutionEnd", 8, SqlDbType.DateTime, ExecutionEnd),
                                new DataClass.TParams("ExecutionCommandLine", ExecutionCommandLine.Length, SqlDbType.VarChar, ExecutionCommandLine),
                                new DataClass.TParams("ScheduledStart", 8, SqlDbType.DateTime, t.NextRunTime)
                        };
                        db.DIRunSP(Parms, "sp_SchedulerTaskLog");
                    }
                    // Job Completed
                    // Job must be complete - Flag as completed and calculate next run time.
                    UpdateJobAndCalcNextRun(CurrentJobID, CurrentLogID, myMachineName);
                    JobActive = false;
                }
                EndTime = StartTime.AddMinutes(1);
                if (EndTime > DateTime.Now)
                {
                    // Wait a bit before looking for the next set of jobs
                    double Milliseconds = EndTime.Subtract(StartTime).TotalMilliseconds;
                    Thread.Sleep((int)Milliseconds);
                }
            }
        }

        void addNewInput(string varName, string varVal)
        {
            vbScript.addScriptInput(varName, varVal);
        }

        void UpdateJobAndCalcNextRun(int JobID, int SchedulerJobLogId, string ServerName)
        {
            DateTime JobCompleted = DateTime.Now;
            Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "UpdateEx"),
                    new DataClass.TParams("SchedulerJobLogID", 4, SqlDbType.Int, SchedulerJobLogId),
                    new DataClass.TParams("ExecutionEnd", 8, SqlDbType.DateTime, JobCompleted)
            };
            db.DIRunSP(Parms, "sp_SchedulerJobLog");

            Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "GetTime"),
                    new DataClass.TParams("SchedulerJobID", 10, SqlDbType.VarChar, JobID),
                    new DataClass.TParams("ServerName", 50, SqlDbType.VarChar, ServerName)
            };
            DataSet dsTime = db.DIRunSPretDs(Parms, "sp_SchedulerJob");
            if (dsTime.Tables[0].Rows.Count > 0)
            {
                DataRow drInterval = dsTime.Tables[0].Rows[0];
                // Calculate Next Run Time
                DateTime LastRunTime = DateTime.Parse(drInterval["NextRunTime"].ToString());
                DateTime NextRunTime = LastRunTime;
                int Interval = int.Parse(drInterval["Interval"].ToString());
                int? StartTime = null;
                if (drInterval["StartTime"].ToString() != string.Empty)
                {
                    StartTime = int.Parse(DateTime.Parse(drInterval["StartTime"].ToString()).ToString("hhmm"));
                }
                int? EndTime = null;
                if (drInterval["EndTime"].ToString() != string.Empty)
                {
                    EndTime = int.Parse(DateTime.Parse(drInterval["EndTime"].ToString()).ToString("hhmm"));
                }
                int? ExclusionStart = null;
                if (drInterval["ExclusionStart"].ToString() != string.Empty)
                {
                    ExclusionStart = int.Parse(DateTime.Parse(drInterval["ExclusionStart"].ToString()).ToString("hhmm"));
                }
                int? ExclusionEnd = null;
                if (drInterval["ExclusionEnd"].ToString() != string.Empty)
                {
                    ExclusionEnd = int.Parse(DateTime.Parse(drInterval["ExclusionEnd"].ToString()).ToString("hhmm"));
                }

                switch (drInterval["IntervalType"])
                {
                    case "Year":
                        {
                            NextRunTime = NextRunTime.AddYears(Interval);
                            break;
                        }
                    case "Month":
                        {
                            NextRunTime = NextRunTime.AddMonths(Interval);
                            break;
                        }
                    case "Week":
                        {
                            NextRunTime = NextRunTime.AddDays(Interval * 7);
                            break;
                        }
                    case "Day":
                        {
                            NextRunTime = NextRunTime.AddDays(Interval);
                            break;
                        }
                    case "Hour":
                        {
                            NextRunTime = NextRunTime.AddHours(Interval);
                            break;
                        }
                    case "Minute":
                        {
                            NextRunTime = NextRunTime.AddMinutes(Interval);
                            break;
                        }
                }
                int NextTime = int.Parse(NextRunTime.ToString("hhmm"));
                if (ExclusionStart != null)
                {
                    if (ExclusionStart < NextTime && ExclusionEnd > NextTime)
                    {
                        NextTime = (int)ExclusionEnd;
                    }
                }
                if (StartTime > NextTime)
                {
                    NextTime = (int)StartTime;
                }
                if (NextTime > EndTime)
                {
                    NextTime = (int)StartTime;
                    NextRunTime = NextRunTime.AddDays(1);
                }
                NextRunTime = DateTime.Parse(NextRunTime.ToShortDateString() + " " + (NextTime / 100) + ":" + NextTime.ToString().Substring(NextTime.ToString().Length - 2));

                if (dsTime.Tables[1].Rows.Count > 0)
                {
                    // Adjust if Exclusions present
                    bool ChangeOccurred = false;
                    while (!ChangeOccurred)
                    {
                        if (ExclusionStart != null)
                        {
                            if (ExclusionStart < NextTime && ExclusionEnd > NextTime)
                            {
                                NextTime = (int)ExclusionEnd;
                                ChangeOccurred = true;
                            }
                        }
                        if (StartTime > NextTime)
                        {
                            NextTime = (int)StartTime;
                            ChangeOccurred = true;
                        }
                        if (NextTime > EndTime)
                        {
                            NextTime = (int)StartTime;
                            NextRunTime = NextRunTime.AddDays(1);
                            ChangeOccurred = true;
                        }
                        // Now Check Agent Exclusions
                        foreach(DataRow drEx in dsTime.Tables[0].Rows)
                        {
                            string day = NextRunTime.ToString("dddd");
                            bool ExclusionDay = bool.Parse(drEx[day].ToString());
                            StartTime = null;
                            if (drEx["StartTime"].ToString() != string.Empty)
                            {
                                StartTime = int.Parse(DateTime.Parse(drEx["StartTime"].ToString()).ToString("hhmm"));
                            }
                            EndTime = null;
                            if (drEx["EndTime"].ToString() != string.Empty)
                            {
                                EndTime = int.Parse(DateTime.Parse(drEx["EndTime"].ToString()).ToString("hhmm"));
                            }
                            if (NextRunTime.DayOfWeek.ToString() == day)
                            {
                                if (StartTime == null)
                                {
                                    NextRunTime = NextRunTime.AddDays(1);
                                    NextTime = 1;
                                    ChangeOccurred = true;
                                }
                                else
                                {
                                    if (EndTime == null)
                                    {
                                        if (NextTime > StartTime)
                                        {
                                            NextTime = 1;
                                            NextRunTime = NextRunTime.AddDays(1);
                                            ChangeOccurred = true;
                                        }
                                    }
                                    else
                                    {
                                        if (StartTime < NextTime && NextTime < EndTime)
                                        {
                                            NextTime = (int)EndTime;
                                            ChangeOccurred = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    NextRunTime = DateTime.Parse(NextRunTime.ToShortDateString() + " " + (NextTime / 100) + ":" + NextTime.ToString().Substring(NextTime.ToString().Length - 2));
                }
                Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "NextRun"),
                        new DataClass.TParams("SchedulerJobId", 4, SqlDbType.Int, JobID),
                        new DataClass.TParams("NextRunTime", 8, SqlDbType.DateTime, NextRunTime)
                };
                db.DIRunSP(Parms, "sp_SchedulerJob");
            }
            else
            {
                Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "NextRun"),
                            new DataClass.TParams("SchedulerJobID", 4, SqlDbType.Int, JobID),
                            new DataClass.TParams("NextRunTime", 8, SqlDbType.DateTime, null)
                    };
                db.DIRunSP(Parms, "sp_SchedulerJob"); // No next run time since no interval assigned
            }
            
        }
    }
}
