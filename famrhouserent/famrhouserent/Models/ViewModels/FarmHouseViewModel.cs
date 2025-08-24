using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace famrhouserent.Models.ViewModels
{
    public class FarmHouseViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        [StringLength(1000, ErrorMessage = "Description is too long.")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1, 20, ErrorMessage = "Bedrooms must be between 1 and 20.")]
        public int Bedrooms { get; set; }

        [Required]
        public string Amenities { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Maximum Guests")]
        [Range(1, 50, ErrorMessage = "Guests must be between 1 and 50.")]
        public int MaxGuests { get; set; }

        [Required]
        [Display(Name = "Price Per Night")]
        [Range(0.01, 100000, ErrorMessage = "Price must be greater than 0.")]
        public decimal PricePerNight { get; set; }
        public string ImageUrl { get; set; }  // 👈 Add this for the main image

        // For displaying existing image URLs
        public List<string>? ImageUrls { get; set; }
        public List<string> ExistingImages { get; set; } // If showing previous images

        // For adding/uploading new images
        public List<IFormFile>? Images { get; set; }
    }
}
