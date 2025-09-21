// MotoInputDTO.cs
using System;

namespace MottuFlow.DTOs
{
    public class MotoInputDTO
    {
        public required string Placa { get; set; }
        public required string Modelo { get; set; }
        public required string Fabricante { get; set; }
        public int Ano { get; set; }
        public int IdPatio { get; set; }
        public required string LocalizacaoAtual { get; set; }
    }
}




