using UnityEngine;
using System.Collections.Generic;

namespace Siudowski.MatrixEditor
{
	public enum MatrixType { Bool, Float, Int }
	
	[CreateAssetMenu(fileName = "MatrixData", menuName = "Scriptable Object/MatrixData")]
	public class MatrixData : ScriptableObject, ISerializationCallbackReceiver
	{
		[SerializeField] MatrixType type;
		[Tooltip("Elements linked together in a matrix. Please note that reordering this list will not update values in respective arrays.")]
		[SerializeField] List<Element> matrixElements;
		
		// main arrays to be worked with outside this system
		// because ScriptableObjects can't be generics, all type that could be useful to support needs a seperate matrix,
		// or I simply didn't figure out a cleaner way to do this
		private bool[,] boolMatrix = new bool[1,1];
		private float[,] floatMatrix = new float[1,1];
		private int[,] intMatrix = new int[1,1];
		
		// arrays that are actually saved (serialized); because Unity doesn't support multi-dimensional array serialization,
		// they are flattened to one-dimensional ones
		[HideInInspector] [SerializeField] bool[] serializedBoolMatrix = new bool[1];
		[HideInInspector] [SerializeField] float[] serializedFloatMatrix = new float[1];
		[HideInInspector] [SerializeField] int[] serializedIntMatrix = new int[1];
		
		// just getter properties
		public MatrixType Type => type;
		public List<Element> MatrixElements => matrixElements;
		public bool[,] BoolMatrix => boolMatrix;
		public float[,] FloatMatrix => floatMatrix;
		public int[,] IntMatrix => intMatrix;
		
		private void OnValidate()
		{
			// when dimensions are changed, arrays are resized to retain already present data instead of being replaced by newly created ones with new dimensions
			// could be irrelevant, as serialized arrays are used now
			if (boolMatrix == null || boolMatrix.Length != matrixElements.Count * matrixElements.Count)
				boolMatrix = ResizeArray<bool>(boolMatrix, matrixElements.Count, matrixElements.Count);
						
			if (floatMatrix == null || floatMatrix.Length != matrixElements.Count * matrixElements.Count)
				floatMatrix = ResizeArray<float>(floatMatrix, matrixElements.Count, matrixElements.Count);
				
			if (intMatrix == null || intMatrix.Length != matrixElements.Count * matrixElements.Count)
				intMatrix = ResizeArray<int>(intMatrix, matrixElements.Count, matrixElements.Count);
		}
		
		public void OnBeforeSerialize()
		{
			serializedBoolMatrix = To1DArray(boolMatrix);
			serializedFloatMatrix = To1DArray(floatMatrix);
			serializedIntMatrix = To1DArray(intMatrix);
		}
		
		public void OnAfterDeserialize()
		{
			boolMatrix = To2DArray(serializedBoolMatrix, matrixElements.Count);
			floatMatrix = To2DArray(serializedFloatMatrix, matrixElements.Count);
			intMatrix = To2DArray(serializedIntMatrix, matrixElements.Count);
		}
		
		/// <summary>
		/// Changes value(s) in array that matches the <c>type</c> of specified elements' indices.
		/// </summary>
		/// <param name="type">either bool, float or int</param>
		/// <param name="indexA">first dimension index in target array</param>
		/// <param name="indexB">second dimension index in target array</param>
		/// <param name="val">value to be written into array</param>
		public void UpdateValuesInMatrix<T>(MatrixType type, int indexA, int indexB, object val)
		{
			switch (type)
			{
				case MatrixType.Bool:
					UpdateValuesInMatrix<bool>(ref boolMatrix, indexA, indexB, (bool)val);
				break;
				
				case MatrixType.Float:
					UpdateValuesInMatrix<float>(ref floatMatrix, indexA, indexB, (float)val);
				break;
				
				case MatrixType.Int:
					UpdateValuesInMatrix<int>(ref intMatrix, indexA, indexB, (int)val);
				break;
			}
		}
		
		/// <summary>
		/// Checks if specified indices lie on array's diagonal, then writes <c>newValue</c> to correct array indices.
		/// </summary>
		/// <typeparam name="T">type of referenced array</typeparam>
		/// <param name="array"></param>
		/// <param name="a">first-dimension index</param>
		/// <param name="b">second-dimension index</param>
		/// <param name="val">value to be written into an array</param>
		private void UpdateValuesInMatrix<T>(ref T[,] array, int a, int b, T val)
		{
			// if true then only one index of an array corresponds to this element
			bool isDiagonal = a == matrixElements.Count - 1 - b;
			
			if (isDiagonal)
				array[a, b] = val;
			else 
			{
				array[a, b] = val;
				array[matrixElements.Count - 1 - b, matrixElements.Count - 1 - a] = val;
			}
		}
		
		/// <summary>
		/// Creates new array of specified size and writes original data to indices in new array.
		/// </summary>
		private T[,] ResizeArray<T>(T[,] input, int rows, int columns)
		{
			var output = new T[rows, columns];
			
			int minRows = System.Math.Min(rows, input.GetLength(0));
			int minColumns = System.Math.Min(columns, input.GetLength(1));
			
			for (int i = 0; i < minRows; i++)
			{
				for (int j = 0; j < minColumns; j++)
					output[i,j] = input[i,j];
			}
			
			return output;
		}
		
		/// <summary>
		/// Flattens a 2D array.
		/// </summary>
		private T[] To1DArray<T>(T[,] input)
		{
			T[] output = new T[input.Length];
			
			int write = 0;
			for (int i = 0; i <= input.GetUpperBound(0); i++)
			{
				for (int j = 0; j <= input.GetUpperBound(1); j++)
				{
					output[write++] = input[i, j];
				}
			}
			
			return output;
		}
		
		/// <summary>
		/// Creates square array out of 1D array.
		/// </summary>
		private T[,] To2DArray<T>(T[] input, int rows)
		{
			T[,] output = new T[rows, rows];
			
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < rows; j++)
				{
					output[i, j] = input[i * rows + j];
				}
			}
			
			return output;
		}
	}
}