from ma_dataplatforms_streaming_support_library.contracts.packet_reading.coverage_cursor_info import CoverageCursorInfo
from ma_dataplatforms_streaming_support_library.contracts.packet_reading.i_packet_reader_service import \
    IPacketReaderService
from ma_dataplatforms_streaming_support_library.contracts.packet_reading.stream_info import StreamInfo
from ma_dataplatforms_streaming_support_library.contracts.session_management.session_association_info import \
    SessionAssociationInfo
from ma_dataplatforms_streaming_support_library.contracts.session_management.session_info import SessionInfo
from ma_dataplatforms_streaming_support_library.core.base.logger import ILogger


class SessionNotifier:
    def __init__(self, packet_reader_service: IPacketReaderService, logger: ILogger):
        self.__packet_reader_service = packet_reader_service
        self.__logger = logger

        # subscribe to support lib events
        self.__packet_reader_service.session_reading_started.subscribe(self.__on_session_reading_started)
        self.__packet_reader_service.session_reading_completed.subscribe(self.__on_session_reading_complete)
        self.__packet_reader_service.stream_reading_started.subscribe(self.__on_stream_reading_started)
        self.__packet_reader_service.stream_reading_completed.subscribe(self.__on_stream_reading_complete)
        self.__packet_reader_service.session_info_updated.subscribe(self.__on_session_info_updated)
        self.__packet_reader_service.coverage_cursor_received.subscribe(self.__on_coverage_cursor_received)
        self.__packet_reader_service.session_association_info_updated.subscribe(self.__on_session_association_updated)

    def unsubscribe_from_events(self):
        self.__packet_reader_service.session_reading_started.unsubscribe(self.__on_session_reading_started)
        self.__packet_reader_service.session_reading_completed.unsubscribe(self.__on_session_reading_complete)
        self.__packet_reader_service.stream_reading_started.unsubscribe(self.__on_stream_reading_started)
        self.__packet_reader_service.stream_reading_completed.unsubscribe(self.__on_stream_reading_complete)
        self.__packet_reader_service.session_info_updated.unsubscribe(self.__on_session_info_updated)
        self.__packet_reader_service.coverage_cursor_received.unsubscribe(self.__on_coverage_cursor_received)
        self.__packet_reader_service.session_association_info_updated.unsubscribe(self.__on_session_association_updated)

    def __on_session_reading_started(self, sender: object, session_info: SessionInfo):
        self.__logger.info(
            f"Session reading started for session {session_info.identifier} with session key {session_info.session_key}")

    def __on_session_reading_complete(self, sender: object, session_info: SessionInfo):
        self.__logger.info(
            f"Session reading completed for session {session_info.identifier} with session key {session_info.session_key}")

    def __on_stream_reading_started(self, sender: object, stream_info: StreamInfo):
        self.__logger.info(
            f"Stream reading started for session key {stream_info.session_key} with stream name {stream_info.stream}")

    def __on_stream_reading_complete(self, sender: object, stream_info: StreamInfo):
        self.__logger.info(
            f"Stream reading completed for session key {stream_info.session_key} with stream name {stream_info.stream}")

    def __on_session_info_updated(self, sender: object, session_info: SessionInfo):
        self.__logger.info(f"Session info updated to {session_info}")

    def __on_coverage_cursor_received(self, sender: object, coverage_cursor_info: CoverageCursorInfo):
        self.__logger.info(
            f"Coverage Cursor received with new timestamp at {coverage_cursor_info.coverage_cursor_time}")

    def __on_session_association_updated(self, sender: object, session_association_info: SessionAssociationInfo):
        self.__logger.info(f"Session Association updated to {session_association_info}")
