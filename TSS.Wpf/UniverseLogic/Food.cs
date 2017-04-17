using System;

namespace TSS.UniverseLogic
{
    [Serializable]
    class Food : UniverseObject, IHasEnergy
    {
        public Food(ConstsUniverse constsUniverse,FoodType foodType):base(constsUniverse)
        {
            //I use delegate to always return value from ConstsUniverse, if it updated.
            if (foodType == FoodType.defaultFood)
            {
                descriptor = -1;
                GetEnergyLevelDelegate = delegate
                  {
                      return ConstsUniverseProperty.EnergyLevel_DefFood;
                  };
            }
            else if (foodType == FoodType.deadCell)
            {
                descriptor = -2;
                GetEnergyLevelDelegate = delegate
                  {
                      return ConstsUniverseProperty.EnergyLevel_DeadCell;
                  };
            }
            else if (foodType == FoodType.poison)
            {
                descriptor = -3;
                GetEnergyLevelDelegate = delegate
                  {
                      return ConstsUniverseProperty.EnergyLevel_PoisonedFood;
                  };
            }
        }
        delegate float GetFloatDelegate();
        GetFloatDelegate GetEnergyLevelDelegate;
        public float GetEnergyLevel()
		{
            return GetEnergyLevelDelegate();
		}
	}
}
