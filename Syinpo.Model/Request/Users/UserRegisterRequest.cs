using System.ComponentModel.DataAnnotations;

namespace Syinpo.Model.Request.Users {
    public class UserRegisterRequest {

        ///<summary>
        /// 合作伙伴主键
        ///</summary>
        public int PartnerId {
            get; set;
        } // PartnerId

        ///<summary>
        /// 父用户主键
        ///</summary>
        public int GroupUserId {
            get; set;
        } // GroupUserId

        ///<summary>
        /// 对外使用主键
        ///</summary>
        public System.Guid UserGuid {
            get; set;
        } // UserGuid

        ///<summary>
        /// 用户名
        ///</summary>
        public string Username {
            get; set;
        } // Username (length: 1000)

        ///<summary>
        /// 密码
        ///</summary>
        public string Password {
            get; set;
        } // Password (length: 1000)

        ///<summary>
        /// 显示名称
        ///</summary>
        public string DisplayName {
            get; set;
        } // DisplayName (length: 1000)

        ///<summary>
        /// 电子邮件
        ///</summary>
        public string Email {
            get; set;
        } // Email (length: 1000)

        ///<summary>
        /// 手机号码
        ///</summary>
        public string Mobile {
            get; set;
        } // Mobile (length: 1000)

        ///<summary>
        /// 安全得分
        ///</summary>
        public int SecurityScore {
            get; set;
        } // SecurityScore

        ///<summary>
        /// 批准
        ///</summary>
        public bool Approved {
            get; set;
        } // Approved



        ///<summary>
        /// 是否在线
        ///</summary>
        public bool IsOnline {
            get; set;
        } // IsOnline




        ///<summary>
        /// 最后登录设备
        ///</summary>
        public int? LastLoginDeviceId {
            get; set;
        } // LastLoginDeviceId
    }
}
