using Microsoft.AspNetCore.Mvc;
using romeo_assistant_core.Models.Whatsapp;
using romeo_assistant_core.Services.Behaviour;

namespace romeo_assistant_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WhatsappController : ControllerBase
    {
        private readonly IBehaviour _behaviour;

        public WhatsappController(IBehaviour behaviour)
        {
            _behaviour = behaviour;
        }

        [HttpPost]
        public async Task<ActionResult> Get([FromBody] IncomingMessage incomingMessage)
        {
            await _behaviour.ExecuteWorkFlow(incomingMessage);
            return Ok();
        }
    }
}
