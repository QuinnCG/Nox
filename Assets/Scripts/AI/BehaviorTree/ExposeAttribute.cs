using System;

namespace Game.AI.BehaviorTree
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class ExposeAttribute : Attribute
	{ }
}
