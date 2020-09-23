﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TPMHelper.AfostoHelper.ProductModel
{
    public class Descriptors
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("short_description")]
        public string Short_Description { get; set; }
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("seo")]
        public Seo Seo { get; set; }
    }
}
