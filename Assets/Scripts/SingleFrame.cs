using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

public class SingleFrame : MonoBehaviour
{

    // Use this for initialization
    //void Start()
    //{
    //    List<KeyValuePair<Vector3, bool>> frame1 = project_loop(0.ToString("D4"));
    //    GameObject test = CreateCloudPoints(frame1);
    //}

    public float timeBetweenFrames = 0.1f;
    float timeSinceLastFrame = 0.0f;
    public int startFrame = 0;
    public int endFrame = 100;
    int currentFrame;
    GameObject currentFrameObject;

    List<GameObject> frames;
    // Use this for initialization
    void Start()
    {
            GenerateFrames();

    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastFrame += Time.deltaTime;

        if (timeSinceLastFrame > timeBetweenFrames)
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

            


            timeSinceLastFrame = 0;
        }
    }

    void GenerateFrames()
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

    List<KeyValuePair<Vector3, bool>> project_loop(string imageFrame)
    {
        List<Matrix<float>> cameraCalibrations = CameraCalibrations.GetCamerasProjMatrix();
        List<KeyValuePair<Vector3, bool>> voxels = InitializeVoxelDictionary();

        for (int i = 0; i < voxels.Count; i++)
        {
            //if (voxels[i].Value)
            //{
                for (int j = 0; j < cameraCalibrations.Count; j++)
                {
                    int cam = j + 1;
                    string imagePath = "silhouettes/Silhouette" + cam + "_" + imageFrame;
                    Texture2D currentImage = Resources.Load(imagePath) as Texture2D;

                    float[,] xyzCoords = new float[,] { { voxels[i].Key.x }, { voxels[i].Key.y }, { voxels[i].Key.z }, { 1 } };

                    Matrix<float> matrixCoords = Matrix<float>.Build.DenseOfArray(xyzCoords);

                    matrixCoords.Transpose();

                    Matrix<float> X = cameraCalibrations[j] * matrixCoords;

                    float uCoord = currentImage.width - X[0, 0] / X[2, 0];
                    float vCoord = currentImage.height - X[1, 0] / X[2, 0];

                    if (currentImage.GetPixel(Mathf.RoundToInt(uCoord), Mathf.RoundToInt(vCoord)).Equals(Color.black))
                        voxels[i] = new KeyValuePair<Vector3, bool>(voxels[i].Key, false);
                   else
                        voxels[i] = new KeyValuePair<Vector3, bool>(voxels[i].Key, true);

                }
           // }

        }

        return voxels;
    }

    GameObject CreateCloudPoints(List<KeyValuePair<Vector3, bool>> voxels)
    {
        GameObject frame = new GameObject();

        foreach (KeyValuePair<Vector3, bool> voxel in voxels)
        {
            if (voxel.Value)
            {
                GameObject voxelGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                voxelGameObject.GetComponent<Renderer>().material.color = Color.red;
                voxelGameObject.transform.position = new Vector3(voxel.Key.x, voxel.Key.y, voxel.Key.z);
                voxelGameObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                voxelGameObject.transform.parent = frame.transform;
            }
        }

        return frame;
    }

    List<KeyValuePair<Vector3, bool>> InitializeVoxelDictionary()
    {
        List<KeyValuePair<Vector3, bool>> voxels = new List<KeyValuePair<Vector3, bool>>();

        for (float x = -2f; x <= 2f; x += 0.1f)
        {
            for (float y = -2f; y <= 2f; y += 0.1f)
            {
                for (float z = -2; z <= 2f; z += 0.1f)
                {
                    Vector3 position = new Vector3(x, y, z);
                    voxels.Add(new KeyValuePair<Vector3, bool>(position, false));
                }
            }
        }

        return voxels;
    }
}
