import sys

from ma_dataplatforms_streaming_support_library.contracts.shared.stream_api_configuration import \
    StreamingApiConfiguration
from ma_dataplatforms_streaming_support_library.contracts.shared.stream_creation_strategy import StreamCreationStrategy
from ma_dataplatforms_streaming_support_library.core.base.logger import Logger
from ma_dataplatforms_streaming_support_library.core.base.support_library_bootstrapper import SupportLibraryBootstrapper

from sample_code.sample_writer.mock_data_writer import MockDataWriter


def main():
    streaming_api_configuration = StreamingApiConfiguration("localhost:9092", StreamCreationStrategy.TOPIC_BASE, [], 10)
    logger = Logger()
    support_lib_api_factory = SupportLibraryBootstrapper.bootstrap(streaming_api_configuration, logger)
    support_lib_api = support_lib_api_factory.create()

    support_lib_api.initialise()
    support_lib_api.start()

    session_management_module = support_lib_api.get_session_manager_api()
    session_management_response = session_management_module.create_service()
    if not session_management_response.success or session_management_response.data is None:
        logger.error("Unable to create session management service")
        sys.exit(1)

    session_management_service = session_management_response.data
    session_management_service.initialise()
    session_management_service.start()

    data_format_management_module = support_lib_api.get_data_format_manager_api()
    data_format_management_response = data_format_management_module.create_service()
    if not data_format_management_response.success or data_format_management_response.data is None:
        logger.error("Unable to create data format management service")
        sys.exit(1)

    data_format_management_service = data_format_management_response.data
    data_format_management_service.initialise()
    data_format_management_service.start()

    packet_writing_module = support_lib_api.get_writing_packet_api()
    packet_writing_response = packet_writing_module.create_service()
    if not packet_writing_response.success or packet_writing_response.data is None:
        logger.error("Unable to create writing packet writing service")
        sys.exit(1)

    packet_writing_service = packet_writing_response.data
    packet_writing_service.initialise()
    packet_writing_service.start()

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
