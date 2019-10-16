<%@ Page Language="C#" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Data" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
    "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
   void Page_Load(Object sender, EventArgs e) 
      {
         // Show/Hide Panel Contents.
         if (Check1.Checked) 
         {
            Panel1.Visible=false;
         }
         else 
         {
            Panel1.Visible=true;
         }
 
         // Generate label controls.
         int numlabels = Int32.Parse(DropDown1.SelectedItem.Value);
         for (int i=1; i<=numlabels; i++) 
         {
            Label l = new Label();
            l.Text = "Label" + (i).ToString();
            l.ID = "Label" + (i).ToString();
            Panel1.Controls.Add(l);
            Panel1.Controls.Add(new LiteralControl("<br>"));
         }
 
         // Generate textbox controls.
         int numtexts = Int32.Parse(DropDown2.SelectedItem.Value);
         for (int i=1; i<=numtexts; i++) 
         {
            TextBox t = new TextBox();
            t.Text = "TextBox" + (i).ToString();
            t.ID = "TextBox" + (i).ToString();
            Panel1.Controls.Add(t);
            Panel1.Controls.Add(new LiteralControl("<br>"));
         }
      }
</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>ASP.NET WebForms Panel Test</title>
</head>
<body>
    <div>
	Panel Test
	<br/><br/>

	<form runat="server">
		<h3>Panel Example</h3>

		<asp:Panel id="Panel1" runat="server"
		   BackColor="gainsboro"
		   Height="200px"
		   Width="300px">
		   Panel1: Here is some static content...
		   <p>
	      	</asp:Panel>
	      	<p>
	      	Generate Labels:
	      	<asp:DropDownList id=DropDown1 runat="server">
		 <asp:ListItem Value="0">0</asp:ListItem>
		 <asp:ListItem Value="1">1</asp:ListItem>
		 <asp:ListItem Value="2">2</asp:ListItem>
		 <asp:ListItem Value="3">3</asp:ListItem>
		 <asp:ListItem Value="4">4</asp:ListItem>
	      	</asp:DropDownList>
	      	<br>
	      	Generate TextBoxes:
	      	<asp:DropDownList id=DropDown2 runat="server">
		 <asp:ListItem Value="0">0</asp:ListItem>
		 <asp:ListItem Value="1">1</asp:ListItem>
		 <asp:ListItem Value="2">2</asp:ListItem>
		 <asp:ListItem Value="3">3</asp:ListItem>
		 <asp:ListItem Value="4">4</asp:ListItem>
	      	</asp:DropDownList>
	      	<p>
	      	<asp:CheckBox id="Check1" Text="Hide Panel" runat="server"/>
	      	<p>
	      	<asp:Button Text="Refresh Panel" runat="server"/>
	</form>
    </div>
    <div style="margin-top:30px;">&nbsp;</div>
</body>
</html>
