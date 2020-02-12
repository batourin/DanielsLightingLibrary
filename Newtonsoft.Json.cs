using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace Newtonsoft.Json.Linq
{
    public static class JTokenEx
    {
        public static object ToObject(this JToken jToken, Type objectType, JsonSerializer jsonSerializer)
        {
            using (JTokenReader jsonReader = new JTokenReader(jToken))
            {
                return jsonSerializer.Deserialize(jsonReader, objectType);
            }
        }
    }
}