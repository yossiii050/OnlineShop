namespace OnlineShop.Models.Cart
{
    public class PromoCode
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; }
    }
}
