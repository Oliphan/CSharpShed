namespace Shed.ResourceManagement.Access.Abstractions
{
	/// <summary>
	/// Exposes shareable and revokable access to a resource.
	/// </summary>
	/// <typeparam name="T">The type of resource whose access is exposed.</typeparam>
	public interface ISharedResource<out T> : IAccessToken<T>
	{
		/// <summary>
		/// Shares access to the resource.
		/// </summary>
		/// <returns>A new <see cref="ISharedResource{T}"/> instance that shares the access to the resource.</returns>
		/// <exception cref="InvalidOperationException">
		/// Thrown if called after access has been revoked.
		/// </exception>
		ISharedResource<T> ShareAccess();
	}
}