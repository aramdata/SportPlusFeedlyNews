using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Rdio.Service;

namespace Rdio.ViewModel.News
{
    public class ServiceVM
    {
        public Models.ContentManager.Category Category { get; set; }
        public List<BlockNewsVM> BlockNews { get; set; }
        public List<Models.Legue.Varzesh3Legue> FootbalLegues { get; set; }
        public List<Models.Legue.Varzesh3LegueFixture> FootbalLeguesFixture { get; set; }

    }
}