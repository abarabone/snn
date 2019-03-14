using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using nn;

public class ActivationFunction : MonoBehaviour
{
	public ActivationFunctionType	FunctionType;
    public enum ActivationFunctionType
	{
		identity,
		sigmoid,
		relu,
		tanh,
		softmax
		relu,
		aaa
	}

	public NeuronUnit.IActivationFunction GetActivationFunction()
	{
		switch( FunctionType )
		{
			case ActivationFunctionType.identity:	return new NeuronUnit.Identity();
			case ActivationFunctionType.sigmoid:	return new NeuronUnit.Sigmoid();
			case ActivationFunctionType.relu:		return new NeuronUnit.ReLU();
			case ActivationFunctionType.tanh:		return new NeuronUnit.Tanh();
			case ActivationFunctionType.softmax:	return new NeuronUnit.SoftMax();
			case ActivationFunctionType.aaa:		return new NeuronUnit.aaa();
		}
		return null;
	}
}
