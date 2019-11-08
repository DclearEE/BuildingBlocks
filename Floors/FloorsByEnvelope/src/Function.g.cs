// This code was generated by Hypar.
// Edits to this code will be overwritten the next time you run 'hypar init'.
// DO NOT EDIT THIS FILE.

using Amazon;
using Amazon.Lambda.Core;
using Hypar.Functions.Execution;
using Hypar.Functions.Execution.AWS;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace FloorsByEnvelope
{
    public class Function
    {
        // Cache the model store for use by subsequent
        // executions of this lambda.
        private IModelStore<FloorsByEnvelopeInputs> store;

        public async Task<FloorsByEnvelopeOutputs> Handler(FloorsByEnvelopeInputs args, ILambdaContext context)
        {
            if(this.store == null)
            {
                this.store = new S3ModelStore<FloorsByEnvelopeInputs>(RegionEndpoint.USWest1);
            }
            
            var l = new InvocationWrapper<FloorsByEnvelopeInputs,FloorsByEnvelopeOutputs>(store, FloorsByEnvelope.Execute);
            var output = await l.InvokeAsync(args);
            return output;
        }
      }
}