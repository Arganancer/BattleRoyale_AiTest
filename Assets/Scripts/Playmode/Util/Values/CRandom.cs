namespace Playmode.Util.Values
{
	public struct CRandom
	{
		private static readonly System.Random CRand = new System.Random();

		public static float Next(int minValue, int maxValue)
		{
			return CRand.Next(minValue, maxValue);
		}

		public static float Nextf(float minValue, float maxValue)
		{
			const float startAffector = 1000f;
			const float endAffector = 0.001f;
			var adjustedMinValue = (int)(minValue * startAffector);
			var adjustedMaxValue = (int)(maxValue * startAffector);

			float result = CRand.Next(adjustedMinValue, adjustedMaxValue);

			return result * endAffector;
		}
	}
}