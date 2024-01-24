using Game.AI;
using Game.AI.BehaviorTree;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.EditorWindows
{
	public class EnemyBrainWindow : EditorWindow
	{
		private readonly Color PrimaryColor = new(0.2196079f, 0.2196079f, 0.2196079f, 1f);
		private readonly Color SecondaryColor = new(0.1568628f, 0.1568628f, 0.1568628f, 1f);
		private readonly Color LightColor = new(0.3f, 0.3f, 0.3f, 1f);
		private readonly Color AccentColor = new(0.172549f, 0.3647059f, 0.5294118f, 1f);
		private readonly Color HighlightColor = new(1f, 0.7568628f, 0.02745098f, 1f);
		private readonly Color ErrorColor = new(1f, 0.4313726f, 0.2509804f, 1f);

		private EnemyBrain _brain;
		private TreeView _tree;
		private VisualElement _blackboard;

		private readonly Dictionary<BTNode, VisualElement> _nodesToItems = new();
		private BTTask _lastActiveTask;

		public static void DisplayBrain(EnemyBrain brain)
		{
			if (Application.isPlaying)
			{
				var window = GetWindow<EnemyBrainWindow>();
				window.titleContent = new GUIContent("Enemy Brain");

				window._brain = brain;
				window.UpdateUI();
			}
		}

		private void OnInspectorUpdate()
		{
			var active = _brain.Tree.ActiveTask;
			if (active != _lastActiveTask)
			{
				if (_lastActiveTask != null)
				{
					SetBranchActive(_nodesToItems[_lastActiveTask], false);
				}

				if (_nodesToItems.TryGetValue(active, out VisualElement element))
				{
					SetBranchActive(element, true);
				}

				_lastActiveTask = active;
			}

			_blackboard.Clear();
			var members = GetBlackBoardMembers();

			foreach (var member in members)
			{
				string name;
				string value = "NO VALUE SET";

				if (member is FieldInfo field)
				{
					name = field.Name;

					var instance = field.GetValue(_brain);
					if (instance is IBTProperty bt)
					{
						value = bt.Value?.ToString();
					}
				}
				else if (member is PropertyInfo property)
				{
					name = property.Name;

					var instance = property.GetValue(_brain);
					if (instance is IBTProperty bt)
					{
						value = bt.Value?.ToString();
					}
				}
				else
				{
					throw new System.Exception();
				}

				var label = new Label($"{name}: {value}");
				label.style.color = Color.white;
				_blackboard.Add(label);
			}
		}

		private void UpdateUI()
		{
			rootVisualElement.Clear();

			var vertical = new VisualElement();
			rootVisualElement.Add(vertical);
			vertical.style.width = Length.Percent(100f);
			vertical.style.height = Length.Percent(100f);
			vertical.style.flexDirection = FlexDirection.Column;

			var toolbar = CreateToolbar();
			vertical.Add(toolbar);

			var split = new TwoPaneSplitView(0, 250f, TwoPaneSplitViewOrientation.Horizontal);
			split.name = "Split View";
			vertical.Add(split);

			var inspector = CreateInspector();
			split.Add(inspector);

			var tree = CreateTree();
			split.Add(tree);
		}

		private VisualElement CreateToolbar()
		{
			var toolbar = new VisualElement();
			toolbar.name = "Toolbar";
			toolbar.style.backgroundColor = PrimaryColor;
			toolbar.style.width = Length.Percent(100f);
			toolbar.style.height = 30f;
			toolbar.style.borderBottomColor = LightColor;
			toolbar.style.borderBottomWidth = 1f;

			return toolbar;
		}

		private VisualElement CreateInspector()
		{
			var inspector = new VisualElement();
			inspector.name = "Inspector";
			inspector.style.backgroundColor = PrimaryColor;
			inspector.style.flexGrow = 1f;
			inspector.style.height = Length.Percent(100f);
			inspector.style.width = Length.Percent(100f);
			inspector.style.borderRightColor = LightColor;
			inspector.style.borderRightWidth = 1f;

			var title = new Label("Black Board");
			title.name = "Title";
			title.style.color = Color.white;
			title.style.width = Length.Percent(100f);
			title.style.fontSize = 18f;
			title.style.unityTextAlign = TextAnchor.MiddleCenter;
			inspector.Add(title);

			_blackboard = new VisualElement();
			_blackboard.style.flexGrow = 1f;
			inspector.Add(_blackboard);

			return inspector;
		}

		private VisualElement CreateTree()
		{
			_nodesToItems.Clear();

			var items = new List<TreeViewItemData<BTNode>>();
			int id = 1;
			BTNode root = _brain.Tree.GetRoot();
			var item = new TreeViewItemData<BTNode>(0, root, CreateTreeDataSet(root, ref id));

			items.Add(item);

			_tree = new TreeView();
			_tree.name = "Tree View";
			_tree.autoExpand = true;
			_tree.style.backgroundColor = SecondaryColor;
			_tree.style.flexGrow = 1f;
			_tree.style.height = Length.Percent(100f);
			rootVisualElement.Add(_tree);

			_tree.makeItem = CreateTreeItem;
			_tree.bindItem = BindItem;
			_tree.SetRootItems(items);

			void BindItem(VisualElement item, int index)
			{
				var data = _tree.GetItemDataForIndex<BTNode>(index);
				UpdateTreeItem(item, data);
			}
			return _tree;
		}

		private List<TreeViewItemData<BTNode>> CreateTreeDataSet(BTNode root, ref int id)
		{
			var children = root.GetChildren();
			var items = new List<TreeViewItemData<BTNode>>();

			foreach (BTNode child in children)
			{
				var item = new TreeViewItemData<BTNode>(id++, child, CreateTreeDataSet(child, ref id));
				items.Add(item);
			}

			return items;
		}

		private VisualElement CreateTreeItem()
		{
			var root = new Label();
			root.style.color = Color.white;

			return root;
		}

		private void UpdateTreeItem(VisualElement item, BTNode node)
		{
			((Label)item).text = node.GetType().Name;

			if (!_nodesToItems.ContainsKey(node))
			{
				_nodesToItems.Add(node, item);
			}
		}

		private void SetBranchActive(VisualElement element, bool highlight)
		{
			VisualElement current = element;
			var color = highlight ? HighlightColor : Color.white;

			while (current != null)
			{
				current.style.color = color;
				current = current.parent;
			}
		}

		private MemberInfo[] GetBlackBoardMembers()
		{
			var members = new List<MemberInfo>();

			var type = _brain.GetType();
			var fields = type.GetFields();
			var properties = type.GetProperties();

			foreach (var property in properties)
			{
				var attribute = property.GetCustomAttribute<ExposeAttribute>();
				if (attribute != null)
				{
					members.Add(property);
				}
			}

			foreach (var field in fields)
			{
				var attribute = field.GetCustomAttribute<ExposeAttribute>();
				if (attribute != null)
				{
					members.Add(field);
				}
			}

			return members.ToArray();
		}
	}

	// TODO: Highlighting is not working.
}
