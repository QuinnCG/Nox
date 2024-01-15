namespace Game.AI.Pathfinding
{
	public class Tile
	{
		public int F;
		public int G;
		public int H => F + G;
	}
}
