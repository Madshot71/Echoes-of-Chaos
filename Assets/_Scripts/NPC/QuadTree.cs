using System;
using System.Collections.Generic;
using UnityEngine;

public class QuadTree
{
    class Node
    {
        public Rect bounds;
        public List<Entry> entries;
        public Node[] children;
        public bool IsLeaf => children == null;
        public Node(Rect b) { bounds = b; entries = new List<Entry>(); children = null; }
    }

    struct Entry { public int id; public Vector2 pos; public Entry(int id, Vector2 pos) { this.id = id; this.pos = pos; } }

    Node root;
    int capacity;

    public QuadTree(Rect bounds, int nodeCapacity = 8)
    {
        root = new Node(bounds);
        capacity = Math.Max(1, nodeCapacity);
    }

    public void Clear() => root = new Node(root.bounds);

    public void Insert(int id, Vector2 pos) => Insert(root, new Entry(id, pos));

    void Insert(Node node, Entry e)
    {
        if (!node.bounds.Contains(e.pos))
        {
            // optionally expand or skip
            return;
        }

        if (node.IsLeaf)
        {
            node.entries.Add(e);
            if (node.entries.Count > capacity)
                Subdivide(node);
            return;
        }

        foreach (var c in node.children)
            if (c.bounds.Contains(e.pos)) { Insert(c, e); return; }

        // fallback
        node.entries.Add(e);
    }

    void Subdivide(Node node)
    {
        node.children = new Node[4];
        var b = node.bounds;
        float hx = b.width * 0.5f, hy = b.height * 0.5f;
        node.children[0] = new Node(new Rect(b.xMin, b.yMin, hx, hy)); // SW
        node.children[1] = new Node(new Rect(b.xMin + hx, b.yMin, hx, hy)); // SE
        node.children[2] = new Node(new Rect(b.xMin, b.yMin + hy, hx, hy)); // NW
        node.children[3] = new Node(new Rect(b.xMin + hx, b.yMin + hy, hx, hy)); // NE

        var old = node.entries;
        node.entries = new List<Entry>();
        foreach (var e in old) Insert(node, e);
    }

    // nearest query: returns id and position; returns false if none found
    public bool FindNearest(Vector2 point, out int foundId, out Vector2 foundPos, Func<int,bool> accept = null)
    {
        foundId = -1; foundPos = default;
        float bestSq = float.MaxValue;
        // Priority traversal using a simple stack sorted by min-dist to rect could be added, but recursive prune is fine.
        SearchNode(root, ref bestSq, point, ref foundId, ref foundPos, accept);
        return foundId != -1;
    }

    void SearchNode(Node node, ref float bestSq, Vector2 point, ref int foundId, ref Vector2 foundPos, Func<int,bool> accept)
    {
        // prune if node rect too far
        float d = RectDistanceSqr(node.bounds, point);
        if (d > bestSq) return;

        // check entries
        foreach (var e in node.entries)
        {
            if (accept != null && !accept(e.id)) continue;
            float dsq = (e.pos - point).sqrMagnitude;
            if (dsq < bestSq)
            {
                bestSq = dsq;
                foundId = e.id;
                foundPos = e.pos;
            }
        }

        if (!node.IsLeaf)
        {
            // Optionally order children by distance to point to prune earlier
            foreach (var c in node.children)
                SearchNode(c, ref bestSq, point, ref foundId, ref foundPos, accept);
        }
    }

    static float RectDistanceSqr(Rect r, Vector2 p)
    {
        float dx = Math.Max(r.xMin - p.x, 0f);
        dx = Math.Max(dx, p.x - r.xMax);
        float dy = Math.Max(r.yMin - p.y, 0f);
        dy = Math.Max(dy, p.y - r.yMax);
        return dx * dx + dy * dy;
    }
}