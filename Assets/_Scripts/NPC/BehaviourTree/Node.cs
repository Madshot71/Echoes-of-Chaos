using System;
using System.Collections.Generic;

namespace GhostBoy.BehaviourTree
{
    public enum Status { Success, Failure, Running }

    public abstract class Node
    {
        public Node parent { get; private set; }
        protected List<Node> children = new List<Node>();

        public Node()
        {
            parent = null;
        }

        public Node(List<Node> children)
        {
            foreach (Node child in children)
            {
                Attach(child);
            }
        }

        public void Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }
        
        public abstract Status Process();
        
        public virtual void Reset() { }
    }

    public class Sequence : Node
    {
        private int _currentNodeIndex = 0;

        public Sequence(List<Node> children) : base(children) { }

        public override Status Process()
        {
            while (_currentNodeIndex < children.Count)
            {
                var childStatus = children[_currentNodeIndex].Process();
                switch (childStatus)
                {
                    case Status.Failure:
                        Reset();
                        return Status.Failure;
                    case Status.Success:
                        _currentNodeIndex++;
                        continue;
                    case Status.Running:
                        return Status.Running;
                }
            }
            
            Reset();
            return Status.Success;
        }

        public override void Reset()
        {
            _currentNodeIndex = 0;
            foreach (var child in children)
            {
                child.Reset();
            }
        }
    }

    public class Selector : Node
    {
        private int _currentNodeIndex = 0;
        
        public Selector(List<Node> children) : base(children) { }

        public override Status Process()
        {
            while (_currentNodeIndex < children.Count)
            {
                var childStatus = children[_currentNodeIndex].Process();
                switch (childStatus)
                {
                    case Status.Failure:
                        _currentNodeIndex++;
                        continue;
                    case Status.Success:
                        Reset();
                        return Status.Success;
                    case Status.Running:
                        return Status.Running;
                }
            }
            
            Reset();
            return Status.Failure;
        }

        public override void Reset()
        {
            _currentNodeIndex = 0;
            foreach (var child in children)
            {
                child.Reset();
            }
        }
    }

    public class ActionNode : Node
    {
        private readonly Func<Status> _action;

        public ActionNode(Func<Status> action)
        {
            _action = action;
        }

        public override Status Process()
        {
            return _action();
        }
    }

    public class Condition : Node
    {
        private readonly Func<bool> _condition;

        public Condition(Func<bool> condition)
        {
            _condition = condition;
        }

        public override Status Process()
        {
            return _condition() ? Status.Success : Status.Failure;
        }
    }
}