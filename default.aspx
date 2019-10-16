<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
    "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
    protected String SayHey()
    {
        return "What do you say? Hey!";
    }

    protected void cmdSave_Click(Object sender, EventArgs e) 
    {
    steve.Text = "You actually got clicked!";
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>ASP.NET WebForms Hello From Mono (Kestrel special)</title>
</head>
<body>
    <div>
        <%=SayHey()%>
    <br /><br /><br />
    With a form <br /><br />
    <form id="myform" runat="server">
        Input Something: <asp:TextBox ID="steve" Text="Hello TextBox" runat="server" />
        <br /><br />
        <asp:Button ID="cmdSave" Text="Click Me!" OnClick="cmdSave_Click" runat="server" />
    </form>
    </div>
    <div style="margin-top:30px;">&nbsp;</div>
</body>
</html>
