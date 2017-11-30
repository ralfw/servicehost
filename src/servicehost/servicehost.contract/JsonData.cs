using System;

namespace servicehost.contract
{
    public struct JsonData {
        public JsonData(string data) => Data = data;
        
        public string Data { get; }
    }
}