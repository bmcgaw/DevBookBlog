using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevBook.Models
{
    public class CommentModel
    {
        public int Id { get; set; }

        [Required]
        public string? Content { get; set; }

        public string? UserId { get; set; }

        public ApplicationUser? User { get; set; }  

        public int PostId { get; set; }

        public PostModel? Post { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
