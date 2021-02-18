namespace GUI
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public abstract class UINode
    {
        private string name;

        private UINode root;

        private UINode parent;

        private readonly List<UINode> children = new List<UINode>();

        public event EventHandler OnAdded = delegate { };

        public event EventHandler OnRemoved = delegate { };

        public event EventHandler OnNameChanged = delegate { };

        public event EventHandler OnParentChanged = delegate { };

        public UINode()
        {
            this.root = this;
        }

        public string Name
        {
            get => this.name;

            set
            {
                if (this.name != value)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        throw new Exception("Name of element can't be empty");
                    }

                    this.name = value;
                    this.OnNameChanged(this, EventArgs.Empty);
                }
            }
        }

        public UINode Root
        {
            get => this.root;

            private set
            {
                if (this.root != value)
                {
                    this.root = value;

                    foreach (var child in this.children)
                    {
                        child.Root = value;
                    }
                }
            }
        }

        public UINode Parent
        {
            get => this.parent;

            set
            {
                if (this.parent != value)
                {
                    if (this == value)
                    {
                        throw new Exception("Node can't be parent and child at the same time");
                    }

                    this.parent?.RemoveNode(this);
                    this.parent = value;

                    if (value != null)
                    {
                        this.Root = this.parent.Root;
                        this.parent.AddChild(this);
                    }

                    this.DoParentChanged();
                    this.OnParentChanged(this, EventArgs.Empty);
                    this.NotifyRoot(UINotification.ParentChanged);
                }
            }
        }

        public IEnumerable<UINode> GetChildren()
        {
            return this.children;
        }

        public IEnumerable<T> GetChildren<T>() where T : UINode
        {
            return this.children.OfType<T>();
        }

        private void AddChild(UINode node)
        {
            if (this.children.Exists(x => !string.IsNullOrEmpty(node.Name) && x.Name == node.Name))
            {
                throw new Exception($"Element with the same name already exists > {node.Name}");
            }

            this.children.Add(node);
            this.DoAdded(node);
            this.OnAdded(node, EventArgs.Empty);
            this.Root.Notification(node, UINotification.EnterTree);
        }

        public UINode AddNode(UINode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            node.Parent = this;
            return node;
        }

        public void RemoveNode(UINode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            node.Free();
            this.children.Remove(node);
            this.DoRemoved(node);
            this.OnRemoved(node, EventArgs.Empty);
            this.Root.Notification(node, UINotification.ExitTree);
        }

        public void RemoveAll(Func<UIControl, bool> predicate)
        {
            var corpses = this.GetChildren<UIControl>().Where(predicate).ToArray();

            for (int i = corpses.Length - 1; i >= 0; i--)
            {
                this.RemoveNode(corpses[i]);
            }
        }

        public void Reset(bool recursive)
        {
            if (recursive)
            {
                this.children.ForEach(child => child.Reset(recursive));
            }

            this.DoReset();
        }

        public void Free()
        {
            this.DoFree();

            while (this.children.Count > 0)
            {
                this.RemoveNode(this.children[0]);
            }

            this.children.Clear();
        }

        public UINode Find(string name)
        {
            if (this.Name == name)
            {
                return this;
            }
            else
            {
                foreach (var child in this.children)
                {
                    var foundNode = child.Find(name);

                    if (foundNode != null)
                    {
                        return foundNode;
                    }
                }
            }

            return null;
        }

        public T Find<T>(string name)
            where T : UINode
        {
            return this.Find(name) as T;
        }

        public void Sort(Comparison<UINode> comparison)
        {
            this.children.Sort(comparison);
        }

        protected void NotifyRoot(UINotification notification)
        {
            this.Root.Notification(this, notification);
        }

        protected virtual void Notification(UINode sender, UINotification notification)
        {
        }

        protected virtual void DoParentChanged()
        {
        }

        protected virtual void DoAdded(UINode node)
        {
        }

        protected virtual void DoRemoved(UINode node)
        {
        }

        protected virtual void DoProcess()
        {
        }

        protected virtual void DoReset()
        {
        }

        protected virtual void DoFree()
        {
        }
    }
}
