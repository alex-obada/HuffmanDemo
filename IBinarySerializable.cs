using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanDemo
{
    internal interface IBinarySerializable
    {
        void Serialize(BinaryWriter writer);

        void Deserialize(BinaryReader reader);
    }

}

