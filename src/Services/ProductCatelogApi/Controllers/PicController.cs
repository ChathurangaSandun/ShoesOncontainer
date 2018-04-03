using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ProductCatelogApi.Controllers
{
    using Microsoft.AspNetCore.Hosting;

    [Produces("application/json")]
    [Route("api/pic")]
    public class PicController : Controller
    {
        private readonly IHostingEnvironment _env;

        public PicController(IHostingEnvironment env)
        {
            this._env = env;
        }

        public IActionResult GetImage(int id)
        {

        }
    }
}