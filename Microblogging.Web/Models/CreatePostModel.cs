using System.ComponentModel.DataAnnotations;

namespace Microblogging.Web.Models
{
    public class CreatePostModel
    {
        [Required(ErrorMessage = "Post content is required")]
        [StringLength(140, ErrorMessage = "Post cannot exceed 140 characters")]
        public string Content { get; set; }

        [DataType(DataType.Upload)]
        [MaxFileSize(2 * 1024 * 1024)] // 2MB
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".webp" })]
        public IFormFile Image { get; set; }
    }
}