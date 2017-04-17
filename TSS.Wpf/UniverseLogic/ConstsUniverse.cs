using System;
using TSS.Helpers;
using System.IO;
using System.Windows;
using TSS.Another;

namespace TSS.UniverseLogic
{
    /// <summary>
    /// The values used to calculate the processes in the universe (game). Their change - the basic essence of the gameplay.
    /// Attributes such as [NumericValues(1, 200)] used in ValuesRedactor giving him an idea of how you can edit this field.
    /// <para></para>
    /// Значения, используемые для расчета процессов во вселенной (игры). Их изменение - основная суть геймплея.
    /// Атрибуты типа[NumericValues(1, 200)] используются в ValuesRedactor давая ему представление о том как можно редактировать данное поле.
    /// </summary>
    [Serializable]
    class ConstsUniverse
    {
        static string DefaultConfigFile
        {
            get { return Environment.CurrentDirectory + "/def_configs.txt"; }
        }
        public ConstsUniverse()
        {
            try
            {
                //Попытка загрузить значения из файла.
                //Try load values from file.
                SerializeHandler.FieldValuesFromXml(this, File.ReadAllText(DefaultConfigFile));
            }
            catch
            {
                string sMessageBoxText =LanguageHandler.GetInstance().ConstsUniverseFileCorrupted;
                string sCaption = "";

                MessageBoxButton btnMessageBox = MessageBoxButton.OKCancel;
                MessageBoxImage icnMessageBox = MessageBoxImage.None;
                MessageBoxResult messageBoxRes = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                if (messageBoxRes == MessageBoxResult.OK)
                {
                    SaveToFile();
                }
                else
                {
                    Environment.Exit(0);
                }
                
            }
        }

        public void SaveToFile()
        {
            File.WriteAllLines(DefaultConfigFile, SerializeHandler.FieldValuesToXml(this).ToString().Split('\n'));
        }

        public int MaxCountOfCellTypes = 10;
        public bool Mutation_Enable = true;
        public bool Mutation_AttackChildrenMutantsOfFirstGeneration = false;
        public bool Mutation_AttackParentIfCellIsYouMutant = false;
        [NumericValues(1, 200)]
        public int Mutation_ChangedValuesAtOne = 1;
        [NumericValuesAttribute(0, 100, NumericValuesWayToShow.Slider)]
        public int Mutation_ChancePercent = 10;
        [NumericValues(int.MinValue, int.MaxValue)]
        public int CellAge_Max = 99999;
        [NumericValues(int.MinValue, int.MaxValue)]
        public int CellAge_AdultCell = 25;
        [NumericValues(float.MinValue, float.MaxValue)]
        public float EnergyLevel_CreatingCell = 125;
        [NumericValues(float.MinValue, float.MaxValue)]
        public float EnergyLevel_NeededForReproduction = 100;
        [NumericValues(float.MinValue, float.MaxValue)]
        public float EnergyLevel_MaxForCell = 1000;
        [NumericValues(float.MinValue, float.MaxValue)]
        public float EnergyLevel_DeadCell = 20;
        [NumericValues(float.MinValue, float.MaxValue)]
        public float EnergyLevel_DefFood = 10;
        [NumericValues(float.MinValue, float.MaxValue)]
        public float EnergyLevel_PoisonedFood = -10;
        [NumericValues(float.MinValue, float.MaxValue)]
        public float EnergyLevel_MovesFriendly = 1;
        [NumericValues(float.MinValue, float.MaxValue)]
        public float EnergyLevel_MovesAggression = 5;
        [NumericValues(int.MinValue, int.MaxValue)]
        public int CellsCount_MaxWithOneType = 9000;
        [NumericValues(int.MinValue, int.MaxValue)]
        public int CellGenome_Child_Aggression=-2;
        [NumericValues(int.MinValue, int.MaxValue)]
        public int CellsCount_MaxAtField = 250;
        [NumericValues(-99999, 99999)]
        public float EnergyEntropyPerSecond = 1;
        [NumericValues(0, 30000)]
        public int Special_FoodCountForTick = 2;
        [NumericValues(0, 30000)]
        public int Special_PoisonCountForTick = 2;
        

        //Genome
        MinMaxInt CellGenome_HungerRange = new MinMaxInt(-10,10);
        MinMaxInt CellGenome_AggressionRange = new MinMaxInt(-10, 10);
        MinMaxInt CellGenome_ReproductionRange = new MinMaxInt(-10, 10);
        MinMaxInt CellGenome_FriendlyRange = new MinMaxInt(-10, 10);
        MinMaxInt CellGenome_PoisonRange = new MinMaxInt(-10, 10);
        MinMaxInt CellGenome_CorpseRange = new MinMaxInt(-10, 10);
        //Genome

        public int CellGenome_Hunger
        {
            get { return RandomFromRange(CellGenome_HungerRange.Min, CellGenome_HungerRange.Max); }
        }
        public int CellGenome_Aggression
        {
            get { return RandomFromRange(CellGenome_AggressionRange.Min, CellGenome_AggressionRange.Max); }
        }
        public int CellGenome_Reproduction
        {
            get { return RandomFromRange(CellGenome_ReproductionRange.Min, CellGenome_ReproductionRange.Max); }
        }
        public int CellGenome_Friendly
        {
            get { return RandomFromRange(CellGenome_FriendlyRange.Min, CellGenome_FriendlyRange.Max); }
        }
        public int CellGenome_PoisonAddiction
        {
            get { return RandomFromRange(CellGenome_PoisonRange.Min, CellGenome_PoisonRange.Max); }
        }
        public int CellGenome_CorpseAddiction
        {
            get { return RandomFromRange(CellGenome_CorpseRange.Min, CellGenome_CorpseRange.Max); }
        }
        int RandomFromRange(int minValue, int maxValue)
        {
            if (minValue >= maxValue)
                return minValue;
            else
                return StableRandom.rd.Next(minValue, maxValue);
        }

        

    }

}
