using Linql.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Linql.Server
{
    public class LinqlBeforeExecutionHook : LinqlCompilerHook
    {
        public virtual Func<LinqlFunction, IEnumerable, Type, MethodInfo, List<object>, Task> Hook { get; set; } = null;

        public LinqlBeforeExecutionHook() : base()
        {

        }

        public LinqlBeforeExecutionHook(Func<LinqlFunction, IEnumerable, Type, MethodInfo, List<object>, Task> Hook) : base()
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
