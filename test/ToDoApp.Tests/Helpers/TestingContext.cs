using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Objects;
using ToDoApp.Objects.Mapping;

namespace ToDoApp
{
    public static class TestingContext
    {
        public static IMapper Mapper { get; }
        private static DbContextOptions Options { get; }

        static TestingContext()
        {
            Mapper = new MapperConfiguration(mapper => mapper.AddProfile(new MappingProfile())).CreateMapper();
            Options = new DbContextOptionsBuilder()
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ToDoAppTest;Trusted_Connection=True;MultipleActiveResultSets=True")
                .Options;

            using Context context = new(Options);

            context.Database.Migrate();
        }

        public static DbContext Create()
        {
            return new Context(Options);
        }

        public static DbContext Drop(this DbContext context)
        {
            context.RemoveRange(context.Set<RolePermission>());
            context.RemoveRange(context.Set<Permission>());
            context.RemoveRange(context.Set<AuditLog>());
            context.RemoveRange(context.Set<Account>());
            context.RemoveRange(context.Set<Role>());

            context.SaveChanges();

            return context;
        }
    }
}
