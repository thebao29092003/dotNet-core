using coreC_.Dtos.Comment;
using coreC_.Models;

namespace coreC_.Interfaces
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetAllCommentsAsync();
        Task<Comment?> GetByIdAsync(int id);
        Task<Comment> CreateCommentAsync(Comment comment);
        Task<Comment?> UpdateCommentAsync(UpdateCommentDto updateCommentDto);
        Task<Comment?> DeleteCommentAsync(int id);
    }
}
