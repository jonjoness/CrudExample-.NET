using Entities;
using EntityFrameworkCoreMock;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using AutoFixture;
using FluentAssertions;
using AutoFixture.Kernel;
using Moq;
using RepositoryContracts;
using System.Linq.Expressions;
namespace CRUDTests
{
    public class PersonsServiceTest
    {
        //private fields
        private readonly IPersonsService _personService;
        //  private readonly ICountriesService _countriesService;

        private readonly Mock<IPersonsRepository> _personsRepositoryMock;




        private readonly IPersonsRepository _personsRepository;

        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;

        //constructor
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();
            _testOutputHelper = testOutputHelper;
            

         //   var countriesInitialData = new List<Country>(){};
           // var personsInitialData = new List<Person>(){};
            //Create Mock for DbContext

          //  DbContextMock<PersonDbContext> dbContextMock = new DbContextMock<PersonDbContext>(new DbContextOptionsBuilder<PersonDbContext>().Options);
            //Access Mock DbContext Object

         //   PersonDbContext dbContext = dbContextMock.Object;
            
         //   dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
          //  dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);
          //  _countriesService = new CountriesService(null);
          //  _personService = new PersonsService(null);

            //Create Mock for Persons Repository
            _personsRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personsRepositoryMock.Object;
           // _countriesService = new CountriesService();
            _personService = new PersonsService(_personsRepository);

        }

        #region AddPerson

        //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            //Act

