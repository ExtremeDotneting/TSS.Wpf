using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TSS.UniverseLogic;
using System.Xml;
using System.Xml.Serialization;



namespace TSS.WorkHelpers
{
    static class UniverseParser
    {

        public static bool SetDefConstsFromDictionary(IDictionary<string, object> parsedDictionary)
        {
            string constName = @"";
            try
            {
                constName = @"ChildrenAggression";
                UniverseConsts.ChildrenAggression = Convert.ToInt32(parsedDictionary[constName]);

                constName = @"MaxCellAge";
                UniverseConsts.MaxCellAge = Convert.ToInt32(parsedDictionary[constName]);

                constName = @"AdultCellAge";
                UniverseConsts.AdultCellAge = Convert.ToInt32(parsedDictionary[constName]);

                constName = @"DefCellEnergyLevel";
                UniverseConsts.DefCellEnergyLevel = Convert.ToSingle(parsedDictionary[constName]);

                constName = @"ReproductEnergyLevel";
                UniverseConsts.ReproductEnergyLevel = Convert.ToSingle(parsedDictionary[constName]);

                constName = @"MaxCellEnergyLevel";
                UniverseConsts.MaxCellEnergyLevel = Convert.ToSingle(parsedDictionary[constName]);

                constName = @"DeadCellsEnergyLevel";
                UniverseConsts.DeadCellsEnergyLevel = Convert.ToSingle(parsedDictionary[constName]);

                constName = @"FoodEnergyLevel";
                UniverseConsts.FoodEnergyLevel = Convert.ToSingle(parsedDictionary[constName]);

                constName = @"PoisonEnergyLevel";
                UniverseConsts.PoisonEnergyLevel = Convert.ToSingle(parsedDictionary[constName]);

                constName = @"EnableMutation";
                UniverseConsts.EnableMutation = Convert.ToBoolean(parsedDictionary[constName]);

                constName = @"MutationAtOne";
                UniverseConsts.MutationAtOne = Convert.ToInt32(parsedDictionary[constName]);

                constName = @"MovesFriendly";
                UniverseConsts.MovesFriendly = Convert.ToSingle(parsedDictionary[constName]);

                constName = @"MovesAggression";
                UniverseConsts.MovesAggression = Convert.ToSingle(parsedDictionary[constName]);

                constName = @"MaxCellsWithOneType";
                UniverseConsts.MaxCellsWithOneType = Convert.ToInt32(parsedDictionary[constName]);

                constName = @"DrawMoveDirections";
                UniverseConsts.DrawMoveDirections = Convert.ToBoolean(parsedDictionary[constName]);

                constName = @"EnergyEntropyPerSecond"; 
                UniverseConsts.EnergyEntropyPerSecond = Convert.ToSingle(parsedDictionary[constName]);

                constName = @"MutationChancePercent"; 
                UniverseConsts.MutationChancePercent = Convert.ToInt32(parsedDictionary[constName]);

                constName = @"DefGenerateCells";
                UniverseConsts.DefGenerateCells = (uint)Convert.ToInt32(parsedDictionary[constName]);

                constName = @"DefFoodForTick";
                UniverseConsts.DefFoodForTick = (uint)Convert.ToInt32(parsedDictionary[constName]);

                constName = @"DefFoodPlace";
                UniverseConsts.DefFoodPlace = Convert.ToString(parsedDictionary[constName]);

                constName = @"DefPoisonForTick";
                UniverseConsts.DefPoisonForTick = (uint)Convert.ToInt32(parsedDictionary[constName]);

                constName = @"DefPoisonPlace";
                UniverseConsts.DefPoisonPlace = Convert.ToString(parsedDictionary[constName]);

                constName = @"MaxCellCount";
                UniverseConsts.MaxCellCount = Convert.ToInt32(parsedDictionary[constName]);

                constName = @"MultiThreading";
                UniverseConsts.MultiThreading = Convert.ToBoolean(parsedDictionary[constName]);

                constName = @"TicksPerSecond";
                UniverseConsts.TicksPerSecond = Convert.ToInt32(parsedDictionary[constName]);

                constName = @"FramesPerSecond";
                UniverseConsts.FramesPerSecond = Convert.ToInt32(parsedDictionary[constName]);

                return true;

            }
            catch (Exception e)
            {

                throw new Exception(string.Format("Exception when work with \"{0}\"!\n{1}\n\nAll keys:\n{2}", constName, 
                    e.Message, DictionaryToString(parsedDictionary)));
                
            }
            
        }

