using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using Stripe;
using Stripe.Checkout;
namespace OnlineShop.Controllers
{
    [Authorize]
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
        public IActionResult OrderConfirmation()
        {
            var service = new SessionService();
            Session session = service.Get(TempData["Session"].ToString());
            if (session.PaymentStatus=="paid")
            {
                var transaction=session.PaymentIntentId.ToString();
                return View("Success");
            }
            return View("Cancel");
            
            
        }
        public IActionResult Success()
        {

            var options = new InvoiceCreateOptions {
                Customer = "cus_PeryQsudAqkUJq",
                CollectionMethod = "send_invoice",
                DaysUntilDue = 30,
            };
            var invoiceService = new InvoiceService();
            var invoice = invoiceService.Create(options);

            var invoiceItemOptions = new InvoiceItemCreateOptions
            {
                Customer = "cus_PeryQsudAqkUJq",
                Price = "price_1Opa7pDsGN87ngbigfpffDFh",
                Invoice = invoice.Id
            };
            var invoiceItemService = new InvoiceItemService();
            invoiceItemService.Create(invoiceItemOptions);

            // Send the Invoice
            invoiceService.SendInvoice(invoice.Id);
            return View();

        }

        public IActionResult Cancel()
        {

            return View();

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
                CancelUrl=domain+"CheckOut/Cancel",
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode="payment",

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

            TempData["Session"]=session.Id;

            Response.Headers.Add("Location", session.Url);


            return new StatusCodeResult(303);
        }
    }
}
