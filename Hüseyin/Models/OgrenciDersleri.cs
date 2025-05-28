using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hüseyin.Models;

[Table("OgrenciDersleri")]
public partial class OgrenciDersleri
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("ogrenciId")]
    public int? OgrenciId { get; set; }

    [Column("dersId")]
    public int? DersId { get; set; }

    [Column("not1")]
    public int? Not1 { get; set; }

    [Column("not2")]
    public int? Not2 { get; set; }

    [ForeignKey("DersId")]
    [InverseProperty("OgrenciDersleris")]
    public virtual Dersler? Ders { get; set; }

    [ForeignKey("OgrenciId")]
    [InverseProperty("OgrenciDersleris")]
    public virtual Ogrenciler? Ogrenci { get; set; }
}
