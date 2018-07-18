﻿
using System.Collections;
using UnityEngine;

public class CoroutineWithData
{
	public Coroutine coroutine { get; private set; }
	private IEnumerator target;
	public object result;

	public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
	{
		this.target = target;
		this.coroutine = owner.StartCoroutine(Run());
	}

	private IEnumerator Run()
	{
		while (target.MoveNext())
		{
			result = target.Current;
			yield return result;
		}
	}
}
