<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.Master" AutoEventWireup="true" CodeBehind="register.aspx.cs" Inherits="WebUI.register" %>

<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <table class="mainTableBackground">
        <tr>
            <td style="width: 70%"></td>
            <td>
                <table class="registerTable">
                    <tr>
                        <td colspan="3">
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 3%"></td>
                        <td>
                            <label style="color: #39F;">CREATE AN ACCOUNT</label>
                        </td>
                        <td style="width: 3%"></td>
                    </tr>
                    <tr>
                        <td style="width: 3%"></td>
                        <td>
                            <hr style="border: 2px solid #DCDCDC;" />
                        </td>
                        <td style="width: 3%"></td>
                    </tr>
                    <tr>
                        <td style="width: 3%"></td>
                        <td>
                            <label>FIRST NAME</label>
                        </td>
                        <td style="width: 3%"></td>
                    </tr>
                    <tr>
                        <td style="width: 3%"></td>
                        <td>
                            <asp:TextBox ID="txtFirstName" runat="server" required="required" placeholder="FIRST NAME"></asp:TextBox>
                        </td>
                        <td style="width: 3%"></td>
                    </tr>
                    <tr>
                        <td style="width: 3%"></td>
                        <td>
                            <label>LAST NAME</label>
                        </td>
                        <td style="width: 3%"></td>
                    </tr>
                    <tr>
                        <td style="width: 3%"></td>
                        <td>
                            <asp:TextBox ID="txtLastName" runat="server" required="required" placeholder="LAST NAME"></asp:TextBox>
                        </td>
                        <td style="width: 3%"></td>
                    </tr>
                    <tr>
                        <td style="width: 3%"></td>
                        <td>
                            <label>EMAIL</label>
                        </td>
                        <td style="width: 3%"></td>
                    </tr>
                    <tr>
                        <td style="width: 3%"></td>
                        <td>
                            <asp:TextBox ID="txtEmail" runat="server" required="required" placeholder="EMAIL"></asp:TextBox>
                        </td>
                        <td style="width: 3%"></td>
                    </tr>
                    <tr>
                        <td style="width: 3%"></td>
                        <td>
                            <asp:CheckBox ID="chkPDPA" runat="server" Width="100%" Text="I accept PDPA" />
                        </td>
                        <td style="width: 3%"></td>
                    </tr>
                    <tr>
                        <td style="width: 3%"></td>
                        <td>
                            <asp:CheckBox ID="chkTC" runat="server" 
                                Text="I accept the Terms & Conditions" Width="100%" />
                        </td>
                        <td style="width: 3%"></td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 3%"></td>
                        <td>
                            <asp:Button ID="btnRegister" runat="server" Text="Register" OnClick="btnRegister_Click" />
                        </td>
                        <td style="width: 3%"></td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <br />
                        </td>
                    </tr>
                </table>
            </td>
            <td style="width: 1%"></td>
        </tr>
    </table>
</asp:Content>
