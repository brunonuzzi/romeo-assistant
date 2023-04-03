using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using romeo_assistant_core.Models.Whatsapp;
using romeo_assistant_core.Services.Behaviour;
using System.IO;
using System.Threading.Tasks;

namespace romeo_assistant_az_functions
{
    public class HttpWhatsapp
    {
        private readonly IBehaviour _behaviour;

        public HttpWhatsapp(IBehaviour behaviour)
        {
            _behaviour = behaviour;
        }

        [FunctionName("HttpWhatsapp")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            IncomingMessage incomingMessage = JsonConvert.DeserializeObject<IncomingMessage>(requestBody);

            await _behaviour.ExecuteWorkFlow(incomingMessage);

            return new OkObjectResult("Ok");
        }
    }
}
