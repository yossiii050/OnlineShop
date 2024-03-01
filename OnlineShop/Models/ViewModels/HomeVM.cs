using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Item> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
