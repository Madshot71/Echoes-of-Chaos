using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace GhostBoy.Nodes
{
    public class Addition : Node, IConnector<float>
    {
        public float data {get; set;}
        public int max { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Init()
        {
            
        }

        public override void Logic()
        {
            for (int i = 0; i < max - 1; i++)
            {
                data += inputs[i].Pass();
            }
        }

        protected override void Output(out float[] outputs)
        {
            //Run Logic and output data
            Logic();
            outputs = new float[] {data};
        }

    }
}