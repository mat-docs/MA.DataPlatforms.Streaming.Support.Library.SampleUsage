import sys

from ma_dataplatforms_streaming_support_library.contracts.packet_reading.packet_reading_configuration import \
    PacketReadingConfiguration
from ma_dataplatforms_streaming_support_library.contracts.packet_reading.packet_reading_type import PacketReadingType
from ma_dataplatforms_streaming_support_library.contracts.shared.stream_api_configuration import \
    StreamingApiConfiguration
from ma_dataplatforms_streaming_support_library.contracts.shared.stream_creation_strategy import StreamCreationStrategy
from ma_dataplatforms_streaming_support_library.core.base.logger import Logger
from ma_dataplatforms_streaming_support_library.core.base.support_library_bootstrapper import SupportLibraryBootstrapper

from sample_code.sample_reader.received_packet_dto_handler import ReceivedPacketDtoHandler
from sample_code.sample_reader.session_notifier import SessionNotifier


def main():
    # Create the streaming api configuration to connect to the broker.
    streaming_api_configuration = StreamingApiConfiguration("localhost:9094", StreamCreationStrategy.TOPIC_BASE, [], 10)

    # Create the logger for the support library. This logger can be the default that comes with the library,
    # Or a custom one that implements the ILogger interface.
    logger = Logger()

    # Bootstrap into the Support Library, connecting Python to C# api via FFI.
    # This provides the Support Library Factory to create the support Library.
    # ALWAYS do this before using the support library
    support_lib_api_factory = SupportLibraryBootstrapper.bootstrap(streaming_api_configuration, logger)

    # Create the support library and make sure to initialise and start it.
    # The support library object contains the calls needed to create the different services that's available.
    support_lib = support_lib_api_factory.create()
    support_lib.initialise()
    support_lib.start()

    # Data Format Management Module Api allows you to create multiple separate data format manager services.
    data_format_management_module_api = support_lib.get_data_format_manager_api()

    # The Data Format Management Module Api gives you a response whether the service is created successfully.
    data_format_management_service_response = data_format_management_module_api.create_service()
    if not data_format_management_service_response.success or data_format_management_service_response.data is None:
        logger.info("Failed to create data format management service")
        sys.exit(1)

    # The Data Format Management Service allows you to create and read data format ids from the broker.
    data_format_management_service = data_format_management_service_response.data

    # Every service must be initialised and started before using it.
    data_format_management_service.initialise()
    data_format_management_service.start()

    # The Packet Reader Service Module Api allows you to create multiple Packet Reader Services.
    packet_reader_service_module_api = support_lib.get_reading_packet_api()

    # To use the packet reader, you need to provide an object that can handle the ReceivedPacketDto object
    # and handle the open data objects contained within.
    # More info in the sample ReceivedPacketDtoHandler class.
    handler = ReceivedPacketDtoHandler(data_format_management_service, logger)

    recording_type = input("Would you like to record live or historic data? (L/H)").upper()
    if recording_type == "L":
        # Packet Reader Config can be setup to wait for live sessions. This is how you set that up.
        # The Packet Reader could also be set to a specific session key or session name if needed.
        packet_reading_config = PacketReadingConfiguration("", 10, False, "Default", "*", PacketReadingType.LIVE, [])

        # The Packet Reader Service Module Api gives you a response if the Packet Reader Service is created succesfully.
        packet_reader_service_response = packet_reader_service_module_api.create_service_with_config(
            packet_reading_config)
        if not packet_reader_service_response.success or packet_reader_service_response.data is None:
            logger.info("Failed to create packet reader service")
            sys.exit(1)

        packet_reader = packet_reader_service_response.data

        # Always initialise and start the service before using them.
        packet_reader.initialise()
        packet_reader.start()

        # Make sure to set a handler for the packet reader to send the data to.
        packet_reader.set_handler(handler)

        # This is a sample SessionNotifier class that reads the events given by the packet reader service and
        # logs the output to console.
        session_notifier = SessionNotifier(packet_reader, logger)

        input("Awaiting Live Sessions... Press Enter to Exit...")
        # Once done using the service, unsubscribe from any events from that service, stop it, and remove the handler.
        # This helps reduce memory leaks.
        session_notifier.unsubscribe_from_events()
        packet_reader.stop()
        packet_reader.remove_handler(handler)

    elif recording_type == "H":
        # The Session Management Module Api allows you to create multiple Session Management services.
        session_management_module = support_lib.get_session_manager_api()
        # The Session Management Module Api returns success if the session management service is created successfully.
        session_management_service_response = session_management_module.create_service()
        if not session_management_service_response.success or session_management_service_response.data is None:
            logger.error("Failed to create session management service")
            sys.exit(1)

        # Always initialise and start each service.
        # The Session Management Service allows you to create, associate, end, update, and get sessions from the broker.
        session_management_service = session_management_service_response.data
        session_management_service.initialise()
        session_management_service.start()

        # This grabs all the sessions in the broker. It returns success if the call was successful to the broker.
        sessions_response = session_management_service.get_all_sessions()
        if not sessions_response.success or sessions_response.data is None:
            logger.error("Failed to get sessions")
            sys.exit(1)

        # Each session is a SessionInfo class that contain information such as session key, type, and identifier.
        sessions = sessions_response.data
        print("The following sessions are available:")
        for i in range(len(sessions)):
            print(f"[{i}] {sessions[i].identifier}")

        session_index_to_record = input("Which session would you like to record? Enter it's index number:")
        session_to_record = sessions[int(session_index_to_record)]
        if session_to_record is None:
            logger.error("Index invalid! Exiting...")
            sys.exit(1)

        # The packet reader can be set to record a specific session key and data source if required.
        packet_reader_response = packet_reader_service_module_api.create_service(session_to_record.data_source,
                                                                                 session_to_record.session_key)
        if not packet_reader_response.success or packet_reader_response.data is None:
            logger.error("Failed to create packet reader service")
            sys.exit(1)

        # Always initialise and Start services.
        packet_reader_service = packet_reader_response.data
        packet_reader_service.initialise()
        packet_reader_service.start()

        # Set the handler once the packet reader is started.
        packet_reader_service.set_handler(handler)

        # This is a sample SessionNotifier class that reads the events given by the packet reader service and
        # logs the output to console.
        session_notifier = SessionNotifier(packet_reader_service, logger)
        input("Awaiting Historic Session to Complete... Press Enter to Exit...")
        # Once done using the service, unsubscribe from any events from that service, stop it, and remove the handler.
        # This helps reduce memory leaks.
        session_notifier.unsubscribe_from_events()
        packet_reader_service.stop()
        packet_reader_service.remove_handler(handler)
        session_management_service.stop()

    # Once done using the service, unsubscribe from any events from that service and stop it.
    # This helps reduce memory leaks.
    data_format_management_service.stop()
    support_lib.stop()


if __name__ == '__main__':
    main()
