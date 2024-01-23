using Game.AI;
using Game.AI.BehaviorTree;
using Sirenix.OdinInspector.Editor;
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
		private readonly Color PrimaryColor = new(0.2196079f, 0.2196079f, 0.2196079f, 1f);
		private readonly Color SecondaryColor = new(0.1568628f, 0.1568628f, 0.1568628f, 1f);
		private readonly Color LightColor = new(0.3f, 0.3f, 0.3f, 1f);
		private readonly Color AccentColor = new(0.172549f, 0.3647059f, 0.5294118f, 1f);
		private readonly Color ErrorColor = new(1f, 0.4313726f, 0.2509804f, 1f);

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

			var brainField = new ObjectField("Brain")
			{
				objectType = typeof(EnemyBrain),
				value = _brain
			};
			brainField.RegisterValueChangedCallback(x =>
			{
				_brain = x.newValue as EnemyBrain;
				UpdateUI();
			});
			toolbar.Add(brainField);

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

			var title = new Label("Inspector");
			title.name = "Title";
			title.style.color = Color.white;
			title.style.width = Length.Percent(100f);
			title.style.fontSize = 18f;
			title.style.unityTextAlign = TextAnchor.MiddleCenter;
			inspector.Add(title);

			return inspector;
		}

		private VisualElement CreateTree()
		{
			TreeViewItemData<BTNode> root;

			if (_brain)
			{
				root = new TreeViewItemData<BTNode>(0, _brain.Tree.GetRoot());
				root = CreateDataset(root);
			}
			else root = new TreeViewItemData<BTNode>();

			var tree = new TreeView();
			tree.name = "Tree View";
			tree.style.backgroundColor = SecondaryColor;
			tree.style.flexGrow = 1f;
			tree.style.height = Length.Percent(100f);
			rootVisualElement.Add(tree);

			tree.makeItem = MakeItem;
			tree.bindItem = BindItem;
			tree.SetRootItems(new List<TreeViewItemData<BTNode>>() { root });

			VisualElement MakeItem() => new Label();
			void BindItem(VisualElement item, int index)
			{
				var label = item as Label;
				label.text = tree.GetItemDataForIndex<BTNode>(index).GetType().Name;
			}

			return tree;
		}

		private TreeViewItemData<BTNode> CreateDataset(TreeViewItemData<BTNode> root)
		{
			var children = new List<TreeViewItemData<BTNode>>();

			int i = 0;
			foreach (var child in root.data.GetChildren())
			{
				var node = new TreeViewItemData<BTNode>(i, child);
				node = CreateDataset(node);
				children.Add(node);

				Debug.Log(child.GetType().Name);

				i++;
			}

			return new TreeViewItemData<BTNode>(root.id, root.data, children);
		}

		private VisualElement CreateEmptyItem()
		{
			return new Label();
		}

		private void UpdateItem(VisualElement item, BTNode node)
		{

		}
	}
}
