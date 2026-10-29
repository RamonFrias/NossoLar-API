using NossoLar.Domain.Common;
using NossoLar.Domain.Exceptions;
using NossoLar.Domain.Residentials.Enums;
using NossoLar.Domain.Residentials.ValueObjects;

namespace NossoLar.Domain.Residentials.Entities
{
    public sealed class Residential : Entity
    {
        public Address Address { get; private set; } = null!;
        public RentalCosts RentalCosts { get; private set; } = null!;
        public ResidentialType ResidentialType { get; private set; }
        public RealEstateAgency? RealEstateAgency { get; private set; }
        public string? Notes { get; private set; }
        public Uri? Url { get; private set; }

        private Residential() { } // EF Core

        public Residential(
            Address address,
            RentalCosts rentalCosts,
            ResidentialType residentialType,
            RealEstateAgency? realEstateAgency = null,
            string? notes = null,
            string? url = null)
        {
            if (!Enum.IsDefined(residentialType))
                throw new DomainException("Invalid residential type.");
            ResidentialType = residentialType;
            ChangeAddress(address);
            ChangeRentalCosts(rentalCosts);
            ChangeAgency(realEstateAgency);
            UpdateNotes(notes);
            ChangeUrl(url);
        }

        public void ChangeAddress(Address address) =>
            Address = address ?? throw new DomainException("Address is required.");

        public void ChangeRentalCosts(RentalCosts rentalCosts) =>
            RentalCosts = rentalCosts ?? throw new DomainException("Rental costs are required.");

        public void ChangeAgency(RealEstateAgency? agency) => RealEstateAgency = agency;

        public void UpdateNotes(string? notes) =>
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();

        public void ChangeUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                Url = null;
                return;
            }

            if (!Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                throw new DomainException("Url must be a valid http(s) address.");

            Url = uri;
        }
    }
}
