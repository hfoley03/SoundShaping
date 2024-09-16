using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// original function in Matlab taken from Dr. Murtaza Ali Khan Dr. Khan
// https://uk.mathworks.com/matlabcentral/fileexchange/15542-cubic-bezier-least-square-fitting
// ported from matlab to c#
// Further adapted for intended purpose in this project

public class BezierLineFunctions : MonoBehaviour
{
    // Approximation of data using cubic Bezier curve least square fitting
    static public Vector3[] BezierInterp(Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3, LineRenderer currentLineRenderer, int numOfCurves)
    {
        int length = currentLineRenderer.positionCount;
        length = 10; //20 //10
        float[] t = Linspace(0, 1, length);

        Vector3[] Q = new Vector3[length];

        Matrix4x4 M = new Matrix4x4(
            new Vector4(-1, 3, -3, 1),
            new Vector4(3, -6, 3, 0),
            new Vector4(-3, 3, 0, 0),
            new Vector4(1, 0, 0, 0));

        for (int k = 0; k < length; k++)
        {
            Vector4 tVector = new Vector4(Mathf.Pow(t[k], 3), Mathf.Pow(t[k], 2), t[k], 1);
            Vector4 PVector = new Vector4(P0.x, P1.x, P2.x, P3.x);
            float x = Vector4.Dot(tVector, M * PVector);

            PVector = new Vector4(P0.y, P1.y, P2.y, P3.y);
            float y = Vector4.Dot(tVector, M * PVector);

            PVector = new Vector4(P0.z, P1.z, P2.z, P3.z);
            float z = Vector4.Dot(tVector, M * PVector);
            Q[k] = new Vector3(x, y, z);
        }
        return Q;
    }

    //Port of standard Matlab function 
    static public float[] Linspace(float start, float end, int n)
    {
        float[] result = new float[n];
        float step = (end - start) / (n - 1);

        for (int i = 0; i < n; i++)
        {
            result[i] = start + step * i;
        }
        return result;
    }
}
