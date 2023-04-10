using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI.NeuralNetwork
{
	[Serializable]
	public class Neuron 
	{
		public List<Synapse> InputSynapses;
		public List<Synapse> OutputSynapses;
		public double Bias;
		public double BiasDelta;
		public double Gradient;
		public double Value;

		public Neuron()
			{
				InputSynapses = new List<Synapse>();
				OutputSynapses = new List<Synapse>();
				Bias = NeuralNet.GetRandom();
			}

			public Neuron(Neurons inputNeurons) : this()
			{
				foreach (var inputNeuron in inputNeurons.lst)
				{
					var synapse = new Synapse(inputNeuron, this);
					inputNeuron.OutputSynapses.Add(synapse);
					InputSynapses.Add(synapse);
				}
			}

			public virtual double CalculateValue()
			{
				return Value = Sigmoid.Output(InputSynapses.Sum(a => a.Weight * a.InputNeuron.Value) + Bias);
			}

			public double CalculateError(double target)
			{
				return target - Value;
			}

			public double CalculateGradient(double? target = null)
			{
				if(target == null)
					return Gradient = OutputSynapses.Sum(a => a.OutputNeuron.Gradient * a.Weight) * Sigmoid.Derivative(Value);

				return Gradient = CalculateError(target.Value) * Sigmoid.Derivative(Value);
			}

			public void UpdateWeights(double learnRate, double momentum)
			{
				var prevDelta = BiasDelta;
				BiasDelta = learnRate * Gradient;
				Bias += BiasDelta + momentum * prevDelta;

				foreach (var synapse in InputSynapses)
				{
					prevDelta = synapse.WeightDelta;
					synapse.WeightDelta = learnRate * Gradient * synapse.InputNeuron.Value;
					synapse.Weight += synapse.WeightDelta + momentum * prevDelta;
				}
			}

	}

	[Serializable]
	public class Neurons
	{
		[SerializeField] private List<Neuron> neurons;

		public List<Neuron> lst => neurons;

		public Neurons()
		{
			neurons = new List<Neuron>();
		}
		
		public Neuron this[int i] => neurons[i];

		public void Add(Neuron n)
		{
			neurons.Add(n);
		}

		public void ForEach(Action<Neuron> a)
		{
			neurons.ForEach(a);
		}
	}

	[Serializable]
	public class Synapse
	{
		public Neuron InputNeuron;
		public Neuron OutputNeuron;
		public double Weight;
		public double WeightDelta;

		public Synapse(Neuron inputNeuron, Neuron outputNeuron)
		{
			InputNeuron = inputNeuron;
			OutputNeuron = outputNeuron;
			Weight = NeuralNet.GetRandom();
		}
	}

	public static class Sigmoid
	{
		public static double Output(double x)
		{
			return x < -45.0 ? 0.0 : x > 45.0 ? 1.0 : 1.0 / (1.0 + Mathf.Exp((float)-x));
		}

		public static double Derivative(double x)
		{
			return x * (1 - x);
		}
	}

	public class DataRow
	{
		public double[] Values { get; set; }
		public double[] Targets { get; set; }

		public DataRow(double[] values, double[] targets)
		{
			Values = values;
			Targets = targets;
		}
	}

}