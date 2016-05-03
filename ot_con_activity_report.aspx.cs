using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ebms;
using System.Configuration;

public partial class ot_con_activity_report : System.Web.UI.Page
{
    common_function cFun = new common_function();
    ebms_function ebmsFunc = new ebms_function();
    esp_dbEntities userEntity = new esp_dbEntities();
    activityEntities aEntity = new activityEntities();
    bool evtstat_lock = bool.Parse(ConfigurationManager.AppSettings.Get("evtstat_lock").ToString());
    string hirerContainsList = System.Configuration.ConfigurationManager.AppSettings.Get("HirerContainsList");
    UserInfo uInfo = new UserInfo();


    #region Session and Query String List
    /*
     * SESSION
     * ----------
     * MainActivity                 - Main activity.
     * LogonUser                    - User who is currently using.
     * OutdoorPerformanceList       - Performance / Activity list.
     * OutdoorPerformanceDelList    - Deleted Outdoor Performance List.
     * OutdoorActivityDetail        - Outdoor activity details info which links to main activity.
     * --------------------------------------------------------------------------------------------
     * USER CONTROL (los_fd_defect)
     * -----------------------------
     * LostFoundList                - Lost Found List
     * LostFoundDelList             - Lost Found Delete List
     * DefectList                   - Defect List
     * DefectDelList                - Defect Del List
     * 
     * QUERY STRING
     * --------------
     * activity     -   Activity ID (Main activity_id)
     * 
     */
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        ClientScriptManager csManager = Page.ClientScript;
        Type csType = this.GetType();

        // For update existing
        int activityID = 0;

        if (cFun.getSession("LogonUser") != null) uInfo = (UserInfo)cFun.getSession("LogonUser");
        else cFun.setSession("LogonUser", cFun.getLogonUser(Environment.UserName) as object);

        if (Request.QueryString["activity"] != null)
            activityID = int.Parse(Request.QueryString["activity"].ToString());

        int activityTypeID = (from ati in aEntity.performance_activity_type
                              where ati.is_display == false && ati.active == true
                              select ati.activity_type_id).SingleOrDefault();

        hdnActivityTypeID.Value = activityTypeID != null ? activityTypeID.ToString() : "0";

