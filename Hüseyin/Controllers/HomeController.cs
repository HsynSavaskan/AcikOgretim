using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Hüseyin.Models;
using Microsoft.EntityFrameworkCore;
using DinkToPdf.Contracts;
using DinkToPdf;
using DinkToPdf.Contracts;
using System.Text;

namespace Hüseyin.Controllers;

public class HomeController : Controller
{
    private readonly VeritabaniContext _context;
    private readonly IConverter _converter;

    public HomeController(VeritabaniContext context, IConverter converter)
    {
        _context = context;
        _converter = converter;
    }
    


    public IActionResult Index()
    {
        var duyurular = _context.Duyurulars
        .Include(d => d.Ogretmen) // navigation varsa
        .OrderByDescending(d => d.EklenmeTarihi)
        .Select(d => new
        {
            Metin = d.DuyuruMetin,
            EklenmeTarihi = d.EklenmeTarihi,
            OgretmenAdSoyad = d.Ogretmen.Ad + " " + d.Ogretmen.Soyad
        })
        .ToList();

        ViewBag.Duyurular = duyurular;
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    [HttpGet]
    public IActionResult DuyuruEkle()
    {
        if (HttpContext.Session.GetInt32("OgretmenId") == null)
            return RedirectToAction("OgretmenGiris");

        return View();
    }

    [HttpPost]
    public IActionResult DuyuruEkle(string DuyuruMetin)
    {
        int? ogretmenId = HttpContext.Session.GetInt32("OgretmenId");

        if (ogretmenId == null)
            return RedirectToAction("OgretmenGiris");

        var duyuru = new Duyurular
        {
            OgretmenId = ogretmenId.Value,
            DuyuruMetin = DuyuruMetin,
            EklenmeTarihi = DateTime.Now
        };

        _context.Duyurulars.Add(duyuru);
        _context.SaveChanges();

        TempData["Mesaj"] = "Duyuru başarıyla eklendi.";
        return RedirectToAction("DuyuruEkle");
    }

    [HttpGet]
    public IActionResult OgrenciGiris()
    {
        return View();
    }
    [HttpPost]
    public IActionResult OgrenciGiris(string Email, string Sifre)
    {
        // Email ve şifreye göre öğrenci arıyoruz
        var ogrenci = _context.Ogrencilers
            .FirstOrDefault(x => x.Email == Email && x.Sifre == Sifre);

        if (ogrenci != null)
        {
            // Session'a öğrenci bilgilerini kaydet
            HttpContext.Session.SetInt32("OgrenciId", ogrenci.OgrenciId);
            HttpContext.Session.SetString("Ad", ogrenci.Ad);
            HttpContext.Session.SetString("Soyad", ogrenci.Soyad);
            HttpContext.Session.SetString("Email", ogrenci.Email);

            // Giriş başarılıysa anasayfaya yönlendir
            return RedirectToAction("OgrenciMenu");
        }

        ViewBag.Hata = "Email veya şifre hatalı!";
        return View();
    }

    public IActionResult OgrenciMenu()
    {
        // Kullanıcı giriş yapmamışsa yönlendir
        if (HttpContext.Session.GetInt32("OgrenciId") == null)
            return RedirectToAction("OgrenciGiris");
        var duyurular = _context.Duyurulars
        .Include(d => d.Ogretmen)
        .OrderByDescending(d => d.EklenmeTarihi)
        .Select(d => new
        {
            Metin = d.DuyuruMetin,
            EklenmeTarihi = d.EklenmeTarihi,
            OgretmenAdSoyad = d.Ogretmen.Ad + " " + d.Ogretmen.Soyad
        })
        .ToList();

        ViewBag.Duyurular = duyurular;
        return View();
    }



    public IActionResult AldigiDersler()
    {
        int? ogrenciId = HttpContext.Session.GetInt32("OgrenciId");

        if (ogrenciId == null)
            return RedirectToAction("OgrenciGiris");

        var dersler = _context.OgrenciDersleris
     .Include(od => od.Ogrenci)
     .Include(od => od.Ders)
         .ThenInclude(d => d.Ogretmen)
     .Where(od => od.OgrenciId == ogrenciId)
     .Select(od => new AlinanDersViewModel
     {
         OgrenciAdSoyad = od.Ogrenci.Ad + " " + od.Ogrenci.Soyad,
         OgretmenAdSoyad = od.Ders.Ogretmen.Ad + " " + od.Ders.Ogretmen.Soyad + " (" + od.Ders.DersAdi + ")",
         Not1 = od.Not1,
         Not2 = od.Not2
     })
     .ToList();

        return View(dersler);
    }
    [HttpGet]
    public IActionResult OgretmenGiris()
    {
        return View();
    }
    [HttpPost]
    public IActionResult OgretmenGiris(string Email, string Sifre)
    {
        var ogretmen = _context.Ogretmenlers
            .FirstOrDefault(x => x.Email == Email && x.Sifre == Sifre);

        if (ogretmen != null)
        {
            // Session'a öğretmen bilgilerini kaydet
            HttpContext.Session.SetInt32("OgretmenId", ogretmen.OgretmenId);
            HttpContext.Session.SetString("Ad", ogretmen.Ad);
            HttpContext.Session.SetString("Soyad", ogretmen.Soyad);
            HttpContext.Session.SetString("Email", ogretmen.Email);

            // Giriş başarılı → öğretmen menüsüne yönlendir
            return RedirectToAction("OgretmenMenu");
        }

        ViewBag.Hata = "Email veya şifre hatalı!";
        return View();
    }

    public IActionResult OgretmenMenu()
    {
        int? ogretmenId = HttpContext.Session.GetInt32("OgretmenId");
        if (ogretmenId == null)
            return RedirectToAction("OgretmenGiris");

        var verilenDersler = _context.Derslers
            .Where(d => d.OgretmenId == ogretmenId)
            .Select(d => new
            {
                d.DersAdi
            })
            .ToList();

        ViewBag.Dersler = verilenDersler;
        return View();
    }

    [HttpGet]
    public IActionResult NotVer(int? dersId)
    {
        int? ogretmenId = HttpContext.Session.GetInt32("OgretmenId");
        if (ogretmenId == null)
            return RedirectToAction("OgretmenGiris");

        // Bu öğretmenin verdiği dersler
        var dersler = _context.Derslers
            .Where(d => d.OgretmenId == ogretmenId)
            .ToList();

        // Seçilen derse göre öğrenciler
        List<OgrenciDersleri> ogrenciDersleri = new();

        if (dersId != null)
        {
            ogrenciDersleri = _context.OgrenciDersleris
                .Include(od => od.Ogrenci)
                .Where(od => od.DersId == dersId)
                .ToList();
        }

        ViewBag.Dersler = dersler;
        ViewBag.OgrenciDersleri = ogrenciDersleri;
        ViewBag.SeciliDersId = dersId;

        return View();
    }

    [HttpPost]
    public IActionResult NotVer(int ogrenciDersId, int not1, int not2)
    {
        var kayit = _context.OgrenciDersleris.FirstOrDefault(od => od.Id == ogrenciDersId);
        if (kayit != null)
        {
            kayit.Not1 = not1;
            kayit.Not2 = not2;
            _context.SaveChanges();
        }

        return RedirectToAction("NotVer", new { dersId = kayit?.DersId });
    }
    [HttpGet]
    public IActionResult OgrenciKaydet()
    {
        return View();
    }

    [HttpPost]
    public IActionResult OgrenciKaydet(Ogrenciler ogr)
    {
        if (ModelState.IsValid)
        {
            _context.Ogrencilers.Add(ogr);
            _context.SaveChanges();
            TempData["Mesaj"] = "Öğrenci başarıyla kaydedildi.";
            return RedirectToAction("OgrenciKaydet");
        }

        TempData["Mesaj"] = "Hatalı giriş!";
        return View(ogr);
    }

    [HttpGet]
    public IActionResult DersAta(int? ogrenciId, string dersAdi)
    {
        ViewBag.Ogrenciler = _context.Ogrencilers.ToList();
        ViewBag.SeciliOgrenciId = ogrenciId;
        ViewBag.SeciliDersAdi = dersAdi;

        // 🔹 Öğrenciye atanmış ders adları (tekrar kaldırmak için)
        List<string> zatenAlinanDersler = new();
        if (ogrenciId != null)
        {
            zatenAlinanDersler = _context.OgrenciDersleris
                .Where(x => x.OgrenciId == ogrenciId)
                .Select(x => x.Ders.DersAdi)
                .Distinct()
                .ToList();
        }

        // 🔹 Tekil ders adları (ama öğrencinin almadıkları)
        var dersAdlari = _context.Derslers
            .Select(d => d.DersAdi)
            .Distinct()
            .Where(ad => !zatenAlinanDersler.Contains(ad))
            .ToList();

        ViewBag.DersAdlari = dersAdlari;

        if (!string.IsNullOrEmpty(dersAdi))
        {
            ViewBag.Ogretmenler = _context.Derslers
                .Where(d => d.DersAdi == dersAdi)
                .Join(_context.Ogretmenlers,
                      ders => ders.OgretmenId,
                      ogr => ogr.OgretmenId,
                      (ders, ogr) => ogr)
                .Distinct()
                .ToList();
        }

        return View();
    }

    [HttpPost]
    public IActionResult DersAta(int OgrenciId, string DersAdi, int OgretmenId)
    {
        var ders = _context.Derslers
            .FirstOrDefault(d => d.DersAdi == DersAdi && d.OgretmenId == OgretmenId);

        if (ders == null)
        {
            TempData["Mesaj"] = "Ders bulunamadı.";
            return RedirectToAction("DersAta");
        }

        var varMi = _context.OgrenciDersleris
            .FirstOrDefault(x => x.OgrenciId == OgrenciId && x.DersId == ders.DersId);

        if (varMi == null)
        {
            _context.OgrenciDersleris.Add(new OgrenciDersleri
            {
                OgrenciId = OgrenciId,
                DersId = ders.DersId
            });

            _context.SaveChanges();
            TempData["Mesaj"] = "Ders başarıyla atandı.";
        }
        else
        {
            TempData["Mesaj"] = "Bu ders zaten atanmış.";
        }

        return RedirectToAction("DersAta");
    }

    public IActionResult AlinanDerslerPdf()
    {
        int? ogrenciId = HttpContext.Session.GetInt32("OgrenciId");
        if (ogrenciId == null)
            return RedirectToAction("OgrenciGiris");

        // 1. Sadece bu öğrenciye ait dersleri al
        var dersler = _context.OgrenciDersleris
            .Include(od => od.Ogrenci)
            .Include(od => od.Ders)
                .ThenInclude(d => d.Ogretmen)
            .Where(od => od.OgrenciId == ogrenciId) //  filtre burası
            .ToList();

        // 2. ViewModel dönüşümü
        var viewModelList = dersler.Select(od => new AlinanDersViewModel
        {
            OgrenciAdSoyad = $"{od.Ogrenci?.Ad} {od.Ogrenci?.Soyad}",
            OgretmenAdSoyad = $"{od.Ders?.Ogretmen?.Ad} {od.Ders?.Ogretmen?.Soyad} ({od.Ders?.DersAdi})",
            Not1 = od.Not1,
            Not2 = od.Not2
        }).ToList();

        // 3. HTML içeriği
        var html = new StringBuilder();
        html.Append("<h2 style='text-align:center;'>Alinan Dersler Raporu</h2>");
        html.Append("<table border='1' cellpadding='8' cellspacing='0' width='100%' style='border-collapse:collapse;'>");
        html.Append("<thead style='background-color:#e0e0e0;'><tr>");
        html.Append("<th>#</th><th>Ogrenci</th><th>Ogretmen (Ders)</th><th>1. Not</th><th>2. Not</th>");
        html.Append("</tr></thead><tbody>");

        int sira = 1;
        foreach (var item in viewModelList)
        {
            html.Append("<tr>");
            html.Append($"<td>{sira++}</td>");
            html.Append($"<td>{item.OgrenciAdSoyad}</td>");
            html.Append($"<td>{item.OgretmenAdSoyad}</td>");
            html.Append($"<td>{item.Not1}</td>");
            html.Append($"<td>{item.Not2}</td>");
            html.Append("</tr>");
        }

        html.Append("</tbody></table>");

        // 4. PDF oluştur
        var doc = new HtmlToPdfDocument()
        {
            GlobalSettings = new GlobalSettings
            {
                PaperSize = PaperKind.A4,
                Orientation = Orientation.Portrait,
                DocumentTitle = "Alınan Dersler"
            },
            Objects = {
            new ObjectSettings
            {
                HtmlContent = html.ToString()
            }
        }
        };

        var pdf = _converter.Convert(doc);
        return File(pdf, "application/pdf", "AlinanDersler.pdf");
    }
    




    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
