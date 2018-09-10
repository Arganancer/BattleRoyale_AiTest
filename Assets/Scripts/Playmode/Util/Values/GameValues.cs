namespace Playmode.Util.Values
{
	//BEN_CORRECTION : J'aurais fait une static class à la place d'une struct avec des variables statique.
	//
	//				   Actuellement, il est toujours possible d'instancier un objet de type GameValues.
	public struct GameValues
	{
		public static int NbOfEnemies = 4;
	}
}