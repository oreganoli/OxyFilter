using System;
using Microsoft.AspNetCore.Mvc;
using Scriban;

namespace O2Decrapper
{
    public class InfoController : Controller
    {
        private readonly IInfoService _infoService;
        private static string _template = System.IO.File.ReadAllText("index.html");

        public InfoController(IInfoService service)
        {
            _infoService = service;
        }
        // GET
        [Route("/")]
        public IActionResult Index()
        {
            var tempInfo = _infoService.Get();
            var last = tempInfo.LastDelete > tempInfo.Epoch ? tempInfo.LastDelete.ToString("o") : "None yet";
            object info = new
            {
                Epoch = tempInfo.Epoch.ToString("o"),
                Last = last,
                tempInfo.Counter
            };
            var uptime = DateTime.UtcNow.Subtract(tempInfo.Epoch).Duration();
            var html = Template.Parse(_template).Render(new {Info = info, Uptime = uptime});
            return Content(html, "text/html");
        }
    }
}