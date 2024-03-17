using System.Text.Json;
using OnlineShop.Models;

namespace OnlineShop.Models.Cart
{
    public class CartItem
    {
        public int Id { get; set; } // Primary key
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string UserId { get; set; } // Add this line
        public string Image { get; set; }

        public virtual Product Product { get; set; }
        public virtual User user { get; set; }
    }

    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }

}
