using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using OnlineShop.Models.Cart;

namespace OnlineShop.Controllers
{
    public class PromoCodeController : Controller
    {
        private readonly DBProjectContext _context;

        public PromoCodeController(DBProjectContext context)
        {
            _context = context;
        }
        // GET: PromoCode
        public ActionResult Index()
        {
            var promoCodes = _context.PromoCodes.ToList();
            return View(promoCodes);
        }

        // GET: PromoCode/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PromoCode/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PromoCode promoCode)
        {
            if (ModelState.IsValid)
            {
                _context.PromoCodes.Add(promoCode);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(promoCode);
        }

        public ActionResult Delete(int id)
        {
            var promoCode = _context.PromoCodes.Find(id);
            if (promoCode == null)
            {
                return View();
            }
            return View(promoCode);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var promoCode = _context.PromoCodes.Find(id);
            _context.PromoCodes.Remove(promoCode);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Add other actions like Delete here if needed

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult ToggleActive(int id)
        {
            var promoCode = _context.PromoCodes.Find(id);
            if (promoCode == null)
            {
                return View();
            }

            promoCode.IsActive = !promoCode.IsActive; // Toggle the active status
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}

