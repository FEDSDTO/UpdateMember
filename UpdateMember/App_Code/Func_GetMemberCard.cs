using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace UpdateMember.App_Code
{
    public class Func_GetMemberCard
    {
        /// <summary>
        /// 取得會員編號檢查碼(EAN10)
        /// </summary>
        /// <param name="Body">未編碼的會員Id</param>
        /// <returns>完整會員編號</returns>
        public string GenerateMemberNo(string Body)
        {
            //補足9碼 (10開頭)
            Body = string.Format(@"1{0}", Body.PadLeft(8, '0'));

            //位數
            int sum = 0;

            //字串轉陣列
            char[] _Char = Body.ToCharArray();

            //奇數位 加權 *1
            for (int i = 0; i < _Char.Length; i += 2)
            {
                sum += Convert.ToInt32(_Char[i].ToString()) * 1;
            }
            //偶數位 加權 *3
            for (int i = 1; i < _Char.Length; i += 2)
            {
                sum += Convert.ToInt32(_Char[i].ToString()) * 3;
            }

            //Sum長度
            int sumLength = sum.ToString().Count();

            // 10 - (Sum的個位數)
            string res = (10 - Convert.ToInt32(sum.ToString().Substring(sumLength - 1, 1))).ToString();

            //return 個位數
            return Body+res.Substring(res.Length - 1, 1);
        }

    }
}