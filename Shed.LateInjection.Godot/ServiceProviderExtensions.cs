using Godot;
using Shed.LateInjection;

namespace Shed.LateInjection.Godot;

public static class ServiceProviderExtensions
{
    public static async Task<List<Node>> LateInjectWithPostCallbacks(
        this IServiceProvider serviceProvider,
        Node node)
    {
        List<Node> injectedNodes = [];
        await serviceProvider.LateInjectWithPostCallbacks(node, injectedNodes);
        return injectedNodes;
    }

    public static async Task LateInjectWithPostCallbacks(
        this IServiceProvider serviceProvider,
        Node node,
        List<Node> injectedNodes)
    {
        serviceProvider.LateInjectNode(node, injectedNodes);

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

    public static List<Node> LateInjectNode(
        this IServiceProvider serviceProvider,
        Node node)
    {
        List<Node> injectedNodes = [];
        serviceProvider.LateInjectNode(node, injectedNodes);
        return injectedNodes;
    }

    public static void LateInjectNode(
        this IServiceProvider serviceProvider,
        Node node,
        List<Node> injectedNodes)
    {
        serviceProvider.LateInject(node);
        injectedNodes.Add(node);
        serviceProvider.LateInjectChildren(node, injectedNodes);
    }

    public static List<Node> LateInjectChildren(
        this IServiceProvider serviceProvider,
        Node node)
    {
        List<Node> injectedNodes = [];
        serviceProvider.LateInjectChildren(node, injectedNodes);
        return injectedNodes;
    }

    public static void LateInjectChildren(
        this IServiceProvider serviceProvider,
        Node node,
        List<Node> injectedNodes)
    {
        foreach (var child in node.GetChildren())
        {
            serviceProvider.LateInject(child);
            injectedNodes.Add(child);
        }

        foreach (var child in node.GetChildren())
        {
            serviceProvider.LateInjectChildren(child, injectedNodes);
        }
    }
}