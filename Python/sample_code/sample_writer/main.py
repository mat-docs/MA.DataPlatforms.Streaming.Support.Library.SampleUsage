import sys

from ma_dataplatforms_streaming_support_library.contracts.shared.stream_api_configuration import \
    StreamingApiConfiguration
from ma_dataplatforms_streaming_support_library.contracts.shared.stream_creation_strategy import StreamCreationStrategy
from ma_dataplatforms_streaming_support_library.core.base.logger import Logger
from ma_dataplatforms_streaming_support_library.core.base.support_library_bootstrapper import SupportLibraryBootstrapper

from sample_code.sample_writer.mock_data_writer import MockDataWriter


def main():
    # Create the streaming api configuration to connect to the broker.
    streaming_api_configuration = StreamingApiConfiguration("localhost:9092", StreamCreationStrategy.TOPIC_BASE, [], 10)

    # Create the logger for the support library. This logger can be the default that comes with the library,
    # Or a custom one that implements the ILogger interface.
    logger = Logger()

    # Bootstrap into the Support Library, connecting Python to C# api via FFI.
    # This provides the Support Library Factory to create the support Library.
    # ALWAYS do this before using the support library
    support_lib_api_factory = SupportLibraryBootstrapper.bootstrap(streaming_api_configuration, logger)

    # Create the support library and make sure to initialise and start it.
    # The support library object contains the calls needed to create the different services that's available.
    support_lib_api = support_lib_api_factory.create()
    support_lib_api.initialise()
    support_lib_api.start()

    # The Session Management Module Api allows you to create multiple Session Management services.
    session_management_module = support_lib_api.get_session_manager_api()
    # The Session Management Module Api returns success if the session management service is created successfully.
    session_management_response = session_management_module.create_service()
    if not session_management_response.success or session_management_response.data is None:
        logger.error("Unable to create session management service")
        sys.exit(1)

    # Always initialise and start each service.
    # The Session Management Service allows you to create, associate, end, update, and get sessions from the broker.
    session_management_service = session_management_response.data
    session_management_service.initialise()
    session_management_service.start()

    # Data Format Management Module Api allows you to create multiple separate data format manager services.
    data_format_management_module = support_lib_api.get_data_format_manager_api()

    # The Data Format Management Module Api gives you a response whether the service is created successfully.
    data_format_management_response = data_format_management_module.create_service()
    if not data_format_management_response.success or data_format_management_response.data is None:
        logger.error("Unable to create data format management service")
        sys.exit(1)

    # The Data Format Management Service allows you to create and read data format ids from the broker.
    data_format_management_service = data_format_management_response.data
    # Every service must be initialised and started before using it.
    data_format_management_service.initialise()
    data_format_management_service.start()

    # The Packet Writing Module Api allows you to create multiple Packet Writer Services.
    packet_writing_module = support_lib_api.get_writing_packet_api()
    # The Packet Writing Module Api sends a success if the packet writing service is successfully created.
    packet_writing_response = packet_writing_module.create_service()
    if not packet_writing_response.success or packet_writing_response.data is None:
        logger.error("Unable to create writing packet writing service")
        sys.exit(1)

    # The Packet Writing Service allows for writing data into the broker.
    packet_writing_service = packet_writing_response.data
    packet_writing_service.initialise()
    packet_writing_service.start()

    # The mock data writer is a Sample class that writes mock data to the stream.
    mock_data_writer = MockDataWriter(packet_writing_service, data_format_management_service,
                                      session_management_service, logger)

    mock_data_writer.create_start_write_and_end_mock_session()

    input("Press Enter to close...")
    session_management_service.stop()
    data_format_management_service.stop()
    packet_writing_service.stop()
    support_lib_api.stop()


if __name__ == '__main__':
    main()
