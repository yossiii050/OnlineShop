namespace OnlineShop.Models.Message
{
    public class MessageInbox
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public DateTime ReceivedTime { get; set; }
        public bool IsRead { get; set; }
        public virtual User User { get; set; }
        public string UserId { get; set; } // Add this line

    }
}
