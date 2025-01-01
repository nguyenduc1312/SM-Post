using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Post.Query.Domain.Entities;

namespace Post.Query.Domain.Repositories
{
    public interface IPostRepository
    {
        Task CreateAsync(PostEntity post);
        Task UpdateAsync(PostEntity post);
        Task DeleteAsync(Guid id);
        Task<PostEntity> GetByIdAsync(Guid id);
        Task<IEnumerable<PostEntity>> GetAllAsync();
        Task<IEnumerable<PostEntity>> GetByAuthorAsync();
        Task<IEnumerable<PostEntity>> GetWithLikeAsync();
        Task<IEnumerable<PostEntity>> GetWithCommentAsync();
    }
}
