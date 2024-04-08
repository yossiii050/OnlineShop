namespace OnlineShop.Models.Message
{
    public class ProductNotification
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public bool Notified { get; set; } = false;
        public string UserId { get; set; }
        public virtual User User { get; set; }

    }

}
