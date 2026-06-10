using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AutoCEMI.Models
{
    public class UserProfile
    {
        [JsonPropertyName("playtime")]
        public TimeSpan PlayTime { get; set; } = TimeSpan.Zero;
    }
}
