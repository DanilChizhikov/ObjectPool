using UnityEngine;

namespace DTech.ObjectPool.Tests
{
	internal sealed class TestComponent : MonoBehaviour, IResettable
	{
		public bool WasReset { get; set; } = false;
		
		public void ResetSettings()
		{
			WasReset = true;
		}
	}
}