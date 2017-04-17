using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml.Linq;

namespace TSS.UniverseLogic
{
    static class CBFHandler
    {
        public static void InitFromDictionaryDef(object obj, Dictionary<string, object> dictionary, IEnumerable<FieldInfo> ignoreList = null)//Init only IFormattable fields.
        {
            List<FieldInfo> ignored = new List<FieldInfo>();
            ignored.AddRange(ignoreList);
            foreach (FieldInfo atr in obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
            {
                if (atr == null || ignored.IndexOf(atr) >= 0)
                    continue;
                object dictValue = dictionary[atr.Name];
                if (typeof(IFormattable).IsAssignableFrom(atr.FieldType))
                {
                    atr.SetValue(
                        obj,
                        Convert.ChangeType(dictValue, atr.FieldType)
                        );
                }
            }
        }

        public static Dictionary<string, object> ToDictionaryDefault(object obj, IEnumerable<object> ignoreList = null)
        {
            List<object> addedObjects = new List<object>();
            if(ignoreList!=null)
                addedObjects.AddRange(ignoreList);
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add(@"ObjType", obj.ToString());
            foreach (var atr in obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
            {
                object value;
                if (atr == null || (value = atr.GetValue(obj)) == null || addedObjects.IndexOf(value)>=0)
                    continue;
                string name = atr.Name;

                if (typeof(IFormattable).IsAssignableFrom(atr.FieldType))
                {
                    dictionary.Add(name, value);
                }
                else if (typeof(IConvertibleByFields).IsAssignableFrom(atr.FieldType))
                {
                    addedObjects.Add(value);
                    dictionary.Add(name, value as IConvertibleByFields);
                }
                else if (typeof(IEnumerable<IFormattable>).IsAssignableFrom(atr.FieldType))
                {
                    dictionary.Add(name, value);
                }
                else if (typeof(IEnumerable<IConvertibleByFields>).IsAssignableFrom(atr.FieldType))
                {
                    addedObjects.Add(value);
                    List<Dictionary<string, object>> dictList = new List<Dictionary<string, object>>();
                    IEnumerable<IConvertibleByFields> objectsEnumerable = value as IEnumerable<IConvertibleByFields>;
                    foreach (IConvertibleByFields item in objectsEnumerable)
                    {
                        dictList.Add(item.ToDictionary());
                        
                    }
                    dictionary.Add(name, dictList);
                }
            }
            return dictionary;
        }

        public static string DictionaryToDefString(Dictionary<string, object> dictionary, string prefix=@"")
        {
            prefix += @".";
            string res = @"";
            foreach (KeyValuePair<string, object> item in dictionary)
            {
                if (item.Value is Dictionary<string, object>)
                    res += DictionaryToDefString(item.Value as Dictionary<string, object>, string.Format(@"{0}{1}", prefix, item.Key));
                else if (item.Value is IEnumerable<Dictionary<string, object>>)
                {
                    int index = 0;
                    IEnumerable<Dictionary<string, object>> dictNumerable = item.Value as IEnumerable<Dictionary<string, object>>;
                    foreach (Dictionary<string, object> subDict in dictNumerable)
                    {
                        res += DictionaryToDefString(subDict, string.Format(@"{0}{1}[{2}]", prefix, item.Key, index));
                        index++;
                    }
                }
                else
                    res += string.Format("{0}{1} = {2};\n", prefix, item.Key, item.Value);
            }
            return res;
        }

        public static string DictionaryToXmlString(Dictionary<string, object> dictionary, string xmlNodeName, string indent = @"")
        {
            string indentDef = @"   ";
            xmlNodeName = xmlNodeName.Trim();
            string res = indent + @"<" + xmlNodeName + ">\n";
            string finBlock = indent + @"</" + xmlNodeName + ">\n";
            indent += indentDef;
            foreach (KeyValuePair<string, object> item in dictionary)
            {
                if (item.Value is Dictionary<string, object>)
                    res += DictionaryToXmlString(item.Value as Dictionary<string, object>, item.Key, indent + indentDef);
                else if (item.Value is IEnumerable<Dictionary<string, object>>)
                {
                    int index = 0;
                    IEnumerable<Dictionary<string, object>> dictNumerable = item.Value as IEnumerable<Dictionary<string, object>>;
                    res += indent + @"<" + item.Key + ">\n";
                    res += indent + indentDef + "<ObjType>IEnumerableDictionary</ObjType>\n";
                    foreach (Dictionary<string, object> subDict in dictNumerable)
                    {
                        res += DictionaryToXmlString(subDict, string.Format(@"item{0}", index), indent + indentDef);
                        index++;
                    }
                    res += indent + @"</" + item.Key + ">\n";
                }
                else
                    res += string.Format(indent + "<{0}>{1}</{0}>\n", item.Key, item.Value);
            }
            res += finBlock;
            return res;
        }

        public static object XElementToDictOrIFormattable(XElement xel)
        {
            //
            //XElement xel = XElement.Parse(xml);
            XElement xelType = xel.Element("ObjType");
            string typeStr = @"none";
            if (xelType != null)
                typeStr = xelType.Value;

            object res=null;

            if (typeStr == @"none")
            {
                res = xel.Value;
            }
            else if (typeStr == @"IEnumerableDictionary")
            {
                int index = 0;
                List < Dictionary < string, object>> dictList = new List<Dictionary<string, object>>();
                XElement item = xel.Element(string.Format(@"item{0}", index++));
                while (item != null)
                {
                    Dictionary<string, object> dict = new Dictionary<string, object>();
                    var elements = item.Elements();
                    foreach (XElement xelLocal in elements)
                    {
                        dict.Add(xelLocal.Name.ToString(), XElementToDictOrIFormattable(xelLocal));
                    }
                    dictList.Add(dict);

                    item = xel.Element(string.Format(@"item{0}", index++));
                }

                res = dictList;
                //This algorithm create list of dictionaries;
            }
            else
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                var elements = xel.Elements();
                foreach (XElement xelLocal in elements)
                {
                    dict.Add(xelLocal.Name.ToString(), XElementToDictOrIFormattable(xelLocal));
                }
                res = dict;
            }

            return res;
        }
    }

}
