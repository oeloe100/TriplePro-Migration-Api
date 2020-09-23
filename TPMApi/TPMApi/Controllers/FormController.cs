﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TPMApi.Controllers
{
    public class FormController : Controller
    {
        public IActionResult LoginPartial(string viewName)
        {
            return PartialView("_LoginFormPartial");
        }

        public IActionResult RegisterPartial(string viewName)
        {
            return PartialView("_RegisterFormPartial");
        }
    }
}