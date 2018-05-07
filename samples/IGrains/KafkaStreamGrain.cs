using Orleans;
using Orleans.Streams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IGrains
{
    [ImplicitStreamSubscription("GrainImplicitStream")]
    public  class KafkaStreamGrain: Grain, IKafkaStream, IAsyncObserver<string>
    {
        protected StreamSubscriptionHandle<string> streamHandle;

        public static int count = 0;
        public override async Task OnActivateAsync()
        {
            var streamId = this.GetPrimaryKey();
            var streamProvider = this.GetStreamProvider("Kafka");
            var stream = streamProvider.GetStream<string>(streamId, "GrainImplicitStream");
            streamHandle = await stream.SubscribeAsync(OnNextAsync);
        }

        public override async Task OnDeactivateAsync()
        {
            if (streamHandle != null)
                await streamHandle.UnsubscribeAsync();
        }

        public Task OnCompletedAsync()
        {
            return Task.CompletedTask;
        }

        public Task OnErrorAsync(Exception ex)
        {
            return Task.CompletedTask;
        }

        public Task OnNextAsync(string item, StreamSequenceToken token = null)
        {
            //System.Threading.Thread.Sleep(TimeSpan.FromHours(1));
            count++;
            Console.WriteLine($"{DateTime.Now.ToString("HH-dd-mm fff")}Received message:{item}");
            Console.WriteLine("\n{0}", count);
            return Task.CompletedTask;
        }

       
    }
}
