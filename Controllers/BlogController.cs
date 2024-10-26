using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using healthycannab.Models;
using healthycannab.Data;
using healthycannab.Controllers;


namespace healthycannab.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Blog()
        {
            return View();
        }

         public IActionResult Blog2()
        {
            return View();
        }
    }
}
