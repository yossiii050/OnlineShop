using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;

namespace OnlineShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly DBUsers _users;
        public AccountController(DBUsers users)
        {
            _users=users;
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
                _users.User.Add(user);
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
            var users=_users.User.ToList();
            return View(users);
        }

        public IActionResult EditUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {
                _users.User.Update(user);
                _users.SaveChanges();             
                return RedirectToAction("DisplayUsers");
            }
            catch (Exception)
            {
                TempData["msg"]="Could not Update!";
                return View();
            }

            //var user_update = _users.User.Find(id);
            //return View(user_update);
        }
        public IActionResult DeleteUser(string id)
        {
            try
            {
                var user_dl = _users.User.Find(id);
                if (user_dl!=null)
                {
                    _users.User.Remove(user_dl);
                    _users.SaveChanges();
                }
            }
            catch (Exception)
            {

                throw;
            }

            return RedirectToAction("DisplayUsers");
        }

    }
}
