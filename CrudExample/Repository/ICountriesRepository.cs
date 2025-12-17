using Entities;

namespace RepositoryContracts
{
    public interface ICountriesRepository
    {
        Task<Country> AddCountrty(Country country);
        Task<List<Country>> GetAllCountries();
        Task<Country?> GetCountryByCountryId(Guid countryId);
        Task<Country?> GetCountryByCountryName(string countryName);
    }
}
