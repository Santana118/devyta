﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLIENT.Controllers
{
    public class AsetController : Controller
    {
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");
            var UserId = HttpContext.Session.GetString("UserId");
            ViewData["sessionRole"] = role;
            ViewData["sessionUserId"] = UserId;
            if (role != null && (role.Equals("Admin") || role.Equals("Staff")))
            {
                return View();

            }
            return RedirectToAction("Unauthorized", "Error");
         
        }

     
    }
}
