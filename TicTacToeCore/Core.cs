using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using TicTacToe.Core.Game;
using TicTacToe.Core.Packet;
using TicTacToe.Core.Packet.Clientbound;
using TicTacToe.Core.Packet.Serverbound;

namespace TicTacToe {
    namespace Core {
        public static class SocketUtil {
            public static void SendPacket(this Socket socket, IPacket packet) {
                MemoryStream packetData = new MemoryStream();
                PacketDataWriter packetDataWriter = new PacketDataWriter(new BinaryWriter(packetData));
                
                int packetDataLength = packetDataWriter.Write(packet);
                int totalLength = 2 * sizeof(int) + packetDataLength; // Packet id + Packet data length + Packet data
                
                packetDataWriter.Flush();
                byte[] data = packetData.GetBuffer();
                packetData.Close();
                
                byte[] packetBuffer = new byte[totalLength];
                packetData = new MemoryStream(packetBuffer);
                BinaryWriter packetWriter = new BinaryWriter(packetData);
                
                packetWriter.Write(PacketRegistry.GetId(packet));
                packetWriter.Write(packetDataLength);
                packetWriter.Write(data, 0, packetDataLength);

                socket.Send(packetBuffer);
            }

            public static IPacket ReceivePacket(this Socket socket, Direction direction) {
                BinaryReader reader = socket.Reader();

                int packetId = reader.ReadInt32();
                
                IPacket packet = PacketRegistry.FromId(packetId, direction);
                
                int dataLength = reader.ReadInt32();

                byte[] dataBuffer = reader.ReadBytes(dataLength);
                
                MemoryStream memoryStream = new MemoryStream(dataBuffer, false);
                packet.Read(new PacketDataReader(new BinaryReader(memoryStream)));

                return packet;
            }

            public static BinaryReader Reader(this Socket socket, int bufferSize = 256) {
                if (bufferSize <= 0) {
                    throw new ArgumentException($"{nameof(bufferSize)} must be > 0");
                }

                MemoryStream memoryStream = new MemoryStream();

                byte[] buffer = new byte[bufferSize];
                int read;

                do {
                    read = socket.Receive(buffer);
                    memoryStream.Write(buffer, 0, read);
                } while (read > 256);

                memoryStream.Position = 0;

                return new BinaryReader(memoryStream);
            }
        }

        namespace Game {
            public enum PlayerType : byte {
                X,
                O,
                None
            }
        }

        namespace Packet {

            public enum Protocol {
                Handshake,
                Play
            }

            public enum Direction {
                Clientbound,
                Serverbound
            }

            public class Packet : Attribute {
                private readonly Direction _direction;
                private readonly Protocol _protocol;

                public Packet(Direction direction, Protocol protocol) {
                    _direction = direction;
                    _protocol = protocol;
                }

                public Direction Direction => _direction;
                
                public Protocol Protocol => _protocol;
            }

            public static class PacketUtil {
                public static Direction GetDirection(this IPacket packet) {
                    Attribute[] attributes = Attribute.GetCustomAttributes(packet.GetType());

                    foreach (Attribute attribute in attributes) {
                        if (attribute is Packet packetInfo) {
                            return packetInfo.Direction;
                        }
                    }

                    throw new Exception($"{packet.GetType().FullName} does not have a {typeof(Packet).FullName} attribute!");
                }
            }
            
            public class PacketDataReader {
                private BinaryReader _reader;

                public PacketDataReader(BinaryReader reader) {
                    _reader = reader;
                }

                public BinaryReader Reader => _reader;

                public byte ReadByte() => _reader.ReadByte();

                public short ReadShort() => _reader.ReadInt16();

                public int ReadInt() => _reader.ReadInt32();

                public long ReadLong() => _reader.ReadInt64();

                public string ReadString() => _reader.ReadString();

                public byte[] ReadByteArray(int count) => _reader.ReadBytes(count);

                public T Read<T>() where T : IPacket, new() {
                    T t = new T();

                    t.Read(this);

                    return t;
                }
            }

            public class PacketDataWriter {
                private BinaryWriter _writer;

                public PacketDataWriter(BinaryWriter writer) {
                    _writer = writer;
                }

                public int Write(byte b) {
                    _writer.Write(b);
                    
                    return sizeof(byte);
                }

                public int Write(short s) {
                    _writer.Write(s);
                    
                    return sizeof(short);
                }

                public int Write(int i) {
                    _writer.Write(i);

                    return sizeof(int);
                }

                public int Write(long l) {
                    _writer.Write(l);

                    return sizeof(long);
                }

