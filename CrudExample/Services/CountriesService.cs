using Entities;
using ServiceContracts;
using ServiceContracts.DTO;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
namespace Services
{
    public class CountriesService : ICountriesService
    {
        //private field
        private readonly ICountriesRepository _coutriesRepository;

        //constructor
        public CountriesService(ICountriesRepository cr)
        {
           _coutriesRepository = cr ;
        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            //Validation: countryAddRequest parameter can't be null
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            //Validation: CountryName can't be null
            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            //Validation: CountryName can't be duplicate
            if (await _coutriesRepository.GetCountryByCountryName(countryAddRequest.CountryName)!=null)
            {
                throw new ArgumentException("Given country name already exists");
            }

            //Convert object from CountryAddRequest to Country type
            Country country = countryAddRequest.ToCountry();

            //generate CountryID
            country.CountryID = Guid.NewGuid();

            // Add country object into _countries
           // _db.Countries.Add(country);
            await _coutriesRepository.AddCountrty(country);
            //await _db.SaveChangesAsync();

            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            List<Country> l1 = await _coutriesRepository.GetAllCountries();
            return  l1.Select(country => country.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null)
                return null;

            Country? country_response_from_list = await _coutriesRepository.GetCountryByCountryId(countryID.Value);

            if (country_response_from_list == null)
                return null;

            return country_response_from_list.ToCountryResponse();
        }
    }
}