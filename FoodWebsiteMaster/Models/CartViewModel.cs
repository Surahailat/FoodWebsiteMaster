namespace FoodWebsiteMaster.Models
{
    public class CartFormModel
    {
        public int CartID { get; set; }
        public int UserID { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<CartItemModel> CartItems { get; set; } // قائمة من العناصر داخل السلة
    }

    public class CartItemModel
    {
        public int CartItemID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public decimal Total => Price * Quantity; // الحاصل الإجمالي (السعر * الكمية)
    }

}
