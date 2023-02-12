using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

[System.Serializable]
public class LevelModel
{
    public int index;
    public string name;
    
    [Range(ConstantValues.MINROWS,ConstantValues.MAXROWS)] public int M;   //Rows
    [Range(ConstantValues.MINCOLUMNS,ConstantValues.MAXCOLUMNS)] public int N;   //Columns
    [Range(ConstantValues.MINCOLORS,ConstantValues.MAXCOLORS)] public int K;   //ColorCount
    public int A;   //DefaultIconLimit
    public int B;   //FirstIconLimit
    public int C;   //SecondIconLimit

    public LevelRule LevelRule = new ();
    
    public BlockColor[] SelectedColors;
    
    //Later feature
    [HideInInspector] public bool IsRandom = true;

    public LevelModel(int index, int m, int n, int k, int a, int b, int c)
    {
        this.index = index;
        M = m;
        N = n;
        K = k;
        A = a;
        B = b;
        C = c;
        SelectRandomColors();
    }

    public void SelectRandomColors()
    {
        SelectedColors = new BlockColor[K];
        
        Random r = new Random();
        var asd = Enumerable.Range(0, ConstantValues.MAXCOLORS).OrderBy(x => r.Next()).ToList();
        for (int i = 0; i < K; i++)
        {
            SelectedColors[i] = (BlockColor)asd[i];
        }
    }

    /*public void SetGrid(Grid grid)
    {
        if (grid != null)
        {
            M = grid.Rows;
            N = grid.Columns;
            
            Grid = grid;

            IsRandom = false;
        }
        else
        {
            IsRandom = true;
        }
    }*/
}
