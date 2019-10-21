using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TicTacToe.Core.Packet.Clientbound;
using TicTacToe.Core.Packet.Serverbound;

namespace TicTacToe.Core.Packet {
    public static class PacketRegistry {
        static PacketRegistry() {
            IDictionary<int, Type> idToTypeClientbound = new Dictionary<int, Type>();
            IDictionary<int, Type> idToTypeServerbound = new Dictionary<int, Type>();

            void Register(BasePacket packet) {
                switch (packet.Direction) {
                    case Direction.Clientbound: {
                        idToTypeClientbound[packet.PacketId] = packet.GetType();
                        break;
                    }

                    case Direction.Serverbound: {
                        idToTypeServerbound[packet.PacketId] = packet.GetType();
                        break;
                    }
                }
            }
            
            Register(new PacketS2CJoinGame());
            Register(new PacketS2CAssignTurn());
            Register(new PacketS2CBoardUpdate());
            Register(new PacketS2CGameOver());
            
            Register(new PacketC2SHello());
            Register(new PacketC2SDoTurn());

            IDictionary<Direction, IDictionary<int, Type>> final = new Dictionary<Direction, IDictionary<int, Type>>();
            
            final[Direction.Clientbound] = new ReadOnlyDictionary<int, Type>(idToTypeClientbound);
            final[Direction.Serverbound] = new ReadOnlyDictionary<int, Type>(idToTypeServerbound);
            
            IdToType = new ReadOnlyDictionary<Direction, IDictionary<int, Type>>(final);
        }
        
        private static readonly IDictionary<Direction, IDictionary<int, Type>> IdToType;

        public static BasePacket FromId(int id, Direction direction) {
            return (BasePacket) IdToType[direction][id].GetConstructor(new Type[0])
                ?.Invoke(new object[0]);
        }
    }
}