<%@ Page Title="" Language="C#" MasterPageFile="~/ActivityReport.master" AutoEventWireup="true" Async="true" CodeFile="ot_con_activity_report.aspx.cs" Inherits="ot_con_activity_report" ValidateRequest="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Src="~/los_fd_defect.ascx" TagPrefix="uc1" TagName="los_fd_defect" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
    </asp:ToolkitScriptManager>

    <script src="./Scripts/jquery-1.8.2.min.js"></script>
    <script src="./Scripts/jquery-ui-1.8.24.js"></script>

    <script type="text/javascript">

        // Performance change
        function perf_OnChange() {
            var perID = $('#mainContent_ddlPerformance option:selected').val();
            var dReport = document.getElementById('mainContent_txtDateOfReport').value;
            var venue = $('#mainContent_ddlVenue option:selected').val();
            var perText = $('#mainContent_ddlPerformance option:selected').text();
            $('#mainContent_hdnSelectedPer').val(perID + '|' + perText);
            $('#mainContent_hdnSelectedPerDate').val(dReport);
        }

        $(function () {
            $("#main_activity").accordion({
                collapsible: true
            });
        });

        function cancel_OnClick() {
            /*if (confirm("Do you want to discard the changes you made in the system?\n\nClick 'OK' to discard.\nClick 'Cancel' to stay in the page.")) {
                window.location = 'activities_listing.aspx';
            } else { return false; }*/

            window.location = 'activities_listing.aspx';
        }

        function setCheckedActivityID(pID) {
            var hPID = $('#mainContent_hdnSelActID').val();
            if (hPID.indexOf(pID) <= -1) {
                hPID = hPID + (pID + ",");
                $('#mainContent_hdnSelActID').val(hPID);
            }
            else {
                hPID = hPID.replace(pID + ",", "");
                $('#mainContent_hdnSelActID').val(hPID);
            }
        }

        function removePerformanceMessage() {
            var hPID = $('#mainContent_hdnSelActID').val();
            if (hPID != "" && hPID != ",") {
                if (confirm("Do you want to remove selected Activity?")) {
                    return true;
                } else {
                    return false;
                }
            }
            else { alert('Please select at least one Activity to remove.'); }
        }

        function ticketAgentValidation(source, arguments) {
            var lfType = $('#mainContent_chkTicketed');

            if (lfType[0].checked) {
                if (arguments.Value == "") arguments.IsValid = false;
                else arguments.IsValid = true;
            }
        }

    </script>
    <table style="width: 100%; padding: 10px;" id="mainTable">
        <tr>
            <td>
                <div id="main_activity">
                    <div class="accordionBarStyle">
                        <span class="accordionInnerText">&nbsp;Main activity
                            <%----%>
                        </span>
                    </div>
                    <div>
                        <table style="width: 100%; padding: 10px; border: 2px solid #DCDCDC; background-color: #F9F9F9;">
                            <tr>
                                <td class="tableCol">Date of report
                                </td>
                                <td class="tableCol">
                                    <asp:TextBox ID="txtDateOfReport" runat="server" CssClass="textBoxStyle" Enabled="false"></asp:TextBox>
                                </td>
                                <td class="tableCol">Ticket mode
                                </td>
                                <td class="tableCol">
                                    <asp:CheckBox ID="chkTicketed" runat="server" Text=" Is ticketed?" Font-Bold="true" AutoPostBack="true" OnCheckedChanged="chkTicketed_CheckedChanged" />
                                    <%--onclick="ticket_OnChange(this);"--%>
                                            &nbsp; &nbsp;
                                    <span>
                                        <asp:Label ID="lblTicketAgent" runat="server" Text="Ticket agent: "></asp:Label>
                                        <asp:DropDownList ID="ddlTicketAgent" runat="server" CssClass="dropDownStyle" Style="width: 190px;" /><br />
                                        <%--<asp:RequiredFieldValidator ID="rfvTicketAgent" runat="server" CssClass="requiredFieldText" ErrorMessage="This is a required field" ControlToValidate="ddlTicketAgent"
                                            ValidationGroup="MainPanel" Display="Dynamic" />--%>
                                        <asp:CustomValidator ID="cvTicketAgent" runat="server" CssClass="requiredFieldText" ErrorMessage="This is a required field" ControlToValidate="ddlTicketAgent" ValidationGroup="MainPanel"
                                            ClientValidationFunction="ticketAgentValidation" />
                                    </span>
                                </td>
                            </tr>
                            <tr>
                                <td class="tableCol">Company</td>
                                <td class="tableCol">
                                    <asp:DropDownList ID="ddlCompany" runat="server" CssClass="dropDownStyle" Enabled="false" /><br />
                                </td>
                                <td class="tableCol">Report type
                                </td>
                                <td class="tableCol">
                                    <asp:DropDownList ID="ddlReportType" runat="server" CssClass="dropDownStyle" Enabled="false" /><br />
                                </td>
                            </tr>
                            <tr>
                                <td class="tableCol">Venue</td>
                                <td class="tableCol">
                                    <%--<asp:DropDownList ID="ddlVenue" runat="server" CssClass="dropDownStyle" AutoPostBack="true" OnSelectedIndexChanged="ddlVenue_SelectedIndexChanged" />--%>
                                    <asp:DropDownList ID="ddlVenue" runat="server" CssClass="dropDownStyle" Enabled="false" /><br />
                                </td>
                                <td class="tableCol">Day of report</td>
                                <td class="tableCol">
                                    <asp:TextBox ID="txtDayOfReport" runat="server" CssClass="textBoxStyle" Enabled="false"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4"></td>
                            </tr>
                            <tr>
                                <td colspan="4" style="border-bottom: 1px solid #DCDCDC;"></td>
                            </tr>
                            <tr>
                                <td colspan="4"></td>
                            </tr>
                            <tr>
                                <td class="tableCol">Performance</td>
                                <td class="tableCol">
                                    <asp:DropDownList ID="ddlPerformance" runat="server" CssClass="dropDownStyle" Enabled="false" /><br />
                                </td>
                                <td class="tableCol">Performance text</td>
                                <td class="tableCol">
                                    <asp:TextBox ID="txtPerformanceText" runat="server" CssClass="textBoxStyle"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="tableCol">EL4</td>
                                <td class="tableCol">
                                    <asp:TextBox ID="txtEL4" runat="server" ReadOnly="true" CssClass="textBoxStyle" Enabled="false"></asp:TextBox>
                                </td>
                                <td class="tableCol">Scheduled start time</td>
                                <td class="tableCol">
                                    <asp:TextBox ID="txtScheduledStartTime" runat="server" CssClass="textBoxStyle" Enabled="false" required></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="tableCol">EL5</td>
                                <td class="tableCol">
                                    <asp:TextBox ID="txtEL5" runat="server" ReadOnly="true" CssClass="textBoxStyle" Enabled="false"></asp:TextBox>
                                </td>
                                <td class="tableCol" rowspan="2">Venue officer/Duty manager</td>
                                <td class="tableCol" rowspan="2">
                                    <asp:ListBox ID="lstVenueManager" runat="server" CssClass="listBoxStyle" Rows="5" SelectionMode="Multiple" required /><br />
                                    <asp:RequiredFieldValidator ID="rfvVenueManager" runat="server" CssClass="requiredFieldText" ErrorMessage="This is a required field" ControlToValidate="lstVenueManager"
                                        ValidationGroup="MainPanel" Display="Dynamic" />
                                </td>
                            </tr>
                            <tr>
                                <td class="tableCol">Hirer
                                </td>
                                <td class="tableCol">
                                    <asp:TextBox ID="txtHirer" runat="server" TextMode="MultiLine" Rows="3" CssClass="textAreaStyle" Enabled="false" required></asp:TextBox>
                                </td>
                                <td></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td class="tableCol">Programming IC / Venue manager
                                </td>
                                <td class="tableCol">
                                    <asp:ListBox ID="lstProgrammingIC" runat="server" CssClass="listBoxStyle" Rows="5" SelectionMode="Multiple"></asp:ListBox><br />
                                    <asp:RequiredFieldValidator ID="rfvProgrammingIC" runat="server" CssClass="requiredFieldText" ErrorMessage="This is a required field" ControlToValidate="lstProgrammingIC"
                                        ValidationGroup="MainPanel" Display="Dynamic" />
                                </td>
                                <td class="tableCol">Production IC
                                </td>
                                <td class="tableCol">
                                    <asp:ListBox ID="lstProductionIC" runat="server" CssClass="listBoxStyle" Rows="5" SelectionMode="Multiple" required></asp:ListBox><br />
                                    <asp:RequiredFieldValidator ID="rfvProductionIC" runat="server" CssClass="requiredFieldText" ErrorMessage="This is a required field" ControlToValidate="lstProductionIC"
                                        ValidationGroup="MainPanel" Display="Dynamic" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </td>
        </tr>
    </table>
    <table style="width: 100%; padding: 10px;">
        <tr>
            <td>
                <table style="width: 100%; padding: 5px; border: 1px solid #DCDCDC;">
                    <tr>
                        <td style="width: 150px; font-weight: bold;">Report status</td>
                        <td id="tdDraft" runat="server" style="width: 150px; text-align: center; color: #333;">draft</td>
                        <td style="width: 20px; text-align: center;"><b>></b></td>
                        <td id="tdSubmit" runat="server" style="width: 150px; text-align: center; color: #333;">submit</td>
                        <td style="width: 20px; text-align: center;"><b>></b></td>
                        <td id="tdApprove" runat="server" style="width: 150px; text-align: center; color: #333;">complete / reject</td>
                        <td></td>
                    </tr>
                </table>
                <br />
            </td>
        </tr>
        <tr>
            <td style="width: 100%; text-align: left;">
                <asp:Label ID="ltrInfoMessage" runat="server" Visible="false" CssClass="infoMessage"></asp:Label>
                <asp:Button ID="btnSaveDraft" runat="server" Text="save as draft" CssClass="buttonStyle" Width="100" OnClick="btnSaveDraft_Click" ValidationGroup="MainData" />
                <%--<asp:Button ID="btnSave" runat="server" Text="save" CssClass="buttonStyle" />--%>
                <asp:Button ID="btnUpdate" runat="server" Text="update" CssClass="buttonStyle" Visible="false" OnClick="btnUpdate_Click" ValidationGroup="MainData" />
                <asp:Button ID="btnSubmit" runat="server" Text="submit" CssClass="buttonStyle" OnClick="btnSubmit_Click" ValidationGroup="MainData" />
                <asp:Button ID="btnComplete" runat="server" Text="complete" CssClass="buttonStyle" OnClick="btnComplete_Click" ValidationGroup="MainData" />
                <asp:Button ID="btnReject" runat="server" Text="reject" CssClass="buttonStyle" OnClick="btnReject_Click" ValidationGroup="MainData" />
                &nbsp; &nbsp;
                <asp:Label ID="lblMessage" runat="server" CssClass="infoMessage"></asp:Label>
                <span style="float: right;">&nbsp;
                    <input type="button" id="btnCancel" value="cancel" class="buttonStyle" onclick="return cancel_OnClick();" />
                </span>&nbsp;
            </td>
        </tr>
        <tr>
            <td>
                <asp:UpdatePanel ID="upDetails" runat="server" UpdateMode="Conditional" EnableViewState="true" ValidateRequestMode="Enabled">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="gridviewActivity" />
                        <asp:AsyncPostBackTrigger ControlID="lnkPerRemove" />
                        <asp:AsyncPostBackTrigger ControlID="lnkPerAdd" />
                        <asp:AsyncPostBackTrigger ControlID="btnSavePer" />
                        <asp:PostBackTrigger ControlID="btnRecalculate" />
                    </Triggers>
                    <ContentTemplate>
                        <table style="width: 100%; padding: 5px; border: 2px solid #DCDCDC;">
                            <tr>
                                <td style="width: 25%">Total exposed audience count
                                </td>
                                <td class="tableCol">
                                    <asp:TextBox ID="txtTotalExposedCounts" runat="server" CssClass="textBoxStyle" Width="150px" Enabled="false"></asp:TextBox>
                                    <asp:TextBox ID="txtTotalExposedPer" runat="server" CssClass="textBoxStyle" Width="150px" ReadOnly="true" Enabled="false"></asp:TextBox>
                                </td>
                                <td class="tableCol"></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td style="width: 25%">Total engaged audience count
                                </td>
                                <td class="tableCol">
                                    <asp:TextBox ID="txtTotalEngagedCounts" runat="server" CssClass="textBoxStyle" Width="150px" Enabled="false"></asp:TextBox>
                                    <asp:TextBox ID="txtTotalEngagedPer" runat="server" CssClass="textBoxStyle" Width="150px" ReadOnly="true" Enabled="false"></asp:TextBox>
                                </td>
                                <td class="tableCol"></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td style="width: 25%;">No of handicapped patrons
                                </td>
                                <td class="tableCol" colspan="2">
                                    <asp:TextBox ID="txtHandiPat" runat="server" CssClass="textBoxStyle" Width="150px" EnableViewState="true" ValidationGroup="MainData"></asp:TextBox>
                                    <asp:TextBox ID="txtHandiPatPercent" runat="server" CssClass="textBoxStyle" Width="150px" ReadOnly="true" Enabled="false"></asp:TextBox>
                                    <asp:CompareValidator ID="cvHandiPat" runat="server" ControlToValidate="txtHandiPat" Operator="DataTypeCheck" CssClass="requiredFieldText" Type="Integer" ErrorMessage="Value must be a number." ValidationGroup="MainData" />
                                </td>
                                <td class="tableCol"></td>
                            </tr>
                            <tr>
                                <td style="width: 25%">No. of ushers (Event package)
                                </td>
                                <td class="tableCol" colspan="2">
                                    <asp:TextBox ID="txtTotalUsherRequired" runat="server" CssClass="textBoxStyle" Width="150px" EnableViewState="true" ValidationGroup="MainData"></asp:TextBox>
                                    <asp:TextBox ID="txtTotalUsherRequiredPercent" runat="server" CssClass="textBoxStyle" Width="150px" Enabled="false"></asp:TextBox>
                                    <asp:CompareValidator ID="cvTotalUsherRequried" runat="server" ControlToValidate="txtTotalUsherRequired" Operator="DataTypeCheck" 
                                        CssClass="requiredFieldText" Type="Integer" ErrorMessage="Value must be a number." ValidationGroup="MainData" />
                                </td>
                                <td class="tableCol"></td>
                            </tr>
                            <tr>
                                <td style="width: 25%">No. of ushers (Actual)
                                </td>
                                <td class="tableCol" colspan="2">
                                    <asp:TextBox ID="txtTotalUsher" runat="server" CssClass="textBoxStyle" Width="150px" EnableViewState="true" ValidationGroup="MainData"></asp:TextBox>
                                    <asp:TextBox ID="txtTotalUsherPercent" runat="server" CssClass="textBoxStyle" Width="150px" Enabled="false"></asp:TextBox>
                                    <asp:CompareValidator ID="cvTotalUsher" runat="server" ControlToValidate="txtTotalUsher" Operator="DataTypeCheck" CssClass="requiredFieldText" Type="Integer" ErrorMessage="Value must be a number." ValidationGroup="MainData" />
                                    <asp:Button ID="btnRecalculate" runat="server" Text="calculate all" CssClass="buttonStyle" Width="120" OnClick="btnRecalculate_Click" />
                                </td>
                                <td class="tableCol"></td>
                            </tr>
                            <tr>
                                <td style="width: 25%; vertical-align: top;">Usher deployment / comments
                                </td>
                                <td colspan="3">
                                    <asp:TextBox ID="txtUsherDeploymentComments" runat="server" TextMode="MultiLine"
                                        Rows="5" CssClass="textAreaStyle" Width="90%" EnableViewState="true"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                        <br />
                        <table style="width: 100%; padding: 5px; border: 2px solid #DCDCDC;">
                            <tr style="height: 25px;">
                                <td style="width: 80%"><b>Activity</b></td>
                                <td style="width: 20%; text-align: right;">
                                    <asp:LinkButton ID="lnkPerAdd" runat="server" Text="Add" />
                                    &nbsp; | &nbsp;
            <asp:LinkButton ID="lnkPerRemove" runat="server" Text="Remove" OnClick="lnkPerRemove_Click" OnClientClick="javascript: return removePerformanceMessage();" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:GridView ID="gridviewActivity" runat="server" AutoGenerateColumns="false" CellPadding="3" GridLines="None"
                                        Width="100%" OnRowDataBound="gridviewActivity_RowDataBound" OnRowCommand="gridviewActivity_RowCommand">
                                        <Columns>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkbtnEdit" runat="server" Text="Edit" CommandName="EditRow" CommandArgument='<%#Container.DataItemIndex %>'
                                                        AlternateText="Edit" Title="Edit" />
                                                </ItemTemplate>
                                                <ItemStyle Width="30px" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkCheck" runat="server" />
                                                    <asp:Literal ID="ltrRowID" runat="server" Text='<%#Eval("per_activity_id") %>' Visible="false" />
                                                    <%--<asp:Literal ID="ltrEventID" runat="server" Text='<%#Eval("evt_id") %>' Visible="false" />--%>
                                                </ItemTemplate>
                                                <ItemStyle Width="30px" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Performance">
                                                <ItemTemplate>
                                                    <asp:Literal ID="ltrPerformance" runat="server" Text='<%#Eval("activity_desc") %>'></asp:Literal>
                                                </ItemTemplate>
                                                <ItemStyle Width="250px" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Start time">
                                                <ItemTemplate>
                                                    <asp:Literal ID="ltrStartTime" runat="server" Text='<%#Eval("start_time") %>'></asp:Literal>
                                                    <asp:Literal ID="ltrStartDT" runat="server" Visible="false" Text='<%#Eval("start_time") %>'></asp:Literal>
                                                </ItemTemplate>
                                                <ItemStyle Width="70px" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="End time">
                                                <ItemTemplate>
                                                    <asp:Literal ID="ltrEndTime" runat="server" Text='<%#Eval("end_time") %>'></asp:Literal>
                                                    <asp:Literal ID="ltrEndDT" runat="server" Visible="false" Text='<%#Eval("end_time") %>'></asp:Literal>
                                                </ItemTemplate>
                                                <ItemStyle Width="70px" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Exp aud">
                                                <ItemTemplate>
                                                    <asp:Literal ID="ltrExposedAudience" runat="server" Text='<%#Eval("exp_audience") %>'></asp:Literal>
                                                </ItemTemplate>
                                                <ItemStyle Width="70px" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Eng aud">
                                                <ItemTemplate>
                                                    <asp:Literal ID="ltrEngagedAudience" runat="server" Text='<%#Eval("eng_audience") %>'></asp:Literal>
                                                </ItemTemplate>
                                                <ItemStyle Width="70px" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Aud seated">
                                                <ItemTemplate>
                                                    <asp:Literal ID="ltrAudienceSeated" runat="server" Text='<%#Eval("audience_seated") %>'></asp:Literal>
                                                </ItemTemplate>
                                                <ItemStyle Width="70px" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Comments">
                                                <ItemTemplate>
                                                    <asp:Literal ID="ltrPerformanceComments" runat="server" Text='<%#Eval("other_comments") %>'></asp:Literal>
                                                </ItemTemplate>
                                                <ItemStyle Width="450px" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                <HeaderStyle HorizontalAlign="Left" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="CRM link">
                                                <ItemTemplate>
                                                    <asp:HyperLink ID="lnkCRMLink" runat="server" />
                                                    <asp:Literal ID="ltrCRMLink" runat="server" Text='<%#Eval("crm_link") %>' Visible="false"></asp:Literal>
                                                </ItemTemplate>
                                                <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                <HeaderStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                        </Columns>
                                        <HeaderStyle HorizontalAlign="Left" CssClass="gridHeaderStyle" />
                                        <RowStyle CssClass="gridRowStyle" />
                                        <AlternatingRowStyle CssClass="gridAlternatingRowStyle" />
                                    </asp:GridView>
                                </td>
                            </tr>
                        </table>
                        <table style="width: 100%; padding: 5px;">
                            <tr>
                                <td></td>
                            </tr>
                        </table>
                        <table style="width: 100%; padding: 5px;">
                            <tr>
                                <td style="width: 25%; vertical-align: top;">Other comments
                                </td>
                                <td colspan="3">
                                    <asp:TextBox ID="txtOtherComments" runat="server" TextMode="MultiLine"
                                        Rows="5" CssClass="textAreaStyle" Width="90%" EnableViewState="true"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                        <asp:ModalPopupExtender ID="popupActivity" runat="server" BackgroundCssClass="modelPopupBackground"
                            TargetControlID="lnkPerAdd" BehaviorID="pnlActivityPopup" PopupControlID="pnlActivityPopup">
                        </asp:ModalPopupExtender>
                        <asp:Panel ID="pnlActivityPopup" runat="server" Width="80%" CssClass="popupPanelStyle">
                            <table style="width: 100%; padding: 5px;" class="coiBoxHeader">
                                <tr>
                                    <td style="vertical-align: middle;">
                                        <b>Add / Edit Activity</b></td>
                                </tr>
                            </table>
                            <table style="width: 100%; padding: 5px">
                                <tr>
                                    <td colspan="4">
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 1%" />
                                    <td style="width: 40%">Name
                                    </td>
                                    <td style="width: 47%">
                                        <asp:TextBox ID="txtPerformance" runat="server" CssClass="textBoxStyle" Width="80%" ValidationGroup="PerformanceActivity" placeholder="Performance">
                                        </asp:TextBox><br />
                                        <asp:RequiredFieldValidator ID="rfvPerformance" runat="server" ControlToValidate="txtPerformance" CssClass="requiredFieldText"
                                            ErrorMessage="This is a required field." ValidationGroup="PerformanceActivity" />
                                    </td>
                                    <td style="width: 1%" />
                                </tr>
                                <tr>
                                    <td style="width: 1%" />
                                    <td style="width: 40%">Start time <span style="color: #333;">(eg: 10:00 PM)</span>
                                    </td>
                                    <td style="width: 47%">
                                        <asp:TextBox ID="txtStartTime" runat="server" CssClass="textBoxStyle" Width="80%" ValidationGroup="PerformanceActivity" placeholder="10:00 PM">
                                        </asp:TextBox><br />
                                        <asp:RequiredFieldValidator ID="rfvStartTime" runat="server" ControlToValidate="txtStartTime" CssClass="requiredFieldText"
                                            ErrorMessage="This is a required field." ValidationGroup="PerformanceActivity" />
                                    </td>
                                    <td style="width: 1%" />
                                </tr>
                                <tr>
                                    <td style="width: 1%" />
                                    <td style="width: 40%">End time <span style="color: #333;">(eg: 10:00 PM)</span>
                                    </td>
                                    <td style="width: 47%">
                                        <asp:TextBox ID="txtEndTime" runat="server" CssClass="textBoxStyle" Width="80%" ValidationGroup="PerformanceActivity" placeholder="10:00 PM">
                                        </asp:TextBox><br />
                                        <asp:RequiredFieldValidator ID="rfvEndTime" runat="server" ControlToValidate="txtEndTime" CssClass="requiredFieldText"
                                            ErrorMessage="This is a required field." ValidationGroup="PerformanceActivity" />
                                    </td>
                                    <td style="width: 1%" />
                                </tr>
                                <tr>
                                    <td style="width: 1%" />
                                    <td style="width: 40%">Exposed audience count
                                    </td>
                                    <td style="width: 47%">
                                        <asp:TextBox ID="txtExposedCount" runat="server" CssClass="textBoxStyle" Width="80%" ValidationGroup="PerformanceActivity" placeholder="0">
                                        </asp:TextBox><br />
                                        <asp:CompareValidator ID="comvExposedCount" runat="server" ControlToValidate="txtExposedCount" CssClass="requiredFieldText" Operator="DataTypeCheck" Type="Integer"
                                            ErrorMessage="Value must be a number." />
                                    </td>
                                    <td style="width: 1%" />
                                </tr>
                                <tr>
                                    <td style="width: 1%" />
                                    <td style="width: 40%">Engaged audience count
                                    </td>
                                    <td style="width: 47%">
                                        <asp:TextBox ID="txtEngagedCount" runat="server" CssClass="textBoxStyle" Width="80%" ValidationGroup="PerformanceActivity" placeholder="0">
                                        </asp:TextBox><br />
                                        <asp:CompareValidator ID="comvEngagedCount" runat="server" ControlToValidate="txtEngagedCount" Operator="DataTypeCheck" Type="Integer"
                                            ValidationGroup="PerformanceActivity" ErrorMessage="Value must be a number." CssClass="requiredFieldText" />
                                    </td>
                                    <td style="width: 1%" />
                                </tr>
                                <tr>
                                    <td style="width: 1%" />
                                    <td style="width: 40%">Audience seated
                                    </td>
                                    <td style="width: 47%">
                                        <asp:TextBox ID="txtAudienceSeated" runat="server" CssClass="textBoxStyle" Width="80%" ValidationGroup="PerformanceActivity" placeholder="0">
                                        </asp:TextBox><br />
                                        <asp:CompareValidator ID="comvAudienceSeated" runat="server" ControlToValidate="txtEngagedCount" Operator="DataTypeCheck" Type="Integer"
                                            ValidationGroup="PerformanceActivity" ErrorMessage="Value must be a number." CssClass="requiredFieldText" />
                                    </td>
                                    <td style="width: 1%" />
                                </tr>
                                <tr>
                                    <td style="width: 1%" />
                                    <td style="width: 40%; vertical-align: top;">Performance comments / incidents
                                    </td>
                                    <td style="width: 47%">
                                        <asp:TextBox ID="txtPerformanceComments" runat="server" CssClass="textAreaStyle" Width="80%"
                                            TextMode="MultiLine" Rows="5" placeholder="Any performance comments or incidents">
                                        </asp:TextBox>
                                    </td>
                                    <td style="width: 1%" />
                                </tr>
                                <tr>
                                    <td style="width: 1%" />
                                    <td style="width: 40%">CRM link
                                    </td>
                                    <td style="width: 47%">
                                        <asp:TextBox ID="txtCRMLink" runat="server" CssClass="textBoxStyle" Width="80%" ValidationGroup="PerformanceActivity" placeholder="Type CRM link (eg: http://crm.esplanade.com.sg)" /><br />
                                        <asp:RegularExpressionValidator ID="regCRMLink" runat="server" CssClass="requiredFieldText" ValidationExpression="^(http|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?"
                                            ValidationGroup="PerformanceActivity" ControlToValidate="txtCRMLink" ErrorMessage="Type a valid URL." />
                                        <%--ValidationExpression="(http(s)?://)?([\w-]+\.)+[\w-]+(/[\w- ;,./?%&=]*)?"--%>
                                        <br />
                                        <a href="http://crm101.esplanade.com.sg/TECLCRM2013/main.aspx#437077234" target="_blank">Create incidents in CRM</a>
                                    </td>
                                    <td style="width: 1%" />
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <br />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 1%" />
                                    <td colspan="2" style="width: 98%; text-align: right;">
                                        <asp:Button ID="btnSavePer" Text="Save" runat="server" CssClass="buttonStyle" OnClick="btnSavePer_Click" ValidationGroup="PerformanceActivity" />
                                        <asp:Button ID="btnCancelPer" Text="Cancel" runat="server" CssClass="buttonStyle" OnClick="btnCancelPer_Click" />
                                    </td>
                                    <td style="width: 1%" />
                                </tr>
                            </table>
                            <asp:HiddenField ID="hdnDummyPopup" runat="server" />
                            <asp:HiddenField ID="hdnEventID" runat="server" />
                            <asp:HiddenField ID="hdnRowID" runat="server" />
                            <asp:HiddenField ID="hdnEventDateTime" runat="server" />
                            <asp:HiddenField ID="hdnActivityID" runat="server" />
                        </asp:Panel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    <table style="width: 100%">
        <tr>
            <td>
                <uc1:los_fd_defect runat="server" ID="los_fd_defect" />
            </td>
        </tr>
    </table>

    <br />
    <div id="footer" class="footerTable">
        <table>
            <tr>
                <td class="footerLabel">Created by: 
                </td>
                <td class="footerControlName">
                    <asp:Literal ID="ltrCreatedBy" runat="server"></asp:Literal>
                </td>
                <td class="footerLabel">Updated by: 
                </td>
                <td class="footerControlName">
                    <asp:Literal ID="ltrUpdatedBy" runat="server"></asp:Literal>
                </td>
                <td class="footerLabel">Draft saved by: 
                </td>
                <td class="footerControlName">
                    <asp:Literal ID="ltrDraftSavedBy" runat="server"></asp:Literal>
                </td>
                <td class="footerLabel">Submitted by: 
                </td>
                <td class="footerControlName">
                    <asp:Literal ID="ltrSubmittedBy" runat="server"></asp:Literal>
                </td>
                <td class="footerLabel">Completed by: 
                </td>
                <td class="footerControlName">
                    <asp:Literal ID="ltrCompletedBy" runat="server"></asp:Literal>
                </td>
                <td class="footerLabel">Rejected by: 
                </td>
                <td class="footerControlName">
                    <asp:Literal ID="ltrRejectedBy" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="footerLabel">Created on: 
                </td>
                <td class="footerControl">
                    <asp:Literal ID="ltrCreatedOn" runat="server"></asp:Literal>
                </td>
                <td class="footerLabel">Updated on: 
                </td>
                <td class="footerControl">
                    <asp:Literal ID="ltrUpdatedOn" runat="server"></asp:Literal>
                </td>
                <td class="footerLabel">Draft saved on: 
                </td>
                <td class="footerControl">
                    <asp:Literal ID="ltrDraftSavedOn" runat="server"></asp:Literal>
                </td>
                <td class="footerLabel">Submitted on: 
                </td>
                <td class="footerControl">
                    <asp:Literal ID="ltrSubmittedOn" runat="server"></asp:Literal>
                </td>
                <td class="footerLabel">Completed on: 
                </td>
                <td class="footerControl">
                    <asp:Literal ID="ltrCompletedOn" runat="server"></asp:Literal>
                </td>
                <td class="footerLabel">Rejected on:</td>
                <td class="footerControl">
                    <asp:Literal ID="ltrRejectedOn" runat="server"></asp:Literal>
                </td>
            </tr>
        </table>
    </div>
    <script type="text/javascript">
        perf_OnChange();
    </script>
    <%-- HiddenField  --%>
    <asp:HiddenField ID="hdnPGDept" runat="server" />
    <asp:HiddenField ID="hdnPRODDept" runat="server" />
    <asp:HiddenField ID="hdnSelectedPer" runat="server" />
    <asp:HiddenField ID="hdnSelectedPerDate" runat="server" />
    <asp:HiddenField ID="hdnSelActID" runat="server" EnableViewState="true" />
    <asp:HiddenField ID="hdnActivityTypeID" runat="server" />
    <asp:HiddenField ID="hdnDisableButton" runat="server" />
</asp:Content>
