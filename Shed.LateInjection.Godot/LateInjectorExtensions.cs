using Godot;
using Shed.LateInjection.Abstractions;

namespace Shed.LateInjection.Godot;

public static class ServiceCollectionExtensions
{
    public static async Task<List<Node>> InjectWithPostCallbacks<InjectorT>(
        this InjectorT lateInjector,
        Node node)
        where InjectorT : ILateInjector
    {
        List<Node> injectedNodes = [];
        await lateInjector.InjectWithPostCallbacks(node, injectedNodes);
        return injectedNodes;
    }

    public static async Task InjectWithPostCallbacks<InjectorT>(
        this InjectorT lateInjector,
        Node node,
        List<Node> injectedNodes)
        where InjectorT : ILateInjector
    {
        lateInjector.InjectNode(node, injectedNodes);

        foreach (var injected in injectedNodes)
        {
            if (injected is IPostInject postInject)
            {
                postInject.PostInject();
            }
            if (injected is IPostInjectAsync postInjectAsync)
            {
                await postInjectAsync.PostInjectAsync();
            }
        }
    }

    public static List<Node> InjectNode<InjectorT>(
        this InjectorT lateInjector,
        Node node)
        where InjectorT : ILateInjector
    {
        List<Node> injectedNodes = [];
        lateInjector.InjectNode(node, injectedNodes);
        return injectedNodes;
    }

    public static void InjectNode<InjectorT>(
        this InjectorT lateInjector,
        Node node,
        List<Node> injectedNodes)
        where InjectorT : ILateInjector
    {
        lateInjector.Inject(node);
        injectedNodes.Add(node);
        lateInjector.InjectChildren(node, injectedNodes);
    }

    public static List<Node> InjectChildren<InjectorT>(
        this InjectorT lateInjector,
        Node node)
        where InjectorT : ILateInjector
    {
        List<Node> injectedNodes = [];
        lateInjector.InjectChildren(node, injectedNodes);
        return injectedNodes;
    }

    public static void InjectChildren<InjectorT>(
        this InjectorT lateInjector,
        Node node,
        List<Node> injectedNodes)
        where InjectorT : ILateInjector
    {
        foreach (var child in node.GetChildren())
        {
            lateInjector.Inject(child);
            injectedNodes.Add(child);
        }

        foreach (var child in node.GetChildren())
        {
            lateInjector.InjectChildren(child, injectedNodes);
        }
    }
}