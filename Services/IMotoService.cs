using System.Collections.Generic;
using System.Threading.Tasks;
using MottuFlowApi.Models;

namespace MottuFlowApi.Services
{
    public interface IMotoService
    {
        Task<PagedResult<Moto>> GetPagedAsync(int pageNumber, int pageSize);
        // outros métodos do serviço...
    }
}



