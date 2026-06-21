namespace Shed.ResourceManagement.Tests.Access
{
	using Shed.ResourceManagement.Access;


	public class SharedResourceTests
	{
		private IDisposable onReleasedResource;

		private SharedResource<int> sharedResource;


		[SetUp]
		public void SetUp()
		{
			onReleasedResource = Substitute.For<IDisposable>();

			sharedResource = new SharedResource<int>(10, onReleasedResource);
		}


		[TearDown]
		public void TearDown()
		{
			onReleasedResource.Dispose();
			sharedResource.Dispose();
		}


		[Test]
		public void AllowsAccessToResourceWhenNotDisposed()
			=> Assert.That(sharedResource.Resource, Is.EqualTo(10));

		[Test]
		public void AccessingResourceWhenDisposedThrowsObjectDisposedException()
		{
			// Act
			sharedResource.Dispose();

			// Assert
			Assert.Throws<ObjectDisposedException>(() => _ = sharedResource.Resource);
		}

		[Test]
		public void DisposalWhenOnlyInstanceCallsOnReleasedResource()
		{
			// Act
			sharedResource.Dispose();

			// Assert
			Assert.DoesNotThrow(() => onReleasedResource.Received(1).Dispose());
		}

		[Test]
		public void DisposalWhenOtherInstancesExistDoesNotCallOnReleasedResource()
		{
			// Arrange
			using var otherInstance = sharedResource.ShareAccess();

			// Act
			sharedResource.Dispose();

			// Assert
			Assert.DoesNotThrow(() => onReleasedResource.DidNotReceive().Dispose());
		}

		[Test]
		public void DisposalOfSharedInstanceWhenInitialInstanceStillExistsDoesCallsOnReleasedResource()
		{
			// Arrange
			var otherInstance = sharedResource.ShareAccess();

			// Act
			otherInstance.Dispose();

			// Assert
			Assert.DoesNotThrow(() => onReleasedResource.DidNotReceive().Dispose());
		}

		[Test]
		public void DisposalOfLastInstanceCallsOnReleasedResource()
		{
			// Arrange
			var otherInstance = sharedResource.ShareAccess();

			// Act
			otherInstance.Dispose();
			sharedResource.Dispose();

			// Assert
			Assert.DoesNotThrow(() => onReleasedResource.Received(1).Dispose());
		}

		[Test]
		public void DisposeIsNotCalledEarly()
			=> Assert.DoesNotThrow(() => onReleasedResource.ReceivedWithAnyArgs(0).Dispose());
	}
}
