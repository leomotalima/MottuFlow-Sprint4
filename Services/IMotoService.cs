using MottuFlow.Models;
using MottuFlowApi.Helpers;

namespace MottuFlowApi.Services
{
    public interface IMotoService
    {
        Task<PagedResult<Moto>> GetPagedAsync(int page, int size);
        Task<Moto> GetByIdAsync(int id);
        Task<Moto> CreateAsync(Moto moto);
        Task<Moto> UpdateAsync(Moto moto);
        Task<bool> DeleteAsync(int id);
    }
}