using NossoLar.Domain.Exceptions;

namespace NossoLar.Domain.Residentials.ValueObjects
{
    public sealed record RentalCosts
    {
        public decimal Rent { get; }
        public decimal Condominium { get; }
        public decimal Iptu { get; }
        public decimal Total => Rent + Condominium + Iptu;

        public RentalCosts(decimal rent, decimal condominium, decimal iptu)
        {
            if (rent <= 0)
                throw new DomainException("Rent value must be greater than zero.");
            if (condominium < 0)
                throw new DomainException("Condominium value cannot be negative.");
            if (iptu < 0)
                throw new DomainException("IPTU value cannot be negative.");

            Rent = Round(rent);
            Condominium = Round(condominium);
            Iptu = Round(iptu);
        }

        private static decimal Round(decimal value) =>
            decimal.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}
