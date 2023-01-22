

namespace Negin.Core.Domain.Entities.Basic
{
	public class Port: BaseBasicInformation<uint>
	{
		public ushort? CountryId { get; set; }
		public Country? Country { get; set; }
		public string PortName { get; set; }
		public string? PortEnglishName { get; set; }
		public string PortSymbol { get; set;}
	}
}
