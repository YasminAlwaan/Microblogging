using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microblogging.Core.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required]
        [StringLength(140, ErrorMessage = "Post cannot exceed 140 characters")]
        public string Content { get; set; }

        public string OriginalImageUrl { get; set; }
        public string ProcessedImageUrl { get; set; }
        public bool IsImageProcessed { get; set; }
        public DateTime CreatedAt { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public Dictionary<string, string> ProcessedImageUrls { get; set; } = new();
    }
}
