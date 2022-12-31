using Microsoft.EntityFrameworkCore;
namespace MInimalAPI_CURD
{

    public class ApiContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }

        public ApiContext(DbContextOptions<ApiContext> options)
            : base(options)
        { }
    }
}
