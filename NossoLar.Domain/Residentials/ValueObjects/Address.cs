using NossoLar.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NossoLar.Domain.Residentials.ValueObjects
{
    public class Address
    {
        public string Street { get; }
        public string Number { get; }
        public string Neighborhood { get; }
        public string City { get; }
        public string State { get; }
        public string ZipCode { get; }
        public string? Complement { get; }
        public string? ReferencePoint { get; }

        public Address(
            string street,
            string number,
            string neighborhood,
            string city,
            string state,
            string zipCode,
            string? complement = null,
            string? referencePoint = null)
        {
            Street = Required(street, "Street");
            Neighborhood = Required(neighborhood, "Neighborhood");
            City = Required(city, "City");

            var num = Required(number, "Number").ToUpperInvariant();
            Number = num is "SN" or "S/N" or "S.N." ? "S/N" : num;

            var uf = Required(state, "State").ToUpperInvariant();
            if (uf.Length != 2 || !uf.All(char.IsAsciiLetterUpper))
                throw new DomainException("State must be a 2-letter code (e.g. SP).");
            State = uf;

            var digits = new string((zipCode ?? string.Empty).Where(char.IsAsciiDigit).ToArray());
            if (digits.Length != 8)
                throw new DomainException("Zip code must have 8 digits.");
            ZipCode = digits;

            Complement = Optional(complement);
            ReferencePoint = Optional(referencePoint);
        }

        private static string Required(string? value, string field) =>
            string.IsNullOrWhiteSpace(value)
                ? throw new DomainException($"{field} is required.")
                : value.Trim();

        private static string? Optional(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        public override string ToString() =>
            $"{Street}, {Number}{(Complement is null ? "" : $" - {Complement}")} - {Neighborhood}, " +
            $"{City}/{State}, {ZipCode[..5]}-{ZipCode[5..]}";
    }
}
