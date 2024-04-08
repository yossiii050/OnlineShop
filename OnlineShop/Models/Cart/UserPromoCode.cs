namespace OnlineShop.Models.Cart
{
    public class UserPromoCode
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int PromoCodeId { get; set; }
        public PromoCode PromoCode { get; set; }
    }

}
