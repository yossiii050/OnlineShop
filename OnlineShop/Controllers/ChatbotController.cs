using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using OnlineShop.Models.ChatBot;
using System.Security.Claims;

namespace OnlineShop.Controllers
{
    public class ChatbotController : BaseController
    {
        private DBProjectContext _db;
        private ChatbotService _chatbotService;

        public ChatbotController(DBProjectContext db) : base(db)
        {
            _db = db;
            _chatbotService = new ChatbotService(_db);

        }
        public ActionResult Chat()
        {
            return View("SmartChat");
        }

        [HttpPost]
        public ActionResult GetResponse(string userQuestion)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var response = _chatbotService.GetResponse(userQuestion, userId);
            return Json(response);
        }
    }
}
