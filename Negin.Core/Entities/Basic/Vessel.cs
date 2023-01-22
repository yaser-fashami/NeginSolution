
using System.ComponentModel.DataAnnotations;

namespace Negin.Core.Domain.Entities.Basic;

public class Vessel : BaseEntity
{
    public string Name { get; set; }
    public byte VesselTypeId { get; set; }
    public VesselType? Type { get; set; }
    public int GrossTonage { get; set; }
    public string? CallSign { get; set; }
    public ushort? NationalityId { get; set; }
    public Country? Nationality { get; set; }
    public ushort? VesselLength { get; set; }
    public ushort? VesselWidth { get; set; }
    public byte? BaysQuantity { get; set; }
    public byte? ActiveCraneQty { get; set; }
    public ushort? FlagId { get; set; }
    public Country? Flag { get; set; }
    public string? Color { get; set; }
}
