using UpdateMember.Models;
using UpdateMember.Models.FEDSMBR;
using UpdateMember.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Globalization;
using PagedList;
using PagedList.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UpdateMember.App_Code;
using System.Net;
using System.Web.Script.Serialization;
using System.Text;
using System.IO;
using static UpdateMember.Models.viewHGAccreditation;
using System.Net.Http;
using System.Configuration;

namespace UpdateMember.Controllers
{
    public class UpdateMemberController : Controller
    {
        private Func_Log _Log = new Func_Log();
        // GET: FEDSMBR
        private FEDSMBREntities _dbFM = new FEDSMBREntities();
        // GET: MemberCard
        private MemberCardEntities _dbLOG = new MemberCardEntities();
        private DB_Connection _dbsql = new DB_Connection();
        private UpdateMember.App_Code.Cookie _cookie = new App_Code.Cookie();
        //private Cookie _cookie = new Cookie();
        private string _SQL = string.Empty;
        private List<SqlParameter> _Parameter = new List<SqlParameter>();
        Func_GetMemberCard _FGMC = new Func_GetMemberCard();

        //[Authorize]
        public ActionResult Index(int page = 1, int pagesize = 10)
        {
            string _UserNo = string.Empty;
            if (User.Identity.IsAuthenticated)
            {
                _UserNo = _cookie.GetId();
                ViewBag.UserNo = _UserNo;
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            string _EventLog = string.Empty;
            //if (Request.Cookies[".ASPXAUTH"] == null)
            //{
            //    // Log Insert
            //    // _UserNo = _cookie.GetId();
            //    //_EventLog = string.Format(@"{0} - Index - Cookie不存在", _UserNo);
            //    //_Log.InsertSystemLog("Member - Index", _EventLog, _UserNo);

            //    return RedirectToAction("Login", "Login");

            //}
            string _sql = @"SELECT TOP(50) M.Id,ISNULL(M.Name,'-') as Name,ISNULL(M.Email,'-') as Email,Convert(varchar(10),M.Birthday,111)as Birthday,ISNULL(M.Gender,'-') as Gender,ISNULL(M.Cellphone,'-') as Cellphone,ISNULL(M.HgCardNo,'-') as HgCardNo,ISNULL(M.TempHgCardNo,'-') as TempHgCardNo,Convert(varchar(10),M.HgValidDate,111)as HgValidDate,ISNULL(M.RocId,'-') as RocId,Convert(varchar(10),M.RealValidDate,111)as RealValidDate,Convert(varchar(10),ME.ValidDate ,111) as EmailValidDate,Convert(varchar(10),MC.ValidDate ,111) as PhoneValidDate,M.Status
                            FROM 
                            (SELECT*
                            FROM Member) M
                            LEFT JOIN (SELECT MAX(ValidDate) AS ValidDate,MemberId FROM  MemberEmailValid WHERE ValidDate IS NOT NULL GROUP BY MemberId) ME ON M.Id = ME.MemberId
                            LEFT JOIN (SELECT MAX(ValidDate) AS ValidDate,MemberId FROM  MemberCellphoneValid WHERE ValidDate IS NOT NULL GROUP BY MemberId) MC ON M.Id = MC.MemberId";
            DataTable _dt = _dbsql.GetDataTable(_sql);
            List<_VResult._VMmember> result = new List<_VResult._VMmember>();

            //DateTimeFormatInfo _dtFormat = new System.Globalization.DateTimeFormatInfo();
            //_dtFormat.ShortDatePattern = "yyyy/MM/dd";


            //if (_dt.Rows.Count > 0)
            //{
            //    foreach (DataRow item in _dt.Rows)
            //    {
            //        //string _ValidExpire = Convert.ToDateTime(item["ValidExpire"]).ToShortDateString().ToString();
            //        //string _PhoneValidExpire = Convert.ToDateTime(item["PhoneValidExpire"]).ToShortDateString().ToString();
            //        //if(_ValidExpire == "1900/01/01")
            //        //{
            //        //    _ValidExpire = "-";
            //        //}
            //        //if (_PhoneValidExpire == "1900/01/01")
            //        //{
            //        //    _PhoneValidExpire = "-";
            //        //}

            //        result.Add(new _VResult._VMmember
            //        {
            //            ID = Convert.ToInt32(item["Id"]),
            //            Name = item["Name"].ToString(),
            //            ROCId = item["ROCId"].ToString(),
            //            Birthday = Convert.ToDateTime(item["Birthday"]).ToString("yyyy/MM/dd"),
            //            Gender = item["Gender"].ToString(),
            //            Email = item["Email"].ToString(),
            //            Phone = item["Cellphone"].ToString(),
            //            HappyGoCard = item["HappyGoCardNo"].ToString(),
            //            THappyGoCard = item["THappyGoCardNo"].ToString(),
            //            EmailValidExpire = item["ValidExpire"].ToString(),
            //            PhoneValidExpire = item["PhoneValidExpire"].ToString(),
            //            Status = Convert.ToInt32(item["Status"]),
            //            RealVerifyDate = item["RealVerifyDate"].ToString(),
            //        });

            //    }
            //}
            int pageIndex = page < 1 ? 1 : page;

            var model = new _VResult
            {
                _VParameter = new _VParameter(),
                PageIndex = pageIndex,
                Cards = result.ToPagedList(pageIndex, pagesize)
            };

            //return View(result.ToPagedList(page, pagesize));

            // Log Insert
            //_UserNo = _cookie.GetId();
            // _EventLog = string.Format(@"{0} - Index - 進入頁面", _UserNo);
            //_Log.InsertSystemLog(_EventLog, _UserNo);

            return View(model);
        }

        //搜尋
        [HttpPost]
        public ActionResult Index(int? id, string phoneNo, string userId, _VResult vResult, int page = 1, int pagesize = 10)
        {
            string _UserNo = string.Empty;
            if (User.Identity.IsAuthenticated)
            {
                _UserNo = _cookie.GetId();
                ViewBag.UserNo = _UserNo;
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
            string _EventLog = string.Empty;
            //if (Request.Cookies[".ASPXAUTH"] == null)
            //{
            //    // Log Insert
            //    //_UserNo = _cookie.GetId();
            //    //_EventLog = string.Format(@"{0} - Search - Cookie不存在", _UserNo);
            //    //_Log.InsertSystemLog(_EventLog, _UserNo);

            //    return RedirectToAction("Login", "Login");

            //}
            DataTable _dt = new DataTable();
            List<_VResult._VMmember> result = new List<_VResult._VMmember>();

            if (id != null || phoneNo != "" || userId != "")
            {
                string _partialsqlid = string.IsNullOrEmpty(Convert.ToString(id)) ? string.Empty : "WHERE Id = @Id";
                string _partialsqlphoneNo = string.IsNullOrEmpty(phoneNo) ? string.Empty : "WHERE Cellphone = @phoneNo";
                string _partialsqluserId = string.IsNullOrEmpty(userId) ? string.Empty : "WHERE  RocId = @userId";

                if (!string.IsNullOrEmpty(Convert.ToString(id)))
                {
                    _partialsqlphoneNo = string.IsNullOrEmpty(phoneNo) ? string.Empty : " AND Cellphone = @phoneNo";
                    _partialsqluserId = string.IsNullOrEmpty(userId) ? string.Empty : " AND  RocId = @userId";
                }
                else
                {
                    if (!string.IsNullOrEmpty(phoneNo))
                    {
                        _partialsqluserId = string.IsNullOrEmpty(userId) ? string.Empty : " AND  RocId = @userId";
                    }
                }

                string _sql = string.Format(@"SELECT TOP(50) M.Id,ISNULL(M.Name,'-') as Name,ISNULL(M.Email,'-') as Email,Convert(varchar(10),M.Birthday,111)as Birthday,ISNULL(M.Gender,'-') as Gender,ISNULL(M.Cellphone,'-') as Cellphone,ISNULL(M.HgCardNo,'-') as HgCardNo,ISNULL(M.TempHgCardNo,'-') as TempHgCardNo,Convert(varchar(10),M.HgValidDate,111)as HgValidDate,ISNULL(M.RocId,'-') as RocId,Convert(varchar(10),M.RealValidDate,111)as RealValidDate,Convert(varchar(10),ME.ValidDate ,111) as EmailValidDate,Convert(varchar(10),MC.ValidDate ,111) as PhoneValidDate,M.Status
                            FROM 
                            (SELECT*
                            FROM Member
                            {0} {1} {2}) M
                            LEFT JOIN (SELECT MAX(ValidDate) AS ValidDate,MemberId FROM  MemberEmailValid WHERE ValidDate IS NOT NULL GROUP BY MemberId) ME ON M.Id = ME.MemberId
                            LEFT JOIN (SELECT MAX(ValidDate) AS ValidDate,MemberId FROM  MemberCellphoneValid WHERE ValidDate IS NOT NULL GROUP BY MemberId) MC ON M.Id = MC.MemberId", _partialsqlid, _partialsqlphoneNo, _partialsqluserId);

                // Record Search Parameter
                string _SearchParameter = string.Empty;

                if (!string.IsNullOrEmpty(Convert.ToString(id)) && !string.IsNullOrEmpty(phoneNo) && !string.IsNullOrEmpty(userId))
                {
                    _SearchParameter = string.Format(@"搜尋參數：ID - {0} ; Phone - {1} ; UserId - {2}", id.ToString(), phoneNo, userId);

                    _Parameter.Add(new SqlParameter("@Id", id));
                    _Parameter.Add(new SqlParameter("@phoneNo", phoneNo));
                    _Parameter.Add(new SqlParameter("@userId", userId));
                    _dt = _dbsql.GetDataTable(_sql, _Parameter);
                }
                else
                {
                    _SearchParameter = @"搜尋參數：";

                    if (!string.IsNullOrEmpty(Convert.ToString(id)))
                    {
                        _SearchParameter = _SearchParameter + string.Format(@" ID - {0} ;", id.ToString());
                        _Parameter.Add(new SqlParameter("@Id", id));
                    }
                    if (!string.IsNullOrEmpty(phoneNo))
                    {
                        _SearchParameter = _SearchParameter + string.Format(@" Phone - {0} ;", phoneNo);
                        _Parameter.Add(new SqlParameter("@phoneNo", phoneNo));
                    }
                    if (!string.IsNullOrEmpty(userId))
                    {
                        _SearchParameter = _SearchParameter + string.Format(@" UserId - {0} ;", userId);
                        _Parameter.Add(new SqlParameter("@userId", userId));
                    }
                    _dt = _dbsql.GetDataTable(_sql, _Parameter);

                    // Log Insert
                    _UserNo = _cookie.GetId();
                    _EventLog = string.Format(@"{0} - 搜尋條件：{1}", _UserNo, _SearchParameter);
                    _Log.InsertSystemLog("Member - Search", _EventLog, _UserNo);
                }

                if (_dt.Rows.Count > 0)
                {
                    foreach (DataRow item in _dt.Rows)
                    {
                        // 22.08.25 - Add Member Card No
                        string _MemberCardNo = _FGMC.GenerateMemberNo(item["Id"].ToString());

                        result.Add(new _VResult._VMmember
                        {
                            ID = Convert.ToInt32(item["Id"]),
                            Name = item["Name"].ToString() == null ? "-" : item["Name"].ToString(),
                            ROCId = item["RocId"].ToString() == null ? "-" : item["RocId"].ToString(),
                            Birthday = item["Birthday"] == null ? "-" : item["Birthday"].ToString(),
                            Gender = item["Gender"] == null ? "-" : item["Gender"].ToString(),
                            Email = item["Email"] == null ? "-" : item["Email"].ToString(),
                            Phone = item["Cellphone"] == null ? "-" : item["Cellphone"].ToString(),
                            HappyGoCard = item["HgCardNo"] == null ? "-" : item["HgCardNo"].ToString(),
                            THappyGoCard = item["TempHgCardNo"] == null ? "-" : item["TempHgCardNo"].ToString(),
                            HgValidDate = item["HgValidDate"] == null ? "-" : item["HgValidDate"].ToString(),
                            EmailValidDate = item["EmailValidDate"] == null ? "-" : item["EmailValidDate"].ToString(),
                            PhoneValidDate = item["PhoneValidDate"] == null ? "-" : item["PhoneValidDate"].ToString(),
                            Status = Convert.ToInt32(item["Status"]),
                            RealVerifyDate = item["RealValidDate"] == null ? "-" : item["RealValidDate"].ToString(),
                            MemberCardNo = _MemberCardNo
                        });
                    }
                }
            }

            int pageIndex = vResult.PageIndex < 1 ? 1 : vResult.PageIndex;

            var model = new _VResult
            {
                _VParameter = new _VParameter(),
                PageIndex = pageIndex,
                Cards = result.ToPagedList(pageIndex, pagesize)
            };

            return View(model);
            //return View(result.ToPagedList(page, pagesize));

        }

        //顯示更多
        public ActionResult ViewDetail(int Id)
        {
            //Index();
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Login");
            }
            Member _m = _dbFM.Member.Where(m => m.Id == Id).FirstOrDefault();
            string _name = _m.Name == null ? "-" : _m.Name;
            string _rocid = _m.RocId == null ? "-" : _m.RocId;
            string _gender = _m.Gender == null ? "-" : _m.Gender;
            string _birthday = _m.Birthday == null ? "-" : Convert.ToDateTime(_m.Birthday).ToString("yyyy/MM/dd");
            string _email = _m.Email == null ? "-" : _m.Email;
            string _cellphone = _m.Cellphone == null ? "-" : _m.Cellphone;
            string _happyGoCardNo = _m.HgCardNo == null ? "-" : _m.HgCardNo;
            string _tHappyGoCardNo = _m.TempHgCardNo == null ? "-" : _m.TempHgCardNo;
            string _tMobileCarrier = _m.MobileCarrier == null ? "-" : _m.MobileCarrier;

            // 22.08.25 - Add Member Card No
            string _MemberCardNo = _FGMC.GenerateMemberNo(_m.Id.ToString());

            MemberEmailValid _mEV = _dbFM.MemberEmailValid.Where(o => o.MemberId == Id && o.Email == _m.Email && o.ValidDate != null).OrderByDescending(o => o.ValidDate).FirstOrDefault();
            string _validDate = "-";
            if (_mEV != null)
                _validDate = _mEV.ValidDate == null ? "-" : Convert.ToDateTime(_mEV.ValidDate).ToString("yyyy/MM/dd HH:mm:ss.fff");

            MemberCellphoneValid _mCV = _dbFM.MemberCellphoneValid.Where(o => o.MemberId == Id && o.Cellphone == _m.Cellphone && o.ValidDate != null).OrderByDescending(o => o.ValidDate).FirstOrDefault();
            string _phoneValidDate = "-";
            if (_mCV != null)
                _phoneValidDate = _mCV.ValidDate == null ? "-" : Convert.ToDateTime(_mCV.ValidDate).ToString("yyyy/MM/dd HH:mm:ss.fff");

            string _realVerifyDate = _m.RealValidDate == null ? "-" : Convert.ToDateTime(_m.RealValidDate).ToString("yyyy/MM/dd HH:mm:ss");
            string _hgValidDate = _m.HgValidDate == null ? "-" : Convert.ToDateTime(_m.HgValidDate).ToString("yyyy/MM/dd HH:mm:ss");

            _VResult._VMmember Member_Result = new _VResult._VMmember
            {
                ID = _m.Id,
                Name = _name,
                ROCId = _rocid,
                Birthday = _birthday,
                Gender = _gender,
                Email = _email,
                Phone = _cellphone,
                HappyGoCard = _happyGoCardNo,
                THappyGoCard = _tHappyGoCardNo,
                HgValidDate = _hgValidDate,
                EmailValidDate = _validDate,
                PhoneValidDate = _phoneValidDate,
                Status = _m.Status,
                RealVerifyDate = _realVerifyDate,
                MemberCardNo = _MemberCardNo,
                MobileCarrier = _tMobileCarrier
            };

            // Log Insert
            string _UserNo = _cookie.GetId();
            string _EventLog = string.Format(@"{0} - 檢視詳細個資：{1}", _UserNo, _m.Id);
            _Log.InsertSystemLog("Member - Detail", _EventLog, _UserNo);

            return View(Member_Result);
        }

        //編輯
        public ActionResult UpdateDetail(int Id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Login");
            }

            Member _m = _dbFM.Member.Where(m => m.Id == Id).FirstOrDefault();
            string _birthday = _m.Birthday == null ? "" : Convert.ToDateTime(_m.Birthday).ToString("yyyy/MM/dd");
            MemberEmailValid _mEV = _dbFM.MemberEmailValid.Where(o => o.MemberId == Id && o.Email == _m.Email && o.ValidDate != null).OrderByDescending(o => o.ValidDate).FirstOrDefault();
            string _validDate = "";
            if (_mEV != null)
                _validDate = _mEV.ValidDate == null ? "" : Convert.ToDateTime(_mEV.ValidDate).ToString("yyyy/MM/dd HH:mm:ss.fff");
            MemberCellphoneValid _mCV = _dbFM.MemberCellphoneValid.Where(o => o.MemberId == Id && o.Cellphone == _m.Cellphone && o.ValidDate != null).OrderByDescending(o => o.ValidDate).FirstOrDefault();
            string _phoneValidDate = "";
            if (_mCV != null)
                _phoneValidDate = _mCV.ValidDate == null ? "" : Convert.ToDateTime(_mCV.ValidDate).ToString("yyyy/MM/dd HH:mm:ss.fff");
            string realVerifyDate = _m.RealValidDate == null ? string.Empty : Convert.ToDateTime(_m.RealValidDate).ToString("yyyy/MM/dd HH:mm:ss");
            string hgValidDate = _m.HgValidDate == null ? string.Empty : Convert.ToDateTime(_m.HgValidDate).ToString("yyyy/MM/dd HH:mm:ss");

            // 23.09.14 - Find MbToken
            MemberToken _MemberToken = _dbFM.MemberToken.Where(m => m.EntityStatus == 1 && m.MemberId == Id).OrderByDescending(m => m.Id).FirstOrDefault();
            string _MbToken = _MemberToken.Token.ToString();

            // 22.08.25 - Add Member Card No
            string _MemberCardNo = _FGMC.GenerateMemberNo(_m.Id.ToString());

            string _tMobileCarrier = _m.MobileCarrier == null ? "" : _m.MobileCarrier;

            _VResult._VMmember Member_Result = new _VResult._VMmember
            {
                ID = _m.Id,
                Name = _m.Name,
                ROCId = _m.RocId,
                Birthday = _birthday,
                Gender = _m.Gender,
                Email = _m.Email,
                Phone = _m.Cellphone,
                HappyGoCard = _m.HgCardNo,
                THappyGoCard = _m.TempHgCardNo,
                HgValidDate = hgValidDate,
                EmailValidDate = _validDate,
                PhoneValidDate = _phoneValidDate,
                Status = _m.Status,
                RealVerifyDate = realVerifyDate,
                MemberCardNo = _MemberCardNo,
                MbToken = _MbToken,
                MobileCarrier = _m.MobileCarrier
            };

            var peopleList = new _Status[]{
                new _Status{name = "啟用", status = 1},
                new _Status{name = "停用", status = 0}
            };

            var genderList = new _GStatus[]{
                new _GStatus{name = "男", status = "male"},
                new _GStatus{name = "女", status = "female"}
            };

            SelectList list = new SelectList(peopleList, "status", "name");
            SelectList glist = new SelectList(genderList, "status", "name");
            ViewBag.statusList = list;
            ViewBag.genderList = glist;

            // Log Insert
            string _UserNo = _cookie.GetId();
            string _EventLog = string.Format(@"{0} - 調整個資：{1}", _UserNo, _m.Id);
            _Log.InsertSystemLog("Member - Detail", _EventLog, _UserNo);

            return View(Member_Result);
        }

        [HttpPost]
        public ActionResult UpdateDetail(int Id, _VResult._VMmember vMmembers)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Login");
            }

