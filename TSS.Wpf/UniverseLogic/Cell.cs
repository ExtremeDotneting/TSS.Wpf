using System;
using System.Collections.Generic;
using TSS.Another;

namespace TSS.UniverseLogic
{
    /// <summary>
    /// Very important class. It is a alive organisms of our game.
    /// <para></para>
    /// ќчень важный класс. ѕредставл€ет из себ€ жывие организмы нашей игры.
    /// </summary>
    [Serializable]
    class Cell : UniverseObject, IHasEnergy
    {
		int age ;
        float energyLevel;
	    Genome genome;
        MoveDirection moveDisperation;
        int parentDescriptor;

        public Cell(ConstsUniverse constsUniverse) : this(constsUniverse, new Genome(constsUniverse), constsUniverse.EnergyLevel_CreatingCell, 1)
        {
           
        }
        public Cell(ConstsUniverse constsUniverse, Genome genome, float energyLevel, int descriptor) : base(constsUniverse)
        {
            this.genome = genome;
            age = 0;
            this.energyLevel = energyLevel;
            if (descriptor < 100)
            {
                int desc = StableRandom.rd.Next(100, int.MaxValue);
                this.descriptor = desc;
            }
            else
                this.descriptor = descriptor;
            parentDescriptor = StableRandom.rd.Next(100, int.MaxValue);
        }

        /// <summary>
        /// An important part of the system. Genome - a cell data storage (its behavioral factors). Cells with identical genomes have identical descriptors.
        /// <para></para>
        /// ¬ажна€ часть системы. √еном - хранилище данных о клетке(ее поведенческие факторы). ” клеток с одинаковым геномом одинаковые дескрипторы.
        /// </summary>
        public Genome GetGenome()
        {
            return genome;
        }
        public MoveDirection GetMoveDisperation()
        {
            return moveDisperation;
        }
        public float GetEnergyLevel()
		{
            return energyLevel;
		}
		public void AddEnergy(float value)
		{
            energyLevel += value;
            if (energyLevel > ConstsUniverseProperty.EnergyLevel_MaxForCell)
                energyLevel = ConstsUniverseProperty.EnergyLevel_MaxForCell;

        }
		public bool IncAge()
		{
            return age++ > ConstsUniverseProperty.CellAge_Max;
		}
        public bool DecEnergy()
        {
            energyLevel -= ConstsUniverseProperty.EnergyEntropyPerSecond;
            return energyLevel  < 0;
        }
        public int GetAge()
        {
            return age;
        }

        /// <summary>
        /// This method is used by universe to create a cells descendants.
        /// If you want to count the number of cells with a particular gene, and always know descriptor of cells parent, then use this method.
        /// <para></para>
        /// ћетод используетс€ вселенной дл€ создани€ потомков клеток.
        /// ≈сли вы хотите подсчитывать количество клеток с конкретным геномом, а также всегда узнавать дескриптор прародител€ клетки, то используйте этот метод.
        /// </summary>
        public Cell CreateChild(bool haveMutationChance)
        {
            age = 0;
            energyLevel = (int)(energyLevel / 2);
            Cell res;
            if (haveMutationChance && StableRandom.rd.Next(100) < ConstsUniverseProperty.Mutation_ChancePercent && ConstsUniverseProperty.Mutation_Enable)
            {
                res = new Cell(ConstsUniverseProperty, genome.CloneAndMutate(ConstsUniverseProperty.Mutation_ChangedValuesAtOne), GetEnergyLevel(), 1);
                res.parentDescriptor = descriptor;
            }
            else
            {
                res= new Cell(ConstsUniverseProperty, genome.Clone(), GetEnergyLevel(), descriptor);
                res.cellsWithThisDescriptorCount = cellsWithThisDescriptorCount;
                cellsWithThisDescriptorCount.Value++;
                res.parentDescriptor = parentDescriptor;
            }
            
            return res;

        }

