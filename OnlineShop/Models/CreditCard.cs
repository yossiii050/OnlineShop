namespace OnlineShop.Models
{
    public class CreditCard
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string NameCardOwner { get; set; }
        public byte[] EncryptedCardNumber { get; set; }
        public string EncryptedExpirationDate { get; set; }
        public byte[] EncryptedCVV { get; set; }
        public string fourLastNumber { get; set; }

        public virtual User User { get; set; }
    }

}
