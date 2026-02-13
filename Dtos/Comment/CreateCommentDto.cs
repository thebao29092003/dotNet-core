using System.ComponentModel.DataAnnotations;

namespace coreC_.Dtos.Comment
{
    public class CreateCommentDto
    {
        public int StockId { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "Title must be 5 characters")]
        [MaxLength(100, ErrorMessage = "Title maximum length is 100 characters")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MinLength(5, ErrorMessage = "Content must be 5 characters")]
        [MaxLength(100, ErrorMessage = "Content maximum length is 100 characters")]
        public string Content { get; set; } = string.Empty;
    }
}
