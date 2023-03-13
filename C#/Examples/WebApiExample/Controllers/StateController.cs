using Microsoft.AspNetCore.Mvc;
using Linql.Core;
using System.IO;
using System.Text.Json;
using Linql.Server;

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
    }
}