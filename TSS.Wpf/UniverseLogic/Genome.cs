using System;
using TSS.Another;

namespace TSS.UniverseLogic
{
    /// <summary>
    /// Storage of cell data.
    /// <para></para>
    /// Хранилище данных о клетке.
    /// </summary>
    [Serializable]
    class Genome
	{
        int hunger;
        int aggression;
        int reproduction;
        int friendly;
        int poisonAddiction;
        int corpseAddiction;

        public Genome(int hunger, int aggression, 
            int reproduction, int friendly, int poisonAddiction, int corpseAddiction)
        {
            this.hunger = hunger;
            this.aggression = aggression;
            this.reproduction = reproduction;
            this.friendly = friendly;
            this.poisonAddiction= poisonAddiction;
            this.corpseAddiction= corpseAddiction;
        }
        public Genome(ConstsUniverse constsUniverse) : 
            this( 
            constsUniverse.CellGenome_Hunger,
            constsUniverse.CellGenome_Aggression,
            constsUniverse.CellGenome_Reproduction,
            constsUniverse.CellGenome_Friendly,
            constsUniverse.CellGenome_PoisonAddiction,
            constsUniverse.CellGenome_CorpseAddiction
            )
        {
        }
        public int GetHunger()
        {
            return hunger;
        }
        public int GetAggression()
        {
            return aggression;
        }
        public int GetReproduction()
        {
            return reproduction;
        }
        public int GetFriendly()
        {
            return friendly;
        }
        public int GetPoisonAddiction()
        {
            return poisonAddiction;
        }
        public int GetCorpseAddiction()
        {
            return corpseAddiction;
        }

        /// <summary>
        /// This method just clone genome.
        /// <para></para>
        /// Этот метод просто клонирует геном.
        /// </summary>
        /// <returns></returns>
        public Genome Clone()
        {
            return new Genome(
                hunger,
                aggression,
                reproduction,
                friendly,
                poisonAddiction,
                corpseAddiction
                );
        }

        /// <summary>
        /// This method clone genome and change some values.
        /// <para></para>
        /// Этот метод клонирует геном, изменяя некоторые его значения.
        /// </summary>
        public Genome CloneAndMutate(int Mutation_ChangedValuesAtOne)
        {
            int modificator;
            int hunger = this.hunger, aggression = this.aggression, reproduction = this.reproduction, friendly = this.friendly;

            for (int i = 0; i < Mutation_ChangedValuesAtOne; i++)
            {
                if (StableRandom.rd.Next(2) == 0)
                    modificator = -1;
                else
                    modificator = 1;
                switch (StableRandom.rd.Next(1, 7))
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
                        friendly += modificator;
                        break;
                    case 5:
                        poisonAddiction += modificator;
                        break;
                    case 6:
                        corpseAddiction += modificator;
                        break;
                }
                
            }
            return new Genome(
                hunger,
                aggression,
                reproduction,
                friendly,
                poisonAddiction,
                corpseAddiction
                );
        }
    }
}
