namespace FoodWebsiteMaster.Models.viewModel
{
    public class tempCart
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public DateTime? AddedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public decimal Price { get; set; }

        public string? Image { get; set; }

    }
}
