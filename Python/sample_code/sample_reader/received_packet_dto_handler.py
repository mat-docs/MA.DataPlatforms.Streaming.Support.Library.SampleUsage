from ma_dataplatforms_streaming_support_library.contracts.data_format_management.i_data_format_management_service import \
    IDataFormatManagementService
from ma_dataplatforms_streaming_support_library.contracts.packet_reading.received_packet_dto import ReceivedPacketDto
from ma_dataplatforms_streaming_support_library.contracts.shared.i_handler import IHandler
from ma_dataplatforms_streaming_support_library.core.base.logger import ILogger
from ma_dataplatforms_streaming_support_library.protos.open_data_pb2 import Packet, PeriodicDataPacket

# This is a sample handler class that handles the ReceivedPacketDto from the packet reader and then displays the
# sample values of a parameter on the console.
# Should you want to make a handler class like this one, make sure to implement the IHandler[ReceivedPacketDto] interface.
class ReceivedPacketDtoHandler(IHandler[ReceivedPacketDto]):
    def __init__(self, data_format_management_service: IDataFormatManagementService, logger: ILogger):
        self._logger = logger
        self._data_format_management_service = data_format_management_service
        self.__interested_parameter = "vCar:Chassis"

    def handle(self, packet: ReceivedPacketDto) -> None:
        # Create an empty packet object and fill the values from the packet bytes data.
        open_data_packet = Packet()
        open_data_packet.ParseFromString(packet.packet_bytes.data)

        # This one filters to only process periodic data. This can be changed as all data from the open data protocol is
        # available.
        if open_data_packet.type != "PeriodicData":
            return

        # Depending on the data type, we then parse it accordingly.
        periodic_packet = PeriodicDataPacket()
        periodic_packet.ParseFromString(open_data_packet.content)

        # Check to see if the packet has parameter identifiers included.
        if len(periodic_packet.data_format.parameter_identifiers.parameter_identifiers) > 0:
            parameters = periodic_packet.data_format.parameter_identifiers.parameter_identifiers
        else:
            # if not, we then use the data format identifier and ask the data format management service for the parameter
            # list.
            parameters_result = self._data_format_management_service.get_parameters_list(packet.data_source,
                                                                                         periodic_packet.data_format.data_format_identifier)
            if not parameters_result.success or parameters_result.data is None:
                self._logger.error("Failed to get parameters")
                return
            parameters = parameters_result.data.parameter_list

        assert len(parameters) == len(
            periodic_packet.columns), "length of parameters should be equal to number of columns"
        for i in range(len(parameters)):
            # check if the interested parameter is in the list.
            if parameters[i] != self.__interested_parameter:
                continue

            # If it is, then get the data column corresponding to that parameter.
            # Then calculate the timestamp based on the start time and the interval.
            data_column = periodic_packet.columns[i]
            if data_column.WhichOneof("list") == "double_samples":
                sample_list = data_column.double_samples.samples
                for j in range(len(sample_list)):
                    timestamp = periodic_packet.start_time + j * periodic_packet.interval
                    sample = sample_list[j]
                    self._logger.info(
                        f"Sample {self.__interested_parameter}: Timestamp {timestamp} -> {sample.value} Status: {sample}")
