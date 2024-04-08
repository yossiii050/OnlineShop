using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace OnlineShop.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<User> _userManager;

        private readonly DBProjectContext _users;
        public AccountController(UserManager<User> userManager,DBProjectContext users) : base(users)
        {
            _users=users;
            _userManager=userManager;

        }
            
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddUser()
        {
            return View("AddUser");
        }

        [HttpPost]
        public IActionResult AddUser(User user) {
            if(!ModelState.IsValid)
            {
                return View("AddUser");
            }
            try
            {
                _users.Users.Add(user);
                _users.SaveChanges();
                TempData["msg"]="Added successfully";
                return RedirectToAction("AddUser");
            }
            catch (Exception)
            {
                TempData["msg"]="Could not added!";
                return View("AddUser");
            }
  
        }

        public IActionResult DisplayUsers()
        {
            var users=_users.Users.ToList();
            return View(users);
        }

        public IActionResult EditUser(string id)
        {
            User user = _users.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public IActionResult EditUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {
                _users.Users.Update(user);
                _users.SaveChanges();             
                return RedirectToAction("DisplayUsers");
            }
            catch (Exception)
            {
                TempData["msg"]="Could not Update!";
                return View();
            }

        }
        public IActionResult DeleteUser(string id)
        {
            try
            {
                var user_dl = _users.Users.Find(id);
                if (user_dl!=null)
                {
                    _users.Users.Remove(user_dl);
                    _users.SaveChanges();
                }
            }
            catch (Exception)
            {

                throw;
            }

            return RedirectToAction("DisplayUsers");
        }

        public async Task<IActionResult> GetOrdersForUser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var orders = _users.Orders
                             .Where(o => o.UserId == user.Id)
                             .Include(o => o.OrderItems)
                             .ThenInclude(oi => oi.Product)
                             .AsNoTracking() 
                             .ToList(); 

            return View("OrderHistory", orders);
        }

    }
}
