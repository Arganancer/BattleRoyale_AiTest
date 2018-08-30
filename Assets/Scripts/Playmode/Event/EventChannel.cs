using UnityEngine;

namespace Playmode.Event
{
	public delegate void EventChannelEventHandler();
	
	public abstract class EventChannel : MonoBehaviour
	{
		public event EventChannelEventHandler OnEventPublished;

		public void Publish()
		{
			if (OnEventPublished != null) OnEventPublished();
		}
	}
}