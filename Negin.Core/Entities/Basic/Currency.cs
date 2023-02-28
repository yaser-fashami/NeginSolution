namespace Negin.Core.Domain.Entities.Basic;

public class Currency: BaseAuditableEntity<uint>
{
    public uint ForeignDollerRate { get; set; }
    public uint PersianDollerRate { get; set; }
    public DateTime Date { get; set; }
}
