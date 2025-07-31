import math
import time

from ma_dataplatforms_streaming_support_library.protos.open_data_pb2 import PeriodicDataPacket, DoubleSample, \
    DataStatus, SampleDataFormat, SampleColumn, DoubleSampleList


class PeriodicPacketGenerator:
    def __init__(self, frequency, data_format_id, first_timestamp):
        self.__interval = int(1E9 / frequency)
        self.__data_format_id = data_format_id
        self.__first_timestamp = first_timestamp

    def generate_packets(self) -> PeriodicDataPacket:
        double_samples = []
        x_value = 0
        intervals = (2 * math.pi) / 100
        sample_count = 100
        for i in range(sample_count):
            start_time = self.__generate_current_timestamp()
            double_samples.append(DoubleSample(value=math.sin(x_value), status=DataStatus.DATA_STATUS_VALID))
            end_time = self.__generate_current_timestamp()
            x_value += intervals
            time.sleep((self.__interval - (end_time - start_time)) / 1E9)

        packet = PeriodicDataPacket(data_format=SampleDataFormat(data_format_identifier=self.__data_format_id),
                                    start_time=self.__first_timestamp,
                                    interval=self.__interval,
                                    columns=[SampleColumn(double_samples=DoubleSampleList(samples=double_samples))])

        self.__first_timestamp += self.__interval * sample_count
        return packet

    @staticmethod
    def __generate_current_timestamp() -> int:
        return int(time.time() * 1E9)
