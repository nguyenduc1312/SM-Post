using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Post.Query.Infrastructure.DataAccess
{
    public class DatabaseContextFactory
    {
        private readonly Action<DbContextOptionsBuilder> _configure;

        public DatabaseContextFactory(Action<DbContextOptionsBuilder> confiure)
        {
            _configure = confiure;
        }

        public ApplicationDBContext CreateDbContext()
        {
            DbContextOptionsBuilder<ApplicationDBContext> options = new();
            _configure(options);

            return new ApplicationDBContext(options.Options);
        }
    }
}
