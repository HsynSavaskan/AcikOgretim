using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hüseyin.Models;

[Table("Duyurular")]
public partial class Duyurular
{
    [Key]
    public int DuyuruId { get; set; }

    [Column("ogretmenId")]
    public int OgretmenId { get; set; }

    [StringLength(500)]
    public string DuyuruMetin { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime EklenmeTarihi { get; set; }

    [ForeignKey("OgretmenId")]
    [InverseProperty("Duyurulars")]
    public virtual Ogretmenler Ogretmen { get; set; } = null!;
}
