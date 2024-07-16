using DevBook.Data;
using DevBook.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DevBook.Areas.Admin.Controllers.Api
{
    [Route("Admin/Api/BlogApi")]
    [ApiController]
    public class BlogApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BlogApiController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public ActionResult<IEnumerable<PostModel>> GetPosts()
        {
            var posts = _context.Posts.ToList().OrderBy(p => p.CreatedAt);

            return Ok(posts);
        }

        [HttpGet("{id}")]
        public ActionResult<PostModel> GetPost(int id)
        {
            var post = _context.Posts.Find(id);

            if (post == null)
            {
                return NotFound();
            }

            return Ok(post);
        }
    }
}
