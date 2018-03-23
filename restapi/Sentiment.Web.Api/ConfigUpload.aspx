<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConfigUpload.aspx.cs" Inherits="Sentiment.Web.Api.ConfigUpload" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Config Upload</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <font size="3">Upload senitment configuration file:</font>
            <br /><br />
            <input id="FileInput" runat="server" type="file" /> 
            <br />
            <div id="messageArea" runat="server"></div>
            <br />
            <input id="ButtonSubmit" runat="server" type="button" value="Submit" onserverclick="ButtonSubmit_Click" />
        </div>
    </form>
</body>
</html>
