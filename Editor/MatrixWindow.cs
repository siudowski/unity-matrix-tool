using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Siudowski.MatrixEditor
{
	public class MatrixWindow : EditorWindow
	{
		public static string WindowTitle => "Matrix editor";
		
		// below values are in pixels
		private static int totalFieldWidth;
		private static int fieldHeight;
		private static int totalContainerWidth;
		
		private const int PADDING = 2;
		private const int BOOL_FIELD_WIDTH = 20;
		private const int NUM_FIELD_WIDTH = 32;
		
		// reference to a scriptable object
		public static MatrixData matrix;

		public void CreateGUI()
		{
			if (!matrix)
				return;
			
			VisualElement root = rootVisualElement;

			// get correct amount of valid data entries
			int entryCount = 0;
			for (int i = 0; i < matrix.MatrixElements.Count; i++)
			{
				if (matrix.MatrixElements[i] != null)
					entryCount++;
				else
					Debug.Log($"Entry is null? at index {i}");
			}
			
			// calculating correct field sizes
			switch (matrix.Type)
			{
				case MatrixType.Bool:
					totalFieldWidth = BOOL_FIELD_WIDTH + PADDING * 2;
					fieldHeight = BOOL_FIELD_WIDTH;
				break;
				
				// int and float types have identical field sizes
				default:
					totalFieldWidth = NUM_FIELD_WIDTH + PADDING * 2;
					fieldHeight = BOOL_FIELD_WIDTH;
				break;
			}
			
			// total width
			totalContainerWidth = entryCount * totalFieldWidth;
			
			// scroll view allows for panning the entire window if feature matrix gets too large
			ScrollView scrollView = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
			scrollView.style.width = new Length(100, LengthUnit.Percent);
			scrollView.style.height = new Length(100, LengthUnit.Percent);
			scrollView.contentContainer.style.flexDirection = FlexDirection.Row;
			scrollView.contentContainer.style.flexWrap = Wrap.Wrap;
			root.Add(scrollView);
			
			// just a hack for flexbox to work
			VisualElement matrixWrap = new VisualElement();
			matrixWrap.style.flexDirection = FlexDirection.Row;
			matrixWrap.style.flexGrow = 1f;
			

			// matrix container with flexbox settings allows generating a grid of VisualElements
			VisualElement matrixContainer = new VisualElement();
			matrixContainer.style.width = new Length(totalContainerWidth, LengthUnit.Pixel);
			matrixContainer.contentContainer.style.flexDirection = FlexDirection.Row;
			matrixContainer.contentContainer.style.flexWrap = Wrap.Wrap;
			matrixWrap.Add(matrixContainer);

			// flexbox for top feature labels
			VisualElement topLabels = new VisualElement();
			topLabels.style.height = new Length(100, LengthUnit.Pixel);
			topLabels.style.flexDirection = FlexDirection.Row;
			topLabels.style.flexBasis = new Length(100, LengthUnit.Percent);
			topLabels.style.flexGrow = 1f;
			topLabels.style.left = new Length(100, LengthUnit.Pixel);
			topLabels.style.alignItems = Align.FlexEnd;
			scrollView.Add(topLabels);

			// flexbox for side feature labels
			VisualElement sideLabels = new VisualElement();
			sideLabels.style.width = new Length(100, LengthUnit.Pixel);
			scrollView.Add(sideLabels);
			
			// added as last child of scrollView
			scrollView.Add(matrixWrap);
			
			// top labels, generated backwards to replicate Unity's physics matrix
			for (int i = entryCount - 1; i >= 0; i--) 
			{
				Label topLabel = new Label(matrix.MatrixElements[i].Name);
				topLabel.style.rotate = new Rotate(90f);
				topLabel.style.width = new Length(totalFieldWidth, LengthUnit.Pixel);
				topLabel.style.height = new Length(totalFieldWidth, LengthUnit.Pixel);
				topLabel.style.unityTextAlign = TextAnchor.MiddleRight;
				
				topLabels.Add(topLabel);
			}

			// main loop for generating fields in a grid-like pattern
			for (int x = 0; x < entryCount; x++)
			{
				// side label for features
				Label sideLabel = new Label(matrix.MatrixElements[x].Name);
				sideLabel.style.height = new Length(fieldHeight, LengthUnit.Pixel);
				sideLabel.style.unityTextAlign = TextAnchor.MiddleRight;
				sideLabels.Add(sideLabel);
				
				for (int y = 0; y < entryCount; y++)
				{
					// parent element to hold a value field
					VisualElement element = new VisualElement();
					element.style.width = new Length(totalFieldWidth, LengthUnit.Pixel);
					element.style.height = new Length(fieldHeight, LengthUnit.Pixel);
					
					element.style.paddingBottom = new Length(PADDING, LengthUnit.Pixel);
					element.style.paddingLeft = new Length(PADDING, LengthUnit.Pixel);
					element.style.paddingRight = new Length(PADDING, LengthUnit.Pixel);
					element.style.paddingTop = new Length(PADDING, LengthUnit.Pixel);
					
					element.style.flexDirection = FlexDirection.Row;
					element.style.flexWrap = Wrap.Wrap;
					
					// draw value fields in a triangle pattern to avoid repetition (and replicate Unity's physics matrix look)
					if (y < entryCount - x)
					{
						// hack, x and y do not work in RegisterValueChangedCallback
						int a = x;
						int b = y;
						
						BindableElement field = new BindableElement();
			
						switch (matrix.Type)
						{
							case MatrixType.Bool:
								field = BuildBoolField(a, b, matrix);
							break;
							
							case MatrixType.Float:
								field = BuildFloatField(a, b, matrix);
							break;
							
							case MatrixType.Int:
								field = BuildIntField(a, b, matrix);
							break;
						}
						
						field.style.width = new Length(100, LengthUnit.Percent);
						field.style.height = new Length(100, LengthUnit.Percent);
						element.Add(field);
					}
					
					// draw empty VisualElement that fills remaining space in a row
					else
					{
						element.style.width = new Length(totalFieldWidth * (entryCount - y), LengthUnit.Pixel);
						matrixContainer.Add(element);
						break;
					}
					
					matrixContainer.Add(element);
				}
			}
		}
		
		public BaseField<bool> BuildBoolField(int x, int y, MatrixData matrix)
		{
			Toggle field = new Toggle();
			
			// writing a value to a field
			field.value = matrix.BoolMatrix[x, y];
			field.RegisterValueChangedCallback(val => matrix.UpdateValuesInMatrix<bool>(MatrixType.Bool, x, y, val.newValue));
			return field;
		}
		
		public BaseField<float> BuildFloatField(int x, int y, MatrixData matrix)
		{
			FloatField field = new FloatField();
			
			// writing a value to a field
			field.value = matrix.FloatMatrix[x, y];
			field.RegisterValueChangedCallback(val => matrix.UpdateValuesInMatrix<float>(MatrixType.Float, x, y, val.newValue));
			return field;
		}
		
		public BaseField<int> BuildIntField(int x, int y, MatrixData matrix)
		{
			IntegerField field = new IntegerField();
			
			// writing a value to a field
			field.value = matrix.IntMatrix[x, y];
			field.RegisterValueChangedCallback(val => matrix.UpdateValuesInMatrix<int>(MatrixType.Int, x, y, val.newValue));
			return field;
		}
	}
}