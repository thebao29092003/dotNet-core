namespace coreC_.Dtos.Comment
{
    public class UpdateCommentDto
    {
        public int CommentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
