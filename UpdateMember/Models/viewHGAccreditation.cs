using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpdateMember.Models
{
    public class viewHGAccreditation
    {
        public class Data
        {
            public string LoyaltyId { get; set; }
            public string Name { get; set; }
            public string ROCId { get; set; }
            public string Birthday { get; set; }
            public string Cellphone { get; set; }
            public string Email { get; set; }
            public string City { get; set; }
            public string Area { get; set; }
            public string Road { get; set; }
            public string Addres { get; set; }
            public string StatusCode { get; set; }
            public bool Status { get; set; }
        }

        public class ValidationEmail
        {
            public string Email { get; set; }
        }

        public class HGAPIError
        {
            public string Code { get; set; }
            public string Message { get; set; }
        }
    }
}