using System;

namespace CW_last.Models
{
    internal class Cart
    {
    }
    class CartItem
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public int Count { get; set; }
        public string ProductId { get; set; }   

        public CartItem(string name, string price, int count,string prodId)
        {
            Name = name;
            Price = price;
            Count = count;
            ProductId = prodId;
        }
        public string returnJsonObject()
        {
            return "{ 'name': '" + Name + "', 'price': '" + Price + "','count': '"+Count+"','productId':'"+ProductId+"'}";
        }
    }

}
