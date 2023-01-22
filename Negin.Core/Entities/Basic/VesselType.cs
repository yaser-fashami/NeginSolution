﻿
namespace Negin.Core.Domain.Entities.Basic;

public class VesselType : BaseBasicInformation<byte>
{
    public string Name { get; set; }
    public string? Description { get; set; }

    public virtual ICollection<Vessel> Vessels { get; set; }
}
