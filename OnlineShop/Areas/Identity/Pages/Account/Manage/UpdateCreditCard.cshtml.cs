using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using Stripe;
using System.ComponentModel.DataAnnotations;

public class UpdateCreditCardModel : PageModel
{
    private readonly UserManager<User> _userManager;
    [TempData]
    public string StatusMessage { get; set; }
    public UpdateCreditCardModel(UserManager<User> userManager)
    {
        _userManager = userManager;


    }

    public class InputModel
    {
        [Required]
        [Display(Name = "New Credit Card Number")]
        public string NewCreditCardNumber { get; set; }
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        await LoadAsync(user);
        return Page();
    }

    private async Task LoadAsync(User user)
    {
        if (!string.IsNullOrEmpty(user.CreditCardNumber))
        {
            // Initialize Stripe services with your API key
            StripeConfiguration.ApiKey = "sk_test_51OoVvpDsGN87ngbiWu2M49BCYYy8udqXd2BAZEQ28dufWZx5bLXRqUPgqOpLLoGamAJU5IKVSgKjNdW51GVQ5VAZ00wmyk9ePQ";

            // Initialize CustomerService with your API key
            var customerService = new CustomerService();

            // Retrieve the customer information from Stripe
            var customer = await customerService.GetAsync(user.CreditCardNumber);

            // Check if the customer has any cards
            
            if (customer.DefaultSourceId != null )
            {
                // Assume we only show the first card
                var card = 1;
                
                

            }
            else
            {
                Console.WriteLine("No card found for customer");
            }
        }
        else
        {
            Console.WriteLine("User does not have a Stripe customer ID");
        }
    }


    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        }

        // Initialize Stripe services with your API key
        StripeConfiguration.ApiKey = "sk_test_51OoVvpDsGN87ngbiWu2M49BCYYy8udqXd2BAZEQ28dufWZx5bLXRqUPgqOpLLoGamAJU5IKVSgKjNdW51GVQ5VAZ00wmyk9ePQ";

        var customerService = new CustomerService();

        // Create a new customer in Stripe
        var customerOptions = new CustomerCreateOptions
        {
            Email = user.Email, // User's email address,
            Name=user.FirstName+user.LastName,
            Phone=user.PhoneNumber,
            

        };

        var customer = customerService.Create(customerOptions);

        // Retrieve the Stripe customer ID
        var stripeCustomerId = customer.Id;

        // Create a new card for the customer
        var cardOptions = new CardCreateOptions
        {
            Source = "tok_visa", // Stripe token representing the new credit card
        };

        var cardService = new CardService();
        var card = cardService.Create(stripeCustomerId, cardOptions);

        // Save the Stripe customer ID to the user record
        user.CreditCardNumber = stripeCustomerId;
        ViewData["CardBrand"] = card.Brand;
        ViewData["CardLast4"] = card.Last4;
        return Page();
    }

}
