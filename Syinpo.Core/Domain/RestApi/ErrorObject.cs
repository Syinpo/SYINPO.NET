namespace Syinpo.Core.Domain.RestApi {
    public class ErrorObject
    {
        public int Code { get; set; } = 500;

        public string Message {
            get; set;
        }

    }

    public class ErrorObject2 {
        public int Code { get; set; } = 500;

        public string Message {
            get; set;
        }

        public object Data {
            get;set;
        }

    }
}
