using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hüseyin.Models;

[Table("Ogretmenler")]
public partial class Ogretmenler
{
    [Key]
    [Column("ogretmenId")]
    public int OgretmenId { get; set; }

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

    [InverseProperty("Ogretmen")]
    public virtual ICollection<Dersler> Derslers { get; set; } = new List<Dersler>();

    [InverseProperty("Ogretmen")]
    public virtual ICollection<Duyurular> Duyurulars { get; set; } = new List<Duyurular>();
}
