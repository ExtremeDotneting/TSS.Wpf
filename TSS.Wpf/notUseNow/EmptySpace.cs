using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace TSS.UniverseLogic
{
    class EmptySpace : UniverseObject
    {
        public EmptySpace(ConstsUniverse ctUn, Dictionary<string, object> dictionary) : base( ctUn, dictionary)
        {
            //foreach (var atr in GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
            //{
            //    if (atr == null)
            //        continue;
            //    string name = atr.Name;

            //    if (typeof(IFormattable).IsAssignableFrom(atr.FieldType))
            //        atr.SetValue(this, dictionary[name]);
            //}
        }
        public EmptySpace(ConstsUniverse ctUn) : base(ctUn) { }

    } 
   
}
