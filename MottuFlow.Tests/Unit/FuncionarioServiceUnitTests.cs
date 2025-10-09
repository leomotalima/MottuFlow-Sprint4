using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using MottuFlow.Services;
using MottuFlow.Models;
using MottuFlow.Repositories;

namespace MottuFlow.Tests.Unit
{
    public class FuncionarioServiceUnitTests
    {
        private readonly FuncionarioService _service;
        private readonly Mock<FuncionarioRepository> _mockRepo;

        public FuncionarioServiceUnitTests()
        {
            _mockRepo = new Mock<FuncionarioRepository>();
            _service = new FuncionarioService(_mockRepo.Object);
        }

        [Fact(DisplayName = "Deve retornar lista de funcionários")]
        public async Task ListarTodos_DeveRetornarFuncionarios()
        {
            var funcionariosMock = new List<Funcionario>
            {
                new Funcionario { Nome = "Léo Mota Lima", Cpf = "12345678900" }
            };

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(funcionariosMock);

            var resultado = await _service.ListarTodos();

            Assert.Single(resultado);
            Assert.Equal("Léo Mota Lima", resultado[0].Nome);
        }

        [Fact(DisplayName = "Deve lançar exceção se nome for vazio")]
        public async Task Adicionar_DeveLancarExcecao_SeNomeVazio()
        {
            var funcionario = new Funcionario { Nome = "" };

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.Adicionar(funcionario));
            Assert.Equal("Nome do funcionário é obrigatório.", ex.Message);
        }
    }
}