        public static bool SetSpecialConstsFromDictionary(IDictionary<string, object> parsedDictionary)
        {
            string constName = @"";
            try
            {
                int min, max;

                constName = @"hunger_min";
                min = Convert.ToInt32(parsedDictionary[constName]);
                constName = @"hunger_max";
                max = Convert.ToInt32(parsedDictionary[constName]);
                SpecialConsts.SetHunger(min, max);

                constName = @"aggression_min";
                min = Convert.ToInt32(parsedDictionary[constName]);
                constName = @"aggression_max";
                max = Convert.ToInt32(parsedDictionary[constName]);
                SpecialConsts.SetAggression(min, max);

                constName = @"friendly_min";
                min = Convert.ToInt32(parsedDictionary[constName]);
                constName = @"friendly_max";
                max = Convert.ToInt32(parsedDictionary[constName]);
                SpecialConsts.SetFriendly(min, max);

                constName = @"reproduction_min";
                min = Convert.ToInt32(parsedDictionary[constName]);
                constName = @"reproduction_max";
                max = Convert.ToInt32(parsedDictionary[constName]);
                SpecialConsts.SetReproduction(min, max);

                constName = @"poisonAddiction_min";
                min = Convert.ToInt32(parsedDictionary[constName]);
                constName = @"poisonAddiction_max";
                max = Convert.ToInt32(parsedDictionary[constName]);
                SpecialConsts.SetPoisonAddiction(min, max);

                return true;

            }
            catch(Exception e)
            {
                throw new Exception(string.Format("Exception when work with \"{0}\"!\n{1}\n\nAll keys:\n{2}", constName,
                e.Message, DictionaryToString(parsedDictionary)));
                
            }

        }

        public static IDictionary<string, object> ConstsToDictionary()
        {
            Dictionary<string, object> dictionary = UniverseConsts.GetConstsAsDictionary();
            SpecialConsts.GetConstsAsDictionary().ToList().ForEach(x => dictionary.Add(x.Key, x.Value));
            return dictionary;
        }

        public static string DictionaryToString(IDictionary<string, object> dictionary)
        {
            var sb = new StringBuilder();
            foreach (KeyValuePair<string, object> pair in dictionary)
            { 
                sb.AppendFormat("{0} = {1};\n", pair.Key, pair.Value);
            }
            return sb.ToString();
        }

        public static IDictionary<string, object> StringToDictionary(string dictStr)
        {
            dictStr = dictStr.Replace(@" ", @"").Replace("\t", @"").Replace("\r", @"").Replace("\n", @"").Trim();
            int commentStart = 0;
            while (commentStart>=0)
            {
                commentStart = dictStr.IndexOf(@"/*");
                int commentEnd = dictStr.IndexOf(@"*/");
                if (commentStart >= 0 && commentEnd> commentStart)
                    dictStr=dictStr.Remove(commentStart, commentEnd-commentStart+2);

            }

            string[] dictArr = dictStr.Split(';');
            int len = dictArr.Length;
            if (dictArr.Last() == string.Empty)
                len--;
            IDictionary<string, object> dict = new Dictionary<string, object>();
            for (int i = 0; i < len; i++)
            {
                
                string[] words = dictArr[i].Split('=');
                if (words.Length != 2)
                    throw new Exception(string.Format("Exception at line \"{0}\"!", dictArr[i]));
                dict.Add(words[0].Trim(), words[1].Trim());
            }
            return dict;
        }


        public static void SaveToFile(string path = @"def")
        {
            if (path == @"def")
                path = Environment.CurrentDirectory + @"\universe_configs.txt";
            string[] configsArr = DictionaryToString(ConstsToDictionary()).Split('\n');

            File.WriteAllLines(path, configsArr);
        }

        public static void LoadFromFile(string path = @"def")
        {
            if (path == @"def")
                path = Environment.CurrentDirectory + @"\universe_configs.txt";
            IDictionary<string, object> parsedDict = StringToDictionary(File.ReadAllText(path) );
            SetDefConstsFromDictionary(parsedDict);
            SetSpecialConstsFromDictionary(parsedDict);
        }

        //public static void FromString(string consts)
        //{
        //    IDictionary<string, object> parsedDict = StringToDictionary(File.ReadAllText(path));
        //    SetDefConstsFromDictionary(parsedDict);
        //    SetSpecialConstsFromDictionary(parsedDict);
        //}

    }




    
}
