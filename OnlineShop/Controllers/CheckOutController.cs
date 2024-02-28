using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
//using Stripe.BillingPortal;
using Stripe.Checkout;
namespace OnlineShop.Controllers
{
    public class CheckOutController : Controller
    {
        public IActionResult Index()
        {
            List<Item> itemList = new List<Item>();

            itemList=new List<Item>
            {
                new Item
                {
                    Id = 1,
                    Name="Rose",
                    Description="Red",
                    Price=150,
                    Quantity=5,
                    ImagePath="images/img-3.jpg"
                },
                new Item
                {
                    Id = 2,
                    Name="Tulips",
                    Description="White",
                    Price=250,
                    Quantity=2,
                    ImagePath="images/img-5.jpg"
                }
                
            };
            return View(itemList);
        }

        public IActionResult CheckOut() 
        {
            List<Item> itemList = new List<Item>();

            itemList=new List<Item>
            {
                new Item
                {
                    Id = 1,
                    Name="Rose",
                    Description="Red",
                    Price=150,
                    Quantity=5,
                    ImagePath="images/img-3.jpg"
                },
                new Item
                {
                    Id = 2,
                    Name="Tulips",
                    Description="White",
                    Price=250,
                    Quantity=2,
                    ImagePath="images/img-5.jpg"
                }

            };

            var domain = "http://localhost:5036/";

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl=domain+$"CheckOut/OrderConfirmation",
                CancelUrl=domain+"CheckOut/Login",
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode="payment"
            };

            foreach (var item in itemList)
            {
                var sessionListItem = new SessionLineItemOptions
                {
                    PriceData=new SessionLineItemPriceDataOptions
                    {
                        UnitAmount=(long)(item.Price*item.Quantity),
                        Currency="ils",
                        ProductData=new SessionLineItemPriceDataProductDataOptions
                        {
                            Name=item.Name.ToString(),
                        }
                    },
                    Quantity=item.Quantity
                };

                options.LineItems.Add(sessionListItem);
            }

            var service = new Stripe.Checkout.SessionService();
            Session session=service.Create(options);

            Response.Headers.Add("Location", session.Url);


            return new StatusCodeResult(303);
        }
    }
}
