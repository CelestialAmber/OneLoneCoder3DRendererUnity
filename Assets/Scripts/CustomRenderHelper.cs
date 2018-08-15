using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Mathematics.math;
using Unity.Mathematics;
namespace CustomRenderer
{
    
    public class RenderMath{

       public static float3 Matrix_MultiplyVector(float3 i, float4x4 m)
        {
            float3 o = float3(0);
            o.x = i.x * m.c0.x + i.y * m.c1.x + i.z * m.c2.x + m.c3.x;
            o.y = i.x * m.c0.y + i.y * m.c1.y + i.z * m.c2.y + m.c3.y;
            o.z = i.x * m.c0.z + i.y * m.c1.z + i.z * m.c2.z + m.c3.z;
            return o;
        }
        public static float4x4 Matrix_MakeIdentity()
        {
            float4x4 matrix = float4x4(0);
            matrix.c0.x = 1.0f;
            matrix.c1.y = 1.0f;
            matrix.c2.z = 1.0f;
            matrix.c3.w = 1.0f;
            return matrix;
        }
        public static float4x4 Matrix_MakeRotation(float3 rotation)
        {
            float4x4 matrix = float4x4(0);
            matrix.c0.x = cos(rotation.y) * cos(rotation.z);
            matrix.c0.y = cos(rotation.y) * sin(rotation.z);
            matrix.c0.z = -sin(rotation.y);
            matrix.c1.x = sin(rotation.x) * sin(rotation.y) * cos(rotation.z) - cos(rotation.x) * sin(rotation.z);
            matrix.c1.y = sin(rotation.x) * sin(rotation.y) * sin(rotation.z) + cos(rotation.x) * cos(rotation.z);
            matrix.c1.z = sin(rotation.x) * cos(rotation.y);
            matrix.c2.x = cos(rotation.x) * sin(rotation.y) * cos(rotation.z) + sin(rotation.x) * sin(rotation.z);
            matrix.c2.y = cos(rotation.x) * sin(rotation.y) * sin(rotation.z) - sin(rotation.x) * cos(rotation.z);
            matrix.c2.z = cos(rotation.x) * cos(rotation.y);
            matrix.c3.w = 1;
            return matrix;

        }

        public static float4x4 Matrix_MakeTranslation(float3 position)
        {
            float4x4 matrix = float4x4(0);
            matrix.c0.x = 1.0f;
            matrix.c1.y = 1.0f;
            matrix.c2.z = 1.0f;
            matrix.c3.w = 1.0f;
            matrix.c3.x = position.x;
            matrix.c3.y = position.y;
            matrix.c3.z = position.z;
            return matrix;
        }

        public static float4x4 Matrix_MakeProjection(float fFovDegrees, float fAspectRatio, float fNear, float fFar)
        {
            float fFovRad = 1.0f / tan(fFovDegrees * 0.5f / 180.0f * Mathf.PI);
            float4x4 matrix = float4x4(0);
            matrix.c0.x = fAspectRatio * fFovRad;
            matrix.c1.y = fFovRad;
            matrix.c2.z = fFar / (fFar - fNear);
            matrix.c3.z = (-fFar * fNear) / (fFar - fNear);
            matrix.c2.w = 1.0f;
            matrix.c3.w = 0.0f;
            return matrix;
        }
        public static float4x4 Matrix_MultiplyMatrix(float4x4 m1, float4x4 m2)
        {
            float4x4 matrix = float4x4(0);

            //Row 1;
            matrix.c0.x = m1.c0.x * m2.c0.x + m1.c0.y * m2.c1.x + m1.c0.z * m2.c2.x + m1.c0.w * m2.c3.x;
            matrix.c1.x = m1.c1.x * m2.c0.x + m1.c1.y * m2.c1.x + m1.c1.z * m2.c2.x + m1.c1.w * m2.c3.x;
            matrix.c2.x = m1.c2.x * m2.c0.x + m1.c2.y * m2.c1.x + m1.c2.z * m2.c2.x + m1.c2.w * m2.c3.x;
            matrix.c3.x = m1.c3.x * m2.c0.x + m1.c3.y * m2.c1.x + m1.c3.z * m2.c2.x + m1.c3.w * m2.c3.x;

            //Row 2;
            matrix.c0.y = m1.c0.x * m2.c0.y + m1.c0.y * m2.c1.y + m1.c0.z * m2.c2.y + m1.c0.w * m2.c3.y;
            matrix.c1.y = m1.c1.x * m2.c0.y + m1.c1.y * m2.c1.y + m1.c1.z * m2.c2.y + m1.c1.w * m2.c3.y;
            matrix.c2.y = m1.c2.x * m2.c0.y + m1.c2.y * m2.c1.y + m1.c2.z * m2.c2.y + m1.c2.w * m2.c3.y;
            matrix.c3.y = m1.c3.x * m2.c0.y + m1.c3.y * m2.c1.y + m1.c3.z * m2.c2.y + m1.c3.w * m2.c3.y;

            //Row 3;
            matrix.c0.z = m1.c0.x * m2.c0.z + m1.c0.y * m2.c1.z + m1.c0.z * m2.c2.z + m1.c0.w * m2.c3.z;
            matrix.c1.z = m1.c1.x * m2.c0.z + m1.c1.y * m2.c1.z + m1.c1.z * m2.c2.z + m1.c1.w * m2.c3.z;
            matrix.c2.z = m1.c2.x * m2.c0.z + m1.c2.y * m2.c1.z + m1.c2.z * m2.c2.z + m1.c2.w * m2.c3.z;
            matrix.c3.z = m1.c3.x * m2.c0.z + m1.c3.y * m2.c1.z + m1.c3.z * m2.c2.z + m1.c3.w * m2.c3.z;

            //Row 4;
            matrix.c0.w = m1.c0.x * m2.c0.w + m1.c0.y * m2.c1.w + m1.c0.z * m2.c2.w + m1.c0.w * m2.c3.w;
            matrix.c1.w = m1.c1.x * m2.c0.w + m1.c1.y * m2.c1.w + m1.c1.z * m2.c2.w + m1.c1.w * m2.c3.w;
            matrix.c2.w = m1.c2.x * m2.c0.w + m1.c2.y * m2.c1.w + m1.c2.z * m2.c2.w + m1.c2.w * m2.c3.w;
            matrix.c3.w = m1.c3.x * m2.c0.w + m1.c3.y * m2.c1.w + m1.c3.z * m2.c2.w + m1.c3.w * m2.c3.w;


            return matrix;
        }

