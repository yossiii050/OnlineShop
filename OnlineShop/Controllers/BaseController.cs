using Microsoft.AspNetCore.Mvc;
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
               
                ViewBag.UnreadMessages = _context.messages.Count(m => m.UserId == userId && !m.IsRead);
                ViewBag.CartProducts=_context.CartItems.Where(m => m.UserId == userId)
                .Sum(m => m.Quantity);

    
            }
                
                
        }
    }
}
