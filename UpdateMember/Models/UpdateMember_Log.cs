//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace UpdateMember.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UpdateMember_Log
    {
        public int Id { get; set; }
        public Nullable<int> MemberId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }
        public string Gender { get; set; }
        public string Cellphone { get; set; }
        public string HappyGoCardNo { get; set; }
        public string ROCId { get; set; }
        public Nullable<System.DateTime> ValidDate { get; set; }
        public Nullable<System.DateTime> PhoneValidDate { get; set; }
        public Nullable<int> Status { get; set; }
        public string UserNo { get; set; }
        public Nullable<System.DateTime> ModifyTime { get; set; }
    }
}
