<%@ Page Language="C#" %>
<%@ Import Namespace="System.Collections.Generic" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
    "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
    protected void Page_Load(Object sender, EventArgs e) 
    {
	List<string> vals = new List<string>{"One", "Two", "One Two Three Four"};
	testList.DataSource = vals;
	testList.DataBind();
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>ASP.NET WebForms Databound Dropdown List</title>
</head>
<body>
    <div>
	This tests a simple data bind from page load.
	<br/><br/><br/>

	<form runat="server">
    		<asp:DropDownList ID="testList" runat="server" />
	</form>
    </div>
    <div style="margin-top:30px;">&nbsp;</div>
</body>
</html>
