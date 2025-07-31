import datetime
import time
from typing import Optional, List

from ma_dataplatforms_streaming_support_library.contracts.data_format_management.i_data_format_management_service import \
    IDataFormatManagementService
from ma_dataplatforms_streaming_support_library.contracts.packet_writing.i_packet_writer_service import \
    IPacketWriterService
from ma_dataplatforms_streaming_support_library.contracts.packet_writing.info_type import InfoType
from ma_dataplatforms_streaming_support_library.contracts.packet_writing.packet_bytes import PacketBytes
from ma_dataplatforms_streaming_support_library.contracts.session_management.i_session_management_service import \
    ISessionManagementService
from ma_dataplatforms_streaming_support_library.contracts.session_management.session_creation_info import \
    SessionCreationInfo
from ma_dataplatforms_streaming_support_library.contracts.session_management.session_info import SessionInfo
from ma_dataplatforms_streaming_support_library.contracts.shared.api_result import ApiResult
from ma_dataplatforms_streaming_support_library.core.base.logger import ILogger
from ma_dataplatforms_streaming_support_library.protos.open_data_pb2 import Packet, MarkerPacket, SessionInfoPacket, \
    ParameterDefinition, DataType, GroupDefinition, ConfigurationPacket, NewSessionPacket, EndOfSessionPacket

from sample_code.sample_writer.packet_id_generator import PacketIdGenerator
from sample_code.sample_writer.periodic_packet_generator import PeriodicPacketGenerator


