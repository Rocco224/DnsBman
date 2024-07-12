using System;
using System.Collections.Generic;

namespace DnsBman.Models;

public partial class Customer : ICloneable
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? ValueBmanIt { get; set; }
    public int IdRecordBmanIt { get; set; }
    public string? ValueBmanShop { get; set; }
    public int IdRecordBmanShop { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ModificationDate { get; set; }

    public object Clone()
    {
        return new Customer
        {
            Id = this.Id,
            Name = this.Name,
            ValueBmanIt = this.ValueBmanIt,
            IdRecordBmanIt = this.IdRecordBmanIt,
            ValueBmanShop = this.ValueBmanShop,
            IdRecordBmanShop = this.IdRecordBmanShop,
            CreationDate = this.CreationDate,
            ModificationDate = this.ModificationDate
        };
    }
}