                public int Write(byte[] bytes) {
                    _writer.Write(bytes);

                    return bytes.Length;
                }

                public int Write(string s) => Write(Encoding.UTF8.GetBytes(s));

                public int Write<T>(T t) where T : IPacket => t.Write(this);

                public void Flush() => _writer.Flush();
            }

            public static class PacketRegistry {
                private static readonly IDictionary<Direction, IDictionary<Type, int>> TypeToId =
                    new ReadOnlyDictionary<Direction, IDictionary<Type, int>>(new Dictionary<Direction, IDictionary<Type, int>> {
                            {
                                Direction.Clientbound, new Dictionary<Type, int> {
                                    {typeof(PacketS2CJoinGame), 0x00},
                                    {typeof(PacketS2CAssignTurn), 0x01},
                                }
                            },

                            {
                                Direction.Serverbound, new Dictionary<Type, int> {
                                    {typeof(PacketC2SHello), 0x00},
                                }
                            }
                        }
                    );
                private static readonly IDictionary<Direction, IDictionary<int, Type>> IdToType = 
                    new ReadOnlyDictionary<Direction, IDictionary<int, Type>>( new Dictionary<Direction, IDictionary<int, Type>> {
                        {
                            Direction.Clientbound, Reverse(TypeToId[Direction.Clientbound])
                        },
                        {
                            Direction.Serverbound, Reverse(TypeToId[Direction.Serverbound])
                        }
                    });

                public static int GetId(IPacket packet) => TypeToId[packet.GetDirection()][packet.GetType()];

                public static IPacket FromId(int id, Direction direction) {
                    return (IPacket) IdToType[direction][id].GetConstructor(new Type[0])
                        ?.Invoke(new object[0]);
                }

                private static IDictionary<TB, TA> Reverse<TA, TB>(IDictionary<TA, TB> dict) {
                    return dict.ToDictionary(pair => pair.Value, pair => pair.Key);
                }
            }

            public interface IPacket {
                void Read(PacketDataReader reader);

                int Write(PacketDataWriter writer);
            }

            namespace Serverbound {
                
                [Packet(Direction.Serverbound, Protocol.Handshake)]
                public class PacketC2SHello : IPacket {
                    private string _playerName;

                    public string PlayerName {
                        get => _playerName;
                        set => _playerName = value;
                    }

                    public PacketC2SHello() { }

                    public PacketC2SHello(string playerName) {
                        _playerName = playerName;
                    }

                    public void Read(PacketDataReader reader) => _playerName = reader.ReadString();

                    public int Write(PacketDataWriter writer) => writer.Write(_playerName);
                }

                [Packet(Direction.Serverbound, Protocol.Play)]
                public class PacketC2SDoTurn : IPacket {
                    private int _field;

                    public PacketC2SDoTurn(int field) {
                        _field = field;
                    }

                    public PacketC2SDoTurn() {
                        
                    }

                    public int Field {
                        get => _field;
                        set => _field = value;
                    }

                    public void Read(PacketDataReader reader) => _field = reader.ReadInt();

                    public int Write(PacketDataWriter writer) => writer.Write(_field);
                }
            }

            namespace Clientbound {
                [Packet(Direction.Clientbound, Protocol.Handshake)]
                public class PacketS2CJoinGame : IPacket {
                    private PlayerType _type;

                    public PacketS2CJoinGame() { }

                    public PacketS2CJoinGame(PlayerType type) {
                        _type = type;
                    }

                    public PlayerType Type {
                        get => _type;
                        set => _type = value;
                    }

                    public void Read(PacketDataReader reader) => _type = (PlayerType) reader.ReadByte();

                    public int Write(PacketDataWriter writer) => writer.Write((byte) _type);
                }

                [Packet(Direction.Clientbound, Protocol.Play)]
                public class PacketS2CAssignTurn : IPacket {
                    
                    private int[] _availableFields;

                    public PacketS2CAssignTurn(int[] availableFields) {
                        _availableFields = availableFields;
                    }

                    public PacketS2CAssignTurn() {
                        
                    }

                    public int[] AvailableFields {
                        get => _availableFields;
                        set => _availableFields = value;
                    }

                    public void Read(PacketDataReader reader) {
                        _availableFields = new int[reader.ReadInt()];

                        for (int i = 0; i < _availableFields.Length; i++) {
                            _availableFields[i] = reader.ReadInt();
                        }
                    }

                    public int Write(PacketDataWriter writer) {
                        int written = writer.Write(_availableFields.Length);

                        foreach (int availableField in _availableFields) {
                            written += writer.Write(availableField);
                        }

                        return written;
                    }
                }
            }
        }
    }
}