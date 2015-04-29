using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Opinion.Infrastructure.Common.Utils
{
    public class IDGeneratorService : IIDGeneratorService
    {
        /*private readonly RandomNumberGenerator rndGenerator;
        public IDGeneratorService(RandomNumberGenerator RndGenerator)
        {
            this.rndGenerator = RndGenerator;
        }*/

        public string GetNDigits(int N)
        {
            var bytes = new byte[4];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            //uint random = BitConverter.ToUInt32(bytes, 0) % 1000000000;
            uint range = Convert.ToUInt32(Math.Pow(10, (double)N));
            uint random = BitConverter.ToUInt32(bytes, 0) % Convert.ToUInt32(Math.Pow(10, (double)N));
            //var targetFormat = string.Format("{0:D{0}}", N);
            return String.Format("{0}:D{1}", random, N);
        }
    }
}

