using AutoFixture;
using CRUDExample.Controllers;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using CRUDExample;
using ServiceContracts.Enums;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using System.Security.Cryptography.Pkcs;
using Entities;
namespace CRUDTest
{
    public class PersonControllerTest
    {
        private readonly IPersonsService _personsService;
        private readonly Mock<IPersonsService> _personServiceMock;
        private readonly ICountriesService _countriesService;
        private readonly Mock<ICountriesService> _countriesServiceMock;
        private readonly IFixture _fixture;
        public PersonControllerTest()
        { 
          _personServiceMock = new Mock<IPersonsService>();
            _personsService = _personServiceMock.Object;
            _countriesServiceMock = new Mock<ICountriesService>();
            _countriesService = _countriesServiceMock.Object;
            _fixture = new Fixture();

        }
        [Fact]
        public async Task Index_shouldReturnViewResult()
        {

            List<PersonResponse> persons_response_list = _fixture.Create<List<PersonResponse>>();
            PersonsController personsController = new PersonsController(_personsService, _countriesService);
            _personServiceMock.Setup(mock => mock.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(persons_response_list);
            _personServiceMock.Setup(mock => mock.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>())).ReturnsAsync(persons_response_list);
            IActionResult result = await personsController.Index(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<SortOrderOptions>());
            ViewResult view = Assert.IsType<ViewResult>(result);
            view.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();

        }
        [Fact]
        public async Task Create_IfModelErrors_Occur()
        {
            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();
            _countriesServiceMock.Setup(mock => mock.GetAllCountries()).ReturnsAsync(countries);
            PersonAddRequest p1 = _fixture.Create<PersonAddRequest>();
            // Person p2 = p1.ToPerson();
            PersonResponse p3 = _fixture.Create<PersonResponse>();
            _personServiceMock.Setup(mock => mock.AddPerson(It.IsAny<PersonAddRequest>())).ReturnsAsync(p3);
            PersonsController controller = new PersonsController(_personsService, _countriesService);

            //Act
            controller.ModelState.AddModelError("PersonName", "Person name can't be null");
            IActionResult view = await controller.Create(p1);
            ViewResult result = Assert.IsType<ViewResult>(view);
            result.ViewData.Model.Should().BeAssignableTo<PersonAddRequest>();
            result.ViewData.Model.Should().Be(p3);
        }
    }
}
