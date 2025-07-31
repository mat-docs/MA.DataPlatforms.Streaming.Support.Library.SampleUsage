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
    streaming_api_configuration = StreamingApiConfiguration("localhost:9092", StreamCreationStrategy.TOPIC_BASE, [], 10)
    logger = Logger()
    support_lib_api_factory = SupportLibraryBootstrapper.bootstrap(streaming_api_configuration, logger)
    support_lib = support_lib_api_factory.create()
    support_lib.initialise()
    support_lib.start()

    data_format_management_module_api = support_lib.get_data_format_manager_api()
    data_format_management_service_response = data_format_management_module_api.create_service()
    if not data_format_management_service_response.success or data_format_management_service_response.data is None:
        logger.info("Failed to create data format management service")
        sys.exit(1)

    data_format_management_service = data_format_management_service_response.data
    data_format_management_service.initialise()
    data_format_management_service.start()

    packet_reader_service_module_api = support_lib.get_reading_packet_api()

    handler = ReceivedPacketDtoHandler(data_format_management_service, logger)

    recording_type = input("Would you like to record live or historic data? (L/H)").upper()
    if recording_type == "L":
        packet_reading_config = PacketReadingConfiguration("", 10, False, "Default", "*", PacketReadingType.LIVE, [])
        packet_reader_service_response = packet_reader_service_module_api.create_service_with_config(
            packet_reading_config)
        if not packet_reader_service_response.success or packet_reader_service_response.data is None:
            logger.info("Failed to create packet reader service")
            sys.exit(1)

        packet_reader = packet_reader_service_response.data

        packet_reader.initialise()
        packet_reader.start()
        packet_reader.set_handler(handler)

        session_notifier = SessionNotifier(packet_reader, logger)

        input("Awaiting Live Sessions... Press Enter to Exit...")
        session_notifier.unsubscribe_from_events()
        packet_reader.stop()
        packet_reader.remove_handler(handler)
    elif recording_type == "H":
        session_management_module = support_lib.get_session_manager_api()
        session_management_service_response = session_management_module.create_service()
        if not session_management_service_response.success or session_management_service_response.data is None:
            logger.error("Failed to create session management service")
            sys.exit(1)

        session_management_service = session_management_service_response.data
        session_management_service.initialise()
        session_management_service.start()

        sessions_response = session_management_service.get_all_sessions()
        if not sessions_response.success or sessions_response.data is None:
            logger.error("Failed to get sessions")
            sys.exit(1)

        sessions = sessions_response.data
        print("The following sessions are available:")
        for i in range(len(sessions)):
            print(f"[{i}] {sessions[i].identifier}")

        session_index_to_record = input("Which session would you like to record? Enter it's index number:")
        session_to_record = sessions[int(session_index_to_record)]
        if session_to_record is None:
            logger.error("Index invalid! Exiting...")
            sys.exit(1)

        packet_reader_response = packet_reader_service_module_api.create_service(session_to_record.data_source,
                                                                                 session_to_record.session_key)
        if not packet_reader_response.success or packet_reader_response.data is None:
            logger.error("Failed to create packet reader service")
            sys.exit(1)
        packet_reader_service = packet_reader_response.data
        packet_reader_service.initialise()
        packet_reader_service.start()
        packet_reader_service.set_handler(handler)
        session_notifier = SessionNotifier(packet_reader_service, logger)
        input("Awaiting Historic Session to Complete... Press Enter to Exit...")
        session_notifier.unsubscribe_from_events()
        packet_reader_service.stop()
        packet_reader_service.remove_handler(handler)
        session_management_service.stop()

    data_format_management_service.stop()
    support_lib.stop()


if __name__ == '__main__':
    main()
