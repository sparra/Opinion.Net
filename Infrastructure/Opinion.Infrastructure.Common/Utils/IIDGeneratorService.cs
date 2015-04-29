using System;
using System.Collections.Generic;
using System.Linq;

namespace Opinion.Infrastructure.Common.Utils
{
    public interface IIDGeneratorService
    {
        string GetNDigits(int N);
    }
}
