using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Task
{
    public int JobID { get; set; }
    public int TaskID { get; set; }
    public string TaskInformation { get; set; }
    public DateTime NextRunTime { get; set; }
    public int MaxInstances { get; set; }
    public int MaxMinutes { get; set; }
    public string TaskTypeName { get; set; }
    public string TaskTypeCode { get; set; }
    public int? SchedulerIntervalID { get; set; }
    public int? SchedulerJobLogId { get; set; }

    public Task(string JobID, string TaskID, string TaskInformation, string NextRunTime, string MaxInstances, string MaxMinutes,
                 string TaskTypeName, string TaskTypeCode, int? SchedulerIntervalID)
    {
        this.JobID = int.Parse(JobID);
        this.TaskID = int.Parse(TaskID);
        this.TaskInformation = TaskInformation;
        this.NextRunTime = DateTime.Now;
        if (NextRunTime != string.Empty)
        {
            this.NextRunTime = DateTime.Parse(NextRunTime);
        }
        this.MaxInstances = int.Parse(MaxInstances);
        this.MaxMinutes = int.Parse(MaxMinutes);
        this.TaskTypeName = TaskTypeName;
        this.TaskTypeCode = TaskTypeCode;
        this.SchedulerIntervalID = 0;
        this.SchedulerIntervalID = SchedulerIntervalID;
    }
}
