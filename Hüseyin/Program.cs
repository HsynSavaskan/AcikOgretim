using DinkToPdf.Contracts;
using DinkToPdf;
using Hüseyin.Models; // VeritabaniContext burada tanýmlý olmalý
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//  DbContext servis olarak ekleniyor
builder.Services.AddDbContext<VeritabaniContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VeritabaniBaglantisi")));
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

//  Session kullanýmý için gerekli ayarlar
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
