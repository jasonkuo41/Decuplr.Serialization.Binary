using System;
using System.Collections.Generic;
using System.Text;
using Decuplr.Serialization.Binary;
using MessagePack;

namespace Benchmark.Models {

    [MessagePackObject, BinaryFormat]
    public partial class SimplePoco {
        [Key(0)]
        public int OldAccountId { get; set; }

        [Key(1)]
        public int NewAccountId { get; set; }

        [Key(2)]
        public long InfoId { get; set; }

        [Key(3)]
        public DateTime LastChangeTime { get; set; }

        [Key(4)]
        public DateTime FinalChangeTime { get; set; }

        [Key(5)]
        public DateTime FastChangeTime { get; set; }

        [Key(6)]
        public ushort FinalCommentId { get; set; }
    }
}
