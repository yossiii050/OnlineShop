namespace OnlineShop.Models.ViewModels
{
    public class OrderDetailsViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderItemViewModel> Items { get; set; }
    }

    public class OrderItemViewModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
