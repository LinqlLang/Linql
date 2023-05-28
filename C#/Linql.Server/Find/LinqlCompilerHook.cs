using Linql.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Linql.Server
{
    public abstract class LinqlCompilerHook
    {
        public abstract bool IsValid { get; }
    }
}
