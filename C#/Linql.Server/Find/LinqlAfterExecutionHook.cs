using Linql.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Linql.Server
{
    public class LinqlAfterExecutionHook : LinqlCompilerHook
    {
        public virtual Func<LinqlFunction, IEnumerable, Type, MethodInfo, List<object>, object, Task> Hook { get; set; } = null;


        public LinqlAfterExecutionHook() : base()
        {

        }

        public LinqlAfterExecutionHook(Func<LinqlFunction, IEnumerable, Type, MethodInfo, List<object>, object, Task> Hook) : base()
        {
            this.Hook = Hook;
        }

        public override bool IsValid
        {
            get
            {
                return this.Hook != null;
            }
        }
    }
}
