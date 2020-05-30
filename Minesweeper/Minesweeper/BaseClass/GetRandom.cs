using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//7.3
namespace Minesweeper.BaseClass
{
    class GetRandom
    {
        public static Random globalRandomGenerator = GenerateNewRandomGenerator();
        public static Random GenerateNewRandomGenerator()
        {
            globalRandomGenerator =new Random((int)DateTime.Now.Ticks);
            return globalRandomGenerator;
        }
        public static int GetRandomInt(int max)
        {
            return globalRandomGenerator.Next(max);
        }
    }
}
