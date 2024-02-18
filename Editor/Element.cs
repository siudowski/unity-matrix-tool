using UnityEngine;

namespace Siudowski.MatrixEditor
{
	/// <summary>
	/// Very simple class that contains only name of an element, to be used with <c>MatrixData</c> matrices.
	/// </summary>
	[System.Serializable]
	public class Element
	{
		[SerializeField] string name;
		public string Name => name;
	}
}