namespace Shed.ResourceManagement.Tests.Disposal
{
	using Shed.ResourceManagement.Disposal;


	public class DisposeHandlerTests
	{
		private readonly Action<DisposeHandler> onDispose = Substitute.For<Action<DisposeHandler>>();

		private DisposeHandler handler;


		public DisposeHandlerTests()
		{
			handler = new DisposeHandler(onDispose);
		}


		[Fact]
		public void DisposeIsNotCalledEarly()
			=> onDispose.DidNotReceiveWithAnyArgs().Invoke(default!);

		[Fact]
		public void DisposeCorrectlyCallsOnDisposeCallbackOnFirstInvokation()
		{
			// Act
			handler.Dispose();

			// Assert
			onDispose.Received(1).Invoke(handler);
		}

		[Fact]
		public void DisposeDoesNotCallOnDisposeCallbackOnSecondInvokation()
		{
			// Arrange
			handler.Dispose();
			onDispose.ClearReceivedCalls();

			// Act
			handler.Dispose();

			// Assert
			onDispose.DidNotReceiveWithAnyArgs().Invoke(default!);
		}
	}
}
