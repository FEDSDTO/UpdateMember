using UpdateMember.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Web.Security;
using UpdateMember.App_Code;
using System.Configuration;

namespace UpdateMember.Controllers
{
    public class LogInController : Controller
    {
        private Func_Log _Log = new Func_Log();
        private GiftsEntities _dbG = new GiftsEntities();
        private MemberCardEntities _dbLOG = new MemberCardEntities();
        private Cookie _Cookie = new Cookie();
        private string _SQL = string.Empty;
        private List<SqlParameter> _Parameter = new List<SqlParameter>();

        // GET: LogIn
        public ActionResult LogIn()
        {
            //if (User.Identity.IsAuthenticated)
            //{
            //    // Log Insert
            //    //string _UserNo = _Cookie.GetId();
            //    //string _EventLog = string.Format(@"{0} - Login - 會員Cookie有效，直接登入", _UserNo);
            //    //_Log.InsertSystemLog(_EventLog, _UserNo);                

            //    return RedirectToAction("Index", "UpdateMember");
            //}
            //else
            //{
            //    // Log Insert
            //    //string _UserNo = _Cookie.GetId();
            //    //string _EventLog = string.Format(@"{0} - Login - Cookie到期", _UserNo);
            //    //_Log.InsertSystemLog(_EventLog, _UserNo);

            //    //若為cookie到期自動登出時，轉頁前先清除cookie
            //    FormsAuthentication.SignOut();
            //}
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(string userNo, string userPassword)
        {
            int MAX_LOGIN_TIME = int.Parse(ConfigurationManager.AppSettings["MAX_LOGIN_TIME"]);
            string ePassword = SHA512Encryption(userPassword);
            //Response.Cookies["userNo"].Value = userNo;
            //Response.Cookies["userPassword"].Value = userPassword;
            var result = _dbG.UserInfo_Setting.Where(us => us.AuthId == 1 || us.AuthId == 7).ToList();
            var finresult = result.Join(_dbG.UserInfo_Main, us => us.UserId, um => um.Id, (us, um) => new { uno = um.UserNo, pwd = um.Password }).ToList();
            var fin = finresult.Where(o => o.uno == userNo && o.pwd == ePassword).FirstOrDefault();
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, userNo, DateTime.Now, DateTime.Now.AddMinutes(MAX_LOGIN_TIME), false, "", FormsAuthentication.FormsCookiePath);
            string enTicket = FormsAuthentication.Encrypt(ticket);

            // Set Cookie
            HttpCookie _HC = new HttpCookie(FormsAuthentication.FormsCookieName, enTicket);
            _HC.Expires = DateTime.Now.AddMinutes(MAX_LOGIN_TIME);
            Response.Cookies.Add(_HC); //將資料存入cookie

            _HC = new HttpCookie("LogOutTime", DateTimeOffset.UtcNow.AddMinutes(MAX_LOGIN_TIME).ToUnixTimeMilliseconds().ToString()); //存登入到期時間
            _HC.Expires = DateTime.Now.AddMinutes(MAX_LOGIN_TIME);
            Response.Cookies.Add(_HC);

            _HC = new HttpCookie("SystemTime", DateTimeOffset.UtcNow.AddMinutes(0).ToUnixTimeMilliseconds().ToString()); //存系統時間
            _HC.Expires = DateTime.Now.AddMinutes(MAX_LOGIN_TIME);
            Response.Cookies.Add(_HC);

            if (fin != null)
            {
                var rresult = new
                {
                    IsSuccess = true,
                };

                // Log Insert
                string _UserNo = _Cookie.GetId();
                string _EventLog = string.Format(@"{0} - 會員帳號密碼驗證成功", _UserNo);
                _Log.InsertSystemLog("Member - Login", _EventLog, _UserNo);

                return Json(rresult, JsonRequestBehavior.AllowGet);
                //return RedirectToAction("Index", "UpdateMember");
                //return View("Index", "UpdateMember");
            }
            else
            {
                var rresult = new
                {
                    IsSuccess = false,
                };
                //return Content(Newtonsoft.Json.JsonConvert.SerializeObject(rresult), "application/json");

                // Log Insert
                string _UserNo = _Cookie.GetId();
                string _EventLog = string.Format(@"{0} - 會員帳號密碼驗證失敗", _UserNo);
                _Log.InsertSystemLog("Member - Login", _EventLog, _UserNo);

                return Json(rresult, JsonRequestBehavior.AllowGet);
            }


        }

        /// <summary>
        /// 將欲加密的文字以SHA512的方式加密
        /// </summary>
        /// <param name="EncryptionText">加密文字</param>
        /// <returns></returns>
        public string SHA512Encryption(string EncryptionText)
        {
            //建立一個SHA512
            SHA512 EncryptionSHA = new SHA512CryptoServiceProvider();
            //將字串轉成byte[]
            byte[] source = Encoding.Default.GetBytes(EncryptionText);
            //進行SHA512加密
            byte[] crypo = EncryptionSHA.ComputeHash(source);

            //return Convert.ToBase64String(crypo);
            return BitConverter.ToString(crypo).Replace("-", string.Empty).ToLower();
        }


        public ActionResult LogOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Log Insert
                string _UserNo = _Cookie.GetId();
                string _EventLog = string.Format(@"{0} - 會員登出", _UserNo);
                _Log.InsertSystemLog("Member - Login", _EventLog, _UserNo);
                FormsAuthentication.SignOut();
                Session.Abandon(); //清除Session內容
            }
            return RedirectToAction("Login", "Login");
        }
        public ActionResult CheckAuthenticated()
        {
            bool isAuth = User.Identity.IsAuthenticated;
            return Json(new { isAuthenticated = isAuth }, JsonRequestBehavior.AllowGet);
        }
    }
}
