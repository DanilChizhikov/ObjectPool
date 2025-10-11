using System;

namespace DTech.ObjectPool
{
	internal sealed class PoolContentNotInitializedException : Exception
	{
		public PoolContentNotInitializedException(PoolContent content, string placement) :
			base($"[{nameof(PoolContent)}] [{placement}] {content.GetType().Name} is not initialized!")
		{
		}
	}
}