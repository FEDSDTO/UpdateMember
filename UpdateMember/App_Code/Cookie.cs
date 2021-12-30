using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace UpdateMember.App_Code
{
    public class Cookie
    {
        public string GetId()
        {
            //取得cookie中的使用者流水號value
            string _value = HttpContext.Current.Request.Cookies[".ASPXAUTH"].Value;
            FormsAuthenticationTicket Ticket = FormsAuthentication.Decrypt(_value);
            return Ticket.Name;
        }
    }
}