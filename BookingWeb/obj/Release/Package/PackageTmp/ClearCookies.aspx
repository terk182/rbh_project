<%@ Page Language="C#" Debug="true"%>
<%@ Import Namespace="System.Web" %>
<html>
<body>
<form id="dataForm" method="post" runat="server">
    
<input class="button" id="SaveButton" onmouseover="this.style.color='#808080';" onmouseout="this.style.color='#000000';" type="button" value="Clear" name="SaveButton" runat="server" onserverclick="SaveButton_ServerClick"/>
</form>
</body>
</html>

<script language="C#" runat="server">

    void Page_Load (Object sender, EventArgs e)
    {
        Response.Write("Page_Load");

    }//method
    void SaveButton_ServerClick(Object sender, EventArgs e)
    {
        try
        {
            BL.Utilities.MemoryCacher cacher = new BL.Utilities.MemoryCacher();
            if (Request["Name"]!=null && Request["Name"].ToString()=="Cookie_RecentView" &&  HttpContext.Current.Request.Cookies["Cookie_RecentView"] != null)
            {
                HttpCookie aCookie = HttpContext.Current.Request.Cookies["Cookie_RecentView"];
                aCookie.Value = "";
                aCookie.Expires = DateTime.Now.AddSeconds(1);
                HttpContext.Current.Response.Cookies.Clear();
                //HttpContext.Current.Response.Cookies.Add(aCookie);
                if (HttpContext.Current.Request.Cookies["Cookie_RecentView"] != null)
                {
                    Response.Write("<br/>Recent Views Not Null");
                }
                else
                {
                    Response.Write("<br/>Recent Views Is Null");
                }
            }
            else if (Request["Name"] != null && Request["Name"].ToString() == "C_ALL_HOTEL" && (cacher.GetValue("C_ALL_HOTEL_EN") != null || cacher.GetValue("C_ALL_HOTEL_TH") != null))
            {
                
                cacher.Delete("C_ALL_HOTEL_EN");
                cacher.Delete("C_ALL_HOTEL_TH");
                if (cacher.GetValue("<br/>C_ALL_HOTEL_EN") != null)
                {
                    Response.Write("<br/>C_ALL_HOTEL_EN Not Null");
                }
                else
                {
                    Response.Write("<br/>C_ALL_HOTEL_EN Is Null");
                }
                if (cacher.GetValue("C_ALL_HOTEL_TH") != null)
                {
                    Response.Write("<br/>C_ALL_HOTEL_TH Not Null");
                }
                else
                {
                    Response.Write("<br/>C_ALL_HOTEL_TH Is Null");
                }
            }
            else if(Request["Name"] != null && Request["Name"].ToString() == "C_ALL_HOTEL_EX" &&  (cacher.GetValue("C_ALL_HOTEL_EX_EN") != null || cacher.GetValue("C_ALL_HOTEL_EX_TH") != null))
            {
               
                cacher.Delete("C_ALL_HOTEL_EX_EN");
                cacher.Delete("C_ALL_HOTEL_EX_TH");
                if (cacher.GetValue("C_ALL_HOTEL_EX_EN") != null)
                {
                    Response.Write("<br/>C_ALL_HOTEL_EX_EN Not Null");
                }
                else
                {
                    Response.Write("<br/>C_ALL_HOTEL_EX_EN Is Null");
                }
                if (cacher.GetValue("C_ALL_HOTEL_EX_TH") != null)
                {
                    Response.Write("<br/>C_ALL_HOTEL_EX_TH Not Null");
                }
                else
                {
                    Response.Write("<br/>C_ALL_HOTEL_EX_TH Is Null");
                }
            }

        }
        catch(Exception ex)
        {
            Response.Write("Error !!!"+ex.ToString());
        }

    }

</script>
<br>
