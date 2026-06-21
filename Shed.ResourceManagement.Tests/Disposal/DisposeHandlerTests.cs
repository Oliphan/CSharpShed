namespace Shed.ResourceManagement.Tests.Disposal
{
	using Shed.ResourceManagement.Disposal;


	public class DisposeHandlerTests
	{
		private Action<DisposeHandler> onDispose;

		private DisposeHandler handler;


		[SetUp]
		public void SetUp()
		{
			onDispose = Substitute.For<Action<DisposeHandler>>();

			handler = new DisposeHandler(onDispose);
		}


		[TearDown]
		public void TearDown()
		{
			handler.Dispose();
		}


		[Test]
		public void DisposeIsNotCalledEarly()
			=> Assert.DoesNotThrow(() => onDispose.DidNotReceiveWithAnyArgs().Invoke(default!));

		[Test]
		public void DisposeCorrectlyCallsOnDisposeCallbackOnFirstInvokation()
		{
			// Act
			handler.Dispose();

			// Assert
			Assert.DoesNotThrow(() => onDispose.Received(1).Invoke(handler));
		}

		[Test]
		public void DisposeDoesNotCallOnDisposeCallbackOnSecondInvokation()
		{
			// Arrange
			handler.Dispose();
			onDispose.ClearReceivedCalls();

			// Act
			handler.Dispose();

			// Assert
			Assert.DoesNotThrow(() => onDispose.DidNotReceiveWithAnyArgs().Invoke(default!));
		}
	}
}
