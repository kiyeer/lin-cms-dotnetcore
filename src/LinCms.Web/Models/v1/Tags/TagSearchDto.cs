﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Zero.Data;

namespace LinCms.Web.Models.v1.Tags
{
    public class TagSearchDto:PageDto
    {
        public List<Guid> TagIds { get; set; }
        public string TagName { get; set; }

        public bool? Status { get; set; }
    }
}
