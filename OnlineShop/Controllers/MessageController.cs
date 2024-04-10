using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using OnlineShop.Models.Message;
using System.Security.Claims;
namespace OnlineShop.Controllers
{
    public class MessageController : BaseController
    {
        private readonly DBProjectContext _context;
        public MessageController(DBProjectContext context): base(context)
        {
            _context = context;
        }
        public IActionResult Index()
        {

            var ms =  _context.messages.ToList();
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var messages = _context.messages.Where(m => m.UserId == userId).ToList(); 
                return View(messages);
            }
            return View("AccessDenied");

        }

        public ActionResult MarkAsRead(int id)
        {
            var message = _context.messages.Find(id);
            if (message != null)
            {
                message.IsRead = true;
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteMessage(int id)
        {
            var message = await _context.messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            _context.messages.Remove(message);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index"); 
        }

    }
}
