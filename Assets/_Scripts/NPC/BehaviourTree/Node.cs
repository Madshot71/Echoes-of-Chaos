using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

namespace GhostBoy.BehaviourTree
{
    public abstract class Node : IProcess
    {
        public bool Repeat {get; private set;}

        public Node parent{get; private set;}
        public Node[] children;
        public int currentIndex;

        public enum Status {Success = 0, Failure = -1, Running = 1}

        public Node(Node[] children)
        {
            this.children = children;
            currentIndex = 0;
        }

        public virtual Status Process()
        {
            return children[currentIndex].Process();
        }

        public void Reset()
        {
            currentIndex = 0;
            foreach (var item in children)
            {
                item.Reset();
            }
        }
    }

    public interface IProcess
    {
        Node.Status Process();
        void Reset(); 
    }

    public class Condition : Node 
    {
        public Node _true;
        public Node _false;
        Func<bool> _condition;

        public Condition(Node _true , Node _false , ref Func<bool> condition) : base(new Node[]{_true , _false})
        {
            this._true = _true;
            this._false = _false;
            this._condition = condition;
        }

        public override Status Process()
        {
            return _condition()? _true.Process() : _false.Process();
        }
    }
}