using System;

namespace DTech.ObjectPool
{
	internal sealed class PoolContentDisposedException : Exception
	{
		public PoolContentDisposedException(PoolContent content, string placement) :
			base($"[{nameof(PoolContent)}] [{placement}] {content.GetType().Name} is disposed!")
		{
		}
	}
}