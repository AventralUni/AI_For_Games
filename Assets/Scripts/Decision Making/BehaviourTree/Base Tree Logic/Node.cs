using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    //single element in the tree and can access both children and parents
    public class Node
    {
        protected NodeState state;

        //Having links to both will make composite nodes more accessible with their shared data
        public Node parent;
        protected List<Node> children = new List<Node>();

        //shared agnostic data using a dictionary
        private Dictionary<string, object> dataContext = new Dictionary<string, object>();

        //assigned in the constructor or will be empty
        //so by default will assign a null parent 
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

        //properly link node with an edge between 
        private void Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }

        //prototupe of the Evalute, virtual so each derrived node can implement it's own evaluate function
        public virtual NodeState Evaluate() => NodeState.FAILURE;

        //to set data add a key to the dictionary
        public void SetData(string key, object value)
        {
            dataContext[key] = value;
        }

        //to get it back is more complex as we want to check if it's defined somewhere in our branch and not just
        //in this node, which will make it easier to access and use the shared data
        //make it recurrsive to look through up through the branch until we've either found the key we were looking
        //for or reached the root of the tree
        public object GetData(string key)
        {
            object value = null;
            if (dataContext.TryGetValue(key, out value))
            {
                return value;
            }

            Node node = parent;
            while (node != null)
            {
                value = node.GetData(key);
                if (value != null)
                {
                    return value;
                }
                node = node.parent;
            }
            return null;
        }

        //to clear the data it is the same process as GetData
        public bool ClearData(string key)
        {

            if (dataContext.ContainsKey(key))
            {
                dataContext.Remove(key);
                return true;
            }

            Node node = parent;
            while (node != null)
            {
                bool cleared = node.ClearData(key);
                if (cleared)
                {
                    return true;
                }
                node = node.parent;
            }
            return false;
        }

    }
}
