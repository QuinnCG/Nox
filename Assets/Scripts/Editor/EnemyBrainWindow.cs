using Game.AI;
using Game.AI.BehaviorTree;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Editor
{
	public class EnemyBrainWindow : EditorWindow
	{
		private EnemyBrain _brain;

		[MenuItem("Window/Enemy Brain")]
		public static void Display()
		{
			var window = GetWindow<EnemyBrainWindow>();
			window.titleContent = new GUIContent("Enemy Brain");
		}

		private void OnEnable()
		{
			UpdateUI();
		}

		private void OnHierarchyChange()
		{
			UpdateUI();
		}

		private void UpdateUI()
		{
			rootVisualElement.Clear();

			var brain = new ObjectField("Brain")
			{
				objectType = typeof(EnemyBrain),
				value = _brain
			};
			brain.RegisterValueChangedCallback(x => _brain = x.newValue as EnemyBrain);
			rootVisualElement.Add(brain);

			if (_brain != null)
			{
				CreateRow(_brain.Tree.GetRoot().GetChildren());
			}
		}

		private VisualElement CreateRow(IEnumerable<BTNode> nodes)
		{
			var row = new VisualElement();
			foreach (var node in nodes)
			{
				row.Add(CreateNode(node));
			}

			return row;
		}

		private VisualElement CreateNode(BTNode node)
		{
			var element = new VisualElement();
			var label = new Label(node.GetType().Name);
			element.Add(label);

			return element;
		}
	}
}