        public static float4x4 Matrix_PointAt(float3 pos, float3 target, float3 up)
        {
            // Calculate new forward direction
            float3 newForward = target - pos;
            newForward = normalize(newForward);

            // Calculate new Up direction
            float3 a = newForward * dot(up, newForward);
            float3 newUp = up - a;
            newUp = normalize(newUp);

            // New Right direction is easy, its just cross product
            float3 newRight = cross(newUp, newForward);

            // Construct Dimensioning and Translation Matrix    
            float4x4 matrix = float4x4(0);
            matrix.c0.x = newRight.x; matrix.c0.y = newRight.y; matrix.c0.z = newRight.z; matrix.c0.w = 0.0f;
            matrix.c1.x = newUp.x; matrix.c1.y = newUp.y; matrix.c1.z = newUp.z; matrix.c1.w = 0.0f;
            matrix.c2.x = newForward.x; matrix.c2.y = newForward.y; matrix.c2.z = newForward.z; matrix.c2.w = 0.0f;
            matrix.c3.x = pos.x; matrix.c3.y = pos.y; matrix.c3.z = pos.z; matrix.c3.w = 1.0f;
            return matrix;

        }
        public static float4x4 Matrix_QuickInverse(float4x4 m) // Only for Rotation/Translation Matrices
        {
            float4x4 matrix = float4x4(0);
            matrix.c0.x = m.c0.x; matrix.c0.y = m.c1.x; matrix.c0.z = m.c2.x; matrix.c0.w = 0.0f;
            matrix.c1.x = m.c0.y; matrix.c1.y = m.c1.y; matrix.c1.z = m.c2.y; matrix.c1.w = 0.0f;
            matrix.c2.x = m.c0.z; matrix.c2.y = m.c1.z; matrix.c2.z = m.c2.z; matrix.c2.w = 0.0f;
            matrix.c3.x = -(m.c3.x * matrix.c0.x + m.c3.y * matrix.c1.x + m.c3.z * matrix.c2.x);
            matrix.c3.y = -(m.c3.x * matrix.c0.y + m.c3.y * matrix.c1.y + m.c3.z * matrix.c2.y);
            matrix.c3.z = -(m.c3.x * matrix.c0.z + m.c3.y * matrix.c1.z + m.c3.z * matrix.c2.z);
            matrix.c3.w = 1.0f;
            return matrix;
        }

        public static  float3 Vector_Normalize(float3 v)
        {
            float l = length(v);
            return float3(v.x / l, v.y / l, v.z / l);
        }

        public static  float3 Vector_CrossProduct(float3 v1, float3 v2)
        {
            float3 v;
            v.x = v1.y * v2.z - v1.z * v2.y;
            v.y = v1.z * v2.x - v1.x * v2.z;
            v.z = v1.x * v2.y - v1.y * v2.x;
            return v;
        }




    }
    public class Rasterizer
    {

        public static void DrawTriangle(float3 p1, float3 p2, float3 p3, Color c, Texture2D tex)
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

        public static void FillTriangle(float3 p1, float3 p2, float3 p3, Color c, Texture2D tex)
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