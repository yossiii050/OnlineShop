namespace OnlineShop.Models
{
    public interface IAESSettings
    {
        string Key { get; set; }
        string IV { get; set; }
    }

    public class AESSettings : IAESSettings
    {
        public string Key { get; set; }
        public string IV { get; set; }
    }
}
