using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
namespace Repositories
{
    public class CountriesRepository : ICountriesRepository
    {
        private readonly PersonDbContext _db;
        public CountriesRepository(PersonDbContext db)
        {
            _db = db;
        }

        public async Task<Country> AddCountrty(Country country)
        {
            _db.Countries.Add(country);
            await _db.SaveChangesAsync();
            return country;
        }

        public async Task<List<Country>> GetAllCountries()
        {
            var result=await _db.Countries.ToListAsync();
            return result;
        }

        public async Task<Country?> GetCountryByCountryId(Guid countryId)
        {
           return await  _db.Countries.FirstOrDefaultAsync(temp => temp.CountryID == countryId);
        }

        public async Task<Country?> GetCountryByCountryName(string countryName)
        {
            return await _db.Countries.FirstOrDefaultAsync(temp => temp.CountryName == countryName);
        }
    }
}