        /// <summary>
        /// The method calculates the direction of cell movement only. Moving makes the universe itself.
        /// <para></para>
        /// ћетод подсчитывает направление движени€ клетки и только. ѕеремещение производит сама вселенна€.
        /// </summary>
        public void CalcMoveDirectionAspiration(Universe universe)
        {
            //map for navigation
            //     q2   b3  q4
            //
            //q0   b1   a1  b5   q6
            //
            //b0   a0   c   a3   b7
            //
            //q1   b2   a2  b6   q7
            //
            //     q3   b4  q5

            //it looks terribly, but it`s optimaized
            int a0 = universe.GetObjectDescriptor(x - 1, y),
                a1 = universe.GetObjectDescriptor(x, y - 1),
                a2 = universe.GetObjectDescriptor(x, y + 1),
                a3 = universe.GetObjectDescriptor(x + 1, y);
            //if(a0>100 && a1 > 100 && a2 > 100 && a3 > 100)
            //{
            //    moveDisperation = MoveDirection.stand;
            //    return;
            //}

            int b0 = universe.GetObjectDescriptor(x - 2, y),
                b1 = universe.GetObjectDescriptor(x - 1, y - 1),
                b2 = universe.GetObjectDescriptor(x - 1, y + 1),
                b3 = universe.GetObjectDescriptor(x, y - 2),
                b4 = universe.GetObjectDescriptor(x, y + 2),
                b5 = universe.GetObjectDescriptor(x + 1, y - 1),
                b6 = universe.GetObjectDescriptor(x + 1, y + 1),
                b7 = universe.GetObjectDescriptor(x + 2, y),
                q0 = universe.GetObjectDescriptor(x-2, y-1),
                q1 = universe.GetObjectDescriptor(x-2, y+1),
                q2 = universe.GetObjectDescriptor(x-1, y-2),
                q3 = universe.GetObjectDescriptor(x-1, y+2),
                q4 = universe.GetObjectDescriptor(x+1, y-2),
                q5 = universe.GetObjectDescriptor(x+1, y+2),
                q6 = universe.GetObjectDescriptor(x+2, y-1),
                q7 = universe.GetObjectDescriptor(x+2, y+1);

            float ad_a0 = AnalizeDescriptor(a0),
                ad_a1 = AnalizeDescriptor(a1),
                ad_a2 = AnalizeDescriptor(a2),
                ad_a3 = AnalizeDescriptor(a3),
                ad_b0 = AnalizeDescriptor(b1),
                ad_b1 = AnalizeDescriptor(b1),
                ad_b2 = AnalizeDescriptor(b2),
                ad_b3 = AnalizeDescriptor(b3),
                ad_b4 = AnalizeDescriptor(b4),
                ad_b5 = AnalizeDescriptor(b5),
                ad_b6 = AnalizeDescriptor(b6),
                ad_b7 = AnalizeDescriptor(b7),
                ad_q0 = AnalizeDescriptor(q0),
                ad_q1 = AnalizeDescriptor(q1),
                ad_q2 = AnalizeDescriptor(q2),
                ad_q3 = AnalizeDescriptor(q3),
                ad_q4 = AnalizeDescriptor(q4),
                ad_q5 = AnalizeDescriptor(q5),
                ad_q6 = AnalizeDescriptor(q6),
                ad_q7 = AnalizeDescriptor(q7);

            float up, down, left, right;
            up = ad_q2+ad_q4 +2*(ad_b1 +ad_b3+ ad_b5) + 3 * ad_a1;
            down = ad_q3 + ad_q5 + 2 * (ad_b2 + ad_b4+ad_b6) + 3 * ad_a2;
            left = ad_q0 + ad_q1 + 2 * (ad_b0 + ad_b1 + ad_b2) + 3 * ad_a0;
            right = ad_q6 + ad_q7 + 2 * (ad_b5 + ad_b6 + ad_b7) + 3 * ad_a3;


            float biggest = up;
            if (down > biggest)
                biggest = down;
            if (left > biggest)
                biggest = left;
            if (right > biggest)
                biggest = right;

            MoveDirection res = MoveDirection.stand;
            if (biggest >= 0)
            {
                List<MoveDirection> md = new List<MoveDirection>(0);
                if (biggest == up)
                    md.Add(MoveDirection.up);
                if (biggest == down)
                    md.Add(MoveDirection.down);
                if (biggest == left)
                    md.Add(MoveDirection.left);
                if (biggest == right)
                    md.Add(MoveDirection.right);

                res = md[StableRandom.rd.Next(md.Count)];
            }
            moveDisperation = res;
        }
        public bool CanReproduct()
        {
            return ((GetEnergyLevel() >= (ConstsUniverseProperty.EnergyLevel_NeededForReproduction - (genome.GetReproduction() * ConstsUniverseProperty.EnergyLevel_NeededForReproduction / 200))) && IsAdult());
        }

        /// <summary>
        /// Among other things, the method reduces 1 from the number of cells with that type.
        /// <para></para>
        ///  роме прочего, метод уменьшает количество клеток с этим типом на 1.
        /// </summary>
        public override void Dispose()
        {
            if (!IsDisposed())
            {
                cellsWithThisDescriptorCount.Value--;
                //cellsWithThisDescriptorCount = null;
            }
            base.Dispose();
            genome = null;
        }
        public int GetCellsCountWithThisDescriptor()
        {
            return cellsWithThisDescriptorCount.Value;
        }
        public int GetParentDescriptor()
        {
            return parentDescriptor;
        }
        bool IsAdult()
        {
            return GetAge() >= ConstsUniverseProperty.CellAge_AdultCell;
        }

        /// <summary>
        /// Returns commitment of cell to move to an object with a handle.
        /// <para></para>
        /// ¬озвращает стремление клетки двигатс€ к объекту с таким дескриптором.
        /// </summary>
        float AnalizeDescriptor(int desc)
        {
            if (desc == 0)//empty
            {
                return 0;
            }
            else if (desc == descriptor)//friend
            {
                return genome.GetFriendly();
            }
            else if (desc < 0)//food
            {
                if (desc == -3)
                    return genome.GetPoisonAddiction();
                if (desc == -2)
                    return genome.GetCorpseAddiction();
                return genome.GetHunger();
            }
            else//enemy
            {
                if (IsAdult())
                    return genome.GetAggression();
                else
                    return ConstsUniverseProperty.CellGenome_Child_Aggression;
            }



        }

        /// <summary>
        /// This local type and a field used to count the number of cells with such a descriptor.
        /// Its essence is that all of the cells with the descriptor has a pointer to a memory area.
        /// So, as each cell creating / deleting change this number, we do not need exhaustive search, to count the number of cells with the genome.
        /// This makes the algorithm fast but unreliable, alas.
        /// <para></para>
        /// Ётот локальный тип и поле используютс€ дл€ подсчета количества клеток с таким дескриптором.
        /// ≈го суть в том, что все клетки с таким дескриптором имеют указатель на одну область пам€ти.
        /// “ак-как кажда€ клетка при создании/удалении мен€ет это число, мы не нуждаемс€ в полном переборе, дл€ подсчета количества клеток с таким геномом.
        /// Ёто делает алгоритм быстрым, но ненадежным, увы.
        /// </summary>
        [Serializable]
        class LinkedInt
        {
            public int Value = 1;
        }
        LinkedInt cellsWithThisDescriptorCount = new LinkedInt();
    }
}
