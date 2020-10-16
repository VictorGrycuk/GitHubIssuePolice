# Helpers

## Serializer

All this class do is deserialize deserialize the configuration file into a class.

```csharp | Serializer.cs
internal static class Serializer
{
    internal static Configuration DeserializeConfiguration(string filePath)
    {
        return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(filePath));
    }
}
```

<br>

## RoslynScripting

The `RoslynScripting` class only has one method that all it does is to receive a list of assemblies, import directives, and a lambda to create an a function of the type `<Generic, Bool>`.

```csharp | RoslynScripting.cs
internal static class RoslynScripting
{
    internal static Func<T, bool> Evaluate<T>(Assembly[] assemblies, string[] imports, string lambda)
    {
        var options = ScriptOptions.Default.AddReferences(assemblies).WithImports(imports);
        return CSharpScript.EvaluateAsync<Func<T, bool>>(lambda, options).Result;
    }
}
```

While this allows a lot of external customization, the drawback is that even simple expressions are expensive to run.

> :ToCPrevNext