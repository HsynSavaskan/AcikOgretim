using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hüseyin.Models;

[Table("Notlar")]
public partial class Notlar
{
    [Key]
    public int Id { get; set; }

    public int OgrenciId { get; set; }

    public int DersId { get; set; }

    public int? Vize { get; set; }

    public int? Final { get; set; }

    [ForeignKey("DersId")]
    [InverseProperty("Notlars")]
    public virtual Dersler Ders { get; set; } = null!;

    [ForeignKey("OgrenciId")]
    [InverseProperty("Notlars")]
    public virtual Ogrenciler Ogrenci { get; set; } = null!;
}
