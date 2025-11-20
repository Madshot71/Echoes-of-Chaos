using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

namespace GhostBoy.Nodes
{
    public abstract class Node : MonoBehaviour 
    {
        public NodeConfig defaults;
        public List<Port> inputs = new List<Port>();
        public List<Port> outputs = new List<Port>();

        private void Awake()
        {
            Init();
        }

        public abstract void Init();
        public abstract void Logic();
        
        protected abstract void Output(out float[] outputs);

        public float GetData(int index)
        {
            Output(out float[] data);

            if(index > data.Length || index < 0)
            {
                throw new ArgumentOutOfRangeException("the value you are trying to get is out of Range of the curent outs");
            }
            return data[index];
        }
        
    }
    
    [CreateAssetMenu(fileName = "Node", menuName = "Node", order = 0)]
    public class NodeConfig : ScriptableObject 
    {
        public PlaceHolder placeHolder;
    }
}


