using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Text;
using System.Configuration;

namespace JobScheduler.Controllers
{
    public class SchedulerController : Controller
    {
        public HomeModel hm = new HomeModel();
        SetupPage BasePage = new SetupPage();
        DataClass db = new DataClass(ConfigurationManager.ConnectionStrings["JobScheduler"].ConnectionString);
        DataClass.TParams[] Parms;

        // GET: Scheduler
        public ActionResult SchedulerServer()
        {
            HomeModel h = BasePage.SetupPageBasics(Request, hm, Session);
            object g = new SchedulerServerModel();
            Utility.UpdateModel(h, ref g);
            SchedulerServerModel sm = (SchedulerServerModel)g;
            sm.ShowServer = "none";

            sm.ServerID = string.Empty;
            sm.ServerName = string.Empty;
            sm.AgentEnabled = true;
            sm.IsActive = true;
            sm.MaxThreads = 0;
            sm.MailServer = string.Empty;
            sm.MailID = string.Empty;
            sm.MailPassword = string.Empty;
            sm.MachineIP4Address = string.Empty;
            sm.MachineIP6Address = string.Empty;
            sm.ServerMessage = string.Empty;
            sm.ServerList = string.Empty;

            if (sm.AdminAccess || sm.ReportAccess || sm.OperatorAccess || sm.ViewAccess)
            {
                // User can see page
                sm.ShowServer = "inline";
                string Edit = Utility.GetParam(Request.Params.ToString(), "edit");
                string Delete = Utility.GetParam(Request.Params.ToString(), "delete");
                if (Request.Params["ServerID"] != null)
                {
                    sm.ServerID = Request.Params["ServerID"].ToString();
                }
                if (Request.Params["ServerName"] != null)
                {
                    sm.ServerName = Request.Params["ServerName"].ToString();
                }
                if (Request.Params["MaxThreads"] != null)
                {
                    sm.MaxThreads = int.Parse(Request.Params["MaxThreads"].ToString());
                }
                if (Request.Params["IsActive"] != null)
                {
                    sm.IsActive = Utility.GetBoolean(Request.Params["IsActive"].ToString());
                }
                if (Request.Params["AgentEnabled"] != null)
                {
                    sm.AgentEnabled = Utility.GetBoolean(Request.Params["AgentEnabled"].ToString());
                }
                if (Request.Params["MailServer"] != null)
                {
                    sm.MailServer = Request.Params["MailServer"].ToString();
                }
                if (Request.Params["MailID"] != null)
                {
                    sm.MailID = Request.Params["MailID"].ToString();
                }
                if (Request.Params["MailPassword"] != null)
                {
                    sm.MailPassword = Request.Params["MailPassword"].ToString();
                }
                if (Request.Params["MachineIP4Address"] != null)
                {
                    sm.MachineIP4Address = Request.Params["MachineIP4Address"].ToString();
                }
                if (Request.Params["MachineIP6Address"] != null)
                {
                    sm.MachineIP6Address = Request.Params["MachineIP6Address"].ToString();
                }
                if (sm.AdminAccess)
                {
                    if (Request.Params["btnServerSave"] != null)
                    {
                        if (sm.ServerName == string.Empty)
                        {
                            sm.ServerMessage += "Server Name must be filled in. ";
                        }
                        if (sm.MachineIP4Address == string.Empty && sm.MachineIP6Address == string.Empty)
                        {
                            sm.ServerMessage = "An IP address must be filled in. ";
                        }
                        if (sm.ServerMessage == string.Empty)
                        {
                            string Command = "Add";
                            if (sm.ServerID != string.Empty)
                            {
                                Command = "Update";
                            }
                            else
                            {
                                sm.ServerID = null;
                            }
                            try
                            {
                                Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, Command),
                                        new DataClass.TParams("SchedulerServerID", 4, SqlDbType.Int, sm.ServerID),
                                        new DataClass.TParams("ServerName", 50, SqlDbType.VarChar, sm.ServerName),
                                        new DataClass.TParams("MaxThreads", 4, SqlDbType.Int, sm.MaxThreads),
                                        new DataClass.TParams("IsActive", 1, SqlDbType.Bit, sm.IsActive),
                                        new DataClass.TParams("AgentEnabled", 1, SqlDbType.Bit, sm.AgentEnabled),
                                        new DataClass.TParams("MailServer", 50, SqlDbType.VarChar, sm.MailServer),
                                        new DataClass.TParams("MailID", 100, SqlDbType.VarChar, sm.MailID),
                                        new DataClass.TParams("MailPassword", 100, SqlDbType.VarChar, DataEncyption.EncryptString(sm.MailPassword)),
                                        new DataClass.TParams("MachineIP4Address", 15, SqlDbType.VarChar, sm.MachineIP4Address),
                                        new DataClass.TParams("MachineIP6Address", 50, SqlDbType.VarChar, sm.MachineIP6Address)
                                };
                                db.DIRunSP(Parms, "sp_SchedulerServer");
                                sm.ServerID = string.Empty;
                                sm.ServerName = string.Empty;
                                sm.MaxThreads = 0;
                                sm.IsActive = true;
                                sm.AgentEnabled = true;
                                sm.MailServer = string.Empty;
                                sm.MailID = string.Empty;
                                sm.MailPassword = string.Empty;
                                sm.MachineIP4Address = string.Empty;
                                sm.MachineIP6Address = string.Empty;
                            }
                            catch (Exception ex)
                            {
                                sm.ServerMessage = ex.Message;
                            }
                        }
                    }
                }
                if (Request.Params["btnServerClear"] != null)
                {
                    // Clear Fields
                    sm.ServerID = string.Empty;
                    sm.ServerName = string.Empty;
                    sm.MaxThreads = 0;
                    sm.IsActive = true;
                    sm.AgentEnabled = true;
                    sm.MailServer = string.Empty;
                    sm.MailID = string.Empty;
                    sm.MailPassword = string.Empty;
                    sm.MachineIP4Address = string.Empty;
                    sm.MachineIP6Address = string.Empty;
                    sm.ServerMessage = string.Empty;
                }
                if (sm.AdminAccess)
                {
                    if (Edit.Length > 4)
                    {
                        Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Get"),
                                new DataClass.TParams("SchedulerServerID", 4, SqlDbType.Int, Edit.Substring(4))
                        };
                        DataSet dsEdit = db.DIRunSPretDs(Parms, "sp_SchedulerServer");
                        if (dsEdit.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = dsEdit.Tables[0].Rows[0];
                            sm.ServerID = dr["SchedulerServerID"].ToString();
                            sm.ServerName = dr["ServerName"].ToString();
                            sm.MaxThreads = int.Parse(dr["MaxThreads"].ToString());
                            sm.IsActive = bool.Parse(dr["IsActive"].ToString());
                            sm.AgentEnabled = bool.Parse(dr["AgentEnabled"].ToString());
                            sm.MailServer = dr["MailServer"].ToString();
                            sm.MailID = dr["MailID"].ToString();
                            sm.MailPassword = DataEncyption.DecryptString(dr["MailPassword"].ToString());
                            sm.MachineIP4Address = dr["MachineIP4Address"].ToString();
                            sm.MachineIP6Address = dr["MachineIP6Address"].ToString();
                            sm.ServerMessage = string.Empty;
                        }
                    }
                }
                if (sm.AdminAccess)
                {
                    if (Delete != string.Empty)
                    {
                        // Handle Delete
                        if (Delete.Length > 6)
                        {
                            if (Delete.Substring(0, 6) == "delete")
                            {
                                string ServerID = Delete.Substring(6);
                                Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Delete"),
                                        new DataClass.TParams("SchedulerServerID", 4, SqlDbType.Int, ServerID)
                                };
                                db.DIRunSP(Parms, "sp_SchedulerServer");
                                sm.ServerID = string.Empty;
                                sm.ServerName = string.Empty;
                                sm.MaxThreads = 0;
                                sm.IsActive = true;
                                sm.AgentEnabled = true;
                                sm.MailServer = string.Empty;
                                sm.MailID = string.Empty;
                                sm.MailPassword = string.Empty;
                                sm.MachineIP4Address = string.Empty;
                                sm.MachineIP6Address = string.Empty;
                                sm.ServerMessage = string.Empty;
                            }
                        }
                    }
                }

                // Fill Grid
                Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "List") };
                DataSet dsServer = db.DIRunSPretDs(Parms, "sp_SchedulerServer");
                StringBuilder b = new StringBuilder();
                b.Append("<table><tr class='gheader'>");
                int tblWidth = 0;
                if (sm.AdminAccess)
                {
                    b.Append("<th style='width:20px;'>&nbsp;</th>");
                    tblWidth += 27;
                }
                b.Append("<th style='width:500px;'>Server Name</th><th style='width:150px;'>Max Threads</th><th style='width:60px;'>Active</th>");
                tblWidth += 507 + 157 + 67;
                if (sm.AdminAccess)
                {
                    b.Append("<th style='width:20px;'>&nbsp;</th>");
                    tblWidth += 27;
                }
                b.AppendLine("<th style='width:16px;'>&nbsp;</th></tr></table><div style='height:370px; width:" +
                    tblWidth.ToString() + "px; overflow:auto;'><table>");
                bool AltRow = false;
                foreach (DataRow dr in dsServer.Tables[0].Rows)
                {
                    b.Append("<tr");
                    if (AltRow)
                    {
                        b.Append(" class='galtrow'");
                    }
                    b.Append(">");
                    if (sm.AdminAccess)
                    {
                        b.Append("<td style='width:20px;'>" +
                                "<button name=\"edit" + dr["SchedulerServerID"].ToString() + "\" type=\"submit\" " +
                                " class='imagebutton' style=\"background: url(../Images/edit_16x16.gif) no-repeat;\"" +
                                ">&nbsp;</button>" +
                                "</td>");
                    }
                    b.Append("<td style='width:500px;'>" + dr["ServerName"].ToString() + "</td>" +
                        "<td style='width:150px;'>" + dr["MaxThreads"].ToString() + "</td>");
                    b.Append("<td style='width:60px;text-align:center;'><input type = 'checkbox' disabled ");
                    if (bool.Parse(dr["IsActive"].ToString()))
                    {
                        b.Append("checked='checked' ");
                    }
                    b.Append("/></td>");
                    if (sm.AdminAccess)
                    {
                        b.Append("<td style='width:20px;'><button name=\"delete" + dr["SchedulerServerID"].ToString() + "\" type=\"submit\" " +
                            " class='imagebutton' style=\"background: url(../Images/delete_16x16.gif) no-repeat;\"" +
                            ">&nbsp;</button></td>");
                    }
                    b.AppendLine("</tr>");
                    AltRow = !AltRow;
                }
                b.AppendLine("</table></div>");
                sm.ServerList = b.ToString();
            }
            return View(sm);
        }

        public ActionResult Scheduler()
        {
            HomeModel h = BasePage.SetupPageBasics(Request, hm, Session);
            object g = new SchedulerModel();
            Utility.UpdateModel(h, ref g);
            SchedulerModel sm = (SchedulerModel)g;
            sm.ShowQueue = "none";
            sm.ShowAgent = "none";
            sm.ShowAgentButtons = "none";
            sm.ShowExclusion = "none";
            sm.ShowFolder = "none";
            sm.ShowFolderButtons = "none";
            sm.ShowInterval = "none";
            sm.ShowJob = "none";
            sm.ShowJobButtons = "none";
            sm.ShowQueueButtons = "none";
            sm.ShowTask = "none";
            sm.ShowSaveButtons = "none";
            sm.ShowVariable = "none";

            // Set up Queue Fields
            sm.QueueID = string.Empty;
            sm.QueueName = string.Empty;
            sm.QueueIsActive = true;
            sm.QueueIsRunning = true;
            sm.MaxThreads = 0;
            sm.MaxMinutes = string.Empty;
            sm.QueueList = string.Empty;
            sm.QueueMessage = string.Empty;

            // Agent Fields
            sm.AgentID = string.Empty;
            sm.ServerID = string.Empty;
            sm.AgentIsActive = true;
            sm.AgentList = string.Empty;
            sm.AgentMessage = string.Empty;

            // Exclusion Fields
            sm.ExclusionID = string.Empty;
            sm.ExclusionIsActive = true;
            sm.Sunday = true;
            sm.Monday = true;
            sm.Tuesday = true;
            sm.Wednesday = true;
            sm.Thursday = true;
            sm.Friday = true;
            sm.Saturday = true;
            sm.StartTime = string.Empty;
            sm.EndTime = string.Empty;
            sm.SpecificDate = string.Empty;
            sm.ExclusionList = string.Empty;
            sm.ExclusionMessage = string.Empty;
            sm.IsGlobal = false;

            // Folder Fields
            sm.FolderId = string.Empty;
            sm.FolderIsActive = true;
            sm.FolderList = string.Empty;
            sm.FolderMessage = string.Empty;
            sm.FolderName = string.Empty;
            sm.FolderSortOrder = 0;
            sm.ParentFolderId = string.Empty;

            // Variable Fields
            sm.VariableDescription = string.Empty;
            sm.VariableId = string.Empty;
            sm.VariableIsActive = true;
            sm.VariableList = string.Empty;
            sm.VariableMessage = string.Empty;
            sm.VariableName = string.Empty;
            sm.VariableSortOrder = 0;
            sm.VariableValue = string.Empty;

            // Job Fields
            sm.JobAgentId = string.Empty;
            sm.JobId = string.Empty;
            sm.JobIntervalId = string.Empty;
            sm.JobIsActive = true;
            sm.JobList = string.Empty;
            sm.JobMessage = string.Empty;
            sm.JobName = string.Empty;
            sm.JobOperatorId = string.Empty;
            sm.JobSortOrder = 0;

            // Interval Fields
            sm.IntervalId = string.Empty;
            sm.IntervalName = string.Empty;
            sm.Interval = 0;
            sm.IntervalIsActive = true;
            sm.Occurrences = string.Empty;
            sm.IntervalType = string.Empty;
            sm.IntervalStartTime = string.Empty;
            sm.IntervalEndTime = string.Empty;
            sm.IntervalExclusionStart = string.Empty;
            sm.IntervalExclusionEnd = string.Empty;
            sm.IntervalRepeatMinutes = string.Empty;
            sm.IntervalDetails = string.Empty;
            sm.IntervalList = string.Empty;
            sm.IntervalMessage = string.Empty;

            // Task Fields
            sm.TaskId = string.Empty;
            sm.TaskName = string.Empty;
            sm.TaskIsActive = true;
            sm.TaskSortOrder = 0;
            sm.TaskTypeId = string.Empty;
            sm.TaskInformation = string.Empty;
            sm.LastRunTime = string.Empty;
            sm.NextRunTime = string.Empty;
            sm.MaxInstances = string.Empty;
            sm.TaskMessage = string.Empty;
            sm.TaskList = string.Empty;

            // Task Type Specific data
            sm.ShowExecuteProgram = "none";
            sm.ExecutePath = string.Empty;
            sm.ExecuteCommand = string.Empty;
            sm.ExecuteParameters = string.Empty;

            sm.ShowSQLTask = "none";
            sm.SQLConnection = string.Empty;
            sm.SQLCommand = string.Empty;

            sm.ShowSPTask = "none";
            sm.SPConnection = string.Empty;
            sm.SPName = string.Empty;
            sm.SPMessage = string.Empty;
            sm.SPParameter = string.Empty;
            sm.SPDataType = string.Empty;
            sm.SPValue = string.Empty;
            sm.SPParameters = string.Empty;
            sm.SPList = string.Empty;

            sm.ShowMailTask = "none";
            sm.MailFrom = string.Empty;
            sm.MailSubject = string.Empty;
            sm.MailTo = string.Empty;
            sm.MailBody = string.Empty;

            sm.ShowFileCopyTask = "none";
            sm.FileCopySource = string.Empty;
            sm.FileCopyDestination = string.Empty;
            sm.FileCopyFiles = string.Empty;

            sm.ShowScriptTask = "none";
            sm.ScriptCode = string.Empty;

            sm.ShowDTS = "none";
            sm.DTSPath = string.Empty;

            sm.ShowDLL = "none";
            sm.DLLPath = string.Empty;
            sm.DLLClass = string.Empty;
            sm.DLLMethod = string.Empty;

            sm.Operators = new List<SelectListItem>();
            sm.TaskTypes = new List<SelectListItem>();
            sm.Intervals = new List<SelectListItem>();
            sm.ParentFolders = new List<SelectListItem>();
            sm.Servers = new List<SelectListItem>();
            sm.Agents = new List<SelectListItem>();
            sm.IntervalTypes = new List<SelectListItem>();
            sm.DataTypes = new List<SelectListItem>();

            if (sm.AdminAccess || sm.ReportAccess || sm.OperatorAccess || sm.ViewAccess)
            {
                sm.ShowQueue = "inline";

                // Grab Incoming Data
                // Show Fields
                if (Request.Params["ShowAgent"] != null)
                {
                    sm.ShowAgent = Request.Params["ShowAgent"].ToString();
                }
                if (Request.Params["ShowAgentButtons"] != null)
                {
                    sm.ShowAgentButtons = Request.Params["ShowAgentButtons"].ToString();
                }
                if (Request.Params["ShowExclusion"] != null)
                {
                    sm.ShowExclusion = Request.Params["ShowExclusion"].ToString();
                }
                if (Request.Params["ShowFolder"] != null)
                {
                    sm.ShowFolder = Request.Params["ShowFolder"].ToString();
                }
                if (Request.Params["ShowFolderButtons"] != null)
                {
                    sm.ShowFolderButtons = Request.Params["ShowFolderButtons"].ToString();
                }
                if (Request.Params["ShowInterval"] != null)
                {
                    sm.ShowInterval = Request.Params["ShowInterval"].ToString();
                }
                if (Request.Params["ShowJob"] != null)
                {
                    sm.ShowJob = Request.Params["ShowJob"].ToString();
                }
                if (Request.Params["ShowJobButtons"] != null)
                {
                    sm.ShowJobButtons = Request.Params["ShowJobButtons"].ToString();
                }
                if (Request.Params["ShowQueueButtons"] != null)
                {
                    sm.ShowQueueButtons = Request.Params["ShowQueueButtons"].ToString();
                }
                if (Request.Params["ShowTask"] != null)
                {
                    sm.ShowTask = Request.Params["ShowTask"].ToString();
                }
                if (Request.Params["ShowVariable"] != null)
                {
                    sm.ShowVariable = Request.Params["ShowVariable"].ToString();
                }

                // Queue Variables
                if (Request.Params["QueueID"] != null)
                {
                    sm.QueueID = Request.Params["QueueID"].ToString();
                }
                if (Request.Params["QueueName"] != null)
                {
                    sm.QueueName = Request.Params["QueueName"].ToString();
                }
                if (Request.Params["QueueIsActive"] != null)
                {
                    sm.QueueIsActive = Utility.GetBoolean(Request.Params["QueueIsActive"].ToString());
                }
                if (Request.Params["QueueIsRunning"] != null)
                {
                    sm.QueueIsRunning = Utility.GetBoolean(Request.Params["QueueIsRunning"].ToString());
                }
                if (Request.Params["MaxThreads"] != null)
                {
                    int MaxThreads = 0;
                    int.TryParse(Request.Params["MaxThreads"].ToString(), out MaxThreads);
                    sm.MaxThreads = MaxThreads;
                }
                if (Request.Params["MaxMinutes"] != null)
                {
                    int MaxMinutes = 0;
                    int.TryParse(Request.Params["MaxMinutes"].ToString(), out MaxMinutes);
                    sm.MaxMinutes = MaxMinutes.ToString();
                }

                // Agent Variables
                if (Request.Params["AgentID"] != null)
                {
                    sm.AgentID = Request.Params["AgentID"].ToString();
                }
                if (Request.Params["ServerID"] != null)
                {
                    sm.ServerID = Request.Params["ServerID"].ToString();
                }
                if (Request.Params["AgentIsActive"] != null)
                {
                    sm.AgentIsActive = Utility.GetBoolean(Request.Params["AgentIsActive"].ToString());
                }

                // Exclusion Variables
                if (Request.Params["ExclusionID"] != null)
                {
                    sm.ExclusionID = Request.Params["ExclusionID"].ToString();
                }
                if (Request.Params["ExclusionIsActive"] != null)
                {
                    sm.ExclusionIsActive = Utility.GetBoolean(Request.Params["ExclusionIsActive"].ToString());
                }
                if (Request.Params["Sunday"] != null)
                {
                    sm.Sunday = Utility.GetBoolean(Request.Params["Sunday"].ToLower());
                }
                if (Request.Params["Monday"] != null)
                {
                    sm.Monday = Utility.GetBoolean(Request.Params["Monday"].ToLower());
                }
                if (Request.Params["Tuesday"] != null)
                {
                    sm.Tuesday = Utility.GetBoolean(Request.Params["Tuesday"].ToLower());
                }
                if (Request.Params["Wednesday"] != null)
                {
                    sm.Wednesday = Utility.GetBoolean(Request.Params["Wednesday"].ToLower());
                }
                if (Request.Params["Thursday"] != null)
                {
                    sm.Thursday = Utility.GetBoolean(Request.Params["Thursday"].ToLower());
                }
                if (Request.Params["Friday"] != null)
                {
                    sm.Friday = Utility.GetBoolean(Request.Params["Friday"].ToLower());
                }
                if (Request.Params["Saturday"] != null)
                {
                    sm.Saturday = Utility.GetBoolean(Request.Params["Saturday"].ToLower());
                }
                if (Request.Params["StartTime"] != null)
                {
                    sm.StartTime = Request.Params["StartTime"].ToString();
                }
                if (Request.Params["EndTime"] != null)
                {
                    sm.EndTime = Request.Params["EndTime"].ToString();
                }
                if (Request.Params["SpecificDate"] != null)
                {
                    sm.SpecificDate = Request.Params["SpecificDate"].ToString();
                }
                if (Request.Params["IsGlobal"] != null)
                {
                    sm.IsGlobal = Utility.GetBoolean(Request.Params["IsGlobal"].ToString());
                }

                // Folder Variables
                if (Request.Params["FolderId"] != null)
                {
                    sm.FolderId = Request.Params["FolderId"].ToString();
                }
                if (Request.Params["FolderIsActive"] != null)
                {
                    sm.FolderIsActive = Utility.GetBoolean(Request.Params["FolderIsActive"].ToString());
                }
                if (Request.Params["FolderName"] != null)
                {
                    sm.FolderName = Request.Params["FolderName"].ToString();
                }
                if (Request.Params["FolderSortOrder"] != null)
                {
                    int FolderSortOrder = 0;
                    int.TryParse(Request.Params["FolderSortOrder"].ToString(), out FolderSortOrder);
                    sm.FolderSortOrder = FolderSortOrder;
                }
                if (Request.Params["ParentFolderId"] != null)
                {
                    sm.ParentFolderId = Request.Params["ParentFolderId"].ToString();
                }

                // Variable Variables
                if (Request.Params["VariableDescription"] != null)
                {
                    sm.VariableDescription = Request.Params["VariableDescription"].ToString();
                }
                if (Request.Params["VariableId"] != null)
                {
                    sm.VariableId = Request.Params["VariableId"].ToString();
                }
                if (Request.Params["VariableIsActive"] != null)
                {
                    sm.VariableIsActive = Utility.GetBoolean(Request.Params["VariableIsActive"].ToString());
                }
                if (Request.Params["VariableName"] != null)
                {
                    sm.VariableName = Request.Params["VariableName"].ToString();
                }
                if (Request.Params["VariableSortOrder"] != null)
                {
                    int SortOrder = 0;
                    int.TryParse(Request.Params["VariableSortOrder"].ToString(), out SortOrder);
                    sm.VariableSortOrder = SortOrder;
                }
                if (Request.Params["VariableValue"] != null)
                {
                    sm.VariableValue = Request.Params["VariableValue"].ToString();
                }

                // Job Variables
                if (Request.Params["JobAgentId"] != null)
                {
                    sm.JobAgentId = Request.Params["JobAgentId"].ToString();
                }
                if (Request.Params["JobId"] != null)
                {
                    sm.JobId = Request.Params["JobId"].ToString();
                }
                if (Request.Params["JobIntervalId"] != null)
                {
                    sm.JobIntervalId = Request.Params["JobIntervalId"].ToString();
                }
                if (Request.Params["JobIsActive"] != null)
                {
                    sm.JobIsActive = Utility.GetBoolean(Request.Params["JobIsActive"].ToString());
                }
                if (Request.Params["JobName"] != null)
                {
                    sm.JobName = Request.Params["JobName"].ToString();
                }
                if (Request.Params["JobOperatorId"] != null)
                {
                    sm.JobOperatorId = Request.Params["JobOperatorId"].ToString();
                }
                if (Request.Params["JobSortOrder"] != null)
                {
                    int JobSortOrder = 0;
                    int.TryParse(Request.Params["JobSortOrder"].ToString(), out JobSortOrder);
                    sm.JobSortOrder = JobSortOrder;
                }

                // Interval Variables
                if (Request.Params["IntervalId"] != null)
                {
                    sm.IntervalId = Request.Params["IntervalId"].ToString();
                }
                if (Request.Params["IntervalName"] != null)
                {
                    sm.IntervalName = Request.Params["IntervalName"].ToString();
                }
                if (Request.Params["Interval"] != null)
                {
                    int Interval = -1;
                    int.TryParse(Request.Params["Interval"].ToString(), out Interval);
                    sm.Interval = Interval;
                }
                if (Request.Params["IntervalIsActive"] != null)
                {
                    sm.IntervalIsActive = Utility.GetBoolean(Request.Params["IntervalIsActive"].ToString());
                }
                if (Request.Params["Occurrences"] != null)
                {
                    sm.Occurrences = Request.Params["Occurrences"].ToString();
                }
                if (Request.Params["IntervalType"] != null)
                {
                    sm.IntervalType = Request.Params["IntervalType"].ToString();
                }
                if (Request.Params["IntervalStartTime"] != null)
                {
                    sm.IntervalStartTime = Request.Params["IntervalStartTime"].ToString();
                }
                if (Request.Params["IntervalEndTime"] != null)
                {
                    sm.IntervalEndTime = Request.Params["IntervalEndTime"].ToString();
                }
                if (Request.Params["IntervalExclusionStart"] != null)
                {
                    sm.IntervalExclusionStart = Request.Params["IntervalExclusionStart"].ToString();
                }
                if (Request.Params["IntervalExclusionEnd"] != null)
                {
                    sm.IntervalExclusionEnd = Request.Params["IntervalExclusionEnd"].ToString();
                }
                if (Request.Params["IntervalRepeatMinutes"] != null)
                {
                    sm.IntervalRepeatMinutes = Request.Params["IntervalRepeatMinutes"].ToString();
                }
                if (Request.Params["IntervalDetails"] != null)
                {
                    sm.IntervalDetails = Request.Params["IntervalDetails"].ToString();
                }

                // Task Variables
                if (Request.Params["TaskId"] != null)
                {
                    sm.TaskId = Request.Params["TaskId"].ToString();
                }
                if (Request.Params["TaskName"] != null)
                {
                    sm.TaskName = Request.Params["TaskName"].ToString();
                }
                if (Request.Params["TaskIsActive"] != null)
                {
                    sm.TaskIsActive = Utility.GetBoolean(Request.Params["TaskIsActive"].ToString());
                }
                if (Request.Params["TaskSortOrder"] != null)
                {
                    int SortOrder = 0;
                    int.TryParse(Request.Params["TaskSortOrder"].ToString(), out SortOrder);
                    sm.TaskSortOrder = SortOrder;
                }
                if (Request.Params["TaskTypeId"] != null)
                {
                    sm.TaskTypeId = Request.Params["TaskTypeId"].ToString();
                }
                if (Request.Params["TaskInformation"] != null)
                {
                    sm.TaskInformation = Request.Params["TaskInformation"].ToString();
                }
                if (Request.Params["LastRunTime"] != null)
                {
                    sm.LastRunTime = Request.Params["LastRunTime"].ToString();
                }
                if (Request.Params["NextRunTime"] != null)
                {
                    sm.NextRunTime = Request.Params["NextRunTime"].ToString();
                }
                if (Request.Params["MaxInstances"] != null)
                {
                    sm.MaxInstances = Request.Params["MaxInstances"].ToString();
                    int MaxInstances = -1;
                    int.TryParse(sm.MaxInstances, out MaxInstances);
                    if (MaxInstances == -1 && sm.MaxInstances != string.Empty)
                    {
                        sm.TaskMessage += "Max Instances is invalid. ";
                    }
                }

                // Task Type Specific data
                if (Request.Params["ShowExecuteProgram"] != null)
                {
                    sm.ShowExecuteProgram = Request.Params["ShowExecuteProgram"].ToString();
                }
                if (Request.Params["ExecutePath"] != null)
                {
                    sm.ExecutePath = Request.Params["ExecutePath"].ToString();
                }
                if (Request.Params["ExecuteCommand"] != null)
                {
                    sm.ExecuteCommand = Request.Params["ExecuteCommand"].ToString();
                }
                if (Request.Params["ExecuteParameters"] != null)
                {
                    sm.ExecuteParameters = Request.Params["ExecuteParameters"].ToString();
                }

                if (Request.Params["ShowSQLTask"] != null)
                {
                    sm.ShowSQLTask = Request.Params["ShowSQLTask"].ToString();
                }
                if (Request.Params["SQLConnection"] != null)
                {
                    sm.SQLConnection = Request.Params["SQLConnection"].ToString();
                }
                if (Request.Params["SQLCommand"] != null)
                {
                    sm.SQLCommand = Request.Params["SQLCommand"].ToString();
                }

                if (Request.Params["ShowSPTask"] != null)
                {
                    sm.ShowSPTask = Request.Params["ShowSPTask"].ToString();
                }
                if (Request.Params["SPConnection"] != null)
                {
                    sm.SPConnection = Request.Params["SPConnection"].ToString();
                }
                if (Request.Params["SPName"] != null)
                {
                    sm.SPName = Request.Params["SPName"].ToString();
                }
                if (Request.Params["SPMessage"] != null)
                {
                    sm.SPMessage = Request.Params["SPMessage"].ToString();
                }
                if (Request.Params["SPParameter"] != null)
                {
                    sm.SPParameter = Request.Params["SPParameter"].ToString();
                }
                if (Request.Params["SPDataType"] != null)
                {
                    sm.SPDataType = Request.Params["SPDataType"].ToString();
                }
                if (Request.Params["SPValue"] != null)
                {
                    sm.SPValue = Request.Params["SPValue"].ToString();
                }
                if (Request.Params["SPParameters"] != null)
                {
                    sm.SPParameters = Request.Params["SPParameters"].ToString();
                }
                if (Request.Params["SPList"] != null)
                {
                    sm.SPList = Request.Params["SPList"].ToString();
                }

                if (Request.Params["ShowMailTask"] != null)
                {
                    sm.ShowMailTask = Request.Params["ShowMailTask"].ToString();
                }
                if (Request.Params["MailFrom"] != null)
                {
                    sm.MailFrom = Request.Params["MailFrom"].ToString();
                }
                if (Request.Params["MailSubject"] != null)
                {
                    sm.MailSubject = Request.Params["MailSubject"].ToString();
                }
                if (Request.Params["MailTo"] != null)
                {
                    sm.MailTo = Request.Params["MailTo"].ToString();
                }
                if (Request.Params["MailBody"] != null)
                {
                    sm.MailBody = Request.Params["MailBody"].ToString();
                }

                if (Request.Params["ShowFileCopyTask"] != null)
                {
                    sm.ShowFileCopyTask = Request.Params["ShowFileCopyTask"].ToString();
                }
                if (Request.Params["FileCopySource"] != null)
                {
                    sm.FileCopySource = Request.Params["FileCopySource"].ToString();
                }
                if (Request.Params["FileCopyDestination"] != null)
                {
                    sm.FileCopyDestination = Request.Params["FileCopyDestination"].ToString();
                }
                if (Request.Params["FileCopyFiles"] != null)
                {
                    sm.FileCopyFiles = Request.Params["FileCopyFiles"].ToString();
                }

                if (Request.Params["ShowScriptTask"] != null)
                {
                    sm.ShowScriptTask = Request.Params["ShowScriptTask"].ToString();
                }
                if (Request.Params["ScriptCode"] != null)
                {
                    sm.ScriptCode = Request.Params["ScriptCode"].ToString();
                }

                if (Request.Params["ShowDTS"] != null)
                {
                    sm.ShowDTS = Request.Params["ShowDTS"].ToString();
                }
                if (Request.Params["DTSPath"] != null)
                {
                    sm.DTSPath = Request.Params["DTSPath"].ToString();
                }

                if (Request.Params["ShowDLL"] != null)
                {
                    sm.ShowDLL = Request.Params["ShowDLL"].ToString();
                }
                if (Request.Params["DLLPath"] != null)
                {
                    sm.DLLPath = Request.Params["DLLPath"].ToString();
                }
                if (Request.Params["DLLClass"] != null)
                {
                    sm.DLLClass = Request.Params["DLLClass"].ToString();
                }
                if (Request.Params["DLLMethod"] != null)
                {
                    sm.DLLMethod = Request.Params["DLLMethod"].ToString();
                }

                // Edit & Delete Parameters
                string edtq = Utility.GetParam(Request.Params.ToString(), "edtq");
                string delq = Utility.GetParam(Request.Params.ToString(), "delq");

                string edta = Utility.GetParam(Request.Params.ToString(), "edta");
                string dela = Utility.GetParam(Request.Params.ToString(), "dela");

                string edte = Utility.GetParam(Request.Params.ToString(), "edte");
                string dele = Utility.GetParam(Request.Params.ToString(), "dele");

                string edtf = Utility.GetParam(Request.Params.ToString(), "edtf");
                string delf = Utility.GetParam(Request.Params.ToString(), "delf");

                string edtv = Utility.GetParam(Request.Params.ToString(), "edtv");
                string delv = Utility.GetParam(Request.Params.ToString(), "delv");

                string edtj = Utility.GetParam(Request.Params.ToString(), "edtj");
                string delj = Utility.GetParam(Request.Params.ToString(), "delj");

                string edti = Utility.GetParam(Request.Params.ToString(), "edti");
                string deli = Utility.GetParam(Request.Params.ToString(), "deli");

                string edtt = Utility.GetParam(Request.Params.ToString(), "edtt");
                string delt = Utility.GetParam(Request.Params.ToString(), "delt");

                string edts = Utility.GetParam(Request.Params.ToString(), "edts");
                string dels = Utility.GetParam(Request.Params.ToString(), "dels");

                // Data Update/Delete Functions
                if (sm.AdminAccess)
                {
                    sm.ShowSaveButtons = "inline";

                    // Saves
                    if (Request.Params["btnQueueSave"] != null)
                    {
                        if (sm.QueueName == string.Empty)
                        {
                            sm.QueueMessage += "Please Enter the Name of the Queue. ";
                        }
                        if (sm.MaxThreads <= 0)
                        {
                            sm.QueueMessage += "Max Threads must be greater than zero. ";
                        }
                        if (sm.QueueMessage == string.Empty)
                        {
                            string Command = "Add";
                            if (sm.QueueID == string.Empty)
                            {
                                sm.QueueID = null;
                            }
                            else
                            {
                                Command = "Update";
                            }
                            if (sm.MaxMinutes == string.Empty)
                            {
                                sm.MaxMinutes = null;
                            }
                            try
                            {
                                Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, Command),
                                        new DataClass.TParams("SchedulerQueueID", 4, SqlDbType.Int, sm.QueueID),
                                        new DataClass.TParams("QueueName", 100, SqlDbType.VarChar, sm.QueueName),
                                        new DataClass.TParams("IsActive", 1, SqlDbType.Bit, sm.QueueIsActive),
                                        new DataClass.TParams("IsRunning", 1, SqlDbType.Bit, sm.QueueIsRunning),
                                        new DataClass.TParams("MaxThreads", 4, SqlDbType.Int, sm.MaxThreads),
                                        new DataClass.TParams("MaxMinutes", 4, SqlDbType.Int, sm.MaxMinutes)
                                };
                                db.DIRunSP(Parms, "sp_SchedulerQueue");
                                sm.QueueID = string.Empty;
                                sm.QueueName = string.Empty;
                                sm.QueueIsActive = true;
                                sm.QueueIsRunning = false;
                                sm.MaxThreads = 1;
                                sm.MaxMinutes = string.Empty;
                            }
                            catch (Exception ex)
                            {
                                sm.QueueMessage = ex.Message;
                            }
                        }
                    }
                    if (Request.Params["btnAgentSave"] != null)
                    {
                        string Command = "Add";
                        if (sm.AgentID == string.Empty)
                        {
                            sm.AgentID = null;
                        }
                        else
                        {
                            Command = "Update";
                        }
                        try
                        {
                            Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, Command),
                                    new DataClass.TParams("SchedulerAgentID", 4, SqlDbType.Int, sm.AgentID),
                                    new DataClass.TParams("SchedulerServerID", 4, SqlDbType.Int, sm.ServerID),
                                    new DataClass.TParams("SchedulerQueueID", 4, SqlDbType.Int, sm.QueueID),
                                    new DataClass.TParams("IsActive", 1, SqlDbType.Bit, sm.AgentIsActive)
                            };
                            db.DIRunSP(Parms, "sp_SchedulerAgent");
                            sm.AgentID = string.Empty;
                            sm.ServerID = string.Empty;
                            sm.AgentIsActive = true;
                            sm.ShowAgentButtons = "none";
                        }
                        catch (Exception ex)
                        {
                            sm.AgentMessage = ex.Message;
                        }
                    }
                    if (Request.Params["btnExclusionSave"] != null)
                    {
                        string Command = "Add";
                        if (sm.ExclusionID == string.Empty)
                        {
                            sm.ExclusionID = null;
                        }
                        else
                        {
                            Command = "Update";
                        }
                        if (sm.StartTime == string.Empty)
                        {
                            sm.StartTime = null;
                        }
                        if (sm.EndTime == string.Empty)
                        {
                            sm.EndTime = null;
                        }
                        if (sm.SpecificDate == string.Empty)
                        {
                            sm.SpecificDate = null;
                        }
                        int? SchedulerAgentID = null;
                        if (!sm.IsGlobal)
                        {
                            SchedulerAgentID = int.Parse(sm.AgentID);
                        }
                        try
                        {
                            Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, Command),
                                    new DataClass.TParams("SchedulerExclusionID", 4, SqlDbType.Int, sm.ExclusionID),
                                    new DataClass.TParams("SchedulerAgentID", 4, SqlDbType.Int, SchedulerAgentID),
                                    new DataClass.TParams("IsActive", 1, SqlDbType.Bit, sm.ExclusionIsActive),
                                    new DataClass.TParams("Sunday", 1, SqlDbType.Bit, sm.Sunday),
                                    new DataClass.TParams("Monday", 1, SqlDbType.Bit, sm.Monday),
                                    new DataClass.TParams("Tuesday", 1, SqlDbType.Bit, sm.Tuesday),
                                    new DataClass.TParams("Wednesday", 1, SqlDbType.Bit, sm.Wednesday),
                                    new DataClass.TParams("Thursday", 1, SqlDbType.Bit, sm.Thursday),
                                    new DataClass.TParams("Friday", 1, SqlDbType.Bit, sm.Friday),
                                    new DataClass.TParams("Saturday", 1, SqlDbType.Bit, sm.Saturday),
                                    new DataClass.TParams("StartTime", 8, SqlDbType.DateTime, sm.StartTime),
                                    new DataClass.TParams("EndTime", 8, SqlDbType.DateTime, sm.EndTime),
                                    new DataClass.TParams("SpecificDate", 8, SqlDbType.Date, sm.SpecificDate)
                            };
                            db.DIRunSP(Parms, "sp_SchedulerExclusion");
                            sm.ExclusionID = string.Empty;
                            sm.ExclusionIsActive = true;
                            sm.Sunday = true;
                            sm.Monday = true;
                            sm.Tuesday = true;
                            sm.Wednesday = true;
                            sm.Thursday = true;
                            sm.Friday = true;
                            sm.Saturday = true;
                            sm.IsGlobal = false;
                            sm.StartTime = string.Empty;
                            sm.EndTime = string.Empty;
                            sm.SpecificDate = string.Empty;
                        }
                        catch (Exception ex)
                        {
                            sm.ExclusionMessage = ex.Message;
                        }
                    }
                    if (Request.Params["btnFolderSave"] != null)
                    {
                        if (sm.FolderName == string.Empty)
                        {
                            sm.FolderMessage += "Please enter the name of the folder. ";
                        }
                        if (sm.FolderMessage == string.Empty)
                        {
                            string Command = "Add";
                            if (sm.FolderId == string.Empty)
                            {
                                sm.FolderId = null;
                            }
                            else
                            {
                                Command = "Update";
                            }
                            if (sm.ParentFolderId == string.Empty)
                            {
                                sm.ParentFolderId = null;
                            }
                            try
                            {
                                Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, Command),
                                        new DataClass.TParams("SchedulerFolderID", 4, SqlDbType.Int, sm.FolderId),
                                        new DataClass.TParams("SchedulerFolderName", 100, SqlDbType.VarChar, sm.FolderName),
                                        new DataClass.TParams("SchedulerQueueID", 4, SqlDbType.Int, sm.QueueID),
                                        new DataClass.TParams("ParentSchedulerFolderID", 4, SqlDbType.Int, sm.ParentFolderId),
                                        new DataClass.TParams("IsActive", 1, SqlDbType.Bit, sm.FolderIsActive),
                                        new DataClass.TParams("SortOrder", 4, SqlDbType.Int, sm.FolderSortOrder),
                                        new DataClass.TParams("CreateDate", 8, SqlDbType.DateTime, DateTime.Now),
                                        new DataClass.TParams("CreatedBy", 4, SqlDbType.Int, sm.UserId)
                                };
                                db.DIRunSP(Parms, "sp_SchedulerFolder");
                                sm.FolderId = string.Empty;
                                sm.FolderName = string.Empty;
                                sm.ParentFolderId = string.Empty;
                                sm.FolderIsActive = true;
                                sm.FolderSortOrder = 0;
                                sm.ShowFolderButtons = "none";
                            }
                            catch (Exception ex)
                            {
                                sm.FolderMessage = ex.Message;
                            }
                        }
                    }
                    if (Request.Params["btnVariableSave"] != null)
                    {
                        if (sm.VariableName == string.Empty)
                        {
                            sm.VariableMessage += "Please enter the name of the variable. ";
                        }
                        if (sm.VariableSortOrder == 0)
                        {
                            sm.VariableMessage += "Please enter the sort order. ";
                        }
                        if (sm.VariableMessage == string.Empty)
                        {
                            string Command = "Add";
                            if (sm.VariableId == string.Empty)
                            {
                                sm.VariableId = null;
                            }
                            else
                            {
                                Command = "Update";
                            }
                            try
                            {
                                Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, Command),
                                        new DataClass.TParams("SchedulerVariableID", 4, SqlDbType.Int, sm.VariableId),
                                        new DataClass.TParams("SchedulerFolderID", 4, SqlDbType.Int, sm.FolderId),
                                        new DataClass.TParams("VariableName", 100, SqlDbType.VarChar, sm.VariableName),
                                        new DataClass.TParams("VariableValue", 1000, SqlDbType.VarChar, sm.VariableValue),
                                        new DataClass.TParams("VariableDescription", 1000, SqlDbType.VarChar, sm.VariableDescription),
                                        new DataClass.TParams("IsActive", 1, SqlDbType.Bit, sm.VariableIsActive),
                                        new DataClass.TParams("SortOrder", 4, SqlDbType.Int, sm.VariableSortOrder)
                                };
                                db.DIRunSP(Parms, "sp_SchedulerVariable");
                                sm.VariableId = string.Empty;
                                sm.VariableName = string.Empty;
                                sm.VariableValue = string.Empty;
                                sm.VariableDescription = string.Empty;
                                sm.VariableIsActive = true;
                                sm.VariableSortOrder = 0;
                            }
                            catch (Exception ex)
                            {
                                sm.VariableMessage = ex.Message;
                            }
                        }
                    }
                    if (Request.Params["btnJobSave"] != null)
                    {
                        if (sm.JobName == string.Empty)
                        {
                            sm.JobMessage += "Please enter the name of the job. ";
                        }
                        if (sm.JobMessage == string.Empty)
                        {
                            string Command = "Add";
                            if (sm.JobId == string.Empty)
                            {
                                sm.JobId = null;
                            }
                            else
                            {
                                Command = "Update";
                            }
                            if (sm.JobIntervalId == string.Empty)
                            {
                                sm.JobIntervalId = null;
                            }
                            if (sm.JobAgentId == string.Empty)
                            {
                                sm.JobAgentId = null;
                            }
                            if (sm.JobOperatorId == string.Empty)
                            {
                                sm.JobOperatorId = null;
                            }
                            if (sm.MaxInstances == string.Empty)
                            {
                                sm.MaxInstances = null;
                            }
                            try
                            {
                                Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, Command),
                                        new DataClass.TParams("SchedulerJobID", 4, SqlDbType.Int, sm.JobId),
                                        new DataClass.TParams("SchedulerFolderID", 4, SqlDbType.Int, sm.FolderId),
                                        new DataClass.TParams("JobName", 100, SqlDbType.VarChar, sm.JobName),
                                        new DataClass.TParams("IsActive", 1, SqlDbType.Bit, sm.JobIsActive),
                                        new DataClass.TParams("CreateDate", 8, SqlDbType.DateTime, DateTime.Now),
                                        new DataClass.TParams("CreatedBy", 4, SqlDbType.Int, sm.UserId),
                                        new DataClass.TParams("SortOrder", 4, SqlDbType.Int, sm.JobSortOrder),
                                        new DataClass.TParams("SchedulerIntervalID", 4, SqlDbType.Int, sm.JobIntervalId),
                                        new DataClass.TParams("SchedulerAgentID", 4, SqlDbType.Int, sm.JobAgentId),
                                        new DataClass.TParams("SchedulerOperatorID", 4, SqlDbType.Int, sm.JobOperatorId),
                                        new DataClass.TParams("MaxInstances", 4, SqlDbType.Int, sm.MaxInstances)
                                };
                                db.DIRunSP(Parms, "sp_SchedulerJob");
                                sm.JobId = string.Empty;
                                sm.JobName = string.Empty;
                                sm.JobIsActive = true;
                                sm.JobSortOrder = 0;
                                sm.JobIntervalId = string.Empty;
                                sm.JobAgentId = string.Empty;
                                sm.JobOperatorId = string.Empty;
                                sm.ShowJobButtons = "none";
                                sm.MaxInstances = string.Empty;
                            }
                            catch (Exception ex)
                            {
                                sm.JobMessage = ex.Message;
                            }
                        }
                    }
                    if (Request.Params["btnIntervalSave"] != null)
                    {
                        if (sm.IntervalName == string.Empty)
                        {
                            sm.IntervalMessage += "Please enter the name of the interval. ";
                        }
                        if (sm.Interval == 0)
                        {
                            sm.IntervalMessage += "Please enter the interval. ";
                        }
                        if (sm.IntervalMessage == string.Empty)
                        {
                            string Command = "Add";
                            if (sm.IntervalId == string.Empty)
                            {
                                sm.IntervalId = null;
                            }
                            else
                            {
                                Command = "Update";
                            }
                            try
                            {
                                if (sm.IntervalStartTime == string.Empty)
                                {
                                    sm.IntervalStartTime = null;
                                }
                                if (sm.IntervalEndTime == string.Empty)
                                {
                                    sm.IntervalEndTime = null;
                                }
                                if (sm.IntervalExclusionStart == string.Empty)
                                {
                                    sm.IntervalExclusionStart = null;
                                }
                                if (sm.IntervalExclusionEnd == string.Empty)
                                {
                                    sm.IntervalExclusionEnd = null;
                                }
                                if (sm.IntervalRepeatMinutes == string.Empty)
                                {
                                    sm.IntervalRepeatMinutes = null;
                                }
                                Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, Command),
                                        new DataClass.TParams("SchedulerIntervalID", 4, SqlDbType.Int, sm.IntervalId),
                                        new DataClass.TParams("IntervalName", 50, SqlDbType.VarChar, sm.IntervalName),
                                        new DataClass.TParams("Interval", 4, SqlDbType.Int, sm.Interval),
                                        new DataClass.TParams("IsActive", 1, SqlDbType.Bit, sm.IntervalIsActive),
                                        new DataClass.TParams("Occurrences", 12, SqlDbType.VarChar, sm.Occurrences),
                                        new DataClass.TParams("IntervalType", 10, SqlDbType.VarChar, sm.IntervalType),
                                        new DataClass.TParams("StartTime", 5, SqlDbType.DateTime, sm.IntervalStartTime),
                                        new DataClass.TParams("EndTime", 5, SqlDbType.DateTime, sm.IntervalEndTime),
                                        new DataClass.TParams("ExclusionStart", 5, SqlDbType.Time, sm.IntervalExclusionStart),
                                        new DataClass.TParams("ExclusionEnd", 5, SqlDbType.Time, sm.IntervalExclusionEnd),
                                        new DataClass.TParams("RepeatMinutes", 4, SqlDbType.Int, sm.IntervalRepeatMinutes),
                                        new DataClass.TParams("SchedulerIntervalDetails", sm.IntervalDetails.Length, SqlDbType.VarChar, sm.IntervalDetails)
                                };
                                db.DIRunSP(Parms, "sp_SchedulerInterval");
                                sm.IntervalId = string.Empty;
                                sm.IntervalName = string.Empty;
                                sm.Interval = 0;
                                sm.IntervalIsActive = true;
                                sm.Occurrences = string.Empty;
                                sm.IntervalType = string.Empty;
                                sm.IntervalStartTime = string.Empty;
                                sm.IntervalEndTime = string.Empty;
                                sm.IntervalExclusionStart = string.Empty;
                                sm.IntervalExclusionEnd = string.Empty;
                                sm.IntervalRepeatMinutes = string.Empty;
                                sm.IntervalDetails = string.Empty;
                            }
                            catch (Exception ex)
                            {
                                sm.IntervalMessage = ex.Message;
                            }
                        }
                    }
                    if (Request.Params["btnTaskSave"] != null)
                    {
                        if (sm.TaskName == string.Empty)
                        {
                            sm.TaskMessage += "Please enter the name of the Task. ";
                        }
                        if (sm.TaskSortOrder == 0)
                        {
                            sm.TaskMessage += "Please enter the sort order. ";
                        }
                        if (sm.TaskMessage == string.Empty)
                        {
                            string Command = "Add";
                            if (sm.TaskId == string.Empty)
                            {
                                sm.TaskId = null;
                            }
                            else
                            {
                                Command = "Update";
                            }
                            if (sm.MaxInstances == string.Empty)
                            {
                                sm.MaxInstances = null;
                            }
                            // Build the Task Information
                            try
                            { 
                                Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Get"),
                                        new DataClass.TParams("SchedulerTaskTypeID", 4, SqlDbType.Int, sm.TaskTypeId)
                                };
                                DataSet dsTT = db.DIRunSPretDs(Parms, "sp_SchedulerTaskType");
                                if (dsTT.Tables[0].Rows.Count > 0)
                                {
                                    string TaskTypeCode = dsTT.Tables[0].Rows[0]["TaskTypeCode"].ToString();
                                    sm.TaskInformation = string.Empty;
                                    StringBuilder t = new StringBuilder();
                                    t.AppendLine("<Task xmlns=\"http://tempuri.org/Task.xsd\"><Type>" + TaskTypeCode + "</Type>");
                                    switch (TaskTypeCode)
                                    {
                                        case "SQL":
                                            {
                                                t.AppendLine("<SQL>");
                                                t.AppendLine("<Connection><![CDATA[" + sm.SQLConnection + "]]></Connection>");
                                                t.AppendLine("<Code><![CDATA[" + sm.SQLCommand + "]]></Code>");
                                                t.AppendLine("</SQL>");
                                                break;
                                            }
                                        case "EXECUTE":
                                            {
                                                t.AppendLine("<Execute>");
                                                t.AppendLine("<Path><![CDATA[" + sm.ExecutePath + "]]></Path>");
                                                t.AppendLine("<Command><![CDATA[" + sm.ExecuteCommand + "]]></Command>");
                                                t.AppendLine("<Parms><![CDATA[" + sm.ExecuteParameters + "]]></Parms>");
                                                t.AppendLine("</Execute>");
                                                break;
                                            }
                                        case "FILECOPY":
                                            {
                                                t.AppendLine("<Copy>");
                                                t.AppendLine("<SourcePath><![CDATA[" + sm.FileCopySource + "]]></SourcePath>");
                                                t.AppendLine("<DestPath><![CDATA[" + sm.FileCopyDestination + "]]></DestPath>");
                                                t.AppendLine("<List><![CDATA[" + sm.FileCopyFiles + "]]></List>");
                                                t.AppendLine("</Copy>");
                                                break;
                                            }
                                        case "SP":
                                            {
                                                t.AppendLine("<SP>");
                                                t.AppendLine("<Connection><![CDATA[" + sm.SPConnection + "]]></Connection>");
                                                t.AppendLine("<Name><![CDATA[" + sm.SPName + "]]></Name>");
                                                t.AppendLine(sm.SPParameters);
                                                t.AppendLine("</SP>");
                                                break;
                                            }
                                        case "EMAIL":
                                            {
                                                t.AppendLine("<Email>");
                                                t.AppendLine("<FromAddress><![CDATA[" + sm.MailFrom + "]]></FromAddress>");
                                                t.AppendLine("<ToAddress><![CDATA[" + sm.MailTo + "]]></ToAddress>");
                                                t.AppendLine("<Subject><![CDATA[" + sm.MailSubject + "]]></Subject>");
                                                t.AppendLine("<Body><![CDATA[" + sm.MailBody + "]]></Body>");
                                                t.AppendLine("</Email>");
                                                break;
                                            }
                                        case "SCRIPT":
                                            {
                                                t.AppendLine("<Script>");
                                                t.AppendLine("<Language>VB</Language>");
                                                t.AppendLine("<Code><![CDATA[" + sm.ScriptCode + "]]></Code>");
                                                t.AppendLine("</Script>");
                                                break;
                                            }
                                        case "DTS":
                                            {
                                                t.AppendLine("<DTS>");
                                                t.AppendLine("<DTSPath><![CDATA[" + sm.DTSPath + "]]></DTSPath>");
                                                t.AppendLine("</DTS>");
                                                break;
                                            }
                                        case "DLL":
                                            {
                                                t.AppendLine("<DLL>");
                                                t.AppendLine("<DLLPath><![CDATA[" + sm.DLLPath + "]]></DLLPath>");
                                                t.AppendLine("<DLLClass><![CDATA[" + sm.DLLClass + "]]></DLLClass");
                                                t.AppendLine("<DLLMethod><![CDATA[" + sm.DLLMethod + "]]></DLLMethod>");
                                                t.AppendLine("</DLL>");
                                                break;
                                            }
                                    }
                                    t.AppendLine("</Task>");
                                    sm.TaskInformation = t.ToString();
                                    // Save Data to database
                                    Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, Command),
                                            new DataClass.TParams("SchedulerTaskID", 4, SqlDbType.Int, sm.TaskId),
                                            new DataClass.TParams("SchedulerJobID", 4, SqlDbType.Int, sm.JobId),
                                            new DataClass.TParams("TaskName", 100, SqlDbType.VarChar, sm.TaskName),
                                            new DataClass.TParams("IsActive", 1, SqlDbType.Bit, sm.TaskIsActive),
                                            new DataClass.TParams("SortOrder", 4, SqlDbType.Int, sm.TaskSortOrder),
                                            new DataClass.TParams("SchedulerTaskTypeID", 4, SqlDbType.Int, sm.TaskTypeId),
                                            new DataClass.TParams("TaskInformation", sm.TaskInformation.Length, SqlDbType.VarChar, sm.TaskInformation)
                                    };
                                    db.DIRunSP(Parms, "sp_SchedulerTask");
                                    sm.TaskId = string.Empty;
                                    sm.TaskName = string.Empty;
                                    sm.TaskIsActive = true;
                                    sm.TaskSortOrder = 0;
                                    sm.TaskTypeId = string.Empty;
                                    sm.TaskInformation = string.Empty;
                                    sm.SQLCommand = string.Empty;
                                    sm.SQLConnection = string.Empty;
                                    sm.FileCopyDestination = string.Empty;
                                    sm.FileCopyFiles = string.Empty;
                                    sm.FileCopySource = string.Empty;
                                    sm.SPConnection = string.Empty;
                                    sm.SPDataType = string.Empty;
                                    sm.SPMessage = string.Empty;
                                    sm.SPName = string.Empty;
                                    sm.SPParameter = string.Empty;
                                    sm.SPParameters = string.Empty;
                                    sm.SPValue = string.Empty;
                                    sm.MailBody = string.Empty;
                                    sm.MailFrom = string.Empty;
                                    sm.MailSubject = string.Empty;
                                    sm.MailTo = string.Empty;
                                    sm.ExecuteCommand = string.Empty;
                                    sm.ExecuteParameters = string.Empty;
                                    sm.ExecutePath = string.Empty;
                                    sm.DTSPath = string.Empty;
                                    sm.DLLPath = string.Empty;
                                    sm.DLLClass = string.Empty;
                                    sm.DLLMethod = string.Empty;
                                    sm.TaskMessage = string.Empty;
                                }
                            }
                            catch (Exception ex)
                            {
                                sm.TaskMessage = ex.Message;
                            }
                        }
                    }

                    if (Request.Params["btnSPSave"] != null)
                    {
                        // Add Parameter to list
                        DataSet dsSP = new DataSet();
                        try
                        {
                            dsSP = db.ConvertXMLToDataSet(sm.SPParameters);
                        }
                        catch { }
                        if (dsSP.Tables.Count == 0)
                        {
                            // Create Table
                            DataTable dt = new DataTable("Parms");
                            DataColumn dc = new DataColumn("Name");
                            dt.Columns.Add(dc);
                            dc = new DataColumn("Value");
                            dt.Columns.Add(dc);
                            dc = new DataColumn("DataType");
                            dt.Columns.Add(dc);
                            dsSP.Tables.Add(dt);
                            dsSP.AcceptChanges();
                        }
                        // Add entry to existing table
                        DataRow drNew = dsSP.Tables[0].NewRow();
                        drNew["Name"] = sm.SPParameter;
                        drNew["Value"] = sm.SPValue;
                        drNew["DataType"] = sm.SPDataType;
                        dsSP.Tables[0].Rows.Add(drNew);
                        sm.SPParameters = db.ConvertDataSetToXML(dsSP);
                        sm.SPParameter = string.Empty;
                        sm.SPValue = string.Empty;
                        sm.SPDataType = string.Empty;
                    }

                    // Deletes
                    if (delq.Length > 4)
                    {
                        sm.QueueID = delq.Substring(4);
                        try
                        {
                            Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Delete"),
                                    new DataClass.TParams("SchedulerQueueID", 4, SqlDbType.Int, sm.QueueID)
                            };
                            db.DIRunSP(Parms, "sp_SchedulerQueue");
                            sm.QueueID = string.Empty;
                            sm.QueueName = string.Empty;
                            sm.QueueIsActive = true;
                            sm.QueueIsRunning = false;
                            sm.MaxThreads = 1;
                            sm.MaxMinutes = string.Empty;
                            sm.QueueMessage = string.Empty;
                            sm.ShowQueueButtons = "none";
                        }
                        catch (Exception ex)
                        {
                            sm.QueueMessage = ex.Message;
                        }
                    }
                    if (dela.Length > 4)
                    {
                        sm.AgentID = dela.Substring(4);
                        try
                        {
                            Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Delete"),
                                    new DataClass.TParams("SchedulerAgentID", 4, SqlDbType.Int, sm.AgentID)
                            };
                            db.DIRunSP(Parms, "sp_SchedulerAgent");
                            sm.AgentID = string.Empty;
                            sm.ServerID = string.Empty;
                            sm.AgentIsActive = true;
                            sm.AgentMessage = string.Empty;
                            sm.ShowAgentButtons = "none";
                        }
                        catch (Exception ex)
                        {
                            sm.AgentMessage = ex.Message;
                        }
                    }
                    if (dele.Length > 4)
                    {
                        sm.ExclusionID = dele.Substring(4);
                        try
                        {
                            Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Delete"),
                                    new DataClass.TParams("SchedulerExclusionID", 4, SqlDbType.Int, sm.ExclusionID)
                            };
                            db.DIRunSP(Parms, "sp_SchedulerExclusion");
                            sm.ExclusionID = string.Empty;
                            sm.ExclusionIsActive = true;
                            sm.Sunday = true;
                            sm.Monday = true;
                            sm.Tuesday = true;
                            sm.Wednesday = true;
                            sm.Thursday = true;
                            sm.Friday = true;
                            sm.Saturday = true;
                            sm.IsGlobal = false;
                            sm.StartTime = string.Empty;
                            sm.EndTime = string.Empty;
                            sm.SpecificDate = string.Empty;
                            sm.ExclusionMessage = string.Empty;
                        }
                        catch (Exception ex)
                        {
                            sm.ExclusionMessage = ex.Message;
                        }
                    }
                    if (delf.Length > 4)
                    {
                        sm.FolderId = delf.Substring(4);
                        try
                        {
                            Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Delete"),
                                    new DataClass.TParams("SchedulerFolderID", 4, SqlDbType.Int, sm.FolderId)
                            };
                            db.DIRunSP(Parms, "sp_SchedulerFolder");
                            sm.FolderId = string.Empty;
                            sm.FolderName = string.Empty;
                            sm.ParentFolderId = string.Empty;
                            sm.FolderIsActive = true;
                            sm.FolderSortOrder = 0;
                            sm.FolderMessage = string.Empty;
                            sm.ShowFolderButtons = "none";
                        }
                        catch (Exception ex)
                        {
                            sm.FolderMessage = ex.Message;
                        }
                    }
                    if (delv.Length > 4)
                    {
                        sm.VariableId = delv.Substring(4);
                        try
                        {
                            Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Delete"),
                                    new DataClass.TParams("SchedulerVariableID", 4, SqlDbType.Int, sm.VariableId)
                            };
                            db.DIRunSP(Parms, "sp_SchedulerVariable");
                            sm.VariableId = string.Empty;
                            sm.VariableName = string.Empty;
                            sm.VariableValue = string.Empty;
                            sm.VariableDescription = string.Empty;
                            sm.VariableIsActive = true;
                            sm.VariableSortOrder = 0;
                            sm.VariableMessage = string.Empty;
                        }
                        catch (Exception ex)
                        {
                            sm.VariableMessage = ex.Message;
                        }
                    }
                    if (delj.Length > 4)
                    {
                        sm.JobId = delj.Substring(4);
                        try
                        {
                            Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Delete"),
                                    new DataClass.TParams("SchedulerJobId", 4, SqlDbType.Int, sm.JobId)
                            };
                            db.DIRunSP(Parms, "sp_SchedulerJob");
                            sm.JobId = string.Empty;
                            sm.JobName = string.Empty;
                            sm.JobIsActive = true;
                            sm.JobSortOrder = 0;
                            sm.JobIntervalId = string.Empty;
                            sm.JobAgentId = string.Empty;
                            sm.JobOperatorId = string.Empty;
                            sm.MaxInstances = string.Empty;
                            sm.JobMessage = string.Empty;
                            sm.ShowJobButtons = "none";
                        }
                        catch (Exception ex)
                        {
                            sm.JobMessage = ex.Message;
                        }
                    }
                    if (deli.Length > 4)
                    {
                        sm.IntervalId = deli.Substring(4);
                        try
                        {
                            Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Delete"),
                                    new DataClass.TParams("SchedulerIntervalID", 4, SqlDbType.Int, sm.IntervalId)
                            };
                            db.DIRunSP(Parms, "sp_SchedulerInterval");
                            sm.IntervalId = string.Empty;
                            sm.IntervalName = string.Empty;
                            sm.IntervalIsActive = true;
                            sm.Occurrences = string.Empty;
                            sm.IntervalType = string.Empty;
                            sm.IntervalStartTime = string.Empty;
                            sm.IntervalEndTime = string.Empty;
                            sm.IntervalExclusionStart = string.Empty;
                            sm.IntervalExclusionEnd = string.Empty;
                            sm.IntervalRepeatMinutes = string.Empty;
                            sm.IntervalDetails = string.Empty;
                            sm.IntervalMessage = string.Empty;
                        }
                        catch (Exception ex)
                        {
                            sm.IntervalMessage = ex.Message;
                        }
                    }
                    if (delt.Length > 4)
                    {
                        sm.TaskId = delt.Substring(4);
                        try
                        {
                            Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Delete"),
                                    new DataClass.TParams("SchedulerTaskID", 4, SqlDbType.Int, sm.TaskId)
                            };
                            db.DIRunSP(Parms, "sp_SchedulerTask");
                            sm.TaskId = string.Empty;
                            sm.TaskName = string.Empty;
                            sm.TaskIsActive = true;
                            sm.TaskSortOrder = 0;
                            sm.TaskTypeId = string.Empty;
                            sm.TaskInformation = string.Empty;
                            sm.SQLCommand = string.Empty;
                            sm.SQLConnection = string.Empty;
                            sm.FileCopyDestination = string.Empty;
                            sm.FileCopyFiles = string.Empty;
                            sm.FileCopySource = string.Empty;
                            sm.SPConnection = string.Empty;
                            sm.SPDataType = string.Empty;
                            sm.SPMessage = string.Empty;
                            sm.SPName = string.Empty;
                            sm.SPParameter = string.Empty;
                            sm.SPParameters = string.Empty;
                            sm.SPValue = string.Empty;
                            sm.MailBody = string.Empty;
                            sm.MailFrom = string.Empty;
                            sm.MailSubject = string.Empty;
                            sm.MailTo = string.Empty;
                            sm.ExecuteCommand = string.Empty;
                            sm.ExecuteParameters = string.Empty;
                            sm.ExecutePath = string.Empty;
                            sm.DTSPath = string.Empty;
                            sm.DLLPath = string.Empty;
                            sm.DLLClass = string.Empty;
                            sm.DLLMethod = string.Empty;
                            sm.TaskMessage = string.Empty;
                            sm.ShowSPTask = "none";
                            sm.ShowSQLTask = "none";
                            sm.ShowScriptTask = "none";
                            sm.ShowMailTask = "none";
                            sm.ShowFileCopyTask = "none";
                            sm.ShowExecuteProgram = "none";
                            sm.ShowDTS = "none";
                            sm.ShowDLL = "none";
                        }
                        catch (Exception ex)
                        {
                            sm.TaskMessage = ex.Message;
                        }
                    }

                    if (dels.Length > 4)
                    {
                        // Delete SP Parameter
                        DataSet dsSP = new DataSet();
                        try
                        {
                            dsSP = db.ConvertXMLToDataSet(sm.SPParameters);
                        }
                        catch { }
                        if (dsSP.Tables.Count > 0)
                        {
                            // look for and delete value
                            foreach(DataRow dr in dsSP.Tables[1].Rows)
                            {
                                if (dr["Name"].ToString() == dels.Substring(4))
                                {
                                    dsSP.Tables[0].Rows.Remove(dr);
                                    break;
                                }
                            }
                        }
                        sm.SPParameters = db.ConvertDataSetToXML(dsSP);
                        sm.SPParameter = string.Empty;
                        sm.SPValue = string.Empty;
                        sm.SPDataType = string.Empty;
                    }
                }

                if (sm.ViewAccess || sm.AdminAccess || sm.OperatorAccess)
                {
                    // Allow Click to View details
                    // Edits
                    if (edtq.Length > 4)
                    {
                        sm.QueueID = edtq.Substring(4);
                        Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Get"),
                                new DataClass.TParams("SchedulerQueueID", 4, SqlDbType.Int, sm.QueueID)
                        };
                        DataSet dsQ = db.DIRunSPretDs(Parms, "sp_SchedulerQueue");
                        if (dsQ.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = dsQ.Tables[0].Rows[0];
                            sm.QueueID = dr["SchedulerQueueID"].ToString();
                            sm.QueueName = dr["QueueName"].ToString();
                            sm.QueueIsActive = bool.Parse(dr["IsActive"].ToString());
                            sm.QueueIsRunning = bool.Parse(dr["IsRunning"].ToString());
                            sm.MaxThreads = int.Parse(dr["MaxThreads"].ToString());
                            sm.MaxMinutes = string.Empty;
                            if (dr["MaxMinutes"].ToString() != string.Empty)
                            {
                                sm.MaxMinutes = dr["MaxMinutes"].ToString();
                            }
                            sm.QueueMessage = string.Empty;
                            sm.ShowQueueButtons = "inline";
                        }
                    }
                    if (edta.Length > 4)
                    {
                        sm.AgentID = edta.Substring(4);
                        Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Get"),
                                new DataClass.TParams("SchedulerAgentID", 4, SqlDbType.Int, sm.AgentID)
                        };
                        DataSet dsA = db.DIRunSPretDs(Parms, "sp_SchedulerAgent");
                        if (dsA.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = dsA.Tables[0].Rows[0];
                            sm.AgentID = dr["SchedulerAgentID"].ToString();
                            sm.ServerID = dr["SchedulerServerID"].ToString();
                            sm.AgentIsActive = bool.Parse(dr["IsActive"].ToString());
                            sm.AgentMessage = string.Empty;
                            sm.ShowAgentButtons = "inline";
                        }
                    }
                    if (edte.Length > 4)
                    {
                        sm.ExclusionID = edte.Substring(4);
                        Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Get"),
                                new DataClass.TParams("SchedulerExclusionID", 4, SqlDbType.Int, sm.ExclusionID)
                        };
                        DataSet dsE = db.DIRunSPretDs(Parms, "sp_SchedulerExclusion");
                        if (dsE.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = dsE.Tables[0].Rows[0];
                            sm.ExclusionID = dr["SchedulerExclusionID"].ToString();
                            if (dr["SchedulerAgentID"].ToString() == string.Empty)
                            {
                                sm.IsGlobal = true;
                            }
                            sm.ExclusionIsActive = bool.Parse(dr["IsActive"].ToString());
                            sm.Sunday = bool.Parse(dr["Sunday"].ToString());
                            sm.Monday = bool.Parse(dr["Monday"].ToString());
                            sm.Tuesday = bool.Parse(dr["Tuesday"].ToString());
                            sm.Wednesday = bool.Parse(dr["Wednesday"].ToString());
                            sm.Thursday = bool.Parse(dr["Thursday"].ToString());
                            sm.Friday = bool.Parse(dr["Friday"].ToString());
                            sm.Saturday = bool.Parse(dr["Saturday"].ToString());
                            sm.StartTime = dr["StartTime"].ToString();
                            sm.EndTime = dr["EndTime"].ToString();
                            sm.SpecificDate = string.Empty;
                            if (dr["SpecificDate"].ToString() != string.Empty)
                            {
                                sm.SpecificDate = DateTime.Parse(dr["SpecificDate"].ToString()).ToString("MM\\/dd\\/yyyy");
                            }
                            sm.ExclusionMessage = string.Empty;
                        }
                    }
                    if (edtf.Length > 4)
                    {
                        sm.FolderId = edtf.Substring(4);
                        Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Get"),
                                new DataClass.TParams("SchedulerFolderID", 4, SqlDbType.Int, sm.FolderId)
                        };
                        DataSet dsF = db.DIRunSPretDs(Parms, "sp_SchedulerFolder");
                        if (dsF.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = dsF.Tables[0].Rows[0];
                            sm.FolderId = dr["SchedulerFolderID"].ToString();
                            sm.FolderName = dr["SchedulerFolderName"].ToString();
                            sm.ParentFolderId = dr["ParentSchedulerFolderID"].ToString();
                            sm.FolderIsActive = bool.Parse(dr["IsActive"].ToString());
                            sm.FolderSortOrder = int.Parse(dr["SortOrder"].ToString());
                            sm.ShowFolderButtons = "inline";
                        }
                    }
                    if (edtj.Length > 4)
                    {
                        sm.JobId = edtj.Substring(4);
                        Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Get"),
                                new DataClass.TParams("SchedulerJobID", 4, SqlDbType.Int, sm.JobId)
                        };
                        DataSet dsJ = db.DIRunSPretDs(Parms, "sp_SchedulerJob");
                        if (dsJ.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = dsJ.Tables[0].Rows[0];
                            sm.JobId = dr["SchedulerJobID"].ToString();
                            sm.JobName = dr["JobName"].ToString();
                            sm.JobIsActive = bool.Parse(dr["IsActive"].ToString());
                            sm.JobSortOrder = int.Parse(dr["SortOrder"].ToString());
                            sm.JobIntervalId = dr["SchedulerIntervalID"].ToString();
                            sm.JobAgentId = dr["SchedulerAgentID"].ToString();
                            sm.JobOperatorId = dr["SchedulerOperatorID"].ToString();
                            sm.MaxInstances = dr["MaxInstances"].ToString();
                            sm.JobMessage = string.Empty;
                            sm.ShowJobButtons = "inline";
                        }
                    }
                    if (edti.Length > 4)
                    {
                        sm.IntervalId = edti.Substring(4);
                        Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Get"),
                                new DataClass.TParams("SchedulerIntervalID", 4, SqlDbType.Int, sm.IntervalId)
                        };
                        DataSet dsI = db.DIRunSPretDs(Parms, "sp_SchedulerInterval");
                        if (dsI.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = dsI.Tables[0].Rows[0];
                            sm.IntervalId = dr["SchedulerIntervalID"].ToString();
                            sm.IntervalName = dr["IntervalName"].ToString();
                            sm.IntervalIsActive = bool.Parse(dr["IsActive"].ToString());
                            sm.Occurrences = dr["Occurrences"].ToString();
                            sm.IntervalType = dr["IntervalType"].ToString();
                            sm.IntervalStartTime = string.Empty;
                            if (dr["StartTime"] != null)
                            {
                                sm.IntervalStartTime = DateTime.Parse(dr["StartTime"].ToString()).ToString("MM\\/dd\\/yyyy");
                            }
                            sm.IntervalEndTime = string.Empty;
                            if (dr["EndTime"] != null)
                            {
                                sm.IntervalEndTime = DateTime.Parse(dr["EndTime"].ToString()).ToString("MM\\/dd\\/yyyy");
                            }
                            sm.IntervalExclusionStart = string.Empty;
                            if (dr["ExclusionStart"] != null)
                            {
                                sm.IntervalExclusionStart = DateTime.Parse(dr["ExclusionStart"].ToString()).ToString("MM\\/dd\\/yyyy");
                            }
                            sm.IntervalExclusionEnd = string.Empty;
                            if (dr["ExclusionEnd"] != null)
                            {
                                sm.IntervalExclusionEnd = DateTime.Parse(dr["ExclusionEnd"].ToString()).ToString("MM\\/dd\\/yyyy");
                            }
                            sm.IntervalRepeatMinutes = string.Empty;
                            if (dr["RepeatMinutes"] != null)
                            {
                                sm.IntervalRepeatMinutes = dr["RepeatMinutes"].ToString();
                            }
                            sm.IntervalMessage = string.Empty;
                        }
                    }
                    if (edtt.Length > 4)
                    {
                        sm.TaskId = edtt.Substring(4);
                        Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Get"),
                                new DataClass.TParams("SchedulerTaskID", 4, SqlDbType.Int, sm.TaskId)
                        };
                        DataSet dsT = db.DIRunSPretDs(Parms, "sp_SchedulerTask");
                        if (dsT.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = dsT.Tables[0].Rows[0];
                            sm.TaskId = dr["SchedulerTaskID"].ToString();
                            sm.TaskName = dr["TaskName"].ToString();
                            sm.TaskIsActive = bool.Parse(dr["IsActive"].ToString());
                            sm.TaskSortOrder = int.Parse(dr["SortOrder"].ToString());
                            sm.TaskTypeId = dr["SchedulerTaskTypeId"].ToString();
                            sm.TaskInformation = dr["TaskInformation"].ToString();
                            DataSet dsTI = new DataSet();
                            if (sm.TaskInformation != string.Empty)
                            {
                                dsTI = db.ConvertXMLToDataSet(sm.TaskInformation);
                                if (dsTI == null)
                                {
                                    sm.TaskMessage = db.ConversionError;
                                }
                            }
                            switch (dr["TaskTypeCode"].ToString())
                            {
                                case "SQL":
                                    {
                                        sm.SQLConnection = dsTI.Tables[1].Rows[0]["Connection"].ToString();
                                        sm.SQLCommand = dsTI.Tables[1].Rows[0]["Code"].ToString();
                                        sm.ShowSPTask = "none";
                                        sm.ShowSQLTask = "inline";
                                        sm.ShowScriptTask = "none";
                                        sm.ShowMailTask = "none";
                                        sm.ShowFileCopyTask = "none";
                                        sm.ShowExecuteProgram = "none";
                                        break;
                                    }
                                case "EXECUTE":
                                    {
                                        sm.ShowSPTask = "none";
                                        sm.ShowSQLTask = "none";
                                        sm.ShowScriptTask = "none";
                                        sm.ShowMailTask = "none";
                                        sm.ShowFileCopyTask = "none";
                                        sm.ShowExecuteProgram = "inline";
                                        if (dsTI != null)
                                        {
                                            if (dsTI.Tables[0].Rows.Count > 0)
                                            {
                                                sm.ExecutePath = dsTI.Tables[1].Rows[0]["Path"].ToString();
                                                sm.ExecuteCommand = dsTI.Tables[1].Rows[0]["Command"].ToString();
                                                sm.ExecuteParameters = dsTI.Tables[1].Rows[0]["Parms"].ToString();
                                            }
                                        }
                                        break;
                                    }
                                case "FILECOPY":
                                    {
                                        sm.FileCopySource = dsTI.Tables[1].Rows[0]["SourcePath"].ToString();
                                        sm.FileCopyDestination = dsTI.Tables[1].Rows[0]["DestPath"].ToString();
                                        sm.FileCopyFiles = dsTI.Tables[1].Rows[0]["List"].ToString();
                                        sm.ShowSPTask = "none";
                                        sm.ShowSQLTask = "none";
                                        sm.ShowScriptTask = "none";
                                        sm.ShowMailTask = "none";
                                        sm.ShowFileCopyTask = "inline";
                                        sm.ShowExecuteProgram = "none";
                                        break;
                                    }
                                case "SP":
                                    {
                                        sm.SPConnection = dsTI.Tables[1].Rows[0]["Connection"].ToString();
                                        sm.SPName = dsTI.Tables[1].Rows[0]["Name"].ToString();
                                        StringBuilder p = new StringBuilder();
                                        p.AppendLine("<Parms>");
                                        foreach(DataRow drp in dsTI.Tables[2].Rows)
                                        {
                                            p.AppendLine("<Parm><Name><![CDATA[" + drp["Name"].ToString() + "]></Name>" +
                                                "<Value><![CDATA[" + drp["Value"].ToString() + "]></Value>" +
                                                "<DataType>" + drp["DataType"].ToString() + "</DataType></Parm>");
                                        }
                                        p.AppendLine("</Parms>");
                                        sm.SPParameters = p.ToString();
                                        sm.ShowSPTask = "inline";
                                        sm.ShowSQLTask = "none";
                                        sm.ShowScriptTask = "none";
                                        sm.ShowMailTask = "none";
                                        sm.ShowFileCopyTask = "none";
                                        sm.ShowExecuteProgram = "none";
                                        break;
                                    }
                                case "EMAIL":
                                    {
                                        sm.MailFrom = dsTI.Tables[1].Rows[0]["FromAddress"].ToString();
                                        sm.MailTo = dsTI.Tables[1].Rows[0]["ToAddress"].ToString();
                                        sm.MailSubject = dsTI.Tables[1].Rows[0]["Subject"].ToString();
                                        sm.MailBody = dsTI.Tables[1].Rows[0]["Body"].ToString();
                                        sm.ShowSPTask = "none";
                                        sm.ShowSQLTask = "none";
                                        sm.ShowScriptTask = "none";
                                        sm.ShowMailTask = "inline";
                                        sm.ShowFileCopyTask = "none";
                                        sm.ShowExecuteProgram = "none";
                                        break;
                                    }
                                case "SCRIPT":
                                    {
                                        sm.ScriptCode = dsTI.Tables[1].Rows[0]["Code"].ToString();
                                        sm.ShowSPTask = "none";
                                        sm.ShowSQLTask = "none";
                                        sm.ShowScriptTask = "inline";
                                        sm.ShowMailTask = "none";
                                        sm.ShowFileCopyTask = "none";
                                        sm.ShowExecuteProgram = "none";
                                        break;
                                    }
                                case "DTS":
                                    {
                                        sm.DTSPath = dsTI.Tables[1].Rows[0]["DTSPath"].ToString();
                                        sm.ShowDTS = "inline";
                                        sm.ShowDLL = "none";
                                        sm.ShowSPTask = "none";
                                        sm.ShowSQLTask = "none";
                                        sm.ShowScriptTask = "none";
                                        sm.ShowMailTask = "none";
                                        sm.ShowFileCopyTask = "none";
                                        sm.ShowExecuteProgram = "none";
                                        break;
                                    }
                                case "DLL":
                                    {
                                        sm.DLLPath = dsTI.Tables[1].Rows[0]["DLLPath"].ToString();
                                        sm.DLLClass = dsTI.Tables[1].Rows[0]["DLLClass"].ToString();
                                        sm.DLLMethod = dsTI.Tables[1].Rows[0]["DLLMethod"].ToString();
                                        sm.ShowDTS = "none";
                                        sm.ShowDLL = "inline";
                                        sm.ShowSPTask = "none";
                                        sm.ShowSQLTask = "none";
                                        sm.ShowScriptTask = "none";
                                        sm.ShowMailTask = "none";
                                        sm.ShowFileCopyTask = "none";
                                        sm.ShowExecuteProgram = "none";
                                        break;
                                    }
                            }
                        }
                    }

                    if (edts.Length > 4)
                    {
                        // Edit SP Parameter
                        DataSet dsSP = new DataSet();
                        try
                        {
                            dsSP = db.ConvertXMLToDataSet(sm.SPParameters);
                        }
                        catch { }
                        if (dsSP.Tables.Count > 0)
                        {
                            // look for and delete value
                            foreach (DataRow dr in dsSP.Tables[1].Rows)
                            {
                                if (dr["Name"].ToString() == edts.Substring(4))
                                {
                                    sm.SPParameter = dr["Name"].ToString();
                                    sm.SPValue = dr["Value"].ToString();
                                    sm.SPDataType = dr["DataType"].ToString();
                                    break;
                                }
                            }
                        }
                    }

                    // Clears
                    if (Request.Params["btnQueueClear"] != null)
                    {
                        sm.QueueID = string.Empty;
                        sm.QueueName = string.Empty;
                        sm.QueueIsActive = true;
                        sm.QueueIsRunning = false;
                        sm.MaxThreads = 1;
                        sm.MaxMinutes = string.Empty;
                        sm.QueueMessage = string.Empty;
                        sm.ShowQueueButtons = "none";
                    }
                    if (Request.Params["btnAgentClear"] != null)
                    {
                        sm.AgentID = string.Empty;
                        sm.ServerID = string.Empty;
                        sm.AgentIsActive = true;
                        sm.AgentMessage = string.Empty;
                        sm.ShowAgentButtons = "none";
                    }
                    if (Request.Params["btnExclusionClear"] != null)
                    {
                        sm.ExclusionID = string.Empty;
                        sm.ExclusionIsActive = true;
                        sm.Sunday = true;
                        sm.Monday = true;
                        sm.Tuesday = true;
                        sm.Wednesday = true;
                        sm.Thursday = true;
                        sm.Friday = true;
                        sm.Saturday = true;
                        sm.IsGlobal = false;
                        sm.StartTime = string.Empty;
                        sm.EndTime = string.Empty;
                        sm.SpecificDate = string.Empty;
                        sm.ExclusionMessage = string.Empty;
                    }
                    if (Request.Params["btnFolderClear"] != null)
                    {
                        sm.FolderId = string.Empty;
                        sm.FolderName = string.Empty;
                        sm.ParentFolderId = string.Empty;
                        sm.FolderIsActive = true;
                        sm.FolderSortOrder = 0;
                        sm.FolderMessage = string.Empty;
                        sm.ShowFolderButtons = "none";
                    }
                    if (Request.Params["btnJobClear"] != null)
                    {
                        sm.JobId = string.Empty;
                        sm.JobName = string.Empty;
                        sm.JobIsActive = true;
                        sm.JobSortOrder = 0;
                        sm.JobIntervalId = string.Empty;
                        sm.JobAgentId = string.Empty;
                        sm.JobOperatorId = string.Empty;
                        sm.MaxInstances = string.Empty;
                        sm.JobMessage = string.Empty;
                        sm.ShowJobButtons = "none";
                    }
                    if (Request.Params["btnIntervalClear"] != null)
                    {
                        sm.IntervalId = string.Empty;
                        sm.IntervalName = string.Empty;
                        sm.Interval = 0;
                        sm.IntervalIsActive = true;
                        sm.IntervalStartTime = string.Empty;
                        sm.IntervalEndTime = string.Empty;
                        sm.IntervalExclusionStart = string.Empty;
                        sm.IntervalExclusionEnd = string.Empty;
                        sm.IntervalRepeatMinutes = string.Empty;
                        sm.IntervalDetails = string.Empty;
                        sm.IntervalMessage = string.Empty;
                    }
                    if (Request.Params["btnTaskClear"] != null)
                    {
                        sm.TaskId = string.Empty;
                        sm.TaskName = string.Empty;
                        sm.TaskIsActive = true;
                        sm.TaskSortOrder = 0;
                        sm.TaskTypeId = string.Empty;
                        sm.TaskInformation = string.Empty;
                        sm.SQLCommand = string.Empty;
                        sm.SQLConnection = string.Empty;
                        sm.FileCopyDestination = string.Empty;
                        sm.FileCopyFiles = string.Empty;
                        sm.FileCopySource = string.Empty;
                        sm.SPConnection = string.Empty;
                        sm.SPDataType = string.Empty;
                        sm.SPMessage = string.Empty;
                        sm.SPName = string.Empty;
                        sm.SPParameter = string.Empty;
                        sm.SPParameters = string.Empty;
                        sm.SPValue = string.Empty;
                        sm.MailBody = string.Empty;
                        sm.MailFrom = string.Empty;
                        sm.MailSubject = string.Empty;
                        sm.MailTo = string.Empty;
                        sm.ExecuteCommand = string.Empty;
                        sm.ExecuteParameters = string.Empty;
                        sm.ExecutePath = string.Empty;
                        sm.DTSPath = string.Empty;
                        sm.DLLPath = string.Empty;
                        sm.DLLClass = string.Empty;
                        sm.DLLMethod = string.Empty;
                        sm.TaskMessage = string.Empty;
                        sm.ShowSPTask = "none";
                        sm.ShowSQLTask = "none";
                        sm.ShowScriptTask = "none";
                        sm.ShowMailTask = "none";
                        sm.ShowFileCopyTask = "none";
                        sm.ShowExecuteProgram = "none";
                    }

                    if (Request.Params["btnSPClear"] != null)
                    {
                        // Clear SP Parameter
                        sm.SPConnection = string.Empty;
                        sm.SPName = string.Empty;
                        sm.SPParameters = string.Empty;
                        sm.SPMessage = string.Empty;
                        sm.SPDataType = string.Empty;
                        sm.SPParameter = string.Empty;
                        sm.SPValue = string.Empty;
                    }

                    // Other Buttons
                    if (Request.Params["btnAgents"] != null)
                    {
                        sm.ShowAgent = "inline";
                    }
                    if (Request.Params["btnFolders"] != null)
                    {
                        sm.ShowFolder = "inline";
                    }
                    if (Request.Params["btnVariables"] != null)
                    {
                        sm.ShowVariable = "inline";
                    }
                    if (Request.Params["btnJobs"] != null)
                    {
                        sm.ShowJob = "inline";
                    }
                    if (Request.Params["btnIntervals"] != null)
                    {
                        sm.ShowInterval = "inline";
                    }
                    if (Request.Params["btnTasks"] != null)
                    {
                        sm.ShowTask = "inline";
                    }
                    if (Request.Params["btnAgentClose"] != null)
                    {
                        sm.ShowAgent = "none";
                    }
                    if (Request.Params["btnExclusions"] != null)
                    {
                        sm.ShowExclusion = "inline";
                    }
                    if (Request.Params["btnExclusionClose"] != null)
                    {
                        sm.ShowExclusion = "none";
                    }
                    if (Request.Params["btnFolderClose"] != null)
                    {
                        sm.ShowFolderButtons = "none";
                        sm.ShowFolder = "none";
                    }
                    if (Request.Params["btnVariableClose"] != null)
                    {
                        sm.ShowVariable = "none";
                    }
                    if (Request.Params["btnJobClose"] != null)
                    {
                        sm.ShowJob = "none";
                        sm.ShowJobButtons = "none";
                    }
                    if (Request.Params["btnIntervalClose"] != null)
                    {
                        sm.ShowInterval = "none";
                    }
                    if (Request.Params["btnTaskClose"] != null)
                    {
                        sm.ShowTask = "none";
                    }

                    if (Request.Params["btnJobTrigger"] != null)
                    {
                        // Set up job to trigger start
                        if (sm.JobId != string.Empty)
                        {
                            Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Trigger"),
                                    new DataClass.TParams("SchedulerJobId", 4, SqlDbType.Int, sm.JobId)
                            };
                            DataSet dsJobTrig = db.DIRunSPretDs(Parms, "sp_SchedulerJob");
                            if (dsJobTrig.Tables[0].Rows.Count > 0)
                            {
                                DataRow dr = dsJobTrig.Tables[0].Rows[0];
                                if (bool.Parse(dr["IsScheduled"].ToString()))
                                {
                                    sm.JobMessage = "Job Scheduled";
                                }
                            }
                        }
                    }
                }

                // Fill In Drop Downs
                sm.Operators = new List<SelectListItem>();
                Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "DropDown") };
                DataSet dsOperators = db.DIRunSPretDs(Parms, "sp_SchedulerOperator");
                sm.Operators.Add(new SelectListItem() { Text = "", Value = "" });
                foreach(DataRow dr in dsOperators.Tables[0].Rows)
                {
                    sm.Operators.Add(new SelectListItem() { Text = dr["SchedulerOperatorName"].ToString(), Value = dr["SchedulerOperatorId"].ToString() });
                }

                sm.TaskTypes = new List<SelectListItem>();
                DataSet dsTaskTypes = db.DIRunSPretDs(Parms, "sp_SchedulerTaskType");
                foreach(DataRow dr in dsTaskTypes.Tables[0].Rows)
                {
                    sm.TaskTypes.Add(new SelectListItem() { Text = dr["TaskTypeName"].ToString(), Value = dr["SchedulerTaskTypeID"].ToString() });
                }
                if (sm.TaskTypeId != string.Empty)
                {
                    DataRow[] drTT = dsTaskTypes.Tables[0].Select("SchedulerTaskTypeID = " + sm.TaskTypeId);
                    if (drTT.Length > 0)
                    {
                        switch (drTT[0]["TaskTypeCode"].ToString())
                        {
                            case "SP":
                                {
                                    sm.ShowSPTask = "inline";
                                    sm.ShowSQLTask = "none";
                                    sm.ShowScriptTask = "none";
                                    sm.ShowMailTask = "none";
                                    sm.ShowFileCopyTask = "none";
                                    sm.ShowExecuteProgram = "none";
                                    sm.ShowDTS = "none";
                                    sm.ShowDLL = "none";
                                    break;
                                }
                            case "EXECUTE":
                                {
                                    sm.ShowSPTask = "none";
                                    sm.ShowSQLTask = "none";
                                    sm.ShowScriptTask = "none";
                                    sm.ShowMailTask = "none";
                                    sm.ShowFileCopyTask = "none";
                                    sm.ShowExecuteProgram = "inline";
                                    sm.ShowDTS = "none";
                                    sm.ShowDLL = "none";
                                    break;
                                }
                            case "FILECOPY":
                                {
                                    sm.ShowSPTask = "none";
                                    sm.ShowSQLTask = "none";
                                    sm.ShowScriptTask = "none";
                                    sm.ShowMailTask = "none";
                                    sm.ShowFileCopyTask = "inline";
                                    sm.ShowExecuteProgram = "none";
                                    sm.ShowDTS = "none";
                                    sm.ShowDLL = "none";
                                    break;
                                }
                            case "SQL":
                                {
                                    sm.ShowSPTask = "none";
                                    sm.ShowSQLTask = "inline";
                                    sm.ShowScriptTask = "none";
                                    sm.ShowMailTask = "none";
                                    sm.ShowFileCopyTask = "none";
                                    sm.ShowExecuteProgram = "none";
                                    sm.ShowDTS = "none";
                                    sm.ShowDLL = "none";
                                    break;
                                }
                            case "EMAIL":
                                {
                                    sm.ShowSPTask = "none";
                                    sm.ShowSQLTask = "none";
                                    sm.ShowScriptTask = "none";
                                    sm.ShowMailTask = "inline";
                                    sm.ShowFileCopyTask = "none";
                                    sm.ShowExecuteProgram = "none";
                                    sm.ShowDTS = "none";
                                    sm.ShowDLL = "none";
                                    break;
                                }
                            case "SCRIPT":
                                {
                                    sm.ShowSPTask = "none";
                                    sm.ShowSQLTask = "none";
                                    sm.ShowScriptTask = "inline";
                                    sm.ShowMailTask = "none";
                                    sm.ShowFileCopyTask = "none";
                                    sm.ShowExecuteProgram = "none";
                                    sm.ShowDTS = "none";
                                    sm.ShowDLL = "none";
                                    break;
                                }
                            case "DTS":
                                {
                                    sm.ShowDTS = "inline";
                                    sm.ShowDLL = "none";
                                    sm.ShowSPTask = "none";
                                    sm.ShowSQLTask = "none";
                                    sm.ShowScriptTask = "none";
                                    sm.ShowMailTask = "none";
                                    sm.ShowFileCopyTask = "none";
                                    sm.ShowExecuteProgram = "none";
                                    break;
                                }
                            case "DLL":
                                {
                                    sm.ShowDTS = "none";
                                    sm.ShowDLL = "inline";
                                    sm.ShowSPTask = "none";
                                    sm.ShowSQLTask = "none";
                                    sm.ShowScriptTask = "none";
                                    sm.ShowMailTask = "none";
                                    sm.ShowFileCopyTask = "none";
                                    sm.ShowExecuteProgram = "none";
                                    break;
                                }
                        }
                    }
                }

                sm.Intervals = new List<SelectListItem>();
                DataSet dsIntervals = db.DIRunSPretDs(Parms, "sp_SchedulerInterval");
                sm.Intervals.Add(new SelectListItem() { Text = "", Value = "" });
                foreach(DataRow dr in dsIntervals.Tables[0].Rows)
                {
                    sm.Intervals.Add(new SelectListItem() { Text = dr["IntervalName"].ToString(), Value = dr["SchedulerIntervalID"].ToString() });
                }

                sm.ParentFolders = new List<SelectListItem>();
                DataSet dsParentFolders = db.DIRunSPretDs(Parms, "sp_SchedulerFolder");
                sm.ParentFolders.Add(new SelectListItem() { Text = "", Value = "" });
                foreach(DataRow dr in dsParentFolders.Tables[0].Rows)
                {
                    sm.ParentFolders.Add(new SelectListItem() { Text = dr["SchedulerFolderName"].ToString(), Value = dr["SchedulerFolderID"].ToString() });
                }

                sm.Servers = new List<SelectListItem>();
                DataSet dsServers = db.DIRunSPretDs(Parms, "sp_SchedulerServer");
                foreach(DataRow dr in dsServers.Tables[0].Rows)
                {
                    sm.Servers.Add(new SelectListItem() { Text = dr["ServerName"].ToString(), Value = dr["SchedulerServerID"].ToString() });
                }

                sm.Agents = new List<SelectListItem>();
                if (sm.QueueID != string.Empty)
                {
                    Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "DropDown"),
                            new DataClass.TParams("SchedulerQueueID", 4, SqlDbType.Int, sm.QueueID)
                    };
                    DataSet dsAgents = db.DIRunSPretDs(Parms, "sp_SchedulerAgent");
                    sm.Agents.Add(new SelectListItem() { Text = "", Value = "" });
                    foreach (DataRow dr in dsAgents.Tables[0].Rows)
                    {
                        sm.Agents.Add(new SelectListItem() { Text = dr["ServerName"].ToString(), Value = dr["SchedulerAgentId"].ToString() });
                    }
                }

                sm.IntervalTypes = new List<SelectListItem>();
                sm.IntervalTypes.Add(new SelectListItem() { Text = "Year", Value = "Year" });
                sm.IntervalTypes.Add(new SelectListItem() { Text = "Month", Value = "Month" });
                sm.IntervalTypes.Add(new SelectListItem() { Text = "Week", Value = "Week" });
                sm.IntervalTypes.Add(new SelectListItem() { Text = "Day", Value = "Day" });
                sm.IntervalTypes.Add(new SelectListItem() { Text = "Hour", Value = "Hour" });
                sm.IntervalTypes.Add(new SelectListItem() { Text = "Minute", Value = "Minute" });

                sm.DataTypes = new List<SelectListItem>();
                sm.DataTypes.Add(new SelectListItem() { Text = "BigInt", Value = "BigInt" });
                sm.DataTypes.Add(new SelectListItem() { Text = "Binary", Value = "Binary" });
                sm.DataTypes.Add(new SelectListItem() { Text = "Bit", Value = "Bit" });
                sm.DataTypes.Add(new SelectListItem() { Text = "Char", Value = "Char" });
                sm.DataTypes.Add(new SelectListItem() { Text = "Date", Value = "Date" });
                sm.DataTypes.Add(new SelectListItem() { Text = "DateTime", Value = "DateTime" });
                sm.DataTypes.Add(new SelectListItem() { Text = "DateTime2", Value = "DateTime2" });
                sm.DataTypes.Add(new SelectListItem() { Text = "DateTimeOffset", Value = "DateTimeOffset" });
                sm.DataTypes.Add(new SelectListItem() { Text = "Decimal", Value = "Decimal" });
                sm.DataTypes.Add(new SelectListItem() { Text = "Float", Value = "Float" });
                sm.DataTypes.Add(new SelectListItem() { Text = "Image", Value = "Image" });
                sm.DataTypes.Add(new SelectListItem() { Text = "Int", Value = "Int" });
                sm.DataTypes.Add(new SelectListItem() { Text = "Money", Value = "Money" });
                sm.DataTypes.Add(new SelectListItem() { Text = "NChar", Value = "NChar" });
                sm.DataTypes.Add(new SelectListItem() { Text = "NText", Value = "NText" });
                sm.DataTypes.Add(new SelectListItem() { Text = "NVarChar", Value = "NVarChar" });
                sm.DataTypes.Add(new SelectListItem() { Text = "Real", Value = "Real" });
                sm.DataTypes.Add(new SelectListItem() { Text = "SmallDateTime", Value = "SmallDateTime" });
                sm.DataTypes.Add(new SelectListItem() { Text = "SmallInt", Value = "SmallInt" });
                sm.DataTypes.Add(new SelectListItem() { Text = "SmallMoney", Value = "SmallMoney" });
                sm.DataTypes.Add(new SelectListItem() { Text = "Structured", Value = "Structured" });
                sm.DataTypes.Add(new SelectListItem() { Text = "Text", Value = "Text" });
                sm.DataTypes.Add(new SelectListItem() { Text = "Time", Value = "Time" });
                sm.DataTypes.Add(new SelectListItem() { Text = "TimeStamp", Value = "TimeStamp" });
                sm.DataTypes.Add(new SelectListItem() { Text = "TinyInt", Value = "TinyInt" });
                sm.DataTypes.Add(new SelectListItem() { Text = "UDT", Value = "UDT" });
                sm.DataTypes.Add(new SelectListItem() { Text = "UniqueIdentifier", Value = "UniqueIdentifier" });
                sm.DataTypes.Add(new SelectListItem() { Text = "VarBinary", Value = "VarBinary" });
                sm.DataTypes.Add(new SelectListItem() { Text = "VarChar", Value = "VarChar" });
                sm.DataTypes.Add(new SelectListItem() { Text = "Variant", Value = "Variant" });
                sm.DataTypes.Add(new SelectListItem() { Text = "Xml", Value = "Xml" });

                // Show Lists
                // Queue
                Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "List") };
                DataSet dsQueue = db.DIRunSPretDs(Parms, "sp_SchedulerQueue");
                StringBuilder b = new StringBuilder();
                b.Append("<table><tr class='gheader'>");
                int tblWidth = 0;
                if (sm.AdminAccess)
                {
                    b.Append("<th style='width:20px;'>&nbsp;</th>");
                    tblWidth += 27;
                }
                b.Append("<th style='width:500px;'>Queue Name</th><th style='width:150px;'>Max Threads</th><th style='width:150px;'>Max Minutes</th>" + 
                    "<th style='width:80px;'>Active</th><th style='width:80px;'>Running</th>");
                tblWidth += 507 + 157 + 157 + 87 + 87;
                if (sm.AdminAccess)
                {
                    b.Append("<th style='width:20px;'>&nbsp;</th>");
                    tblWidth += 27;
                }
                b.AppendLine("<th style='width:16px;'>&nbsp;</th></tr></table><div style='height:370px; width:" +
                    tblWidth.ToString() + "px; overflow:auto;'><table>");
                bool AltRow = false;
                foreach (DataRow dr in dsQueue.Tables[0].Rows)
                {
                    b.Append("<tr");
                    if (AltRow)
                    {
                        b.Append(" class='galtrow'");
                    }
                    b.Append(">");
                    if (sm.AdminAccess)
                    {
                        b.Append("<td style='width:20px;'>" +
                                "<button name=\"edtq" + dr["SchedulerQueueID"].ToString() + "\" type=\"submit\" " +
                                " class='imagebutton' style=\"background: url(../Images/edit_16x16.gif) no-repeat;\"" +
                                ">&nbsp;</button>" +
                                "</td>");
                    }
                    b.Append("<td style='width:500px;'>" + dr["QueueName"].ToString() + "</td>" +
                        "<td style='width:150px;'>" + dr["MaxThreads"].ToString() + "</td>" +
                        "<td style='width:150px;'>" + dr["MaxMinutes"].ToString() + "</td>");
                    b.Append("<td style='width:80px;text-align:center;'><input type = 'checkbox' disabled ");
                    if (bool.Parse(dr["IsActive"].ToString()))
                    {
                        b.Append("checked='checked' ");
                    }
                    b.Append("/></td>");
                    b.Append("<td style='width:80px;text-align:center;'><input type = 'checkbox' disabled ");
                    if (bool.Parse(dr["IsRunning"].ToString()))
                    {
                        b.Append("checked='checked' ");
                    }
                    b.Append("/></td>");
                    if (sm.AdminAccess)
                    {
                        b.Append("<td style='width:20px;'><button name=\"delq" + dr["SchedulerQueueID"].ToString() + "\" type=\"submit\" " +
                            " class='imagebutton' style=\"background: url(../Images/delete_16x16.gif) no-repeat;\"" +
                            ">&nbsp;</button></td>");
                    }
                    b.AppendLine("</tr>");
                    AltRow = !AltRow;
                }
                b.AppendLine("</table></div>");
                sm.QueueList = b.ToString();

                // Agent
                if (sm.ShowAgent == "inline")
                {
                    Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "List"),
                            new DataClass.TParams("SchedulerQueueID", 4, SqlDbType.Int, sm.QueueID)
                    };
                    DataSet dsAgent = db.DIRunSPretDs(Parms, "sp_SchedulerAgent");
                    b = new StringBuilder();
                    b.Append("<table><tr class='gheader'>");
                    tblWidth = 0;
                    if (sm.AdminAccess)
                    {
                        b.Append("<th style='width:20px;'>&nbsp;</th>");
                        tblWidth += 27;
                    }
                    b.Append("<th style='width:500px;'>Server Name</th><th style='width:80px;'>Active</th>");
                    tblWidth += 507 + 87;
                    if (sm.AdminAccess)
                    {
                        b.Append("<th style='width:20px;'>&nbsp;</th>");
                        tblWidth += 27;
                    }
                    b.AppendLine("<th style='width:16px;'>&nbsp;</th></tr></table><div style='height:370px; width:" +
                        tblWidth.ToString() + "px; overflow:auto;'><table>");
                    AltRow = false;
                    foreach (DataRow dr in dsAgent.Tables[0].Rows)
                    {
                        b.Append("<tr");
                        if (AltRow)
                        {
                            b.Append(" class='galtrow'");
                        }
                        b.Append(">");
                        if (sm.AdminAccess)
                        {
                            b.Append("<td style='width:20px;'>" +
                                    "<button name=\"edta" + dr["SchedulerAgentID"].ToString() + "\" type=\"submit\" " +
                                    " class='imagebutton' style=\"background: url(../Images/edit_16x16.gif) no-repeat;\"" +
                                    ">&nbsp;</button>" +
                                    "</td>");
                        }
                        b.Append("<td style='width:500px;'>" + dr["ServerName"].ToString() + "</td>");
                        b.Append("<td style='width:80px;text-align:center;'><input type = 'checkbox' disabled ");
                        if (bool.Parse(dr["IsActive"].ToString()))
                        {
                            b.Append("checked='checked' ");
                        }
                        b.Append("/></td>");
                        if (sm.AdminAccess)
                        {
                            b.Append("<td style='width:20px;'><button name=\"dela" + dr["SchedulerAgentID"].ToString() + "\" type=\"submit\" " +
                                " class='imagebutton' style=\"background: url(../Images/delete_16x16.gif) no-repeat;\"" +
                                ">&nbsp;</button></td>");
                        }
                        b.AppendLine("</tr>");
                        AltRow = !AltRow;
                    }
                    b.AppendLine("</table></div>");
                    sm.AgentList = b.ToString();
                }
                // Exclusion
                if (sm.ShowExclusion == "inline")
                {
                    Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "List"),
                            new DataClass.TParams("SchedulerAgentID", 4, SqlDbType.Int, sm.AgentID)
                    };
                    DataSet dsExclusion = db.DIRunSPretDs(Parms, "sp_SchedulerExclusion");
                    b = new StringBuilder();
                    b.Append("<table><tr class='gheader'>");
                    tblWidth = 0;
                    if (sm.AdminAccess)
                    {
                        b.Append("<th style='width:20px;'>&nbsp;</th>");
                        tblWidth += 27;
                    }
                    b.Append("<th style='width:100px;'>Sunday</th><th style='width:100px;'>Monday</th><th style='width:100px;'>Tuesday</th>" +
                        "<th style='width:100px;'>Wednesday</th><th style='width:100px;'>Thursday</th>" +
                        "<th style='width:100px;'>Friday</th><th style='width:100px;'>Saturday</th>" +
                        "<th style='width:80px;'>Active</th><th style='width:150px;'>Start</th>" +
                        "<th style='width:150px;'>End Time</th><th style='width:150px;'>Specific Date</th>");
                    tblWidth += 107 + 107 + 107 + 107 + 107 + 107 + 107 + 87 + 157 + 157 + 157;
                    if (sm.AdminAccess)
                    {
                        b.Append("<th style='width:20px;'>&nbsp;</th>");
                        tblWidth += 27;
                    }
                    b.AppendLine("<th style='width:16px;'>&nbsp;</th></tr></table><div style='height:370px; width:" +
                        tblWidth.ToString() + "px; overflow:auto;'><table>");
                    AltRow = false;
                    foreach (DataRow dr in dsExclusion.Tables[0].Rows)
                    {
                        b.Append("<tr");
                        if (AltRow)
                        {
                            b.Append(" class='galtrow'");
                        }
                        b.Append(">");
                        if (sm.AdminAccess)
                        {
                            b.Append("<td style='width:20px;'>" +
                                    "<button name=\"edte" + dr["SchedulerExclusionID"].ToString() + "\" type=\"submit\" " +
                                    " class='imagebutton' style=\"background: url(../Images/edit_16x16.gif) no-repeat;\"" +
                                    ">&nbsp;</button>" +
                                    "</td>");
                        }
                        b.Append("<td style='width:100px;text-align:center;'><input type = 'checkbox' disabled ");
                        if (bool.Parse(dr["Sunday"].ToString()))
                        {
                            b.Append("checked='checked' ");
                        }
                        b.Append("/></td>");
                        b.Append("<td style='width:100px;text-align:center;'><input type = 'checkbox' disabled ");
                        if (bool.Parse(dr["Monday"].ToString()))
                        {
                            b.Append("checked='checked' ");
                        }
                        b.Append("/></td>");
                        b.Append("<td style='width:100px;text-align:center;'><input type = 'checkbox' disabled ");
                        if (bool.Parse(dr["Tuesday"].ToString()))
                        {
                            b.Append("checked='checked' ");
                        }
                        b.Append("/></td>");
                        b.Append("<td style='width:100px;text-align:center;'><input type = 'checkbox' disabled ");
                        if (bool.Parse(dr["Wednesday"].ToString()))
                        {
                            b.Append("checked='checked' ");
                        }
                        b.Append("/></td>");
                        b.Append("<td style='width:100px;text-align:center;'><input type = 'checkbox' disabled ");
                        if (bool.Parse(dr["Thursday"].ToString()))
                        {
                            b.Append("checked='checked' ");
                        }
                        b.Append("/></td>");
                        b.Append("<td style='width:100px;text-align:center;'><input type = 'checkbox' disabled ");
                        if (bool.Parse(dr["Friday"].ToString()))
                        {
                            b.Append("checked='checked' ");
                        }
                        b.Append("/></td>");
                        b.Append("<td style='width:100px;text-align:center;'><input type = 'checkbox' disabled ");
                        if (bool.Parse(dr["Saturday"].ToString()))
                        {
                            b.Append("checked='checked' ");
                        }
                        b.Append("/></td>");
                        b.Append("<td style='width:80px;text-align:center;'><input type = 'checkbox' disabled ");
                        if (bool.Parse(dr["IsActive"].ToString()))
                        {
                            b.Append("checked='checked' ");
                        }
                        b.Append("/></td>");
                        b.Append("<td style='width:150px;'>" + dr["StartTime"].ToString() + "</td>" +
                            "<td style='width:150px;'>" + dr["EndTime"].ToString() + "</td>" +
                            "<td style='width:150px;'>");
                        if (dr["SpecificDate"].ToString() != string.Empty)
                        {
                            b.Append(DateTime.Parse(dr["SpecificDate"].ToString()).ToString("MM\\/dd\\/yyyy"));
                        }
                        b.Append("</td>");
                        if (sm.AdminAccess)
                        {
                            b.Append("<td style='width:20px;'><button name=\"dele" + dr["SchedulerExclusionID"].ToString() + "\" type=\"submit\" " +
                                " class='imagebutton' style=\"background: url(../Images/delete_16x16.gif) no-repeat;\"" +
                                ">&nbsp;</button></td>");
                        }
                        b.AppendLine("</tr>");
                        AltRow = !AltRow;
                    }
                    b.AppendLine("</table></div>");
                    sm.ExclusionList = b.ToString();
                }
                if (sm.ShowFolder == "inline")
                {
                    Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "List"),
                            new DataClass.TParams("SchedulerQueueID", 4, SqlDbType.Int, sm.QueueID)
                    };
                    DataSet dsFolder = db.DIRunSPretDs(Parms, "sp_SchedulerFolder");
                    b = new StringBuilder();
                    b.Append("<table><tr class='gheader'>");
                    tblWidth = 0;
                    if (sm.AdminAccess)
                    {
                        b.Append("<th style='width:20px;'>&nbsp;</th>");
                        tblWidth += 27;
                    }
                    b.Append("<th style='width:500px;'>Folder Name</th><th style='width:500px;'>Parent Folder</th><th style='width:100px;'>Sort Order</th>" +
                        "<th style='width:80px;'>Active</th>");
                    tblWidth += 507 + 507 + 107 + 87;
                    if (sm.AdminAccess)
                    {
                        b.Append("<th style='width:20px;'>&nbsp;</th>");
                        tblWidth += 27;
                    }
                    b.AppendLine("<th style='width:16px;'>&nbsp;</th></tr></table><div style='height:370px; width:" +
                        tblWidth.ToString() + "px; overflow:auto;'><table>");
                    AltRow = false;
                    foreach (DataRow dr in dsFolder.Tables[0].Rows)
                    {
                        b.Append("<tr");
                        if (AltRow)
                        {
                            b.Append(" class='galtrow'");
                        }
                        b.Append(">");
                        if (sm.AdminAccess)
                        {
                            b.Append("<td style='width:20px;'>" +
                                    "<button name=\"edtf" + dr["SchedulerFolderID"].ToString() + "\" type=\"submit\" " +
                                    " class='imagebutton' style=\"background: url(../Images/edit_16x16.gif) no-repeat;\"" +
                                    ">&nbsp;</button>" +
                                    "</td>");
                        }
                        b.Append("<td style='width:500px;'>" + dr["SchedulerFolderName"].ToString() + "</td>" +
                                "<td style='width:500px;'>" + dr["ParentFolderName"].ToString() + "</td>" +
                                "<td style='width:100px;'>" + dr["SortOrder"].ToString() + "</td>");
                        b.Append("<td style='width:80px;text-align:center;'><input type = 'checkbox' disabled ");
                        if (bool.Parse(dr["IsActive"].ToString()))
                        {
                            b.Append("checked='checked' ");
                        }
                        b.Append("/></td>");
                        if (sm.AdminAccess)
                        {
                            b.Append("<td style='width:20px;'><button name=\"delf" + dr["SchedulerFolderID"].ToString() + "\" type=\"submit\" " +
                                " class='imagebutton' style=\"background: url(../Images/delete_16x16.gif) no-repeat;\"" +
                                ">&nbsp;</button></td>");
                        }
                        b.AppendLine("</tr>");
                        AltRow = !AltRow;
                    }
                    b.AppendLine("</table></div>");
                    sm.FolderList = b.ToString();
                }
                if (sm.ShowVariable == "inline")
                {
                    Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "List"),
                            new DataClass.TParams("SchedulerFolderID", 4, SqlDbType.Int, sm.QueueID)
                    };
                    DataSet dsVariable = db.DIRunSPretDs(Parms, "sp_SchedulerVariable");
                    b = new StringBuilder();
                    b.Append("<table><tr class='gheader'>");
                    tblWidth = 0;
                    if (sm.AdminAccess)
                    {
                        b.Append("<th style='width:20px;'>&nbsp;</th>");
                        tblWidth += 27;
                    }
                    b.Append("<th style='width:500px;'>Variable Name</th><th style='width:500px;'>Value</th>" + 
                        "<th style='width:500px;'>Description</th><th style='width:100px;'>Sort Order</th>" +
                        "<th style='width:80px;'>Active</th>");
                    tblWidth += 507 + 507 + 507 + 107 + 87;
                    if (sm.AdminAccess)
                    {
                        b.Append("<th style='width:20px;'>&nbsp;</th>");
                        tblWidth += 27;
                    }
                    b.AppendLine("<th style='width:16px;'>&nbsp;</th></tr></table><div style='height:370px; width:" +
                        tblWidth.ToString() + "px; overflow:auto;'><table>");
                    AltRow = false;
                    foreach (DataRow dr in dsVariable.Tables[0].Rows)
                    {
                        b.Append("<tr");
                        if (AltRow)
                        {
                            b.Append(" class='galtrow'");
                        }
                        b.Append(">");
                        if (sm.AdminAccess)
                        {
                            b.Append("<td style='width:20px;'>" +
                                    "<button name=\"edtv" + dr["SchedulerVariableID"].ToString() + "\" type=\"submit\" " +
                                    " class='imagebutton' style=\"background: url(../Images/edit_16x16.gif) no-repeat;\"" +
                                    ">&nbsp;</button>" +
                                    "</td>");
                        }
                        b.Append("<td style='width:500px;'>" + dr["VariableName"].ToString() + "</td>" +
                                "<td style='width:500px;'>" + dr["VariableValue"].ToString() + "</td>" +
                                "<td style='width:500px;'>" + dr["VariableDescription"].ToString() + "</td>" +
                                "<td style='width:100px;'>" + dr["SortOrder"].ToString() + "</td>");
                        b.Append("<td style='width:80px;text-align:center;'><input type = 'checkbox' disabled ");
                        if (bool.Parse(dr["IsActive"].ToString()))
                        {
                            b.Append("checked='checked' ");
                        }
                        b.Append("/></td>");
                        if (sm.AdminAccess)
                        {
                            b.Append("<td style='width:20px;'><button name=\"delv" + dr["SchedulerVariableID"].ToString() + "\" type=\"submit\" " +
                                " class='imagebutton' style=\"background: url(../Images/delete_16x16.gif) no-repeat;\"" +
                                ">&nbsp;</button></td>");
                        }
                        b.AppendLine("</tr>");
                        AltRow = !AltRow;
                    }
                    b.AppendLine("</table></div>");
                    sm.VariableList = b.ToString();
                }
                if (sm.ShowJob == "inline")
                {
                    Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "List"),
                            new DataClass.TParams("SchedulerFolderID", 4, SqlDbType.Int, sm.FolderId)
                    };
                    DataSet dsJob = db.DIRunSPretDs(Parms, "sp_SchedulerJob");
                    b = new StringBuilder();
                    b.Append("<table><tr class='gheader'>");
                    tblWidth = 0;
                    if (sm.AdminAccess)
                    {
                        b.Append("<th style='width:20px;'>&nbsp;</th>");
                        tblWidth += 27;
                    }
                    b.Append("<th style='width:500px;'>Job Name</th><th style='width:500px;'>Operator</th><th style='width:100px;'>Sort Order</th>" +
                        "<th style='width:80px;'>Active</th><th style='width:100px;'>Scheduled</th>");
                    tblWidth += 507 + 507 + 107 + 87 + 107;
                    if (sm.AdminAccess)
                    {
                        b.Append("<th style='width:20px;'>&nbsp;</th>");
                        tblWidth += 27;
                    }
                    b.AppendLine("<th style='width:16px;'>&nbsp;</th></tr></table><div style='height:370px; width:" +
                        tblWidth.ToString() + "px; overflow:auto;'><table>");
                    AltRow = false;
                    foreach (DataRow dr in dsJob.Tables[0].Rows)
                    {
                        b.Append("<tr");
                        if (AltRow)
                        {
                            b.Append(" class='galtrow'");
                        }
                        b.Append(">");
                        if (sm.AdminAccess)
                        {
                            b.Append("<td style='width:20px;'>" +
                                    "<button name=\"edtj" + dr["SchedulerJobID"].ToString() + "\" type=\"submit\" " +
                                    " class='imagebutton' style=\"background: url(../Images/edit_16x16.gif) no-repeat;\"" +
                                    ">&nbsp;</button>" +
                                    "</td>");
                        }
                        b.Append("<td style='width:500px;'>" + dr["JobName"].ToString() + "</td>" +
                                "<td style='width:500px;'>" + dr["SchedulerOperatorName"].ToString() + "</td>" +
                                "<td style='width:100px;'>" + dr["SortOrder"].ToString() + "</td>");
                        b.Append("<td style='width:80px;text-align:center;'><input type = 'checkbox' disabled ");
                        if (bool.Parse(dr["IsActive"].ToString()))
                        {
                            b.Append("checked='checked' ");
                        }
                        b.Append("/></td><td style='width:100px;text-align:center;'><input type = 'checkbox' disabled ");
                        if (bool.Parse(dr["IsScheduled"].ToString()))
                        {
                            b.Append("checked='checked' ");
                        }
                        b.Append("/></td>");
                        if (sm.AdminAccess)
                        {
                            b.Append("<td style='width:20px;'><button name=\"delj" + dr["SchedulerJobID"].ToString() + "\" type=\"submit\" " +
                                " class='imagebutton' style=\"background: url(../Images/delete_16x16.gif) no-repeat;\"" +
                                ">&nbsp;</button></td>");
                        }
                        b.AppendLine("</tr>");
                        AltRow = !AltRow;
                    }
                    b.AppendLine("</table></div>");
                    sm.JobList = b.ToString();
                }
                if (sm.ShowInterval == "inline")
                {
                    Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "List")
                    };
                    DataSet dsInterval = db.DIRunSPretDs(Parms, "sp_SchedulerInterval");
                    b = new StringBuilder();
                    b.Append("<table><tr class='gheader'>");
                    tblWidth = 0;
                    if (sm.AdminAccess)
                    {
                        b.Append("<th style='width:20px;'>&nbsp;</th>");
                        tblWidth += 27;
                    }
                    b.Append("<th style='width:500px;'>Interval Name</th><th style='width:500px;'>Interval Type</th><th style='width:100px;'>Interval</th>" +
                        "<th style='width:80px;'>Active</th>");
                    tblWidth += 507 + 507 + 107 + 87;
                    if (sm.AdminAccess)
                    {
                        b.Append("<th style='width:20px;'>&nbsp;</th>");
                        tblWidth += 27;
                    }
                    b.AppendLine("<th style='width:16px;'>&nbsp;</th></tr></table><div style='height:370px; width:" +
                        tblWidth.ToString() + "px; overflow:auto;'><table>");
                    AltRow = false;
                    foreach (DataRow dr in dsInterval.Tables[0].Rows)
                    {
                        b.Append("<tr");
                        if (AltRow)
                        {
                            b.Append(" class='galtrow'");
                        }
                        b.Append(">");
                        if (sm.AdminAccess)
                        {
                            b.Append("<td style='width:20px;'>" +
                                    "<button name=\"edti" + dr["SchedulerIntervalID"].ToString() + "\" type=\"submit\" " +
                                    " class='imagebutton' style=\"background: url(../Images/edit_16x16.gif) no-repeat;\"" +
                                    ">&nbsp;</button>" +
                                    "</td>");
                        }
                        b.Append("<td style='width:500px;'>" + dr["IntervalName"].ToString() + "</td>" +
                                "<td style='width:500px;'>" + dr["IntervalType"].ToString() + "</td>" +
                                "<td style='width:100px;'>" + dr["Interval"].ToString() + "</td>");
                        b.Append("<td style='width:80px;text-align:center;'><input type = 'checkbox' disabled ");
                        if (bool.Parse(dr["IsActive"].ToString()))
                        {
                            b.Append("checked='checked' ");
                        }
                        b.Append("/></td>");
                        if (sm.AdminAccess)
                        {
                            b.Append("<td style='width:20px;'><button name=\"deli" + dr["SchedulerIntervalID"].ToString() + "\" type=\"submit\" " +
                                " class='imagebutton' style=\"background: url(../Images/delete_16x16.gif) no-repeat;\"" +
                                ">&nbsp;</button></td>");
                        }
                        b.AppendLine("</tr>");
                        AltRow = !AltRow;
                    }
                    b.AppendLine("</table></div>");
                    sm.IntervalList = b.ToString();
                }
                if (sm.ShowTask == "inline")
                {
                    Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "List"),
                            new DataClass.TParams("SchedulerJobID", 4, SqlDbType.Int, sm.JobId)
                    };
                    DataSet dsTask = db.DIRunSPretDs(Parms, "sp_SchedulerTask");
                    b = new StringBuilder();
                    b.Append("<table><tr class='gheader'>");
                    tblWidth = 0;
                    if (sm.AdminAccess)
                    {
                        b.Append("<th style='width:20px;'>&nbsp;</th>");
                        tblWidth += 27;
                    }
                    b.Append("<th style='width:500px;'>Task Name</th><th style='width:500px;'>Task Type</th><th style='width:100px;'>Sort Order</th>" +
                        "<th style='width:80px;'>Active</th>");
                    tblWidth += 507 + 507 + 107 + 87;
                    if (sm.AdminAccess)
                    {
                        b.Append("<th style='width:20px;'>&nbsp;</th>");
                        tblWidth += 27;
                    }
                    b.AppendLine("<th style='width:16px;'>&nbsp;</th></tr></table><div style='height:200px; width:" +
                        tblWidth.ToString() + "px; overflow:auto;'><table>");
                    AltRow = false;
                    foreach (DataRow dr in dsTask.Tables[0].Rows)
                    {
                        b.Append("<tr");
                        if (AltRow)
                        {
                            b.Append(" class='galtrow'");
                        }
                        b.Append(">");
                        if (sm.AdminAccess)
                        {
                            b.Append("<td style='width:20px;'>" +
                                    "<button name=\"edtt" + dr["SchedulerTaskID"].ToString() + "\" type=\"submit\" " +
                                    " class='imagebutton' style=\"background: url(../Images/edit_16x16.gif) no-repeat;\"" +
                                    ">&nbsp;</button>" +
                                    "</td>");
                        }
                        b.Append("<td style='width:500px;'>" + dr["TaskName"].ToString() + "</td>" +
                                "<td style='width:500px;'>" + dr["TaskTypeName"].ToString() + "</td>" +
                                "<td style='width:100px;'>" + dr["SortOrder"].ToString() + "</td>");
                        b.Append("<td style='width:80px;text-align:center;'><input type = 'checkbox' disabled ");
                        if (bool.Parse(dr["IsActive"].ToString()))
                        {
                            b.Append("checked='checked' ");
                        }
                        b.Append("/></td>");
                        if (sm.AdminAccess)
                        {
                            b.Append("<td style='width:20px;'><button name=\"delt" + dr["SchedulerTaskID"].ToString() + "\" type=\"submit\" " +
                                " class='imagebutton' style=\"background: url(../Images/delete_16x16.gif) no-repeat;\"" +
                                ">&nbsp;</button></td>");
                        }
                        b.AppendLine("</tr>");
                        AltRow = !AltRow;
                    }
                    b.AppendLine("</table></div>");
                    sm.TaskList = b.ToString();
                }
            }

            return View(sm);
        }
    }
}