using Godot;
using Microsoft.Extensions.DependencyInjection;
using Shed.LateInjection.Attributes;
using Shed.LateInjection.Generated;
using static GdUnit4.Assertions;

namespace Shed.LateInjection.Godot.Tests;

[TestSuite]
public partial class LateInjectorExtensionsTests
{
    public partial class TestNode(
        string name,
        List<string> injectList)
        : Node, IPostInject, IPostInjectAsync
    {
        [LateInject]
        public void Inject()
            => injectList.Add($"{name} Inject");

        public void PostInject()
            => injectList.Add($"{name} PostInject");
            
        public async Task PostInjectAsync()
        {
            await Task.Delay(1);
            injectList.Add($"{name} PostInjectAsync");
        }
    }

    [TestCase]
    [RequireGodotRuntime]
    public async Task InjectWithPostCallbacks_InjectsAndInvokesCallbacksInExpectedOrder()
    {
        // Arrange
        var services = new ServiceCollection()
            .BuildServiceProvider();

        var injector = new LateInjector(services);

        List<string> injectList = [];

        var root = AutoFree(new TestNode(
            "root",
            injectList
        ))!;

        var child1 = AutoFree(new TestNode(
            "child1",
            injectList
        ))!;

        var subchild1 = AutoFree(new TestNode(
            "subchild1",
            injectList
        ))!;

        var subchild2 = AutoFree(new TestNode(
            "subchild2",
            injectList
        ))!;

        var child2 = AutoFree(new TestNode(
            "child2",
            injectList
        ))!;

        var subchild3 = AutoFree(new TestNode(
            "subchild3",
            injectList
        ))!;

        root.AddChild(child1);
        root.AddChild(child2);
        child1.AddChild(subchild1);
        child1.AddChild(subchild2);
        child2.AddChild(subchild3);

        // Act
        var injectedNodes = await injector.InjectWithPostCallbacks(root);

        // Assert
        AssertThat(injectList)
            .IsEqual(
                new string[]
                {
                    "root Inject",
                    "child1 Inject",
                    "child2 Inject",
                    "subchild1 Inject",
                    "subchild2 Inject",
                    "subchild3 Inject",
                    "root PostInject",
                    "root PostInjectAsync",
                    "child1 PostInject",
                    "child1 PostInjectAsync",
                    "child2 PostInject",
                    "child2 PostInjectAsync",
                    "subchild1 PostInject",
                    "subchild1 PostInjectAsync",
                    "subchild2 PostInject",
                    "subchild2 PostInjectAsync",
                    "subchild3 PostInject",
                    "subchild3 PostInjectAsync"
                });

        AssertThat(injectedNodes)
            .IsEqual(
                new Node[]
                {
                    root,
                    child1,
                    child2,
                    subchild1,
                    subchild2,
                    subchild3
                });
    }
}