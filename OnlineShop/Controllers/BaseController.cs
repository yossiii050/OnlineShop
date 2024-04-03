﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OnlineShop.Models;
using System.Security.Claims;

namespace OnlineShop.Controllers
{
    public class BaseController : Controller
    {
        private readonly DBProjectContext _context;

        public BaseController(DBProjectContext context)
        {
            _context = context;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // Count unread messages for the current user
                ViewBag.UnreadMessages = _context.messages.Count(m => m.UserId == userId && !m.IsRead);
            }
                // Get the current user's ID
                
        }
    }
}