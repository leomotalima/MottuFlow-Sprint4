using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MottuFlowApi.Models;
using MottuFlowApi.Repositories;

namespace MottuFlowApi.Services
{
    public class FuncionarioService
    {
        private readonly FuncionarioRepository _repository;

        public FuncionarioService(FuncionarioRepository repository)
        {
            _repository = repository;
        }

        // ✅ método esperado pelo teste
        public async Task<List<Funcionario>> ListarTodos()
        {
            return await _repository.GetAllAsync();
        }

        // ✅ método esperado pelo teste
        public async Task Adicionar(Funcionario funcionario)
        {
            if (string.IsNullOrWhiteSpace(funcionario.Nome))
                throw new Exception("Nome do funcionário é obrigatório.");

            await _repository.AddAsync(funcionario);
        }
    }
}
