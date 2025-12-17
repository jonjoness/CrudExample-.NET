using Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace RepositoryContracts
{
   public interface IPersonsRepository
    {
        Task<Person> AddPerson(Person person);
        Task<List<Person>> GetAllPersons();
        Task<Person?> GetPersonByPersonId(Guid personId);
        Task<List<Person>> GetFillteredPersons(Expression<Func<Person, bool>> predicate);
        Task<bool> DeletePersonByPersonId(Guid personId);
        Task<Person> UpdatePerson(Person person);

    }
}
