using ASM1641_.Data;
using ASM1641_.IService;
using ASM1641_.Service;
using ASM1641_.Services;
using BookStore.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<DatabaseSetting>(builder.Configuration.GetSection("MyDatabase"));
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IAuthorService, AuthorService>();
builder.Services.AddTransient<IBookService, BookService>();
builder.Services.AddTransient<ICategoryService, CategoryService>();

var app = builder.Build();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
