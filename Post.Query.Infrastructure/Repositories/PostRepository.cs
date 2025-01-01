using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly DatabaseContextFactory _databaseContextFactory;
        public PostRepository(DatabaseContextFactory databaseContextFactory)
        {
            _databaseContextFactory = databaseContextFactory;
        }
        public async Task CreateAsync(PostEntity post)
        {
            using ApplicationDBContext context = _databaseContextFactory.CreateDbContext();
            context.Posts.Add(post);

            _ = await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid postId)
        {
            using ApplicationDBContext context = _databaseContextFactory.CreateDbContext();
            var post = await GetByIdAsync(postId);

            if (post == null) return;

            context.Posts.Remove(post);
            _ = await context.SaveChangesAsync();
        }

        public async Task<PostEntity> GetByIdAsync(Guid postId)
        {
            using ApplicationDBContext context = _databaseContextFactory.CreateDbContext();
            return await context.Posts.Include(item => item.Comments).FirstOrDefaultAsync(item => item.PostId == postId);
        }

        public async Task<List<PostEntity>> ListAllAsync()
        {
            using ApplicationDBContext context = _databaseContextFactory.CreateDbContext();
            return await context.Posts.AsNoTracking()
                    .Include(p => p.Comments).AsNoTracking()
                    .ToListAsync();
        }

        public async Task<List<PostEntity>> ListByAuthorAsync(string author)
        {
            using ApplicationDBContext context = _databaseContextFactory.CreateDbContext();
            return await context.Posts.AsNoTracking()
                    .Include(p => p.Comments).AsNoTracking()
                    .Where(x => x.Author.Contains(author))
                    .ToListAsync();
        }

        public async Task<List<PostEntity>> ListWithCommentsAsync()
        {
            using ApplicationDBContext context = _databaseContextFactory.CreateDbContext();
            return await context.Posts.AsNoTracking()
                    .Include(p => p.Comments).AsNoTracking()
                    .Where(x => x.Comments != null && x.Comments.Any())
                    .ToListAsync();
        }

        public async Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes)
        {
            using ApplicationDBContext context = _databaseContextFactory.CreateDbContext();
            return await context.Posts.AsNoTracking()
                    .Include(p => p.Comments).AsNoTracking()
                    .Where(x => x.Likes >= numberOfLikes)
                    .ToListAsync();
        }

        public async Task UpdateAsync(PostEntity post)
        {
            using ApplicationDBContext context = _databaseContextFactory.CreateDbContext();
            context.Posts.Update(post);

            _ = await context.SaveChangesAsync();
        }
    }
}
