<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Sentiment.Web.Api.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sentiment Home</title>
</head>
<body>
    <font size="5">Sentiment REST API</font><br />
    <br />
    Endpoints:<br />
    <br />
    Tweet Controller:<br />
    /tweets/byId/{id}<br />
    /tweets/byCfg/{configId}/{startDate?} (limited to first 100 tweets)<br />
    /tweets/byKeyword/{keyword}/{startDate?} (limited to first 100 tweets)<br />
    <br />
    Sentiment Controller:<br />
    /sentiment/byCfg/{configId}/{startDate?}<br />
    /sentiment/byKeyword/{keyword}/{startDate?}<br />
    <br />
    Config Controller:<br />
    /config/all<br />
    /config/byId/{id}<br />
    /config/byUser/{user}<br />
    /config/post (this is for http post requests)<br />
    /config/upload (this is an upload page)<br />
    <br />
    System Controller:<br />
    /system/ip<br />
    <br />
    <br />
    Server is running the:<br />
    <div id="analyzer" runat="server"></div>
    sentiment analyzer.
</body>
</html>
