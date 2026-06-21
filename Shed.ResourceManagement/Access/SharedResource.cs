using System;
using System.Collections.Generic;


namespace Shed.ResourceManagement.Access
{
	using Abstractions;
	using Shed.ResourceManagement.Disposal;



	/// <summary>
	/// Manages shared ownership of, and access to, a shared resource.
	/// </summary>
	/// <typeparam name="T">The type of resource to share ownership of.</typeparam>
	public sealed class SharedResource<T> : ISharedResource<T>
	{
		private readonly object accessLock;

		private readonly T resource;

		private readonly IDisposable onRelease;

		private HashSet<SharedResource<T>>? sharedInstances;


		/// <inheritdoc/>
		public T Resource => sharedInstances == null
			? throw new ObjectDisposedException("Access to the resource has been revoked.")
			: resource;


		public SharedResource(T resource, IDisposable onRelease)
		{
			accessLock = new object();
			sharedInstances = new HashSet<SharedResource<T>>() { this };
			this.resource = resource;
			this.onRelease = onRelease;
		}


		private SharedResource(SharedResource<T> sharedResource)
		{
			accessLock = sharedResource.accessLock;
			sharedInstances = sharedResource.sharedInstances;
			sharedInstances!.Add(this);
			resource = sharedResource.resource;
			onRelease = sharedResource.onRelease;
		}


		/// <inheritdoc cref="ISharedResource{T}.ShareAccess"/>
		public SharedResource<T> ShareAccess()
		{
			lock (accessLock)
			{
				if (sharedInstances == null)
					throw new InvalidOperationException("Cannot share disposed resource access.");

				return new SharedResource<T>(this);
			}
		}

		/// <summary>
		/// Disposes the shared resource instance,
		/// disposing the underlying resource if this is the last remaining instance referring to
		/// the resource.
		/// </summary>
		public void Dispose()
		{
			lock (accessLock)
			{
				if (sharedInstances == null)
					return;

				sharedInstances.Remove(this);

				if (sharedInstances.Count == 0)
				{
					onRelease.Dispose();
				}

				sharedInstances = null;
			}
		}


		ISharedResource<T> ISharedResource<T>.ShareAccess()
			=> ShareAccess();
	}


	/// <summary>
	/// Static helper class for creating <see cref="SharedResource{T}"/>s.
	/// </summary>
	public static class SharedResource
	{
		/// <summary>
		/// Creates a <see cref="SharedResource{T}"/> for the specified <paramref name="resource"/>.
		/// </summary>
		/// <typeparam name="T">The type of resource to have shared ownership of.</typeparam>
		/// <param name="resource">The resource to share ownership of.</param>
		/// <returns>The shared resource.</returns>
		public static SharedResource<T> Create<T>(T resource)
			where T : IDisposable
			=> new SharedResource<T>(resource, new DisposeHandler(resource.Dispose));

		/// <summary>
		/// Creates a <see cref="SharedResource{U}"/> for the specified <paramref name="resource"/>.
		/// </summary>
		/// <typeparam name="T">The type of resource to have shared ownership of.</typeparam>
		/// <typeparam name="U">
		/// The type of by which to expose shared access to the resource.
		/// </typeparam>
		/// <param name="resource">The resource to share ownership of.</param>
		/// <returns>The shared resource.</returns>
		public static SharedResource<U> Create<T, U>(T resource)
			where T : IDisposable, U
			=> new SharedResource<U>(resource, new DisposeHandler(resource.Dispose));

		/// <summary>
		/// Creates a <see cref="SharedResource{T}"/> for the specified <paramref name="resource"/>.
		/// </summary>
		/// <typeparam name="T">The type of resource to have shared ownership of.</typeparam>
		/// <typeparam name="U">
		/// The type via which access to the resource should be exposed.
		/// </typeparam>
		/// <param name="resource">The resource to share ownership of.</param>
		/// <param name="onRelease">
		/// The handler for when the last shared instance releases the resource.
		/// </param>
		/// <returns>The shared resource.</returns>
		public static SharedResource<T> Create<T>(T resource, DisposeHandler onRelease)
			=> new SharedResource<T>(resource, onRelease);
	}
}