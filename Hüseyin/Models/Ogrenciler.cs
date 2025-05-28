using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hüseyin.Models;

[Table("Ogrenciler")]
public partial class Ogrenciler
{
    [Key]
    [Column("ogrenciId")]
    public int OgrenciId { get; set; }

    [Column("ad")]
    [StringLength(50)]
    public string? Ad { get; set; }

    [Column("soyad")]
    [StringLength(50)]
    public string? Soyad { get; set; }

    [Column("email")]
    [StringLength(50)]
    public string? Email { get; set; }

    [Column("sifre")]
    [StringLength(50)]
    public string? Sifre { get; set; }

    [InverseProperty("Ogrenci")]
    public virtual ICollection<OgrenciDersleri> OgrenciDersleris { get; set; } = new List<OgrenciDersleri>();
}
