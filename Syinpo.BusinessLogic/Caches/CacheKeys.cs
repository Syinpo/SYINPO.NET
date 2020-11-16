namespace Syinpo.BusinessLogic.Caches {
    public class CacheKeys {
        #region Device
        public static readonly string Device_Hash = "devices:hash";
        public static readonly string Device_Hash_Id = "device:id.{0}";
        public static readonly string Device_Hash_Uuid = "device:uuid:{0}";

        public static readonly string Device_WeiXin_Hash = "devices:weixin:hash";

        #endregion

        #region User
        public static readonly string User_Hash = "users:hash";
        public static readonly string User_Hash_Id = "users:id:{0}";
        public static readonly string User_Hash_Name = "users:name:{0}";
        #endregion

        #region Signalr
        public static readonly string Online_Hash = "onlines:hash";
        public static readonly string Online_Device_Hash = "onlines:devices";
        public static readonly string Online_User_Hash = "onlines:users";
        public static readonly string Online_Hash_Id = "online:id:{0}";

        public static readonly string OnlineReq_Hash = "onlines:reqs";
        public static readonly string OnlineReq_Hash_Id = "online:reqs:id:{0}";

        #endregion

    }
}