            Func<Task> action=(async () =>
            {
                await _personService.AddPerson(personAddRequest);
            });
           await action.Should().ThrowAsync<ArgumentNullException>();
            //  await Assert.ThrowsAsync<ArgumentNullException>(async () =>
           // {
             //   await _personService.AddPerson(personAddRequest);
            //});
        }


        //When we supply null value as PersonName, it should throw ArgumentException
        [Fact]
        public async Task AddPerson_PersonNameIsNull()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.PersonName, null as string).Create();

            Person person = personAddRequest.ToPerson();
            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);
            //Act
            Func<Task> action = (async ()=>
            {
                await _personService.AddPerson(personAddRequest);
            });
           await action.Should().ThrowAsync<ArgumentException>();
        }

        //When we supply proper person details, it should insert the person into the persons list; and it should return an object of PersonResponse, which includes with the newly generated person id
        [Fact]
        public async Task AddPerson_ProperPersonDetails()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>().With(temp=>temp.Email,"someone@gamil.com").Create();

            Person person = personAddRequest.ToPerson();
            PersonResponse personResponse = person.ToPersonResponse();
            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);
            //Act
            PersonResponse person_response_from_add = await _personService.AddPerson(personAddRequest);

           // List<PersonResponse> persons_list = await _personService.GetAllPersons();
           personResponse.PersonID = person_response_from_add.PersonID;
            //Assert
            // Assert.True(person_response_from_add.PersonID != Guid.Empty);
            person_response_from_add.PersonID.Should().NotBe(Guid.Empty);
            // Assert.Contains(person_response_from_add, persons_list);
            person_response_from_add.Should().Be(personResponse);
        }

        #endregion


        #region GetPersonByPersonID

        //If we supply null as PersonID, it should return null as PersonResponse
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID()
        {
            //Arrange
            Guid? personID = null;

            //Act
            PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(personID);

            //Assert
         //   Assert.Null(person_response_from_get);
            person_response_from_get.Should().BeNull();
        }


        //If we supply a valid person id, it should return the valid person details as PersonResponse object
        [Fact]
        public async Task GetPersonByPersonID_WithPersonID()
        {
            //Arange
    //        CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();
  //          CountryResponse country_response = await _countriesService.AddCountry(country_request);

//            PersonAddRequest person_request = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "someone@example.com").Create();

        //    PersonResponse person_response_from_add =await _personService.AddPerson(person_request);

        //    PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(person_response_from_add.PersonID);

            //Assert
            //Assert.Equal(person_response_from_add, person_response_from_get);
            Person person = _fixture.Build<Person>().With(temp => temp.Email, "someone@gamil.com").With(temp=>temp.Country,null  as Country).Create();
            PersonResponse? person_response1 = person.ToPersonResponse();
             _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(person);
            PersonResponse? person_response2 = await _personService.GetPersonByPersonID(person.PersonID);
            person_response2.Should().Be(person_response1);
           
        }

        #endregion


        #region GetAllPersons

        //The GetAllPersons() should return an empty list by default
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(new List<Person>());
            //Act
            List<PersonResponse> persons_from_get =await  _personService.GetAllPersons();

            //Assert
            //Assert.Empty(persons_from_get);
            persons_from_get.Should().BeEmpty();
        }


        //First, we will add few persons; and then when we call GetAllPersons(), it should return the same persons that were added
        [Fact]
        public async Task GetAllPersons_AddFewPersons()
        {
            //Arrange


            List<Person> persons = new List<Person>() {
            _fixture.Build<Person>().With(temp => temp.Email, "someone_1@gamil.com").With(temp=>temp.Country,null as Country).Create(),_fixture.Build<Person>().With(temp => temp.Email, "someone_2@gamil.com").With(temp=>temp.Country,null as Country).Create(),_fixture.Build<Person>().With(temp => temp.Email, "someone_3@gamil.com").With(temp=>temp.Country,null as Country).Create()};
           
            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

           
            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }
            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);
            //Act
            List<PersonResponse> persons_list_from_get = await _personService.GetAllPersons();

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_get)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
          //  foreach (PersonResponse person_response_from_add in person_response_list_from_add)
           // {
                //Assert.Contains(person_response_from_add, persons_list_from_get);
               
           // }
            persons_list_from_get.Should().BeEquivalentTo(person_response_list_expected);
        }
        #endregion


        #region GetFilteredPersons

        //If the search text is empty and search by is "PersonName", it should return all persons
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText()
        {
            //Arrange

            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>().With(temp=>temp.Email,"someone_1@gmail.com").With(temp=>temp.Country,null as Country).Create(),_fixture.Build<Person>().With(temp=>temp.Email,"someone_2@gmail.com").With(temp=>temp.Country,null as Country).Create(),_fixture.Build<Person>().With(temp=>temp.Email,"someone_3@gmail.com").With(temp=>temp.Country,null as Country).Create()
            };
            List<PersonResponse> personResponses_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();     
          
            _personsRepositoryMock.Setup(temp=>temp.GetFillteredPersons(It.IsAny<Expression<Func<Person,bool>>>())).ReturnsAsync(persons);
            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in personResponses_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            //Act
            List<PersonResponse> persons_list_from_search =await  _personService.GetFilteredPersons(nameof(Person.PersonName), "");

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            //foreach (PersonResponse person_response_from_add in person_response_list_from_add)
           // {
            //    Assert.Contains(person_response_from_add, persons_list_from_search);
          //  }
            persons_list_from_search.Should().BeEquivalentTo(personResponses_expected);
        }


        //First we will add few persons; and then we will search based on person name with some search string. It should return the matching persons
        [Fact]
        public async Task GetFilteredPersons_SearchByPersonName()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>().With(temp=>temp.Email,"someone_1@gmail.com").With(temp=>temp.Country,null as Country).Create(),_fixture.Build<Person>().With(temp=>temp.Email,"someone_2@gmail.com").With(temp=>temp.Country,null as Country).Create(),_fixture.Build<Person>().With(temp=>temp.Email,"someone_3@gmail.com").With(temp=>temp.Country,null as Country).Create()
            };
            List<PersonResponse> personResponses_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _personsRepositoryMock.Setup(temp => temp.GetFillteredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);
            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in personResponses_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }

            //Act
            List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(nameof(Person.PersonName), "sa");

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }

            //Assert
            //foreach (PersonResponse person_response_from_add in person_response_list_from_add)
            // {
            //    Assert.Contains(person_response_from_add, persons_list_from_search);
            //  }
            persons_list_from_search.Should().BeEquivalentTo(personResponses_expected);
        }

        #endregion


        #region GetSortedPersons

        //When we sort based on PersonName in DESC, it should return persons list in descending on PersonName
        [Fact]
        public async Task GetSortedPersons()
        {
            //Arrange
            List<Person> persons = new List<Person>()
            {
                _fixture.Build<Person>().With(temp=>temp.Email,"someone_1@gmail.com").With(temp=>temp.Country,null as Country).Create(),_fixture.Build<Person>().With(temp=>temp.Email,"someone_2@gmail.com").With(temp=>temp.Country,null as Country).Create(),_fixture.Build<Person>().With(temp=>temp.Email,"someone_3@gmail.com").With(temp=>temp.Country,null as Country).Create()
            };
            List<PersonResponse> personResponses_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();
            _personsRepositoryMock.Setup(temp=>temp.GetAllPersons()).ReturnsAsync(persons);
           

            //print person_response_list_from_add
            _testOutputHelper.WriteLine("Expected:");
            foreach (PersonResponse person_response_from_add in personResponses_expected)
            {
                _testOutputHelper.WriteLine(person_response_from_add.ToString());
            }
            List<PersonResponse> allPersons = await _personService.GetAllPersons();

            //Act
            List<PersonResponse> persons_list_from_sort = await _personService.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            //print persons_list_from_get
            _testOutputHelper.WriteLine("Actual:");
            foreach (PersonResponse person_response_from_get in persons_list_from_sort)
            {
                _testOutputHelper.WriteLine(person_response_from_get.ToString());
            }
            // person_response_list_from_add = person_response_list_from_add.OrderByDescending(temp => temp.PersonName).ToList();

            //Assert
            //for (int i = 0; i < person_response_list_from_add.Count; i++)
            // {
            //   Assert.Equal(person_response_list_from_add[i], persons_list_from_sort[i]);
            // }
            persons_list_from_sort.Should().BeInDescendingOrder(temp => temp.PersonName);
            #endregion

        }
        #region UpdatePerson

        //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            //Arrange
            PersonUpdateRequest? person_update_request = null;

            //Assert
           // await Assert.ThrowsAsync<ArgumentNullException>(async () => {
            //    //Act
            //    await _personService.UpdatePerson(person_update_request);
          //  });
                Func<Task> action = (async () =>
                {
                    await _personService.UpdatePerson(person_update_request);
                });
                await action.Should().ThrowAsync<ArgumentNullException>();
        }


        //When we supply invalid person id, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_InvalidPersonID()
        {
            //Arrange
            // PersonUpdateRequest? person_update_request = new PersonUpdateRequest() { PersonID = Guid.NewGuid() };
            // Person p = _fixture.Build<Person>().With(temp => temp.Email, "someone_1@gmail.com").With(temp=>temp.Country,null as Country).With(temp=>temp.PersonID,Guid.Empty).Create();
            //Assert
            //await Assert.ThrowsAsync<ArgumentException>(async () => {
            //Act
            //    await _personService.UpdatePerson(person_update_request);
            //   });
            //PersonResponse person_response = p.ToPersonResponse();
            PersonUpdateRequest person_update_request = _fixture.Build<PersonUpdateRequest>().Create();
            Func<Task> action = (async () =>
            {
                await _personService.UpdatePerson(person_update_request);
            });
            await action.Should().ThrowAsync<ArgumentException>();
        }


        //When PersonName is null, it should throw ArgumentException
        [Fact]
        public async Task UpdatePerson_PersonNameIsNull()
        {
            //Arrange

            Person p = _fixture.Build<Person>().With(temp => temp.Email, "someone_1@gmail.com").With(temp => temp.Country, null as Country).With(temp => temp.PersonName, null as string).With(temp=>temp.Gender,"Male").Create();

            //Assert
            //  await Assert.ThrowsAsync<ArgumentException>(async () => {
            //Act
            //    await _personService.UpdatePerson(person_update_request);
            // });
            PersonResponse person_response = p.ToPersonResponse();
            PersonUpdateRequest person_update_request = person_response.ToPersonUpdateRequest();
            Func<Task> action = (async () =>
            {
                await _personService.UpdatePerson(person_update_request);
            });
            await action.Should().ThrowAsync<ArgumentException>();

        }


        //First, add a new person and try to update the person name and email
        [Fact]
        public async Task UpdatePerson_PersonFullDetailsUpdation()
        {
            //Arrange
            Person p = _fixture.Build<Person>().With(temp => temp.Email, "someone_1@gmail.com").With(temp => temp.Country, null as Country).With(temp => temp.Gender, "Male").Create();
            PersonResponse p1 = p.ToPersonResponse();
            PersonUpdateRequest p2 = p1.ToPersonUpdateRequest();
            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(p);
            _personsRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(p);
            //Act
            PersonResponse person_response_from_update = await _personService.UpdatePerson(p2);

          //  PersonResponse? person_response_from_get = await _personService.GetPersonByPersonID(person_response_from_update.PersonID);

            //Assert
          //  Assert.Equal(person_response_from_get, person_response_from_update);
            person_response_from_update.Should().Be(p1);

        }

        #endregion


        #region DeletePerson

        //If you supply an valid PersonID, it should return true
        [Fact]
        public async Task DeletePerson_ValidPersonID()
        {
            //Arrange
            Person p = _fixture.Build<Person>().With(temp => temp.Email, "someone_1@gmail.com").With(temp => temp.Country, null as Country).With(temp => temp.Gender, "Male").Create();
          //  PersonResponse p1 = p.ToPersonResponse();
            _personsRepositoryMock.Setup(temp => temp.DeletePersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(true);
            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>())).ReturnsAsync(p);
            //Act
            bool isDeleted =await  _personService.DeletePerson(p.PersonID);

            //Assert
           // Assert.True(isDeleted);
            isDeleted.Should().BeTrue();
        }


        //If you supply an invalid PersonID, it should return false
        [Fact]
        public async Task DeletePerson_InvalidPersonID()
        {
            //Act
            bool isDeleted = await _personService.DeletePerson(Guid.NewGuid());

            //Assert
           // Assert.False(isDeleted);
            isDeleted.Should().BeFalse();
        }

        #endregion
    }
}