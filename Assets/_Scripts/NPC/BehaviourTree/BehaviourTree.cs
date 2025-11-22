using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace GhostBoy.BehaviourTree
{
    public class BehaviourTree<T>
    {
        T main;

        Node root;

        public BehaviourTree(T main , Node root)
        {
            this.main = main;
            this.root = root;
        }


        private void Start()
        {
            
        }

        private void Update()
        {
            
        }


    }
}