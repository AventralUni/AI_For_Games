using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class Tree : MonoBehaviour
    {
        //reference to a root that is a node that itself recurrsively contains the entire tree
        private Node root = null;

        //upon start the tree class will build the behaviour tree 
        protected void Start()
        {
            root = SetUpTree();
        }

        // if it has a tree it will evaluate
        private void Update()
        {
            if (root != null)
            {
                root.Evaluate();
            }
        }

        protected abstract Node SetUpTree();
     




    }

}
