using AutoCEMI.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AutoCEMI
{
    [JsonSerializable(typeof(GameProfile))]
    [JsonSerializable(typeof(UserProfile))]
    public partial class AppJsonContext : JsonSerializerContext
    {
    }
}
