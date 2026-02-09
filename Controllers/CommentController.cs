using AutoMapper;
using coreC_.Dtos.Comment;
using coreC_.Interfaces;
using coreC_.Models;
using Microsoft.AspNetCore.Mvc;

namespace coreC_.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IStockRepository _stockRepository;
        private readonly IMapper _mapper;
        public CommentController(ICommentRepository commentRepository, IStockRepository stockRepository, IMapper mapper)
        {
            _commentRepository = commentRepository;
            _stockRepository = stockRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var comments = await _commentRepository.GetAllCommentsAsync();
            List<CommentDto> commentDto = _mapper.Map<List<CommentDto>>(comments);
            return Ok(commentDto);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            CommentDto commentDto = _mapper.Map<CommentDto>(comment);
            return Ok(commentDto);
        }

        [HttpPost]
        // biến trong param(stockId) phải giống trong route("{stockId}")
        public async Task<IActionResult> Create(CreateCommentDto commentDto)
        {
            if (!await _stockRepository.StockExistsAsync(commentDto.StockId))
            {
                return BadRequest("Stock does not exist");
            }

            var commentModel = _mapper.Map<Comment>(commentDto);
            var comment = await _commentRepository.CreateCommentAsync(commentModel);
            return CreatedAtAction(nameof(GetById), new {id = comment.Id}, comment);
        }

    }
}
