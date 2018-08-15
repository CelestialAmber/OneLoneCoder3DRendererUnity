using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CustomRenderer
{
    [System.Serializable]
    public class vec3 {
        public float x;
        public float y;
        public float z;
        public float w;
        public vec3(float x, float y, float z){
            this.x = x;
            this.y = y;
            this.z = z;
            w = 1;
        }

        public static vec3 zero{
            get{
                return new vec3(0, 0, 0);
            }
        }
        public static vec3 operator * (vec3 lhs, float rhs){
            return new vec3(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs);
        }
        public static vec3 operator *(vec3 lhs, vec3 rhs)
        {
            return new vec3(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z);
        }
        public static vec3 operator /(vec3 lhs, float rhs)
        {
            return new vec3(lhs.x / rhs, lhs.y / rhs, lhs.z / rhs);
        }
        public static vec3 operator /(vec3 lhs, vec3 rhs)
        {
            return new vec3(lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z);
        }
        public static vec3 operator +(vec3 lhs, float rhs)
        {
            return new vec3(lhs.x + rhs, lhs.y + rhs, lhs.z + rhs);
        }
        public static vec3 operator +(vec3 lhs, vec3 rhs)
        {
            return new vec3(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
        }
        public static vec3 operator -(vec3 lhs, float rhs)
        {
            return new vec3(lhs.x - rhs, lhs.y - rhs, lhs.z - rhs);
        }
        public static vec3 operator -(vec3 lhs, vec3 rhs)
        {
            return new vec3(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
        }


    }
    public class RenderMath{

       public static vec3 Matrix_MultiplyVector(vec3 i, Matrix4x4 m)
        {
            vec3 o = vec3.zero;

            o.x = i.x * m.m00 + i.y * m.m10 + i.z * m.m20 + m.m30;
            o.y = i.x * m.m01  + i.y * m.m11 + i.z * m.m21 + m.m31;
            o.z = i.x * m.m02 + i.y * m.m12 + i.z * m.m22 + m.m32;
            o.w = i.x * m.m03 + i.y * m.m13 + i.z * m.m23 + m.m33;


            return o;
        }
        public static Matrix4x4 Matrix_MakeIdentity()
        {
            Matrix4x4 matrix = new Matrix4x4();
            matrix.m00 = 1.0f;
            matrix.m11 = 1.0f;
            matrix.m22 = 1.0f;
            matrix.m33 = 1.0f;
            return matrix;
        }
        public static Matrix4x4 Matrix_MakeRotation(vec3 rotation)
        {
            Matrix4x4 matrix = new Matrix4x4();
            matrix.m00 = Mathf.Cos(rotation.y) * Mathf.Cos(rotation.z);
            matrix.m01 = Mathf.Cos(rotation.y) * Mathf.Sin(rotation.z);
            matrix.m02 = -Mathf.Sin(rotation.y);
            matrix.m10 = Mathf.Sin(rotation.x) * Mathf.Sin(rotation.y) * Mathf.Cos(rotation.z) - Mathf.Cos(rotation.x) * Mathf.Sin(rotation.z);
            matrix.m11 = Mathf.Sin(rotation.x) * Mathf.Sin(rotation.y) * Mathf.Sin(rotation.z) + Mathf.Cos(rotation.x) * Mathf.Cos(rotation.z);
            matrix.m12 = Mathf.Sin(rotation.x) * Mathf.Cos(rotation.y);
            matrix.m20 = Mathf.Cos(rotation.x) * Mathf.Sin(rotation.y) * Mathf.Cos(rotation.z) + Mathf.Sin(rotation.x) * Mathf.Sin(rotation.z);
            matrix.m21 = Mathf.Cos(rotation.x) * Mathf.Sin(rotation.y) * Mathf.Sin(rotation.z) - Mathf.Sin(rotation.x) * Mathf.Cos(rotation.z);
            matrix.m22 = Mathf.Cos(rotation.x) * Mathf.Cos(rotation.y);
            matrix.m33 = 1;
            return matrix;

        }

        public static Matrix4x4 Matrix_MakeTranslation(vec3 position)
        {
            Matrix4x4 matrix = new Matrix4x4();
            matrix.m00 = 1.0f;
            matrix.m11 = 1.0f;
            matrix.m22 = 1.0f;
            matrix.m33 = 1.0f;
            matrix.m30 = position.x;
            matrix.m31 = position.y;
            matrix.m32 = position.z;
            return matrix;
        }

        public static Matrix4x4 Matrix_MakeProjection(float fFovDegrees, float fAspectRatio, float fNear, float fFar)
        {
            float fFovRad = 1.0f / Mathf.Tan(fFovDegrees * 0.5f / 180.0f * Mathf.PI);
            Matrix4x4 matrix = new Matrix4x4();
            matrix.m00 = fAspectRatio * fFovRad;
            matrix.m11 = fFovRad;
            matrix.m22 = fFar / (fFar - fNear);
            matrix.m32 = (-fFar * fNear) / (fFar - fNear);
            matrix.m23 = 1.0f;
            matrix.m33 = 0.0f;
            return matrix;
        }
        public static Matrix4x4 Matrix_MultiplyMatrix(Matrix4x4 m1, Matrix4x4 m2)
        {
            return m1 * m2;
        }

        public static Matrix4x4 Matrix_PointAt(vec3 pos, vec3 target, vec3 up)
        {
            // Calculate new forward direction
            vec3 newForward = target - pos;
            newForward = RenderMath.Vector_Normalize(newForward);

            // Calculate new Up direction
            vec3 a = newForward * RenderMath.Vector_DotProduct(up, newForward);
            vec3 newUp = up - a;
            newUp = RenderMath.Vector_Normalize(newUp);

            // New Right direction is easy, its just cross product
            vec3 newRight = Vector_CrossProduct(newUp, newForward);

            // Construct Dimensioning and Translation Matrix    
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetColumn(0,new Vector4(newRight.x, newUp.x, newForward.x, pos.x));
            matrix.SetColumn(1, new Vector4(newRight.y, newUp.y, newForward.y, pos.y));
            matrix.SetColumn(2, new Vector4(newRight.z, newUp.z, newForward.z, pos.z));
            matrix.SetColumn(3, new Vector4(0,0,0, 1.0f));
         
            return matrix;

        }
        public static Matrix4x4 Matrix_QuickInverse(Matrix4x4 m) // Only for Rotation/Translation Matrices
        {
            return m.inverse;
        }

        public static  vec3 Vector_Normalize(vec3 v)
        {
            float l = Vector_Length(v);
            return new vec3(v.x / l, v.y / l, v.z / l);
        }
        public static float Vector_DotProduct(vec3 v1, vec3 v2)
        {
            return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        }

        public static  vec3 Vector_CrossProduct(vec3 v1, vec3 v2)
        {
            vec3 v = vec3.zero;
            v.x = v1.y * v2.z - v1.z * v2.y;
            v.y = v1.z * v2.x - v1.x * v2.z;
            v.z = v1.x * v2.y - v1.y * v2.x;
            return v;
        }
        public static float Vector_Length(vec3 v){
            return Mathf.Sqrt(Vector_DotProduct(v, v));
        }




    }
    public class Rasterizer
    {

        public static void DrawTriangle(vec3 p1, vec3 p2, vec3 p3, Color c, Texture2D tex)
        {
            DrawLine((int)p1.x, (int)p1.y, (int)p2.x, (int)p2.y, c, tex);
            DrawLine((int)p2.x, (int)p2.y, (int)p3.x, (int)p3.y, c, tex);
            DrawLine((int)p3.x, (int)p3.y, (int)p1.x, (int)p1.y, c, tex);
        }
        public static void DrawLine(int x1, int y1, int x2, int y2, Color c, Texture2D tex)
        {
            int x, y, dx, dy, dx1, dy1, px, py, xe, ye, i;
            dx = x2 - x1; dy = y2 - y1;
            dx1 = Mathf.Abs(dx); dy1 = Mathf.Abs(dy);
            px = 2 * dy1 - dx1; py = 2 * dx1 - dy1;
            if (dy1 <= dx1)
            {
                if (dx >= 0)
                { x = x1; y = y1; xe = x2; }
                else
                { x = x2; y = y2; xe = x1; }

                Draw(x, y, c, tex);

                for (i = 0; x < xe; i++)
                {
                    x = x + 1;
                    if (px < 0)
                        px = px + 2 * dy1;
                    else
                    {
                        if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0)) y = y + 1; else y = y - 1;
                        px = px + 2 * (dy1 - dx1);
                    }
                    Draw(x, y, c, tex);
                }
            }
            else
            {
                if (dy >= 0)
                { x = x1; y = y1; ye = y2; }
                else
                { x = x2; y = y2; ye = y1; }

                Draw(x, y, c, tex);

                for (i = 0; y < ye; i++)
                {
                    y = y + 1;
                    if (py <= 0)
                        py = py + 2 * dx1;
                    else
                    {
                        if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0)) x = x + 1; else x = x - 1;
                        py = py + 2 * (dx1 - dy1);
                    }
                    Draw(x, y, c, tex);
                }
            }
        }
        public static void HorizontalDraw(int sx, int ex, int ny, Color c, Texture2D tex)
        {
            for (int i = sx; i <= ex; i++) Draw(i, ny, c, tex);

        }

        public static void FillTriangle(vec3 p1, vec3 p2, vec3 p3, Color c, Texture2D tex)
        {


            int x1 = (int)p1.x, y1 = (int)p1.y, x2 = (int)p2.x, y2 = (int)p2.y, x3 = (int)p3.x, y3 = (int)p3.y;
            int t1x, t2x, y, minx, maxx, t1xp, t2xp;
            bool changed1 = false;
            bool changed2 = false;
            int signx1, signx2, dx1, dy1, dx2, dy2;
            int e1, e2;
            int swap;
            // Sort vertices
            if (y1 > y2) { swap = y1; y1 = y2; y2 = swap; swap = x1; x1 = x2; x2 = swap; }
            if (y1 > y3) { swap = y1; y1 = y3; y3 = swap; swap = x1; x1 = x3; x3 = swap; }
            if (y2 > y3) { swap = y2; y2 = y3; y3 = swap; swap = x2; x2 = x3; x3 = swap; }

            t1x = t2x = x1; y = y1;   // Starting points
            dx1 = (int)(x2 - x1); if (dx1 < 0) { dx1 = -dx1; signx1 = -1; }
            else signx1 = 1;
            dy1 = (int)(y2 - y1);

            dx2 = (int)(x3 - x1); if (dx2 < 0) { dx2 = -dx2; signx2 = -1; }
            else signx2 = 1;
            dy2 = (int)(y3 - y1);

            if (dy1 > dx1)
            {   // swap values
                swap = dy1; dy1 = dx1; dx1 = swap;
                changed1 = true;
            }
            if (dy2 > dx2)
            {   // swap values
                swap = dy2; dy2 = dx2; dx2 = swap;
                changed2 = true;
            }

            e2 = (int)(dx2 >> 1);
            // Flat top, just process the second half
            if (y1 == y2) goto next;
            e1 = (int)(dx1 >> 1);

            for (int i = 0; i < dx1;)
            {
                t1xp = 0; t2xp = 0;
                if (t1x < t2x) { minx = t1x; maxx = t2x; }
                else { minx = t2x; maxx = t1x; }
                // process first line until y value is about to change
                while (i < dx1)
                {
                    i++;
                    e1 += dy1;
                    while (e1 >= dx1)
                    {
                        e1 -= dx1;
                        if (changed1) t1xp = signx1;//t1x += signx1;
                        else goto next1;
                    }
                    if (changed1) break;
                    else t1x += signx1;
                }
            // Move line
            next1:
                // process second line until y value is about to change
                while (true)
                {
                    e2 += dy2;
                    while (e2 >= dx2)
                    {
                        e2 -= dx2;
                        if (changed2) t2xp = signx2;//t2x += signx2;
                        else goto next2;
                    }
                    if (changed2) break;
                    else t2x += signx2;
                }
            next2:
                if (minx > t1x) minx = t1x; if (minx > t2x) minx = t2x;
                if (maxx < t1x) maxx = t1x; if (maxx < t2x) maxx = t2x;
                HorizontalDraw(minx, maxx, y, c,tex);    // Draw line from min to max points found on the y
                                                     // Now increase y
                if (!changed1) t1x += signx1;
                t1x += t1xp;
                if (!changed2) t2x += signx2;
                t2x += t2xp;
                y += 1;
                if (y == y2) break;

            }
        next:
            // Second half
            dx1 = (int)(x3 - x2); if (dx1 < 0) { dx1 = -dx1; signx1 = -1; }
            else signx1 = 1;
            dy1 = (int)(y3 - y2);
            t1x = x2;

            if (dy1 > dx1)
            {   // swap values
                swap = dy1; dy1 = dx1; dx1 = swap;
                changed1 = true;
            }
            else changed1 = false;

            e1 = (int)(dx1 >> 1);

            for (int i = 0; i <= dx1; i++)
            {
                t1xp = 0; t2xp = 0;
                if (t1x < t2x) { minx = t1x; maxx = t2x; }
                else { minx = t2x; maxx = t1x; }
                // process first line until y value is about to change
                while (i < dx1)
                {
                    e1 += dy1;
                    while (e1 >= dx1)
                    {
                        e1 -= dx1;
                        if (changed1) { t1xp = signx1; break; }//t1x += signx1;
                        else goto next3;
                    }
                    if (changed1) break;
                    else t1x += signx1;
                    if (i < dx1) i++;
                }
            next3:
                // process second line until y value is about to change
                while (t2x != x3)
                {
                    e2 += dy2;
                    while (e2 >= dx2)
                    {
                        e2 -= dx2;
                        if (changed2) t2xp = signx2;
                        else goto next4;
                    }
                    if (changed2) break;
                    else t2x += signx2;
                }
            next4:

                if (minx > t1x) minx = t1x; if (minx > t2x) minx = t2x;
                if (maxx < t1x) maxx = t1x; if (maxx < t2x) maxx = t2x;
                HorizontalDraw(minx, maxx, y, c, tex);
                if (!changed1) t1x += signx1;
                t1x += t1xp;
                if (!changed2) t2x += signx2;
                t2x += t2xp;
                y += 1;
                if (y > y3) return;
            }
        }


        public static void Draw(int x, int y, Color c, Texture2D tex)
        {
            tex.SetPixel(x, y, c);

        }
    }

}