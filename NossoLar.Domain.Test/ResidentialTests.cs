using NossoLar.Domain.Exceptions;
using NossoLar.Domain.Residentials.Entities;
using NossoLar.Domain.Residentials.Enums;
using NossoLar.Domain.Residentials.ValueObjects;

namespace NossoLar.Domain.Test
{
    public class ResidentialTests
    {
        private static Address ValidAddress(string street = "Rua das Flores") =>
        new(street, "123", "Centro", "Campinas", "SP", "13010000", "Apto 12");

        private static RentalCosts ValidCosts(decimal rent = 1500m) =>
            new(rent, 300m, 100m);

        private static RealEstateAgency ValidAgency() =>
            new("Imobiliária Lar Doce Lar",
                ValidAddress("Av. Brasil"),
                new CNPJDocument("11222333000181"),
                "19999990000");

        private static Residential ValidResidential(
            RealEstateAgency? agency = null, string? notes = null, string? url = null) =>
            new(ValidAddress(), ValidCosts(), ResidentialType.Apartment, agency, notes, url);

        [Fact]
        public void Constructor_WithRequiredData_SetsProperties()
        {
            var address = ValidAddress();
            var costs = ValidCosts();

            var residential = new Residential(address, costs, ResidentialType.House);

            Assert.Equal(address, residential.Address);
            Assert.Equal(costs, residential.RentalCosts);
            Assert.Equal(ResidentialType.House, residential.ResidentialType);
        }

        [Fact]
        public void Constructor_WithoutOptionalData_LeavesOptionalsNull()
        {
            var residential = ValidResidential();

            Assert.Null(residential.RealEstateAgency);
            Assert.Null(residential.Notes);
            Assert.Null(residential.Url);
        }

        [Fact]
        public void Constructor_WithAllData_SetsOptionals()
        {
            var agency = ValidAgency();

            var residential = ValidResidential(agency, "Perto do mercado", "https://www.site.com/imovel/1");

            Assert.Equal(agency, residential.RealEstateAgency);
            Assert.Equal("Perto do mercado", residential.Notes);
            Assert.Equal(new Uri("https://www.site.com/imovel/1"), residential.Url);
        }

        [Fact]
        public void Constructor_GeneratesId()
        {
            var residential = ValidResidential();

            Assert.NotEqual(Guid.Empty, residential.Id);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(99)]
        [InlineData(-1)]
        public void Constructor_WithInvalidType_Throws(int type)
        {
            var ex = Assert.Throws<DomainException>(() =>
                new Residential(ValidAddress(), ValidCosts(), (ResidentialType)type));

            Assert.Equal("Invalid residential type.", ex.Message);
        }

        [Fact]
        public void Constructor_WithNullAddress_Throws()
        {
            var ex = Assert.Throws<DomainException>(() =>
                new Residential(null!, ValidCosts(), ResidentialType.Apartment));

            Assert.Equal("Address is required.", ex.Message);
        }

        [Fact]
        public void Constructor_WithNullRentalCosts_Throws()
        {
            var ex = Assert.Throws<DomainException>(() =>
                new Residential(ValidAddress(), null!, ResidentialType.Apartment));

            Assert.Equal("Rental costs are required.", ex.Message);
        }

        [Fact]
        public void Constructor_WithInvalidUrl_Throws()
        {
            Assert.Throws<DomainException>(() => ValidResidential(url: "not a url"));
        }

        [Fact]
        public void ChangeAddress_WithValidAddress_ReplacesAddress()
        {
            var residential = ValidResidential();
            var newAddress = ValidAddress("Rua Nova");

            residential.ChangeAddress(newAddress);

            Assert.Equal(newAddress, residential.Address);
        }

        [Fact]
        public void ChangeAddress_WithNull_ThrowsAndKeepsCurrentAddress()
        {
            var residential = ValidResidential();
            var current = residential.Address;

            Assert.Throws<DomainException>(() => residential.ChangeAddress(null!));
            Assert.Equal(current, residential.Address);
        }

        [Fact]
        public void ChangeRentalCosts_WithValidCosts_ReplacesCosts()
        {
            var residential = ValidResidential();
            var newCosts = ValidCosts(rent: 1800m);

            residential.ChangeRentalCosts(newCosts);

            Assert.Equal(newCosts, residential.RentalCosts);
            Assert.Equal(2200m, residential.RentalCosts.Total);
        }

        [Fact]
        public void ChangeRentalCosts_WithNull_ThrowsAndKeepsCurrentCosts()
        {
            var residential = ValidResidential();
            var current = residential.RentalCosts;

            Assert.Throws<DomainException>(() => residential.ChangeRentalCosts(null!));
            Assert.Equal(current, residential.RentalCosts);
        }

        [Fact]
        public void ChangeAgency_WithAgency_SetsAgency()
        {
            var residential = ValidResidential();
            var agency = ValidAgency();

            residential.ChangeAgency(agency);

            Assert.Equal(agency, residential.RealEstateAgency);
        }

        [Fact]
        public void ChangeAgency_WithNull_RemovesAgency()
        {
            var residential = ValidResidential(agency: ValidAgency());

            residential.ChangeAgency(null);

            Assert.Null(residential.RealEstateAgency);
        }

        [Fact]
        public void UpdateNotes_TrimsText()
        {
            var residential = ValidResidential();

            residential.UpdateNotes("   Aceita pet   ");

            Assert.Equal("Aceita pet", residential.Notes);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void UpdateNotes_WithEmptyText_ClearsNotes(string? notes)
        {
            var residential = ValidResidential(notes: "Nota antiga");

            residential.UpdateNotes(notes);

            Assert.Null(residential.Notes);
        }


        [Theory]
        [InlineData("https://www.vivareal.com.br/imovel/123")]
        [InlineData("http://site.com.br")]
        public void ChangeUrl_WithHttpOrHttps_SetsUrl(string url)
        {
            var residential = ValidResidential();

            residential.ChangeUrl(url);

            Assert.Equal(new Uri(url), residential.Url);
        }

        [Fact]
        public void ChangeUrl_TrimsSpaces()
        {
            var residential = ValidResidential();

            residential.ChangeUrl("  https://site.com.br/imovel  ");

            Assert.Equal(new Uri("https://site.com.br/imovel"), residential.Url);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ChangeUrl_WithEmptyValue_ClearsUrl(string? url)
        {
            var residential = ValidResidential(url: "https://site.com.br");

            residential.ChangeUrl(url);

            Assert.Null(residential.Url);
        }

        [Theory]
        [InlineData("not a url")]
        [InlineData("www.site.com.br")]
        [InlineData("ftp://site.com.br")]
        [InlineData("mailto:corretor@site.com")]
        [InlineData("/imovel/123")]
        public void ChangeUrl_WithInvalidValue_Throws(string url)
        {
            var residential = ValidResidential();

            var ex = Assert.Throws<DomainException>(() => residential.ChangeUrl(url));

            Assert.Equal("Url must be a valid http(s) address.", ex.Message);
        }

        [Fact]
        public void ChangeUrl_WithInvalidValue_KeepsCurrentUrl()
        {
            var residential = ValidResidential(url: "https://site.com.br");

            Assert.Throws<DomainException>(() => residential.ChangeUrl("ftp://outro.com"));

            Assert.Equal(new Uri("https://site.com.br"), residential.Url);
        }
    }
}
