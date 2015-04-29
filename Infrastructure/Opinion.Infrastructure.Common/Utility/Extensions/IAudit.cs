using System;
using System.Collections.Generic;
using System.Linq;

namespace Opinion.Infrastructure.Common.Utility
{    
    public interface IAudit
    {
        string CreatedBy { get; set; }
        string UpdatedBy { get; set; }
        DateTime? CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
    }

    public interface IAudit<T> : IAudit { }   
}
