using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Xml.Linq;
using System.IO;
using System.Collections.Generic;


namespace TSS.Helpers
{
    /// <summary>
    /// Well here it is clear, helping to serialize objects. The one I wrote myself - FieldValuesToXml. A reflection preserves the values of the fields, it is convenient for editing.
    /// Second serialized to Base64 string, it helps keep heavy objects as the Universe.
    /// <para></para>
    /// Ну тут ясно, помогает сериализировать объекты. Один я написал сам - FieldValuesToXml. Через рефлексию сохраняет значения полей, удобно для редактирования.
    /// Второй сериализирует в строку Base64, помогает сохранить тяжелые объекты, как Universe.
    /// </summary>
    static class SerializeHandler
    {
        public static string ToBase64String<T>(T obj)
        {
            using (var stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, obj);
                return Convert.ToBase64String(stream.GetBuffer(), 0, checked((int)stream.Length)); // Throw an exception on overflow.
            }
        }
        public static T FromBase64String<T>(string data)
        {
            return FromBase64String<T>(data, null);
        }
        public static T FromBase64String<T>(string data, BinaryFormatter formatter)
        {
            using (var stream = new MemoryStream(Convert.FromBase64String(data)))
            {
                formatter = (formatter ?? new BinaryFormatter());
                var obj = formatter.Deserialize(stream);
                if (obj is T)
                    return (T)obj;
                return default(T);
            }
        }
        public static XElement FieldValuesToXml(object objectWithValue)
        {
            XElement res = new XElement(@"ConstsUniverse");
            Type objType = objectWithValue.GetType();

            foreach (var fieldInfo in objType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
            {
                if (typeof(IFormattable).IsAssignableFrom(fieldInfo.FieldType) || typeof(bool).IsAssignableFrom(fieldInfo.FieldType))
                {
                    Type fieldType = fieldInfo.FieldType;
                    object fieldValue = fieldInfo.GetValue(objectWithValue);
                    NumericValuesAttribute atr = fieldInfo.GetCustomAttribute<NumericValuesAttribute>();

                    XElement xelLoc = new XElement(fieldInfo.Name);
                    xelLoc.Add(
                         Convert.ToString(fieldValue)
                        );
                    res.Add(xelLoc);
                }
            }
            return res;
        }
        public static void FieldValuesFromXml(object objectToBeInit, string xmlStr)
        {
            FieldValuesFromXml(objectToBeInit, XElement.Parse( xmlStr));
        }
        public static void FieldValuesFromXml(object objectToBeInit, XElement xel)
        {
            Type objType = objectToBeInit.GetType();
            Dictionary<string, object> convertedValuesDictionary = new Dictionary<string, object>();
            foreach (var fieldInfo in objType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
            {
                if (typeof(IFormattable).IsAssignableFrom(fieldInfo.FieldType) || typeof(bool).IsAssignableFrom(fieldInfo.FieldType))
                {
                    Type fieldType = fieldInfo.FieldType;
                    object fieldValueFromXml = xel.Element(fieldInfo.Name).Value;
                    object convertedFieldValue;
                    NumericValuesAttribute atr = fieldInfo.GetCustomAttribute<NumericValuesAttribute>();
                    if (fieldType.GetCustomAttribute<ParsebleAttribute>() != null)
                    {
                        convertedFieldValue = ParsebleAttribute.Parse(fieldValueFromXml.ToString(), fieldType);
                    }
                    else
                    {
                        convertedFieldValue = Convert.ChangeType(fieldValueFromXml, fieldType);
                    }
                    if (convertedFieldValue.GetType() != fieldType)
                    {
                        throw new Exception(string.Format("Wrong xml value for field {0}.", fieldInfo.Name));
                    }
                    convertedValuesDictionary.Add(fieldInfo.Name, convertedFieldValue);    
                }
            }
            foreach (var fieldInfo in objType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
            {
                if (typeof(IFormattable).IsAssignableFrom(fieldInfo.FieldType) || typeof(bool).IsAssignableFrom(fieldInfo.FieldType))
                {
                    fieldInfo.SetValue(objectToBeInit,convertedValuesDictionary[fieldInfo.Name]);
                }
            } 
        }
    }
}
