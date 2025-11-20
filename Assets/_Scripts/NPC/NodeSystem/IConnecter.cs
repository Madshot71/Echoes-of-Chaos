namespace GhostBoy.Nodes
{
    interface IConnector<T>
    {
        public T data {get; set;}
        public int max{get; set;}
    }
}