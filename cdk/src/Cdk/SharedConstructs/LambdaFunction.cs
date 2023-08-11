using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Logs;
using Constructs;
using XaasKit.CDK.AWS.Lambda.DotNet;

namespace Cdk.SharedConstructs;

using Amazon.CDK.AWS.Lambda.Destinations;
using Amazon.CDK.AWS.SQS;

public class LambdaFunction : Construct
{
    public Function Function { get; }
    
    public LambdaFunction(Construct scope, string id, string codePath, string handler, Dictionary<string, string> environmentVariables, bool isNativeAot = false) : base(scope, id)
    {
        if (isNativeAot)
        {
            this.Function = new Function(this, id, new FunctionProps()
            {
                FunctionName = id,
                Runtime = Runtime.PROVIDED_AL2,
                MemorySize = 1024,
                LogRetention = RetentionDays.ONE_DAY,
                Handler = "bootstrap",
                Environment = environmentVariables,
                Tracing = Tracing.ACTIVE,
                Code = Code.FromAsset(codePath),
                Architecture = Architecture.ARM_64,
                OnFailure = new SqsDestination(new Queue(this, $"{id}FunctionDLQ")),
            });
        }
        else
        {
            this.Function = new DotNetFunction(this, id, new DotNetFunctionProps()
            {
                FunctionName = id,
                Runtime = Runtime.DOTNET_6,
                MemorySize = 1024,
                LogRetention = RetentionDays.ONE_DAY,
                Handler = handler,
                Environment = environmentVariables,
                Tracing = Tracing.ACTIVE,
                ProjectDir = codePath,
                Architecture = Architecture.X86_64,
                OnFailure = new SqsDestination(new Queue(this, $"{id}FunctionDLQ")),
            });   
        }
    }
}