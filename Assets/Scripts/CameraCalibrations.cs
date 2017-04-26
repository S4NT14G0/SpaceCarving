using System.Collections;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;

public class CameraCalibrations
{

    static float[,] camera1 = new float[,]
    {
        { 1.483215e+03f, -7.953666e+02f, -9.153119e+02f, 4.046004e+03f },
        { -5.395400e+01f, -1.719494e+03f, 3.606972e+02f, 3.961539e+03f },
        { -6.991278e-02f, -9.069575e-01f, -1.063456e+00f, 4.753082e+00f}
    };

    static float[,] camera2 = new float[,]
    {
        { 1.677218e+03f, -7.084734e+02f, 5.732087e+02f, 5.171564e+03f },
        {-1.814967e+02f, -1.743858e+03f, 3.993065e+00f, 4.314523e+03f },
        { 8.810557e-01f, -7.603295e-01f, -7.786652e-01f, 6.074740e+00f}
    };

    static float[,] camera3 = new float[,]
    {
        { 9.269854e+02f, -7.025415e+02f, 1.509225e+03f, 4.448627e+03f },
        { -1.922770e+02f, -1.737088e+03f, -7.511179e+01f, 4.261084e+03f },
        { 1.152985e+00f, -7.936465e-01f, -3.782901e-02f, 5.152725e+00f}
    };

    static float[,] camera4 = new float[,]
    {
        { -3.529096e+02f, -6.068026e+02f, 1.769746e+03f, 4.453332e+03f },
        { -5.880053e+01f, -1.765468e+03f, -8.726954e+01f, 4.527056e+03f },
        { 8.825374e-01f, -7.435539e-01f, 7.937028e-01f, 6.003132e+00f }
    };

    static float[,] camera5 = new float[,]
    {
        { -1.532662e+03f, -7.698217e+02f, 8.246429e+02f, 3.787523e+03f },
        { 2.544591e+01f, -1.720718e+03f, -4.054952e+02f, 3.874761e+03f },
        { 1.776188e-02f, -9.415247e-01f, 1.036173e+00f, 4.695420e+00f }
    };

    static float[,] camera6 = new float[,]
    {
        { -1.732989e+03f, -5.827524e+02f, -6.096631e+02f, 4.869964e+03f },
        { 8.629205e+01f, -1.771143e+03f, -1.076821e+02f, 4.426927e+03f },
        { -9.177131e-01f, -7.547954e-01f, 7.410083e-01f, 6.032102e+00f }
    };

    static float[,] camera7 = new float[,]
    {
        { -8.914168e+02f, -6.960165e+02f, -1.548135e+03f, 4.017688e+03f },
        { 2.466475e+02f, -1.750562e+03f, 2.269384e+01f, 4.171231e+03f },
        { -1.119922e+00f, -8.401569e-01f, 2.586530e-04f, 5.181506e+00f }
    };

    static float[,] camera8 = new float[,]
    {
        { 4.011197e+02f, -5.661652e+02f, -1.788971e+03f, 4.441950e+03f },
        { 1.039635e+02f, -1.761553e+03f, 4.867437e+01f, 4.503671e+03f },
        { -8.417806e-01f, -7.415664e-01f, -8.374394e-01f, 6.065466e+00f }
    };

    public static List<Matrix<float>> GetCamerasProjMatrix()
    {
        List<Matrix<float>> cameras = new List<Matrix<float>>();
        for (int i = 1; i <= 8; i++)
        {
            cameras.Add(GetProjMatrix(i));
        }

        return cameras;
    }

    public static Matrix<float> GetProjMatrix(int i)
    {
        switch (i)
        {
            case (1):
                return Matrix<float>.Build.DenseOfArray(camera1);
            case (2):
                return Matrix<float>.Build.DenseOfArray(camera2);
            case (3):
                return Matrix<float>.Build.DenseOfArray(camera3);
            case (4):
                return Matrix<float>.Build.DenseOfArray(camera4);
            case (5):
                return Matrix<float>.Build.DenseOfArray(camera5);
            case (6):
                return Matrix<float>.Build.DenseOfArray(camera6);
            case (7):
                return Matrix<float>.Build.DenseOfArray(camera7);
            case (8):
                return Matrix<float>.Build.DenseOfArray(camera8);

            default:
                return null;
        }
    }
}
