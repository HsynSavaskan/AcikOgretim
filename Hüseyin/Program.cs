using DinkToPdf.Contracts;
using DinkToPdf;
using H�seyin.Models; // VeritabaniContext burada tan�ml� olmal�
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//  DbContext servis olarak ekleniyor
builder.Services.AddDbContext<VeritabaniContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VeritabaniBaglantisi")));
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

//  Session kullan�m� i�in gerekli ayarlar
builder.Services.AddSession();
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession(); // <-- Session aktif edilmeli
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
