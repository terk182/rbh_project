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
    public string sURL = "https://www5.plusibe.com/GogojiiUAT/";
    void Page_Load (Object sender, EventArgs e)
    {
        Response.Write("Page_Load");
        bool bEnableKiwi = ConfigurationManager.AppSettings["KIWI.Enable"] != null ? Convert.ToBoolean(ConfigurationManager.AppSettings["KIWI.Enable"]) : false;
        Response.Write("<br/>bEnableKiwi=" + bEnableKiwi.ToString());
        BL.Utilities.MemoryCacher cacher = new BL.Utilities.MemoryCacher();
        if (Request["Name"] != null && Request["Name"].ToString() == "C_KiwiMarkup")
        {
            if (cacher.GetValue("C_KiwiMarkup") != null)
            {
                Response.Write("<br/>1.KiwiMarkup Not Null");
            }
            else if (System.Web.HttpContext.Current.Cache["C_KiwiMarkup"] != null)
            {
                Response.Write("<br/>2.KiwiMarkup Not Null");
            }
            else
            {
                Response.Write("<br/>KiwiMarkup is Null");
            }

        }
        else if (Request["Name"] != null && Request["Name"].ToString() == "C_FARENET")
        {
            if (cacher.GetValue("C_FARENET") != null)
            {
                Response.Write("<br/>1.Markup Not Null");
            }
            else if (System.Web.HttpContext.Current.Cache["C_FARENET"] != null)
            {
                Response.Write("<br/>2.Markup Not Null");
            }
            else
            {
                Response.Write("<br/>Markup is Null");
            }
        }
        else if (Request["Name"] != null && Request["Name"].ToString() == "Cookie_RecentView")
        {
            if (HttpContext.Current.Request.Cookies["Cookie_RecentView"] != null)
            {
                Response.Write("<br/>RecentView Not Null");
            }
            else
            {
                Response.Write("<br/>RecentView is Null");
            }
        }
        else if (Request["Name"] != null && Request["Name"].ToString() == "C_ALL_HOTEL")
        {
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
        else if (Request["Name"] != null && Request["Name"].ToString() == "C_ALL_HOTEL_EX")
        {
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
        else if (Request["Name"] != null && Request["Name"].ToString() == "C_KiwiAirlineControl")
        {
            if (cacher.GetValue("C_KiwiAirlineControl") != null)
            {
                Response.Write("<br/>1.KiwiAirlineControl Not Null");
            }
            else if (System.Web.HttpContext.Current.Cache["C_KiwiAirlineControl"] != null)
            {
                Response.Write("<br/>2.KiwiAirlineControl Not Null");
            }
            else
            {
                Response.Write("<br/>KiwiAirlineControl is Null");
            }

        }//C_KiwiCondition
        else if (Request["Name"] != null && Request["Name"].ToString() == "C_KiwiCondition")
        {
            if (cacher.GetValue("C_KiwiCondition") != null)
            {
                Response.Write("<br/>1.KiwiCondition Not Null");
            }
            else if (System.Web.HttpContext.Current.Cache["C_KiwiCondition"] != null)
            {
                Response.Write("<br/>2.KiwiCondition Not Null");
            }
            else
            {
                Response.Write("<br/>KiwiCondition is Null");
            }

        }
        else if (Request["Name"] != null && Request["Name"].ToString() == "C_AIRLINE")
        {
            if (cacher.GetValue("C_AIRLINE") != null)
            {
                Response.Write("<br/>1.AIRLINE Not Null");
            }
            else if (System.Web.HttpContext.Current.Cache["C_AIRLINE"] != null)
            {
                Response.Write("<br/>2.AIRLINE Not Null");
            }
            else
            {
                Response.Write("<br/>AIRLINE is Null");
            }

        }
        else if (Request["Name"] != null && Request["Name"].ToString() == "C_COUNTRY")
        {
            if (cacher.GetValue("C_COUNTRY") != null)
            {
                Response.Write("<br/>1.COUNTRY Not Null");
            }
            else if (System.Web.HttpContext.Current.Cache["C_COUNTRY"] != null)
            {
                Response.Write("<br/>2.COUNTRY Not Null");
            }
            else
            {
                Response.Write("<br/>COUNTRY is Null");
            }

        }
        else if (Request["Name"] != null && Request["Name"].ToString() == "C_CITY")
        {
            if (cacher.GetValue("C_CITY") != null)
            {
                Response.Write("<br/>1.CITY Not Null");
            }
            else if (System.Web.HttpContext.Current.Cache["C_CITY"] != null)
            {
                Response.Write("<br/>2.CITY Not Null");
            }
            else
            {
                Response.Write("<br/>CITY is Null");
            }

        }

        Response.Write(String.Format("<br/> Clear Kiwi Markup <a href='{0}ClearCookies.aspx?Name=C_KiwiMarkup'>Click<a>",sURL));
        Response.Write(String.Format("<br/> Clear Kiwi AirlineControl <a href='{0}ClearCookies.aspx?Name=C_KiwiAirlineControl'>Click<a>",sURL));
        Response.Write(String.Format("<br/> Clear Kiwi Condition <a href='{0}ClearCookies.aspx?Name=C_KiwiCondition'>Click<a>",sURL));
        Response.Write(String.Format("<br/> Clear Amadeus Markup <a href='{0}ClearCookies.aspx?Name=C_FARENET'>Click<a>",sURL));
        Response.Write(String.Format("<br/> Clear Cookie_RecentView <a href='{0}ClearCookies.aspx?Name=Cookie_RecentView'>Click<a>",sURL));
        Response.Write(String.Format("<br/> Clear ALL_HOTEL <a href='{0}ClearCookies.aspx?Name=C_ALL_HOTEL'>Click<a>",sURL));
        Response.Write(String.Format("<br/> Clear ALL_HOTEL_EX <a href='{0}ClearCookies.aspx?Name=C_ALL_HOTEL_EX'>Click<a>",sURL));
        Response.Write(String.Format("<br/> Clear AIRLINE <a href='{0}ClearCookies.aspx?Name=C_AIRLINE'>Click<a>",sURL));
        Response.Write(String.Format("<br/> Clear COUNTRY <a href='{0}ClearCookies.aspx?Name=C_COUNTRY'>Click<a>",sURL));
        Response.Write(String.Format("<br/> Clear CITY <a href='{0}ClearCookies.aspx?Name=C_CITY'>Click<a>",sURL));
    }//method
    void SaveButton_ServerClick(Object sender, EventArgs e)
    {
        Response.Write("<br/>Click");
        try
        {
            BL.Utilities.MemoryCacher cacher = new BL.Utilities.MemoryCacher();
            if (Request["Name"] != null && Request["Name"].ToString() == "Cookie_RecentView" && HttpContext.Current.Request.Cookies["Cookie_RecentView"] != null)
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
            else if (Request["Name"] != null && Request["Name"].ToString() == "C_ALL_HOTEL_EX" && (cacher.GetValue("C_ALL_HOTEL_EX_EN") != null || cacher.GetValue("C_ALL_HOTEL_EX_TH") != null))
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
            else if (Request["Name"] != null && Request["Name"].ToString() == "C_FARENET" && cacher.GetValue("C_FARENET") != null )
            {
                cacher.Delete("C_FARENET");
                if (cacher.GetValue("C_FARENET") != null)
                {
                    Response.Write("<br/>FARENET Not Null");
                }
                else
                {
                    Response.Write("<br/>FARENET Is Null");
                }
            }
            else if (Request["Name"] != null && Request["Name"].ToString() == "C_KiwiMarkup" && cacher.GetValue("C_KiwiMarkup") != null)
            {
                cacher.Delete("C_KiwiMarkup");
                if (cacher.GetValue("C_KiwiMarkup") != null)
                {
                    Response.Write("<br/>KiwiMarkup Not Null");
                }
                else
                {
                    Response.Write("<br/>KiwiMarkup Is Null");
                }
            }
            else if (Request["Name"] != null && Request["Name"].ToString() == "C_KiwiAirlineControl" && cacher.GetValue("C_KiwiAirlineControl") != null)
            {
                cacher.Delete("C_KiwiAirlineControl");
                if (cacher.GetValue("C_KiwiAirlineControl") != null)
                {
                    Response.Write("<br/>KiwiAirlineControl Not Null");
                }
                else
                {
                    Response.Write("<br/>KiwiAirlineControl Is Null");
                }
            }
            else if (Request["Name"] != null && Request["Name"].ToString() == "C_KiwiCondition" && cacher.GetValue("C_KiwiCondition") != null)
            {
                cacher.Delete("C_KiwiCondition");
                if (cacher.GetValue("C_KiwiCondition") != null)
                {
                    Response.Write("<br/>KiwiCondition Not Null");
                }
                else
                {
                    Response.Write("<br/>KiwiCondition Is Null");
                }
            }
            else if (Request["Name"] != null && Request["Name"].ToString() == "C_AIRLINE" && cacher.GetValue("C_AIRLINE") != null)
            {
                cacher.Delete("C_AIRLINE");
                if (cacher.GetValue("C_AIRLINE") != null)
                {
                    Response.Write("<br/>AIRLINE Not Null");
                }
                else
                {
                    Response.Write("<br/>AIRLINE Is Null");
                }
            }
            else if (Request["Name"] != null && Request["Name"].ToString() == "C_COUNTRY" && cacher.GetValue("C_COUNTRY") != null)
            {
                cacher.Delete("C_COUNTRY");
                if (cacher.GetValue("C_COUNTRY") != null)
                {
                    Response.Write("<br/>COUNTRY Not Null");
                }
                else
                {
                    Response.Write("<br/>COUNTRY Is Null");
                }
            }
            else if (Request["Name"] != null && Request["Name"].ToString() == "C_CITY" && cacher.GetValue("C_CITY") != null)
            {
                cacher.Delete("C_CITY");
                if (cacher.GetValue("C_CITY") != null)
                {
                    Response.Write("<br/>CITY Not Null");
                }
                else
                {
                    Response.Write("<br/>CITY Is Null");
                }
            }//

        }
        catch(Exception ex)
        {
            Response.Write("Error !!!"+ex.ToString());
        }

    }

</script>
<br>
