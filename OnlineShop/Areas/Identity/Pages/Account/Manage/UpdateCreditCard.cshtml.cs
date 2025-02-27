﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Newtonsoft.Json.Linq;


namespace OnlineShop.Areas.Identity.Pages.Account.Manage
{

    public class UpdateCreditCardModel : PageModel
    {
        private DBProjectContext _db;
        private List<CreditCard> userCardss;
        private readonly UserManager<User> _userManager;
        public string StatusMessage { get; set; }
        public List<CreditCard> CreditCards { get; set; }

        public string CardNumber { get; set; }
        public string ExpirationMonth { get; set; }
        public string ExpirationYear { get; set; }

        public string CVV { get; set; }
        public string CardOwner { get; set; }

        private readonly IAESSettings _aesSettings;


        [BindProperty]
        public UpdateCreditCardInputModel InputModel { get; set; }

        public class UpdateCreditCardInputModel
        {
            [Required]
            [Display(Name = "CardNumber")]
            public string CardNumber { get; set; }

            [Required]
            [Display(Name = "ExpirationMonth")]
            public string ExpirationMonth { get; set; }
            [Required]
            [Display(Name = "ExpirationYear")]
            public string ExpirationYear { get; set; }

            [Required]
            [Display(Name = "CVV")]
            public string CVV { get; set; }

            [Required]
            [Display(Name = "CardOwner")]
            public string CardOwner { get; set; }
        }


        public UpdateCreditCardModel(UserManager<User> userManager, IAESSettings aesSettings, DBProjectContext db)
        {
            _userManager = userManager;
            _db = db;
            _aesSettings = aesSettings;

        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<CreditCard> userCards = _db.CreditCards.Where(c => c.UserId == userId).ToList();

            foreach (var card in userCards)
            {
            }

            CreditCards = userCards;

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userCard = _db.CreditCards.FirstOrDefault(c => c.UserId == userId);

            byte[] key = Convert.FromBase64String(_aesSettings.Key);
            byte[] iv = Convert.FromBase64String(_aesSettings.IV);
            

            if (!ModelState.IsValid)
            {
                TempData["UpdateMessage"] = "Fields Error.";

                userCardss = _db.CreditCards.Where(c => c.UserId == userId).ToList();
                CreditCards=userCardss;
                return Page();
            }


            CardNumber=InputModel.CardNumber;
            ExpirationYear=InputModel.ExpirationYear;
            ExpirationMonth=InputModel.ExpirationMonth;

            CVV=InputModel.CVV;
            CardOwner=InputModel.CardOwner;

            if(!IsCardNumberValid(CardNumber))
            {
                userCardss = _db.CreditCards.Where(c => c.UserId == userId).ToList();
                CreditCards=userCardss;
                TempData["UpdateMessage"] = "Credit Card Number Error.";
                return Page();
            }
            if(!IsExpirationDateValid(ExpirationMonth, ExpirationYear))
            {
                userCardss = _db.CreditCards.Where(c => c.UserId == userId).ToList();
                CreditCards=userCardss;
                TempData["UpdateMessage"] = "Expiration Date Error.";
                return Page();
            }
            
            if (!IsCvvValid(CVV))
            {
                userCardss = _db.CreditCards.Where(c => c.UserId == userId).ToList();
                CreditCards=userCardss;
                TempData["UpdateMessage"] = "CVV Error.";
                return Page();
            }

            byte[] encryptedCardNumber = OnlineShop.Utillity.EncryptionHelper.EncryptStringToBytes_Aes(CardNumber, key, iv);
            byte[] encryptedCVV = OnlineShop.Utillity.EncryptionHelper.EncryptStringToBytes_Aes(CVV, key, iv);

            _db.CreditCards.Add(new CreditCard
            {
                EncryptedCardNumber = encryptedCardNumber,
                EncryptedExpirationDate=ExpirationMonth+"/"+ExpirationYear,
                EncryptedCVV = encryptedCVV,
                UserId=userId,
                NameCardOwner=CardOwner,
                fourLastNumber=CardNumber.Substring(CardNumber.Length - 4)
            });
            _db.SaveChanges();
           
            TempData["SuccesMessage"] = "Credit Card Added.";
            userCardss = _db.CreditCards.Where(c => c.UserId == userId).ToList();
            CreditCards=userCardss;
            return Page();
        }
        [HttpPost]
        public async Task<IActionResult> OnPostDeleteCreditCardAsync(int cardId)
        {
            var cardToDelete = _db.CreditCards.FirstOrDefault(c => c.Id == cardId);
            if (cardToDelete != null)
            {
                _db.CreditCards.Remove(cardToDelete);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Credit Card Deleted.";
            }
            return RedirectToPage();
        }
        public static bool IsCardNumberValid(string cardNumber)
        {
            int sum = 0;
            bool alternate = false;
            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                char[] digits = cardNumber.ToCharArray();
                int n = int.Parse(digits[i].ToString());
                if (alternate)
                {
                    n *= 2;
                    if (n > 9)
                    {
                        n = (n % 10) + 1;
                    }
                }
                sum += n;
                alternate = !alternate;
            }
            return (sum % 10 == 0);
        }
        public static bool IsExpirationDateValid(string ExpirationMonth, string ExpirationYear)
        {
            ExpirationMonth = ExpirationMonth.PadLeft(2, '0');

            if (DateTime.TryParseExact(ExpirationMonth+"/"+ExpirationYear, "MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime expDate))
            {
                return expDate > DateTime.Now;
            }
            return false;
        }

        public static bool IsCvvValid(string cvv)
        {
            return cvv.Length >= 3 && cvv.Length <= 4 && cvv.All(char.IsDigit);
        }

    }
}
