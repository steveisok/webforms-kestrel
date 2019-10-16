<%@ Page Language="C#" %>
<%@ Import Namespace="System.Collections.Generic" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
    "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
    protected void Page_Load(Object sender, EventArgs e) 
    {
	if (!IsPostBack) 
        {
            ArrayList values = new ArrayList();
            values.Add(new PositionData("Microsoft", "Msft"));
            values.Add(new PositionData("Intel", "Intc"));
            values.Add(new PositionData("Dell", "Dell"));
            Repeater1.DataSource = values;
            Repeater1.DataBind();
                
            Repeater2.DataSource = values;
            Repeater2.DataBind();
        }
    }

     public class PositionData 
     {
         private string name;
         private string ticker;

         public PositionData(string name, string ticker) 
         {
            this.name = name;
            this.ticker = ticker;
         }

         public string Name 
         {
            get 
            {
               return name;
            }
         }

         public string Ticker 
         {
            get 
            {
               return ticker;
            }
         }
      } 
</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>ASP.NET WebForms Repeater Test</title>
</head>
<body>
    <div>
	This a data bound repeater.
	<br/><br/><br/>

	<form runat="server">
    		<h3>Repeater Example</h3>
      		<b>Repeater1:</b>
      		<p>
      		<asp:Repeater id=Repeater1 runat="server">
         		<HeaderTemplate>
			    <table border=1>
			       <tr>
				  <td><b>Company</b></td> 
				  <td><b>Symbol</b></td>
			       </tr>
			 </HeaderTemplate>
         		<ItemTemplate>
            			<tr>
			       <td> 
				  <%# DataBinder.Eval(Container.DataItem, "Name") %> 
			       </td>
			       <td> 
				  <%# DataBinder.Eval(Container.DataItem, "Ticker") %>
			       </td>
			    </tr>
			 </ItemTemplate>
			 <FooterTemplate>
			    </table>
			 </FooterTemplate>
	      </asp:Repeater>
	      <p>
	      <b>Repeater2:</b>
	      <p>
	      <asp:Repeater id=Repeater2 runat="server">
		 <HeaderTemplate>
		    Company data:
		 </HeaderTemplate> 
		 <ItemTemplate>
		    <%# DataBinder.Eval(Container.DataItem, "Name") %> 
		    (<%# DataBinder.Eval(Container.DataItem, "Ticker") %>)
		 </ItemTemplate>
		 <SeparatorTemplate>
		    , 
		 </SeparatorTemplate>
	      </asp:Repeater>
	</form>
    </div>
    <div style="margin-top:30px;">&nbsp;</div>
</body>
</html>
