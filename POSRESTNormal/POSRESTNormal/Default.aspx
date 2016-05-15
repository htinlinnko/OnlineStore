
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="POSRESTNormal.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="./Scripts/jquery-1.10.2.js" type="text/javascript"></script>
        <script src="./Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
        <script type="text/javascript">
            function getHello() {
                $.ajax({
                    type: "GET",
                    url: "/POS.SVC/getHello",
                    data: {},
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        $('#txtHello').val(response.d);
                    },
                    error: function (response) {
                        alert(response.d);
                    }
                });
            }
        </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Button ID="btnGetHello" runat="server" Text="Get Hello" OnClientClick="getHello(); return false;" />
        <asp:TextBox ID="txtHello" runat="server" />
    </form>
</body>
</html>