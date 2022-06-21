using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anitoa
{
    public class t_user
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int UID { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string Uname { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Pass { get; set; }

        /// <summary>
        /// 用户类型0用户1管理员
        /// </summary>
        public int Utype { get; set; }
       
    }
}
