﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rdio.ViewModel.ContentManager
{
    public class SimpleSiteVM : ServiceResult
    {
        public List<Models.ContentManager.Site> Data { get; set; }
    }
}