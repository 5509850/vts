using Newtonsoft.Json;

namespace VTS.Core.CrossCutting.Extensions
{
    public static class JsonConvertorExtention
    {
        public static string ToJsonString(this object self)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(self);
        }

        public static T FromJsonString <T>(this string self)
        {
            return JsonConvert.DeserializeObject<T>(self);
        }
    }
}
