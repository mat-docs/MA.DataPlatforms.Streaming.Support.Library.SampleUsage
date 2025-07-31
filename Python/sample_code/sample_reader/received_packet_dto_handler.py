from ma_dataplatforms_streaming_support_library.contracts.data_format_management.i_data_format_management_service import \
    IDataFormatManagementService
from ma_dataplatforms_streaming_support_library.contracts.packet_reading.received_packet_dto import ReceivedPacketDto
from ma_dataplatforms_streaming_support_library.contracts.shared.i_handler import IHandler
from ma_dataplatforms_streaming_support_library.core.base.logger import ILogger
from ma_dataplatforms_streaming_support_library.protos.open_data_pb2 import Packet, PeriodicDataPacket


class ReceivedPacketDtoHandler(IHandler[ReceivedPacketDto]):
    def __init__(self, data_format_management_service: IDataFormatManagementService, logger: ILogger):
        self._logger = logger
        self._data_format_management_service = data_format_management_service
        self.__interested_parameter = "vCar:Chassis"

    def handle(self, packet: ReceivedPacketDto) -> None:
        open_data_packet = Packet()
        open_data_packet.ParseFromString(packet.packet_bytes.data)

        if open_data_packet.type != "PeriodicData":
            return

        periodic_packet = PeriodicDataPacket()
        periodic_packet.ParseFromString(open_data_packet.content)

        if len(periodic_packet.data_format.parameter_identifiers.parameter_identifiers) > 0:
            parameters = periodic_packet.data_format.parameter_identifiers.parameter_identifiers
        else:
            parameters_result = self._data_format_management_service.get_parameters_list(packet.data_source,
                                                                                         periodic_packet.data_format.data_format_identifier)
            if not parameters_result.success or parameters_result.data is None:
                self._logger.error("Failed to get parameters")
                return
            parameters = parameters_result.data.parameter_list

        assert len(parameters) == len(
            periodic_packet.columns), "length of parameters should be equal to number of columns"
        for i in range(len(parameters)):
            if parameters[i] != self.__interested_parameter:
                continue

            data_column = periodic_packet.columns[i]
            if data_column.WhichOneof("list") == "double_samples":
                sample_list = data_column.double_samples.samples
                for j in range(len(sample_list)):
                    timestamp = periodic_packet.start_time + j * periodic_packet.interval
                    sample = sample_list[j]
                    self._logger.info(
                        f"Sample {self.__interested_parameter}: Timestamp {timestamp} -> {sample.value} Status: {sample}")
