using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace First.Domain.Services
{
    public interface IDataService<F>
    {
        Task<IEnumerable<F>> GetAll();

        Task<F> Get(int id);
        Task<F> Create(F entity);

        Task<F> Update(int id, F entity);
        Task<F> Delete(int id);
    }
}
