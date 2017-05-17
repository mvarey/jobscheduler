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
    public class UserController : Controller
    {
        public HomeModel hm = new HomeModel();
        SetupPage BasePage = new SetupPage();
        DataClass db = new DataClass(ConfigurationManager.ConnectionStrings["JobScheduler"].ConnectionString);
        DataClass.TParams[] Parms;

        // GET: User
        public ActionResult SchedulerUser()
        {
            HomeModel h = BasePage.SetupPageBasics(Request, hm, Session);
            object g = new SchedulerUserModel();
            Utility.UpdateModel(h, ref g);
            SchedulerUserModel sm = (SchedulerUserModel)g;
            sm.ShowUser = "none";

            sm.UserID = string.Empty;
            sm.FirstName = string.Empty;
            sm.LastName = string.Empty;
            sm.UserLogin = string.Empty;
            sm.UserEmail = string.Empty;
            sm.UserPassword = string.Empty;
            sm.IsActive = true;
            sm.Admin = false;
            sm.View = false;
            sm.Report = false;
            sm.Operator = false;
            sm.UserMessage = string.Empty;
            sm.UserList = string.Empty;

            if (sm.AdminAccess || sm.ReportAccess || sm.OperatorAccess || sm.ViewAccess)
            {
                // User can see page
                sm.ShowUser = "inline";
                string Edit = Utility.GetParam(Request.Params.ToString(), "edit");
                string Delete = Utility.GetParam(Request.Params.ToString(), "delete");
                if (Request.Params["UserID"] != null)
                {
                    sm.UserID = Request.Params["UserID"].ToString();
                }
                if (Request.Params["FirstName"] != null)
                {
                    sm.FirstName = Request.Params["FirstName"].ToString();
                }
                if (Request.Params["LastName"] != null)
                {
                    sm.LastName = Request.Params["LastName"].ToString();
                }
                if (Request.Params["IsActive"] != null)
                {
                    sm.IsActive = Utility.GetBoolean(Request.Params["IsActive"].ToString());
                }
                if (Request.Params["UserLogin"] != null)
                {
                    sm.UserLogin = Request.Params["UserLogin"].ToString();
                }
                if (Request.Params["UserEmail"] != null)
                {
                    sm.UserEmail = Request.Params["UserEmail"].ToString();
                }
                if (Request.Params["UserPassword"] != null)
                {
                    sm.UserPassword = Request.Params["UserPassword"].ToString();
                }
                if (Request.Params["Admin"] != null)
                {
                    sm.Admin = Utility.GetBoolean(Request.Params["Admin"].ToString());
                }
                if (Request.Params["View"] != null)
                {
                    sm.View = Utility.GetBoolean(Request.Params["View"].ToString());
                }
                if (Request.Params["Report"] != null)
                {
                    sm.Report = Utility.GetBoolean(Request.Params["Report"].ToString());
                }
                if (Request.Params["Operator"] != null)
                {
                    sm.Operator = Utility.GetBoolean(Request.Params["Operator"].ToString());
                }
                if (sm.AdminAccess)
                {
                    if (Request.Params["btnUserSave"] != null)
                    {
                        if (sm.FirstName == string.Empty)
                        {
                            sm.UserMessage += "First Name must be filled in. ";
                        }
                        if (sm.LastName == string.Empty)
                        {
                            sm.UserMessage = "Last Name must be filled in. ";
                        }
                        if (sm.UserMessage == string.Empty)
                        {
                            string Command = "Add";
                            if (sm.UserID != string.Empty)
                            {
                                Command = "Update";
                            }
                            else
                            {
                                sm.UserID = null;
                            }
                            try
                            {
                                Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, Command),
                                        new DataClass.TParams("SchedulerUserID", 4, SqlDbType.Int, sm.UserID),
                                        new DataClass.TParams("UserFirstName", 20, SqlDbType.VarChar, sm.FirstName),
                                        new DataClass.TParams("UserLastName", 20, SqlDbType.VarChar, sm.LastName),
                                        new DataClass.TParams("IsActive", 1, SqlDbType.Bit, sm.IsActive),
                                        new DataClass.TParams("UserLogin", 50, SqlDbType.VarChar, sm.UserLogin),
                                        new DataClass.TParams("UserEmail", 50, SqlDbType.VarChar, sm.UserEmail),
                                        new DataClass.TParams("UserPassword", 100, SqlDbType.VarChar, DataEncyption.EncryptString(sm.UserPassword)),
                                        new DataClass.TParams("AdminAccess", 1, SqlDbType.Bit, sm.Admin),
                                        new DataClass.TParams("ViewAccess", 1, SqlDbType.Bit, sm.View),
                                        new DataClass.TParams("ReportAccess", 1, SqlDbType.Bit, sm.Report),
                                        new DataClass.TParams("OperatorAccess", 1, SqlDbType.Bit, sm.Operator)
                                };
                                db.DIRunSP(Parms, "sp_SchedulerUser");
                                sm.UserID = string.Empty;
                                sm.FirstName = string.Empty;
                                sm.LastName = string.Empty;
                                sm.IsActive = true;
                                sm.UserLogin = string.Empty;
                                sm.UserEmail = string.Empty;
                                sm.UserPassword = string.Empty;
                                sm.Admin = false;
                                sm.View = false;
                                sm.Operator = false;
                                sm.Report = false;
                            }
                            catch (Exception ex)
                            {
                                sm.UserMessage = ex.Message;
                            }
                        }
                    }
                }
                if (Request.Params["btnUserClear"] != null)
                {
                    // Clear Fields
                    sm.UserID = string.Empty;
                    sm.FirstName = string.Empty;
                    sm.LastName = string.Empty;
                    sm.IsActive = true;
                    sm.UserLogin = string.Empty;
                    sm.UserEmail = string.Empty;
                    sm.UserPassword = string.Empty;
                    sm.Admin = false;
                    sm.View = false;
                    sm.Report = false;
                    sm.Operator = false;
                    sm.UserMessage = string.Empty;
                }
                if (sm.AdminAccess)
                {
                    if (Edit.Length > 4)
                    {
                        Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Get"),
                                new DataClass.TParams("SchedulerUserID", 4, SqlDbType.Int, Edit.Substring(4))
                        };
                        DataSet dsEdit = db.DIRunSPretDs(Parms, "sp_SchedulerUser");
                        if (dsEdit.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = dsEdit.Tables[0].Rows[0];
                            sm.UserID = dr["SchedulerUserID"].ToString();
                            sm.FirstName = dr["UserFirstName"].ToString();
                            sm.LastName = dr["UserLastName"].ToString();
                            sm.IsActive = bool.Parse(dr["IsActive"].ToString());
                            sm.UserLogin = dr["UserLogin"].ToString();
                            sm.UserEmail = dr["UserEmail"].ToString();
                            sm.UserPassword = string.Empty;
                            sm.Admin = bool.Parse(dr["AdminAccess"].ToString());
                            sm.View = bool.Parse(dr["ViewAccess"].ToString());
                            sm.Report = bool.Parse(dr["ReportAccess"].ToString());
                            sm.Operator = bool.Parse(dr["OperatorAccess"].ToString());
                            sm.UserMessage = string.Empty;
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
                                        new DataClass.TParams("SchedulerUserID", 4, SqlDbType.Int, ServerID)
                                };
                                db.DIRunSP(Parms, "sp_SchedulerUser");
                                sm.UserID = string.Empty;
                                sm.FirstName = string.Empty;
                                sm.LastName = string.Empty;
                                sm.IsActive = true;
                                sm.UserLogin = string.Empty;
                                sm.UserEmail = string.Empty;
                                sm.UserPassword = string.Empty;
                                sm.Admin = false;
                                sm.View = false;
                                sm.Report = false;
                                sm.Operator = false;
                                sm.UserMessage = string.Empty;
                            }
                        }
                    }
                }

                // Fill Grid
                Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "List") };
                DataSet dsUser = db.DIRunSPretDs(Parms, "sp_SchedulerUser");
                StringBuilder b = new StringBuilder();
                b.Append("<table><tr class='gheader'>");
                int tblWidth = 0;
                if (sm.AdminAccess)
                {
                    b.Append("<th style='width:20px;'>&nbsp;</th>");
                    tblWidth += 27;
                }
                b.Append("<th style='width:200px;'>First Name</th><th style='width:200px;'>Last Name</th>" +
                        "<th style='width:500px;'>Login</th>" +
                        "<th style='width:60px;'>Active</th>");
                tblWidth += 207 + 207 + 67 + 507;
                if (sm.AdminAccess)
                {
                    b.Append("<th style='width:20px;'>&nbsp;</th>");
                    tblWidth += 27;
                }
                b.AppendLine("<th style='width:16px;'>&nbsp;</th></tr></table><div style='height:370px; width:" +
                    tblWidth.ToString() + "px; overflow:auto;'><table>");
                bool AltRow = false;
                foreach (DataRow dr in dsUser.Tables[0].Rows)
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
                                "<button name=\"edit" + dr["SchedulerUserID"].ToString() + "\" type=\"submit\" " +
                                " class='imagebutton' style=\"background: url(../Images/edit_16x16.gif) no-repeat;\"" +
                                ">&nbsp;</button>" +
                                "</td>");
                    }
                    b.Append("<td style='width:200px;'>" + dr["UserFirstName"].ToString() + "</td>" +
                        "<td style='width:200px;'>" + dr["UserLastName"].ToString() + "</td>" +
                        "<td style='width:500px;'>" + dr["UserLogin"].ToString() + "</td>");
                    b.Append("<td style='width:60px;text-align:center;'><input type = 'checkbox' disabled ");
                    if (bool.Parse(dr["IsActive"].ToString()))
                    {
                        b.Append("checked='checked' ");
                    }
                    b.Append("/></td>");
                    if (sm.AdminAccess)
                    {
                        b.Append("<td style='width:20px;'><button name=\"delete" + dr["SchedulerUserID"].ToString() + "\" type=\"submit\" " +
                            " class='imagebutton' style=\"background: url(../Images/delete_16x16.gif) no-repeat;\"" +
                            ">&nbsp;</button></td>");
                    }
                    b.AppendLine("</tr>");
                    AltRow = !AltRow;
                }
                b.AppendLine("</table></div>");
                sm.UserList = b.ToString();
            }
            return View(sm);
        }

        public ActionResult SchedulerOperator()
        {
            HomeModel h = BasePage.SetupPageBasics(Request, hm, Session);
            object g = new SchedulerOperatorModel();
            Utility.UpdateModel(h, ref g);
            SchedulerOperatorModel sm = (SchedulerOperatorModel)g;
            sm.ShowOperators = "none";

            sm.OperatorID = string.Empty;
            sm.OperatorUserID = string.Empty;
            sm.OperatorName = string.Empty;
            sm.OperatorIsActive = true;
            sm.OperatorList = string.Empty;
            sm.OperatorMessage = string.Empty;

            sm.OperatorUserID = string.Empty;
            sm.OperatorUserIsActive = true;
            sm.UserID = string.Empty;
            sm.OperatorUserList = string.Empty;
            sm.OperatorUserMessage = string.Empty;

            sm.ShowUserButton = "none";
            sm.ShowUsers = "none";

            if (sm.AdminAccess || sm.ReportAccess || sm.OperatorAccess || sm.ViewAccess)
            {
                // User can see page
                sm.ShowOperators = "inline";
                string Edit = Utility.GetParam(Request.Params.ToString(), "edit");
                string Delete = Utility.GetParam(Request.Params.ToString(), "delete");
                string Edtu = Utility.GetParam(Request.Params.ToString(), "edtu");
                string Delu = Utility.GetParam(Request.Params.ToString(), "delu");
                if (Request.Params["OperatorID"] != null)
                {
                    sm.OperatorID = Request.Params["OperatorID"].ToString();
                }
                if (Request.Params["OperatorName"] != null)
                {
                    sm.OperatorName = Request.Params["OperatorName"].ToString();
                }
                if (Request.Params["OperatorIsActive"] != null)
                {
                    sm.OperatorIsActive = Utility.GetBoolean(Request.Params["OperatorIsActive"].ToString());
                }

                if (Request.Params["ShowUserButton"] != null)
                {
                    sm.ShowUserButton = Request.Params["ShowUserButton"].ToString();
                }
                if (Request.Params["ShowUsers"] != null)
                {
                    sm.ShowUsers = Request.Params["ShowUsers"].ToString();
                }

                if (Request.Params["OperatorUserID"] != null)
                {
                    sm.OperatorUserID = Request.Params["OperatorUserID"].ToString();
                }
                if (Request.Params["UserID"] != null)
                {
                    sm.UserID = Request.Params["UserID"].ToString();
                }
                if (Request.Params["OperatorUserIsActive"] != null)
                {
                    sm.OperatorUserIsActive = Utility.GetBoolean(Request.Params["OperatorUserIsActive"].ToString());
                }

                if (sm.AdminAccess)
                {
                    if (Request.Params["btnOperatorSave"] != null)
                    {
                        if (sm.OperatorName == string.Empty)
                        {
                            sm.OperatorMessage += "Operator Group Name must be filled in. ";
                        }
                        if (sm.OperatorMessage == string.Empty)
                        {
                            string Command = "Add";
                            if (sm.OperatorID != string.Empty)
                            {
                                Command = "Update";
                            }
                            else
                            {
                                sm.OperatorID = null;
                            }
                            try
                            {
                                Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, Command),
                                        new DataClass.TParams("SchedulerOperatorID", 4, SqlDbType.Int, sm.OperatorID),
                                        new DataClass.TParams("SchedulerOperatorName", 50, SqlDbType.VarChar, sm.OperatorName),
                                        new DataClass.TParams("IsActive", 1, SqlDbType.Bit, sm.OperatorIsActive),
                                };
                                db.DIRunSP(Parms, "sp_SchedulerOperator");
                                sm.OperatorID = string.Empty;
                                sm.OperatorName = string.Empty;
                                sm.OperatorIsActive = true;
                                
                            }
                            catch (Exception ex)
                            {
                                sm.OperatorMessage = ex.Message;
                            }
                        }
                    }
                    if (Request.Params["btnUserSave"] != null)
                    {
                        if (sm.OperatorUserMessage == string.Empty)
                        {
                            string Command = "Add";
                            if (sm.OperatorUserID != string.Empty)
                            {
                                Command = "Update";
                            }
                            else
                            {
                                sm.OperatorUserID = null;
                            }
                            try
                            {
                                Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, Command),
                                        new DataClass.TParams("SchedulerOperatorUserId", 4, SqlDbType.Int, sm.OperatorUserID),
                                        new DataClass.TParams("SchedulerOperatorId", 4, SqlDbType.Int, sm.OperatorID),
                                        new DataClass.TParams("SchedulerUserId", 4, SqlDbType.Int, sm.UserID),
                                        new DataClass.TParams("IsActive", 1, SqlDbType.Bit, sm.OperatorUserIsActive)
                                };
                                db.DIRunSP(Parms, "sp_SchedulerOperatorUser");
                                sm.OperatorUserID = string.Empty;
                                sm.UserID = string.Empty;
                                sm.OperatorUserIsActive = true;
                            }
                            catch (Exception ex)
                            {
                                sm.OperatorUserMessage = ex.Message;
                            }
                        }
                    }
                }
                if (Request.Params["btnOperatorClear"] != null)
                {
                    // Clear Fields
                    sm.OperatorID = string.Empty;
                    sm.OperatorName = string.Empty;
                    sm.OperatorIsActive = true;
                    sm.OperatorMessage = string.Empty;
                }
                if (Request.Params["btnUserClear"] != null)
                {
                    sm.OperatorUserID = string.Empty;
                    sm.UserID = string.Empty;
                    sm.OperatorUserIsActive = true;
                    sm.OperatorUserMessage = string.Empty;
                }
                if (sm.AdminAccess)
                {
                    if (Edit.Length > 4)
                    {
                        Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Get"),
                                new DataClass.TParams("SchedulerOperatorID", 4, SqlDbType.Int, Edit.Substring(4))
                        };
                        DataSet dsEdit = db.DIRunSPretDs(Parms, "sp_SchedulerOperator");
                        if (dsEdit.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = dsEdit.Tables[0].Rows[0];
                            sm.OperatorID = dr["SchedulerOperatorID"].ToString();
                            sm.OperatorName = dr["SchedulerOperatorName"].ToString();
                            sm.OperatorIsActive = bool.Parse(dr["IsActive"].ToString());
                            sm.OperatorMessage = string.Empty;
                        }
                    }
                    if (Edtu.Length > 4)
                    {
                        Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Get"),
                                new DataClass.TParams("SchedulerOperatorUserId", 4, SqlDbType.Int, Edtu.Substring(4))
                        };
                        DataSet dsEdtu = db.DIRunSPretDs(Parms, "sp_SchedulerOperatorUser");
                        if (dsEdtu.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = dsEdtu.Tables[0].Rows[0];
                            sm.OperatorUserID = dr["SchedulerOperatorUserId"].ToString();
                            sm.UserID = dr["SchedulerUserId"].ToString();
                            sm.OperatorUserIsActive = bool.Parse(dr["IsActive"].ToString());
                            sm.OperatorUserMessage = string.Empty;
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
                                string OperatorID = Delete.Substring(6);
                                Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Delete"),
                                        new DataClass.TParams("SchedulerOperatorID", 4, SqlDbType.Int, OperatorID)
                                };
                                db.DIRunSP(Parms, "sp_SchedulerOperator");
                                sm.OperatorID = string.Empty;
                                sm.OperatorName = string.Empty;
                                sm.OperatorIsActive = true;
                                sm.OperatorMessage = string.Empty;
                            }
                        }
                    }
                    if (Delu.Length > 4)
                    {
                        string OperatorUserId = Delu.Substring(4);
                        Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "Delete"),
                                new DataClass.TParams("SchedulerOperatorUserId", 4, SqlDbType.Int, OperatorUserId)
                        };
                        db.DIRunSP(Parms, "sp_SchedulerOperatorUser");
                        sm.OperatorUserID = string.Empty;
                        sm.UserID = string.Empty;
                        sm.OperatorUserIsActive = true;
                    }
                }

                // Fill Grid
                Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "List") };
                DataSet dsOperator = db.DIRunSPretDs(Parms, "sp_SchedulerOperator");
                StringBuilder b = new StringBuilder();
                b.Append("<table><tr class='gheader'>");
                int tblWidth = 0;
                if (sm.AdminAccess)
                {
                    b.Append("<th style='width:20px;'>&nbsp;</th>");
                    tblWidth += 27;
                }
                b.Append("<th style='width:500px;'>Operator Group Name</th>" +
                        "<th style='width:60px;'>Active</th>");
                tblWidth += 507 + 67;
                if (sm.AdminAccess)
                {
                    b.Append("<th style='width:20px;'>&nbsp;</th>");
                    tblWidth += 27;
                }
                b.AppendLine("<th style='width:16px;'>&nbsp;</th></tr></table><div style='height:370px; width:" +
                    tblWidth.ToString() + "px; overflow:auto;'><table>");
                bool AltRow = false;
                foreach (DataRow dr in dsOperator.Tables[0].Rows)
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
                                "<button name=\"edit" + dr["SchedulerOperatorID"].ToString() + "\" type=\"submit\" " +
                                " class='imagebutton' style=\"background: url(../Images/edit_16x16.gif) no-repeat;\"" +
                                ">&nbsp;</button>" +
                                "</td>");
                    }
                    b.Append("<td style='width:500px;'>" + dr["SchedulerOperatorName"].ToString() + "</td>");
                    b.Append("<td style='width:60px;text-align:center;'><input type = 'checkbox' disabled ");
                    if (bool.Parse(dr["IsActive"].ToString()))
                    {
                        b.Append("checked='checked' ");
                    }
                    b.Append("/></td>");
                    if (sm.AdminAccess)
                    {
                        b.Append("<td style='width:20px;'><button name=\"delete" + dr["SchedulerOperatorID"].ToString() + "\" type=\"submit\" " +
                            " class='imagebutton' style=\"background: url(../Images/delete_16x16.gif) no-repeat;\"" +
                            ">&nbsp;</button></td>");
                    }
                    b.AppendLine("</tr>");
                    AltRow = !AltRow;
                }
                b.AppendLine("</table></div>");
                sm.OperatorList = b.ToString();
                if (sm.OperatorID != string.Empty)
                {
                    // Show Users in Operator Group
                    Parms = new DataClass.TParams[] { new DataClass.TParams("Command", 10, SqlDbType.VarChar, "List") };
                    DataSet dsUsers = db.DIRunSPretDs(Parms, "sp_SchedulerOperatorUser");
                    b = new StringBuilder();
                    b.Append("<table><tr class='gheader'>");
                    tblWidth = 0;
                    if (sm.AdminAccess)
                    {
                        b.Append("<th style='width:20px;'>&nbsp;</th>");
                        tblWidth += 27;
                    }
                    b.Append("<th style='width:500px;'>User Name</th>" +
                            "<th style='width:60px;'>Active</th>");
                    tblWidth += 507 + 67;
                    if (sm.AdminAccess)
                    {
                        b.Append("<th style='width:20px;'>&nbsp;</th>");
                        tblWidth += 27;
                    }
                    b.AppendLine("<th style='width:16px;'>&nbsp;</th></tr></table><div style='height:370px; width:" +
                        tblWidth.ToString() + "px; overflow:auto;'><table>");
                    AltRow = false;
                    foreach (DataRow dr in dsOperator.Tables[0].Rows)
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
                                    "<button name=\"edit" + dr["SchedulerOperatorUserID"].ToString() + "\" type=\"submit\" " +
                                    " class='imagebutton' style=\"background: url(../Images/edit_16x16.gif) no-repeat;\"" +
                                    ">&nbsp;</button>" +
                                    "</td>");
                        }
                        b.Append("<td style='width:500px;'>" + dr["UserName"].ToString() + "</td>");
                        b.Append("<td style='width:60px;text-align:center;'><input type = 'checkbox' disabled ");
                        if (bool.Parse(dr["IsActive"].ToString()))
                        {
                            b.Append("checked='checked' ");
                        }
                        b.Append("/></td>");
                        if (sm.AdminAccess)
                        {
                            b.Append("<td style='width:20px;'><button name=\"delete" + dr["SchedulerOperatorUserID"].ToString() + "\" type=\"submit\" " +
                                " class='imagebutton' style=\"background: url(../Images/delete_16x16.gif) no-repeat;\"" +
                                ">&nbsp;</button></td>");
                        }
                        b.AppendLine("</tr>");
                        AltRow = !AltRow;
                    }
                    b.AppendLine("</table></div>");
                    sm.OperatorUserList = b.ToString();
                }
            }
            return View(sm);
        }

    }
}