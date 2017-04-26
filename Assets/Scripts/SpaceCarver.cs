using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

public class SpaceCarver : MonoBehaviour {

	// Use this for initialization
	void Start () {
        project_loop();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void project_loop ()
    {
        Texture2D image1 = Resources.Load("silhouettes/Silhouette1_0000") as Texture2D;

        Matrix<float> p = CameraCalibrations.GetProjMatrix(1);

        List<float> u = new List<float>();
        List<float> v = new List<float>();

        for (float x = -1; x <= 1; x += 0.2f)
        {
            for (float y = -1; y <= 1; y += 0.2f)
            {
                for (float z = -1; z <= 1; z += 0.2f)
                {
                    float[,] xyzCoords = new float[,] { { x }, { y }, { z }, { 1 } };
                    Matrix<float> matrixCoords = Matrix<float>.Build.DenseOfArray(xyzCoords);

                    Matrix<float> X = p * matrixCoords;

                    X.Transpose();

                    u.Add(X[0, 0] / X[2, 0]);
                    v.Add(X[1, 0] / X[2, 0]);

                }
            }
        }

        //plotPoints(u, v, image1);
        //Texture2D plot = pointPlot(u, v);
        //CreateTargetImage("plot", plot, new Vector3(0, 0, 0), new Vector3(1, 1, 1));
        CreateVoxels(u, v, image1);
        CreateTargetImage("img1", image1, new Vector3(0, 0, 0), new Vector3(1, 1, 1));
        
    }

    void CreateVoxels(List<float> u, List<float> v, Texture2D originalTexture)
    {
        for (int i = 0; i < u.Count; i++)
        {
            if (!originalTexture.GetPixel((int)u[i], (int)v[i]).Equals(Color.black))
            {
                GameObject voxel = GameObject.CreatePrimitive(PrimitiveType.Cube);
                voxel.GetComponent<Renderer>().material.color = Color.red;
                voxel.transform.position = new Vector3(u[i] / 100, v[i] /100);
            }
        }

    }

    void plotPoints (List<float> u, List<float> v, Texture2D originalTexture)
    {
        Debug.Log(Color.black);
        for (int i = 0; i < u.Count; i++)
        {
            Color color = originalTexture.GetPixel((int)u[i], (int)v[i]);
            if (color != new Color(0, 0, 0, 1))
                Debug.Log(color);
            originalTexture.SetPixel((int)u[i], (int)v[i], Color.red);
        }

        originalTexture.Apply();
    }

    Texture2D pointPlot (List<float> u, List<float> v)
    {
        Texture2D plot = new Texture2D(2048, 1028);

        for (int i = 0; i < plot.width; i++)
        {
            for (int j = 0; j < plot.height; j++)
            {
                plot.SetPixel(i, j, Color.clear);
            }
        }

        for (int i = 0; i < u.Count; i++)
        {
            plot.SetPixel((int)u[i], (int)v[i], Color.red);
        }

        plot.Apply();

        return plot;
    }




    /// <summary>
    /// Create a new 2D gameobject
    /// </summary>
    /// <param name="name">Name of gameobject</param>
    /// <param name="tex">Texture to be applied to gameobject</param>
    /// <param name="position">Initial position</param>
    /// <param name="scale">Initial scale</param>
    void CreateTargetImage(string name, Texture2D tex, Vector3 position, Vector3 scale)
    {
        GameObject go = new GameObject(name);
        go.AddComponent<SpriteRenderer>().sprite = TextureToSprite(tex);
        go.transform.position = position;
        go.transform.localScale = scale;

    }

    /// <summary>
    /// Helper function to convert a Texture2D to a Sprite
    /// </summary>
    /// <param name="tex"></param>
    /// <returns></returns>
    Sprite TextureToSprite(Texture2D tex)
    {
        Rect rec = new Rect(0, 0, tex.width, tex.height);

        Sprite spriteFromTex = Sprite.Create(tex, rec, new Vector2(0, 0), 100);

        return spriteFromTex;
    }
}
