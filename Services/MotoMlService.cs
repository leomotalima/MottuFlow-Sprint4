using Microsoft.ML;
using Microsoft.ML.Data;
using MottuFlowApi.Models; 
using System;
using System.IO;


public class MotoMlService
{
    private readonly MLContext _mlContext;
    private readonly ITransformer _model; 

    private static readonly string _dataPath = Path.Combine("Scripts", "ml.csv");

    public MotoMlService(MLContext mlContext)
    {
        _mlContext = mlContext;
        _model = TrainModel();
    }

    public ManutencaoPredicao Predict(MotoData input)
    {
        var predictionEngine = _mlContext.Model.CreatePredictionEngine<MotoData, ManutencaoPredicao>(_model);
        return predictionEngine.Predict(input);
    }

    private ITransformer TrainModel()
    {
        if (!File.Exists(_dataPath))
            throw new FileNotFoundException($"Arquivo de dados não encontrado em: {_dataPath}");
        
        var dataView = _mlContext.Data.LoadFromTextFile<MotoData>(
            path: _dataPath,
            hasHeader: true,
            separatorChar: ',');

        var pipeline = _mlContext.Transforms
            .Concatenate("Features",
                nameof(MotoData.Vibracao),
                nameof(MotoData.TemperaturaMotor),
                nameof(MotoData.KMRodados),
                nameof(MotoData.IdadeOleoDias))
            .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                labelColumnName: "Label",
                featureColumnName: "Features"));

        return pipeline.Fit(dataView);
    }
}
