using System;
using System.Collections.Generic;

namespace projeto.Model
{
    public partial class Imagen
    {
        public int Id { get; set; }
        public string? Uri { get; set; }
        public string? Title { get; set; }
        public byte[] Bytes { get; set; } = null!;
    }
}
