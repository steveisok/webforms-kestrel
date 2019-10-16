<%@ Control Language="C#" %>
<script runat="server">
    public String Name {
        get
        {
            return labelOutput.Text;
        }
        set
        {
            textName.Text = Server.HtmlEncode(value);
            labelOutput.Text = Server.HtmlEncode(value);
        }
    }
    
    void buttonDisplayName_Click(object sender, EventArgs e) {
       labelOutput.Text = textName.Text;
    }
</script>

<table>
    <tbody>
        <tr>
            <td>
                <b>Enter your name:</b></td>
        </tr>
        <tr>
            <td>
                <asp:TextBox id="textName" 
                    runat="server">
                </asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:button id="buttonDisplayName" 
                   onclick="buttonDisplayName_Click" 
                   runat="server" text="Submit">
                </asp:button>
            </td>
        </tr>
        <tr>
            <td><b>Hello, 
                 <asp:Label id="labelOutput" 
                     runat="server">
                 </asp:Label>.</b>
            </td>
        </tr>
    </tbody>
</table>
