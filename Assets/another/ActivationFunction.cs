using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using nn;
using nn.ActivationFunctions;

public class ActivationFunction : MonoBehaviour
{
	public ActivationFunctionType	FunctionType;
    public enum ActivationFunctionType
	{
		identity,
		sigmoid,
		relu,
		tanh,
		aaa
	}

	public IActivationFunction GetActivationFunction()
	{
		switch( FunctionType )
		{
			case ActivationFunctionType.identity:	return new Identity();
			case ActivationFunctionType.sigmoid:	return new Sigmoid();
			case ActivationFunctionType.relu:		return new ReLU();
			case ActivationFunctionType.tanh:		return new Tanh();
			case ActivationFunctionType.aaa:		return new aaa();
		}
		return null;
	}
}
