using UnityEngine;

namespace GhostBoy.Nodes
{
    public class Port
    {
        public Node endPoint;
        public Node main {get; private set;}
        public PortType type;
        private int index;
    

        public enum PortType
        {
            Input,
            Output
        }

        public void Init(Node main , int index)
        {
            this.main = main;
            this.index = index;
        }

        public void Connnect(Node endPoint)
        {
            this.endPoint = endPoint;
        }

        public float Pass()
        {
            return 0;
        }
    }
}