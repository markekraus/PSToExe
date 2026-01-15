using System.Management.Automation;

// Retrieve the user script from manifest resources
string scriptBody = string.Empty;
using (Stream? resourceStream = typeof(Program).Assembly.GetManifestResourceStream("PSToExe.Script.ps1"))
{
    if (resourceStream is null)
    {
        Console.WriteLine("Unable to load embedded Script.ps1.");
        Environment.Exit(1);
    }
    using StreamReader reader = new (resourceStream, System.Text.Encoding.UTF8);
    scriptBody = reader.ReadToEnd();
}

// Wrapping the user script in a script block and pipping to Out-String
// will take care of formatting output in a PS-familiar way
// and maintains table and list formatting.
// It's not ideal, but it'll do.
// also... curly braces make this interpolated string painful to read. /shrug
var script = @$"
{{
{scriptBody}
}}.Invoke() | Out-String";

var ps = PowerShell.Create();

// Prepare to capture the various data streams.
ps.Streams.Verbose.DataAdded += ConsumePSDataStream;
ps.Streams.Debug.DataAdded += ConsumePSDataStream;
ps.Streams.Information.DataAdded += ConsumePSDataStream;
ps.Streams.Warning.DataAdded += ConsumePSDataStream;
var outputCollection = new PSDataCollection<PSObject>();
outputCollection.DataAdded += ConsumePSDataStream;

// Run the script
ps.AddScript(script);
ps.Invoke(null, outputCollection);


// callback for PowerShell data stream events
static void ConsumePSDataStream(object? sender, DataAddedEventArgs evtArgs)
{
    // early out if nulls
    if (sender is null || evtArgs is null)
        return;
    
    // Attempt to grab the most meaningful object to output
    var data = ((System.Collections.IList)sender)[evtArgs.Index];
    var output = data switch
    {
        VerboseRecord vr => vr.Message,
        InformationRecord ir => ir.MessageData,
        WarningRecord wr => wr.Message,
        DebugRecord dr => dr.Message,
        _ => data,
    };
    // It's always Console.WriteLine!
    Console.WriteLine(output);
}