class MockDataWriter:
    def __init__(self, packet_writer_service: IPacketWriterService,
                 data_format_management_service: IDataFormatManagementService,
                 session_management_service: ISessionManagementService,
                 logger: ILogger):
        self.__packet_writer_service = packet_writer_service
        self.__data_format_management_service = data_format_management_service
        self.__session_management_service = session_management_service
        self.__logger = logger
        self.__packet_id_generator = PacketIdGenerator()
        self.__streams = ["", "Stream1"]
        self.__number_of_packets = 100

    def create_start_write_and_end_mock_session(self):
        # Create the session
        timedelta_offset = datetime.datetime.now().astimezone().utcoffset()
        session_creation_info = SessionCreationInfo("Default", "SupportLibrarySession", "Session", 1,
                                                    timedelta_offset, [], [])
        session_info_response = self.__create_session(session_creation_info)
        if not session_info_response.success or session_info_response.data is None:
            self.__logger.error("Failed to create new session")
            return

        session_info = session_info_response.data

        # Start the session by sending NewSessionPackets to each stream you want to write to
        self.__start_session(session_info)

        # Send config packet to the stream (needed for the stream api recorder)
        config_packet = self.__create_config_packet()
        success = self.__create_and_send_packet(
            Packet(type="Configuration", session_key=session_info.session_key, is_essential=True,
                   content=config_packet.SerializeToString(), id=self.__packet_id_generator.get_packet_id()),
            session_info.data_source,
            session_info.session_key, "")

        if not success:
            self.__logger.error("Failed to send config packet")
            self.__end_session(session_info.data_source, session_info.session_key)
            return

        # Create a data format id for your parameter
        data_format_id_response = self.__data_format_management_service.get_parameter_data_format_id("Default",
                                                                                                     ["Sin:MyApp"])

        if not data_format_id_response.success or data_format_id_response.data is None:
            self.__logger.error("Failed to get data format ID")
            self.__end_session(session_info.data_source, session_info.session_key)
            return

        data_format_id = data_format_id_response.data.data_format_id

        first_timestamp = int(time.time() * 1E9)

        # Send an Out Lap so ATLAS can properly overlay it over other sessions
        lap_packet = MarkerPacket(timestamp=first_timestamp, label="Out Lap", type="Lap Trigger",
                                  description="Out Lap Marker", source="0", value=1)
        self.__create_and_send_packet(
            Packet(type="Marker", session_key=session_info.session_key, is_essential=False,
                   content=lap_packet.SerializeToString(), id=self.__packet_id_generator.get_packet_id()),
            session_info.data_source,
            session_info.session_key, "")

        # It's possible to update session information after the session is created.
        new_session_detail = SessionInfoPacket(data_source=session_info.data_source, identifier=session_info.identifier,
                                               type=session_info.type, version=session_info.version,
                                               details={"Test Detail": "Test Value"})
        self.__create_and_send_session_info_packet(
            Packet(type="SessionInfo", session_key=session_info.session_key, is_essential=False,
                   content=new_session_detail.SerializeToString(), id=self.__packet_id_generator.get_packet_id()))

        # Generate data
        periodic_packet_generator = PeriodicPacketGenerator(100, data_format_id, first_timestamp)

        for i in range(self.__number_of_packets):
            periodic_packet = periodic_packet_generator.generate_packets()
            self.__create_and_send_packet(
                Packet(type="PeriodicData", session_key=session_info.session_key, is_essential=False,
                       content=periodic_packet.SerializeToString(), id=self.__packet_id_generator.get_packet_id()),
                session_info.data_source, session_info.session_key, "Stream1")

        # End session by sending an EndSessionPackets to each stream you have written to and then calling end session to the session itself.
        self.__end_session(session_info.data_source, session_info.session_key)

    @staticmethod
    def __create_timestamps(number_of_samples: int, first_timestamp: int, frequency: float) -> List[int]:
        timestamps = []
        period = 1E9 / frequency
        for i in range(number_of_samples):
            timestamps.append(int(first_timestamp + i * period))

        return timestamps

    @staticmethod
    def __create_config_packet() -> Packet:
        parameter_definition = ParameterDefinition(identifier="Sin:MyApp", name="Sin", application_name="MyApp",
                                                   description="Sine Wave", groups=[], units="test unit",
                                                   data_type=DataType.DATA_TYPE_FLOAT64, format_string="%5.2f",
                                                   min_value=-1, max_value=1, warning_max_value=-1, warning_min_value=1,
                                                   frequencies=[100], includes_row_data=False,
                                                   includes_synchro_data=False, conversion=None, formula=None)
        group_definition = GroupDefinition(identifier="MyApp", name="MyApp", application_name="MyApp",
                                           description="MyApp", groups=[])
        return ConfigurationPacket(config_id="ConfigPacket", parameter_definitions=[parameter_definition],
                                   group_definitions=[group_definition])

    def __create_and_send_packet(self, packet: Packet, data_source: str, session_key: str, stream: str) -> bool:
        packet_bytes = PacketBytes(packet.SerializeToString())
        result = self.__packet_writer_service.write_data(data_source, stream, session_key, packet_bytes)
        return result.success

    def __create_and_send_session_info_packet(self, session_info: Packet) -> bool:
        packet_bytes = PacketBytes(session_info.SerializeToString())
        result = self.__packet_writer_service.write_info(packet_bytes, InfoType.SESSION_INFO)
        return result.success

    def __create_session(self, session_creation_info: SessionCreationInfo) -> ApiResult[Optional[SessionInfo]]:
        return self.__session_management_service.create_new_session(session_creation_info)

    def __start_session(self, session_info: SessionInfo) -> None:
        new_session_packet = NewSessionPacket(data_source=session_info.data_source, utc_offset=session_info.utc_offset,
                                              session_info=SessionInfoPacket(identifier=session_info.identifier,
                                                                             type=session_info.type,
                                                                             version=session_info.version,
                                                                             details=session_info.details,
                                                                             associate_session_keys=session_info.associate_session_keys))
        for stream in self.__streams:
            self.__create_and_send_packet(
                Packet(type="NewSession", session_key=session_info.session_key, is_essential=False,
                       content=new_session_packet.SerializeToString(), id=self.__packet_id_generator.get_packet_id()),
                session_info.data_source,
                session_info.session_key, stream)

    def __end_session(self, data_source: str, session_key: str) -> bool:
        end_session_packet = EndOfSessionPacket(data_source=data_source)
        for stream in self.__streams:
            self.__create_and_send_packet(
                Packet(type="EndOfSession", session_key=session_key, is_essential=False,
                       content=end_session_packet.SerializeToString(), id=self.__packet_id_generator.get_packet_id()),
                data_source, session_key, stream)

        response = self.__session_management_service.end_session(data_source, session_key)
        return response.success
