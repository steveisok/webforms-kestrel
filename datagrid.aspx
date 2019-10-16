<%@ Page Language="C#" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Data" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
    "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
    ICollection CreateDataSource() 
   {
      DataTable dt = new DataTable();
      DataRow dr;

      dt.Columns.Add(new DataColumn("IntegerValue", typeof(Int32)));
      dt.Columns.Add(new DataColumn("StringValue", typeof(string)));
      dt.Columns.Add(new DataColumn("CurrencyValue", typeof(double)));

      for (int i = 0; i < 9; i++) 
      {
         dr = dt.NewRow();

         dr[0] = i;
         dr[1] = "Item " + i.ToString();
         dr[2] = 1.23 * (i + 1);

         dt.Rows.Add(dr);
      }

      DataView dv = new DataView(dt);
      return dv;
   }

   void Page_Load(Object sender, EventArgs e) 
   {

      if (!IsPostBack) 
      {
         // Need to load this data only once.
         ItemsGrid.DataSource= CreateDataSource();
         ItemsGrid.DataBind();
      }
   }
</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>ASP.NET WebForms DataGrid Test</title>
</head>
<body>
    <div>
	This a data bound data grid.
	<br/><br/>

	<form runat="server">
		<h3>DataGrid Example</h3>

		<b>Product List</b>

		<asp:DataGrid id="ItemsGrid"
		   BorderColor="black"
		   BorderWidth="1"
		   CellPadding="3"
		   HeaderStyle-BackColor="#00aaaa"
		   AutoGenerateColumns="true"
		   runat="server">
		</asp:DataGrid>
	</form>
    </div>
    <div style="margin-top:30px;">&nbsp;</div>
</body>
</html>
