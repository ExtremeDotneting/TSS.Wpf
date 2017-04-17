using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TSS.WorkHelpers
{
    static class ParseHelper
    {
        public static List<XElement> DictionaryToXElementList(Dictionary<string, object> dictionary)
        {
            List<XElement> res = new List<XElement>();
            foreach ( KeyValuePair<string, object> pair in dictionary)
                res.Add(new XElement(pair.Key, pair.Value));
            return res;
        }

        public static Dictionary<string, object> XElementToDictionary(IEnumerable<XElement> xElementList)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            foreach (XElement xEl in xElementList)
                res.Add(xEl.Name.LocalName, xEl.Value);
            return res;
        }
    }
}
