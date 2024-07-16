using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevBook.Models
{
    public class PostModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Title { get; set; }

        [Required]
        public string? Content { get; set; }

        public string? UserId { get; set; }

        public ApplicationUser? User { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Created Date")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<CommentModel>? Comments { get; set; }
        public ICollection<PostTagModel>? PostTags { get; set; }

        [NotMapped]
        [DisplayName("Tags")]
        public string? TagList { get; set; }

        [ValidateNever]
        public string? ImageUrl { get; set; }
    }
}


