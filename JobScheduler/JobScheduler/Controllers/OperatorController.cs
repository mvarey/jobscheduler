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
    public class OperatorController : Controller
    {
        public HomeModel hm = new HomeModel();
        SetupPage BasePage = new SetupPage();
        DataClass db = new DataClass(ConfigurationManager.ConnectionStrings["JobScheduler"].ConnectionString);
        DataClass.TParams[] Parms;

        // GET: Operator
        public ActionResult Dashboard()
        {
            HomeModel h = BasePage.SetupPageBasics(Request, hm, Session);
            object g = new DashboardModel();
            Utility.UpdateModel(h, ref g);
            DashboardModel dm = (DashboardModel)g;
            dm.ShowDashboard = "none";
            dm.ScheduledJobs = string.Empty;
            dm.ActiveJobs = string.Empty;
            dm.HistoryJobs = string.Empty;
            dm.ErrorJobs = string.Empty;

            if (dm.AdminAccess || dm.OperatorAccess)
            {
                dm.ShowDashboard = "inline";
                // Get List of Jobs that are scheduled
                Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "GetSched") };
                DataSet dsJobs = db.DIRunSPretDs(Parms, "sp_SchedulerJob");
                StringBuilder b = new StringBuilder();
                foreach(DataRow dr in dsJobs.Tables[0].Rows)
                {
                    DateTime NextRunTime = DateTime.Parse(dr["NextRunTime"].ToString());
                    DateTime FinalTime = NextRunTime.AddMinutes(10);
                    if (b.Length > 0)
                    {
                        b.Append("," + Environment.NewLine);
                    }
                    b.AppendLine("[ '" + dr["JobName"].ToString() + "', new Date(" + NextRunTime.Year.ToString() + "," + (NextRunTime.Month - 1).ToString() + "," + NextRunTime.Day.ToString() +
                                "," + NextRunTime.Hour.ToString() + "," + NextRunTime.Minute + "," + NextRunTime.Second.ToString() + "), new Date(" +
                                FinalTime.Year.ToString() + "," + (FinalTime.Month - 1).ToString() + "," + FinalTime.Day.ToString() + "," +
                                FinalTime.Hour.ToString() + "," + FinalTime.Minute.ToString() + "," + FinalTime.Second.ToString() + ") ]");
                }
                if (b.Length > 0)
                {
                    b.Append("," + Environment.NewLine);
                }
                b.AppendLine("[ '.', new Date(" + DateTime.Now.Year.ToString() + "," + (DateTime.Now.Month - 1).ToString() + "," + DateTime.Now.Day.ToString() + "," +
                            "23,59,59), new Date(" + DateTime.Now.Year.ToString() + "," + (DateTime.Now.Month - 1).ToString() + "," + DateTime.Now.Day.ToString() + "," +
                            "23,59,59) ]");
                dm.ScheduledJobs = b.ToString();

                Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Active") };
                DataSet dsActive = db.DIRunSPretDs(Parms, "sp_SchedulerJobLog");
                b = new StringBuilder();
                foreach (DataRow dr in dsActive.Tables[0].Rows)
                {
                    DateTime NextRunTime = DateTime.Parse(dr["ExecutionStart"].ToString());
                    DateTime FinalTime = DateTime.Now;
                    if (b.Length > 0)
                    {
                        b.Append("," + Environment.NewLine);
                    }
                    b.AppendLine("[ '" + dr["JobName"].ToString() + "', new Date(" + NextRunTime.Year.ToString() + "," + (NextRunTime.Month - 1).ToString() + "," + NextRunTime.Day.ToString() +
                                "," + NextRunTime.Hour.ToString() + "," + NextRunTime.Minute + "," + NextRunTime.Second.ToString() + "), new Date(" +
                                FinalTime.Year.ToString() + "," + (FinalTime.Month - 1).ToString() + "," + FinalTime.Day.ToString() + "," +
                                FinalTime.Hour.ToString() + "," + FinalTime.Minute.ToString() + "," + FinalTime.Second.ToString() + ") ]");
                }
                dm.ActiveJobs = b.ToString();

                Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "History") };
                DataSet dsHistory = db.DIRunSPretDs(Parms, "sp_SchedulerJobLog");
                b = new StringBuilder();
                foreach (DataRow dr in dsHistory.Tables[0].Rows)
                {
                    DateTime NextRunTime = DateTime.Parse(dr["ExecutionStart"].ToString());
                    DateTime FinalTime = DateTime.Parse(dr["ExecutionEnd"].ToString());
                    if (b.Length > 0)
                    {
                        b.Append("," + Environment.NewLine);
                    }
                    b.AppendLine("[ '" + dr["JobName"].ToString() + "', new Date(" + NextRunTime.Year.ToString() + "," + (NextRunTime.Month - 1).ToString() + "," + NextRunTime.Day.ToString() +
                                "," + NextRunTime.Hour.ToString() + "," + NextRunTime.Minute + "," + NextRunTime.Second.ToString() + "), new Date(" +
                                FinalTime.Year.ToString() + "," + (FinalTime.Month - 1).ToString() + "," + FinalTime.Day.ToString() + "," +
                                FinalTime.Hour.ToString() + "," + FinalTime.Minute.ToString() + "," + FinalTime.Second.ToString() + ") ]");
                }
                dm.HistoryJobs = b.ToString();

                Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Error") };
                DataSet dsError = db.DIRunSPretDs(Parms, "sp_SchedulerJobLog");
                b = new StringBuilder();
                foreach (DataRow dr in dsError.Tables[0].Rows)
                {
                    DateTime NextRunTime = DateTime.Parse(dr["ExecutionStart"].ToString());
                    DateTime FinalTime = DateTime.Parse(dr["ExecutionEnd"].ToString());
                    if (b.Length > 0)
                    {
                        b.Append("," + Environment.NewLine);
                    }
                    b.AppendLine("[ '" + dr["JobName"].ToString() + "', new Date(" + NextRunTime.Year.ToString() + "," + (NextRunTime.Month - 1).ToString() + "," + NextRunTime.Day.ToString() +
                                "," + NextRunTime.Hour.ToString() + "," + NextRunTime.Minute + "," + NextRunTime.Second.ToString() + "), new Date(" +
                                FinalTime.Year.ToString() + "," + (FinalTime.Month - 1).ToString() + "," + FinalTime.Day.ToString() + "," +
                                FinalTime.Hour.ToString() + "," + FinalTime.Minute.ToString() + "," + FinalTime.Second.ToString() + ") ]");
                }
                dm.ErrorJobs = b.ToString();

            }
            return View(dm);
        }

        public ActionResult JobHistory()
        {
            HomeModel h = BasePage.SetupPageBasics(Request, hm, Session);
            object g = new HistoryModel();
            Utility.UpdateModel(h, ref g);
            HistoryModel jh = (HistoryModel)g;
            jh.ShowHistory = "none";
            jh.ExecutedBy = string.Empty;
            jh.ExecutionEnd = string.Empty;
            jh.ExecutionScheduled = string.Empty;
            jh.ExecutionStart = string.Empty;
            jh.FolderID = string.Empty;
            jh.Folders = new List<SelectListItem>();
            jh.HistoryMessage = string.Empty;
            jh.JobHistoryList = string.Empty;
            jh.JobID = string.Empty;
            jh.Jobs = new List<SelectListItem>();
            jh.QueueID = string.Empty;
            jh.Queues = new List<SelectListItem>();
            jh.ServerName = string.Empty;
            jh.ShowJobInfo = "none";
            jh.ShowTaskInfo = "none";
            jh.StandardError = string.Empty;
            jh.StandardOutput = string.Empty;
            jh.TaskCommandLine = string.Empty;
            jh.TaskExecutionEnd = string.Empty;
            jh.TaskExecutionStart = string.Empty;
            jh.TaskHistoryList = string.Empty;
            jh.TaskName = string.Empty;
            jh.TaskScheduledStart = string.Empty;
            jh.TaskType = string.Empty;
            jh.JobLogID = string.Empty;

            if (jh.AdminAccess || jh.OperatorAccess || jh.ViewAccess || jh.ReportAccess)
            {
                jh.ShowHistory = "inline";
                if (Request.Params["QueueID"] != null)
                {
                    jh.QueueID = Request.Params["QueueID"].ToString();
                }
                if (Request.Params["FolderID"] != null)
                {
                    jh.FolderID = Request.Params["FolderID"].ToString();
                }
                if (Request.Params["JobLogID"] != null)
                {
                    jh.JobLogID = Request.Params["JobLogID"].ToString();
                }
                if (Request.Params["JobID"] != null)
                {
                    jh.JobID = Request.Params["JobID"].ToString();
                }
                if (Request.Params["ShowJobInfo"] != null)
                {
                    jh.ShowJobInfo = Request.Params["ShowJobInfo"].ToString();
                }
                if (Request.Params["ShowTaskInfo"] != null)
                {
                    jh.ShowTaskInfo = Request.Params["ShowTaskInfo"].ToString();
                }
                string viewjob = Utility.GetParam(Request.Params.ToString(), "viewjob");
                string viewtask = Utility.GetParam(Request.Params.ToString(), "viewtask");

                if (viewjob == string.Empty && jh.JobLogID != string.Empty)
                {
                    viewjob = "viewjob" + jh.JobLogID;
                }
                if (viewjob != string.Empty)
                {
                    string SchedulerJobLogId = viewjob.Substring(7);
                    jh.JobLogID = SchedulerJobLogId;
                    if (SchedulerJobLogId != string.Empty)
                    {
                        Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Get"),
                                new DataClass.TParams("SchedulerJobLogId", 4, SqlDbType.Int, SchedulerJobLogId)
                        };
                        DataSet dsJobLog = db.DIRunSPretDs(Parms, "sp_SchedulerJobLog");
                        if (dsJobLog.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = dsJobLog.Tables[0].Rows[0];
                            jh.ExecutionScheduled = DateTime.Parse(dr["ExecutionScheduled"].ToString()).ToString("MMM dd, yyyy hh:mm:ss tt");
                            if (dr["ExecutionStart"].ToString() != string.Empty)
                            {
                                jh.ExecutionStart = DateTime.Parse(dr["ExecutionStart"].ToString()).ToString("MMM dd, yyyy hh:mm:ss tt");
                            }
                            if (dr["ExecutionEnd"].ToString() != string.Empty)
                            {
                                jh.ExecutionEnd = DateTime.Parse(dr["ExecutionEnd"].ToString()).ToString("MMM dd, yyyy hh:mm:ss tt");
                            }
                            if (dr["SchedulerUserId"].ToString() != string.Empty)
                            {
                                int UserID = int.Parse(dr["SchedulerUserId"].ToString());
                                Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Get"),
                                        new DataClass.TParams("SchedulerUserId", 4, SqlDbType.Int, UserID)
                                };
                                DataSet dsUser = db.DIRunSPretDs(Parms, "sp_SchedulerUser");
                                if (dsUser.Tables[0].Rows.Count > 0)
                                {
                                    DataRow drUser = dsUser.Tables[0].Rows[0];
                                    jh.ExecutedBy = drUser["UserFirstName"].ToString() + " " + drUser["UserLastName"].ToString();
                                }
                            }
                            jh.ShowJobInfo = "inline";
                        }
                    }
                }

                if (Request.Params["btnShowTasks"] != null)
                {
                    jh.ShowTaskInfo = "inline";
                }
                if (Request.Params["btnTaskClose"] != null)
                {
                    jh.ShowTaskInfo = "none";
                }

                if (jh.ShowTaskInfo == "inline")
                {
                    // query for task logs that are part of the selected job log
                    Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "GetByJob"),
                            new DataClass.TParams("SchedulerJobLogId", 4, SqlDbType.Int, "0" + jh.JobLogID)
                    };
                    DataSet dsTaskLog = db.DIRunSPretDs(Parms, "sp_SchedulerTaskLog");
                    StringBuilder b = new StringBuilder();
                    bool AltRow = false;
                    b.Append("<table><tr class='gheader'>");
                    int tblWidth = 0;
                    b.Append("<th style='width:20px;'>&nbsp;</th>");
                    tblWidth += 27;
                    b.Append("<th style='width:200px;'>Scheduled Time</th><th style='width:200px;'>Started At</th><th style='width:200px;'>Ended At</th><th style='width:300px;'>Server</th>");
                    tblWidth += 207 + 207 + 207 + 307;
                    b.AppendLine("<th style='width:16px;'>&nbsp;</th></tr></table><div style='height:370px; width:" +
                        tblWidth.ToString() + "px; overflow:auto;'><table>");
                    foreach (DataRow drLog in dsTaskLog.Tables[0].Rows)
                    {
                        b.Append("<tr");
                        if (AltRow)
                        {
                            b.Append(" class='galtrow'");
                        }
                        b.Append(">");
                        b.Append("<td style='width:20px;'>" +
                                "<button name=\"viewtask" + drLog["SchedulerTaskLogId"].ToString() + "\" type=\"submit\" " +
                                " class='imagebutton' style=\"background: url(../Images/edit_16x16.gif) no-repeat;\"" +
                                ">&nbsp;</button>" +
                                "</td>");
                        b.Append("<td style='width:200px;'>" + DateTime.Parse(drLog["ExecutionScheduled"].ToString()).ToString("MMM dd, yyyy hh:mm:ss tt") + "</td>");
                        string ExecutionStart = string.Empty;
                        if (drLog["ExecutionScheduled"].ToString() != string.Empty)
                        {
                            ExecutionStart = DateTime.Parse(drLog["ExecutionScheduled"].ToString()).ToString("MMM dd, yyyy hh:mm:ss tt");
                        }
                        b.Append("<td style='width:200px;'>" + ExecutionStart + "</td>");
                        string ExecutionEnd = string.Empty;
                        if (drLog["ExecutionEnd"].ToString() != string.Empty)
                        {
                            ExecutionEnd = DateTime.Parse(drLog["ExecutionEnd"].ToString()).ToString("MMM dd, yyyy hh:mm:ss tt");
                        }
                        b.Append("<td style='width:200px;'>" + ExecutionEnd + "</td>");
                        b.Append("<td style='width:300px;'>" + drLog["ServerName"].ToString() + "</td></tr>");
                    }
                    b.Append("</table></div>");
                    jh.TaskHistoryList = b.ToString();
                }

                Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "List") };
                DataSet dsQueue = db.DIRunSPretDs(Parms, "sp_SchedulerQueue");
                foreach(DataRow dr in dsQueue.Tables[0].Rows)
                {
                    jh.Queues.Add(new SelectListItem() { Text = dr["QueueName"].ToString(), Value = dr["SchedulerQueueID"].ToString() });
                    if (jh.QueueID == string.Empty)
                    {
                        jh.QueueID = dr["SchedulerQueueID"].ToString();
                    }
                }

                if (jh.QueueID != string.Empty)
                {
                    Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "List"),
                            new DataClass.TParams("SchedulerQueueID", 4, SqlDbType.Int, jh.QueueID)
                    };
                    DataSet dsFolder = db.DIRunSPretDs(Parms, "sp_SchedulerFolder");
                    foreach(DataRow drf in dsFolder.Tables[0].Rows)
                    {
                        jh.Folders.Add(new SelectListItem() { Text = drf["SchedulerFolderName"].ToString(), Value = drf["SchedulerFolderID"].ToString() });
                        if (jh.FolderID == string.Empty)
                        {
                            jh.FolderID = drf["SchedulerFolderID"].ToString();
                        }
                    }
                }

                if (jh.FolderID != string.Empty)
                {
                    Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "List"),
                            new DataClass.TParams("SchedulerFolderID", 4, SqlDbType.Int, jh.FolderID)
                    };
                    DataSet dsJobs = db.DIRunSPretDs(Parms, "sp_SchedulerJob");
                    foreach(DataRow drj in dsJobs.Tables[0].Rows)
                    {
                        jh.Jobs.Add(new SelectListItem() { Text = drj["JobName"].ToString(), Value = drj["SchedulerJobID"].ToString() });
                        if (jh.JobID == string.Empty)
                        {
                            jh.JobID = drj["SchedulerJobID"].ToString();
                        }
                    }
                }

                if (jh.JobID != string.Empty)
                {
                    Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "JobHistory"),
                                new DataClass.TParams("SchedulerJobId", 4, SqlDbType.Int, jh.JobID)
                        };
                    DataSet dsJob = db.DIRunSPretDs(Parms, "sp_SchedulerJobLog");
                    StringBuilder b = new StringBuilder();
                    bool AltRow = false;
                    b.Append("<table><tr class='gheader'>");
                    int tblWidth = 0;
                    b.Append("<th style='width:20px;'>&nbsp;</th>");
                    tblWidth += 27;
                    b.Append("<th style='width:200px;'>Scheduled Time</th><th style='width:200px;'>Started At</th><th style='width:200px;'>Ended At</th><th style='width:300px;'>Run By</th>");
                    tblWidth += 207 + 207 + 207 + 307;
                    b.AppendLine("<th style='width:16px;'>&nbsp;</th></tr></table><div style='height:370px; width:" +
                        tblWidth.ToString() + "px; overflow:auto;'><table>");
                    foreach (DataRow drLog in dsJob.Tables[0].Rows)
                    {
                        b.Append("<tr");
                        if (AltRow)
                        {
                            b.Append(" class='galtrow'");
                        }
                        b.Append(">");
                        b.Append("<td style='width:20px;'>" +
                                "<button name=\"viewjob" + drLog["SchedulerJobLogId"].ToString() + "\" type=\"submit\" " +
                                " class='imagebutton' style=\"background: url(../Images/edit_16x16.gif) no-repeat;\"" +
                                ">&nbsp;</button>" +
                                "</td>");
                        b.Append("<td style='width:200px;'>" + DateTime.Parse(drLog["ExecutionScheduled"].ToString()).ToString("MMM dd, yyyy hh:mm:ss tt") + "</td>");
                        string ExecutionStart = string.Empty;
                        if (drLog["ExecutionScheduled"].ToString() != string.Empty)
                        {
                            ExecutionStart = DateTime.Parse(drLog["ExecutionScheduled"].ToString()).ToString("MMM dd, yyyy hh:mm:ss tt");
                        }
                        b.Append("<td style='width:200px;'>" + ExecutionStart + "</td>");
                        string ExecutionEnd = string.Empty;
                        if (drLog["ExecutionEnd"].ToString() != string.Empty)
                        {
                            ExecutionEnd = DateTime.Parse(drLog["ExecutionEnd"].ToString()).ToString("MMM dd, yyyy hh:mm:ss tt");
                        }
                        b.Append("<td style='width:200px;'>" + ExecutionEnd + "</td>");
                        int UserID = 0;
                        string ExecutedBy = string.Empty;
                        if (drLog["SchedulerUserId"].ToString() != string.Empty)
                        {
                            int.Parse(drLog["SchedulerUserId"].ToString());
                            Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Get"),
                                        new DataClass.TParams("SchedulerUserId", 4, SqlDbType.Int, UserID)
                                };
                            DataSet dsUser = db.DIRunSPretDs(Parms, "sp_SchedulerUser");
                            if (dsUser.Tables[0].Rows.Count > 0)
                            {
                                DataRow drUser = dsUser.Tables[0].Rows[0];
                                ExecutedBy = drUser["UserFirstName"].ToString() + " " + drUser["UserLastName"].ToString();
                            }
                        }
                        b.Append("<td style='width:300px;'>" + ExecutedBy + "</td></tr>");
                    }
                    b.Append("</table></div>");
                    jh.JobHistoryList = b.ToString();
                }

                if (viewtask.Length > 0)
                {
                    Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Get"),
                            new DataClass.TParams("SchedulerTaskLogId", 4, SqlDbType.Int, viewtask.Substring(8))
                    };
                    DataSet dsTaskLog = db.DIRunSPretDs(Parms, "sp_SchedulerTaskLog");
                    if (dsTaskLog.Tables[0].Rows.Count > 0)
                    {
                        DataRow drLog = dsTaskLog.Tables[0].Rows[0];
                        jh.TaskCommandLine = drLog["ExecutionCommandLine"].ToString();
                        if (drLog["ExecutionEnd"].ToString() != string.Empty)
                        {
                            jh.TaskExecutionEnd = DateTime.Parse(drLog["ExecutionEnd"].ToString()).ToString("MMM dd, yyyy hh:mm:ss tt");
                        }
                        if (drLog["ExecutionStart"].ToString() != string.Empty)
                        {
                            jh.TaskExecutionStart = DateTime.Parse(drLog["ExecutionStart"].ToString()).ToString("MMM dd, yyyy hh:mm:ss tt");
                        }
                        jh.TaskName = drLog["TaskName"].ToString();
                        if (drLog["ScheduledStart"].ToString() != string.Empty)
                        {
                            jh.TaskScheduledStart = DateTime.Parse(drLog["ScheduledStart"].ToString()).ToString("MMM dd, yyyy hh:mm:ss tt");
                        }
                        jh.ServerName = drLog["ServerName"].ToString();
                        jh.TaskName = drLog["TaskName"].ToString();
                        jh.TaskType = drLog["TaskTypeName"].ToString();
                        jh.StandardOutput = drLog["StandardOutput"].ToString();
                        jh.StandardError = drLog["StandardError"].ToString();
                    }
                }
            }

            return View(jh);
        }
    }
}