
namespace TinyRoar.Framework
{
    /*
     * Usage: RandomGenerator.Instance.Next(13, 37);
     * [max] int not included, e.g. max=3 gives int 0, 1 or 2
     */
    public class RandomGenerator : Singleton<RandomGenerator>, IRandomGenerator
    {
        
        // Properties
        private MersenneTwister _rGen
        {
            get;
            set;
        }

        // Construct
        public RandomGenerator()
        {
            _rGen = new MersenneTwister();
        }

        // between 0 and [int]
        public int Next()
        {
            return _rGen.Next();
        }

        // between 0 and [double]
        public double NextDouble()
        {
            return _rGen.NextDouble();
        }

        // between 0 and [float]
        public float NextFloat()
        {
            return (float)_rGen.NextDouble();
        }

        // [max] not included
        public int Next(int max)
        {
            return _rGen.Next(max);
        }

        public float NextFloat(float max)
        {
            return _rGen.NextFloat(max);
        }

        // [max] not included
        public int Next(int min, int max)
        {
            return _rGen.Next(min, max);
        }

        public float NextFloat(float min, float max)
        {
            return _rGen.NextFloat(min, max);
        }

        // Range method like known in UnityEngine
        public float Range(int min, int max)
        {
            return this.NextFloat(min, max);
        }

        // Generate a unique PlayerModel ID
        public string GeneratePlayerID()
        {
            string chars = "QWRTYPSDFGHJKLZXVCBNM23456789";
            char[] array = new char[29];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = chars[RandomGenerator.Instance.Next(chars.Length)];
            }
            array[5] = '-';
            array[11] = '-';
            array[17] = '-';
            array[23] = '-';
            return new string(array);
        }

        // Generate a unique PlayerModel ID
        public int GenerateRandomSeed()
        {
            var length = 9;
            var maxChar = 9;
            var value = 0;
            var multiple = 1;
            for (var i = 0; i < length; i++)
            {
                value += RandomGenerator.Instance.Next(maxChar) * multiple;
                multiple *= 10;
            }
            return value;
        }

    }
}
