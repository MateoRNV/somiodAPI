﻿using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace somiod.Models
{
    public class Application
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime creation_dt { get; set; }
    }
}