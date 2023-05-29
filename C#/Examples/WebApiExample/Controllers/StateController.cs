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
        public async Task<object> Linql(LinqlSearch Search)
        {
            object result = await this.Compiler.ExecuteAsync(Search, this.DataService.StateData.AsQueryable());
            return result;
        }

        [HttpPost("/Batch")]
        public async Task<List<object>> Batch(List<LinqlSearch> Searches)
        {
            List<Task<object>> tasks = Searches.Select(r =>
            {
                LinqlCompiler compiler = new CustomLinqlCompiler();
                Task<object> result = compiler.ExecuteAsync(r, this.DataService.StateData.AsQueryable());
                return result;
            }).ToList();

            var taskResults = await Task.WhenAll(tasks);
            List<object> results = taskResults.ToList();

            return results;
        }

        //[HttpPost]
        //public async Task<object> Linql(LinqlSearch Search)
        //{
        //    object result = await this.Compiler.ExecuteAsync(Search, this.DataService.StateData.AsQueryable());
        //    return result;
        //}

        //[HttpPost("/Batch")]
        //public List<object> Batch(List<LinqlSearch> Searches)
        //{
        //    ConcurrentDictionary<long, object> results = new ConcurrentDictionary<long, object>();
        //    Parallel.ForEach(Searches, async (search, options, index) =>
        //    {
        //        LinqlCompiler compiler = new CustomLinqlCompiler();
        //        object result = await compiler.ExecuteAsync(search, this.DataService.StateData.AsQueryable());
        //        results.GetOrAdd(index, (key) => result);
        //    });

        //    return results.OrderBy(r => r.Key).Select(r => r.Value).ToList();
        //}
    }
}