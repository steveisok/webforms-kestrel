<%@ Page Language="C#" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Data" %>
<%@ Register TagPrefix="steve" TagName="Test" Src="~/mycontrol.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
    "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
   
</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>ASP.NET WebForms UserControl Test</title>
</head>
<body>
    <div>
	This is a user control test
	<br/><br/>

	<form runat="server">
		<h3>User Control</h3>

		<steve:Test runat="server" />
	</form>
    </div>
    <div style="margin-top:30px;">&nbsp;</div>
</body>
</html>
