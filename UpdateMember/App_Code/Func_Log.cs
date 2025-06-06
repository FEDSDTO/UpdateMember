using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UpdateMember.Models;

namespace UpdateMember.App_Code
{
    public class Func_Log
    {
        private GiftsEntities _Gifts = new GiftsEntities();
        private MemberCardEntities _MemberCard = new MemberCardEntities();

        /// <summary>
        /// 新增系統Log
        /// </summary>
        /// <param name="Controller">Controller</param>
        /// <param name="Event">事件</param>
        /// <param name="UserNo">工號</param>
        public void InsertSystemLog(string Controller, string Event, string UserNo)
        {
            try
            {
                // 將工號轉成Id
                UserInfo_Main _UM = _Gifts.UserInfo_Main.Where(m => m.UserNo == UserNo).Where(m => m.IsUse == true).FirstOrDefault();
                int _UId = _UM != null ? _UM.Id : -1;
                long _ULId = Convert.ToInt64(_UId);

                // Log Insert
                _MemberCard.System_Log.Add(new System_Log
                {
                    Controller = Controller,
                    Event = Event,
                    ModifyUser = _ULId,
                    ModifyDate = DateTime.Now
                });

                _MemberCard.SaveChanges();
            }
            catch(Exception ex)
            {
                InsertSyetemErrorLog(ex.ToString());
            }    
        }

        /// <summary>
        /// 新增系統Error Log
        /// </summary>
        /// <param name="ErrorMsg">Error Event</param>
        public void InsertSyetemErrorLog(string ErrorMsg)
        {
            // Log Insert
            _MemberCard.SystemError_Log.Add(new SystemError_Log
            {
                Controller = "Member",
                Event = ErrorMsg,
                ModifyUser = -1,
                ModifyDate = DateTime.Now
            });

            _MemberCard.SaveChanges();
        }
    }
}