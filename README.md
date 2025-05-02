# Streaming Data Buffering and Interpolation Sample

This sample application demonstrates how to use the `MA.DataPlatforms.Streaming.Support.Library` for buffering and interpolating streaming data. It processes real-time data from a Stream API, performs buffering and interpolation, and stores the results in a SQL Race session for visualization and analysis.

## Features

- **Stream API Integration**: Listens to a Stream API (e.g., Kafka) for real-time data.
- **Data Buffering**: Efficiently buffers incoming data with configurable window settings.
- **Interpolation**: Provides first, last, mean, min, and max values of samples, with support for custom processors.
- **SQL Race Integration**: Stores processed data in a SQLite database for visualization and analysis.

## Main Components

### Buffering Module
Handles two types of data:
- **Sample Data**: Each `SampleData` object contains one parameter with multiple samples and timestamps.
- **Timestamp Data**: Each `TimestampData` object contains multiple parameters per timestamp.

### Interpolation Module
Processes the data to provide:
- First, Last, Mean, Min, and Max values of samples.
- Support for custom processors to extend interpolation logic.

## Configuration

The application includes several configurable components:

### `StreamingApiConfiguration`
Sets up the connection to the Stream API (e.g., Kafka broker).

### `PacketReadingConfig`
Configures how sessions are read from the broker.

### `BufferingConfiguration`
Controls buffering behavior with options like:
- Window length
- Sliding window percentage
- Data types to include (markers, events, errors, CAN data)

### `InterpolationConfiguration`
Controls interpolation behavior with:
- Subscription frequency
- Delivery frequency
- Custom processors

## Example Usage

The sample application:
1. Subscribes to specific parameters (e.g., `"vCar:Chassis"`, `"sLap:Chassis"`).
2. Sets up buffering with a 3000ms window.
3. Configures interpolation at 2Hz.
4. Stores results in a SQLite database.

## Data Flow

1. Data is ingested from the Stream API.
2. It is buffered according to the configured settings.
3. The buffered data is processed by the interpolation module.
4. Results are stored in SQL Race for visualization.

## Getting Started

### Prerequisites
- .NET runtime (version specified in the project)
- Access to a Stream API (e.g., Kafka broker)
- SQL Race for visualization (optional)

### Setup
1. Clone this repository.
2. Update the `StreamingApiConfiguration` to match your Stream API setup.
3. Modify the `PacketReadingConfig` to suit your needs.
4. Update the `subscribedParameters` list with the parameters you want to monitor.
5. Configure the buffering and interpolation settings in `BufferingConfiguration` and `InterpolationConfiguration`.

### Running the Application
1. Build the solution.
2. Run the application executable.

## Customization
You can extend the functionality by:
- Implementing custom interpolation processors.
- Adjusting buffering and interpolation settings in the configuration files.
- Adding new parameter subscriptions.
