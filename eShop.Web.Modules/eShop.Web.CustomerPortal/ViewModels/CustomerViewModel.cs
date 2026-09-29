using System.ComponentModel.DataAnnotations;

namespace eShop.Web.CustomerPortal.ViewModels
{
    public class CustomerViewModel
    {
        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        public string CustomerAddress { get; set; } = string.Empty;

        [Required]
        public string CustomerCity { get; set; } = string.Empty;

        [Required]
        public string CustomerStateProvince { get; set; } = string.Empty;

        [Required]
        public string CustomerCountry { get; set; } = string.Empty;
    }
}
