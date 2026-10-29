using NossoLar.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NossoLar.Domain.Residentials.ValueObjects
{
    public sealed record RealEstateAgency
    {
        public string Name { get; private set; } = null!;
        public Address Address { get; private set; } = null!;
        public CNPJDocument Cnpj { get; private set; } = null!;
        public string Phone { get; private set; } = null!;

        private RealEstateAgency() { }

        public RealEstateAgency(string name, Address address, CNPJDocument cnpj, string phone)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Name is required.");

            var digits = new string((phone ?? string.Empty).Where(char.IsDigit).ToArray());
            if (digits.Length is < 10 or > 11)
                throw new DomainException("Phone must have 10 or 11 digits (with area code).");

            Name = name.Trim();
            Address = address ?? throw new DomainException("Address is required.");
            Cnpj = cnpj ?? throw new DomainException("CNPJ is required.");
            Phone = digits;
        }
    }
}
