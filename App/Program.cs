using Game.BLL.Services;
using Game.DAL.Repos;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<Game.DAL.Repos.GameRepository>();
builder.Services.AddScoped<Game.BLL.Services.GameService>();

builder.Services.AddDbContext<Game.DAL.EF.GameSpdbContext>();
builder.Services.AddScoped<GameRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<GameService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AdminService>();  
builder.Services.AddScoped<NotificationService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Game.DAL.EF.GameSpdbContext>();
    Game.DAL.EF.DbInitializer.Initialize(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
   app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
