using Newtonsoft.Json;
using System;

namespace MMS.SupportedPlatforms.HitBtc.Model
{
    public class ExceptionInfo
    {
        [JsonProperty("exception")]
        public ExceptionDetail ExceptionDetail { get; set; }

        public override string ToString()
        {
            return ExceptionDetail != null ? $"[{DateTime.UtcNow.TimeOfDay}] HitBtc Exception:\n{ExceptionDetail.ToString()}" : $"[{DateTime.UtcNow.TimeOfDay}] HitBtc Exception:\nnull";
        }
    }

    public class ExceptionDetail
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("fullInfo")]
        public string FullInfo { get; set; }

        public override string ToString()
        {
            return $"message: {Message}\nfull info: {FullInfo}";
        }
    }
}
