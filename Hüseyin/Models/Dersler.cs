using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hüseyin.Models;

[Table("Dersler")]
public partial class Dersler
{
    [Key]
    [Column("dersId")]
    public int DersId { get; set; }

    [Column("dersAdi")]
    [StringLength(50)]
    public string? DersAdi { get; set; }

    [Column("ogretmenId")]
    public int? OgretmenId { get; set; }

    [InverseProperty("Ders")]
    public virtual ICollection<OgrenciDersleri> OgrenciDersleris { get; set; } = new List<OgrenciDersleri>();

    [ForeignKey("OgretmenId")]
    [InverseProperty("Derslers")]
    public virtual Ogretmenler? Ogretmen { get; set; }
}
