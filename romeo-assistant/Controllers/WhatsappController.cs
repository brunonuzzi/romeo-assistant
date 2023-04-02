using Microsoft.AspNetCore.Mvc;
using romeo_assistant.Models.Whatsapp;
using romeo_assistant.Services.Behaviour;

namespace romeo_assistant.Controllers
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
