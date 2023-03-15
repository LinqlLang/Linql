using Microsoft.AspNetCore.Mvc;
using Linql.Core;
using System.IO;
using System.Text.Json;
using Linql.Server;
using System.Collections.Concurrent;

namespace WebApiExample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StateController : ControllerBase
    {
        private readonly ILogger<StateController> _logger;

        protected DataService DataService { get; set; }

        protected LinqlCompiler Compiler { get; set; }

        public StateController(ILogger<StateController> logger, DataService DataService, LinqlCompiler Compiler)
        {
            _logger = logger;
            this.DataService = DataService;
            this.Compiler = Compiler;

        }

        [HttpPost]
        public object Linql(LinqlSearch Search)
        {
            object result = this.Compiler.Execute(Search, this.DataService.StateData.AsQueryable());
            return result;
        }

        [HttpPost("/Batch")]
        public List<object> Batch(List<LinqlSearch> Searches)
        {
            ConcurrentDictionary<long, object> results = new ConcurrentDictionary<long, object>();
            Parallel.ForEach(Searches, (search, options, index) =>
            {
                object result = this.Compiler.Execute(search, this.DataService.StateData.AsQueryable());
                results.GetOrAdd(index, (key) => result);
            });
           
            return results.OrderBy(r => r.Key).Select(r => r.Value).ToList();
        }
    }
}