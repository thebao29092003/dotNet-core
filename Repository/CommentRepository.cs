using AutoMapper;
using coreC_.Data;
using coreC_.Dtos.Comment;
using coreC_.Interfaces;
using coreC_.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace coreC_.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;

        public CommentRepository(ApplicationDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return comment;
        }


        public async Task<List<Comment>> GetAllCommentsAsync()
        {
            return await _context.Comments.ToListAsync();
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
            return comment;
        }

        public async Task<Comment?> UpdateCommentAsync(UpdateCommentDto commentDto)
        {
            var existingComment = await GetByIdAsync(commentDto.CommentId);
            if (existingComment == null)
            {
                return null;
            }
            _mapper.Map(commentDto, existingComment);
            await _context.SaveChangesAsync();

            return existingComment;
        }

        public async Task<Comment?> DeleteCommentAsync(int id)
        {
            var existingComment = await GetByIdAsync(id);
            if (existingComment == null)
            {
                return null;
            }
            _context.Comments.Remove(existingComment);
            await _context.SaveChangesAsync();
            return existingComment;
        }
    }
}
