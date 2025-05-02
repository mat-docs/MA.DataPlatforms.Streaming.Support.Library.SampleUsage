# Buffering and Interpolation Example
The following code shows how to use the buffering and interpolation modules of the Support Library.
This code will listen in the Stream API using the support library and plot the results of the Buffering and Interpolation of the subscribed parameters into a SQL Race session.

## Usage
In the `Program.cs` there are some settings that need to be modified depending on your Stream API setup.

Update the `StreamingApiConfiguration` to match your Stream API setup.

Update the `PacketReadingConfig` to suit how you want the support library to read the sessions from the broker.

Update the `subscribedParameters` list to a list of parameters you want to subscribe to.

## Buffering
There are two kinds of buffering available from the Support Library:

### Sample Data
This is where every `SampleData` object contains only one "Parameter" containing multiple samples and timestamps per parameter.
This "Parameter" is not limited to samples but can be used for Events, Markers, Can Data, and Errors. At which case, it will contain the number of occurences of which it occured.

An example `SampleDataHandler` is provided to show how you can handle this type of data.

### Timestamp Data
This is where every `TimestampData` object contains multiple "Parameters" per timestamp.
Like previously, each "Parameter" is not limited to samples but can be used for Events, Markers, Can Data, and Errors.

An example `TimestampDataHandler` is provided to show how to handle this type of data.

### Configuration
Update the `BufferingConfiguration` to setup how you want the buffering to work. Here are the options:

- `SubscribedParameters` - A list of parameters to subscribe to. If left null, then it will subscribe to ALL parameters.
- `BufferingWindowLength` - The window length in milliseconds which the buffering will begin. Default is 3000 ms.
- `SlidingWindowPercentage` - The percentage of the buffering window length which controls the interval at which data is sent. Default is 5%.
- `MergeStrategy` - A custom merge strategy can be injected here. If left null, then it will use the built in merge strategy.
- `IncludeMarkerData` - Whether to include marker data in the buffered stream. Default is false.
- `IncludeEventData` - Whether to include event data in the buffered stream. Default is false.
- `IncludeErrorData` - Whether to include error data in the buffered stream. Default is false.
- `IncludeCanData` - Whether to include CAN data in the buffered stream. Default is false.

Once configured, the buffering will only start when you call `BufferingSubscribe` or `BufferingSubscribeAll`.
You can unsubscribe from the buffering by calling `BufferingUnsubscribe` or `BufferingUnsubscribeAll`.

## Interpolation
Interpolation is done using a processor that implements the `ISubscriptionProcessor` and then the results will be described with a object that implements the `IProcessResult` interface.
In the Support Library, we have created a default processor that gives the First, Last, Mean, Min, and Max of a set of samples and give it to a handler which will handle those results.

An example `InterpolationResultHandler` is given to show how to handle the results from the default processor.
Should you wish to create you own processor, you will need a handler that will be able to handle the results from that processor.

Interpolation begins when the `InterpolationSubscribe` method is called. Here you can configure how each processor does its interpolation with the following options:
- `SubscriptionKey` - This is a unique key at which you can reference this interpolation.
- `ParameterIdentifiers` - A list of parameter identifiers to send to the interpolation processor.
- `SubscriptionFrequencyHz` - The frequency at which the subscription processor gets the data. Any data within the time period of this frequency will be sent to the processor.
- `Handler` - A object that implements the `IBatchResultHandler` which will handle the results of the processor.
- `DeliveryFrequencyHz` - The frequency at which data from the processor will be sent to the handler. Any data within the time period of this frequency will be sent to the handler. If left null, it will use the same frequency as the Subscription Frequency.
- `Processor` - A custom processor at which you can inject to do the interpolation. If left null, then it will use the default processor.

