using FMOD;
using UnityEditor;
using UnityEngine;

namespace Game
{
	public class GizmosHelper : MonoBehaviour
	{
		private GizmosHelperType _type;
		private Color _color;
		private float _value1, _value2;
		private string _text;

		public static void DrawCircle(Vector2 position, float radius, Color color)
		{
			var instance = new GameObject("Gizmos Helper");
			var helper = instance.AddComponent<GizmosHelper>();

			helper.transform.position = position;
			helper._color = color;
			helper._type = GizmosHelperType.Circle;
			helper._value1 = radius;
		}
		public static void DrawCircle(Vector2 position, float radius)
		{
			DrawCircle(position, radius, Color.white);
		}

		public static void DrawBox(Vector2 position, Vector2 size, Color color)
		{
			var instance = new GameObject("Gizmos Helper");
			var helper = instance.AddComponent<GizmosHelper>();

			helper.transform.position = position;
			helper._color = color;
			helper._type = GizmosHelperType.Box;
			helper._value1 = size.x;
			helper._value2 = size.y;
		}
		public static void DrawBox(Vector2 position, Vector2 size)
		{
			DrawBox(position, size, Color.white);
		}

		public static void DrawText(Vector2 position, string text)
		{
			var instance = new GameObject("Gizmos Helper");
			var helper = instance.AddComponent<GizmosHelper>();

			helper.transform.position = position;
			helper._type = GizmosHelperType.Text;
			helper._text = text;
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			Gizmos.color = _color;

			if (_type == GizmosHelperType.Circle)
			{
				Gizmos.DrawWireSphere(transform.position, _value1);
			}
			else if (_type == GizmosHelperType.Box)
			{
				Gizmos.DrawWireCube(transform.position, new Vector2(_value1, _value2));
			}
			else if (_type == GizmosHelperType.Text)
			{
				Handles.Label(transform.position, _text);
			}
		}
#endif
	}
}
