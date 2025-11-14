using System.Collections.Generic;
using System.Collections;

namespace GhostBoy.Mission
{
    public interface IMission<T>
    {
        public List<T> Targets { get; set; }
        public T measure { get; set; }
    } 
}
