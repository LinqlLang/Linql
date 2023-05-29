using Linql.Client;
using Linql.Core;
using Linql.Core.Test;
using Linql.Test.Files;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace Linql.Server.Test
{
    public class CompilerHook_Test
    {
        public IQueryable<DataModel> Data { get; set; }

        public LinqlCompiler Compiler { get; set; }

        public LinqlBeforeExecutionHook NoSelect { get; set; } = new LinqlBeforeExecutionHook((fun, input, inputType, method, args) =>
        {
            MemberInfo prop = typeof(DataModel).GetMember(nameof(DataModel.Decimal)).FirstOrDefault();

            if(fun.FunctionName == nameof(Queryable.Select))
            {
                LambdaExpression lam = args.Where(r => r is LambdaExpression).Cast<LambdaExpression>().FirstOrDefault();

                if(lam != null && lam.Body is MemberExpression member && member.Member == prop)
                {
                    throw new Exception($"Not allowed to select into property {nameof(DataModel.Decimal)} on type {nameof(DataModel)}");
                }
            }

            return Task.CompletedTask;
        });


        [OneTimeSetUp]
        public async Task Setup()
        {
            List<DataModel> dataList = new List<DataModel>();
            dataList.Where(r => true).ToList();
            foreach (int index in Enumerable.Range(1, 100))
            {
                DataModel data = new CompiledDataModel(index, true);
                dataList.Add(data);
            }

            Data = dataList.AsQueryable();

            HashSet<Assembly> assemblies = new HashSet<Assembly>()
            {
                typeof(Boolean).Assembly,
                typeof(Enumerable).Assembly,
                typeof(Queryable).Assembly
            };

            HashSet<Assembly> extensionCompiler = new HashSet<Assembly>(assemblies);
            extensionCompiler.Add(typeof(Application_Test).Assembly);

            this.Compiler = new LinqlCompiler(assemblies);


        }

        [Test]
        public void SelectNotAllowedDecimal()
        {
            LinqlSearch<DataModel> search = new LinqlSearch<DataModel>();
            IQueryable<decimal> decimalSearch = search.Select(r => r.Decimal);

            this.Compiler.AddHook(this.NoSelect);

            Assert.Catch(() =>
            {
                this.Compiler.Execute(decimalSearch.ToLinqlSearch(), this.Data);
            });

            this.Compiler.RemoveHook(this.NoSelect);

            Assert.DoesNotThrow(() =>
            {
                this.Compiler.Execute(decimalSearch.ToLinqlSearch(), this.Data);
            });
        }
    }
}