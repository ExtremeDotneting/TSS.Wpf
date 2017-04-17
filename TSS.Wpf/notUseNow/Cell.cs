using System;
using System.Collections.Generic;
using System.Reflection;

namespace TSS.UniverseLogic
{
    class Cell : UniverseObject
	{
		int age ;
        float energyLevel;
		//MoveDirection moveDirectionAspiration = MoveDirection.stand;
	    Genome genome;
        MoveDirection moveDisperation;

        public Cell(ConstsUniverse ctUn):base(ctUn)
        {
            Initialize(new Genome(ConstsUniverse), 1, ConstsUniverse.EnergyLevel_CreatingCell);
        }

        public Cell(ConstsUniverse ctUn, Genome genome):base(ctUn)
        {
            Initialize(genome, 1, ConstsUniverse.EnergyLevel_CreatingCell);
        }

        public Cell(ConstsUniverse ctUn, Genome genome, float energyLevel) : base(ctUn)
        {
            Initialize(genome, 1, energyLevel);
        }

        public Cell(ConstsUniverse ctUn, Genome genome, float energyLevel, int descriptor) : base(ctUn)
        {
            Initialize(genome, descriptor, energyLevel);
        }

        public Cell(ConstsUniverse ctUn, Dictionary<string, object> dictionary):base(ctUn)
        {
            Initialize(
                new Genome(
                    ConstsUniverse,
                    Convert.ToInt32(dictionary["hunger"]),
                    Convert.ToInt32(dictionary["aggression"]),
                    Convert.ToInt32(dictionary["reproduction"]),
                    Convert.ToInt32(dictionary["friendly"]),
                    Convert.ToInt32(dictionary["poisonAddiction"])
                    ),
                1000,1000);
            foreach (var atr in GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
            {
                if (atr == null)
                    continue;
                string name = atr.Name;

                if (typeof(IFormattable).IsAssignableFrom(atr.FieldType))
                    atr.SetValue(this, dictionary[name]);
            }
        }

        public override Dictionary<string, object> ToDictionary()
        {
            //Dictionary<string, object> dictionary = new Dictionary<string, object>();
            //dictionary.Add(@"ObjType", ToString());
            //foreach (var atr in this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public))
            //{
            //    object value;
            //    if (atr == null || (value = atr.GetValue(this)) == null)
            //        continue;
            //    string name = atr.Name;

            //    if (typeof(IFormattable).IsAssignableFrom(atr.FieldType))
            //        dictionary.Add(name, value);
            //}
            object[] ignoreList = new object[] { universe };
            Dictionary<string, object> dictionary= CBFHandler.ToDictionaryDefault(this, ignoreList);

            dictionary.Add(@"aggression", genome.Aggression);
            dictionary.Add(@"friendly", genome.Friendly);
            dictionary.Add(@"hunger", genome.Hunger);
            dictionary.Add(@"poisonAddiction", genome.PoisonAddiction);
            dictionary.Add(@"reproduction", genome.Reproduction);

            return dictionary;
        }

        void Initialize(Genome genome, int descriptor, float energyLevel) 
        {
            //this.disposed = false;
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
        }

        public MoveDirection MoveDisperation
        {
            get { return moveDisperation; }
        }

        public Genome Genome
        {
            get { return genome; }
        }

        public float EnergyLevel
		{
            get{ return energyLevel; }
		}

		public void AddEnergy(float value)
		{
            energyLevel += value;
            if (energyLevel > ConstsUniverse.EnergyLevel_MaxForCell)
                energyLevel = ConstsUniverse.EnergyLevel_MaxForCell;

        }

		public bool IncAge()
		{
            return age++ > ConstsUniverse.CellAge_Max;
		}

        public bool DecEnergy()
        {
            energyLevel -= ConstsUniverse.EnergyEntropyPerSecond;
            return energyLevel  < 0;
        }

        public int Age
        {
            get { return age; }
        }

        public Cell Reproduct()
        {
            age = 0;
            energyLevel = (int)(energyLevel / 2);
            if (StableRandom.rd.Next(100) < ConstsUniverse.Mutation_ChancePercent && ConstsUniverse.Mutation_Enable)
            {
                Cell res = new Cell(ConstsUniverse, genome.CloneAndMutate(), energyLevel);
                //res.AddEnergy(/*UniverseConsts.DefCellEnergyLevel * 8*/energyLevel);
                return res;
            }
            else
                return new Cell(ConstsUniverse,genome.Clone(), energyLevel, descriptor);
           
        }


        //genome
        //genome
        private bool IsAdult()
        {
            return Age >= ConstsUniverse.CellAge_AdultCell;
        }

        private float AnalizeDescriptor(int x, int y)
        {
            int desc = universe.GetObjectDescriptor(x, y);
            if (desc == 0)//empty
            {
                return 0;
            }
            else if (desc == descriptor)//friend
            {
                return genome.Friendly;
            }
            else if (desc < 0)//food
            {
                if (desc == -3)
                    return genome.PoisonAddiction;
                return genome.Hunger;
            }
            else//enemy
            {
                if (Age >= ConstsUniverse.CellAge_AdultCell)
                    return genome.Aggression;
                else
                    return ConstsUniverse.CellGenome_Child_Aggression;
            }
        }

        public MoveDirection CalcMoveDirectionAspiration()
        {
            float up, down, left, right;

            //up = AnalizeDescriptor(x - 1, y - 2) + 2*AnalizeDescriptor(x, y - 2) + AnalizeDescriptor(x + 1, y - 2) +
            //    2 * AnalizeDescriptor(x - 1, y - 1) + 3*AnalizeDescriptor(x, y - 1) + 2*AnalizeDescriptor(x + 1, y - 1);

            //down = AnalizeDescriptor(x - 1, y + 2) + 2*AnalizeDescriptor(x, y + 2) + AnalizeDescriptor(x + 1, y + 2) +
            //    2 * AnalizeDescriptor(x - 1, y + 1) + 3*AnalizeDescriptor(x, y + 1) + 2*AnalizeDescriptor(x + 1, y + 1);

            //left = AnalizeDescriptor(x - 2, y - 1) + 2*AnalizeDescriptor(x - 2, y) + AnalizeDescriptor(x - 2, y + 1) +
            //    2 * AnalizeDescriptor(x - 1, y - 1) + 3*AnalizeDescriptor(x - 1, y) + 2*AnalizeDescriptor(x - 1, y + 1);

            //right = AnalizeDescriptor(x + 2, y - 1) + 2*AnalizeDescriptor(x + 2, y) + AnalizeDescriptor(x + 2, y + 1) +
            //    2 * AnalizeDescriptor(x + 1, y - 1) + 3*AnalizeDescriptor(x + 1, y) + 2*AnalizeDescriptor(x + 1, y + 1);

            up = AnalizeDescriptor(x - 1, y - 2) + AnalizeDescriptor(x + 1, y - 2) + 3 * AnalizeDescriptor(x, y - 1)+
            2 * (AnalizeDescriptor(x, y - 2) + AnalizeDescriptor(x - 1, y - 1) + AnalizeDescriptor(x + 1, y - 1));

            down = AnalizeDescriptor(x - 1, y + 2) + AnalizeDescriptor(x + 1, y + 2) + 3 * AnalizeDescriptor(x, y + 1) +
                2*(AnalizeDescriptor(x - 1, y + 1) +  AnalizeDescriptor(x, y + 2)  +  AnalizeDescriptor(x + 1, y + 1));

            left = AnalizeDescriptor(x - 2, y - 1) + AnalizeDescriptor(x - 2, y + 1) + 3 * AnalizeDescriptor(x - 1, y) +
                2 * (AnalizeDescriptor(x - 1, y - 1) + AnalizeDescriptor(x - 2, y) + AnalizeDescriptor(x - 1, y + 1));

            right = AnalizeDescriptor(x + 2, y - 1) + 3 * AnalizeDescriptor(x + 1, y) + AnalizeDescriptor(x + 2, y + 1) +
                2 *( AnalizeDescriptor(x + 1, y - 1) +  AnalizeDescriptor(x + 2, y) +  AnalizeDescriptor(x + 1, y + 1));


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

                res= md[StableRandom.rd.Next(md.Count)];

            }
            moveDisperation = res;
            return res;


        }

        public bool CalcReproduction()
        {
            return ((EnergyLevel >= (ConstsUniverse.EnergyLevel_NeededForReproduction 
                -(genome.Reproduction * ConstsUniverse.EnergyLevel_NeededForReproduction / 10))) && IsAdult());
        }

    }
}
