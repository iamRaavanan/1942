using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
    public static bool isGameOver = false;
    public static bool isGameStarted = false;
    public static bool isRetryClicked = false;
    static public Vector3 GetNormal(Vector3 vector)
    {
        float length = Distance(Vector3.zero, vector);
        vector.x /= length;
        vector.y /= length;
        vector.z /= length;

        return vector;
    }

    static public float Distance(Vector3 point1, Vector3 point2)
    {
        float diffSquared = Square(point1.x - point2.x) +
                            Square(point1.y - point2.y) +
                            Square(point1.z - point2.z);
        float squareRoot = Mathf.Sqrt(diffSquared);
        return squareRoot;

    }

    static public float Square(float value)
    {
        return value * value;
    }

    static public Vector3 LookAt2D(Vector3 forwardVector, Vector3 position, Vector3 focusPoint)
    {
        //Vector3 direction = new Vector3(focusPoint.x - position.x, focusPoint.y - position.y, position.z);
        Vector3 direction = focusPoint - position;
        direction = GetNormal(direction);
        float angle = Angle(forwardVector, direction);
        bool clockwise = false;
        if (Cross(forwardVector, direction).z < 0)
            clockwise = true;

        Vector3 newDir = Rotate(forwardVector, angle, clockwise);
        return newDir;
    }

    static public float Angle(Vector3 vector1, Vector3 vector2)
    {
        float dotDivide = Dot(vector1, vector2) /
                    (Distance(Vector3.zero, vector1) * Distance(Vector3.zero, vector2));
        return Mathf.Acos(dotDivide); //radians.  For degrees * 180/Mathf.PI;
    }

    static public Vector3 Rotate(Vector3 vector, float angle, bool clockwise) //in radians
    {
        if (clockwise)
        {
            angle = 2 * Mathf.PI - angle;
        }

        float xVal = vector.x * Mathf.Cos(angle) - vector.y * Mathf.Sin(angle);
        float yVal = vector.x * Mathf.Sin(angle) + vector.y * Mathf.Cos(angle);
        return new Vector3(xVal, yVal, 0);
    }

    static public float Dot(Vector3 vector1, Vector3 vector2)
    {
        return (vector1.x * vector2.x + vector1.y * vector2.y + vector1.z * vector2.z);
    }

    static public Vector3 Cross(Vector3 vector1, Vector3 vector2)
    {
        float xMult = vector1.y * vector2.z - vector1.z * vector2.y;
        float yMult = vector1.z * vector2.x - vector1.x * vector2.z;
        float zMult = vector1.x * vector2.y - vector1.y * vector2.x;
        Vector3 crossProd = new Vector3(xMult, yMult, zMult);
        return crossProd;
    }
}
