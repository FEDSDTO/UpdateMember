using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UpdateMember.Models;

namespace UpdateMember.ViewModel
{
    public class _VParameter
    {
        public int? id { get; set; }
        public string phoneNo { get; set; }
        public string userId { get; set; }
    }

    public class _VResult
    {
        public _VParameter _VParameter { get; set; }
        public IPagedList<_VMmember> Cards { get; set; }
        public int PageIndex { get; set; }
        public class _VMmember
        {
            public int ID { get; set; }

            public string MemberCardNo { get; set; }

            public string MbToken { get; set; }

            public string Name { get; set; }

            public string ROCId { get; set; }

            public string Birthday { get; set; }
            
            public string Gender { get; set; }

            public string Email { get; set; }

            //[Required(ErrorMessage = "*電話為必填")]
            public string Phone { get; set; }

            public string HappyGoCard { get; set; }

            public string THappyGoCard { get; set; }

            public string HgValidDate { get; set; }

            public string EmailValidDate { get; set; }

            public string PhoneValidDate { get; set; }

            public int Status { get; set; }

            public string RealVerifyDate { get; set; }
            public string MobileCarrier { get; set; }
        }
    }

    public class _Status
    {
        public string name{ get;set;}
        public int status { get; set; }
    }

    public class _GStatus
    {
        public string name { get; set; }
        public string status { get; set; }
    }
}