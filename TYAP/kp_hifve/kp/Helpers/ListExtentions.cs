using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWork.Helpers;
internal static class ListExtentions
{
    public static List<string> EachWithEach(this List<string> a, List<string> b)
    {
        if (a.Count == 0)
        {
            return b.ToList();
        }
        if (b.Count == 0)
        {
            return a.ToList();
        }
        var first = a.ToList();
        var second = b.ToList();
        var result = new List<string>();
        for (var i = 0; i < first.Count; i++)
        {
            for (var j = 0; j < second.Count; j++)
            {
                result.Add(first[i] + second[j]);
            }
        }
        return result;
    }
}
