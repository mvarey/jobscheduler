using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

public class SchedulerServerModel : HomeModel
{
    public string ShowServer { get; set; }
    public string ServerMessage { get; set; }
    public string ServerID { get; set; }
    public string ServerName { get; set; }
    public bool IsActive { get; set; }
    public bool AgentEnabled { get; set; }
    public int MaxThreads { get; set; }
    public string MailServer { get; set; }
    public string MailID { get; set; }
    public string MailPassword { get; set; }
    public string MachineIP4Address { get; set; }
    public string MachineIP6Address { get; set; }
    public string ServerList { get; set; }
}