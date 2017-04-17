using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSS.UniverseLogic
{
    static class StableRandom
    {
        public static Random rd = new Random();

    }
    public enum MoveDirection
    {
        stand,
        up,
        down,
        left,
        right
    }
    public struct DescAndMoveDir
    {
        public MoveDirection moveDir;
        public int desc;
    }
    public enum FoodType
    {
        poison,
        deadCell,
        defaultFood
    }
}
