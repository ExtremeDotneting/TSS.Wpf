using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSS.UniverseLogic
{
    class ConstsUniverse : IConvertibleByFields
    {
        public int CellAge_Max = 99999;
        public int CellAge_AdultCell = 25;
        public float EnergyLevel_CreatingCell = 125;
        public float EnergyLevel_NeededForReproduction = 100;
        public float EnergyLevel_MaxForCell = 1000;
        public float EnergyLevel_DeadCell = 20;
        public float EnergyLevel_DefFood = 10;
        public float EnergyLevel_Poison = -10;
        public float EnergyLevel_MovesFriendly = (float)1.5;
        public float EnergyLevel_MovesAggression = 5;
        public bool Mutation_Enable = true;
        public int Mutation_ChangedValuesAtOne = 1;
        public int Mutation_ChancePercent = 50;
        public int CellsCount_MaxWithOneType = 9000;
        public int CellsCount_MaxAtField = 250;
        public float EnergyEntropyPerSecond = 1;
        public int CellGenome_Child_Aggression = -2;

        //Genome
        int CellGenome_hungerMin = 0;
        int CellGenome_hungerMax = 0;
        int CellGenome_aggressionMin = 0;
        int CellGenome_aggressionMax = 0;
        int CellGenome_reproductionMin = 0;
        int CellGenome_reproductionMax = 0;
        int CellGenome_friendlyMin = 0;
        int CellGenome_friendlyMax = 0;
        int CellGenome_poisonAddictionMin = 0;
        int CellGenome_poisonAddictionMax = 0;
        //Genome

        public int CellGenome_Hunger
        {
            get { return RandomFromRange(CellGenome_hungerMin, CellGenome_hungerMax); }
        }

        public int CellGenome_Aggression
        {
            get { return RandomFromRange(CellGenome_aggressionMin, CellGenome_aggressionMax); }
        }

        public int CellGenome_Reproduction
        {
            get { return RandomFromRange(CellGenome_reproductionMin, CellGenome_reproductionMax); }
        }

        public int CellGenome_Friendly
        {
            get { return RandomFromRange(CellGenome_friendlyMin, CellGenome_friendlyMax); }
        }

        public int CellGenome_PoisonAddiction
        {
            get { return RandomFromRange(CellGenome_poisonAddictionMin, CellGenome_poisonAddictionMax); }
        }

        int RandomFromRange(int minValue, int maxValue)
        {
            if (minValue >= maxValue)
                return minValue;
            else
                return StableRandom.rd.Next(minValue, maxValue);
        }

        public Dictionary<string, object> ToDictionary()
        {
            return CBFHandler.ToDictionaryDefault(this);
        }

    }


}
