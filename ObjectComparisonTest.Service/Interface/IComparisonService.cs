using ObjectComparisonTest.Service.ObjectResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectComparisonTest.Service.Interface
{
    public interface IComparisonService
    {
        ComparisonResponse GetChanges(object objectA, object objectB);
        bool CanDirectlyCompare(Type type);
        bool AreValuesEqual(object valueA, object valueB);
    }
}
