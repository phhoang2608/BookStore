using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.Utility
{
    public static class SessionExtension
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            //pass the key and value this will SET the session.
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        public static T GetObject<T>(this ISession session, string key)
        {
            //pass the key this will GET the session string based on the key.
            var value = session.GetString(key);
            //check if value is null, the return a default implementation of the generic object T or we will cnonvert 
            //it back using Json Convert to type T and get it.
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
