
using OnlineShop.Models.Message;
using Stripe.Climate;
using System.Linq;
using System.Security.Claims;

namespace OnlineShop.Models.ChatBot
{
    public class ChatbotService
    {
        private DBProjectContext _db;
        public ChatbotService(DBProjectContext _db)
        {
            this._db = _db;
        }
        public ChatbotResponse GetResponse(string userQuestion, string userId)
        {

            var response = new ChatbotResponse { UserQuestion = userQuestion };

            switch (userQuestion.ToLower())
            {
                case "hello":
                    response.BotAnswer = "Hi there! How can I help you today?";
                    break;

                case "contact me":
                    if (userId!=null)
                    {
                        NotifyAdminForContactRequest(userId);
                        response.BotAnswer = "Thank you for your request. Our admin will contact you soon.";
                        break;
                    }
                    response.BotAnswer ="Please Login First.";
                    break;

                case "what is my order status?":
                    if (userId!=null)
                    {
                        
                        var order = _db.Orders
                            .Where(o => o.UserId == userId)
                            .OrderByDescending(o => o.OrderDate)
                            .FirstOrDefault();

                        if (order != null)
                        {
                            response.BotAnswer = $"Your latest order details are:\n" +
                                                     $"Order Number: {order.confirmationNumber}\n" +
                                                     $"Order Date: {order.OrderDate.ToString("MM/dd/yyyy")}\n" +
                                                     $"Order Status: {order.Status}";
                        }
                        else
                        {
                            response.BotAnswer = "You don't have any orders yet.";
                        }
                        break;
                    }
                    response.BotAnswer ="Please Login First.";
                    break;

                case "what is your return policy?":
                    response.BotAnswer = "Our return policy is customer-friendly. You can return products within 30 days of purchase for a full refund or exchange. Please ensure the items are in their original condition and packaging. For more details, visit our Return Policy page.";
                    break;
                case "do you offer free shipping?":
                    response.BotAnswer = "Absolutely! We offer free shipping on all orders over $50. For orders under $50, a standard shipping fee applies. We strive to deliver your products swiftly and securely.";
                    break;
                case "how can i track my order?":
                    response.BotAnswer = "Tracking your order is easy! Once your order is shipped, you will receive a confirmation email with a tracking number. You can use this number on our website's tracking page or the carrier's website to monitor your order's progress.";
                    break;
                case "are your products organic?":
                    response.BotAnswer = "Yes, we take pride in offering products that are 100% certified organic. We believe in sustainability and providing our customers with healthy and environmentally friendly options.";
                    break;
                case "what are your store hours?":
                    response.BotAnswer = "Our store is open from 9am to 5pm, Monday to Friday. We are closed on weekends and public holidays. We look forward to welcoming you during our operating hours!";
                    break;

                case "can i return online purchases in-store?":
                    response.BotAnswer = "Certainly! You can return online purchases at any of our store locations. Please bring the items along with your receipt or order confirmation for a hassle-free return process.";
                    break;

                case "help":
                    response.BotAnswer = "Here are some questions you can ask me:\n" +
                                         "- Hello\n" +
                                         "-what is my order status?\n"+
                                         "-contact me\n"+
                                         "- What is your return policy?\n" +
                                         "- Do you offer free shipping?\n" +
                                         "- How can I track my order?\n" +
                                         "- Are your products organic?\n" +
                                         "- What are your store hours?\n" +
                                         "- Can I return online purchases in-store?\n" +
                                         "- Help";
                    break;
                default:
                    response.BotAnswer = "Sorry, I'm not sure how to answer that. Can you try asking something else?\n"+
                        "Type Help for see all questions";
                    break;
            }

            return response;
        }
        private void NotifyAdminForContactRequest(string userId)
        {
            var adminroleID=_db.Roles.Where(u=>u.Name=="Admin").ToList();
            var adminUsers = _db.UserRoles.Where(u => u.RoleId==adminroleID[0].Id).ToList();

            foreach (var admin in adminUsers)
            {
                var message = new MessageInbox
                {
                    Subject = "Contact Request from User",
                    Content = $"User with ID {userId} has requested to be contacted.",
                    ReceivedTime = DateTime.Now,
                    IsRead = false,
                    UserId = admin.UserId 
                };

                _db.messages.Add(message);
            }

            _db.SaveChanges();
        }

    }

}
