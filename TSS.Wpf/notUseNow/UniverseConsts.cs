using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSS.UniverseLogic
{
    static class UniverseConsts
    {

        public static int ChildrenAggression = -2;
        public static int MaxCellAge = 99999;
        public static int AdultCellAge = 25;
        public static float DefCellEnergyLevel = 125;
        public static float ReproductEnergyLevel = 100;
        public static float MaxCellEnergyLevel = 1000;
        public static float DeadCellsEnergyLevel = 20;
        public static float FoodEnergyLevel = 10;
        public static float PoisonEnergyLevel = -FoodEnergyLevel;
        public static bool EnableMutation = true;
        public static int MutationAtOne = 1;
        public static float MovesFriendly = (float)1.5;
        public static float MovesAggression = 5;
        public static int MaxCellsWithOneType = 9000;
        public static bool DrawMoveDirections = false;
        public static float EnergyEntropyPerSecond = 1;
        public static int MutationChancePercent = 50;

        public static uint DefGenerateCells = 15;
        public static uint DefFoodForTick = 8;
        public static string DefFoodPlace = @"50, 0, 100, 27";
        public static uint DefPoisonForTick =1;
        public static string DefPoisonPlace =@"37, 0, 49, 30";
        public static int MaxCellCount=250;
        public static bool MultiThreading = false;
        public static int TicksPerSecond =5;
        public static int FramesPerSecond =10;



        public static Dictionary<string, object> GetConstsAsDictionary()
        {
            Dictionary<string, object> res = new Dictionary<string, object>();

            res.Add(@"ChildrenAggression", ChildrenAggression);
            res.Add(@"MaxCellAge", MaxCellAge);
            res.Add(@"AdultCellAge", AdultCellAge);
            res.Add(@"DefCellEnergyLevel", DefCellEnergyLevel);
            res.Add(@"ReproductEnergyLevel", ReproductEnergyLevel);
            res.Add(@"MaxCellEnergyLevel", MaxCellEnergyLevel);
            res.Add(@"DeadCellsEnergyLevel", DeadCellsEnergyLevel);
            res.Add(@"FoodEnergyLevel", FoodEnergyLevel);
            res.Add(@"PoisonEnergyLevel", PoisonEnergyLevel);
            res.Add(@"EnableMutation", EnableMutation);
            res.Add(@"MutationAtOne", MutationAtOne);
            res.Add(@"MovesFriendly", MovesFriendly);
            res.Add(@"MovesAggression", MovesAggression);
            res.Add(@"MaxCellsWithOneType", MaxCellsWithOneType);
            res.Add(@"DrawMoveDirections", DrawMoveDirections);
            res.Add(@"EnergyEntropyPerSecond", EnergyEntropyPerSecond);
            res.Add(@"MutationChancePercent", MutationChancePercent);

            res.Add(@"DefGenerateCells", DefGenerateCells);
            res.Add(@"DefFoodForTick", DefFoodForTick);
            res.Add(@"DefFoodPlace", DefFoodPlace);
            res.Add(@"DefPoisonForTick", DefPoisonForTick);
            res.Add(@"DefPoisonPlace", DefPoisonPlace);
            res.Add(@"MaxCellCount", MaxCellCount);
            res.Add(@"MultiThreading", MultiThreading);
            res.Add(@"TicksPerSecond", TicksPerSecond);
            res.Add(@"FramesPerSecond", FramesPerSecond);


            return res;

        }

        



    }


}
