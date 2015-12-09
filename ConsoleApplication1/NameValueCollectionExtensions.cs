using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public static class NameValueCollectionExtensions
    {
        /// <summary>
        /// Loops through the values of a NameValueCollection and adds them to an IDictionary.
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        public static IDictionary<string, string> ToDictionary(this NameValueCollection col)
        {
            IDictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var k in col.AllKeys)
            {
                dict.Add(k, col[k]);
            }
            return dict;
        }
    }
}
