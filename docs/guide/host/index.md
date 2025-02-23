# Integration with IHost

::: tip warning
The functionality to integrate Oakton into .Net Core projects in *Oakton.AspNetCore* was combined
into the main Oakton library for V3.0.
:::

Oakton works well with the [generic HostBuilder](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-5.0) in .Net Core and .Net 5 to extend the command line support
of your applications.

* [An improved, default run command](/guide/host/run)
* [A new command for environment check support](/guide/host/environment)
* [Ability to add your own commands running in the context of your application](/guide/host/extensions)
* [Extensible diagnostic data about your application](/guide/host/describe)

To enable the extended command line options in your .Net application bootstrapped by `IHostBuilder`, first install the `Oakton` nuget to your project. Then modify the `Program.Main()` method generated by the typical .Net project templates
as shown in some of the examples below:

## .Net 6 Console Integration

In .Net 6, you have the option to allow .Net to magically create a `Program.Main()` entry point over the
code in the `Program` file. Oakton integration with `IHost` works every so slightly differently than the .Net 5 
and before versions, but you can see an example below:

<!-- snippet: sample_bootstrapping_minimal_api -->
<a id='snippet-sample_bootstrapping_minimal_api'></a>
```cs
using Oakton;

var builder = WebApplication.CreateBuilder(args);

// This isn't required, but it "helps" Oakton to enable
// some additional diagnostics for the stateful resource 
// model
builder.Host.ApplyOaktonExtensions();

var app = builder.Build();
app.MapGet("/", () => "Hello World!");

// Note the usage of await to force the implied
// Program.Main() method to be asynchronous
return await app.RunOaktonCommands(args);
```
<sup><a href='https://github.com/JasperFx/alba/blob/master/src/MinimalApi/Program.cs#L1-L19' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_bootstrapping_minimal_api' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Just note that the `return await app.RunOaktonCommands(args);` line at the bottom is important because:

1. Using `return` here tells .Net to make the `Program.Main()` return an exit code that the OS itself needs in order to understand if an execution was successful or not -- and one of the common usages of Oakton is to create validation commands that require this.
2. Using `await` here tells .Net to make `Program.Main()` return `Task<int>` and execute asynchronously instead of Oakton having to do evil `.GetAwaiter().GetResult()` calls behind the scenes.
3. `RunOaktonCommands()` makes the .Net program use Oakton as the command line executor to begin with

## "Old School Program.Main()"

Oakton usage for "old school" < .Net 6 applications is very similar:

<!-- snippet: sample_using_run_oakton_commands_3 -->
<a id='snippet-sample_using_run_oakton_commands_3'></a>
```cs
public class Program
{
    public static Task<int> Main(string[] args)
    {
        return CreateHostBuilder(args)
            
            // This extension method replaces the calls to
            // IWebHost.Build() and Start()
            .RunOaktonCommands(args);
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(x => x.UseStartup<Startup>());
    
}
```
<sup><a href='https://github.com/JasperFx/alba/blob/master/src/MvcApp/Program.cs#L37-L54' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_using_run_oakton_commands_3' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

or with `IWebHostBuilder`:

<!-- snippet: sample_using_run_oakton_commands -->
<a id='snippet-sample_using_run_oakton_commands'></a>
```cs
public class Program
{
    public static Task<int> Main(string[] args)
    {
        return CreateWebHostBuilder(args)
            
            // This extension method replaces the calls to
            // IWebHost.Build() and Start()
            .RunOaktonCommands(args);
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>();
    
}
```
<sup><a href='https://github.com/JasperFx/alba/blob/master/src/MvcApp/Program.cs#L17-L34' title='Snippet source file'>snippet source</a> | <a href='#snippet-sample_using_run_oakton_commands' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

There are just a couple things to note:

1. The return value of the `Program.Main()` method now needs to be `Task<int>` rather than `void`. This is done so that Oakton
   can return either successful or failure exit codes for usage in diagnostic commands you may want to stop automated builds upon
   failures.
1. You will use the `RunOaktonCommands()` method to accept the command line arguments and invoke your system rather than manually
   building and/or starting the `IWebHost` yourself
