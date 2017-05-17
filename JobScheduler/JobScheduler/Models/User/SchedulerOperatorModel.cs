using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

public class SchedulerOperatorModel : HomeModel
{
    public string ShowOperators { get; set; }
    public string OperatorMessage { get; set; }
    public string OperatorID { get; set; }
    public string OperatorName { get; set; }
    public bool OperatorIsActive { get; set; }
    public string OperatorList { get; set; }

    public string ShowUserButton { get; set; }
    public string ShowUsers { get; set; }
    public string OperatorUserMessage { get; set; }
    public List<SelectListItem> Users { get; set; }
    public string OperatorUserID { get; set; }
    public string UserID { get; set; }
    public bool OperatorUserIsActive { get; set; }
    public string OperatorUserList { get; set; }
}