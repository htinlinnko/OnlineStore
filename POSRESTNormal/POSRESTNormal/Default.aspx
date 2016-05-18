
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
                    url: "http://192.168.0.146/POSRESTNormal/POS.SVC/getHello",
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

            function getAnotherFunction() {
                var paraString = $('#txtHello').val();

                var parValue = [
                    { parameter1: paraString }
                ]

                $.ajax({
                    type: "GET",
                    url: "http://192.168.0.146/POSRESTNormal/POS.SVC/getTestParameter",
                    data: {parameter1: paraString},
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        alert(response.d);
                    },
                    error: function (response) {
                        alert(response.d);
                    }
                });
            }

            function InsertToDB() {
                var paraString = $('#txtTextInsert').val();

                if (paraString == null || paraString == '') {
                    alert('Type text to insert...');
                    return false;
                }

                parVal = {
                    dataValue: paraString
                };

                $.ajax({
                    type: "GET",
                    url: "http://192.168.0.146/POSRESTNormal/POS.SVC/insertData",
                    data: { dataValue: paraString },
                    processData: true,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        alert(response.d);
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
        <asp:Button ID="btnAnotherButton" runat="server" Text="Another" OnClientClick="getAnotherFunction(); return false;" />
        <asp:TextBox ID="txtHello" runat="server" />
        <hr />
        Insert data from the textbox ; 
        Text to insert : 
        <asp:TextBox ID="txtTextInsert" runat="server" placeholder="Type text..." />
        <br />
        <asp:Button ID="btnInsertToDB" runat="server" Text="Insert" OnClientClick="InsertToDB(); return false;" />
    </form>
</body>
</html>