namespace OnlineShop.Models.ViewModels
{
    public class OrderDetailsViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string UserName { get; set; } 
        public string UserNameandLname { get; set; } 

        public decimal FinalPrice { get; set; }
        public decimal DiscountHas { get; set; }
        public List<OrderItemViewModel> Items { get; set; }
        public string ShipStreet { get; set; }
        public string ShipCity { get; set; }
        public string ShipCountry { get; set; }
        public string ShipZipCode { get; set; }
        public OrderStatus Status { get; set; }
        public string fourCardNumber { get; set; }
        public string confirmationNumber { get; set; }

        public string phoneNumber { get; set; }
    }

    public class OrderItemViewModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
    }
}
