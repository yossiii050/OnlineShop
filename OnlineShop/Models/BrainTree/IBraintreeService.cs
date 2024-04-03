using Braintree;

namespace OnlineShop.Models.BrainTree
{
    public interface IBraintreeService
    {
        IBraintreeGateway CreateGateway();

        IBraintreeGateway GetGateway();
    }
}