        #region Page Not Post Back
        if (!IsPostBack)
        {
            if (Request.QueryString["sta"] != null)
            {
                string rptStatus = Request.QueryString["sta"].ToString();
                if (rptStatus == "sa")
                    lblMessage.Text = "The report has been successfully saved.";
                else if (rptStatus == "su")
                    lblMessage.Text = "The report has been successfully submitted to Duty manager.";
                else if (rptStatus == "com")
                    lblMessage.Text = "The report has been successfully completed.";
                else if (rptStatus == "rj")
                    lblMessage.Text = "The report has been rejected. Notification will be sent to the user.";
                else if (rptStatus == "up")
                    lblMessage.Text = "Your report has been successfully updated.";
                else lblMessage.Text = "";
            }
            //lnkPerRemove.Attributes.Add("onclick", "javascript: return removePerformanceMessage();");
            btnSaveDraft.Enabled = false;
            btnSubmit.Enabled = false;
            btnComplete.Visible = false;

            /* #1. Reset session */
            resetSession();

            uInfo = cFun.getLogonUser(Environment.UserName);

            if (cFun.getSession("LogonUser") != null) cFun.removeSession("LogonUser");
            cFun.setSession("LogonUser", uInfo);
            ddlTicketAgent.Items.Clear();
            ddlTicketAgent.Items.Insert(0, new ListItem("NA", "NA"));

            if (activityID != 0)
            {
                var amCheck = from a in aEntity.activity_main
                              where a.activity_id == activityID
                              select a;
                //string jsScript = string.Format("<script language='javascript'>alert('Report cannot be load. Either report has been deleted or An error occured while loading report.');</script>");
                //csManager.RegisterStartupScript(csType, "ActivityLoad", jsScript);
                // Existing record
                if (amCheck.Any())
                {
                    activity_main am = new activity_main();
                    am = (from a in aEntity.activity_main
                          where a.activity_id == activityID
                          select a).SingleOrDefault();

                    // set session
                    if (cFun.getSession("MainActivity") != null) cFun.removeSession("MainActivity");
                    cFun.setSession("MainActivity", am as object);

                    // retrieve data
                    txtDateOfReport.Text = am.date_of_report.Value.ToString("dd-MMM-yyyy");
                    BindVenue();
                    setCompany();
                    setReportType();
                    ddlVenue.SelectedValue = am.venue;
                    ddlCompany.SelectedValue = am.company;
                    ddlReportType.SelectedValue = am.report_type_id.ToString();

                    // Company event override
                    ddlCompany_SelectedIndexChanged(sender, new EventArgs());

                    // Venue event override
                    ddlVenue_SelectedIndexChanged(sender, new EventArgs());
                    foreach (ListItem lItem in ddlPerformance.Items)
                    {
                        if (lItem.Text.Contains(am.performance_1))
                        {
                            lItem.Selected = true;
                            break;
                        }
                    }

                    if (am.ticketing_agent != "" && am.ticketing_agent != null)
                    {
                        chkTicketed.Checked = true;
                        chkTicketed_CheckedChanged(sender, new EventArgs());
                        ddlTicketAgent.SelectedValue = am.ticketing_agent;
                    }

                    // Performance event override
                    ddlPerformance_SelectedIndexChanged(sender, new EventArgs());

                    selectListItem(am.production_ic, lstProductionIC);
                    selectListItem(am.programming_ic, lstProgrammingIC);
                    selectListItem(am.venue_officer, lstVenueManager);

                    // Save, Proceed function
                    saveProceedLoadFunction();
                    btnSubmit.Enabled = true;
                    btnSaveDraft.Enabled = true;

                    #region Get event id
                    string selectedPerfText = ddlPerformance.SelectedItem.Text;
                    string selectedPerfValue = ddlPerformance.SelectedValue;
                    string selectedPerfDate = txtDateOfReport.Text;
                    int eventID = selectedPerfValue != "" ? int.Parse(selectedPerfValue.Split(',')[1]) : 0;
                    hdnEventID.Value = eventID == 0 ? "" : eventID.ToString();
                    hdnEventDateTime.Value = selectedPerfDate;
                    hdnActivityID.Value = activityID.ToString();
                    #endregion

                    if (hdnEventID.Value != "" && hdnActivityID.Value != "")
                    {
                        #region Performance activity
                        List<performance_activity> perActivity = new List<performance_activity>();
                        try
                        {
                            if (cFun.getSession("OutdoorPerformanceList") == null)
                            {
                                perActivity = (from pa in aEntity.performance_activity
                                               where pa.activity_id == activityID
                                               select pa).ToList<performance_activity>();
                            }
                            else { perActivity = (List<performance_activity>)cFun.getSession("OutdoorPerformanceList"); }

                            if (!perActivity.Any()) // Get activity main session
                            { BindActivityDataGrid(null, activityID, activityTypeID); }
                            else // if got something // load that
                            { BindActivityDataGrid(perActivity.ToList<performance_activity>(), activityID, null); }
                        }
                        catch (Exception pae)
                        {
                            lblMessage.CssClass = "errorMessage";
                            lblMessage.Text = "An error occured. " + pae.Message;
                            writeLog.log(pae, "ot_con_activity_report.aspx > Page_Load > Loading performance");
                        }
                        #endregion

                        #region Activity details
                        try
                        {
                            var aDetails = aEntity.activity_details.Where(ad => ad.activity_id == activityID);
                            int expTotal, engTotal = 0;

                            // #1. Get Activity Details from control
                            activity_details actDetails = new activity_details(); // Only has 1 Details in Outdoor and Concourse                    

                            if (cFun.getSession("OutdoorActivityDetail") == null && !aDetails.Any()) // Session must be null and doesn't have data in db
                            {
                                perActivity = (List<performance_activity>)cFun.getSession("OutdoorPerformanceList");
                                var expt = from pa in perActivity
                                           where pa.exp_audience != null
                                           select new { exp = pa.exp_audience };

                                var engt = from pa in perActivity
                                           where pa.eng_audience != null
                                           select new { eng = pa.eng_audience };

                                expTotal = expt.Sum(es => es.exp).Value;
                                engTotal = engt.Sum(es => es.eng).Value;

                                actDetails.total_exposed_audience = expTotal;
                                actDetails.total_engaged_audience = engTotal;
                            }
                            else if (cFun.getSession("OutdoorActivityDetail") == null && aDetails.Any()) // Session must be null and have data in db
                            { actDetails = aDetails.SingleOrDefault(); }
                            else if (cFun.getSession("OutdoorActivityDetail") != null)
                            { actDetails = (activity_details)cFun.getSession("OutdoorActivityDetail"); }

                            txtTotalExposedCounts.Text = actDetails.total_exposed_audience.HasValue ? actDetails.total_exposed_audience.Value.ToString() : "0";
                            txtTotalEngagedCounts.Text = actDetails.total_engaged_audience.HasValue ? actDetails.total_engaged_audience.Value.ToString() : "0";
                            txtHandiPat.Text = actDetails.no_of_handicapped_patron.HasValue ? actDetails.no_of_handicapped_patron.Value.ToString() : "0";
                            txtTotalUsher.Text = actDetails.no_of_ushers.HasValue ? actDetails.no_of_ushers.Value.ToString() : "0";
                            txtTotalUsherRequired.Text = actDetails.no_of_ushers_default.HasValue ? actDetails.no_of_ushers_default.Value.ToString() : cFun.returnUsherCount(am.venue).ToString();
                            txtUsherDeploymentComments.Text = actDetails.usher_dply_comments;
                            txtOtherComments.Text = actDetails.other_comments;

                            cFun.setSession("OutdoorActivityDetail", actDetails as object);
                        }
                        catch (Exception ade)
                        {
                            lblMessage.CssClass = "errorMessage";
                            lblMessage.Text = "An error occured. " + ade.Message;
                            writeLog.log(ade, "ot_con_venue.ascx > Page_Load > Loading activity details");
                        }
                        #endregion

                        btnRecalculate_Click(sender, new EventArgs());
                    }
                }
                else
                {
                    /*string jsScript = string.Format("<script language='javascript'>alert('Report cannot be load. Either report has been deleted or An error occured while loading report.');</script>");
                    csManager.RegisterStartupScript(csType, "ActivityLoad", jsScript);*/
                }
            }
            else
            {
                // Not Post back function
                txtDateOfReport.Text = DateTime.Now.ToString("dd-MMM-yyyy");
                BindVenue();
                setCompany();
                setReportType();
                btnSubmit.Enabled = false;
                btnSaveDraft.Enabled = false;
            }
        }
        #endregion
    }

    private void selectListItem(string _selected, ListBox lstBox)
    {
        try
        {
            foreach (ListItem li in lstBox.Items)
            {
                //if (li.Value.Contains(_selected)) li.Selected = true;
                if (_selected.Contains(li.Value)) li.Selected = true;
            }
        }
        catch (Exception ex) { throw ex; }
    }

    /// <summary>
    /// Reset current session of the page.
    /// </summary>
    private void resetSession()
    {
        #region Outdoor activity session
        cFun.removeSession("OutdoorPerformanceList");
        cFun.removeSession("OutdoorPerformanceDelList");
        cFun.removeSession("OutdoorActivityDetail");
        #endregion

        #region Main activity
        cFun.removeSession("MainActivity");
        cFun.removeSession("LogonUser");
        #endregion
    }

    private void setCompany()
    {
        if (ddlCompany.Items.Count > 0) ddlCompany.Items.Clear();
        var companyList = (from comp in userEntity.TECL_COMPANY
                           where comp.status == true
                           select comp);

        ddlCompany.DataSource = companyList.OrderBy(o => o.order).ToList<TECL_COMPANY>();
        ddlCompany.DataValueField = "company_id";
        ddlCompany.DataTextField = "company_name";
        ddlCompany.DataBind();

        ddlCompany.Items.Insert(0, new ListItem("", ""));
    }

    private void saveProceedLoadFunction()
    {
        int reportID = int.Parse(ddlReportType.SelectedValue);
        string ctrlName = aEntity.report_type.Where(rt => rt.report_type_id == reportID).SingleOrDefault().
            load_ctrl_name.ToString();

        string[] ctrlArray = ctrlName.Split(',');

        setMainActivitySession(false);
        activity_main am = new activity_main();
        am = (activity_main)Session["MainActivity"];

        string evtTemp = hdnSelectedPer.Value.Split('|')[0];
        int eventID = evtTemp != "" ? int.Parse(evtTemp.Split(',')[1]) : 0;
    }

    /* Set main activity action to Session */
    private void setMainActivitySession(bool isNew = true)
    {
        ClientScriptManager csManager = Page.ClientScript;
        Type csType = this.GetType();
        try
        {
            hdnSelectedPer.Value = ddlPerformance.SelectedValue + "|" + ddlPerformance.SelectedItem.Text;
            hdnSelectedPerDate.Value = txtDateOfReport.Text;

            if (cFun.getSession("LogonUser") != null) uInfo = (UserInfo)cFun.getSession("LogonUser");
            else uInfo = cFun.getLogonUser(Environment.UserName);

            //List<activity_user> actUser = new List<activity_user>();

            //if (cFun.getSession("ActivityUser") != null) actUser = (List<activity_user>)cFun.getSession("ActivityUser");

            activity_main am = new activity_main();
            if (cFun.getSession("MainActivity") != null) am = (activity_main)cFun.getSession("MainActivity");
            else
            {
                am.company = ddlCompany.SelectedValue;
                am.venue = ddlVenue.SelectedValue;
                am.report_type_id = int.Parse(ddlReportType.SelectedValue);
                am.date_of_report = txtDateOfReport.Text != "" ? DateTime.Parse(txtDateOfReport.Text) : (Nullable<DateTime>)null;
                am.scheduled_start_time = txtScheduledStartTime.Text;
                string per1 = hdnSelectedPer.Value; //Request.Form[ddlPerformance.ClientID];
                am.performance_1 = hdnSelectedPer.Value.Split('|')[1]; //Request.Form[ddlPerformance.ClientID].ToString();// ddlPerformance.Text;
                am.performance_2 = txtPerformanceText.Text != "" ? txtPerformanceText.Text : hdnSelectedPer.Value.Split('|')[1];
                am.venue_officer = getSelectedStringFromListBox(lstVenueManager);
                am.programming_ic = getSelectedStringFromListBox(lstProgrammingIC);
                am.production_ic = getSelectedStringFromListBox(lstProductionIC);
                am.el4 = txtEL4.Text;
                am.el5 = txtEL5.Text;
                am.hirer = txtHirer.Text;
                am.ticketing_agent = chkTicketed.Checked ? ddlTicketAgent.SelectedValue : null;
                am.is_draft = true;
                am.draft_last_saved_date = DateTime.Now;
                am.draft_last_saved_by = uInfo.Display_name;
                am.created_by = uInfo.Display_name;
                am.created_date = DateTime.Now;
            }

            txtPerformanceText.Text = (am.performance_2 == null || am.performance_2 == "") ? am.performance_1 : am.performance_2;

            if (am.is_submitted.HasValue && am.is_submitted.Value == true)
            {
                tdSubmit.BgColor = "#339933";
                tdSubmit.Style.Add("color", "#FFF");
                tdSubmit.InnerHtml = "Submitted";
                btnSaveDraft.Visible = false;
                btnSubmit.Visible = false;
                ltrSubmittedBy.Text = am.submitted_by;
                ltrSubmittedOn.Text = am.submitted_date.HasValue ? am.submitted_date.Value.ToString("dd-MMM-yyyy hh:mm tt") : "";
            }

            if (am.is_approved.HasValue)
            {
                if (am.is_approved.Value) // approve
                {
                    tdApprove.BgColor = "#339933";
                    tdApprove.Style.Add("color", "#FFF");
                    tdApprove.InnerHtml = "Completed";
                    ltrCompletedBy.Text = am.approved_by;
                    ltrCompletedOn.Text = am.approved_date.HasValue ? am.approved_date.Value.ToString("dd-MMM-yyyy hh:mm tt") : "";
                }
                else // reject
                {
                    tdApprove.BgColor = "#FF6600";
                    tdApprove.Style.Add("color", "#FFF");
                    tdApprove.InnerHtml = "Rejected";
                    ltrRejectedBy.Text = am.approved_by;
                    ltrRejectedOn.Text = am.approved_date.HasValue ? am.approved_date.Value.ToString("dd-MMM-yyyy hh:mm tt") : "";
                    btnSaveDraft.Visible = true;
                    btnSubmit.Visible = true;
                }
            }

            if (isNew)
            {
                aEntity.activity_main.Add(am);
                aEntity.SaveChanges();
            }

            ltrCreatedBy.Text = am.created_by;
            ltrCreatedOn.Text = am.created_date.HasValue ? am.created_date.Value.ToString("dd-MMM-yyyy hh:mm tt") : "";

            ltrUpdatedBy.Text = am.updated_by;
            ltrUpdatedOn.Text = am.updated_date.HasValue ? am.updated_date.Value.ToString("dd-MMM-yyyy hh:mm tt") : "";

            if (am.is_draft.HasValue)
            {
                tdDraft.BgColor = "#339933";
                tdDraft.Style.Add("color", "#FFF");
                tdDraft.InnerHtml = "Draft saved";
                ltrDraftSavedBy.Text = am.draft_last_saved_by;
                ltrDraftSavedOn.Text = am.draft_last_saved_date.HasValue ? am.draft_last_saved_date.Value.ToString("dd-MMM-yyyy hh:mm tt") : "";
            }

            AccessRightControl(am); //, actUser);

            cFun.setSession("MainActivity", am as object);
        }
        catch (Exception ex)
        {
            lblMessage.CssClass = "errorMessage";
            lblMessage.Text = "An error occured. " + ex.Message;
            writeLog.log(ex);
        }
    }

    private void AccessRightControl(activity_main am) //, List<activity_user> auList)
    {
        try
        {
            btnSaveDraft.Visible = true;
            btnUpdate.Visible = true;
            btnComplete.Visible = true;
            btnSubmit.Visible = true;
            btnReject.Visible = true;

            // Get DMList
            List<string> dmList = new List<string>();
            dmList = cFun.getDutyManagerList();

            if (!dmList.Contains(uInfo.User_id)) // Duty Manager
            {
                btnUpdate.Visible = false;
                btnComplete.Visible = false;
                btnReject.Visible = false;
            }

            if (am.is_approved.HasValue)
            {
                btnSaveDraft.Visible = false;
                btnSubmit.Visible = false;

                if (am.is_approved.Value && dmList.Contains(uInfo.User_id))
                {
                    btnUpdate.Visible = true;
                    btnComplete.Visible = false;
                    btnReject.Visible = false;
                }
                else if (am.is_approved.Value && !dmList.Contains(uInfo.User_id))
                {
                    btnUpdate.Visible = btnComplete.Visible = btnReject.Visible = false;
                    hdnDisableButton.Value = "True";
                    disableControl();
                }
                else if (!am.is_approved.Value && !dmList.Contains(uInfo.User_id))
                {
                    btnSaveDraft.Visible = btnSubmit.Visible = true;
                    btnUpdate.Visible = btnComplete.Visible = btnReject.Visible = false;
                }
                else if (!am.is_approved.Value && dmList.Contains(uInfo.User_id))
                {
                    btnUpdate.Visible = btnComplete.Visible = btnReject.Visible = false;
                    btnSaveDraft.Visible = btnSubmit.Visible = true;
                }
            }
            else if (am.is_submitted.HasValue)
            {
                btnSaveDraft.Visible = btnSubmit.Visible = false;

                if (am.is_submitted.Value && dmList.Contains(uInfo.User_id))
                {
                    btnUpdate.Visible = false;
                    btnComplete.Visible = true;
                    btnReject.Visible = true;
                    //hdnDisableButton.Value = "True";
                    //disableControl();
                }
                else if (am.is_submitted.Value && !dmList.Contains(uInfo.User_id))
                {
                    btnUpdate.Visible = btnComplete.Visible = btnReject.Visible = false;
                    hdnDisableButton.Value = "True";
                    disableControl();
                }
                /*else if (!am.is_submitted.Value && !dmList.Contains(uInfo.User_id))
                {
                    btnSaveDraft.Visible = true;
                    btnUpdate.Visible = btnComplete.Visible = btnReject.Visible = false;
                }*/
            }
            else
            {
                btnUpdate.Visible = false;
                btnComplete.Visible = false;
                btnReject.Visible = false;
            }

            // Event stats lock
            DateTime evtstatLockDate = cFun.getEventStatsCutOffDate();
            DateTime dateOfReport = DateTime.Parse(txtDateOfReport.Text);
            DateTime tempDate = new DateTime(evtstatLockDate.Year, evtstatLockDate.Month, 1);
            DateTime prevMonthLastDate = tempDate.AddDays(-1);

            if (evtstat_lock)
            {
                if (DateTime.Now > evtstatLockDate && dateOfReport <= prevMonthLastDate)
                {
                    ltrInfoMessage.Visible = true;
                    ltrInfoMessage.Text = "This report is locked for editing.";
                    btnSaveDraft.Visible = false;
                    btnUpdate.Visible = false;
                    btnComplete.Visible = false;
                    btnSubmit.Visible = false;
                    btnReject.Visible = false;
                    disableControl();
                }
            }

            /*
            if (!am.is_submitted.HasValue) { btnSubmit.Visible = true; }
            else btnSubmit.Visible = btnSaveDraft.Visible = false;

            if (am.is_approved.HasValue)
            {
                if (!am.is_approved.Value)
                    btnComplete.Visible = btnReject.Visible = true;
            }
            else { btnComplete.Visible = btnReject.Visible = true; }

            // Get user not in USER ROLE AND AM role
            var rlist = from ul in auList
                        join rl in aEntity.activity_user_role
                        on ul.role_id equals rl.role_id
                        where !rl.role_short_name.Contains("USR") && !rl.role_short_name.Contains("AM")
                        && ul.window_login == uInfo.Windows_login
                        select ul;

            bool isUserRole = (from ul in auList
                        join rl in aEntity.activity_user_role
                        on ul.role_id equals rl.role_id
                        where rl.role_short_name.Contains("USR")
                        && ul.window_login == uInfo.Windows_login
                        select ul.active.Value).SingleOrDefault();

            if (!rlist.Any())
            {
                btnSaveDraft.Visible = false;
                btnUpdate.Visible = false;
                btnComplete.Visible = false;
                btnReject.Visible = false;
                btnSubmit.Visible = false;
            }

            if (am.is_approved.HasValue)
            {
                if (!am.is_approved.Value && isUserRole) { btnUpdate.Visible = false; btnSubmit.Visible = true; btnSaveDraft.Visible = true; }
                if (am.is_approved.Value) { btnComplete.Visible = btnReject.Visible = false; }
            }

            if (!am.is_submitted.HasValue)
            {
                btnSubmit.Visible = true; btnSaveDraft.Visible = true;
            }
            */
        }
        catch (Exception ex) { throw ex; }
    }

    private string getSelectedStringFromListBox(ListBox lstBox)
    {
        List<string> lItemStr = new List<string>();
        foreach (ListItem lItem in lstBox.Items) { if (lItem.Selected) lItemStr.Add(lItem.Value); }

        return string.Join(",", lItemStr.ToArray());
    }

    protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlCompany.SelectedValue != "")
        {
            BindVenue();
            BindVenueOfficer();
        }
        else
        {
            if (lstVenueManager.Items.Count > 0) lstVenueManager.Items.Clear();
            if (lstProgrammingIC.Items.Count > 0) lstProgrammingIC.Items.Clear();
        }
    }

    private void BindVenue()
    {
        DateTime selectedDate = new DateTime();
        try { selectedDate = DateTime.Parse(txtDateOfReport.Text); }
        catch { }

        txtDayOfReport.Text = selectedDate.DayOfWeek.ToString();
        DataTable dTable = new DataTable();
        dTable = ebmsFunc.getVenueFromEBMS(selectedDate, selectedDate, ddlCompany.SelectedValue);
        if (ddlVenue.Items.Count > 0) ddlVenue.Items.Clear();

        ddlVenue.DataSource = dTable;
        ddlVenue.DataValueField = "EV800_SPACE_CODE";
        ddlVenue.DataTextField = "EV800_SPACE_DESC";
        ddlVenue.DataBind();

        ddlVenue.Items.Insert(0, new ListItem("", ""));
    }

    private void BindVenueOfficer()
    {
        List<UPS_USER> venueUserList = new List<UPS_USER>();
        List<UPS_USER> prodUserList = new List<UPS_USER>();
        //venueUserList = cFun.getVenueOfficerManager(ddlCompany.SelectedValue);
        venueUserList = cFun.getUserList(cFun.venueManagerDept);
        prodUserList = cFun.getUserList(cFun.prodDept);

        if (lstVenueManager.Items.Count > 0) lstVenueManager.Items.Clear();
        lstVenueManager.DataSource = venueUserList;
        lstVenueManager.DataValueField = "user_id";
        lstVenueManager.DataTextField = "display_name";
        lstVenueManager.DataBind();

        if (lstProductionIC.Items.Count > 0) lstProductionIC.Items.Clear();
        lstProductionIC.DataSource = prodUserList;
        lstProductionIC.DataValueField = "user_id";
        lstProductionIC.DataTextField = "display_name";
        lstProductionIC.DataBind();

        /*venueUserList = cFun.getProgrammingOfficer();
        if (lstProgrammingIC.Items.Count > 0) lstProgrammingIC.Items.Clear();
        lstProgrammingIC.DataSource = venueUserList;
        lstProgrammingIC.DataValueField = "user_id";
        lstProgrammingIC.DataTextField = "display_name";
        lstProgrammingIC.DataBind();*/
    }

    private void setReportType()
    {
        List<report_type> reportTypeList = new List<report_type>();
        reportTypeList = cFun.getReportTypeInformation();

        if (ddlReportType.Items.Count > 0) ddlReportType.Items.Clear();
        ddlReportType.DataSource = reportTypeList;
        ddlReportType.DataValueField = "report_type_id";
        ddlReportType.DataTextField = "report_type_name";
        ddlReportType.DataBind();

        ddlReportType.Items.Insert(0, new ListItem("", ""));
    }

    protected void chkTicketed_CheckedChanged(object sender, EventArgs e)
    {
        ddlTicketAgent.Items.Clear();
        if (chkTicketed.Checked)
        {
            List<ticket_agent> taList = new List<ticket_agent>();
            taList = cFun.getTicketAgentList();
            if (taList != null)
            {
                foreach (ticket_agent ta in taList) { ddlTicketAgent.Items.Add(new ListItem(ta.tix_agent_name.ToString(), ta.tix_agent_id.ToString())); }
                ddlTicketAgent.Items.Insert(0, new ListItem("", ""));
            }
            else { ddlTicketAgent.Items.Add(new ListItem("NA", "NA")); }
        }
        else { ddlTicketAgent.Items.Add(new ListItem("NA", "NA")); }
    }

    protected void ddlVenue_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlPerformance.Items.Clear();

        if (ddlVenue.SelectedValue != "")
        {
            List<ebms_info> eList = new List<ebms_info>();
            eList = ebmsFunc.getEbmsInformation(txtDateOfReport.Text, txtDateOfReport.Text, ddlVenue.SelectedValue);

            if (eList != null)
            {
                foreach (ebms_info ei in eList)
                {
                    ddlPerformance.Items.Add(new ListItem(ei.Evt_desc, ei.Id + "," + ei.Evt_id));
                }
                ddlPerformance.Items.Insert(0, new ListItem("", ""));
            }
        }
    }

    protected void ddlPerformance_SelectedIndexChanged(object sender, EventArgs e)
    {
        lstProgrammingIC.Items.Clear();

        if (ddlPerformance.SelectedValue != "")
        {
            // Store value to hidden field
            hdnSelectedPer.Value = ddlPerformance.SelectedValue + "|" + ddlPerformance.SelectedItem.Text;
            hdnSelectedPerDate.Value = txtDateOfReport.Text;

            List<ebms_performance_info> ebmsPerfList = new List<ebms_performance_info>();
            ebmsPerfList = ebmsFunc.getEbmsPerformanceInformation(txtDateOfReport.Text, txtDateOfReport.Text,
                ddlPerformance.SelectedValue, ddlPerformance.SelectedItem.Text, ddlVenue.SelectedValue);
            if (ebmsPerfList != null)
            {
                txtScheduledStartTime.Text = DateTime.Parse(ebmsPerfList[0].Start_datetime).ToString("hh:mm tt");
                txtHirer.Text = ebmsPerfList[0].Hirer_name;
                txtEL4.Text = ebmsPerfList[0].El4;
                txtEL5.Text = ebmsPerfList[0].El5;

                // Load either PG or VP
                List<UserInfo> uInfoList;
                string[] hirercontainListArray = hirerContainsList.Split(',');

                var listExists = hirercontainListArray.Where(wh => wh.ToLower().Equals(txtHirer.Text.ToLower()));

                //if (txtHirer.Text != "" && txtHirer.Text.ToLower().Contains("programming")) //PG // Comment by Htin, 14 Sep 2015
                //if (txtHirer.Text != "" && listExists.Any())

                //if (txtHirer.Text != "" && txtHirer.Text.ToLower().Contains("programming")) //PG // Comment by Htin, 14 Sep 2015
                //if (txtHirer.Text != "" && hirercontainListArray.Contains(txtHirer.Text))
                if (txtHirer.Text != "" && listExists.Any())
                {
                    uInfoList = new List<UserInfo>();
                    uInfoList = cFun.getVPUserList(ddlCompany.SelectedValue);
                    foreach (UserInfo ui in uInfoList) { lstProgrammingIC.Items.Add(new ListItem(ui.Display_name, ui.User_id)); }
                }
                else if (txtHirer.Text != "" && txtHirer.Text.ToLower().Contains("tecl"))
                {
                    uInfoList = new List<UserInfo>();
                    uInfoList = cFun.getPGUserList();
                    foreach (UserInfo ui in uInfoList) { lstProgrammingIC.Items.Add(new ListItem(ui.Display_name, ui.User_id)); }

                    //uInfoList = cFun.getVPUserList(ddlCompany.SelectedValue);
                    //foreach (UserInfo ui in uInfoList) { lstProgrammingIC.Items.Add(new ListItem(ui.Display_name, ui.User_id)); }
                }
                else // VP
                {
                    uInfoList = new List<UserInfo>();
                    uInfoList = cFun.getVPUserList(ddlCompany.SelectedValue);
                    foreach (UserInfo ui in uInfoList) { lstProgrammingIC.Items.Add(new ListItem(ui.Display_name, ui.User_id)); }
                }
            }
        }
    }

    protected void btnSaveDraft_Click(object sender, EventArgs e)
    {
        try
        {
            if (chkTicketed.Checked) if (ddlTicketAgent.SelectedValue == "") { cvTicketAgent.IsValid = false; }
            if (Page.IsValid && cvTicketAgent.IsValid)
            {
                hdnSelectedPer.Value = ddlPerformance.SelectedValue + "|" + ddlPerformance.SelectedItem.Text;
                hdnSelectedPerDate.Value = txtDateOfReport.Text;

                if (cFun.getSession("LogonUser") != null) uInfo = (UserInfo)cFun.getSession("LogonUser");
                else uInfo = cFun.getLogonUser(Environment.UserName);

                #region Save Main Activity
                activity_main am = new activity_main();
                am = (activity_main)cFun.getSession("MainActivity");

                aEntity.activity_main.Where(a => a.activity_id == am.activity_id).ToList().ForEach(fe =>
                {
                    fe.company = ddlCompany.SelectedValue;
                    fe.venue = ddlVenue.SelectedValue;
                    fe.report_type_id = int.Parse(ddlReportType.SelectedValue);
                    fe.date_of_report = txtDateOfReport.Text != "" ? DateTime.Parse(txtDateOfReport.Text) : (Nullable<DateTime>)null;
                    fe.scheduled_start_time = txtScheduledStartTime.Text;
                    string per1 = hdnSelectedPer.Value; //Request.Form[ddlPerformance.ClientID];
                    fe.performance_1 = hdnSelectedPer.Value.Split('|')[1]; // PREV CODE
                    fe.performance_2 = txtPerformanceText.Text != "" ? txtPerformanceText.Text : hdnSelectedPer.Value.Split('|')[1];
                    fe.venue_officer = getSelectedStringFromListBox(lstVenueManager);
                    fe.programming_ic = getSelectedStringFromListBox(lstProgrammingIC);
                    fe.production_ic = getSelectedStringFromListBox(lstProductionIC);
                    fe.el4 = txtEL4.Text;
                    fe.el5 = txtEL5.Text;
                    fe.hirer = txtHirer.Text;
                    fe.ticketing_agent = chkTicketed.Checked ? ddlTicketAgent.SelectedValue : null;
                    fe.is_draft = true;
                    fe.draft_last_saved_date = DateTime.Now;
                    fe.draft_last_saved_by = uInfo.Display_name;
                    fe.updated_by = uInfo.Display_name;
                    fe.updated_date = DateTime.Now;
                });

                aEntity.SaveChanges();
                #endregion

                if (ddlReportType.SelectedValue == "1001")
                {
                    #region Activity Details

                    // #1. Get Activity Details from control
                    string totalExpCountStr = txtTotalExposedCounts.Text;//((TextBox)ot_con_venue.FindControl("txtTotalExposedCounts")).Text;
                    string totalEngCountStr = txtTotalEngagedCounts.Text; //((TextBox)ot_con_venue.FindControl("txtTotalEngagedCounts")).Text;
                    string handiPatStr = txtHandiPat.Text; //((TextBox)ot_con_venue.FindControl("txtHandiPat")).Text;
                    string noOfUsher = txtTotalUsher.Text;// ((TextBox)ot_con_venue.FindControl("txtTotalUsher")).Text;
                    string noOfUsherRequired = txtTotalUsherRequired.Text;
                    string usherComm = txtUsherDeploymentComments.Text;// ((TextBox)ot_con_venue.FindControl("txtUsherDeploymentComments")).Text;
                    string otherComm = txtOtherComments.Text;// ((TextBox)ot_con_venue.FindControl("txtOtherComments")).Text;

                    activity_details actDetails = new activity_details();
                    actDetails.total_exposed_audience = totalExpCountStr != "" ? int.Parse(totalExpCountStr) : (Nullable<int>)null;
                    actDetails.total_engaged_audience = totalEngCountStr != "" ? int.Parse(totalEngCountStr) : (Nullable<int>)null;
                    actDetails.no_of_handicapped_patron = handiPatStr != "" ? int.Parse(handiPatStr) : (Nullable<int>)null;
                    actDetails.no_of_ushers = noOfUsher != "" ? int.Parse(noOfUsher) : (Nullable<int>)null;
                    actDetails.no_of_ushers_default = noOfUsherRequired != "" ? int.Parse(noOfUsherRequired) : (Nullable<int>)null;
                    actDetails.usher_dply_comments = usherComm;
                    actDetails.other_comments = otherComm;
                    actDetails.activity_id = am.activity_id;

                    var actExists = (from ad in aEntity.activity_details
                                     where ad.activity_id == am.activity_id
                                     select ad).SingleOrDefault();

                    if (actExists != null)
                    {
                        aEntity.activity_details.Where(ad => ad.activity_id == actExists.activity_id
                            && ad.activity_details_id == actExists.activity_details_id).ToList().ForEach(fe =>
                            {
                                fe.total_exposed_audience = totalExpCountStr != "" ? int.Parse(totalExpCountStr) : (Nullable<int>)null;
                                fe.total_engaged_audience = totalEngCountStr != "" ? int.Parse(totalEngCountStr) : (Nullable<int>)null;
                                fe.no_of_handicapped_patron = handiPatStr != "" ? int.Parse(handiPatStr) : (Nullable<int>)null;
                                fe.no_of_ushers = noOfUsher != "" ? int.Parse(noOfUsher) : (Nullable<int>)null;
                                fe.no_of_ushers_default = noOfUsherRequired != "" ? int.Parse(noOfUsherRequired) : (Nullable<int>)null;
                                fe.usher_dply_comments = usherComm;
                                fe.other_comments = otherComm;
                            });
                    }
                    else { aEntity.activity_details.Add(actDetails); }

                    aEntity.SaveChanges();
                    actDetails = aEntity.activity_details.Where(ad => ad.activity_id == am.activity_id).SingleOrDefault();
                    cFun.setSession("OutdoorActivityDetail", actDetails as object);

                    #endregion

                    // Save performance activity session
                    #region Update, Add, Delete Performance Activity

                    // Update, Save
                    List<performance_activity> perfActivityList = new List<performance_activity>();
                    if (cFun.getSession("OutdoorPerformanceList") != null)
                    {
                        perfActivityList = (List<performance_activity>)cFun.getSession("OutdoorPerformanceList");

                        if (perfActivityList != null)
                        {
                            foreach (performance_activity pa in perfActivityList)
                            {
                                // Check if it is exists in database
                                var paExists = from pad in aEntity.performance_activity
                                               where pad.per_activity_id == pa.per_activity_id &&
                                               pad.activity_id == pa.activity_id
                                               select pad;
                                if (paExists.Any()) // Exists -- do update
                                {
                                    aEntity.performance_activity.Where(pat => pat.per_activity_id == pa.per_activity_id).ToList().ForEach(fe =>
                                    {
                                        fe.EBMS_ORIG_START_TIME = ebmsFunc.getEBMSStartTimeBasedOnEBMSCombineKey(fe.EV700_EVT_ID, fe.EV700_ORG_CODE, fe.EV700_FUNC_ID);
                                        fe.start_time = pa.start_time;
                                        fe.end_time = pa.end_time;
                                        fe.activity_desc = pa.activity_desc;
                                        fe.exp_audience = pa.exp_audience;
                                        fe.eng_audience = pa.eng_audience;
                                        fe.audience_seated = pa.audience_seated;
                                        fe.other_comments = pa.other_comments;
                                        fe.crm_link = pa.crm_link;
                                        fe.updated_by = pa.updated_by;
                                        fe.updated_date = pa.updated_date;
                                    });
                                }
                                else { aEntity.performance_activity.Add(pa); } // Not Exists -- do save
                            }
                        }
                    }
                    // Delete
                    List<performance_activity> delPerfActivityList = new List<performance_activity>();
                    //delPerfActivityList = (List<performance_activity>)cFun.getSession("OutdoorPerformanceDelList");
                    if (cFun.getSession("OutdoorPerformanceDelList") != null)
                    {
                        delPerfActivityList = (List<performance_activity>)cFun.getSession("OutdoorPerformanceDelList");

                        foreach (performance_activity pa in delPerfActivityList)
                        {
                            var paExists = from pad in aEntity.performance_activity
                                           where pad.per_activity_id == pa.per_activity_id &&
                                           pad.activity_id == pa.activity_id
                                           select pad;
                            if (paExists.Any()) aEntity.performance_activity.Remove(paExists.SingleOrDefault());
                        }
                    }

                    aEntity.SaveChanges();


                    #endregion

                    #region Lost and Found

                    //Update, Save
                    List<lost_found> lfList = new List<lost_found>();
                    if (cFun.getSession("LostFoundList") != null)
                    {
                        lfList = (List<lost_found>)cFun.getSession("LostFoundList");
                        if (lfList != null)
                        {
                            foreach (lost_found lf in lfList)
                            {
                                // Check if it is exists in DB
                                var lfExists = from l in aEntity.lost_found
                                               where l.lost_found_id == lf.lost_found_id
                                               select l;
                                if (lfExists.Any())
                                {
                                    aEntity.lost_found.Where(l => l.lost_found_id == lf.lost_found_id).
                                        ToList().ForEach(fe =>
                                        {
                                            fe.lf_type_id = lf.lf_type_id;
                                            fe.activity_id = lf.activity_id;
                                            fe.item = lf.item;
                                            fe.located_at = lf.located_at;
                                            fe.tag_no = lf.tag_no;
                                            fe.action_taken = lf.action_taken;
                                            fe.crm_link = lf.crm_link;
                                            lf.updated_by = lf.updated_by;
                                            lf.updated_date = lf.updated_date;
                                        });
                                }
                                else { aEntity.lost_found.Add(lf); }
                            }
                        }
                    }

                    // Delete
                    List<lost_found> delLFList = new List<lost_found>();
                    if (cFun.getSession("LostFoundDelList") != null)
                    {
                        delLFList = (List<lost_found>)cFun.getSession("LostFoundDelList");
                        foreach (lost_found lf in delLFList)
                        {
                            // Check if it is exists in DB
                            var lfExists = from l in aEntity.lost_found
                                           where l.lost_found_id == lf.lost_found_id
                                           select l;
                            if (lfExists.Any())
                            {
                                aEntity.lost_found.Remove(lfExists.SingleOrDefault());
                            }
                        }
                    }

                    aEntity.SaveChanges();

                    #endregion

                    #region Defects

                    // Update, Save
                    List<defect> defectList = new List<defect>();
                    if (cFun.getSession("DefectList") != null)
                    {
                        defectList = (List<defect>)cFun.getSession("DefectList");
                        if (defectList != null)
                        {
                            foreach (defect de in defectList)
                            {
                                // Check if it is exists in DB
                                var deExists = from d in aEntity.defects
                                               where d.defect_id == de.defect_id
                                               select d;
                                if (deExists.Any())
                                {
                                    aEntity.defects.Where(w => w.defect_id == de.defect_id).ToList().ForEach(fe =>
                                        {
                                            fe.activity_id = de.activity_id;
                                            fe.defect1 = de.defect1;
                                            fe.located_at = de.located_at;
                                            fe.action_taken = de.action_taken;
                                            fe.remarks = de.remarks;
                                            fe.bms_id = de.bms_id;
                                            fe.updated_by = de.updated_by;
                                            fe.updated_date = de.updated_date;
                                        });
                                }
                                else
                                {
                                    aEntity.defects.Add(de);
                                }
                            }
                        }
                    }

                    //Delete
                    List<defect> delDefList = new List<defect>();
                    if (cFun.getSession("DefectDelList") != null)
                    {
                        delDefList = (List<defect>)cFun.getSession("DefectDelList");

                        foreach (defect de in delDefList)
                        {
                            // Check if it is exists in DB
                            var deExists = from d in aEntity.defects
                                           where d.defect_id == de.defect_id
                                           select d;
                            if (deExists.Any())
                            {
                                aEntity.defects.Remove(deExists.SingleOrDefault());
                            }
                        }
                    }

                    aEntity.SaveChanges();

                    #endregion
                }
                Response.Redirect("ot_con_activity_report.aspx?activity=" + am.activity_id + "&sta=sa", false);
            }
        }
        catch (Exception ex)
        {
            lblMessage.CssClass = "errorMessage";
            lblMessage.Text = "An error has occured. " + ex.Message;
            writeLog.log(ex, "OT_CON > Save Draft");
        }
    }

    #region Outdoor Event

    /// <summary>
    /// Bind Activity data grid. If param is null, get data from database.
    /// </summary>
    /// <param name="pList">Performance list (Nullable)</param>
    private void BindActivityDataGrid(List<performance_activity> pList = null, Nullable<int> activityID = null, Nullable<int> activityTypeID = null)
    {
        try
        {
            if (pList == null)
            {
                List<performance_activity> perfList = new List<performance_activity>();

                if (cFun.getSession("OutdoorPerformanceList") != null)
                    perfList = (List<performance_activity>)cFun.getSession("OutdoorPerformanceList");
                else
                    perfList = ebmsFunc.getEbmsPerformanceListByEventID(hdnEventID.Value, hdnEventDateTime.Value,
                        activityID, activityTypeID);

                cFun.setSession("OutdoorPerformanceList", perfList as object);
                gridviewActivity.DataSource = perfList;
                gridviewActivity.DataBind();
            }
            else
            {
                cFun.setSession("OutdoorPerformanceList", pList as object);
                gridviewActivity.DataSource = pList;
                gridviewActivity.DataBind();
            }
        }
        catch (Exception ex)
        {
            lblMessage.CssClass = "errorMessage";
            lblMessage.Text = "An error occured. " + ex.Message;
            throw ex;
        }
    }

    protected void gridviewActivity_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            /*Literal ltrStartDT = (Literal)e.Row.FindControl("ltrStartDT");
            Literal ltrStartTime = (Literal)e.Row.FindControl("ltrStartTime");
            ltrStartTime.Text = DateTime.Parse(ltrStartDT.Text).ToString("hh:mm tt");

            Literal ltrEndDT = (Literal)e.Row.FindControl("ltrEndDT");
            Literal ltrEndTime = (Literal)e.Row.FindControl("ltrEndTime");
            ltrEndTime.Text = DateTime.Parse(ltrEndDT.Text).ToString("hh:mm tt");*/

            Literal ltrPerActID = (Literal)e.Row.FindControl("ltrRowID");
            CheckBox chkCheck = (CheckBox)e.Row.FindControl("chkCheck");
            chkCheck.Attributes.Add("onclick", "javascript:setCheckedActivityID(" + ltrPerActID.Text + ");");

            LinkButton lnkbtnEdit = (LinkButton)e.Row.FindControl("lnkbtnEdit");

            if (hdnDisableButton.Value == "True")
                lnkbtnEdit.Enabled = false;

            #region CRM Link
            HyperLink hlCRMLink = (HyperLink)e.Row.FindControl("lnkCRMLink");
            Literal ltrCRMLink = (Literal)e.Row.FindControl("ltrCRMLink");
            if (ltrCRMLink.Text != "")
            {
                hlCRMLink.ImageUrl = "./Images/ExpandBlack.png";
                hlCRMLink.ImageHeight = Unit.Pixel(17);
                hlCRMLink.ImageWidth = Unit.Pixel(17);
                hlCRMLink.NavigateUrl = ltrCRMLink.Text;
                hlCRMLink.Target = "_blank";
                hlCRMLink.ToolTip = "Click to open link to CRM";
            }
            #endregion
        }
    }

    protected void gridviewActivity_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        List<performance_activity> perfList = new List<performance_activity>();
        try
        {
            #region Edit section
            if (e.CommandName == "EditRow")
            {
                if (cFun.getSession("OutdoorPerformanceList") != null)
                    perfList = (List<performance_activity>)cFun.getSession("OutdoorPerformanceList");

                if (perfList != null)
                {
                    performance_activity pa = new performance_activity();
                    int index = Convert.ToInt32(e.CommandArgument);
                    GridViewRow gRow = gridviewActivity.Rows[index];
                    int rowID = int.Parse(((Literal)gRow.FindControl("ltrRowID")).Text);
                    string activityDesc = ((Literal)gRow.FindControl("ltrPerformance")).Text;
                    pa = (from pf in perfList
                          where pf.per_activity_id == rowID //&& pf.activity_desc == activityDesc
                          select pf).SingleOrDefault();

                    txtPerformance.Text = pa.activity_desc;
                    txtStartTime.Text = pa.start_time;
                    txtEndTime.Text = pa.end_time;
                    txtExposedCount.Text = pa.exp_audience.HasValue ? pa.exp_audience.Value.ToString() : "";
                    txtEngagedCount.Text = pa.eng_audience.HasValue ? pa.eng_audience.Value.ToString() : "";
                    txtAudienceSeated.Text = pa.audience_seated.HasValue ? pa.audience_seated.Value.ToString() : "";
                    txtPerformanceComments.Text = pa.other_comments;
                    txtCRMLink.Text = pa.crm_link;
                    hdnRowID.Value = pa.per_activity_id.ToString();
                    popupActivity.Show();
                }
            }
            #endregion
        }
        catch (Exception ex)
        {
            lblMessage.CssClass = "errorMessage";
            lblMessage.Text = "An error occured. " + ex.Message;
            throw ex;
        }
    }

    protected void btnSavePer_Click(object sender, EventArgs e)
    {
        ClientScriptManager csManager = Page.ClientScript;
        Type csType = this.GetType();
        //List<ebms_performance_list> perfList = new List<ebms_performance_list>();
        List<performance_activity> perfList = new List<performance_activity>();
        if (cFun.getSession("LogonUser") != null) uInfo = (UserInfo)cFun.getSession("LogonUser");
        uInfo = cFun.getLogonUser(Environment.UserName);

        try
        {
            if (cFun.getSession("OutdoorPerformanceList") != null)
                perfList = (List<performance_activity>)cFun.getSession("OutdoorPerformanceList");

            if (hdnRowID.Value != "") // Update
            {
                #region Update
                int rowID = int.Parse(hdnRowID.Value);

                perfList.Where(pl => pl.per_activity_id == rowID).ToList().ForEach(elist =>
                {
                    elist.activity_desc = txtPerformance.Text;
                    elist.start_time = txtStartTime.Text;
                    elist.end_time = txtEndTime.Text;
                    elist.exp_audience = txtExposedCount.Text != "" ? int.Parse(txtExposedCount.Text) : (Nullable<int>)null;
                    elist.eng_audience = txtEngagedCount.Text != "" ? int.Parse(txtEngagedCount.Text) : (Nullable<int>)null;
                    elist.audience_seated = txtAudienceSeated.Text != "" ? int.Parse(txtAudienceSeated.Text) : (Nullable<int>)null;
                    elist.other_comments = txtPerformanceComments.Text;
                    elist.crm_link = txtCRMLink.Text;
                    elist.updated_by = uInfo.Display_name;
                    elist.updated_date = DateTime.Now;
                });
                #endregion
            }
            else // New record
            {
                #region New Save
                Random rd = new Random();
                performance_activity elist = new performance_activity();
                elist.start_time = txtStartTime.Text;
                elist.end_time = txtEndTime.Text;
                elist.activity_desc = txtPerformance.Text;
                elist.exp_audience = txtExposedCount.Text != "" ? int.Parse(txtExposedCount.Text) : (Nullable<int>)null;
                elist.eng_audience = txtEngagedCount.Text != "" ? int.Parse(txtEngagedCount.Text) : (Nullable<int>)null;
                elist.audience_seated = txtAudienceSeated.Text != "" ? int.Parse(txtAudienceSeated.Text) : (Nullable<int>)null;
                elist.other_comments = txtPerformanceComments.Text;
                elist.crm_link = txtCRMLink.Text;
                elist.activity_id = int.Parse(hdnActivityID.Value);
                elist.activity_type_id = int.Parse(hdnActivityTypeID.Value);
                elist.per_activity_id = rd.Next();
                perfList.Add(elist);
                #endregion
            }

            // Exposed count
            var expList = from pl in perfList
                          where pl.exp_audience != null
                          select new { exp = pl.exp_audience };

            // Engaged count
            var engList = from pl in perfList
                          where pl.eng_audience != null
                          select new { eng = pl.eng_audience };

            txtTotalExposedCounts.Text = expList.Sum(el => el.exp).ToString();
            txtTotalEngagedCounts.Text = engList.Sum(el => el.eng).ToString();

            hdnRowID.Value = "";
            BindActivityDataGrid(perfList);
            ClearActivityForm();
            popupActivity.Hide();
        }
        catch (Exception ex)
        {
            lblMessage.CssClass = "errorMessage";
            lblMessage.Text = "An error occured. " + ex.Message;
            throw ex;
        }
    }

    private void ClearActivityForm()
    {
        try
        {
            hdnRowID.Value = "";
            txtPerformance.Text = "";
            txtStartTime.Text = "";
            txtEndTime.Text = "";
            txtExposedCount.Text = "";
            txtEngagedCount.Text = "";
            txtAudienceSeated.Text = "";
            txtPerformanceComments.Text = "";
            txtCRMLink.Text = "";
        }
        catch (Exception ex) { throw ex; }
    }

    protected void lnkPerRemove_Click(object sender, EventArgs e)
    {
        try
        {
            string[] perActivityIDArray = hdnSelActID.Value != "" ? hdnSelActID.Value.Split(',') : null;

            List<performance_activity> perfActivityList = new List<performance_activity>();
            List<performance_activity> delPerfActivityList = new List<performance_activity>();
            perfActivityList = cFun.getSession("OutdoorPerformanceList") != null ? (List<performance_activity>)cFun.getSession("OutdoorPerformanceList") : null;
            delPerfActivityList = cFun.getSession("OutdoorPerformanceDelList") != null ? (List<performance_activity>)cFun.getSession("OutdoorPerformanceDelList") : null;

            if (perActivityIDArray.Length > 0)
            {
                foreach (string pactID in perActivityIDArray)
                {
                    if (pactID != "")
                    {
                        int pactIDInt = int.Parse(pactID);
                        performance_activity perAct = new performance_activity();
                        perAct = (from pa in perfActivityList
                                  where pa.per_activity_id == pactIDInt
                                  select pa).SingleOrDefault();

                        if (delPerfActivityList == null)
                        {
                            delPerfActivityList = new List<performance_activity>();
                            delPerfActivityList.Add(perAct);
                        }
                        else { delPerfActivityList.Add(perAct); }
                        perfActivityList.Remove(perAct);
                    }
                }
                cFun.setSession("OutdoorPerformanceList", perfActivityList as object);
                cFun.setSession("OutdoorPerformanceDelList", delPerfActivityList as object);
            }
            BindActivityDataGrid(perfActivityList);
        }
        catch (Exception ex)
        {
            throw ex;
            writeLog.log(ex, "ot_con_activity_report.aspx > lnkPerRemove_Click");
        }
    }

    protected void btnCancelPer_Click(object sender, EventArgs e)
    {
        ClearActivityForm();
        popupActivity.Hide();
    }

    #endregion

    /// <summary>
    /// Submit to Duty Manager
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            if (chkTicketed.Checked) if (ddlTicketAgent.SelectedValue == "") { cvTicketAgent.IsValid = false; }
            if (Page.IsValid && cvTicketAgent.IsValid)
            {
                hdnSelectedPer.Value = ddlPerformance.SelectedValue + "|" + ddlPerformance.SelectedItem.Text;
                hdnSelectedPerDate.Value = txtDateOfReport.Text;

                if (cFun.getSession("LogonUser") != null) uInfo = (UserInfo)cFun.getSession("LogonUser");
                else uInfo = cFun.getLogonUser(Environment.UserName);

                #region Save Main Activity
                activity_main am = new activity_main();
                am = (activity_main)cFun.getSession("MainActivity");

                aEntity.activity_main.Where(a => a.activity_id == am.activity_id).ToList().ForEach(fe =>
                {
                    fe.company = ddlCompany.SelectedValue;
                    fe.venue = ddlVenue.SelectedValue;
                    fe.report_type_id = int.Parse(ddlReportType.SelectedValue);
                    fe.date_of_report = txtDateOfReport.Text != "" ? DateTime.Parse(txtDateOfReport.Text) : (Nullable<DateTime>)null;
                    fe.scheduled_start_time = txtScheduledStartTime.Text;
                    string per1 = hdnSelectedPer.Value; //Request.Form[ddlPerformance.ClientID];
                    fe.performance_1 = hdnSelectedPer.Value.Split('|')[1]; // PREV CODE
                    fe.performance_2 = txtPerformanceText.Text != "" ? txtPerformanceText.Text : hdnSelectedPer.Value.Split('|')[1];
                    fe.venue_officer = getSelectedStringFromListBox(lstVenueManager);
                    fe.programming_ic = getSelectedStringFromListBox(lstProgrammingIC);
                    fe.production_ic = getSelectedStringFromListBox(lstProductionIC);
                    fe.el4 = txtEL4.Text;
                    fe.el5 = txtEL5.Text;
                    fe.hirer = txtHirer.Text;
                    fe.ticketing_agent = chkTicketed.Checked ? ddlTicketAgent.SelectedValue : null;
                    fe.is_draft = true;
                    fe.draft_last_saved_date = DateTime.Now;
                    fe.draft_last_saved_by = uInfo.Display_name;
                    fe.is_submitted = true;
                    fe.submitted_user_id = uInfo.User_id;
                    fe.submitted_by = uInfo.Display_name;
                    fe.submitted_date = DateTime.Now;
                    fe.is_approved = (Nullable<bool>)null;
                    fe.approved_by = "";
                    fe.approved_date = (Nullable<DateTime>)null;
                    fe.updated_by = uInfo.Display_name;
                    fe.updated_date = DateTime.Now;
                });

                aEntity.SaveChanges();
                #endregion

                if (ddlReportType.SelectedValue == "1001")
                {
                    #region Activity Details

                    // #1. Get Activity Details from control
                    string totalExpCountStr = txtTotalExposedCounts.Text;//((TextBox)ot_con_venue.FindControl("txtTotalExposedCounts")).Text;
                    string totalEngCountStr = txtTotalEngagedCounts.Text; //((TextBox)ot_con_venue.FindControl("txtTotalEngagedCounts")).Text;
                    string handiPatStr = txtHandiPat.Text; //((TextBox)ot_con_venue.FindControl("txtHandiPat")).Text;
                    string noOfUsherRequired = txtTotalUsherRequired.Text;
                    string noOfUsher = txtTotalUsher.Text;// ((TextBox)ot_con_venue.FindControl("txtTotalUsher")).Text;
                    string usherComm = txtUsherDeploymentComments.Text;// ((TextBox)ot_con_venue.FindControl("txtUsherDeploymentComments")).Text;
                    string otherComm = txtOtherComments.Text;// ((TextBox)ot_con_venue.FindControl("txtOtherComments")).Text;

                    activity_details actDetails = new activity_details();
                    actDetails.total_exposed_audience = totalExpCountStr != "" ? int.Parse(totalExpCountStr) : (Nullable<int>)null;
                    actDetails.total_engaged_audience = totalEngCountStr != "" ? int.Parse(totalEngCountStr) : (Nullable<int>)null;
                    actDetails.no_of_handicapped_patron = handiPatStr != "" ? int.Parse(handiPatStr) : (Nullable<int>)null;
                    actDetails.no_of_ushers = noOfUsher != "" ? int.Parse(noOfUsher) : (Nullable<int>)null;
                    actDetails.no_of_ushers_default = noOfUsherRequired != "" ? int.Parse(noOfUsherRequired) : (Nullable<int>)null;
                    actDetails.usher_dply_comments = usherComm;
                    actDetails.other_comments = otherComm;
                    actDetails.activity_id = am.activity_id;

                    var actExists = (from ad in aEntity.activity_details
                                     where ad.activity_id == am.activity_id
                                     select ad).SingleOrDefault();

                    if (actExists != null)
                    {
                        aEntity.activity_details.Where(ad => ad.activity_id == actExists.activity_id
                            && ad.activity_details_id == actExists.activity_details_id).ToList().ForEach(fe =>
                            {
                                fe.total_exposed_audience = totalExpCountStr != "" ? int.Parse(totalExpCountStr) : (Nullable<int>)null;
                                fe.total_engaged_audience = totalEngCountStr != "" ? int.Parse(totalEngCountStr) : (Nullable<int>)null;
                                fe.no_of_handicapped_patron = handiPatStr != "" ? int.Parse(handiPatStr) : (Nullable<int>)null;
                                fe.no_of_ushers = noOfUsher != "" ? int.Parse(noOfUsher) : (Nullable<int>)null;
                                fe.no_of_ushers_default = noOfUsherRequired != "" ? int.Parse(noOfUsherRequired) : (Nullable<int>)null;
                                fe.usher_dply_comments = usherComm;
                                fe.other_comments = otherComm;
                            });
                    }
                    else { aEntity.activity_details.Add(actDetails); }

                    aEntity.SaveChanges();
                    actDetails = aEntity.activity_details.Where(ad => ad.activity_id == am.activity_id).SingleOrDefault();
                    cFun.setSession("OutdoorActivityDetail", actDetails as object);

                    #endregion

                    // Save performance activity session
                    #region Update, Add, Delete Performance Activity

                    // Update, Save
                    List<performance_activity> perfActivityList = new List<performance_activity>();
                    if (cFun.getSession("OutdoorPerformanceList") != null)
                    {
                        perfActivityList = (List<performance_activity>)cFun.getSession("OutdoorPerformanceList");

                        if (perfActivityList != null)
                        {
                            foreach (performance_activity pa in perfActivityList)
                            {
                                // Check if it is exists in database
                                var paExists = from pad in aEntity.performance_activity
                                               where pad.per_activity_id == pa.per_activity_id &&
                                               pad.activity_id == pa.activity_id
                                               select pad;
                                if (paExists.Any()) // Exists -- do update
                                {
                                    aEntity.performance_activity.Where(pat => pat.per_activity_id == pa.per_activity_id).ToList().ForEach(fe =>
                                    {
                                        fe.EBMS_ORIG_START_TIME = ebmsFunc.getEBMSStartTimeBasedOnEBMSCombineKey(fe.EV700_EVT_ID, fe.EV700_ORG_CODE, fe.EV700_FUNC_ID);
                                        fe.start_time = pa.start_time;
                                        fe.end_time = pa.end_time;
                                        fe.activity_desc = pa.activity_desc;
                                        fe.exp_audience = pa.exp_audience;
                                        fe.eng_audience = pa.eng_audience;
                                        fe.audience_seated = pa.audience_seated;
                                        fe.other_comments = pa.other_comments;
                                        fe.crm_link = pa.crm_link;
                                        fe.updated_by = pa.updated_by;
                                        fe.updated_date = pa.updated_date;
                                    });
                                }
                                else { aEntity.performance_activity.Add(pa); } // Not Exists -- do save
                            }
                        }
                    }
                    // Delete
                    List<performance_activity> delPerfActivityList = new List<performance_activity>();
                    //delPerfActivityList = (List<performance_activity>)cFun.getSession("OutdoorPerformanceDelList");
                    if (cFun.getSession("OutdoorPerformanceDelList") != null)
                    {
                        delPerfActivityList = (List<performance_activity>)cFun.getSession("OutdoorPerformanceDelList");

                        foreach (performance_activity pa in delPerfActivityList)
                        {
                            var paExists = from pad in aEntity.performance_activity
                                           where pad.per_activity_id == pa.per_activity_id &&
                                           pad.activity_id == pa.activity_id
                                           select pad;
                            if (paExists.Any()) aEntity.performance_activity.Remove(paExists.SingleOrDefault());
                        }
                    }

                    aEntity.SaveChanges();


                    #endregion

                    #region Lost and Found

                    //Update, Save
                    List<lost_found> lfList = new List<lost_found>();
                    if (cFun.getSession("LostFoundList") != null)
                    {
                        lfList = (List<lost_found>)cFun.getSession("LostFoundList");
                        if (lfList != null)
                        {
                            foreach (lost_found lf in lfList)
                            {
                                // Check if it is exists in DB
                                var lfExists = from l in aEntity.lost_found
                                               where l.lost_found_id == lf.lost_found_id
                                               select l;
                                if (lfExists.Any())
                                {
                                    aEntity.lost_found.Where(l => l.lost_found_id == lf.lost_found_id).
                                        ToList().ForEach(fe =>
                                        {
                                            fe.lf_type_id = lf.lf_type_id;
                                            fe.activity_id = lf.activity_id;
                                            fe.item = lf.item;
                                            fe.located_at = lf.located_at;
                                            fe.tag_no = lf.tag_no;
                                            fe.action_taken = lf.action_taken;
                                            fe.crm_link = lf.crm_link;
                                            lf.updated_by = lf.updated_by;
                                            lf.updated_date = lf.updated_date;
                                        });
                                }
                                else { aEntity.lost_found.Add(lf); }
                            }
                        }
                    }

                    // Delete
                    List<lost_found> delLFList = new List<lost_found>();
                    if (cFun.getSession("LostFoundDelList") != null)
                    {
                        delLFList = (List<lost_found>)cFun.getSession("LostFoundDelList");
                        foreach (lost_found lf in delLFList)
                        {
                            // Check if it is exists in DB
                            var lfExists = from l in aEntity.lost_found
                                           where l.lost_found_id == lf.lost_found_id
                                           select l;
                            if (lfExists.Any())
                            {
                                aEntity.lost_found.Remove(lfExists.SingleOrDefault());
                            }
                        }
                    }

                    aEntity.SaveChanges();

                    #endregion

                    #region Defects

                    // Update, Save
                    List<defect> defectList = new List<defect>();
                    if (cFun.getSession("DefectList") != null)
                    {
                        defectList = (List<defect>)cFun.getSession("DefectList");
                        if (defectList != null)
                        {
                            foreach (defect de in defectList)
                            {
                                // Check if it is exists in DB
                                var deExists = from d in aEntity.defects
                                               where d.defect_id == de.defect_id
                                               select d;
                                if (deExists.Any())
                                {
                                    aEntity.defects.Where(w => w.defect_id == de.defect_id).ToList().ForEach(fe =>
                                    {
                                        fe.activity_id = de.activity_id;
                                        fe.defect1 = de.defect1;
                                        fe.located_at = de.located_at;
                                        fe.action_taken = de.action_taken;
                                        fe.remarks = de.remarks;
                                        fe.bms_id = de.bms_id;
                                        fe.updated_by = de.updated_by;
                                        fe.updated_date = de.updated_date;
                                    });
                                }
                                else
                                {
                                    aEntity.defects.Add(de);
                                }
                            }
                        }
                    }

                    //Delete
                    List<defect> delDefList = new List<defect>();
                    if (cFun.getSession("DefectDelList") != null)
                    {
                        delDefList = (List<defect>)cFun.getSession("DefectDelList");

                        foreach (defect de in delDefList)
                        {
                            // Check if it is exists in DB
                            var deExists = from d in aEntity.defects
                                           where d.defect_id == de.defect_id
                                           select d;
                            if (deExists.Any())
                            {
                                aEntity.defects.Remove(deExists.SingleOrDefault());
                            }
                        }
                    }

                    aEntity.SaveChanges();

                    #endregion
                }

                #region Send to Duty Manager ### WIP ###
                /*
                 * Commented on 29 December 2015 by Htin Linn Ko
                 * Please refer to IT Change request sytem and ReadMe.txt
                 */ 
                /*
                string urlString = "ot_con_activity_report.aspx?activity=" + am.activity_id;
                string reportStr = am.date_of_report.Value.DayOfWeek.ToString() + ", " +
                    am.date_of_report.Value.ToString("dd MMM yyyy") + " at " + ebmsFunc.getVenueLongName(am.venue);

                email eml = new email();
                eml.sendSubmitEmailToDutyManager(email.activityURL + urlString, "Activity Report for " + reportStr, reportStr);
                */
                /* DONE */
                #endregion

                Response.Redirect("ot_con_activity_report.aspx?activity=" + am.activity_id + "&sta=su", false);
            }
        }
        catch (Exception ex)
        {
            lblMessage.CssClass = "errorMessage";
            lblMessage.Text = "An error occured. " + ex.Message;
            writeLog.log(ex);
            throw ex;
        }
    }

    protected void btnComplete_Click(object sender, EventArgs e)
    {
        // Complete action
        try
        {
            if (chkTicketed.Checked) if (ddlTicketAgent.SelectedValue == "") { cvTicketAgent.IsValid = false; }
            if (Page.IsValid && cvTicketAgent.IsValid)
            {
                hdnSelectedPer.Value = ddlPerformance.SelectedValue + "|" + ddlPerformance.SelectedItem.Text;
                hdnSelectedPerDate.Value = txtDateOfReport.Text;

                if (cFun.getSession("LogonUser") != null) uInfo = (UserInfo)cFun.getSession("LogonUser");
                else uInfo = cFun.getLogonUser(Environment.UserName);

                #region Save Main Activity
                activity_main am = new activity_main();
                am = (activity_main)cFun.getSession("MainActivity");

                aEntity.activity_main.Where(a => a.activity_id == am.activity_id).ToList().ForEach(fe =>
                {
                    fe.company = ddlCompany.SelectedValue;
                    fe.venue = ddlVenue.SelectedValue;
                    fe.report_type_id = int.Parse(ddlReportType.SelectedValue);
                    fe.date_of_report = txtDateOfReport.Text != "" ? DateTime.Parse(txtDateOfReport.Text) : (Nullable<DateTime>)null;
                    fe.scheduled_start_time = txtScheduledStartTime.Text;
                    string per1 = hdnSelectedPer.Value; //Request.Form[ddlPerformance.ClientID];
                    fe.performance_1 = hdnSelectedPer.Value.Split('|')[1]; // PREV CODE
                    fe.performance_2 = txtPerformanceText.Text != "" ? txtPerformanceText.Text : hdnSelectedPer.Value.Split('|')[1];
                    fe.venue_officer = getSelectedStringFromListBox(lstVenueManager);
                    fe.programming_ic = getSelectedStringFromListBox(lstProgrammingIC);
                    fe.production_ic = getSelectedStringFromListBox(lstProductionIC);
                    fe.el4 = txtEL4.Text;
                    fe.el5 = txtEL5.Text;
                    fe.hirer = txtHirer.Text;
                    fe.ticketing_agent = chkTicketed.Checked ? ddlTicketAgent.SelectedValue : null;
                    /*fe.is_draft = true;
                    fe.draft_last_saved_date = DateTime.Now;
                    fe.draft_last_saved_by = uInfo.Display_name;
                    fe.is_submitted = true;
                    fe.submitted_by = uInfo.Display_name;
                    fe.submitted_date = DateTime.Now;*/
                    fe.is_approved = true;
                    fe.approved_by = uInfo.Display_name;
                    fe.approved_date = DateTime.Now;
                    fe.updated_by = uInfo.Display_name;
                    fe.updated_date = DateTime.Now;
                });

                aEntity.SaveChanges();
                #endregion

                if (ddlReportType.SelectedValue == "1001")
                {
                    #region Activity Details

                    // #1. Get Activity Details from control
                    string totalExpCountStr = txtTotalExposedCounts.Text;//((TextBox)ot_con_venue.FindControl("txtTotalExposedCounts")).Text;
                    string totalEngCountStr = txtTotalEngagedCounts.Text; //((TextBox)ot_con_venue.FindControl("txtTotalEngagedCounts")).Text;
                    string handiPatStr = txtHandiPat.Text; //((TextBox)ot_con_venue.FindControl("txtHandiPat")).Text;
                    string noOfUsherRequired = txtTotalUsherRequired.Text;
                    string noOfUsher = txtTotalUsher.Text;// ((TextBox)ot_con_venue.FindControl("txtTotalUsher")).Text;
                    string usherComm = txtUsherDeploymentComments.Text;// ((TextBox)ot_con_venue.FindControl("txtUsherDeploymentComments")).Text;
                    string otherComm = txtOtherComments.Text;// ((TextBox)ot_con_venue.FindControl("txtOtherComments")).Text;

                    activity_details actDetails = new activity_details();
                    actDetails.total_exposed_audience = totalExpCountStr != "" ? int.Parse(totalExpCountStr) : (Nullable<int>)null;
                    actDetails.total_engaged_audience = totalEngCountStr != "" ? int.Parse(totalEngCountStr) : (Nullable<int>)null;
                    actDetails.no_of_handicapped_patron = handiPatStr != "" ? int.Parse(handiPatStr) : (Nullable<int>)null;
                    actDetails.no_of_ushers = noOfUsher != "" ? int.Parse(noOfUsher) : (Nullable<int>)null;
                    actDetails.no_of_ushers_default = noOfUsherRequired != "" ? int.Parse(noOfUsherRequired) : (Nullable<int>)null;
                    actDetails.usher_dply_comments = usherComm;
                    actDetails.other_comments = otherComm;
                    actDetails.activity_id = am.activity_id;

                    var actExists = (from ad in aEntity.activity_details
                                     where ad.activity_id == am.activity_id
                                     select ad).SingleOrDefault();

                    if (actExists != null)
                    {
                        aEntity.activity_details.Where(ad => ad.activity_id == actExists.activity_id
                            && ad.activity_details_id == actExists.activity_details_id).ToList().ForEach(fe =>
                            {
                                fe.total_exposed_audience = totalExpCountStr != "" ? int.Parse(totalExpCountStr) : (Nullable<int>)null;
                                fe.total_engaged_audience = totalEngCountStr != "" ? int.Parse(totalEngCountStr) : (Nullable<int>)null;
                                fe.no_of_handicapped_patron = handiPatStr != "" ? int.Parse(handiPatStr) : (Nullable<int>)null;
                                fe.no_of_ushers = noOfUsher != "" ? int.Parse(noOfUsher) : (Nullable<int>)null;
                                fe.no_of_ushers_default = noOfUsherRequired != "" ? int.Parse(noOfUsherRequired) : (Nullable<int>)null;
                                fe.usher_dply_comments = usherComm;
                                fe.other_comments = otherComm;
                            });
                    }
                    else { aEntity.activity_details.Add(actDetails); }

                    aEntity.SaveChanges();
                    actDetails = aEntity.activity_details.Where(ad => ad.activity_id == am.activity_id).SingleOrDefault();
                    cFun.setSession("OutdoorActivityDetail", actDetails as object);

                    #endregion

                    // Save performance activity session
                    #region Update, Add, Delete Performance Activity

                    // Update, Save
                    List<performance_activity> perfActivityList = new List<performance_activity>();
                    if (cFun.getSession("OutdoorPerformanceList") != null)
                    {
                        perfActivityList = (List<performance_activity>)cFun.getSession("OutdoorPerformanceList");

                        if (perfActivityList != null)
                        {
                            foreach (performance_activity pa in perfActivityList)
                            {
                                // Check if it is exists in database
                                var paExists = from pad in aEntity.performance_activity
                                               where pad.per_activity_id == pa.per_activity_id &&
                                               pad.activity_id == pa.activity_id
                                               select pad;
                                if (paExists.Any()) // Exists -- do update
                                {
                                    aEntity.performance_activity.Where(pat => pat.per_activity_id == pa.per_activity_id).ToList().ForEach(fe =>
                                    {
                                        fe.EBMS_ORIG_START_TIME = ebmsFunc.getEBMSStartTimeBasedOnEBMSCombineKey(fe.EV700_EVT_ID, fe.EV700_ORG_CODE, fe.EV700_FUNC_ID);
                                        fe.start_time = pa.start_time;
                                        fe.end_time = pa.end_time;
                                        fe.activity_desc = pa.activity_desc;
                                        fe.exp_audience = pa.exp_audience;
                                        fe.eng_audience = pa.eng_audience;
                                        fe.audience_seated = pa.audience_seated;
                                        fe.other_comments = pa.other_comments;
                                        fe.crm_link = pa.crm_link;
                                        fe.updated_by = pa.updated_by;
                                        fe.updated_date = pa.updated_date;
                                    });
                                }
                                else { aEntity.performance_activity.Add(pa); } // Not Exists -- do save
                            }
                        }
                    }
                    // Delete
                    List<performance_activity> delPerfActivityList = new List<performance_activity>();
                    //delPerfActivityList = (List<performance_activity>)cFun.getSession("OutdoorPerformanceDelList");
                    if (cFun.getSession("OutdoorPerformanceDelList") != null)
                    {
                        delPerfActivityList = (List<performance_activity>)cFun.getSession("OutdoorPerformanceDelList");

                        foreach (performance_activity pa in delPerfActivityList)
                        {
                            var paExists = from pad in aEntity.performance_activity
                                           where pad.per_activity_id == pa.per_activity_id &&
                                           pad.activity_id == pa.activity_id
                                           select pad;
                            if (paExists.Any()) aEntity.performance_activity.Remove(paExists.SingleOrDefault());
                        }
                    }

                    aEntity.SaveChanges();


                    #endregion

                    #region Lost and Found

                    //Update, Save
                    List<lost_found> lfList = new List<lost_found>();
                    if (cFun.getSession("LostFoundList") != null)
                    {
                        lfList = (List<lost_found>)cFun.getSession("LostFoundList");
                        if (lfList != null)
                        {
                            foreach (lost_found lf in lfList)
                            {
                                // Check if it is exists in DB
                                var lfExists = from l in aEntity.lost_found
                                               where l.lost_found_id == lf.lost_found_id
                                               select l;
                                if (lfExists.Any())
                                {
                                    aEntity.lost_found.Where(l => l.lost_found_id == lf.lost_found_id).
                                        ToList().ForEach(fe =>
                                        {
                                            fe.lf_type_id = lf.lf_type_id;
                                            fe.activity_id = lf.activity_id;
                                            fe.item = lf.item;
                                            fe.located_at = lf.located_at;
                                            fe.tag_no = lf.tag_no;
                                            fe.action_taken = lf.action_taken;
                                            fe.crm_link = lf.crm_link;
                                            lf.updated_by = lf.updated_by;
                                            lf.updated_date = lf.updated_date;
                                        });
                                }
                                else { aEntity.lost_found.Add(lf); }
                            }
                        }
                    }

                    // Delete
                    List<lost_found> delLFList = new List<lost_found>();
                    if (cFun.getSession("LostFoundDelList") != null)
                    {
                        delLFList = (List<lost_found>)cFun.getSession("LostFoundDelList");
                        foreach (lost_found lf in delLFList)
                        {
                            // Check if it is exists in DB
                            var lfExists = from l in aEntity.lost_found
                                           where l.lost_found_id == lf.lost_found_id
                                           select l;
                            if (lfExists.Any())
                            {
                                aEntity.lost_found.Remove(lfExists.SingleOrDefault());
                            }
                        }
                    }

                    aEntity.SaveChanges();

                    #endregion

                    #region Defects

                    // Update, Save
                    List<defect> defectList = new List<defect>();
                    if (cFun.getSession("DefectList") != null)
                    {
                        defectList = (List<defect>)cFun.getSession("DefectList");
                        if (defectList != null)
                        {
                            foreach (defect de in defectList)
                            {
                                // Check if it is exists in DB
                                var deExists = from d in aEntity.defects
                                               where d.defect_id == de.defect_id
                                               select d;
                                if (deExists.Any())
                                {
                                    aEntity.defects.Where(w => w.defect_id == de.defect_id).ToList().ForEach(fe =>
                                    {
                                        fe.activity_id = de.activity_id;
                                        fe.defect1 = de.defect1;
                                        fe.located_at = de.located_at;
                                        fe.action_taken = de.action_taken;
                                        fe.remarks = de.remarks;
                                        fe.bms_id = de.bms_id;
                                        fe.updated_by = de.updated_by;
                                        fe.updated_date = de.updated_date;
                                    });
                                }
                                else
                                {
                                    aEntity.defects.Add(de);
                                }
                            }
                        }
                    }

                    //Delete
                    List<defect> delDefList = new List<defect>();
                    if (cFun.getSession("DefectDelList") != null)
                    {
                        delDefList = (List<defect>)cFun.getSession("DefectDelList");

                        foreach (defect de in delDefList)
                        {
                            // Check if it is exists in DB
                            var deExists = from d in aEntity.defects
                                           where d.defect_id == de.defect_id
                                           select d;
                            if (deExists.Any())
                            {
                                aEntity.defects.Remove(deExists.SingleOrDefault());
                            }
                        }
                    }

                    aEntity.SaveChanges();

                    #endregion
                }

                #region Send to USER ### NEED TO CONFIRM ###
                #endregion

                Response.Redirect("ot_con_activity_report.aspx?activity=" + am.activity_id + "&sta=com", false);
            }
        }
        catch (Exception ex)
        {
            lblMessage.CssClass = "errorMessage";
            lblMessage.Text = "An error occured. " + ex.Message;
            writeLog.log(ex);
            throw ex;
        }
    }

    protected void btnReject_Click(object sender, EventArgs e)
    {
        // Reject action
        try
        {
            if (chkTicketed.Checked) if (ddlTicketAgent.SelectedValue == "") { cvTicketAgent.IsValid = false; }
            if (Page.IsValid && cvTicketAgent.IsValid)
            {
                hdnSelectedPer.Value = ddlPerformance.SelectedValue + "|" + ddlPerformance.SelectedItem.Text;
                hdnSelectedPerDate.Value = txtDateOfReport.Text;

                if (cFun.getSession("LogonUser") != null) uInfo = (UserInfo)cFun.getSession("LogonUser");
                else uInfo = cFun.getLogonUser(Environment.UserName);

                #region Save Main Activity
                activity_main am = new activity_main();
                am = (activity_main)cFun.getSession("MainActivity");

                aEntity.activity_main.Where(a => a.activity_id == am.activity_id).ToList().ForEach(fe =>
                {
                    fe.company = ddlCompany.SelectedValue;
                    fe.venue = ddlVenue.SelectedValue;
                    fe.report_type_id = int.Parse(ddlReportType.SelectedValue);
                    fe.date_of_report = txtDateOfReport.Text != "" ? DateTime.Parse(txtDateOfReport.Text) : (Nullable<DateTime>)null;
                    fe.scheduled_start_time = txtScheduledStartTime.Text;
                    string per1 = hdnSelectedPer.Value; //Request.Form[ddlPerformance.ClientID];
                    fe.performance_1 = hdnSelectedPer.Value.Split('|')[1]; // PREV CODE
                    fe.performance_2 = txtPerformanceText.Text != "" ? txtPerformanceText.Text : hdnSelectedPer.Value.Split('|')[1];
                    fe.venue_officer = getSelectedStringFromListBox(lstVenueManager);
                    fe.programming_ic = getSelectedStringFromListBox(lstProgrammingIC);
                    fe.production_ic = getSelectedStringFromListBox(lstProductionIC);
                    fe.el4 = txtEL4.Text;
                    fe.el5 = txtEL5.Text;
                    fe.hirer = txtHirer.Text;
                    fe.ticketing_agent = chkTicketed.Checked ? ddlTicketAgent.SelectedValue : null;
                    /*fe.is_draft = true;
                    fe.draft_last_saved_date = DateTime.Now;
                    fe.draft_last_saved_by = uInfo.Display_name;
                    fe.is_submitted = true;
                    fe.submitted_by = uInfo.Display_name;
                    fe.submitted_date = DateTime.Now;*/
                    fe.is_approved = false;
                    fe.approved_by = uInfo.Display_name;
                    fe.approved_date = DateTime.Now;
                    fe.updated_by = uInfo.Display_name;
                    fe.updated_date = DateTime.Now;
                });

                aEntity.SaveChanges();
                #endregion

                #region Send to USER ### NEED TO CONFIRM ###
                string urlString = "ot_con_activity_report.aspx?activity=" + am.activity_id;
                email eml = new email();
                eml.sendRejectEmailToSubmitUser(email.activityURL + urlString, am.submitted_user_id);
                #endregion

                Response.Redirect("ot_con_activity_report.aspx?activity=" + am.activity_id + "&sta=rj", false);
            }
        }
        catch (Exception ex)
        {
            lblMessage.CssClass = "errorMessage";
            lblMessage.Text = "An error occured. " + ex.Message;
            writeLog.log(ex);
            throw ex;
        }
    }

    protected void btnRecalculate_Click(object sender, EventArgs e)
    {
        if (txtTotalUsher.Text != "0" || txtTotalUsher.Text != "")
            txtTotalUsherPercent.Text = "100%";

        if (txtHandiPat.Text != "0" || txtHandiPat.Text != "")
        {
            if (cFun.getSession("OutdoorPerformanceList") != null)
            {
                int expTotal = 0;
                var perActivity = (List<performance_activity>)cFun.getSession("OutdoorPerformanceList");
                var expt = from pa in perActivity
                           where pa.exp_audience != null
                           select new { exp = pa.exp_audience };

                expTotal = expt.Sum(es => es.exp).Value;

                if (expTotal != 0 && txtHandiPat.Text != "0")
                    txtHandiPatPercent.Text = cFun.calFunctionOutdoorConcourse(expTotal, int.Parse(txtHandiPat.Text)).ToString("#.##") + "%";
                else txtHandiPatPercent.Text = "0%";

                /*if (txtTotalUsher.Text != "" && txtTotalUsher.Text != "0") txtTotalUsherPercent.Text = "100%";
                else txtTotalUsherPercent.Text = "0%";*/
                txtTotalUsherPercent.Text = cFun.returnCalculateResult(txtTotalUsher.Text, txtTotalUsherRequired.Text).ToString("0.##") + "%";
            }
        }
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        try
        {
            if (chkTicketed.Checked) if (ddlTicketAgent.SelectedValue == "") { cvTicketAgent.IsValid = false; }
            if (Page.IsValid && cvTicketAgent.IsValid)
            {
                hdnSelectedPer.Value = ddlPerformance.SelectedValue + "|" + ddlPerformance.SelectedItem.Text;
                hdnSelectedPerDate.Value = txtDateOfReport.Text;

                if (cFun.getSession("LogonUser") != null) uInfo = (UserInfo)cFun.getSession("LogonUser");
                else uInfo = cFun.getLogonUser(Environment.UserName);

                #region Save Main Activity
                activity_main am = new activity_main();
                am = (activity_main)cFun.getSession("MainActivity");

                aEntity.activity_main.Where(a => a.activity_id == am.activity_id).ToList().ForEach(fe =>
                {
                    fe.company = ddlCompany.SelectedValue;
                    fe.venue = ddlVenue.SelectedValue;
                    fe.report_type_id = int.Parse(ddlReportType.SelectedValue);
                    fe.date_of_report = txtDateOfReport.Text != "" ? DateTime.Parse(txtDateOfReport.Text) : (Nullable<DateTime>)null;
                    fe.scheduled_start_time = txtScheduledStartTime.Text;
                    string per1 = hdnSelectedPer.Value; //Request.Form[ddlPerformance.ClientID];
                    fe.performance_1 = hdnSelectedPer.Value.Split('|')[1]; // PREV CODE
                    fe.performance_2 = txtPerformanceText.Text != "" ? txtPerformanceText.Text : hdnSelectedPer.Value.Split('|')[1];
                    fe.venue_officer = getSelectedStringFromListBox(lstVenueManager);
                    fe.programming_ic = getSelectedStringFromListBox(lstProgrammingIC);
                    fe.production_ic = getSelectedStringFromListBox(lstProductionIC);
                    fe.el4 = txtEL4.Text;
                    fe.el5 = txtEL5.Text;
                    fe.hirer = txtHirer.Text;
                    fe.ticketing_agent = chkTicketed.Checked ? ddlTicketAgent.SelectedValue : null;
                    /*fe.is_draft = true;
                    fe.draft_last_saved_date = DateTime.Now;
                    fe.draft_last_saved_by = uInfo.Display_name;*/
                    fe.updated_by = uInfo.Display_name;
                    fe.updated_date = DateTime.Now;
                });

                aEntity.SaveChanges();
                #endregion

                if (ddlReportType.SelectedValue == "1001")
                {
                    #region Activity Details

                    // #1. Get Activity Details from control
                    string totalExpCountStr = txtTotalExposedCounts.Text;//((TextBox)ot_con_venue.FindControl("txtTotalExposedCounts")).Text;
                    string totalEngCountStr = txtTotalEngagedCounts.Text; //((TextBox)ot_con_venue.FindControl("txtTotalEngagedCounts")).Text;
                    string handiPatStr = txtHandiPat.Text; //((TextBox)ot_con_venue.FindControl("txtHandiPat")).Text;
                    string noOfUsherRequired = txtTotalUsherRequired.Text;
                    string noOfUsher = txtTotalUsher.Text;// ((TextBox)ot_con_venue.FindControl("txtTotalUsher")).Text;
                    string usherComm = txtUsherDeploymentComments.Text;// ((TextBox)ot_con_venue.FindControl("txtUsherDeploymentComments")).Text;
                    string otherComm = txtOtherComments.Text;// ((TextBox)ot_con_venue.FindControl("txtOtherComments")).Text;

                    activity_details actDetails = new activity_details();
                    actDetails.total_exposed_audience = totalExpCountStr != "" ? int.Parse(totalExpCountStr) : (Nullable<int>)null;
                    actDetails.total_engaged_audience = totalEngCountStr != "" ? int.Parse(totalEngCountStr) : (Nullable<int>)null;
                    actDetails.no_of_handicapped_patron = handiPatStr != "" ? int.Parse(handiPatStr) : (Nullable<int>)null;
                    actDetails.no_of_ushers = noOfUsher != "" ? int.Parse(noOfUsher) : (Nullable<int>)null;
                    actDetails.no_of_ushers_default = noOfUsherRequired != "" ? int.Parse(noOfUsherRequired) : (Nullable<int>)null;
                    actDetails.usher_dply_comments = usherComm;
                    actDetails.other_comments = otherComm;
                    actDetails.activity_id = am.activity_id;

                    var actExists = (from ad in aEntity.activity_details
                                     where ad.activity_id == am.activity_id
                                     select ad).SingleOrDefault();

                    if (actExists != null)
                    {
                        aEntity.activity_details.Where(ad => ad.activity_id == actExists.activity_id
                            && ad.activity_details_id == actExists.activity_details_id).ToList().ForEach(fe =>
                            {
                                fe.total_exposed_audience = totalExpCountStr != "" ? int.Parse(totalExpCountStr) : (Nullable<int>)null;
                                fe.total_engaged_audience = totalEngCountStr != "" ? int.Parse(totalEngCountStr) : (Nullable<int>)null;
                                fe.no_of_handicapped_patron = handiPatStr != "" ? int.Parse(handiPatStr) : (Nullable<int>)null;
                                fe.no_of_ushers_default = noOfUsherRequired != "" ? int.Parse(noOfUsherRequired) : (Nullable<int>)null;
                                fe.no_of_ushers = noOfUsher != "" ? int.Parse(noOfUsher) : (Nullable<int>)null;
                                fe.no_of_ushers_default = noOfUsherRequired != "" ? int.Parse(noOfUsherRequired) : (Nullable<int>)null;
                                fe.usher_dply_comments = usherComm;
                                fe.other_comments = otherComm;
                            });
                    }
                    else { aEntity.activity_details.Add(actDetails); }

                    aEntity.SaveChanges();
                    actDetails = aEntity.activity_details.Where(ad => ad.activity_id == am.activity_id).SingleOrDefault();
                    cFun.setSession("OutdoorActivityDetail", actDetails as object);

                    #endregion

                    // Save performance activity session
                    #region Update, Add, Delete Performance Activity

                    // Update, Save
                    List<performance_activity> perfActivityList = new List<performance_activity>();
                    if (cFun.getSession("OutdoorPerformanceList") != null)
                    {
                        perfActivityList = (List<performance_activity>)cFun.getSession("OutdoorPerformanceList");

                        if (perfActivityList != null)
                        {
                            foreach (performance_activity pa in perfActivityList)
                            {
                                // Check if it is exists in database
                                var paExists = from pad in aEntity.performance_activity
                                               where pad.per_activity_id == pa.per_activity_id &&
                                               pad.activity_id == pa.activity_id
                                               select pad;
                                if (paExists.Any()) // Exists -- do update
                                {
                                    aEntity.performance_activity.Where(pat => pat.per_activity_id == pa.per_activity_id).ToList().ForEach(fe =>
                                    {
                                        fe.EBMS_ORIG_START_TIME = ebmsFunc.getEBMSStartTimeBasedOnEBMSCombineKey(fe.EV700_EVT_ID, fe.EV700_ORG_CODE, fe.EV700_FUNC_ID);
                                        fe.start_time = pa.start_time;
                                        fe.end_time = pa.end_time;
                                        fe.activity_desc = pa.activity_desc;
                                        fe.exp_audience = pa.exp_audience;
                                        fe.eng_audience = pa.eng_audience;
                                        fe.audience_seated = pa.audience_seated;
                                        fe.other_comments = pa.other_comments;
                                        fe.crm_link = pa.crm_link;
                                        fe.updated_by = pa.updated_by;
                                        fe.updated_date = pa.updated_date;
                                    });
                                }
                                else { aEntity.performance_activity.Add(pa); } // Not Exists -- do save
                            }
                        }
                    }
                    // Delete
                    List<performance_activity> delPerfActivityList = new List<performance_activity>();
                    //delPerfActivityList = (List<performance_activity>)cFun.getSession("OutdoorPerformanceDelList");
                    if (cFun.getSession("OutdoorPerformanceDelList") != null)
                    {
                        delPerfActivityList = (List<performance_activity>)cFun.getSession("OutdoorPerformanceDelList");

                        foreach (performance_activity pa in delPerfActivityList)
                        {
                            var paExists = from pad in aEntity.performance_activity
                                           where pad.per_activity_id == pa.per_activity_id &&
                                           pad.activity_id == pa.activity_id
                                           select pad;
                            if (paExists.Any()) aEntity.performance_activity.Remove(paExists.SingleOrDefault());
                        }
                    }

                    aEntity.SaveChanges();


                    #endregion

                    #region Lost and Found

                    //Update, Save
                    List<lost_found> lfList = new List<lost_found>();
                    if (cFun.getSession("LostFoundList") != null)
                    {
                        lfList = (List<lost_found>)cFun.getSession("LostFoundList");
                        if (lfList != null)
                        {
                            foreach (lost_found lf in lfList)
                            {
                                // Check if it is exists in DB
                                var lfExists = from l in aEntity.lost_found
                                               where l.lost_found_id == lf.lost_found_id
                                               select l;
                                if (lfExists.Any())
                                {
                                    aEntity.lost_found.Where(l => l.lost_found_id == lf.lost_found_id).
                                        ToList().ForEach(fe =>
                                        {
                                            fe.lf_type_id = lf.lf_type_id;
                                            fe.activity_id = lf.activity_id;
                                            fe.item = lf.item;
                                            fe.located_at = lf.located_at;
                                            fe.tag_no = lf.tag_no;
                                            fe.action_taken = lf.action_taken;
                                            fe.crm_link = lf.crm_link;
                                            lf.updated_by = lf.updated_by;
                                            lf.updated_date = lf.updated_date;
                                        });
                                }
                                else { aEntity.lost_found.Add(lf); }
                            }
                        }
                    }

                    // Delete
                    List<lost_found> delLFList = new List<lost_found>();
                    if (cFun.getSession("LostFoundDelList") != null)
                    {
                        delLFList = (List<lost_found>)cFun.getSession("LostFoundDelList");
                        foreach (lost_found lf in delLFList)
                        {
                            // Check if it is exists in DB
                            var lfExists = from l in aEntity.lost_found
                                           where l.lost_found_id == lf.lost_found_id
                                           select l;
                            if (lfExists.Any())
                            {
                                aEntity.lost_found.Remove(lfExists.SingleOrDefault());
                            }
                        }
                    }

                    aEntity.SaveChanges();

                    #endregion

                    #region Defects

                    // Update, Save
                    List<defect> defectList = new List<defect>();
                    if (cFun.getSession("DefectList") != null)
                    {
                        defectList = (List<defect>)cFun.getSession("DefectList");
                        if (defectList != null)
                        {
                            foreach (defect de in defectList)
                            {
                                // Check if it is exists in DB
                                var deExists = from d in aEntity.defects
                                               where d.defect_id == de.defect_id
                                               select d;
                                if (deExists.Any())
                                {
                                    aEntity.defects.Where(w => w.defect_id == de.defect_id).ToList().ForEach(fe =>
                                    {
                                        fe.activity_id = de.activity_id;
                                        fe.defect1 = de.defect1;
                                        fe.located_at = de.located_at;
                                        fe.action_taken = de.action_taken;
                                        fe.remarks = de.remarks;
                                        fe.bms_id = de.bms_id;
                                        fe.updated_by = de.updated_by;
                                        fe.updated_date = de.updated_date;
                                    });
                                }
                                else
                                {
                                    aEntity.defects.Add(de);
                                }
                            }
                        }
                    }

                    //Delete
                    List<defect> delDefList = new List<defect>();
                    if (cFun.getSession("DefectDelList") != null)
                    {
                        delDefList = (List<defect>)cFun.getSession("DefectDelList");

                        foreach (defect de in delDefList)
                        {
                            // Check if it is exists in DB
                            var deExists = from d in aEntity.defects
                                           where d.defect_id == de.defect_id
                                           select d;
                            if (deExists.Any())
                            {
                                aEntity.defects.Remove(deExists.SingleOrDefault());
                            }
                        }
                    }

                    aEntity.SaveChanges();

                    #endregion
                }
                Response.Redirect("ot_con_activity_report.aspx?activity=" + am.activity_id + "&sta=up", false);
            }
        }
        catch (Exception ex)
        {
            lblMessage.CssClass = "errorMessage";
            lblMessage.Text = "An error occured. " + ex.Message;
            throw ex;
        }
    }

    private void disableControl()
    {
        //lnkPerAdd.Enabled = false;
        lnkPerAdd.Text = "";
        lnkPerRemove.Text = "";
        //lnkPerRemove.Enabled = false;

        foreach (Control ctrl in los_fd_defect.Controls)
        {
            if (ctrl.GetType().Equals(typeof(LinkButton)))
            { ((LinkButton)ctrl).Enabled = false; }
        }
    }
}