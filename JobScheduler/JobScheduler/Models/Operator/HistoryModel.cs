using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

public class HistoryModel : HomeModel
{
    public string ShowHistory { get; set; }
    public string HistoryMessage { get; set; }
    public string QueueID { get; set; }
    public string FolderID { get; set; }
    public string JobID { get; set; }

    public List<SelectListItem> Queues { get; set; }
    public List<SelectListItem> Folders { get; set; }
    public List<SelectListItem> Jobs { get; set; }

    public string JobHistoryList { get; set; }

    public string ShowJobInfo { get; set; }
    public string ExecutionScheduled { get; set; }
    public string ExecutionStart { get; set; }
    public string ExecutionEnd { get; set; }
    public string ExecutedBy { get; set; }
    public string JobLogID { get; set; }

    public string TaskHistoryList { get; set; }

    public string ShowTaskInfo { get; set; }
    public string ServerName { get; set; }
    public string TaskName { get; set; }
    public string TaskType { get; set; }
    public string StandardOutput { get; set; }
    public string StandardError { get; set; }
    public string TaskExecutionStart { get; set; }
    public string TaskExecutionEnd { get; set; }
    public string TaskCommandLine { get; set; }
    public string TaskScheduledStart { get; set; }

}