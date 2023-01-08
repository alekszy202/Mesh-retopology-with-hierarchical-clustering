using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ResultReader
{
    #region Parameters
    private string inputPath;
    private int vertexCount;
    private int clusterCount;
    private SortedDictionary<int, int[]> clusterDist;
    private SortedDictionary<int, ClusterNode> rootDist;
    private ClusterNode[] rootArray;
    #endregion


    #region Initialization methods
    public ResultReader(int vertexCount, string inputPath)
    {
        this.vertexCount = vertexCount;
        this.inputPath = inputPath;
    }

    public ClusterNode[] RootList()
    {
        if (ReadFile())
        {
            CreateRootDist();
            UpdateRootDist();

            rootArray = rootDist.Values.ToArray();
            Logger.LogInfo($"Finished recreating clusters tree from {inputPath}");
            return rootArray;
        }
        else
        {
            Logger.LogError($"Python output file error: {inputPath}");
            return null;
        }
    }
    #endregion


    #region File management
    private bool ReadFile()
    {
        try
        {
            using (StreamReader sr = new StreamReader(inputPath))
            {
                clusterDist = new SortedDictionary<int, int[]>();
                string line;
                int index = vertexCount;
                int firstChild;
                int secondChild;
                int lineCounter = 1;

                while ((line = sr.ReadLine()) != null)
                {
                    try
                    {
                        if (lineCounter == 1) { clusterCount = int.Parse(line); }
                        else
                        {
                            string[] values = line.Split(',');

                            firstChild = int.Parse(values[0]);
                            secondChild = int.Parse(values[1]);
                            int[] childList = new int[2] { firstChild, secondChild };
                            clusterDist.Add(index, childList);

                            index++;
                        }
                    }
                    catch (Exception err)
                    {
                        Debug.Log($"Error, while parsing data in line {lineCounter}: {err.Message}");
                        return false;
                    }

                    lineCounter++;
                }
            }
        }
        catch (Exception err)
        {
            Debug.Log($"Error, while reading file: {err.Message}");
            return false;
        }
        return true;
    }
    #endregion


    #region Dictionary methods
    private void CreateRootDist()
    {
        rootDist = new SortedDictionary<int, ClusterNode>();
        for (int i = 0; i < vertexCount; i++)
        {
            ClusterNode tmp = new ClusterNode(i);
            rootDist.Add(i, tmp);
        }
    }

    private void UpdateRootDist()
    {
        int rootCount = vertexCount;
        foreach (var pair in clusterDist)
        {
            if (rootCount == clusterCount) { break; }
            else
            {
                int index = pair.Key;
                int[] clusters = pair.Value;
                int firstIndex = clusters[0];
                int secondIndex = clusters[1];
                ClusterNode firstChild = rootDist[firstIndex];
                ClusterNode secondChild = rootDist[secondIndex];

                ClusterNode[] children = new ClusterNode[2] { firstChild, secondChild };
                ClusterNode tmp = new ClusterNode(children, index);
                rootDist.Add(index, tmp);
                rootDist.Remove(firstIndex);
                rootDist.Remove(secondIndex);

                rootCount--;
            }
        }
    }
    #endregion
}