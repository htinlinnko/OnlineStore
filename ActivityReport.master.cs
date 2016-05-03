using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ActivityReport : System.Web.UI.MasterPage
{
    activityEntities aEntity = new activityEntities();
    esp_dbEntities userEntity = new esp_dbEntities();
    common_function cFun = new common_function();
    UserInfo uInfo = new UserInfo();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (cFun.getSession("LogonUser") != null) uInfo = (UserInfo)cFun.getSession("LogonUser");
        else
        {
            uInfo = cFun.getLogonUser(Environment.UserName);
            cFun.setSession("LogonUser", uInfo as object);
        }

        try
        {
            ltrUserName.Text = uInfo.Display_name;
        }
        catch { ltrUserName.Text = ""; }
        
        // Get User role
        /*var aUser = from au in aEntity.activity_user
                    where au.active == true && au.window_login == uInfo.Windows_login
                    select au;*/

        var aUser = from uar in userEntity.UPS_USER_APPS_ROLE
                    where uar.status == true && uar.window_login == uInfo.Windows_login
                    select uar;

        if (aUser.Any()) cFun.setSession("ActivityUser", aUser as object);
        else Response.Redirect("NoAccess.html");
    }
}
