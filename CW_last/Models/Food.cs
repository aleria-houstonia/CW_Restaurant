using System;

namespace CW_last.Models
{
    class MyProduct
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public string Image { get; set; }
        public static int Counter = 0;
        public string Category { get; set; }
        public int ID { get; private set; }
        Random rnd = new Random();

        public MyProduct(string name, string price, string image, string category)
        {
            Counter++;
            Name = name;
            Price = price;
            Image = image;
            Category = category;
            ID = rnd.Next(0, 100);
        }
        public string returnJsonObject()
        {
            return "{ 'name': '" + Name + "', 'price': '" + Price + "', 'image': '" + Image + "', 'id': '" + ID + "','category': '" + Category + "'}";
        }
        public string returnFormatLikeComboBoxItem()
        {
            return $"Name:{Name} | Price: {Price} | Category:{Category}";
        }

    }

}
