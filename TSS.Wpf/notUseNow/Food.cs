using System;
using System.Collections.Generic;
using System.Reflection;

namespace TSS.UniverseLogic
{
	class Food : UniverseObject
	{
        float energyLevel ;
		FoodType foodType;
        public float EnergyLevel
        {
            get { return energyLevel; }
        }

        public FoodType FoodType
        {
            get { return foodType; }
        }

        public Food(ConstsUniverse ctUn) : this(ctUn, FoodType.defaultFood) { }

        public Food(ConstsUniverse ctUn,FoodType foodType) : base(ctUn)
        {
            switch (foodType)
            {
                case FoodType.deadCell:
                    descriptor = -2;
                    energyLevel = ConstsUniverse.EnergyLevel_DeadCell;
                    break;

                case FoodType.defaultFood:
                    descriptor = -1;
                    energyLevel = ConstsUniverse.EnergyLevel_DefFood;
                    break;

                case FoodType.poison:
                    descriptor = -3;
                    energyLevel = ConstsUniverse.EnergyLevel_Poison;
                    break;

                default:
                    descriptor = -1;
                    energyLevel = 1;
                    break;
            }
            
            this.foodType = foodType;

        }

        public Food(ConstsUniverse ctUn, Dictionary<string, object> dictionary) : base(ctUn, dictionary)
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

        
	}
}
