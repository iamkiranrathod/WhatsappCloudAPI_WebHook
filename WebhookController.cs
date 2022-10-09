using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace whatsapp.webhook.Controllers
{
    [Route("[Controller]")]
    [AllowAnonymous]
    [ApiController]
    public class WebhookController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment environment;

        public WebhookController(IConfiguration _config, IWebHostEnvironment _environment)
        {
            this._configuration = _config;
            this.environment = _environment;
        }

        [HttpGet]
        public ActionResult<string> Get([FromQuery(Name = "hub.mode")] string hubMode,
         [FromQuery(Name = "hub.challenge")] int hubChallenge,
         [FromQuery(Name = "hub.verify_token")] string hubVerifyToken)
        {
            if (string.IsNullOrEmpty(hubMode) && String.IsNullOrEmpty(hubVerifyToken))
            {
                WriteLog("BAD REQUEST: 403");
                return BadRequest(403);

            }
            if (hubVerifyToken != this._configuration.GetValue<string>("VerficationToken"))
            {
                WriteLog("BAD REQUEST: 403");
                return BadRequest(403);
            }
            WriteLog($"SUCCESS {hubChallenge.ToString()}");
            return Ok(hubChallenge);
        }

        [HttpPost]
        public ActionResult Post([FromBody] dynamic data)
        {
            WriteLog(data);
            return Ok(data);
        }

        [NonAction]
        private void WriteLog(string data)
        {
            var path = this.environment.ContentRootPath;
            var fileName = path + "Logs/log.txt";
            if (System.IO.File.Exists(fileName))
            {

                using (StreamWriter sw = System.IO.File.AppendText(fileName))
                {
                    sw.WriteLine("-----------------------------");
                    sw.WriteLine(DateTime.Now.ToString());
                    sw.WriteLine(data);
                    sw.WriteLine("-----------------------------");
                    sw.Close();
                    sw.Dispose();
                }
            }
        }
    }
}
