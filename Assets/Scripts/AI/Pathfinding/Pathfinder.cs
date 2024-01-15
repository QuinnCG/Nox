using UnityEngine;

namespace Game.AI.Pathfinding
{
	public static class Pathfinder
	{
		public static Path CalculatePath(NavMesh navMesh, Vector2 start, Vector2 end)
		{
			// var start
			// var end
			// var open[]
			// var closed[]
			//
			// add tiles adjacent to start into open[]
			// explore(start)
			//
			// void explore(tile) {
			// add tile to closed[]
			// add adjacent to tile into open[]
			// find lowest h-cost: lowest, explore(lowest)
			// }

			throw new System.NotImplementedException();
		}
	}
}
