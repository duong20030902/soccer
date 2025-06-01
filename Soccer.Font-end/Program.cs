using Soccer.Font_end.Areas.Services;
using Soccer.Font_end.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<Soccer.Font_end.Services.ProductService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<ForgotPasswordService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<Soccer.Font_end.Areas.Services.BookingService>();
builder.Services.AddScoped<BrandService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ColorService>();
builder.Services.AddScoped<Soccer.Font_end.Areas.Services.FieldService>();
builder.Services.AddScoped<OrderStatusService>();
builder.Services.AddScoped<PaymentMethodService>();
builder.Services.AddScoped<ShippingMethodService>();
builder.Services.AddScoped<ShippingMethodService>();
builder.Services.AddScoped<Soccer.Font_end.Areas.Services.ProductService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<SizeService>();
builder.Services.AddScoped<SupplierService>();
builder.Services.AddScoped<TimeslotService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<Soccer.Font_end.Services.FieldService>();
builder.Services.AddScoped<Soccer.Font_end.Services.BookingService>();
builder.Services.AddSession();
builder.WebHost.UseUrls("https://+:7170");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
