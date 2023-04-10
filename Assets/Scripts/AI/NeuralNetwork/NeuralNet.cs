using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;

namespace AI.NeuralNetwork
{
    [Serializable]
    public class NeuralNet
    {
        public string Version;
        public double LearnRate;
        public double Momentum;
        public Neurons InputLayer;
        public List<Neurons> HiddenLayers;
        public Neurons OutputLayer;

        private static readonly System.Random Random = new System.Random();

        public NeuralNet(int inputSize, int hiddenSize, int outputSize, int numHiddenLayers = 1,
            double? learnRate = null, double? momentum = null)
        {
            LearnRate = learnRate ?? .4;
            Momentum = momentum ?? .9;
            InputLayer = new Neurons();
            HiddenLayers = new List<Neurons>();
            OutputLayer = new Neurons();

            for (var i = 0; i < inputSize; i++)
                InputLayer.Add(new Neuron());

            for (int i = 0; i < numHiddenLayers; i++)
            {
                HiddenLayers.Add(new Neurons());
                for (var j = 0; j < hiddenSize; j++)
                    HiddenLayers[i].Add(new Neuron(i == 0 ? InputLayer : HiddenLayers[i - 1]));
            }

            for (var i = 0; i < outputSize; i++)
                OutputLayer.Add(new Neuron(HiddenLayers[numHiddenLayers - 1]));
        }

        public void Train(List<DataRow> dataSets, int numEpochs)
        {
            for (var i = 0; i < numEpochs; i++)
            {
                foreach (var dataSet in dataSets)
                {
                    ForwardPropagate(dataSet.Values);
                    BackPropagate(dataSet.Targets);
                }
            }
        }

        public void Train(List<DataRow> dataSets, double minimumError, int epochLimit = 10_000, bool silent = false)
        {
            var error = 1.0;
            var numEpochs = 0;

            while (error > minimumError && numEpochs < epochLimit)
            {
                var errors = new List<double>();
                foreach (var dataSet in dataSets)
                {
                    ForwardPropagate(dataSet.Values);
                    BackPropagate(dataSet.Targets);
                    errors.Add(CalculateError(dataSet.Targets));
                }

                error = errors.Average();
                numEpochs++;
            }

            if (error > minimumError)
            {
                string msg = $"Could not reach minimumError {minimumError} (got {error})";
                if (silent)
                    Debug.LogWarning(msg);
                else
                    throw new Exception(msg);
            }
        }

        private void ForwardPropagate(params double[] inputs)
        {
            var i = 0;
            InputLayer.ForEach(a => a.Value = inputs[i++]);
            foreach (var layer in HiddenLayers)
                layer.ForEach(a => a.CalculateValue());
            OutputLayer.ForEach(a => a.CalculateValue());
        }

        private void BackPropagate(params double[] targets)
        {
            var i = 0;
            OutputLayer.ForEach(a => a.CalculateGradient(targets[i++]));
            foreach (var layer in HiddenLayers.AsEnumerable().Reverse())
            {
                layer.ForEach(a => a.CalculateGradient());
                layer.ForEach(a => a.UpdateWeights(LearnRate, Momentum));
            }

            OutputLayer.ForEach(a => a.UpdateWeights(LearnRate, Momentum));
        }

        public double[] Compute(params double[] inputs)
        {
            ForwardPropagate(inputs);
            return OutputLayer.lst.Select(a => a.Value).ToArray();
        }

        private double CalculateError(params double[] targets)
        {
            var i = 0;
            return OutputLayer.lst.Sum(a => Mathf.Abs((float) a.CalculateError(targets[i++])));
        }

        public static double GetRandom()
        {
            return 2 * Random.NextDouble() - 1;
        }
    }

    public enum TrainingType
    {
        Epoch,
        MinimumError
    }
}