using Domain.Enums;

namespace Domain.Models
{
    public class BaseResponseModel
    {
        public EnumStatusCodeReturn Code { get; set; }

        public string? Title { get; set; }

        /// <summary>
        /// Use for render to client on UI
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Use for developer read log exception
        /// </summary>
        public string? ErrorMessage { get; set; }

        public dynamic? Data { get; set; }

        public static BaseResponseModel ReturnData(dynamic? data = null, string message = "", string title = "")
        {
            var res = new BaseResponseModel
            {
                Code = EnumStatusCodeReturn.Success,
                Title = title,
                Message = message,
                Data = data ?? true
            };
            return res;
        }

        public static BaseResponseModel ReturnError(string message = "", string errorMessage = "", string title = "")
        {
            var res = new BaseResponseModel
            {
                Code = EnumStatusCodeReturn.Error,
                Title = title,
                Message = message,
                ErrorMessage = errorMessage,
                Data = null
            };
            return res;
        }
    }
}
