using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using OnlineShop.Models.Cart;

namespace OnlineShop.Controllers
{
    public class PromoCodeController : BaseController
    {
        private readonly DBProjectContext _context;

        public PromoCodeController(DBProjectContext context) : base(context)
        {
            _context = context;
        }
        public ActionResult Index()
        {
            var promoCodes = _context.PromoCodes.ToList();
            return View(promoCodes);
        }

        public ActionResult Create()
        {
            return View();
        }

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

            promoCode.IsActive = !promoCode.IsActive; 
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        

    }
}

