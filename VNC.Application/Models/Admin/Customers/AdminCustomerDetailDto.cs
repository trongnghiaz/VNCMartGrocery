namespace VNC.Application.Models.Admin.Customers
{
    public class AdminCustomerDetailDto : AdminCustomerDto
    {
        public string? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? ZaloId { get; set; }

        public List<AdminCustomerAddressDto> Addresses { get; set; } = new();
    }

    public class AdminCustomerAddressDto
    {
        public int CustomerAddressId { get; set; }

        public string ReceiverName { get; set; } = null!;

        public string ReceiverPhone { get; set; } = null!;

        public string FullAddress { get; set; } = null!;

        public bool IsDefault { get; set; }
    }
}