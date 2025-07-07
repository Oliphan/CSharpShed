using System;


namespace Shed.ResourceManagement.Disposal
{
	/// <summary>
	/// Handles the execution of an action on first disposal.
	/// </summary>
	public sealed class DisposeHandler : IDisposable
	{
		private readonly object disposalLock = new object();

		private Action? onDispose;


		public DisposeHandler()
			=> this.onDispose = null;

		public DisposeHandler(Action onDispose)
			=> this.onDispose = onDispose;

		public DisposeHandler(Action<DisposeHandler> onDispose)
			=> this.onDispose = () => onDispose(this);


		/// <summary>
		/// When invoked for the first time, invokes the onDispose delegate specified when the
		/// <see cref="DisposeHandler"/> was constructed.
		/// Subsequent invokations do nothing.
		/// </summary>
		public void Dispose()
		{
			lock (disposalLock)
			{
				if (onDispose == null)
					return;

				onDispose?.Invoke();
				onDispose = null;
			}
		}
	}
}