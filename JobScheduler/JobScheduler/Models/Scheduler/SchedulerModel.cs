using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

public class SchedulerModel : HomeModel
{
    public string ShowQueue { get; set; }
    public string ShowAgent { get; set; }
    public string ShowQueueButtons { get; set; }
    public string ShowAgentButtons { get; set; }
    public string ShowExclusion { get; set; }
    public string ShowFolder { get; set; }
    public string ShowFolderButtons { get; set; }
    public string ShowJob { get; set; }
    public string ShowJobButtons { get; set; }
    public string ShowInterval { get; set; }
    public string ShowTask { get; set; }
    public string ShowSaveButtons { get; set; }
    public string ShowVariable { get; set; }

    // Queue Management
    public string QueueID { get; set; }
    public string QueueName { get; set; }
    public bool QueueIsActive { get; set; }
    public bool QueueIsRunning { get; set; }
    public int MaxThreads { get; set; }
    public string MaxMinutes { get; set; }
    public string QueueList { get; set; }
    public string QueueMessage { get; set; }

    // Agent Management
    public string AgentID { get; set; }
    public string ServerID { get; set; }
    public bool AgentIsActive { get; set; }
    public string AgentList { get; set; }
    public string AgentMessage { get; set; }

    // Exclusion Management
    public string ExclusionID { get; set; }
    public bool ExclusionIsActive { get; set; }
    public bool Sunday { get; set; }
    public bool Monday { get; set; }
    public bool Tuesday { get; set; }
    public bool Wednesday { get; set; }
    public bool Thursday { get; set; }
    public bool Friday { get; set; }
    public bool Saturday { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public string SpecificDate { get; set; }
    public string ExclusionList { get; set; }
    public string ExclusionMessage { get; set; }
    public bool IsGlobal { get; set; }

    // Folder Management
    public string FolderId { get; set; }
    public string FolderName { get; set; }
    public string ParentFolderId { get; set; }
    public bool FolderIsActive { get; set; }
    public int FolderSortOrder { get; set; }
    public string FolderList { get; set; }
    public string FolderMessage { get; set; }

    // Variable Management
    public string VariableId { get; set; }
    public string VariableName { get; set; }
    public string VariableValue { get; set; }
    public string VariableDescription { get; set; }
    public bool VariableIsActive { get; set; }
    public int VariableSortOrder { get; set; }
    public string VariableList { get; set; }
    public string VariableMessage { get; set; }

    // Job Management
    public string JobId { get; set; }
    public string JobName { get; set; }
    public bool JobIsActive { get; set; }
    public int JobSortOrder { get; set; }
    public string JobIntervalId { get; set; }
    public string JobAgentId { get; set; }
    public string JobOperatorId { get; set; }
    public string LastRunTime { get; set; }
    public string NextRunTime { get; set; }
    public string MaxInstances { get; set; }
    public bool JobIsScheduled { get; set; }
    public string JobList { get; set; }
    public string JobMessage { get; set; }

    // Interval Management
    public string IntervalId { get; set; }
    public string IntervalName { get; set; }
    public int Interval { get; set; }
    public bool IntervalIsActive { get; set; }
    public string Occurrences { get; set; }
    public string IntervalType { get; set; }
    public string IntervalStartTime { get; set; }
    public string IntervalEndTime { get; set; }
    public string IntervalExclusionStart { get; set; }
    public string IntervalExclusionEnd { get; set; }
    public string IntervalRepeatMinutes { get; set; }
    public string IntervalDetails { get; set; }
    public string IntervalList { get; set; }
    public string IntervalMessage { get; set; }

    // Task Management
    public string TaskId { get; set; }
    public string TaskName { get; set; }
    public bool TaskIsActive { get; set; }
    public int TaskSortOrder { get; set; }
    public string TaskTypeId { get; set; }
    public string TaskInformation { get; set; }
    public string TaskList { get; set; }
    public string TaskMessage { get; set; }

    // Task Type Specific data
    public string ShowExecuteProgram { get; set; }
    public string ExecutePath { get; set; }
    public string ExecuteCommand { get; set; }
    public string ExecuteParameters { get; set; }

    public string ShowSQLTask { get; set; }
    public string SQLConnection { get; set; }
    public string SQLCommand { get; set; }

    public string ShowSPTask { get; set; }
    public string SPConnection { get; set; }
    public string SPName { get; set; }
    public string SPMessage { get; set; }
    public string SPParameter { get; set; }
    public string SPDataType { get; set; }
    public string SPValue { get; set; }
    public string SPParameters { get; set; }
    public string SPList { get; set; }

    public string ShowMailTask { get; set; }
    public string MailFrom { get; set; }
    public string MailSubject { get; set; }
    public string MailTo { get; set; }
    public string MailBody { get; set; }

    public string ShowFileCopyTask { get; set; }
    public string FileCopySource { get; set; }
    public string FileCopyDestination { get; set; }
    public string FileCopyFiles { get; set; }

    public string ShowScriptTask { get; set; }
    public string ScriptCode { get; set; }

    public string ShowDTS { get; set; }
    public string DTSPath { get; set; }

    public string ShowDLL { get; set; }
    public string DLLPath { get; set; }
    public string DLLClass { get; set; }
    public string DLLMethod { get; set; }

    public List<SelectListItem> Operators { get; set; }
    public List<SelectListItem> TaskTypes { get; set; }
    public List<SelectListItem> Intervals { get; set; }
    public List<SelectListItem> ParentFolders { get; set; }
    public List<SelectListItem> Servers { get; set; }
    public List<SelectListItem> Agents { get; set; }
    public List<SelectListItem> IntervalTypes { get; set; }
    public List<SelectListItem> DataTypes { get; set; }
}