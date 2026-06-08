using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AcemiEngine.Models
{
    internal class UserProfile
    {
        [JsonPropertyName("playtime")]
        public TimeSpan PlayTime = new();
    }
}