            try
            {
                //取得cookie中的使用者流水號value
                string _value = _cookie.GetId();

                if (ModelState.IsValid)
                {
                    DateTime? vaildDate = new DateTime();
                    DateTime? phoneVaildDate = new DateTime();

                    if (string.IsNullOrEmpty(vMmembers.EmailValidDate))
                        vaildDate = null;
                    else
                        vaildDate = Convert.ToDateTime(vMmembers.EmailValidDate);

                    if (string.IsNullOrEmpty(vMmembers.PhoneValidDate))
                        phoneVaildDate = null;
                    else
                        phoneVaildDate = Convert.ToDateTime(vMmembers.PhoneValidDate);

                    Member _m = _dbFM.Member.Where(m => m.Id == Id).FirstOrDefault();
                    MemberEmailValid _mEV = _dbFM.MemberEmailValid.Where(o => o.MemberId == Id && o.Email == _m.Email && o.ValidDate != null).OrderByDescending(o => o.ValidDate).FirstOrDefault();
                    MemberCellphoneValid _mCV = _dbFM.MemberCellphoneValid.Where(o => o.MemberId == Id && o.Cellphone == _m.Cellphone && o.ValidDate != null).OrderByDescending(o => o.ValidDate).FirstOrDefault();
                    //電話是否改
                    bool phone_change = false;
                    //信箱是否改
                    bool email_change = false;

                    //25.04.16只存欄位名 不存值
                    //紀LOG會員改之前的資料
                    JObject _DataJsonString = new JObject();
                    if (vMmembers.Name != _m.Name)
                        _DataJsonString["UpdateColumn"] += "Name, ";
                    if (vMmembers.ROCId != _m.RocId)
                        _DataJsonString["UpdateColumn"] += "RocId, ";
                    if (Convert.ToDateTime(vMmembers.Birthday) != _m.Birthday)
                        _DataJsonString["UpdateColumn"] += "Birthday, ";
                    if (vMmembers.Gender != _m.Gender)
                        _DataJsonString["UpdateColumn"] += "Gender, ";
                    if (vMmembers.HappyGoCard != _m.HgCardNo)
                        _DataJsonString["UpdateColumn"] += "HgCardNo, ";
                    if (vMmembers.Status != _m.Status)
                        _DataJsonString["UpdateColumn"] += "Status, ";
                    //_DataJsonString["Status"] = _m.Status;
                    if (vMmembers.MobileCarrier != _m.MobileCarrier)
                        _DataJsonString["UpdateColumn"] += "MobileCarrier, ";

                    //Email修改機制
                    if (vMmembers.Email != _m.Email)
                    {
                        _DataJsonString["UpdateColumn"] += "Email, ";
                        if (vMmembers.Email != null && vMmembers.Email != "")
                        {
                            //重複電話的Email
                            var _Memail = _dbFM.Member.Where(o => o.Email == vMmembers.Email).ToList();
                            if (_Memail.Count() > 0)
                            {
                                foreach (var e in _Memail)
                                {
                                    //紀LOG會員改之前的資料
                                    JObject _mDeleteEmail = new JObject();
                                    _mDeleteEmail["Email"] = e.Email;
                                    string deletePhoneMsg = JsonConvert.SerializeObject(_mDeleteEmail);
                                    _dbFM.MemberLog.Add(new MemberLog
                                    {
                                        ApId = 8,
                                        MemberId = e.Id,
                                        Date = DateTime.Now,
                                        DataJsonString = deletePhoneMsg
                                    });

                                    //清空會員Email
                                    e.Email = null;
                                    _dbFM.Entry(e).State = EntityState.Modified;
                                }
                                _dbFM.SaveChanges();
                            }
                            email_change = true;
                        }
                    }
                    //電話修改機制
                    if (vMmembers.Phone != _m.Cellphone)
                    {
                        _DataJsonString["UpdateColumn"] += "Cellphone, ";
                        if (vMmembers.Phone != null && vMmembers.Phone != "")
                        {
                            //重複電話的會員
                            var _Mphone = _dbFM.Member.Where(o => o.Cellphone == vMmembers.Phone).ToList();
                            if (_Mphone.Count() > 0)
                            {
                                foreach (var m in _Mphone)
                                {
                                    //紀LOG會員改之前的資料
                                    JObject _mDeletePhone = new JObject();
                                    _mDeletePhone["Cellphone"] = m.Cellphone;
                                    _mDeletePhone["Status"] = m.Status;
                                    string deletePhoneMsg = JsonConvert.SerializeObject(_mDeletePhone);
                                    _dbFM.MemberLog.Add(new MemberLog
                                    {
                                        ApId = 8,
                                        MemberId = m.Id,
                                        Date = DateTime.Now,
                                        DataJsonString = deletePhoneMsg
                                    });

                                    //清空會員電話&改狀態為0
                                    m.Cellphone = null;
                                    m.Status = 0;
                                    _dbFM.Entry(m).State = EntityState.Modified;
                                }
                                _dbFM.SaveChanges();
                            }
                            phone_change = true;
                        }
                    }

                    if (_mEV != null)
                    {
                        if (vaildDate != _mEV.ValidDate)
                            _DataJsonString["UpdateColumn"] += "EmailValidDate, ";
                    }
                    else
                    {
                        if (vaildDate != null)
                        {
                            email_change = true;
                            _DataJsonString["EmailValidDate"] = null;
                        }
                    }

                    if (phone_change == true)
                    {
                        if (_mCV != null)
                        {
                            if (phoneVaildDate != _mCV.ValidDate)
                                _DataJsonString["UpdateColumn"] += "PhoneValidDate, ";
                        }
                        else
                        {
                            if (phoneVaildDate != null)
                                _DataJsonString["PhoneValidDate"] = null;
                        }
                    }


                    string ResultMsg = JsonConvert.SerializeObject(_DataJsonString["UpdateColumn"]);
                    if (ResultMsg != "null")

                    {
                        _dbFM.MemberLog.Add(new MemberLog
                        {
                            ApId = 8,
                            MemberId = Id,
                            Event = "會員資料更改",
                            Date = DateTime.Now,
                            DataJsonString = ResultMsg
                        });
                    }

                    _m.Name = vMmembers.Name;
                    _m.RocId = vMmembers.ROCId;
                    if (string.IsNullOrEmpty(vMmembers.Birthday))   //沒填生日填null
                        _m.Birthday = null;
                    else
                        _m.Birthday = Convert.ToDateTime(vMmembers.Birthday);
                    _m.Gender = vMmembers.Gender;
                    _m.Email = vMmembers.Email;
                    _m.Cellphone = vMmembers.Phone;
                    _m.HgCardNo = vMmembers.HappyGoCard;
                    _m.MobileCarrier = vMmembers.MobileCarrier == null ? null : vMmembers.MobileCarrier.ToUpper();
                    if (vMmembers.Phone == null)
                        _m.Status = 0;
                    else
                        _m.Status = vMmembers.Status;

                    if (email_change == true)
                    {
                        //有信箱就改日期 ; 沒有就新增資料
                        MemberEmailValid _e = _dbFM.MemberEmailValid.Where(o => o.MemberId == Id && o.Email == vMmembers.Email).OrderByDescending(o => o.ValidDate).FirstOrDefault();
                        if (_e != null)
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(vaildDate)))
                                _e.ValidDate = vaildDate;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(vaildDate)))
                            {
                                _dbFM.MemberEmailValid.Add(new MemberEmailValid
                                {
                                    MemberId = Id,
                                    Email = vMmembers.Email,
                                    Code = "0000",
                                    ExpireDate = vaildDate.Value.AddSeconds(300),
                                    ValidDate = vaildDate
                                });
                            }
                        }
                    }
                    if (phone_change == true)
                    {
                        //有電話就改日期 ; 沒有就新增資料
                        MemberCellphoneValid _p = _dbFM.MemberCellphoneValid.Where(o => o.MemberId == Id && o.Cellphone == vMmembers.Phone).OrderByDescending(o => o.ValidDate).FirstOrDefault();
                        if (_p == null)
                        {
                            _dbFM.MemberCellphoneValid.Add(new MemberCellphoneValid
                            {
                                MemberId = Id,
                                Cellphone = vMmembers.Phone,
                                Code = "0000",
                                ExpireDate = phoneVaildDate.Value.AddSeconds(300),
                                ValidDate = phoneVaildDate
                            });
                        }
                    }
                    _dbFM.SaveChanges();

                    //更改後的驗證日期
                    MemberEmailValid _mEVupdate = _dbFM.MemberEmailValid.Where(o => o.MemberId == Id && o.Email == _m.Email && o.ValidDate != null).OrderByDescending(o => o.ValidDate).FirstOrDefault();
                    MemberCellphoneValid _mCVupdate = _dbFM.MemberCellphoneValid.Where(o => o.MemberId == Id && o.Cellphone == _m.Cellphone && o.ValidDate != null).OrderByDescending(o => o.ValidDate).FirstOrDefault();
                    DateTime? vaildDate_u = new DateTime();
                    if (_mEVupdate == null)
                        vaildDate_u = null;
                    else
                        vaildDate_u = _mEVupdate.ValidDate;
                    DateTime? phoneVaildDate_u = new DateTime();
                    if (_mCVupdate == null)
                        phoneVaildDate_u = null;
                    else
                        phoneVaildDate_u = _mCVupdate.ValidDate;
                    ////25.04.16不存UpdateMember_Log
                    //_dbLOG.UpdateMember_Log.Add(new UpdateMember_Log
                    //{
                    //    MemberId = _m.Id,
                    //    Name = _m.Name,
                    //    ROCId = _m.RocId,
                    //    Birthday = _m.Birthday,
                    //    Gender = _m.Gender,
                    //    Email = _m.Email,
                    //    Cellphone = _m.Cellphone,
                    //    HappyGoCardNo = _m.HgCardNo,
                    //    ValidDate = vaildDate_u,
                    //    PhoneValidDate = phoneVaildDate_u,
                    //    Status = _m.Status,
                    //    UserNo = _value,
                    //    ModifyTime = DateTime.Now,
                    //});
                    //_dbLOG.SaveChanges();

                    TempData["alert"] = "資料編輯完成！";

                    var result = new
                    {
                        IsSuccess = true
                    };

                    // Log Insert
                    string _UserNo = _cookie.GetId();
                    string _EventLog = string.Format(@"{0} - 調整個資成功：{1}{2}", _UserNo, _m.Id, ResultMsg == "null" ? "" : " - " + ResultMsg);
                    _Log.InsertSystemLog("Member - Detail", _EventLog, _UserNo);

                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(result), "application/json");
                }
                else
                {
                    var result = new
                    {
                        IsSuccess = false,
                        ModelStateErrors = ModelState.Where(o => o.Value.Errors.Count > 0)
                                                    .ToDictionary(k => k.Key, k => k.Value.Errors.Select(e => e.ErrorMessage).ToArray())
                    };

                    // Log Insert
                    string _UserNo = _cookie.GetId();
                    string _EventLog = string.Format(@"{0} - 調整個資失敗", _UserNo);
                    _Log.InsertSystemLog("Member - Detail", _EventLog, _UserNo);

                    return Content(Newtonsoft.Json.JsonConvert.SerializeObject(result), "application/json");
                }
            }
            catch (Exception ex)
            {
                _Log.InsertSyetemErrorLog(ex.ToString());
                var result = new
                {
                    IsSuccess = false
                };
                return Content(Newtonsoft.Json.JsonConvert.SerializeObject(result), "application/json");
            }
            //return View(Member_Result);
        }

        public ActionResult LogOut()
        {
            // Log Insert
            string _UserNo = _cookie.GetId();
            string _EventLog = string.Format(@"{0} - 登出系統", _UserNo);
            _Log.InsertSystemLog("Member - Login", _EventLog, _UserNo);

            Session.Abandon();
            return RedirectToAction("LogIn", "LogIn");
        }

        [HttpPost]
        public ActionResult HGValidation(int Id)
        {
            bool _IsSuccess = false;
            string _Message = "開卡失敗";
            Data _data = new Data();
            ValidationEmail _VE = new ValidationEmail();
            // Find Email & MbToken
            Member _Member = _dbFM.Member.Where(m => m.Id == Id).FirstOrDefault();
            try
            {
                MemberToken _MemberToken = _dbFM.MemberToken.Where(m => m.EntityStatus == 1 && m.MemberId == Id && m.ExpireDate >= DateTime.Now).OrderByDescending(m => m.Id).FirstOrDefault();
                
                var fieldsToCheck = new Dictionary<string, FieldCheck>
                {
                    { "Name", new FieldCheck { GetValue = m => m.Name, ErrorMessage = "需填寫姓名才能開卡" } },
                    { "RocId", new FieldCheck { GetValue = m => m.RocId, ErrorMessage = "需填寫身分證才能開卡" } },
                    { "Email", new FieldCheck { GetValue = m => m.Email, ErrorMessage = "需填寫Email才能開卡" } },
                    { "Birthday", new FieldCheck
                        {
                            GetValue = m => m.Birthday.HasValue ? m.Birthday.Value.ToString() : null,
                            ErrorMessage = "需填寫生日才能開卡"
                        }
                    }
                };
                //先檢查開卡要的資料有沒有缺少
                foreach (var field in fieldsToCheck)
                {
                    string value = field.Value.GetValue(_Member);
                    if (string.IsNullOrEmpty(value))
                    {
                        var result2 = new
                        {
                            IsSuccess = _IsSuccess,
                            Message = field.Value.ErrorMessage
                        };
                        // Log Insert
                        _Log.InsertSyetemErrorLog($"開通HG虛擬卡：{_Member.Id} - {result2}");
                        return Content(JsonConvert.SerializeObject(result2), "application/json");
                    }
                }

                bool CheckHgCardNo = false;
                _VE.Email = _Member.Email;
                string _MbToken = _MemberToken.Token.ToString();

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

                //// API Url
                string appapiURL = ConfigurationManager.AppSettings["appapiURL"];
                string _ApiUrl = $"{appapiURL}/happygocard/accreditation";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_ApiUrl);
                //request.Headers.Set("Host", "https://appapi.feds.com.tw");
                request.Headers.Add("X-Ap-Token", "E6FA93C1A76E492CB5EC9EFAD8243123");
                request.Headers.Add("X-Mb-Token", _MbToken);
                request.ContentType = "application/json";
                request.Method = "POST";

                string _jsonText = JsonConvert.SerializeObject(_VE);
                //byte[] _payload = Encoding.UTF8.GetBytes(_jsonText);
                //request.ContentLength = _payload.Length;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(_jsonText);
                }

                try
                {
                    using (HttpWebResponse httpResponse = (HttpWebResponse)request.GetResponse())
                    {
                        //第一次申請HappyGoCard
                        if ((int)httpResponse.StatusCode == 200)
                        {
                            CheckHgCardNo = true;
                        }
                    }
                }
                catch (WebException ex)
                {
                    // 這裡會捕捉到 HTTP 400 的錯誤
                    using (HttpWebResponse errorResponse = (HttpWebResponse)ex.Response)
                    {
                        using (StreamReader streamReader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            string errorText = streamReader.ReadToEnd();
                            if ((int)errorResponse.StatusCode == 400)
                            {
                                var hgError = JsonConvert.DeserializeObject<List<HGAPIError>>(errorText)[0];
                                if (hgError.Code == "CLTA120005" || hgError.Code == "CLTA020006")
                                {
                                    //CLTA120005：已重複綁定HappyGoCard
                                    //CLTA020006：重複頒發徽章
                                    CheckHgCardNo = true;
                                }
                                else
                                {
                                    //CLTA120004：綁定系統發生異常
                                    ////沒填姓名
                                    ////沒填身分證
                                    ////沒填生日
                                    _IsSuccess = false;
                                    _Message = "請確認相關個資與HG端是否不一致！";
                                    _Log.InsertSyetemErrorLog($"開通HG虛擬卡：{_Member.Id} - {errorText}");
                                }
                            }
                            else
                            {
                                ////停用帳號開HG卡會回500
                                _Log.InsertSyetemErrorLog($"開通HG虛擬卡：{_Member.Id} - API ({(int)errorResponse.StatusCode}) Respone：{errorText}");
                            }
                        }
                    }
                }
                if (CheckHgCardNo)
                {
                    _dbFM.Entry(_Member).Reload();
                    if (_Member.HgCardNo != null)
                    {
                        _IsSuccess = true;
                        _Message = "開卡成功";
                    }
                }
            }
            catch (Exception ex)
            {
                _Log.InsertSyetemErrorLog("開通HG虛擬卡：" + ex.ToString());
            }
            var result = new
            {
                IsSuccess = _IsSuccess,
                Message = _Message
            };
            // Log Insert
            string _UserNo = _cookie.GetId();
            string _EventLog = string.Format(@"{0} - 開通HG虛擬卡 ：{1}", _UserNo, _Member.Id);
            _Log.InsertSystemLog("Member - HGValidation", _EventLog, _UserNo);
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }
        public class FieldCheck
        {
            public Func<Member, string> GetValue { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}