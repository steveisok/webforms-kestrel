<%@ Page Language="C#" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Data" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
    "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
   
</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>ASP.NET WebForms DataGrid Test</title>
</head>
<body>
    <div>
	This a validation summary example.
	<br/><br/>

   <form runat="server">

      <h3>ValidationSummary Sample</h3>

      <table cellpadding="10">
         <tr>
            <td>
               <table bgcolor="#eeeeee" cellpadding="10">

                  <tr>
                     <td colspan="3">
                        <b>Credit Card Information</b>
                     </td>
                  </tr>
                  <tr>
                     <td align="right">
                        Card Type:
                     </td>
                     <td>
                        <asp:RadioButtonList id="RadioButtonList1" 
                             RepeatLayout="Flow"
                             runat=server>

                           <asp:ListItem>MasterCard</asp:ListItem>
                           <asp:ListItem>Visa</asp:ListItem>

                        </asp:RadioButtonList>
                     </td>
                     <td align="middle" rowspan="1">
                        <asp:RequiredFieldValidator
                             id="RequiredFieldValidator1"
                             ControlToValidate="RadioButtonList1"
                             ErrorMessage="Card Type."
                             Display="Static"
                             InitialValue="" 
                             Width="100%" 
                             Text="*"
                             runat="server"/>
                     </td>
                  </tr>
                  <tr>
                     <td align="right">
                        Card Number:
                     </td>
                     <td>
                        <asp:TextBox id="TextBox1" 
                             runat="server" />
                     </td>
                     <td>
                        <asp:RequiredFieldValidator
                             id="RequiredFieldValidator2"
                             ControlToValidate="TextBox1" 
                             ErrorMessage="Card Number. "
                             Display="Static"
                             Width="100%"
                             Text="*" 
                             runat=server/>
                     </td>
                  </tr>

                  <tr>
                     <td></td>
                     <td>
                        <asp:Button id="Button1" 
                             Text="Validate" 
                             runat=server />
                     </td>
                     <td></td>
                  </tr>
               </table>
            </td>
            <td valign=top>
               <table cellpadding="20">
                  <tr>
                     <td>
                        <asp:ValidationSummary id="valSum" 
                             DisplayMode="BulletList"
                             EnableClientScript="true"
                             HeaderText="You must enter a value in the following fields:"
                             runat="server"/>
                     </td>
                  </tr>
               </table>
            </td>
         </tr>
      </table>

   </form>
    </div>
    <div style="margin-top:30px;">&nbsp;</div>
</body>
</html>
