using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GetMyItchIoRanking
{
    class ItchResponse
    {
        [JsonPropertyName("num_items")]
        public int Num_items { get; set; }
        [JsonPropertyName("content")]
        public string Content { get; set; }
        [JsonPropertyName("page")]
        public int Page { get; set; }
    }
}
