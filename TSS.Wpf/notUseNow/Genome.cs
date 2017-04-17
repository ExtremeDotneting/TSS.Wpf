using System;
using System.Collections.Generic;

namespace TSS.UniverseLogic
{
    class Genome
	{
        public int Hunger;
        public int Aggression;
        public int Reproduction;
        public int Friendly;
        public int PoisonAddiction;
        ConstsUniverse constsUniverse;

        public ConstsUniverse ConstsUniverse
        {
            get { return constsUniverse; }
            private set { constsUniverse = value; }
        }

        public Genome(ConstsUniverse ctUn, int hunger, int aggression, int reproduction, int friendly, int poisonAddiction)
        {
            ConstsUniverse = ctUn;
            Hunger = hunger;
            Aggression = aggression;
            Reproduction = reproduction;
            Friendly = friendly;
            PoisonAddiction = poisonAddiction;
        }

        public Genome(ConstsUniverse ctUn)
        {
            ConstsUniverse = ctUn;
            Hunger = ConstsUniverse.CellGenome_Hunger;
            Aggression = ConstsUniverse.CellGenome_Aggression;
            Reproduction = ConstsUniverse.CellGenome_Reproduction;
            Friendly = ConstsUniverse.CellGenome_Friendly;
            PoisonAddiction = ConstsUniverse.CellGenome_PoisonAddiction;
        }

        public Genome Clone()
        {
            return new Genome(ConstsUniverse, Hunger, Aggression, Reproduction, Friendly, PoisonAddiction);
        }

        public Genome CloneAndMutate()
        {
            int modificator;
            int hunger = Hunger, aggression = Aggression, reproduction = Reproduction, friendly = Friendly, poisonAddiction = PoisonAddiction;

            for (int i = 0; i < ConstsUniverse.Mutation_ChangedValuesAtOne; i++)
            {
                if (StableRandom.rd.Next(2) == 0)
                    modificator = -1;
                else
                    modificator = 1;
                switch (StableRandom.rd.Next(1, 6))
                {
                    case 1:
                        hunger += modificator;
                        break;

                    case 2:
                        aggression += modificator;
                        break;

                    case 3:
                        reproduction += modificator;
                        break;

                    case 4:
                        friendly +=modificator;
                        break;

                    case 5:
                        poisonAddiction += modificator;
                        break;

                }
                
            }
            return new Genome(ConstsUniverse, hunger, aggression, reproduction, friendly, poisonAddiction);
           

        }



    }
}
