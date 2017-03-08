
namespace TinyRoar.Framework
{
    /// <summary>
    /// Usage: RandomGenerator.Instance.Next(13, 37);
    /// [max] int isn't included, e.g. max=3 gives int 0, 1 or 2
    /// </summary>
    public class RandomGenerator : Singleton<RandomGenerator>, IRandomGenerator
    {

        /// <summary>
        /// Property of the MersenneTwister
        /// </summary>
        private MersenneTwister _rGen
        {
            get;
            set;
        }

        /// <summary>
        /// Construct
        /// </summary>
        public RandomGenerator()
        {
            _rGen = new MersenneTwister();
        }

        /// <summary>
        /// between 0 and 2.147.483.647 (int-range)
        /// </summary>
        /// <returns>a random int</returns>
        public int Next()
        {
            return _rGen.Next();
        }

        /// <summary>
        /// get a double between 0 and [double]
        /// </summary>
        /// <returns>a random double</returns>
        public double NextDouble()
        {
            return _rGen.NextDouble();
        }

        /// <summary>
        /// get a float between 0 and [float]
        /// </summary>
        /// <returns>a random float</returns>
        public float NextFloat()
        {
            return (float)_rGen.NextDouble();
        }

        /// <summary>
        /// get a int between 0 and [max-1]. [max] not included!
        /// </summary>
        /// <param name="max"></param>
        /// <returns>a random int</returns>
        public int Next(int max)
        {
            return _rGen.Next(max);
        }

        /// <summary>
        /// get a float between 0 and [max-1]. [max] not included! 
        /// </summary>
        /// <param name="max"></param>
        /// <returns>a random float</returns>
        public float NextFloat(float max)
        {
            return _rGen.NextFloat(max);
        }

        /// <summary>
        /// get a int between min and max. [max] not included
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns>a random int</returns>
        public int Next(int min, int max)
        {
            return _rGen.Next(min, max);
        }

        /// <summary>
        /// get a float between min and max. [max] not included
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns>a random float</returns>
        public float NextFloat(float min, float max)
        {
            return _rGen.NextFloat(min, max);
        }

        /// <summary>
        /// Range method like known in UnityEngine
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns>a random float </returns>
        public float Range(int min, int max)
        {
            return this.NextFloat(min, max);
        }

        /// <summary>
        /// Generate a unique PlayerModel ID
        /// </summary>
        /// <returns></returns>
        public string GeneratePlayerID()
        {
            string chars = "QWRTYPSDFGHJKLZXVCBNM23456789";
            char[] array = new char[29];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = chars[this.Next(chars.Length)];
            }
            array[5] = '-';
            array[11] = '-';
            array[17] = '-';
            array[23] = '-';
            return new string(array);
        }

        /// <summary>
        /// Generate a unique Random Seed
        /// </summary>
        /// <returns></returns>
        public int GenerateRandomSeed()
        {
            var length = 9;
            var maxChar = 9;
            var value = 0;
            var multiple = 1;
            for (var i = 0; i < length; i++)
            {
                value += this.Next(maxChar) * multiple;
                multiple *= 10;
            }
            return value;
        }

    }
}
