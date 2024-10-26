using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using healthycannab.Models;
using healthycannab.Data;
using Microsoft.AspNetCore.Mvc.Filters;

namespace healthycannab.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
           if (User.Identity.IsAuthenticated)
        {
            ViewData["Layout"] = "~/Views/Shared/_layoutLogin.cshtml";
        }
        else
        {
            ViewData["Layout"] = "~/Views/Shared/_layout.cshtml";
        }

        base.OnActionExecuting(filterContext);
        }
    }
}