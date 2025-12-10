using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testcontainers.AutoSetup.Core.Abstractions;

namespace Testcontainers.AutoSetup.EntityFramework.Abstractions
{
    public interface IEfCoreMigrator<TContext> : IDbSeeder
    {
        
    }
}