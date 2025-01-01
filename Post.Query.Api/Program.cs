using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;
using Post.Query.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

Action<DbContextOptionsBuilder> configureDB = (o => o.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));
builder.Services.AddDbContext<ApplicationDBContext>(configureDB);
builder.Services.AddSingleton<DatabaseContextFactory>(new DatabaseContextFactory(configureDB));

//Create DB and table
var dataContext = builder.Services.BuildServiceProvider().GetRequiredService<ApplicationDBContext>();
dataContext.Database.EnsureCreated();

// Add services to the container.
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
