class PacketIdGenerator:
    def __init__(self):
        self.__packet_id = 0

    def get_packet_id(self) -> int:
        packet_id = self.__packet_id
        self.__packet_id += 1
        return packet_id