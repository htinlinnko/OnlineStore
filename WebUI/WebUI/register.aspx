<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.Master" AutoEventWireup="true" CodeBehind="register.aspx.cs" Inherits="WebUI.register" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <table>
        <tr>
            <td>
                FIRST NAME : 
                <asp:TextBox ID="txtFirstName" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                LAST NAME : 
                <asp:TextBox ID="txtLastName" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                EMAIL : 
                <asp:TextBox ID="txtEmail" runat="server" AutoCompleteType="Email"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:CheckBox ID="chkPDPA" runat="server" Text="I accept PDPA." />
            </td>
        </tr>
        <tr>
            <td>
                <asp:CheckBox ID="chkTC" runat="server" Text="Terms & Conditions" />
            </td>
        </tr>
    </table>
</asp:Content>
