namespace Shed.ResourceManagement.Tests.Access
{
	using Shed.ResourceManagement.Access;

    public class SharedResourceTests
	{
		private readonly IDisposable onReleasedResource = Substitute.For<IDisposable>();

		private readonly SharedResource<int> originalSharedResourceInstance;


		public SharedResourceTests()
		{
			originalSharedResourceInstance = new SharedResource<int>(10, onReleasedResource);
		}


		[Fact]
		public void Resource_WhenNotDisposed_AllowsAccessToResource()
			=> Assert.Equal(10, originalSharedResourceInstance.Resource);

		[Fact]
		public void Resource_WhenDisposed_ThrowsObjectDisposedException()
		{
			// Act
			originalSharedResourceInstance.Dispose();

			// Assert
			Assert.Throws<ObjectDisposedException>(() => _ = originalSharedResourceInstance.Resource);
		}

		[Fact]
		public void Dispose_WhenOnlyInstance_CallsOnReleasedResource()
		{
			// Act
			originalSharedResourceInstance.Dispose();

			// Assert
			onReleasedResource.Received(1).Dispose();
		}

		[Fact]
		public void DisposeOriginalInstance_WhenInstanceCreatedByShareAccessNotDisposed_DoesNotCallOnReleasedResource()
		{
			// Arrange
			using var otherInstance = originalSharedResourceInstance.ShareAccess();

			// Act
			originalSharedResourceInstance.Dispose();

			// Assert
			onReleasedResource.DidNotReceive().Dispose();
		}

		[Fact]
		public void DisposeInstanceCreatedByShareAccess_WhenOriginalInstanceNotDisposed_DoesCallsOnReleasedResource()
		{
			// Arrange
			var otherInstance = originalSharedResourceInstance.ShareAccess();

			// Act
			otherInstance.Dispose();

			// Assert
			onReleasedResource.DidNotReceive().Dispose();
		}

		[Fact]
		public void Dispose_WhenCalledOnSharedInstanceAfterOriginal_CallsOnReleasedResource()
		{
			// Arrange
			var otherInstance = originalSharedResourceInstance.ShareAccess();

			// Act
			otherInstance.Dispose();
			originalSharedResourceInstance.Dispose();

			// Assert
			onReleasedResource.Received(1).Dispose();
		}

		[Fact]
		public void Dispose_WhenCalledOnOriginalInstanceAfterShared_CallsOnReleasedResource()
		{
			// Arrange
			var otherInstance = originalSharedResourceInstance.ShareAccess();

			// Act
			originalSharedResourceInstance.Dispose();
			otherInstance.Dispose();

			// Assert
			onReleasedResource.Received(1).Dispose();
		}

		[Fact]
		public void DisposeIsNotCalledEarly()
			=> onReleasedResource.DidNotReceiveWithAnyArgs().Dispose();
	}
}
