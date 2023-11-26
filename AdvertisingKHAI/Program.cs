using AdvertisingKHAI.Models.DataBaseContext;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext and specify the connection string
builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseSqlite("Data Source=helloapp.db");
});

builder.Services.AddAuthentication("CookieAuthentication")
    .AddCookie("CookieAuthentication", options =>
    {
        options.Cookie.Name = "CookieAuthentication";
        options.LoginPath = "/Authorization/Login"; // Путь к странице входа
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//code for test
using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;
    ApplicationContext db = services.GetRequiredService<ApplicationContext>();
    db.ReBild();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
