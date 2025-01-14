using Microsoft.EntityFrameworkCore;
using BazaMuzyczna.Models;
using BazaMuzyczna.Controllers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbConnection")));

var app = builder.Build();


app.UseRouting();
//app.UseAuthorization();
app.MapControllers();

app.Run();
