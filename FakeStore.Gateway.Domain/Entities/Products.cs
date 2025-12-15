using System;
using System.Collections.Generic;
using System.Text;

namespace FakeStore.Gateway.Domain.Entities
{
    public class Products
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public decimal Price { get; set; }
        public string Description { get; set; } = "";
        public string Category { get; set; } = "";
        public string Image { get; set; } = "";
    }
}
