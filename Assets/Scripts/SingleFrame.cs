using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

public class SingleFrame : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        List<KeyValuePair<Vector3, bool>> frame1 = project_loop(0.ToString("D4"));
        GameObject test = CreateCloudPoints(frame1);
    }

    List<KeyValuePair<Vector3, bool>> project_loop(string imageFrame)
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

                    float uCoord = currentImage.width - X[0, 0] / X[2, 0];
                    float vCoord = currentImage.height - X[1, 0] / X[2, 0];

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
        //frame.SetActive(false);
        return frame;
    }

    List<KeyValuePair<Vector3, bool>> InitializeVoxelDictionary()
    {
        List<KeyValuePair<Vector3, bool>> voxels = new List<KeyValuePair<Vector3, bool>>();

        for (float x = -15; x <= 15; x += 0.1f)
        {
            for (float y = -15; y <= 15; y += 0.1f)
            {
                for (float z = -15; z <= 15; z += 0.1f)
                {
                    Vector3 position = new Vector3(x, y, z);
                    voxels.Add(new KeyValuePair<Vector3, bool>(position, true));
                }
            }
        }

        return voxels;
    }
}
