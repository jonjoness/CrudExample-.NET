using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Text;
using Entities;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.EntityFrameworkCore;
namespace Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly PersonDbContext _db;
        public PersonsRepository(PersonDbContext db) {
            _db = db;
        }
        public async Task<Person> AddPerson(Person person)
        {
             _db.Persons.Add(person);
            await _db.SaveChangesAsync();
            return person;
        }

        public async Task<bool> DeletePersonByPersonId(Guid personId)
        {
            _db.Persons.RemoveRange(_db.Persons.Where(temp => temp.PersonID == personId));
            int rowDeleted=await _db.SaveChangesAsync();
            return rowDeleted>0;
        }

        public async Task<List<Person>> GetAllPersons()
        {
            return await _db.Persons.Include("Country").ToListAsync();
        }

        public async Task<List<Person>> GetFillteredPersons(Expression<Func<Person, bool>> predicate)
        {
            return await _db.Persons.Include("Country").Where(predicate).ToListAsync();
        }

        public async Task<Person?> GetPersonByPersonId(Guid personId)
        {
            return await _db.Persons.Include("Country").FirstOrDefaultAsync(temp => temp.PersonID == personId);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            Person? p = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonID == person.PersonID);
            if (p == null)
            {
                return null;
            }
           // return p;
            
            p.ReceiveNewsLetters = person.ReceiveNewsLetters;
            p.Gender = person.Gender;
            p.Address = person.Address;
            p.Email = person.Email;
            p.PersonName = person.PersonName;
            p.DateOfBirth = person.DateOfBirth;
            p.CountryID = person.CountryID;
            await _db.SaveChangesAsync();
            return p;
        }
    }
}
