namespace Syinpo.Model.Request.Users {
    public class UserChangPasswordParametersModel {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username {
            get;set;
        }

        ///<summary>
        /// 密码
        ///</summary>
        public string NewPassword {
            get; set;
        } // Password (length: 1000)
    }
}
