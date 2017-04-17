using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSS.UniverseLogic
{
    static class SpecialConsts
    {
        static class ConstsRealization
        {
            public delegate int propFunc();

            public static propFunc fHunger = delegate { return 1; };
            public static int h1 = 1, h2 = 1;

            public static propFunc fAggression = delegate { return 1; };
            public static int a1 = 1, a2 = 1;

            public static propFunc fReproduction = delegate { return 1; };
            public static int r1 = 1, r2 = 1;

            public static propFunc fFriendly = delegate { return 1; };
            public static int f1 = 1, f2 = 1;

            public static propFunc fPoisonAddiction = delegate { return 1; };
            public static int p1 = 1, p2 = 1;

        }

        public static Dictionary<string, object> GetConstsAsDictionary()
        {
            Dictionary<string, object> res = new Dictionary<string, object>();
            string constName;

            constName = @"hunger_min";
            res.Add(constName, ConstsRealization.h1);
            constName = @"hunger_max";
            res.Add(constName, ConstsRealization.h2); 

            constName = @"aggression_min";
            res.Add(constName, ConstsRealization.a1);
            constName = @"aggression_max";
            res.Add(constName, ConstsRealization.a2);

            constName = @"friendly_min";
            res.Add(constName, ConstsRealization.f1);
            constName = @"friendly_max";
            res.Add(constName, ConstsRealization.f2);

            constName = @"reproduction_min";
            res.Add(constName, ConstsRealization.r1);
            constName = @"reproduction_max";
            res.Add(constName, ConstsRealization.r2);

            constName = @"poisonAddiction_min";
            res.Add(constName, ConstsRealization.p1);
            constName = @"poisonAddiction_max";
            res.Add(constName, ConstsRealization.p2);

            return res;
        }

        //Set const
        public static void SetHunger(int value)
        {
            ConstsRealization.fHunger = delegate { return value; };
            ConstsRealization.h1 = value;
        }

        public static void SetAggression(int value)
        {
            ConstsRealization.fAggression = delegate { return value; };
            ConstsRealization.a1 = value;
        }

        public static void SetReproduction(int value)
        {
            ConstsRealization.fReproduction = delegate { return value; };
            ConstsRealization.r1 = value;
        }

        public static void SetFriendly(int value)
        {
            ConstsRealization.fFriendly = delegate { return value; };
            ConstsRealization.f1 = value;
        }

        public static void SetPoisonAddiction(int value)
        {
            ConstsRealization.fPoisonAddiction = delegate { return value; };
            ConstsRealization.p1 = value;
        }
        //Set const

        //Set random
        public static void SetHunger(int minValue, int maxValue)
        {
            if (minValue == maxValue)
            {
                SetHunger(minValue);
                return;
            }
            ConstsRealization.fHunger = delegate { return StableRandom.rd.Next(minValue, maxValue); };
            ConstsRealization.h1 = minValue;
            ConstsRealization.h2 = maxValue;
        }

        public static void SetAggression(int minValue, int maxValue)
        {
            if (minValue == maxValue)
            {
                SetAggression(minValue);
                return;
            }
            ConstsRealization.fAggression = delegate { return StableRandom.rd.Next(minValue, maxValue); };
            ConstsRealization.a1 = minValue;
            ConstsRealization.a2 = maxValue;
        }

        public static void SetReproduction(int minValue, int maxValue)
        {
            if (minValue == maxValue)
            {
                SetReproduction(minValue);
                return;
            }
            ConstsRealization.fReproduction = delegate { return StableRandom.rd.Next(minValue, maxValue); };
            ConstsRealization.r1 = minValue;
            ConstsRealization.r2 = maxValue;
        }

        public static void SetFriendly(int minValue, int maxValue)
        {
            if (minValue == maxValue)
            {
                SetFriendly(minValue);
                return;
            }
            ConstsRealization.fFriendly = delegate { return StableRandom.rd.Next(minValue, maxValue); };
            ConstsRealization.f1 = minValue;
            ConstsRealization.f2 = maxValue;
        }

        public static void SetPoisonAddiction(int minValue, int maxValue)
        {
            if (minValue == maxValue)
            {
                SetPoisonAddiction(minValue);
                return;
            }
            ConstsRealization.fPoisonAddiction = delegate { return StableRandom.rd.Next(minValue, maxValue); };
            ConstsRealization.p1 = minValue;
            ConstsRealization.p2 = maxValue;
            
        }
        //Set random



        public static int GetHunger()
        {
            return ConstsRealization.fHunger();
        }

        public static int GetAggression()
        {
            return ConstsRealization.fAggression();
        }
        public static int GetReproduction()
        {
            return ConstsRealization.fReproduction();
        }
        public static int GetFriendly()
        {
            return ConstsRealization.fFriendly();
        }
        public static int GetPoisonAddiction()
        {
            return ConstsRealization.fPoisonAddiction();
        }
    }
}
