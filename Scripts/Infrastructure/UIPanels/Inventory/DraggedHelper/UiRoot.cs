using UnityEngine;

namespace CodeBase.UI
{
	public class UiRoot : MonoBehaviour
	{
		private static UiRoot _instance;
		private static Canvas _canvas;

		private void Awake()
		{
			_instance = this;
			_canvas = GetComponent<Canvas>();
		}

		public static float GetCanvasScaleFactor() => _canvas.scaleFactor;
	}
}