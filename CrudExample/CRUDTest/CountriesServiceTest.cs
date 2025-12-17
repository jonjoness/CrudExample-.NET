using AutoFixture;
using Azure.Core;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Xunit;
using AutoFixture.Kernel;
namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;
        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly ICountriesRepository _countriesRepository;
        private readonly IFixture _fixture;
        //constructor
        public CountriesServiceTest()
        {
          //  var countriesInitialData = new List<Country>() {};
           // DbContextMock<PersonDbContext> dbContextMock = new DbContextMock<PersonDbContext>(new DbContextOptionsBuilder<PersonDbContext>().Options);
           // PersonDbContext dbContext = dbContextMock.Object;
           // dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
           // _countriesService = new CountriesService(null);

            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;
            _countriesService = new CountriesService(_countriesRepository);
            _fixture = new Fixture();
        }

        #region AddCountry
        //When CountryAddRequest is null, it should throw ArgumentNullException
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;

            //Assert
          // await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            //{
                //Act
              //  await _countriesService.AddCountry(request);
            //});
            Func<Task> action = (async () =>
            {
                await _countriesService.AddCountry(request);
            });
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        //When the CountryName is null, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            //Arrange
            //CountryAddRequest? request = new CountryAddRequest() { CountryName = null };
            CountryAddRequest request = _fixture.Build<CountryAddRequest>().With(temp => temp.CountryName, null as string).Create();
            Country country = _fixture.Build<Country>().With(temp => temp.Persons, null as List<Person>).Create();
            _countriesRepositoryMock.Setup(temp => temp.AddCountrty(It.IsAny<Country>())).ReturnsAsync(country);
            //Assert
           // await Assert.ThrowsAsync<ArgumentException>(async () =>
           // {
                //Act
            //    await _countriesService.AddCountry(request);
           // });
            Func<Task> action = (async () =>
            {
                await _countriesService.AddCountry(request);
            });
            await action.Should().ThrowAsync<ArgumentException>();
        }


        //When the CountryName is duplicate, it should throw ArgumentException
        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            //Arrange
            CountryAddRequest? request1 = _fixture.Build<CountryAddRequest>().With(temp => temp.CountryName, "USA").Create();
            CountryAddRequest? request2 = _fixture.Build<CountryAddRequest>().With(temp => temp.CountryName, "USA").Create();

            //Assert
            //     await Assert.ThrowsAsync<ArgumentException>(async () =>
            //   {
            //Act
            //    await _countriesService.AddCountry(request1);
            //     await _countriesService.AddCountry(request2);
            // });
            Country country1= request1.ToCountry();
            //Country country2 = request2.ToCountry();
            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(null as Country);
            _countriesRepositoryMock.Setup(temp => temp.AddCountrty(It.IsAny<Country>())).ReturnsAsync(country1);
            
            CountryResponse c1 = await _countriesService.AddCountry(request1);
            Func<Task> action2 = (async () =>
            {
                _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(country1);
                _countriesRepositoryMock.Setup(temp => temp.AddCountrty(It.IsAny<Country>())).ReturnsAsync(country1);
                await _countriesService.AddCountry(request2);
            });
            
            await action2.Should().ThrowAsync<ArgumentException>();
        }


        //When you supply proper country name, it should insert (add) the country to the existing list of countries
        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest() { CountryName = "Japan" };
            Country country  = request.ToCountry();
            CountryResponse response = country.ToCountryResponse();
            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(null as Country);
            _countriesRepositoryMock.Setup(temp => temp.AddCountrty(It.IsAny<Country>())).ReturnsAsync(country);
 
            //Act
            CountryResponse response1 = await _countriesService.AddCountry(request);
           // List<CountryResponse> countries_from_GetAllCountries = await _countriesService.GetAllCountries();
           country.CountryID=response1.CountryID;
            response.CountryID= response1.CountryID;
            //Assert
            //   Assert.True(response.CountryID != Guid.Empty);
            response1.CountryID.Should().NotBe(Guid.Empty);
         //   Assert.Contains(response, countries_from_GetAllCountries);
            response1.Should().BeEquivalentTo(response); 
        }

        #endregion


        #region GetAllCountries

        [Fact]
        //The list of countries should be empty by default (before adding any countries)
        public async Task GetAllCountries_EmptyList()
        {
            //Act
            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(new List<Country>());
            List<CountryResponse> actual_country_response_list =await _countriesService.GetAllCountries();

            //Assert
            //Assert.Empty(actual_country_response_list);
            actual_country_response_list.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllCountries_AddFewCountries()
        {
            //Arrange
            List<Country> country_list = new List<Country>() {
        _fixture.Build<Country>()
        .With(temp => temp.Persons, null as List<Person>).Create(),
        _fixture.Build<Country>()
        .With(temp => temp.Persons, null as List<Person>).Create()
      };

            List<CountryResponse> country_response_list = country_list.Select(temp => temp.ToCountryResponse()).ToList();

            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(country_list);

            //Act
            List<CountryResponse> actualCountryResponseList = await _countriesService.GetAllCountries();

            //Assert
            actualCountryResponseList.Should().BeEquivalentTo(country_response_list);
        }
        #endregion


        #region GetCountryByCountryID

        [Fact]
        //If we supply null as CountryID, it should return null as CountryResponse
        public async Task GetCountryByCountryID_NullCountryID()
        {
            //Arrange
            Guid? countrID = null;

            //Act
            CountryResponse? country_response_from_get_method =await _countriesService.GetCountryByCountryID(countrID);


            //Assert
          //  Assert.Null(country_response_from_get_method);
            country_response_from_get_method.Should().BeNull();
        }


        [Fact]
        //If we supply a valid country id, it should return the matching country details as CountryResponse object
        public async Task GetCountryByCountryID_ValidCountryID()
        {
            //Arrange
           Country country= _fixture.Build<Country>()
           .With(temp => temp.Persons, null as List<Person>)
           .Create();
            CountryResponse country_response_from_add = country.ToCountryResponse();
            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryId(It.IsAny<Guid>())).ReturnsAsync(country);

            //Act
            CountryResponse? country_response_from_get =await  _countriesService.GetCountryByCountryID(country_response_from_add.CountryID);
          //  country_response_from_add.CountryID = country_response_from_get.CountryID;
            //Assert
            //   Assert.Equal(country_response_from_add, country_response_from_get);
            country_response_from_get.Should().Be(country_response_from_add);
        }
        #endregion
    }
}