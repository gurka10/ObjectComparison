using ObjectComparisonTest.Service.ObjectParameters;
using ObjectComparisonTest.Service.ObjectResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ObjectComparisonTest.Models
{
    public class HomeModel
    {
        public SomeTypeParameters ComparisonA { get; set; }
        public SomeTypeParameters ComparisonB { get; set; }

        public SomeTypeParameters ComparisonC { get; set; }
        public SomeTypeParameters ComparisonD { get; set; }

        public ComparisonResponse ComparisonResponseForAB { get; set; }

        public ComparisonResponse ComparisonResponseForCD { get; set; }
    }
}