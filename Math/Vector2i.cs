using System;
using System.Runtime.InteropServices;
using UnityEngine;

[Serializable]
public struct Vector2i {

    public int x;
    public int y;

    public Vector2i(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Vector2i(float x, float y)
    {
        this.x = (int)x;
        this.y = (int)y;
    }

    public Vector2i(Vector2 vec2)
    {
        this.x = (int)vec2.x;
        this.y = (int)vec2.y;
    }

    public static bool operator == (Vector2i v1, Vector2i v2)
    {
        return v1.x == v2.x && v1.y == v2.y;
    }

    public static bool operator != (Vector2i v1, Vector2i v2)
    {
        return !(v1.x == v2.x && v1.y == v2.y);
    }

    public static bool operator < (Vector2i v1, Vector2i v2)
    {
        return v1.Sum() < v2.Sum();
    }
    public static bool operator <= (Vector2i v1, Vector2i v2)
    {
        return v1.Sum() <= v2.Sum();
    }

    public static bool operator > (Vector2i v1, Vector2i v2)
    {
        return v1.Sum() > v2.Sum();
    }

    public static bool operator >= (Vector2i v1, Vector2i v2)
    {
        return v1.Sum() >= v2.Sum();
    }

    public static Vector2i operator + (Vector2i v1, Vector2i v2)
    {
        return new Vector2i(v1.x + v2.x, v1.y + v2.y);
    }

    public static Vector2i operator - (Vector2i v1, Vector2i v2)
    {
        return new Vector2i(v1.x - v2.x, v1.y - v2.y);
    }

    public static Vector2i operator * (Vector2i v1, Vector2i v2)
    {
        return new Vector2i(v1.x * v2.x, v1.y * v2.y);
    }

    public static Vector2i operator *(Vector2i v1, int v2)
    {
        return new Vector2i(v1.x * v2, v1.y * v2);
    }

    public static Vector2i operator / (Vector2i v1, Vector2i v2)
    {
        return new Vector2i(v1.x / v2.x, v1.y / v2.y);
    }

    public override bool Equals(object other)
    {
        // Check object other is a Vector3 object
        if (other is Vector2i)
        {
            // Convert object to Vector3
            Vector2i otherVector = (Vector2i)other;

            // Check for equality
            return otherVector.Equals(this);
        }
        return false;
    }

    public bool Equals(Vector2i other)
    {
        return
           this.x.Equals(other.x) &&
           this.y.Equals(other.y);
    }

	public override int GetHashCode() {
		return x ^ y;
	}

    public int Sum()
    {
        return x + y;
    }

    public override string ToString()
    {
        return "X: " + x + " Y: " + y;
    }

    public string ToServerString()
    {
        return x + "x" + y;
    }

    public static Vector2i FromString(string coor)
    {
        string[] result = coor.Split('x');

        return new Vector2i(Convert.ToInt32(result[0]), Convert.ToInt32(result[1]));
    }

    public static Vector2i FromVector3XZ(Vector3 pos)
    {
        return new Vector2i(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));
    }

    public static Vector2i FromDirectionVector3XZ(Vector3 pos)
    {
        Vector3 dir = Vector3.zero;

        if (pos != Vector3.zero)
        {
            float angle = Quaternion.LookRotation(pos, Vector3.forward).eulerAngles.z;

            if (angle < 45)
                angle = 0;
            else if (angle < 135)
                angle = 90;
            else if (angle < 225)
                angle = 180;
            else if (angle < 335)
                angle = 270;
            else
                angle = 0;

            dir = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward;
        }

        return new Vector2i(Mathf.RoundToInt(dir.x), Mathf.RoundToInt(dir.z));
    }

    public static Vector2i FromVector3XY(Vector3 pos)
    {
        return new Vector2i(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
    }

    public Vector3 ToVector3XY()
    {
        return new Vector3(this.x, this.y, 0);
    }

    public Vector3 ToVector3XZ()
    {
        return new Vector3(this.x, 0, this.y);
    }

    public Vector2 ToVector2()
    {
        return new Vector2(this.x, this.y);
    }

    public static Vector2i FromDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.Left:
                return left;
            case Direction.Right:
                return right;
            case Direction.Top:
                return up;
            case Direction.Bottom:
                return down;
        }
        return new Vector2i();
    }

    //
    // Summary:
    //     ///
    //     Shorthand for writing Vector2i(0, -1).
    //     ///
    public static Vector2i down
    {
        get { return new Vector2i(0, -1); }
    }

    //
    // Summary:
    //     ///
    //     Shorthand for writing Vector2i(-1, 0).
    //     ///
    public static Vector2i left
    {
        get { return new Vector2i(-1, 0); }
    }

    //
    // Summary:
    //     ///
    //     Shorthand for writing Vector2i(1, 1).
    //     ///
    public static Vector2i one
    {
        get { return new Vector2i(1, 1); }
    }

    //
    // Summary:
    //     ///
    //     Shorthand for writing Vector2i(1, 0).
    //     ///
    public static Vector2i right
    {
        get { return new Vector2i(1, 0); }
    }

    //
    // Summary:
    //     ///
    //     Shorthand for writing Vector2i(0, 1).
    //     ///
    public static Vector2i up
    {
        get { return new Vector2i(0, 1); }
    }

    //
    // Summary:
    //     ///
    //     Shorthand for writing Vector2i(0, 0).
    //     ///
    public static Vector2i zero
    {
        get { return new Vector2i(); }
    }
}
