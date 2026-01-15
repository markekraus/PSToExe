using System.Management.Automation;
using System.Management.Automation.Runspaces;

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

var ps = PowerShell.Create();

// Prepare to capture the various data streams.
ps.Streams.Verbose.DataAdded += ConsumePSDataStream;
ps.Streams.Debug.DataAdded += ConsumePSDataStream;
ps.Streams.Information.DataAdded += ConsumePSDataStream;
ps.Streams.Warning.DataAdded += ConsumePSDataStream;
ps.Streams.Error.DataAdded += ConsumePSDataStream;
var outputCollection = new PSDataCollection<PSObject>();
outputCollection.DataAdded += ConsumePSDataStream;

// Run the script
ps.AddScript(scriptBody).AddCommand("Out-String");
ps.Commands.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);
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
        ErrorRecord er => er.ToString(),
        _ => data,
    };
    // It's always Console.WriteLine!
    Console.WriteLine(output);
}