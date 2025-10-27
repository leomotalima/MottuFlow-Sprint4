using Microsoft.ML;
using MottuFlowApi.Models;

namespace MottuFlowApi.Services
{
    public class MotoMlService
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _modelo;

        public MotoMlService()
        {
            _mlContext = new MLContext();

            // Dados de exemplo para o treinamento do modelo
            var dadosTreinamento = new List<MotoData>
            {
                new MotoData { Quilometragem = 1000, TempoUsoMeses = 2, PrecisaManutencao = false },
                new MotoData { Quilometragem = 5000, TempoUsoMeses = 6, PrecisaManutencao = true },
                new MotoData { Quilometragem = 3000, TempoUsoMeses = 3, PrecisaManutencao = false },
                new MotoData { Quilometragem = 8000, TempoUsoMeses = 10, PrecisaManutencao = true },
                new MotoData { Quilometragem = 12000, TempoUsoMeses = 12, PrecisaManutencao = true }
            };

            // Carrega os dados em memória
            var dados = _mlContext.Data.LoadFromEnumerable(dadosTreinamento);

            // Cria o pipeline de transformação + modelo
            var pipeline = _mlContext.Transforms.Concatenate("Features", new[] { "Quilometragem", "TempoUsoMeses" })
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "PrecisaManutencao"));

            // Treina o modelo
            _modelo = pipeline.Fit(dados);
        }

        // Método que faz a predição
        public MotoPrediction Prever(float quilometragem, float tempoUsoMeses)
        {
            var dadosEntrada = new MotoData { Quilometragem = quilometragem, TempoUsoMeses = tempoUsoMeses };

            var engine = _mlContext.Model.CreatePredictionEngine<MotoData, MotoPredictionInternal>(_modelo);
            var resultado = engine.Predict(dadosEntrada);

            return new MotoPrediction
            {
                Predicao = resultado.PrecisaManutencao,
                Probabilidade = resultado.Probability
            };
        }

        // Classe interna usada apenas pelo ML.NET
        private class MotoPredictionInternal
        {
            public bool PrecisaManutencao { get; set; }
            public float Probability { get; set; }
        }
    }
}
