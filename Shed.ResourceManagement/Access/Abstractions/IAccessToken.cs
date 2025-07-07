using System;


namespace Shed.ResourceManagement.Access.Abstractions
{
	/// <summary>
	/// Exposes revokeable access to a resource.
	/// </summary>
	/// <typeparam name="T">The type of resource whose access is exposed.</typeparam>
	public interface IAccessToken<out T> : IDisposable
	{
		/// <summary>
		/// The resource, access to which is granted.
		/// </summary>
		/// <exception cref="ObjectDisposedException">
		/// Thrown if accessed after access has been revoked.
		/// </exception>
		T Resource { get; }
	}
}