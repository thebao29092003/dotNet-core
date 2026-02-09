using coreC_.Models;

namespace coreC_.Interfaces
{
    public interface ICommentRepository
    {
        Task<List<Comment>> GetAllCommentsAsync();
        Task<Comment?> GetByIdAsync(int id);
        Task<Comment> CreateCommentAsync(Comment comment);
    }
}
