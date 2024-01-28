using UnityEngine;

public class UnionFind
{
    private readonly int[] parent;
    private readonly int[] rank;

    public UnionFind(int size)
    {
        parent = new int[size];
        rank = new int[size];
        for (int i = 0; i < size; i++)
        {
            parent[i] = i;
            rank[i] = 0;
        }
    }

    public int Find(int x)
    {
        while (true)
        {
            if (x == parent[x])
                return x;
            x = parent[x];
        }
    }

    public void Union(int x, int y)
    {
        int rootX = Find(x);
        int rootY = Find(y);

        if (rootX == rootY)
            return;
        if (rank[rootX] > rank[rootY])
            parent[rootY] = rootX;
        else 
            parent[rootX] = rootY;
    }

    public bool AreUnioned(int x, int y)
    {
        return Find(x) == Find(y);
    }
}
