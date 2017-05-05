using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

public class SpaceCarver : MonoBehaviour {

    public float timeBetweenFrames = 0.1f;
    float timeSinceLastFrame = 0.0f;
    public int startFrame = 0;
    public int endFrame = 100;
    public bool debug = false;
    int currentFrame;
    GameObject currentFrameObject;

    List<GameObject> frames;
	// Use this for initialization
	void Start () {
        if (!debug)
            GenerateFrames();
        else
            PlotUVProjectionOnImage();
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastFrame += Time.deltaTime;

        if (timeSinceLastFrame > timeBetweenFrames)
        {

            if (!debug)
            {
                currentFrameObject.SetActive(false);
                currentFrameObject = frames[currentFrame];
                currentFrameObject.SetActive(true);

                if (currentFrame < endFrame - startFrame)
                {
                    currentFrame++;
                }
                else
                {
                    currentFrame = 0;
                }

            }


            timeSinceLastFrame = 0;
        }
    }

    void GenerateFrames ()
    {
        frames = new List<GameObject>();
        currentFrame = 0;
        for (int i = startFrame; i <= endFrame; i++)
        {
            List<KeyValuePair<Vector3, bool>> voxelModel = project_loop(i.ToString("D4"));

            frames.Add(CreateCloudPoints(voxelModel));
        }
        currentFrameObject = frames[currentFrame];
    }


    List<KeyValuePair<Vector3, bool>> project_loop (string imageFrame)
    {
        List<Matrix<float>> cameraCalibrations = CameraCalibrations.GetCamerasProjMatrix();
        List<KeyValuePair<Vector3, bool>> voxels = InitializeVoxelDictionary();

        for (int i = 0; i < voxels.Count; i++)
        {
            if (voxels[i].Value)
            {

                for (int j = 0; j < cameraCalibrations.Count; j++)
                {
                    int cam = j + 1;
                    string imagePath = "silhouettes/Silhouette" + cam + "_" + imageFrame;
                    Texture2D currentImage = Resources.Load(imagePath) as Texture2D;

                    float[,] xyzCoords = new float[,] { { voxels[i].Key.x }, { voxels[i].Key.y }, { voxels[i].Key.z }, { 1 } };

                    Matrix<float> matrixCoords = Matrix<float>.Build.DenseOfArray(xyzCoords);

                    matrixCoords.Transpose();

                    Matrix<float> X = cameraCalibrations[j] * matrixCoords;


                    float uCoord = currentImage.width - X[0, 0] / X[2, 0] ;
                    float vCoord = currentImage.height - X[1, 0] / X[2, 0] + 300f;

                    if (currentImage.GetPixel(Mathf.RoundToInt(uCoord), Mathf.RoundToInt(vCoord)).Equals(Color.black))
                    {
                        voxels[i] = new KeyValuePair<Vector3, bool>(voxels[i].Key, false);
                        break;
                    }
                }
            }

        }

        return voxels; 
    }

    void PlotUVProjectionOnImage()
    {
        List<Matrix<float>> cameraCalibrations = CameraCalibrations.GetCamerasProjMatrix();
        string imagePath = "silhouettes/Silhouette3_0000";
        Texture2D currentImage = Resources.Load(imagePath) as Texture2D;
        Matrix<float> P = CameraCalibrations.GetProjMatrix(3);
        List<float> u = new List<float>();
        List<float> v = new List<float>();

        List<Vector2> uvCoords = new List<Vector2>();

        for (float x = -1f; x <= 1f; x += 0.1f)
        {
            for (float y = -1f; y <= 1f; y += 0.1f)
            {
                for (float z = -1f; z <= 1f; z += 0.1f)
                {
                    float[,] xyzCoords = new float[,] { { x }, { y }, { z }, { 1 } };

                    Matrix<float> matrixCoords = Matrix<float>.Build.DenseOfArray(xyzCoords);

                   // matrixCoords.Transpose();

                    Matrix<float> X = P * matrixCoords;


                    uvCoords.Add(new Vector2((X[0, 0] / X[2, 0]), ( X[1, 0] / X[2, 0])));
                }
            }
        }

        PlotPoints(uvCoords, currentImage);
        //PlotPoints(uvCoords);
        
        CreateTargetImage("img1", currentImage, new Vector3(0, 0, 0), new Vector3(1, 1, 1));
    }

    GameObject CreateCloudPoints (List<KeyValuePair<Vector3, bool>> voxels)
    {
        GameObject frame = new GameObject();
        
        foreach (KeyValuePair<Vector3, bool> voxel in voxels)
        {
            if (voxel.Value)
            {
                GameObject voxelGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                voxelGameObject.GetComponent<Renderer>().material.color = Color.red;
                voxelGameObject.transform.position = new Vector3(voxel.Key.x, voxel.Key.y, voxel.Key.z);
                voxelGameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                voxelGameObject.transform.parent = frame.transform;
            }
        }
        frame.SetActive(false);
        return frame;
    }

    List<KeyValuePair< Vector3, bool>> InitializeVoxelDictionary()
    {
        List<KeyValuePair<Vector3, bool>> voxels = new List<KeyValuePair<Vector3, bool>>();

        for (float x = -1; x <= 1; x+= 0.05f)
        {
            for (float y = -1; y <= 1; y += 0.05f)
            {
                for (float z = -1; z <= 1; z += 0.05f)
                {
                    Vector3 position = new Vector3(x, y, z);
                    voxels.Add(new KeyValuePair<Vector3, bool>(position, true));
                }
            }
        }

        return voxels;
    }

    void PlotPoints(List<Vector2> uvCoords)
    {
        GameObject parent = new GameObject();
        for (int i = 0; i < uvCoords.Count; i++)
        {
            //originalTexture.SetPixel((int)uvCoords[i].x, (int)uvCoords[i].y, Color.red);
            GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            point.transform.position = new Vector3(uvCoords[i].x, uvCoords[i].y, 0.0f);
            point.transform.parent = parent.transform;
        }

    }

    void PlotPoints (List<float> u, List<float> v, Texture2D originalTexture)
    {
        for (int i = 0; i < u.Count; i++)
        {
            originalTexture.SetPixel((int)u[i], (int)v[i], Color.red);
        }

        originalTexture.Apply();
    }

    void PlotPoints(List<Vector2> uvCoords, Texture2D originalTexture)
    {
        for (int i = 0; i < uvCoords.Count; i++)
        {
            originalTexture.SetPixel((int) uvCoords[i].x, (int)uvCoords[i].y, Color.red);
        }

        originalTexture.Apply();
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
