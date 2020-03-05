using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReachBeyond.VariableObjects;

public class Adder : MonoBehaviour
{
	public IntConstReference addend;
	public IntReference storage;

	public void DoAdd() {
		storage.Value = storage + addend;
	}
}
