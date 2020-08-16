using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Decuplr.Serialization.Namespaces {
    internal class NamespaceNode : INamespaceNode {

        private class ReadOnlyNamespaceDictionary : IReadOnlyDictionary<string, IReadOnlyNamespaceNode>, IReadOnlyDictionary<string, INamespaceNode> {
            private readonly NamespaceNode _parent;

            public ReadOnlyNamespaceDictionary(NamespaceNode parent) => _parent = parent;

            IReadOnlyNamespaceNode IReadOnlyDictionary<string, IReadOnlyNamespaceNode>.this[string key] => _parent._childNodes[key];
            INamespaceNode IReadOnlyDictionary<string, INamespaceNode>.this[string key] => _parent._childNodes[key];

            IEnumerable<IReadOnlyNamespaceNode> IReadOnlyDictionary<string, IReadOnlyNamespaceNode>.Values => _parent._childNodes.Values;
            IEnumerable<INamespaceNode> IReadOnlyDictionary<string, INamespaceNode>.Values => _parent._childNodes.Values;

            public IEnumerable<string> Keys => _parent._childNodes.Keys;
            public int Count => _parent._childNodes.Count;

            public bool ContainsKey(string key) => _parent._childNodes.ContainsKey(key);

            public bool TryGetValue(string key, out IReadOnlyNamespaceNode value) {
                var result = _parent._childNodes.TryGetValue(key, out var node);
                value = node;
                return result;
            }

            public bool TryGetValue(string key, out INamespaceNode value) {
                var result = _parent._childNodes.TryGetValue(key, out var node);
                value = node;
                return result;
            }

            IEnumerator<KeyValuePair<string, IReadOnlyNamespaceNode>> IEnumerable<KeyValuePair<string, IReadOnlyNamespaceNode>>.GetEnumerator() 
                => _parent._childNodes.Select(x => new KeyValuePair<string, IReadOnlyNamespaceNode>(x.Key, x.Value)).GetEnumerator();

            IEnumerator<KeyValuePair<string, INamespaceNode>> IEnumerable<KeyValuePair<string, INamespaceNode>>.GetEnumerator()
                => _parent._childNodes.Select(x => new KeyValuePair<string, INamespaceNode>(x.Key, x.Value)).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => ((IReadOnlyDictionary<string, INamespaceNode>)this).GetEnumerator();

        }

        private readonly Dictionary<TypeEntryInfo, object> _container = new Dictionary<TypeEntryInfo, object>();
        private readonly Dictionary<string, NamespaceNode> _childNodes = new Dictionary<string, NamespaceNode>();
        private readonly ReadOnlyNamespaceDictionary _readonlyNodes;
        private readonly NamespaceTreeSource _root;

        public INamespaceNode? Parent { get; }

        public string Identifier { get; }

        public string FullName { get; }

        internal NamespaceNode(INamespaceNode? parent, NamespaceTreeSource root, string identifier) {
            _readonlyNodes = new ReadOnlyNamespaceDictionary(this);
            Parent = parent;
            _root = root;
            Identifier = identifier;
            FullName = parent != null ? $"{parent.FullName}.{identifier}" : identifier;
        }

        private protected NamespaceNode(string identifier) {
            Debug.Assert(this is NamespaceTreeSource);
            _readonlyNodes = new ReadOnlyNamespaceDictionary(this);
            Parent = null;
            Identifier = identifier;
            FullName = identifier;
            _root = (NamespaceTreeSource)this;
        }

        public INamespaceTree Root => _root;

        public IReadOnlyDictionary<string, INamespaceNode> ChildNodes => _readonlyNodes;

        object IReadOnlyNamespaceNode.this[Assembly assembly, Type type] => this[assembly, type];

        IReadOnlyNamespaceNode? IReadOnlyNamespaceNode.Parent => Parent;

        IReadOnlyDictionary<string, IReadOnlyNamespaceNode> IReadOnlyNamespaceNode.ChildNodes => _readonlyNodes;

        public object this[Assembly assembly, Type type] {
            get => _container[(assembly, type)];
            set {
                _container[(assembly, type)] = value;
                _root.Modified();
            }
        }

        private INamespaceNode GetChildNamespace(ReadOnlySpan<string> splitNamespaces) {
            var nextIdent = splitNamespaces[0];
            // Validate if this is a valid namespace name
            if (!nextIdent.IsValidIdentifier())
                throw new ArgumentException($"{nextIdent} is not a valid namespace name");

            // Check if it exists or not
            if (_childNodes.TryGetValue(nextIdent, out var node))
                return GetExactNode(splitNamespaces, node);

            // Create one if not found
            // ADVISE : maybe make it lazy?
            node = new NamespaceNode(this, _root, nextIdent);
            _childNodes.Add(nextIdent, node);
            _root.Modified();
            return GetExactNode(splitNamespaces, node);

            static INamespaceNode GetExactNode(ReadOnlySpan<string> splitNamespaces, NamespaceNode node) {
                if (splitNamespaces.Length == 0)
                    return node;
                return node.GetChildNamespace(splitNamespaces.Slice(1));
            }
        }

        public INamespaceNode GetChildNamespace(string namespaceName) => GetChildNamespace(namespaceName.Split('.'));

        public TKind Get<TKind>(Assembly assembly) => (TKind)_container[(assembly, typeof(TKind))];

        public void Set<TKind>(Assembly assembly, TKind item) {
            _container[(assembly, typeof(TKind))] = item ?? throw new ArgumentNullException(nameof(item));
            _root.Modified();
        }

        public IEnumerator<KeyValuePair<TypeEntryInfo, object>> GetEnumerator() => _container.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
