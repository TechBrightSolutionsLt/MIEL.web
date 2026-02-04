using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MIEL.web.Data;
using MIEL.web.Repositories;
using MIEL.web.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container.
//builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDBContext>(Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("MielConnectionString")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICategorySpecificationRepository, CategorySpecificationRepository>();
builder.Services.AddScoped<CategorySpecifications>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ProductService>();

var app = builder.Build();
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();        // AFTER routing
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
