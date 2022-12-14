<%@ Page Language="C#" Debug="true"%>
<%@ Import Namespace="System.Net.Mail" %>
<%@ Import Namespace="System.Configuration" %>
<%@ Import Namespace="System.Data" %>
<html>
<body>
<form id="dataForm" method="post" runat="server">
From: <asp:TextBox id="txtFrom" runat="server" Width="300px"></asp:TextBox><br>
To: <asp:TextBox id="txtTo" runat="server" Width="300px"></asp:TextBox><br>
Cc: <asp:TextBox id="txtCc" runat="server" Width="300px"></asp:TextBox><br>
Bcc: <asp:TextBox id="txtBcc" runat="server" Width="300px"></asp:TextBox><br>
<input class="button" id="SaveButton" onmouseover="this.style.color='#808080';" onmouseout="this.style.color='#000000';" type="button" value="Sent By.Net" name="SaveButton" runat="server" onserverclick="SaveButton_ServerClick"/>
<input class="button" id="SaveWebButton" onmouseover="this.style.color='#808080';" onmouseout="this.style.color='#000000';" type="button" value="Sent By.Web" name="SaveWebButton" runat="server" onserverclick="SaveWebButton_ServerClick"/>
</form>
</body>
</html>

<script language="C#" runat="server">

void Page_Load (Object sender, EventArgs e)
{
	
}//method
void SaveButton_ServerClick(Object sender, EventArgs e)
{
    try
	{

		MailMessage mailMsg = new MailMessage();
		mailMsg.BodyEncoding = System.Text.Encoding.UTF8;
		mailMsg.IsBodyHtml = true;

		string MailForm = txtFrom.Text;
		mailMsg.From =new MailAddress(MailForm); 
		mailMsg.To.Add(new MailAddress(txtTo.Text));
		mailMsg.CC.Add(txtCc.Text.Replace(";", ","));
		mailMsg.Bcc.Add(txtBcc.Text);
		mailMsg.Subject = "My test";
		mailMsg.Body = "Hello";
		SmtpClient client = new SmtpClient("smtp.office365.com");    
		client.EnableSsl = true;
        client.UseDefaultCredentials = false;		
        client.Credentials = new  System.Net.NetworkCredential("rsvn@gogojii.com", "Gon02ep!y");
		
        client.Send(mailMsg);
		Response.Write("OK");
	}
	catch(Exception ex)
	{
		Response.Write("Error !!!"+ex.ToString());
	}

}
void SaveWebButton_ServerClick(Object sender, EventArgs e)
{
    try
	{

		System.Web.Mail.MailMessage mailMsg = new System.Web.Mail.MailMessage();
		mailMsg.From = txtFrom.Text;
		mailMsg.To = txtTo.Text;
		mailMsg.Cc = txtCc.Text;
		mailMsg.Bcc = txtBcc.Text;
		mailMsg.Subject = "My test";
		mailMsg.Body = "System.Web.Mail Hello";
		System.Web.Mail.SmtpMail.SmtpServer = ConfigurationSettings.AppSettings["Mail.SmtpServer"];		
		System.Web.Mail.SmtpMail.Send(mailMsg);				
		
		Response.Write("System.Web.Mail OK");
	}
	catch(Exception ex)
	{
		Response.Write("System.Web.Mail Error !!!"+ex.ToString());
	}

}
</script>
<br>
