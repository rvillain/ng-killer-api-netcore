﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebPush;
using NgKillerApiCore.DAL;

namespace NgKillerApiCore.Controllers
{

    public class PayloadVm
    {
        public string Payload { get; set; }
    }
    public class WebPushController : Controller
    {
        private readonly IConfiguration _configuration;

        private readonly KillerContext _context;

        public WebPushController(KillerContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult GenerateKeys()
        {
            var keys = VapidHelper.GenerateVapidKeys();
            ViewBag.PublicKey = keys.PublicKey;
            ViewBag.PrivateKey = keys.PrivateKey;
            return View();
        }
    }
}